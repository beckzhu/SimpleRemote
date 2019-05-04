using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace AutoUpdate
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            args = new string[3];
            args[0] = @"C:\ProgramData\SimpleRemote\Update.zip";
            args[1] = @"D:\编程开发\C#\远程桌面管理工具\SimoleRemote\bin\Debug";
            args[2] = @"D:\编程开发\C#\远程桌面管理工具\SimoleRemote\bin\Debug\SimpleRemote.exe";
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
#endif
            string inFile = args[0], outDir = args[1], mainFile = args[2];
            string unMainFile = Path.Combine(outDir, "SimpleRemote.exe");

            Exception exception = null;
            if (args.Length > 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        UnZip(inFile, outDir, true, null);
                        if (string.Compare(unMainFile, mainFile,true) != 0)
                        {
                            if (File.Exists(mainFile)) File.Delete(mainFile);
                            File.Move(unMainFile, mainFile);
                        }
                        Process.Start(mainFile);
                        File.Delete(inFile);
                        return;
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                    System.Threading.Thread.Sleep(500);
                }
                ErrorForm errorForm = new ErrorForm(exception.Message);
                errorForm.ShowDialog();
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string resourceName = $"Program.Lib.{new AssemblyName(args.Name).Name}.dll";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }

        /// <summary>
        /// 解压缩一个 zip 文件。
        /// </summary>
        /// <param name="zipedFile">压缩文件</param>
        /// <param name="strDirectory">解压目录</param>
        /// <param name="overWrite">是否覆盖已存在的文件。</param>
        /// <param name="password">zip 文件的密码。</param>
        public static void UnZip(string zipedFile, string strDirectory, bool overWrite, string password)
        {
            try
            {
                if (strDirectory == "")
                    strDirectory = Directory.GetCurrentDirectory();
                if (!strDirectory.EndsWith("\\"))
                    strDirectory = strDirectory + "\\";

                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipedFile)))
                {
                    if (password != null)
                    {
                        s.Password = password;
                    }
                    ZipEntry theEntry;

                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        string directoryName = "";
                        string pathToZip = "";
                        pathToZip = theEntry.Name;

                        if (pathToZip != "")
                            directoryName = Path.GetDirectoryName(pathToZip) + "\\";

                        string fileName = Path.GetFileName(pathToZip);

                        Directory.CreateDirectory(strDirectory + directoryName);

                        if (fileName != "")
                        {
                            if ((File.Exists(strDirectory + directoryName + fileName) && overWrite) || (!File.Exists(strDirectory + directoryName + fileName)))
                            {
                                using (FileStream streamWriter = File.Create(strDirectory + directoryName + fileName))
                                {
                                    int size = 2048;
                                    byte[] data = new byte[2048];
                                    while (true)
                                    {
                                        size = s.Read(data, 0, data.Length);

                                        if (size > 0)
                                            streamWriter.Write(data, 0, size);
                                        else
                                            break;
                                    }
                                    streamWriter.Close();
                                }
                            }
                        }
                    }

                    s.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"解压文件失败,{e.Message}");
            }
        }
    }
}
