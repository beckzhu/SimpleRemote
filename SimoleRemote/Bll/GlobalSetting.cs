using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleRemote.Modes;
using System.Xml;
using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SimpleRemote.DllLibrary;
using System.Windows.Resources;
using System.Resources;
using System.Collections;
using System.Reflection;

namespace SimpleRemote.Bll
{
    
    public static class GlobalSetting
    {
        /// <summary>加载配置</summary>
        public static void LoadSetiing()
        {
            try
            {
                Settings_rdp = (DbItemSetting_rdp)Database.GetGlobalSetting(RemoteType.rdp);
                Settings_ssh = (DbItemSetting_ssh)Database.GetGlobalSetting(RemoteType.ssh);
                Settings_telnet = (DbItemSetting_telnet)Database.GetGlobalSetting(RemoteType.telnet);

                Encodings = Encoding.GetEncodings();
                PuttyColorlNames = Database.GetPuttyColorlNames();
                EqualWidthFonts = GetEqualWidthFonts();
                ScreenSizes = DisplaySettings.GetSupportedScreenResolutions().ToArray();
            }
            catch (Exception e)
            {
                throw new Exception($"加载全局设置失败。\n原因：{e.Message}");
            }
        }
        /// <summary>Rdp相关的全局设置</summary>
        public static DbItemSetting_rdp Settings_rdp { get; set; }
        /// <summary>Rdp相关的全局设置</summary>
        public static DbItemSetting_ssh Settings_ssh { get; set; }
        /// <summary>Rdp相关的全局设置</summary>
        public static DbItemSetting_telnet Settings_telnet { get; set; }
        /// <summary>本地计算机支持的分辨率列表</summary>
        public static Size[] ScreenSizes { get; internal set; }
        /// <summary>枚举系统所有的等宽字体</summary>
        public static string[] EqualWidthFonts { get; internal set; }
        /// <summary>获取系统支持的字符集</summary>
        public static EncodingInfo[] Encodings { get; internal set; }
        /// <summary>获取Putty配色列表</summary>
        public static string[] PuttyColorlNames { get; internal set; }
        
        /// <summary>
        /// 获取系统安装的等宽字体
        /// </summary>
        /// <returns></returns>
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
    }
}
