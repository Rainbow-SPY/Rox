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
        /// 从任意 SteamID 格式获取好友代码（SteamID32）
        /// </summary>
        /// <param name="AnySteamID"> 任意格式的 <see cref="Rox.GameExpansionFeatures.Steam.Converter.SteamID"/></param>
        /// <returns> 好友代码（SteamID32）</returns>
        public static string GetFriendCode(string AnySteamID)
        {
            switch (Identifier(AnySteamID))
            {
                case SteamIDType.SteamID64:
                    return SteamID64orSteamID3ToSteamID32(AnySteamID);
                case SteamIDType.SteamID32:
                    return AnySteamID;
                case SteamIDType.SteamID3:
                    return SteamID64orSteamID3ToSteamID32(AnySteamID);
                case SteamIDType.SteamID:
                    break;
                case SteamIDType.Invalid:
                    break;
                default:
                    break;
            }
            return null;
        }
    }
}
