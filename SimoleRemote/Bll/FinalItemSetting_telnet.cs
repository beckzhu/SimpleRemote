using SimpleRemote.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRemote.Bll
{
    public class FinalItemSetting_telnet : FinalItemSetting
    {
        private DbItemSetting_telnet _itemSettingssh;

        public FinalItemSetting_telnet(DbItemSetting_telnet itemSettingssh)
            : base(itemSettingssh)
        {
            _itemSettingssh = itemSettingssh;
        }
        /// <summary>
        /// 光标
        /// </summary>
        public int Cursor { get => _itemSettingssh.Cursor == 0 ? GlobalSetting.Settings_ssh.Cursor : _itemSettingssh.Cursor; }
        /// <summary>
        /// 字体
        /// </summary>
        public string FontName { get => string.IsNullOrEmpty(_itemSettingssh.FontName) ? GlobalSetting.Settings_ssh.FontName : _itemSettingssh.FontName; }
        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize { get => _itemSettingssh.FontSize == 0 ? GlobalSetting.Settings_ssh.FontSize : _itemSettingssh.FontSize; }
        /// <summary>
        /// 字符集CodePage
        /// </summary>
        public int Character { get => _itemSettingssh.Character == 0 ? GlobalSetting.Settings_ssh.Character : _itemSettingssh.Character; }
        /// <summary>
        /// 回退键
        /// </summary>
        public int Fallbackkeys { get => _itemSettingssh.Fallbackkeys == 0 ? GlobalSetting.Settings_ssh.Fallbackkeys : _itemSettingssh.Fallbackkeys; }
        /// <summary>
        /// 鼠标动作
        /// </summary>
        public int MouseAction { get => _itemSettingssh.MouseAction == 0 ? GlobalSetting.Settings_ssh.MouseAction : _itemSettingssh.MouseAction; }
        /// <summary>
        /// 配色方案
        /// </summary>
        public string ColorScheme { get => string.IsNullOrEmpty(_itemSettingssh.ColorScheme) ? GlobalSetting.Settings_ssh.ColorScheme : _itemSettingssh.ColorScheme; }
        /// <summary>
        /// Home和End键
        /// </summary>
        public int HomeAndEnd { get => _itemSettingssh.HomeAndEnd == 0 ? GlobalSetting.Settings_ssh.HomeAndEnd : _itemSettingssh.HomeAndEnd; }
        /// <summary>
        /// Fn和小键盘
        /// </summary>
        public int FnAndKeypad { get => _itemSettingssh.FnAndKeypad == 0 ? GlobalSetting.Settings_ssh.FnAndKeypad : _itemSettingssh.FnAndKeypad; }
        /// <summary>
        /// 在每个LF字符后增加CR
        /// </summary>
        public bool? LFImpliesCR { get => _itemSettingssh.LFImpliesCR == null ? GlobalSetting.Settings_ssh.LFImpliesCR : _itemSettingssh.LFImpliesCR; }
        /// <summary>
        /// 在每个CR字符后增加LF
        /// </summary>
        public bool? CRImpliesLF { get => _itemSettingssh.CRImpliesLF == null ? GlobalSetting.Settings_ssh.CRImpliesLF : _itemSettingssh.CRImpliesLF; }
        /// <summary>
        /// 将不确定字符处理为CJK
        /// </summary>
        public bool? CJKAmbigWide { get => _itemSettingssh.CJKAmbigWide == null ? GlobalSetting.Settings_ssh.CJKAmbigWide : _itemSettingssh.CJKAmbigWide; }
        /// <summary>
        /// CapsLock用于Cyrillic切换
        /// </summary>
        public bool? CapsLockCyr { get => _itemSettingssh.CapsLockCyr == null ? GlobalSetting.Settings_ssh.CapsLockCyr : _itemSettingssh.CapsLockCyr; }
        /// <summary>
        /// 获取配色相关信息
        /// </summary>
        /// <returns></returns>
        public DbPuttyColor GetPuttyColor()
        {
            return Database.GetPuttyColor(ColorScheme);
        }
    }
}
