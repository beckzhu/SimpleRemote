using System;
using System.Collections.Generic;
using SimpleRemote.Modes;
using System.Windows;
using System.Windows.Controls;
using SimpleRemote.Container;
using SimpleRemote.Control;
using System.Diagnostics;

namespace SimpleRemote.Bll
{
    /// <summary>对远程桌面的条目进行操作。</summary>
    public static class RemoteItems
    {
        struct RemoteRunTime
        {
            public RemoteRunTime(int openMode, IRemoteControl remoteControl)
            {
                OpenMode = openMode;
                RemoteControl = remoteControl;
            }
            public int OpenMode;
            public IRemoteControl RemoteControl;
        }

        private static TreeView _remoteTreeView;
        private static Dictionary<string, RemoteRunTime> _remoteRunTime;

        /// <summary>加载远程桌面的条目</summary>
        public static void LoadItems(TreeView treeView)
        {
            _remoteTreeView = treeView;
            _remoteRunTime = new Dictionary<string, RemoteRunTime>();

            Dictionary<string, RemoteTreeViewItem> keyValues = new Dictionary<string, RemoteTreeViewItem>();
            foreach (DbItemDirectory item in Database.EnumDirectory())
            {
                RemoteTreeViewItem treeViewItem = new RemoteTreeViewItem(item.Name, RemoteType.dir, item.Id);
                if (string.IsNullOrEmpty(item.ParentId)) _remoteTreeView.Items.Add(treeViewItem);
                else keyValues[item.ParentId].Items.Add(treeViewItem);
                keyValues.Add(item.Id, treeViewItem);
            }
            foreach (DbItemRemoteLink item in Database.EnumRemoteLink())
            {
                RemoteTreeViewItem treeViewItem = new RemoteTreeViewItem(item.Name, (RemoteType)(item.Type), item.Id);
                if (string.IsNullOrEmpty(item.ParentId)) _remoteTreeView.Items.Add(treeViewItem);
                else keyValues[item.ParentId].Items.Add(treeViewItem);
                keyValues.Add(item.Id, treeViewItem);
            }
        }
        /// <summary>添加条目,treeViewItem=选中的节点,会自动处理父子逻辑</summary>
        public static RemoteTreeViewItem Insert(RemoteType type, RemoteTreeViewItem selectItem, string name)
        {
            ItemCollection itemCollection;
            if (selectItem == null) itemCollection = _remoteTreeView.Items;
            else if (selectItem.RemoteType == RemoteType.dir) itemCollection = selectItem.Items;
            else itemCollection = ((ItemsControl)selectItem.Parent).Items;

            //计算文件夹在前端的合适插入位置
            int index = 0;
            if (type == RemoteType.dir)
            {
                foreach (RemoteTreeViewItem item in itemCollection)
                {
                    if (item.RemoteType != RemoteType.dir) break;
                    index += 1;
                }
            }
            else
            {
                index = itemCollection.Count;
            }
            //计算项目的父Id
            string uuid = null;
            if (selectItem != null)
            {
                if (selectItem?.RemoteType == RemoteType.dir) uuid = selectItem.uuid;
                else
                {
                    RemoteTreeViewItem viewItem = selectItem.Parent as RemoteTreeViewItem;
                    if (viewItem != null) uuid = viewItem.uuid;
                }
            }
            string id = Database.InsertRemoteItem(type, uuid, name);
            RemoteTreeViewItem treeViewItem = new RemoteTreeViewItem(name, type, id);
            itemCollection.Insert(index, treeViewItem);

            return treeViewItem;
        }
        /// <summary>删除一条远程桌面的记录</summary>
        public static void Delete(RemoteTreeViewItem selectItem)
        {
            int i;
            Database.DeleteRemoteItem(selectItem.RemoteType, selectItem.uuid);
            ItemCollection itemCollection = selectItem.Parent is TreeView ? _remoteTreeView.Items : ((TreeViewItem)selectItem.Parent).Items;

            i = itemCollection.IndexOf(selectItem);
            itemCollection.Remove(selectItem);
            if (i >= itemCollection.Count) i -= 1;
            if (i >= 0)
            {
                RemoteTreeViewItem treeViewItem = (RemoteTreeViewItem)itemCollection[i];
                treeViewItem.IsSelected = true;
                treeViewItem.Focus();
            }
        }
        /// <summary>打开远程桌面</summary>
        public static void OpenRemote(RemoteTreeViewItem treeTtem, int openMode)
        {
            IRemoteControl remoteControl = null;
            DbItemRemoteLink itemRemoteLink = Database.GetRemoteLink(treeTtem.uuid);
            if (string.IsNullOrEmpty(itemRemoteLink.Server))
            {
                throw new Exception("服务器地址不能为空。");
            }

            DbItemSetting itemSetting = Database.GetRemoteSetting(itemRemoteLink);
            if (openMode == DbItemSetting.OPEN_DEFAULT) openMode = new FinalItemSetting(itemSetting).OpenMode;

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
            _remoteRunTime.Add(treeTtem.uuid, new RemoteRunTime(openMode, remoteControl));
            remoteControl.OnRemove += (sender, e) => _remoteRunTime.Remove(treeTtem.uuid);
        }
    }
}
