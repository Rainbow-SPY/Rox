using static Rox.Entertainment.Steam.Converter.SteamID;

namespace Rox.Entertainment
{
    /// <summary>
    /// Steam 游戏平台相关扩展功能库
    /// </summary>
    public partial class Steam
    {
        public partial class SteamID
        {
            /// <summary>
            /// 从任意 <see cref="SteamIDType"/> 格式获取好友代码（<see cref="SteamIDType.SteamID32"/>）, 此方法重定向到 <see cref="ToSteamID32(string)"/> 方法。
            /// </summary>
            /// <param name="AnySteamID"> 任意格式的 <see cref="Converter.SteamID"/></param>
            /// <returns> 好友代码（<see cref="SteamIDType.SteamID32"/>）</returns>
            public static string GetFriendCode(string AnySteamID) => ToSteamID32(AnySteamID);
        }
    }
}