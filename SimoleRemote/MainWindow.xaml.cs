using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SimpleRemote.Container;
using SimpleRemote.Controls;
using SimpleRemote.Controls.Dragablz;
using SimpleRemote.Controls.Notifications;
using SimpleRemote.Controls.Notifications.Controls;
using SimpleRemote.Controls.NotifyIconWpf;
using SimpleRemote.Core;
using SimpleRemote.Modes;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleRemote
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Home PART_Home;
        private TaskbarIcon PART_TaskbarIcon;

        public MainWindow()
        {
            InitializeComponent();
            //初始化控件
            MainTabControl.ClosingItemCallback = MainTabControl_ClosingItem;
            PART_Noice.Manager = new NotificationMessageManager();
            try
            {
                UserSettings.Init();
                Width = UserSettings.MainWindow_Width;
                Height = UserSettings.MainWindow_Height;
                if (UserSettings.MainWindow_Maximize) WindowState = WindowState.Maximized;

                PART_Home = new Home();
                MainTabControl.Visibility = Visibility.Collapsed;
                TabItem_Home.Content = PART_Home;

                if (!DatabaseServices.Open(null))
                {
                    LoginStartDialog loginStartDialog = new LoginStartDialog();
                    Grid.SetColumnSpan(loginStartDialog, 3);
                    PART_Main.Children.Add(loginStartDialog);
                    loginStartDialog.OnLoginClick += (sender, password) =>
                    {
                        PART_Main.Children.Remove(loginStartDialog);
                        DatabaseServices.Open(password);
                        MainTabControl.Visibility = Visibility.Visible;
                        PART_Home.Init();
                    };
                }
                else
                {
                    MainTabControl.Visibility = Visibility.Visible;
                    PART_Home.Init();
                }
            }
            catch (Exception e)
            {
                ShowMessageDialog("错误", e.Message, true);
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowButtonCommands.MaxButtonClick += WindowButtonCommands_MaxButtonClick;
        }

        /// <summary>主窗口将被关闭 </summary>
        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            foreach (Window win in App.Current.Windows) if (win != this) win.Close();//关闭所有子窗口

            PART_Home?.Save();
            UserSettings.MainWindow_Maximize = WindowState == WindowState.Maximized;
            if (!UserSettings.MainWindow_Maximize)
            {
                UserSettings.MainWindow_Width = Width;
                UserSettings.MainWindow_Height = Height;
            }
            UserSettings.Save();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (PART_TaskbarIcon != null)
            {
                e.Cancel = true;
                Visibility = Visibility.Hidden;
                PART_TaskbarIcon.ShowBalloonTip("提示：", "应用程序在后台运行", BalloonIcon.Info);
            }
        }

        /// <summary>
        /// 显示托盘图标
        /// </summary>
        public void ShowNotifyIcon()
        {
            if (PART_TaskbarIcon != null) return;
            PART_TaskbarIcon = new TaskbarIcon();
            PART_TaskbarIcon.ToolTipText = "SimpleRemote";//最小化到托盘时，鼠标点击时显示的文本
            PART_TaskbarIcon.Icon = new System.Drawing.Icon(CommonServices.GetResourceStream("Icon/Logo.ico"));//程序图标
            //构建菜单
            ContextMenu menu = new ContextMenu();
            menu.MinWidth = 100;
            MenuItem menuItem = new MenuItem { Header = "退出" };
            menuItem.Click += (s, e) =>
            {
                PART_TaskbarIcon = null;
                Application.Current.Shutdown(0);
            };
            menu.Items.Add(menuItem);
            PART_TaskbarIcon.ContextMenu = menu;

            PART_TaskbarIcon.TrayLeftMouseDown += (s, e) =>
            {
                if (IsVisible) Visibility = Visibility.Hidden;
                else
                {
                    Visibility = Visibility.Visible;
                    Activate();
                }
            };
        }

        /// <summary>
        /// 关闭托盘图标
        /// </summary>
        public void CloseNotifyIcon()
        {
            if (PART_TaskbarIcon == null) return;
            PART_TaskbarIcon.CloseBalloon();
            PART_TaskbarIcon.Dispose();
            PART_TaskbarIcon = null;
        }

        /// <summary>
        /// 显示通知
        /// </summary>
        public static void ShowNoticeDialog(string title, string message)
        {
            MainWindow main = Application.Current.MainWindow as MainWindow;
            if (main == null) return;

           NotificationMessage notification = null;
            notification = new NotificationMessage
            {
                Message = message,
                BadgeText = title,
                AccentBrush = new SolidColorBrush(Color.FromRgb(51, 115, 242)),
                Background = Brushes.White,
                Foreground = Brushes.Black,
                FontSize = 14,
                Animates = true,
                AnimationInDuration = 0.2,
                AnimationOutDuration = 0.5,
                Buttons = new ObservableCollection<object>
                {
                    new NotificationMessageButton()
                    {
                        Content = "关闭",
                        Callback = button =>
                        {
                            main.PART_Noice.Manager.Dismiss(notification);
                        }
                    },
                }
            };
            Task.Delay(5000).ContinueWith(m => main.PART_Noice.Manager.Dismiss(notification), TaskScheduler.FromCurrentSynchronizationContext());
            main.PART_Noice.Manager.Queue(notification);
        }

        /// <summary>在主窗口显示提示框 </summary>
        public static async void ShowMessageDialog(string title, string message, bool exit = false, MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainTabControl.IsEnabled = false;
                await mainWindow.ShowMessageAsync(title, message, style);
                mainWindow.MainTabControl.IsEnabled = true;
                if (exit) mainWindow.Close();
            }
        }

        /// <summary>在主窗口添加选项卡</summary>
        public static RemoteTabItem AddTabItem(string header, UserControl userControl, bool jump = false)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                RemoteTabItem tabItem = new RemoteTabItem();
                tabItem.Header = header;
                tabItem.Content = userControl;
                mainWindow.MainTabControl.Items.Add(tabItem);
                if (jump) mainWindow.MainTabControl.SelectedItem = tabItem;
                return tabItem;
            }
            return null;
        }

        /// <summary>在主窗口选中选项卡</summary>
        public static void SelectedTabItem(RemoteTabItem tabItem)
        {
            if (tabItem != null)
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null) mainWindow.MainTabControl.SelectedItem = tabItem;
            }
        }

        /// <summary>在主窗口移除选项卡</summary>
        public static void RemoveTabItem(RemoteTabItem tabItem)
        {
            if (tabItem != null)
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null) mainWindow.MainTabControl.Items.Remove(tabItem);
                tabItem.Closed?.Invoke(tabItem);
            }
        }

        private void WindowButtonCommands_MaxButtonClick(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (WindowState == WindowState.Normal)//准备点击最大化按钮
            {
                IRemoteControl remoteControl = MainTabControl.SelectedContent as RemoteTabControl;
                if (remoteControl != null)
                {
                    bool state = remoteControl.FullScreen(true);
                    if (state) e.Cancel = true;
                }
            }
        }

        private void MainTabControl_ClosingItem(ItemActionCallbackArgs<TabablzControl> args)
        {
            RemoteTabItem tabItem = args.DragablzItem.Content as RemoteTabItem;
            if (tabItem != null)
            {
                tabItem.Closed?.Invoke(tabItem);
            }
        }
    }
}