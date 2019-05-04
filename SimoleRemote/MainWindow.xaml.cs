using Dragablz;
using Hardcodet.Wpf.TaskbarNotification;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SimpleRemote.Bll;
using SimpleRemote.Container;
using SimpleRemote.Control;
using SimpleRemote.Modes;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SimpleRemote
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Home _Home;
        private TaskbarIcon _taskbarIcon;

        public MainWindow()
        {
            InitializeComponent();
            MainTabControl.ClosingItemCallback = MainTabControl_ClosingItem;
            try
            {
                Common.Init();
                UserSettings.Open();
                Width = UserSettings.MainWindow_Width;
                Height = UserSettings.MainWindow_Height;
                if (UserSettings.MainWindow_Maximize) WindowState = WindowState.Maximized;

                _Home = new Home();
                MainTabControl.Visibility = Visibility.Collapsed;
                TabItem_Home.Content = _Home;

                if (!Database.Open(null))
                {
                    LoginStartDialog loginStartDialog = new LoginStartDialog();
                    Grid_Main.Children.Add(loginStartDialog);
                    loginStartDialog.OnLoginClick += (sender, password) =>
                    {
                        Grid_Main.Children.Remove(loginStartDialog);
                        Database.Open(password);
                        MainTabControl.Visibility = Visibility.Visible;
                        _Home.Load();
                    };
                }
                else
                {
                    MainTabControl.Visibility = Visibility.Visible;
                    _Home.Load();
                }
            }
            catch (Exception e)
            {
                ShowMessageDialog("错误", e.Message, true);
            }
        }
        /// <summary>
        /// 显示托盘图标
        /// </summary>
        public void ShowNotifyIcon()
        {
            if (_taskbarIcon != null) return;
            _taskbarIcon = new TaskbarIcon();
            _taskbarIcon.ToolTipText = "SimpleRemote";//最小化到托盘时，鼠标点击时显示的文本
            _taskbarIcon.Icon = new System.Drawing.Icon(Common.GetResourceStream("Icon/Logo.ico"));//程序图标
            //构建菜单
            ContextMenu menu = new ContextMenu();
            menu.MinWidth = 100;
            MenuItem menuItem = new MenuItem { Header = "退出" };
            menuItem.Click += (s, e) => 
            {
                _taskbarIcon = null;
                Application.Current.Shutdown(0);
            };
            menu.Items.Add(menuItem);
            _taskbarIcon.ContextMenu = menu;

            _taskbarIcon.TrayLeftMouseDown += (s,e)=> 
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
            if (_taskbarIcon == null) return;
            _taskbarIcon.CloseBalloon();
            _taskbarIcon.Dispose();
            _taskbarIcon = null;
        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowButtonCommands.MaxButtonClick += WindowButtonCommands_MaxButtonClick;
        }

        /// <summary>主窗口将被关闭 </summary>
        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            foreach (Window win in App.Current.Windows) if (win != this) win.Close();//关闭所有子窗口

            _Home?.Save();
            UserSettings.MainWindow_Maximize = WindowState == WindowState.Maximized;
            if (!UserSettings.MainWindow_Maximize)
            {
                UserSettings.MainWindow_Width = Width;
                UserSettings.MainWindow_Height = Height;
            }
            UserSettings.Save();
            Environment.Exit(0);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_taskbarIcon != null)
            {
                e.Cancel = true;
                Visibility = Visibility.Hidden;
                _taskbarIcon.ShowBalloonTip("提示：", "应用程序在后台运行", BalloonIcon.Info);
            }
        }

        /// <summary>在主窗口显示提示框 </summary>
        public static async void ShowMessageDialog(string title, string message, bool exit = false, MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainTabControl.IsEnabled = false;
                MetroDialogSettings metroDialogSettings = new MetroDialogSettings();
                metroDialogSettings.OwnerCanCloseWithDialog = true;
                await mainWindow.ShowMessageAsync(title, message, style, metroDialogSettings);
                mainWindow.MainTabControl.IsEnabled = true;
                if (exit) mainWindow.Close();
            }
        }
        /// <summary>在主窗口添加选项卡</summary>
        public static RemoteTabItem AddTabItem(string header,UserControl userControl,bool jump = false)
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
