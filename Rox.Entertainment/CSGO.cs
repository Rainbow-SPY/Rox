using Microsoft.Win32;

namespace Rox.Entertainment
{
    /// <summary>
    /// CSGO / CS2相关功能类。
    /// </summary>
    public partial class CSGO
    {
        /// <summary>
        /// 通过注册表 <see cref="Registry"/> 获取CS2的安装路径。
        /// </summary>
        /// <returns> CS2的安装路径，如果未找到则返回null。</returns>
        public static string GetCS2Path()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\cs2"))
                return key?.GetValue("installpath")?.ToString();
        }
        /// <summary>
        /// 获取CS2的配置文件夹路径。
        /// </summary>
        /// <returns> CS2的配置文件夹路径，如果未找到则返回null。</returns>
        public static string GetConfigPath() => GetCS2Path() + @"\game\csgo\cfg";
    }
}
