using Microsoft.Win32;
using System.Net.NetworkInformation;

namespace Rox.GameExpansionFeatures
{
    public partial class Steam
    {
        /// <summary>
        /// 通过注册表 <see cref="Registry"/> 获取Steam的安装路径。
        /// </summary>
        /// <returns> Steam的安装路径，如果未找到则返回null。</returns>
        public static string GetSteamPath()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
            {
                if (key != null)
                {
                    object value = key.GetValue("SteamPath");
                    if (value != null)
                    {
                        return value.ToString();
                    }
                    return null;
                }
                return null;
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
            public static string GeteSteamStoreDelay()
            {
                Ping store = new Ping();
                store.Send("store.steampowered.com", 120);
                return (store.Send("store.steampowered.com").RoundtripTime.ToString()) ?? null;
            }
            /// <summary>
            /// 获取当前Steam社区的网络延迟（Ping值）。
            /// </summary>
            /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
            public static string GetSteamCommunityDelay()
            {
                Ping community = new Ping();
                community.Send("steamcommunity.com", 120);
                return (community.Send("steamcommunity.com").RoundtripTime.ToString()) ?? null;
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
                    Ping xinliu = new Ping();
                    xinliu.Send("dl.steam.clngaa.com", 120);
                    return (xinliu.Send("dl.steam.clngaa.com").RoundtripTime.ToString()) ?? null;
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
                        Ping baishan1 = new Ping();
                        baishan1.Send("st.dl.eccdnx.com", 120);
                        return (baishan1.Send("st.dl.eccdnx.com").RoundtripTime.ToString()) ?? null;
                    }
                    /// <summary>
                    /// 获取Steam白山云下载地址2的网络延迟（Ping值）。
                    /// </summary>
                    /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
                    public static string GetBaiShanCloud_2_Address()
                    {
                        Ping baishan2 = new Ping();
                        baishan2.Send("st.dl.bscstorage.net", 120);
                        return (baishan2.Send("st.dl.bscstorage.net").RoundtripTime.ToString()) ?? null;
                    }
                    /// <summary>
                    /// 获取Steam白山云下载地址3的网络延迟（Ping值）。
                    /// </summary>
                    /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
                    public static string GetBaiShanCloud_3_Address()
                    {
                        Ping baishan3 = new Ping();
                        baishan3.Send("trts.baishancdnx.com", 120);
                        return (baishan3.Send("trts.baishancdnx.com").RoundtripTime.ToString()) ?? null;
                    }
                    /// <summary>
                    /// 获取Steam白山云下载地址4的网络延迟（Ping值）。
                    /// </summary>
                    /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
                    public static string GetBaiShanCloud_4_Address()
                    {
                        Ping baishan4 = new Ping();
                        baishan4.Send("st-bak.viv.wanwang.space", 120);
                        return (baishan4.Send("dl.bscstorage.net").RoundtripTime.ToString()) ?? null;
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
                        Ping alicloud1 = new Ping();
                        alicloud1.Send("lv.queniujq.cn", 120);
                        return (alicloud1.Send("lv.queniujq.cn").RoundtripTime.ToString()) ?? null;
                    }
                    /// <summary>
                    /// 获取Steam阿里云下载地址2的网络延迟（Ping值）。
                    /// </summary>
                    /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
                    public static string GetAliCloud_2_Address()
                    {
                        Ping alicloud2 = new Ping();
                        alicloud2.Send("alibaba.cdn.steampipe.steamcontent.com", 120);
                        return (alicloud2.Send("alibaba.cdn.steampipe.steamcontent.com").RoundtripTime.ToString()) ?? null;
                    }
                    /// <summary>
                    /// 获取Steam阿里云下载地址3的网络延迟（Ping值）。
                    /// </summary>
                    /// <returns> 返回延迟时间，单位为毫秒。如果无法Ping通，返回null。</returns>
                    public static string GetAliCloud_3_Address()
                    {
                        Ping alicloud3 = new Ping();
                        alicloud3.Send("xz.pphimalayanrt.com", 120);
                        return (alicloud3.Send("xz.pphimalayanrt.com").RoundtripTime.ToString()) ?? null;
                    }
                }
            }

        }
    }
}
