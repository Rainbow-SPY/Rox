using Microsoft.Win32;
using Rox.Runtimes;

namespace Rox.Entertainment
{
    public partial class Steam
    {
        /// <summary>
        /// 通过注册表 <see cref="Registry"/> 获取Steam的安装路径。
        /// </summary>
        /// <returns> Steam的安装路径，如果未找到则返回null。</returns>
        public static string GetSteamPath()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
            {
                var value = key?.GetValue("SteamPath");
                return value?.ToString();
            }
        }

        /// <summary>
        /// 获取Steam相关的网络延迟（Ping值）。
        /// </summary>
        public class Delay
        {
            /// <summary>
            /// 获取当前Steam商店的网络延迟（Ping值）。
            /// </summary>
            /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
            public static string GetSteamStoreDelay()
            {
                var resultDelayMs = Network_I.PingAsync("steampowered.com").Result.DelayMs;
                return resultDelayMs != null ? resultDelayMs.ToString() : null;
            }

            /// <summary>
            /// 获取当前Steam社区的网络延迟（Ping值）。
            /// </summary>
            /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
            public static string GetSteamCommunityDelay()
            {
                var _0x1 = Network_I.PingAsync("steamcommunity.com").Result.DelayMs;
                return _0x1 != null ? _0x1.ToString() : null;
            }

            /// <summary>
            /// 获取当前Steam内容服务器的网络延迟（Ping值）。
            /// </summary>
            public class DownloadAddress
            {
                /// <summary>
                /// 获取Steam新流云下载地址的网络延迟（Ping值）。
                /// </summary>
                /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
                public static string GetXinLiuCloudAddress()
                {
                    var _0x1 = Network_I.PingAsync("dl.steam.clngaa.com").Result.DelayMs;
                    return _0x1 != null ? _0x1.ToString() : null;
                }

                /// <summary>
                /// 获取Steam白山云下载地址的网络延迟（Ping值）。
                /// </summary>
                public class BaiShanCloud
                {
                    /// <summary>
                    /// 获取Steam白山云下载地址1的网络延迟（Ping值）。
                    /// </summary>
                    /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
                    public static string GetBaiShanCloud_1_Address()
                    {
                        var _0x1 = Network_I.PingAsync("st.dl.eccdnx.com").Result.DelayMs;
                        return _0x1 != null ? _0x1.ToString() : null;
                    }

                    /// <summary>
                    /// 获取Steam白山云下载地址2的网络延迟（Ping值）。
                    /// </summary>
                    /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
                    public static string GetBaiShanCloud_2_Address()
                    {
                        var _0x1 = Network_I.PingAsync("st.dl.bscstorage.net").Result.DelayMs;
                        return _0x1 != null ? _0x1.ToString() : null;
                    }

                    /// <summary>
                    /// 获取Steam白山云下载地址3的网络延迟（Ping值）。
                    /// </summary>
                    /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
                    public static string GetBaiShanCloud_3_Address()
                    {
                        var _0x1 = Network_I.PingAsync("trts.baishancdnx.com").Result.DelayMs;
                        return _0x1 != null ? _0x1.ToString() : null;
                    }

                    /// <summary>
                    /// 获取Steam白山云下载地址4的网络延迟（Ping值）。
                    /// </summary>
                    /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
                    public static string GetBaiShanCloud_4_Address()
                    {
                        var _0x1 = Network_I.PingAsync("st-bak.viv.wanwang.space").Result.DelayMs;
                        return _0x1 != null ? _0x1.ToString() : null;
                    }
                }

                /// <summary>
                /// 获取Steam阿里云下载地址的网络延迟（Ping值）。
                /// </summary>
                public class AliCloud
                {
                    /// <summary>
                    /// 获取Steam阿里云下载地址1的网络延迟（Ping值）。
                    /// </summary>
                    /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
                    public static string GetAliCloud_1_Address()
                    {
                        var _0x1 = Network_I.PingAsync("lv.queniujq.cn").Result.DelayMs;
                        return _0x1 != null ? _0x1.ToString() : null;
                    }

                    /// <summary>
                    /// 获取Steam阿里云下载地址2的网络延迟（Ping值）。
                    /// </summary>
                    /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
                    public static string GetAliCloud_2_Address()
                    {
                        var _0x1 = Network_I.PingAsync("alibaba.cdn.steampipe.steamcontent.com").Result.DelayMs;
                        return _0x1 != null ? _0x1.ToString() : null;
                    }


                    /// <summary>
                    /// 获取Steam阿里云下载地址3的网络延迟（Ping值）。
                    /// </summary>
                    /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
                    public static string GetAliCloud_3_Address()
                    {
                        var _0x1 = Network_I.PingAsync("xz.pphimalayanrt.com").Result.DelayMs;
                        return _0x1 != null ? _0x1.ToString() : null;
                    }
                }
            }
        }
    }
}