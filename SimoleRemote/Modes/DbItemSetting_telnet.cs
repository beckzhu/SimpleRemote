using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRemote.Modes
{
    public class DbItemSetting_telnet : DbItemSetting
    {
        /// <summary>
        /// 光标
        /// </summary>
        public int Cursor { get; set; }
        /// <summary>
        /// 字体
        /// </summary>
        public string FontName { get; set; }
        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize { get; set; }
        /// <summary>
        /// 字符集
        /// </summary>
        public int Character { get; set; }
        /// <summary>
        /// 回退键
        /// </summary>
        public int Fallbackkeys { get; set; }
        /// <summary>
        /// 鼠标动作
        /// </summary>
        public int MouseAction { get; set; }
        /// <summary>
        /// 配色方案
        /// </summary>
        public string ColorScheme { get; set; }
        /// <summary>
        /// Home和End键
        /// </summary>
        public int HomeAndEnd { get; set; }
        /// <summary>
        /// Fn和小键盘
        /// </summary>
        public int FnAndKeypad { get; set; }
        /// <summary>
        /// 在每个LF字符后增加CR
        /// </summary>
        public bool? LFImpliesCR { get; set; }
        /// <summary>
        /// 在每个CR字符后增加LF
        /// </summary>
        public bool? CRImpliesLF { get; set; }
        /// <summary>
        /// 将不确定字符处理为CJK
        /// </summary>
        public bool? CJKAmbigWide { get; set; }
        /// <summary>
        /// CapsLock用于Cyrillic切换
        /// </summary>
        public bool? CapsLockCyr { get; set; }

    }
}
