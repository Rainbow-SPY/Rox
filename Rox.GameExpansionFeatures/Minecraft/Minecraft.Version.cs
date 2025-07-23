using System;
using System.Text.RegularExpressions;
using static Rox.Runtimes.LogLibraries;

namespace Rox.GameExpansionFeatures
{
    public partial class Minecraft
    {
        /// <summary>
        ///  
        /// </summary>
        public class Version
        {
            /// <summary>
            /// 正则表达式，用于匹配我的世界 JE 版本号
            /// </summary>
            public static string VersionChek(string version)
            {
                // 支持的大版本列表
                string[] AllMasterVersions =
                {
                        "1.7",
                        "1.8",
                        "1.9",
                        "1.10",
                        "1.11",
                        "1.12",
                        "1.13",
                        "1.14",
                        "1.15",
                        "1.16",
                        "1.17",
                        "1.18",
                        "1.19",
                        "1.20",
                        "1.21"
                 };

                version = version.Trim(); // 去除前后空格
                if (version.EndsWith("."))
                {
                    version = version.TrimEnd('.'); // 去除末尾的点
                }
                WriteLog.Info(LogKind.System, $"正在检查版本: {version}");

                Regex regex = new Regex(@"(?:(?<release>\d+\.\d+(?:\.\d+)?)|(?<snapshot>\d{2}w\d{2}[a-z](?:_[a-z]+)?)|(?<pre_release>\d\.\d+(?:\.\d)?-pre\d+)|(?<rc>\d\.\d+(?:\.\d)?-rc\d+)|(?<experimental>(?:Experimental )?Snapshot \d{2}w\d{2}[a-z]?))");
                Match match = regex.Match(version);
                switch (GetMatchGroup(match))
                {
                    case "Release": // 正式版
                        string limitVer = version.Substring(0, 3); // 获取前3个字符
                        if (limitVer == "1.1" || limitVer == "1.2")// 如果版本为 1.10+ 则获取前4个字符
                        {
                            limitVer = version.Substring(0, 4); // 获取前4个字符
                        }
                        // 检查是否在支持的大版本列表中
                        if (Array.Exists(AllMasterVersions, v => v == limitVer))
                        {
                            WriteLog.Info(LogKind.Regex, $"版本 {version} 在 正则表达式匹配的结果为 {limitVer} 正式版(Release)");
                            return limitVer; // 返回大版本号
                        }
                        return limitVer; // 返回大版本号
                    case "Experimental Snapshot": // 实验性快照版
                        WriteLog.Error(LogKind.Regex, $"版本 {version} 在 正则表达式匹配的结果为 实验性快照(Experimental Snapshot)");
                        return "Experimental Snapshot"; // 返回实验性快照
                    case "Release Candidate": // 候选发布版
                        WriteLog.Error(LogKind.Regex, $"版本 {version} 在 正则表达式匹配的结果为 候选发布版(Release Candidate)");
                        return "Release Candidate"; // 返回候选发布版
                    case "Pre-Release": // 预发布版
                        WriteLog.Error(LogKind.Regex, $"版本 {version} 在 正则表达式匹配的结果为 预发布版(Pre-release)");
                        return "Pre-release"; // 返回预发布版
                    case "Snapshot": // 快照版
                        WriteLog.Error(LogKind.Regex, $"版本 {version} 在 正则表达式匹配的结果为 快照(Snapshot)");
                        return "Snapshot"; // 返回快照版
                    default: // 未知结果
                        WriteLog.Error(LogKind.Regex, $"版本 {version} 在 正则表达式匹配的结果为 未知结果");
                        return "Unknow"; // 返回未知结果
                }
            }
            /// <summary>
            /// 对正则表达式匹配结果进行分组，获取版本类型
            /// </summary>
            /// <param name="match"> 正则表达式匹配结果 </param>
            /// <returns> 版本类型字符串 </returns>
            public static string GetMatchGroup(Match match)
            {
                if (match.Groups["release"].Success) return "Release";
                if (match.Groups["snapshot"].Success) return "Snapshot";
                if (match.Groups["pre_release"].Success) return "Pre-release";
                if (match.Groups["rc"].Success) return "Release Candidate";
                if (match.Groups["experimental"].Success) return "Experimental Snapshot";
                return "unknow";
            }
        }
    }
}
