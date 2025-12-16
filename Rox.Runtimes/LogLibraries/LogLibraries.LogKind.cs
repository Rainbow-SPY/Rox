namespace Rox
{
    namespace Runtimes
    {
        public partial class LogLibraries
        {
            /// <summary>
            /// 日志类型
            /// </summary>
            public enum LogKind
            {
                #region Network
                /// <summary>
                /// 客户端  <see cref="System.Net.Sockets.TcpClient"/>
                /// </summary>
                Client,
                /// <summary>
                /// 服务器  <see cref="System.Net.Sockets.TcpListener"/>
                /// </summary>
                Server,
                /// <summary>
                /// Tcp  <see cref="System.Net.Sockets.TcpClient"/>
                /// </summary>
                TcpClient,
                /// <summary>
                /// Tcp监听器  <see cref="System.Net.Sockets.TcpListener"/>
                /// </summary>
                TcpListener,
                /// <summary>
                /// HTTP请求 <see cref="System.Net.Http"/>
                /// </summary>
                Http,
                /// <summary>
                /// HTTP POST请求 <see cref="System.Net.Http.HttpClient.PostAsync(string, System.Net.Http.HttpContent)"/> 
                /// </summary>
                POST,
                /// <summary>
                /// HTTP GET请求 <see cref="System.Net.Http.HttpClient.GetAsync(string)"/>
                /// </summary>
                GET,
                /// <summary>
                /// 网络 <see cref="System.Net.NetworkInformation"/> "/>
                /// </summary>
                Network,

                #endregion


                #region IO
                /// <summary>
                /// 目录操作 <see cref="System.IO.Directory"/>
                /// </summary>
                Directory,
                /// <summary>
                /// 创建目录 <see cref="System.IO.Directory.CreateDirectory(string)"/>
                /// </summary>
                CreateDirectory,
                /// <summary>
                /// 删除目录 <see cref="System.IO.Directory.Delete(string, bool)"/>
                /// </summary>
                DeleteDirectory,
                /// <summary>
                /// 文件操作 <see cref="System.IO"/>
                /// </summary>
                File,
                /// <summary>
                /// 读取文件 <see cref="System.IO.File.ReadAllText(string)"/>
                /// </summary>
                ReadFile,
                /// <summary>
                /// 写入文件 <see cref="System.IO.File.WriteAllText(string, string)"/>
                /// </summary>
                WriteFile,
                /// <summary>
                /// 删除文件 <see cref="System.IO.File.Delete(string)"/>
                /// </summary>
                DeleteFile,
                /// <summary>
                /// 读取数据库
                /// </summary>
                ReadDatabase,
                /// <summary>
                /// 写入数据库
                /// </summary>
                WriteDatabase,
                /// <summary>
                /// 数据库操作
                /// </summary>
                Database,
                /// <summary>
                /// 加密解密
                /// </summary>
                Crypto,
                /// <summary>
                /// 加密
                /// </summary>
                Encrypt,
                /// <summary>
                /// 解密
                /// </summary>
                Decrypt,
                /// <summary>
                /// 配置文件 <see cref="System.Configuration"/>
                /// </summary>
                Config,
                /// <summary>
                /// 读取配置文件
                /// </summary>
                ReadConfig,
                /// <summary>
                /// 写入配置文件
                /// </summary>
                WriteConfig,
                #endregion


                /// <summary>
                /// 反射 <see cref="System.Reflection"/>
                /// </summary>
                Reflection,
                /// <summary>
                /// 时间操作 <see cref="System.DateTime"/>
                /// </summary>
                DateTime,
                /// <summary>
                /// 异常 <see cref="System.Exception"/>
                /// </summary>
                Exception,
                /// <summary>
                /// 命令行 <see cref="System.Console"/>
                /// </summary>
                CommandLine,
                /// <summary>
                /// XML操作 <see cref="System.Xml"/>
                /// </summary>
                Xml,
                /// <summary>
                /// 缓存
                /// </summary>
                Cache,
                /// <summary>
                /// 权限 <see cref="System.Security.Principal"/>
                /// </summary>
                Permission,
                /// <summary>
                /// 序列化 <see cref="System.Runtime.Serialization"/>
                /// </summary>
                Serialization,
                /// <summary>
                /// 下载器
                /// </summary>
                Downloader,
                /// <summary>
                /// PowerShell
                /// </summary>
                PowerShell,
                /// <summary>
                /// 注册表 <see cref="Microsoft.Win32.Registry"/>
                /// </summary>
                Registry,
                /// <summary>
                /// 崩溃
                /// </summary>
                Crush,


                #region Form
                /// <summary>
                /// 窗体  <see cref="System.Windows.Forms"/>
                /// </summary>
                Form,
                /// <summary>
                /// 创建窗体 <see cref="System.Windows.Forms.Form"/>
                /// </summary>
                CreateForm,
                /// <summary>
                /// 关闭窗体 <see cref="System.Windows.Forms.Form.Close()"/>
                /// </summary>
                CloseForm,
                /// <summary>
                /// 打开窗体 <see cref="System.Windows.Forms.Form.ShowDialog()"/>
                /// </summary>
                OpenForm,
                #endregion


                #region Thread & PRocess
                /// <summary>
                /// 线程 <see cref="System.Threading.Thread"/>
                /// </summary>
                Thread,
                /// <summary>
                /// 启动线程 <see cref="System.Threading.Thread.Start()"/>
                /// </summary>
                StartThread,
                /// <summary>
                /// 结束线程 <see cref="System.Threading.Thread.Abort()"/>
                /// </summary>
                CloseThread,
                /// <summary>
                /// 进程 <see cref="System.Diagnostics.Process"/>
                /// </summary>
                Process,
                /// <summary>
                /// 启动进程 <see cref="System.Diagnostics.Process.Start(string)"/>
                /// </summary>
                StartProcess,
                /// <summary>
                /// 结束进程 <see cref="System.Diagnostics.Process.Kill()"/>
                /// </summary>
                CloseProcess,
                #endregion


                #region Service 
                /// <summary>
                /// 服务
                /// </summary>
                Service,
                /// <summary>
                /// 启动服务
                /// </summary>
                StartService,
                /// <summary>
                /// 关闭服务
                /// </summary>
                CloseService,
                #endregion


                #region Task
                /// <summary>
                /// 任务 <see cref="System.Threading.Tasks.Task"/>
                /// </summary>
                Task,
                /// <summary>
                /// 启动任务 <see cref="System.Threading.Tasks.Task.Start()"/>
                /// </summary>
                StartTask,
                /// <summary>
                /// 关闭任务 <see cref="System.Threading.Tasks.Task.Dispose()"/>
                /// </summary>
                CloseTask,
                #endregion


                #region System
                /// <summary>
                /// 系统
                /// </summary>
                System,
                /// <summary>
                /// 获取系统信息 <see cref="System.Environment"/>
                /// </summary>
                GetSystemInfo,
                /// <summary>
                /// 获取GPU信息 <see cref="Rox.Runtimes.Hardware.GPU.General.GetInformation()"/>
                /// </summary>
                GetGPUInfo,
                /// <summary>
                /// 获取CPU信息
                /// </summary>
                GetCPUInfo,
                /// <summary>
                /// 获取主板信息
                /// </summary>
                GetMotherboardInfo,
                /// <summary>
                /// 获取显示器信息
                /// </summary>
                GetMonitorInfo,
                /// <summary>
                /// 获取TPM信息
                /// </summary>
                GetTPMInfo,
                /// <summary>
                /// 获取BIOS信息
                /// </summary>
                GetBIOSInfo,
                #endregion


                #region Json
                /// <summary>
                /// Json
                /// </summary>
                Json,
                /// <summary>
                /// 解析Json 
                /// </summary>
                ParseJson,
                /// <summary>
                /// 反序列化Json
                /// </summary>
                DeserializeJson,
                /// <summary>
                /// 序列化Json 
                /// </summary>
                SerializeJson,
                /// <summary>
                /// 压缩Json
                /// </summary>
                ComposeJson,
                /// <summary>
                /// JObject
                /// </summary>
                JObject,
                #endregion


                #region Regex
                /// <summary>
                /// 正则表达式 <see cref="System.Text.RegularExpressions.Regex"/>
                /// </summary>
                Regex,
                /// <summary>
                /// 匹配正则表达式 <see cref="System.Text.RegularExpressions.Regex.Match(string)"/>
                /// </summary>
                MatchRegex,
                #endregion


                #region Math
                /// <summary>
                /// 数学计算 <see cref="System.Math"/>
                /// </summary>
                Math,
                /// <summary>
                /// 最大数学计算 <see cref="System.Math.Max(double, double)"/>
                /// </summary>
                MaxMath,
                /// <summary>
                /// 最小数学计算 <see cref="System.Math.Min(double, double)"/>"/>
                /// </summary>
                MinMath,
                /// <summary>
                /// Sin数学计算 <see cref="System.Math.Asin(double)"/>
                /// </summary>
                SinMath,
                /// <summary>
                /// Cos数学计算 <see cref="System.Math.Acos(double)"/>
                /// </summary>
                CosMath,
                /// <summary>
                /// Tan数学计算 <see cref="System.Math.Atan(double)"/>
                /// </summary>
                TanMath,
                /// <summary>
                /// 四舍五入数学计算 <see cref="System.Math.Round(double)"/>
                /// </summary>
                RoundMath,
                #endregion
            }

        }
    }
}
