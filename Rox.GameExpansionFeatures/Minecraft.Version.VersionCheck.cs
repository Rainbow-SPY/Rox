using System;
using System.Text.RegularExpressions;
using static Rox.Runtimes.LogLibraries;

namespace Rox.GameExpansionFeatures
{
    public partial class Minecraft
    {
        /// <summary>
        ///  我的世界 JE 版本相关处理类
        /// </summary>
        public class Version
        {
            /// <summary>
            /// 正则表达式，用于匹配我的世界 JE 版本号
            /// </summary>
            public static string VersionCheck(string version)
            {
#if DEBUG
                WriteLog.Debug("调用方法, 模式: Debug");
#endif
                // 支持的正式版大版本列表
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
                // 不支持的正式版大版本列表
                string[] UnsupportVersions =
                {
                    "1.0",
                    "1.1",
                    "1.2",
                    "1.3",
                    "1.4",
                    "1.5",
                    "1.6",
                };

                version = version.Trim(); // 去除前后空格
                string a = version.Substring(0, 1); // 获取前1个字符
#if DEBUG
                WriteLog.Debug(LogKind.Regex, $"正在检查版本: {version}，前缀: {a}");
#endif
                // 检查版本号的前缀
                if (a == "rd-" || a == "c0" || a == "a1" || a == "b1" || a == "inf-")
                {
                    WriteLog.Error(LogKind.Regex, $"版本 {version} 检测到为 " + (
                        a == "rd-"
                            ? "最初开发版 (Pre-Classic)" : (
                                a == "c0"
                                    ? "第一个长期开发版(Classic)" : (
                                        a == "inf-"
                                            ? "无限开发版 (Infdev)" : (
                                                a == "a1"
                                                    ? "Alpha版" : (
                                                        a == "b1"
                                                        ? "Beta版" : "未知版本"
                                                      )
                                              )
                                      )
                              )
                          ) + " ，可能不是一个正常的版本");
                    return "Unknow"; // 返回未知结果
                }
#if DEBUG
                WriteLog.Debug($"当前参数: version 的值为: {version.ToString()}, 他的前缀为 {version.Substring(0,1)}");
#endif
                // 这个条件的意思是：如果字符串不以"1"开头，或者字符串不以"2"开头，就执行if块中的代码。
                // 如果字符串以"1"开头，那么 !version.StartsWith("2") 为true
                // 如果字符串以"2"开头，那么 !version.StartsWith("1") 为true
                // 如果字符串以其他字符开头，两个条件都为true
                // 应该使用 与(&&) 而不是 或(||)
                if (!version.StartsWith("1") && !version.StartsWith("2"))
                {
                    WriteLog.Error(LogKind.Regex, $"版本 {version} 不以 1 或 2 开头，可能不是一个正常的版本");
                    return "Unknow"; // 返回未知结果
                }
                if (version.EndsWith("."))
                {
                    version = version.TrimEnd('.'); // 去除末尾的点
                }
                WriteLog.Info(LogKind.System, $"正在检查版本: {version}");
                // 检查版本长度是否小于3
                if (version.Length < 3)
                {
                    WriteLog.Error(LogKind.Regex, $"版本 {version} 长度小于3，无法进行正则表达式匹配, 可能不是一个正常的版本");
                    return "Unknow"; // 返回未知结果
                }
                // 检查是否在不支持的大版本列表中
                if (Array.Exists(UnsupportVersions, v => v == version))
                {
                    WriteLog.Error(LogKind.Regex, $"版本 {version} 不在支持的大版本列表中");
                    return "Unsupport"; // 返回未知结果
                }

                Regex regex = new Regex(@"(?:(?<release>\d+\.\d+(?:\.\d+)?)|(?<snapshot>\d{2}w\d{2}[a-z](?:_[a-z]+)?)|(?<pre_release>\d\.\d+(?:\.\d)?-pre\d+)|(?<rc>\d\.\d+(?:\.\d)?-rc\d+)|(?<experimental>(?:Experimental )?Snapshot \d{2}w\d{2}[a-z]?))");
                Match match = regex.Match(version);
                string limitVer = null; // 初始化 limitVer 为 null
                switch (GetMatchGroup(match))
                {
                    case "Release": // 正式版
                        limitVer = version.Substring(0, 3); // 获取前3个字符
                        if (limitVer == "1.0" || limitVer == "1.1" || limitVer == "1.2")// 如果版本为 1.10+ 则获取前4个字符
                        {
                            limitVer = version.Substring(0, 4); // 获取前4个字符
                        }
                        // 检查是否在不支持的大版本列表中
                        if (Array.Exists(UnsupportVersions, v => v == version))
                        {
                            WriteLog.Error(LogKind.Regex, $"版本 {version} 不在支持的大版本列表中");
                            return "Unsupport"; // 返回未知结果
                        }
                        // 检查是否在支持的大版本列表中
                        if (Array.Exists(AllMasterVersions, v => v == limitVer))
                        {
                            WriteLog.Info(LogKind.Regex, $"版本 {version} 在 正则表达式匹配的结果为 {limitVer} 正式版(Release)");
                            return limitVer; // 返回大版本号
                        }
                        WriteLog.Error(LogKind.Regex, $"版本 {version} 在 正则表达式匹配的结果为 {limitVer} 正式版(Release)，但不在支持的大版本列表中");
                        return "Unsupport"; // 返回不支持的版本
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
