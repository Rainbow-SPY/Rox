namespace Rox
{
    namespace Runtimes
    {
        public partial class LogLibraries
        {
            /// <summary>
            /// 消息窗口类库,用于在 UI 上显示消息
            /// </summary>
            public class MessageBox_I
            {
                /// <summary>
                /// 指定为信息类别的消息窗口
                /// </summary>
                /// <param name="message">日志消息</param>
                public static void Info(string message) => MessageBox_Core("Info", message);
                /// <summary>
                /// 指定为错误类别的消息窗口
                /// </summary>
                /// <param name="message"> 日志消息 </param>
                public static void Error(string message) => MessageBox_Core("Info", message);
                /// <summary>
                /// 指定为警告类别的消息窗口
                /// </summary>
                /// <param name="message"> 日志消息 </param>
                public static void Warning(string message) => MessageBox_Core("Info", message);
                /// <summary>
                /// 指定为普通类别的消息窗口
                /// </summary>
                /// <param name="message"> 日志消息 </param>
                public static void Common(string message) => MessageBox_Core("Info", message);
            }
        }
    }
}
