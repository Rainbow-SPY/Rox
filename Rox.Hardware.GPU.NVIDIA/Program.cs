using System;
using System.Linq;
using System.Management;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
using static Rox.Runtimes.Hardware.GPU.NVIDIA;
namespace Rox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string a = Runtimes.Hardware.GPU.NVIDIA.GetGPUMemory();
                string b = Runtimes.Hardware.GPU.NVIDIA.GetGPUFullName();
                string c = Runtimes.Hardware.GPU.NVIDIA.GetGPUDriverVersion();
                WriteLog.Info("GPU Full Name: " + b);
                WriteLog.Info("GPU Memory: " + a);
                WriteLog.Info("GPU Driver Model: " + c);

                Console.WriteLine($"专用显存: {GetGPUMemory()} MB");
                Console.WriteLine($"共享内存: {GetGPUSharedMemory()} MB");
                Console.WriteLine($"显存类型: {GetGPUMemoryType()}");
                Console.WriteLine($"显存总线宽度: {GetGPUMemoryBusWidth()} bit");

                Console.WriteLine($"GPU核心使用率: {GetGPUUsage()}%");
                Console.WriteLine($"显存使用率: {GetGPUMemoryUsage()}%");
                Console.WriteLine($"视频引擎使用率: {GetGPUVideoEngineUsage()}%");
                // 温度信息
                Console.WriteLine($"温度传感器(1): {GetGPUTemperature()} ℃");
                Console.WriteLine($"风扇传感器(1): {GetCooler1RPM()}");
                Console.WriteLine($"风扇传感器(2): {GetCooler2RPM()}");
                Console.WriteLine($"风扇传感器(3): {GetCooler3RPM()}");

                Console.WriteLine($"GPU架构: {GetGPUArchitecture()}");
            }
            catch (Exception ex)
            {
                WriteLog.Error(_Exception_With_xKind("x", ex));
            }
            using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
            {
                foreach (var obj in searcher.Get().Cast<ManagementObject>())
                {
                    WriteLog.Info("DriverVersion  -  " + obj["DriverVersion"]);
                }
            }
        }
    }
}
