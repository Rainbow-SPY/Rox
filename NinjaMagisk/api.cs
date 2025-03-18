using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NinjaMagisk
{
    /// <summary>
    /// 提供API查询
    /// </summary>
    public class API
    {
        /// <summary>
        /// 查询Steam用户信息
        /// </summary>
        /// <param name="SteamID">SteamID,通常以7656为开头</param>
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
            bool CustomURLSteam = false;
            if (SteamID.StartsWith("http"))
            {
                httpSteam = true;
            }
            else if (SteamID.StartsWith("7656"))
            {
                ID64Steam = true;
            }
            else if (SteamID.Length == 10)
            {
                ID3Steam = true;
            }
            else
            {
                CustomURLSteam = true;
            }

            if (httpSteam) //解析个人主页
            {
                // 正则表达式匹配 17 位数字
                Match match = Regex.Match(SteamID, @"\/profiles\/(\d{17})");
                if (match.Success)
                {
                    // 返回匹配的 17 位数字
                    string id3 = match.Groups[1].Value;
                    if (id3.Length != 17)
                    {
                        MessageBox.Show("SteamID64不满足17位唯一标识符!\n\n请和开发者联系修复", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                    return await SendQueryMessage(id3, httpClient);
                }
            }
            if (ID64Steam) //解析SteamID64
            {
                if (SteamID.Length != 17)
                {
                    MessageBox.Show("SteamID64不满足17位唯一标识符!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                return await SendQueryMessage(SteamID, httpClient);
            }
            if (ID3Steam)
            {
                if (SteamID.Length != 10)
                {
                    MessageBox.Show("SteamID3不满足10位唯一标识符!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                return await SendQueryMessage(SteamID, httpClient);
            }
            if (CustomURLSteam)
            {
                return await SendQueryMessage(SteamID, httpClient);
            }
            return null;
        }
        /// <summary>
        /// 向api发送请求获取 <see cref="Text.Json"/> 文本
        /// </summary>
        /// <param name="SteamID"></param>
        /// <param name="httpClient"></param>
        /// <returns>返回 <see cref="SteamType"/> 格式的 <see cref="Text.Json"/> 文本</returns>
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
                Text.Json.JObject jObject = Text.Json.JObject.Parse(compressedJson);
                // 反序列化为 SteamType 对象
                var SteamType = Text.Json.DeserializeObject<SteamType>(compressedJson);
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
        /// 
        /// </summary>
        public class SteamType
        {
            public int code { get; set; }
            public string communitystate { get; set; }
            public string steamID { get; set; }
            public string steamID3 { get; set; }
            public string steamID64 { get; set; }
            public string username { get; set; }
            public string realname { get; set; }
            public string profileurl { get; set; }
            public string avatar { get; set; }
            public string accountcreationdate { get; set; }
            public string lastlogoff { get; set; }
            public string location { get; set; }
            public string onlinestatus { get; set; }
            public string friendcode => steamID3.Split(':')[2].TrimEnd(']');
        }
        #endregion
    }
}
