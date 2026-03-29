using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
using Convert = System.Convert;

namespace Rox
{
    namespace Text
    {
        /// <summary>
        /// 用于处理 <see cref="Json"/> 格式的文本
        /// </summary>
        public class Json
        {
            /// <summary>
            /// 压缩 <see cref="Text.Json"/> 字符串
            /// </summary>
            /// <param name="json"> <see cref="Text.Json"/> 字符串</param>
            /// <returns> 压缩后的 <see cref="Text.Json"/> 字符串</returns>
            public static string CompressJson(string json)
            {
                var result = new StringBuilder();
                var inString = false; // 是否在字符串内
                var skipWhitespace = false; // 是否跳过空白字符

                for (var i = 0; i < json.Length; i++)
                {
                    var currentChar = json[i];

                    if (currentChar == '"')
                    {
                        inString = !inString;
                        result.Append(currentChar);
                        continue;
                    }

                    if (inString)
                    {
                        result.Append(currentChar);
                        continue;
                    }

                    if (char.IsWhiteSpace(currentChar))
                    {
                        if (skipWhitespace)
                            continue;
                        if (IsDateTimeField(json, i))
                        {
                            result.Append(currentChar);
                            skipWhitespace = true;
                        }

                        continue;
                    }

                    result.Append(currentChar);
                    skipWhitespace = false;
                }

                return result.ToString();
            }

            /// <summary>
            /// 检查当前字符是否属于日期时间字段
            /// </summary>
            /// <param name="json"> <see cref="Text.Json"/> 字符串</param>
            /// <param name="index"> 当前字符索引</param>
            /// <returns> 是否在日期时间字段中</returns>
            private static bool IsDateTimeField(string json, int index) =>
                new[] { "accountcreationdate", "lastlogoff" }.Any(field =>
                    index >= field.Length && json.Substring(index - field.Length, field.Length) == field);

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
                    get => _properties.TryGetValue(key, out var item) ? item : null;
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

                    if (json == "{}")
                        return result;

                    if (!json.StartsWith("{") || !json.EndsWith("}"))
                        throw new FormatException("Invalid JSON object format.");

                    json = json.Substring(1, json.Length - 2).Trim();

                    if (string.IsNullOrEmpty(json))
                    {
                        WriteLog.Error(_value_Not_Is_NullOrEmpty("json"));
                        return result;
                    }

                    foreach (var pair in SplitJson(json, ','))
                    {
                        var keyValue = SplitJson(pair, ':');
                        if (keyValue.Length != 2)
                            throw new FormatException("Invalid JSON object format.");

                        var key = keyValue[0].Trim().Trim('"');
                        var value = keyValue[1].Trim();

                        if (value.StartsWith("{") && value.EndsWith("}"))
                            result[key] = Parse(value);
                        else if (value.StartsWith("[") && value.EndsWith("]"))
                            result[key] = ParseArray(value);
                        else if (value.StartsWith("\"") && value.EndsWith("\""))
                            result[key] = value.Trim('"');
                        else if (value == "true" || value == "false")
                            result[key] = bool.Parse(value);
                        else if (Regex.IsMatch(value, @"^-?\d+(\.\d+)?$"))
                            result[key] = ParseNumber(value);
                        else
                            result[key] = value == "null"
                                ? (object)null
                                : throw new NotSupportedException($"Unsupported JSON value: {value}");
                    }

                    return result;
                }

                /// <summary>
                /// 检查是否含有指定键
                /// </summary>
                /// <param name="key"></param>
                /// <returns></returns>
                public bool ContainsKey(string key) => _properties.ContainsKey(key);

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
                        WriteLog.Error(_value_Not_Is_NullOrEmpty("json"));
                        return result;
                    }

                    foreach (var item in SplitJson(json, ','))
                    {
                        if (item.StartsWith("{") && item.EndsWith("}"))
                            result.Add(Parse(item));
                        else if (item.StartsWith("[") && item.EndsWith("]"))
                            result.Add(ParseArray(item));
                        else if (item.StartsWith("\"") && item.EndsWith("\""))
                            result.Add(item.Trim('"'));
                        else if (item == "true" || item == "false")
                            result.Add(bool.Parse(item));
                        else if (Regex.IsMatch(item, @"^-?\d+(\.\d+)?$"))
                            result.Add(ParseNumber(item));
                        else if (item == "null")
                            result.Add(null);
                        else
                            throw new NotSupportedException($"Unsupported JSON value: {item}");
                    }

