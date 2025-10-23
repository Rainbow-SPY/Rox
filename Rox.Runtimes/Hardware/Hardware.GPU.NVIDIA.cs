using NvAPIWrapper.GPU;
using System;
using System.Linq;
using System.Management;
using static Rox.Runtimes.Convert;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;

namespace Rox.Runtimes.Hardware.GPU
{
    /// <summary>
    /// NVIDIA GPU 信息获取类
    /// </summary>
    public class NVIDIA
    {
        private static Information _inf;
        /// <summary>
        /// 获取NVIDIA GPU的显存大小
        /// </summary>
        /// <returns> 显存大小的字符串表示，例如 "8 GB" </returns>
        public static Information GetInformation()
        {
            try
            {
                var a = PhysicalGPU.GetPhysicalGPUs()[0];
                var b = a.MemoryInformation;
                var c = a.UsageInformation;
                var d = a.CoolerInformation;
                Information Information = new Information
                {
                    FullName = a.FullName ?? "N/A",
                    Memory = Math.Round(ToGB(ToBytes($"{b.DedicatedVideoMemoryInkB}KB")), 0),
                    SharedMemory = Math.Round(ToGB(ToBytes($"{b.SharedSystemMemoryInkB}KB")), 0),
                    MemoryType = b.RAMType.ToString() ?? "N/A",
                    MemoryBusWidth = (int)b.RAMBusWidth,
                    GPUUsage = c.GPU.Percentage,
                    MemoryUsage = c.FrameBuffer.Percentage,
                    VideoEngineUsage = c.VideoEngine.Percentage,
                    Temperature = (int)(a.ThermalInformation.ThermalSensors.FirstOrDefault()?.CurrentTemperature),
                    Cooler_1_RPM = d.Coolers.ElementAtOrDefault(0)?.CurrentFanSpeedInRPM ?? 0,
                    Cooler_2_RPM = d.Coolers.ElementAtOrDefault(1)?.CurrentFanSpeedInRPM ?? 0,
                    Cooler_3_RPM = d.Coolers.ElementAtOrDefault(2)?.CurrentFanSpeedInRPM ?? 0,
                    GPUArchitecture = a.ArchitectInformation.ShortName ?? "N/A"
                };
                using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
                {
                    foreach (var obj in searcher.Get().Cast<ManagementObject>())
                    {
                        Information.DriverVersion = (string)obj["DriverVersion"];
                        break;
                    }
                }
                _inf = Information;
                return Information;

            }
            catch (Exception ex)
            {
                WriteLog.Error(_Exception_With_xKind("x", ex));
                return null;
            }
        }
        /// <summary>
        /// 获取NVIDIA GPU的显存大小
        /// </summary>
        /// <returns> 返回显存大小字符串 </returns>
        public static double? GetGPUMemory()
        {
            if (_inf == null)
            {
                _inf = GetInformation();
            }
            return _inf?.Memory;
        }
        /// <summary>
        /// 获取NVIDIA GPU的完整名称
        /// </summary>
        /// <returns> 返回GPU完整名称字符串 </returns>
        public static string GetGPUFullName()
        {
            if (_inf == null)
            {
                _inf = GetInformation();
            }
            return _inf?.FullName;
        }
        /// <summary>
        /// 获取NVIDIA GPU的驱动版本
        /// </summary>
        /// <returns> 返回驱动版本字符串 </returns>
        public static string GetGPUDriverVersion()
        {
            if (_inf == null)
            {
                _inf = GetInformation();
            }
            return _inf?.DriverVersion;
        }
        /// <summary>
        /// 获取NVIDIA GPU的共享内存大小 
        /// </summary>
        /// <returns></returns>
        public static double? GetGPUSharedMemory()
        {
            if (_inf == null)
            {
                _inf = GetInformation();
            }
            return _inf?.SharedMemory;
        }
        /// <summary>
        /// 获取NVIDIA GPU的内存类型
        /// </summary>
        /// <returns></returns>
        public static string GetGPUMemoryType()
        {
            if (_inf == null)
            {
                _inf = GetInformation();
            }
            return _inf?.MemoryType;
        }
        /// <summary>
        /// 获取NVIDIA GPU的内存总线宽度
        /// </summary>
        /// <returns></returns>
        public static int? GetGPUMemoryBusWidth()
        {
            if (_inf == null)
            {
                _inf = GetInformation();
            }
            return _inf?.MemoryBusWidth;
        }
        /// <summary>
        /// 获取NVIDIA GPU的使用率
        /// </summary>
        /// <returns></returns>
        public static int? GetGPUUsage()
        {
            if (_inf == null)
            {
                _inf = GetInformation();
            }
            return _inf?.GPUUsage;
        }
        /// <summary>
        /// 获取NVIDIA GPU的内存使用率
        /// </summary>
        /// <returns></returns>
        public static int? GetGPUMemoryUsage()
        {
            if (_inf == null)
            {
                _inf = GetInformation();
            }
            return _inf?.MemoryUsage;
        }
        /// <summary>
        /// 获取NVIDIA GPU的视频引擎使用率
        /// </summary>
        /// <returns></returns>
        public static int? GetGPUVideoEngineUsage()
        {
            if (_inf == null)
            {
                _inf = GetInformation();
            }
            return _inf?.VideoEngineUsage;
        }
        /// <summary>
        /// 获取NVIDIA GPU的温度
        /// </summary>
        /// <returns></returns>
        public static int? GetGPUTemperature()
        {
            if (_inf == null)
            {
                _inf = GetInformation();
            }
            return _inf?.Temperature;
        }
        /// <summary>
        /// 获取NVIDIA GPU的架构
        /// </summary>
        /// <returns></returns>
        public static string GetGPUArchitecture()
        {
            if (_inf == null)
            {
                _inf = GetInformation();
            }
            return _inf?.GPUArchitecture;
        }
        /// <summary>
        /// 获取NVIDIA GPU的风扇1转速 RPM
        /// </summary>
        /// <returns></returns>
        public static int? GetCooler1RPM()
        {
            if (_inf == null)
            {
                _inf = GetInformation();
            }
            return _inf?.Cooler_1_RPM;
        }
        /// <summary>
        /// 获取NVIDIA GPU的风扇2转速 RPM
        /// </summary>
        /// <returns></returns>
        public static int? GetCooler2RPM()
        {
            if (_inf == null)
            {
                _inf = GetInformation();
            }
            return _inf?.Cooler_2_RPM;
        }
        /// <summary>
        /// 获取NVIDIA GPU的风扇3转速 RPM
        /// </summary>
        /// <returns></returns>
        public static int? GetCooler3RPM()
        {
            if (_inf == null)
            {
                _inf = GetInformation();
            }
            return _inf?.Cooler_3_RPM;
        }


