using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;

namespace Rox.Runtimes
{
    /// <summary>
    /// 高性能网络 Ping 操作类
    /// </summary>
    public class Network_I
    {
        // 默认 Ping 超时时间（毫秒），平衡速度与准确性
        private const int DefaultPingTimeout = 500;

        /// <summary>
        /// 异步 Ping 单个目标（IP/网站域名）
        /// </summary>
        /// <param name="target">目标：IP地址（如 8.8.8.8）或网站域名（如 www.baidu.com）</param>
        /// <param name="timeout">超时时间（毫秒），默认 500ms</param>
        /// <returns>Ping 结果（包含是否成功、延迟、目标地址等）</returns>
        public async Task<PingResult> PingAsync(string target, int timeout = DefaultPingTimeout)
        {
            var result = new PingResult { Target = target };

            try
            {
                // 步骤1：解析目标（如果是域名，先解析为 IP，便于后续排查）
                var targetIp = await ResolveDomainToIpAsync(target);
                result.ResolvedIp = targetIp;

                // 步骤2：异步 Ping 目标
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync(targetIp, timeout);

                    // 步骤3：解析 Ping 结果
                    result.IsSuccess = reply.Status == IPStatus.Success;
                    result.DelayMs = result.IsSuccess ? reply.RoundtripTime : (long?)null;
                    result.Status = reply.Status.ToString();
                }
            }
            catch (PingException ex)
            {
                result.IsSuccess = false;
                result.Status = "PingException";
                WriteLog.Warning(_Exception_With_xKind($"PingAsync({target})", ex));
            }
            catch (WebException ex)
            {
                result.IsSuccess = false;
                result.Status = "DomainResolveFailed";
                WriteLog.Warning(_Exception_With_xKind($"DomainResolve({target})", ex));
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Status = "UnknownError";
                WriteLog.Warning(_Exception_With_xKind($"PingAsync({target})", ex));
            }

            return result;
        }

        /// <summary>
        /// 批量并行 Ping 多个目标（高性能）
        /// </summary>
        /// <param name="targets">目标列表（IP/域名）</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <returns>批量 Ping 结果</returns>
        public async Task<List<PingResult>> BatchPingAsync(IEnumerable<string> targets,
            int timeout = DefaultPingTimeout)
        {
            IEnumerable<string> enumerable = targets as string[] ?? targets.ToArray();
            return !enumerable.ToList().Any()
                ? new List<PingResult>()
                :
                // 并行执行多个 Ping 任务（不限制并发数，若需控制可加 SemaphoreSlim）
                (await Task.WhenAll(enumerable.ToList().Select(target => PingAsync(target, timeout)).ToList()))
                .ToList();
        }

        /// <summary>
        /// 同步 Ping 目标（兼容旧逻辑，建议优先用异步版）
        /// </summary>
        /// <param name="target">IP/域名</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>是否 Ping 成功</returns>
        public bool Ping(string target, int timeout = DefaultPingTimeout) =>
            PingAsync(target, timeout).GetAwaiter().GetResult().IsSuccess;

        /// <summary>
        /// 获取 Ping 延迟（同步版，返回毫秒数，失败返回 null）
        /// </summary>
        /// <param name="target">IP/域名</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>延迟毫秒数 / null</returns>
        public long? GetPingDelay(string target, int timeout = DefaultPingTimeout) =>
            PingAsync(target, timeout).GetAwaiter().GetResult().DelayMs;

        /// <summary>
        /// 异步解析域名到 IP 地址（优先取 IPv4）
        /// </summary>
        /// <param name="domain">域名（如 www.baidu.com）</param>
        /// <returns>解析后的 IP 地址</returns>
        private async Task<string> ResolveDomainToIpAsync(string domain) =>
            // 若已是 IP 地址，直接返回
            IPAddress.TryParse(domain, out _)
                ? domain
                // 异步解析域名（优先 IPv4）
                : (await Dns.GetHostAddressesAsync(domain))?.FirstOrDefault(ip =>
                    ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) == null
                    ? throw new WebException("未解析到 IPv4 地址")
                    : (await Dns.GetHostAddressesAsync(domain))
                    .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    ?.ToString();

        /// <summary>
        /// Ping 结果结构化模型
        /// </summary>
        public class PingResult
        {
            /// <summary>原始目标（IP/域名）</summary>
            public string Target { get; set; }

            /// <summary>解析后的 IP 地址（若为域名）</summary>
            public string ResolvedIp { get; set; }

            /// <summary>是否 Ping 成功</summary>
            public bool IsSuccess { get; set; }

            /// <summary>Ping 延迟（毫秒），失败则为 null</summary>
            public long? DelayMs { get; set; }

            /// <summary>状态描述（Success / 异常类型 / 错误信息）</summary>
            public string Status { get; set; }
        }

        // 保留原有 IsNetworkAvailable 方法（兼容旧逻辑）
        /// <summary>
        /// 检测网络是否可用
        /// </summary>
        /// <returns><see langword="true"/> / <see langword="false"/></returns>
        public static bool IsNetworkAvailable()
        {
            try
            {
                using (var ping = new Ping())
                    return ping.Send("8.8.8.8", 2000)?.Status == IPStatus.Success;
            }
            catch
            {
                WriteLog.Warning(_NOTAVAILABLE_NETWORK);
                return false;
            }
        }
    }
}