using Rox.Runtimes.Properties;
using System;
using System.Resources;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    namespace Runtimes
    {
        /// <summary>
        /// 根据语言获取资源文件
        /// </summary>
        public class ResourceHelper
        {
            /// <summary>
            /// 判断是否为简体中文
            /// </summary>
            /// <param name="lang"></param>
            /// <returns>语言字符串</returns>
            private static bool IsChineseSimple(string lang) => lang == "zh-CN" || lang == "zh-CHS";
            /// <summary>
            /// 获取资源管理器
            /// </summary>
            /// <param name="lang"></param>
            /// <returns>指定语言文件的资源管理器</returns>
            private static ResourceManager GetResourceManager(string lang) => new ResourceManager($"Rox.Runtimes.Properties.Resource{(IsChineseSimple(lang) ? "s" : "1")}", typeof(Resource1).Assembly);
            /// <summary>
            /// 获取字符串资源
            /// </summary>
            /// <param name="key">自定义字段</param>
            /// <param name="lang">语言代码</param>
            /// <returns>指定语言文件中的字符串</returns>
            internal static string GetString(string key, string lang)
            {
                try
                {
                    return GetResourceManager(lang).GetString(key);
                }
                catch (Exception ex)
                {
                    WriteLog.Error("ResourceManager", $"资源键 '{key}' 未找到，语言: {lang}");
                    WriteLog.Error("ResourceManager", _Exception_With_xKind("ResourceHelper.GetString", ex));
                    return null;
                }
            }
        }
    }
}
