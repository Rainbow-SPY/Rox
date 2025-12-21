using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox.Runtimes.Hardware
{
    /// <summary>
    /// 通用硬件信息获取类
    /// </summary>
    public class General
    {
        private static Information _inf;
        private static readonly string path = $"{Path.GetTempPath()}msinfo_output.ralog";
        /// <summary>
        /// 获取硬件信息
        /// </summary>
        /// <returns> 硬件信息实例 <see cref=" Information "/> </returns>
        public Information GetInformation()
        {
            try
            {
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = "msinfo32";
                    p.StartInfo.Arguments = $"/report \"{path}\"";
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.WaitForExit();
                }
                Information info = new Information();
                if (!File.Exists(path))
                {
                    return info;
                }
                foreach (var line in File.ReadLines(path))
                {
                    if (line.Contains("操作系统名称"))
                        info.System.OperatingSystemName = line.Split('\t')[1].Trim();
                    else if (line.Contains("系统制造商"))
                        info.System.Manufacturer = line.Split('\t')[1].Trim();
                    else if (line.Contains("系统型号"))
                        info.System.SystemModel = line.Split('\t')[1].Trim();
                    else if (line.Contains("BIOS 模式"))
                        info.System.IsUEFIBoot = line.Split('\t')[1].Trim() == "UEFI";
                    else if (line.Contains("已安装的物理内存(RAM)"))
                        info.System.Memory = line.Split('\t')[1].Trim();
                    else if (line.Contains("页面文件空间"))
                        info.System.PageFileSize = line.Split('\t')[1].Trim();
                    else if (line.Contains("主板制造商"))
                        info.Motherboard.Manufacturer = line.Split('\t')[1].Trim();
                    else if (line.Contains("内核 DMA 保护"))
                        info.System.KenelDMAProtection = line.Split('\t')[1].Trim() != "关闭";
                    else if (line.Contains("主板产品"))
                        info.Motherboard.Model = line.Split('\t')[1].Trim();
                    else if (line.Contains("安全启动状态"))
                        info.System.IsSecureBootEnabled = line.Split('\t')[1].Trim() != "关闭";
                    else if (line.Contains("基于虚拟机的安全性"))
                        info.System.SecurityBaseOnVirtualization = line.Split('\t')[1].Trim() != "未启用";
                }
                // 获取声音设备信息
                var a = GetSoundDevice();
                info.Sound.Name = a?.Name;
                info.Sound.Description = a?.Description;
                _inf = info;
                return info;
            }
            catch (Exception ex)
            {
                WriteLog.Error(_Exception_With_xKind("x", ex));
                return null;

            }
        }
        /// <summary>
        /// 获取声音设备信息
        /// </summary>
        /// <returns> 声音设备信息实例 <see cref=" Information._Sound "/> </returns>
        public static Information._Sound GetSoundDevice()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SoundDevice"))
                {
                    foreach (ManagementObject device in searcher.Get().Cast<ManagementObject>())
                    {
                        return new Information._Sound
                        {
                            Name = (device["Name"]?.ToString()),
                            Description = (device["Description"]?.ToString())
                        };
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                WriteLog.Error(_Exception_With_xKind("x", ex));
                return null;
            }
        }
        /// <summary>
        /// 硬件信息类
        /// </summary>
        public class Information
        {
            /// <summary>
            /// 新建获取系统信息实例
            /// </summary>
            public _System System { get; set; } = new _System();
            /// <summary>
            /// 新建获取显卡信息实例
            /// </summary>
            public _GPU GPU { get; set; } = new _GPU();
            /// <summary>
            /// 新建获取显示器信息实例
            /// </summary>
            public _Monitor Monitor { get; set; } = new _Monitor();
            /// <summary>
            /// 新建获取主板信息实例
            /// </summary>
            public _Motherboard Motherboard { get; set; } = new _Motherboard();
            /// <summary>
            /// 新建获取声音设备信息实例
            /// </summary>
            public _Sound Sound { get; set; } = new _Sound();
            /// <summary>
            /// GPU信息
            /// </summary>
            public class _GPU
            {
                /// <summary>
                /// GPU完整名称
                /// </summary>
                public string FullName { get; set; }
                /// <summary>
                /// 显存大小 (GB)
                /// </summary>
                public double Memory { get; set; }
                /// <summary>
                /// 共享显存大小 (GB)
                /// </summary>
                public double SharedMemory { get; set; }

            }
            /// <summary>
            /// 显示器信息
            /// </summary>
            public class _Monitor
            {
                /// <summary>
                /// HDR支持情况
                /// </summary>
                public bool IsHDRSupported { get; set; }

            }
            /// <summary>
            /// 声音设备信息
            /// </summary>
            public class _Sound
            {
                /// <summary>
                /// 声音设备名称
                /// </summary>
                public string Name { get; set; }
                /// <summary>
                /// 声音设备描述
                /// </summary>
                public string Description { get; set; }
            }
            /// <summary>
            /// 系统信息
            /// </summary>
            public class _System
            {
                /// <summary>
                /// 系统制造商
                /// </summary>
                public string Manufacturer { get; set; }
                /// <summary>
                /// 操作系统名称
                /// </summary>
                public string OperatingSystemName { get; set; }
                /// <summary>
                /// 计算机名称
                /// </summary>
                public string MachineName { get; set; }
                /// <summary>
                /// 是否为UEFI启动
                /// </summary>
                public bool IsUEFIBoot { get; set; }
                /// <summary>
                /// 是否启用安全启动
                /// </summary>
                public bool IsSecureBootEnabled { get; set; }
                /// <summary>
                /// 系统内存大小
                /// </summary>
                public string Memory { get; set; }
                /// <summary>
                /// 页面文件大小
                /// </summary>
                public string PageFileSize { get; set; }
                /// <summary>
                /// 系统型号
                /// </summary>
                public string SystemModel { get; set; }
                /// <summary>
                /// 内核DMA保护状态
                /// </summary>
                public bool KenelDMAProtection { get; set; }
                /// <summary>
                /// 基于虚拟机的安全性状态
                /// </summary>
                public bool SecurityBaseOnVirtualization { get; set; }
            }
            /// <summary>
            /// 主板信息
            /// </summary>
            public class _Motherboard
            {
                /// <summary>
                /// 主板制造商
                /// </summary>
                public string Manufacturer { get; set; }
                /// <summary>
                /// 主板型号
                /// </summary>
                public string Model { get; set; }
            }
        }
    }
}
