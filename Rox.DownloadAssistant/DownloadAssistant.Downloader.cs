using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    public partial class DownloadAssistant
    {
        /// <summary>
        /// 检查 filePath 是否存在,不存在则从资源中提取
        /// </summary>
        /// <param name="filePath">文件路径</param>
        private static void CheckFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                WriteLog.Info(LogKind.Downloader, $"{_GET_ARIA2C_PATH}: {filePath}");
            }
            else
            {
                // 获取当前正在执行的类库的程序集
                Assembly assembly = Assembly.GetExecutingAssembly();

                // 假设aria2c.exe是嵌入在"Namespace.Resources"命名空间中的

                string resourceName = "Rox.Runtimes.Properties.Resources"; // 替换为你的资源路径

                // 创建 ResourceManager 实例
                ResourceManager rm = new ResourceManager(resourceName, assembly);
                WriteLog.Info($"{_NEW_RM}");
                // 从资源中获取aria2c.exe文件的字节数据
                byte[] Aria2cExeData = (byte[])rm.GetObject("aria2c");
                WriteLog.Info(LogKind.Downloader, $"{_GET_RM_OBJ}: aria2c");
                if (Aria2cExeData != null)
                {
                    // 将文件保存到当前目录
                    string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "bin");
                    // 检查并创建目录
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                        WriteLog.Info(LogKind.System, $"{_CREATE_DIRECTORY}");
                    }
                    WriteLog.Info(LogKind.System, $"{_GET_OUTPUT_DIRECTORY}: {outputDirectory}");
                    // 保存文件路径
                    string outputFilePath = Path.Combine(outputDirectory, "aria2c.exe");
                    WriteLog.Info(LogKind.System, $"{_GET_OUTPUT_NAME}: {outputDirectory}");
                    // 写入文件，确保保存为二进制数据
                    WriteLog.Info(LogKind.System, $"{_FILE_WRITING}");
                    System.IO.File.WriteAllBytes(outputFilePath, Aria2cExeData);
                    WriteLog.Info(LogKind.System, $"aria2c.exe {_FILE_EXIST_PATH} {outputFilePath}");
                }
                else
                {
                    WriteLog.Error($"{_RES_FILE_NOT_FIND}");
                }
            }
        }
        /// <summary>
        /// 用于从指定的 URL 下载多个文件到调用者所在目录, 即 <see cref="Application.StartupPath"/>
        /// </summary>
        /// <param name="urls"> 指定多个下载链接</param>
        public static void Downloader(string[] urls) => MutiFileCoreDownloader(urls, string.Empty, false);
        /// <summary>
        /// 用于从指定的 URL 下载多个文件到指定的目录，使用 aria2c 工具并支持多线程下载
        /// </summary>
        /// <param name="urls"> 指定多个下载链接</param>
        /// <param name="location"> 下载位置</param>
        public static void Downloader(string[] urls, string location) => MutiFileCoreDownloader(urls, location, false);
        /// <summary>
        /// 用于从指定的 URL 下载多个文件到调用者所在目录, 即 <see cref="Application.StartupPath"/>, 使用 aria2c 工具并支持多线程下载
        /// </summary>
        /// <param name="urls"> 指定多个下载链接</param>
        /// <param name="log"> 是否记录日志</param>
        public static void Downloader(string[] urls, bool log) => MutiFileCoreDownloader(urls, string.Empty, log);
        /// <summary>
        /// 用于从指定的 URL 下载多个文件到指定的目录, 并决定是否记录日志，使用 aria2c 工具并支持多线程下载
        /// </summary>
        /// <param name="urls"> 指定多个下载链接</param>
        /// <param name="location"> 下载位置</param>
        /// <param name="log"> 是否记录日志</param>
        public static void Downloader(string[] urls, string location, bool log) => MutiFileCoreDownloader(urls, location, log);
        /// <summary>
        /// 用于从指定的 URL 下载文件到指定的目录, 并决定是否记录日志，使用 aria2c 工具并支持多线程下载
        /// </summary>
        /// <param name="url"> 指定下载链接</param>
        /// <param name="location"> 下载位置</param>
        /// <param name="log"> 是否记录日志</param>
        public static void Downloader(string url, string location, bool log) => CoreDownloader($"-x 16 -s 16 --check-certificate=false {(log ? $"--log=\"{Path.Combine(Application.StartupPath, "aria2c.log")}\"" : string.Empty)} -d {location} {url}");
        /// <summary>
        /// 用于从指定的 URL 下载文件到指定的目录，使用 aria2c 工具并支持多线程下载
        /// </summary>
        /// <param name="url"> 指定下载链接</param>
        /// <param name="location"> 下载位置</param>
        public static void Downloader(string url, string location) => CoreDownloader($"-x 16 -s 16 --check-certificate=false -d {location} {url}");
        /// <summary>
        /// 用于从指定的 URL 下载文件到调用者所在目录, 即 <see cref="Application.StartupPath"/>, 并决定是否记录日志, 使用 aria2c 工具并支持多线程下载
        /// </summary>
        /// <param name="url"> 指定下载链接</param>
        /// <param name="log"> 是否记录日志</param>
        public static void Downloader(string url, bool log) => CoreDownloader($"-x 16 -s 16 --check-certificate=false {(log ? $"--log=\"{Path.Combine(Application.StartupPath, "aria2c.log")}\"" : string.Empty)} {url}");
        /// <summary>
        /// 用于从指定的 URL 下载文件到调用者所在目录, 即 <see cref="Application.StartupPath"/>, 使用 aria2c 工具并支持多线程下载
        /// </summary>
        /// <param name="url"> 指定下载链接</param>
        public static void Downloader(string url) => CoreDownloader($"-x 16 -s 16 --check-certificate=false {url}");
    }
}
