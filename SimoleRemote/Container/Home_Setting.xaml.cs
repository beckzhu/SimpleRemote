using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SimpleRemote.Bll;
using SimpleRemote.Modes;

namespace SimpleRemote.Container
{
    /// <summary>
    /// RdpPage.xaml 的交互逻辑
    /// </summary>
    public partial class Home_Setting : UserControl
    {
        private DbItemSetting_rdp _globalSetting_rdp;
        private DbItemSetting_ssh _globalSetting_ssh;
        private DbItemSetting_telnet _globalSetting_telnet;
        private bool isEvent;

        public Home_Setting()
        {
            _globalSetting_rdp = GlobalSetting.Settings_rdp;
            _globalSetting_ssh = GlobalSetting.Settings_ssh;
            _globalSetting_telnet = GlobalSetting.Settings_telnet;

            InitializeComponent();

            Grid_Left_rdp.DataContext = _globalSetting_rdp;
            Grid_Right_rdp.DataContext = _globalSetting_rdp;
            Grid_Left_ssh.DataContext = _globalSetting_ssh;
            Grid_Right_ssh.DataContext = _globalSetting_ssh;
            Grid_Left_Telnet.DataContext = _globalSetting_telnet;
            Grid_Right_Telnet.DataContext = _globalSetting_telnet;

            //RDP加载系统支持的分辨率
            foreach (var item in GlobalSetting.ScreenSizes)
            {
                Rdp_ComboBox_DeskTopSize.Items.Add($"{item.Width} x {item.Height}");
            }
            if (Common.OSVersion <= 6.1f) Rdp_ComboBox_Recording.IsEnabled = false;

            //SSH Telnet加载系统拥有的等宽字体
            foreach (var item in GlobalSetting.EqualWidthFonts)
            {
                SSH_ComboBox_FontName.Items.Add(item);
                Telnet_ComboBox_FontName.Items.Add(item);
            }
            //SSH Telnet加载字体大小
            for (int i = 8; i < 33; i++)
            {
                SSH_ComboBox_FontSize.Items.Add(i);
                Telnet_ComboBox_FontSize.Items.Add(i);
            }
            //SSH Telnet加载系统支持的字符集
            foreach (var item in GlobalSetting.Encodings)
            {
                SSH_ComboBox_Character.Items.Add(item.DisplayName);
                Telnet_ComboBox_Character.Items.Add(item.DisplayName);
            }
            //SSH Telnet加载配色方案
            foreach (var name in GlobalSetting.PuttyColorlNames)
            {
                SSH_ComboBox_Color.Items.Add(name);
                Telnet_ComboBox_Color.Items.Add(name);
            }
            //SSH设置默认选中
            SSH_ComboBox_FontName.SelectedIndex = Array.FindIndex(GlobalSetting.EqualWidthFonts, m => m == _globalSetting_ssh.FontName) + 1;
            SSH_ComboBox_Character.SelectedIndex = Array.FindIndex(GlobalSetting.Encodings, m => m.CodePage == _globalSetting_ssh.Character) + 1;
            SSH_ComboBox_Color.SelectedIndex = Array.FindIndex(GlobalSetting.PuttyColorlNames, m => m == _globalSetting_ssh.ColorScheme) + 1;
            //Telnet设置默认选中
            Telnet_ComboBox_FontName.SelectedIndex = Array.FindIndex(GlobalSetting.EqualWidthFonts, m => m == _globalSetting_telnet.FontName) + 1;
            Telnet_ComboBox_Character.SelectedIndex = Array.FindIndex(GlobalSetting.Encodings, m => m.CodePage == _globalSetting_telnet.Character) + 1;
            Telnet_ComboBox_Color.SelectedIndex = Array.FindIndex(GlobalSetting.PuttyColorlNames, m => m == _globalSetting_telnet.ColorScheme) + 1;

            if (Database.IsPassword(null)) Psw_PasswordBox_Old.IsEnabled = false;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isEvent)//设置ComboBox 选中项被改变事件
            {
                //常规
                foreach (var item in Grid_General.GetChildObjects<CheckBox>())
                    item.Click += CheckBox_Checked_General;
                //rdp
                foreach (var item in Grid_Left_rdp.GetChildObjects<ComboBox>())
                    item.SelectionChanged += ComboBox_SelectionChanged_rdp;
                foreach (var item in Grid_Right_rdp.GetChildObjects<ComboBox>())
                    item.SelectionChanged += ComboBox_SelectionChanged_rdp;
                //ssh
                foreach (var item in Grid_Left_ssh.GetChildObjects<ComboBox>())
                    item.SelectionChanged += ComboBox_SelectionChanged_ssh;
                foreach (var item in Grid_Right_ssh.GetChildObjects<ComboBox>())
                    item.SelectionChanged += ComboBox_SelectionChanged_ssh;
                foreach (var item in Grid_Right_ssh.GetChildObjects<CheckBox>())
                    item.Click += CheckBox_Checked_ssh;
                //telnet
                foreach (var item in Grid_Left_Telnet.GetChildObjects<ComboBox>())
                    item.SelectionChanged += ComboBox_SelectionChanged_telnet;
                foreach (var item in Grid_Right_Telnet.GetChildObjects<ComboBox>())
                    item.SelectionChanged += ComboBox_SelectionChanged_telnet;
                foreach (var item in Grid_Right_Telnet.GetChildObjects<CheckBox>())
                    item.Click += CheckBox_Checked_telnet;
                isEvent = true;
            }
        }

