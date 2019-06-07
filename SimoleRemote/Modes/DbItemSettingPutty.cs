using SimpleRemote.Core;

namespace SimpleRemote.Modes
{
    public abstract class DbItemSettingPutty : DbItemSetting
    {
        private int _cursor;
        private string _fontName;
        private int _fontSize;
        private int _character;
        private int _fallbackkeys;
        private int _mouseAction;
        private string _colorScheme;
        private int _homeAndEnd;
        private int _fnAndKeypad;
        private bool? _lFImpliesCR;
        private bool? _cRImpliesLF;
        private bool? _cJKAmbigWide;
        private bool? _capsLockCyr;

        /// <summary>
        /// 光标
        /// </summary>
        public int Cursor
        {
            get => _cursor;
            set
            {
                _cursor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 字体
        /// </summary>
        public string FontName
        {
            get => _fontName;
            set
            {
                _fontName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize
        {
            get => _fontSize; set
            {
                _fontSize = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 字符集
        /// </summary>
        public int Character
        {
            get => _character; set
            {
                _character = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 回退键
        /// </summary>
        public int Fallbackkeys
        {
            get => _fallbackkeys; set
            {
                _fallbackkeys = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 鼠标动作
        /// </summary>
        public int MouseAction
        {
            get => _mouseAction;
            set
            {
                _mouseAction = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 配色方案
        /// </summary>
        public string ColorScheme
        {
            get => _colorScheme;
            set
            {
                _colorScheme = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Home和End键
        /// </summary>
        public int HomeAndEnd
        {
            get => _homeAndEnd; set
            {
                _homeAndEnd = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Fn和小键盘
        /// </summary>
        public int FnAndKeypad
        {
            get => _fnAndKeypad; set
            {
                _fnAndKeypad = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 在每个LF字符后增加CR
        /// </summary>
        public bool? LFImpliesCR
        {
            get => _lFImpliesCR; set
            {
                _lFImpliesCR = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 在每个CR字符后增加LF
        /// </summary>
        public bool? CRImpliesLF
        {
            get => _cRImpliesLF; set
            {
                _cRImpliesLF = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 将不确定字符处理为CJK
        /// </summary>s
        public bool? CJKAmbigWide
        {
            get => _cJKAmbigWide; set
            {
                _cJKAmbigWide = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// CapsLock用于Cyrillic切换
        /// </summary>
        public bool? CapsLockCyr
        {
            get => _capsLockCyr; set
            {
                _capsLockCyr = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 重置所有设置
        /// </summary>
        public override void Reset()
        {
            Cursor = 0;
            FontName = null;
            FontSize = 0;
            Character = 0;
            Fallbackkeys = 0;
            MouseAction = 0;
            ColorScheme = null;
            HomeAndEnd = 0;
            FnAndKeypad = 0;
            LFImpliesCR = null;
            CRImpliesLF = null;
            CJKAmbigWide = null;
            CapsLockCyr = null;
        }

        /// <summary>
        /// 用另外一个设置来更新当前设置，当源属性值不为空的时候  才会被更新
        /// </summary>
        /// <param name="dbItemSetting"></param>
        public override void UpdateValue(DbItemSetting sourceSetting)
        {
            base.UpdateValue(sourceSetting);
            DbItemSettingPutty sourceSettingPutty = sourceSetting as DbItemSettingPutty;
            if (sourceSettingPutty != null)
            {
                if (sourceSettingPutty.Cursor != 0) Cursor = sourceSettingPutty.Cursor;
                if (!string.IsNullOrEmpty(sourceSettingPutty.FontName)) FontName = sourceSettingPutty.FontName;
                if (sourceSettingPutty.FontSize != 0) FontSize = sourceSettingPutty.FontSize;
                if (sourceSettingPutty.Character != 0) Character = sourceSettingPutty.Character;
                if (sourceSettingPutty.Fallbackkeys != 0) Fallbackkeys = sourceSettingPutty.Fallbackkeys;
                if (sourceSettingPutty.MouseAction != 0) MouseAction = sourceSettingPutty.MouseAction;
                if (!string.IsNullOrEmpty(sourceSettingPutty.ColorScheme)) ColorScheme = sourceSettingPutty.ColorScheme;
                if (sourceSettingPutty.HomeAndEnd != 0) HomeAndEnd = sourceSettingPutty.HomeAndEnd;
                if (sourceSettingPutty.FnAndKeypad != 0) FnAndKeypad = sourceSettingPutty.FnAndKeypad;
                if (sourceSettingPutty.LFImpliesCR != null) LFImpliesCR = sourceSettingPutty.LFImpliesCR;
                if (sourceSettingPutty.CRImpliesLF != null) CRImpliesLF = sourceSettingPutty.CRImpliesLF;
                if (sourceSettingPutty.CJKAmbigWide != null) CJKAmbigWide = sourceSettingPutty.CJKAmbigWide;
                if (sourceSettingPutty.CapsLockCyr != null) CapsLockCyr = sourceSettingPutty.CapsLockCyr;
            }
        }

        /// <summary>
        /// 获取配色相关信息
        /// </summary>
        /// <returns></returns>
        public DbPuttyColor GetPuttyColor()
        {
            return DatabaseServices.GetPuttyColor(ColorScheme);
        }
    }
}