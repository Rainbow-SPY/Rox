using Rox.Runtimes;
using System;
using System.IO;
using System.Windows.Forms;
using static Rox.API;
using static Rox.Runtimes.LogLibraries;

namespace Rox.GameExpansionFeatures
{
public partial class Minecraft
    {
        /// <summary>
        /// 返回我的世界 JE 完整命令 (带斜杠 "/")
        /// </summary>
        public class Command
        {
            /// <summary>
            /// 返回我的世界 JE 完整给予 (/give) 命令
            /// </summary>
            public class Give
            {
                /// <summary>
                /// 我的世界 JE 给予目标枚举
                /// </summary>
                public enum Target
                {
                    /// <summary>
                    /// 所有玩家 All Player
                    /// </summary>
                    a,
                    /// <summary>
                    /// 所有实体 All Entity
                    /// </summary>
                    e,
                    /// <summary>
                    /// 距离最近的玩家 Nearest Player
                    /// </summary>
                    p,
                    /// <summary>
                    /// 随机一位玩家 Random Player
                    /// </summary>
                    r,
                    /// <summary>
                    /// 命令的执行者 Executor Self
                    /// </summary>
                    s,
                }
                /// <summary>
                /// 从原版 Json 里提取ItemID
                /// </summary>
                /// <param name="JEVerFolder">我的世界 JE 文件夹目录 e.g D:\Minecraft\Minecraft 1.19.4-Forge-Optifine</param>
                public static string GetItemID(string JEVerFolder)
                {
                    // 检测是否为文件夹
                    if (!System.IO.Directory.Exists(JEVerFolder))
                    {
                        WriteLog.Error(LogKind.System, "指定的文件夹不存在或不是一个有效的文件夹。");
                        MessageBox.Show(JEVerFolder + " 不是一个有效的文件夹。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return null;
                    }
                    // 删除路径末尾的反斜杠
                    if (JEVerFolder.EndsWith("\\"))
                        JEVerFolder.TrimEnd('\\');
                    // 遍历 Version文件夹, 寻找版本
                    string[] folders_ = Directory.GetDirectories(JEVerFolder + "\\versions");
                    // folders 这里是 文件夹内的所有文件夹和文件的名称
                    try
                    {
                        foreach (string folder in folders_)
                        {
                            // folder 这里是 文件夹的名称, e.g . "1.20.2" "1.19.4-forge-45.0.30"
                            // 判断是否为文件夹, 而不是文件
                            if (!Directory.Exists(folder))
                                continue;
                            // 分割文件夹名称, 以便获取版本号
                            string[] parts = folder.Split('\\');
                            // 获取最后一个部分作为文件夹名称
                            string folderName = parts[parts.Length - 1];
                            WriteLog.Info(LogKind.Json, $"正在处理文件夹: {folderName}");
                            // 游戏版本的Json文件名称
                            string JsonName = folderName + ".json";
                            WriteLog.Info(LogKind.Json, $"Json 文件名称: {JsonName}");
                            if (File.Exists($"{folder}\\{JsonName}"))
                            {
                                // 读取文件
                                LogLibraries.WriteLog.Info(LogKind.Json, "获取原始 Json 内容");
                                var data = File.ReadAllText(folder + "\\" + JsonName);
                                LogLibraries.WriteLog.Info(LogKind.Json, "压缩 Json");
                                string compressedJson = CompressJson(data);
                                LogLibraries.WriteLog.Info(LogKind.Json, "反序列化 Json 对象");
                                var jsonObject = Rox.Text.Json.DeserializeObject<MinecraftType>(compressedJson);
                                WriteLog.Info(LogKind.Json, jsonObject.clientVersion);
                                return jsonObject.clientVersion; // 返回版本号
                            }
                        }
                        return null; // 如果没有找到任何有效的 Json 文件，则返回 null
                    }
                    catch (Exception ex)
                    {
                        WriteLog.Error(LogKind.System, "读取文件时发生错误: " + ex.Message);
                        MessageBox.Show("读取文件时发生错误: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return null;
                    }
                }
                /// <summary>
                /// 我的世界 JE 版本 Json 对象
                /// </summary>
                public class MinecraftType
                {
                    /// <summary>
                    /// 版本 ID
                    /// </summary>
                    public string clientVersion { get; set; }
                }
            }
        }
    }
}
