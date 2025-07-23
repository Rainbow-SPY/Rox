using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    namespace Runtimes
    {
        /// <summary>
        /// Windows Toast 通知类
        /// </summary>
        public class WindowsToast
        {
            /// <summary>
            /// 提取 WindowsToast 模块,并解压缩到指定路径
            /// </summary>
            /// <param name="ExtractPath"></param>
            public static void ExtractToastModule(string ExtractPath)
            {
                // 获取当前正在执行的类库的程序集
                Assembly assembly = Assembly.GetExecutingAssembly();

                // 假设WindowsToast是嵌入在"Namespace.Resources"命名空间中的

                string resourceName = "Rox.Runtimes.Properties.Resources"; // 替换为你的资源路径
                string path = Path.GetTempPath(); // 获取临时目录路径
                // 创建 ResourceManager 实例
                ResourceManager rm = new ResourceManager(resourceName, assembly);
                WriteLog.Info($"{_NEW_RM}");
                // 从资源中获取WindowsToast文件的字节数据
                byte[] ToastZipData = (byte[])rm.GetObject("WindowsToast");
                WriteLog.Info($"{_GET_RM_OBJ}: WindowsToast Module");
                if (ToastZipData != null)
                {
                    // 检查并创建目录
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        WriteLog.Info($"{_CREATE_DIRECTORY}");
                    }
                    WriteLog.Info($"{_GET_OUTPUT_DIRECTORY}: {path}");
                    // 保存文件路径
                    string outputFilePath = Path.Combine(path, "WindowsToast.zip");
                    WriteLog.Info($"{_GET_OUTPUT_NAME}: {path}");
                    // 写入文件，确保保存为二进制数据
                    WriteLog.Info($"{_FILE_WRITING}");
                    System.IO.File.WriteAllBytes(outputFilePath, ToastZipData);
                    WriteLog.Info($"WindowsToast {_FILE_EXIST_PATH} {outputFilePath}");
                }
                else
                {
                    WriteLog.Error($"{_RES_FILE_NOT_FIND}");
                }
                UnzipToastModuleZip(ExtractPath, path);
            }
            /// <summary>
            /// 解压缩 WindowsToast 模块的 ZIP 文件到指定路径
            /// </summary>
            /// <param name="path"></param>
            /// <param name="OriginalFile"></param>
            private static void UnzipToastModuleZip(string path, string OriginalFile)
            {
                if (path == string.Empty || OriginalFile == string.Empty)
                {
                    WriteLog.Error($"{(path == string.Empty ? "指定的目标路径" : "原始压缩文件路径")} 为null或空值, 将使用默认路径\".\\bin\"");
                    path = $"{Directory.GetCurrentDirectory()}\\bin";
                    if (!Directory.Exists($"{Directory.GetCurrentDirectory()}\\bin"))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                //检查参数是否合法
                if (string.IsNullOrWhiteSpace(path) || Path.GetFileName(path) == string.Empty)
                {
                    WriteLog.Error($"{path}值为null或空字符串");
                    MessageBox.Show($"{path}值为null或空字符串", "错误的路径! - Rox", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }
                else
                {
                    WriteLog.Info($"{_GET_DIRECTORY}: {OriginalFile}");
                }
                // 检查文件是否存在
                string zipFilePath = Path.Combine(OriginalFile, "WindowsToast.zip");
                if (System.IO.File.Exists(zipFilePath))
                {
                    // 解压缩文件
                    Process zip = new Process();
                    zip.StartInfo.FileName = "powershell.exe";
                    zip.StartInfo.Arguments = $"-Command \"Expand-Archive -Path '{zipFilePath}' -DestinationPath '{path}'\"";
                    zip.StartInfo.UseShellExecute = false;
                    zip.StartInfo.CreateNoWindow = true;
                    zip.Start();
                    zip.WaitForExit();
                    if (zip.ExitCode == 0)
                    {
                        WriteLog.Info($"WindowsToast {_DOWNLOADING_COMPLETE}");
                        WriteLog.Info($"{_FILE_EXIST_PATH} {path}");
                    }
                    else
                    {
                        WriteLog.Error($"WindowsToast {_DOWNLOADING_FAILED}");
                        WriteLog.Error($"{zip.ExitCode}");
                    }
                }
                else
                {
                    WriteLog.Error($"WindowsToast {_RES_FILE_NOT_FIND}");
                    MessageBox.Show($"{_RES_FILE_NOT_FIND}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }
            }
            /// <summary>
            /// 发送 Toast 通知
            /// </summary>
            /// <param name="title"> Toast 通知标题</param>
            /// <param name="content"> Toast 通知内容</param>
            public static void PostToastNotification(string title, string content)
            {
                // 检查 ToastNotification 模块是否存在
                if (!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "bin", "WindowsToast.exe")) && !File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "WindowsToast.exe")))
                {
                    WriteLog.Error("WindowsToast 模块未找到，正在提取模块。");
                    ExtractToastModule(Directory.GetCurrentDirectory() + "\\bin");
                    PostToastNotification(title, content);
                    return;
                }
                try
                {
                    // 加载 WindowsToast 模块
                    string toastModulePath = null;
                    if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "bin", "WindowsToast.exe")))
                    {
                        toastModulePath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "WindowsToast.exe");
                    }
                    else
                    {
                        toastModulePath = Path.Combine(Directory.GetCurrentDirectory(), "WindowsToast.exe");
                    }

                    Process process = new Process();
                    process.StartInfo.FileName = toastModulePath;
                    process.StartInfo.Arguments = $"-title=\"{title}\" -message=\"{content}\""; // 使用双引号包裹参数以处理空格
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true; // 不显示命令行窗口
                    process.Start();
                    process.WaitForExit(); // 等待进程结束
                    if (process.ExitCode == 0)
                    {
                        WriteLog.Info("Toast 通知发送成功。");
                    }
                    else
                    {
                        WriteLog.Error($"Toast 通知发送失败，退出代码: {process.ExitCode}");
                    }
                }
                catch (Exception ex)
                {
                    WriteLog.Error($"发送 Toast 通知失败: {ex.Message}");
                }
            }
        }
    }
}
