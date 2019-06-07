using SimpleRemote.Modes;
using System.Windows;
using System.Windows.Controls;

namespace SimpleRemote.Container.RemoteSetting
{
    /// <summary>
    /// RdpPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingUtility : SettingControl
    {
        public SettingUtility()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 加载分辨率列表
        /// </summary>
        public void AddDeskSize(Size[] size)
        {
            for (int i = 0; i < size.Length; i++)
            {
                PART_DeskTopSize.Items.Add($"{size[i].Width} x {size[i].Height}");
            }
        }
    }
}
