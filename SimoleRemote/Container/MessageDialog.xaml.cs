using SimpleRemote.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleRemote.Container
{
    /// <summary>
    /// MessageDialog.xaml 的交互逻辑
    /// </summary>
    public partial class MessageDialog : UserControl
    {
        public const int IDOK = 1;//确认
        public const int IDCANCEL = 2;//取消
        public const int IDYES = 6;//是
        public const int IDNO = 7;//否

        public const int MB_OK = 0;//确认
        public const int MB_OKCANCEL = 1;//确认取消
        public const int MB_YESNO = 4;//是否
        public const int MB_YESNOCANCEL = 4;//是、否取消钮

        public delegate void ButtnClick(int btnType);

        private ButtnClick _click;
        private ContentControl _parent;
        private int _dialogRet;
        private object _content;
        private Timer _timer;

        /// <summary>
        /// 创建对话框
        /// </summary>
        /// <param name="title"> 标题</param>
        /// <param name="message">消息</param>
        /// <param name="lpCaption">类型 参考MB_开头常量</param>
        public MessageDialog(string title, string message, int lpCaption, int highlight = IDOK, int timer = 0)
        {
            InitializeComponent();
            Text_Title.Text = title;
            Text_Message.Text = message;
            if (lpCaption == MB_OK)
            {
                Button_Ok.Tag = IDOK;
                Button_Ok.Content = "确认";
                Button_Ok.Visibility = Visibility.Visible;
            }
            if (lpCaption == MB_OKCANCEL)
            {
                Button_Ok.Tag = IDOK;
                Button_Ok.Content = "确认";
                Button_Ok.Visibility = Visibility.Visible;
                Button_No.Tag = IDCANCEL;
                Button_No.Content = "取消";
                Button_No.Visibility = Visibility.Visible;
            }
            if (lpCaption == MB_YESNO)
            {
                Button_Ok.Tag = IDYES;
                Button_Ok.Content = "是";
                Button_Ok.Visibility = Visibility.Visible;
                Button_No.Tag = IDNO;
                Button_No.Content = "否";
                Button_No.Visibility = Visibility.Visible;
            }
            if (lpCaption == MB_YESNOCANCEL)
            {
                Button_Ok.Tag = IDYES;
                Button_Ok.Content = "是";
                Button_Ok.Visibility = Visibility.Visible;
                Button_No.Tag = IDNO;
                Button_No.Content = "否";
                Button_No.Visibility = Visibility.Visible;
                Button_Cancel.Tag = IDCANCEL;
                Button_Cancel.Content = "取消";
                Button_Cancel.Visibility = Visibility.Visible;
            }

            if (highlight == IDOK || highlight == IDYES)
            {
                Button_Ok.SetResourceReference(StyleProperty, "AccentedSquareButtonStyle");
                Button_Ok.Focus();
            }
            if (highlight == IDCANCEL)
            {
                Button_Cancel.SetResourceReference(StyleProperty, "AccentedSquareButtonStyle");
                Button_Cancel.Focus();
            }
            if (highlight == IDNO)
            {
                Button_No.SetResourceReference(StyleProperty, "AccentedSquareButtonStyle");
                Button_No.Focus();
            }

            if (timer > 0)
            {
                Text_Timer.Text = timer.ToString();
                Text_Timer.Tag = timer;
                Text_Timer.Visibility = Visibility.Visible;
            }
        }
        public static void Show(ContentControl parent, string title, string message, int lpCaption, ButtnClick click = null, int highlight = IDOK,int timer=0)
        {
            new MessageDialog(title, message, lpCaption, highlight,timer).Show(parent, click);
        }

        public void Show(ContentControl parent, ButtnClick click)
        {
            if (parent.ActualHeight < 1) return;

            _parent = parent;
            _click = click;
            Row_Dialog.MinHeight = parent.ActualHeight / 4.0 - 110;
            Row_Dialog.MaxHeight = parent.ActualHeight;

            parent.SizeChanged += (s, e) =>
            {
                Row_Dialog.MinHeight = parent.ActualHeight / 4.0 - 110;
                Row_Dialog.MaxHeight = parent.ActualHeight;
            };

            _dialogRet = -1;
            _content = parent.Content;
            parent.Content = null;
            parent.Content = this;

            EventHandler bAnimationCompleted = (sender, e) =>
            {
                DoubleAnimation dAnimation = new DoubleAnimation();
                dAnimation.From = 0;
                dAnimation.To = 1;
                dAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.4));
                if (Text_Timer.Visibility == Visibility.Visible)
                {
                    _timer = new Timer(TimerCallback, null, 1000, 1000);
                }
            };

            DoubleAnimation bAnimation = new DoubleAnimation();
            bAnimation.From = 0;
            bAnimation.To = 1;
            bAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.4));
            bAnimation.Completed += bAnimationCompleted;
            BeginAnimation(OpacityProperty, bAnimation);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _parent.Content = _content;
            _dialogRet = (int)(((Button)sender).Tag);
            _click?.Invoke(_dialogRet);
            _timer?.Dispose();
        }

        private void TimerCallback(object state)
        {
            Dispatcher.Invoke(() =>
            {
                int i = (int)Text_Timer.Tag;
                i--;
                if (i <= 0)
                {
                    _parent.Content = _content;
                    _click?.Invoke(IDCANCEL);
                    return;
                }
                Text_Timer.Tag = i;
                Text_Timer.Text = i.ToString();
            });
        }
    }
}
