using Rox.Runtimes;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static Rox.Entertainment.Steam;
using static Rox.Entertainment.Steam.SteamID;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
using static Rox.Text.Json;
namespace Rox.Entertainment
{
    /// <summary>
    /// Steam用户信息查询, 但是新版本
    /// </summary>
    public class SteamUserData_v1
    {
        /// <summary>
        /// 新版请求Steam Web API Json的方法
        /// </summary>
        /// <param name="SteamID">其中一种的 <see cref="SteamIDType"/></param>
        /// <returns><see cref="SteamType"/> 对象</returns>
        public static async Task<SteamType> GetDataJson_v1(string SteamID) => await GetDataJson_v1(SteamID, false);
        /// <summary>
        /// 新版请求Steam Web API Json的方法
        /// </summary>
        /// <param name="SteamID"> 其中一种的 <see cref="SteamIDType"/></param>
        /// <param name="IsMessageBox">是否弹窗提示</param>
        /// <returns><see cref="SteamType"/> 对象</returns>
        public static async Task<SteamType> GetDataJson_v1(string SteamID, bool IsMessageBox)
        {
            if (string.IsNullOrEmpty(SteamID))
            {
                WriteLog.Error(LogKind.System, $"{_value_Not_Is_NullOrEmpty("SteamID")}, 错误代码: {_String_NullOrEmpty}");
                MessageBox_I.Error($"{_value_Not_Is_NullOrEmpty("SteamID")}, 错误代码: {_String_NullOrEmpty}", "Error");
                return null;
            }

            // 2025/8/4 支持完整的个人资料链接, 自定义URL名称, 好友代码
            var httpClient = new HttpClient();
            bool httpSteam = false;
            bool ID64Steam = false;
            bool ID3Steam = false;
            bool FriendCodeSteam = false;
            bool CustomSteam = false;

            switch (Identifier(SteamID))
            {
                case SteamIDType.SteamID:
                    WriteLog.Info(LogKind.System, "输入的SteamID为: SteamID");
                    httpSteam = true;
                    break;
                case SteamIDType.SteamID3:
                    WriteLog.Info(LogKind.System, "输入的SteamID为: SteamID3");
                    ID3Steam = true;
                    break;
                case SteamIDType.SteamID32:
                    WriteLog.Info(LogKind.System, "输入的SteamID为: 好友代码");
                    FriendCodeSteam = true;
                    break;
                case SteamIDType.SteamID64:
                    WriteLog.Info(LogKind.System, "输入的SteamID为: SteamID64");
                    ID64Steam = true;
                    break;
                default:
                    CustomSteam = true; //自定义ID
                    break;
            }

            if (httpSteam)
            {
                WriteLog.Info(LogKind.Regex, $"正在解析个人主页链接: {SteamID}");
                switch (SteamUserData.ExtractSteamID(SteamID))
                {
                    case null:
                        WriteLog.Error(LogKind.Json, $"无法解析SteamID64, 错误代码: {_Json_Parse_SteamID64}");
                        MessageBox_I.Error($"无法解析SteamID64, 错误代码: {_Json_Parse_SteamID64}", _ERROR);
                        return null;
                    default:
                        if (SteamUserData.ExtractSteamID(SteamID) == _Regex_Match_Unknow_Exception)
                        {
                            WriteLog.Error(LogKind.Regex, $"{_Exception_With_xKind("Regex")}, 返回的错误代码: {_Regex_Match_Unknow_Exception} ");
                            if (IsMessageBox)
                                MessageBox_I.Error($"{_Exception_With_xKind("Regex")}, 返回的错误代码: {_Regex_Match_Unknow_Exception}", _ERROR);
                            return null;
                        }
                        else if (SteamUserData.ExtractSteamID(SteamID) == _Regex_Match_Not_Found_Any)
                        {
                            WriteLog.Error(LogKind.Regex, $"未匹配到任何 正则表达式 , 返回的错误代码: {_Regex_Match_Not_Found_Any}");
                            if (IsMessageBox)
                                MessageBox_I.Error($"未匹配到任何 正则表达式 , 返回的错误代码: {_Regex_Match_Not_Found_Any}", _ERROR);
                            return null;
                        }
                        else
                            return await SendQueryMessage("steamid",SteamUserData.ExtractSteamID(SteamID), new HttpClient(), IsMessageBox); //解析SteamID64
                }
            }

            if (ID64Steam)
            {
                WriteLog.Info(LogKind.Regex, $"正在解析SteamID64: {SteamID}");
                return await SendQueryMessage("steamid",SteamID, httpClient, IsMessageBox); //解析SteamID64
            }
            if (FriendCodeSteam)
            {
                WriteLog.Info(LogKind.Regex, $"正在解析好友代码: {SteamID}");
                return await SendQueryMessage("steamid",$"[U:1:{SteamID}]", httpClient, IsMessageBox); //解析好友代码
            }
            if (ID3Steam)//解析SteamID3
            {
                WriteLog.Info(LogKind.Regex, $"正在解析SteamID3: {SteamID}");
                return await SendQueryMessage("id3",SteamID, httpClient, IsMessageBox); //解析SteamID3
            }
            if (CustomSteam)//解析自定义ID
            {
                WriteLog.Info(LogKind.Regex, $"正在解析自定义ID: {SteamID}");
                return await SendQueryMessage("steamid",SteamID, httpClient, IsMessageBox); //解析自定义ID
            }
            WriteLog.Error(_input_value_Not_Is_xType(SteamID, "SteamIDType"));
            if (IsMessageBox)
                MessageBox_I.Error(_input_value_Not_Is_xType(SteamID, "SteamIDType"), _ERROR);
            return null;//返回空值
        }
        /// <summary>
        /// 向api发送请求获取 <see cref="Text.Json"/> 文本
        /// </summary>
        /// <param name="Type">指定为steamid, id, ,id3 </param>
        /// <param name="SteamID64">SteamID64</param>
        /// <param name="httpClient"><see cref="HttpClient"/> 实例</param>
        /// <param name="IsMessageBox">是否启用消息窗体输出</param>
        /// <returns><see cref="SteamType"/> 格式的 <see cref="Text.Json"/> 文本</returns>
        private static async Task<SteamType> SendQueryMessage(string Type,string SteamID64, HttpClient httpClient, bool IsMessageBox)
        {
            try
            {
                var requestUrl = $"https://uapis.cn/api/v1/game/steam/summary?{Type}={SteamID64}";

                WriteLog.Info(LogKind.Network, $"{_SEND_REQUEST}: {requestUrl}");
                // 发送GET请求并获取响应
                var response = await httpClient.GetAsync(requestUrl);
                // 检查响应是否成功
                if (!response.IsSuccessStatusCode)
                {
                    WriteLog.Error(LogKind.Network, $"请求失败: {response.StatusCode}, {_HttpClient_Request_Failed}");
                    if (IsMessageBox)
                        MessageBox_I.Error($"请求失败: {response.StatusCode}, {_HttpClient_Request_Failed}", _ERROR);
                    return null;
                }
                // 读取响应内容
                var responseData = await response.Content.ReadAsStringAsync();
                WriteLog.Info(LogKind.Json, "获取原始 Json 内容");

                // 压缩 JSON 字符串
                WriteLog.Info(LogKind.Json, "压缩 Json");
                string compressedJson = CompressJson(responseData);
                WriteLog.Info(LogKind.Json, "反序列化 Json");
                var SteamType = DeserializeObject<SteamType>(compressedJson);
                switch ((int)response.StatusCode)
                {
                    case 404: // 未找到账户 或 完全私密个人资料
                        WriteLog.Error(LogKind.Network, $"API返回响应: Steam账户不存在或完全私密了个人资料, 错误代码: {_Steam_Not_Found_Account}");
                        if (IsMessageBox)
                            MessageBox_I.Error($"Steam账户不存在或完全私密了个人资料, 错误代码: {_Steam_Not_Found_Account}", _ERROR);
                        return null;
                    case 400: // 错误的请求
                        WriteLog.Error(LogKind.Network, $"API返回响应: 无效的输入, 错误代码: {Invaid_String_Input}");
                        if (IsMessageBox)
                            MessageBox_I.Error($"无效的输入, 错误代码: {Invaid_String_Input}", _ERROR);
                        return null;
                    case 200:
                        WriteLog.Info(LogKind.Network, $"API返回响应: Json解析成功");
                        break;
                    case 502: //服务器网关错误
                        WriteLog.Error(LogKind.Network, $"API返回响应: 上游服务错误, 在向 Steam 的官方 API 请求数据时遇到了问题, 这可能是他们的服务暂时中断，请稍后重试, 错误代码: {_Steam_Service_Error}");
                        if (IsMessageBox)
                            MessageBox_I.Error($"上游服务错误, 在向 Steam 的官方 API 请求数据时遇到了问题, 这可能是他们的服务暂时中断，请稍后重试. 错误代码: {_Steam_Service_Error}", _ERROR);
                        throw new IException.UAPI.Steam.SteamServiceError();
                    case 401: //未经授权
                        WriteLog.Error(LogKind.Network, $"API返回响应: 认证失败。你提供的 Steam Web API Key 无效或已过期，或者你没有提供 Key。请检查你的 Key. 错误代码: {_Steam_Server_UnAuthenticated}");
                        if (IsMessageBox)
                            MessageBox_I.Error($"认证失败。你提供的 Steam Web API Key 无效或已过期，或者你没有提供 Key。请检查你的 Key. 错误代码: {_Steam_Server_UnAuthenticated}", _ERROR);
                        throw new IException.UAPI.Steam.UnAuthenticatedSteamKey();

                    default:
                        WriteLog.Error(LogKind.Network, $"未知异常, 请联系管理员, 错误代码: {_UNKNOW_ERROR}");
                        throw new IException.UAPI.General.UnknowUAPIException();
                }
                // 输出字段值
                WriteLog.Info(LogKind.Network, $"API 返回的代码: {SteamType.code}");
                WriteLog.Info(LogKind.Network, $"SteamID64: {SteamType.steamid}");
                WriteLog.Info(LogKind.Network, $"个人资料可见性: {SteamType.communityvisibilitystate}");
                WriteLog.Info(LogKind.Network, $"Steam ID3: {SteamType.steamID3}");
                WriteLog.Info(LogKind.Network, $"Steam 用户名: {SteamType.personaname}");
                WriteLog.Info(LogKind.Network, $"个人资料主页链接: {SteamType.profileurl}");
                WriteLog.Info(LogKind.Network, $"头像地址: {SteamType.avatarfull}");
                WriteLog.Info(LogKind.Network, $"在线状态: {SteamType.personastate}");
                WriteLog.Info(LogKind.Network, $"真实姓名: {SteamType.realname}");
                WriteLog.Info(LogKind.Network, $"主要社区组ID: {SteamType.primaryclanid}");
                WriteLog.Info(LogKind.Network, $"账户创建时间戳: {SteamType.timecreated}");
                WriteLog.Info(LogKind.Network, $"账户创建时间: {SteamType.timecreated_str}");
                //WriteLog.Info(LogKind.Network, $"Last Logoff: {SteamType.lastlogoff}");
                WriteLog.Info(LogKind.Network, $"账户所属国家或地区: {SteamType.loccountrycode}");
                WriteLog.Info(LogKind.Network, $"好友代码: {SteamType.friendcode}");
                return SteamType;
            }
            catch (Exception ex)
            {
                // 捕获并输出异常
                WriteLog.Error($"获取 Steam 个人信息失败，请检查网络连接或API服务状态: {ex.Message}, 错误代码: {_Steam_Unknow_Exception}\n" +
                    _Exception_With_xKind("SteamUserData_v1.SendQueryMessage", ex));
                if (IsMessageBox)
                    MessageBox_I.Error($"获取 Steam 个人信息失败，请检查网络连接或API服务状态: {ex.Message}, 错误代码: {_Steam_Unknow_Exception}", _ERROR);
                return null;
            }
        }

