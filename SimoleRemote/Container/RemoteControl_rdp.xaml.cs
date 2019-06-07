using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using AxMSTSCLib;
using MSTSCLib;
using SimpleRemote.Core;
using SimpleRemote.Native;
using SimpleRemote.Modes;

namespace SimpleRemote.Container
{
    /// <summary>
    /// RemoteControl_Rdp.xaml 的交互逻辑
    /// </summary>
    public partial class RemoteControl_rdp : BaseRemoteControl
    {
        private AxMsRdpClient7NotSafeForScripting MsRdpClient7;
        private AxMsRdpClient9NotSafeForScripting MsRdpClient9;
        private IMsRdpClientNonScriptable5 MsRdpClientOcx;

        private IntPtr _oldRdpClientWinProc;
        private User32.WindowFunc _winRdpClientProc;
        private IntPtr _oldInRdpClientWinProc;
        private User32.WindowFunc _winInRdpClientProc;
        private IntPtr _iHWindowHwnd;
        private IntPtr _topWindowHwnd;

        public RemoteControl_rdp(ContentControl contentControl)
            : base(contentControl)
        {
            InitializeComponent();
            if (CommonServices.OSVersion <= 6.1f)
                MsRdpClient7 = new AxMsRdpClient7NotSafeForScripting();
            else
                MsRdpClient9 = new AxMsRdpClient9NotSafeForScripting();
            //win7 及以下版本
            if (MsRdpClient7 != null)
            {
                MsRdpClient7.BeginInit();
                MsRdpClient7.Dock = System.Windows.Forms.DockStyle.Fill;
                FormsHost.Child = MsRdpClient7;
                MsRdpClient7.EndInit();

                MsRdpClient7.OnConnected += MyRdp_OnConnected;
                MsRdpClient7.OnFatalError += MyRdp_OnFatalError;
                MsRdpClient7.OnLogonError += MyRdp_OnLogonError;
                MsRdpClient7.OnDisconnected += MyRdp_OnDisconnected;
                MsRdpClient7.OnRequestGoFullScreen += MyRdp_OnRequestGoFullScreen;
                MsRdpClient7.OnRequestLeaveFullScreen += MyRdp_OnRequestLeaveFullScreen;
            }
            else
            {
                MsRdpClient9.BeginInit();
                MsRdpClient9.Dock = System.Windows.Forms.DockStyle.Fill;
                FormsHost.Child = MsRdpClient9;
                MsRdpClient9.EndInit();

                MsRdpClient9.OnConnected += MyRdp_OnConnected;
                MsRdpClient9.OnFatalError += MyRdp_OnFatalError;
                MsRdpClient9.OnLogonError += MyRdp_OnLogonError;
                MsRdpClient9.OnDisconnected += MyRdp_OnDisconnected;
                MsRdpClient9.OnRequestGoFullScreen += MyRdp_OnRequestGoFullScreen;
                MsRdpClient9.OnRequestLeaveFullScreen += MyRdp_OnRequestLeaveFullScreen;
            }

            _winRdpClientProc = WinRdpClientProc;
            _winInRdpClientProc = WinInRdpClientProc;
        }

