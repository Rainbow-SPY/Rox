using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;

namespace Rox.Runtimes.Hardware.GPU
{
    /// <summary>
    /// 通用GPU信息获取类
    /// </summary>
    public class General
    {
        private static Information _inf;
        private static string path = $"{Path.GetTempPath()}dxdiag_output.ralog";
        /// <summary>
        /// 获取GPU信息
        /// </summary>
        /// <returns> GPU信息实例 </returns>
        public static Information GetInformation()
        {
            try
            {
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = "dxdiag";
                    p.StartInfo.Arguments = $"/t \"{path}\"";
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.WaitForExit();
                }
                Information Information = new Information();
                if (File.Exists(path))
                {
                    // 只读取前2000行, 避免文件过大
                    var lines = File.ReadLines(path).Take(200);
                    foreach (var a in lines)
                    {
                        var line = a.Trim();
                        if (line.Contains("Card name:"))
                        {
                            Information.FullName = line.Split(':')[1].Trim();
                        }

                        else if (line.Contains("Manufacturer:"))
                        {
                            Information.Manufacturer = line.Split(':')[1].Trim();
                        }
                        else if (line.Contains("Dedicated Memory:"))
                        {
                            var memStr = line.Split(':')[1].Trim();
                            if (memStr.EndsWith("MB"))
                            {
                                if (double.TryParse(memStr.Replace("MB", "").Trim(), out double memMB))
                                {
                                    Information.Memory = Math.Round(memMB / 1024, 0);
                                }
                            }
                            else if (memStr.EndsWith("GB"))
                            {
                                if (double.TryParse(memStr.Replace("GB", "").Trim(), out double memGB))
                                {
                                    Information.Memory = Math.Round(memGB, 0);
                                }
                            }
                        }
                        else if (line.Contains("Shared Memory:"))
                        {
                            var sharedMemStr = line.Split(':')[1].Trim();
                            if (sharedMemStr.EndsWith("MB"))
                            {
                                if (double.TryParse(sharedMemStr.Replace("MB", "").Trim(), out double sharedMemMB))
                                {
                                    Information.SharedMemory = Math.Round(sharedMemMB / 1024, 0);
                                }
                            }
                            else if (sharedMemStr.EndsWith("GB"))
                            {
                                if (double.TryParse(sharedMemStr.Replace("GB", "").Trim(), out double sharedMemGB))
                                {
                                    Information.SharedMemory = Math.Round(sharedMemGB, 0);
                                }
                            }
                        }
                    }
                }
                _inf = Information;
                return Information;
            }
            catch (Exception ex)
            {
                WriteLog.Error(_Exception_With_xKind("Hardware.GPU.General.GetInformation", ex));
                return null;
            }
        }



        /// <summary>
        /// GPU信息类
        /// </summary>
        public class Information
        {
            /// <summary>
            /// GPU完整名称
            /// </summary>
            public string FullName { get; set; }
            /// <summary>
            /// 制造商
            /// </summary>
            public string Manufacturer { get; set; }
            /// <summary>
            /// 显存大小 (GB)
            /// </summary>
            public double Memory { get; set; }
            /// <summary>
            /// 共享显存大小 (GB)
            /// </summary>
            public double SharedMemory { get; set; }
        }
    }
}
