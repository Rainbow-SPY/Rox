using System;
using System.Net.NetworkInformation;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    namespace Runtimes
    {
        /// <summary>
        /// 网络相关操作
        /// </summary>
        public class Network_I
        {
            /// <summary>
            /// 检查网络是否可用
            /// </summary>
            /// <returns> 可用返回 <see langword="true"></see> 不可用返回 <see langword="false"></see></returns>
            public static bool IsNetworkAvailable()
            {
                try
                {
                    using (var ping = new Ping())
                    {
                        PingReply reply = ping.Send("8.8.8.8", 2000); // 尝试 Ping Google 的公共 DNS
                        return reply != null && reply.Status == IPStatus.Success;
                    }
                }
                catch
                {
                    // 发生异常视为无网络
                    WriteLog.Warning($"{_NOTAVAILABLE_NETWORK}");
                    return false;
                }
            }
            /// <summary>
            /// 检查网络是否可用
            /// </summary>
            /// <param name="ip"> IP地址</param>
            /// <returns> 可用返回 <see langword="true"></see> 不可用返回 <see langword="false"></see></returns>
            public static bool Ping(string ip)
            {
                try
                {
                    using (var ping = new Ping())
                    {
                        PingReply reply = ping.Send(ip, 120);
                        return reply.Status == IPStatus.Success;
                    }
                }
                catch (Exception e)
                {
                    WriteLog.Warning(_Exception_With_xKind("Ping", e));
                    return false;
                }
            }

            /// <summary>
            /// 获取指定IP的Ping延迟
            /// </summary>
            /// <param name="ip"></param>
            /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
            public string GetPingDelay(string ip)
            {
                try
                {
                    using (var ping = new Ping())
                    {
                        PingReply reply = ping.Send(ip, 120);
                        if (reply.Status == IPStatus.Success)
                            return reply.RoundtripTime.ToString();
                        else
                            return null;
                    }
                }
                catch (Exception e)
                {
                    WriteLog.Warning(_Exception_With_xKind("GetPingDelay", e));
                    return null;
                }
            }
        }
    }
}