        private void ComboBox_SelectionChanged_rdp(object sender, SelectionChangedEventArgs e)
        {
            Database.Update(_globalSetting_rdp.Id, _globalSetting_rdp);
        }

        private void ComboBox_SelectionChanged_ssh(object sender, SelectionChangedEventArgs e)
        {
            if (sender == SSH_ComboBox_FontName) _globalSetting_ssh.FontName = GlobalSetting.EqualWidthFonts[SSH_ComboBox_FontName.SelectedIndex - 1];
            if (sender == SSH_ComboBox_Character && SSH_ComboBox_Character.SelectedIndex <= GlobalSetting.Encodings.Length)
                _globalSetting_ssh.Character = GlobalSetting.Encodings[SSH_ComboBox_Character.SelectedIndex - 1].CodePage;
            if (sender == SSH_ComboBox_Color) _globalSetting_ssh.ColorScheme = SSH_ComboBox_Color.SelectedItem.ToString();
            Database.Update(_globalSetting_ssh.Id, _globalSetting_ssh);
        }

        private void ComboBox_SelectionChanged_telnet(object sender, SelectionChangedEventArgs e)
        {
            if (sender == Telnet_ComboBox_FontName) _globalSetting_telnet.FontName = GlobalSetting.EqualWidthFonts[Telnet_ComboBox_FontName.SelectedIndex - 1];
            if (sender == Telnet_ComboBox_Character && Telnet_ComboBox_Character.SelectedIndex <= GlobalSetting.Encodings.Length)
                _globalSetting_telnet.Character = GlobalSetting.Encodings[Telnet_ComboBox_Character.SelectedIndex - 1].CodePage;
            if (sender == Telnet_ComboBox_Color) _globalSetting_telnet.ColorScheme = Telnet_ComboBox_Color.SelectedItem.ToString();
            Database.Update(_globalSetting_telnet.Id, _globalSetting_telnet);
        }

        private void CheckBox_Clicked_rdp(object sender, RoutedEventArgs e)
        {
            Database.Update(_globalSetting_rdp.Id, _globalSetting_rdp);
        }

        private void CheckBox_Checked_ssh(object sender, RoutedEventArgs e)
        {
            Database.Update(_globalSetting_ssh.Id, _globalSetting_ssh);
        }

        private void CheckBox_Checked_telnet(object sender, RoutedEventArgs e)
        {
            Database.Update(_globalSetting_telnet.Id, _globalSetting_telnet);
        }

        private void CheckBox_Checked_General(object sender, RoutedEventArgs e)
        {
            if(sender== General_CheckBox_Tray) UserSettings.NotifyIcon = General_CheckBox_Tray.IsChecked.Value;

            if (sender == General_CheckBox_Bootup)//开机自启
            {
                try
                {
                    UserSettings.Bootup = General_CheckBox_Bootup.IsChecked.Value;
                }
                catch
                {
                    General_CheckBox_Bootup.IsChecked = false;
                    MainWindow.ShowMessageDialog("提示", "无法设置开机自启，请尝试用管理员权限运行。");
                }
            }
        }

        private void Psw_Button_Change_Click(object sender, RoutedEventArgs e)
        {
            if (Psw_PasswordBox_Old.IsEnabled && !Database.IsPassword(Psw_PasswordBox_Old.Password))
            {
                MainWindow.ShowMessageDialog("提示", "原密码不正确。");
                return;
            }
            else if (Psw_PasswordBox_New.Password != Psw_PasswordBox_Repeat.Password)
            {
                MainWindow.ShowMessageDialog("提示", "两次输入的密码不一致。");
                return;
            }
            Database.ChangePassword(Psw_PasswordBox_New.Password);
            MainWindow.ShowMessageDialog("提示", "密码修改成功。");

            Psw_PasswordBox_Old.IsEnabled = !Database.IsPassword(null);
            Psw_PasswordBox_New.Password = "";
            Psw_PasswordBox_Old.Password = "";
            Psw_PasswordBox_Repeat.Password = "";
        }
    }
}
