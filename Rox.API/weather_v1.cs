using Rox.Runtimes;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    public partial class API
    {
        /// <summary>
        /// 天气查询, 但是新版本
        /// </summary>
        public class Weather_v1
        {
            /// <summary>
            /// 获取天气信息
            /// </summary>
            /// <param name="city">城市名称</param>
            /// <returns><see cref="WeatherType"/> 类型的 <see cref="Text.Json"/> 对象</returns>
            public static async Task<WeatherType> GetWeatherDataJson(string city)
            {
                return await SendMessageRequest(city, "city");
            }
            /// <summary>
            /// 获取天气信息
            /// </summary>
            /// <param name="adcode">高德地图的6位数字城市编码</param>
            /// <returns><see cref="WeatherType"/> 类型的 <see cref="Text.Json"/> 对象</returns>
            public static async Task<WeatherType> GetWeatherDataJson(int adcode)
            {
                return await SendMessageRequest(adcode.ToString(), "adcode");
            }
            private static async Task<WeatherType> SendMessageRequest(string city_Or_adcode, string param)
            {
                try
                {
                    if (string.IsNullOrEmpty(city_Or_adcode))
                    {
                        WriteLog.Error(LogKind.Network, $"{_value_Not_Is_NullOrEmpty("city_Or_adcode")}, 错误代码: {_String_NullOrEmpty}");
                        MessageBox_I.Error($"{_value_Not_Is_NullOrEmpty("city_Or_adcode")}, 错误代码: {_String_NullOrEmpty}", _ERROR);
                        return null;
                    }
                    var httpClient = new HttpClient();
                    var requestUrl = $"https://api.uapis.cn/api/v1/misc/weather?{param}={city_Or_adcode}";
                    var response = await httpClient.GetAsync(requestUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox_I.Error($"请求失败: {response.StatusCode}, 错误代码: {_HttpClient_Request_Failed}", _ERROR);
                        return null;
                    }
                    var responseData = await response.Content.ReadAsStringAsync();
                    string compressedJson = Rox.Text.Json.CompressJson(responseData);
                    LogLibraries.WriteLog.Info(LogKind.Json, "压缩 Json");
                    var weatherType = Rox.Text.Json.DeserializeObject<WeatherType>(compressedJson);
                    WriteLog.Info(LogKind.Json, "反序列化 Json 对象");
                    switch ((int)response.StatusCode) // 修改为通过实例访问 code 属性
                    {
                        case 400:
                            WriteLog.Error(LogKind.Network, $"{_value_Not_Is_NullOrEmpty("city_Or_adcode")}, 错误代码: {_String_NullOrEmpty}, 错误信息: {weatherType.code} - {weatherType.message}");
                            MessageBox_I.Error($"{_value_Not_Is_NullOrEmpty("city_Or_adcode")}, 错误代码: {_String_NullOrEmpty}, 错误信息: {weatherType.code} - {weatherType.message}", _ERROR);
                            return null;
                        case 410:
                            WriteLog.Error(LogKind.Network, $"请求的城市不存在或未找到, 错误代码: {_Weather_City_Not_Found}, 错误信息: {weatherType.code} - {weatherType.message}");
                            MessageBox_I.Error($"请求的城市不存在或未找到, 错误代码: {_Weather_City_Not_Found}, 错误信息: {weatherType.code} - {weatherType.message}", _ERROR);
                            return null;
                        case 502:
                            WriteLog.Error(LogKind.Network, $"上游服务错误, 天气供应商API暂时不可用或返回了错误, 错误代码: {_Weather_Service_Error}, 错误信息: {weatherType.code} - {weatherType.message}");
                            MessageBox_I.Error($"上游服务错误, 天气供应商API暂时不可用或返回了错误, 错误代码: {_Weather_Service_Error}, 错误信息: {weatherType.code} - {weatherType.message}", _ERROR);
                            return null;
                        case 500:
                            WriteLog.Error(LogKind.Network, $"服务器内部错误。在处理天气数据时发生了未知问题, 错误代码: {_Weather_Unknow_Exception}, 错误信息: {weatherType.code} - {weatherType.message}");
                            MessageBox_I.Error($"服务器内部错误。在处理天气数据时发生了未知问题, 错误代码: {_Weather_Unknow_Exception}, 错误信息: {weatherType.code} - {weatherType.message}", _ERROR);
                            return null;
                        case 200:
                            WriteLog.Info(LogKind.Network, $"请求成功");
                            break;
                    }
                    if (weatherType.weather == "" || weatherType.weather == string.Empty || string.IsNullOrWhiteSpace(weatherType.weather))
                        return null;
                    return weatherType;
                }
                catch (Exception ex)
                {
                    WriteLog.Error(LogKind.Network, $"获取天气信息失败，请检查网络连接或API服务状态: {ex.Message}, 错误代码:  {_Weather_Unknow_Exception}");
                    return null;
                }
            }
            /// <summary>
            /// 获取指定城市的数据更新时间信息属性
            /// </summary>
            public class WeatherType
            {
                /// <summary>
                /// 错误的返回值
                /// </summary>
                public string code { get; set; }
                /// <summary>
                /// 省份名称
                /// </summary>
                public string province { get; set; }
                /// <summary>
                /// 城市名称
                /// </summary>
                public string city { get; set; }
                /// <summary>
                /// 高德地图的6位数字城市编码
                /// </summary>
                public string adcode { get; set; }
                /// <summary>
                /// 天气状况
                /// </summary>
                public string weather { get; set; }
                /// <summary>
                /// 温度,带单位
                /// </summary>
                public string temperature_1 => temperature.ToString() + "℃"; // 20℃ 30℃
                /// <summary>   
                /// 温度
                /// </summary>
                public int temperature { get; set; }
                ///// <summary>
                ///// 风向,带单位
                ///// </summary>
                //public string wind_direction_1 => wind_direction + "风"; // 东南风 西北风
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
                public string humidity_1 => humidity.ToString() + "%";
                /// <summary>
                /// 湿度
                /// </summary>
                public int humidity { get; set; }
                /// <summary>
                /// 数据更新时间	
                /// </summary>
                public string report_time { get; set; }
                /// <summary>
                /// 错误信息
                /// </summary>
                public string message { get; set; }
            }
        }
    }
}
