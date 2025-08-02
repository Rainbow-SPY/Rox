using System.Text.RegularExpressions;
using static Rox.GameExpansionFeatures.Steam.Converter.SteamID;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox.GameExpansionFeatures
{
    public partial class Steam
    {
        /// <summary>
        /// SteamID 类
        /// </summary>
        public partial class SteamID
        {
            /// <summary>
            /// SteamID 的类型
            /// </summary>
            public enum SteamIDType
            {
                /// <summary>
                /// STEAM_x_xxxx
                /// </summary>
                SteamID,
                /// <summary>
                /// [U:1:1xxxx]
                /// </summary>
                SteamID3,
                /// <summary>
                /// 1xxxxxx
                /// </summary>
                SteamID32,
                /// <summary>
                /// 以 7656 为开头的
                /// </summary>
                SteamID64,
                /// <summary>
                /// 未识别到任何 <see cref="SteamIDType"/>, 等效于 <see langword="null"/>
                /// </summary>
                Invalid,
            }
            /// <summary>
            /// 匹配识别 <see cref="SteamIDType"/> 的类型
            /// </summary>
            /// <param name="AnySteamID">任何类型的 <see cref="SteamIDType"/></param>
            /// <returns>其中一种类型的<see cref="SteamIDType"/> 或返回 <see cref="SteamIDType.Invalid"/></returns>
            public static SteamIDType Identifier(string AnySteamID)
            {
                if (string.IsNullOrWhiteSpace(AnySteamID))
                {
                    WriteLog.Error(LogKind.Regex, _value_Not_Is_NullOrEmpty(AnySteamID));
                    return SteamIDType.Invalid;
                }

                if (Regex.IsMatch(AnySteamID, _Regex_ID)) return SteamIDType.SteamID;
                if (Regex.IsMatch(AnySteamID, _Regex_ID3)) return SteamIDType.SteamID3;
                if (Regex.IsMatch(AnySteamID, _Regex_ID32)) return SteamIDType.SteamID32;
                if (Regex.IsMatch(AnySteamID, _Regex_ID64)) return SteamIDType.SteamID64;
                return SteamIDType.Invalid;
            }

        }
    }
}
