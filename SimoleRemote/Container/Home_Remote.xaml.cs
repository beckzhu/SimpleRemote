using Microsoft.Win32;
using System.IO;
using SimpleRemote.Bll;
using SimpleRemote.Control;
using SimpleRemote.Modes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace SimpleRemote.Container
{
    /// <summary>
    /// Home_Tree.xaml 的交互逻辑
    /// </summary>
    public partial class Home_Remote : UserControl
    {
        private DbItemRemoteLink _itemRemoteLink;
        private Home_Remote_rdp _home_Remote_rdp;
        private Home_Remote_ssh _home_Remote_ssh;
        private Home_Remote_telnet _home_Remote_telnet;

        public Home_Remote()
        {
            InitializeComponent();
            _home_Remote_ssh = new Home_Remote_ssh();
            _home_Remote_rdp = new Home_Remote_rdp();
            _home_Remote_telnet = new Home_Remote_telnet();
        }

        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(TreeView_Item);
                RemoteTreeViewItem element = TreeView_Item.GetElementFromPoint<RemoteTreeViewItem>(point);
                foreach (MenuItem item in TreeView_Item.ContextMenu.Items) item.IsEnabled = true;//将所有右键菜单启用

                if (element != null)
                {
                    element.IsSelected = true;
                    if (element.RemoteType == RemoteType.dir)
                    {
                        ((MenuItem)TreeView_Item.ContextMenu.Items[0]).IsEnabled = false;//打开
                        ((MenuItem)TreeView_Item.ContextMenu.Items[1]).IsEnabled = false;//"打开(后台)"
                        ((MenuItem)TreeView_Item.ContextMenu.Items[2]).IsEnabled = false;//"打开(外部)"
                    }
                }
                else
                {
                    TreeViewItem item = TreeView_Item.SelectedItem as TreeViewItem;
                    if (item != null) item.IsSelected = false;
                    ((MenuItem)TreeView_Item.ContextMenu.Items[0]).IsEnabled = false;//打开
                    ((MenuItem)TreeView_Item.ContextMenu.Items[1]).IsEnabled = false;//"打开(后台)"
                    ((MenuItem)TreeView_Item.ContextMenu.Items[2]).IsEnabled = false;//"打开(外部)"
                    ((MenuItem)TreeView_Item.ContextMenu.Items[5]).IsEnabled = false;//"重命名"
                    ((MenuItem)TreeView_Item.ContextMenu.Items[6]).IsEnabled = false;//"删除"
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuItem = sender as MenuItem; if (menuItem == null) return;
                RemoteTreeViewItem selectItem = (RemoteTreeViewItem)TreeView_Item.SelectedItem;
                RemoteTreeViewItem treeViewItem;
                switch (menuItem.Header.ToString())
                {
                    case "新建目录":
                        treeViewItem = RemoteItems.Insert(RemoteType.dir, selectItem, "新建目录"); ;
                        if (treeViewItem.IsVisible)
                            treeViewItem.Loaded += (treeItemsender, treeIteme) => treeViewItem.HeaderEdit(Home_Tree_EditHeaderClosing);
                        break;
                    case "RDP连接":
                        treeViewItem = RemoteItems.Insert(RemoteType.rdp, selectItem, "新建RDP连接");
                        if (treeViewItem.IsVisible)
                            treeViewItem.Loaded += (treeItemsender, treeIteme) => treeViewItem.HeaderEdit(Home_Tree_EditHeaderClosing);
                        break;
                    case "SSH连接":
                        treeViewItem = RemoteItems.Insert(RemoteType.ssh, selectItem, "新建SSH连接");
                        if (treeViewItem.IsVisible)
                            treeViewItem.Loaded += (treeItemsender, treeIteme) => treeViewItem.HeaderEdit(Home_Tree_EditHeaderClosing);
                        break;
                    case "Telnet连接":
                        treeViewItem = RemoteItems.Insert(RemoteType.telnet, selectItem, "新建Telnet连接");
                        if (treeViewItem.IsVisible)
                            treeViewItem.Loaded += (treeItemsender, treeIteme) => treeViewItem.HeaderEdit(Home_Tree_EditHeaderClosing);
                        break;
                    case "删除":
                        RemoteItems.Delete(selectItem);
                        break;
                    case "打开":
                        RemoteItems.OpenRemote(selectItem, DbItemSetting.OPEN_TAB);
                        break;
                    case "打开(后台)":
                        RemoteItems.OpenRemote(selectItem, DbItemSetting.OPEN_TAB_BACKSTAGE);
                        break;
                    case "打开(外部)":
                        RemoteItems.OpenRemote(selectItem, DbItemSetting.OPEN_WINDOW);
                        break;
                    case "重命名":
                        selectItem.HeaderEdit(Home_Tree_EditHeaderClosing);
                        break;
                }
            }
            catch (Exception ex)
            {
                MainWindow.ShowMessageDialog("错误", ex.Message);
            }
        }

        private void Home_Tree_EditHeaderClosing(object sender, CancelEditEventArgs e)
        {
            RemoteTreeViewItem treeItem = ((RemoteTreeViewItem)sender); if (treeItem == null) return;
            if (string.IsNullOrEmpty(e.NewValue))
            {
                MainWindow.ShowMessageDialog("提示", "名称不能为空");
                e.Cancel = true;
                return;
            }

            if (treeItem.RemoteType == RemoteType.dir)
            {
                DbItemDirectory itemDirectory = Database.GeyDirectory(treeItem.uuid);
                itemDirectory.Name = e.NewValue;
                Database.Update(itemDirectory.Id, itemDirectory);
            }
            else
            {
                if (_itemRemoteLink != null && _itemRemoteLink.Id == treeItem.uuid)
                {
                    _itemRemoteLink.Name = e.NewValue;
                    Database.Update(_itemRemoteLink.Id, _itemRemoteLink);
                    TextBox_Name.Text = e.NewValue;
                }
                else
                {
                    DbItemRemoteLink itemRemoteLink = Database.GetRemoteLink(treeItem.uuid);
                    itemRemoteLink.Name = e.NewValue;
                    Database.Update(itemRemoteLink.Id, itemRemoteLink);
                }

            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                if (_itemRemoteLink != null) Database.Update(_itemRemoteLink.Id, _itemRemoteLink);
                RemoteTreeViewItem selectItem = (RemoteTreeViewItem)e.NewValue;

                if (selectItem.RemoteType != RemoteType.dir)
                {
                    ScrollViewer.Visibility = Visibility.Visible;
                    _itemRemoteLink = Database.GetRemoteLink(selectItem.uuid);
                    ScrollViewer.DataContext = _itemRemoteLink;
                    TextBox_Password.Password = _itemRemoteLink.Password;
                    TextBox_Name.Text = _itemRemoteLink.Name;

                    switch (selectItem.RemoteType)
                    {
                        case RemoteType.rdp:
                            Expander_Setting.Content = _home_Remote_rdp;
                            _home_Remote_rdp.Load(_itemRemoteLink);
                            Grid_PrivateKey.Visibility = Visibility.Collapsed;
                            break;
                        case RemoteType.ssh:
                            //加载私钥
                            if (_itemRemoteLink.PrivateKey == null)
                                CheckBox_PrivateKey.IsChecked = false;
                            else
                                CheckBox_PrivateKey.IsChecked = true;
                            CheckBox_PrivateKey_Click(null, null);

                            Expander_Setting.Content = _home_Remote_ssh;
                            _home_Remote_ssh.Load(_itemRemoteLink);
                            Grid_PrivateKey.Visibility = Visibility.Visible;
                            break;
                        case RemoteType.telnet:
                            Expander_Setting.Content = _home_Remote_telnet;
                            _home_Remote_telnet.Load(_itemRemoteLink);
                            TextBox_Password.IsEnabled = true;
                            Grid_PrivateKey.Visibility = Visibility.Collapsed;
                            break;
                        default:
                            Expander_Setting.Content = null;
                            break;
                    }
                }
                else ScrollViewer.Visibility = Visibility.Collapsed;
            }
        }

        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Point point = e.GetPosition(TreeView_Item);
                RemoteTreeViewItem element = TreeView_Item.GetElementFromPoint<RemoteTreeViewItem>(point);
                if (element != null)
                {
                    if (element.IsHeaderEdit == false && element.RemoteType != RemoteType.dir)
                    {
                        RemoteItems.OpenRemote((RemoteTreeViewItem)TreeView_Item.SelectedItem, DbItemSetting.OPEN_DEFAULT);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.ShowMessageDialog("错误", ex.Message);
            }
        }

        private void TreeView_KeyDown(object sender, KeyEventArgs e)
        {
            RemoteTreeViewItem selectItem = TreeView_Item.SelectedItem as RemoteTreeViewItem;
            if (selectItem == null) return;

            if (e.Key == Key.F2)
            {
                selectItem.HeaderEdit(Home_Tree_EditHeaderClosing);
            }
            if (e.Key == Key.Delete)
            {
                RemoteItems.Delete(selectItem);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender == TextBox_Name)
            {
                if (string.IsNullOrEmpty(TextBox_Name.Text))
                {
                    MainWindow.ShowMessageDialog("提示", "名称不能为空。");
                    TextBox_Name.Text = _itemRemoteLink.Name;
                    return;
                }
                else
                {
                    _itemRemoteLink.Name = TextBox_Name.Text;
                    if (TreeView_Item.SelectedItem != null) ((RemoteTreeViewItem)TreeView_Item.SelectedItem).Header = _itemRemoteLink.Name;
                }
            }
            if (sender == TextBox_Password)
            {
                _itemRemoteLink.Password = TextBox_Password.Password;
            }

            Database.Update(_itemRemoteLink.Id, _itemRemoteLink);
        }

        private void MyExpander_IsExpanded(object sender, RoutedEventArgs e)
        {
            Database.Update(_itemRemoteLink.Id, _itemRemoteLink);
        }

        public void Save()
        {
            UserSettings.HomeRemoteTreeView_Width = Grid_Main.ColumnDefinitions[0].Width;
        }

        private void Button_PrivateKey_Upload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            //op.InitialDirectory=默认的打开路径
            op.RestoreDirectory = true;
            op.Filter = "所有文件(*.*)|*.*";
            op.ShowDialog();
            if (!string.IsNullOrEmpty(op.FileName))
            {
                FileInfo fileInfo = new FileInfo(op.FileName);
                if (fileInfo.Length <= 10000)
                {
                    _itemRemoteLink.PrivateKey = File.ReadAllText(op.FileName);
                    TextBox_PrivateKey.Text = _itemRemoteLink.PrivateKey;
                    Database.Update(_itemRemoteLink.Id, _itemRemoteLink);
                }
                else MainWindow.ShowMessageDialog("错误", "密钥文件不能大于10000个字节");
            }
        }

        private void CheckBox_PrivateKey_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBox_PrivateKey.IsChecked.Value)
            {
                TextBox_PrivateKey.Visibility = Visibility.Visible;
                TextBox_Password.IsEnabled = false;
                TextBox_Password.Password = null;
                _itemRemoteLink.Password = null;
                if (_itemRemoteLink.PrivateKey == null)
                {
                    _itemRemoteLink.PrivateKey = "";
                }
                TextBox_PrivateKey.Text = _itemRemoteLink.PrivateKey;
            }
            else
            {
                TextBox_PrivateKey.Visibility = Visibility.Collapsed;
                TextBox_Password.IsEnabled = true;
                _itemRemoteLink.PrivateKey = null;
            }
            if (sender != null) Database.Update(_itemRemoteLink.Id, _itemRemoteLink);
        }
    }
}