        public override void Connect(DbItemRemoteLink linkSettings, DbItemSetting lastSetting)
        {
            DbItemSettingRdp lastSettingRdp = lastSetting as DbItemSettingRdp;
            if (lastSettingRdp == null) return;
            
            //分离服务器地址和端口
            string[] addr = linkSettings.Server.Split(':');
            int port = 3389;
            if (addr.Length > 1) int.TryParse(addr[1], out port);
            if (port <= 0) port = 3389;

            //初始化远程连接属性
            if (MsRdpClient7 != null) //win7及以下版本
            {
                MsRdpClient7.Server = addr[0];
                MsRdpClient7.UserName = linkSettings.UserName;
                MsRdpClient7.AdvancedSettings2.ClearTextPassword = linkSettings.Password;
                MsRdpClient7.AdvancedSettings2.RDPPort = port;
                MsRdpClientOcx = (IMsRdpClientNonScriptable5)MsRdpClient7.GetOcx();
                MsRdpClientOcx.PromptForCredentials = false;//凭据提示对话框
                MsRdpClientOcx.AllowPromptingForCredentials = true;//显示密码输入框
                MsRdpClientOcx.DisableConnectionBar = true;//禁用连接栏

                //将窗口句柄设置或检索为控件显示的任何对话框的父窗口
                var parentHwnd = CommonServices.HWNDtoRemotableHandle(new WindowInteropHelper(Window.GetWindow(this)).Handle);
                MsRdpClientOcx.set_UIParentWindowHandle(ref parentHwnd);

                MsRdpClient7.AdvancedSettings.BitmapPeristence = 1;//启用位图缓存
                MsRdpClient7.AdvancedSettings.Compress = 1;//启用压缩
                MsRdpClient7.AdvancedSettings.ContainerHandledFullScreen = 1;//启用容器处理的全屏模式。
                MsRdpClient7.AdvancedSettings2.BitmapPersistence = 1;//持久位图缓存
                MsRdpClient7.AdvancedSettings2.CachePersistenceActive = 1;//持久位图缓存
                MsRdpClient7.AdvancedSettings2.GrabFocusOnConnect = false;//连接的时候获取焦点
                MsRdpClient7.AdvancedSettings7.EnableCredSspSupport = true;//指定是否为此连接启用凭据安全服务提供程序

                //分辨率
                if (lastSettingRdp.SizeIndex == DbItemSetting.DESKSIZE_AUTO)//自适应分辨率
                {
                    Window windows = Window.GetWindow(this);
                    MsRdpClient7.DesktopWidth = (int)windows.Width - 4;
                    MsRdpClient7.DesktopHeight = (int)windows.Height - 34;
                }
                else 
                {
                    var size = lastSettingRdp.GetDeskTopSize();
                    MsRdpClient7.DesktopWidth = (int)size.Width;
                    MsRdpClient7.DesktopHeight = (int)size.Height;
                }

                //性能选项
                if (lastSettingRdp.Performance != DbItemSettingRdp.CONNECTION_TYPE_AUTO)
                    MsRdpClient7.AdvancedSettings8.NetworkConnectionType = (uint)lastSettingRdp.Performance;
                //颜色深度
                switch (lastSettingRdp.ColorDepthMode)
                {
                    case DbItemSettingRdp.COLOR_15BPP: MsRdpClient7.ColorDepth = 15; break;
                    case DbItemSettingRdp.COLOR_16BPP: MsRdpClient7.ColorDepth = 16; break;
                    case DbItemSettingRdp.COLOR_24BPP: MsRdpClient7.ColorDepth = 24; break;
                    case DbItemSettingRdp.COLOR_32BPP: MsRdpClient7.ColorDepth = 32; break;
                    default: MsRdpClient7.ColorDepth = 32; break;
                }
                //音频
                MsRdpClient7.AdvancedSettings6.AudioRedirectionMode = (uint)lastSettingRdp.AudioRedirectionMode - 1;
                //组合键
                MsRdpClient7.SecuredSettings2.KeyboardHookMode = lastSettingRdp.KeyboardHookMode - 1;
                //本地资源
                MsRdpClient7.AdvancedSettings2.RedirectPrinters = lastSettingRdp.RedirectionPrintf.Value ? true : false;//打印机
                MsRdpClient7.AdvancedSettings6.RedirectClipboard = lastSettingRdp.RedirectionClipboard.Value ? true : false;//剪贴板重定向
                MsRdpClient7.AdvancedSettings3.RedirectSmartCards = lastSettingRdp.RedirectionsMartcard.Value ? true : false;//智能卡重定向
                MsRdpClient7.AdvancedSettings3.RedirectPorts = lastSettingRdp.RedirectionsPort.Value ? true : false;//端口重定向
                MsRdpClient7.AdvancedSettings3.RedirectDrives = lastSettingRdp.RedirectionsDriver.Value ? true : false;//驱动器重定向
                                                                                                                    

                MsRdpClient7.AdvancedSettings4.ConnectionBarShowMinimizeButton = false;//显示全部工具栏上的最小化按钮
                MsRdpClient7.AdvancedSettings7.ConnectToAdministerServer = false;

                MsRdpClient7.Connect();
                User32.EnumChildWindows(MsRdpClient7.Handle, EnumWindowsProc, IntPtr.Zero);
            }
            else //win8 及以上版本
            {
                MsRdpClient9.Server = addr[0];
                MsRdpClient9.UserName = linkSettings.UserName;
                MsRdpClient9.AdvancedSettings2.ClearTextPassword = linkSettings.Password;
                MsRdpClient9.AdvancedSettings2.RDPPort = port;
                MsRdpClientOcx = (IMsRdpClientNonScriptable5)MsRdpClient9.GetOcx();
                MsRdpClientOcx.PromptForCredentials = false;//凭据提示对话框
                MsRdpClientOcx.AllowPromptingForCredentials = true;//显示密码输入框
                MsRdpClientOcx.DisableConnectionBar = true;//禁用连接栏

                //将窗口句柄设置或检索为控件显示的任何对话框的父窗口
                var parentHwnd = CommonServices.HWNDtoRemotableHandle(new WindowInteropHelper(Window.GetWindow(this)).Handle);
                MsRdpClientOcx.set_UIParentWindowHandle(ref parentHwnd);

                MsRdpClient9.AdvancedSettings.BitmapPeristence = 1;//启用位图缓存
                MsRdpClient9.AdvancedSettings.Compress = 1;//启用压缩
                MsRdpClient9.AdvancedSettings.ContainerHandledFullScreen = 1;//启用容器处理的全屏模式。
                MsRdpClient9.AdvancedSettings2.BitmapPersistence = 1;//持久位图缓存
                MsRdpClient9.AdvancedSettings2.CachePersistenceActive = 1;//持久位图缓存
                MsRdpClient9.AdvancedSettings2.GrabFocusOnConnect = false;//连接的时候获取焦点
                MsRdpClient9.AdvancedSettings7.EnableCredSspSupport = true;//指定是否为此连接启用凭据安全服务提供程序

                //分辨率
                if (lastSettingRdp.SizeIndex == DbItemSetting.DESKSIZE_AUTO)//自适应分辨率
                {
                    Window windows = Window.GetWindow(this);
                    MsRdpClient9.DesktopWidth = (int)windows.Width - 4;
                    MsRdpClient9.DesktopHeight = (int)windows.Height - 34;
                }
                else
                {
                    var size = lastSettingRdp.GetDeskTopSize();
                    MsRdpClient9.DesktopWidth = (int)size.Width;
                    MsRdpClient9.DesktopHeight = (int)size.Height;
                }
                //性能选项
                if (lastSettingRdp.Performance == DbItemSettingRdp.CONNECTION_TYPE_AUTO) MsRdpClient9.AdvancedSettings9.BandwidthDetection = true;//自动检查带宽
                else MsRdpClient9.AdvancedSettings8.NetworkConnectionType = (uint)lastSettingRdp.Performance;
                //颜色深度
                switch (lastSettingRdp.ColorDepthMode)
                {
                    case DbItemSettingRdp.COLOR_15BPP: MsRdpClient9.ColorDepth = 15; break;
                    case DbItemSettingRdp.COLOR_16BPP: MsRdpClient9.ColorDepth = 16; break;
                    case DbItemSettingRdp.COLOR_24BPP: MsRdpClient9.ColorDepth = 24; break;
                    case DbItemSettingRdp.COLOR_32BPP: MsRdpClient9.ColorDepth = 32; break;
                    default: MsRdpClient9.ColorDepth = 32; break;
                }
                //音频
                MsRdpClient9.AdvancedSettings6.AudioRedirectionMode = (uint)lastSettingRdp.AudioRedirectionMode - 1;
                //录音设备
                MsRdpClient9.AdvancedSettings8.AudioCaptureRedirectionMode = lastSettingRdp.AudioCaptureRedirectionMode == DbItemSettingRdp.AAUDIOCAPTURE_TRUE;
                //组合键
                MsRdpClient9.SecuredSettings2.KeyboardHookMode = lastSettingRdp.KeyboardHookMode - 1;
                //重定向
                MsRdpClient9.AdvancedSettings2.RedirectPrinters = lastSettingRdp.RedirectionPrintf.Value ? true : false;//打印机
                MsRdpClient9.AdvancedSettings6.RedirectClipboard = lastSettingRdp.RedirectionClipboard.Value ? true : false;//剪贴板重定向
                MsRdpClient9.AdvancedSettings3.RedirectSmartCards = lastSettingRdp.RedirectionsMartcard.Value ? true : false;//智能卡重定向
                MsRdpClient9.AdvancedSettings3.RedirectPorts = lastSettingRdp.RedirectionsPort.Value ? true : false;//端口重定向
                MsRdpClient9.AdvancedSettings3.RedirectDrives = lastSettingRdp.RedirectionsDriver.Value ? true : false;//驱动器重定向
                MsRdpClient9.AdvancedSettings4.ConnectionBarShowMinimizeButton = false;//显示全部工具栏上的最小化按钮

                MsRdpClient9.Connect();
                User32.EnumChildWindows(MsRdpClient9.Handle, EnumWindowsProc, IntPtr.Zero);
            }
        }

