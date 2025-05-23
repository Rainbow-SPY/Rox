using Rox.Runtimes.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
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
        /// 日志类库,在控制台输出日志并记录到文件
        /// </summary>
        public class LogLibraries
        {
            private static readonly string _CLEAR_LOGFILE = "清空日志文件成功";
            private static readonly string _CANNOT_CLEAR_LOGFILE = "清空日志文件失败";
            public static Action<LogLevel, string> LogToUi { get; set; }
            /// <summary>
            /// 日志等级,分为Info,Error,Warning
            /// </summary>
            public enum LogLevel
            {
                /// <summary>
                /// 信息
                /// </summary>
                Info,
                /// <summary>
                /// 错误
                /// </summary>
                Error,
                /// <summary>
                /// 警告
                /// </summary>
                Warning
            }
            /// <summary>
            /// 日志类型,分为Form,Thread,Process,Service,Task,System,PowerShell,Registry,Network
            /// </summary>
            public enum LogKind
            {
                /// <summary>
                /// 窗体  
                /// </summary>
                Form,
                /// <summary>
                /// 线程
                /// </summary>
                Thread,
                /// <summary>
                /// 进程
                /// </summary>
                Process,
                /// <summary>
                /// 服务
                /// </summary>
                Service,
                /// <summary>
                /// 任务
                /// </summary>
                Task,
                /// <summary>
                /// 系统
                /// </summary>
                System,
                /// <summary>
                /// PowerShell
                /// </summary>
                PowerShell,
                /// <summary>
                /// 注册表
                /// </summary>
                Registry,
                /// <summary>
                /// 网络
                /// </summary>
                Network,
            }
            // 定义日志文件名和路径（当前目录下的 Assistant.log 文件）
            /// <summary>
            /// 日志文件名
            /// </summary>
            private static readonly string logFileName = "Assistant.log";
            /// <summary>
            /// 日志文件路径
            /// </summary>
            private static readonly string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), logFileName);
            /// <summary>
            /// 根据日志等级和日志类型向文件写入日志,并在控制台输出日志,并记录到文件
            /// </summary>
            /// <param name="logLevel">日志等级</param>
            /// <param name="logKind">日志类型</param>
            /// <param name="message">消息</param>
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
            /// <summary>
            /// 根据日志等级向文件写入日志,并在控制台输出日志,并记录到文件
            /// </summary>
            /// <param name="logLevel">日志等级</param>
            /// <param name="message">消息</param>
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
            /// <summary>
            /// 根据日志等级记录日志到文件
            /// </summary>
            /// <param name="logLevel">日志等级</param>
            /// <param name="message">消息</param>
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
            /// <summary>
            /// 根据日志等级和日志类型记录日志到文件
            /// </summary>
            /// <param name="logLevel"></param>
            /// <param name="logkind"></param>
            /// <param name="message"></param>
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
            /// <summary>
            /// 清空日志文件
            /// </summary>
            /// <param name="filePath"></param>
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
        /// <summary>
        /// 根据语言获取资源文件
        /// </summary>
        public class ResourceHelper
        {
            /// <summary>
            /// 判断是否为简体中文
            /// </summary>
            /// <param name="lang"></param>
            /// <returns>语言字符串</returns>
            private static bool IsChineseSimple(string lang)
            {
                return lang == "zh-CN" || lang == "zh-CHS";
            }
            /// <summary>
            /// 获取资源管理器
            /// </summary>
            /// <param name="lang"></param>
            /// <returns>指定语言文件的资源管理器</returns>
            private static ResourceManager GetResourceManager(string lang)
            {
                if (IsChineseSimple(lang))
                {
                    // 如果是中文（zh-CN 或 zh-Hans），返回 Resources.resx 的资源管理器
                    return new ResourceManager("Rox.Runtimes.Properties.Resources", typeof(Resources).Assembly);
                }
                else
                {
                    // 如果是其他语言，返回 Resource1.resx 的资源管理器
                    return new ResourceManager("Rox.Runtimes.Properties.Resource1", typeof(Resource1).Assembly);
                }
            }
            /// <summary>
            /// 获取字符串资源
            /// </summary>
            /// <param name="key">自定义字段</param>
            /// <param name="lang">语言代码</param>
            /// <returns>指定语言文件中的字符串</returns>
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
        /// <summary>
        /// 本地化字符串类
        /// </summary>
        public class LocalizedString
        {
            public static string Language = GetLocalizedString("Language");
            public static readonly string Version = GetLocalizedString("Version");
            public static readonly string Author = GetLocalizedString("Author");
            public static readonly string Copyright = GetLocalizedString("Copyright");
            public static readonly string Protect = GetLocalizedString("Protect");
            public static readonly string _FILE_EXIST = GetLocalizedString("_FILE_EXIST");
            public static readonly string _FILE_WRITING = GetLocalizedString("_FILE_WRITING");
            public static readonly string _FILE_EXIST_PATH = GetLocalizedString("_FILE_EXIST_PATH");
            public static readonly string _RES_FILE_NOT_FIND = GetLocalizedString("_RES_FILE_NOT_FIND");
            public static readonly string _CANNOT_WRITE_LOGFILE = GetLocalizedString("_CANNOT_WRITE_LOGFILE");
            public static readonly string _CLEAR_LOGFILE = GetLocalizedString("_CLEAR_LOGFILE");
            public static readonly string _CANNOT_CLEAR_LOGFILE = GetLocalizedString("_CANNOT_CLEAR_LOGFILE");
            public static readonly string _GET_OUTPUT_DIRECTORY = GetLocalizedString("_GET_OUTPUT_DIRECTORY");
            public static readonly string _GET_OUTPUT_NAME = GetLocalizedString("_GET_OUTPUT_NAME");
            public static readonly string _CREATE_DIRECTORY = GetLocalizedString("_CREATE_DIRECTORY");
            public static readonly string _GET_DIRECTORY = GetLocalizedString("_GET_DIRECTORY");
            public static readonly string _NOTAVAILABLE_NETWORK = GetLocalizedString("_NOTAVAILABLE_NETWORK");
            public static readonly string _NOTAVAILABLE_NETWORK_TIPS = GetLocalizedString("_NOTAVAILABLE_NETWORK_TIPS");
            public static readonly string _TIPS = GetLocalizedString("_TIPS");
            public static readonly string _ERROR = GetLocalizedString("_ERROR");
            public static readonly string _WARNING = GetLocalizedString("_WARNING");
            public static readonly string _GET_URL = GetLocalizedString("_GET_URL");
            public static readonly string _GET_TEMP = GetLocalizedString("_GET_TEMP");
            public static readonly string _REGEX_GET_FILE = GetLocalizedString("_REGEX_GET_FILE");
            public static readonly string _GET_FILES_IN_DIRECTORY = GetLocalizedString("_GET_FILES_IN_DIRECTORY");
            public static readonly string _GET_SYSTEM_BIT = GetLocalizedString("_GET_SYSTEM_BIT");
            public static readonly string _FINDING_HTML_DOWNLOAD_LINK = GetLocalizedString("_FINDING_HTML_DOWNLOAD_LINK");
            public static readonly string _FIND_HTML_CODE = GetLocalizedString("_FIND_HTML_CODE");
            public static readonly string _SECURITY_RUNNING = GetLocalizedString("_SECURITY_RUNNING");
            public static readonly string _PROCESS_STARTED = GetLocalizedString("_PROCESS_STARTED");
            public static readonly string _PROCESS_EXITED = GetLocalizedString("_PROCESS_EXITED");
            public static readonly string _CANNOT_DISENABLE_HIBERNATE = GetLocalizedString("_CANNOT_DISENABLE_HIBERNATE");
            public static readonly string _DISENABLE_HIBERNATE = GetLocalizedString("_DISENABLE_HIBERNATE");
            public static readonly string _CANNOT_ENABLE_HIGHPOWERCFG = GetLocalizedString("_CANNOT_ENABLE_HIGHPOWERCFG");
            public static readonly string _ENABLE_HIGHPOWERCFG = GetLocalizedString("_ENABLE_HIGHPOWERCFG");
            public static readonly string _CANNOT_DISABLE_SECURITY_CENTER = GetLocalizedString("_CANNOT_DISABLE_SECURITY_CENTER");
            public static readonly string _CANNOT_ENABLE_SECURITY_CENTER = GetLocalizedString("_CANNOT_ENABLE_SECURITY_CENTER");
            public static readonly string _DISABLE_SECURITY_CENTER = GetLocalizedString("_DISABLE_SECURITY_CENTER");
            public static readonly string _ENABLE_SECURITY_CENTER = GetLocalizedString("_ENABLE_SECURITY_CENTER");
            public static readonly string _WRITE_REGISTRY = GetLocalizedString("_WRITE_REGISTRY");
            public static readonly string _CANNOT_DISABLE_WINDOWS_UPDATER = GetLocalizedString("_CANNOT_DISABLE_WINDOWS_UPDATER");
            public static readonly string _CANNOT_ENABLE_WINDOWS_UPDATER = GetLocalizedString("_CANNOT_ENABLE_WINDOWS_UPDATER");
            public static readonly string _DISABLE_WINDOWS_UPDATER = GetLocalizedString("_DISABLE_WINDOWS_UPDATER");
            public static readonly string _ENABLE_WINDOWS_UPDATER = GetLocalizedString("_ENABLE_WINDOWS_UPDATER");
            public static readonly string _ACTIVE_WINDOWS = GetLocalizedString("_ACTIVE_WINDOWS");
            public static readonly string _CANNOT_ACTIVE_WINDOWS = GetLocalizedString("_CANNOT_ACTIVE_WINDOWS");
            public static readonly string _SUCESS_WRITE_REGISTRY = GetLocalizedString("_SUCESS_WRITE_REGISTRY");
            public static readonly string _WRITE_REGISTRY_FAILED = GetLocalizedString("_WRITE_REGISTRY_FAILED");
            public static readonly string _GET_ARIA2C_ARGS = GetLocalizedString("_GET_ARIA2C_ARGS");
            public static readonly string _GET_ARIA2C_PATH = GetLocalizedString("_GET_ARIA2C_PATH");
            public static readonly string _GET_ARIA2C_EXITCODE = GetLocalizedString("_GET_ARIA2C_EXITCODE");
            public static readonly string _ENABLE_ARIA2C_LOG_OUTPUT = GetLocalizedString("_ENABLE_ARIA2C_LOG_OUTPUT");
            public static readonly string _DISABLE_ARIA2C_LOG_OUTPUT = GetLocalizedString("_DISABLE_ARIA2C_LOG_OUTPUT");
            public static readonly string _DOWNLOADING_FILE = GetLocalizedString("_DOWNLOADING_FILE");
            public static readonly string _DOWNLOADING_COMPLETE = GetLocalizedString("_DOWNLOADING_COMPLETE");
            public static readonly string _DOWNLOADING_FAILED = GetLocalizedString("_DOWNLOADING_FAILED");
            public static readonly string _GET_64_LINK = GetLocalizedString("_GET_64_LINK");
            public static readonly string _GET_32_LINK = GetLocalizedString("_GET_32_LINK");
            public static readonly string _GET_RM_NAME = GetLocalizedString("_GET_RM_NAME");
            public static readonly string _GET_RM_OBJ = GetLocalizedString("_GET_RM_OBJ");
            public static readonly string _NEW_RM = GetLocalizedString("_NEW_RM");
            public static readonly string _GET_HTML = GetLocalizedString("_GET_HTML");
            public static readonly string _32 = GetLocalizedString("_32");
            public static readonly string _64 = GetLocalizedString("_64");
            public static readonly string _GET_FILE = GetLocalizedString("_GET_FILE");
            public static readonly string _WAIT_DOWNLOADING = GetLocalizedString("_WAIT_DOWNLOADING");
            public static readonly string _RETRY_DOWNLOAD = GetLocalizedString("_RETRY_DOWNLOAD");
            public static readonly string _ERROR_CODE = GetLocalizedString("_ERROR_CODE");
            public static readonly string _LOGIN_ERROR_USER_OR_PASSWORD = GetLocalizedString("_LOGIN_ERROR_USER_OR_PASSWORD");
            public static readonly string _LOGIN_VERIFY = GetLocalizedString("_LOGIN_VERIFY");
            public static readonly string _CANCEL_OP = GetLocalizedString("_CANCEL_OP");
            public static readonly string _UNKNOW_ERROR = GetLocalizedString("_UNKNOW_ERROR");
            public static readonly string _GET_RESPONSE = GetLocalizedString("_GET_RESPONSE");
            public static readonly string _ANSWER = GetLocalizedString("_ANSWER");
            public static readonly string _SEND_REQUEST = GetLocalizedString("_SEND_REQUEST");
            public static readonly string _LOGIN_VERIFY_ERROR = GetLocalizedString("_LOGIN_VERIFY_ERROR");
            public static readonly string _ENTER_CREDENTIALS = GetLocalizedString("_ENTER_CREDENTIALS");
            public static readonly string _SUCCESS_VERIFY = GetLocalizedString("_SUCCESS_VERITY");
            public static readonly string _LATEST_VERSION = GetLocalizedString("_LATEST_VERSION");
            public static readonly string _UNSUPPORT_PLATFORM = GetLocalizedString("_UNSUPPORT_PLATFORM");
            public static readonly string _JSON_PARSING_FAILED = GetLocalizedString("_JSON_PARSING_FAILED");
            public static readonly string _NEW_VERSION_AVAILABLE = GetLocalizedString("_NEW_VERSION_AVAILABLE");
            public static readonly string _NON_NEW_VER = GetLocalizedString("_NON_NEW_VER");
            public static readonly string _CURRENT_VER = GetLocalizedString("_CURRENT_VER");
            public static readonly string _ADD_NEW_LINE = GetLocalizedString("_ADD_NEW_LINE");
            public static readonly string _UPDATE_LINE = GetLocalizedString("_UPDATE_LINE");
            public static readonly string _READ_FILE = GetLocalizedString("_READ_FILE");
            public static readonly string _WRITE_FILE = GetLocalizedString("_WRITE_FILE");
            public static readonly string _WINDOWS_UPDATER_DISABLED = GetLocalizedString("_WINDOWS_UPDATER_DISABLED");
            public static readonly string _WINDOWS_UPDATER_ENABLED = GetLocalizedString("_WINDOWS_UPDATER_ENABLED");
            public static readonly string _READ_REGISTRY_FAILED = GetLocalizedString("_READ_REGISTRY_FAILED");
            /// <summary>
            /// 获取本地化字符串
            /// </summary>
            /// <param name="key">字符串常量</param>
            /// <returns>指定语言文件中的字符串</returns>
            public static string GetLocalizedString(string key)
            {
                return ResourceHelper.GetString(key, System.Globalization.CultureInfo.InstalledUICulture.Name.ToString());
            }

        }
        /// <summary>
        /// Node.Js 类
        /// </summary>
        public class NodeJs
        {
            /// <summary>
            /// 提取 Node.Js,版本号: node-v22.15.1-win-x86
            /// </summary>
            /// <param name="ExtraedFolder"> 存放的文件夹</param>
            /// <returns> 返回提取结果, 如果提取成功则返回文件路径, 否则返回错误信息或返回值</returns>
            public static string ExtractNodeJs(string ExtraedFolder)
            {
                //检查文件夹是否合法
                // 伪代码：
                // 1. 检查 ExtraedFolder 是否为 null 或空字符串 或 不是有效路径（如只包含无效字符或为根目录等）
                // 2. 如果非法，则爆出错误返回
                // 3. 否则，使用传入的 ExtraedFolder

                // 替换原有判断逻辑如下：
                if (string.IsNullOrWhiteSpace(ExtraedFolder) || Path.GetFileName(ExtraedFolder) == string.Empty)
                {
                    WriteLog(LogLevel.Error, $"{ExtraedFolder}值为null或空字符串");
                    MessageBox.Show($"{ExtraedFolder}值为null或空字符串", "错误的路径! - Rox", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return $"Error";
                }
                else
                {
                    LogLibraries.WriteLog(LogLevel.Info, $"{_GET_DIRECTORY}: {ExtraedFolder}");
                }
                // 检查参数是否为文件夹格式
                // 检查文件夹是否存在
                if (!Directory.Exists(ExtraedFolder))
                {
                    // 创建文件夹
                    Directory.CreateDirectory(ExtraedFolder);
                    WriteLog(LogLevel.Info, $"{_CREATE_DIRECTORY}: {ExtraedFolder}");
                }


                // 检查文件是否存在
                if (File.Exists(Path.Combine(ExtraedFolder, "node-v22.15.1-win-x86", "node.exe")))
                {
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, "Node.Js 已经提取");
                    LogLibraries.WriteLog(LogLevel.Info, $"{_FILE_EXIST}: {Path.Combine(ExtraedFolder, "node.exe")}");
                    return $"{Path.Combine(ExtraedFolder, "node-v22.15.1-win-x86", "node.exe")}"; // 返回文件路径
                }
                else
                {
                    // 从Resources.resx文件中提取NodeJs
                    // 这里假设你已经有一个资源文件，里面包含了NodeJs的压缩包
                    // 获取当前正在执行的类库的程序集
                    Assembly assembly = Assembly.GetExecutingAssembly();

                    // 假设Node.Js.zip是嵌入在"Namespace.Resources"命名空间中的

                    string resourceName = "Rox.Runtimes.Properties.Resources"; // 替换为你的资源路径

                    // 创建 ResourceManager 实例
                    ResourceManager rm = new ResourceManager(resourceName, assembly);
                    LogLibraries.WriteLog(LogLevel.Info, $"{_NEW_RM}");
                    // 从资源中获取Node.Js.zip文件的字节数据
                    byte[] NodeJsZipData = (byte[])rm.GetObject("Node_js");
                    LogLibraries.WriteLog(LogLevel.Info, $"{_GET_RM_OBJ}: Node_js");
                    if (NodeJsZipData != null)
                    {
                        // 将文件保存到当前目录
                        string outputDirectory = Path.GetTempPath();
                        // 检查并创建目录
                        if (!Directory.Exists(outputDirectory))
                        {
                            Directory.CreateDirectory(outputDirectory);
                            LogLibraries.WriteLog(LogLevel.Info, $"{_CREATE_DIRECTORY}");
                        }
                        LogLibraries.WriteLog(LogLevel.Info, $"{_GET_OUTPUT_DIRECTORY}: {outputDirectory}");
                        // 保存文件路径
                        string outputFilePath = Path.Combine(outputDirectory, "Node.Js.zip");
                        LogLibraries.WriteLog(LogLevel.Info, $"{_GET_OUTPUT_NAME}: {outputDirectory}");
                        // 写入文件，确保保存为二进制数据
                        LogLibraries.WriteLog(LogLevel.Info, $"{_FILE_WRITING}");
                        System.IO.File.WriteAllBytes(outputFilePath, NodeJsZipData);
                        LogLibraries.WriteLog(LogLevel.Info, $"Node.Js.zip {_FILE_EXIST_PATH} {outputFilePath}");
                    }
                    else
                    {
                        LogLibraries.WriteLog(LogLevel.Error, $"{_RES_FILE_NOT_FIND}");
                        return _RES_FILE_NOT_FIND;
                    }
                    // 解压缩文件
                    string zipFilePath = Path.Combine(Path.GetTempPath(), "Node.Js.zip");
                    Process zip = new Process();
                    zip.StartInfo.FileName = "powershell.exe";
                    zip.StartInfo.Arguments = $"-Command \"Expand-Archive -Path '{zipFilePath}' -DestinationPath '{ExtraedFolder}'\"";
                    zip.StartInfo.UseShellExecute = false;
                    zip.StartInfo.CreateNoWindow = true;
                    zip.Start();
                    zip.WaitForExit();
                    if (zip.ExitCode == 0)
                    {
                        LogLibraries.WriteLog(LogLevel.Info, $"Node.js {_DOWNLOADING_COMPLETE}");
                        LogLibraries.WriteLog(LogLevel.Info, $"{_FILE_EXIST_PATH} {ExtraedFolder}");
                        return $"{Path.Combine(ExtraedFolder, "node-v22.15.1-win-x86", "node.exe")}";
                    }
                    else
                    {
                        LogLibraries.WriteLog(LogLevel.Error, $"Node.js {_DOWNLOADING_FAILED}");
                        LogLibraries.WriteLog(LogLevel.Error, $"{zip.ExitCode}");
                        return $"{zip.ExitCode}";
                    }
                }
            }
            /// <summary>
            /// 检查 Node.Js 是否存在
            /// </summary>
            /// <param name="ExtraedFolder"></param>
            /// <returns> 返回提取结果, 如果提取成功则返回文件路径, 否则返回错误信息或返回值</returns>
            public static string CheckNodeJs(string ExtraedFolder)
            {
                // 检查文件夹是否合法
                // 伪代码：
                // 1. 检查 ExtraedFolder 是否为 null 或空字符串 或 不是有效路径（如只包含无效字符或为根目录等）
                // 2. 如果非法，则爆出错误返回
                // 3. 否则，使用传入的 ExtraedFolder
                // 替换原有判断逻辑如下：
                if (string.IsNullOrWhiteSpace(ExtraedFolder) || Path.GetFileName(ExtraedFolder) == string.Empty)
                {
                    WriteLog(LogLevel.Error, $"{ExtraedFolder}值为null或空字符串");
                    MessageBox.Show($"{ExtraedFolder}值为null或空字符串", "错误的路径! - Rox", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return $"Error";
                }
                else
                {
                    LogLibraries.WriteLog(LogLevel.Info, $"{_GET_DIRECTORY}: {ExtraedFolder}");
                }
                // 检查参数是否为文件夹格式
                // 检查文件夹是否存在
                if (!Directory.Exists(ExtraedFolder))
                {
                    // 创建文件夹
                    Directory.CreateDirectory(ExtraedFolder);
                    WriteLog(LogLevel.Info, $"{_CREATE_DIRECTORY}: {ExtraedFolder}");
                }
                // 检查文件是否存在
                if (File.Exists(Path.Combine(ExtraedFolder, "node-v22.15.1-win-x86", "node.exe")))
                {
                    LogLibraries.WriteLog(LogLevel.Info, "Node.Js 已经提取");
                    LogLibraries.WriteLog(LogLevel.Info, $"{_FILE_EXIST}: {Path.Combine(ExtraedFolder, "node.exe")}");
                    return $"{Path.Combine(ExtraedFolder, "node-v22.15.1-win-x86", "node.exe")}"; // 返回文件路径
                }
                else
                {
                    LogLibraries.WriteLog(LogLevel.Error, $"File Not Exist");
                    string _returnValue = Rox.Runtimes.NodeJs.ExtractNodeJs(ExtraedFolder);
                    char drive = _returnValue[0];
                    if ((drive >= 'A' && drive <= 'Z') || (drive >= 'a' && drive <= 'z') && _returnValue[1] == ':')
                    {
                        WriteLog(LogLevel.Info, $"{_FILE_EXIST} {_returnValue}");
                        return _returnValue; // 返回文件路径
                    }

                    if (_returnValue == "Error")
                    {
                        WriteLog(LogLevel.Error, "Node.js 在 ResourceManager中提取资源包失败.");
                        return "Error"; //返回错误信息
                    }
                    // 检查返回值是否为int
                    else if (int.TryParse(_returnValue, out int exitCode))
                    {
                        // 如果返回值是int类型，则表示提取失败
                        WriteLog(LogLevel.Error, $"Node.js 在 PowerShell 中解压缩资源包时失败，错误代码: {exitCode}");
                        return exitCode.ToString(); // 返回错误代码
                    }
                    else
                    {
                        WriteLog(LogLevel.Info, $"Node.js 在 ResourceManager中返回了 {_RES_FILE_NOT_FIND}, 请检查资源文件是否存在.");
                        return _RES_FILE_NOT_FIND; // 返回错误信息
                    }
                }
                // 检查返回值是否为路径
            }
        }
        /// <summary>
        /// 网络相关操作
        /// </summary>
        public class Network
        {
            /// <summary>
            /// 检查网络是否可用
            /// </summary>
            /// <returns> 可用返回 <see langword="true"></see> 不可用返回 <see langword="false"></see></returns>
            public static bool IsNetworkAvailable()
            {
                try
                {
                    // 检查网络适配器是否有可用的
                    //if (!NetworkInterface.GetIsNetworkAvailable())
                    //{
                    //    WriteLog(LogLevel.Info, $"{_NOTAVAILABLE_NETWORK}");
                    //    return false;
                    //}

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
                    WriteLog(LogLevel.Warning, $"{_NOTAVAILABLE_NETWORK}");
                    return false;
                }
            }
            /// <summary>
            /// 检查网络是否可用
            /// </summary>
            /// <param name="ip"> IP地址</param>
            /// <returns> 可用返回 <see langword="true"></see> 不可用返回 <see langword="false"></see></returns>
            public static bool Ping(string ip)
            {
                try
                {
                    using (var ping = new Ping())
                    {
                        PingReply reply = ping.Send(ip, 120);
                        return reply.Status == IPStatus.Success ? true : false;
                    }
                }
                catch
                {
                    WriteLog(LogLevel.Warning, $"{_NOTAVAILABLE_NETWORK}");
                    return false;
                }
            }

        }

    }
}
