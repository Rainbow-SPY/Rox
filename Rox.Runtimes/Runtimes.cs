using Microsoft.Win32;
using Rox.Runtimes.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    namespace Runtimes
    {
        /// <summary>
        /// 用于处理文件操作
        /// </summary>
        public class File_I
        {
            /// <summary>
            /// 定义了文件的厘性选项，包括只读、系统、隐藏和归档。
            /// </summary>
            public enum Properties
            {
                /// <summary>
                /// 只读属性
                /// </summary>
                Readonly,
                /// <summary>
                /// 系统属性
                /// </summary>
                System,
                /// <summary>
                /// 隐藏属性
                /// </summary>
                Hidden,
                /// <summary>
                /// 归档属性
                /// </summary>
                Archive,
            }
            /// <summary>
            /// 用于设置文件的属性
            /// </summary>
            /// <param name="path"> 文件路径</param>
            /// <param name="key"> 属性选项</param>
            /// <param name="Enable"> 开关</param>
            public void FileProperties(string path, Properties key, bool Enable)
            {
                string arg;
                string Switch = Enable ? "+" : "-";
                if (key == Properties.Readonly) arg = $"{Switch}r";
                else if (key == Properties.System) arg = $"{Switch}s";
                else if (key == Properties.Hidden) arg = $"{Switch}h";
                else if (key == Properties.Archive) arg = $"{Switch}a";
                else
                {
                    MessageBox.Show("Unsupported property type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    WriteLog(LogLevel.Error, "_UNSUPPORT_PROPERTY_TYPE");
                    return;
                }
                Process process = new Process();
                process.StartInfo.FileName = "attrib";
                process.StartInfo.Arguments = $"{arg} {path}";
                process.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process.Id}");
                process.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process.ExitCode}");
                process.Close();
            }
            /// <summary>
            /// 用于检查文件的哈希值是否与预期的哈希值匹配
            /// </summary>
            /// <param name="filePath"> 文件路径</param>
            /// <param name="expectedMD5"> 预期的MD5哈希值</param>
            /// <returns> 如果哈希值匹配，则返回 <see langword="true"/>，否则返回 <see langword="false"/></returns>
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
        /// <summary>
        /// 日志类库,在控制台输出日志并记录到文件
        /// </summary>
        public class LogLibraries
        {
            private static readonly string _CLEAR_LOGFILE = "清空日志文件成功";
            private static readonly string _CANNOT_CLEAR_LOGFILE = "清空日志文件失败";
            /// <summary>
            /// 日志输出到 UI 的委托,用于在 UI 上显示日志
            /// </summary>
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
                /// 窗体  <see cref="System.Windows.Forms"/>
                /// </summary>
                Form,
                /// <summary>
                /// 线程 <see cref="System.Threading.Thread"/>
                /// </summary>
                Thread,
                /// <summary>
                /// 进程 <see cref="System.Diagnostics.Process"/>
                /// </summary>
                Process,
                /// <summary>
                /// 服务
                /// </summary>
                Service,
                /// <summary>
                /// 任务 <see cref="System.Threading.Tasks.Task"/>
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
                /// <summary>
                /// Json
                /// </summary>
                Json,
                /// <summary>
                /// 正则表达式 <see cref="System.Text.RegularExpressions.Regex"/>
                /// </summary>
                Regex,
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
                LogToFile(logLevel, logKind, message);
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
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] [{logkind}]: {message}";

                try
                {
                    // 如果日志文件不存在，则创建
                    if (!File.Exists(logFilePath))
                    {
                        File.Create(logFilePath).Close();
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
        /// 用于处理注册表操作
        /// </summary>
        public class Registry_I
        {
            /// <summary>
            /// 用于写入注册表项的值
            /// </summary>
            /// <param name="keyPath"> 注册表项路径</param>
            /// <param name="valueName"> 注册表项名称</param>
            /// <param name="valueType"> 注册表项类型</param>
            /// <param name="valueData"> 注册表项数据</param>
            public static void Write(string keyPath, string valueName, object valueData, RegistryValueKind valueType)
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
            /// <summary>
            /// 用于读取注册表项的值
            /// </summary>
            /// <param name="keyName"> 注册表项路径</param>
            /// <param name="valueName"> 注册表项名称</param>
            /// <returns> 返回注册表项的值</returns>
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
            #region 本地化字符串
            /// <summary>
            /// 语言
            /// </summary>
            public static string Language = GetLocalizedString("Language");
            /// <summary>
            /// 版本
            /// </summary>
            public static readonly string Version = GetLocalizedString("Version");
            /// <summary>
            /// 作者
            /// </summary>
            public static readonly string Author = GetLocalizedString("Author");
            /// <summary>
            /// 版权所有
            /// </summary>
            public static readonly string Copyright = GetLocalizedString("Copyright");
            /// <summary>
            /// 产品
            /// </summary>
            public static readonly string Product = GetLocalizedString("Product");
            /// <summary>
            /// 文件存在
            /// </summary>
            public static readonly string _FILE_EXIST = GetLocalizedString("_FILE_EXIST");
            /// <summary>
            /// 写入文件
            /// </summary>
            public static readonly string _FILE_WRITING = GetLocalizedString("_FILE_WRITING");
            /// <summary>
            /// 文件已保存到
            /// </summary>
            public static readonly string _FILE_EXIST_PATH = GetLocalizedString("_FILE_EXIST_PATH");
            /// <summary>
            /// 资源文件未找到
            /// </summary>
            public static readonly string _RES_FILE_NOT_FIND = GetLocalizedString("_RES_FILE_NOT_FIND");
            /// <summary>
            /// 未能写入日志文件
            /// </summary>
            public static readonly string _CANNOT_WRITE_LOGFILE = GetLocalizedString("_CANNOT_WRITE_LOGFILE");
            /// <summary>
            /// 清空日志
            /// </summary>
            public static readonly string _CLEAR_LOGFILE = GetLocalizedString("_CLEAR_LOGFILE");
            /// <summary>
            /// 未能清空日志文件
            /// </summary>
            public static readonly string _CANNOT_CLEAR_LOGFILE = GetLocalizedString("_CANNOT_CLEAR_LOGFILE");
            /// <summary>
            /// 获取输出目录
            /// </summary>
            public static readonly string _GET_OUTPUT_DIRECTORY = GetLocalizedString("_GET_OUTPUT_DIRECTORY");
            /// <summary>
            /// 获取输出名称
            /// </summary>
            public static readonly string _GET_OUTPUT_NAME = GetLocalizedString("_GET_OUTPUT_NAME");
            /// <summary>
            /// 创建目录
            /// </summary>
            public static readonly string _CREATE_DIRECTORY = GetLocalizedString("_CREATE_DIRECTORY");
            /// <summary>
            /// 获取目录
            /// </summary>
            public static readonly string _GET_DIRECTORY = GetLocalizedString("_GET_DIRECTORY");
            /// <summary>
            /// 网络不可用
            /// </summary>
            public static readonly string _NOTAVAILABLE_NETWORK = GetLocalizedString("_NOTAVAILABLE_NETWORK");
            /// <summary>
            /// 网络不可用, 是否执行步骤?
            /// </summary>
            public static readonly string _NOTAVAILABLE_NETWORK_TIPS = GetLocalizedString("_NOTAVAILABLE_NETWORK_TIPS");
            /// <summary>
            /// 提示
            /// </summary>
            public static readonly string _TIPS = GetLocalizedString("_TIPS");
            /// <summary>
            /// 错误
            /// </summary>
            public static readonly string _ERROR = GetLocalizedString("_ERROR");
            /// <summary>
            /// 警告
            /// </summary>
            public static readonly string _WARNING = GetLocalizedString("_WARNING");
            /// <summary>
            /// 获取 URL
            /// </summary>
            public static readonly string _GET_URL = GetLocalizedString("_GET_URL");
            /// <summary>
            /// 获取临时目录
            /// </summary>
            public static readonly string _GET_TEMP = GetLocalizedString("_GET_TEMP");
            /// <summary>
            /// 正则表达式获取文件
            /// </summary>
            public static readonly string _REGEX_GET_FILE = GetLocalizedString("_REGEX_GET_FILE");
            /// <summary>
            /// 获取目录内的文件
            /// </summary>
            public static readonly string _GET_FILES_IN_DIRECTORY = GetLocalizedString("_GET_FILES_IN_DIRECTORY");
            /// <summary>
            /// 获取系统位数
            /// </summary>
            public static readonly string _GET_SYSTEM_BIT = GetLocalizedString("_GET_SYSTEM_BIT");
            /// <summary>
            /// 获取网页文件下载链接
            /// </summary>
            public static readonly string _FINDING_HTML_DOWNLOAD_LINK = GetLocalizedString("_FINDING_HTML_DOWNLOAD_LINK");
            /// <summary>
            /// 获取 HTML 代码
            /// </summary>
            public static readonly string _FIND_HTML_CODE = GetLocalizedString("_FIND_HTML_CODE");
            /// <summary>
            /// 安全软件正在运行
            /// </summary>
            public static readonly string _SECURITY_RUNNING = GetLocalizedString("_SECURITY_RUNNING");
            /// <summary>
            /// 进程已启动
            /// </summary>
            public static readonly string _PROCESS_STARTED = GetLocalizedString("_PROCESS_STARTED");
            /// <summary>
            /// 进程已退出
            /// </summary>
            public static readonly string _PROCESS_EXITED = GetLocalizedString("_PROCESS_EXITED");
            /// <summary>
            /// 未能禁用休眠
            /// </summary>
            public static readonly string _CANNOT_DISENABLE_HIBERNATE = GetLocalizedString("_CANNOT_DISENABLE_HIBERNATE");
            /// <summary>
            /// 已禁用休眠
            /// </summary>
            public static readonly string _DISENABLE_HIBERNATE = GetLocalizedString("_DISENABLE_HIBERNATE");
            /// <summary>
            /// 未能启用卓越性能模式
            /// </summary>
            public static readonly string _CANNOT_ENABLE_HIGHPOWERCFG = GetLocalizedString("_CANNOT_ENABLE_HIGHPOWERCFG");
            /// <summary>
            /// 启用卓越性能模式
            /// </summary>
            public static readonly string _ENABLE_HIGHPOWERCFG = GetLocalizedString("_ENABLE_HIGHPOWERCFG");
            /// <summary>
            /// 未能禁用安全中心
            /// </summary>
            public static readonly string _CANNOT_DISABLE_SECURITY_CENTER = GetLocalizedString("_CANNOT_DISABLE_SECURITY_CENTER");
            /// <summary>
            /// 未能启用安全中心
            /// </summary>
            public static readonly string _CANNOT_ENABLE_SECURITY_CENTER = GetLocalizedString("_CANNOT_ENABLE_SECURITY_CENTER");
            /// <summary>
            /// 已禁用安全中心
            /// </summary>
            public static readonly string _DISABLE_SECURITY_CENTER = GetLocalizedString("_DISABLE_SECURITY_CENTER");
            /// <summary>
            /// 已启用安全中心
            /// </summary>
            public static readonly string _ENABLE_SECURITY_CENTER = GetLocalizedString("_ENABLE_SECURITY_CENTER");
            /// <summary>
            /// 写入注册表
            /// </summary>
            public static readonly string _WRITE_REGISTRY = GetLocalizedString("_WRITE_REGISTRY");
            /// <summary>
            /// 未能禁用 Windows 更新
            /// </summary>
            public static readonly string _CANNOT_DISABLE_WINDOWS_UPDATER = GetLocalizedString("_CANNOT_DISABLE_WINDOWS_UPDATER");
            /// <summary>
            /// 未能启用 Windows 更新
            /// </summary>
            public static readonly string _CANNOT_ENABLE_WINDOWS_UPDATER = GetLocalizedString("_CANNOT_ENABLE_WINDOWS_UPDATER");
            /// <summary>
            /// 已禁用 Windows 更新
            /// </summary>
            public static readonly string _DISABLE_WINDOWS_UPDATER = GetLocalizedString("_DISABLE_WINDOWS_UPDATER");
            /// <summary>
            /// 已启用 Windows 更新
            /// </summary>
            public static readonly string _ENABLE_WINDOWS_UPDATER = GetLocalizedString("_ENABLE_WINDOWS_UPDATER");
            /// <summary>
            /// Windows 已激活
            /// </summary>
            public static readonly string _ACTIVE_WINDOWS = GetLocalizedString("_ACTIVE_WINDOWS");
            /// <summary>
            /// 未能激活 Windows
            /// </summary>
            public static readonly string _CANNOT_ACTIVE_WINDOWS = GetLocalizedString("_CANNOT_ACTIVE_WINDOWS");
            /// <summary>
            /// 成功写入注册表
            /// </summary>
            public static readonly string _SUCESS_WRITE_REGISTRY = GetLocalizedString("_SUCESS_WRITE_REGISTRY");
            /// <summary>
            /// 未能写入注册表
            /// </summary>
            public static readonly string _WRITE_REGISTRY_FAILED = GetLocalizedString("_WRITE_REGISTRY_FAILED");
            /// <summary>
            /// 获取 Aria2c 参数
            /// </summary>
            public static readonly string _GET_ARIA2C_ARGS = GetLocalizedString("_GET_ARIA2C_ARGS");
            /// <summary>
            /// 获取 Aria2c 路径
            /// </summary>
            public static readonly string _GET_ARIA2C_PATH = GetLocalizedString("_GET_ARIA2C_PATH");
            /// <summary>
            /// 获取 Aria2c 退出代码
            /// </summary>
            public static readonly string _GET_ARIA2C_EXITCODE = GetLocalizedString("_GET_ARIA2C_EXITCODE");
            /// <summary>
            /// 已启用 Aria2c 日志输出
            /// </summary>
            public static readonly string _ENABLE_ARIA2C_LOG_OUTPUT = GetLocalizedString("_ENABLE_ARIA2C_LOG_OUTPUT");
            /// <summary>
            /// 已禁用 Aria2c 日志输出
            /// </summary>
            public static readonly string _DISABLE_ARIA2C_LOG_OUTPUT = GetLocalizedString("_DISABLE_ARIA2C_LOG_OUTPUT");
            /// <summary>
            /// 正在下载文件
            /// </summary>
            public static readonly string _DOWNLOADING_FILE = GetLocalizedString("_DOWNLOADING_FILE");
            /// <summary>
            /// 下载完成
            /// </summary>
            public static readonly string _DOWNLOADING_COMPLETE = GetLocalizedString("_DOWNLOADING_COMPLETE");
            /// <summary>
            /// 下载失败
            /// </summary>
            public static readonly string _DOWNLOADING_FAILED = GetLocalizedString("_DOWNLOADING_FAILED");
            /// <summary>
            /// 获取 64 位下载链接
            /// </summary>
            public static readonly string _GET_64_LINK = GetLocalizedString("_GET_64_LINK");
            /// <summary>
            /// 获取 32 位下载链接
            /// </summary>
            public static readonly string _GET_32_LINK = GetLocalizedString("_GET_32_LINK");
            /// <summary>
            /// 获取 ResourceManager 名称
            /// </summary>
            public static readonly string _GET_RM_NAME = GetLocalizedString("_GET_RM_NAME");
            /// <summary>
            /// 获取 ResourceManager 对象
            /// </summary>
            public static readonly string _GET_RM_OBJ = GetLocalizedString("_GET_RM_OBJ");
            /// <summary>
            /// 创建新的 ResourceManager 实例
            /// </summary>
            public static readonly string _NEW_RM = GetLocalizedString("_NEW_RM");
            /// <summary>
            /// 获取 HTML 页面文件
            /// </summary>
            public static readonly string _GET_HTML = GetLocalizedString("_GET_HTML");
            /// <summary>
            /// 32 位
            /// </summary>
            public static readonly string _32 = GetLocalizedString("_32");
            /// <summary>
            /// 64 位
            /// </summary>
            public static readonly string _64 = GetLocalizedString("_64");
            /// <summary>
            /// 获取文件
            /// </summary>
            public static readonly string _GET_FILE = GetLocalizedString("_GET_FILE");
            /// <summary>
            /// 等待下载
            /// </summary>
            public static readonly string _WAIT_DOWNLOADING = GetLocalizedString("_WAIT_DOWNLOADING");
            /// <summary>
            /// 重试下载
            /// </summary>
            public static readonly string _RETRY_DOWNLOAD = GetLocalizedString("_RETRY_DOWNLOAD");
            /// <summary>
            /// 错误代码
            /// </summary>
            public static readonly string _ERROR_CODE = GetLocalizedString("_ERROR_CODE");
            /// <summary>
            /// 登录失败: 用户名未知或密码错误
            /// </summary>
            public static readonly string _LOGIN_ERROR_USER_OR_PASSWORD = GetLocalizedString("_LOGIN_ERROR_USER_OR_PASSWORD");
            /// <summary>
            /// 请验证您的身份
            /// </summary>
            public static readonly string _LOGIN_VERIFY = GetLocalizedString("_LOGIN_VERIFY");
            /// <summary>
            /// 用户取消了操作
            /// </summary>
            public static readonly string _CANCEL_OP = GetLocalizedString("_CANCEL_OP");
            /// <summary>
            /// 未知错误
            /// </summary>
            public static readonly string _UNKNOW_ERROR = GetLocalizedString("_UNKNOW_ERROR");
            /// <summary>
            /// 接受到响应
            /// </summary>
            public static readonly string _GET_RESPONSE = GetLocalizedString("_GET_RESPONSE");
            /// <summary>
            /// 回答
            /// </summary>
            public static readonly string _ANSWER = GetLocalizedString("_ANSWER");
            /// <summary>
            /// 发送请求
            /// </summary>
            public static readonly string _SEND_REQUEST = GetLocalizedString("_SEND_REQUEST");
            /// <summary>
            /// 验证失败
            /// </summary>
            public static readonly string _LOGIN_VERIFY_ERROR = GetLocalizedString("_LOGIN_VERIFY_ERROR");
            /// <summary>
            /// 请输入您的凭据
            /// </summary>
            public static readonly string _ENTER_CREDENTIALS = GetLocalizedString("_ENTER_CREDENTIALS");
            /// <summary>
            /// 验证成功
            /// </summary>
            public static readonly string _SUCCESS_VERIFY = GetLocalizedString("_SUCCESS_VERITY");
            /// <summary>
            /// 最新版本
            /// </summary>
            public static readonly string _LATEST_VERSION = GetLocalizedString("_LATEST_VERSION");
            /// <summary>
            /// 不支持的平台
            /// </summary>
            public static readonly string _UNSUPPORT_PLATFORM = GetLocalizedString("_UNSUPPORT_PLATFORM");
            /// <summary>
            /// 解析 JSON 失败
            /// </summary>
            public static readonly string _JSON_PARSING_FAILED = GetLocalizedString("_JSON_PARSING_FAILED");
            /// <summary>
            /// 新版本可用
            /// </summary>
            public static readonly string _NEW_VERSION_AVAILABLE = GetLocalizedString("_NEW_VERSION_AVAILABLE");
            /// <summary>
            /// 当前版本为最新版本
            /// </summary>
            public static readonly string _NON_NEW_VER = GetLocalizedString("_NON_NEW_VER");
            /// <summary>
            /// 当前版本
            /// </summary>
            public static readonly string _CURRENT_VER = GetLocalizedString("_CURRENT_VER");
            /// <summary>
            /// 添加新行
            /// </summary>
            public static readonly string _ADD_NEW_LINE = GetLocalizedString("_ADD_NEW_LINE");
            /// <summary>
            /// 更新行值
            /// </summary>
            public static readonly string _UPDATE_LINE = GetLocalizedString("_UPDATE_LINE");
            /// <summary>
            /// 读取文件
            /// </summary>
            public static readonly string _READ_FILE = GetLocalizedString("_READ_FILE");
            /// <summary>
            /// 写入文件
            /// </summary>
            public static readonly string _WRITE_FILE = GetLocalizedString("_WRITE_FILE");
            /// <summary>
            /// Windows 更新 已禁用
            /// </summary>
            public static readonly string _WINDOWS_UPDATER_DISABLED = GetLocalizedString("_WINDOWS_UPDATER_DISABLED");
            /// <summary>
            /// Windows 更新 已启用
            /// </summary>
            public static readonly string _WINDOWS_UPDATER_ENABLED = GetLocalizedString("_WINDOWS_UPDATER_ENABLED");
            /// <summary>
            /// 读取注册表失败
            /// </summary>
            public static readonly string _READ_REGISTRY_FAILED = GetLocalizedString("_READ_REGISTRY_FAILED");
            #endregion
            #region 错误代码
            /// <summary>
            /// 不符合 17 位 SteamID64 格式
            /// </summary>
            public static readonly string Not_Allow_17_SteamID64 = "Not_Allow_17_SteamID64 (6003)";
            /// <summary>
            /// 未找到 Steam 账户
            /// </summary>
            public static readonly string _Steam_Not_Found_Account = "_Steam_Not_Found_Account (6006)";
            /// <summary>
            /// 查询 Steam 账户信息时发生未知异常
            /// </summary>
            public static readonly string _Steam_Unknow_Exception = "_Steam_Unknow_Exception (6007)";
            /// <summary>
            /// 处理 Json 时发生未知异常
            /// </summary>
            public static readonly string _Json_Unknow_Exception = "_Json_Unknow_Exception (6001)";
            /// <summary>
            /// 解析 SteamType 对象时发生错误 或 无法解析 SteamID64
            /// </summary>
            public static readonly string _Json_Parse_SteamID64 = "_Json_Parse_SteamID64 (6002)";
            /// <summary>
            /// 指定的 Json 数据在反序列化过程中出现未知异常
            /// </summary>
            public static readonly string _Json_DeObject_Unknow_Exception = "_Json_DeObject_Unknow_Exception (6201)";
            /// <summary>
            /// 指定的字符串为 <see cref="string.Empty"/> 或 <see langword="null"/>
            /// </summary>
            public static readonly string _String_NullOrEmpty = "_String_NullOrEmpty (1002)";
            /// <summary>
            /// 无效的字符串输入, 通常由API返回响应时回复错误
            /// </summary>
            public static readonly string Invaid_String_Input = "Invaid_String_Input (1001)";
            /// <summary>
            /// 处理 正则表达式 时发生未知异常
            /// </summary>
            public static readonly string _Regex_Match_Unknow_Exception = "_Regex_Match_Unknow_Exception (4002)";
            /// <summary>
            /// 指定的 正则表达式 <see cref="Regex.Match(string)"/> 未匹配出结果而导致输出字符串为 <see cref="string.Empty"/> 或 <see langword="null"/>
            /// </summary>
            public static readonly string _Regex_Match_Not_Found_Any = "_Regex_Match_Not_Found_Any (4001)";
            /// <summary>
            /// 使用 <see cref="HttpClient.GetAsync(string)"/> 发送请求时出现错误
            /// </summary>
            public static readonly string _HttpClient_Request_Failed = "_HttpClient_Request_Failed (1301)";
            /// <summary>
            /// 检测到非法/不安全的请求, 服务器访问已拒绝
            /// </summary>
            public static readonly string _HttpClient_Request_UnsafeOrIllegal_Denied = "_HttpClient_Request_UnsafeOrIllegal_Denied (1302)";
            /// <summary>
            /// 请求的天气名称不存在或未找到
            /// </summary>
            public static readonly string _Weather_City_Not_Found = "_Weather_City_Not_Found (1201)";
            /// <summary>
            /// 查询天气时发生未知异常
            /// </summary>
            public static readonly string _Weather_Unknow_Exception = "_Weather_Unknow_Exception (1202)";
            #endregion
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
                WriteLog(LogLevel.Info, $"{_NEW_RM}");
                // 从资源中获取WindowsToast文件的字节数据
                byte[] ToastZipData = (byte[])rm.GetObject("WindowsToast");
                WriteLog(LogLevel.Info, $"{_GET_RM_OBJ}: WindowsToast Module");
                if (ToastZipData != null)
                {
                    // 检查并创建目录
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        WriteLog(LogLevel.Info, $"{_CREATE_DIRECTORY}");
                    }
                    WriteLog(LogLevel.Info, $"{_GET_OUTPUT_DIRECTORY}: {path}");
                    // 保存文件路径
                    string outputFilePath = Path.Combine(path, "WindowsToast.zip");
                    WriteLog(LogLevel.Info, $"{_GET_OUTPUT_NAME}: {path}");
                    // 写入文件，确保保存为二进制数据
                    WriteLog(LogLevel.Info, $"{_FILE_WRITING}");
                    System.IO.File.WriteAllBytes(outputFilePath, ToastZipData);
                    WriteLog(LogLevel.Info, $"WindowsToast {_FILE_EXIST_PATH} {outputFilePath}");
                }
                else
                {
                    WriteLog(LogLevel.Error, $"{_RES_FILE_NOT_FIND}");
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
                    WriteLog(LogLevel.Error, $"{(path == string.Empty ? "指定的目标路径" : "原始压缩文件路径")} 为null或空值, 将使用默认路径\".\\bin\"");
                    path = $"{Directory.GetCurrentDirectory()}\\bin";
                    if (!Directory.Exists($"{Directory.GetCurrentDirectory()}\\bin"))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                //检查参数是否合法
                if (string.IsNullOrWhiteSpace(path) || Path.GetFileName(path) == string.Empty)
                {
                    WriteLog(LogLevel.Error, $"{path}值为null或空字符串");
                    MessageBox.Show($"{path}值为null或空字符串", "错误的路径! - Rox", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }
                else
                {
                    WriteLog(LogLevel.Info, $"{_GET_DIRECTORY}: {OriginalFile}");
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
                        WriteLog(LogLevel.Info, $"WindowsToast {_DOWNLOADING_COMPLETE}");
                        WriteLog(LogLevel.Info, $"{_FILE_EXIST_PATH} {path}");
                    }
                    else
                    {
                        WriteLog(LogLevel.Error, $"WindowsToast {_DOWNLOADING_FAILED}");
                        WriteLog(LogLevel.Error, $"{zip.ExitCode}");
                    }
                }
                else
                {
                    WriteLog(LogLevel.Error, $"WindowsToast {_RES_FILE_NOT_FIND}");
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
                    WriteLog(LogLevel.Error, "WindowsToast 模块未找到，正在提取模块。");
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
                        WriteLog(LogLevel.Info, "Toast 通知发送成功。");
                    }
                    else
                    {
                        WriteLog(LogLevel.Error, $"Toast 通知发送失败，退出代码: {process.ExitCode}");
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(LogLevel.Error, $"发送 Toast 通知失败: {ex.Message}");
                }
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
                if (System.IO.File.Exists(Path.Combine(ExtraedFolder, "node-v22.15.1-win-x86", "node.exe")))
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
                if (System.IO.File.Exists(Path.Combine(ExtraedFolder, "node-v22.15.1-win-x86", "node.exe")))
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
        public class Network_I
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
                    //if (NetworkInterface.GetIsNetworkAvailable())
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
                        return reply.Status == IPStatus.Success;
                    }
                }
                catch
                {
                    WriteLog(LogLevel.Warning, $"{_NOTAVAILABLE_NETWORK}");
                    return false;
                }
            }

        }
        /// <summary>
        /// Windows 目录操作类
        /// </summary>
        public class Directory_I
        {
            #region COM 接口定义 (修正版)
            /// <summary>
            /// 文件复制操作接口
            /// </summary>
            [ComImport]
            [Guid("947aab5f-0a5c-4c13-b4d6-4bf7836fc9f8")] // 正确的IFileOperation IID
            [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IFileOperation
            {
                /// <summary>
                /// 注册文件操作事件监听器
                /// </summary>
                /// <param name="pfops"> 文件操作事件监听器接口</param>
                /// <param name="pdwCookie"> 事件监听器的唯一标识符</param>
                /// <returns> 返回事件监听器的唯一标识符</returns>
                uint Advise(IntPtr pfops, out uint pdwCookie);
                /// <summary>
                /// 取消注册文件操作事件监听器
                /// </summary>
                /// <param name="dwCookie"> 事件监听器的唯一标识符</param>
                void Unadvise(uint dwCookie);
                /// <summary>
                /// 设置操作标志
                /// </summary>
                /// <param name="dwOperationFlags"> 操作标志</param>
                void SetOperationFlags(FileOperationFlags dwOperationFlags);
                /// <summary>
                /// 设置进度消息
                /// </summary>
                /// <param name="pszMessage"> 进度消息</param>
                void SetProgressMessage([MarshalAs(UnmanagedType.LPWStr)] string pszMessage);
                /// <summary>
                /// 设置进度对话框
                /// </summary>
                /// <param name="popd"> 进度对话框对象</param>
                void SetProgressDialog([MarshalAs(UnmanagedType.Interface)] object popd);
                /// <summary>
                /// 设置属性数组
                /// </summary>
                /// <param name="pproparray">属性数组对象</param>
                void SetProperties([MarshalAs(UnmanagedType.Interface)] object pproparray);
                /// <summary>
                /// 设置所有者窗口
                /// </summary>
                /// <param name="hwndParent"> 所有者窗口的句柄</param>
                void SetOwnerWindow(uint hwndParent);
                /// <summary>
                /// 应用属性到单个项目
                /// </summary>
                /// <param name="psiItem"> 要应用属性的ShellItem</param>
                void ApplyPropertiesToItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem);
                /// <summary>
                /// 应用属性到多个项目
                /// </summary>
                /// <param name="punkItems"> 要应用属性的项目集合</param>
                void ApplyPropertiesToItems([MarshalAs(UnmanagedType.Interface)] object punkItems);
                /// <summary>
                /// 重命名单个项目
                /// </summary>
                /// <param name="psiItem"> 要重命名的ShellItem</param>
                /// <param name="pszNewName"> 新名称</param>
                /// <param name="pfopsItem"> 操作标志</param>
                void RenameItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem,
                               [MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
                               IntPtr pfopsItem);
                /// <summary>
                /// 重命名多个项目
                /// </summary>
                /// <param name="pUnkItems"> 要重命名的项目集合</param>
                /// <param name="pszNewName"> 新名称</param>
                void RenameItems([MarshalAs(UnmanagedType.Interface)] object pUnkItems,
                                [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
                /// <summary>
                /// 移动单个项目到指定目录
                /// </summary>
                /// <param name="psiItem"> 要移动的ShellItem</param>
                /// <param name="psiDestinationFolder"> 目标目录的ShellItem</param>
                /// <param name="pszNewName"> 新名称</param>
                /// <param name="pfopsItem"> 操作标志</param>
                void MoveItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem,
                             [MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder,
                             [MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
                             IntPtr pfopsItem);
                /// <summary>
                /// 移动多个项目到指定目录
                /// </summary>
                /// <param name="punkItems"> 要移动的项目集合</param>
                /// <param name="psiDestinationFolder"> 目标目录的ShellItem</param>
                void MoveItems([MarshalAs(UnmanagedType.Interface)] object punkItems,
                              [MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder);
                /// <summary>
                /// 复制单个项目到指定目录
                /// </summary>
                /// <param name="psiItem"> 要复制的ShellItem</param>
                /// <param name="psiDestinationFolder"> 目标目录的ShellItem</param>
                /// <param name="pszNewName"> 新名称</param>
                /// <param name="pfopsItem"> 操作标志</param>
                void CopyItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem,
                             [MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder,
                             [MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
                             IntPtr pfopsItem);
                /// <summary>
                /// 复制多个项目到指定目录
                /// </summary>
                /// <param name="punkItems"> 要复制的项目集合</param>
                /// <param name="psiDestinationFolder"> 目标目录的ShellItem</param>
                void CopyItems([MarshalAs(UnmanagedType.Interface)] object punkItems,
                              [MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder);
                /// <summary>
                /// 删除单个项目
                /// </summary>
                /// <param name="psiItem"> 要删除的ShellItem</param>
                /// <param name="pfopsItem"> 操作标志</param>
                void DeleteItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem,
                               IntPtr pfopsItem);
                /// <summary>
                /// 删除多个项目
                /// </summary>
                /// <param name="punkItems"> 要删除的项目集合</param>
                void DeleteItems([MarshalAs(UnmanagedType.Interface)] object punkItems);
                /// <summary>
                /// 创建新项目
                /// </summary>
                /// <param name="psiDestinationFolder"> 目标目录的ShellItem</param>
                /// <param name="dwFileAttributes"> 文件属性</param>
                /// <param name="pszName"> 新项目的名称</param>
                /// <param name="pszTemplateName"> 模板名称</param>
                /// <param name="pfopsItem"> 操作标志</param>
                /// <returns></returns>
                uint NewItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder,
                            uint dwFileAttributes,
                            [MarshalAs(UnmanagedType.LPWStr)] string pszName,
                            [MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName,
                            IntPtr pfopsItem);
                /// <summary>
                /// 执行所有已排队的文件操作
                /// </summary>
                void PerformOperations();
                /// <summary>
                /// 检查是否有任何操作被中止
                /// </summary>
                /// <returns> 如果有任何操作被中止，则返回 <see langword="true"/>，否则返回 <see langword="false"/></returns>
                [return: MarshalAs(UnmanagedType.Bool)]
                bool GetAnyOperationsAborted();
            }
            /// <summary>
            /// 文件操作类的 COM 实现
            /// </summary>
            [ComImport]
            [Guid("3ad05575-8857-4850-9277-11b85bdb8e09")] // IFileOperation 的 CLSID
            [ClassInterface(ClassInterfaceType.None)]
            public class FileOperation { }
            /// <summary>
            /// 文件操作标志枚举
            /// </summary>
            [Flags]
            public enum FileOperationFlags : uint
            {
                /// <summary>
                /// 多目标文件操作标志
                /// </summary>
                FOF_MULTIDESTFILES = 0x0001,
                /// <summary>
                /// 确认鼠标操作标志
                /// </summary>
                FOF_CONFIRMMOUSE = 0x0002,
                /// <summary>
                /// 静默操作标志，不显示任何对话框或消息框
                /// </summary>
                FOF_SILENT = 0x0004,
                /// <summary>
                /// 重命名冲突时重命名文件
                /// </summary>
                FOF_RENAMEONCOLLISION = 0x0008,
                /// <summary>
                /// 不显示确认对话框
                /// </summary>
                FOF_NOCONFIRMATION = 0x0010,
                /// <summary>
                /// 希望获取映射句柄
                /// </summary>
                FOF_WANTMAPPINGHANDLE = 0x0020,
                /// <summary>
                /// 允许撤销操作
                /// </summary>
                FOF_ALLOWUNDO = 0x0040,
                /// <summary>
                /// 仅操作文件，不操作目录
                /// </summary>
                FOF_FILESONLY = 0x0080,
                /// <summary>
                /// 显示简单进度条，不显示详细进度信息
                /// </summary>
                FOF_SIMPLEPROGRESS = 0x0100,
                /// <summary>
                /// 不确认创建目录
                /// </summary>
                FOF_NOCONFIRMMKDIR = 0x0200,
                /// <summary>
                /// 不显示错误消息框
                /// </summary>
                FOF_NOERRORUI = 0x0400,
                /// <summary>
                /// 不复制安全属性
                /// </summary>
                FOF_NOCOPYSECURITYATTRIBS = 0x0800,
                /// <summary>
                /// 不递归操作子目录
                /// </summary>
                FOF_NORECURSION = 0x1000,
                /// <summary>
                /// 不处理连接的元素（例如符号链接或快捷方式）
                /// </summary>
                FOF_NO_CONNECTED_ELEMENTS = 0x2000,
                /// <summary>
                /// 希望在删除文件时显示警告对话框
                /// </summary>
                FOF_WANTNUKEWARNING = 0x4000,
                /// <summary>
                /// 不递归处理重解析点（例如符号链接或挂载点）
                /// </summary>
                FOF_NORECURSEREPARSE = 0x8000
            }
            /// <summary>
            /// ShellItem 接口定义
            /// </summary>
            [ComImport]
            [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
            [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IShellItem
            {
                /// <summary>
                /// 绑定到处理程序
                /// </summary>
                /// <param name="pbc"> 绑定上下文</param>
                /// <param name="bhid"> 处理程序标识符</param>
                /// <param name="riid"> 请求的接口标识符</param>
                /// <param name="ppv"> 返回的接口指针</param>
                void BindToHandler(IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid bhid,
                                  [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppv);
                /// <summary>
                /// 获取父项
                /// </summary>
                /// <param name="ppsi"> 父项的ShellItem</param>
                void GetParent(out IShellItem ppsi);
                /// <summary>
                /// 获取显示名称
                /// </summary>
                /// <param name="sigdnName"> 显示名称的类型</param>
                /// <param name="ppszName"> 显示名称字符串</param>
                void GetDisplayName(SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
                /// <summary>
                /// 获取属性
                /// </summary>
                /// <param name="sfgaoMask"> 属性标志掩码</param>
                /// <param name="psfgaoAttribs"> 属性标志掩码</param>
                void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);
                /// <summary>
                /// 获取图标索引
                /// </summary>
                /// <param name="psi"> ShellItem</param>
                /// <param name="hint"> 提示</param>
                /// <param name="piOrder"> 图标索引</param>
                void Compare(IShellItem psi, uint hint, out int piOrder);
            }
            /// <summary>
            /// ShellItem 显示名称类型枚举
            /// </summary>
            public enum SIGDN : uint
            {
                /// <summary>
                /// 正常显示名称
                /// </summary>
                NORMALDISPLAY = 0,
                /// <summary>
                /// 全路径显示名称
                /// </summary>
                PARENTRELATIVEPARSING = 0x80018001,
                /// <summary>
                /// 父相对解析显示名称
                /// </summary>
                PARENTRELATIVEFORADDRESSBAR = 0x8001c001,
                /// <summary>
                /// 父相对地址栏显示名称
                /// </summary>
                DESKTOPABSOLUTEPARSING = 0x80028000,
                /// <summary>
                /// 桌面绝对解析显示名称
                /// </summary>
                PARENTRELATIVEEDITING = 0x80031001,
                /// <summary>
                /// 父相对编辑显示名称
                /// </summary>
                DESKTOPABSOLUTEEDITING = 0x8004c000,
                /// <summary>
                /// 桌面绝对编辑显示名称
                /// </summary>
                FILESYSPATH = 0x80058000,
                /// <summary>
                /// 文件系统路径显示名称
                /// </summary>
                URL = 0x80068000
            }
            /// <summary>
            /// 创建 ShellItem 的方法
            /// </summary>
            /// <param name="pszPath"> 要解析的路径</param>
            /// <param name="pbc"> 绑定上下文</param>
            /// <param name="riid"> 请求的接口标识符</param>
            /// <param name="ppv"> 返回的接口指针</param>
            /// <returns> HRESULT</returns>
            [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern int SHCreateItemFromParsingName(
                [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
                IntPtr pbc,
                [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                out IShellItem ppv);
            #endregion

            /// <summary>
            /// 复制目录方法（保持原始调用方式不变）
            /// </summary>
            public static bool CopyDirectory(string sourceDirectory, string destinationDirectory, IWin32Window ownerWindow = null,
                FileOperationFlags flags = FileOperationFlags.FOF_NOCONFIRMMKDIR | FileOperationFlags.FOF_SIMPLEPROGRESS)
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    MessageBox.Show("源目录不存在: " + sourceDirectory, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    WriteLog(LogLevel.Error, $"CopyDirectory failed: Source directory does not exist: {sourceDirectory}");
                    return false;
                }

                try
                {
                    // 创建文件操作对象（正确方式）
                    var fileOperation = (IFileOperation)new FileOperation();

                    // 设置操作标志
                    fileOperation.SetOperationFlags(flags);

                    // 设置所有者窗口
                    if (ownerWindow != null)
                    {
                        fileOperation.SetOwnerWindow((uint)ownerWindow.Handle);
                    }

                    // 创建源目录的ShellItem
                    IShellItem sourceItem;
                    int hr = SHCreateItemFromParsingName(sourceDirectory, IntPtr.Zero, typeof(IShellItem).GUID, out sourceItem);
                    if (hr != 0) Marshal.ThrowExceptionForHR(hr);

                    // 创建目标父目录的ShellItem
                    string destParent = Path.GetDirectoryName(destinationDirectory);
                    string newFolderName = Path.GetFileName(destinationDirectory);

                    IShellItem destParentItem;
                    hr = SHCreateItemFromParsingName(destParent, IntPtr.Zero, typeof(IShellItem).GUID, out destParentItem);
                    if (hr != 0)
                    {
                        Marshal.ReleaseComObject(sourceItem);
                        Marshal.ThrowExceptionForHR(hr);
                    }

                    // 执行复制操作
                    fileOperation.CopyItem(sourceItem, destParentItem, newFolderName, IntPtr.Zero);
                    fileOperation.PerformOperations();

                    bool aborted = fileOperation.GetAnyOperationsAborted();

                    // 释放资源
                    Marshal.ReleaseComObject(sourceItem);
                    Marshal.ReleaseComObject(destParentItem);
                    Marshal.ReleaseComObject(fileOperation);

                    return !aborted;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"复制目录时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    WriteLog(LogLevel.Error, $"CopyDirectory failed: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
