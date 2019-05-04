using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml;

namespace SimpleRemote.Bll
{
    public static class UserSettings
    {
        private static string _dataFile;
        private static XmlDocument _xmldoc;
        private static XmlElement _xmlGeneral;
        private static XmlElement _xmlUserHabits;

        private static bool _notifyIcon;

        public static void Open()
        {
            try
            {
                AppdDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\SimpleRemote";
                if (!Directory.Exists(AppdDataDir)) Directory.CreateDirectory(AppdDataDir);

                _dataFile = $"{AppdDataDir}\\UserSettings.xml";
                _xmldoc = new XmlDocument();

                if (!File.Exists(_dataFile))
                {
                    _xmldoc.AppendChild(_xmldoc.CreateXmlDeclaration("1.0", "utf-8", null));
                    XmlElement xmlRoot = _xmldoc.CreateElement("Settings");
                    xmlRoot.AppendChild(_xmldoc.CreateElement("General"));
                    xmlRoot.AppendChild(_xmldoc.CreateElement("UserHabits"));
                    _xmldoc.AppendChild(xmlRoot);
                    _xmldoc.Save(_dataFile);
                }
                _xmldoc.Load(_dataFile);

                _xmlGeneral = (XmlElement)_xmldoc.DocumentElement.SelectSingleNode("General");
                _xmlUserHabits = (XmlElement)_xmldoc.DocumentElement.SelectSingleNode("UserHabits");
                if (_xmlGeneral == null || _xmlUserHabits == null) throw new Exception("用户配置文件加载失败。\n原因：Xml节点不能为空。");

                double.TryParse(_xmlUserHabits.GetAttribute("MainWindow_Width"), out double value);
                MainWindow_Width = value; if (MainWindow_Width < 800) MainWindow_Width = 1000;

                double.TryParse(_xmlUserHabits.GetAttribute("MainWindow_Height"), out value);
                MainWindow_Height = value; if (MainWindow_Height < 600) MainWindow_Height = 800;

                MainWindow_Maximize = _xmlUserHabits.GetAttribute("MainWindow_Maximize") == true.ToString();

                Home_IsPaneOpen = _xmlUserHabits.GetAttribute("Home_IsPaneOpen") == true.ToString();
                if (string.IsNullOrEmpty(_xmlUserHabits.GetAttribute("Home_IsPaneOpen"))) Home_IsPaneOpen = true;

                double.TryParse(_xmlUserHabits.GetAttribute("HomeRemoteTreeView_Width"), out value);
                HomeRemoteTreeView_Width = new GridLength(value); if (HomeRemoteTreeView_Width.Value < 200) HomeRemoteTreeView_Width = new GridLength(260);

                DateTime.TryParse(_xmlUserHabits.GetAttribute("FinalCheckDateTime"), out DateTime dateTime);
                FinalCheckDateTime = dateTime;

                NotifyIcon = _xmlGeneral.GetAttribute("TrayIcon") == true.ToString();
            }
            catch (Exception e)
            {
                throw new Exception($"加载用户配置文件失败。\n原因：{e.Message}");
            }
        }
        public static void Save()
        {
            _xmlUserHabits.SetAttribute("MainWindow_Width", MainWindow_Width.ToString());
            _xmlUserHabits.SetAttribute("MainWindow_Height", MainWindow_Height.ToString());
            _xmlUserHabits.SetAttribute("MainWindow_Maximize", MainWindow_Maximize.ToString());
            _xmlUserHabits.SetAttribute("Home_IsPaneOpen", Home_IsPaneOpen.ToString());
            _xmlUserHabits.SetAttribute("HomeRemoteTreeView_Width", HomeRemoteTreeView_Width.ToString());
            _xmlUserHabits.SetAttribute("FinalCheckDateTime", FinalCheckDateTime.ToString("yyyy.MM.dd"));
            _xmlGeneral.SetAttribute("TrayIcon", NotifyIcon.ToString());
            _xmldoc.Save(_dataFile);
        }
        /// <summary>获取数据存储目录</summary>
        public static string AppdDataDir { get; internal set; }
        /// <summary>主窗口的宽度</summary>
        public static double MainWindow_Width { get; set; }
        /// <summary>主窗口的高度</summary>
        public static double MainWindow_Height { get; set; }
        /// <summary>主窗口的状态,是否最大化</summary>
        public static bool MainWindow_Maximize { get; set; }
        /// <summary>主页HamburgerMenu 是否展开</summary>
        public static bool Home_IsPaneOpen { get; set; }
        /// <summary>主页-远程桌面 TreeView 宽度</summary>
        public static GridLength HomeRemoteTreeView_Width { get; set; }
        /// <summary>显示托盘图标</summary>
        public static bool NotifyIcon
        {
            get => _notifyIcon;
            set
            {
                _notifyIcon = value;
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                if (value) mainWindow.ShowNotifyIcon();
                else mainWindow.CloseNotifyIcon();
            }
        }
        /// <summary>最后检查日期</summary>
        public static DateTime FinalCheckDateTime { get; set; }
        /// <summary>开机自启</summary>
        public static bool Bootup
        {
            get
            {
                RegistryKey run = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                string value = run.GetValue("RemoteManage")?.ToString();
                return string.Equals(value, Process.GetCurrentProcess().MainModule.FileName);
            }
            set
            {
                RegistryKey run = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                run.SetValue("RemoteManage", Process.GetCurrentProcess().MainModule.FileName);
            }
        }
    }
}
