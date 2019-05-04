using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Resources;

namespace SimpleRemote.Bll
{
    public static class Common
    {
        /// <summary>
        /// 操作系统版本> 6.1 则Win8以上
        /// </summary>
        public static float OSVersion;
        public static void Init()
        {
            OSVersion = float.Parse($"{Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor}");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve; ;
        }
        /// <summary>
        /// 从嵌入的资源加载加载失败的程序集
        /// </summary>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string resourceName = $"SimpleRemote.Lib.{new AssemblyName(args.Name).Name}.dll.Compress";
            byte[] buf= GetCompressResBytes(resourceName);
            if (buf != null) return Assembly.Load(buf);
            return null;
        }
        /// <summary>
        /// 获取程序集  生成操作-Resource  对应的资源
        /// </summary>
        public static Stream GetResourceStream(string resourcePath)
        {
            try
            {
                var uri = new Uri($"pack://application:,,,/SimpleRemote;component//{resourcePath}");
                StreamResourceInfo sri = System.Windows.Application.GetResourceStream(uri);
                return sri.Stream;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取嵌入的资源被压缩的资源并解压
        /// </summary>
        public static byte[] GetCompressResBytes(string name)
        {
            var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
            if (resStream != null)
            {
                MemoryStream stream = new MemoryStream();
                DeflateStream compressionStream = new DeflateStream(resStream, CompressionMode.Decompress);
                compressionStream.CopyTo(stream);
                compressionStream.Close();
                stream.Close();
                return stream.ToArray();
            }
            return null;
        }
    }
}
