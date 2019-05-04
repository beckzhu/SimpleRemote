using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using AxMSTSCLib;
using MSTSCLib;
using SimpleRemote.Bll;
using SimpleRemote.Modes;
using System.Windows.Interop;
using SimpleRemote.Control;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Threading;
using Dragablz;

namespace SimpleRemote.Container
{
    /// <summary>
    /// RdpControl.xaml 的交互逻辑
    /// </summary>
    public partial class RemoteTabControl : UserControl,IRemoteControl
    {
        private FullScreenWindow _fullScreenWindow;
        private BaseRemoteControl _remoteControl;
        private RemoteTabItem _tabItem;
        private bool _startFullScreen;
        private bool _isConntec;
        private bool _fullScreen;

        public event EventHandler OnRemove;

        public RemoteTabControl()
        {
            InitializeComponent();
            _fullScreenWindow = new FullScreenWindow();
            _fullScreenWindow.Closed += (sender, e) =>
            {
                _fullScreen = false;
                OnFatalError("错误", "远程连接被关闭。");
            };
        }

        public void Remove()
        {
            MainWindow.RemoveTabItem(_tabItem);
        }

        public void Jump()
        {
            _tabItem.IsSelected = true;
        }

        public void Open(DbItemRemoteLink linkSettings, DbItemSetting itemSetting, bool jump)
        {
            _tabItem = MainWindow.AddTabItem(linkSettings.Name, this, jump);
            if (linkSettings.Password == null) linkSettings.Password = "";
            _tabItem.Closed = _tabItem_Closed;

            try
            {
                FinalItemSetting finalItemSetting = null;
                if (linkSettings.Type == (int)RemoteType.rdp)
                {
                    _remoteControl = new RemoteControl_rdp(this);
                    finalItemSetting = new FinalItemSetting_rdp((DbItemSetting_rdp)itemSetting);
                }
                if (linkSettings.Type == (int)RemoteType.ssh)
                {
                    _remoteControl = new RemoteControl_ssh(this);
                    finalItemSetting = new FinalItemSetting_ssh((DbItemSetting_ssh)itemSetting);
                }
                if (linkSettings.Type == (int)RemoteType.telnet)
                {
                    _remoteControl = new RemoteControl_telnet(this);
                    finalItemSetting = new FinalItemSetting_telnet((DbItemSetting_telnet)itemSetting);
                }
                if (_remoteControl == null)
                {
                    throw new Exception("可能是不支持的远程连接类型。");
                }
                _remoteControl.Visibility = Visibility.Collapsed;
                Panel_Animation.Visibility = Visibility.Visible;
                Grid.Children.Add(_remoteControl);

                _remoteControl.OnConnected = _remoteControl_OnConnected;
                _remoteControl.OnFatalError = OnFatalError;
                _remoteControl.OnNonfatal = OnNonfatal;
                _remoteControl.Closed = Remove;
                _remoteControl.FullScreen = FullScreen;

                _remoteControl.Connect(linkSettings, finalItemSetting);
                _startFullScreen = finalItemSetting.FullScreen;
            }
            catch (Exception e)
            {
                OnFatalError("错误", $"远程桌面连接失败。\n原因：{e.Message}");
            }
        }

        private void _tabItem_Closed(object sender)
        {
            _remoteControl?.Release();
            _fullScreenWindow?.Close();
            OnRemove?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 全屏,成功返回true 失败返回false
        /// </summary>
        public bool FullScreen(bool state)
        {
            if (!_isConntec) return false;
            if (_fullScreen == state) return true;

            _fullScreen = state;
            if (state)
            {
                Grid.Children.Remove(_remoteControl);
                _fullScreenWindow.Content = _remoteControl;
                _fullScreenWindow.Show();
                Button_ExitFull.Visibility = Visibility.Visible;
            }
            else
            {
                _fullScreenWindow.Visibility = Visibility.Collapsed;
                _fullScreenWindow.Content = null;
                Grid.Children.Add(_remoteControl);
                Button_ExitFull.Visibility = Visibility.Collapsed;
            }

            _remoteControl.GoFullScreen(state);
            return true;
        }
        /// <summary>
        /// 远程桌面连接成功
        /// </summary>
        private void _remoteControl_OnConnected()
        {
            _isConntec = true;
            if (_startFullScreen) FullScreen(true);
            Panel_Animation.Visibility = Visibility.Collapsed;
            _remoteControl.Visibility = Visibility.Visible;

            //解决调整窗口尺寸的时候闪烁的问题
            if (_remoteControl.GetType() == typeof(RemoteControl_rdp))
            {
                TimerCallback ThreadMethod = (s) =>
                {
                    Dispatcher.Invoke(() => { Panel_Animation.Visibility = Visibility.Collapsed; });
                };
                Timer threadTimer = null;

                var window = Window.GetWindow(this);
                window.SizeChanged += (s, e) =>
                {
                    if (IsVisible)
                    {
                        if (threadTimer == null) threadTimer = new Timer(ThreadMethod, null, -1, -1);
                        threadTimer.Change(3000, -1);
                        Panel_Animation.Visibility = Visibility.Visible;
                    }
                };
            }
        }
        /// <summary>
        /// 远程桌面发生非致命错误
        /// </summary>
        private void OnNonfatal(string title, string errorText)
        {
            if (_remoteControl != null) _remoteControl.Visibility = Visibility.Hidden;
            if (_startFullScreen) MessageDialog.Show(_fullScreenWindow, title, errorText, MessageDialog.MB_OK);
            else MessageDialog.Show(this, title, errorText, MessageDialog.MB_OK);
            if (_remoteControl != null) _remoteControl.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// 远程桌面发生致命错误
        /// </summary>
        private void OnFatalError(string title ,string errorText)
        {
            if (_remoteControl != null) _remoteControl.Visibility = Visibility.Collapsed;
            MessageDialog.ButtnClick Button_Ok_Click = (btnType) =>
            {
                MainWindow.RemoveTabItem(_tabItem);
            };
            if (_fullScreen) MessageDialog.Show(_fullScreenWindow, title, errorText, MessageDialog.MB_OK, Button_Ok_Click);
            else
            {
                if (ActualHeight < 1) UpdateLayout();//防止弹出报错的时候，控件还没有正常在界面显示
                MessageDialog.Show(this, title, errorText, MessageDialog.MB_OK, Button_Ok_Click);
            }
        }

        private void Button_ExitFull_Click(object sender, RoutedEventArgs e)
        {
            FullScreen(false);
        }
    }
}
