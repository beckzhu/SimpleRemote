using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SimpleRemote.Core
{
    public static class UpdateServices
    {
        private static string _logUrl;
        private static string _appUrl;
        private static string _hashCode;

        /// <summary>
        /// 更新地址
        /// </summary>
        public static string UpdateUrl { get => "http://www.91fk.net/Update.xml"; }

        /// <summary>
        /// 本地更新日期
        /// </summary>
        public static DateTime LocalUpdated { get; } = new DateTime(2019, 6, 7);

        /// <summary>
        /// 本地版本号
        /// </summary>
        public static string LocalVersion { get; internal set; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// 远程更新日期
        /// </summary>
        public static DateTime RemoteUpdated { get; internal set; }

        /// <summary>
        /// 远程版本号
        /// </summary>
        public static string RemoteVersion { get; internal set; }

        /// <summary>
        /// 文件的哈希值
        /// </summary>
        public static string HashCode { get; internal set; }

        /// <summary>
        /// 强制更新
        /// </summary>
        public static bool Force { get; internal set; }

        /// <summary>
        /// 检查更新
        /// </summary>
        public static async Task<bool> Check()
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var xmlStream = await httpClient.GetStreamAsync(UpdateUrl);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlStream);

                    RemoteVersion = xmlDoc.DocumentElement.GetAttribute("Version");
                    DateTime.TryParse(xmlDoc.DocumentElement.GetAttribute("Updated"), out DateTime value);
                    RemoteUpdated = value;
                    Force = xmlDoc.DocumentElement.GetAttribute("Force") == "True";
                    _logUrl = ((XmlElement)xmlDoc.DocumentElement.SelectSingleNode("LogUrl")).GetAttribute("url");
                    _appUrl = ((XmlElement)xmlDoc.DocumentElement.SelectSingleNode("AppUrl")).GetAttribute("url");
                    _hashCode = ((XmlElement)xmlDoc.DocumentElement.SelectSingleNode("HashCode")).GetAttribute("md5");
                    if (RemoteUpdated == null || RemoteVersion == null) return false;

                    if (LocalUpdated < RemoteUpdated) return true;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"获取最新版本失败,原因：{e.Message}");
            }
            return false;
        }

        /// <summary>
        /// 获取更新日志
        /// </summary>
        public static async Task<string> GetUpdateLog()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                return await httpClient.GetStringAsync(_logUrl);
            }
            catch (Exception e)
            {
                throw new Exception($"获取更新日志失败,原因：{e.Message}");
            }
        }

        /// <summary>
        /// 下载最新版本
        /// </summary>
        public static async Task Download()
        {
            string zipFile = Path.Combine(UserSettings.AppDataDir, "Update.zip");
            if (string.Compare(GetFileMd5(zipFile), _hashCode, true) != 0)
            {
                try
                {
                    WebClient webClient = new WebClient();
                    await webClient.DownloadFileTaskAsync(_appUrl, zipFile);
                    if (string.Compare(GetFileMd5(zipFile), _hashCode, true) != 0)
                    {
                        throw new Exception($"下载最新版本失败,原因：文件校验不通过，可能遭到非法篡改");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"下载最新版本失败,原因：{e.Message}");
                }
            }
        }

        /// <summary>
        /// 获取指定文件的md5,如果文件不存在 将返回Null
        /// </summary>
        public static string GetFileMd5(string fileName)
        {
            try
            {
                if (!File.Exists(fileName)) return null;
                FileStream file = new FileStream(fileName, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("X2"));
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                throw new Exception("获取文件Md5() 失败," + e.Message);
            }
        }

        /// <summary>
        /// 获取指定字节数组的md5
        /// </summary>
        public static string GetBytesMd5(byte[] bytes)
        {
            try
            {
                if (bytes == null) return null;
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("X2"));
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                throw new Exception("获取文件Md5() 失败," + e.Message);
            }
        }
    }
}