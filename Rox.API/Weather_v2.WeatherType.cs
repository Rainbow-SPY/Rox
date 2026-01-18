using System.Collections.Generic;

namespace Rox
{
    public partial class API
    {
        public partial class Weather_v1
        {
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
                /// <see langword="int"/>高德地图的6位数字城市编码
                /// </summary>
                public int adcode { get; set; }
                /// <summary>
                /// 天气状况
                /// </summary>
                public string weather { get; set; }
                /// <summary>   
                /// <see langword="double"/> 温度
                /// </summary>
                public double temperature { get; set; }
                /// <summary>
                /// 风向
                /// </summary>
                public string wind_direction { get; set; }
                /// <summary>
                /// 风力等级
                /// </summary>
                public string wind_power { get; set; }
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
                /// <summary>
                /// <see langword="double"/> 一天中的最高温度
                /// </summary>
                public double temp_max { get; set; }
                /// <summary>
                /// <see langword="double"/> 一天中的最低温度
                /// </summary>
                public double temp_min { get; set; }
                /// <summary>
                /// <see cref="List{Forcast}"/> 未来三天的天气预报
                /// </summary>
                public List<Forcast> forecast { get; set; }
                /// <summary>
                /// 未来3天天气预报（forecast=true 时返回）
                /// </summary>
                public class Forcast
                {
                    /// <summary>
                    /// 预告日期
                    /// </summary>
                    public string date { get; set; }
                    /// <summary>
                    /// <see langword="double"/> 一天中的最高温度
                    /// </summary>
                    public double temp_max { get; set; }
                    /// <summary>
                    /// <see langword="double"/> 一天中的最低温度
                    /// </summary>
                    public double temp_min { get; set; }
                    /// <summary>
                    /// 白天天气
                    /// </summary>
                    public string weather_day { get; set; }
                    /// <summary>
                    /// 夜间天气
                    /// </summary>
                    public string weather_night { get; set; }
                    /// <summary>
                    /// <see langword="int"/> 湿度
                    /// </summary>
                    public int humidity { get; set; }
                    /// <summary>
                    /// 降水量 mm
                    /// </summary>
                    public double precip { get; set; }
                    /// <summary>
                    /// 能见度 km
                    /// </summary>
                    public int visibility { get; set; }
                    /// <summary>
                    /// 紫外线指数 0-11+
                    /// </summary>
                    public double uv_index { get; set; }
                }
                /// <summary>
                /// 体感温度 °C（extended=true 时返回）
                /// </summary>
                public double feels_like { get; set; }
                /// <summary>
                /// 能见度 km（extended=true 时返回）
                /// </summary>
                public int visibility { get; set; }
                /// <summary>
                /// 气压 hPa（extended=true 时返回）
                /// </summary>
                public int pressure { get; set; }
                /// <summary>
                /// 紫外线指数 0-11+（extended=true 时返回）
                /// </summary>
                public double uv { get; set; }
                /// <summary>
                /// 空气质量指数 0-500（extended=true 时返回）
                /// </summary>
                public int aqi { get; set; }
                /// <summary>
                /// 降水量 mm（extended=true 时返回）
                /// </summary>
                public double precipitation { get; set; }
                /// <summary>
                /// 云量 %（extended=true 时返回）
                /// </summary>
                public int cloud { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public class IndicesLevel
                {
                    /// <summary>
                    /// 等级
                    /// </summary>
                    public string level { get; set; }
                    /// <summary>
                    /// 简述
                    /// </summary>
                    public string brief { get; set; }
                    /// <summary>
                    /// 建议
                    /// </summary>
                    public string advice { get; set; }
                }
                /// <summary>
                /// 生活指数
                /// </summary>
                public Life_Indices life_indices { get; set; }
                /// <summary>
                /// 生活指数（indices=true 时返回）
                /// </summary>
                public class Life_Indices
                {
                    /// <summary>
                    /// 穿衣指数
                    /// </summary>
                    public IndicesLevel clothing { get; set; }
                    /// <summary>
                    /// 紫外线指数
                    /// </summary>
                    public IndicesLevel uv { get; set; }
                    /// <summary>
                    /// 洗车指数
                    /// </summary>
                    public IndicesLevel car_wash { get; set; }
                    /// <summary>
                    /// 晾晒指数
                    /// </summary>
                    public IndicesLevel drying { get; set; }
                    /// <summary>
                    /// 空调指数
                    /// </summary>
                    public IndicesLevel air_conditioner { get; set; }
                    /// <summary>
                    /// 感冒指数
                    /// </summary>
                    public IndicesLevel cold_risk { get; set; }
                    /// <summary>
                    /// 运动指数
                    /// </summary>
                    public IndicesLevel exercise { get; set; }
                    /// <summary>
                    /// 舒适度指数
                    /// </summary>
                    public IndicesLevel comfort { get; set; }
                }
            }
        }
    }
}
