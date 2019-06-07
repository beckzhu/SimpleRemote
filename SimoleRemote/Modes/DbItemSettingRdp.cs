using LiteDB;
using SimpleRemote.Core;
using System.Windows;

namespace SimpleRemote.Modes
{
    public class DbItemSettingRdp : DbItemSetting
    {
        //性能
        /// <summary>
        /// 默认
        /// </summary>
        public const int CONNECTION_DEFAULT = 0;

        /// <summary>
        /// 调制解调器（56 Kbps）
        /// </summary>
        public const int CONNECTION_TYPE_MODEM = 1;

        /// <summary>
        /// 低速宽带（256 Kbps至2 Mbps）
        /// </summary>
        public const int CONNECTION_TYPE_BROADBAND_LOW = 2;

        /// <summary>
        /// 卫星（2 Mbps至16 Mbps，高延迟）
        /// </summary>
        public const int CONNECTION_TYPE_SATELLITE = 3;

        /// <summary>
        /// 高速宽带（2 Mbps至10 Mbps）
        /// </summary>
        public const int CONNECTION_TYPE_BROADBAND_HIGH = 4;

        /// <summary>
        /// 广域网（WAN）（10 Mbps或更高，具有高延迟）
        /// </summary>
        public const int CONNECTION_TYPE_WAN = 5;

        /// <summary>
        /// 局域网（LAN）（10 Mbps或更高）
        /// </summary>
        public const int CONNECTION_TYPE_LAN = 6;

        /// <summary>
        /// 自动检测配置
        /// </summary>
        public const int CONNECTION_TYPE_AUTO = 7;

        //颜色深度
        /// <summary>
        /// 默认
        /// </summary>
        public const int COLOR_DEFAULT = 0;

        public const int COLOR_15BPP = 1;
        public const int COLOR_16BPP = 2;
        public const int COLOR_24BPP = 3;
        public const int COLOR_32BPP = 4;

        //音频重定向
        /// <summary>
        /// 默认
        /// </summary>
        public const int AUDIO_DEFAULT = 0;

        ///<summary>在本地计算机播放</summary>
        public const int AUDIO_MODE_REDIRECT = 1;

        ///<summary>在远程计算机播放</summary>
        public const int AUDIO_MODE_PLAY_ON_SERVER = 2;

        ///<summary>不播放</summary>
        public const int AUDIO_MODE_NONE = 3;

        //录音重定向
        /// <summary>
        /// 默认
        /// </summary>
        public const int AUDIOCAPTURE_DEFAULT = 0;

        ///<summary>录制</summary>
        public const int AAUDIOCAPTURE_TRUE = 1;

        ///<summary>不录制</summary>
        public const int AUDIOCAPTURE_FALSE = 2;

        //键盘重定向
        /// <summary>
        /// 默认
        /// </summary>
        public const int KEYBOARD_DEFAULT = 0;

        ///<summary>仅在客户端计算机上本地应用组合键</summary>
        public const int KEYBOARD_MODE_REDIRECT = 1;

        ///<summary>在远程服务器上应用组合键</summary>
        public const int KEYBOARD_MODE_SERVER = 2;

        ///<summary>仅当客户端以全屏模式运行时，才将组合键应用于远程服务器</summary>
        public const int KEYBOARD_MODE_FULL = 3;

        private int _performance;
        private int _colorDepthMode;
        private int _audioRedirectionMode;
        private int _audioCaptureRedirectionMode;
        private int _keyboardHookMode;
        private bool? _redirectionPrintf;
        private bool? _redirectionClipboard;
        private bool? _redirectionsMartcard;
        private bool? _redirectionsPort;
        private bool? _redirectionsDriver;

        private static DbItemSettingRdp _defaultSetting;

        /// <summary>
        /// 创建系统默认的设置
        /// </summary>
        public static DbItemSettingRdp FromDefault()
        {
            DbItemSettingRdp itemSetting = new DbItemSettingRdp
            {
                Performance = 7,
                ColorDepthMode = 4,
                AudioRedirectionMode = 1,
                AudioCaptureRedirectionMode = 2,
                KeyboardHookMode = 3,
                RedirectionPrintf = false,
                RedirectionClipboard = false,
                RedirectionsMartcard = false,
                RedirectionsPort = false,
                RedirectionsDriver = false,
                OpenMode = 1,
                SizeIndex = 1
            };
            return itemSetting;
        }

