using System.Text.RegularExpressions;
using static Rox.GameExpansionFeatures.Steam.SteamID;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox.GameExpansionFeatures
{
    public partial class Steam
    {
        /// <summary>
        /// 
        /// </summary>
        public class Converter
        {
            /// <summary>
            /// SteamID 之间的相互转换
            /// </summary>
            public partial class SteamID
            {
                /// <summary>
                /// SteamID 的 正则表达式 <see cref="System.Text.RegularExpressions.Regex"/> 
                /// </summary>
                internal static string _Regex_ID = @"^STEAM_[0-5]:[01]:\d+$";


                /// <summary>
                /// SteamID3 的 正则表达式 <see cref="System.Text.RegularExpressions.Regex"/> 
                /// </summary>
                internal static string _Regex_ID3 = @"^\[U:1:([0-9]+)\]$";


                /// <summary>
                /// SteamID32 (好友代码) 的 正则表达式 <see cref="System.Text.RegularExpressions.Regex"/> 
                /// </summary>
                internal static string _Regex_ID32 = @"^[0-9]{1,16}$";


                /// <summary>
                /// SteamID64 的 正则表达式 <see cref="System.Text.RegularExpressions.Regex"/> 
                /// </summary>
                internal static string _Regex_ID64 = @"^7656[0-9]*$";


                /// <summary>
                /// 将其中一种的 <see cref="SteamIDType"/> 转换为 <see cref="SteamIDType.SteamID32"/>(好友代码)
                /// </summary>
                /// <param name="SteamID">其中一种的 <see cref="SteamIDType"/> </param>
                /// <returns> <see cref="SteamIDType.SteamID32"/> (好友代码)</returns>
                public static string ToSteamID32(string SteamID)
                {
                    if (string.IsNullOrWhiteSpace(SteamID))
                    {
                        WriteLog.Error(LogKind.Regex, _value_Not_Is_NullOrEmpty("SteamID"));
                        return null;
                    }


                    switch (Identifier(SteamID))
                    {
                        case SteamIDType.SteamID:
                            string[] split = SteamID.Split(':');
                            return (long.Parse(split[2]) * 2 + long.Parse(split[1])).ToString();
                        case SteamIDType.SteamID3:
                            Match match = Regex.Match(SteamID, _Regex_ID3);
                            return match.Success ? match.Groups[1].Value : null;
                        case SteamIDType.SteamID32:
                            return SteamID;
                        case SteamIDType.SteamID64:
                            return (long.Parse(SteamID) - 76561197960265728).ToString();
                        case SteamIDType.Invalid:
                            return null;
                    }
                    return null;
                }


                /// <summary>
                /// 将其中一种的 <see cref="SteamIDType"/> 转换为 <see cref="SteamIDType.SteamID64"/> 
                /// </summary>
                /// <param name="SteamID">其中一种的 <see cref="SteamIDType"/></param>
                /// <returns> <see cref="SteamIDType.SteamID64"/></returns>
                public static string ToSteamID64(string SteamID)
                {
                    if (string.IsNullOrWhiteSpace(SteamID))
                    {
                        WriteLog.Error(LogKind.Regex, _value_Not_Is_NullOrEmpty("SteamID"));
                        return null;
                    }


                    switch (Identifier(SteamID))
                    {
                        case SteamIDType.SteamID:
                            string[] split = SteamID.Split(':');
                            return (long.Parse(split[2]) * 2 + 76561197960265728 + long.Parse(split[1])).ToString();
                        case SteamIDType.SteamID3:
                            Match match = Regex.Match(SteamID, _Regex_ID3);
                            if (match.Success)
                                return $"[U:1:{long.Parse(match.Groups[1].Value) + 76561197960265728}]";
                            return null;
                        case SteamIDType.SteamID32:
                            return (long.Parse(SteamID) + 76561197960265728).ToString();
                        case SteamIDType.SteamID64:
                            return SteamID;
                        case SteamIDType.Invalid:
                            return null;
                    }
                    return null;
                }


                /// <summary>
                /// 将其中一种的 <see cref="SteamIDType"/> 转换为 <see cref="SteamIDType.SteamID3"/>
                /// </summary>
                /// <param name="SteamID"> 其中一种的 <see cref="SteamIDType"/></param>
                /// <returns> <see cref="SteamIDType.SteamID3"/></returns>
                public static string ToSteamID3(string SteamID)
                {
                    if (string.IsNullOrWhiteSpace(SteamID))
                    {
                        WriteLog.Error(LogKind.Regex, _value_Not_Is_NullOrEmpty("SteamID"));
                        return null;
                    }


                    switch (Identifier(SteamID))
                    {
                        case SteamIDType.SteamID:
                            return $"[U:1:{ToSteamID32(SteamID)}]";
                        case SteamIDType.SteamID3:
                            return SteamID;
                        case SteamIDType.SteamID64:
                            return $"[U:1:{ToSteamID32(SteamID)}]";
                        case SteamIDType.SteamID32:
                            return $"[U:1:{SteamID}]";
                        case SteamIDType.Invalid:
                            return null;
                    }
                    return null;
                }
                /// <summary>
                /// 将其中一种的 <see cref="SteamIDType"/> 转换为 <see cref="SteamIDType.SteamID"/>
                /// </summary>
                /// <param name="SteamID"> 其中一种的 <see cref="SteamIDType"/></param>
                /// <returns> <see cref="SteamIDType.SteamID"/></returns>
                public static string ToSteamID(string SteamID)
                {
                    if (string.IsNullOrWhiteSpace(SteamID))
                    {
                        WriteLog.Error(LogKind.Regex, _value_Not_Is_NullOrEmpty("SteamID"));
                        return null;
                    }

                    long SteamID32 = 0;

                    switch (Identifier(SteamID))
                    {
                        case SteamIDType.SteamID:
                            return SteamID;
                        case SteamIDType.SteamID3:
                            SteamID32 = long.Parse(ToSteamID64(SteamID)) - 76561197960265728;
                            break;
                        case SteamIDType.SteamID32:
                            SteamID32 = long.Parse(SteamID);
                            break;
                        case SteamIDType.SteamID64:
                            SteamID32 = long.Parse(SteamID) - 76561197960265728;
                            break;
                        case SteamIDType.Invalid:
                            WriteLog.Error(_input_value_Not_Is_xType(SteamID, $"{SteamIDType.SteamID} 或 {SteamIDType.SteamID3} 或 {SteamIDType.SteamID32} 或 {SteamIDType.SteamID64}"));
                            return null;
                    }

                    return $"STEAM_0:{(SteamID32) & 1}:{(SteamID32 - ((SteamID32) & 1)) / 2}";
                }
            }
        }
    }
}
