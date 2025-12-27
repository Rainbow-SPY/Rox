using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
using static Rox.Text.Json;
namespace Rox
{
    public partial class API
    {
        /// <summary>
        /// 获取QQ用户信息
        /// </summary>
        public class GetQQUserData
        {
            /// <summary>
            /// 获取QQ用户信息
            /// </summary>
            /// <param name="QQ"> QQ号</param>
            /// <returns> <see cref="QQType"/> 对象</returns> 
            public static async Task<QQType> GetQQUserDataJson(string QQ) => await SendMessageRequest(QQ);
            private static async Task<QQType> SendMessageRequest(string QQ)
            {
                if (string.IsNullOrEmpty(QQ))
                {
                    WriteLog.Error("GetQQUserData", $"{_value_Not_Is_NullOrEmpty("QQ")}, 错误代码: {_String_NullOrEmpty}");
                    return null;
                }

                try
                {
                    using (var client = new HttpClient())
                    {
                        var requestUrl = $"https://uapis.cn/api/v1/social/qq/userinfo?qq={QQ}";
                        using (var response = await client.GetAsync(requestUrl))
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                WriteLog.Error(LogKind.Network, $"请求失败: {response.StatusCode}, 错误代码: {_HttpClient_Request_Failed}");
                                MessageBox_I.Error($"请求失败: {response.StatusCode}, 错误代码: {_HttpClient_Request_Failed}", _ERROR);
                                return null;
                            }
                            var responseData = await response.Content.ReadAsStringAsync();
                            WriteLog.Info(LogKind.Json, "压缩 Json");
                            string compressedJson = CompressJson(responseData);
                            WriteLog.Info(LogKind.Json, "反序列化 Json 对象");
                            var QQType = DeserializeObject<QQType>(compressedJson);
                            switch ((int)response.StatusCode)
                            {
                                case 200:
                                    WriteLog.Info(LogKind.Network, $"请求成功");
                                    break;
                                case 400:
                                    WriteLog.Error(LogKind.Network, $"无效的请求, 缺少必要的参数");
                                    break;
                                case 404:
                                    WriteLog.Error(LogKind.Network, $"获取QQ用户信息失败或用户不存在");
                                    break;
                            }
                            if (QQType.qq == "" || QQType.qq_level == string.Empty || string.IsNullOrWhiteSpace(QQType.qq))
                                return null;
                            return QQType;
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog.Info("GetQQUserData", _Exception_With_xKind("GGetQQUserDatJson", ex));
                    return null;
                }
            }
            /// <summary>
            /// QQ用户信息类型
            /// </summary>
            public class QQType
            {
                /// <summary>
                /// 错误代码
                /// </summary>
                public string code { get; set; }
                /// <summary>
                /// 错误信息
                /// </summary>
                public string message { get; set; }
                /// <summary>
                /// 错误信息
                /// </summary>
                public string details { get; set; }
                /// <summary>
                /// QQ号
                /// </summary>
                public string qq { get; set; }

                /// <summary>
                /// 昵称
                /// </summary>
                public string nickname { get; set; }
                /// <summary>
                /// 个性签名
                /// </summary>
                public string long_nick { get; set; }
                /// <summary>
                /// 头像链接
                /// </summary>
                public string avatar_url { get; set; }
                /// <summary>
                /// 年龄
                /// </summary>
                public string age { get; set; }
                /// <summary>
                /// 性别
                /// </summary>
                public string sex { get; set; }
                /// <summary>
                /// QQ个性域名
                /// </summary>
                public string qid { get; set; }
                /// <summary>
                /// QQ等级
                /// </summary>
                public string qq_level { get; set; }
                /// <summary>
                /// 地理位置
                /// </summary>
                public string location { get; set; }
                /// <summary>
                /// 电子邮箱
                /// </summary>
                public string email { get; set; }
                /// <summary>
                /// 是否开了SVIP
                /// </summary>
                public bool is_vip { get; set; }
                /// <summary>
                /// 会员等级
                /// </summary>
                public int vip_level { get; set; }
                /// <summary>
                /// 注册时间（ISO 8601格式）
                /// </summary>
                public string reg_time { get; set; }
                /// <summary>
                /// 注册时间 (字符串格式)
                /// </summary>
                public string reg_time_str => DateTime.ParseExact(reg_time, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal).ToString();
                /// <summary>
                /// 最后更新时间（ISO 8601格式）
                /// </summary>
                public string last_updated { get; set; }
                /// <summary>
                /// 最后更新时间 (字符串格式)
                /// </summary>
                public string last_updated_str => DateTime.ParseExact(last_updated, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal).ToString();

            }

        }
    }
}
