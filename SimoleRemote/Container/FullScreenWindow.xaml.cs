using MahApps.Metro.Controls;
using SimpleRemote.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimpleRemote.Container
{
    /// <summary>
    /// FullScreenWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FullScreenWindow : Window
    {
        private FullScreenConnectBar PART_connectionBar;
        private bool _connectionBar_IsVisible;
        private Rect _connectionBar_Rect;//将连接栏尺寸保存起来，以便跨线程调用
        System.Timers.Timer _timer;

        public FullScreenWindow()
        {
            InitializeComponent();
            PART_connectionBar = new FullScreenConnectBar();
            PART_connectionBar.Left = (Width - PART_connectionBar.Width) / 2;
            _connectionBar_Rect = new Rect(PART_connectionBar.Left, PART_connectionBar.Top, PART_connectionBar.Width, PART_connectionBar.Height);


            PART_connectionBar.Deactivated += ConnectBar_Deactivated;
            PART_connectionBar.IsVisibleChanged += _connectionBar_IsVisibleChanged;
            PART_connectionBar.LocationChanged += _connectionBar_LocationChanged;
        }

        public string ConnectionTitle { get => PART_connectionBar.PART_Title.Text; set => PART_connectionBar.PART_Title.Text = value; }
        public void MouseMoveProc(int x, int y)
        {
            if (!_connectionBar_IsVisible && y < 5 && x > _connectionBar_Rect.Left && x < _connectionBar_Rect.Left + _connectionBar_Rect.Width)
            {
                if (_timer == null)
                {
                    _timer = new System.Timers.Timer(500);
                    _timer.AutoReset = false;
                    _timer.Enabled = true;
                    _timer.Elapsed += (s, e) =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (!PART_connectionBar.IsVisible)
                            {
                                PART_connectionBar.Visibility = Visibility.Visible;
                                var bAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.4)));
                                PART_connectionBar.BeginAnimation(OpacityProperty, bAnimation);
                            }
                        });
                        _timer = null;
                    };
                }
            }
            else
            {
                if (_timer != null)
                {
                    _timer?.Stop();
                    _timer = null;
                }
            }
        }
        public RoutedEventHandler ButtonClick
        {
            set
            {
                PART_connectionBar.PART_Min.Click += value;
                PART_connectionBar.PART_Resize.Click += value;
                PART_connectionBar.PART_Close.Click += value;
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PART_connectionBar.Owner = this;
            PART_connectionBar.Visibility = Visibility;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            PART_connectionBar.Close();
        }
        private void ConnectBar_Deactivated(object sender, EventArgs e)
        {
            if (PART_connectionBar.IsVisible)
            {
                var bAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.4)));
                bAnimation.Completed += (s, a) =>
                  {
                      PART_connectionBar.Visibility = Visibility.Collapsed;
                  };
                PART_connectionBar.BeginAnimation(OpacityProperty, bAnimation);
            }
        }
        private void _connectionBar_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _connectionBar_IsVisible = PART_connectionBar.IsVisible;
        }
        private void _connectionBar_LocationChanged(object sender, EventArgs e)
        {
            _connectionBar_Rect.X = PART_connectionBar.Left;
        }
    }
}
