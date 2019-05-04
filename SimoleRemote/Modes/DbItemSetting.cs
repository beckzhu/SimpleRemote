using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRemote.Modes
{
    public class DbItemSetting
    {
        /// <summary>默认</summary>
        public const int OPEN_DEFAULT = 0;
        /// <summary>选项卡</summary>
        public const int OPEN_TAB = 1;
        /// <summary>后台</summary>
        public const int OPEN_TAB_BACKSTAGE = 2;
        /// <summary>外部窗口</summary>
        public const int OPEN_WINDOW = 3;

        public string Id { get; set; }
        /// <summary>打开远程的首选方式,参考OPEN_开头常量 </summary>
        public int OpenMode { get; set; }
        // <summary>远程桌面尺寸与ScreenResolutions对应的尺寸,从1开始 </summary>
        public int SizeIndex { get; set; }
    }
}
