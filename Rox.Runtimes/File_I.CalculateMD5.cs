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
        public partial class File_I
        {
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
