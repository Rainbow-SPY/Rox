using System.Collections.Generic;

namespace Rox.GameExpansionFeatures.EpicGames
{
    public partial class GetFreeGames
    {
        /// <summary>
        /// Epic Games API
        /// </summary>
        public class EpicType
        {
            /// <summary>
            /// 返回值
            /// </summary>
            public string code { get; set; }
            /// <summary>
            /// 信息
            /// </summary>
            public string message { get; set; }
            /// <summary>
            /// 免费游戏列表（数组）
            /// </summary>
            public List<GameData> data { get; set; }
            /// <summary>
            /// 免费游戏列表数组。
            /// </summary>
            public class GameData
            {
                /// <summary>
                /// 游戏的唯一标识ID。
                /// </summary>
                public string id { get; set; }
                /// <summary>
                /// 游戏的完整标题名称。
                /// </summary>
                public string title { get; set; }
                /// <summary>
                /// 游戏封面图片的URL地址。
                /// </summary>
                public string cover { get; set; }
                /// <summary>
                /// 游戏的原价，单位为人民币元。
                /// </summary>
                public int original_price { get; set; }
                /// <summary>
                /// 格式化后的原价描述字符串。
                /// </summary>
                public string original_price_desc { get; set; }
                /// <summary>
                /// 游戏的简介描述。
                /// </summary>
                public string description { get; set; }
                /// <summary>
                /// 发行商
                /// </summary>
                public string seller { get; set; }
                /// <summary>
                /// 当前是否处于免费状态。
                /// </summary>
                public bool is_free_now { get; set; }
                /// <summary>
                /// 免费开始时间的可读字符串格式。
                /// </summary>
                public string free_start { get; set; }
                /// <summary>
                /// 免费开始时间的13位毫秒时间戳。
                /// </summary>
                public long free_start_at { get; set; }
                /// <summary>
                /// 免费结束时间的可读字符串格式。
                /// </summary>
                public string free_end { get; set; }
                /// <summary>
                /// 免费结束时间的13位毫秒时间戳。
                /// </summary>
                public long free_end_at { get; set; }
                /// <summary>
                /// 游戏在Epic Games商店的详情页链接。
                /// </summary>
                public string link { get; set; }
            }
        }
    }
}
