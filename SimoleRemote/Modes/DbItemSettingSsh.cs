using LiteDB;
using SimpleRemote.Core;

namespace SimpleRemote.Modes
{
    public class DbItemSettingSsh : DbItemSettingPutty
    {
        private static DbItemSettingSsh _defaultSetting;

        /// <summary>
        /// 创建系统默认的设置
        /// </summary>
        public static DbItemSettingSsh FromDefault()
        {
            DbItemSettingSsh itemSetting = new DbItemSettingSsh
            {
                Cursor = 1,
                FontSize = 12,
                Character = 65001,
                Fallbackkeys = 2,
                MouseAction = 1,
                HomeAndEnd = 1,
                FnAndKeypad = 3,
                OpenMode = 1,
                SizeIndex = 1,
                FontName = "Consolas",
                ColorScheme = "默认配色",
                CRImpliesLF = false,
                LFImpliesCR = false,
                CJKAmbigWide = false,
                CapsLockCyr = false,
            };
            return itemSetting;
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
        /// 默认设置
        /// </summary>
        [BsonIgnore]
        public override DbItemSetting DefaultSetting { get => _defaultSetting; }

        /// <summary>
        /// 获取与默认设置计算后的结果
        /// </summary>
        /// <returns></returns>
        public override DbItemSetting GetLastSetting()
        {
            DbItemSetting lastSetting = new DbItemSettingSsh
            {
                OpenMode = OpenMode == 0 ? _defaultSetting.OpenMode : OpenMode,
                SizeIndex = SizeIndex == 0 ? _defaultSetting.SizeIndex : SizeIndex,
                Cursor = Cursor == 0 ? _defaultSetting.Cursor : Cursor,
                FontName = string.IsNullOrEmpty(FontName) ? _defaultSetting.FontName : FontName,
                FontSize = FontSize == 0 ? _defaultSetting.FontSize : FontSize,
                Character = Character == 0 ? _defaultSetting.Character : Character,
                Fallbackkeys = Fallbackkeys == 0 ? _defaultSetting.Fallbackkeys : Fallbackkeys,
                MouseAction = MouseAction == 0 ? _defaultSetting.MouseAction : MouseAction,
                ColorScheme = string.IsNullOrEmpty(ColorScheme) ? _defaultSetting.ColorScheme : ColorScheme,
                HomeAndEnd = HomeAndEnd == 0 ? _defaultSetting.HomeAndEnd : HomeAndEnd,
                FnAndKeypad = FnAndKeypad == 0 ? _defaultSetting.FnAndKeypad : FnAndKeypad,
                LFImpliesCR = LFImpliesCR == null ? _defaultSetting.LFImpliesCR : LFImpliesCR,
                CRImpliesLF = LFImpliesCR == null ? _defaultSetting.CRImpliesLF : CRImpliesLF,
                CJKAmbigWide = LFImpliesCR == null ? _defaultSetting.CJKAmbigWide : CJKAmbigWide,
                CapsLockCyr = LFImpliesCR == null ? _defaultSetting.CapsLockCyr : CapsLockCyr,
            };
            return lastSetting;
        }
    }
}