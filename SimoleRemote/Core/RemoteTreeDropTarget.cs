using SimpleRemote.Controls.WpfDragDrop;
using SimpleRemote.Controls;
using SimpleRemote.Modes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleRemote.Core
{
    public class RemoteTreeDropTarget : IDropTarget
    {
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (DefaultDropHandler.CanAcceptData(dropInfo))
            {
                var isTreeViewItem = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter) && dropInfo.VisualTargetItem is TreeViewItem;
                RemoteTreeViewItem srcItem = dropInfo.Data as RemoteTreeViewItem;
                RemoteTreeViewItem destItem = dropInfo.TargetItem as RemoteTreeViewItem;
                if (destItem != null)
                {
                    if (destItem.RemoteType != RemoteType.dir && destItem.Parent == srcItem.Parent) return;
                    if (destItem == srcItem.Parent) return;
                }
                else
                {
                    if (srcItem.Parent is RemoteTreeViewItem == false) return;
                }
                dropInfo.Effects = DefaultDropHandler.ShouldCopyData(dropInfo) ? DragDropEffects.Copy : DragDropEffects.Move;
                if (isTreeViewItem) dropInfo.DropTargetAdorner = typeof(RemooeTreeHighlightAdorner);
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (dropInfo == null || dropInfo.DragInfo == null) return;

            RemoteTreeViewItem srcItem = dropInfo.Data as RemoteTreeViewItem;
            RemoteTreeViewItem destItem = dropInfo.TargetItem as RemoteTreeViewItem;
            RemoteItems.Move(srcItem, destItem);
        }
    }

    public class RemooeTreeHighlightAdorner : DropTargetAdorner
    {
        [Obsolete("This constructor is obsolete and will be deleted in next major release.")]
        public RemooeTreeHighlightAdorner(UIElement adornedElement)
            : base(adornedElement, (DropInfo)null)
        {
        }

        public RemooeTreeHighlightAdorner(UIElement adornedElement, DropInfo dropInfo)
            : base(adornedElement, dropInfo)
        {
        }

        /// <summary>
        /// 重画鼠标滑过时的样式
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var dropInfo = this.DropInfo;
            var visualTargetItem = dropInfo.VisualTargetItem;
            if (visualTargetItem != null)
            {
                var rect = Rect.Empty;

                var tvItem = visualTargetItem as TreeViewItem;
                if (tvItem != null && VisualTreeHelper.GetChildrenCount(tvItem) > 0)
                {
                    var descendant = VisualTreeHelper.GetDescendantBounds(tvItem);
                    var translatePoint = tvItem.TranslatePoint(new Point(), this.AdornedElement);
                    var itemRect = new Rect(translatePoint, tvItem.RenderSize);
                    descendant.Union(itemRect);
                    rect = new Rect(translatePoint, new Size(descendant.Width - translatePoint.X, tvItem.GetChildElement<Border>().ActualHeight));
                }
                if (rect.IsEmpty)
                {
                    rect = new Rect(visualTargetItem.TranslatePoint(new Point(), this.AdornedElement), VisualTreeHelper.GetDescendantBounds(visualTargetItem).Size);
                }
                var brush = MahApps.Metro.Controls.ItemHelper.GetHoverBackgroundBrush(visualTargetItem);
                drawingContext.DrawRectangle(brush, null, rect);
            }
        }
    }
}
