using System;

namespace Rox.Runtimes
{
    public partial class IException : Exception
    {
        public partial class UAPI
        {
            /// <summary>
            /// <see langword="Rox.GameExpansionFeatures.EpicGames"/> 的常见异常
            /// </summary>
            public class EpicGames : Exception
            {
                /// <summary>
                /// Epic Games 服务不可用, 无法访问 Epic Games 相关服务, 可以访问 <see href="https://status.epicgames.com/"/> 查看 Epic Online Services 当前情况
                /// </summary>
                public class EpicGamesServerError : Exception
                {
                    /// <summary>
                    /// Epic Games 服务不可用, 无法访问 Epic Games 相关服务, 可以访问 <see href="https://status.epicgames.com/"/> 查看 Epic Online Services 当前情况
                    /// </summary>
                    public EpicGamesServerError() : base()
                    {
                    }
                    /// <summary>
                    /// Epic Games 服务不可用, 无法访问 Epic Games 相关服务, 可以访问 <see href="https://status.epicgames.com/"/> 查看 Epic Online Services 当前情况
                    /// </summary>
                    /// <param name="message">异常信息</param>
                    public EpicGamesServerError(string message) : base(message)
                    {
                    }
                    /// <summary>
                    /// Epic Games 服务不可用, 无法访问 Epic Games 相关服务, 可以访问 <see href="https://status.epicgames.com/"/> 查看 Epic Online Services 当前情况
                    /// </summary>
                    /// <param name="message">异常消息</param>
                    /// <param name="exception"></param>
                    public EpicGamesServerError(string message, Exception exception) : base(message, exception)
                    {
                    }
                }
            }
        }
    }
}
