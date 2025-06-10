using Rox.Runtimes;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    /// <summary>
    /// 下载助手,处理下载任务
    /// </summary>
    public class DownloadAssistant
    {
        /// <summary>
        /// 检查 filePath 是否存在,不存在则从资源中提取
        /// </summary>
        /// <param name="filePath">文件路径</param>
        private static void CheckFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                WriteLog(LogLevel.Info, $"{_GET_ARIA2C_PATH}: {filePath}");
                Assembly assembly = Assembly.GetExecutingAssembly();
                foreach (var resource in assembly.GetManifestResourceNames())
                {
                    WriteLog(LogLevel.Info, $"{_GET_RM_NAME}: {resource}");
                }
                return;
            }
            else
            {
                // 获取当前正在执行的类库的程序集
                Assembly assembly = Assembly.GetExecutingAssembly();

                // 假设aria2c.exe是嵌入在"Namespace.Resources"命名空间中的

                string resourceName = "Rox.DownloadAssistant.Properties.Resources"; // 替换为你的资源路径

                // 创建 ResourceManager 实例
                ResourceManager rm = new ResourceManager(resourceName, assembly);
                WriteLog(LogLevel.Info, $"{_NEW_RM}");
                // 从资源中获取aria2c.exe文件的字节数据
                byte[] Aria2cExeData = (byte[])rm.GetObject("aria2c");
                WriteLog(LogLevel.Info, $"{_GET_RM_OBJ}: aria2c");
                if (Aria2cExeData != null)
                {
                    // 将文件保存到当前目录
                    string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "bin");
                    // 检查并创建目录
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                        WriteLog(LogLevel.Info, $"{_CREATE_DIRECTORY}");
                    }
                    WriteLog(LogLevel.Info, $"{_GET_OUTPUT_DIRECTORY}: {outputDirectory}");
                    // 保存文件路径
                    string outputFilePath = Path.Combine(outputDirectory, "aria2c.exe");
                    WriteLog(LogLevel.Info, $"{_GET_OUTPUT_NAME}: {outputDirectory}");
                    // 写入文件，确保保存为二进制数据
                    WriteLog(LogLevel.Info, $"{_FILE_WRITING}");
                    System.IO.File.WriteAllBytes(outputFilePath, Aria2cExeData);
                    WriteLog(LogLevel.Info, $"aria2c.exe {_FILE_EXIST_PATH} {outputFilePath}");
                }
                else
                {
                    WriteLog(LogLevel.Error, $"{_RES_FILE_NOT_FIND}");
                }
            }
        }
        /// <summary>
        /// 定义下载相关模块选项
        /// </summary>
        public enum Module
        {
            /// <summary>
            /// 7-zip
            /// </summary>
            zip,
            /// <summary>
            /// Visual C++ Redistributable
            /// </summary>
            VC,
            /// <summary>
            /// Windows Update Blocker
            /// </summary>
            Wub,
            /// <summary>
            /// Windows KMS Activator
            /// </summary>
            Activator,
        }
        /// <summary>
        /// 定义下载相关应用选项
        /// </summary>
        public enum App
        {
            //             Edge,
            /// <summary>
            /// 希沃白板  5
            /// </summary>
            EasiNote5,
            /// <summary>
            /// 希沃视频展台
            /// </summary>
            EasiCamera,
            /// <summary>
            /// 希沃服务
            /// </summary>
            SeewoService,
            /// <summary>
            /// 微信
            /// </summary>
            WeChat,
            /// <summary>
            /// ToDesk远程控制
            /// </summary>
            ToDesk,
        };
        /// <summary>
        /// 用于从指定的 URL 下载文件到指定的目录，使用 aria2c 工具并支持多线程下载
        /// </summary>
        /// <param name="url">指定下载链接</param>
        /// <param name="Downloadvocation">下载位置</param>
        public static void Downloader(string url, string Downloadvocation)
        {
            CheckFile($"{Directory.GetCurrentDirectory()}\\bin\\aria2c.exe");
            string arg = $"-x 16 -d \"{Downloadvocation}\" \"{url}\"";
            /* -x 线程数, 修改版可以上限1000线程
             * -d, --dir=<DIR>  存储下载文件的目录。
             * -l, --log=<LOG>  日志文件的文件名。如果指定了``-，则日志将写入``stdout。如果指定了空字符串(“”)，或者省略了此选项，则根本不会将日志写入磁盘。
             * -o, --out=<FILE> 下载文件的文件名。它始终是相对于 --dir 选项中给定的目录。使用 --force-sequential 选项时，此选项将被忽略。
             * -q, --quiet      静默下载
             */
            Process p = new Process();
            p.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\aria2c";
            p.StartInfo.Arguments = arg;
            WriteLog(LogLevel.Info, $"{_GET_ARIA2C_ARGS}: {arg}");
            p.Start();
            WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {p.Id}");
            WriteLog(LogLevel.Info, $"{_DOWNLOADING_FILE}...");
            p.WaitForExit();
            WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {p.ExitCode}");
            if (p.ExitCode != 0)
            {
                LogLibraries.WriteLog(LogLibraries.LogLevel.Error, $"{_ERROR}! {_GET_ARIA2C_EXITCODE}: {p.ExitCode}");
            }
            else
            {
                WriteLog(LogLevel.Info, $"{_DOWNLOADING_COMPLETE}");
            }
            p.Close();
        }
        /// <summary>
        /// 用于从指定的 URL 下载文件到指定的目录，使用 aria2c 工具并支持多线程下载
        /// </summary>
        /// <param name="url"> 指定下载链接</param>
        /// <param name="Downloadvocation"> 下载位置</param>
        /// <param name="log"> 是否记录日志</param>
        public static void Downloader(string url, string Downloadvocation, bool log)
        {
            string arg;
            CheckFile($"{Directory.GetCurrentDirectory()}\\bin\\aria2c.exe");
            if (!log)
            {
                WriteLog(LogLevel.Info, $"{_DISABLE_ARIA2C_LOG_OUTPUT}");
                arg = $"-x 16 -d \"{Downloadvocation}\" \"{url}\"";
            }
            else
            {
                WriteLog(LogLevel.Info, $"{_ENABLE_ARIA2C_LOG_OUTPUT}");
                arg = $"-x 16 -d \"{Downloadvocation}\" -l \"{Directory.GetCurrentDirectory()}\\aria2c.log\" \"{url}\"";
            }
            /* -d, --dir=<DIR>  存储下载文件的目录。
             * -l, --log=<LOG>  日志文件的文件名。如果指定了``-，则日志将写入``stdout。如果指定了空字符串(“”)，或者省略了此选项，则根本不会将日志写入磁盘。
             * -o, --out=<FILE> 下载文件的文件名。它始终是相对于 --dir 选项中给定的目录。使用 --force-sequential 选项时，此选项将被忽略。
             * -q, --quiet      静默下载
             */
            Process p = new Process();
            p.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\aria2c";
            p.StartInfo.Arguments = arg;
            WriteLog(LogLevel.Info, $"{_GET_ARIA2C_ARGS}: {arg}");
            p.Start();
            WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {p.Id}");
            WriteLog(LogLevel.Info, $"{_DOWNLOADING_FILE}...");
            p.WaitForExit();
            WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {p.ExitCode}");
            if (p.ExitCode != 0)
            {
                LogLibraries.WriteLog(LogLibraries.LogLevel.Error, $"{_ERROR}! {_GET_ARIA2C_EXITCODE}: {p.ExitCode}");
            }
            else
            {
                WriteLog(LogLevel.Info, $"{_DOWNLOADING_COMPLETE}");
            }
            p.Close();
        }
        /// <summary>
        /// 用于从指定的 URL 下载文件到指定的目录，使用 aria2c 工具并支持多线程下载
        /// </summary>
        /// <param name="url"> 指定下载链接</param>
        /// <param name="Downloadvocation"> 下载位置</param>
        /// <param name="outputName"> 输出文件名</param>
        public static void Downloader(string url, string Downloadvocation, string outputName)
        {
            string arg = $"-x 16 -d \"{Downloadvocation}\" \"{url}\" -o \"{outputName}\"";
            CheckFile($"{Directory.GetCurrentDirectory()}\\bin\\aria2c.exe");
            /* -d, --dir=<DIR>  存储下载文件的目录。
             * -l, --log=<LOG>  日志文件的文件名。如果指定了``-，则日志将写入``stdout。如果指定了空字符串(“”)，或者省略了此选项，则根本不会将日志写入磁盘。
             * -o, --out=<FILE> 下载文件的文件名。它始终是相对于 --dir 选项中给定的目录。使用 --force-sequential 选项时，此选项将被忽略。
             * -q, --quiet      静默下载
             */
            Process p = new Process();
            p.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\aria2c";
            p.StartInfo.Arguments = arg;
            WriteLog(LogLevel.Info, $"{_GET_ARIA2C_ARGS}: {arg}");
            p.Start();
            WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {p.Id}");
            WriteLog(LogLevel.Info, $"{_DOWNLOADING_FILE}...");
            p.WaitForExit();
            WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {p.ExitCode}");
            if (p.ExitCode != 0)
            {
                LogLibraries.WriteLog(LogLibraries.LogLevel.Error, $"{_ERROR}! {_GET_ARIA2C_EXITCODE}: {p.ExitCode}");
            }
            else
            {
                WriteLog(LogLevel.Info, $"{_DOWNLOADING_COMPLETE}");
            }
            p.Close();
        }
        /// <summary>
        /// 用于从指定的 URL 下载文件到指定的目录，使用 aria2c 工具并支持多线程下载
        /// </summary>
        /// <param name="url"> 指定下载链接</param> 
        /// <param name="Downloadvocation"> 下载位置</param>
        /// <param name="outputName"> 输出文件名</param>
        /// <param name="log"> 是否记录日志</param>
        public static void Downloader(string url, string Downloadvocation, string outputName, bool log)
        {
            string arg;
            CheckFile($"{Directory.GetCurrentDirectory()}\\bin\\aria2c.exe");
            if (!log)
            {
                WriteLog(LogLevel.Info, $"{_DISABLE_ARIA2C_LOG_OUTPUT}");
                arg = $"-x 16 -d \"{Downloadvocation}\" -q \"{url}\" -o \"{outputName}\"";
            }
            else
            {
                WriteLog(LogLevel.Info, $"{_ENABLE_ARIA2C_LOG_OUTPUT}");
                arg = $"-x 16 -d \"{Downloadvocation}\" -l \"{Directory.GetCurrentDirectory()}\\aria2c.log\" \"{url}\" -o \"{outputName}\"";
            }
            /* -x 线程数, 修改版可以上限1000线程
             * -d, --dir=<DIR>  存储下载文件的目录。
             * -l, --log=<LOG>  日志文件的文件名。如果指定了``-，则日志将写入``stdout。如果指定了空字符串(“”)，或者省略了此选项，则根本不会将日志写入磁盘。
             * -o, --out=<FILE> 下载文件的文件名。它始终是相对于 --dir 选项中给定的目录。使用 --force-sequential 选项时，此选项将被忽略。
             * -q, --quiet      静默下载
             */
            Process p = new Process();
            p.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\aria2c";
            p.StartInfo.Arguments = arg;
            WriteLog(LogLevel.Info, $"{_GET_ARIA2C_ARGS}: {arg}");
            p.Start();
            WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {p.Id}");
            WriteLog(LogLevel.Info, $"{_DOWNLOADING_FILE}...");
            p.WaitForExit();
            WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {p.ExitCode}");
            if (p.ExitCode != 0)
            {
                LogLibraries.WriteLog(LogLibraries.LogLevel.Error, $"{_ERROR}! {_GET_ARIA2C_EXITCODE}: {p.ExitCode}");
            }
            else
            {
                WriteLog(LogLevel.Info, $"{_DOWNLOADING_COMPLETE}");
            }
            p.Close();
        }
        /// <summary>
        /// 用于下载指定模块的相关文件，并在网络不可用时提示用户
        /// </summary>
        /// <param name="module"> 指定下载模块</param>
        public static void ModuleDownloader(Module module)
        {
            string filePath = $"{Directory.GetCurrentDirectory()}\\bin\\aria2c.exe";
            CheckFile(filePath);
            if (!Rox.Runtimes.Network.IsNetworkAvailable())
            {
                DialogResult dialogResult = MessageBox.Show($"{_NOTAVAILABLE_NETWORK_TIPS}", $"{_TIPS}! {_NOTAVAILABLE_NETWORK}", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }
            if (module == Module.Wub)
            {
                string[] url =
                {
                    "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/resource/Wub_x64.exe",
                    "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/resource/Wub.ini",
                    "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/resource/Wubx32.exe",
                    "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/resource/Wubx32.ini",
                    };
                WriteLog(LogLevel.Info, $"{_GET_URL} {url}");
                Download(url);
                return;
            }
            if (module == Module.Activator)
            {
                string url = "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/resource/HEU_KMS_Activator_v19.6.0.exe";
                WriteLog(LogLevel.Info, $"{_GET_URL} {url}");
                Download(url);
                return;
            }

            if (module == Module.zip)
            {
                string[] url =
                {
                    "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/resource/7-zip/7za.dll",
                    "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/resource/7-zip/7za.exe",
                    "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/resource/7-zip/7zxa.dll",
                    };
                if (!System.IO.File.Exists($"{Directory.GetCurrentDirectory()}\\bin\\7zxa.dll"))
                {
                    WriteLog(LogLevel.Info, $"{_GET_URL} {url}");
                    Download(url);
                }
                else
                {
                    WriteLog(LogLevel.Info, $"{_FILE_EXIST}: {Directory.GetCurrentDirectory()}\\bin\\7zxa.dll");
                }
                return;
            }
            if (module == Module.VC)
            {
                string[] url =
                {
                        "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/main/VC.zip.001",
                        "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/main/VC.zip.002",
                        "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/main/VC.zip.003",
                        "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/main/VC.zip.004",
                        "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/main/VC.zip.005",
                        "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/main/VC.zip.006",
                        "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/main/VC.zip.007",
                        "https://gitee.com/Rainbow-SPY/GoldSource-Engine-ToolKit-.NET/raw/main/VC.zip.008"
                    };
                if (!System.IO.File.Exists($"{Directory.GetCurrentDirectory()}\\bin\\VC.zip.001"))
                {
                    WriteLog(LogLevel.Info, $"{_GET_URL} {url}");
                    Download(url);
                }
                else
                {
                    WriteLog(LogLevel.Info, $"{_FILE_EXIST}: {Directory.GetCurrentDirectory()}\\bin\\VC.zip.001");
                }
                Thread.Sleep(3000);
                if (System.IO.File.Exists($"{Directory.GetCurrentDirectory()}\\bin\\VC.zip.008"))
                {
                    var temp = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp";
                    WriteLog(LogLevel.Info, $"{_GET_TEMP} {temp}");
                    DownloadAssistant.ModuleDownloader(Module.zip);
                    Process p = new Process();
                    p.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\7za";
                    p.StartInfo.Arguments = $" x -y {Directory.GetCurrentDirectory()}\\bin\\VC.zip.001 -o{temp}";
                    p.Start();
                    p.WaitForExit();
                    if (p.ExitCode != 0)
                    {
                        WriteLog(LogLevel.Error, $"{_PROCESS_EXITED} {p.ExitCode}");
                    }
                    else
                    {
                        WriteLog(LogLevel.Info, $"{_PROCESS_EXITED} {p.ExitCode}");
                    }
                    p.Close();
                    Process w = new Process();
                    w.StartInfo.FileName = $"{temp}\\VC.exe";
                    w.Start();
                    w.WaitForExit();
                    if (w.ExitCode != 0)
                    {
                        WriteLog(LogLevel.Error, $"{_PROCESS_EXITED} {w.ExitCode}");
                    }
                    else
                    {
                        WriteLog(LogLevel.Info, $"{_PROCESS_EXITED} {w.ExitCode}");
                    }
                    w.Close();
                    return;
                }
            }
        }
        /// <summary>
        /// 用于下载指定应用的相关文件，并在网络不可用时提示用户
        /// </summary>
        /// <param name="app"> 指定下载应用</param>
        public static void ApplicationDownloader(App app)
        {
            var temp = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp\\Rox";
            WriteLog(LogLevel.Info, $"{GetLocalizedString("_GET_TEMP")}");
            string folderPath = $"{temp}";
            string filePath = $"{Directory.GetCurrentDirectory()}\\bin\\aria2c.exe";
            CheckFile(filePath);
            if (!Rox.Runtimes.Network.IsNetworkAvailable())
            {
                DialogResult dialogResult = MessageBox.Show($"{_NOTAVAILABLE_NETWORK_TIPS}", $"{_TIPS}", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                if (dialogResult == DialogResult.No)
                {
                    return;
                }
                else
                {

                }

            }
            if (app == App.EasiNote5)
            {
                const string pattern = @"EasiNoteSetup_\d+(\.\d+)*_seewo\.exe";
                const int maxRetries = 3;
                const int checkInterval = 2000;
                const int timeout = 60000;
                string url = "https://e.seewo.com/download/file?code=EasiNote5";
                bool downloadSuccess = false;
                int retryCount = 0;
                Regex regex = new Regex(pattern);

                while (!downloadSuccess && retryCount < maxRetries)
                {
                    AppDownload(url, folderPath);
                    var checkTimer = new Stopwatch();
                    checkTimer.Start();
                    while (checkTimer.ElapsedMilliseconds < timeout)
                    {
                        var files = Directory.GetFiles(folderPath)
                                    .Where(f => regex.IsMatch(Path.GetFileName(f)))
                                    .ToArray();
                        WriteLog(LogLevel.Info, $"{_GET_DIRECTORY}: {folderPath}");
                        if (files.Any())
                        {
                            WriteLog(LogLevel.Info, $"{_REGEX_GET_FILE}: {Path.GetFileName(files.First())}");
                            downloadSuccess = true;
                            break;
                        }
                        WriteLog(LogLevel.Info, $"{_WAIT_DOWNLOADING}... {checkTimer.ElapsedMilliseconds / 1000}s");
                        Thread.Sleep(checkInterval);
                    }
                    if (!downloadSuccess)
                    {
                        retryCount++;
                        WriteLog(LogLevel.Warning, $"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                        Directory.GetFiles(folderPath).ToList().ForEach(System.IO.File.Delete); // 清理残留文件
                    }
                }
                if (!downloadSuccess)
                {
                    WriteLog(LogLevel.Error, $"{_DOWNLOADING_FAILED}: EasiNote5");
                }
            }
            if (app == App.EasiCamera)
            {
                const string targetPrefix = "EasiCameraSetup_";
                const int maxRetries = 3;
                const int checkInterval = 2000;
                const int timeout = 60000;
                string url = "https://e.seewo.com/download/file?code=EasiCamera";
                bool downloadSuccess = false;
                int retryCount = 0;
                while (!downloadSuccess && retryCount < maxRetries)
                {
                    AppDownload(url, folderPath);
                    var checkTimer = new Stopwatch();
                    checkTimer.Start();
                    while (checkTimer.ElapsedMilliseconds < timeout)
                    {
                        var files = Directory.GetFiles(folderPath, $"{targetPrefix}*.exe");
                        WriteLog(LogLevel.Info, $"{_GET_DIRECTORY}: {folderPath}");
                        if (files.Any())
                        {
                            WriteLog(LogLevel.Info, $"{_GET_FILE}: {Path.GetFileName(files.First())}");
                            downloadSuccess = true;
                            break;
                        }
                        WriteLog(LogLevel.Info, $"{_WAIT_DOWNLOADING}... {checkTimer.ElapsedMilliseconds / 1000}s");
                        Thread.Sleep(checkInterval);
                    }
                    if (!downloadSuccess)
                    {
                        retryCount++;
                        WriteLog(LogLevel.Warning, $"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                        Directory.GetFiles(folderPath).ToList().ForEach(System.IO.File.Delete);
                    }
                }
                if (!downloadSuccess)
                {
                    WriteLog(LogLevel.Error, $"{_DOWNLOADING_FAILED}: EasiCamera");
                }
            }
            if (app == App.WeChat)
            {
                const string targetFile = "WeChatSetup.exe";
                const int maxRetries = 3;
                const int checkInterval = 2000; // 2秒
                const int timeout = 60000;      // 60秒
                var downloadvocation = $"{Directory.GetCurrentDirectory()}\\temp";
                Downloader("https://pc.weixin.qq.com", downloadvocation, true);
                var html = System.IO.File.ReadAllText($"{downloadvocation}\\index.html");
                WriteLog(LogLevel.Info, $"{_GET_HTML}: {downloadvocation}\\index.html");
                // 根据系统架构确定要下载的链接
                bool is64Bit = Environment.Is64BitOperatingSystem;
                string systemBit;
                // 获取下载链接（合并32/64位处理）
                string downloadLink = GetWeChatDownloadLink(html, !is64Bit);
                if (is64Bit)
                {
                    systemBit = _64;
                    WriteLog(LogLevel.Info, $"{_GET_64_LINK}: {downloadLink}");
                }
                else
                {
                    systemBit = _32;
                    WriteLog(LogLevel.Info, $"{_GET_32_LINK}: {downloadLink}");
                }
                WriteLog(LogLevel.Info, $"{_GET_SYSTEM_BIT}: {systemBit}");
                // 下载主逻辑
                bool downloadSuccess = false;
                int retryCount = 0;
                while (!downloadSuccess && retryCount < maxRetries)
                {
                    // 执行下载
                    Downloader(downloadLink, folderPath);
                    // 检查文件是否下载成功
                    var checkTimer = new Stopwatch();
                    checkTimer.Start();
                    while (checkTimer.ElapsedMilliseconds < timeout)
                    {
                        var files = Directory.GetFiles(folderPath, targetFile);
                        WriteLog(LogLevel.Info, $"{_GET_FILES_IN_DIRECTORY}: {folderPath}");
                        if (files.Any())
                        {
                            WriteLog(LogLevel.Info, $"{_GET_FILE}: {files.First()}");
                            var newFile = files.First();
                            downloadSuccess = true;
                            Process process = new Process();
                            process.StartInfo.FileName = newFile;
                            process.Start();
                            WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process.Id}");
                            process.WaitForExit();
                            WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process.ExitCode}");
                            process.Close();
                            break;
                        }
                        WriteLog(LogLevel.Info, $"{_WAIT_DOWNLOADING}... waiting {checkTimer.ElapsedMilliseconds / 1000} second");
                        Thread.Sleep(checkInterval);
                    }
                    if (!downloadSuccess)
                    {
                        retryCount++;
                        WriteLog(LogLevel.Warning, $"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                    }
                }
                if (!downloadSuccess)
                {
                    WriteLog(LogLevel.Error, $"{_DOWNLOADING_FAILED}: {retryCount}/{maxRetries}");
                }
            }
            if (app == App.SeewoService)
            {
                const string targetPrefix = "SeewoServiceSetup_";
                const int maxRetries = 3;
                const int checkInterval = 2000;
                const int timeout = 60000;
                string url = "https://e.seewo.com/download/file?code=SeewoServiceSetup";
                bool downloadSuccess = false;
                int retryCount = 0;
                while (!downloadSuccess && retryCount < maxRetries)
                {
                    AppDownload(url, folderPath);
                    var checkTimer = new Stopwatch();
                    checkTimer.Start();
                    while (checkTimer.ElapsedMilliseconds < timeout)
                    {
                        var files = Directory.GetFiles(folderPath, $"{targetPrefix}*.exe");
                        WriteLog(LogLevel.Info, $"{_GET_DIRECTORY}: {folderPath}");
                        if (files.Any())
                        {
                            WriteLog(LogLevel.Info, $"{_GET_FILE}: {Path.GetFileName(files.First())}");
                            downloadSuccess = true;
                            break;
                        }
                        WriteLog(LogLevel.Info, $"{_WAIT_DOWNLOADING}... {checkTimer.ElapsedMilliseconds / 1000}s");
                        Thread.Sleep(checkInterval);
                    }
                    if (!downloadSuccess)
                    {
                        retryCount++;
                        WriteLog(LogLevel.Warning, $"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                        Directory.GetFiles(folderPath).ToList().ForEach(System.IO.File.Delete);
                    }
                }
                if (!downloadSuccess)
                {
                    WriteLog(LogLevel.Error, $"{_DOWNLOADING_FAILED}: SeewoService");
                }
            }
            if (app == App.ToDesk)
            {
                const string targetFile = "ToDeskSetup.exe";
                const int maxRetries = 3;
                const int checkInterval = 2000;
                const int timeout = 60000;
                var downloadvocation = $"{Directory.GetCurrentDirectory()}\\temp";
                Downloader("https://www.todesk.com/download.html", downloadvocation, true);
                var html = System.IO.File.ReadAllText($"{downloadvocation}\\download.html");
                WriteLog(LogLevel.Info, $"{_GET_HTML}: {downloadvocation}\\download.html");
                string downloadLink = GetToDeskDownlaodLink(html);
                WriteLog(LogLevel.Info, $"{_GET_URL}: {downloadLink}");
                bool downloadSuccess = false;
                int retryCount = 0;

                while (!downloadSuccess && retryCount < maxRetries)
                {
                    Downloader(downloadLink, folderPath, "ToDeskSetup.exe", true);
                    var checkTimer = new Stopwatch();
                    checkTimer.Start();
                    while (checkTimer.ElapsedMilliseconds < timeout)
                    {
                        var files = Directory.GetFiles(folderPath, targetFile);
                        WriteLog(LogLevel.Info, $"{_GET_FILES_IN_DIRECTORY}: {folderPath}");
                        if (files.Any())
                        {
                            WriteLog(LogLevel.Info, $"{_GET_FILE}: {files.First()}");
                            var newFile = files.First();
                            downloadSuccess = true;
                            Process process = new Process();
                            process.StartInfo.FileName = newFile;
                            process.Start();
                            WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process.Id}");
                            process.WaitForExit();
                            WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process.ExitCode}");
                            process.Close();
                            break;
                        }
                        WriteLog(LogLevel.Info, $"{_WAIT_DOWNLOADING}... waiting {checkTimer.ElapsedMilliseconds / 1000} second");
                        Thread.Sleep(checkInterval);
                    }
                    if (!downloadSuccess)
                    {
                        retryCount++;
                        WriteLog(LogLevel.Warning, $"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                    }
                }
                if (!downloadSuccess)
                {
                    WriteLog(LogLevel.Error, $"{_DOWNLOADING_FAILED}: {retryCount}/{maxRetries}");
                }
            }
        }
        /// <summary>
        /// 用于从给定的 HTML 内容中提取微信下载链接，支持选择 32 位或 64 位版本。
        /// </summary>
        /// <param name="htmlContent"> HTML 内容</param>
        /// <param name="is32Bit"> 是否选择 32 位版本</param>
        /// <returns> 返回微信下载链接</returns>
        internal static string GetWeChatDownloadLink(string htmlContent, bool is32Bit = false)
        {
            //var doc = new HtmlAgilityPack.HtmlDocument();
            //doc.LoadHtml(htmlContent);
            //// 精确匹配元素
            //var node = is32Bit
            //    ? doc.DocumentNode.SelectSingleNode("//a[@id='x86' and contains(@class,'download-item')]")
            //    : doc.DocumentNode.SelectSingleNode("//a[@id='downloadButton' and contains(@class,'download-button')]");
            //return node?.GetAttributeValue("href", null);
            string pattern;
            if (is32Bit)
            {
                pattern = @"<a\s+id=""x86""\s+class=""download-item x86_tips""\s+href=""([^""]+)""";
            }
            else
            {
                pattern = @"<a\s+[^>]*class=""download-button""\s+[^>]*href=""([^""]+)""";
            }

            Match match = Regex.Match(htmlContent, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value; // 返回 href 属性的值
            }

            return null; // 如果没有匹配到，返回 null
        }
        /// <summary>
        /// 用于从给定的 HTML 内容中提取 ToDesk 下载链接
        /// </summary>
        /// <param name="htmlContent"> HTML 内容</param>
        /// <returns> 返回 ToDesk 下载链接</returns>
        internal static string GetToDeskDownlaodLink(string htmlContent)
        {
            //var doc = new HtmlAgilityPack.HtmlDocument();
            //doc.LoadHtml(htmlContent);
            //// 精确匹配元素
            //var node = doc.DocumentNode.SelectSingleNode("//div[@class='win_download']/a[@class='btn' and starts-with(@href,'https://')]");
            //return node?.GetAttributeValue("href", null);
            // 正则表达式匹配 <div class="win_download"> 下的 <a> 标签，并且 href 以 https:// 开头
            string pattern = @"<div\s+class=""win_download""[^>]*>\s*<a\s+[^>]*href=""([^""]+)""";

            Match match = Regex.Match(htmlContent, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value; // 返回 href 属性的值
            }

            return null; // 如果没有匹配到，返回 null
        }
        /// <summary>
        /// 用于下载指定的 URL 链接
        /// </summary>
        /// <param name="url"> 指定多个下载链接</param>
        internal static void Download(string[] url)
        {
            foreach (string ul in url)
            {
                string arg = $" - x 16 --check-certificate=false -l \"{Directory.GetCurrentDirectory()}\\temp\\aria2c.log\" -d \"{Directory.GetCurrentDirectory()}\\bin\" {ul}";
                /* -d, --dir=<DIR>  存储下载文件的目录。
                * -l, --log=<LOG>  日志文件的文件名。如果指定了``-，则日志将写入``stdout。如果指定了空字符串(“”)，或者省略了此选项，则根本不会将日志写入磁盘。
                * -o, --out=<FILE> 下载文件的文件名。它始终是相对于 --dir 选项中给定的目录。使用 --force-sequential 选项时，此选项将被忽略。
                * -q, --quiet      静默下载
                */
                Process p = new Process();
                p.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\aria2c";
                p.StartInfo.Arguments = arg;
                WriteLog(LogLevel.Info, $"{_GET_ARIA2C_ARGS}: {arg}");
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {p.Id}");
                WriteLog(LogLevel.Info, $"{_DOWNLOADING_FILE}...");
                p.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {p.ExitCode}");
                if (p.ExitCode != 0)
                {
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Error, $"{_ERROR}! {_GET_ARIA2C_EXITCODE}: {p.ExitCode}");
                }
                else
                {
                    WriteLog(LogLevel.Info, $"{_DOWNLOADING_COMPLETE}");
                }
                p.Close();
            }
            return;
        }
        /// <summary>
        /// 用于下载指定的 URL 链接
        /// </summary>
        /// <param name="url"> 指定下载链接</param>
        internal static void Download(string url)
        {
            string arg = $"-x 16 -s 16 --check-certificate=false -d {Directory.GetCurrentDirectory()}\\bin {url}";
            /* -d, --dir=<DIR>  存储下载文件的目录。
            * -l, --log=<LOG>  日志文件的文件名。如果指定了``-，则日志将写入``stdout。如果指定了空字符串(“”)，或者省略了此选项，则根本不会将日志写入磁盘。
            * -o, --out=<FILE> 下载文件的文件名。它始终是相对于 --dir 选项中给定的目录。使用 --force-sequential 选项时，此选项将被忽略。
            * -q, --quiet      静默下载
            */
            Process p = new Process();
            p.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\aria2c";
            p.StartInfo.Arguments = arg;
            WriteLog(LogLevel.Info, $"{_GET_ARIA2C_ARGS}: {arg}");
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {p.Id}");
            WriteLog(LogLevel.Info, $"{_DOWNLOADING_FILE}...");
            p.WaitForExit();
            WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {p.ExitCode}");
            if (p.ExitCode != 0)
            {
                LogLibraries.WriteLog(LogLibraries.LogLevel.Error, $"{_ERROR}! {_GET_ARIA2C_EXITCODE}: {p.ExitCode}");
            }
            else
            {
                WriteLog(LogLevel.Info, $"{_DOWNLOADING_COMPLETE}");
            }
            p.Close();
        }
        /// <summary>
        /// 用于从指定的 URL 下载应用程序到指定的目录，使用 aria2c 工具并支持多线程下载
        /// </summary>
        /// <param name="url"> 指定多个下载链接</param>
        /// <param name="location"> 下载位置</param>
        public static void AppDownload(string[] url, string location)
        {
            foreach (string ul in url)
            {
                string arg = $"-x 16 --check-certificate=false -l \"{Directory.GetCurrentDirectory()}\\temp\\aria2c.log\" -d \"{location}\" {ul}";
                /* -d, --dir=<DIR>  存储下载文件的目录。
                * -l, --log=<LOG>  日志文件的文件名。如果指定了``-，则日志将写入``stdout。如果指定了空字符串(“”)，或者省略了此选项，则根本不会将日志写入磁盘。
                * -o, --out=<FILE> 下载文件的文件名。它始终是相对于 --dir 选项中给定的目录。使用 --force-sequential 选项时，此选项将被忽略。
                * -q, --quiet      静默下载
                */
                Process p = new Process();
                p.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\aria2c";
                p.StartInfo.Arguments = arg;
                WriteLog(LogLevel.Info, $"{_GET_ARIA2C_ARGS}: {arg}");
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {p.Id}");
                WriteLog(LogLevel.Info, $"{_DOWNLOADING_FILE}...");
                p.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {p.ExitCode}");
                if (p.ExitCode != 0)
                {
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Error, $"{_ERROR}! {_GET_ARIA2C_EXITCODE}: {p.ExitCode}");
                }
                else
                {
                    WriteLog(LogLevel.Info, $"{_DOWNLOADING_COMPLETE}");
                }
                p.Close();
            }
        }
        /// <summary>
        /// 用于从指定的 URL 下载应用程序到指定的目录，使用 aria2c 工具并支持多线程下载
        /// </summary>
        /// <param name="url"> 指定下载链接</param>
        /// <param name="location"> 下载位置</param>
        public static void AppDownload(string url, string location)
        {
            string arg = $"-x 16 -s 16 --check-certificate=false -l \"{Directory.GetCurrentDirectory()}\\temp\\aria2c.log\" -d {location} {url}";
            /* -d, --dir=<DIR>  存储下载文件的目录。
            * -l, --log=<LOG>  日志文件的文件名。如果指定了``-，则日志将写入``stdout。如果指定了空字符串(“”)，或者省略了此选项，则根本不会将日志写入磁盘。
            * -o, --out=<FILE> 下载文件的文件名。它始终是相对于 --dir 选项中给定的目录。使用 --force-sequential 选项时，此选项将被忽略。
            * -q, --quiet      静默下载
            */
            Process p = new Process();
            p.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\aria2c";
            p.StartInfo.Arguments = arg;
            WriteLog(LogLevel.Info, $"{_GET_ARIA2C_ARGS}: {arg}");
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {p.Id}");
            WriteLog(LogLevel.Info, $"{_DOWNLOADING_FILE}...");
            p.WaitForExit();
            WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {p.ExitCode}");
            if (p.ExitCode != 0)
            {
                LogLibraries.WriteLog(LogLibraries.LogLevel.Error, $"{_ERROR}! {_GET_ARIA2C_EXITCODE}: {p.ExitCode}");
            }
            else
            {
                WriteLog(LogLevel.Info, $"{_DOWNLOADING_COMPLETE}");
            }
            p.Close();
        }
    }
}