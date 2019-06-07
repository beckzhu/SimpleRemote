using SimpleRemote.Core;
using SimpleRemote.Modes;
using SimpleRemote.Native;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SimpleRemote.Container
{
    /// <summary>
    /// RemoteControl_SSH.xaml 的交互逻辑
    /// </summary>
    public partial class RemoteControl_ssh : BaseRemoteControl
    {
        private const int ERROR_NOTIC = 0;
        private const int ERROR_FATAL = 1;

        private Putty _putty;
        private string _privateKey;

        private Putty.ErrorEvent _event_error;
        private Putty.SecurityAlertEvent _event_securityAlert;
        private Putty.ConnectedEvent _event_connected;
        private Putty.VerifyHostKeyEvent _event_verifyhostkey;
        private Putty.StoreHostKeyEvent _event_storehostkey;
        private Putty.KeyDownEvent _event_keydown;
        private Putty.MouseMoveEvent _event_mousemove;
        private Putty.DebugEvent _event_debug;

        public RemoteControl_ssh(ContentControl contentControl)
            : base(contentControl)
        {
            InitializeComponent();

            try
            {
                _putty = new Putty();
                _event_error = Error_Event;
                _event_securityAlert = SecurityAlert_Event;
                _event_connected = Connected_Event;
                _event_verifyhostkey = VerifyHostKey_Event;
                _event_storehostkey = StoreHostKey_Event;
                _event_keydown = KeyDown_Event;
                _event_mousemove = MouseMove_Event;
                _event_debug = Debug_Event;

                _putty.SetCallback(_event_error, _event_connected, _event_securityAlert, _event_verifyhostkey,
                                   _event_storehostkey, _event_keydown, _event_mousemove, _event_debug);
                if (!_putty.Init())
                {
                    string errorText = Marshal.PtrToStringAnsi(_putty.GetError());
                    throw new Exception(errorText);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"加载putty失败,{e.Message}。");
            }
        }

        private void Debug_Event(IntPtr text)
        {
            Debug.WriteLine(Marshal.PtrToStringAnsi(text));
        }

        public override void Connect(DbItemRemoteLink linkSettings, DbItemSetting lastSetting)
        {
            DbItemSettingSsh lastSettingSsh = lastSetting as DbItemSettingSsh;
            if (lastSettingSsh == null) return;

            string[] values = linkSettings.Server.Split(':');
            if (values.Length > 2) throw new Exception($"服务器地址不正确。");//地址不正确
            string server = values[0];
            string port = values.Length == 2 ? $"-P {values[1]}" : "-P 22";

            IntPtr parentHwnd = FormsControl.Handle;
            Window windows = Window.GetWindow(this);
            int width = (int)windows.Width - 4;
            int height = (int)windows.Height - 34;
            Thread thread = new Thread(() =>
            {
                Putty.Settings settings = new Putty.Settings
                {
                    fontname = lastSettingSsh.FontName,
                    fontsize = lastSettingSsh.FontSize,
                    curtype = lastSettingSsh.Cursor - 1,
                    linecodepage = Encoding.GetEncoding(lastSettingSsh.Character).BodyName,
                    backspaceisdelete = lastSettingSsh.Fallbackkeys - 1,
                    mouseisxterm = lastSettingSsh.MouseAction - 1,
                    puttycolor = new Putty.PuttyColor(lastSettingSsh.GetPuttyColor()),
                    rxvthomeend = lastSettingSsh.HomeAndEnd - 1,
                    functionkeys = lastSettingSsh.FnAndKeypad - 1,
                    cjkambigwide = lastSettingSsh.CJKAmbigWide.Value,
                    capslockcyr = lastSettingSsh.CapsLockCyr.Value,
                    crimplieslf = lastSettingSsh.CRImpliesLF.Value,
                    lfimpliescr = lastSettingSsh.LFImpliesCR.Value,
                };

                string user = string.IsNullOrEmpty(linkSettings.UserName) ? "" : $"-l {linkSettings.UserName}";
                string pw = null;
                if (string.IsNullOrWhiteSpace(linkSettings.PrivateKey))
                {
                    pw = string.IsNullOrEmpty(linkSettings.Password) ? "" : $"-pw {linkSettings.Password}";
                }
                else
                {
                    _privateKey = Path.GetTempPath() + linkSettings.Id;
                    using (StreamWriter streamWriter = new StreamWriter(_privateKey, false, Encoding.ASCII))
                    {
                        streamWriter.Write(linkSettings.PrivateKey);
                        streamWriter.Flush();
                    }
                    pw = $"-i \"{_privateKey}\"";
                }

                if (!_putty.Create(parentHwnd, $"-ssh {port} {user} {pw} {server}", 0, 0, width, height, settings))
                {
                    string errorText = Marshal.PtrToStringAnsi(_putty.GetError());
                    Dispatcher.Invoke(() => { OnFatalError?.Invoke("错误", errorText); });
                }

                _putty.Dispose();
                _putty = null;
                if (_privateKey != null) File.Delete(_privateKey);
                GC.Collect();
                GC.SuppressFinalize(this);
            });
            thread.Start();
        }

        public override void Release()
        {
            _putty?.Exit();
        }

        /// <summary>
        /// 连接成功
        /// </summary>
        public void Connected_Event()
        {
            Dispatcher.Invoke(() =>
            {
                OnConnected.Invoke();
            });
            _putty.Show();
        }

        private void Error_Event(int level, IntPtr title, IntPtr text, int lParam)
        {
            Dispatcher.Invoke(() =>
            {
                if (level == ERROR_FATAL) OnFatalError?.Invoke(Marshal.PtrToStringAnsi(title), Marshal.PtrToStringAnsi(text));
                if (level == ERROR_NOTIC) OnNonfatal?.Invoke(Marshal.PtrToStringAnsi(title), Marshal.PtrToStringAnsi(text));
            });
        }

        /// <summary>
        /// 弹出安全提示
        /// </summary>
        private int SecurityAlert_Event(IntPtr text)
        {
            int ret = -1;
            MessageDialog.ButtnClick buttnClick = (type) =>
            {
                ret = type;
                if (ret == MessageDialog.IDCANCEL)
                {
                    Dispatcher.Invoke(() => { Closed?.Invoke(); });
                }
            };

            Dispatcher.Invoke(() =>
            {
                MessageDialog.Show(ParentControl, "安全警告", Marshal.PtrToStringAnsi(text), MessageDialog.MB_YESNOCANCEL,
                                   buttnClick, MessageDialog.IDCANCEL, 60);
            });

            while (true)
            {
                Thread.Sleep(15);
                if (ret >= 0) break;
            }

            return ret;
        }

        /// <summary>
        /// 读取主机key
        /// </summary>
        private int VerifyHostKey_Event(string key, IntPtr otherstr, ref int readlen)
        {
            byte[] value = DatabaseServices.GetSshHostKey(key);
            if (value == null) return 2;
            if (readlen < value.Length) return 1;

            readlen = value.Length;
            Marshal.Copy(value, 0, otherstr, readlen);
            return 0;
        }

        /// <summary>
        /// 写入主机key
        /// </summary>
        private void StoreHostKey_Event(string key, IntPtr otherstr, int readlen)
        {
            byte[] value = new byte[readlen];
            Marshal.Copy(otherstr, value, 0, readlen);
            DatabaseServices.SetSshHostKey(key, value);
        }

        /// <summary>
        /// 键盘按下事件
        /// </summary>
        private void KeyDown_Event(int wParam, int lParam)
        {
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        private void MouseMove_Event(int wParam, int lParam)
        {
            MouseMoveProc?.Invoke(wParam, lParam);
        }

        private void FormsControl_SizeChanged(object sender, EventArgs e)
        {
            if (FormsControl.Width > 0 && FormsControl.Height > 0)
                _putty?.Move.Invoke(0, 0, FormsControl.Width, FormsControl.Height);
        }
    }
}