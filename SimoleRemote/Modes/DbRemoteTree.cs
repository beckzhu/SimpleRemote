using System;
using System.Collections.Generic;

namespace SimpleRemote.Modes
{
    public class DbRemoteTree
    {
        public DbRemoteTree(string uuid, string name, RemoteType type)
        {
            this.uuid = uuid;
            this.Name = name;
            this.Type = type;
            Dirs = new List<DbRemoteTree>();
            Items = new List<DbRemoteTree>();
        }

        public string uuid { get; set; }
        public string Name { get; set; }
        public RemoteType Type { get; set; }
        public List<DbRemoteTree> Dirs { get; set; }
        public List<DbRemoteTree> Items { get; set; }

        /// <summary>
        /// 对所有元素进行排序
        /// </summary>
        public void Sort()
        {
            foreach (var item in Dirs)
            {
                if (item.Dirs.Count > 0 || item.Items.Count > 0) item.Sort();
            }
            Dirs.Sort((x, y) => string.Compare(x.Name, y.Name));
            Items.Sort((x, y) => string.Compare(x.Name, y.Name));
        }

        public List<DbRemoteTree> Screening(string text)
        {
            List<DbRemoteTree> list = new List<DbRemoteTree>();
            foreach (var item in Dirs)
            {
                if (item.Dirs.Count > 0 || item.Items.Count > 0)
                {
                    list.AddRange(item.Screening(text));
                }
            }
            list.AddRange(Items.FindAll(m => m.Name.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0));
            return list;
        }

        public void Remove(DbRemoteTree remoteTree)
        {
            if (remoteTree.Type == RemoteType.dir) Dirs.Remove(remoteTree);
            else Items.Remove(remoteTree);
        }

        public int Add(DbRemoteTree remoteTree)
        {
            var items = remoteTree.Type == RemoteType.dir ? Dirs : Items;
            items.Add(remoteTree);
            if (remoteTree.Type == RemoteType.dir) return items.Count - 1;
            return Dirs.Count + items.Count - 1;
        }

        public int Insert(DbRemoteTree remoteTree)
        {
            var items = remoteTree.Type == RemoteType.dir ? Dirs : Items;
            items.Add(remoteTree);
            items.Sort((x, y) => string.Compare(x.Name, y.Name));
            int index = items.FindIndex(m => m.uuid == remoteTree.uuid);
            if (remoteTree.Type == RemoteType.dir) return index;
            return index + Dirs.Count;
        }

        public int GetSortIndex(DbRemoteTree remoteTree)
        {
            var items = remoteTree.Type == RemoteType.dir ? Dirs : Items;
            items.Sort((x, y) => string.Compare(x.Name, y.Name));
            int index = items.FindIndex(m => m.uuid == remoteTree.uuid);
            if (remoteTree.Type == RemoteType.dir) return index;
            return index + Dirs.Count;
        }
    }
}