using System;
using System.Text;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
using Convert = System.Convert;
namespace Rox
{
    namespace Text
    {
        /// <summary>
        /// 用于读取和写入配置文件的类，提供了静态方法 <see cref="ReadConfig"/> 和 <see cref="WriteConfig"/> 来处理指定路径的配置文件
        /// </summary>
        public class Config
        {
            /// <summary>
            /// 用于从指定的INI文件中读取与给定头文本匹配的配置值,
            /// </summary>
            /// <param name="iniPath"> INI文件路径</param>
            /// <param name="HeadText"> 头文本</param>
            /// <returns> 返回与头文本匹配的配置值</returns>    
            public static string ReadConfig(string iniPath, string HeadText)
            {
                foreach (string Text in System.IO.File.ReadAllLines(iniPath))
                    if (Text.Contains(HeadText))
                        return Text.Split('=')[1].Trim();
                return null;
            }
            /// <summary>
            /// 用于将指定的值写入指定的INI文件中，如果文件不存在，则创建文件并写入头部和值
            /// </summary>
            /// <param name="iniPath"> INI文件路径</param>
            /// <param name="HeadText"> 头文本</param>
            /// <param name="Value"> 值</param>
            public static void WriteConfig(string iniPath, string HeadText, string Value)
            {
                // 如果文件不存在，创建文件并写入头部和值
                if (!System.IO.File.Exists(iniPath))
                {
                    System.IO.File.Create(iniPath).Close();
                    return;
                }

                // 读取文件的所有行
                string[] lines = System.IO.File.ReadAllLines(iniPath);
                WriteLog.Info($"{_READ_FILE}: {iniPath}");
                bool found = false;

                // 遍历每一行，查找是否有匹配的头部
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith(HeadText))
                    {
                        // 如果找到匹配的头部，替换该行的值\
                        WriteLog.Info($"{_UPDATE_LINE}: {HeadText} = {Value}");
                        lines[i] = $"{HeadText}={Value}";
                        found = true;
                        break;
                    }
                }

                // 如果没有找到匹配的头部，追加一行
                if (!found)
                {
                    WriteLog.Info($"{_ADD_NEW_LINE}: {HeadText} = {Value}");
                    Array.Resize(ref lines, lines.Length + 1);
                    lines[lines.Length - 1] = $"{HeadText}={Value}";
                }

                // 将修改后的内容写回文件
                System.IO.File.WriteAllLines(iniPath, lines);
                WriteLog.Info($"{_WRITE_FILE}: {iniPath}");
            }
        }
        /// <summary>
        /// 用于处理字符串的类
        /// </summary>
        public class String_I
        {
            /// <summary>
            /// 字符串转Base64
            /// </summary>
            /// <param name="str"> 字符串</param>
            /// <returns>返回Base64字符串</returns>
            public static string ToBase64(string str) => Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
            /// <summary>
            /// 字符串转16进制字符
            /// </summary>
            /// <param name="_str">字符串</param>
            /// <param name="encode">编码格式</param>
            /// <returns> 返回16进制字符串</returns>   
            public static string ToHexString(string _str, Encoding encode)
            {
                StringBuilder result = new StringBuilder();
                foreach (byte b in encode.GetBytes(_str))
                    // 格式化每个字节为2位十六进制（补前导零）
                    result.AppendFormat("{0:x2}", b);
                return result.ToString();
            }
        }
    }
}