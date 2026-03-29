using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Rox.Runtimes;
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
        }

        /// <summary>
        /// 用于下载指定应用的相关文件，并在网络不可用时提示用户
        /// </summary>
        /// <param name="app"> 指定下载应用</param>
        public static void ApplicationDownloader(App app)
        {
            WriteLog.Info(LogKind.System, _GET_TEMP);
            var folderPath = Path.GetTempPath();
            if (!Network_I.IsNetworkAvailable() && MessageBox.Show($"{_NOTAVAILABLE_NETWORK_TIPS}", $"{_TIPS}",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly) == DialogResult.No)
                return;

            switch (app)
            {
                case App.EasiNote5:
                {
                    const int maxRetries = 3;
                    const int checkInterval = 2000;
                    const int timeout = 60000;
                    const string url = "https://e.seewo.com/download/file?code=EasiNote5";
                    var downloadSuccess = false;
                    var retryCount = 0;

                    while (!downloadSuccess && retryCount < maxRetries)
                    {
                        Download(url, folderPath);
                        var checkTimer = new Stopwatch();
                        checkTimer.Start();
                        while (checkTimer.ElapsedMilliseconds < timeout)
                        {
                            var files = Directory.GetFiles(folderPath)
                                .Where(f =>
                                    new Regex(@"EasiNoteSetup_\d+(\.\d+)*_seewo\.exe").IsMatch(Path.GetFileName(f)))
                                .ToArray();
                            WriteLog.Info(LogKind.System, $"{_GET_DIRECTORY}: {folderPath}");
                            if (files.Any())
                            {
                                WriteLog.Info(LogKind.Regex, $"{_REGEX_GET_FILE}: {Path.GetFileName(files.First())}");
                                downloadSuccess = true;
                                break;
                            }

                            WriteLog.Info(LogKind.Downloader,
                                $"{_WAIT_DOWNLOADING}... {checkTimer.ElapsedMilliseconds / 1000}s");
                            Thread.Sleep(checkInterval);
                        }

                        if (downloadSuccess) continue;
                        retryCount++;
                        WriteLog.Warning(LogKind.Downloader, $"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                        Directory.GetFiles(folderPath).ToList().ForEach(File.Delete); // 清理残留文件
                    }

                    if (!downloadSuccess)
                        WriteLog.Error(LogKind.Downloader, $"{_DOWNLOADING_FAILED}: EasiNote5");
                    break;
                }
                case App.EasiCamera:
                {
                    const string targetPrefix = "EasiCameraSetup_";
                    const int maxRetries = 3;
                    const int checkInterval = 2000;
                    const int timeout = 60000;
                    const string url = "https://e.seewo.com/download/file?code=EasiCamera";
                    var downloadSuccess = false;
                    var retryCount = 0;
                    while (!downloadSuccess && retryCount < maxRetries)
                    {
                        Download(url, folderPath);
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

                            WriteLog.Info(LogKind.Downloader,
                                $"{_WAIT_DOWNLOADING}... {checkTimer.ElapsedMilliseconds / 1000}s");
                            Thread.Sleep(checkInterval);
                        }

                        if (downloadSuccess) continue;
                        retryCount++;
                        WriteLog.Warning(LogKind.Downloader, $"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                        Directory.GetFiles(folderPath).ToList().ForEach(File.Delete);
                    }

                    if (!downloadSuccess)
                        WriteLog.Error(LogKind.Downloader, $"{_DOWNLOADING_FAILED}: EasiCamera");
                    break;
                }
                case App.WeChat:
                {
                    const string targetFile = "WeChatSetup.exe";
                    const int maxRetries = 3;
                    const int checkInterval = 2000; // 2秒
                    const int timeout = 60000; // 60秒
                    var download_location = Path.GetTempPath();
                    Download("https://pc.weixin.qq.com", download_location);
                    WriteLog.Info(LogKind.Downloader, $"{_GET_HTML}: {download_location}\\index.html");
                    var downloadLink = GetWeChatDownloadLink(File.ReadAllText($"{download_location}\\index.html"),
                        !Environment.Is64BitOperatingSystem);
                    var systemBit = Environment.Is64BitOperatingSystem ? _64 : _32;
                    WriteLog.Info(LogKind.Downloader,
                        Environment.Is64BitOperatingSystem ? _GET_64_LINK : _GET_32_LINK + $": {downloadLink}");
                    WriteLog.Info($"{_GET_SYSTEM_BIT}: {systemBit}");
                    var downloadSuccess = false;
                    var retryCount = 0;
                    while (!downloadSuccess && retryCount < maxRetries)
                    {
                        Download(downloadLink, folderPath);
                        var checkTimer = new Stopwatch();
                        checkTimer.Start();
                        while (checkTimer.ElapsedMilliseconds < timeout)
                        {
                            var files = Directory.GetFiles(folderPath, targetFile);
                            WriteLog.Info(LogKind.System, $"{_GET_FILES_IN_DIRECTORY}: {folderPath}");
                            if (files.Any())
                            {
                                var newFile = files.First();
                                WriteLog.Info(LogKind.System, $"{_GET_FILE}: {newFile}");
                                downloadSuccess = true;
                                Install(newFile);
                                break;
                            }

                            WriteLog.Info(LogKind.Downloader,
                                $"{_WAIT_DOWNLOADING}... waiting {checkTimer.ElapsedMilliseconds / 1000} second");
                            Thread.Sleep(checkInterval);
                        }

                        if (downloadSuccess) continue;
                        retryCount++;
                        WriteLog.Warning(LogKind.Downloader, $"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                    }

                    if (!downloadSuccess)
                        WriteLog.Error(LogKind.Downloader, $"{_DOWNLOADING_FAILED}: {retryCount}/{maxRetries}");
                    break;
                }

                case App.SeewoService:
                {
                    const string targetPrefix = "SeewoServiceSetup_";
                    const int maxRetries = 3;
                    const int checkInterval = 2000;
                    const int timeout = 60000;
                    const string url = "https://e.seewo.com/download/file?code=SeewoServiceSetup";
                    var downloadSuccess = false;
                    var retryCount = 0;
                    while (!downloadSuccess && retryCount < maxRetries)
                    {
                        Download(url, folderPath);
                        var checkTimer = new Stopwatch();
                        checkTimer.Start();
                        while (checkTimer.ElapsedMilliseconds < timeout)
                        {
                            var files = Directory.GetFiles(folderPath, $"{targetPrefix}*.exe");
                            WriteLog.Info(LogKind.System, $"{_GET_DIRECTORY}: {folderPath}");
                            if (files.Any())
                            {
                                WriteLog.Info(LogKind.System, $"{_GET_FILE}: {Path.GetFileName(files.First())}");
                                downloadSuccess = true;
                                break;
                            }

                            WriteLog.Info(LogKind.Downloader,
                                $"{_WAIT_DOWNLOADING}... {checkTimer.ElapsedMilliseconds / 1000}s");
                            Thread.Sleep(checkInterval);
                        }

                        if (downloadSuccess) continue;
                        retryCount++;
                        WriteLog.Warning(LogKind.Downloader, $"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                        Directory.GetFiles(folderPath).ToList().ForEach(File.Delete);
                    }

                    if (!downloadSuccess)
                        WriteLog.Error(LogKind.Downloader, $"{_DOWNLOADING_FAILED}: SeewoService");
                    break;
                }
                case App.ToDesk:
                {
                    const string targetFile = "ToDeskSetup.exe";
                    const int maxRetries = 3;
                    const int checkInterval = 2000;
                    const int timeout = 60000;
                    var download_location = $"{Directory.GetCurrentDirectory()}\\temp";
                    Download("https://www.todesk.com/download.html", download_location);
                    WriteLog.Info($"{_GET_HTML}: {download_location}\\download.html");
                    var downloadLink = GetToDeskDownloadLink(File.ReadAllText($"{download_location}\\download.html"));
                    WriteLog.Info($"{_GET_URL}: {downloadLink}");
                    var downloadSuccess = false;
                    var retryCount = 0;

                    while (!downloadSuccess && retryCount < maxRetries)
                    {
                        Download(downloadLink, folderPath);
                        var checkTimer = new Stopwatch();
                        checkTimer.Start();
                        while (checkTimer.ElapsedMilliseconds < timeout)
                        {
                            WriteLog.Info(LogKind.System, $"{_GET_FILES_IN_DIRECTORY}: {folderPath}");
                            var files = Directory.GetFiles(folderPath, targetFile);
                            if (files.Any())
                            {
                                WriteLog.Info(LogKind.System, $"{_GET_FILE}: {files.First()}");
                                var newFile = files.First();
                                downloadSuccess = true;
                                Install(newFile);
                                break;
                            }

                            WriteLog.Info(LogKind.Downloader,
                                $"{_WAIT_DOWNLOADING}... waiting {checkTimer.ElapsedMilliseconds / 1000} second");
                            Thread.Sleep(checkInterval);
                        }

                        if (downloadSuccess) continue;
                        retryCount++;
                        WriteLog.Warning(LogKind.Downloader, $"{_RETRY_DOWNLOAD}: {retryCount}/{maxRetries}");
                    }

                    if (!downloadSuccess)
                        WriteLog.Error(LogKind.Downloader, $"{_DOWNLOADING_FAILED}: {retryCount}/{maxRetries}");
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(app), app, null);
            }
        }

        internal static void Install(string FileName)
        {
            using (var install = new Process())
            {
                install.StartInfo.FileName = FileName;
                install.Start();
                WriteLog.Info(LogKind.Process, $"{_PROCESS_STARTED}: {install.Id}");
                install.WaitForExit();
                if (install.ExitCode != 0)
                {
                    WriteLog.Error(LogKind.Process, $"{_PROCESS_EXITED}: {install.ExitCode}");
                    WriteLog.Error(LogKind.Downloader, $"{_ERROR}! {_GET_ARIA2C_EXITCODE}: {install.ExitCode}");
                    MessageBox_I.Error($"安装程序发生错误, 进程结束代码: {install.ExitCode}", _ERROR);
                }
                else
                {
                    WriteLog.Info(LogKind.Process, $"{_PROCESS_EXITED}: {install.ExitCode}");
                    WriteLog.Info(LogKind.Downloader, $"{_DOWNLOADING_COMPLETE}");
                }
            }
        }

        /// <summary>
        /// 用于从给定的 HTML 内容中提取微信下载链接，支持选择 32 位或 64 位版本。
        /// </summary>
        /// <param name="htmlContent"> HTML 内容</param>
        /// <param name="is32Bit"> 是否选择 32 位版本</param>
        /// <returns> 返回微信下载链接</returns>
        public static string GetWeChatDownloadLink(string htmlContent, bool is32Bit = true)
        {
            var match = Regex.Match(htmlContent, is32Bit
                ? @"<a\s+id=""x86""\s+class=""download-item x86_tips""\s+href=""([^""]+)"""
                : @"<a\s+[^>]*class=""download-button""\s+[^>]*href=""([^""]+)""");
            if (!match.Success) return null; // 如果没有匹配到，返回 null
            WriteLog.Info(LogKind.Regex, $"{_REGEX_GET_FILE}: {match.Groups[1].Value}");
            return match.Groups[1].Value; // 返回 href 属性的值
        }

        /// <summary>
        /// 用于从给定的 HTML 内容中提取 ToDesk 下载链接
        /// </summary>
        /// <param name="htmlContent"> HTML 内容</param>
        /// <returns> 返回 ToDesk 下载链接</returns>
        public static string GetToDeskDownloadLink(string htmlContent)
        {
            var match = Regex.Match(htmlContent, @"<div\s+class=""win_download""[^>]*>\s*<a\s+[^>]*href=""([^""]+)""");
            if (!match.Success) return null; // 如果没有匹配到，返回 null
            WriteLog.Info(LogKind.Regex, $"{_REGEX_GET_FILE}: {match.Groups[1].Value}");
            return match.Groups[1].Value; // 返回 href 属性的值
        }
    }
}