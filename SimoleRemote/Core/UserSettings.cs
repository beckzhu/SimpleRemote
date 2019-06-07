using Microsoft.Win32;
using SimpleRemote.Modes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml;

namespace SimpleRemote.Core
{
    public static class UserSettings
    {
        private static string _xmlFile;
        private static XmlDocument _xmldoc;
        private static XmlElement _xmlGeneral;
        private static XmlElement _xmlUserHabits;
        private static XmlElement _xmlRemoteTreeView;

        private static string[] _appDataDirs = new string[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SimpleRemote"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleRemote"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data")
        };

        private static string[] _appDataFiles = new string[]
        {
            "RemoteItems.db",
            "UserSettings.xml"
        };

        private static bool _notifyIcon;

        public static void Init()
        {
            try
            {
                //获取数据文件的存储目录
                for (int i = _appDataDirs.Length - 1; i >= 0; i--)
                {
                    if (File.Exists(Path.Combine(_appDataDirs[i], _appDataFiles[0])) && File.Exists(Path.Combine(_appDataDirs[i], _appDataFiles[1])))
                    {
                        AppDataDir = _appDataDirs[i];
                        AppDataLocation = (DataStoreLocation)i;
                        break;
                    }
                }
                if (AppDataDir == null) AppDataDir = _appDataDirs[0];

                //创建并加载xml文件
                _xmlFile = Path.Combine(AppDataDir, "UserSettings.xml");
                _xmldoc = new XmlDocument();

                if (!File.Exists(_xmlFile))
                {
                    _xmldoc.AppendChild(_xmldoc.CreateXmlDeclaration("1.0", "utf-8", null));
                    XmlElement xmlRoot = _xmldoc.CreateElement("Settings");
                    xmlRoot.AppendChild(_xmldoc.CreateElement("General"));
                    xmlRoot.AppendChild(_xmldoc.CreateElement("UserHabits"));
                    _xmldoc.AppendChild(xmlRoot);
                    _xmldoc.Save(_xmlFile);
                }
                _xmldoc.Load(_xmlFile);

                _xmlGeneral = (XmlElement)_xmldoc.DocumentElement.SelectSingleNode("General");
                _xmlUserHabits = (XmlElement)_xmldoc.DocumentElement.SelectSingleNode("UserHabits");
                if (_xmlGeneral == null || _xmlUserHabits == null)
                {
                    throw new Exception("用户配置文件加载失败。\n原因：Xml节点不能为空。");
                }
                //使用习惯
                MainWindow_Width = _xmlUserHabits.GetAttrDouble("MainWindow_Width");
                if (MainWindow_Width < 800) MainWindow_Width = 1000;
                MainWindow_Height = _xmlUserHabits.GetAttrDouble("MainWindow_Height");
                if (MainWindow_Height < 600) MainWindow_Height = 800;
                MainWindow_Maximize = _xmlUserHabits.GetAttrBool("MainWindow_Maximize");
                Home_IsPaneOpen = _xmlUserHabits.GetAttrBool("Home_IsPaneOpen", true);
                HomeRemoteTreeView_Width = new GridLength(_xmlUserHabits.GetAttrDouble("HomeRemoteTreeView_Width"));
                if (HomeRemoteTreeView_Width.Value < 200) HomeRemoteTreeView_Width = new GridLength(260);
                FinalCheckDateTime = _xmlUserHabits.GetAttrDateTime("FinalCheckDateTime");
                //常规设置
                NotifyIcon = _xmlGeneral.GetAttrBool("TrayIcon");
                NewAppDataLocation = (DataStoreLocation)_xmlGeneral.GetAttrInt("NewAppDataLocation");

                if (NewAppDataLocation != AppDataLocation)//数据位置被改变
                {
                    MoveDataToNewLocation();
                }
                //远程列表展开的节点
                RemoteTreeExpand = new List<string>();
                _xmlRemoteTreeView = (XmlElement)_xmlUserHabits.SelectSingleNode("RemoteTreeView");
                if (_xmlRemoteTreeView == null)
                {
                    _xmlRemoteTreeView = _xmldoc.CreateElement("RemoteTreeView");
                    _xmlUserHabits.AppendChild(_xmlRemoteTreeView);
                }
                var expandNodes = _xmlRemoteTreeView.SelectNodes("ExpandItem");
                foreach (var node in expandNodes)
                {
                    XmlElement xmlElement = node as XmlElement;
                    if (xmlElement != null)
                    {
                        RemoteTreeExpand.Add(xmlElement.GetAttribute("uuid"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"加载用户配置文件失败。\n原因：{e.Message}");
            }
        }

        public static void Save()
        {
            if (_xmldoc == null) return;
            if (_xmlUserHabits != null)
            {
                _xmlUserHabits.SetAttribute("MainWindow_Width", MainWindow_Width);
                _xmlUserHabits.SetAttribute("MainWindow_Height", MainWindow_Height);
                _xmlUserHabits.SetAttribute("MainWindow_Maximize", MainWindow_Maximize);
                _xmlUserHabits.SetAttribute("Home_IsPaneOpen", Home_IsPaneOpen);
                _xmlUserHabits.SetAttribute("HomeRemoteTreeView_Width", HomeRemoteTreeView_Width);
                _xmlUserHabits.SetAttribute("FinalCheckDateTime", FinalCheckDateTime.ToString("yyyy.MM.dd"));
            }
            if (_xmlGeneral != null)
            {
                _xmlGeneral.SetAttribute("TrayIcon", NotifyIcon);
                _xmlGeneral.SetAttribute("NewAppDataLocation", (int)NewAppDataLocation);
            }
            if (_xmlRemoteTreeView != null)
            {
                _xmlRemoteTreeView.RemoveAll();
                foreach (var item in RemoteTreeExpand)
                {
                    XmlElement xmlElement = _xmldoc.CreateElement("ExpandItem");
                    xmlElement.SetAttribute("uuid", item);
                    _xmlRemoteTreeView.AppendChild(xmlElement);
                }
            }
            _xmldoc?.Save(_xmlFile);
        }

        /// <summary>获取数据存储目录</summary>
        public static string AppDataDir { get; internal set; }

        /// <summary>数据存储位置</summary>
        public static DataStoreLocation AppDataLocation { get; internal set; }

        /// <summary>新数据存储目录</summary>
        public static DataStoreLocation NewAppDataLocation { internal get; set; }

        /// <summary>主窗口的宽度</summary>
        public static double MainWindow_Width { get; set; }

        /// <summary>主窗口的高度</summary>
        public static double MainWindow_Height { get; set; }

        /// <summary>主窗口的状态,是否最大化</summary>
        public static bool MainWindow_Maximize { get; set; }

        /// <summary>主页HamburgerMenu 是否展开</summary>
        public static bool Home_IsPaneOpen { get; set; }

        /// <summary>远程条目列表展开的条目</summary>
        public static List<string> RemoteTreeExpand { get; internal set; }

        /// <summary>主页-远程桌面 TreeView 宽度</summary>
        public static GridLength HomeRemoteTreeView_Width { get; set; }

        /// <summary>显示托盘图标</summary>
        public static bool NotifyIcon
        {
            #region 托盘图标

            get => _notifyIcon;
            set
            {
                _notifyIcon = value;
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                if (value) mainWindow.ShowNotifyIcon();
                else mainWindow.CloseNotifyIcon();
            }

            #endregion 托盘图标
        }

        /// <summary>最后检查日期</summary>
        public static DateTime FinalCheckDateTime { get; set; }

        /// <summary>开机自启</summary>
        public static bool Bootup
        {
            #region 设置开机自启

            get
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                string value = regKey.GetValue("SimoleRemote")?.ToString();
                return string.Compare(value, Process.GetCurrentProcess().MainModule.FileName) == 0;
            }
            set
            {
                if (value)
                {
                    RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                    regKey.SetValue("SimoleRemote", Process.GetCurrentProcess().MainModule.FileName);
                }
                else
                {
                    RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                    if (regKey.GetValue("SimoleRemote") != null)
                        regKey.DeleteValue("SimoleRemote");
                }
            }

            #endregion 设置开机自启
        }

        /// <summary>拷贝数据文件移动到AppDataLocation目录,拷贝完成后会重启应用程序</summary>
        public static void MoveDataToNewLocation()
        {
            #region 迁移用户数据文件

            try
            {
                string dataDir = _appDataDirs[(int)NewAppDataLocation];
                if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
                foreach (var file in _appDataFiles)
                {
                    string newFile = Path.Combine(dataDir, file);
                    if (File.Exists(newFile)) File.Delete(newFile);
                    File.Move(Path.Combine(AppDataDir, file), newFile);
                }

                try
                {
                    Directory.Delete(AppDataDir);
                }
                catch { }
                AppDataDir = dataDir;
                _xmlFile = Path.Combine(AppDataDir, "UserSettings.xml");
                AppDataLocation = NewAppDataLocation;
            }
            catch (Exception e)
            {
                throw new Exception($"迁移用应用数据文件时出错，{e.Message}");
            }

            #endregion 迁移用户数据文件
        }
    }
}