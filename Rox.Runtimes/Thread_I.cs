using System;
using System.Threading;
using static Rox.Runtimes.LogLibraries;
using static Rox.Runtimes.LocalizedString;

namespace Rox.Runtimes
{
    public class Thread_I
    {
        /// <summary>
        /// 创建一个新线程并执行指定的方法。
        /// </summary>
        /// <param name="_void"> 要在线程中执行的方法。</param>
        public static void NewThread(ThreadStart _void)
        {
            Thread thread = new Thread(_void);
            thread.Start();
        }
        /// <summary>
        /// 辅助线程报告异常
        /// </summary>
        /// <param name="thread"> 要执行的线程方法。</param>
        public void ReportHelperThread(ThreadStart thread)
        {
            try
            {
                Thread t = new Thread(thread);
                t.Start();
            }
            catch (Exception e)
            {
                WriteLog.Error(_Exception_With_xKind(thread.ToString(), e));
                
            }
        }
    }
}
