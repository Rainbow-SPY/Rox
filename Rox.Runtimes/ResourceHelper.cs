using Rox.Runtimes.Properties;
using System.Resources;
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
            private static bool IsChineseSimple(string lang)
            {
                return lang == "zh-CN" || lang == "zh-CHS";
            }
            /// <summary>
            /// 获取资源管理器
            /// </summary>
            /// <param name="lang"></param>
            /// <returns>指定语言文件的资源管理器</returns>
            private static ResourceManager GetResourceManager(string lang)
            {
                if (IsChineseSimple(lang))
                {
                    // 如果是中文（zh-CN 或 zh-Hans），返回 Resources.resx 的资源管理器
                    return new ResourceManager("Rox.Runtimes.Properties.Resources", typeof(Resources).Assembly);
                }
                else
                {
                    // 如果是其他语言，返回 Resource1.resx 的资源管理器
                    return new ResourceManager("Rox.Runtimes.Properties.Resource1", typeof(Resource1).Assembly);
                }
            }
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
                    ResourceManager resourceManager = GetResourceManager(lang);
                    string value = resourceManager.GetString(key);
                    return value;
                }
                catch
                {
                    WriteLog.Error($"资源键 '{key}' 未找到，语言: {lang}");
                    WriteLog.Info($"Error:{key} ");
                    return null;
                }
            }
        }
    }
}
