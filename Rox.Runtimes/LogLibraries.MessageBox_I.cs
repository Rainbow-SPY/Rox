using System.Windows.Forms;

namespace Rox
{
    namespace Runtimes
    {
        public partial class LogLibraries
        {
            /// <summary>
            /// 消息窗口类库,用于在 UI 上显示消息, 此类重定向到 <see cref="MessageBox.Show(string ,string,MessageBoxButtons,MessageBoxIcon)"/> 方法
            /// </summary>
            public class MessageBox_I
            {
                /// <summary>
                /// 指定为信息类别的消息窗口, 此方法重定向到 <see cref="MessageBox.Show(string ,string,MessageBoxButtons,MessageBoxIcon)"/> 方法
                /// </summary>
                /// <param name="message">日志消息</param>
                /// <param name="title"> 消息标题 </param>
                public static void Info(string message, string title) => MessageBox_Core("Info", message, title);
                /// <summary>
                /// 指定为错误类别的消息窗口, 此方法重定向到 <see cref="MessageBox.Show(string ,string,MessageBoxButtons,MessageBoxIcon)"/> 方法
                /// </summary>
                /// <param name="message"> 日志消息 </param>
                /// <param name="title"> 消息标题 </param>
                public static void Error(string message, string title) => MessageBox_Core("Info", message, title);
                /// <summary>
                /// 指定为警告类别的消息窗口, 此方法重定向到 <see cref="MessageBox.Show(string ,string,MessageBoxButtons,MessageBoxIcon)"/> 方法
                /// </summary>
                /// <param name="message"> 日志消息 </param>
                /// <param name="title"> 消息标题 </param>
                public static void Warning(string message, string title) => MessageBox_Core("Info", message, title);
                /// <summary>
                /// 指定为普通类别的消息窗口, 此方法重定向到 <see cref="MessageBox.Show(string ,string,MessageBoxButtons,MessageBoxIcon)"/> 方法
                /// </summary>
                /// <param name="message"> 日志消息 </param>
                /// <param name="title"> 消息标题 </param>
                public static void Common(string message, string title) => MessageBox_Core("Info", message, title);
            }
        }
    }
}
