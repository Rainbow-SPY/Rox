using static Rox.GameExpansionFeatures.Steam.Converter.SteamID;
using static Rox.GameExpansionFeatures.Steam.SteamID;

namespace Rox.GameExpansionFeatures
{
    /// <summary>
    /// Steam 游戏平台相关扩展功能库
    /// </summary>
    public partial class Steam
    {
        /// <summary>
        /// 从任意 <see cref="SteamIDType"/> 格式获取好友代码（<see cref="SteamIDType.SteamID32"/>）, 此方法重定向到 <see cref="Converter.SteamID.ToSteamID32(string)"/> 方法。
        /// </summary>
        /// <param name="AnySteamID"> 任意格式的 <see cref="Rox.GameExpansionFeatures.Steam.Converter.SteamID"/></param>
        /// <returns> 好友代码（<see cref="SteamIDType.SteamID32"/>）</returns>
        public static string GetFriendCode(string AnySteamID) => ToSteamID32(AnySteamID);
    }
}
