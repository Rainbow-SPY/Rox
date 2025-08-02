using System;
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
                /// 将 <see cref="SteamIDType.SteamID64"/> 或 <see cref="SteamIDType.SteamID3"/> 转换为 <see cref="SteamIDType.SteamID32"/>(好友代码)
                /// </summary>
                /// <param name="SteamID64orSteamID3"> <see cref="SteamIDType.SteamID64"/> </param>
                /// <returns> <see cref="SteamIDType.SteamID32"/> (好友代码)</returns>
                public static string SteamID64orSteamID3ToSteamID32(string SteamID64orSteamID3)
                {
                    if (string.IsNullOrWhiteSpace(SteamID64orSteamID3))
                    {
                        WriteLog.Error(LogKind.Regex, _value_Not_Is_NullOrEmpty(SteamID64orSteamID3));
                        return null;
                    }

                    SteamIDType AnySteamID = Identifier(SteamID64orSteamID3);
                    if (AnySteamID != SteamIDType.SteamID64 && AnySteamID != SteamIDType.SteamID3)
                    {
                        WriteLog.Error(_input_value_Not_Is_xType(SteamID64orSteamID3, $"{SteamIDType.SteamID64} 或 {SteamIDType.SteamID3}"));
                        return null;
                    }
                    if (AnySteamID == SteamIDType.SteamID64) return (long.Parse(SteamID64orSteamID3) - 76561197960265728).ToString();
                    else return SteamID3ToSteamID32(SteamID64orSteamID3);
                }
                /// <summary>
                /// <see cref="SteamIDType.SteamID3"/> 或 <see cref="SteamIDType.SteamID32"/> 转换为 <see cref="SteamIDType.SteamID64"/> 
                /// </summary>
                /// <param name="SteamID3orSteamID32"><see cref="SteamIDType.SteamID3"/> 或 <see cref="SteamIDType.SteamID32"/></param>
                /// <returns> <see cref="SteamIDType.SteamID64"/></returns>
                public static string SteamID3orSteamID32ToSteamID64(string SteamID3orSteamID32)
                {
                    if (string.IsNullOrWhiteSpace(SteamID3orSteamID32))
                    {
                        WriteLog.Error(LogKind.Regex, _value_Not_Is_NullOrEmpty(SteamID3orSteamID32));
                        return null;
                    }

                    try
                    {
                        SteamIDType AnysteamID = Identifier(SteamID3orSteamID32);
                        if (AnysteamID != SteamIDType.SteamID3 && AnysteamID != SteamIDType.SteamID32)
                        {
                            WriteLog.Error(_input_value_Not_Is_xType(SteamID3orSteamID32, $"{SteamIDType.SteamID64} 或 {SteamIDType.SteamID32}"));
                            return null;
                        }
                        if (AnysteamID == SteamIDType.SteamID32) return (long.Parse(SteamID3orSteamID32) + 76561197960265728).ToString();

                        else
                        {
                            string a = SteamID3ToSteamID32(SteamID3orSteamID32);
                            if (a == null)
                            {
                                WriteLog.Error(_Convert_Kind_To_Kind($"{SteamIDType.SteamID3} 或 {SteamIDType.SteamID32}", SteamIDType.SteamID64.ToString()) + $": {_void_value_null("SteamID3ToSteamID32", "string")}");
                                return null;
                            }
                            return (long.Parse(a) + 76561197960265728).ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog.Error(LogKind.Math, $"{_Convert_Kind_To_Kind($"{SteamIDType.SteamID3} 或 {SteamIDType.SteamID32}", SteamIDType.SteamID64.ToString())}: {ex.Message}");
                        return null;
                    }
                }
                /// <summary>
                /// <see cref="SteamIDType.SteamID3"/> 转换为 <see cref="SteamIDType.SteamID32"/> 
                /// </summary>
                /// <param name="SteamID3"><see cref="SteamIDType.SteamID3"/></param>
                /// <returns><see cref="SteamIDType.SteamID32"/> </returns>
                internal static string SteamID3ToSteamID32(string SteamID3)
                {
                    if (string.IsNullOrWhiteSpace(SteamID3))
                    {
                        WriteLog.Error(LogKind.Regex, _value_Not_Is_NullOrEmpty(SteamID3));
                        return null;
                    }

                    if (Identifier(SteamID3) != SteamIDType.SteamID3)
                    {
                        WriteLog.Error(_input_value_Not_Is_xType(SteamID3, SteamIDType.SteamID3.ToString()));
                        return null;
                    }
                    Match match = Regex.Match(SteamID3, _Regex_ID3);
                    return match.Success == true ? match.Groups[1].Value : null;
                }
                /// <summary>
                /// 将 <see cref="SteamIDType.SteamID64"/> 或 <see cref="SteamIDType.SteamID32"/> 转换为 <see cref="SteamIDType.SteamID3"/>
                /// </summary>
                /// <param name="SteamID64orSteam32"> <see cref="SteamIDType.SteamID64"/> 或 <see cref="SteamIDType.SteamID32"/></param>
                /// <returns> <see cref="SteamIDType.SteamID3"/></returns>
                public static string Steam64OrSteamID32ToSteamID3(string SteamID64orSteam32)
                {
                    if (string.IsNullOrWhiteSpace(SteamID64orSteam32))
                    {
                        WriteLog.Error(LogKind.Regex, _value_Not_Is_NullOrEmpty(SteamID64orSteam32));   
                        return null;
                    }

                    SteamIDType AnySteamID = Identifier(SteamID64orSteam32);
                    if (AnySteamID != SteamIDType.SteamID32 && AnySteamID != SteamIDType.SteamID64)
                    {
                        WriteLog.Error(_input_value_Not_Is_xType(SteamID64orSteam32, $"{SteamIDType.SteamID64} 或 {SteamIDType.SteamID32}"));
                        return null;
                    }
                    if (AnySteamID == SteamIDType.SteamID32) return $"[U:1:{SteamID64orSteam32}]";
                    else return $"[U:1:{SteamID64orSteamID3ToSteamID32(SteamID64orSteam32)}]";
                }
            }
        }
    }
}
