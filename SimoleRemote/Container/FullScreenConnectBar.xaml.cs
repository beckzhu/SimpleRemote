using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace SimpleRemote.Container
{
    /// <summary>
    /// MsRdpToolStrips.xaml 的交互逻辑
    /// </summary>
    public partial class FullScreenConnectBar : Window
    {
        private double anchorX;

        public FullScreenConnectBar()
        {
            InitializeComponent();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point pt = PointToScreen(e.GetPosition(this));
                Left += (pt.X - anchorX);
                anchorX = pt.X;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = PointToScreen(e.GetPosition(this));
            anchorX = pt.X;
            CaptureMouse();
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DoubleAnimation bAnimation = null;
            if ((bool)e.NewValue)
            {
                bAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                Opacity = 0;
            }
            else
            {
                bAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            }
            BeginAnimation(OpacityProperty, bAnimation);
        }
    }
}