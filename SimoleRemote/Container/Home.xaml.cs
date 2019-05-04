using System;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using SimpleRemote.Bll;

namespace SimpleRemote.Container
{
    /// <summary>
    /// Home.xaml 的交互逻辑
    /// </summary>
    public partial class Home : UserControl
    {
        private Home_Remote Home_Items;
        private Home_Setting Home_Setting;
        private Home_About Home_About;

        public Home()
        {
            InitializeComponent();
        }

        public async void Load()
        {
            GlobalSetting.LoadSetiing();
            Home_Items = new Home_Remote();
            Home_Setting = new Home_Setting();
            Home_About = new Home_About();
            HamburgerMenu.Content = Home_Items;
            RemoteItems.LoadItems(Home_Items.TreeView_Item);

            DateTime localDateTime = DateTime.Today;
            //DateTime localDateTime = new DateTime();//测试使用 每次都检查更新
            if (UserSettings.FinalCheckDateTime != localDateTime)
            {
                UserSettings.FinalCheckDateTime = localDateTime;
                if (await Home_About.CheckUpdates())
                {
                    HamburgerMenu.Content = Home_About;
                    HamburgerMenu.SelectedIndex = 2;
                }
            }
        }

        private void HamburgerMenu_ItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            var menuItem = e.InvokedItem as HamburgerMenuItem;
            if (menuItem != null)
            {
                switch (menuItem.Label)
                {
                    case "远程桌面":
                        HamburgerMenu.Content = Home_Items;
                        break;
                    case "全局设置":
                        HamburgerMenu.Content = Home_Setting;
                        break;
                    case "关于":
                        HamburgerMenu.Content = Home_About;
                        break;
                    default:
                        HamburgerMenu.Content = null;
                        break;
                }
            }
        }

        public void Save()
        {
            Home_Items?.Save();
            UserSettings.Home_IsPaneOpen = HamburgerMenu.IsPaneOpen;
        }

        
    }
}