                    return result;
                }
            }

            /// <summary>
            /// 用于将 <see cref="Json"/> 字符串反序列化为指定类型的对象
            /// </summary>
            /// <typeparam name="T"> 对象类型</typeparam>
            /// <param name="json"> <see cref="Json"/>字符串</param>
            /// <returns>返回对象</returns>
            public static T DeserializeObject<T>(string json) where T : new() =>
                MapJsonToObject<T>(JObject.Parse(json));

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

                var type = obj.GetType();

                if (type == typeof(string) || type == typeof(char))
                    return $"\"{EscapeString(obj.ToString())}\"";

                if (type.IsPrimitive || type == typeof(decimal))
                    return obj.ToString().ToLower();

                if (type.IsEnum)
                    return $"\"{obj}\"";

                if (type.IsArray || (type.IsGenericType &&
                                     typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
                {
                    var sb = new StringBuilder("[");
                    foreach (var item in (IEnumerable)obj)
                        sb.Append(SerializeObject(item).Replace("\n", "")
                            .Replace("\r", "")); // 去除换行符                        first = false;

                    sb.Append("]");
                    return sb.ToString();
                }

                if (!type.IsClass && !type.IsValueType)
                    throw new NotSupportedException($"Type {type.Name} is not supported for serialization.");
                var sb1 = new StringBuilder("{");
                var first1 = true;
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!first1)
                        sb1.Append(",");
                    sb1.Append(
                        $"\"{property.Name}\":{SerializeObject(property.GetValue(obj)).Replace("\n", "").Replace("\r", "")}"); // 去除换行符
                    first1 = false;
                }

                sb1.Append("}");
                return sb1.ToString();
            }

            /// <summary>
            /// 用于将字符串中的特殊字符转换为 <see cref="Json"/> 格式的转义字符,
            /// </summary>
            /// <param name="str"> 字符串</param>
            /// <returns>  返回转义后的字符串</returns>
            private static string EscapeString(string str) => str.Replace("\\", @"\\")
                .Replace("\"", "\\\"")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");

            /// <summary>
            /// 用于将 <see cref="Json"/> 字符串反序列化为动态对象
            /// </summary>
            /// <param name="json"> <see cref="Json"/> 字符串</param>
            /// <returns> 返回动态对象</returns>
            public static dynamic DeserializeObject(string json) => ParseJson(json);

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
                    throw new FormatException("Invalid JSON object format.");

                var result = new Dictionary<string, object>();
                json = json.Substring(1, json.Length - 2).Trim();

                if (string.IsNullOrEmpty(json))
                {
                    WriteLog.Error(_value_Not_Is_NullOrEmpty("json"));
                    return result;
                }

                foreach (var pair in SplitJson(json, ','))
                {
                    var keyValue = SplitJson(pair, ':');
                    if (keyValue.Length != 2)
                        throw new FormatException("Invalid JSON object format.");

                    var key = keyValue[0].Trim().Trim('"');
                    var value = keyValue[1].Trim();
                    if (value.StartsWith("{") && value.EndsWith("}"))
                        result[key] = ParseJson(value);
                    else if (value.StartsWith("[") && value.EndsWith("]"))
                        result[key] = ParseJsonArray(value);
                    else if (value.StartsWith("\"") && value.EndsWith("\""))
                        result[key] = value.Trim('"');
                    else if (value == "true" || value == "false")
                        result[key] = bool.Parse(value);
                    else if (Regex.IsMatch(value, @"^-?\d+(\.\d+)?$"))
                        result[key] = ParseNumber(value);
                    else if (value == "null")
                        result[key] = null; // null
                    else
                        throw new NotSupportedException($"Unsupported JSON value: {value}");
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
                    WriteLog.Error(_value_Not_Is_NullOrEmpty("json"));
                    return result;
                }

                foreach (var item in SplitJson(json, ','))
                    if (item.StartsWith("{") && item.EndsWith("}"))
                        result.Add(ParseJson(item));
                    else if (item.StartsWith("[") && item.EndsWith("]"))
                        result.Add(ParseJsonArray(item));
                    else if (item.StartsWith("\"") && item.EndsWith("\""))
                        result.Add(item.Trim('"'));
                    else if (item == "true" || item == "false")
                        result.Add(bool.Parse(item));
                    else if (Regex.IsMatch(item, @"^-?\d+(\.\d+)?$"))
                        result.Add(ParseNumber(item));
                    else if (item == "null")
                        result.Add(null);
                    else
                        throw new NotSupportedException($"Unsupported JSON value: {item}");

                return result;
            }

            /// <summary>
            /// 将 <see cref="Json"/>  数字字符串解析为数字
            /// </summary>
            /// <param name="value"> <see cref="Json"/> 数字字符串</param>
            /// <returns> 返回数字</returns>
            private static object ParseNumber(string value) =>
                value.Contains(".") ? double.Parse(value) : (object)long.Parse(value);

            /// <summary>
            /// 将 <see cref="Json"/> 字符串分割为数组
            /// </summary>
            /// <param name="json"> <see cref="Json"/> 字符串</param>
            /// <param name="separator"> 分隔符</param>
            /// <returns> 返回数组</returns>
            private static string[] SplitJson(string json, char separator)
            {
                var result = new List<string>();
                var depth = 0;
                var start = 0;

                for (var i = 0; i < json.Length; i++)
                {
                    var c = json[i];
                    switch (c)
                    {
                        case '{':
                        case '[':
                            depth++; // 进入嵌套对象或数组
                            break;
                        case '}':
                        case ']':
                            depth--; // 退出嵌套对象或数组
                            break;
                        default:
                        {
                            if (c == separator && depth == 0)
                            {
                                // 只有在最外层时才分割
                                result.Add(json.Substring(start, i - start).Trim());
                                start = i + 1;
                            }

                            break;
                        }
                    }
                }

                if (start < json.Length)
                    result.Add(json.Substring(start).Trim());

                return result.ToArray();
            }

            /// <summary>
            /// 将 <see cref="Json"/> 字符串转换为指定类型的对象
            /// </summary>
            /// <typeparam name="T"> 对象类型</typeparam>
            /// <param name="jsonObject"> <see cref="Json"/> 对象</param>
            /// <returns> 返回对象</returns>
            private static T MapJsonToObject<T>(JObject jsonObject) where T : new()
            {
                var result = new T();

                foreach (var property in typeof(T).GetProperties())
                    if (jsonObject.ContainsKey(property.Name))
                    {
                        var value = jsonObject[property.Name];
                        if (value != null)
                            property.SetValue(result, Convert.ChangeType(value, property.PropertyType));
                    }

                return result;
            }
        }
    }
}