        /// <summary>
        /// Steam用户信息
        /// </summary>
        public class SteamType
        {
            /// <summary>
            /// 返回值
            /// </summary>
            public string code { get; set; }
            /// <summary>
            /// SteamID64
            /// </summary>
            public long steamid { get; set; }
            /// <summary>
            /// Steam社区状态, 1 为可见 3为隐藏
            /// </summary>
            public int communityvisibilitystate { get; set; }
            /// <summary>
            /// 如果为 1 , 代表用户已经填写了个人资料
            /// </summary>
            public int profilestate { get; set; }
            /// <summary>
            /// Steam用户名
            /// </summary>
            public string personaname { get; set; }
            /// <summary>
            /// Steam个人主页
            /// </summary>
            public string profileurl { get; set; }
            /// <summary>
            /// Steam个人主页,替换转义字符
            /// </summary>
            public string profileurl_1 => profileurl_1.Replace("\\/", "/");
            /// <summary>
            /// Steam头像, 32*32图像
            /// </summary>
            public string avatar { get; set; }
            /// <summary>
            /// Steam头像,替换转义字符, 32*32图像
            /// </summary>
            public string avatar_1 => avatar_1.Replace("\\/", "/");
            /// <summary>
            /// Steam头像, 64*64图像
            /// </summary>
            public string avatarmedium { get; set; }
            /// <summary>
            /// Steam头像,替换转义字符, 64*64图像
            /// </summary>
            public string avatarmedium_1 => avatarmedium.Replace("\\/", "/");
            /// <summary>
            /// Steam头像, 184*184图像
            /// </summary>
            public string avatarfull { get; set; }
            /// <summary>
            /// Steam头像,替换转义字符, 184*184图像
            /// </summary>
            public string avatarfull_1 => avatarfull.Replace("\\/", "/");
            /// <summary>
            /// Steam在线状态, 0-离线/隐私, 1-在线, 2-忙碌, 3-离开, 4-打盹, 5-想交易, 6-想玩。
            /// </summary>
            public int personastate { get; set; }
            /// <summary>
            /// Steam真实姓名
            /// </summary>
            public string realname { get; set; }
            /// <summary>
            ///  主要社区组
            /// </summary>
            public string primaryclanid { get; set; }
            /// <summary>
            /// Steam账号创建日期的时间刻
            /// </summary>
            public int timecreated { get; set; }
            /// <summary>
            /// Steam账号创建日期的时间戳
            /// </summary>
            public string timecreated_str { get; set; }
            /// <summary>
            /// Steam账号绑定区域
            /// </summary>
            public string loccountrycode { get; set; }
            /// <summary>
            /// 好友代码 （SteamID32）
            /// </summary>
            public string friendcode => Rox.GameExpansionFeatures.Steam.SteamID.GetFriendCode(steamid.ToString());
            /// <summary>
            /// SteamID3
            /// </summary>
            public string steamID3 => Rox.GameExpansionFeatures.Steam.Converter.SteamID.ToSteamID3(steamid.ToString());
            ///// <summary>
            ///// Steam最后登出日期
            ///// </summary>
            //public string lastlogoff { get; set; }
        }
    }
}
