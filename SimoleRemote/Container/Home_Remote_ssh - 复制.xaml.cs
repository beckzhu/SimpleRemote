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
using SimpleRemote.Control;
using SimpleRemote.Modes;

namespace SimpleRemote.Container
{
    /// <summary>
    /// RdpPage.xaml 的交互逻辑
    /// </summary>
    public partial class Home_Remote_ssh : UserControl
    {
        private DbItemSetting_ssh _itemSshSetting;

        public Home_Remote_ssh()
        {
            InitializeComponent();
            //加载系统拥有的等宽字体
            foreach (var item in GlobalSetting.EqualWidthFonts) SSH_ComboBox_FontName.Items.Add(item);
            //加载字体大小
            for (int i = 8; i < 33; i++) SSH_ComboBox_FontSize.Items.Add(i);
            //加载系统支持的字符集
            foreach (var item in GlobalSetting.Encodings) SSH_ComboBox_Character.Items.Add(item.DisplayName);
            DataContext = null;
            foreach (var item in Grid_ssh.GetChildObjects<CheckBox>()) item.Click += CheckBox_Click;
            //加载配色方案
            foreach (var name in GlobalSetting.PuttyColorlNames) SSH_ComboBox_Color.Items.Add(name);
        }

        public void Load(DbItemRemoteLink itemRemoteLink)
        {
            //清除ComboBox_SelectionChanged的事件，避免更改DataContext后立即引发事件
            foreach (var item in Grid_ssh.GetChildObjects<ComboBox>()) item.SelectionChanged -= ComboBox_SelectionChanged;
            _itemSshSetting = (DbItemSetting_ssh)Database.GetRemoteSetting(itemRemoteLink);
            DataContext = _itemSshSetting;
            SSH_ComboBox_FontName.SelectedIndex = Array.FindIndex(GlobalSetting.EqualWidthFonts, m => m == _itemSshSetting.FontName) + 1;
            SSH_ComboBox_Character.SelectedIndex = Array.FindIndex(GlobalSetting.Encodings, m => m.CodePage == _itemSshSetting.Character) + 1;
            SSH_ComboBox_Color.SelectedIndex = Array.FindIndex(GlobalSetting.PuttyColorlNames, m => m == _itemSshSetting.ColorScheme) + 1;
            foreach (var item in Grid_ssh.GetChildObjects<ComboBox>()) item.SelectionChanged += ComboBox_SelectionChanged;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            Database.Update(_itemSshSetting.Id, _itemSshSetting);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == SSH_ComboBox_FontName)
            {
                if (SSH_ComboBox_FontName.SelectedIndex - 1 < 0) _itemSshSetting.FontName = null;
                else _itemSshSetting.FontName = GlobalSetting.EqualWidthFonts[SSH_ComboBox_FontName.SelectedIndex - 1];
            }
            if (sender == SSH_ComboBox_Character)
            {
                if (SSH_ComboBox_Character.SelectedIndex - 1 < 0) _itemSshSetting.Character = 0;
                else _itemSshSetting.Character = GlobalSetting.Encodings[SSH_ComboBox_Character.SelectedIndex - 1].CodePage;
            }
            if (sender == SSH_ComboBox_Color)
            {
                if (SSH_ComboBox_Color.SelectedIndex - 1 < 0) _itemSshSetting.ColorScheme = null;
                _itemSshSetting.ColorScheme = SSH_ComboBox_Color.SelectedItem.ToString();
            }
            Database.Update(_itemSshSetting.Id, _itemSshSetting);
        }

    }
}
