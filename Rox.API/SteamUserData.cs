using Rox.Runtimes;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Rox.GameExpansionFeatures.Steam.SteamID;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;

namespace Rox
{
    public partial class API
    {
        /// <summary>
        /// Steam用户信息查询
        /// </summary>
        public class SteamUserData
        {
            private static SteamType _lastSteamData;
            /// <summary>
            /// 查询Steam用户信息
            /// </summary>
            /// <param name="SteamID">支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns><see cref="SteamType"/> 格式的 <see cref="Text.Json"/> 文本</returns>
            public static async Task<SteamType> GetDataJson(string SteamID)
            {
                if (string.IsNullOrEmpty(SteamID))
                {
                    WriteLog.Error(LogKind.System, $"{_value_Not_Is_NullOrEmpty("SteamID")}, 错误代码: {_String_NullOrEmpty}");
                    MessageBox_I.Error($"{_value_Not_Is_NullOrEmpty("SteamID")}, 错误代码: {_String_NullOrEmpty}", "Error");
                    return null;
                }
                // 创建HttpClient实例
                var httpClient = new HttpClient();
                bool httpSteam = false;
                bool ID64Steam = false;
                bool ID3Steam = false;
                bool FriendCodeSteam = false;
                bool CustomSteam = false;

                switch (Identifier(SteamID))
                {
                    case SteamIDType.SteamID:
                        ID3Steam = true; //STEAM_x_xxxx
                        break;
                    case SteamIDType.SteamID3:
                        ID3Steam = true; // [U:1:1xxxx]
                        break;
                    case SteamIDType.SteamID32:
                        FriendCodeSteam = true; // 1xxxxxx
                        break;
                    case SteamIDType.SteamID64:
                        ID64Steam = true; // 7656xxxxxxxxxx
                        break;
                    case SteamIDType.Invalid:
                        break;
                }
                if (SteamID.StartsWith("http")) //个人主页链接
                {
                    httpSteam = true;
                }
                else //自定义ID
                {
                    CustomSteam = true;
                }

                if (httpSteam) //解析个人主页
                {
                    WriteLog.Info(LogKind.Regex, $"正在解析个人主页链接: {SteamID}");
                    string SteamID64 = ExtractSteamID(SteamID);

                    switch (SteamID64)
                    {
                        case null:
                            WriteLog.Error(LogKind.Json, $"无法解析SteamID64, 错误代码: {_Json_Parse_SteamID64}");
                            MessageBox_I.Error($"无法解析SteamID64, 错误代码: {_Json_Parse_SteamID64}", _ERROR);
                            return null;
                        default:
                            if (SteamID64 == _Regex_Match_Unknow_Exception)
                            {
                                WriteLog.Error(LogKind.Regex, $"{_Exception_With_xKind("Regex")}, 返回的错误代码: {_Regex_Match_Unknow_Exception} ");
                                return null;
                            }
                            else if (SteamID64 == _Regex_Match_Not_Found_Any)
                            {
                                WriteLog.Error(LogKind.Regex, $"未匹配到任何 正则表达式 , 返回的错误代码: {_Regex_Match_Not_Found_Any}");
                                return null;
                            }
                            else
                            {
                                _lastSteamData = await SendQueryMessage(SteamID, new HttpClient());
                                return await SendQueryMessage(SteamID64, httpClient); //解析SteamID64
                            }
                    }
                }
                if (ID64Steam) //解析SteamID64
                {
                    WriteLog.Info(LogKind.Regex, $"正在解析SteamID64: {SteamID}");
                    _lastSteamData = await SendQueryMessage(SteamID, new HttpClient());
                    return await SendQueryMessage(SteamID, httpClient); //解析SteamID64
                }
                if (FriendCodeSteam)//解析好友代码
                {
                    WriteLog.Info(LogKind.Regex, $"正在解析好友代码: {SteamID}");
                    _lastSteamData = await SendQueryMessage(SteamID, new HttpClient());
                    return await SendQueryMessage($"[U:1:{SteamID}]", httpClient); //解析好友代码
                }
                if (ID3Steam)//解析SteamID3
                {
                    WriteLog.Info(LogKind.Regex, $"正在解析SteamID3: {SteamID}");
                    _lastSteamData = await SendQueryMessage(SteamID, new HttpClient());
                    return await SendQueryMessage(SteamID, httpClient); //解析SteamID3
                }
                if (CustomSteam)//解析自定义ID
                {
                    WriteLog.Info(LogKind.Regex, $"正在解析自定义ID: {SteamID}");
                    _lastSteamData = await SendQueryMessage(SteamID, new HttpClient());
                    return await SendQueryMessage(SteamID, httpClient); //解析自定义ID
                }
                WriteLog.Error(_input_value_Not_Is_xType(SteamID, "SteamIDType"));
                return null;//返回空值
            }
            /// <summary>
            /// 使用 <see cref="System.Text.RegularExpressions.Match"/> 正则表达式匹配 SteamID
            /// </summary> 
            /// <param name="url"> Steam 个人主页链接</param>
            /// <returns> <see cref="string"/> 格式的文本</returns>
            internal static string ExtractSteamID(string url)
            {
                try
                {
                    // 正则表达式匹配自定义 ID
                    string customIdPattern = @"\/id\/([^\/]+)";
                    // 正则表达式匹配 17 位数字 ID
                    string numericIdPattern = @"\/profiles\/(\d{17})";

                    // 尝试匹配自定义 ID
                    WriteLog.Info(LogKind.Regex, "正则表达式匹配自定义ID");
                    Match customIdMatch = Regex.Match(url, customIdPattern);
                    if (customIdMatch.Success)
                    {
                        WriteLog.Info(LogKind.Regex, $"返回正则表达式匹配值: {customIdMatch.Groups[1].Value} ");
                        return customIdMatch.Groups[1].Value; // 返回自定义 ID
                    }

                    // 尝试匹配 17 位数字 ID
                    WriteLog.Info(LogKind.Regex, $"正则表达式匹配17位ID");
                    Match numericIdMatch = Regex.Match(url, numericIdPattern);
                    if (numericIdMatch.Success)
                    {
                        if (numericIdMatch.Groups[1].Value.Length != 17)
                        {
                            WriteLog.Error(LogKind.System, $"SteamID64不满足17位唯一标识符!, 错误代码: {Not_Allow_17_SteamID64}");
                            MessageBox_I.Error($"SteamID64不满足17位唯一标识符!, 错误代码: {Not_Allow_17_SteamID64}", _ERROR);
                            return Not_Allow_17_SteamID64;
                        }
                        WriteLog.Info(LogKind.Regex, _Return_xKind_value("正则表达式", numericIdMatch.Groups[1].Value));
                        return numericIdMatch.Groups[1].Value; // 返回 17 位数字 ID
                    }

                    // 如果未匹配到任何 ID，返回 null
                    WriteLog.Error(LogKind.Regex, $"未匹配到任何ID , 错误代码: {_Regex_Match_Not_Found_Any}");
                    return _Regex_Match_Not_Found_Any;
                }
                catch (Exception e)
                {
                    WriteLog.Error(LogKind.Regex, $"{_Exception_With_xKind("正则表达式")}: {e}, 错误代码: {_Regex_Match_Unknow_Exception}");
                    MessageBox_I.Error($"{_Exception_With_xKind("正则表达式")}: {e} , 错误代码: {_Regex_Match_Unknow_Exception}", _ERROR);
                    return _Regex_Match_Unknow_Exception;
                }
            }
            #region GetSimpleData 获取Steam用户信息简易数据
            /// <summary>
            /// 获取Steam用户的社区状态字符串
            /// </summary>
            /// <param name="SteamID"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns> 社区状态字符串</returns>
            public static async Task<string> GetCommmunityState(string SteamID)
            {
                if (_lastSteamData == null)
                {
                    _lastSteamData = await SendQueryMessage(SteamID, new HttpClient());
                }
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastSteamData?.communitystate));
                return _lastSteamData?.communitystate;
            }
            /// <summary>
            /// 获取Steam用户的SteamID字符串
            /// </summary>
            /// <param name="SteamID"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns> SteamID字符串</returns>
            public static async Task<string> GetSteamIDString(string SteamID)
            {
                if (_lastSteamData == null)
                {
                    _lastSteamData = await SendQueryMessage(SteamID, new HttpClient());
                }
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastSteamData?.steamID));
                return _lastSteamData?.steamID;
            }
            /// <summary>
            /// 获取Steam用户的SteamID3字符串
            /// </summary>
            /// <param name="SteamID"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns> SteamID3字符串</returns>
            public static async Task<string> GetSteamID3String(string SteamID)
            {
                if (_lastSteamData == null)
                {
                    _lastSteamData = await SendQueryMessage(SteamID, new HttpClient());
                }
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastSteamData?.steamID3));
                return _lastSteamData?.steamID3;
            }
            /// <summary>
            /// 获取Steam用户的用户名字符串
            /// </summary>
            /// <param name="SteamID"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns> 用户名字符串</returns>
            public static async Task<string> GetUsernameString(string SteamID)
            {
                if (_lastSteamData == null)
                {
                    _lastSteamData = await SendQueryMessage(SteamID, new HttpClient());
                }
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastSteamData?.username));
                return _lastSteamData?.username;
            }
            /// <summary>
            /// 获取Steam用户的ID64字符串
            /// </summary>
            /// <param name="SteamId"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns> ID64字符串</returns>
            public static async Task<string> GetSteamID64String(string SteamId)
            {
                if (_lastSteamData == null)
                {
                    _lastSteamData = await SendQueryMessage(SteamId, new HttpClient());
                }
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastSteamData?.steamID64));
                return _lastSteamData?.steamID64;
            }
            /// <summary>
            /// 获取Steam用户的个人主页链接字符串
            /// </summary>
            /// <param name="SteamId"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns> 个人主页链接字符串</returns>
            public static async Task<string> GetProfileUrlString(string SteamId)
            {
                if (_lastSteamData == null)
                {
                    _lastSteamData = await SendQueryMessage(SteamId, new HttpClient());
                }
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastSteamData?.profileurl_1));
                return _lastSteamData?.profileurl_1;
            }
            /// <summary>
            /// 获取Steam用户的头像字符串
            /// </summary>
            /// <param name="SteamId"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns> 头像字符串</returns>
            public static async Task<string> GetAvatarString(string SteamId)
            {
                if (_lastSteamData == null)
                {
                    _lastSteamData = await SendQueryMessage(SteamId, new HttpClient());
                }
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastSteamData?.avatar_1));
                return _lastSteamData?.avatar_1;
            }
            /// <summary>
            /// 获取Steam用户的账号创建日期字符串
            /// </summary>
            /// <param name="SteamId"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns> 账号创建日期字符串</returns>
            public static async Task<string> GetAccountCreationDateString(string SteamId)
            {
                if (_lastSteamData == null)
                {
                    _lastSteamData = await SendQueryMessage(SteamId, new HttpClient());
                }
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastSteamData?.accountcreationdate));
                return _lastSteamData?.accountcreationdate;
            }
            /// <summary>
            /// 获取Steam用户的最后登出时间字符串
            /// </summary>
            /// <param name="SteamId"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns> 最后登出时间字符串</returns>
            public static async Task<string> GetLastLogoffString(string SteamId)
            {
                if (_lastSteamData == null)
                {
                    _lastSteamData = await SendQueryMessage(SteamId, new HttpClient());
                }
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastSteamData?.lastlogoff));
                return _lastSteamData?.lastlogoff;
            }
            /// <summary>
            /// 获取Steam用户的账号绑定区域字符串
            /// </summary>
            /// <param name="SteamId"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns> 账号绑定区域字符串</returns>
            public static async Task<string> GetLocationString(string SteamId)
            {
                if (_lastSteamData == null)
                {
                    _lastSteamData = await SendQueryMessage(SteamId, new HttpClient());
                }
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastSteamData?.location));
                return _lastSteamData?.location;
            }
            /// <summary>
            /// 获取Steam用户的在线状态字符串
            /// </summary>
            /// <param name="SteamId"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns> Steam在线状态字符串</returns>
            public static async Task<string> GetOnlineStatusString(string SteamId)
            {
                if (_lastSteamData == null)
                {
                    _lastSteamData = await SendQueryMessage(SteamId, new HttpClient());
                }
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastSteamData?.onlinestatus));
                return _lastSteamData?.onlinestatus;
            }
            /// <summary>
            /// 获取Steam用户的好友代码
            /// </summary>
            /// <param name="SteamId"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns> 好友代码字符串</returns>
            public static async Task<string> GetFriendCodeString(string SteamId)
            {
                if (_lastSteamData == null)
                {
                    _lastSteamData = await SendQueryMessage(SteamId, new HttpClient());
                }
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastSteamData?.friendcode));
                return _lastSteamData?.friendcode;
            }
            /// <summary>
            /// 获取Steam用户的真实姓名  
            /// </summary>
            /// <param name="SteamId"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns> 真实姓名字符串</returns>
            public static async Task<string> GetRealNameString(string SteamId)
            {
                if (_lastSteamData == null)
                {
                    _lastSteamData = await SendQueryMessage(SteamId, new HttpClient());
                }
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastSteamData?.realname));
                return _lastSteamData?.realname;
            }
            #endregion
            /// <summary>
            /// 向api发送请求获取 <see cref="Text.Json"/> 文本
            /// </summary>
            /// <param name="SteamID"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <param name="httpClient"> HttpClient实例</param>
            /// <returns> <see cref="SteamType"/> 格式的 <see cref="Text.Json"/> 文本</returns>
            private static async Task<SteamType> SendQueryMessage(string SteamID, HttpClient httpClient)
            {
                try
                {
                    // 构建GET请求的URL，将用户ID作为查询参数
                    var requestUrl = $"https://uapis.cn/api/steamuserinfo?input={SteamID}";

                    WriteLog.Info(LogKind.Network, $"{_SEND_REQUEST}: {requestUrl}");
                    // 发送GET请求并获取响应
                    var response = await httpClient.GetAsync(requestUrl);

                    // 检查响应是否成功
                    if (!response.IsSuccessStatusCode)
                    {
                        LogLibraries.WriteLog.Error($"请求失败: {response.StatusCode}, {_HttpClient_Request_Failed}");
                        return null;
                    }
                    // 读取响应内容
                    var responseData = await response.Content.ReadAsStringAsync();
                    LogLibraries.WriteLog.Info(LogKind.Json, "获取原始 Json 内容");

                    // 压缩 JSON 字符串
                    string compressedJson = CompressJson(responseData);
                    LogLibraries.WriteLog.Info(LogKind.Json, "压缩 Json");

                    // 直接解析 JSON 字符串
                    //Text.Json.JObject jObject = Rox.Text.Json.JObject.Parse(compressedJson);
                    // 反序列化为 SteamType 对象
                    WriteLog.Info(LogKind.Json, $"反序列化 Json");
                    var SteamType = Rox.Text.Json.DeserializeObject<SteamType>(compressedJson);
                    switch (SteamType.code)
                    {
                        case 432:
                            WriteLog.Info(LogKind.Network, $"API返回响应: Steam账户不存在, 错误代码: {_Steam_Not_Found_Account}");
                            MessageBox_I.Error($"Steam账户不存在, 错误代码: {_Steam_Not_Found_Account}", _ERROR);
                            return null;
                        case 443:
                            WriteLog.Info(LogKind.Network, $"API返回响应: 无效的输入, 错误代码: {Invaid_String_Input}");
                            MessageBox_I.Error($"无效的输入, 错误代码: {Invaid_String_Input}", _ERROR);
                            return null;
                        case 200:
                            WriteLog.Info(LogKind.Network, $"API返回响应: Json解析成功");
                            break;
                        default:
                            WriteLog.Info(LogKind.Json, $"Json 反序列化过程中出现未知错误, 错误代码: {_Json_DeObject_Unknow_Exception}");
                            MessageBox_I.Error($"Json 反序列化过程中出现未知错误, 错误代码: {_Json_DeObject_Unknow_Exception}", _ERROR);
                            return null;
                    }
                    // 检查 jObject 是否为空
                    //if (jObject == null)
                    //{
                    //    LogLibraries.WriteLog.Error("解析 Json 对象时出错, 错误代码: _Json_Parse_jObject_Failed (6003)");
                    //    return null;
                    //}
                    //string[] parts = SteamType.steamID3.Split(':'); // 按冒号分割字符串
                    //string friendCode = parts[2].TrimEnd(']'); // 提取第三部分并去掉末尾的 ']'
                    // 输出字段值
                    LogLibraries.WriteLog.Info($"Code: {SteamType.code}");
                    LogLibraries.WriteLog.Info($"Community State: {SteamType.communitystate}");
                    LogLibraries.WriteLog.Info($"Steam ID: {SteamType.steamID}");
                    LogLibraries.WriteLog.Info($"Steam ID3: {SteamType.steamID3}");
                    LogLibraries.WriteLog.Info($"Steam ID64: {SteamType.steamID64}");
                    LogLibraries.WriteLog.Info($"Username: {SteamType.username}");
                    LogLibraries.WriteLog.Info($"Real Name: {SteamType.realname}");
                    LogLibraries.WriteLog.Info($"Profile URL: {SteamType.profileurl_1}");
                    LogLibraries.WriteLog.Info($"Avatar: {SteamType.avatar_1}");
                    LogLibraries.WriteLog.Info($"Account Creation Date: {SteamType.accountcreationdate}");
                    LogLibraries.WriteLog.Info($"Last Logoff: {SteamType.lastlogoff}");
                    LogLibraries.WriteLog.Info($"Location: {SteamType.location}");
                    LogLibraries.WriteLog.Info($"Online Status: {SteamType.onlinestatus}");
                    LogLibraries.WriteLog.Info($"Friend Code: {SteamType.friendcode}");
                    //MessageBox_I.(
                    //    "Steam 个人信息查询\n\n" +
                    //    $"Https 返回值: {SteamType.code}\n" +
                    //    $"SteamID: {SteamType.steamID}\n" +
                    //    $"用户名: {SteamType.username} \n" +
                    //    $"个人主页地址: {SteamType.profileurl.Replace("\\/", "/")}\n" +
                    //    $"账号创建日期: {SteamType.accountcreationdate}\n" +
                    //    $"账号绑定区域: {SteamType.location}\n" +
                    //    $"当前状态: {SteamType.onlinestatus}\n","查询结果",MessageBoxButtons.OK,MessageBoxIcon.Information,MessageBoxDefaultButton.Button1,MessageBoxOptions.DefaultDesktopOnly);
                    return SteamType;
                }
                catch (Exception ex)
                {
                    // 捕获并输出异常
                    LogLibraries.WriteLog.Error($"获取 Steam 个人信息失败，请检查网络连接或API服务状态: {ex.Message}, 错误代码: {_Steam_Unknow_Exception}");
                    return null;
                }
            }
            #region Steam Get Json Process
            // 自定义类型，用于反序列化 JSON 数据
            /// <summary>
            /// Steam用户信息
            /// </summary>
            public class SteamType
            {
                /// <summary>
                /// 返回值
                /// </summary>
                public int code { get; set; }
                /// <summary>
                /// Steam社区状态
                /// </summary>
                public string communitystate { get; set; }
                /// <summary>
                /// SteamID3
                /// </summary>
                public string steamID { get; set; }
                /// <summary>
                /// SteamID3
                /// </summary>
                public string steamID3 { get; set; }
                /// <summary>
                /// SteamID64
                /// </summary>
                public string steamID64 { get; set; }
                /// <summary>
                /// Steam用户名
                /// </summary>
                public string username { get; set; }
                /// <summary>
                /// Steam真实姓名
                /// </summary>
                public string realname { get; set; }
                /// <summary>
                /// Steam个人主页
                /// </summary>
                public string profileurl { get; set; }
                /// <summary>
                /// Steam个人主页,替换转义字符
                /// </summary>
                public string profileurl_1 => profileurl_1.Replace("\\/", "/");
                /// <summary>
                /// Steam头像
                /// </summary>
                public string avatar { get; set; }
                /// <summary>
                /// Steam头像,替换转义字符
                /// </summary>
                public string avatar_1 => avatar_1.Replace("\\/", "/");
                /// <summary>
                /// Steam账号创建日期
                /// </summary>
                public string accountcreationdate { get; set; }
                /// <summary>
                /// Steam最后登出日期
                /// </summary>
                public string lastlogoff { get; set; }
                /// <summary>
                /// Steam账号绑定区域
                /// </summary>
                public string location { get; set; }
                /// <summary>
                /// Steam在线状态
                /// </summary>
                public string onlinestatus { get; set; }
                /// <summary>
                /// Steam好友代码
                /// </summary>
                public string friendcode => steamID3.Split(':')[2].TrimEnd(']');
            }
            #endregion
        }

    }
}
