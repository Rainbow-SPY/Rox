using System;

namespace Rox.Runtimes
{
    public partial class IException
    {
        /// <summary>
        /// <see langword="Rox.API.Weather_v1"/> 的常见异常
        /// </summary>
        public class Weather : Exception
        {
            /// <summary>
            /// 天气供应商API不可用, 无法访问天气供应商提供的API
            /// </summary>
            public class WeatherServiceError : Exception
            {
                /// <summary>
                /// 天气供应商API不可用, 无法访问天气供应商提供的API
                /// </summary>
                public WeatherServiceError() : base()
                {
                }
                /// <summary>
                /// 天气供应商API不可用, 无法访问天气供应商提供的API
                /// </summary>
                /// <param name="message">异常消息</param>
                public WeatherServiceError(string message) : base(message)
                {
                }
                /// <summary>
                /// 天气供应商API不可用, 无法访问天气供应商提供的API
                /// </summary>
                /// <param name="message">异常消息</param>
                /// <param name="innerException"></param>
                public WeatherServiceError(string message, Exception innerException) : base(message, innerException)
                {
                }
            }
            /// <summary>
            /// UAPI 服务器内部错误, 在处理天气数据时发生了未知问题
            /// </summary>
            public class WeatherAPIServerError : Exception
            {
                /// <summary>
                /// UAPI 服务器内部错误, 在处理天气数据时发生了未知问题
                /// </summary>
                public WeatherAPIServerError() : base()
                {
                }
                /// <summary>
                /// UAPI 服务器内部错误, 在处理天气数据时发生了未知问题
                /// </summary>
                /// <param name="message">异常消息</param>
                public WeatherAPIServerError(string message) : base(message)
                {
                }
                /// <summary>
                /// UAPI 服务器内部错误, 在处理天气数据时发生了未知问题
                /// </summary>
                /// <param name="message">异常消息</param>
                /// <param name="exception"></param>
                public WeatherAPIServerError(string message, Exception exception) : base(message, exception)
                {
                }
            }
        }
    }
}
