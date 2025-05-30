using Rox.Text;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;

namespace Rox
{
    namespace AI// AI
    {
        /// <summary>
        /// 用于与 DeepSeek API 进行聊天交互的类，提供了配置 API URL 和发送消息的功能。
        /// </summary>
        public class DeepSeek
        {
            // ==================== API配置区（后期可修改） ====================
            /// <summary>
            /// DeepSeek API的URL
            /// </summary>
            private const string ApiUrl = "https://api.deepseek.com/v1/chat/completions"; // DeepSeek API的URL
            /// <summary>
            /// 表示模型的名称，默认为 <see langword="default"/>
            /// </summary>
            public string Model { get; set; } = "default"; // 默认模型
            /// <summary>
            /// 用于与 DeepSeek API 进行聊天交互
            /// </summary>
            /// <param name="text"> 要发送的消息</param>
            /// <param name="api"> DeepSeek API的密钥</param>
            /// <returns></returns>
            public static async Task Chat(string text, string api)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {api}");
                        var requestBody = new
                        {
                            model = "deepseek-chat",
                            messages = new[]
                            {
                            new { role = "user", content = text }
                        }
                        };
                        string json = Json.SerializeObject(requestBody);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        WriteLog(LogLevel.Info, $"{_SEND_REQUEST}...{text}");
                        HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                        string responseJson = await response.Content.ReadAsStringAsync();
                        WriteLog(LogLevel.Info, _GET_RESPONSE);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseObject = Text.Json.DeserializeObject<dynamic>(responseJson);
                            string answer = responseObject.choices[0].message.content;
                            WriteLog(LogLevel.Info, $"{_ANSWER}: {answer}");
                        }
                        else
                        {
                            WriteLog(LogLevel.Error, $"{_ERROR}: {responseJson}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(LogLevel.Error, $"{_ERROR}: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// 用于与 OpenAI API 进行聊天交互的类，提供了配置 API URL 和发送消息的功能。
        /// </summary>
        public class ChatGPT
        {
            // ==================== API配置区（后期可修改） ====================
            /// <summary>
            /// OpenAI API的URL
            /// </summary>
            private const string ApiUrl = "https://api.openai.com/v1/chat/completions"; // OpenAI API的URL
            /// <summary>
            /// 表示模型的名称，默认为 <see langword="gpt-3.5-turbo"/>
            /// </summary>
            public string Model { get; set; } = "gpt-3.5-turbo"; // 默认模型
            /// <summary>
            /// 用于与 OpenAI API 进行聊天交互
            /// </summary>
            /// <param name="text"> 要发送的消息</param>
            /// <param name="apiKey"> OpenAI API的密钥</param>
            /// <returns></returns>
            public static async Task Chat(string text, string apiKey)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                        var requestBody = new
                        {
                            model = "gpt-4o-mini", // 模型名称
                            messages = new[]
                            {
                                new { role = "user", content = text }
                            }
                        };
                        string json = Text.Json.SerializeObject(requestBody);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        WriteLog(LogLevel.Info, $"{_SEND_REQUEST}...{text}");
                        HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                        string responseJson = await response.Content.ReadAsStringAsync();
                        WriteLog(LogLevel.Info, _GET_RESPONSE);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseObject = Text.Json.DeserializeObject<dynamic>(responseJson);
                            string answer = responseObject.choices[0].message.content;
                            WriteLog(LogLevel.Info, $"{_ANSWER}: {answer}");
                        }
                        else
                        {
                            WriteLog(LogLevel.Error, $"{_ERROR} : {responseJson}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(LogLevel.Error, $"{_ERROR} : {ex.Message}");
                }
            }
        }
    }
}
