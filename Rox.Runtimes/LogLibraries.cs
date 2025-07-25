using System;
using System.IO;
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
            /// <summary>
            /// 日志输出到 UI 的委托,用于在 UI 上显示日志
            /// </summary>
            public static Action<string, string> LogToUi { get; set; }
            /// <summary>
            /// 向控制台输出日志, 并指定日志等级和日志类型
            /// </summary>
            public class WriteLog
            {
                /// <summary>
                /// 指定为信息类别的日志
                /// </summary>
                /// <param name="message"> 日志消息 </param>
                public static void Info(string message) => WriteLog_("Info", message);
                /// <summary>
                /// 指定为错误类别的日志
                /// </summary>
                /// <param name="message"> 日志消息 </param>
                public static void Error(string message) => WriteLog_("Error", message);
                /// <summary>
                /// 指定为警告类别的日志
                /// </summary>
                /// <param name="message"> 日志消息 </param>
                public static void Warning(string message) => WriteLog_("Warning", message);
                /// <summary>
                ///  指定为调试类别的日志
                /// </summary>
                /// <param name="message"></param>
                public static void Debug(string message) => WriteLog_("Debug", message);
                /// <summary>
                /// 指定为信息类别的日志,并指定日志类型
                /// </summary>
                /// <param name="kind"> 日志类型 </param>
                /// <param name="message"> 日志消息 </param>
                public static void Info(LogKind kind, string message) => WriteLog_("Info", kind, message);
                /// <summary>
                /// 指定为错误类别的日志,并指定日志类型
                /// </summary>
                /// <param name="kind"> 日志类型 </param>
                /// <param name="message"> 日志消息 </param>
                public static void Error(LogKind kind, string message) => WriteLog_("Error", kind, message);
                /// <summary>
                ///  指定为警告类别的日志,并指定日志类型
                /// </summary>
                /// <param name="kind"> 日志类型 </param>
                /// <param name="message"> 日志消息 </param>
                public static void Warning(LogKind kind, string message) => WriteLog_("Warning", kind, message);
                /// <summary>
                ///  指定为调试类别的日志,并指定日志类型
                /// </summary>
                /// <param name="kind"></param>
                /// <param name="message"></param>
                public static void Debug(LogKind kind, string message) => WriteLog_("Debug", kind, message);
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
                /// 注册表 <see cref="Microsoft.Win32.Registry"/>
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
                /// <summary>
                /// 下载器
                /// </summary>
                Downloader,
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
            internal static void WriteLog_(string logLevel, LogKind logKind, string message)
            {
                switch (logLevel)
                {
                    case "Info":
                        Console.ForegroundColor = ConsoleColor.Green; // 设置绿色
                        break;
                    case "Error":
                        Console.ForegroundColor = ConsoleColor.Red; // 设置红色
                        break;
                    case "Warning":
                        Console.ForegroundColor = ConsoleColor.Yellow; // 设置黄色
                        break;
                    case "Debug":
                        Console.ForegroundColor = ConsoleColor.Cyan; // 设置青色
                        break;
                }
                // 设置颜色
                Console.ForegroundColor = ConsoleColor.Green;// 设置绿色
                Console.Write($"[{logLevel}] ");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"{logKind}: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write($"{message}\n");
                Console.ResetColor();
                // 打印日志到控制台
                LogToUi?.Invoke(logLevel, message);
                // 记录日志到文件
                LogToFile(logLevel, logKind, message);
            }
            /// <summary>
            /// 根据日志等级向文件写入日志,并在控制台输出日志,并记录到文件
            /// </summary>
            /// <param name="logLevel">日志等级</param>
            /// <param name="message">消息</param>
            internal static void WriteLog_(string logLevel, string message)
            {
                // 设置颜色
                // 设置控制台颜色
                switch (logLevel)
                {
                    case "Info":
                        Console.ForegroundColor = ConsoleColor.Green; // 设置绿色
                        break;
                    case "Error":
                        Console.ForegroundColor = ConsoleColor.Red; // 设置红色
                        break;
                    case "Warning":
                        Console.ForegroundColor = ConsoleColor.Yellow; // 设置黄色
                        break;
                    case "Debug":
#if DEBUG
                        Console.ForegroundColor = ConsoleColor.Cyan; // 设置青色
#elif RELEASE            
                        return; 
#endif
                        break;
                }
                Console.Write($"[{logLevel}]");
                Console.ForegroundColor = ConsoleColor.DarkYellow; // 设置绿色
                Console.Write($": {message}\n");
                Console.ResetColor();

                // 打印日志到控制台
                LogToUi?.Invoke(logLevel, message);
                // 记录日志到文件
                LogToFile(logLevel, message);
            }
            /// <summary>
            /// 根据日志等级记录日志到文件
            /// </summary>
            /// <param name="logLevel">日志等级</param>
            /// <param name="message">消息</param>
            public static void LogToFile(string logLevel, string message)
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
            public static void LogToFile(string logLevel, LogKind logkind, string message)
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
                    WriteLog.Info($"[Error] Error writing to log file: {ex.Message}");
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

                    WriteLog.Info($"{_CLEAR_LOGFILE}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    WriteLog.Error($"{_CANNOT_CLEAR_LOGFILE}: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}
