using SimpleRemote.Core;
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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleRemote.Container
{
    /// <summary>
    /// LoginStartDialog.xaml 的交互逻辑
    /// </summary>
    public partial class LoginStartDialog : UserControl
    {
        public delegate void LoginClickEvent(object sender, string password);

        public LoginStartDialog()
        {
            InitializeComponent();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                TextBlock_Info.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                TextBlock_Info.Text = "密码不能为空"; return;
            }
            if (!DatabaseServices.IsPassword(PasswordBox.Password))
            {
                TextBlock_Info.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                TextBlock_Info.Text = "密码不正确"; return;
            }
            OnLoginClick?.Invoke(this, PasswordBox.Password);
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Ok_Click(null, null);
            }
        }

        public event LoginClickEvent OnLoginClick;
    }
}
