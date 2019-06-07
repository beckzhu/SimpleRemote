using LiteDB;
using SimpleRemote.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleRemote.Modes
{
    public abstract class DbItemSetting : INotifyPropertyChanged
    {
        /// <summary>
        /// 默认
        /// </summary>
        public const int OPEN_DEFAULT = 0;

        /// <summary>
        /// 选项卡
        /// </summary>
        public const int OPEN_TAB = 1;

        /// <summary>
        /// 后台
        /// </summary>
        public const int OPEN_TAB_BACKSTAGE = 2;

        /// <summary>
        /// 外部窗口
        /// </summary>
        public const int OPEN_WINDOW = 3;

        /// <summary>
        /// 默认
        /// </summary>
        public const int DESKSIZE_DEFAULT = 0;

        /// <summary>
        /// 选项卡
        /// </summary>
        public const int DESKSIZE_AUTO = 1;

        /// <summary>
        /// 后台
        /// </summary>
        public const int DESKSIZE_FULLSCREEN = 2;

        private int _openMode;
        private int _sizeIndex;
        public string Id { get; set; }

        /// <summary>
        /// 默认设置
        /// </summary>
        [BsonIgnore]
        public virtual DbItemSetting DefaultSetting { get; }

        /// <summary>
        /// 获取默认打开方式
        /// </summary>
        public int GetOpenMode()
        {
            return OpenMode == OPEN_DEFAULT ? DefaultSetting.OpenMode : OpenMode;
        }
        /// <summary>
        /// 获取远程桌面是否全屏
        /// </summary>
        public bool GetIsFullscreen()
        {
            return SizeIndex == OPEN_DEFAULT ? DefaultSetting.SizeIndex == DESKSIZE_FULLSCREEN : SizeIndex == DESKSIZE_FULLSCREEN;
        }
        /// <summary>
        /// 获取与 DefaultSetting 计算后的结果
        /// </summary>
        /// <returns></returns>
        public virtual DbItemSetting GetItemSetting()
        {
            return null;
        }

        /// <summary>打开远程的首选方式,参考OPEN_开头常量 </summary>
        public int OpenMode
        {
            get => _openMode; set
            {
                _openMode = value;
                OnPropertyChanged();
            }
        }

        // <summary>远程桌面尺寸与ScreenResolutions对应的尺寸,从1开始 </summary>
        public int SizeIndex
        {
            get => _sizeIndex; set
            {
                _sizeIndex = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 重置所有设置
        /// </summary>
        public virtual void Reset()
        {
            OpenMode = 0;
            SizeIndex = 0;
        }

        /// <summary>
        /// 用另外一个设置来更新当前设置，当源属性值不为空的时候  才会被更新
        /// </summary>
        /// <param name="dbItemSetting"></param>
        public virtual void UpdateValue(DbItemSetting sourceSetting)
        {
            if (sourceSetting.OpenMode != OPEN_DEFAULT) OpenMode = sourceSetting.OpenMode;
            if (sourceSetting.SizeIndex != 0) SizeIndex = sourceSetting.SizeIndex;
        }
        /// <summary>
        /// 将当前设置为默认设置
        /// </summary>
        public virtual void SetDefaultSetting()
        {
           
        }
        /// <summary>
        /// 获取与默认设置计算后的结果
        /// </summary>
        /// <returns></returns>
        public virtual DbItemSetting GetLastSetting()
        {
            return null;
        }
    }
}