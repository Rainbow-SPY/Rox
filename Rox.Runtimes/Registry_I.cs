using Microsoft.Win32;
using System;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    namespace Runtimes
    {
        /// <summary>
        /// 用于处理注册表操作
        /// </summary>
        public class Registry_I
        {
            /// <summary>
            /// 用于写入注册表项的值
            /// </summary>
            /// <param name="keyPath"> 注册表项路径</param>
            /// <param name="valueName"> 注册表项名称</param>
            /// <param name="valueType"> 注册表项类型</param>
            /// <param name="valueData"> 注册表项数据</param>
            public static void Write(string keyPath, string valueName, object valueData, RegistryValueKind valueType)
            {
                try
                {
                    // 打开注册表项
                    using (RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(keyPath))
                    {
                        // 写入值
                        WriteLog.Info($"{_WRITE_REGISTRY}");
                        key.SetValue(valueName, valueData, valueType);
                        key.Close();
                    }
                    LogLibraries.WriteLog.Info($"{_SUCESS_WRITE_REGISTRY}");
                }
                catch (Exception ex)
                {
                    LogLibraries.WriteLog.Error($"{_WRITE_REGISTRY_FAILED}: {ex.Message}");
                }
            }
            /// <summary>
            /// 用于读取注册表项的值
            /// </summary>
            /// <param name="keyName"> 注册表项路径</param>
            /// <param name="valueName"> 注册表项名称</param>
            /// <returns> 返回注册表项的值</returns>
            internal static string GetRegistryValue(string keyName, string valueName)
            {
                string value = "";
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(keyName))
                {
                    if (key != null)
                    {
                        value = key.GetValue(valueName) as string;
                        key.Close();
                    }
                }
                return value;
            }
        }
    }
}
