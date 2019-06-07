using System;
using System.Threading;
using System.Windows;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// A class that represents the settings used by Metro Dialogs.
    /// </summary>
    public class MetroDialogSettings
    {
        public MetroDialogSettings()
        {
            this.OwnerCanCloseWithDialog = true;

            this.AffirmativeButtonText = "确认";
            this.NegativeButtonText = "取消";

            this.ColorScheme = MetroDialogColorScheme.Theme;
            this.AnimateShow = this.AnimateHide = true;

            this.MaximumBodyHeight = Double.NaN;

            this.DefaultText = "";
            this.DefaultButtonFocus = MessageDialogResult.Negative;
            this.CancellationToken = CancellationToken.None;
            this.DialogTitleFontSize = Double.NaN;
            this.DialogMessageFontSize = Double.NaN;
            this.DialogResultOnCancel = null;
        }

        /// <summary>
        /// 获取或设置对话框的所在的窗口可以关闭.
        /// </summary>
        public bool OwnerCanCloseWithDialog { get; set; }

        /// <summary>
        /// 获取或设置用于按钮的文本。 例如：“确定”或“是”。
        /// </summary>
        public string AffirmativeButtonText { get; set; }

        /// <summary>
        ///启用或禁用对话框隐藏动画
        ///“True” - 播放隐藏动画。
        ///“False” - 跳过隐藏动画。
        /// </summary>
        public bool AnimateHide { get; set; }

        /// <summary>
        ///启用或禁用显示动画的对话框。
        ///“True” - 播放显示动画。
        ///“False” - 跳过显示动画。
        /// </summary>
        public bool AnimateShow { get; set; }

        /// <summary>
        /// Gets or sets a token to cancel the dialog.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        ///获取或设置城域对话框是否应使用默认的黑白外观（主题）.
        /// </summary>
        public MetroDialogColorScheme ColorScheme { get; set; }

        /// <summary>
        /// 获取或设置可包含自定义样式，画笔或其他内容的自定义资源字典.
        /// </summary>
        public ResourceDictionary CustomResourceDictionary { get; set; }

        /// <summary>
        /// 获取或设置默认情况下哪个按钮拥有焦点
        /// </summary>
        public MessageDialogResult DefaultButtonFocus { get; set; }

        /// <summary>
        /// 获取或设置默认文本（只限输入对话框）
        /// </summary>
        public string DefaultText { get; set; }

        /// <summary>
        /// 获取或设置对话框消息字体的大小.
        /// </summary>
        /// <value>
        /// 对话消息字体的大小.
        /// </value>
        public double DialogMessageFontSize { get; set; }

        /// <summary>
        /// 获取或设置当用户按下'ESC'键是否关闭对话框
        /// </summary>
        /// <remarks>If the value is <see langword="null"/> the default behavior is determined 
        /// by the <see cref="MessageDialogStyle"/>.
        /// <table>
        /// <tr><td><see cref="MessageDialogStyle"/></td><td><see cref="MessageDialogResult"/></td></tr>
        /// <tr><td><see cref="MessageDialogStyle.Affirmative"/></td><td><see cref="MessageDialogResult.Affirmative"/></td></tr>
        /// <tr><td>
        /// <list type="bullet">
        /// <item><see cref="MessageDialogStyle.AffirmativeAndNegative"/></item>
        /// <item><see cref="MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary"/></item>
        /// <item><see cref="MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary"/></item>
        /// </list></td>
        /// <td><see cref="MessageDialogResult.Negative"/></td></tr></table></remarks>
        public MessageDialogResult? DialogResultOnCancel { get; set; }

        /// <summary>
        /// 获取或设置对话标题字体的大小.
        /// </summary>
        /// <value>
        /// 对话标题字体的大小.
        /// </value>
        public double DialogTitleFontSize { get; set; }

        /// <summary>
        /// 获取或设置用于第一个辅助按钮的文本.
        /// </summary>
        public string FirstAuxiliaryButtonText { get; set; }

        /// <summary>
        ///获取或设置最大高度.（默认是无限高度）, <a href="http://msdn.microsoft.com/de-de/library/system.double.nan">Double.NaN</a>)
        /// </summary>
        public double MaximumBodyHeight { get; set; }

        /// <summary>
        /// 获取或设置用于否定按钮的文本。 例如：“取消”或“否”.
        /// </summary>
        public string NegativeButtonText { get; set; }

        /// <summary>
        /// 获取或设置用于否定按钮的文本。 例如：“取消”或“否”.
        /// </summary>
        public string SecondAuxiliaryButtonText { get; set; }

        /// <summary>
        /// 如果设置，则停止应用于对话框的标准资源字典.
        /// </summary>
        [Obsolete("This property will be deleted in the next release.")]
        public bool SuppressDefaultResources { get; set; }
    }
}