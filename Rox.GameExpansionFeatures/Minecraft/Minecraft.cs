using System;
using static Rox.Runtimes.LogLibraries;

namespace Rox.GameExpansionFeatures
{
    /// <summary>
    /// 我的世界 JE 相关扩展功能库
    /// </summary>
    public partial class Minecraft
    {
        /// <summary>
        /// "村庄英雄"效果加持下的交易价格计算器
        /// </summary>
        /// <param name="BasePrice"></param>
        /// <param name="HearoOfVillage_Level"></param>
        /// <returns></returns>
        public int TradingWithHeroOfVillage_Calculator(int BasePrice, int HearoOfVillage_Level)
        {
            // Minecraft折扣公式：1 - (0.3 + 0.0625 * (heroLevel - 1))
            double discount = 0.3 + 0.0625 * (HearoOfVillage_Level - 1);
            double finalPrice = BasePrice * (1 - discount);
            int value = Math.Max(1, (int)Math.Round(finalPrice));
            WriteLog(LogLevel.Info, LogKind.System, $"村庄英雄交易价格计算器 输出的结果: {value}");
            // 四舍五入并确保最小值为1
            return value;
        }
    }
}
