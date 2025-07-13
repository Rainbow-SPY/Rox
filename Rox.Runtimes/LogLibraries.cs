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
    }
}
