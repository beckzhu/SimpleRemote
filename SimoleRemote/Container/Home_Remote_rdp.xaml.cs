using System.Windows;
using System.Windows.Controls;
using SimpleRemote.Bll;
using SimpleRemote.Modes;

namespace SimpleRemote.Container
{
    /// <summary>
    /// RdpPage.xaml 的交互逻辑
    /// </summary>
    public partial class Home_Remote_rdp : UserControl
    {
        private DbItemSetting_rdp _itemRdpSetting;

        public Home_Remote_rdp()
        {
            InitializeComponent();
            //加载系统支持的分辨率
            foreach (var item in GlobalSetting.ScreenSizes)
            ComboBox_DeskTopSize.Items.Add($"{item.Width} x {item.Height}");
            DataContext = null;

            if (Common.OSVersion <= 6.1f) ComboBox_Recording.IsEnabled = false;
            {
              
            } 
        }
        public void Load(DbItemRemoteLink itemRemoteLink)
        {
            _itemRdpSetting = (DbItemSetting_rdp)Database.GetRemoteSetting(itemRemoteLink);
            DataContext = _itemRdpSetting;
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            Database.Update(_itemRdpSetting.Id, _itemRdpSetting);
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Database.Update(_itemRdpSetting.Id,_itemRdpSetting);
        } 
    }
}
