using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using SimpleRemote.Container.RemoteSetting;
using SimpleRemote.Controls;
using SimpleRemote.Core;
using SimpleRemote.Modes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DragDrop = SimpleRemote.Controls.WpfDragDrop.DragDrop;

namespace SimpleRemote.Container
{
    /// <summary>
    /// Home_Tree.xaml 的交互逻辑
    /// </summary>
    public partial class Home_Remote : UserControl
    {
        private SettingControl _currentSetControl;
        private SettingRdp PART_SettingRdp;
        private SettingSsh PART_SettingSsh;
        private SettingTelnet PART_SettingTelnet;

        public Home_Remote()
        {
            InitializeComponent();
            RemoteItems.LoadTree(PART_RemoteTree);

            PART_SettingSsh = new SettingSsh();
            PART_SettingRdp = new SettingRdp();
            PART_SettingTelnet = new SettingTelnet();

            DragDrop.SetDropHandler(PART_RemoteTree, new RemoteTreeDropTarget());
            DragDrop.SetDropTargetAdornerBrush(PART_RemoteTree, new SolidColorBrush(Color.FromRgb(33, 150, 243)));
        }

        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(PART_RemoteTree);
                RemoteTreeViewItem element = PART_RemoteTree.GetElementFromPoint<RemoteTreeViewItem>(point);
                Dictionary<string, MenuItem> menuItemKeyValues = new Dictionary<string, MenuItem>();
                foreach (var item in PART_RemoteTree.ContextMenu.Items)
                {
                    MenuItem menuItem = item as MenuItem;
                    if (menuItem != null)
                    {
                        menuItem.IsEnabled = true;//将所有右键菜单启用
                        menuItemKeyValues.Add(menuItem.Name, menuItem);
                    }
                }