        public override void GoFullScreen(bool state)
        {
            //将窗口句柄设置或检索为控件显示的任何对话框的父窗口
            var parentHwnd = CommonServices.HWNDtoRemotableHandle(new WindowInteropHelper(Window.GetWindow(this)).Handle);
            MsRdpClientOcx.set_UIParentWindowHandle(ref parentHwnd);

            if (MsRdpClient7 != null)
                MsRdpClient7.FullScreen = state;
            else
                MsRdpClient9.FullScreen = state;
        }

        public override void Release()
        {
            FormsHost.Dispose();
        }
        /// <summary>
        /// 远程桌面进入全屏
        /// </summary>
        private void MyRdp_OnRequestGoFullScreen(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 远程桌面退出全屏
        /// </summary>
        private void MyRdp_OnRequestLeaveFullScreen(object sender, EventArgs e)
        {
            FullScreen(false);
        }
        /// <summary>
        /// 远程桌面连接成功
        /// </summary>\
        private void MyRdp_OnConnected(object sender, EventArgs e)
        {
            OnConnected?.Invoke();
            User32.SetFocus(_iHWindowHwnd);
        }
        /// <summary>
        /// 远程桌面断开连接
        /// </summary>
        private void MyRdp_OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            if (e.discReason == 0x001)
            {
                MyRdp_OnRequestLeaveFullScreen(null, null);
                Closed?.Invoke();
                return;
            }
            string discReason = $"未知错误，错误代码{e.discReason}。";
            if (e.discReason == 0x904) discReason = $"网络连接被关闭，错误代码{e.discReason}。";
            else if (e.discReason == 0x3) discReason = $"远程桌面服务会话已结束。\n\n另一用户已连接到此远程计算机，因此你的连接已丢失，请尝试再次连接。";
            else if (e.discReason == 0x108) discReason = $"连接超时，错误代码{e.discReason}。";
            else if (e.discReason == 0x104) discReason = $"DNS名称查找失败，错误代码{e.discReason}。";
            else if (e.discReason == 0x508) discReason = $"DNS查找失败，错误代码{e.discReason}。";
            else if (e.discReason == 0x208) discReason = $"主机未找到错误，错误代码{e.discReason}。";
            else if (e.discReason == 0x408) discReason = $"内部错误，错误代码{e.discReason}。";
            else if (e.discReason == 0x906 || e.discReason == 0xA06) discReason = $"内部安全错误，错误代码{e.discReason}。";
            else if (e.discReason == 0x804) discReason = $"指定了错误的IP地址，错误代码{e.discReason}。";
            else if (e.discReason == 0x308) discReason = $"指定的IP地址无效，错误代码{e.discReason}。";
            else if (e.discReason == 0x106 || e.discReason == 0x206 || e.discReason == 0x306) discReason = $"内存不足，错误代码{e.discReason}。";
            else if (e.discReason == 0x2) discReason = $"用户远程断开连接，错误代码{e.discReason}。";
            else if (e.discReason == 0x204) discReason = $"网络连接失败，错误代码{e.discReason}。";
            OnFatalError?.Invoke("断开连接", discReason);
        }
        /// <summary>
        /// 在发生登录错误或其他登录事件时调用
        /// </summary>
        private void MyRdp_OnLogonError(object sender, IMsTscAxEvents_OnLogonErrorEvent e)
        {
            string discReason = $"未知错误，错误代码{e.lError}。"; ;
            if (e.lError == (int)LogonError.ACCESSDENIED) discReason = "用户拒绝访问。";
            if (e.lError == (int)LogonError.ERRORUSERPASSWRD) discReason = "用户名和密码不匹配。";
            if (e.lError == (int)LogonError.LOCKUSER) discReason = "用户被锁定。";
            if (e.lError == (int)LogonError.PASSWORDEXPIRED) discReason = "密码已过期。";
            if (e.lError == (int)LogonError.OTHER) discReason = $"其他错误：错误代码{e.lError}。";
            OnFatalError?.Invoke("登入错误", discReason);
        }
        /// <summary>
        /// rdp控件遇到致命错误
        /// </summary>
        private void MyRdp_OnFatalError(object sender, IMsTscAxEvents_OnFatalErrorEvent e)
        {
            string errorText = $"未知错误，错误代码{e.errorCode}。";
            if (e.errorCode == 0) errorText = "出现未知错误。";
            if (e.errorCode == 1) errorText = "内部错误代码1。";
            if (e.errorCode == 2) errorText = "发生了内存不足错误。";
            if (e.errorCode == 3) errorText = "发生了窗口创建错误。";
            if (e.errorCode == 4) errorText = "内部错误代码2。";
            if (e.errorCode == 5) errorText = "内部错误代码3.这不是有效状态。";
            if (e.errorCode == 6) errorText = "内部错误代码4。";
            if (e.errorCode == 7) errorText = "客户端连接期间发生了不可恢复的错误。";
            if (e.errorCode == 100) errorText = "Winsock初始化错误。";
            OnFatalError?.Invoke("错误", errorText);
        }

