using SimpleRemote.Core;
using SimpleRemote.Modes;
using System.Windows;
using System.Windows.Controls;

namespace SimpleRemote.Container.RemoteSetting
{
    public class SettingControl : UserControl
    {
        public new virtual void Loaded(DbItemRemoteLink itemRemoteLink)
        {
        }

        public virtual void UnLoaded()
        {
        }

        /// <summary>
        /// 更新ItemSetting 到数据库
        /// </summary>
        public void UpdataData()
        {
            DatabaseServices.Update(ItemSetting.Id, ItemSetting);
        }

        /// <summary>
        /// 添加重置 和 设置为默认  按钮
        /// </summary>

        public void AddUtilityButton(Grid grid, int row, int column, int rowSpan = 1, int columnSpan = 1)
        {
            DefaultButton button = new DefaultButton();
            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            Grid.SetRowSpan(button, rowSpan);
            Grid.SetColumnSpan(button, columnSpan);
            grid.Children.Add(button);
            button.PART_Reset.Click += PART_Reset_Click;
            button.PART_Default.Click += PART_Default_Click;
        }

        /// <summary>
        /// 更新默认设置
        /// </summary>
        public void UpdateDefault()
        {
            ItemSetting.SetDefaultSetting();
        }

        public DbItemSetting ItemSetting { get; set; }

        public event RoutedEventHandler ResetButton_Click;

        public event RoutedEventHandler DefaultButton_Click;

        private void PART_Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetButton_Click?.Invoke(sender, e);
        }

        private void PART_Default_Click(object sender, RoutedEventArgs e)
        {
            DefaultButton_Click?.Invoke(sender, e);
        }
    }
}