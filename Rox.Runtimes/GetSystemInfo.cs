using System.Globalization;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using static Rox.Runtimes.LogLibraries;

namespace Rox.Runtimes
{
    /// <summary>
    /// 获取系统信息
    /// </summary>
    public class GetSystemInfo
    {
        // 系统名称（类似systeminfo中的"OS名称"）
        /// <summary>
        /// 操作系统名称
        /// </summary>
        public static string OSName { get; set; }

        // 详细版本号（如26100.4770）
        /// <summary>
        /// 操作系统内部版本号
        /// </summary>
        public static string OSBuildNumber { get; set; }

        // 系统架构（x64/x86/ARM等）
        /// <summary>
        /// 操作系统架构
        /// </summary>
        public static string OSArchitecture { get; set; }

        // 系统语言（如zh-CN/en-US）
        /// <summary>
        /// 系统语言
        /// </summary>
        public static string SystemLanguage { get; set; }
        /// <summary>
        /// 处理器名称
        /// </summary>
        public static string ProcessorName { get; set; }
        /// <summary>
        /// 初始化所有系统信息
        /// </summary>
        public static void InitializeSystemInfo()
        {
            GetOSDetails();
            GetSystemLanguage();
            GetProcessorName();
        }

        /// <summary>
        /// 获取操作系统版本详情（版本名称和内部版本号）
        /// </summary>
        public static void GetOSDetails()
        {
            try
            {

                // 创建 WMI 查询以获取操作系统信息
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject os in searcher.Get().Cast<ManagementObject>())
                {
                    OSName = os["Caption"]?.ToString() ?? "未知操作系统";
                    OSBuildNumber = os["Version"]?.ToString() ?? "未知版本号";
                    OSArchitecture = os["OSArchitecture"]?.ToString() ?? "未知架构";
                }
            }
            catch
            {
                OSName = "获取系统型号失败";
                OSBuildNumber = "获取版本号失败";
                OSArchitecture = "获取架构失败";
            }
        }
        /// <summary>
        /// 获取系统语言
        /// </summary>
        public static void GetSystemLanguage()
        {
            try
            {
                SystemLanguage = CultureInfo.CurrentUICulture.Name;
            }
            catch
            {
                SystemLanguage = "获取语言失败";
            }
        }
        /// <summary>
        /// 获取处理器名称
        /// </summary>
        /// <returns></returns>
        public static void GetProcessorName()
        {
            var version = new StringBuilder();
            var moc = new ManagementClass("Win32_Processor").GetInstances();
            foreach (ManagementObject mo in moc.Cast<ManagementObject>())
            {
                foreach (var item in mo.Properties)
                {
                    if (item.Name != "Name") continue;
                    //   WriteLog.Debug(version.Append($"{item.Name}:{item.Value}\r\n").ToString());
                    ProcessorName = version.Append($"{item.Value}\n").ToString() ?? "未知CPU型号";
                }
            }
        }
    }
}