        /// <summary>枚举子窗口</summary>
        private bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam)
        {
            if (_topWindowHwnd == IntPtr.Zero)
            {
                _topWindowHwnd = hwnd;
                _oldRdpClientWinProc = User32.SetWindowLongPtr(hwnd, -4, _winRdpClientProc);
            }
            string className = new string((char)0, 255);
            User32.GetClassNameW(hwnd, className, 255);

            //负责处理鼠标键盘输入的窗口
            if (string.Compare(className, "IHWindowClass", true) == 0)
            {
                _iHWindowHwnd = hwnd;
                _oldInRdpClientWinProc = User32.SetWindowLongPtr(hwnd, -4, _winInRdpClientProc);
                return false;
            }
            return true;
        }

        /// <summary>处理Rdp控件的一些消息</summary>
        private int WinRdpClientProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == 15)
            {
                User32.CallWindowProc(_oldRdpClientWinProc, hwnd, msg, wParam, lParam);
                Graphics grap = Graphics.FromHwnd(hwnd);
                grap.Clear(Color.White);
                return 0;
            }
            if (msg == 20)
            {
                Graphics grap = Graphics.FromHwnd(hwnd);
                grap.Clear(Color.White);
                return 0;
            }
            if (msg == 133)
            {
                User32.CallWindowProc(_oldRdpClientWinProc, hwnd, msg, wParam, lParam);
                return 0;
            }
            if (msg == 512)//WM_MOUSEMOVE
            {
                int num = lParam.ToInt32();
                MouseMoveProc?.Invoke(CommonServices.LOWORD(num), CommonServices.HIWORD(num));
            }
            return User32.CallWindowProc(_oldRdpClientWinProc, hwnd, msg, wParam, lParam);
        }
        /// <summary>键盘鼠标输入框窗口的消息处理</summary>
        private int WinInRdpClientProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == 512)//WM_MOUSEMOVE
            {
                int num = lParam.ToInt32();
                User32.GetWindowRect(hwnd, out var chileRect);
                User32.GetWindowRect(_topWindowHwnd, out var parentRect);
                int x = chileRect.Left - parentRect.Left;
                int y = chileRect.Top - parentRect.Top;
                MouseMoveProc?.Invoke(CommonServices.LOWORD(num) + x, CommonServices.HIWORD(num) + y);
            }
            if (msg == 513)//WM_LBUTTONDOWN
            {
                User32.SetFocus(hwnd);
            }
            return User32.CallWindowProc(_oldInRdpClientWinProc, hwnd, msg, wParam, lParam);
        }

    }
}

