using System;
using System.Windows.Forms;

namespace Rox.Runtimes
{
    public partial class File_I
    {

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
    }
}