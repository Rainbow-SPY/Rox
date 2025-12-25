using System;
using System.Net.Http;
using System.Threading.Tasks;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
using static Rox.Text.Json;
namespace Rox.GameExpansionFeatures.EpicGames
{
    /// <summary>
    /// 获取当前Epic Games的免费游戏
    /// </summary>
    public partial class GetFreeGames
    {
        private static readonly string _au = "Epic Games";
        /// <summary>
        /// 请求Epic Games 当前免费游戏的方法
        /// </summary>
        /// <returns><see cref="EpicType"/> 对象</returns>
        public static async Task<EpicType> GetDataJson() => await SendQueryMessage(new HttpClient());

        private static async Task<EpicType> SendQueryMessage(HttpClient httpClient)
        {
            try
            {
                var requestUrl = @"https://uapis.cn/api/v1/game/epic-free";
                WriteLog.Info(LogKind.Network, $"{_SEND_REQUEST}: {requestUrl}");
                // 发送GET请求并获取响应
                using (var response = await httpClient.GetAsync(requestUrl))
                {
                    // 检查响应是否成功
                    if (!response.IsSuccessStatusCode)
                    {
                        WriteLog.Error(LogKind.Network, $"请求失败: {response.StatusCode}, {_HttpClient_Request_Failed}");
                        return null;
                    }
                    // 读取响应内容
                    var responseData = await response.Content.ReadAsStringAsync();
                    WriteLog.Info(LogKind.Json, "获取原始 Json 内容");

                    // 压缩 JSON 字符串
                    WriteLog.Info(LogKind.Json, "压缩 Json");
                    string compressedJson = CompressJson(responseData);
                    WriteLog.Info(LogKind.Json, $"反序列化 Json");
                    WriteLog.Info("开始解析Json");
                    var type = Newtonsoft.Json.JsonConvert.DeserializeObject<EpicType>(compressedJson);
                    switch (type.code)
                    {
                        case 200:
                            WriteLog.Info(LogKind.Network, $"API返回响应: Json解析成功");
                            break;
                        case 500:
                            WriteLog.Error(LogKind.Network, $"Epic Games 免费游戏服务暂时不可用，请稍后再试");
                            throw new Rox.Runtimes.IException.EpicGames.EpicGamesServerError("Epic Online Services 免费游戏服务器不可用");
                    }
                    foreach (var game in type.data)
                    {
                        WriteLog.Info(_au, $"游戏唯一ID {game.id}");
                        WriteLog.Info(_au, $"游戏名: {game.title}");
                        WriteLog.Info(_au, $"当前是否免费? {(game.is_free_now ? "Free" : "UnKnow")}");
                        WriteLog.Info(_au, $"免费开始的时间: {game.free_start}");
                        WriteLog.Info(_au, $"免费结束的时间: {game.free_end}");
                        WriteLog.Info(_au, $"游戏封面的URL: {game.cover}");
                        WriteLog.Info(_au, $"免费结束的时间戳: {game.free_end_at}");
                        WriteLog.Info(_au, $"详情页: {game.link}");
                        WriteLog.Info(_au, $"游戏介绍: {game.description}");
                    }
                    return type;
                }
            }
            catch (Exception ex)
            {
                WriteLog.Error(_Exception_With_xKind("SendQueryMessage", ex));
                return null;
            }
        }
    }
}
