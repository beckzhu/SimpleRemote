using SimpleRemote.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Windows.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Reflection;
using SimpleRemote.Bll;

namespace SimpleRemote.Control
{
    /// <summary>
    /// 为Header被编辑事件提供数据。
    /// </summary>
    public class CancelEditEventArgs : CancelEventArgs
    {
        public CancelEditEventArgs(string oldValue, string newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
    /// <summary>
    /// Header被编辑引发的事件。
    /// </summary>
    public delegate void CancelEditEventHandler(object sender, CancelEditEventArgs e);

    public class RemoteTreeViewItem : TreeViewItem
    {
        //属性名称1.属性类型2.该属性所有者3.即将该属性注册到那个类上4.属性默认值
        private static DependencyProperty TypeProperty = DependencyProperty.Register("RemoteType", typeof(RemoteType), typeof(RemoteTreeViewItem), new PropertyMetadata(RemoteType.dir));

        public RemoteTreeViewItem(string header, RemoteType type, string databaseId)
        {
            Header = header;
            RemoteType = type;
            uuid = databaseId;
        }
        /// <summary>远程条目的类型</summary>
        public RemoteType RemoteType
        {
            get { return (RemoteType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        /// <summary>开始编辑控件的名称,资源在控件加载完毕后才有效</summary>
        public void HeaderEdit(CancelEditEventHandler EditHeaderClosing)
        {
            TextBox textBox = GetTemplateChild("PART_HeaderEdit") as TextBox;
            if (textBox != null)
            {
                textBox.Visibility = Visibility.Visible;
                textBox.Text = (string)Header;
                textBox.SelectAll();
                textBox.Focus();
                Window parentWindow = Window.GetWindow(textBox);

                MouseButtonEventHandler parentMouseDown = null;
                parentMouseDown = (sender, e) => 
                {
                    if (parentWindow.GetElementUnderMouse<TextBox>() != textBox)
                    {
                        textBox.Visibility = Visibility.Collapsed;
                        CancelEditEventArgs cancelEditEventArgs = new CancelEditEventArgs((string)Header, textBox.Text);
                        EditHeaderClosing?.Invoke(this, cancelEditEventArgs);
                        if (cancelEditEventArgs.Cancel == false)
                        {
                            Header = cancelEditEventArgs.NewValue;
                        }
                        parentWindow.PreviewMouseDown -= parentMouseDown;
                    }
                };
                parentWindow.PreviewMouseDown += parentMouseDown;
            }
        }
        /// <summary>表头是否处于编辑模式</summary>
        public bool IsHeaderEdit
        {
            get
            {
                TextBox textBox = GetTemplateChild("PART_HeaderEdit") as TextBox;
                if (textBox == null) return false;
                return textBox.IsVisible;
            }
        }
        /// <summary>关联的数据库表条目Id</summary>
        public string uuid { get; set; }
    }
}