                if (element != null)
                {
                    element.IsSelected = true;
                    if (element.RemoteType == RemoteType.dir)
                    {
                        menuItemKeyValues["MenuItem_Link"].IsEnabled = false;
                        menuItemKeyValues["MenuItem_LinkBackend"].IsEnabled = false;
                        menuItemKeyValues["MenuItem_LinkSeparate"].IsEnabled = false;
                    }
                }
                else
                {
                    TreeViewItem item = PART_RemoteTree.SelectedItem as TreeViewItem;
                    if (item != null) item.IsSelected = false;
                    menuItemKeyValues["MenuItem_Link"].IsEnabled = false;
                    menuItemKeyValues["MenuItem_LinkBackend"].IsEnabled = false;
                    menuItemKeyValues["MenuItem_LinkSeparate"].IsEnabled = false;
                    menuItemKeyValues["MenuItem_Rename"].IsEnabled = false;
                    menuItemKeyValues["MenuItem_Delete"].IsEnabled = false;
                }
            }
        }

        private void NewRemoteItem(RemoteTreeViewItem selectItem, RemoteType type, string name = null)
        {
            if (name == null)
            {
                if (type == RemoteType.dir) name = "新建目录";
                if (type == RemoteType.rdp) name = "新建RDP连接";
                if (type == RemoteType.ssh) name = "新建SSH连接";
                if (type == RemoteType.telnet) name = "新建Telnet连接";
            }

            var treeViewItem = RemoteItems.Insert(type, selectItem, name);
            treeViewItem.UpdateLayout();
            treeViewItem.HeaderEdit(Home_Tree_EditHeaderClosing);
        }
        private async void DeleteRemoteItem(RemoteTreeViewItem selectItem)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                var result = await mainWindow.ShowMessageAsync("提示", $"是否要删除 “{selectItem.Header}”", MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Affirmative)
                {
                    RemoteItems.Delete(selectItem);
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuItem = sender as MenuItem;
                if (menuItem == null) return;
                RemoteTreeViewItem selectItem = (RemoteTreeViewItem)PART_RemoteTree.SelectedItem;
                if (menuItem.Name == "MenuItem_NewDir") NewRemoteItem(selectItem, RemoteType.dir);
                else if (menuItem.Name == "MenuItem_Newrdp") NewRemoteItem(selectItem, RemoteType.rdp);
                else if (menuItem.Name == "MenuItem_Newssh") NewRemoteItem(selectItem, RemoteType.ssh);
                else if (menuItem.Name == "MenuItem_NewTelnet") NewRemoteItem(selectItem, RemoteType.telnet);
                else if (menuItem.Name == "MenuItem_Delete") DeleteRemoteItem(selectItem);
                else if (menuItem.Name == "MenuItem_Link") RemoteItems.Open(selectItem, DbItemSetting.OPEN_TAB);
                else if (menuItem.Name == "MenuItem_LinkBackend") RemoteItems.Open(selectItem, DbItemSetting.OPEN_TAB_BACKSTAGE);
                else if (menuItem.Name == "MenuItem_LinkSeparate") RemoteItems.Open(selectItem, DbItemSetting.OPEN_WINDOW);
                else if (menuItem.Name == "MenuItem_Rename") selectItem.HeaderEdit(Home_Tree_EditHeaderClosing);
                else if (menuItem.Name == "MenuItem_Shear") RemoteItems.Shear(selectItem);
                else if (menuItem.Name == "MenuItem_Copy") RemoteItems.Copy(selectItem);
                else if (menuItem.Name == "MenuItem_Paste") RemoteItems.Paste(selectItem);
            }
            catch (Exception ex)
            {
                MainWindow.ShowMessageDialog("错误", ex.Message);
            }
        }

        private void Home_Tree_EditHeaderClosing(object sender, CancelEditEventArgs e)
        {
            e.Cancel = true;
            RemoteTreeViewItem treeItem = sender as RemoteTreeViewItem;
            if (treeItem == null) return;

            if (string.IsNullOrEmpty(e.NewValue))
            {
                MainWindow.ShowNoticeDialog("提示", "名称不能为空");
                return;
            }

            if (treeItem.RemoteType == RemoteType.dir)
            {
                RemoteItems.GetItemDirectory(treeItem.uuid);
                RemoteItems.ItemDirectory.Name = e.NewValue;
                RemoteItems.UpdateItemDirectory();
            }
            else if (RemoteItems.ItemRemoteLink != null && RemoteItems.ItemRemoteLink.Id == treeItem.uuid)
            {
                RemoteItems.ItemRemoteLink.Name = e.NewValue;
                RemoteItems.UpdateItemRemoteLink();
                TextBox_Name.Text = e.NewValue;
            }

            RemoteItems.SetItemName(treeItem, e.NewValue);
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                RemoteTreeViewItem selectItem = (RemoteTreeViewItem)e.NewValue;

                if (selectItem.RemoteType != RemoteType.dir)
                {
                    ScrollViewer.Visibility = Visibility.Visible;
                    RemoteItems.GetItemRemoteLink(selectItem.uuid);
                    ScrollViewer.DataContext = RemoteItems.ItemRemoteLink;
                    TextBox_Password.Password = RemoteItems.ItemRemoteLink.Password;
                    TextBox_Name.Text = RemoteItems.ItemRemoteLink.Name;
                    TextBox_Password.IsEnabled = true;

                    _currentSetControl?.UnLoaded();
                    Grid_PrivateKey.Visibility = Visibility.Collapsed;
                    if (selectItem.RemoteType == RemoteType.rdp)
                    {
                        _currentSetControl = PART_SettingRdp;
                    }
                    if (selectItem.RemoteType == RemoteType.ssh)
                    {
                        //加载私钥
                        if (RemoteItems.ItemRemoteLink.PrivateKey == null)
                            CheckBox_PrivateKey.IsChecked = false;
                        else
                            CheckBox_PrivateKey.IsChecked = true;
                        CheckBox_PrivateKey_Click(null, null);
                        _currentSetControl = PART_SettingSsh;
                        Grid_PrivateKey.Visibility = Visibility.Visible;
                    }
                    if (selectItem.RemoteType == RemoteType.telnet)
                    {
                        _currentSetControl = PART_SettingTelnet;
                    }
                    Expander_Setting.Content = _currentSetControl;
                    _currentSetControl?.Loaded(RemoteItems.ItemRemoteLink);
                }
                else ScrollViewer.Visibility = Visibility.Collapsed;
            }
        }

        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Point point = e.GetPosition(PART_RemoteTree);
                RemoteTreeViewItem element = PART_RemoteTree.GetElementFromPoint<RemoteTreeViewItem>(point);
                if (element != null)
                {
                    if (element.IsHeaderEdit == false && element.RemoteType != RemoteType.dir)
                    {
                        RemoteItems.Open((RemoteTreeViewItem)PART_RemoteTree.SelectedItem, DbItemSetting.OPEN_DEFAULT);
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
            var selectItem = PART_RemoteTree.SelectedItem as RemoteTreeViewItem;
            if (selectItem != null)
            {
                if (e.Key == Key.F2)
                {
                    selectItem.HeaderEdit(Home_Tree_EditHeaderClosing);
                }
                if (e.Key == Key.Delete)
                {
                    DeleteRemoteItem(selectItem);
                }
                if (e.Key == Key.Enter)
                {
                    if (selectItem == null) return;
                    if (selectItem.RemoteType == RemoteType.dir)
                    {
                        selectItem.IsExpanded = !selectItem.IsExpanded;
                    }
                    else
                    {
                        RemoteItems.Open((RemoteTreeViewItem)PART_RemoteTree.SelectedItem, DbItemSetting.OPEN_DEFAULT);
                    }
                }
            }
            //Ctrl
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.C) RemoteItems.Copy(selectItem);
                if (e.Key == Key.X) RemoteItems.Shear(selectItem);
                if (e.Key == Key.V) RemoteItems.Paste(selectItem);
            }
            //Ctrl+Shift
            if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                if (e.Key == Key.N) NewRemoteItem(selectItem, RemoteType.dir);
                if (e.Key == Key.R) NewRemoteItem(selectItem, RemoteType.rdp);
                if (e.Key == Key.S) NewRemoteItem(selectItem, RemoteType.ssh);
                if (e.Key == Key.T) NewRemoteItem(selectItem, RemoteType.telnet);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender == TextBox_Name)
            {
                if (string.IsNullOrEmpty(TextBox_Name.Text))
                {
                    MainWindow.ShowNoticeDialog("提示", "名称不能为空。");
                    TextBox_Name.Text = RemoteItems.ItemRemoteLink.Name;
                    return;
                }
                else
                {
                    RemoteItems.ItemRemoteLink.Name = TextBox_Name.Text;
                    if (PART_RemoteTree.SelectedItem != null)
                    {
                        RemoteItems.UpdateItemRemoteLink();
                        RemoteItems.SetItemName((RemoteTreeViewItem)PART_RemoteTree.SelectedItem, RemoteItems.ItemRemoteLink.Name);
                        return;
                    }
                }
            }

            if (sender == TextBox_Password)
            {
                RemoteItems.ItemRemoteLink.Password = TextBox_Password.Password;
            }

            RemoteItems.UpdateItemRemoteLink();
        }

        private void MyExpander_IsExpanded(object sender, RoutedEventArgs e)
        {
            RemoteItems.UpdateItemRemoteLink();
        }

        public void Save()
        {
            UserSettings.HomeRemoteTreeView_Width = Grid_Main.ColumnDefinitions[0].Width;
            RemoteItems.UpdateExpandTreeItem();
        }

        private void GetExpandTreeItem(ref List<string> list, ItemCollection items)
        {
            foreach (RemoteTreeViewItem item in items)
            {
                if (item.Items.Count > 0)
                {
                    GetExpandTreeItem(ref list, item.Items);
                }
                if(item.IsExpanded) list.Add(item.uuid);
            }
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
                    RemoteItems.ItemRemoteLink.PrivateKey = File.ReadAllText(op.FileName);
                    TextBox_PrivateKey.Text = RemoteItems.ItemRemoteLink.PrivateKey;
                    RemoteItems.UpdateItemRemoteLink();
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
                RemoteItems.ItemRemoteLink.Password = null;
                if (RemoteItems.ItemRemoteLink.PrivateKey == null)
                {
                    RemoteItems.ItemRemoteLink.PrivateKey = "";
                }
                TextBox_PrivateKey.Text = RemoteItems.ItemRemoteLink.PrivateKey;
            }
            else
            {
                TextBox_PrivateKey.Visibility = Visibility.Collapsed;
                TextBox_Password.IsEnabled = true;
                RemoteItems.ItemRemoteLink.PrivateKey = null;
            }
            if (sender != null)
            {
                RemoteItems.UpdateItemRemoteLink();
            }
        }

        private void SelectionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //RemoteItems.Screening(Text_Screen.Text);
        }

        private void Text_Screen_TextChanged(object sender, TextChangedEventArgs e)
        {
            RemoteItems.Screening(Text_Screen.Text);
        }
    }
}