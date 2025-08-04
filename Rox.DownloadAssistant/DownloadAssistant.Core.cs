using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;

namespace Rox
{
    public partial class DownloadAssistant
    {
        /// <summary>
        /// 核心下载器，使用 aria2c 工具进行多线程下载
        /// </summary>
        /// <param name="args"></param>
        public static void CoreDownloader(string args)
        {
            // 设置文件地址
            string filePath = Path.Combine(Application.StartupPath, "bin", "aria2c.exe");
            // 检查 aria2c.exe 是否存在
            CheckFile(filePath);
            // 解析参数
            /* -x 线程数, 修改版可以上限1000线程
             * -d, --dir=<DIR>  存储下载文件的目录。
             * -l, --log=<LOG>  日志文件的文件名。如果指定了``-，则日志将写入``stdout。如果指定了空字符串(“”)，或者省略了此选项，则根本不会将日志写入磁盘。
             * -o, --out=<FILE> 下载文件的文件名。它始终是相对于 --dir 选项中给定的目录。使用 --force-sequential 选项时，此选项将被忽略。
             * -q, --quiet      静默下载
             */
            using (Process aria2c = new Process())
            {
                aria2c.StartInfo.FileName = filePath;
                aria2c.StartInfo.Arguments = args;
                WriteLog.Info(LogKind.Downloader, $"{_GET_ARIA2C_ARGS}: {args}");
                aria2c.Start();
                WriteLog.Info(LogKind.Downloader, $"{_PROCESS_STARTED}: {aria2c.Id}");
                WriteLog.Info(LogKind.Downloader, $"{_DOWNLOADING_FILE}...");
                aria2c.WaitForExit();
                if (aria2c.ExitCode != 0)
                {
                    WriteLog.Error(LogKind.Process, $"{_PROCESS_EXITED}: {aria2c.ExitCode}");
                    WriteLog.Error(LogKind.Downloader, $"{_ERROR}! {_GET_ARIA2C_EXITCODE}: {aria2c.ExitCode}");
                    MessageBox_I.Error($"下载器发生错误, 进程结束代码: {aria2c.ExitCode}", _ERROR);
                }
                else
                {
                    WriteLog.Info(LogKind.Downloader, $"{_DOWNLOADING_COMPLETE}");
                }
            }
        }
        internal static void MutiFileCoreDownloader(string[] urls, string location, bool log)
        {
            string logarg = string.Empty;
            if (log)
                logarg = $"--log={Path.Combine(Application.StartupPath, "log", "aria2c.log")}";
            // 设置多文件下载清单
            string motherFilePath = Path.GetTempFileName();
            using (StreamWriter writer = new StreamWriter(motherFilePath, true, Encoding.UTF8))
            {
                // 写入每个下载链接到临时文件
                foreach (var arg in urls)
                {
                    writer.WriteLine(arg);
                }
            }
            CoreDownloader($"--input-file=\"{motherFilePath}\" -x 16 -s 16 --check-certificate=false {location} {logarg}");
        }
    }
}
// User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.7103.48 Safari/537.36