using System.Text;

namespace Rox
{
    /// <summary>
    /// 提供API查询
    /// </summary>
    public partial class API
    {
        /// <summary>
        /// 压缩 <see cref="Text.Json"/> 字符串
        /// </summary>
        /// <param name="json"> <see cref="Text.Json"/> 字符串</param>
        /// <returns> 压缩后的 <see cref="Text.Json"/> 字符串</returns>
        public static string CompressJson(string json)
        {
            var result = new StringBuilder();
            bool inString = false; // 是否在字符串内
            bool skipWhitespace = false; // 是否跳过空白字符

            for (int i = 0; i < json.Length; i++)
            {
                char currentChar = json[i];

                // 处理引号
                if (currentChar == '"')
                {
                    inString = !inString; // 切换字符串状态
                    result.Append(currentChar);
                    continue;
                }

                // 在字符串内，直接追加字符
                if (inString)
                {
                    result.Append(currentChar);
                    continue;
                }

                // 处理空白字符
                if (char.IsWhiteSpace(currentChar))
                {
                    // 如果当前字符是空格，且需要跳过空白字符，则跳过
                    if (skipWhitespace)
                    {
                        continue;
                    }

                    // 检查是否需要保留空格（如日期时间字段）
                    if (IsDateTimeField(json, i))
                    {
                        result.Append(currentChar);
                        skipWhitespace = true; // 跳过后续空白字符
                        continue;
                    }

                    // 否则，跳过空白字符
                    continue;
                }

                // 追加非空白字符
                result.Append(currentChar);
                skipWhitespace = false; // 重置跳过空白字符的标志
            }

            return result.ToString();
        }

        // 检查当前字符是否属于日期时间字段
        /// <summary>
        /// 检查当前字符是否属于日期时间字段
        /// </summary>
        /// <param name="json"> <see cref="Text.Json"/> 字符串</param>
        /// <param name="index"> 当前字符索引</param>
        /// <returns> 是否在日期时间字段中</returns>
        private static bool IsDateTimeField(string json, int index)
        {
            // 检查当前字符是否在日期时间字段中
            // 例如："accountcreationdate":"2022-10-23 20:23:58"
            string[] dateTimeFields = { "accountcreationdate", "lastlogoff" };

            foreach (var field in dateTimeFields)
            {
                // 检查字段名是否出现在当前位置之前
                if (index >= field.Length && json.Substring(index - field.Length, field.Length) == field)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
