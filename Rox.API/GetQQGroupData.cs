using Rox.Runtimes;
using System;
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
        /// 获取QQ群公开摘要
        /// </summary>
        public class GetQQGroupData
        {
            /// <summary>
            ///  获取QQ群公开摘要
            /// </summary>
            /// <param name="QGroupID">QQ群ID</param>
            /// <returns><see cref="QGroupType"/> 对象</returns>
            public static async Task<QGroupType> GetQQGroupDataJson(string QGroupID) => await SendMessageRequest(QGroupID);

            private static async Task<QGroupType> SendMessageRequest(string QGroupID)
            {
                if (string.IsNullOrEmpty(QGroupID))
                {
                    WriteLog.Error("GetQQGroupData", $"{_value_Not_Is_NullOrEmpty("QQ")}, 错误代码: {_String_NullOrEmpty}");
                    return null;
                }

                try
                {
                    using (var client = new HttpClient())
                    {
                        // 请求地址
                        var requestUrl = $"https://uapis.cn/api/v1/social/qq/groupinfo?group_id={QGroupID}";
                        using (var response = await client.GetAsync(requestUrl))
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                WriteLog.Error(LogKind.Network, $"请求失败: {response.StatusCode}, 错误代码: {_HttpClient_Request_Failed}");
                                MessageBox_I.Error($"请求失败: {response.StatusCode}, 错误代码: {_HttpClient_Request_Failed}",_ERROR);
                                return null;
                            }
                            var responseData = await response.Content.ReadAsStringAsync();
                            WriteLog.Info(LogKind.Json, "压缩 Json");
                            string compressedJson = CompressJson(responseData);
                            WriteLog.Info(LogKind.Json, "反序列化 Json 对象");
                            var qGroupType = DeserializeObject<QGroupType>(compressedJson);
                            switch ((int)response.StatusCode)
                            {
                                case 200:
                                    WriteLog.Info(LogKind.Network, $"请求成功");
                                    break;
                                case 400:
                                    WriteLog.Error(LogKind.Network, $"无效的请求, 缺少必要的参数");
                                    break;
                                case 404:
                                    WriteLog.Error(LogKind.Network, $"获取QQ群不存在或无法访问");
                                    break;
                                default:
                                    WriteLog.Error(LogKind.Network, $"未知异常, 请联系管理员, 错误代码: {_UNKNOW_ERROR}");
                                    throw new IException.UAPI.General.UnknowUAPIException();
                            }
                            if (qGroupType.group_name == "" || qGroupType.group_id == string.Empty || string.IsNullOrEmpty(qGroupType.member_count.ToString()))
                                return null;
                            return qGroupType;
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog.Error("GetQQGroupData", _Exception_With_xKind("GetQQGroupDataJson", ex));
                    return null;
                }
            }
            /// <summary>
            /// QQ群信息类型
            /// </summary>
            public class QGroupType
            {
                /// <summary>
                /// 群ID
                /// </summary>
                public string group_id { get; set; }
                /// <summary>
                /// 群名称
                /// </summary>
                public string group_name { get; set; }
                /// <summary>
                /// 群头像链接地址
                /// </summary>
                public string avatar_url { get; set; }
                /// <summary>
                /// 群简介
                /// </summary>
                public string description { get; set; }
                /// <summary>
                /// 群标签
                /// </summary>
                public string tag { get; set; }
                /// <summary>
                /// 群QR码地址
                /// </summary>
                public string join_url { get; set; }
                /// <summary>
                /// 最后更新时间 (ISO 8601)
                /// </summary>
                public string last_updated { get; set; }
                /// <summary>
                /// 当前成员数
                /// </summary>
                public int member_count { get; set; }
                /// <summary>
                /// 最大成员数量
                /// </summary>
                public int max_member_count { get; set; }
                /// <summary>
                /// 活跃成员数（可选，部分群有此数据）
                /// </summary>
                public int active_member_num { get; set; }
                /// <summary>
                /// 群主QQ号（可选）
                /// </summary>
                public string owner_uin { get; set; }
                /// <summary>
                /// 群主UID（可选）
                /// </summary>
                public string owner_uid { get; set; }
                /// <summary>
                /// 建群时间戳（Unix时间戳，可选）
                /// </summary>
                public string create_time { get; set; }
                /// <summary>
                /// 建群时间格式化字符串（可选）
                /// </summary>
                public string create_time_str { get; set; }
                /// <summary>
                /// 群等级（可选）
                /// </summary>
                public int group_grade { get; set; }
                /// <summary>
                /// 群公告/简介（可选）
                /// </summary>
                public string group_memo { get; set; }
                /// <summary>
                /// 认证类型（0=未认证，可选）
                /// </summary>
                public int cert_type { get; set; }
                /// <summary>
                /// 认证类型
                /// </summary>
                public string cert_type_str => cert_type == 0 ? "未认证" : "已认证";
                /// <summary>
                /// 认证说明文本（可选）
                /// </summary>
                public string cert_text { get; set; }
                //////////////////////////////////////////////////////////////////////
                /// <summary>
                /// 错误代码
                /// </summary>
                public string code { get; set; }
                /// <summary>
                /// ???
                /// </summary>
                public string[] details { get; set; }
                /// <summary>
                ///  错误消息
                /// </summary>
                public string message { get; set; }
                /// <summary>
                /// 错误消息
                /// </summary>
                public string error { get; set; }
            }
        }
    }
}
