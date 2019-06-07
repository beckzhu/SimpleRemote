using SimpleRemote.Core;
using SimpleRemote.Modes;

namespace SimpleRemote.Container.RemoteSetting
{
    /// <summary>
    /// RdpPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingRdp : SettingControl
    {
        private string _remoteName;

        public SettingRdp()
        {
            InitializeComponent();
            DataContext = null;
            PART_Utility.AddDeskSize(CommonServices.DesktopSizes);
            if (CommonServices.OSVersion <= 6.1f) PART_Recording.IsEnabled = false;//win8以下  不能设置录音
            AddUtilityButton(PART_Main, 15, 2, 1, 1);
            base.ResetButton_Click += ResetButton_Click;
            base.DefaultButton_Click += DefaultButton_Click;
        }

        private new void DefaultButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateDefault();
            MainWindow.ShowNoticeDialog("信息", $"已将 \"{_remoteName}\" 作为RDP的默认设置。");
        }

        private new void ResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ItemSetting.Reset();
            UpdataData();
            MainWindow.ShowNoticeDialog("信息", $"\"{_remoteName}\" 远程桌面设置已重置。");
        }

        public override void Loaded(DbItemRemoteLink itemRemoteLink)
        {
            var itemRdpSetting = DatabaseServices.GetRemoteSetting(itemRemoteLink) as DbItemSettingRdp;
            if (itemRdpSetting != null)
            {
                _remoteName = itemRemoteLink.Name;
                DataContext = itemRdpSetting;
                ItemSetting = itemRdpSetting;
                itemRdpSetting.PropertyChanged += PropertyChanged;
                PART_Utility.Loaded(itemRemoteLink);
            }
        }

        public override void UnLoaded()
        {
            PART_Utility.UnLoaded();
            ItemSetting.PropertyChanged -= PropertyChanged;
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdataData();
        }
    }
}