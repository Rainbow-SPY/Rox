using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    namespace Runtimes
    {
        /// <summary>
        /// 用于处理文件操作
        /// </summary>
        public class File_I
        {
            /// <summary>
            /// 定义了文件的厘性选项，包括只读、系统、隐藏和归档。
            /// </summary>
            public enum Properties
            {
                /// <summary>
                /// 只读属性
                /// </summary>
                Readonly,
                /// <summary>
                /// 系统属性
                /// </summary>
                System,
                /// <summary>
                /// 隐藏属性
                /// </summary>
                Hidden,
                /// <summary>
                /// 归档属性
                /// </summary>
                Archive,
            }
            /// <summary>
            /// 用于设置文件的属性
            /// </summary>
            /// <param name="path"> 文件路径</param>
            /// <param name="key"> 属性选项</param>
            /// <param name="Enable"> 开关</param>
            public void FileProperties(string path, Properties key, bool Enable)
            {
                string arg;
                string Switch = Enable ? "+" : "-";
                if (key == Properties.Readonly) arg = $"{Switch}r";
                else if (key == Properties.System) arg = $"{Switch}s";
                else if (key == Properties.Hidden) arg = $"{Switch}h";
                else if (key == Properties.Archive) arg = $"{Switch}a";
                else
                {
                    MessageBox.Show("Unsupported property type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    WriteLog(LogLevel.Error, "_UNSUPPORT_PROPERTY_TYPE");
                    return;
                }
                Process process = new Process();
                process.StartInfo.FileName = "attrib";
                process.StartInfo.Arguments = $"{arg} {path}";
                process.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process.Id}");
                process.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process.ExitCode}");
                process.Close();
            }
            /// <summary>
            /// 用于检查文件的哈希值是否与预期的哈希值匹配
            /// </summary>
            /// <param name="filePath"> 文件路径</param>
            /// <param name="expectedMD5"> 预期的MD5哈希值</param>
            /// <returns> 如果哈希值匹配，则返回 <see langword="true"/>，否则返回 <see langword="false"/></returns>
            public static bool CheckFileHash(string filePath, string expectedMD5)
            {
                try
                {
                    // 检查文件是否存在
                    if (!System.IO.File.Exists(filePath))
                    {
                        MessageBox.Show($"文件 {filePath} 不存在。");
                        return false;
                    }

                    // 计算文件的MD5哈希值
                    string actualMD5 = CalculateMD5(filePath);

                    // 检查哈希值是否匹配
                    if (actualMD5 != expectedMD5)
                    {
                        MessageBox.Show($"文件 {filePath} 的MD5哈希值不匹配。\n预期: {expectedMD5}\n实际: {actualMD5}");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"检查文件 {filePath} 时发生错误: {ex.Message}");
                    return false;
                }
            }
            /// <summary>
            /// 计算文件的MD5哈希值
            /// </summary>
            /// <param name="filePath">文件路径</param>
            /// <returns>MD5哈希值</returns>
            public static string CalculateMD5(string filePath)
            {
                using (MD5 md5 = MD5.Create())
                {
                    using (FileStream stream = System.IO.File.OpenRead(filePath))
                    {
                        byte[] hashBytes = md5.ComputeHash(stream);
                        return BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
                    }
                }
            }
        }
    }
}
