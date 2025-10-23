namespace Rox.Runtimes.Hardware
{

    public static Information GetInformation()
    {
        Information infoNVIDIA = GPU.NVIDIA.GetInformation();
        if (infoNVIDIA != null)
        {
            return infoNVIDIA;
        }
        Information infoGeneral = GPU.General.GetInformation();
        return infoGeneral;
    }

    public class Information
    {
        public class GPU
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
        public class Monitor
        {
            /// <summary>
            /// HDR支持情况
            /// </summary>
            public bool IsHDRSupported { get; set; }

        }
        public class System
        {
            public string MachineName { get; set; }
            /// <summary>
            /// 是否为UEFI启动
            /// </summary>
            public bool IsUEFIBoot { get; set; }
            /// <summary>
            /// 系统内存大小
            /// </summary>
            public string Memory { get; set; }
            /// <summary>
            /// 页面文件大小
            /// </summary>
            public string PageFileSize { get; set; }
        }

        public class Motherboard
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
