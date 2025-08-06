using static Rox.GameExpansionFeatures.Steam.Converter.SteamID;

namespace Rox.GameExpansionFeatures
{
    /// <summary>
    /// Steam 游戏平台相关扩展功能库
    /// </summary>
    public partial class Steam
    {
        public partial class SteamID
        {
            /// <summary>
            /// 从任意 <see cref="SteamIDType"/> 格式获取好友代码（<see cref="SteamIDType.SteamID32"/>）, 此方法重定向到 <see cref="Converter.SteamID.ToSteamID32(string)"/> 方法。
            /// </summary>
            /// <param name="AnySteamID"> 任意格式的 <see cref="Rox.GameExpansionFeatures.Steam.Converter.SteamID"/></param>
            /// <returns> 好友代码（<see cref="SteamIDType.SteamID32"/>）</returns>
            public static string GetFriendCode(string AnySteamID) => ToSteamID32(AnySteamID);
            /// <summary>
            /// 获取 Steam 用户的在线状态, 此方法仅限于 <see cref="SteamUserData_v1.SteamType"/> 的 <see cref="SteamUserData_v1.SteamType.personastate"/> 属性。
            /// </summary>
            /// <param name="steamType"> <see cref="SteamUserData_v1.SteamType"/> 对象, 其中包含了 Steam 用户的在线状态信息</param>
            /// <returns> Steam 用户的在线状态字符串</returns>
            public static string GetPersonaState(SteamUserData_v1.SteamType steamType)
            {
                switch (steamType.personastate)
                {
                    case 0:
                        return "离线或私密";
                    case 1:
                        return "在线";
                    case 2:
                        return "忙碌";
                    case 3:
                        return "离开";
                    case 4:
                        return "打盹";
                    case 5:
                        return "想交易";
                    case 6:
                        return "想玩";
                }
                return "未知状态";
            }
        }
    }
}
