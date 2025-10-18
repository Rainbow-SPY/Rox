using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static Rox.Runtimes.Hardware.GPU.NVIDIA;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;

namespace Rox.Runtimes.Hardware.GPU
{
    /// <summary>
    /// 通用GPU信息获取类
    /// </summary>
    public class General
    {
        private static Infomation _inf;
        private static string path = $"{Path.GetTempPath()}dxdiag_output.ralog";
        public static Infomation GetInfomation()
        {
            try
            {
                Infomation infomation = new Infomation
                {

                };
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = "dxdiag";
                    p.StartInfo.Arguments = $"/t \"{path}\"";
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.WaitForExit();
                }
                if (File.Exists(path))
                {
                    // 只读取前2000行, 避免文件过大
                    var lines = File.ReadLines(path).Take(200);
                    foreach (var line in lines)
                    {
                        if (line.Contains("Card name:"))
                        {
                            infomation.FullName = line.Split(new string[] { "Card name:" }, StringSplitOptions.None)[1].Trim();
                        }
                        else if (line.Contains("Display Memory:"))
                        {
                            var memStr = line.Split(new string[] { "Display Memory:" }, StringSplitOptions.None)[1].Trim();
                            if (memStr.EndsWith("MB"))
                            {
                                if (double.TryParse(memStr.Replace("MB", "").Trim(), out double memMB))
                                {
                                    infomation.Memory = Math.Round(memMB / 1024, 0);
                                }
                            }
                            else if (memStr.EndsWith("GB"))
                            {
                                if (double.TryParse(memStr.Replace("GB", "").Trim(), out double memGB))
                                {
                                    infomation.Memory = Math.Round(memGB, 0);
                                }
                            }
                        }
                    }
                }

                _inf = infomation;
                return infomation;


            }
            catch (Exception ex)
            {
                WriteLog.Error(_Exception_With_xKind("x", ex));
                return null;
            }
        }




        public class Information
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
            /// GPU使用率 (%)
            /// </summary>
            public int GPUUsage { get; set; }
            /// <summary>
            /// 温度 (°C)
            /// </summary>
            public int Temperature { get; set; }
        }
    }
}
