using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace SimpleRemote.Core
{
    public static class CommonExtended
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
        public static List<T> GetChildElements<T>(this DependencyObject obj, bool directChild = false) where T : FrameworkElement
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
                if (!directChild) childList.AddRange(GetChildElements<T>(child));
            }
            return childList;
        }

        public static T GetChildElement<T>(this DependencyObject obj) where T : FrameworkElement
        {
            DependencyObject child = null;

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T)
                {
                    return (T)child;
                }
                child = GetChildElement<T>(child);
                if (child != null) return (T)child;
            }
            return null;
        }

        /// <summary>
        /// 将指定集合的元素添加到 ItemCollection 的末尾。
        /// </summary>
        public static void AddRange<T>(this ItemCollection obj, IEnumerable<T> newitem)
        {
            if (newitem == null) return;
            foreach (var item in newitem)
            {
                obj.Add(item);
            }
        }

        //Xml扩展方法
        public static double GetAttrDouble(this XmlElement element, string name)
        {
            double.TryParse(element.GetAttribute(name), out double value);
            return value;
        }

        public static int GetAttrInt(this XmlElement element, string name)
        {
            int.TryParse(element.GetAttribute(name), out int value);
            return value;
        }

        public static bool GetAttrBool(this XmlElement element, string name, bool defaule = false)
        {
            try
            {
                return bool.Parse(element.GetAttribute(name));
            }
            catch { }
            return defaule;
        }

        public static DateTime GetAttrDateTime(this XmlElement element, string name)
        {
            DateTime.TryParse(element.GetAttribute(name), out DateTime value);
            return value;
        }

        public static void SetAttribute(this XmlElement element, string name, object value)
        {
            element.SetAttribute(name, value.ToString());
        }
    }
}