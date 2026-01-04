using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;
using static Rox.Runtimes.LocalizedString;
namespace Rox
{
    namespace Runtimes
    {
        /// <summary>
        /// 日志类库,在控制台输出日志 <see cref="Console.WriteLine(string)"/> 并记录到文件或重定向 <see cref="MessageBox.Show(string, string, MessageBoxButtons, MessageBoxIcon, MessageBoxDefaultButton)"/> 消息弹窗
        /// </summary>
        public partial class LogLibraries
        {
            /// <summary>
            /// 日志输出到 UI 的委托,用于在 UI 上显示日志
            /// </summary>
            public static Action<string, string> LogToUi { get; set; }
            internal static void MessageBox_Core(string logLevel, string message, string title)
            {
                string[] b = { logLevel, message, title };

                foreach (string a in b)
                {
                    if (string.IsNullOrWhiteSpace(a))
                    {
                        WriteLog.Error(LogKind.Form, _value_Not_Is_NullOrEmpty("a"));
                        return;
                    }
                }
                switch (logLevel)
                {
                    case "Info":
                        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "Error":
                        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "Warning":
                        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case "Common":
                        MessageBox.Show(message, title, MessageBoxButtons.OK);
                        break;
                }
            }
            // 定义日志文件名和路径（当前目录下的 log.ralog 文件）
            /// <summary>
            /// 日志文件名
            /// </summary>
            private static readonly string logFileName = "log.ralog";
            /// <summary>
            /// 日志文件路径
            /// </summary>
            private static readonly string logFilePath = Path.Combine(Application.StartupPath, logFileName);
            internal static void RegisteredExt()
            {
                // 检测注册表 HKCR\.rxcfg / .rxtemp / .ralog 是否存在
                if (Registry.ClassesRoot.OpenSubKey(".rxcfg") == null ||
                    Registry.ClassesRoot.OpenSubKey(".rxtemp") == null ||
                    Registry.ClassesRoot.OpenSubKey(".ralog") == null)
                {
                    WriteLog.Warning(LogKind.Registry, "正在注册Rox相关文件扩展名...");
                    Config.RegisteredFileExt();
                }
            }
            /// <summary>
            /// 根据日志等级和日志类型向文件写入日志,并在控制台输出日志,并记录到文件
            /// </summary>
            /// <param name="logLevel">日志等级</param>
            /// <param name="logKind">日志类型</param>
            /// <param name="message">消息</param>
            internal static void WriteLog_(string logLevel, LogKind logKind, string message) => WriteLog_(logLevel, logKind.ToString(), message);

            internal static void WriteLog_(string logLevel, string logKind, string message)
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
                Console.Write($"[{logLevel}] ");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                if (logKind != null)
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
            internal static void WriteLog_(string logLevel, string message) => WriteLog_(logLevel, null, message);
            /// <summary>
            /// 根据日志等级记录日志到文件
            /// </summary>
            /// <param name="logLevel">日志等级</param>
            /// <param name="message">消息</param>
            public static void LogToFile(string logLevel, string message) => LogToFile(logLevel, null, message);
            /// <summary>
            /// 根据日志等级和日志类型记录日志到文件
            /// </summary>
            /// <param name="logLevel"></param>
            /// <param name="logkind"></param>
            /// <param name="message"></param>
            public static void LogToFile(string logLevel, LogKind logkind, string message) => LogToFile(logLevel, logkind.ToString(), message);
            /// <summary>
            /// 根据日志等级和日志类型记录日志到文件
            /// </summary>
            /// <param name="logLevel"></param>
            /// <param name="logkind"></param>
            /// <param name="message"></param>
            public static void LogToFile(string logLevel, string logkind, string message)
            {
                // 创建日志信息
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {(logkind == null ? "" : $"[{logkind}]: ")}{message}";

                try
                {
                    // 如果日志文件不存在，则创建
                    if (!File.Exists(logFilePath))
                        File.Create(logFilePath).Close();

                    // 以追加方式写入日志内容
                    using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
                    {
                        writer.WriteLine(logMessage);
                        writer.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    WriteLog.Error($"Error writing to log file: {ex.Message}");
                    Console.ResetColor();
                }
            }
            /// <summary>
            /// 根据日志等级和日志类型记录崩溃日志到文件
            /// </summary>
            /// <param name="CrushFilePath"> 崩溃日志文件路径 </param>
            /// <param name="message"> 消息 </param>
            public static void LogToCrushFile(string CrushFilePath, string message)
            {
                // 创建日志信息
                try
                {
                    // 如果日志文件不存在，则创建
                    if (!File.Exists(CrushFilePath))
                    {
                        File.Create(CrushFilePath).Close();
                        WriteLog.Info($"{_GET_FILE} {CrushFilePath}");
                    }
                    File.WriteAllText(CrushFilePath, message);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    WriteLog.Error($"Error writing to log file: {ex.Message}");
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
