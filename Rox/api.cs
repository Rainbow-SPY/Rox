using Rox.Runtimes;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Rox.Runtimes.LogLibraries;

namespace Rox
{
    /// <summary>
    /// 提供API查询
    /// </summary>
    public class API
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
                    MessageBox.Show("SteamID64为空值", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                // 创建HttpClient实例
                var httpClient = new HttpClient();
                bool httpSteam = false;
                bool ID64Steam = false;
                bool ID3Steam = false;
                bool FriendCodeSteam = false;
                bool CustomSteam = false;
                if (SteamID.StartsWith("http")) //个人主页
                {
                    httpSteam = true;
                }
                else if (SteamID.StartsWith("7656")) //SteamID64
                {
                    ID64Steam = true;
                }
                else if (SteamID.StartsWith("[U:")) //SteamID3
                {
                    ID3Steam = true;
                }
                else if (SteamID.All(char.IsDigit)) //好友代码
                {
                    FriendCodeSteam = true;
                }
                else //自定义ID
                {
                    CustomSteam = true;
                }

                if (httpSteam) //解析个人主页
                {
                    string SteamID64 = ExtractSteamID(SteamID);
                    if (SteamID64 != null)
                    {
                        _lastSteamData = await SendQueryMessage(SteamID, new HttpClient());
                        return await SendQueryMessage(SteamID64, httpClient); //解析SteamID64
                    }
                    else
                    {
                        MessageBox.Show("无法解析SteamID64", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
                if (ID64Steam) //解析SteamID64
                {
                    if (SteamID.Length != 17)
                    {
                        MessageBox.Show("SteamID64不满足17位唯一标识符!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                    _lastSteamData = await SendQueryMessage(SteamID, new HttpClient());
                    return await SendQueryMessage(SteamID, httpClient); //解析SteamID64
                }
                if (FriendCodeSteam)//解析好友代码
                {
                    _lastSteamData = await SendQueryMessage(SteamID, new HttpClient());
                    return await SendQueryMessage($"[U:1:{SteamID}]", httpClient); //解析好友代码
                }
                if (ID3Steam)//解析SteamID3
                {
                    _lastSteamData = await SendQueryMessage(SteamID, new HttpClient());
                    return await SendQueryMessage(SteamID, httpClient); //解析SteamID3
                }
                if (CustomSteam)//解析自定义ID
                {
                    _lastSteamData = await SendQueryMessage(SteamID, new HttpClient());
                    return await SendQueryMessage(SteamID, httpClient); //解析自定义ID
                }
                return null;//返回空值
            }
            /// <summary>
            /// 向api发送请求获取 <see cref="Text.Json"/> 文本
            /// </summary>
            /// <param name="url"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns> <see cref="SteamType"/> 格式的 <see cref="Text.Json"/> 文本</returns>
            private static string ExtractSteamID(string url)
            {
                // 正则表达式匹配自定义 ID
                string customIdPattern = @"\/id\/([^\/]+)";
                // 正则表达式匹配 17 位数字 ID
                string numericIdPattern = @"\/profiles\/(\d{17})";

                // 尝试匹配自定义 ID
                Match customIdMatch = Regex.Match(url, customIdPattern);
                if (customIdMatch.Success)
                {
                    return customIdMatch.Groups[1].Value; // 返回自定义 ID
                }

                // 尝试匹配 17 位数字 ID
                Match numericIdMatch = Regex.Match(url, numericIdPattern);
                if (numericIdMatch.Success)
                {
                    if (numericIdMatch.Groups[1].Value.Length != 17)
                    {
                        MessageBox.Show("SteamID64不满足17位唯一标识符!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                    return numericIdMatch.Groups[1].Value; // 返回 17 位数字 ID
                }

                // 如果未匹配到任何 ID，返回 null
                return null;
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

                    // 发送GET请求并获取响应
                    var response = await httpClient.GetAsync(requestUrl);

                    // 检查响应是否成功
                    if (!response.IsSuccessStatusCode)
                    {
                        LogLibraries.WriteLog(LogLibraries.LogLevel.Error, $"Request failed with status code: {response.StatusCode}");
                        return null;
                    }
                    var t = await SteamUserData.GetDataJson("");
                    string e = t.username;
                    // 读取响应内容
                    var responseData = await response.Content.ReadAsStringAsync();
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, "Raw JSON Response:");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, responseData);

                    // 压缩 JSON 字符串
                    string compressedJson = CompressJson(responseData);
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, "Compressed JSON:");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, compressedJson);

                    // 直接解析 JSON 字符串
                    Text.Json.JObject jObject = Rox.Text.Json.JObject.Parse(compressedJson);
                    // 反序列化为 SteamType 对象
                    var SteamType = Rox.Text.Json.DeserializeObject<SteamType>(compressedJson);
                    switch (SteamType.code)
                    {
                        case 432:
                            MessageBox.Show("Steam账户不存在", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        case 443:
                            MessageBox.Show("无效的输入", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        case 200:
                            break;
                        default:
                            MessageBox.Show("未知错误", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                    }
                    // 检查 jObject 是否为空
                    if (jObject == null)
                    {
                        LogLibraries.WriteLog(LogLibraries.LogLevel.Error, "Failed to parse JSON object.");
                        return null;
                    }
                    //string[] parts = SteamType.steamID3.Split(':'); // 按冒号分割字符串
                    //string friendCode = parts[2].TrimEnd(']'); // 提取第三部分并去掉末尾的 ']'
                    // 输出字段值
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Code: {SteamType.code}");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Community State: {SteamType.communitystate}");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Steam ID: {SteamType.steamID}");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Steam ID3: {SteamType.steamID3}");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Steam ID64: {SteamType.steamID64}");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Username: {SteamType.username}");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Real Name: {SteamType.realname}");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Profile URL: {SteamType.profileurl_1}");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Avatar: {SteamType.avatar_1}");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Account Creation Date: {SteamType.accountcreationdate}");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Last Logoff: {SteamType.lastlogoff}");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Location: {SteamType.location}");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Online Status: {SteamType.onlinestatus}");
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Friend Code: {SteamType.friendcode}");
                    //MessageBox.Show(
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
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Error, $"获取天气信息失败，请检查网络连接或API服务状态: {ex.Message}");
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
        /// <summary>
        /// 天气查询
        /// </summary>
        public class Weather
        {
            /// <summary>
            /// 存储上次获取的天气数据
            /// </summary>
            private static WeatherType _lastWeatherData;
            /// <summary>
            /// 获取天气信息
            /// </summary>
            /// <param name="city">城市名称</param>
            /// <returns>天气信息字符串</returns>
            public static async Task<WeatherType> GetWeatherDataJson(string city)
            {
                try
                {
                    if (string.IsNullOrEmpty(city))
                    {
                        MessageBox.Show("城市名称不能为空", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                    var httpClient = new HttpClient();
                    var requestUrl = $"https://uapis.cn/api/weather?name={city}";
                    var response = await httpClient.GetAsync(requestUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("无法获取天气信息", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                    var responseData = await response.Content.ReadAsStringAsync();
                    string compressedJson = CompressJson(responseData);
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, "Compressed JSON");
                    // 直接解析 JSON 字符串
                    Text.Json.JObject jObject = Rox.Text.Json.JObject.Parse(compressedJson);
                    var weatherType = Rox.Text.Json.DeserializeObject<WeatherType>(compressedJson);
                    switch (weatherType.code) // 修改为通过实例访问 code 属性
                    {
                        case 400:
                            MessageBox.Show("城市名称不能为空", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        case 500:
                            MessageBox.Show("请求的城市不存在,请键入\"广东省、北京市、海淀区\"", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        case 0:
                            MessageBox.Show("检测到非法/不安全的请求!访问已拒绝", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                    }
                    if (jObject == null)
                    {
                        LogLibraries.WriteLog(LogLibraries.LogLevel.Error, "Failed to parse JSON object.");
                        return null;
                    }
                    WriteLog(LogLibraries.LogLevel.Info, $"Code: {weatherType.code}");
                    WriteLog(LogLibraries.LogLevel.Info, $"获取省份名称: {weatherType.province}");
                    WriteLog(LogLibraries.LogLevel.Info, $"获取城市名称: {weatherType.city}");
                    WriteLog(LogLibraries.LogLevel.Info, $"获取温度: {weatherType.temperature_1}");
                    WriteLog(LogLibraries.LogLevel.Info, $"获取天气状况: {weatherType.weather}");
                    WriteLog(LogLibraries.LogLevel.Info, $"获取风向: {weatherType.wind_direction_1}");
                    WriteLog(LogLibraries.LogLevel.Info, $"获取风力等级: {weatherType.wind_power_1}");
                    WriteLog(LogLibraries.LogLevel.Info, $"获取湿度: {weatherType.humidity_1}");
                    WriteLog(LogLibraries.LogLevel.Info, $"数据更新时间: {weatherType.reporttime}");
                    return weatherType;
                }
                catch(Exception ex)
                {
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Error, $"获取天气信息失败，请检查网络连接或API服务状态: {ex.Message}");
                    return null;
                }

            }
            /// <summary>
            /// 获取指定城市的温度信息
            /// </summary>
            /// <param name="city"> 城市名称</param>
            /// <returns> 温度信息字符串</returns>
            public static async Task<string> GetTemperature(string city)
            {
                if (_lastWeatherData == null)
                {
                    _lastWeatherData = await GetWeatherDataJson(city);
                }
                return _lastWeatherData?.temperature_1;
            }
            /// <summary>
            /// 获取指定城市的天气状况信息
            /// </summary>
            /// <param name="city">城市名称</param>
            /// <returns>天气状况信息字符串</returns>
            public static async Task<string> GetWeather(string city)
            {
                if (_lastWeatherData == null)
                {
                    _lastWeatherData = await GetWeatherDataJson(city);
                }
                return _lastWeatherData?.weather;
            }
            /// <summary>
            /// 获取指定城市的风向信息
            /// </summary>
            /// <param name="city"> 城市名称</param>
            /// <returns> 风向信息字符串</returns>
            public static async Task<string> GetWindDirection(string city)
            {
                if (_lastWeatherData == null)
                {
                    _lastWeatherData = await GetWeatherDataJson(city);
                }
                return _lastWeatherData?.wind_direction_1;
            }
            /// <summary>
            /// 获取指定城市的风力等级信息
            /// </summary>
            /// <param name="city"> 城市名称</param>
            /// <returns> 风力等级信息字符串</returns>
            public static async Task<string> GetWindPower(string city)
            {
                if (_lastWeatherData == null)
                {
                    _lastWeatherData = await GetWeatherDataJson(city);
                }
                return _lastWeatherData?.wind_power_1;
            }
            /// <summary>
            /// 获取指定城市的湿度信息
            /// </summary>
            /// <param name="city"> 城市名称</param>
            /// <returns> 湿度信息字符串</returns>
            public static async Task<string> GetHumidity(string city)
            {
                if (_lastWeatherData == null)
                {
                    _lastWeatherData = await GetWeatherDataJson(city);
                }
                return _lastWeatherData?.humidity_1;
            }
            /// <summary>
            /// 获取指定城市的数据更新时间信息属性
            /// </summary>
            public class WeatherType
            {
                /// <summary>
                /// 返回值
                /// </summary>
                public int code { get; set; }
                /// <summary>
                /// 省份名称
                /// </summary>
                public string province { get; set; }
                /// <summary>
                /// 城市名称
                /// </summary>
                public string city { get; set; }
                /// <summary>
                /// 温度,带单位
                /// </summary>
                public string temperature_1 => temperature + "℃"; // 20℃ 30℃
                /// <summary>
                /// 温度
                /// </summary>
                public string temperature { get; set; }
                /// <summary>
                /// 天气状况
                /// </summary>
                public string weather { get; set; }
                /// <summary>
                /// 风向,带单位
                /// </summary>
                public string wind_direction_1 => wind_direction + "风"; // 东南风 西北风
                /// <summary>
                /// 风向
                /// </summary>
                public string wind_direction { get; set; }
                /// <summary>
                /// 风力等级,带单位
                /// </summary>
                public string wind_power_1 => wind_power + "级"; // 1级 2级
                /// <summary>
                /// 风力等级
                /// </summary>
                public string wind_power { get; set; }
                /// <summary>
                /// 湿度,带单位
                /// </summary>
                public string humidity_1 => humidity + "%";
                /// <summary>
                /// 湿度
                /// </summary>
                public string humidity { get; set; }
                /// <summary>
                /// 数据更新时间	
                /// </summary>
                public string reporttime { get; set; }
                /// <summary>
                /// 错误信息
                /// </summary>
                public string msg { get; set; }
            }
        }
        /// <summary>
        /// 压缩 <see cref="Text.Json"/> 字符串
        /// </summary>
        /// <param name="json"> <see cref="Text.Json"/> 字符串</param>
        /// <returns> 压缩后的 <see cref="Text.Json"/> 字符串</returns>
        private static string CompressJson(string json)
        {
            var result = new StringBuilder();
            bool inString = false; // 是否在字符串内
            bool skipWhitespace = false; // 是否跳过空白字符

            for (int i = 0; i < json.Length; i++)
            {
                char currentChar = json[i];

                // 处理引号
                if (currentChar == '"')
                {
                    inString = !inString; // 切换字符串状态
                    result.Append(currentChar);
                    continue;
                }

                // 在字符串内，直接追加字符
                if (inString)
                {
                    result.Append(currentChar);
                    continue;
                }

                // 处理空白字符
                if (char.IsWhiteSpace(currentChar))
                {
                    // 如果当前字符是空格，且需要跳过空白字符，则跳过
                    if (skipWhitespace)
                    {
                        continue;
                    }

                    // 检查是否需要保留空格（如日期时间字段）
                    if (IsDateTimeField(json, i))
                    {
                        result.Append(currentChar);
                        skipWhitespace = true; // 跳过后续空白字符
                        continue;
                    }

                    // 否则，跳过空白字符
                    continue;
                }

                // 追加非空白字符
                result.Append(currentChar);
                skipWhitespace = false; // 重置跳过空白字符的标志
            }

            return result.ToString();
        }

        // 检查当前字符是否属于日期时间字段
        /// <summary>
        /// 检查当前字符是否属于日期时间字段
        /// </summary>
        /// <param name="json"> <see cref="Text.Json"/> 字符串</param>
        /// <param name="index"> 当前字符索引</param>
        /// <returns> 是否在日期时间字段中</returns>
        private static bool IsDateTimeField(string json, int index)
        {
            // 检查当前字符是否在日期时间字段中
            // 例如："accountcreationdate":"2022-10-23 20:23:58"
            string[] dateTimeFields = { "accountcreationdate", "lastlogoff" };

            foreach (var field in dateTimeFields)
            {
                // 检查字段名是否出现在当前位置之前
                if (index >= field.Length && json.Substring(index - field.Length, field.Length) == field)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
