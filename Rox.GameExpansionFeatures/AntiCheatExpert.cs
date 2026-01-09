using System;
using System.Diagnostics;
using static Rox.Runtimes.IProcess;
using static Rox.Runtimes.LogLibraries;

namespace Rox.Entertainment
{
    /// <summary>
    /// 针对 AntiCheatExpert（ACE）客户端的优化功能
    /// </summary>
    public class AntiCheatExpert
    {
        private static readonly string _au = "AntiCheatExpert";
        /// <summary>
        /// ACE客户端进程优化
        /// </summary>
        public static void ACEClientProcessOptimization()
        {
            WriteLog.Info(_au, "开始优化ACE");
            using (var targetProcess = GetTargetProcess("SGuard64.exe"))
            {
                if (targetProcess == null)
                {
                    WriteLog.Error(LogKind.Process, "未找到进程");
                    return;
                }
                WriteLog.Info(LogKind.Process, $"ACE 目标进程: {targetProcess.ProcessName} PID: {targetProcess.Id}\n" +
                    $"当前进程优先级: {targetProcess.PriorityClass}\n" +
                    $"当前进程相关性: {(targetProcess.ProcessorAffinity == (IntPtr)(-1) ? "所有CPU核心" : targetProcess.ProcessorAffinity.ToString())}");

                WriteLog.Info(_au, "开始调整优先级: 低");
                bool a = ChangeProcessPriority(targetProcess, ProcessPriorityClass.Idle);
                if (!a)
                    WriteLog.Error(_au, "调整优先级失败");
                else
                    WriteLog.Info(_au, "调整优先级成功");
                bool b = ChangeProcessorAffinity(targetProcess, GetLastCPUCore());
                if (!b)
                    WriteLog.Error(_au, "调整相关性失败");
                else
                    WriteLog.Info(_au, "调整相关性成功");

                WriteLog.Info(_au, "优化完成, 当前ACE进程信息如下:\n" +
                    $"进程优先级: {targetProcess.PriorityClass}\n" +
                    $"进程相关性: {targetProcess.ProcessorAffinity}\n" +
                    $"新处理器相关性（对应CPU核心）：{GetCpuCoreList(targetProcess.ProcessorAffinity)}");
            }
        }
    }
}
