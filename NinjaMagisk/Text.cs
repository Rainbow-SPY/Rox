using NinjaMagisk.Runtimes;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static NinjaMagisk.Runtimes.LocalizedString;
using static NinjaMagisk.Runtimes.LogLibraries;
namespace NinjaMagisk
{
    /// <summary>
    /// 用于文本处理
    /// </summary>
    public class Text
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
                string[] Texts = System.IO.File.ReadAllLines(iniPath);
                foreach (string Text in Texts)
                {
                    if (Text.Contains(HeadText))
                    {
                        string[] part = Text.Split('=');
                        return part[1].Trim();
                    }
                }
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
                WriteLog(LogLevel.Info, $"{_READ_FILE}: {iniPath}");
                bool found = false;

                // 遍历每一行，查找是否有匹配的头部
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith(HeadText))
                    {
                        // 如果找到匹配的头部，替换该行的值\
                        WriteLog(LogLevel.Info, $"{_UPDATE_LINE}: {HeadText} = {Value}");
                        lines[i] = $"{HeadText}={Value}";
                        found = true;
                        break;
                    }
                }

                // 如果没有找到匹配的头部，追加一行
                if (!found)
                {
                    WriteLog(LogLevel.Info, $"{_ADD_NEW_LINE}: {HeadText} = {Value}");
                    Array.Resize(ref lines, lines.Length + 1);
                    lines[lines.Length - 1] = $"{HeadText}={Value}";
                }

                // 将修改后的内容写回文件
                System.IO.File.WriteAllLines(iniPath, lines);
                WriteLog(LogLevel.Info, $"{_WRITE_FILE}: {iniPath}");
            }
        }
        /// <summary>
        /// 用于处理 <see cref="Json"/> 格式的文本
        /// </summary>
        public class Json
        {
            /// <summary>
            /// 表示和操作 <see cref="Json"/> 对象的动态类
            /// </summary>
            public class JObject : DynamicObject
            {
                /// <summary>
                /// 用于存储JSON对象的属性
                /// </summary>
                private readonly Dictionary<string, object> _properties;
                /// <summary>
                /// 初始化一个新的JSON对象,并创造一个空的属性字典
                /// </summary>
                public JObject()
                {
                    _properties = new Dictionary<string, object>();
                }
                /// <summary>
                /// 获取或设置指定键的属性
                /// </summary>
                /// <param name="key"> 键</param>
                /// <returns> 返回属性值</returns>
                public object this[string key]
                {
                    get
                    {
                        if (_properties.ContainsKey(key))
                        {
                            return _properties[key];
                        }
                        return null;
                    }
                    set => _properties[key] = value;
                }
                /// <summary>
                /// 用于将字符串格式的 <see cref="Json"/> 对象解析为 <see cref="JObject"/> 实例
                /// </summary>
                /// <param name="json"> JSON对象</param>
                /// <returns></returns>
                /// <exception cref="FormatException"> 无效的JSON对象格式</exception>
                /// <exception cref="NotSupportedException"> 不支持的JSON值</exception>
                public static JObject Parse(string json)
                {
                    var result = new JObject();
                    json = json.Trim();

                    // 允许空对象
                    if (json == "{}")
                    {
                        return result;
                    }

                    // 检查是否以 { 开头和 } 结尾
                    if (!json.StartsWith("{") || !json.EndsWith("}"))
                    {
                        throw new FormatException("Invalid JSON object format.");
                    }

                    // 去掉最外层的 { 和 }
                    json = json.Substring(1, json.Length - 2).Trim();

                    if (string.IsNullOrEmpty(json))
                    {
                        return result;
                    }

                    // 使用修复后的 SplitJson 方法分割键值对
                    var pairs = SplitJson(json, ',');
                    foreach (var pair in pairs)
                    {
                        var keyValue = SplitJson(pair, ':');
                        if (keyValue.Length != 2)
                        {
                            throw new FormatException("Invalid JSON object format.");
                        }

                        string key = keyValue[0].Trim().Trim('"');
                        string value = keyValue[1].Trim();

                        // 处理嵌套对象
                        if (value.StartsWith("{") && value.EndsWith("}"))
                        {
                            result[key] = Parse(value); // 递归解析嵌套对象
                        }
                        // 处理数组
                        else if (value.StartsWith("[") && value.EndsWith("]"))
                        {
                            result[key] = ParseArray(value); // 解析数组
                        }
                        // 处理字符串
                        else if (value.StartsWith("\"") && value.EndsWith("\""))
                        {
                            result[key] = value.Trim('"'); // 去掉引号
                        }
                        // 处理布尔值
                        else if (value == "true" || value == "false")
                        {
                            result[key] = bool.Parse(value);
                        }
                        // 处理数字
                        else if (Regex.IsMatch(value, @"^-?\d+(\.\d+)?$"))
                        {
                            result[key] = ParseNumber(value);
                        }
                        // 处理 null
                        else if (value == "null")
                        {
                            result[key] = null;
                        }
                        // 其他情况
                        else
                        {
                            throw new NotSupportedException($"Unsupported JSON value: {value}");
                        }
                    }

                    return result;
                }
                // 添加 ContainsKey 方法
                /// <summary>
                /// 检查是否含有指定键
                /// </summary>
                /// <param name="key"></param>
                /// <returns></returns>
                public bool ContainsKey(string key)
                {
                    return _properties.ContainsKey(key);
                }
                /// <summary>
                /// 用于解析 <see cref="Json"/> 字符串并将其转换为对象列表，支持嵌套对象和数组的处理。
                /// </summary>
                /// <param name="json"> JSON数组</param>
                /// <returns> 返回JSON数组</returns>
                /// <exception cref="NotSupportedException"> 不支持的JSON值</exception>
                private static List<object> ParseArray(string json)
                {
                    var result = new List<object>();
                    json = json.Substring(1, json.Length - 2).Trim();

                    if (string.IsNullOrEmpty(json))
                    {
                        return result;
                    }

                    var items = SplitJson(json, ',');
                    foreach (var item in items)
                    {
                        if (item.StartsWith("{") && item.EndsWith("}"))
                        {
                            result.Add(Parse(item)); // 嵌套对象
                        }
                        else if (item.StartsWith("[") && item.EndsWith("]"))
                        {
                            result.Add(ParseArray(item)); // 嵌套数组
                        }
                        else if (item.StartsWith("\"") && item.EndsWith("\""))
                        {
                            result.Add(item.Trim('"')); // 字符串
                        }
                        else if (item == "true" || item == "false")
                        {
                            result.Add(bool.Parse(item)); // 布尔值
                        }
                        else if (Regex.IsMatch(item, @"^-?\d+(\.\d+)?$"))
                        {
                            result.Add(ParseNumber(item)); // 数字
                        }
                        else if (item == "null")
                        {
                            result.Add(null); // null
                        }
                        else
                        {
                            throw new NotSupportedException($"Unsupported JSON value: {item}");
                        }
                    }

                    return result;
                }
                /// <summary>
                /// 将<see cref="Json"/>数字字符串解析为数字
                /// </summary>
                /// <param name="value"> <see cref="Json"/>数字字符串</param>
                /// <returns> 返回数字</returns>
                private static object ParseNumber(string value)
                {
                    if (value.Contains("."))
                    {
                        return double.Parse(value);
                    }
                    else
                    {
                        return long.Parse(value);
                    }
                }
                /// <summary>
                /// 将<see cref="Json"/>字符串分割为数组
                /// </summary>
                /// <param name="json"> <see cref="Json"/>字符串</param>
                /// <param name="separator"> 分隔符</param>
                /// <returns> 返回数组</returns>
                private static string[] SplitJson(string json, char separator)
                {
                    var result = new List<string>();
                    int depth = 0; // 用于跟踪嵌套层级
                    int start = 0;
                    bool inString = false; // 是否在字符串内

                    for (int i = 0; i < json.Length; i++)
                    {
                        char currentChar = json[i];

                        // 处理字符串
                        if (currentChar == '"')
                        {
                            inString = !inString; // 切换字符串状态
                        }

                        // 处理嵌套对象和数组
                        if (!inString)
                        {
                            if (currentChar == '{' || currentChar == '[')
                            {
                                depth++; // 进入嵌套对象或数组
                            }
                            else if (currentChar == '}' || currentChar == ']')
                            {
                                depth--; // 退出嵌套对象或数组
                            }
                        }

                        // 处理分隔符
                        if (currentChar == separator && depth == 0 && !inString)
                        {
                            // 只有在最外层且不在字符串内时才分割
                            result.Add(json.Substring(start, i - start).Trim());
                            start = i + 1;
                        }
                    }

                    // 添加最后一段
                    if (start < json.Length)
                    {
                        result.Add(json.Substring(start).Trim());
                    }

                    return result.ToArray();
                }
            }
            /// <summary>
            /// 用于将 <see cref="Json"/> 字符串反序列化为指定类型的对象
            /// </summary>
            /// <typeparam name="T"> 对象类型</typeparam>
            /// <param name="json"> <see cref="Json"/>字符串</param>
            /// <returns>   返回对象</returns>
            // 支持泛型反序列化
            public static T DeserializeObject<T>(string json) where T : new()
            {
                var jsonObject = Json.JObject.Parse(json);
                return MapJsonToObject<T>(jsonObject);
            }
            /// <summary>
            /// 用于将给定的对象序列化为 <see cref="Json"/> 字符串
            /// </summary>
            /// <param name="obj"> 对象</param>
            /// <returns> 返回 <see cref="Json"/> 字符串</returns>
            /// <exception cref="NotSupportedException"> 不支持的JSON值</exception>
            public static string SerializeObject(object obj)
            {
                if (obj == null)
                    return "null";

                Type type = obj.GetType();

                if (type == typeof(string) || type == typeof(char))
                    return $"\"{EscapeString(obj.ToString())}\"";

                if (type.IsPrimitive || type == typeof(decimal))
                    return obj.ToString().ToLower(); // 处理数字和布尔值

                if (type.IsEnum)
                    return $"\"{obj}\"";

                if (type.IsArray || (type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
                {
                    var sb = new StringBuilder("[");
                    var enumerable = obj as System.Collections.IEnumerable;
                    bool first = true;
                    foreach (var item in enumerable)
                    {
                        if (!first)
                            sb.Append(",");
                        sb.Append(SerializeObject(item).Replace("\n", "").Replace("\r", "")); // 去除换行符                        first = false;
                    }
                    sb.Append("]");
                    return sb.ToString();
                }

                if (type.IsClass || type.IsValueType)
                {
                    var sb = new StringBuilder("{");
                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    bool first = true;
                    foreach (var property in properties)
                    {
                        if (!first)
                            sb.Append(",");
                        sb.Append($"\"{property.Name}\":{SerializeObject(property.GetValue(obj)).Replace("\n", "").Replace("\r", "")}"); // 去除换行符
                        first = false;
                    }
                    sb.Append("}");
                    return sb.ToString();
                }

                throw new NotSupportedException($"Type {type.Name} is not supported for serialization.");
            }
            /// <summary>
            /// 用于将字符串中的特殊字符转换为 <see cref="Json"/> 格式的转义字符,
            /// </summary>
            /// <param name="str"> 字符串</param>
            /// <returns>  返回转义后的字符串</returns>
            private static string EscapeString(string str)
            {
                return str.Replace("\\", "\\\\")
                          .Replace("\"", "\\\"")
                          .Replace("\b", "\\b")
                          .Replace("\f", "\\f")
                          .Replace("\n", "\\n")
                          .Replace("\r", "\\r")
                          .Replace("\t", "\\t");
            }

            // 支持非泛型反序列化（返回 dynamic）
            /// <summary>
            /// 用于将 <see cref="Json"/> 字符串反序列化为动态对象
            /// </summary>
            /// <param name="json"> <see cref="Json"/> 字符串</param>
            /// <returns> 返回动态对象</returns>
            public static dynamic DeserializeObject(string json)
            {
                return ParseJson(json);
            }
            /// <summary>
            /// 用于将字符串格式的 <see cref="Json"/> 对象解析为 <see cref="JObject"/> 实例
            /// </summary>
            /// <param name="json"> <see cref="Json"/> 对象</param>
            /// <returns> 返回 <see cref="JObject"/> 实例</returns>
            /// <exception cref="FormatException"></exception>
            /// <exception cref="NotSupportedException"></exception>
            private static Dictionary<string, object> ParseJson(string json)
            {
                json = json.Trim();
                if (!json.StartsWith("{") || !json.EndsWith("}"))
                {
                    throw new FormatException("Invalid JSON object format.");
                }

                var result = new Dictionary<string, object>();
                json = json.Substring(1, json.Length - 2).Trim();

                if (string.IsNullOrEmpty(json))
                {
                    return result;
                }

                var pairs = SplitJson(json, ',');
                foreach (var pair in pairs)
                {
                    var keyValue = SplitJson(pair, ':');
                    if (keyValue.Length != 2)
                    {
                        throw new FormatException("Invalid JSON object format.");
                    }

                    string key = keyValue[0].Trim().Trim('"');
                    string value = keyValue[1].Trim();

                    if (value.StartsWith("{") && value.EndsWith("}"))
                    {
                        result[key] = ParseJson(value); // 嵌套对象
                    }
                    else if (value.StartsWith("[") && value.EndsWith("]"))
                    {
                        result[key] = ParseJsonArray(value); // 数组
                    }
                    else if (value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        result[key] = value.Trim('"'); // 字符串
                    }
                    else if (value == "true" || value == "false")
                    {
                        result[key] = bool.Parse(value); // 布尔值
                    }
                    else if (Regex.IsMatch(value, @"^-?\d+(\.\d+)?$"))
                    {
                        result[key] = ParseNumber(value); // 数字
                    }
                    else if (value == "null")
                    {
                        result[key] = null; // null
                    }
                    else
                    {
                        throw new NotSupportedException($"Unsupported JSON value: {value}");
                    }
                }

                return result;
            }
            /// <summary>
            /// 用于解析 <see cref="Json"/> 字符串并将其转换为对象列表，支持嵌套对象和数组的处理。
            /// </summary>
            /// <param name="json"> <see cref="Json"/> 字符串</param>
            /// <returns> 返回对象列表</returns>
            /// <exception cref="NotSupportedException"> 不支持的JSON值</exception>
            private static List<object> ParseJsonArray(string json)
            {
                var result = new List<object>();
                json = json.Substring(1, json.Length - 2).Trim();

                if (string.IsNullOrEmpty(json))
                {
                    return result;
                }

                var items = SplitJson(json, ',');
                foreach (var item in items)
                {
                    if (item.StartsWith("{") && item.EndsWith("}"))
                    {
                        result.Add(ParseJson(item)); // 嵌套对象
                    }
                    else if (item.StartsWith("[") && item.EndsWith("]"))
                    {
                        result.Add(ParseJsonArray(item)); // 嵌套数组
                    }
                    else if (item.StartsWith("\"") && item.EndsWith("\""))
                    {
                        result.Add(item.Trim('"')); // 字符串
                    }
                    else if (item == "true" || item == "false")
                    {
                        result.Add(bool.Parse(item)); // 布尔值
                    }
                    else if (Regex.IsMatch(item, @"^-?\d+(\.\d+)?$"))
                    {
                        result.Add(ParseNumber(item)); // 数字
                    }
                    else if (item == "null")
                    {
                        result.Add(null); // null
                    }
                    else
                    {
                        throw new NotSupportedException($"Unsupported JSON value: {item}");
                    }
                }

                return result;
            }
            /// <summary>
            /// 将 <see cref="Json"/>  数字字符串解析为数字
            /// </summary>
            /// <param name="value"> <see cref="Json"/> 数字字符串</param>
            /// <returns> 返回数字</returns>
            private static object ParseNumber(string value)
            {
                if (value.Contains("."))
                {
                    return double.Parse(value);
                }
                else
                {
                    return long.Parse(value);
                }
            }
            /// <summary>
            /// 将 <see cref="Json"/> 字符串分割为数组
            /// </summary>
            /// <param name="json"> <see cref="Json"/> 字符串</param>
            /// <param name="separator"> 分隔符</param>
            /// <returns> 返回数组</returns>
            private static string[] SplitJson(string json, char separator)
            {
                var result = new List<string>();
                int depth = 0;
                int start = 0;

                for (int i = 0; i < json.Length; i++)
                {
                    char c = json[i];
                    if (c == '{' || c == '[')
                    {
                        depth++; // 进入嵌套对象或数组
                    }
                    else if (c == '}' || c == ']')
                    {
                        depth--; // 退出嵌套对象或数组
                    }
                    else if (c == separator && depth == 0)
                    {
                        // 只有在最外层时才分割
                        result.Add(json.Substring(start, i - start).Trim());
                        start = i + 1;
                    }
                }

                // 添加最后一段
                if (start < json.Length)
                {
                    result.Add(json.Substring(start).Trim());
                }

                return result.ToArray();
            }
            /// <summary>
            /// 将 <see cref="Json"/> 字符串转换为指定类型的对象
            /// </summary>
            /// <typeparam name="T"> 对象类型</typeparam>
            /// <param name="jsonObject"> <see cref="Json"/> 对象</param>
            /// <returns> 返回对象</returns>
            private static T MapJsonToObject<T>(Text.Json.JObject jsonObject) where T : new()
            {
                var result = new T();
                var properties = typeof(T).GetProperties();

                foreach (var property in properties)
                {
                    if (jsonObject.ContainsKey(property.Name))
                    {
                        var value = jsonObject[property.Name];
                        if (value != null)
                        {
                            property.SetValue(result, Convert.ChangeType(value, property.PropertyType));
                        }
                    }
                }

                return result;
            }
            /// <summary>
            /// 用于将 <see cref="Json"/> 对象映射到指定类型的实例，支持根据性名称从字典中提取值并设置到相应的属性。
            /// </summary>
            /// <param name="type"> 类型</param>
            /// <param name="jsonObject"> <see cref="Json"/> 对象</param>
            /// <returns> 返回对象</returns>
            private static object MapJsonToObject(Type type, Dictionary<string, object> jsonObject)
            {
                var result = Activator.CreateInstance(type);
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var property in properties)
                {
                    if (jsonObject.ContainsKey(property.Name))
                    {
                        var value = jsonObject[property.Name];
                        if (value != null)
                        {
                            if (property.PropertyType == typeof(string))
                            {
                                property.SetValue(result, value.ToString());
                            }
                            else if (property.PropertyType == typeof(int))
                            {
                                property.SetValue(result, Convert.ToInt32(value));
                            }
                            else if (property.PropertyType == typeof(long))
                            {
                                property.SetValue(result, Convert.ToInt64(value));
                            }
                            else if (property.PropertyType == typeof(double))
                            {
                                property.SetValue(result, Convert.ToDouble(value));
                            }
                            else if (property.PropertyType == typeof(bool))
                            {
                                property.SetValue(result, Convert.ToBoolean(value));
                            }
                            else if (property.PropertyType.IsEnum)
                            {
                                property.SetValue(result, Enum.Parse(property.PropertyType, value.ToString()));
                            }
                            else if (property.PropertyType.IsClass || property.PropertyType.IsValueType)
                            {
                                var nestedObject = MapJsonToObject(property.PropertyType, value as Dictionary<string, object>);
                                property.SetValue(result, nestedObject);
                            }
                        }
                    }
                }

                return result;
            }
        }
    }
}