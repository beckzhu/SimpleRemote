using SimpleRemote.Core;
using SimpleRemote.Modes;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SimpleRemote.Container.RemoteSetting
{
    /// <summary>
    /// RdpPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPutty : SettingControl
    {
        public SettingPutty()
        {
            InitializeComponent();
            PART_FontName.Items.AddRange(CommonServices.EqualWidthFonts);//加载系统拥有的等宽字体
            for (int i = 8; i < 33; i++) PART_FontSize.Items.Add(i);//加载字体大小
            PART_Character.Items.AddRange(CommonServices.Encodings.Select(m => m.DisplayName));//加载系统支持的字符集
            PART_Color.Items.AddRange(CommonServices.PuttyColorlNames); //加载配色方案
        }

        public override void Loaded(DbItemRemoteLink itemRemoteLink)
        {
            PART_Utility.Loaded(itemRemoteLink);
        }

        public override void UnLoaded()
        {
            PART_Utility.UnLoaded();
        }
    }

    internal class FontNameConverIndex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = Array.IndexOf(CommonServices.EqualWidthFonts, value) + 1;
            return index < 0 ? 0 : index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value - 1;
            if (index >= 0 && index < CommonServices.EqualWidthFonts.Length)
            {
                return CommonServices.EqualWidthFonts[index];
            }
            return string.Empty;
        }
    }

    internal class FontSizeConverIndex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value - 7;
            return index < 0 ? 0 : index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int size = (int)value + 7;
            return size < 8 ? 0 : size;
        }
    }
    
    internal class CharacterConverIndex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = Array.FindIndex(CommonServices.Encodings, m => m.CodePage == (int)value) + 1;
            return index < 0 ? 0 : index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value - 1;
            if (index >= 0 && index < CommonServices.Encodings.Length)
            {
                return CommonServices.Encodings[index].CodePage;
            }
            return 0;
        }
    }
    internal class ColorConverIndex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = Array.IndexOf(CommonServices.PuttyColorlNames, value) + 1;
            return index < 0 ? 0 : index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value - 1;
            if (index >= 0 && index < CommonServices.PuttyColorlNames.Length)
            {
                return CommonServices.PuttyColorlNames[index];
            }
            return string.Empty;
        }
    }

}
