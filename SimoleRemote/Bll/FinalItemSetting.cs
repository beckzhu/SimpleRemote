using SimpleRemote.Bll;
using SimpleRemote.Modes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace SimpleRemote.Bll
{
    public class FinalItemSetting
    {
        private DbItemSetting _itemSetting;

        public FinalItemSetting(DbItemSetting remoteSetting)
        {
            _itemSetting = remoteSetting;
        }
        /// <summary>
        /// 打开方式
        /// </summary>
        public int OpenMode
        {
            get
            {
                if (_itemSetting.OpenMode == DbItemSetting.OPEN_DEFAULT)
                {
                    if (_itemSetting.GetType() == typeof(DbItemSetting_rdp)) return GlobalSetting.Settings_rdp.OpenMode;
                    if (_itemSetting.GetType() == typeof(DbItemSetting_ssh)) return GlobalSetting.Settings_ssh.OpenMode;
                    if (_itemSetting.GetType() == typeof(DbItemSetting_telnet)) return GlobalSetting.Settings_telnet.OpenMode;
                    return 0;
                }
                else return _itemSetting.OpenMode;
            }
        }
        public Size DesktopSize
        {
            get
            {
                int sizeIndex = 1;
                if (_itemSetting.SizeIndex > 0 && _itemSetting.SizeIndex - 4 < GlobalSetting.ScreenSizes.Length) sizeIndex = _itemSetting.SizeIndex;
                else
                {
                    if (_itemSetting.GetType() == typeof(DbItemSetting_rdp)) sizeIndex = GlobalSetting.Settings_rdp.SizeIndex;
                }
                if (sizeIndex == 1) return new Size();//自适应分辨率
                else if (sizeIndex == 2 || sizeIndex == 3)//全屏或使用系统设置
                    return new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                else return GlobalSetting.ScreenSizes[sizeIndex - 4];
            }
        }
        public bool FullScreen
        {
            get
            {
                if (_itemSetting.SizeIndex == 0)
                {
                    if (_itemSetting.GetType() == typeof(DbItemSetting_rdp)) return GlobalSetting.Settings_rdp.SizeIndex == 3;//使用默认设置
                    if (_itemSetting.GetType() == typeof(DbItemSetting_ssh)) return GlobalSetting.Settings_rdp.SizeIndex == 3;//使用默认设置
                    if (_itemSetting.GetType() == typeof(DbItemSetting_telnet)) return GlobalSetting.Settings_rdp.SizeIndex == 3;//使用默认设置
                    return false;
                } 
                else return _itemSetting.SizeIndex == 2;
            }
        }
    }
}
