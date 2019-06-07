using SimpleRemote.Core;
using SimpleRemote.Modes;

namespace SimpleRemote.Container.RemoteSetting
{
    /// <summary>
    /// RdpPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingSsh : SettingControl
    {
        private string _remoteName;

        public SettingSsh()
        {
            InitializeComponent();
            DataContext = null;
            AddUtilityButton(PART_Main, 3, 2, 1, 1);
            base.ResetButton_Click += ResetButton_Click;
            base.DefaultButton_Click += DefaultButton_Click;
        }

        private new void DefaultButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateDefault();
            MainWindow.ShowNoticeDialog("信息", $"已将 \"{_remoteName}\" 作为SSH的默认设置。");
        }

        private new void ResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ItemSetting.Reset();
            UpdataData();
            MainWindow.ShowNoticeDialog("信息", $"\"{_remoteName}\" 远程桌面设置已重置。");
        }

        public override void Loaded(DbItemRemoteLink itemRemoteLink)
        {
            var itemRdpSetting = DatabaseServices.GetRemoteSetting(itemRemoteLink) as DbItemSettingSsh;
            if (itemRdpSetting != null)
            {
                _remoteName = itemRemoteLink.Name;
                DataContext = itemRdpSetting;
                ItemSetting = itemRdpSetting;
                itemRdpSetting.PropertyChanged += PropertyChanged;
                PART_Putty.Loaded(itemRemoteLink);
            }
        }

        public override void UnLoaded()
        {
            ItemSetting.PropertyChanged -= PropertyChanged;
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdataData();
        }
    }
}