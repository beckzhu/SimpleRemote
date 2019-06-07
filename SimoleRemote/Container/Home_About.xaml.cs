using SimpleRemote.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SimpleRemote.Container
{
    /// <summary>
    /// Home_Update.xaml 的交互逻辑
    /// </summary>
    public partial class Home_About : UserControl
    {
        public Home_About()
        {
            InitializeComponent();
            RichRun_Var.Text = $"版 本：{UpdateServices.LocalVersion}({UpdateServices.LocalUpdated.ToString("yyyy.MM.dd")})";
        }
        /// <summary>
        /// 在后台检查更新,当需要强制更新时才返回True
        /// </summary>
        public async Task<bool> CheckUpdates()
        {
            if (Button_Update.Content.ToString() == "检查更新")
            {
                Button_Update.Content = "检查中"; Button_Update.IsEnabled = false;
                try
                {
                    if (await UpdateServices.Check())
                    {
                        if (!UpdateServices.Force) return false;
                        Text_Describe.Text = $"最新版本：{UpdateServices.RemoteVersion}({UpdateServices.RemoteUpdated.ToString("yyyy.MM.dd")})";
                        var logStr = await UpdateServices.GetUpdateLog();
                        TextBox_Log.Visibility = Visibility.Visible;
                        TextBox_Log.Text = logStr;
                        Button_Update.Content = "下载中";
                        await UpdateServices.Download();
                        Button_Update.Content = "立即更新"; Button_Update.IsEnabled = true;
                        return true;
                    }
                }
                catch
                {
                    Button_Update.Content = "检查更新";
                    Button_Update.IsEnabled = true;
                }
            }
            return false;
        }

        private async void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            if (Button_Update.Content.ToString() == "检查更新")
            {
                Button_Update.Content = "检查中"; Button_Update.IsEnabled = false;
                try
                {
                    if (await UpdateServices.Check())
                    {
                        Text_Describe.Text = $"最新版本：{UpdateServices.RemoteVersion}({UpdateServices.RemoteUpdated.ToString("yyyy.MM.dd")})";
                        var logStr = await UpdateServices.GetUpdateLog();
                        TextBox_Log.Visibility = Visibility.Visible;
                        TextBox_Log.Text = logStr;
                        Button_Update.Content = "下载中";
                        await UpdateServices.Download();
                        Button_Update.Content = "立即更新"; Button_Update.IsEnabled = true;
                    }
                    else
                    {
                        Text_Describe.Text = "当前已经是最新版本";
                        Button_Update.Content = "检查更新"; Button_Update.IsEnabled = true;
                    }
                    
                }
                catch (Exception ex)
                {
                    MainWindow.ShowMessageDialog("错误",ex.Message);
                    Button_Update.Content = "检查更新"; Button_Update.IsEnabled = true;
                }
                return;
            }
            if (Button_Update.Content.ToString() == "立即更新")
            {
                try
                {
                    string exeFile = Path.Combine(UserSettings.AppDataDir, "AutoUpdate.exe");
                    File.WriteAllBytes(exeFile, CommonServices.GetCompressResBytes("SimpleRemote.Lib.AutoUpdate.exe.Compress"));
                    string updateFile = Path.Combine(UserSettings.AppDataDir, "Update.zip");
                    string unzipPath = AppDomain.CurrentDomain.BaseDirectory;
                    Process.Start(exeFile, $"{updateFile} {unzipPath} {Assembly.GetEntryAssembly().Location}");
                    Application.Current.MainWindow.Close();
                }
                catch (Exception ex)
                {
                    MainWindow.ShowMessageDialog("错误", ex.Message);
                    Button_Update.Content = "检查更新"; Button_Update.IsEnabled = true;
                }
            }
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            if (link != null)
            {
                Process.Start(link.NavigateUri.ToString());
            }
        }
    }
}
