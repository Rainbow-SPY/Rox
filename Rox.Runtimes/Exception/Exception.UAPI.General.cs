using System;

namespace Rox.Runtimes
{
    public partial class IException
    {
        /// <summary>
        /// 总结来自 <see href="https://uapis.cn"/> 的异常
        /// </summary>
        public partial class UAPI : Exception
        {
            /// <summary>
            /// <see href="https://uapis.cn"/> 的常见异常
            /// </summary>
            public class General : Exception
            {
                /// <summary>
                ///  来自UAPI的未知异常
                /// </summary>
                public class UnknowUAPIException : Exception
                {
                    /// <summary>
                    /// 来自UAPI的未知异常
                    /// </summary>
                    public UnknowUAPIException() : base()
                    {
                    }
                    /// <summary>
                    ///  来自UAPI的未知异常
                    /// </summary>
                    /// <param name="message">错误消息</param>
                    public UnknowUAPIException(string message) : base(message)
                    {
                    }
                    /// <summary>
                    ///  来自UAPI的未知异常
                    /// </summary>
                    /// <param name="message">错误消息</param>
                    /// <param name="innerException"></param>
                    public UnknowUAPIException(string message, Exception innerException) : base(message, innerException)
                    {
                    }
                }
            }
        }
    }
}
