using Rox.Runtimes;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rox;
using Rox.Text;

namespace Rox
{
    /// <summary>
    /// 提供API查询
    /// </summary>
    public class API
    {
        /// <summary>
        /// 查询Steam用户信息
        /// </summary>
        /// <param name="SteamID">支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
        /// <returns><see cref="SteamType"/> 格式的 <see cref="Text.Json"/> 文本</returns>
        public static async Task<SteamType> SteamUserData(string SteamID)
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
                return await SendQueryMessage(SteamID, httpClient); //解析SteamID64
            }
            if (FriendCodeSteam)//解析好友代码
            {
                return await SendQueryMessage($"[U:1:{SteamID}]", httpClient); //解析好友代码
            }
            if (ID3Steam)//解析SteamID3
            {
                return await SendQueryMessage(SteamID, httpClient); //解析SteamID3
            }
            if (CustomSteam)//解析自定义ID
            {
                return await SendQueryMessage(SteamID, httpClient); //解析自定义ID
            }
            return null;//返回空值
        }
        /// <summary>
        /// 向api发送请求获取 <see cref="Text.Json"/> 文本
        /// </summary>
        /// <param name="url"> 支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
        /// <returns> <see cref="SteamType"/> 格式的 <see cref="Text.Json"/> 文本</returns>
        static string ExtractSteamID(string url)
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
                LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Profile URL: {SteamType.profileurl.Replace("\\/", "/")}");
                LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"Avatar: {SteamType.avatar.Replace("\\/", "/")}");
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
                LogLibraries.WriteLog(LogLibraries.LogLevel.Error, $"An error occurred: {ex.Message}");
                return null;
            }
        }
        #region Steam Get Json Process
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
            /// Steam头像
            /// </summary>
            public string avatar { get; set; }
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
