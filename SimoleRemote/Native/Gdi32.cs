using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRemote.Native
{
    public static class Gdi32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public String lfFaceName;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct ENUMLOGFONT
        {
            public LOGFONT elfLogFont;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string elfFullName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string elfStyle;
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern int EnumFontFamiliesEx(IntPtr hdc, ref LOGFONT lpLogfont, FontEnumProc lpEnumFontFamExProc, IntPtr lParam, uint dwFlags);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern int EnumFontFamiliesEx(IntPtr hdc, IntPtr lpLogfont, FontEnumProc lpEnumFontFamExProc, IntPtr lParam, uint dwFlags);
        public delegate int FontEnumProc(ref ENUMLOGFONT lpelf, IntPtr lpntm, uint FontType, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
    }
}
