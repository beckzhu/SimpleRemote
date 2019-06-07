using MSTSCLib;
using SimpleRemote.Modes;
using SimpleRemote.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Resources;

namespace SimpleRemote.Core
{
    public static class CommonServices
    {
        /// <summary>
        /// 操作系统版本> 6.1 则Win8以上
        /// </summary>
        public static float OSVersion { get; internal set; }

        public static Size[] DesktopSizes { get; internal set; }

        /// <summary>
        /// 枚举系统所有的等宽字体</summary>
        public static string[] EqualWidthFonts { get; internal set; }

        /// <summary>
        /// 获取系统支持的字符集
        /// </summary>
        public static EncodingInfo[] Encodings { get; internal set; }

        /// <summary>
        /// 获取Putty配色列表</summary>
        public static string[] PuttyColorlNames { get; internal set; }

        public static void Init()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve; ;

            OSVersion = float.Parse($"{Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor}");
            Encodings = Encoding.GetEncodings();
            PuttyColorlNames = DatabaseServices.GetPuttyColorlNames();
            EqualWidthFonts = GetEqualWidthFonts();
            DesktopSizes = GetSystemResolutions();

            if (Encodings == null) Encodings = new EncodingInfo[0];
            if (PuttyColorlNames == null) PuttyColorlNames = new string[0];
            if (EqualWidthFonts == null) EqualWidthFonts = new string[0];
            if (DesktopSizes == null) DesktopSizes = new Size[0];

            //加载默认设置
            DatabaseServices.GetDefaultSetting(RemoteType.rdp).SetDefaultSetting();
            DatabaseServices.GetDefaultSetting(RemoteType.ssh).SetDefaultSetting();
            DatabaseServices.GetDefaultSetting(RemoteType.telnet).SetDefaultSetting();
        }

        /// <summary>
        /// 从嵌入的资源加载加载失败的程序集
        /// </summary>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string resourceName = $"SimpleRemote.Lib.{new AssemblyName(args.Name).Name}.dll.Compress";
            byte[] buf = GetCompressResBytes(resourceName);
            if (buf != null) return Assembly.Load(buf);
            return null;
        }

        /// <summary>
        /// 获取程序集  生成操作=Resource  对应的资源
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

        public static _RemotableHandle HWNDtoRemotableHandle(IntPtr hwnd)
        {
            _RemotableHandle handle = new _RemotableHandle();
            Kernel32.CopyMemory(ref handle, ref hwnd, IntPtr.Size);
            return (handle);
        }

        public static string GetStringMd5(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes;
            hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(text));
            StringBuilder tmp = new StringBuilder();
            foreach (byte i in hashedDataBytes)
            {
                tmp.Append(i.ToString("x2"));
            }
            return tmp.ToString();
        }

        /// <summary>
        /// 获取系统支持的分辨率
        /// </summary>
        public static Size[] GetSystemResolutions()
        {
            List<Size> list = new List<Size>();

            var devmode = new User32.DEVMODE();
            devmode.dmDeviceName = new string(new char[32]);
            devmode.dmFormName = new string(new char[32]);
            devmode.dmSize = (short)Marshal.SizeOf(devmode);

            int i = 0, displayWidth = 0;
            while (User32.EnumDisplaySettings(null, i, ref devmode) == 1)
            {
                if (devmode.dmPelsWidth >= 800 && displayWidth != devmode.dmPelsWidth)
                {
                    displayWidth = devmode.dmPelsWidth;
                    list.Add(new Size { Width = (short)devmode.dmPelsWidth, Height = (short)devmode.dmPelsHeight });
                }
                i++;
            }
            list.Reverse();
            return list.ToArray();
        }

        /// <summary>
        /// 获取系统安装的等宽字体
        /// </summary>
        private static string[] GetEqualWidthFonts()
        {
            List<string> list = new List<string>();

            Gdi32.FontEnumProc FontEnumProc = (ref Gdi32.ENUMLOGFONT lpelf, IntPtr lpntm, uint FontType, IntPtr lParam) =>
            {
                if ((lpelf.elfLogFont.lfPitchAndFamily & 0x3) == 1)
                {
                    string fontName = lpelf.elfLogFont.lfFaceName;
                    if (list.FindIndex(m => m == fontName) < 0)
                    {
                        list.Add(fontName);
                    }
                }
                return 1;
            };

            IntPtr hdc = Gdi32.GetDC(IntPtr.Zero);
            Gdi32.EnumFontFamiliesEx(hdc, IntPtr.Zero, FontEnumProc, IntPtr.Zero, 0);
            Gdi32.ReleaseDC(IntPtr.Zero, hdc);

            return list.ToArray();
        }

        /// <summary>
        /// 取高16位
        /// </summary>
        public static int LOWORD(int value)
        {
            return value & 0xFFFF;
        }

        /// <summary>
        /// 取低16位
        /// </summary>
        public static int HIWORD(int value)
        {
            return value >> 16;
        }

        /// <summary>
        /// 取高8位
        /// </summary>
        public static byte LOWBYTE(ushort value)
        {
            return (byte)(value & 0xFF);
        }

        /// <summary>
        /// 取低16位
        /// </summary>
        public static byte HIGHBYTE(ushort value)
        {
            return (byte)(value >> 8);
        }
    }
}