        /// <summary>
        /// 远程桌面的性能设置,参考CONNECTION_开头常量
        /// </summary>
        public int Performance
        {
            get => _performance; set
            {
                _performance = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 颜色深度,参考COLOR_开头常量
        /// </summary>
        public int ColorDepthMode
        {
            get => _colorDepthMode; set
            {
                _colorDepthMode = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 音频重定义,参考AUDIO_开头常量
        /// </summary>
        public int AudioRedirectionMode
        {
            get => _audioRedirectionMode; set
            {
                _audioRedirectionMode = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 录音重定向,参考AUDIOCAPTURE_开头常量
        /// </summary>
        public int AudioCaptureRedirectionMode
        {
            get => _audioCaptureRedirectionMode; set
            {
                _audioCaptureRedirectionMode = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 组合键,参考KEYBOARD_开头常量
        /// </summary>
        public int KeyboardHookMode
        {
            get => _keyboardHookMode; set
            {
                _keyboardHookMode = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 重定向打印机
        /// </summary>
        public bool? RedirectionPrintf
        {
            get => _redirectionPrintf; set
            {
                _redirectionPrintf = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 重定向剪贴板
        /// </summary>
        public bool? RedirectionClipboard
        {
            get => _redirectionClipboard; set
            {
                _redirectionClipboard = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 重定向智能卡
        /// </summary>
        public bool? RedirectionsMartcard
        {
            get => _redirectionsMartcard; set
            {
                _redirectionsMartcard = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 重定向端口
        /// </summary>
        public bool? RedirectionsPort
        {
            get => _redirectionsPort; set
            {
                _redirectionsPort = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 重定向驱动器
        /// </summary>
        public bool? RedirectionsDriver
        {
            get => _redirectionsDriver; set
            {
                _redirectionsDriver = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 默认设置
        /// </summary>
        [BsonIgnore]
        public override DbItemSetting DefaultSetting { get => _defaultSetting; }

        /// <summary>
        /// 重置所有设置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Performance = 0;
            ColorDepthMode = 0;
            AudioRedirectionMode = 0;
            AudioCaptureRedirectionMode = 0;
            KeyboardHookMode = 0;
            RedirectionPrintf = null;
            RedirectionClipboard = null;
            RedirectionsMartcard = null;
            RedirectionsPort = null;
            RedirectionsDriver = null;
        }

        /// <summary>
        /// 用另外一个设置来更新当前设置，当源属性值不为空的时候 才会被更新
        /// </summary>
        /// <param name="dbItemSetting"></param>
        public override void UpdateValue(DbItemSetting sourceSetting)
        {
            base.UpdateValue(sourceSetting);
            DbItemSettingRdp sourceSettingRdp = sourceSetting as DbItemSettingRdp;
            if (sourceSettingRdp != null)
            {
                if (sourceSettingRdp.Performance != 0) Performance = sourceSettingRdp.Performance;
                if (sourceSettingRdp.ColorDepthMode != 0) ColorDepthMode = sourceSettingRdp.ColorDepthMode;
                if (sourceSettingRdp.AudioRedirectionMode != 0) AudioRedirectionMode = sourceSettingRdp.AudioRedirectionMode;
                if (sourceSettingRdp.AudioCaptureRedirectionMode != 0) AudioCaptureRedirectionMode = sourceSettingRdp.AudioCaptureRedirectionMode;
                if (sourceSettingRdp.KeyboardHookMode != 0) KeyboardHookMode = sourceSettingRdp.KeyboardHookMode;
                if (sourceSettingRdp.RedirectionPrintf != null) RedirectionPrintf = sourceSettingRdp.RedirectionPrintf;
                if (sourceSettingRdp.RedirectionClipboard != null) RedirectionClipboard = sourceSettingRdp.RedirectionClipboard;
                if (sourceSettingRdp.RedirectionsMartcard != null) RedirectionsMartcard = sourceSettingRdp.RedirectionsMartcard;
                if (sourceSettingRdp.RedirectionsPort != null) RedirectionsPort = sourceSettingRdp.RedirectionsPort;
                if (sourceSettingRdp.RedirectionsDriver != null) RedirectionsDriver = sourceSettingRdp.RedirectionsDriver;
            }
        }

        /// <summary>
        /// 将当前设置为默认设置
        /// </summary>
        public override void SetDefaultSetting()
        {
            if (_defaultSetting == null)
            {
                _defaultSetting = this;
            }
            else
            {
                _defaultSetting.UpdateValue(this);
            }
            DatabaseServices.Update(DefaultSetting.Id, DefaultSetting);
        }

        /// <summary>
        /// 获取与默认设置计算后的结果
        /// </summary>
        /// <returns></returns>
        public override DbItemSetting GetLastSetting()
        {
            DbItemSetting lastSetting = new DbItemSettingRdp
            {
                OpenMode = OpenMode == 0 ? _defaultSetting.OpenMode : OpenMode,
                SizeIndex = SizeIndex == 0 ? _defaultSetting.SizeIndex : SizeIndex,
                Performance = Performance == 0 ? _defaultSetting.Performance : Performance,
                ColorDepthMode = ColorDepthMode == 0 ? _defaultSetting.ColorDepthMode : ColorDepthMode,
                AudioRedirectionMode = AudioRedirectionMode == 0 ? _defaultSetting.AudioRedirectionMode : AudioRedirectionMode,
                AudioCaptureRedirectionMode = AudioCaptureRedirectionMode == 0 ? _defaultSetting.AudioCaptureRedirectionMode : AudioCaptureRedirectionMode,
                KeyboardHookMode = KeyboardHookMode == 0 ? _defaultSetting.KeyboardHookMode : KeyboardHookMode,
                RedirectionPrintf = RedirectionPrintf == null ? _defaultSetting.RedirectionPrintf : RedirectionPrintf,
                RedirectionClipboard = RedirectionClipboard == null ? _defaultSetting.RedirectionClipboard : RedirectionClipboard,
                RedirectionsMartcard = RedirectionsMartcard == null ? _defaultSetting.RedirectionsMartcard : RedirectionsMartcard,
                RedirectionsPort = RedirectionsPort == null ? _defaultSetting.RedirectionsPort : RedirectionsPort,
                RedirectionsDriver = RedirectionsDriver == null ? _defaultSetting.RedirectionsDriver : RedirectionsDriver,
            };
            return lastSetting;
        }

        public Size GetDeskTopSize()
        {
            if (SizeIndex < 2 || SizeIndex - 3 >= CommonServices.DesktopSizes.Length) return new Size();
            if (SizeIndex == DESKSIZE_FULLSCREEN)
            {
                return new Size
                {
                    Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width,
                    Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height
                };
            }
            else
            {
                return CommonServices.DesktopSizes[SizeIndex - 3];
            }
        }
    }
}