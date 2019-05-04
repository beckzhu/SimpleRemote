using System;
using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// EventArgs used for the <see cref="HamburgerMenu"/> ItemClick and OptionsItemClick event.
    /// </summary>
    public class ItemClickEventArgs : RoutedEventArgs
    {
        public ItemClickEventArgs(object clickedObject)
        {
            ClickedItem = clickedObject;
        }

        /// <summary>
        /// Gets the clicked item
        /// </summary>
        public object ClickedItem { get; internal set; }
    }

    public delegate void ItemClickEventHandler(object sender, ItemClickEventArgs e);

    /// <summary>
    /// EventArgs used for the <see cref="HamburgerMenu"/> ItemInvoked event.
    /// </summary>
    public class HamburgerMenuItemInvokedEventArgs : EventArgs
    {
        /// <summary>
        /// 获取调用的项目
        /// </summary>
        public object InvokedItem { get; internal set; }

        /// <summary>
        /// 获取一个值，该值指示被调用的项目是否为选项
        /// </summary>
        public bool IsItemOptions { get; internal set; }
    }
}
