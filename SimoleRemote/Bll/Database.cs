using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using LiteDB;
using SimpleRemote.Modes;
using SimpleRemote.Properties;

namespace SimpleRemote.Bll
{
    public class Database
    {
        private static LiteDatabase _litedb;
        private static string _dataFile;
        private static LiteCollection<DbItemDirectory> _tableDirectory;
        private static LiteCollection<DbItemRemoteLink> _tableRemoteLink;
        private static LiteCollection<DbItemSetting_rdp> _tableSetting_rdp;
        private static LiteCollection<DbItemSetting_ssh> _tableSetting_ssh;
        private static LiteCollection<DbItemSetting_telnet> _tableSetting_telnet;
        private static LiteCollection<DbSshHostKeys> _tableSshHostKeys;
        private static LiteCollection<DbPuttyColor> _tablePuttyColor;
        /// <summary>
        /// 判断指定的数据库密码是否正确
        /// </summary>
        public static bool IsPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) password = "08817334c6451b95b0097de95d0bb953";
            else password = NumCalculation.GetStringMd5(password);
            try
            {
                LiteEngine liteEngine = new LiteEngine(_dataFile, password);
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 更改密码
        /// </summary>
        public static void ChangePassword(string password)
        {
            if (string.IsNullOrEmpty(password)) password = "08817334c6451b95b0097de95d0bb953";
            else password = NumCalculation.GetStringMd5(password);
            _litedb.Shrink(password);
        }
        /// <summary>
        /// 用指定的密码打开LiteDb数据库文件，如果不成功将返回Flase
        /// </summary>
        public static bool Open(string password)
        {
            try
            {
                _dataFile = $"{UserSettings.AppdDataDir}\\RemoteItems.db";
                if (!File.Exists(_dataFile))//新建数据库文件
                {
                    File.WriteAllBytes(_dataFile, Resources.RemoteItems);
                }
                if (string.IsNullOrEmpty(password)) password = "08817334c6451b95b0097de95d0bb953";
                else password = NumCalculation.GetStringMd5(password);
                //验证密码是否正确
                try { LiteEngine liteEngine = new LiteEngine(_dataFile, password); }
                catch { return false; };

                _litedb = new LiteDatabase($"Filename={_dataFile};Password={password}");
                _litedb.Mapper.EmptyStringToNull = false;
                _litedb.Mapper.TrimWhitespace = false;

                _tableDirectory = _litedb.GetCollection<DbItemDirectory>("Directory");
                _tableRemoteLink = _litedb.GetCollection<DbItemRemoteLink>("RemoteLink");
                _tableSetting_rdp = _litedb.GetCollection<DbItemSetting_rdp>("Setting_rdp");
                _tableSetting_ssh = _litedb.GetCollection<DbItemSetting_ssh>("Setting_ssh");
                _tableSetting_telnet = _litedb.GetCollection<DbItemSetting_telnet>("Setting_telnet");
                _tableSshHostKeys = _litedb.GetCollection<DbSshHostKeys>("SshHostKeys");
                _tablePuttyColor = _litedb.GetCollection<DbPuttyColor>("PuttyColor");
                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"打开数据库文件失败。\n原因：{e.Message}");
            }
        }
        /// <summary>
        /// 枚举目录
        /// </summary>
        public static IEnumerable<DbItemDirectory> EnumDirectory()
        {
            return _tableDirectory.FindAll();
        }
        /// <summary>
        /// 枚举远程连接设置
        /// </summary>
        public static IEnumerable<DbItemRemoteLink> EnumRemoteLink()
        {
            return _tableRemoteLink.FindAll();
        }
        /// <summary>
        /// 插入远程连接条目,返回Id
        /// </summary>
        public static string InsertRemoteItem(RemoteType type, string parentId, string name)
        {
            if (type == RemoteType.dir)
            {
                return _tableDirectory.Insert(new DbItemDirectory { Id = ObjectId.NewObjectId().ToString(), ParentId = parentId, Name = name });
            }
            else
            {
                string userName = type == RemoteType.rdp ? "Administrator" : "root";
                string uuid = ObjectId.NewObjectId().ToString();
                if (type == RemoteType.rdp) _tableSetting_rdp.Insert(new DbItemSetting_rdp { Id = uuid });
                if (type == RemoteType.ssh) _tableSetting_ssh.Insert(new DbItemSetting_ssh { Id = uuid });
                if (type == RemoteType.telnet) _tableSetting_telnet.Insert(new DbItemSetting_telnet { Id = uuid });
                return _tableRemoteLink.Insert(new DbItemRemoteLink { Id = uuid, ParentId = parentId, Name = name, UserName = userName, Type = (int)type, IsExpander1 = true });
            }
        }
        /// <summary>
        /// 删除远程连接条目,成功返回=True,失败返回False
        /// </summary>
        public static void DeleteRemoteItem(RemoteType type, string id)
        {
            if (type == RemoteType.dir)
            {
                var dirItems = _tableDirectory.Find(m => m.ParentId == id);
                foreach (var item in dirItems)
                {
                    DeleteRemoteItem(RemoteType.dir, item.Id);
                }
                var linkItems = _tableRemoteLink.Find(m => m.ParentId == id);
                foreach (var item in linkItems)
                {
                    DeleteRemoteItem((RemoteType)item.Type, item.Id);
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
        public static DbItemSetting GetGlobalSetting(RemoteType type)
        {
            if (type == RemoteType.rdp) return _tableSetting_rdp.FindById("0001");
            if (type == RemoteType.ssh) return _tableSetting_ssh.FindById("0001");
            if (type == RemoteType.telnet) return _tableSetting_telnet.FindById("0001");
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
        public static void SetSshHostKey(string id,byte[] value)
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
            string[] names = new string[_tablePuttyColor.Count()+1];
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
            return _tablePuttyColor.FindById(name);

        }
        /// <summary>
        /// 更新指定的条目连接信息.
        /// </summary>
        public static bool Update(string id,DbItemRemoteLink document)
        {
            return _tableRemoteLink.Update(id,document);
        }
        /// <summary>
        /// 更新指定的目录信息.
        /// </summary>
        public static bool Update(string id,DbItemDirectory document)
        {
            return _tableDirectory.Update(id,document);
        }
        /// <summary>
        /// 更新指定的Rdp设置信息.
        /// </summary>
        public static bool Update(string id, DbItemSetting_rdp document)
        {
            return _tableSetting_rdp.Update(id, document);
        }
        /// <summary>
        /// 更新指定的SSH设置信息.
        /// </summary>
        public static bool Update(string id, DbItemSetting_ssh document)
        {
            return _tableSetting_ssh.Update(id, document);
        }
        /// <summary>
        /// 更新指定的Telnet设置信息.
        /// </summary>
        public static bool Update(string id, DbItemSetting_telnet document)
        {
            return _tableSetting_telnet.Update(id, document);
        }
    }
}
