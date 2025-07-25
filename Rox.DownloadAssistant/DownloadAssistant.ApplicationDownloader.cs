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
    public partial class DownloadAssistant
    {
        /// <summary>
        /// 定义下载相关应用选项
        /// </summary>
        public enum App
        {
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
        /// 用于下载指定应用的相关文件，并在网络不可用时提示用户
        /// </summary>
        /// <param name="app"> 指定下载应用</param>
        public static void ApplicationDownloader(App app)
        {
            WriteLog.Info(LogKind.System, _GET_TEMP);
            string folderPath = Path.GetTempPath();
            string filePath = $"{Application.StartupPath}\\bin\\aria2c.exe";
            CheckFile(filePath);
            if (!Runtimes.Network_I.IsNetworkAvailable())
            {
                DialogResult dialogResult = MessageBox.Show($"{_NOTAVAILABLE_NETWORK_TIPS}", $"{_TIPS}", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }
            switch (app)
            {
                case App.EasiNote5:
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
                            Downloader(url, folderPath);
                            var checkTimer = new Stopwatch();
                            checkTimer.Start();
                            while (checkTimer.ElapsedMilliseconds < timeout)
                            {
                                var files = Directory.GetFiles(folderPath)
                                            .Where(f => regex.IsMatch(Path.GetFileName(f)))
                                            .ToArray();
                                WriteLog.Info(LogKind.System, $"{_GET_DIRECTORY}: {folderPath}");
                                if (files.Any())
                                {
                                    WriteLog.Info(LogKind.Regex, $"{_REGEX_GET_FILE}: {Path.GetFileName(files.First())}");
                                    downloadSuccess = true;

                                    break;
                                }
                                WriteLog.Info(LogKind.Downloader, $"{_WAIT_DOWNLOADING}... {checkTimer.ElapsedMilliseconds / 1000}s");
                                Thread.Sleep(checkInterval);
                            }
                            if (!downloadSuccess)
                            {
                                retryCount++;
                                WriteLog.Warning(LogKind.Downloader, $"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                                Directory.GetFiles(folderPath).ToList().ForEach(File.Delete); // 清理残留文件
                            }
                        }
                        if (!downloadSuccess)
                        {
                            WriteLog.Error(LogKind.Downloader, $"{_DOWNLOADING_FAILED}: EasiNote5");
                        }

                        break;
                    }

                case App.EasiCamera:
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
                            Downloader(url, folderPath);
                            var checkTimer = new Stopwatch();
                            checkTimer.Start();
                            while (checkTimer.ElapsedMilliseconds < timeout)
                            {
                                var files = Directory.GetFiles(folderPath, $"{targetPrefix}*.exe");
                                WriteLog.Info(LogKind.System, $"{_GET_DIRECTORY}: {folderPath}");
                                if (files.Any())
                                {
                                    WriteLog.Info(LogKind.Regex, $"{_REGEX_GET_FILE}: {Path.GetFileName(files.First())}");
                                    downloadSuccess = true;
                                    break;
                                }
                                WriteLog.Info(LogKind.Downloader, $"{_WAIT_DOWNLOADING}... {checkTimer.ElapsedMilliseconds / 1000}s");
                                Thread.Sleep(checkInterval);
                            }
                            if (!downloadSuccess)
                            {
                                retryCount++;
                                WriteLog.Warning(LogKind.Downloader, $"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                                Directory.GetFiles(folderPath).ToList().ForEach(File.Delete);
                            }
                        }
                        if (!downloadSuccess)
                        {
                            WriteLog.Error(LogKind.Downloader, $"{_DOWNLOADING_FAILED}: EasiCamera");
                        }

                        break;
                    }

                case App.WeChat:
                    {
                        const string targetFile = "WeChatSetup.exe";
                        const int maxRetries = 3;
                        const int checkInterval = 2000; // 2秒
                        const int timeout = 60000;      // 60秒
                        var downloadvocation = Path.GetTempPath();
                        Downloader("https://pc.weixin.qq.com", downloadvocation, true);
                        var html = File.ReadAllText($"{downloadvocation}\\index.html");
                        WriteLog.Info(LogKind.Downloader, $"{_GET_HTML}: {downloadvocation}\\index.html");
                        // 根据系统架构确定要下载的链接
                        bool is64Bit = Environment.Is64BitOperatingSystem;
                        string systemBit;
                        // 获取下载链接（合并32/64位处理）
                        string downloadLink = GetWeChatDownloadLink(html, !is64Bit);
                        if (is64Bit)
                        {
                            systemBit = _64;
                            WriteLog.Info(LogKind.Downloader, $"{_GET_64_LINK}: {downloadLink}");
                        }
                        else
                        {
                            systemBit = _32;
                            WriteLog.Info(LogKind.Downloader, $"{_GET_32_LINK}: {downloadLink}");
                        }
                        WriteLog.Info($"{_GET_SYSTEM_BIT}: {systemBit}");
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
                                WriteLog.Info(LogKind.System, $"{_GET_FILES_IN_DIRECTORY}: {folderPath}");
                                if (files.Any())
                                {
                                    WriteLog.Info(LogKind.System, $"{_GET_FILE}: {files.First()}");
                                    var newFile = files.First();
                                    downloadSuccess = true;
                                    Process process = new Process();
                                    process.StartInfo.FileName = newFile;
                                    process.Start();
                                    WriteLog.Info($"{_PROCESS_STARTED}: {process.Id}");
                                    process.WaitForExit();
                                    WriteLog.Info($"{_PROCESS_EXITED}: {process.ExitCode}");
                                    process.Close();
                                    break;
                                }
                                WriteLog.Info($"{_WAIT_DOWNLOADING}... waiting {checkTimer.ElapsedMilliseconds / 1000} second");
                                Thread.Sleep(checkInterval);
                            }
                            if (!downloadSuccess)
                            {
                                retryCount++;
                                WriteLog.Warning($"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                            }
                        }
                        if (!downloadSuccess)
                        {
                            WriteLog.Error($"{_DOWNLOADING_FAILED}: {retryCount}/{maxRetries}");
                        }

                        break;
                    }

                case App.SeewoService:
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
                            Downloader(url, folderPath);
                            var checkTimer = new Stopwatch();
                            checkTimer.Start();
                            while (checkTimer.ElapsedMilliseconds < timeout)
                            {
                                var files = Directory.GetFiles(folderPath, $"{targetPrefix}*.exe");
                                WriteLog.Info($"{_GET_DIRECTORY}: {folderPath}");
                                if (files.Any())
                                {
                                    WriteLog.Info($"{_GET_FILE}: {Path.GetFileName(files.First())}");
                                    downloadSuccess = true;
                                    break;
                                }
                                WriteLog.Info($"{_WAIT_DOWNLOADING}... {checkTimer.ElapsedMilliseconds / 1000}s");
                                Thread.Sleep(checkInterval);
                            }
                            if (!downloadSuccess)
                            {
                                retryCount++;
                                WriteLog.Warning($"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                                Directory.GetFiles(folderPath).ToList().ForEach(File.Delete);
                            }
                        }
                        if (!downloadSuccess)
                        {
                            WriteLog.Error($"{_DOWNLOADING_FAILED}: SeewoService");
                        }

                        break;
                    }

                case App.ToDesk:
                    {
                        const string targetFile = "ToDeskSetup.exe";
                        const int maxRetries = 3;
                        const int checkInterval = 2000;
                        const int timeout = 60000;
                        var downloadvocation = $"{Directory.GetCurrentDirectory()}\\temp";
                        Downloader("https://www.todesk.com/download.html", downloadvocation, true);
                        var html = File.ReadAllText($"{downloadvocation}\\download.html");
                        WriteLog.Info($"{_GET_HTML}: {downloadvocation}\\download.html");
                        string downloadLink = GetToDeskDownlaodLink(html);
                        WriteLog.Info($"{_GET_URL}: {downloadLink}");
                        bool downloadSuccess = false;
                        int retryCount = 0;

                        while (!downloadSuccess && retryCount < maxRetries)
                        {
                            Downloader(downloadLink, folderPath, true);
                            var checkTimer = new Stopwatch();
                            checkTimer.Start();
                            while (checkTimer.ElapsedMilliseconds < timeout)
                            {
                                var files = Directory.GetFiles(folderPath, targetFile);
                                WriteLog.Info($"{_GET_FILES_IN_DIRECTORY}: {folderPath}");
                                if (files.Any())
                                {
                                    WriteLog.Info($"{_GET_FILE}: {files.First()}");
                                    var newFile = files.First();
                                    downloadSuccess = true;
                                    Process process = new Process();
                                    process.StartInfo.FileName = newFile;
                                    process.Start();
                                    WriteLog.Info($"{_PROCESS_STARTED}: {process.Id}");
                                    process.WaitForExit();
                                    WriteLog.Info($"{_PROCESS_EXITED}: {process.ExitCode}");
                                    process.Close();
                                    break;
                                }
                                WriteLog.Info($"{_WAIT_DOWNLOADING}... waiting {checkTimer.ElapsedMilliseconds / 1000} second");
                                Thread.Sleep(checkInterval);
                            }
                            if (!downloadSuccess)
                            {
                                retryCount++;
                                WriteLog.Warning($"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                            }
                        }
                        if (!downloadSuccess)
                        {
                            WriteLog.Error($"{_DOWNLOADING_FAILED}: {retryCount}/{maxRetries}");
                        }

                        break;
                    }
            }
        }
        /// <summary>
        /// 用于从给定的 HTML 内容中提取微信下载链接，支持选择 32 位或 64 位版本。
        /// </summary>
        /// <param name="htmlContent"> HTML 内容</param>
        /// <param name="is32Bit"> 是否选择 32 位版本</param>
        /// <returns> 返回微信下载链接</returns>
        public static string GetWeChatDownloadLink(string htmlContent, bool is32Bit = false)
        {
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
                WriteLog.Info(LogKind.Regex, $"{_REGEX_GET_FILE}: {match.Groups[1].Value}");
                return match.Groups[1].Value; // 返回 href 属性的值
            }
            return null; // 如果没有匹配到，返回 null
        }
        /// <summary>
        /// 用于从给定的 HTML 内容中提取 ToDesk 下载链接
        /// </summary>
        /// <param name="htmlContent"> HTML 内容</param>
        /// <returns> 返回 ToDesk 下载链接</returns>
        public static string GetToDeskDownlaodLink(string htmlContent)
        {
            Match match = Regex.Match(htmlContent, @"<div\s+class=""win_download""[^>]*>\s*<a\s+[^>]*href=""([^""]+)""");
            if (match.Success)
            {
                WriteLog.Info(LogKind.Regex, $"{_REGEX_GET_FILE}: {match.Groups[1].Value}");
                return match.Groups[1].Value; // 返回 href 属性的值
            }
            return null; // 如果没有匹配到，返回 null
        }
    }
}