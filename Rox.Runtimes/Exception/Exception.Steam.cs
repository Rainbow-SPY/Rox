using System;

namespace Rox.Runtimes
{
    /// <summary>
    /// 自定义类库内的常见异常
    /// </summary>
    public partial class IException : System.Exception
    {
        /// <summary>
        /// <see langword="Rox.GameExpansionFeatures.Steam"/> 的常见异常
        /// </summary>
        public class Steam : System.Exception
        {
            /// <summary>
            /// 提供的Steam Web API Key 无效或已过期
            /// </summary>
            public class UnAuthenticatedSteamKey : System.Exception
            {
                /// <summary>
                /// 提供的Steam Web API Key 无效或已过期
                /// </summary>
                public UnAuthenticatedSteamKey() : base()
                {
                }
                /// <summary>
                /// 提供的Steam Web API Key 无效或已过期
                /// </summary>
                /// <param name="message">异常消息</param>
                public UnAuthenticatedSteamKey(string message) : base(message)
                {
                }
                /// <summary>
                /// 
                /// </summary>
                /// <param name="message">异常消息</param>
                /// <param name="innerException"></param>
                public UnAuthenticatedSteamKey(string message, Exception innerException) : base(message, innerException)
                {
                }
            }
            /// <summary>
            /// Steam 服务不可用, 无法访问 Steam 相关服务, 可以访问 <see href="https://steamstat.us/"/> 查看 Steam 服务当前情况
            /// </summary>
            public class SteamServiceError : System.Exception
            {
                /// <summary>
                /// Steam 服务暂时中断, 可以访问 <see href="https://steamstat.us/"/> 查看 Steam 服务当前情况
                /// </summary>
                public SteamServiceError() : base()
                {
                }
                /// <summary>
                /// Steam 服务暂时中断, 可以访问 <see href="https://steamstat.us/"/> 查看 Steam 服务当前情况
                /// </summary>
                public SteamServiceError(string message) : base(message)
                {
                }
                /// <summary>
                /// Steam 服务暂时中断, 可以访问 <see href="https://steamstat.us/"/> 查看 Steam 服务当前情况
                /// </summary>
                public SteamServiceError(string message, Exception innerException) : base(message, innerException)
                {
                }
            }

           //public class 
        }
    }
}
