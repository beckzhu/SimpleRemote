using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SimpleRemote.Bll
{
    public static class ExMethod
    {
        /// <summary>
        /// 获取鼠标位置下的元素
        /// </summary>
        public static T GetElementFromPoint<T>(this ItemsControl itemsControl, Point point) where T : class
        {
            UIElement element = itemsControl.InputHitTest(point) as UIElement;
            while (element != null)
            {
                if (element == itemsControl)
                    return default(T);
                object item = itemsControl.ItemContainerGenerator.ItemFromContainer(element);
                if (!item.Equals(DependencyProperty.UnsetValue))
                    return item as T;
                if (element is T)
                {
                    return element as T;
                }
                element = (UIElement)VisualTreeHelper.GetParent(element);
            }
            return default(T);
        }
        /// <summary>
        /// 获取当前鼠标下的元素
        /// </summary>
        public static T GetElementUnderMouse<T>(this UIElement uIElement) where T : UIElement
        {
            UIElement parent = Mouse.DirectlyOver as UIElement;
            while (parent != null)
            {
                var correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        /// <summary>
        /// 获取元素的父元素
        /// </summary>
        public static T GetElementParent<T>(this UIElement uIElement) where T : UIElement
        {
            UIElement parent = uIElement;
            while (parent != null)
            {
                var correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        /// <summary>  
        /// 获得指定元素的所有子元素  
        /// </summary>  
        public static List<T> GetChildObjects<T>(this DependencyObject obj) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T)
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child));
            }
            return childList;
        }
    }
}
