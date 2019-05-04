using System;
using System.Collections.Generic;
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
    /// Home_Remote_telnet.xaml 的交互逻辑
    /// </summary>
    public partial class Home_Remote_telnet : UserControl
    {
        private DbItemSetting_telnet _itemTelnetSetting;

        public Home_Remote_telnet()
        {
            InitializeComponent();
            //加载系统拥有的等宽字体
            foreach (var item in GlobalSetting.EqualWidthFonts) Telnet_ComboBox_FontName.Items.Add(item);
            //加载字体大小
            for (int i = 8; i < 33; i++) Telnet_ComboBox_FontSize.Items.Add(i);
            //加载系统支持的字符集
            foreach (var item in GlobalSetting.Encodings) Telnet_ComboBox_Character.Items.Add(item.DisplayName);
            DataContext = null;
            foreach (var item in Grid_Telnet.GetChildObjects<CheckBox>()) item.Click += CheckBox_Click;
            //加载配色方案
            foreach (var name in GlobalSetting.PuttyColorlNames) Telnet_ComboBox_Color.Items.Add(name);
        }


        public void Load(DbItemRemoteLink itemRemoteLink)
        {
            //清除ComboBox_SelectionChanged的事件，避免更改DataContext后立即引发事件
            foreach (var item in Grid_Telnet.GetChildObjects<ComboBox>()) item.SelectionChanged -= ComboBox_SelectionChanged;
            _itemTelnetSetting = (DbItemSetting_telnet)Database.GetRemoteSetting(itemRemoteLink);
            DataContext = _itemTelnetSetting;
            Telnet_ComboBox_FontName.SelectedIndex = Array.FindIndex(GlobalSetting.EqualWidthFonts, m => m == _itemTelnetSetting.FontName) + 1;
            Telnet_ComboBox_Character.SelectedIndex = Array.FindIndex(GlobalSetting.Encodings, m => m.CodePage == _itemTelnetSetting.Character) + 1;
            Telnet_ComboBox_Color.SelectedIndex = Array.FindIndex(GlobalSetting.PuttyColorlNames, m => m == _itemTelnetSetting.ColorScheme) + 1;
            foreach (var item in Grid_Telnet.GetChildObjects<ComboBox>()) item.SelectionChanged += ComboBox_SelectionChanged;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            Database.Update(_itemTelnetSetting.Id, _itemTelnetSetting);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == Telnet_ComboBox_FontName)
            {
                if (Telnet_ComboBox_FontName.SelectedIndex - 1 < 0) _itemTelnetSetting.FontName = null;
                else _itemTelnetSetting.FontName = GlobalSetting.EqualWidthFonts[Telnet_ComboBox_FontName.SelectedIndex - 1];
            }
            if (sender == Telnet_ComboBox_Character)
            {
                if (Telnet_ComboBox_Character.SelectedIndex - 1 < 0) _itemTelnetSetting.Character = 0;
                else _itemTelnetSetting.Character = GlobalSetting.Encodings[Telnet_ComboBox_Character.SelectedIndex - 1].CodePage;
            }
            if (sender == Telnet_ComboBox_Color)
            {
                if (Telnet_ComboBox_Color.SelectedIndex - 1 < 0) _itemTelnetSetting.ColorScheme = null;
                else _itemTelnetSetting.ColorScheme = Telnet_ComboBox_Color.SelectedItem.ToString();
            }
            Database.Update(_itemTelnetSetting.Id, _itemTelnetSetting);
        }
    }
}
