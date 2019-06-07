using SimpleRemote.Core;
using SimpleRemote.Modes;
using System.Windows;
using System.Windows.Controls;

namespace SimpleRemote.Container
{
    /// <summary>
    /// RdpPage.xaml 的交互逻辑
    /// </summary>
    public partial class Home_Setting : UserControl
    {
        public Home_Setting()
        {
            InitializeComponent();

            if (DatabaseServices.IsPassword(null)) PART_Psw_Old.IsEnabled = false;
            General_ComboBox_Location.SelectedIndex = (int)UserSettings.AppDataLocation;
            General_ComboBox_Location.SelectionChanged += General_ComboBox_Location_SelectionChanged;
        }

        private void CheckBox_Checked_General(object sender, RoutedEventArgs e)
        {
            //在系统托盘（通知区域）显示应用程序
            if (sender == PART_Tray)
            {
                UserSettings.NotifyIcon = PART_Tray.IsChecked.Value;
            }
            //开机自启
            if (sender == PART_Bootup)
            {
                try
                {
                    UserSettings.Bootup = PART_Bootup.IsChecked.Value;
                }
                catch
                {
                    PART_Bootup.IsChecked = false;
                    MainWindow.ShowMessageDialog("提示", "无法设置开机自启，请尝试用管理员权限运行。");
                }
            }
        }

        private void PART_Psw_Change_Click(object sender, RoutedEventArgs e)
        {
            if (PART_Psw_Old.IsEnabled && !DatabaseServices.IsPassword(PART_Psw_Old.Password))
            {
                MainWindow.ShowNoticeDialog("提示", "修改密码失败，原密码不正确。");
                return;
            }
            else if (PART_Psw_New.Password != PART_Psw_Repeat.Password)
            {
                MainWindow.ShowNoticeDialog("提示", "修改密码失败，两次输入的密码不一致。");
                return;
            }
            DatabaseServices.ChangePassword(PART_Psw_New.Password);
            MainWindow.ShowNoticeDialog("提示", "修改密码成功。");

            PART_Psw_Old.IsEnabled = !DatabaseServices.IsPassword(null);
            PART_Psw_New.Password = "";
            PART_Psw_Old.Password = "";
            PART_Psw_Repeat.Password = "";
        }

        /// <summary>
        /// 数据存储目录被改变
        /// </summary>
        private void General_ComboBox_Location_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserSettings.NewAppDataLocation = (DataStoreLocation)General_ComboBox_Location.SelectedIndex;
        }

        /// <summary>
        /// 重置远程连接默认设置
        /// </summary>
        private void PART_Reset_Change_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            DbItemSetting itemSetting;
            if (PART_Reset_Rdp.IsChecked.Value)
            {
                i++;
                itemSetting = DbItemSettingRdp.FromDefault();
                itemSetting.SetDefaultSetting();
            }
            if (PART_Reset_Ssh.IsChecked.Value)
            {
                i++;
                itemSetting = DbItemSettingSsh.FromDefault();
                itemSetting.SetDefaultSetting();
            }
            if (PART_Reset_Telnet.IsChecked.Value)
            {
                i++;
                itemSetting = DbItemSettingTelnet.FromDefault();
                itemSetting.SetDefaultSetting();
            }

            MainWindow.ShowNoticeDialog("提示", i == 0 ? "至少选择一个要重置的项目" : "所选的项目重置重置成功");
        }
    }
}