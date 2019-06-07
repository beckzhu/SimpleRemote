using SimpleRemote.Container;
using SimpleRemote.Controls;
using SimpleRemote.Modes;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SimpleRemote.Core
{
    /// <summary>
    /// 对远程桌面的条目进行操作。
    /// </summary>
    public static class RemoteItems
    {
        private struct RemoteRunTime
        {
            public RemoteRunTime(int openMode, IRemoteControl remoteControl)
            {
                OpenMode = openMode;
                RemoteControl = remoteControl;
            }

            public int OpenMode;
            public IRemoteControl RemoteControl;
        }

        private struct RemoteClipboard
        {
            public RemoteTreeViewItem RemoteTreeViewItem;
            public DbRemoteTree RemoteTree { get => RemoteTreeViewItem?.RemoteTree; }
            public bool IsShear;//剪切
        }

        private static DbRemoteTree _remoteTree;
        private static TreeView _remoteTreeView;
        private static Dictionary<string, RemoteRunTime> _remoteRunTime;//控制只能运行一个选项卡
        private static bool _isSelection;
        private static RemoteClipboard _remoteClipboard;

        private static void LoadItems(ItemCollection parent, DbRemoteTree tree)
        {
            //加载目录
            foreach (var item in tree.Dirs)
            {
                RemoteTreeViewItem treeItem = new RemoteTreeViewItem(item);
                if (UserSettings.RemoteTreeExpand.Contains(item.uuid))
                {
                    treeItem.IsExpanded = true;
                }
                parent.Add(treeItem);
                if (item.Items.Count > 0 || item.Dirs.Count > 0)
                {
                    LoadItems(treeItem.Items, item);
                }
            }
            //加载条目
            foreach (var item in tree.Items)
            {
                RemoteTreeViewItem treeItem = new RemoteTreeViewItem(item);
                parent.Add(treeItem);
                if (item.Items.Count > 0)
                {
                    LoadItems(treeItem.Items, item);
                }
            }
        }

        /// <summary>
        /// 加载远程桌面的条目
        /// </summary>
        public static void LoadTree(TreeView treeView)
        {
            _remoteTreeView = treeView;
            _remoteRunTime = new Dictionary<string, RemoteRunTime>();

            _remoteTree = DatabaseServices.GetRemoteTree();
            _remoteTree.Sort();
            LoadItems(_remoteTreeView.Items, _remoteTree);
        }

        private static RemoteTreeViewItem GetInsertParentItem(RemoteTreeViewItem selectItem, out DbRemoteTree parentTree, out ItemCollection items)
        {
            RemoteTreeViewItem parentItem = null;
            if (selectItem != null)
            {
                if (selectItem.RemoteType == RemoteType.dir) parentItem = selectItem;
                else
                {
                    parentItem = selectItem.Parent is RemoteTreeViewItem ? (RemoteTreeViewItem)selectItem.Parent : null;
                }
            }
            parentTree = parentItem == null ? _remoteTree : parentItem.RemoteTree;
            items = parentItem == null ? _remoteTreeView.Items : parentItem.Items;
            return parentItem;
        }

        private static RemoteTreeViewItem GetParentItem(RemoteTreeViewItem item, out DbRemoteTree parentTree, out ItemCollection items)
        {
            RemoteTreeViewItem parentItem = item.Parent is RemoteTreeViewItem ? (RemoteTreeViewItem)item.Parent : null;
            parentTree = parentItem == null ? _remoteTree : parentItem.RemoteTree;
            items = parentItem == null ? _remoteTreeView.Items : parentItem.Items;
            return parentItem;
        }

        /// <summary>
        /// 添加条目,treeViewItem=选中的节点,会自动处理父子逻辑
        /// </summary>
        public static RemoteTreeViewItem Insert(RemoteType type, RemoteTreeViewItem selectItem, string name, bool isSort = false)
        {
            RemoteTreeViewItem parentItem = GetInsertParentItem(selectItem, out var parentTree, out var items);

            string uuid = DatabaseServices.Insert(type, parentTree.uuid, name);
            if (string.IsNullOrEmpty(uuid)) return null;

            DbRemoteTree remoteTree = new DbRemoteTree(uuid, name, type);
            int index = isSort ? parentTree.Insert(remoteTree) : parentTree.Add(remoteTree);

            RemoteTreeViewItem treeViewItem = new RemoteTreeViewItem(remoteTree);
            items.Insert(index, treeViewItem);

            return treeViewItem;
        }

        private static RemoteTreeViewItem Insert(RemoteTreeViewItem selectItem, DbItemRemoteLink itemRemoteLink, DbItemSetting itemSetting, bool isSort = false)
        {
            RemoteType type = (RemoteType)itemRemoteLink.Type;
            RemoteTreeViewItem parentItem = GetInsertParentItem(selectItem, out var parentTree, out var items);

            itemRemoteLink.ParentId = parentTree.uuid;
            string uuid = DatabaseServices.Insert(itemRemoteLink, itemSetting);
            if (string.IsNullOrEmpty(uuid)) return null;

            DbRemoteTree remoteTree = new DbRemoteTree(uuid, itemRemoteLink.Name, type);
            int index = isSort ? parentTree.Insert(remoteTree) : parentTree.Add(remoteTree);

            RemoteTreeViewItem treeViewItem = new RemoteTreeViewItem(remoteTree);
            items.Insert(index, treeViewItem);

            return treeViewItem;
        }

        /// <summary>
        /// 移动一个远程条目到其他文件夹
        /// </summary>
        public static bool Move(RemoteTreeViewItem src, RemoteTreeViewItem dest)
        {
            RemoteTreeViewItem parentItem = GetInsertParentItem(dest, out var destParentTree, out var items);
            RemoteTreeViewItem srcParentItem = GetParentItem(src, out DbRemoteTree srcParentTree, out ItemCollection srcitems);

            var parentid = parentItem?.uuid;
            if (!DatabaseServices.Move(src.uuid, parentid)) return false;

            srcParentTree.Remove(src.RemoteTree);
            int index = destParentTree.Insert(src.RemoteTree);

            srcitems.Remove(src);
            items.Insert(index, src);

            if (ItemRemoteLink != null) ItemRemoteLink.ParentId = parentid;
            return true;
        }

        /// <summary>
        /// 删除一条远程桌面的记录
        /// </summary>
        public static void Delete(RemoteTreeViewItem item)
        {
            RemoteTreeViewItem parentItem = GetParentItem(item, out DbRemoteTree parentTree, out ItemCollection items);
            DatabaseServices.Delete(item.RemoteType, item.uuid);

            //删除条目对应的RemoteTree
            parentTree.Remove(item.RemoteTree);

            int selectIndex = items.IndexOf(item);
            items.Remove(item);
            //设置默认选择的条目
            if (selectIndex >= items.Count) selectIndex -= 1;
            if (selectIndex >= 0)
            {
                RemoteTreeViewItem treeViewItem = (RemoteTreeViewItem)items[selectIndex];
                treeViewItem.IsSelected = true;
                treeViewItem.Focus();
            }
        }

        /// <summary>
        /// 设置指定远程条目的名称
        /// </summary>
        public static void SetItemName(RemoteTreeViewItem item, string name)
        {
            RemoteTreeViewItem parentItem = GetParentItem(item, out DbRemoteTree parentTree, out ItemCollection items);
            item.Header = name;
            item.RemoteTree.Name = name;

            int index = parentTree.GetSortIndex(item.RemoteTree);
            items.Remove(item);
            items.Insert(index, item);
            item.IsSelected = true;
        }

        /// <summary>
        /// 打开远程桌面
        /// </summary>
        public static void Open(RemoteTreeViewItem treeTtem, int openMode)
        {
            if (treeTtem.RemoteType == RemoteType.dir) return;
            IRemoteControl remoteControl = null;
            DbItemRemoteLink itemRemoteLink = DatabaseServices.GetRemoteLink(treeTtem.uuid);
            if (string.IsNullOrEmpty(itemRemoteLink.Server))
            {
                throw new Exception("服务器地址不能为空。");
            }

            DbItemSetting itemSetting = DatabaseServices.GetRemoteSetting(itemRemoteLink);
            if (openMode == DbItemSetting.OPEN_DEFAULT) openMode = itemSetting.DefaultSetting.GetOpenMode();

            //如果指定的远程桌面有在后台运行,则跳转
            if (_remoteRunTime.ContainsKey(treeTtem.uuid))
            {
                var value = _remoteRunTime[treeTtem.uuid];
                if (value.OpenMode == openMode)
                {
                    value.RemoteControl.Jump();
                    return;
                }
                value.RemoteControl.Remove();
                _remoteRunTime.Remove(treeTtem.uuid);
            }

            //开始连接远程桌面
            if (openMode == DbItemSetting.OPEN_WINDOW) remoteControl = new RemoteWinControl();
            else remoteControl = new RemoteTabControl();
            remoteControl.Open(itemRemoteLink, itemSetting, openMode == DbItemSetting.OPEN_TAB);
            if (treeTtem.RemoteType == RemoteType.rdp)
            {
                _remoteRunTime.Add(treeTtem.uuid, new RemoteRunTime(openMode, remoteControl));
                remoteControl.OnRemove += (sender, e) => _remoteRunTime.Remove(treeTtem.uuid);
            }
        }

        /// <summary>
        /// 筛选远程条目
        /// </summary>
        public static void Screening(string text)
        {
            if (string.IsNullOrEmpty(text) && _isSelection)
            {
                _remoteTreeView.Items.Clear();
                LoadItems(_remoteTreeView.Items, _remoteTree);
                _isSelection = false;
            }
            else
            {
                if (!_isSelection)
                {
                    _isSelection = true;
                    UpdateExpandTreeItem();
                }

                var list = _remoteTree.Screening(text);
                _remoteTreeView.Items.Clear();
                foreach (var item in list)
                {
                    RemoteTreeViewItem treeItem = new RemoteTreeViewItem(item);
                    _remoteTreeView.Items.Add(treeItem);
                }
            }
        }

        /// <summary>
        /// 获取远程条目列表展开的目录，更新到用户设置
        /// </summary>
        /// <param name="items"></param>
        public static void UpdateExpandTreeItem(ItemCollection items=null)
        {
            if (items == null)
            {
                UserSettings.RemoteTreeExpand.Clear();
                items = _remoteTreeView.Items;
            }
            foreach (RemoteTreeViewItem item in items)
            {
                if (item.Items.Count > 0)
                {
                    UpdateExpandTreeItem(item.Items);
                }
                if (item.IsExpanded) UserSettings.RemoteTreeExpand.Add(item.uuid);
            }
        }

        /// <summary>
        /// 复制
        /// </summary>
        public static void Copy(RemoteTreeViewItem treeTtem)
        {
            _remoteClipboard.RemoteTreeViewItem = treeTtem;
            _remoteClipboard.IsShear = false;
        }

        /// <summary>
        /// 剪切
        /// </summary>
        public static void Shear(RemoteTreeViewItem treeTtem)
        {
            _remoteClipboard.RemoteTreeViewItem = treeTtem;
            _remoteClipboard.IsShear = true;
        }

        /// <summary>
        /// 粘贴
        /// </summary>
        public static void Paste(RemoteTreeViewItem selectItem)
        {
            if (_remoteClipboard.RemoteTree != null)
            {
                selectItem = selectItem.uuid == _remoteClipboard.RemoteTree.uuid ? selectItem.Parent as RemoteTreeViewItem : selectItem;

                if (_remoteClipboard.RemoteTree.Type == RemoteType.dir)
                {
                    DbItemDirectory itemDirectory = DatabaseServices.GetItemDirectory(_remoteClipboard.RemoteTree.uuid);
                    if (itemDirectory == null) return;
                    RemoteTreeViewItem treeItem = Insert(RemoteType.dir, selectItem, _remoteClipboard.RemoteTree.Name, true);
                    Paste(treeItem, _remoteClipboard.RemoteTree);
                }
                else
                {
                    DbItemRemoteLink itemRemoteLink = DatabaseServices.GetRemoteLink(_remoteClipboard.RemoteTree.uuid);
                    if (itemRemoteLink == null) return;
                    Insert(selectItem, itemRemoteLink, DatabaseServices.GetRemoteSetting(itemRemoteLink), true);
                }

                if (_remoteClipboard.IsShear)
                {
                    Delete(_remoteClipboard.RemoteTreeViewItem);
                    _remoteClipboard.IsShear = false;
                }
            }
        }

        /// <summary>
        /// 粘贴,处理文件夹的粘贴逻辑
        /// </summary>
        private static void Paste(RemoteTreeViewItem parent, DbRemoteTree tree)
        {
            //加载目录
            foreach (var item in tree.Dirs)
            {
                RemoteTreeViewItem treeItem = Insert(RemoteType.dir, parent, item.Name);
                if (item.Items.Count > 0) Paste(treeItem, item);
            }
            //加载条目
            foreach (var item in tree.Items)
            {
                DbItemRemoteLink itemRemoteLink = DatabaseServices.GetRemoteLink(item.uuid);
                Insert(parent, itemRemoteLink, DatabaseServices.GetRemoteSetting(itemRemoteLink));
            }
        }

        public static DbItemRemoteLink ItemRemoteLink { get; set; }

        /// <summary>
        /// 获取指定的远程连接信息  保存到ItemRemoteLink 属性
        /// </summary>
        public static void GetItemRemoteLink(string uuid)
        {
            ItemRemoteLink = DatabaseServices.GetRemoteLink(uuid);
        }

        /// <summary>
        /// 将ItemRemoteLink 更新到数据库
        /// </summary>
        public static bool UpdateItemRemoteLink()
        {
            if (ItemRemoteLink == null) return false;
            return DatabaseServices.Update(ItemRemoteLink.Id, ItemRemoteLink);
        }

        public static DbItemDirectory ItemDirectory { get; set; }

        /// <summary>
        /// 获取指定的目录信息  保存到ItemDirectory 属性
        /// </summary>
        public static void GetItemDirectory(string uuid)
        {
            ItemDirectory = DatabaseServices.GeyDirectory(uuid);
        }

        /// <summary>
        /// 将ItemDirectory 更新到数据库
        /// </summary>
        public static bool UpdateItemDirectory()
        {
            if (ItemDirectory == null) return false;
            return DatabaseServices.Update(ItemDirectory.Id, ItemDirectory);
        }
    }
}