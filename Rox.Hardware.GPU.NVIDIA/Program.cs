using System;
using System.Linq;
using System.Management;
using static Rox.Runtimes.Hardware.GPU.NVIDIA;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"专用显存: {GetGPUMemory()} GB");
                Console.WriteLine($"共享内存: {GetGPUSharedMemory()} GB");
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
            Console.Clear();
            Main2();
        }
        static void Main2()
        {
            try
            {
                // 查询 Win32_VideoController 类
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");

                // 遍历所有显卡（可能有多个，如集成显卡+独立显卡）
                foreach (ManagementObject mo in searcher.Get())
                {
                    Console.WriteLine("=== 显卡信息 ===");

                    // 遍历所有可用属性并输出（包括名称、类型、驱动、分辨率等）
                    foreach (PropertyData prop in mo.Properties)
                    {
                        // 属性名和值（值可能为 null，需处理）
                        string value = prop.Value?.ToString() ?? "null";
                        Console.WriteLine($"{prop.Name}: {value}");
                    }

                    Console.WriteLine("\n" + new string('-', 50) + "\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("错误: " + ex.Message);
            }
        }
    }
}
