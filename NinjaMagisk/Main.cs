using Microsoft.Win32;
using NinjaMagisk.Interface.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NinjaMagisk.LocalizedString;
using static NinjaMagisk.LogLibraries;
using static NinjaMagisk.Text;
namespace NinjaMagisk
{
    /// <summary>
    /// 根据语言获取资源文件
    /// </summary>
    internal class ResourceHelper
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
                return new ResourceManager("NinjaMagisk.Interface.Properties.Resources", typeof(Resources).Assembly);
            }
            else
            {
                // 如果是其他语言，返回 Resource1.resx 的资源管理器
                return new ResourceManager("NinjaMagisk.Interface.Properties.Resource1", typeof(Resource1).Assembly);
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
    /// 日志类库,在控制台输出日志并记录到文件
    /// </summary>
    public class LogLibraries
    {
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
    /// 本地化字符串,根据语言获取资源文件,并提供本地化字符串
    /// </summary>
    public class LocalizedString
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
        internal static readonly string _ADD_NEW_LINE = GetLocalizedString("_ADD_NEW_LINE");
        internal static readonly string _UPDATE_LINE = GetLocalizedString("_UPDATE_LINE");
        internal static readonly string _READ_FILE = GetLocalizedString("_READ_FILE");
        internal static readonly string _WRITE_FILE = GetLocalizedString("_WRITE_FILE");
        internal static readonly string _WINDOWS_UPDATER_DISABLED = GetLocalizedString("_WINDOWS_UPDATER_DISABLED");
        internal static readonly string _WINDOWS_UPDATER_ENABLED = GetLocalizedString("_WINDOWS_UPDATER_ENABLED");
        internal static readonly string _READ_REGISTRY_FAILED = GetLocalizedString("_READ_REGISTRY_FAILED");
        //internal static string lang = System.Globalization.CultureInfo.InstalledUICulture.Name.ToString();
        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="key">字符串常量</param>
        /// <returns>指定语言文件中的字符串</returns>
        internal static string GetLocalizedString(string key)
        {
            return ResourceHelper.GetString(key, System.Globalization.CultureInfo.InstalledUICulture.Name.ToString());
        }
    }
    /// <summary>
    /// 检测特定安全软件是否在运行
    /// </summary>
    public class Security
    {
        /// <summary>
        /// 检测360安全卫士是否在运行
        /// </summary>
        /// <returns>运行返回 <see langword="true"></see> 未运行返回 <see langword="false"></see></returns>
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
        /// <summary>
        /// 检测火绒安全软件是否在运行
        /// </summary>
        /// <returns> 运行返回 <see langword="true"></see> 未运行返回 <see langword="false"></see></returns>
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
        public class WindowsSecurity
        {
            public static void Enable()
            {
                Windows.WindowsSecurityCenter.Enable();
            }

            public static void Disable()
            {
                Windows.WindowsSecurityCenter.Disable();
            }
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
    /// <summary>
    /// 用于处理注册表操作
    /// </summary>
    public class Registry
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
    /// 一个包含AI聊天功能的类，提供与 APl 交互的能力,
    /// </summary>
    public class AI// AI
    {
        /// <summary>
        /// 用于与 DeepSeek API 进行聊天交互的类，提供了配置 API URL 和发送消息的功能。
        /// </summary>
        public class DeepSeek
        {
            // ==================== API配置区（后期可修改） ====================
            /// <summary>
            /// DeepSeek API的URL
            /// </summary>
            private const string ApiUrl = "https://api.deepseek.com/v1/chat/completions"; // DeepSeek API的URL
            /// <summary>
            /// 表示模型的名称，默认为 <see langword="default"/>
            /// </summary>
            public string Model { get; set; } = "default"; // 默认模型
            /// <summary>
            /// 用于与 DeepSeek API 进行聊天交互
            /// </summary>
            /// <param name="text"> 要发送的消息</param>
            /// <param name="api"> DeepSeek API的密钥</param>
            /// <returns></returns>
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
                        string json = Json.SerializeObject(requestBody);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        WriteLog(LogLevel.Info, $"{_SEND_REQUEST}...{text}");
                        HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                        string responseJson = await response.Content.ReadAsStringAsync();
                        WriteLog(LogLevel.Info, _GET_RESPONSE);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseObject = Text.Json.DeserializeObject<dynamic>(responseJson);
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
        /// <summary>
        /// 用于与 OpenAI API 进行聊天交互的类，提供了配置 API URL 和发送消息的功能。
        /// </summary>
        public class ChatGPT
        {
            // ==================== API配置区（后期可修改） ====================
            /// <summary>
            /// OpenAI API的URL
            /// </summary>
            private const string ApiUrl = "https://api.openai.com/v1/chat/completions"; // OpenAI API的URL
            /// <summary>
            /// 表示模型的名称，默认为 <see langword="gpt-3.5-turbo"/>
            /// </summary>
            public string Model { get; set; } = "gpt-3.5-turbo"; // 默认模型
            /// <summary>
            /// 用于与 OpenAI API 进行聊天交互
            /// </summary>
            /// <param name="text"> 要发送的消息</param>
            /// <param name="apiKey"> OpenAI API的密钥</param>
            /// <returns></returns>
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
                        string json = Text.Json.SerializeObject(requestBody);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        WriteLog(LogLevel.Info, $"{_SEND_REQUEST}...{text}");
                        HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                        string responseJson = await response.Content.ReadAsStringAsync();
                        WriteLog(LogLevel.Info, _GET_RESPONSE);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseObject = Text.Json.DeserializeObject<dynamic>(responseJson);
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
    /// <summary>
    /// 用于处理文件操作
    /// </summary>
    public class File
    {
        /// <summary>
        /// 用于处理文件的加密和解密操作
        /// </summary>
        public class AESEncryption
        {
            /// <summary>
            /// 用于使用指定的 256 位密钥和 128 位初始化向量对给定的明文字符串进行 AES 加密，并返回加密后的字符串。
            /// </summary>
            /// <param name="plainText"> 要加密的明文字符串</param>
            /// <param name="Key"> 256位密钥</param>
            /// <param name="IV"> 128位初始化向量</param>
            /// <returns> 返回加密后的字符串</returns>
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
            /// <summary>
            /// 用于使用指定的 256 位密钥和 128  位初始化向量对给定的密文字符串进行 AES 解密，并返回解密后的字符串。
            /// </summary>
            /// <param name="cipherText"> 要解密的密文字符串</param>
            /// <param name="Key"> 256位密钥</param>
            /// <param name="IV"> 128位初始化向量</param>
            /// <returns> 返回解密后的字符串</returns>
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
        /// <summary>
        /// 定义了文件的厘性选项，包括只读、系统、隐藏和归档。
        /// </summary>

        public enum AtOp
        {
            Readonly,
            System,
            Hidden,
            Archive,
        }
        /// <summary>
        /// 用于设置文件的属性
        /// </summary>
        /// <param name="path"> 文件路径</param>
        /// <param name="Key"> 属性选项</param>
        /// <param name="Switch"> 开关</param>
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
    /// 用于更新软件
    /// </summary>
    public class Update
    {
        /// <summary>
        /// 用于根据加载状态创建或更新一个批处理文件，以便在 NinjaMagisk 自动更新过程中执行特定命令
        /// </summary>
        /// <param name="Loading"> 加载状态</param>
        /// <param name="code"> 命令</param>
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
        /// <summary>
        /// 用于检查更新, 比较两个版本号字符串并返回更新的版本或指示版本相同
        /// </summary>
        /// <param name="version1"> 版本1</param>
        /// <param name="version2"> 版本2</param>
        /// <returns> 返回更新的版本或指示版本相同</returns>
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
        /// <summary>
        /// 发送HTTP请求的HttpClient实例
        /// </summary>
        private static readonly HttpClient _httpClient = new HttpClient();
        /// <summary>
        /// 用于检查更新的平台
        /// </summary>
        public enum Platform
        {
            /// <summary>
            /// Github
            /// </summary>
            Github,
            /// <summary>
            /// Gitee
            /// </summary>
            Gitee,
        }
        /// <summary>
        /// 用于检查更新
        /// </summary>
        /// <param name="CheckUpdateUrl"> 检查更新的URL</param>
        /// <param name="platform"> 平台</param>
        /// <returns> 如果有新版本可用，则返回 <see langword="true"/>，否则返回 <see langword="false"/></returns>
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
                string res = NewerVersions(LocalizedString.Version, strings[1]);
                if (res == "same" || res == LocalizedString.Version)
                {
                    WriteLog(LogLevel.Info, _NON_NEW_VER);
                    return false;
                }
                else
                {
                    WriteLog(LogLevel.Info, $"{_NEW_VERSION_AVAILABLE}: {res} {_CURRENT_VER}: {LocalizedString.Version}");
                    return true;
                }
            }
            else
            {
                WriteLog(LogLevel.Error, _JSON_PARSING_FAILED);
                return false;
            }
        }
        /// <summary>
        /// 用于从指定的URL获取<see cref="Json"/> 数据,并在请求失败时返回<see langword="null"/>
        /// </summary>
        /// <param name="CheckUpdateUrl"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static string GetUpdateJson(string CheckUpdateUrl, Platform platform)
        {
            try
            {
                string jsonResponse = FetchJsonFromUrl(CheckUpdateUrl).Result;
                var (TagName, Name) = ExtractTagAndName(jsonResponse, platform);
                WriteLog(LogLevel.Info, $"{_LATEST_VERSION}: {TagName} - {Name}");
                string strings1 = $"{TagName};{Name}";
                return strings1;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }
        /// <summary>
        /// 用于从指定的URL获取<see cref="Json"/> 数据,并在请求失败时返回<see langword="null"/>
        /// </summary>
        /// <param name="url"> URL</param>
        /// <returns> 返回<see cref="Json"/>字符串</returns>
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
        /// <summary>
        /// 用于提取标签和名称
        /// </summary>
        /// <param name="json">  <see cref="Json"/> 格式的数据</param>
        /// <param name="platform"> 平台</param>
        /// <returns> 返回标签和名称</returns>
        private static (string TagName, string Name) ExtractTagAndName(string json, Platform platform)
        {
            try
            {
                // 解析 JSON
                Json.JObject jsonObject = Json.JObject.Parse(json);

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
    规定 在压缩包内包含了 `update.ini` 和 `filehash.ini` 文件,以及更新文件

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
        /// <summary>
        /// 用于处理应用程序的自我更新，包括创建更新目录和选择更新文件。
        /// </summary>
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
        /// <summary>
        /// 用于提取更新文件
        /// </summary>
        /// <param name="path"> 更新文件的储存路径</param>
        /// <param name="UpdateFile"> 更新文件</param>  
        /// <returns></returns>
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
        /// <summary>
        /// 用于检查压缩包中是否包含指定的文件
        /// </summary>
        /// <param name="UpdateFile"> 更新文件</param>
        /// <returns> 如果包含指定的文件，则返回 <see langword="true"/>，否则返回 <see langword="false"/></returns>
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
}