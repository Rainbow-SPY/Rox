using System;
using System.Diagnostics;
using System.Linq;
using static Rox.Runtimes.LogLibraries;

namespace Rox.GameExpansionFeatures
{
    public class AntiCheatExpert
    {
        public static int ACEClientProcessOptimization(string ProcessName)
        {
            var targetProcess = GetTargetProcess(ProcessName);
            if (targetProcess == null)
            {
                WriteLog.Error(LogKind.Process, "未找到进程");
                return -1;
            }
            WriteLog.Info(LogKind.Process, $"目标进程: {targetProcess.ProcessName} PID: {targetProcess.Id}\n" +
                $"当前进程优先级: {targetProcess.PriorityClass}\n" +
                $"当前进程相关性: {(targetProcess.ProcessorAffinity == (IntPtr)(-1) ? "所有CPU核心" : targetProcess.ProcessorAffinity.ToString())}");
        }

        private static Process GetTargetProcess(string processName) => Process.GetProcessesByName(processName).FirstOrDefault();

        /// <summary>
        /// 调整处理器相关性
        /// </summary>
        private static void AdjustProcessorAffinity(Process process, int cpuMask)
        {
            if (process == null) throw new ArgumentNullException(nameof(process));
            if (cpuMask < 1) throw new ArgumentOutOfRangeException(nameof(cpuMask), "CPU掩码不能小于1");

            // 将十进制CPU掩码转换为IntPtr赋值给ProcessorAffinity
            process.ProcessorAffinity = (IntPtr)cpuMask;
            WriteLog.Info(LogKind.Process,$"处理器相关性已成功修改为（十进制）：{cpuMask}");
        }
        /// <summary>
        /// 调整进程优先级
        /// </summary>
        private static void AdjustProcessPriority(Process process, ProcessPriorityClass targetPriority)
        {
            if (process == null) throw new ArgumentNullException(nameof(process));

            process.PriorityClass = targetPriority;
            Console.WriteLine($"\n进程优先级已成功修改为：{targetPriority}");
        }
        /// <summary>
        /// 获取当前处理器的最后一个核心
        /// </summary>
        /// <returns></returns>
        public static int GetLastCPUCores() => 1 << (Environment.ProcessorCount - 1);
    }
}
