using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using NinjaMagisk.Interface.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NinjaMagisk.LogLibraries;
using static NinjaMagisk.Software;
namespace NinjaMagisk
{
    internal class ResourceHelper
    {
        private static bool IsChineseSimple(string lang)
        {
            return lang == "zh-CN" || lang == "zh-CHS";
        }
        private static ResourceManager GetResourceManager(string lang)
        {
            if (IsChineseSimple(lang))
            {
                // 如果是中文（zh-CN 或 zh-Hans），返回 Resources.resx 的资源管理器
                return new ResourceManager("NinjaMagisk.Interface.Properties.Resources", typeof(Resources).Assembly);
            }
            else
            {
                // 如果是其他语言，返回 Resource1.resx 的资源管理器
                return new ResourceManager("NinjaMagisk.Interface.Properties.Resource1", typeof(Resource1).Assembly);
            }
        }
        internal static string GetString(string key, string lang)
        {
            try
            {
                ResourceManager resourceManager = GetResourceManager(lang);
                string value = resourceManager.GetString(key);
                return value;
            }
            catch
            {
                WriteLog(LogLevel.Error, $"资源键 '{key}' 未找到，语言: {lang}");
                WriteLog(LogLevel.Info, $"Error:{key} ");
                return null;
            }
        }
    }
    public class LogLibraries
    {
        public static Action<LogLevel, string> LogToUi { get; set; }
        public enum LogLevel
        {
            Info,
            Error,
            Warning
        }
        public enum LogKind
        {
            Form,
            Thread,
            Process,
            Service,
            Task,
            System,
            PowerShell,
            Registry,
            Network,
        }
        // 定义日志文件名和路径（当前目录下的 Assistant.log 文件）
        private static readonly string logFileName = "Assistant.log";
        private static readonly string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), logFileName);
        //
        public static void WriteLog(LogLevel logLevel, LogKind logKind, string message)
        {
            // 在写日志之前先检查并处理文件重命名

            // 设置颜色
            if (logLevel == LogLevel.Info)
            {
                Console.ForegroundColor = ConsoleColor.Green;// 设置绿色
                Console.Write($"[{logLevel}] ");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"{logKind}: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write($"{message}\n");
                Console.ResetColor();

            }
            else if (logLevel == LogLevel.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"[{logLevel}] ");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"{logKind}: ");
                Console.ForegroundColor = ConsoleColor.Red; // 设置绿色
                Console.Write($"{message}\n");
                Console.ResetColor();
            }
            else if (logLevel == LogLevel.Warning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[{logLevel}] ");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"{logKind}: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow; // 设置绿色
                Console.Write($"{message}\n");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"[{logLevel}] {logKind}: {message}");
            }

            // 打印日志到控制台
            Console.ResetColor();
            LogToUi?.Invoke(logLevel, message);
            // 记录日志到文件
            LogToFile(logLevel, message);
        }
        public static void WriteLog(LogLevel logLevel, string message)
        {
            // 在写日志之前先检查并处理文件
            // 设置颜色
            if (logLevel == LogLevel.Info)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"[{logLevel}]");
                Console.ForegroundColor = ConsoleColor.DarkYellow; // 设置绿色
                Console.Write($": {message}\n");
                Console.ResetColor();
            }
            else if (logLevel == LogLevel.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"[{logLevel}]");
                Console.ForegroundColor = ConsoleColor.Red; // 设置绿色
                Console.Write($": {message}\n");
                Console.ResetColor();
            }
            else if (logLevel == LogLevel.Warning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[{logLevel}] ");
                Console.ForegroundColor = ConsoleColor.DarkYellow; // 设置绿色
                Console.Write($"{message}\n");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"[{logLevel}] {message}");
            }

            // 打印日志到控制台
            Console.ResetColor();
            LogToUi?.Invoke(logLevel, message);
            // 记录日志到文件
            LogToFile(logLevel, message);
        }
        public static void LogToFile(LogLevel logLevel, string message)
        {
            // 创建日志信息
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}]: {message}";

            try
            {
                // 如果日志文件不存在，则创建
                if (!System.IO.File.Exists(logFilePath))
                {
                    System.IO.File.Create(logFilePath).Close();
                }

                // 以追加方式写入日志内容
                using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
                {
                    writer.WriteLine(logMessage);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Error] Error writing to log file: {ex.Message}");
                Console.ResetColor();
            }
        }
        public static void LogToFile(LogLevel logLevel, LogKind logkind, string message)
        {
            // 创建日志信息
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logkind}] [{logLevel}]: {message}";

            try
            {
                // 如果日志文件不存在，则创建
                if (!System.IO.File.Exists(logFilePath))
                {
                    System.IO.File.Create(logFilePath).Close();
                }

                // 以追加方式写入日志内容
                using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
                {
                    writer.WriteLine(logMessage);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                WriteLog(LogLevel.Error, $"[Error] Error writing to log file: {ex.Message}");
                Console.ResetColor();
            }
        }
        public static void ClearFile(string filePath)
        {
            try
            {
                // 打开文件并清空内容
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Write))
                {
                    fs.SetLength(0); // 设置文件长度为0，即清空文件内容
                }

                WriteLog(LogLevel.Info, $"{_CLEAR_LOGFILE}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                WriteLog(LogLevel.Error, $"{_CANNOT_CLEAR_LOGFILE}: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
    public class Software
    {
        internal static string Language = GetLocalizedString("Language");
        internal static readonly string Version = GetLocalizedString("Version");
        internal static readonly string Author = GetLocalizedString("Author");
        internal static readonly string Copyright = GetLocalizedString("Copyright");
        internal static readonly string Protect = GetLocalizedString("Protect");
        internal static readonly string _FILE_EXIST = GetLocalizedString("_FILE_EXIST");
        internal static readonly string _FILE_WRITING = GetLocalizedString("_FILE_WRITING");
        internal static readonly string _FILE_EXIST_PATH = GetLocalizedString("_FILE_EXIST_PATH");
        internal static readonly string _RES_FILE_NOT_FIND = GetLocalizedString("_RES_FILE_NOT_FIND");
        internal static readonly string _CANNOT_WRITE_LOGFILE = GetLocalizedString("_CANNOT_WRITE_LOGFILE");
        internal static readonly string _CLEAR_LOGFILE = GetLocalizedString("_CLEAR_LOGFILE");
        internal static readonly string _CANNOT_CLEAR_LOGFILE = GetLocalizedString("_CANNOT_CLEAR_LOGFILE");
        internal static readonly string _GET_OUTPUT_DIRECTORY = GetLocalizedString("_GET_OUTPUT_DIRECTORY");
        internal static readonly string _GET_OUTPUT_NAME = GetLocalizedString("_GET_OUTPUT_NAME");
        internal static readonly string _CREATE_DIRECTORY = GetLocalizedString("_CREATE_DIRECTORY");
        internal static readonly string _GET_DIRECTORY = GetLocalizedString("_GET_DIRECTORY");
        internal static readonly string _NOTAVAILABLE_NETWORK = GetLocalizedString("_NOTAVAILABLE_NETWORK");
        internal static readonly string _NOTAVAILABLE_NETWORK_TIPS = GetLocalizedString("_NOTAVAILABLE_NETWORK_TIPS");
        internal static readonly string _TIPS = GetLocalizedString("_TIPS");
        internal static readonly string _ERROR = GetLocalizedString("_ERROR");
        internal static readonly string _WARNING = GetLocalizedString("_WARNING");
        internal static readonly string _GET_URL = GetLocalizedString("_GET_URL");
        internal static readonly string _GET_TEMP = GetLocalizedString("_GET_TEMP");
        internal static readonly string _REGEX_GET_FILE = GetLocalizedString("_REGEX_GET_FILE");
        internal static readonly string _GET_FILES_IN_DIRECTORY = GetLocalizedString("_GET_FILES_IN_DIRECTORY");
        internal static readonly string _GET_SYSTEM_BIT = GetLocalizedString("_GET_SYSTEM_BIT");
        internal static readonly string _FINDING_HTML_DOWNLOAD_LINK = GetLocalizedString("_FINDING_HTML_DOWNLOAD_LINK");
        internal static readonly string _FIND_HTML_CODE = GetLocalizedString("_FIND_HTML_CODE");
        internal static readonly string _SECURITY_RUNNING = GetLocalizedString("_SECURITY_RUNNING");
        internal static readonly string _PROCESS_STARTED = GetLocalizedString("_PROCESS_STARTED");
        internal static readonly string _PROCESS_EXITED = GetLocalizedString("_PROCESS_EXITED");
        internal static readonly string _CANNOT_DISENABLE_HIBERNATE = GetLocalizedString("_CANNOT_DISENABLE_HIBERNATE");
        internal static readonly string _DISENABLE_HIBERNATE = GetLocalizedString("_DISENABLE_HIBERNATE");
        internal static readonly string _CANNOT_ENABLE_HIGHPOWERCFG = GetLocalizedString("_CANNOT_ENABLE_HIGHPOWERCFG");
        internal static readonly string _ENABLE_HIGHPOWERCFG = GetLocalizedString("_ENABLE_HIGHPOWERCFG");
        internal static readonly string _CANNOT_DISABLE_SECURITY_CENTER = GetLocalizedString("_CANNOT_DISABLE_SECURITY_CENTER");
        internal static readonly string _CANNOT_ENABLE_SECURITY_CENTER = GetLocalizedString("_CANNOT_ENABLE_SECURITY_CENTER");
        internal static readonly string _DISABLE_SECURITY_CENTER = GetLocalizedString("_DISABLE_SECURITY_CENTER");
        internal static readonly string _ENABLE_SECURITY_CENTER = GetLocalizedString("_ENABLE_SECURITY_CENTER");
        internal static readonly string _WRITE_REGISTRY = GetLocalizedString("_WRITE_REGISTRY");
        internal static readonly string _CANNOT_DISABLE_WINDOWS_UPDATER = GetLocalizedString("_CANNOT_DISABLE_WINDOWS_UPDATER");
        internal static readonly string _CANNOT_ENABLE_WINDOWS_UPDATER = GetLocalizedString("_CANNOT_ENABLE_WINDOWS_UPDATER");
        internal static readonly string _DISABLE_WINDOWS_UPDATER = GetLocalizedString("_DISABLE_WINDOWS_UPDATER");
        internal static readonly string _ENABLE_WINDOWS_UPDATER = GetLocalizedString("_ENABLE_WINDOWS_UPDATER");
        internal static readonly string _ACTIVE_WINDOWS = GetLocalizedString("_ACTIVE_WINDOWS");
        internal static readonly string _CANNOT_ACTIVE_WINDOWS = GetLocalizedString("_CANNOT_ACTIVE_WINDOWS");
        internal static readonly string _SUCESS_WRITE_REGISTRY = GetLocalizedString("_SUCESS_WRITE_REGISTRY");
        internal static readonly string _WRITE_REGISTRY_FAILED = GetLocalizedString("_WRITE_REGISTRY_FAILED");
        internal static readonly string _GET_ARIA2C_ARGS = GetLocalizedString("_GET_ARIA2C_ARGS");
        internal static readonly string _GET_ARIA2C_PATH = GetLocalizedString("_GET_ARIA2C_PATH");
        internal static readonly string _GET_ARIA2C_EXITCODE = GetLocalizedString("_GET_ARIA2C_EXITCODE");
        internal static readonly string _ENABLE_ARIA2C_LOG_OUTPUT = GetLocalizedString("_ENABLE_ARIA2C_LOG_OUTPUT");
        internal static readonly string _DISABLE_ARIA2C_LOG_OUTPUT = GetLocalizedString("_DISABLE_ARIA2C_LOG_OUTPUT");
        internal static readonly string _DOWNLOADING_FILE = GetLocalizedString("_DOWNLOADING_FILE");
        internal static readonly string _DOWNLOADING_COMPLETE = GetLocalizedString("_DOWNLOADING_COMPLETE");
        internal static readonly string _DOWNLOADING_FAILED = GetLocalizedString("_DOWNLOADING_FAILED");
        internal static readonly string _GET_64_LINK = GetLocalizedString("_GET_64_LINK");
        internal static readonly string _GET_32_LINK = GetLocalizedString("_GET_32_LINK");
        internal static readonly string _GET_RM_NAME = GetLocalizedString("_GET_RM_NAME");
        internal static readonly string _GET_RM_OBJ = GetLocalizedString("_GET_RM_OBJ");
        internal static readonly string _NEW_RM = GetLocalizedString("_NEW_RM");
        internal static readonly string _GET_HTML = GetLocalizedString("_GET_HTML");
        internal static readonly string _32 = GetLocalizedString("_32");
        internal static readonly string _64 = GetLocalizedString("_64");
        internal static readonly string _GET_FILE = GetLocalizedString("_GET_FILE");
        internal static readonly string _WAIT_DOWNLOADING = GetLocalizedString("_WAIT_DOWNLOADING");
        internal static readonly string _RETRY_DOWNLOAD = GetLocalizedString("_RETRY_DOWNLOAD");
        internal static readonly string _ERROR_CODE = GetLocalizedString("_ERROR_CODE");
        internal static readonly string _LOGIN_ERROR_USER_OR_PASSWORD = GetLocalizedString("_LOGIN_ERROR_USER_OR_PASSWORD");
        internal static readonly string _LOGIN_VERIFY = GetLocalizedString("_LOGIN_VERIFY");
        internal static readonly string _CANCEL_OP = GetLocalizedString("_CANCEL_OP");
        internal static readonly string _UNKNOW_ERROR = GetLocalizedString("_UNKNOW_ERROR");
        internal static readonly string _GET_RESPONSE = GetLocalizedString("_GET_RESPONSE");
        internal static readonly string _ANSWER = GetLocalizedString("_ANSWER");
        internal static readonly string _SEND_REQUEST = GetLocalizedString("_SEND_REQUEST");
        internal static readonly string _LOGIN_VERIFY_ERROR = GetLocalizedString("_LOGIN_VERIFY_ERROR");
        internal static readonly string _ENTER_CREDENTIALS = GetLocalizedString("_ENTER_CREDENTIALS");
        internal static readonly string _SUCCESS_VERIFY = GetLocalizedString("_SUCCESS_VERITY");
        internal static readonly string _LATEST_VERSION = GetLocalizedString("_LATEST_VERSION");
        internal static readonly string _UNSUPPORT_PLATFORM = GetLocalizedString("_UNSUPPORT_PLATFORM");
        internal static readonly string _JSON_PARSING_FAILED = GetLocalizedString("_JSON_PARSING_FAILED");
        internal static readonly string _NEW_VERSION_AVAILABLE = GetLocalizedString("_NEW_VERSION_AVAILABLE");
        internal static readonly string _NON_NEW_VER = GetLocalizedString("_NON_NEW_VER");
        internal static readonly string _CURRENT_VER = GetLocalizedString("_CURRENT_VER");
        //internal static string lang = System.Globalization.CultureInfo.InstalledUICulture.Name.ToString();
        internal static string GetLocalizedString(string key)
        {
            return ResourceHelper.GetString(key, System.Globalization.CultureInfo.InstalledUICulture.Name.ToString());
        }

        public class DownloadAssistant
        {
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
                    string resourceName = "NinjaMagisk.Interface.Properties.Resources"; // 替换为你的资源路径

                    // 创建 ResourceManager 实例
                    ResourceManager rm = new ResourceManager(resourceName, assembly);
                    WriteLog(LogLevel.Info, $"{_NEW_RM}");
                    // 从资源中获取aria2c.exe文件的字节数据
                    byte[] aria2cExeData = (byte[])rm.GetObject("aria2c");
                    WriteLog(LogLevel.Info, $"{_GET_RM_OBJ}: VC");
                    if (aria2cExeData != null)
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
                        System.IO.File.WriteAllBytes(outputFilePath, aria2cExeData);
                        WriteLog(LogLevel.Info, $"aria2c.exe {_FILE_EXIST_PATH} {outputFilePath}");
                    }
                    else
                    {
                        WriteLog(LogLevel.Error, $"{_RES_FILE_NOT_FIND}");
                    }
                }
            }
            public enum Module
            {
                zip,
                VC,
                Wub,
                Activator,
            }
            public enum App
            {
                //             Edge,
                EasiNote5,
                EasiCamera,
                SeewoService,
                WeChat,
                ToDesk,
            };
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
            public static void ModuleDownloader(Module module)
            {
                string filePath = $"{Directory.GetCurrentDirectory()}\\bin\\aria2c.exe";
                CheckFile(filePath);
                if (!NinjaMagisk.Network.IsNetworkAvailable())
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
            public static void ApplicationDownloader(App app)
            {
                var temp = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp\\NinjaMagisk";
                WriteLog(LogLevel.Info, $"{System.Globalization.CultureInfo.InstalledUICulture.Name}");
                WriteLog(LogLevel.Info, $"{GetLocalizedString("_GET_TEMP")}");
                string folderPath = $"{temp}";
                string filePath = $"{Directory.GetCurrentDirectory()}\\bin\\aria2c.exe";
                CheckFile(filePath);
                if (!NinjaMagisk.Network.IsNetworkAvailable())
                {
                    DialogResult dialogResult = MessageBox.Show($"{_NOTAVAILABLE_NETWORK_TIPS}", $"{_TIPS}", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    if (dialogResult == DialogResult.No)
                    {
                        return;
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
                    WriteLog(LogLevel.Info, $"{_GET_HTML}: {html}");
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
                    WriteLog(LogLevel.Info, $"{_GET_HTML}: {html}");
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
            internal static string GetWeChatDownloadLink(string htmlContent, bool is32Bit = false)
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);
                // 精确匹配元素
                var node = is32Bit
                    ? doc.DocumentNode.SelectSingleNode("//a[@id='x86' and contains(@class,'download-item')]")
                    : doc.DocumentNode.SelectSingleNode("//a[@id='downloadButton' and contains(@class,'download-button')]");
                return node?.GetAttributeValue("href", null);
            }
            internal static string GetToDeskDownlaodLink(string htmlContent)
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);
                // 精确匹配元素
                var node = doc.DocumentNode.SelectSingleNode("//div[@class='win_download']/a[@class='btn' and starts-with(@href,'https://')]");
                return node?.GetAttributeValue("href", null);
            }
            internal static void Download(string[] url)
            {
                foreach (string ul in url)
                {
                    string arg = $"-x 16 --check-certificate=false -l \"{Directory.GetCurrentDirectory()}\\temp\\aria2c.log\" -d \"{Directory.GetCurrentDirectory()}\\bin\" {ul}";
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
            internal static void Download(string url)
            {
                string arg = $"-x 16 -s 16 --check-certificate=false -l \"{Directory.GetCurrentDirectory()}\\temp\\aria2c.log\" -d {Directory.GetCurrentDirectory()}\\bin {url}";
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
        public class AntiSecurity
        {
            public static bool Anti360Security()
            {
                Process[] processes = Process.GetProcessesByName("360Tray");
                if (processes.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public static bool AntiHuoRongSecurity()
            {
                Process[] processes = Process.GetProcessesByName("HipsTray");
                if (processes.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
    }
    public class Network
    {
        /// <summary>
        /// 检测是否有网络连接。
        /// </summary>
        /// <returns>有网络连接返回 <see langword="true"/>，无网络连接返回 <see langword="false"/>。</returns>
        public static bool IsNetworkAvailable()
        {
            try
            {
                // 检查网络适配器是否有可用的
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    WriteLog(LogLevel.Info, $"{_NOTAVAILABLE_NETWORK}");
                    return false;
                }

                // 进一步通过 Ping 验证网络连接
                using (var ping = new Ping())
                {
                    PingReply reply = ping.Send("8.8.8.8", 2000); // 尝试 Ping Google 的公共 DNS
                    return reply != null && reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                // 发生异常视为无网络
                WriteLog(LogLevel.Info, $"{_NOTAVAILABLE_NETWORK}");
                return false;
            }
        }
    }
    public class Windows
    {
        public class Hibernate
        {
            public static void Enable()
            {
                Switch("on");
            }
            public static void Disable()
            {
                Switch("off");
            }
            static void Switch(string key)
            {
                Process Sleep = new Process();
                Sleep.StartInfo.FileName = "powercfg.exe";
                Sleep.StartInfo.Arguments = "/hibernate " + key;
                Sleep.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {Sleep.Id}");
                Sleep.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {Sleep.ExitCode}");
                if (Sleep.ExitCode != 0)
                {
                    WriteLog(LogLevel.Error, LogKind.System, $"{_CANNOT_DISENABLE_HIBERNATE}: {Sleep.ExitCode}");
                }
                else
                {
                    WriteLog(LogLevel.Info, LogKind.System, $"{_DISENABLE_HIBERNATE}");
                }
            }
        }//休眠
        public void EnableHighPowercfg()
        {
            Process p = new Process();
            p.StartInfo.FileName = "powercfg";
            p.StartInfo.Arguments = "-duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61";
            p.Start();
            WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {p.Id}");
            p.WaitForExit();
            WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {p.ExitCode}");
            if (p.ExitCode != 0)
            {
                WriteLog(LogLevel.Error, $"{_CANNOT_ENABLE_HIGHPOWERCFG}: {p.ExitCode}");
            }
            else
            {
                WriteLog(LogLevel.Info, LogKind.Process, $"{_ENABLE_HIGHPOWERCFG}");
            }
        }//卓越性能电源方案
        public class WindowsSecurityCenter//Windows安全中心
        {
            public static void Enable()
            {
                Switch(0);
            }
            public static void Disable()
            {
                Switch(1);

            }
            static void Switch(int value)
            {
                while (AntiSecurity.Anti360Security() || AntiSecurity.AntiHuoRongSecurity())
                {
                    DialogResult dialogResult = MessageBox.Show($"{_SECURITY_RUNNING}", $"{_WARNING}", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    if (dialogResult == DialogResult.OK)
                    {
                        continue;
                    }
                }
                RegistryKey key;
                try
                {
                    // 修改 HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender
                    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender");
                    WriteLog(LogLevel.Info, $"{_WRITE_REGISTRY}");
                    key.SetValue("DisableAntiSpyware", value, RegistryValueKind.DWord);  // 禁用防间谍软件
                    WriteLog(LogLevel.Info, $"{_SUCESS_WRITE_REGISTRY}");
                    key.Close();  // 关闭注册表项

                    // 修改 HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection
                    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection");
                    WriteLog(LogLevel.Info, $"{_WRITE_REGISTRY}");
                    key.SetValue("DisableBehaviorMonitoring", value, RegistryValueKind.DWord);  // 禁用行为监控
                    WriteLog(LogLevel.Info, $"{_SUCESS_WRITE_REGISTRY}");
                    WriteLog(LogLevel.Info, $"{_WRITE_REGISTRY}");
                    key.SetValue("DisableIOAVProtection", value, RegistryValueKind.DWord);  // 禁用文件扫描
                    WriteLog(LogLevel.Info, $"{_SUCESS_WRITE_REGISTRY}");
                    WriteLog(LogLevel.Info, $"{_WRITE_REGISTRY}");
                    key.SetValue("DisableOnAccessProtection", value, RegistryValueKind.DWord);  // 禁用访问保护
                    WriteLog(LogLevel.Info, $"{_SUCESS_WRITE_REGISTRY}");
                    WriteLog(LogLevel.Info, $"{_WRITE_REGISTRY}");
                    key.SetValue("DisableRealtimeMonitoring", value, RegistryValueKind.DWord);  // 禁用实时监控
                    WriteLog(LogLevel.Info, $"{_SUCESS_WRITE_REGISTRY}");
                    key.Close();  // 关闭注册表项

                    // 修改 HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SecurityHealthService
                    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\SecurityHealthService");
                    WriteLog(LogLevel.Info, $"{_WRITE_REGISTRY}");
                    key.SetValue("Start", value + 2, RegistryValueKind.DWord);  // 设置服务启动类型为2自动 3手动 
                    WriteLog(LogLevel.Info, $"{_SUCESS_WRITE_REGISTRY}");
                    key.Close();
                    // 关闭注册表项
                }
                catch
                {
                    WriteLog(LogLevel.Error, $"{_WRITE_REGISTRY_FAILED}");
                    Thread.Sleep(1000);
                }
            }//开关
        }
        public class WindowsUpdate//Windows更新服务
        {
            public static void Enable()
            {
                Switch("/E");
            }
            public static void Disable()
            {
                Switch("/D");
            }
            private static string IsEnable(string value)
            {
                if (value == "/D")
                {
                    return _DISABLE_WINDOWS_UPDATER;
                }
                if (value == "/E")
                {
                    return _ENABLE_WINDOWS_UPDATER;
                }
                return null;
            }
            private static string ErrorEnable(string value)
            {
                if (value == "/D")
                {
                    return _CANNOT_DISABLE_WINDOWS_UPDATER;
                }
                if (value == "/E")
                {
                    return _CANNOT_ENABLE_WINDOWS_UPDATER;
                }
                return null;
            }
            static void Switch(string value)
            {
                while (AntiSecurity.Anti360Security() || AntiSecurity.AntiHuoRongSecurity())
                {
                    DialogResult dialogResult = MessageBox.Show($"{_NOTAVAILABLE_NETWORK_TIPS}", $"{_WARNING}", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    if (dialogResult == DialogResult.OK)
                    {
                        continue;
                    }
                }
                try
                {
                    string modulePath = $"{AppDomain.CurrentDomain.BaseDirectory}\\bin";
                    bool is64Bit = Environment.Is64BitOperatingSystem;
                    string fileName;
                    if (is64Bit)
                    {
                        fileName = "Wub_x64.exe";
                    }
                    else
                    {
                        fileName = "Wubx32.exe";
                    }
                    DownloadAssistant.ModuleDownloader(DownloadAssistant.Module.Wub);
                    Process p = new Process();
                    p.StartInfo.FileName = Path.Combine(modulePath, fileName);
                    p.StartInfo.Arguments = value;
                    p.Start();
                    WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {p.Id}");
                    p.WaitForExit();
                    WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {p.ExitCode}");
                    if (p.ExitCode != 0)
                    {
                        WriteLog(LogLevel.Warning, $"{ErrorEnable(value)}: ExitCode= {p.ExitCode}");
                    }
                    else
                    {
                        WriteLog(LogLevel.Info, $"{IsEnable(value)}");
                    }
                    p.Close();
                }
                catch (Exception e)
                {
                    WriteLog(LogLevel.Error, $"{ErrorEnable(value)}: {e}");
                }
            }//开关
        }
        public void ActiveWindows()//Windows激活
        {
            while (AntiSecurity.Anti360Security() || AntiSecurity.AntiHuoRongSecurity())
            {
                DialogResult dialogResult = MessageBox.Show($"{_NOTAVAILABLE_NETWORK_TIPS}", $"{_WARNING}", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                if (dialogResult == DialogResult.OK)
                {
                    continue;
                }
            }
            DownloadAssistant.ModuleDownloader(DownloadAssistant.Module.Activator);
            try
            {
                Process process12 = new Process();
                process12.StartInfo.FileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\bin\\HEU_KMS_Activator_v19.6.0.exe";
                process12.StartInfo.Arguments = "/kms38";
                process12.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process12.Id}");
                WriteLog(LogLevel.Info, LogKind.Process, "process started");
                WriteLog(LogLevel.Info, LogKind.Process, $"Args: {AppDomain.CurrentDomain.BaseDirectory}\\bin\\HEU_KMS_Activator_v19.6.0.exe /kms38");
                process12.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process12.ExitCode}");
                if (process12.ExitCode != 0)
                {
                    WriteLog(LogLevel.Error, $"{_CANNOT_ACTIVE_WINDOWS}: {process12.ExitCode}");
                }
                else
                {
                    WriteLog(LogLevel.Info, $"{_ACTIVE_WINDOWS}");
                }
                process12.Close();
            }
            catch (Exception exception)
            {
                WriteLog(LogLevel.Error, $"{_CANNOT_ACTIVE_WINDOWS}: {exception}");
            }
        }
        #region Windows身份验证

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CREDUI_INFO
        {
            public int cbSize;
            public IntPtr hwndParent;
            public string pszMessageText;
            public string pszCaptionText;
            public IntPtr hbmBanner;
        }
        [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
        private static extern void CoTaskMemFree(IntPtr ptr);
        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        private static extern int CredUIPromptForWindowsCredentials(
                ref CREDUI_INFO pUiInfo,
                int dwAuthError,
                ref uint pulAuthPackage,
                IntPtr pvInAuthBuffer,
                uint ulInAuthBufferSize,
                out IntPtr ppvOutAuthBuffer,
                out uint pulOutAuthBufferSize,
                ref bool pfSave,
                int dwFlags);
        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        private static extern bool CredUnPackAuthenticationBuffer(
            int dwFlags,
            IntPtr pAuthBuffer,
            uint cbAuthBuffer,
            StringBuilder pszUserName,
            ref int pcchMaxUserName,
            StringBuilder pszDomainName,
            ref int pcchMaxDomainName,
            StringBuilder pszPassword,
            ref int pcchMaxPassword);
        // 导入 LogonUser API
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);
        // 验证用户名和密码
        public static bool Authentication()
        {
            CREDUI_INFO credUI = new CREDUI_INFO
            {
                cbSize = Marshal.SizeOf(typeof(CREDUI_INFO)),
                pszCaptionText = _LOGIN_VERIFY,
                pszMessageText = _ENTER_CREDENTIALS,
                hwndParent = IntPtr.Zero,
                hbmBanner = IntPtr.Zero
            };
            bool isAuthenticated = false;
            bool userCancelled = false;
            do
            {
                uint authPackage = 0;
                IntPtr outCredBuffer;
                uint outCredBufferSize;
                bool save = false;

                int result = CredUIPromptForWindowsCredentials(
                    ref credUI,
                    0,
                    ref authPackage,
                    IntPtr.Zero,
                    0,
                    out outCredBuffer,
                    out outCredBufferSize,
                    ref save,
                    0x1); // CREDUIWIN_GENERIC

                if (result == 0)
                {
                    int maxUserName = 100;
                    int maxDomainName = 100;
                    int maxPassword = 100;
                    StringBuilder userName = new StringBuilder(maxUserName);
                    StringBuilder domainName = new StringBuilder(maxDomainName);
                    StringBuilder password = new StringBuilder(maxPassword);

                    if (CredUnPackAuthenticationBuffer(0, outCredBuffer, outCredBufferSize, userName, ref maxUserName, domainName, ref maxDomainName, password, ref maxPassword))
                    {
                        // 验证用户名和密码
                        IntPtr userToken;
                        bool isValid = LogonUser(
                            userName.ToString(),
                            domainName.ToString(),
                            password.ToString(),
                            2, // LOGON32_LOGON_INTERACTIVE
                            0, // LOGON32_PROVIDER_DEFAULT
                            out userToken);
                        string ExtraMessage;
                        if (isValid)
                        {
                            isAuthenticated = true;
                            CloseHandle(userToken);
                            MessageBox.Show(_SUCCESS_VERIFY, _TIPS, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            // 验证失败，显示错误提示
                            int errorCode = Marshal.GetLastWin32Error();
                            if (errorCode == 1326)
                            {
                                ExtraMessage = _LOGIN_ERROR_USER_OR_PASSWORD;
                            }
                            else
                            {
                                ExtraMessage = _UNKNOW_ERROR;
                            }
                            MessageBox.Show($"{_LOGIN_VERIFY_ERROR}（{_ERROR_CODE}：{errorCode} {ExtraMessage}）", _ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        // 调用验证方法
                    }
                    CoTaskMemFree(outCredBuffer);
                }
                else
                {
                    userCancelled = true;
                }
            }
            while (!isAuthenticated && !userCancelled); // 未成功且未取消时循环
            if (isAuthenticated)
            {
                // 执行后续操作（例如打开受保护的功能）
                WriteLog(LogLevel.Info, _SUCCESS_VERIFY);
                return true;
            }
            else
            {
                WriteLog(LogLevel.Info, _CANCEL_OP);
                return false;
            }
        }//Windows安全中心身份验证
        #endregion
    }
    public class Registry
    {
        public static void Write(string keyPath, string valueName, RegistryValueKind valueType, object valueData)
        {
            try
            {
                // 打开注册表项
                using (RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    // 写入值
                    WriteLog(LogLevel.Info, $"{_WRITE_REGISTRY}");
                    key.SetValue(valueName, valueData, valueType);
                    key.Close();
                }
                LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"{_SUCESS_WRITE_REGISTRY}");
            }
            catch (Exception ex)
            {
                LogLibraries.WriteLog(LogLibraries.LogLevel.Error, $"{_WRITE_REGISTRY_FAILED}: {ex.Message}");
            }
        }
        internal static string GetRegistryValue(string keyName, string valueName)
        {
            string value = "";
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(keyName))
            {
                if (key != null)
                {
                    value = key.GetValue(valueName) as string;
                    key.Close();
                }
            }
            return value;
        }
    }
    public class AI// AI
    {
        public class DeepSeek
        {
            // ==================== API配置区（后期可修改） ====================
            private const string ApiUrl = "https://api.deepseek.com/v1/chat/completions"; // DeepSeek API的URL
            public string Model { get; set; } = "default"; // 默认模型
            public static async Task Chat(string text, string api)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {api}");
                        var requestBody = new
                        {
                            model = "deepseek-chat",
                            messages = new[]
                            {
                            new { role = "user", content = text }
                        }
                        };
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        WriteLog(LogLevel.Info, $"{_SEND_REQUEST}...{text}");
                        HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                        string responseJson = await response.Content.ReadAsStringAsync();
                        WriteLog(LogLevel.Info, _GET_RESPONSE);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseJson);
                            string answer = responseObject.choices[0].message.content;
                            WriteLog(LogLevel.Info, $"{_ANSWER}: {answer}");
                        }
                        else
                        {
                            WriteLog(LogLevel.Error, $"{_ERROR}: {responseJson}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(LogLevel.Error, $"{_ERROR}: {ex.Message}");
                }
            }
        }
        public class ChatGPT
        {
            // ==================== API配置区（后期可修改） ====================
            private const string ApiUrl = "https://api.openai.com/v1/chat/completions"; // OpenAI API的URL
            public string Model { get; set; } = "gpt-3.5-turbo"; // 默认模型
            public static async Task Chat(string text, string apiKey)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                        var requestBody = new
                        {
                            model = "gpt-4o-mini", // 模型名称
                            messages = new[]
                            {
                                new { role = "user", content = text }
                            }
                        };
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        WriteLog(LogLevel.Info, $"{_SEND_REQUEST}...{text}");
                        HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                        string responseJson = await response.Content.ReadAsStringAsync();
                        WriteLog(LogLevel.Info, _GET_RESPONSE);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseJson);
                            string answer = responseObject.choices[0].message.content;
                            WriteLog(LogLevel.Info, $"{_ANSWER}: {answer}");
                        }
                        else
                        {
                            WriteLog(LogLevel.Error, $"{_ERROR} : {responseJson}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(LogLevel.Error, $"{_ERROR} : {ex.Message}");
                }
            }
        }
    }
    public class File
    {
        public class AESEncryption
        {
            public static string Encrypt(string plainText, byte[] Key/*256-bit*/, byte[] IV/*128-bit*/)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (var msEncrypt = new System.IO.MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }
                            return Convert.ToBase64String(msEncrypt.ToArray());
                        }
                    }
                }
            }// 加密方法
            public static string Decrypt(string cipherText, byte[] Key/*256-bit*/, byte[] IV/*128-bit*/)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (var msDecrypt = new System.IO.MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }// 解密方法
        }

        public enum AtOp
        {
            Readonly,
            System,
            Hidden,
            Archive,
        }
        public void Attrib(string path, AtOp Key, bool Switch)
        {
            string key;
            if (Switch)
            {
                key = "+";
            }
            else
            {
                key = "-";
            }
            if (Key == AtOp.Readonly)
            {
                string arg = $"{key}r";
                Process process = new Process();
                process.StartInfo.FileName = "attrib";
                process.StartInfo.Arguments = $"{arg} {path}";
                process.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process.Id}");
                process.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process.ExitCode}");
                process.Close();
            }
            if (Key == AtOp.System)
            {
                string arg = $"{key}s";
                Process process = new Process();
                process.StartInfo.FileName = "attrib";
                process.StartInfo.Arguments = $"{arg} {path}";
                process.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process.Id}");
                process.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process.ExitCode}");
                process.Close();
            }
            if (Key == AtOp.Hidden)
            {
                string arg = $"{key}h";
                Process process = new Process();
                process.StartInfo.FileName = "attrib";
                process.StartInfo.Arguments = $"{arg} {path}";
                process.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process.Id}");
                process.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process.ExitCode}");
                process.Close();
            }
            if (Key == AtOp.Archive)
            {
                string arg = $"{key}a";
                Process process = new Process();
                process.StartInfo.FileName = "attrib";
                process.StartInfo.Arguments = $"{arg} {path}";
                process.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process.Id}");
                process.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process.ExitCode}");
                process.Close();
            }
        }
        public static bool CheckFileHash(string filePath, string expectedMD5)
        {
            try
            {
                // 检查文件是否存在
                if (!System.IO.File.Exists(filePath))
                {
                    MessageBox.Show($"文件 {filePath} 不存在。");
                    return false;
                }

                // 计算文件的MD5哈希值
                string actualMD5 = CalculateMD5(filePath);

                // 检查哈希值是否匹配
                if (actualMD5 != expectedMD5)
                {
                    MessageBox.Show($"文件 {filePath} 的MD5哈希值不匹配。\n预期: {expectedMD5}\n实际: {actualMD5}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"检查文件 {filePath} 时发生错误: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// 计算文件的MD5哈希值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>MD5哈希值</returns>
        public static string CalculateMD5(string filePath)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = System.IO.File.OpenRead(filePath))
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
                }
            }
        }

    }
    public class Update
    {
        private static void BatchWriter(bool Loading, string code)
        {
            string batPath = $"{Directory.GetCurrentDirectory()}\\temp\\update.bat";
            if (!System.IO.File.Exists(batPath))
            {
                System.IO.File.Create(batPath).Close();
            }
            if (Loading)
            {
                ClearFile(batPath);
                string command = $"@echo off&echo ==================================&echo =   NinjaMagisk Auto Updater     =&echo =                                =&echo =   Version 0.0.1                =&echo ==================================&cd /d {AppDomain.CurrentDomain.BaseDirectory}\n";
                using (StreamWriter sw = new StreamWriter(batPath, true, Encoding.Default))
                {
                    sw.Write(command);
                    sw.Close();
                }
                return;
            }
            // copy /y C:\123.r D:\123.r & start D:\123.r & pause
            using (StreamWriter sw = new StreamWriter(batPath, true, Encoding.Default))
            {
                sw.WriteLine(code);
                sw.Close();
            }
            return;
        }
        internal static string NewerVersions(string version1, string version2)
        {
            // 将版本号按小数点分割成数组
            string[] v1Parts = version1.Split('.');
            string[] v2Parts = version2.Split('.');

            // 获取两个版本号的最大长度
            int maxLength = Math.Max(v1Parts.Length, v2Parts.Length);

            for (int i = 0; i < maxLength; i++)
            {
                // 如果当前部分超出数组范围，则补0
                int v1Part = i < v1Parts.Length ? int.Parse(v1Parts[i]) : 0;
                int v2Part = i < v2Parts.Length ? int.Parse(v2Parts[i]) : 0;

                // 比较当前部分的大小
                if (v1Part > v2Part)
                {
                    return version1; // version1 更新
                }
                else if (v1Part < v2Part)
                {
                    return version2; // version2 更新
                }
            }

            // 如果所有部分都相同，则版本号相同
            return "same";
        }
        private static readonly HttpClient _httpClient = new HttpClient();
        public enum Platform
        {
            Github,
            Gitee,
        }
        public static async Task<bool> CheckUpdate(string CheckUpdateUrl, Platform platform)
        {
            /* Github API规定的Release最新发行版查询地址为       https://api/github.com/repos/{用户名}/{仓库}/releases/latest
             * 
             * Gitee API规定的Release最新发行版查询地址为       https://gitee.com/api/v5/repos/{用户名}/{仓库}/releases/latest
             * 
             * 返回的json中包含了最新发行版的信息，包括版本号、发布时间、下载地址等 例如,最新的版本号为 "tag_name": "v1.4",
             */
            string jsonResponse = await FetchJsonFromUrl(CheckUpdateUrl);
            if (!string.IsNullOrEmpty(jsonResponse))
            {
                var (TagName, Name) = ExtractTagAndName(jsonResponse, platform);
                WriteLog(LogLevel.Info, $"{_LATEST_VERSION}: {TagName} - {Name}");
                string[] strings = TagName.Split('v');
                string res = NewerVersions(Software.Version, strings[1]);
                if (res == "same" || res == Software.Version)
                {
                    WriteLog(LogLevel.Info, _NON_NEW_VER);
                    return false;
                }
                else
                {
                    WriteLog(LogLevel.Info, $"{_NEW_VERSION_AVAILABLE}: {res} {_CURRENT_VER}: {Software.Version}");
                    return true;
                }
            }
            else
            {
                WriteLog(LogLevel.Error, _JSON_PARSING_FAILED);
                return false;
            }
        }
        private static async Task<string> FetchJsonFromUrl(string url)
        {
            try
            {
                // GitHub API 需要 User-Agent 头
                if (url.Contains("github.com") && !_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
                {
                    _httpClient.DefaultRequestHeaders.Add("User-Agent", "C# HttpClient");
                }

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // 确保请求成功
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                WriteLog(LogLevel.Error, $"{_ERROR}: {ex.Message}");
                return null;
            }
        }
        private static (string TagName, string Name) ExtractTagAndName(string json, Platform platform)
        {
            try
            {
                // 解析 JSON
                JObject jsonObject = JObject.Parse(json);

                // 提取 tag_name
                string tagName = jsonObject["tag_name"]?.ToString();

                // 根据平台提取 name
                string name;
                switch (platform)
                {
                    case Platform.Github:
                        name = jsonObject["name"]?.ToString(); // GitHub 的 name 在根节点
                        break;
                    case Platform.Gitee:
                        name = jsonObject["name"]?.ToString(); // Gitee 的 name 在 prerelease 对象中
                        break;
                    default:
                        throw new ArgumentException(_UNSUPPORT_PLATFORM);
                }
                return (tagName, name);
            }
            catch (Exception ex)
            {
                WriteLog(LogLevel.Error, $"{_JSON_PARSING_FAILED}: {ex.Message}");
                return (null, null);
            }
        }
        /*        
规定 更新文件为 `Update_{version}.zip` ,并且在压缩包内包含了 `update.ini` 和 `filehash.ini` 文件,以及更新文件

{version} 为版本号

```
 压缩包文件目录:
Update_{ version}.zip          // 更新文件压缩包    
    ├── update.ini             // 更新信息    
    ├── filehash.ini           // 文件哈希值        
    └── #(update files)        // 更新文件 
```

 规定 ``update.ini`` 规格:
```
    1 > version = ""                              // 版本号     
    2 > type = [Release / HotFix / bugFix]        // 更新类型       
    3 > description = ""                          // 更新说明        
    4 > updatefilecount = ""                      // 更新文件数量       
    5 > hashurl = ""                              // 哈希值文件下载地址       
    6 > hash = ""                                 // 文件数量 
```

 规定 ``filehash.ini`` 规格:
```
    > { fileName},{ fileHash}
示例:
    1 > Library.dll,4CC1ED4D70DFC8A7455822EC8339D387
    2 > Library.pdb, FDFA7596701DCC2E96D462DBC35E7823
```           
*/
        public static void SelfUpdater()
        {
            try
            {
                string updateDir = Path.Combine(Directory.GetCurrentDirectory(), "update");
                if (!Directory.Exists(updateDir))
                {
                    Directory.CreateDirectory(updateDir);
                    WriteLog(LogLevel.Info, $"{_CREATE_DIRECTORY}: {updateDir}");
                }
                var temp = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp";
                var update = $"{Directory.GetCurrentDirectory()}\\update";
                string batPath = $"{Directory.GetCurrentDirectory()}\\temp\\update.bat";
                OpenFileDialog fileDialog = new OpenFileDialog
                {
                    DefaultExt = "",
                    Title = "选择一个更新文件",
                    Filter = "更新文件压缩包(*.zip)|*.zip"
                };
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {

                    string UpdateFile = fileDialog.FileName;
                    WriteLog(LogLevel.Info, $"Selected update file: {UpdateFile}");
                    if (CheckFilesInArchive(UpdateFile))
                    {
                        WriteLog(LogLevel.Info, $"Archive Passed");
                        string path = ExtractUpdateFiles(update, UpdateFile);
                        if (string.IsNullOrEmpty(path))
                        {
                            WriteLog(LogLevel.Error, "Failed to extract update files.");
                            return;
                        }

                        WriteLog(LogLevel.Info, $"Files extracted to: {path}");
                        // 读取 filehash.ini 文件
                        string updateIniPath = Path.Combine(path, "filehash.ini");
                        if (!System.IO.File.Exists(updateIniPath))
                        {
                            WriteLog(LogLevel.Error, $"update.ini not found in: {path}");
                            return;
                        }

                        string[] lines = System.IO.File.ReadAllLines(updateIniPath);
                        WriteLog(LogLevel.Info, $"Reading filehash.ini from: {updateIniPath}");
                        BatchWriter(true, "");
                        foreach (string line in lines)
                        {
                            WriteLog(LogLevel.Info, "Reading line: " + line);
                            string[] parts = line.Split(',');
                            if (parts.Length < 2)
                            {
                                WriteLog(LogLevel.Error, $"Invalid line in filehash.ini: {line}");
                                break;
                            }
                            string fileName = parts[0].Trim();
                            string expectedHash = parts[1].Trim();

                            BatchWriter(false, $"copy /y {Directory.GetCurrentDirectory()}\\update\\{fileName} {Directory.GetCurrentDirectory()}");
                            string filePath = Path.Combine(path, fileName);
                            string destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "update", fileName);

                            WriteLog(LogLevel.Info, $"Verifying file: {filePath}, Expected Hash: {expectedHash}");

                            // 验证文件哈希值

                            if (NinjaMagisk.File.CheckFileHash(filePath, expectedHash))
                            {
                                WriteLog(LogLevel.Info, $"File {fileName} Passed");

                                // 确保目标目录存在
                                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                                // 移动文件
                                System.IO.File.Move(filePath, destinationPath);
                                WriteLog(LogLevel.Info, $"File {fileName} moved to: {destinationPath}");
                            }
                            else
                            {
                                WriteLog(LogLevel.Error, $"File {fileName} Failed");
                                MessageBox.Show($"{_ERROR}: The file {fileName} did not pass MD5 verification.", _ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            }
                        }
                        BatchWriter(false, $"echo     Update successful!&pause&start {Application.ExecutablePath}");
                        Process.Start(batPath);
                        Application.Exit();
                    }
                    else
                    {
                        WriteLog(LogLevel.Error, $"Archive Failed");
                        MessageBox.Show($"{_ERROR}: The archive not passed structure verify.", _ERROR, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogLevel.Error, $"{_ERROR}: {ex.Message}");
            }
        }


        private static string ExtractUpdateFiles(string path, string UpdateFile)
        {
            Process process = new Process();
            process.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\7za.exe";
            process.StartInfo.Arguments = $"x \"{UpdateFile}\" -o{path} -y -aoa";
            process.Start();
            process.WaitForExit();
            // 获取解压后的文件夹路径
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (file.Contains("update.ini"))
                {
                    return Path.GetDirectoryName(file);
                }
            }
            return null;
        }
        private static bool CheckFilesInArchive(string UpdateFile)
        {
            string[] filesToCheck = { "update.ini", "filehash.ini" };
            DownloadAssistant.ModuleDownloader(DownloadAssistant.Module.zip);
            Process process = new Process();
            process.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\7za.exe";
            process.StartInfo.Arguments = $"l \"{UpdateFile}\"";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            StreamReader streamReader = process.StandardOutput;
            string output = streamReader.ReadToEnd();
            foreach (string file in filesToCheck)
            {
                if (output.Contains(file))
                {
                    process.Close();
                    return true;
                }
            }
            if (process.ExitCode != 0)
            {
                WriteLog(LogLevel.Error, $"{_ERROR}: {process.ExitCode}");
                process.Close();
                return false;
            }
            process.Close();
            return false;
        }
    }
    public class Config
    {
        public static string ReadConfig(string iniPath,string HeadText)
        {
            string[] Texts = System.IO.File.ReadAllLines(iniPath);
            foreach (string Text in Texts)
            {
                if (Text.Contains(HeadText))
                {
                    string[] part2 = Text.Split('=');
                    return part2[1].Trim();
                }
            }
            return null;
        }
    }
}