using System;

namespace Rox
{
    namespace Runtimes
    {
        public partial class LogLibraries
        {
            /// <summary>
            /// 向控制台输出日志, 并指定日志等级和日志类型, 此类重定向到 <see cref="Console.WriteLine(string)"/> 方法
            /// </summary>
            public class WriteLog
            {
                /// <summary>
                /// 指定为信息类别的日志, 此方法重定向到 <see cref="Console.WriteLine(string)"/> 方法
                /// </summary>
                /// <param name="message"> 日志消息 </param>
                public static void Info(string message) => WriteLog_("Info", message);
                /// <summary>
                /// 指定为错误类别的日志, 此方法重定向到 <see cref="Console.WriteLine(string)"/> 方法
                /// </summary>
                /// <param name="message"> 日志消息 </param>
                public static void Error(string message) => WriteLog_("Error", message);
                /// <summary>
                /// 指定为警告类别的日志, 此方法重定向到 <see cref="Console.WriteLine(string)"/> 方法
                /// </summary>
                /// <param name="message"> 日志消息 </param>
                public static void Warning(string message) => WriteLog_("Warning", message);
                /// <summary>
                ///  指定为调试类别的日志, 此方法重定向到 <see cref="Console.WriteLine(string)"/> 方法
                /// </summary>
                /// <param name="message"></param>
                public static void Debug(string message) => WriteLog_("Debug", message);
                /// <summary>
                /// 指定为信息类别的日志, 并指定日志类型, 此方法重定向到 <see cref="Console.WriteLine(string)"/> 方法
                /// </summary>
                /// <param name="kind"> 日志类型 </param>
                /// <param name="message"> 日志消息 </param>
                public static void Info(LogKind kind, string message) => WriteLog_("Info", kind, message);
                /// <summary>
                /// 指定为信息类别的日志, 并指定日志类型, 此方法重定向到 <see cref="Console.WriteLine(string)"/> 方法
                /// </summary>
                /// <param name="kind">日志类型</param>
                /// <param name="message">日志消息</param>
                public static void Info(string kind, string message) => WriteLog_("Info", kind, message);
                /// <summary>
                /// 指定为错误类别的日志, 并指定日志类型, 此方法重定向到 <see cref="Console.WriteLine(string)"/> 方法
                /// </summary>
                /// <param name="kind"> 日志类型 </param>
                /// <param name="message"> 日志消息 </param>
                public static void Error(LogKind kind, string message) => WriteLog_("Error", kind, message);
                /// <summary>
                /// 指定为错误类别的日志, 并指定日志类型, 此方法重定向到 <see cref="Console.WriteLine(string)"/> 方法
                /// </summary>
                /// <param name="kind"> 日志类型 </param>
                /// <param name="message"> 日志消息 </param>
                public static void Error(string kind, string message) => WriteLog_("Error", kind, message);
                /// <summary>
                ///  指定为警告类别的日志, 并指定日志类型, 此方法重定向到 <see cref="Console.WriteLine(string)"/> 方法
                /// </summary>
                /// <param name="kind"> 日志类型 </param>
                /// <param name="message"> 日志消息 </param>
                public static void Warning(LogKind kind, string message) => WriteLog_("Warning", kind, message);
                /// <summary>
                ///  指定为警告类别的日志, 并指定日志类型, 此方法重定向到 <see cref="Console.WriteLine(string)"/> 方法
                /// </summary>
                /// <param name="kind"> 日志类型 </param>
                /// <param name="message"> 日志消息 </param>
                public static void Warning(string kind, string message) => WriteLog_("Warning", kind, message);
                /// <summary>
                ///  指定为调试类别的日志, 并指定日志类型, 此方法重定向到 <see cref="Console.WriteLine(string)"/> 方法
                /// </summary>
                /// <param name="kind"></param>
                /// <param name="message"></param>
                public static void Debug(LogKind kind, string message) => WriteLog_("Debug", kind, message);
                /// <summary>
                ///  指定为调试类别的日志, 并指定日志类型, 此方法重定向到 <see cref="Console.WriteLine(string)"/> 方法
                /// </summary>
                /// <param name="kind"></param>
                /// <param name="message"></param>
                public static void Debug(string kind, string message) => WriteLog_("Debug", kind, message);
            }
            public class CustomWriteLog
            {

                public static void Info(string CustomLogKind, string message) => WriteLog_(CustomLogKind, "Info", message);
            }
        }
    }
}
