using LiteDB;
using SimpleRemote.Modes;
using SimpleRemote.Properties;
using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleRemote.Core
{
    public static class DatabaseServices
    {
        private static LiteDatabase _litedb;
        private static string _dataFile;
        private static LiteCollection<DbItemDirectory> _tableDirectory;
        private static LiteCollection<DbItemRemoteLink> _tableRemoteLink;
        private static LiteCollection<DbItemSettingRdp> _tableSetting_rdp;
        private static LiteCollection<DbItemSettingSsh> _tableSetting_ssh;
        private static LiteCollection<DbItemSettingTelnet> _tableSetting_telnet;
        private static LiteCollection<DbSshHostKeys> _tableSshHostKeys;
        private static LiteCollection<DbPuttyColor> _tablePuttyColor;

        /// <summary>
        /// 判断指定的数据库密码是否正确
        /// </summary>
        public static bool IsPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) password = "08817334c6451b95b0097de95d0bb953";
            else password = CommonServices.GetStringMd5(password);
            try
            {
                LiteEngine liteEngine = new LiteEngine(_dataFile, password);
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// 更改密码
        /// </summary>
        public static void ChangePassword(string password)
        {
            if (string.IsNullOrEmpty(password)) password = "08817334c6451b95b0097de95d0bb953";
            else password = CommonServices.GetStringMd5(password);
            _litedb.Shrink(password);
        }

        /// <summary>
        /// 用指定的密码打开LiteDb数据库文件，如果不成功将返回Flase
        /// </summary>
        public static bool Open(string password)
        {
            try
            {
                _dataFile = $"{UserSettings.AppDataDir}\\RemoteItems.db";
                if (!File.Exists(_dataFile))//新建数据库文件
                {
                    File.WriteAllBytes(_dataFile, Resources.RemoteItems);
                }
                if (string.IsNullOrEmpty(password)) password = "08817334c6451b95b0097de95d0bb953";
                else password = CommonServices.GetStringMd5(password);
                //验证密码是否正确
                try { LiteEngine liteEngine = new LiteEngine(_dataFile, password); }
                catch { return false; };

                _litedb = new LiteDatabase($"Filename={_dataFile};Password={password}");
                _litedb.Mapper.EmptyStringToNull = false;
                _litedb.Mapper.TrimWhitespace = false;

                _tableDirectory = _litedb.GetCollection<DbItemDirectory>("Directory");
                _tableRemoteLink = _litedb.GetCollection<DbItemRemoteLink>("RemoteLink");
                _tableSetting_rdp = _litedb.GetCollection<DbItemSettingRdp>("Setting_rdp");
                _tableSetting_ssh = _litedb.GetCollection<DbItemSettingSsh>("Setting_ssh");
                _tableSetting_telnet = _litedb.GetCollection<DbItemSettingTelnet>("Setting_telnet");
                _tableSshHostKeys = _litedb.GetCollection<DbSshHostKeys>("SshHostKeys");
                _tablePuttyColor = _litedb.GetCollection<DbPuttyColor>("PuttyColor");

                #region 数据库升级

                if (_litedb.Engine.UserVersion < 1)
                {
                    //将原来的默认设置id="0001" 改成"default"
                    foreach (var item in _tableSetting_rdp.Find(m => m.Id == "0001"))
                    {
                        _tableSetting_rdp.Delete("0001");
                        _tableSetting_rdp.Insert("default", item);
                    }
                    foreach (var item in _tableSetting_ssh.Find(m => m.Id == "0001"))
                    {
                        _tableSetting_ssh.Delete("0001");
                        _tableSetting_ssh.Insert("default", item);
                    }
                    foreach (var item in _tableSetting_telnet.Find(m => m.Id == "0001"))
                    {
                        _tableSetting_telnet.Delete("0001");
                        _tableSetting_telnet.Insert("default", item);
                    }
                    _litedb.Engine.UserVersion = 1;
                }

                #endregion 数据库升级

                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"打开数据库文件失败。\n原因：{e.Message}");
            }
        }

        /// <summary>
        /// 读取数据库，生成一个树结构，包含所有的节点
        /// </summary>
        /// <returns></returns>
        public static DbRemoteTree GetRemoteTree()
        {
            DbRemoteTree tree = new DbRemoteTree(string.Empty, string.Empty, RemoteType.dir);
            Dictionary<string, DbRemoteTree> keyValues = new Dictionary<string, DbRemoteTree>();

            var Dirs = _tableDirectory.FindAll();
            foreach (DbItemDirectory item in Dirs)
            {
                DbRemoteTree treeItem = new DbRemoteTree(item.Id, item.Name, RemoteType.dir);
                keyValues.Add(item.Id, treeItem);
            }

            foreach (DbItemDirectory item in Dirs)
            {
                if (string.IsNullOrEmpty(item.ParentId))
                {
                    tree.Dirs.Add(keyValues[item.Id]);
                }
                else
                {
                    if (!keyValues.ContainsKey(item.ParentId)) continue;
                    keyValues[item.ParentId].Dirs.Add(keyValues[item.Id]);
                }
            }

            foreach (DbItemRemoteLink item in _tableRemoteLink.FindAll())
            {
                DbRemoteTree treeItem = new DbRemoteTree(item.Id, item.Name, (RemoteType)(item.Type));
                if (string.IsNullOrEmpty(item.ParentId))
                {
                    tree.Items.Add(treeItem);
                }
                else
                {
                    if (!keyValues.ContainsKey(item.ParentId)) continue;
                    keyValues[item.ParentId].Items.Add(treeItem);
                }
                keyValues.Add(item.Id, treeItem);
            }
            return tree;
        }

        /// <summary>
        /// 插入远程连接条目,返回Id
        /// </summary>
        public static string Insert(RemoteType type, string parentId, string name)
        {
            if (type == RemoteType.dir)
            {
                return _tableDirectory.Insert(new DbItemDirectory { Id = ObjectId.NewObjectId().ToString(), ParentId = parentId, Name = name });
            }
            else
            {
                string userName = type == RemoteType.rdp ? "Administrator" : "root";
                string uuid = ObjectId.NewObjectId().ToString();
                if (type == RemoteType.rdp) _tableSetting_rdp.Insert(new DbItemSettingRdp { Id = uuid });
                if (type == RemoteType.ssh) _tableSetting_ssh.Insert(new DbItemSettingSsh { Id = uuid });
                if (type == RemoteType.telnet) _tableSetting_telnet.Insert(new DbItemSettingTelnet { Id = uuid });
                return _tableRemoteLink.Insert(new DbItemRemoteLink { Id = uuid, ParentId = parentId, Name = name, UserName = userName, Type = (int)type, IsExpander1 = true });
            }
        }

        public static string Insert(DbItemRemoteLink itemRemoteLink, DbItemSetting itemSetting)
        {
            itemRemoteLink.Id = ObjectId.NewObjectId().ToString();
            itemSetting.Id = itemRemoteLink.Id;

            if (itemRemoteLink.Type == (int)RemoteType.rdp)
            {
                _tableSetting_rdp.Insert((DbItemSettingRdp)itemSetting);
            }
            else if (itemRemoteLink.Type == (int)RemoteType.ssh)
            {
                _tableSetting_ssh.Insert((DbItemSettingSsh)itemSetting);
            }
            else if (itemRemoteLink.Type == (int)RemoteType.telnet)
            {
                _tableSetting_telnet.Insert((DbItemSettingTelnet)itemSetting);
            }
            else
            {
                return null;
            }

            return _tableRemoteLink.Insert(itemRemoteLink);
        }

        /// <summary>
        /// 移动一个远程条目,或文件夹 到其他文件夹
        /// </summary>
        public static bool Move(string id, string parentId)
        {
            var item = _tableRemoteLink.FindById(id);
            if (item != null)
            {
                item.ParentId = parentId;
                return _tableRemoteLink.Update(id, item);
            }
            var dir = _tableDirectory.FindById(id);
            if (dir != null)
            {
                dir.ParentId = parentId;
                return _tableDirectory.Update(id, dir);
            }
            return false;
        }

        /// <summary>
        /// 删除远程连接条目
        /// </summary>
        public static void Delete(RemoteType type, string id)
        {
            if (type == RemoteType.dir)
            {
                var dirItems = _tableDirectory.Find(m => m.ParentId == id);
                foreach (var item in dirItems)
                {
                    Delete(RemoteType.dir, item.Id);
                }
                var linkItems = _tableRemoteLink.Find(m => m.ParentId == id);
                foreach (var item in linkItems)
                {
                    Delete((RemoteType)item.Type, item.Id);
                }
                _tableDirectory.Delete(id);
            }
            if (_tableRemoteLink.Delete(id))
            {
                switch (type)
                {
                    case RemoteType.rdp:
                        _tableSetting_rdp.Delete(id);
                        break;

                    case RemoteType.ssh:
                        _tableSetting_ssh.Delete(id);
                        break;

                    case RemoteType.telnet:
                        break;
                }
            }
        }

        /// <summary>
        /// 获取远程桌面的连接信息,包含用户名和密码等.
        /// </summary>
        public static DbItemDirectory GetItemDirectory(string id)
        {
            return _tableDirectory.FindById(id);
        }

        /// <summary>
        /// 获取远程桌面的连接信息,包含用户名和密码等.
        /// </summary>
        public static DbItemRemoteLink GetRemoteLink(string id)
        {
            return _tableRemoteLink.FindById(id);
        }

        /// <summary>
        /// 获取远程桌面的设置信息,包含分辨率和音频重定向等.
        /// </summary>
        public static DbItemSetting GetRemoteSetting(DbItemRemoteLink itemRemoteLink)
        {
            if (itemRemoteLink.Type == (int)RemoteType.rdp) return _tableSetting_rdp.FindById(itemRemoteLink.Id);
            if (itemRemoteLink.Type == (int)RemoteType.ssh) return _tableSetting_ssh.FindById(itemRemoteLink.Id);
            if (itemRemoteLink.Type == (int)RemoteType.telnet) return _tableSetting_telnet.FindById(itemRemoteLink.Id);
            return null;
        }

        /// <summary>
        /// 获取全局设置.
        /// </summary>
        public static DbItemSetting GetDefaultSetting(RemoteType type)
        {
            if (type == RemoteType.rdp) return _tableSetting_rdp.FindById("default");
            if (type == RemoteType.ssh) return _tableSetting_ssh.FindById("default");
            if (type == RemoteType.telnet) return _tableSetting_telnet.FindById("default");
            return null;
        }

        /// <summary>
        /// 获取远程目录的信息.
        /// </summary>
        public static DbItemDirectory GeyDirectory(string id)
        {
            return _tableDirectory.FindById(id);
        }

        /// <summary>
        /// 获取指定ssh主机在本地保存的密钥
        /// </summary>
        public static byte[] GetSshHostKey(string id)
        {
            return _tableSshHostKeys.FindById(id)?.value;
        }

        /// <summary>
        /// 设置指定ssh主机在本地保存的密钥
        /// </summary>
        public static void SetSshHostKey(string id, byte[] value)
        {
            DbSshHostKeys dbSshHostKeys = _tableSshHostKeys.FindById(id);
            if (dbSshHostKeys == null)
            {
                dbSshHostKeys = new DbSshHostKeys { id = id, value = value };
                _tableSshHostKeys.Insert(dbSshHostKeys);
                return;
            }
            dbSshHostKeys.value = value;
            _tableSshHostKeys.Update(dbSshHostKeys);
        }

        /// <summary>
        /// 获取Putty所有配色名,配色名=id
        /// </summary>
        /// <returns></returns>
        public static string[] GetPuttyColorlNames()
        {
            string[] names = new string[_tablePuttyColor.Count() + 1];
            names[0] = "默认配色";
            int i = 1;
            foreach (var item in _tablePuttyColor.FindAll())
            {
                names[i] = item.id;
                i++;
            }
            return names;
        }

        /// <summary>
        /// 获取配色的相关信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DbPuttyColor GetPuttyColor(string name)
        {
            if (string.IsNullOrEmpty(name)) name = "默认配色";
            return _tablePuttyColor.FindById(name);
        }

        /// <summary>
        /// 更新指定的条目连接信息.
        /// </summary>
        public static bool Update(string id, DbItemRemoteLink document)
        {
            return _tableRemoteLink.Update(id, document);
        }

        /// <summary>
        /// 更新指定的目录信息.
        /// </summary>
        public static bool Update(string id, DbItemDirectory document)
        {
            return _tableDirectory.Update(id, document);
        }

        /// <summary>
        /// 更新指定远程桌面连接的设置.
        /// </summary>
        public static bool Update(string id, DbItemSetting document)
        {
            if (document is DbItemSettingRdp)
            {
                return _tableSetting_rdp.Update(id, (DbItemSettingRdp)document);
            }
            if (document is DbItemSettingSsh)
            {
                return _tableSetting_ssh.Update(id, (DbItemSettingSsh)document);
            }
            if (document is DbItemSettingTelnet)
            {
                return _tableSetting_telnet.Update(id, (DbItemSettingTelnet)document);
            }
            return false;
        }
    }
}