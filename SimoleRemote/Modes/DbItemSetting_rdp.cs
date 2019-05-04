using SimpleRemote.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRemote.Modes
{
    public class DbItemSetting_rdp:DbItemSetting
    {
        //性能
        /// <summary>默认</summary>
        public const int CONNECTION_DEFAULT = 0;
        /// <summary>调制解调器（56 Kbps）</summary>
        public const int CONNECTION_TYPE_MODEM = 1;
        /// <summary>低速宽带（256 Kbps至2 Mbps）</summary>
        public const int CONNECTION_TYPE_BROADBAND_LOW = 2;
        /// <summary>卫星（2 Mbps至16 Mbps，高延迟）</summary>
        public const int CONNECTION_TYPE_SATELLITE = 3;
        /// <summary>高速宽带（2 Mbps至10 Mbps）</summary>
        public const int CONNECTION_TYPE_BROADBAND_HIGH = 4;
        /// <summary>广域网（WAN）（10 Mbps或更高，具有高延迟）</summary>
        public const int CONNECTION_TYPE_WAN = 5;
        /// <summary>局域网（LAN）（10 Mbps或更高）</summary>
        public const int CONNECTION_TYPE_LAN = 6;
        /// <summary>自动检测配置</summary>
        public const int CONNECTION_TYPE_AUTO = 7;

        //颜色深度
        /// <summary>默认</summary>
        public const int COLOR_DEFAULT = 0;
        public const int COLOR_15BPP = 1;
        public const int COLOR_16BPP = 2;
        public const int COLOR_24BPP = 3;
        public const int COLOR_32BPP = 4;

        //音频重定向
        /// <summary>默认</summary>
        public const int AUDIO_DEFAULT = 0;
        ///<summary>在本地计算机播放</summary>
        public const int AUDIO_MODE_REDIRECT = 1;
        ///<summary>在远程计算机播放</summary>
        public const int AUDIO_MODE_PLAY_ON_SERVER = 2;
        ///<summary>不播放</summary>
        public const int AUDIO_MODE_NONE = 3;

        //录音重定向
        /// <summary>默认</summary>
        public const int AUDIOCAPTURE_DEFAULT = 0;
        ///<summary>录制</summary>
        public const int AAUDIOCAPTURE_TRUE = 1;
        ///<summary>不录制</summary>
        public const int AUDIOCAPTURE_FALSE = 2;

        //键盘重定向
        /// <summary>默认</summary>
        public const int KEYBOARD_DEFAULT = 0;
        ///<summary>仅在客户端计算机上本地应用组合键</summary>
        public const int KEYBOARD_MODE_REDIRECT = 1;
        ///<summary>在远程服务器上应用组合键</summary>
        public const int KEYBOARD_MODE_SERVER = 2;
        ///<summary>仅当客户端以全屏模式运行时，才将组合键应用于远程服务器</summary>
        public const int KEYBOARD_MODE_FULL = 3;

        //本地资源
        /// <summary>默认</summary>
        public const int RESOURCES_DEFAULT = 0;
        /// <summary>无重定向</summary>
        public const int RESOURCES_NULL = 1;
        /// <summary>打印机</summary>
        public const int RESOURCES_PRINTF = 2;
        /// <summary>剪贴板</summary>
        public const int RESOURCES_CLIPBOARD = 4;
        /// <summary>智能卡</summary>
        public const int RESOURCES_SMARTCARD = 8;
        /// <summary>端口</summary>
        public const int RESOURCES_PORT = 16;
        /// <summary>驱动器</summary>
        public const int RESOURCES_DRIVER = 32;
        /// <summary>视频设备</summary>
        public const int RESOURCES_VIDEO = 64;

        /// <summary>远程桌面的性能设置,参考CONNECTION_开头常量 </summary>
        public int Performance { get; set; }
        /// <summary>颜色深度,参考COLOR_开头常量 </summary>
        public int ColorDepthMode { get; set; }
        /// <summary>音频重定义,参考AUDIO_开头常量 </summary>
        public int AudioRedirectionMode { get; set; }
        /// <summary>录音重定向,参考AUDIOCAPTURE_开头常量 </summary>
        public int AudioCaptureRedirectionMode { get; set; }
        /// <summary>组合键,参考KEYBOARD_开头常量 </summary>
        public int KeyboardHookMode { get; set; }
        /// <summary>重定向打印机</summary>
        public bool? RedirectionPrintf { get; set; }
        /// <summary>重定向剪贴板</summary>
        public bool? RedirectionClipboard { get; set; }
        /// <summary>重定向智能卡</summary>
        public bool? RedirectionsMartcard { get; set; }
        /// <summary>重定向端口</summary>
        public bool? RedirectionsPort { get; set; }
        /// <summary>重定向驱动器</summary>
        public bool? RedirectionsDriver { get; set; }
        /// <summary>重定向视频设备</summary>
        //public bool? RedirectionsVideo { get; set; }
    }
}
