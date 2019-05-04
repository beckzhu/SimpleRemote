using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace SimpleRemote.Modes
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DEVMODE
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;

        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;
        public int dmPositionX;
        public int dmPositionY;
        public int dmDisplayOrientation;
        public int dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;

        public short dmLogPixels;
        public short dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;
        public int dmPanningWidth;
        public int dmPanningHeight;
    };
    class DisplaySettings
    {
        [DllImport("user32.dll")]
        private static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        /// <summary> 获取显示器支持的分辨率</summary>
        public static List<Size> GetSupportedScreenResolutions()
        {
            List<Size> list = new List<Size>();
            
            var devmode = new DEVMODE();
            devmode.dmDeviceName = new String(new char[32]);
            devmode.dmFormName = new String(new char[32]);
            devmode.dmSize = (short)Marshal.SizeOf(devmode);

            int i = 0, displayWidth = 0;
            while (EnumDisplaySettings(null, i, ref devmode) == 1)
            {
                if (devmode.dmPelsWidth >= 800 && displayWidth != devmode.dmPelsWidth)
                {
                    displayWidth = devmode.dmPelsWidth;
                    list.Add(new Size { Width = (short)devmode.dmPelsWidth, Height = (short)devmode.dmPelsHeight });
                }
                i++;
            }
            list.Reverse();
            return list;
        }
    }
}