        /// <summary>
        /// NVIDIA GPU信息
        /// </summary>
        public class Information
        {
            /// <summary>
            /// GPU完整名称
            /// </summary>
            public string FullName { get; set; }
            /// <summary>
            /// GPU显存大小
            /// </summary>
            public double Memory { get; set; }
            /// <summary>
            /// GPU共享内存大小
            /// </summary>
            public double SharedMemory { get; set; }
            /// <summary>
            /// GPU内存类型
            /// </summary>
            public string MemoryType { get; set; }
            /// <summary>
            /// GPU内存总线宽度
            /// </summary>
            public int MemoryBusWidth { get; set; }
            /// <summary>
            /// GPU使用率
            /// </summary>
            public int GPUUsage { get; set; }
            /// <summary>
            /// GPU内存使用率
            /// </summary>
            public int MemoryUsage { get; set; }
            /// <summary>
            /// 视频引擎使用率
            /// </summary>
            public int VideoEngineUsage { get; set; }
            /// <summary>
            /// GPU温度
            /// </summary>
            public int Temperature { get; set; }
            /// <summary>
            /// 风扇1 转速 RPM
            /// </summary>
            public int Cooler_1_RPM { get; set; }
            /// <summary>
            /// 风扇2 转速 RPM
            /// </summary>
            public int Cooler_2_RPM { get; set; }
            /// <summary>
            /// 风扇3 转速 RPM
            /// </summary>
            public int Cooler_3_RPM { get; set; }
            /// <summary>
            /// GPU架构
            /// </summary>
            public string GPUArchitecture { get; set; }
            /// <summary>
            /// GPU驱动版本
            /// </summary>
            public string DriverVersion { get; set; }
        }
    }
}
