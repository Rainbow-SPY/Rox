using System;
using System.IO;
using System.Net;
using System.Threading;
using static Rox.Runtimes.LogLibraries;

namespace Rox
{
    public partial class DownloadAssistant
    {
        // 临时文件后缀（用于标记分块文件）
        private const string TempFileSuffix = ".tmp";

        /// <summary>
        /// 多线程下载文件（基础版）
        /// </summary>
        /// <param name="downloadUrl">下载地址</param>
        /// <param name="savePath">最终保存路径（含文件名）</param>
        /// <param name="threadCount">线程数</param>
        /// <param name="timeoutMs">请求超时时间（默认10秒）</param>
        public static void Download(string downloadUrl, string savePath, int threadCount = 8, int timeoutMs = 10000)
        {
            if (threadCount < 1) threadCount = 1; // 至少1个线程

            try
            {
                // 步骤1：获取文件总大小
                var fileTotalSize = GetFileSize(downloadUrl, timeoutMs);
                if (fileTotalSize <= 0)
                {
                    WriteLog.Warning("无法获取文件大小，可能服务器不支持Range请求");
                    // 降级为单线程下载（不分块）
                    SingleThreadDownload(downloadUrl, savePath);
                    return;
                }

                // 步骤2：计算每个线程要下载的块大小
                var blockSize = fileTotalSize / threadCount;
                var threads = new Thread[threadCount];
                var tempFilePaths = new string[threadCount];

                // 步骤3：启动多线程下载每个块
                for (var i = 0; i < threadCount; i++)
                {
                    var threadIndex = i;
                    // 计算当前块的起始/结束位置
                    var start = threadIndex * blockSize;

                    tempFilePaths[threadIndex] = $"{savePath}{TempFileSuffix}{threadIndex}";
                    threads[threadIndex] = new Thread(() =>
                    {
                        DownloadBlock(downloadUrl, tempFilePaths[threadIndex], start,
                            threadIndex == threadCount - 1
                                ? fileTotalSize - 1
                                : start + blockSize - 1,
                            timeoutMs);
                    });
                    threads[threadIndex].Start();
                }

                // 步骤4：等待所有线程下载完成
                foreach (var thread in threads)
                {
                    thread.Join();
                }

                // 步骤5：合并临时文件为最终文件
                MergeTempFiles(tempFilePaths, savePath);

                // 步骤6：清理临时文件
                CleanTempFiles(tempFilePaths);

                WriteLog.Info(LogKind.Downloader, $"文件下载完成：{savePath}");
            }
            catch (Exception ex)
            {
                WriteLog.Warning(LogKind.Downloader, $"下载失败：{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 单线程下载（降级方案，用于不支持分块的服务器）
        /// </summary>
        private static void SingleThreadDownload(string url, string savePath)
        {
            using (var client = new WebClient())
                client.DownloadFile(url, savePath);
        }

        /// <summary>
        /// 获取文件总大小（通过HEAD请求+Content-Length）
        /// </summary>
        private static long GetFileSize(string url, int timeoutMs)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            request.Timeout = timeoutMs;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                // 检查服务器是否支持分块下载（Accept-Ranges: bytes）
                if (response.Headers["Accept-Ranges"] != "bytes")
                    return -1;

                return response.ContentLength;
            }
        }

        /// <summary>
        /// 下载单个文件块
        /// </summary>
        private static void DownloadBlock(string url, string tempSavePath, long start, long end, int timeoutMs)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = timeoutMs;
                request.AddRange(start, end);

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var fileStream = new FileStream(tempSavePath, FileMode.Create, FileAccess.Write))
                {
                    var buffer = new byte[4096]; // 4K缓冲区（临时用不用调大）
                    int readSize;
                    while (responseStream != null && (readSize = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        fileStream.Write(buffer, 0, readSize);
                }

                WriteLog.Info(LogKind.Downloader, $"块[{start}-{end}]下载完成：{tempSavePath}");
            }
            catch (Exception ex)
            {
                WriteLog.Warning(LogKind.Downloader, $"块[{start}-{end}]下载失败：{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 合并所有临时块文件为最终文件
        /// </summary>
        private static void MergeTempFiles(string[] tempFilePaths, string finalSavePath)
        {
            using (var finalFileStream = new FileStream(finalSavePath, FileMode.Create, FileAccess.Write))
                foreach (var tempPath in tempFilePaths)
                {
                    if (!File.Exists(tempPath)) continue;
                    using (var tempFileStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read))
                        tempFileStream.CopyTo(finalFileStream);
                }
        }

        /// <summary>
        /// 清理临时分块文件
        /// </summary>
        private static void CleanTempFiles(string[] tempFilePaths)
        {
            foreach (var tempPath in tempFilePaths)
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
        }
    }
}
// User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.7103.48 Safari/537.36