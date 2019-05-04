using SimpleRemote.Bll;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleRemote.Modes;

namespace SimpleRemote.Bll
{
    public class FinalItemSetting_rdp:FinalItemSetting
    {
        private DbItemSetting_rdp _itemSettingRdp;

        public FinalItemSetting_rdp(DbItemSetting_rdp itemSettingRdp)
            : base(itemSettingRdp)
        {
            _itemSettingRdp = itemSettingRdp;
        }
        /// <summary>远程桌面的性能设置 </summary>
        public int Performance { get => _itemSettingRdp.Performance == DbItemSetting_rdp.CONNECTION_DEFAULT ? GlobalSetting.Settings_rdp.Performance : _itemSettingRdp.Performance; }
        /// <summary>颜色深度 </summary>
        public int ColorDepthMode { get => _itemSettingRdp.ColorDepthMode == DbItemSetting_rdp.COLOR_DEFAULT ? GlobalSetting.Settings_rdp.ColorDepthMode : _itemSettingRdp.ColorDepthMode; }
        /// <summary>音频重定义 </summary>
        public int AudioRedirectionMode { get => _itemSettingRdp.AudioRedirectionMode == DbItemSetting_rdp.AUDIO_DEFAULT ? GlobalSetting.Settings_rdp.AudioRedirectionMode : _itemSettingRdp.AudioRedirectionMode; }
        /// <summary>录音重定向 </summary>
        public int AudioCaptureRedirectionMode { get => _itemSettingRdp.AudioCaptureRedirectionMode == DbItemSetting_rdp.AUDIOCAPTURE_DEFAULT ? GlobalSetting.Settings_rdp.AudioCaptureRedirectionMode : _itemSettingRdp.AudioCaptureRedirectionMode; }
        /// <summary>组合键 </summary>
        public int KeyboardHookMode { get => _itemSettingRdp.KeyboardHookMode == DbItemSetting_rdp.KEYBOARD_DEFAULT ? GlobalSetting.Settings_rdp.KeyboardHookMode : _itemSettingRdp.KeyboardHookMode; }
        /// <summary>重定向打印机</summary>
        public bool? RedirectionPrintf { get => _itemSettingRdp.RedirectionPrintf == null ? GlobalSetting.Settings_rdp.RedirectionPrintf : _itemSettingRdp.RedirectionPrintf; }
        /// <summary>重定向剪贴板</summary>
        public bool? RedirectionClipboard { get => _itemSettingRdp.RedirectionClipboard == null ? GlobalSetting.Settings_rdp.RedirectionClipboard : _itemSettingRdp.RedirectionClipboard; }
        /// <summary>重定向智能卡</summary>
        public bool? RedirectionsMartcard { get => _itemSettingRdp.RedirectionsMartcard == null ? GlobalSetting.Settings_rdp.RedirectionsMartcard : _itemSettingRdp.RedirectionsMartcard; }
        /// <summary>重定向端口</summary>
        public bool? RedirectionsPort { get => _itemSettingRdp.RedirectionsPort == null ? GlobalSetting.Settings_rdp.RedirectionsPort : _itemSettingRdp.RedirectionsPort; }
        /// <summary>重定向驱动器</summary>
        public bool? RedirectionsDriver { get => _itemSettingRdp.RedirectionsDriver == null ? GlobalSetting.Settings_rdp.RedirectionsDriver : _itemSettingRdp.RedirectionsDriver; }
        /// <summary>重定向视频设备</summary>
        //public bool? RedirectionsVideo { get => _itemSettingRdp.RedirectionsVideo == null ? GlobalSetting.Settings_rdp.RedirectionsVideo : _itemSettingRdp.RedirectionsVideo; }
    }
}
