using System;
using System.Diagnostics;
using System.Linq;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;

namespace Rox.Runtimes
{
    /// <summary>
    /// 进程相关操作
    /// </summary>
    public class IProcess
    {
        /// <summary>
        /// 获取目标进程的进程对象
        /// </summary>
        /// <param name="processName"> 进程名称 </param>
        /// <returns> 目标进程对象 </returns>
        public static Process GetTargetProcess(string processName) => Process.GetProcessesByName(processName).FirstOrDefault();

        /// <summary>
        /// 调整指定进程对象的处理器相关性
        /// </summary>
        /// <param name="process"> 目标进程 </param>
        /// <param name="cpuMask"> CPU掩码（十进制） </param>
        /// <returns> 是否修改成功 </returns>
        public static bool ChangeProcessorAffinity(Process process, int cpuMask)
        {
            if (process == null) throw new ArgumentNullException(nameof(process));
            if (cpuMask < 1) throw new ArgumentOutOfRangeException(nameof(cpuMask), "CPU掩码不能小于1");

            // 将十进制CPU掩码转换为IntPtr赋值给ProcessorAffinity
            try
            {
                process.ProcessorAffinity = (IntPtr)cpuMask;
                WriteLog.Info(LogKind.Process, $"处理器相关性已成功修改为（十进制）：{cpuMask}");
                return true;
            }
            catch (Exception ex)
            {
                WriteLog.Error(LogKind.Process, _Exception_With_xKind("ChangeProcessorAffinity", ex));
                return false;
            }


        }
        /// <summary>
        /// 调整指定进程对象的进程优先级
        /// </summary>
        /// <param name="process"> 目标进程 </param>
        /// <param name="targetPriority"> 目标优先级 </param>
        /// <returns> 是否修改成功 </returns>
        public static bool ChangeProcessPriority(Process process, ProcessPriorityClass targetPriority)
        {
            if (process == null) throw new ArgumentNullException(nameof(process));
            try
            {
                process.PriorityClass = targetPriority;
                WriteLog.Info(LogKind.Process, $"\n进程优先级已成功修改为：{targetPriority}");
                return true;
            }
            catch (Exception ex)
            {
                WriteLog.Error(LogKind.Process, _Exception_With_xKind("ChangeProcessPriority", ex));
                return false;
            }

        }
        /// <summary>
        /// 获取当前处理器的最后一个核心
        /// </summary>
        /// <returns> 最后一个CPU核心的掩码 </returns>
        public static int GetLastCPUCore() => 1 << (Environment.ProcessorCount - 1);
        /// <summary>
        /// 解析处理器相关性对应的CPU核心列表
        /// </summary>
        /// <param name="affinityMask"> 处理器相关性掩码 </param>
        /// <returns> CPU核心列表字符串 </returns>
        public static string GetCpuCoreList(IntPtr affinityMask)
        {
            long mask = (long)affinityMask;
            if (mask == -1) return "所有CPU核心";

            var coreList = new System.Collections.Generic.List<int>();
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                // 按位与判断对应位是否为1
                if ((mask & (1 << i)) != 0)
                {
                    coreList.Add(i);
                }
            }

            return coreList.Count > 0 ? string.Join(", ", coreList) : "无有效CPU核心";
        }
    }
}
