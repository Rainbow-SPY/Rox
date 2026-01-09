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
        /// 天气查询, 但是新版本
        /// </summary>
        public partial class Weather_v1
        {
            /// <summary>
            /// 获取天气信息
            /// </summary>
            /// <param name="city">城市名称</param>
            /// <param name="extended">是否返回扩展气象字段（体感温度、能见度、气压、紫外线指数、空气质量、降水量、云量）。</param>
            /// <param name="indices">是否返回生活指数（穿衣、紫外线、洗车、晾晒、空调、感冒、运动、舒适度）。</param>
            /// <param name="forecast">是否返回预报数据（当日最高/最低气温及未来3天天气预报）。</param>
            /// <returns><see cref="WeatherType"/> 类型的 <see cref="Text.Json"/> 对象</returns>
            public static async Task<WeatherType> GetWeatherDataJson(string city, bool extended = false, bool indices = false, bool forecast = false) => await SendMessageRequest(city, "city", extended, indices, forecast);
            /// <summary>
            /// 获取天气信息
            /// </summary>
            /// <param name="adcode">高德地图的6位数字城市编码</param>
            /// <param name="extended">是否返回扩展气象字段（体感温度、能见度、气压、紫外线指数、空气质量、降水量、云量）。</param>
            /// <param name="indices">是否返回生活指数（穿衣、紫外线、洗车、晾晒、空调、感冒、运动、舒适度）。</param>
            /// <param name="forecast">是否返回预报数据（当日最高/最低气温及未来3天天气预报）。</param>
            /// <returns><see cref="WeatherType"/> 类型的 <see cref="Text.Json"/> 对象</returns>
            public static async Task<WeatherType> GetWeatherDataJson(int adcode, bool extended = false, bool indices = false, bool forecast = false) => await SendMessageRequest(adcode.ToString(), "adcode", extended, indices, forecast);
            /// <summary>
            /// 获取天气信息
            /// </summary>
            /// <param name="city_Or_adcode">城市名称或<br/>高德地图的6位数字城市编码</param>
            /// <param name="param">方法</param>
            /// <param name="extended">是否返回扩展气象字段（体感温度、能见度、气压、紫外线指数、空气质量、降水量、云量）。</param>
            /// <param name="indices">是否返回生活指数（穿衣、紫外线、洗车、晾晒、空调、感冒、运动、舒适度）。</param>
            /// <param name="forecast">是否返回预报数据（当日最高/最低气温及未来3天天气预报）。</param>
            /// <returns><see cref="WeatherType"/> 类型的 <see cref="Text.Json"/> 对象</returns>
            private static async Task<WeatherType> SendMessageRequest(string city_Or_adcode, string param, bool extended = false, bool indices = false, bool forecast = false)
            {
                try
                {
                    if (string.IsNullOrEmpty(city_Or_adcode))
                    {
                        WriteLog.Error(LogKind.Network, $"{_value_Not_Is_NullOrEmpty("city_Or_adcode")}, 错误代码: {_String_NullOrEmpty}");
                        MessageBox_I.Error($"{_value_Not_Is_NullOrEmpty("city_Or_adcode")}, 错误代码: {_String_NullOrEmpty}", _ERROR);
                        return null;
                    }
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var requestUrl = $"https://uapis.cn/api/v1/misc/weather?{param}={city_Or_adcode}" +
                            (extended ? "&extended=true" : "") +
                            (indices ? "&indices=true" : "") +
                            (forecast ? "&forecast=true" : "");
                        using (var response = await httpClient.GetAsync(requestUrl))
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                MessageBox_I.Error($"请求失败: {response.StatusCode}, 错误代码: {_HttpClient_Request_Failed}", _ERROR);
                                return null;
                            }
                            var responseData = await response.Content.ReadAsStringAsync();
                            WriteLog.Info(LogKind.Json, "压缩 Json");
                            string compressedJson = CompressJson(responseData);
                            WriteLog.Info(LogKind.Json, "反序列化 Json 对象");
                            var weatherType = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherType>(compressedJson);
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
                                    throw new Rox.Runtimes.IException.UAPI.Weather.WeatherServiceError();
                                case 500:
                                    WriteLog.Error(LogKind.Network, $"服务器内部错误。在处理天气数据时发生了未知问题, 错误代码: {_Weather_Unknow_Exception}, 错误信息: {weatherType.code} - {weatherType.message}");
                                    MessageBox_I.Error($"服务器内部错误。在处理天气数据时发生了未知问题, 错误代码: {_Weather_Unknow_Exception}, 错误信息: {weatherType.code} - {weatherType.message}", _ERROR);
                                    throw new Rox.Runtimes.IException.UAPI.Weather.WeatherAPIServerError();
                                case 200:
                                    WriteLog.Info(LogKind.Network, $"请求成功");
                                    break;
                                default:
                                    WriteLog.Error(LogKind.Network, $"未知异常, 请联系管理员, 错误代码: {_UNKNOW_ERROR}");
                                    throw new IException.UAPI.General.UnknowUAPIException();

                            }
                            WriteLog.Info("Weather", $"请求位置: {weatherType.province} {weatherType.city} Adcode: {weatherType.adcode}\n" +
                                $"今日天气: {weatherType.weather}, 气温:{weatherType.temperature} ℃, 最高气温: {weatherType.temp_max} ℃, 最低气温: {weatherType.temp_min} ℃\n" +
                                $"风向: {weatherType.wind_direction}, 风力 {weatherType.wind_power}, 湿度 {weatherType.humidity}%\n" +
                                "\n");
                            if (forecast)
                            {
                                // 先校验 ForcastData 是否为 null，避免空引用
                                if (weatherType?.forecast == null || weatherType.forecast.Count == 0)
                                {
                                    WriteLog.Warning("Weather", "未来三天天气预报数据为空，跳过遍历");
                                }
                                else
                                {
                                    WriteLog.Info("Weather", $"未来三天的天气预报");
                                    foreach (var _data in weatherType.forecast)
                                    {
                                        WriteLog.Info("Weather Forcast", $"{_data.date} 的天气预报:\n" +
                                            $"白天天气: {_data.weather_day}, 夜间天气: {_data.weather_night}\n" +
                                            $"最高温度: {_data.temp_max} ℃, 最低温度: {_data.temp_min} ℃\n" +
                                            $"降水量: {_data.precip} mm, 能见度: {_data.visibility} km, 紫外线指数: {_data.uv_index}");
                                    }
                                }
                            }
                            if (extended)
                            {
                                WriteLog.Info("Weather", $"体感温度: {weatherType.feels_like} ℃, 能见度: {weatherType.visibility} km, 紫外线指数: {weatherType.uv}\n" +
                                    $"空气质量指数: {weatherType.aqi}, 降水量: {weatherType.precipitation} mm, 云量: {weatherType.cloud} %, 气压: {weatherType.pressure} hPa");
                            }
                            if (indices)
                            {
                                var b = weatherType.life_indices;
                                WriteLog.Info("Weather Indices", $"穿衣指数: {b.clothing.level},简述: {b.clothing.brief},建议: {b.clothing.advice}\n" +
                                    $"紫外线指数: {b.uv.level},简述: {b.uv.brief},建议: {b.uv.advice}\n" +
                                    $"洗车指数: {b.car_wash.level},简述: {b.car_wash.brief},建议: {b.car_wash.advice}\n" +
                                    $"晾晒指数: {b.drying.level},简述: {b.drying.brief},建议: {b.drying.advice}\n" +
                                    $"空调指数: {b.air_conditioner.level},简述: {b.air_conditioner.brief},建议: {b.air_conditioner.advice}\n" +
                                    $"感冒指数: {b.cold_risk.level},简述: {b.cold_risk.brief},建议: {b.cold_risk.advice}\n" +
                                    $"运动指数: {b.exercise.level},简述: {b.exercise.brief},建议: {b.exercise.advice}\n" +
                                    $"舒适度指数: {b.comfort.level},简述: {b.comfort.brief},建议: {b.comfort.advice}");
                            }
                            if (weatherType.weather == "" || weatherType.weather == string.Empty || string.IsNullOrWhiteSpace(weatherType.weather))
                                return null;
                            return weatherType;
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    WriteLog.Error(LogKind.Network, $"HttpClient 请求失败, 请检查网络连接或API服务状态: {ex.Message}, 错误代码: {_HttpClient_Request_Failed}");
                    return null;
                }
                catch (Exception ex)
                {
                    WriteLog.Error(LogKind.Network, $"获取天气信息失败，请检查网络连接或API服务状态: {_Exception_With_xKind("SendMessageRequest", ex)}, 错误代码:  {_Weather_Unknow_Exception}");
                    return null;
                }

            }
        }
    }
}
