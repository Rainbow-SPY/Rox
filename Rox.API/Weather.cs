using Rox.Runtimes;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;

namespace Rox
{
    /// <summary>
    /// API 类
    /// </summary>
    public partial class API
    {
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
            /// <returns><see cref="WeatherType"/> 类型的 <see cref="Text.Json"/> 对象</returns>
            public static async Task<WeatherType> GetWeatherDataJson(string city)
            {
                try
                {
                    if (string.IsNullOrEmpty(city))
                    {
                        MessageBox_I.Error($"{_value_Not_Is_NullOrEmpty("city")}, 错误代码: {_String_NullOrEmpty}", _ERROR);
                        return null;
                    }
                    var httpClient = new HttpClient();
                    var requestUrl = $"https://uapis.cn/api/weather?name={city}";
                    var response = await httpClient.GetAsync(requestUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox_I.Error($"请求失败: {response.StatusCode}, 错误代码: {_HttpClient_Request_Failed}", _ERROR);
                        return null;
                    }
                    var responseData = await response.Content.ReadAsStringAsync();
                    string compressedJson = Rox.Text.Json.CompressJson(responseData);
                    LogLibraries.WriteLog.Info("压缩 Json");
                    // 直接解析 JSON 字符串
                    //Text.Json.JObject jObject = Rox.Text.Json.JObject.Parse(compressedJson);
                    var weatherType = Rox.Text.Json.DeserializeObject<WeatherType>(compressedJson);
                    switch (weatherType.code) // 修改为通过实例访问 code 属性
                    {
                        case 400:
                            WriteLog.Error(LogKind.Network, $"{_value_Not_Is_NullOrEmpty("city")}, 错误代码: {_String_NullOrEmpty}");
                            MessageBox_I.Error($"{_value_Not_Is_NullOrEmpty("city")} , 错误代码: {_String_NullOrEmpty}", _ERROR);
                            return null;
                        case 500:
                            WriteLog.Error(LogKind.Network, $"请求的城市不存在或未找到, 错误代码: {_Weather_City_Not_Found}");
                            MessageBox_I.Error($"请求的城市不存在或未找到, 错误代码: {_Weather_City_Not_Found}", _ERROR);
                            return null;
                        case 0:
                            WriteLog.Error(LogKind.Network, $"检测到非法/不安全的请求!访问已拒绝, 错误代码: {_HttpClient_Request_UnsafeOrIllegal_Denied}");
                            MessageBox_I.Error($"检测到非法/不安全的请求!访问已拒绝, 错误代码: {_HttpClient_Request_UnsafeOrIllegal_Denied}", _ERROR);
                            return null;
                    }
                    //if (jObject == null)
                    //{
                    //    LogLibraries.WriteLog.Error("Failed to parse JSON object.");
                    //    return null;
                    //}
                    // 检测是否传入null
                    if (weatherType.temperature == "" || weatherType.temperature == string.Empty || string.IsNullOrWhiteSpace(weatherType.temperature))
                        return null;
                    WriteLog.Info($"Code: {weatherType.code}");
                    WriteLog.Info($"获取省份名称: {weatherType.province}");
                    WriteLog.Info($"获取城市名称: {weatherType.city}");
                    WriteLog.Info($"获取温度: {weatherType.temperature_1}");
                    WriteLog.Info($"获取天气状况: {weatherType.weather}");
                    WriteLog.Info($"获取风向: {weatherType.wind_direction_1}");
                    WriteLog.Info($"获取风力等级: {weatherType.wind_power_1}");
                    WriteLog.Info($"获取湿度: {weatherType.humidity_1}");
                    WriteLog.Info($"数据更新时间: {weatherType.reporttime}");
                    return weatherType;
                }
                catch (Exception ex)
                {
                    LogLibraries.WriteLog.Error($"获取天气信息失败，请检查网络连接或API服务状态: {ex.Message}, 错误代码:  {_Weather_Unknow_Exception}");
                    return null;
                }
            }
            #region 获取简单天气信息
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
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastWeatherData?.temperature_1));
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
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastWeatherData?.weather));
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
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastWeatherData?.wind_direction_1));
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
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastWeatherData?.wind_power_1));
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
                WriteLog.Info(LogKind.Json, _Return_xKind_value("Json", _lastWeatherData?.humidity_1));
                return _lastWeatherData?.humidity_1;
            }
            #endregion
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

    }
}
