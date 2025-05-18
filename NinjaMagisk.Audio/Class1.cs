using NinjaMagisk.Runtimes;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
namespace NinjaMagisk
{
    namespace Audio
    {
        /* 解析酷狗音乐.kgg加密音乐 拓展库
         * 此工具需要依赖于 NinjaMagisk 主程序
         */
        /// <summary>
        /// 解析酷狗音乐.kgg文件
        /// </summary>
        public class PaserKGG
        {
            /// <summary>
            /// 酷狗音乐数据库路径
            /// </summary>
            static string database = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\KuGou8\\KGMusicV3.db";
            /// <summary>
            /// 读取加密文件
            /// </summary>
            /// <param name="filepath"></param>
            public static void ReadKGGFile(string filepath)
            {
                Process(filepath, database); // 解析文件 
            }
            /// <summary>
            /// 读取加密文件
            /// </summary>
            public static void ChooseSigleKGGFile()
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "KGG文件|*.kgg",
                    Title = "选择KGG文件",
                    // 只能选择单个文件
                    Multiselect = false,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                };
                openFileDialog.ShowDialog();
                string file = openFileDialog.FileName;
                Process(file, database); // 解析文件
            }
            /// <summary>
            /// 处理文件
            /// </summary>
            /// <param name="file"></param>
            /// <param name="database"></param>
            private static void Process(string file, string database)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                // 假设 kgg_dec.exe是嵌入在"Namespace.Resources"命名空间中的
                string resourceName = "NinjaMagisk.Audio.Properties.Resources"; // 替换为你的资源路径
                // 创建 ResourceManager 实例
                ResourceManager rm = new ResourceManager(resourceName, assembly);
                // 从资源中获取aria2c.exe文件的字节数据
                byte[] kggdec = (byte[])rm.GetObject("kgg_dec");
                if (kggdec != null)
                {
                    try
                    {
                        // 将字节数据写入临时文件
                        string tempFilePath = Path.Combine(Path.GetTempPath(), "kgg_dec.exe");
                        System.IO.File.WriteAllBytes(tempFilePath, kggdec);
                        // 设置临时文件为可执行
                        System.IO.File.SetAttributes(tempFilePath, FileAttributes.Normal);
                        // "C:\Users\Administrator\AppData\Roaming\KuGou8\KGMusicV3.db"

                        if (!System.IO.File.Exists(database))
                        {
                            MessageBox.Show("未能找到酷狗音乐数据库,请在单击\"确认\"后手动指定 \"KGMusicV3.db\" 文件位置 \n 如果您没有安装酷狗音乐,请在安装包内提取");

                        database:
                            OpenFileDialog db = new OpenFileDialog();
                            db.Filter = "KGMusicV3.db文件|KGMusicV3.db";
                            db.Title = "选择 KGMusicV3.db 文件";
                            db.CheckFileExists = true;
                            db.CheckPathExists = true;
                            db.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                            db.ShowDialog();
                            string dbPath = db.FileName;
                            if (dbPath == "")
                            {
                                MessageBox.Show("未能找到酷狗音乐数据库,请在单击\"确认\"后手动指定 \"KGMusicV3.db\" 文件位置");
                                goto database;
                            }
                            else
                            {
                                database = dbPath;
                                Parse(tempFilePath, file, database);
                            }
                        }

                        Parse(tempFilePath, file, database); // 解析文件
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("解密文件失败" + ex);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("资源文件不存在");
                    return;
                }
                if (file.Length == 0)
                {
                    MessageBox.Show("没有选择文件");
                    return;
                }

            }
            /// <summary>
            /// 启动程序解密文件
            /// </summary>
            /// <param name="path"> 解密程序路径</param>
            /// <param name="file"> 待解密文件</param>
            /// <param name="database"> 数据库路径</param>
            private static void Parse(string path, string file, string database)
            {
                Process process = new Process();
                process.StartInfo.FileName = path;
                process.StartInfo.Arguments = $"\"{file}\" --db {database} --suffix \"\" ";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();

                // 提取 file 的 文件名 和不带文件名的路径
                string fileName = Path.GetFileName(file);
                string filePath = Path.GetDirectoryName(file);

                // 假设加密文件为 example.kgg 解密后的文件为 example.ogg
                // 识别目录里是否有解密后的文件
                string decryptedFileName = Path.Combine(filePath, fileName.Substring(0, fileName.Length - 4) + ".ogg");
                if (System.IO.File.Exists(decryptedFileName))
                {
                    MessageBox.Show("解密成功");
                    Windows.Explorer.OpenFolderInExplorer(filePath);
                }
                else
                {
                    MessageBox.Show("解密失败");
                }
            }
        }
        public class PaserKGM
        {
            /// <summary>
            /// 读取目录下的所有加密文件
            /// </summary>
            /// <param name="filepath"> 目录路径</param>
            public static void ReadKGMFile(string filepath)
            {
                string path = Process();
                // 等待Process完成
                ParseAll(path, filepath);
                return;
            }
            public static void ChooseSigleKGMFile()
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "KGM文件|*.kgm",
                    Title = "选择KGM文件",
                    // 只能选择单个文件
                    Multiselect = false,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                };
                openFileDialog.ShowDialog();
                string file = openFileDialog.FileName;
                string path = Process();
                Parse(path, file); // 解析文件
            }
            /// <summary>
            /// 解密程序路径
            /// </summary>
            /// <returns> 解密程序路径</returns>
            private static string Process()
            {
                string tempOncePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                if (!Directory.Exists(tempOncePath))
                {
                    Directory.CreateDirectory(tempOncePath);
                }
                Assembly assembly = Assembly.GetExecutingAssembly();
                // 假设 kgg_dec.exe是嵌入在"Namespace.Resources"命名空间中的
                string resourceName = "NinjaMagisk.Audio.Properties.Resources"; // 替换为你的资源路径
                // 创建 ResourceManager 实例
                ResourceManager rm = new ResourceManager(resourceName, assembly);
                // Fix for the line causing CS0029, CS1003, and CS1525 errors
                byte[] kgm = (byte[])rm.GetObject(Environment.Is64BitOperatingSystem ? "kgm_x64" : "kgm_x86");
                byte[] mask = (byte[])rm.GetObject("mask");
                if (kgm != null || mask != null)
                {
                    try
                    {
                        // 在临时目录中创建随机文件夹合并成临时文件路径
                        string kgmPath = Path.Combine(tempOncePath, "unlock-kugou-windows-amd64-alpha2.exe");
                        string maskPath = Path.Combine(tempOncePath, "kgm.mask");
                        // 将字节数据写入临时文件
                        System.IO.File.WriteAllBytes(kgmPath, kgm);
                        System.IO.File.WriteAllBytes(maskPath, mask);
                        // 设置临时文件为可执行
                        //System.IO.File.SetAttributes(kgmPath, FileAttributes.Normal);
                        return kgmPath;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("解密文件失败" + ex);
                        return null;
                    }
                }
                return null;
            }
            /// <summary>
            /// 启动程序解密文件
            /// </summary>
            /// <param name="path"> 解密程序路径</param>
            /// <param name="folder"> 待解密文件目录</param>
            private static void ParseAll(string path, string folder)
            {
                Process process = new Process();
                process.StartInfo.FileName = path;
                process.StartInfo.WorkingDirectory = folder;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                LogLibraries.WriteLog(LogLibraries.LogLevel.Info, "已启动,请等待完成");
                process.WaitForExit();
                string[] files = Directory.GetFiles(folder + "\\kgm-vpr-out", "*.mp3");
                if (System.IO.File.Exists(files[0]))
                {
                    process.Dispose();
                    MessageBox.Show("解密成功");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"解密成功: {folder}kgm-vpr-out");
                    Windows.Explorer.OpenFolderInExplorer(Path.Combine(folder,"kgm-vpr-out"));
                    return;
                }
                process.Dispose();

                return;
            }
            private static void Parse(string path, string file)
            {
                // 复制(多个)file 到 path
                string musicPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(file));
                System.IO.File.Copy(file, musicPath, true);
                Thread.Sleep(5000);
                // 复制之后启动程序自动加密到本目录\kgm-vpr-out

                Process process = new Process();
                process.StartInfo.FileName = path;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
                process.StartInfo.UseShellExecute = false;
                //process.StartInfo.CreateNoWindow = true;
                //process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                // 等待主程序5秒
                Thread.Sleep(5000);
                // 向打开的程序发送 Enter
                SendKeys.SendWait("{ENTER}");
                process.WaitForExit();
                //Console.WriteLine(process.StandardOutput.ReadToEnd());
                // 对 path 增加目录 kgm-vpr-out
                string outPath = Path.Combine(Path.GetDirectoryName(path), "kgm-vpr-out");
                string fileName = Path.GetFileName(file);
                // filename 提取不带后缀的文件名
                fileName = fileName.Substring(0, fileName.Length - 4);
                // 解密后的文件后缀为 .mp3, 检测是否存在
                string decryptedFileName = Path.Combine(outPath, fileName + ".mp3");
                if (System.IO.File.Exists(decryptedFileName))
                {
                    // 复制解密后的文件到原目录
                    System.IO.File.Copy(decryptedFileName, Path.Combine(Path.GetDirectoryName(file), fileName + ".mp3"), true);
                    MessageBox.Show("解密成功");
                    Windows.Explorer.OpenFolderInExplorer(Path.GetDirectoryName(file));
                }
                else
                {
                    MessageBox.Show("解密失败");
                }
            }
        }
    }
}