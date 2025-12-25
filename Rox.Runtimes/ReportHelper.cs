using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static Rox.Runtimes.LogLibraries;

namespace Rox.Runtimes
{
    /// <summary>
    /// 异常报告助手
    /// </summary>
    public partial class Reporter : AntdUI.Window
    {
        /// <summary>
        /// 异常报告助手
        /// </summary>
        /// <param name="exception"> 要报告的异常对象。</param>
        public Reporter(Exception exception)
        {
            InitializeComponent();
            Icon = System.Drawing.SystemIcons.Error;
            GetSystemInfo.InitializeSystemInfo();
            string _Ex_type = exception.GetType().ToString();
            string _Ex_message = exception.Message;
            string _Ex_stacktrace = exception.StackTrace;
            string NowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Date = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            string osName = GetSystemInfo.OSName;
            string osBuild = GetSystemInfo.OSBuildNumber;
            string architecture = GetSystemInfo.OSArchitecture;
            string processor = GetSystemInfo.ProcessorName;
            string language = GetSystemInfo.SystemLanguage;
            string _Crush_File_Path = Path.Combine(Application.StartupPath, $"crush_{Date}.log");
            // 获取显示屏分辨率
            string width = Screen.PrimaryScreen.Bounds.Width.ToString();
            string height = Screen.PrimaryScreen.Bounds.Height.ToString();
            string screenInfo = $"{width}x{height}";
            richTextBox1.Clear();
            string log =
                "-------------Exception--------------------\n"
                + $"Exception Type: {_Ex_type}\n"
                + $"Exception Message: {_Ex_message}\n"
                + $"Exception StackTrance: \n{_Ex_stacktrace}\n"
                + $"Now Time: {NowTime}\n"
                + "-------------SystemInfo-------------------\n"
                + $"SystemName: {osName}\n"
                + $"SystemBuild: {osBuild}\n"
                + $"SystemArchitecture: {architecture}\n"
                + $"SystemLanguage: {language}\n"
                + $"MonitorResolution: {screenInfo}\n"
                // 获取处理器型号
                + $"Processor: {processor}\n"
                + $"-------------AppInfo----------------------\n"
                + $"AppVersion: {Application.ProductVersion}\n"
                + $"AppExecutable: {Application.ExecutablePath}\n"
                ;

            // 获取已加载的程序集信息
            log += "-------------Loaded Assemblies------------\n";
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var name = assembly.GetName();
                    log += $"Assembly: {name.Name}, Version: {name.Version}\n";
                }
                catch
                {
                    // 忽略无??获取名称的程序集
                }
            }


            WriteLog.Error(log);
            _Crush_Path.Text = _Crush_File_Path;
            LogToCrushFile(_Crush_File_Path, log);
            richTextBox1.Text += log;
            string _Ex_Description;
            switch (exception.GetType().FullName)
            {
                case "System.NullReferenceException":
                    _Ex_Description = "未将对象引用设置到对象的实例。通常是因为访问了值为null的对象的成员（如属性、方法），例如调用string s = null; s.Length;时会触发。";
                    break;
                case "System.IndexOutOfRangeException":
                    _Ex_Description = "索引超出了数组界限。当访问数组、集合时使用的索引值小于 0 或大于等于集合长度时发生，例如int[] arr = new int[3]; arr[3] = 1;。";
                    break;
                case "System.ArgumentNullException":
                    _Ex_Description = "传递给方法的参数为null，但该参数不允许为null。例如方法要求传入非空字符串，却传入null时触发。";
                    break;
                case "System.ArgumentOutOfRangeException":
                    _Ex_Description = "参数值超出了有效范围。常见于对长度、数值范围有要求的方法，例如string.Substring(-1)或设置负数超时时间。";
                    break;
                case "System.InvalidCastException":
                    _Ex_Description = "无法将一个类型强制转换为另一个类型。例如将string类型强制转换为int类型（(int)\"123\"）时，若字符串无法转换则触发。";
                    break;
                case "System.FormatException":
                    _Ex_Description = "格式不符合要求。常见于类型转换时，如int.Parse(\"abc\")或DateTime.Parse(\"2023/13/01\")（月份超出范围）。";
                    break;
                case "System.IO.FileNotFoundException":
                    _Ex_Description = "找不到指定的文件。当尝试访问不存在的文件路径（如File.Open(\"nonexistent.txt\")）时触发。";
                    break;
                case "System.IO.DirectoryNotFoundException":
                    _Ex_Description = "找不到指定的目录。与文件未找到类似，但针对文件夹路径无效的情况。";
                    break;
                case "System.IO.IOException":
                    _Ex_Description = "I/O 操作失败。例如文件被占用时尝试写入（如打开的文件未关闭时删除）、磁盘空间不足等。";
                    break;
                case "System.UnauthorizedAccessException":
                    _Ex_Description = "访问被拒绝。通常因权限不足导致，如尝试写入受保护的系统目录或修改只读文件。尝试使用管理员权限运行程序";
                    break;
                case "System.DivideByZeroException":
                    _Ex_Description = "尝试除以零。在整数除法中（如int a = 5 / 0;）触发，浮点数除法会得到Infinity而非此异常。";
                    break;
                case "System.StackOverflowException":
                    _Ex_Description = "堆栈溢出。通常由无限递归（如方法 A 调用方法 B，方法 B 又调用方法 A）或深层递归导致，会直接终止程序。";
                    break;
                case "System.OutOfMemoryException":
                    _Ex_Description = "内存不足。当程序申请的内存超过系统可用内存时触发，常见于处理超大对象或内存泄漏场景。";
                    break;
                case "System.InvalidOperationException":
                    _Ex_Description = "对象处于无效状态时执行操作。例如对已关闭的Stream调用Read()，或在未初始化的集合上执行特定操作。";
                    break;
                case "System.NotSupportedException":
                    _Ex_Description = "不支持所请求的操作。例如对只读集合调用Add()方法，或使用不支持的序列化方式。";
                    break;
                case "System.ArgumentException":
                    _Ex_Description = "参数无效（非范围或空值问题）。例如向方法传递格式正确但语义无效的参数（如传递负数作为年龄）。\r\n";
                    break;
                case "System.ObjectDisposedException":
                    _Ex_Description = "对已释放的对象执行操作。例如使用using块外的已释放资源（如Dispose()后的SqlConnection）。";
                    break;
                case "System.KeyNotFoundException":
                    _Ex_Description = "在字典中找不到指定的键。当使用Dictionary<TKey, TValue>的[]访问不存在的键时触发。";
                    break;
                case "System.InvalidProgramException":
                    _Ex_Description = "程序包含无效的 Microsoft 中间语言（MSIL）或元数据。通常由编译错误、损坏的程序集或内存损坏导致。";
                    break;
                case "System.Runtime.InteropServices.COMException":
                    _Ex_Description = "COM 操作失败。在调用 COM 组件（如 Office 插件、系统 API）时，因组件错误返回失败代码触发。";
                    break;
                case "Newtonsoft.Json.JsonException":
                    _Ex_Description = "Json.NET的通用基类异常，所有Json.NET专属错误都继承自此。通常表示 JSON 处理过程中发生了未分类的通用错误，如序列化 / 反序列化的核心流程异常。";
                    break;
                case "Newtonsoft.Json.SerializationException":
                    _Ex_Description = "序列化或反序列化过程中发生的序列化逻辑错误。不同于格式错误，该异常通常是对象结构与序列化配置不兼容导致（如循环引用、无法访问的私有成员、类型映射失败）。";
                    break;
                case "Newtonsoft.Json.JsonReaderException":
                    _Ex_Description = "JSON 读取器在解析 JSON 字符串时遇到无效的语法格式。";
                    break;
                case "Newtonsoft.Json.JsonWriterException":
                    _Ex_Description = "JSON 写入器在序列化生成 JSON 字符串时的格式错误。常见场景包括：手动使用JsonWriter时写入了不符合规范的 JSON 节点（如重复的键、未闭合的数组）、向写入器写入无效的字符（如控制字符）。";
                    break;
                case "Newtonsoft.Json.JsonSerializationException":
                    _Ex_Description = "JSON 值类型与目标属性类型不兼容/ 缺少必需的属性/ 未知的枚举值";
                    break;
                case "Newtonsoft.Json.JsonConverterException":
                    _Ex_Description = "自定义JsonConverter（JSON 转换器）执行过程中出错。常见场景包括：自定义转换器的ReadJson/WriteJson方法实现逻辑错误、转换器返回的类型与目标类型不匹配、转换器处理空值时未做判空。";
                    break;
                case "Newtonsoft.Json.MissingMemberException":
                    _Ex_Description = "反序列化时 JSON 中包含目标 C# 类不存在的成员（属性 / 字段），且配置了MissingMemberHandling.Error（默认是MissingMemberHandling.Ignore，不会抛异常）。";
                    break;
                case "Newtonsoft.Json.InvalidCastException":
                    _Ex_Description = "Json.NET在处理多态类型、接口 / 抽象类反序列化时的类型转换错误。例如将 JSON 反序列化为接口类型（如IList<string>）但未配置TypeNameHandling，或多态类型的子类型无法转换为父类型。";
                    break;
                case "System.Data.SqlClient.SqlException":
                    _Ex_Description = "SQL Server 操作失败，包含多种子场景：SQL 语法错误（如字段名写错、表不存在）、主键 / 唯一键冲突（插入重复数据）、外键约束失败（删除被关联的主表数据）、数据库连接超时、权限不足（账户无查询 / 写入权限）、死锁（多个事务互相占用资源）。";
                    break;
                case "System.Data.Entity.Core.EntityException":
                    _Ex_Description = "Entity Framework（EF）核心异常，基类错误，涵盖 EF 与数据库交互的通用失败（如连接字符串配置错误、模型与数据库表结构不匹配）";
                    break;
                case "System.Data.Entity.Core.InvalidOperationException（EF 专属）":
                    _Ex_Description = "EF 对象状态无效时执行操作，如对已附加的实体再次调用Attach、未将实体添加到上下文就执行SaveChanges、LINQ to Entities 查询中使用不支持的本地方法（如DateTime.Now.ToString()）。";
                    break;
                case "System.Data.Entity.Core.UpdateException":
                    _Ex_Description = "EF 执行SaveChanges时更新数据失败，通常由数据库约束（主键、外键、字段长度）导致，内部会嵌套SqlException。";
                    break;
                case "System.Data.Common.DbException":
                    _Ex_Description = "所有ADO.NET数据库提供程序的基类异常，当使用非 SQL Server 数据库（如 MySQL/Oracle）时，会抛出该类的子类（如MySqlException），表示数据库通用操作失败。";
                    break;
                case "System.Net.WebException":
                    _Ex_Description = ".NET 传统网络请求（HttpWebRequest）的核心异常，如请求超时（服务端无响应）、远程服务器返回错误状态码（404/500/403）、无法连接到服务器（域名解析失败、网络断开）、SSL 证书验证失败（访问 HTTPS 站点时证书过期 / 无效）。";
                    break;
                case "System.Net.Sockets.SocketException":
                    _Ex_Description = "Socket 套接字操作失败，如端口被占用（绑定已使用的端口）、连接被拒绝（服务端未监听目标端口）、远程主机强制关闭连接（服务端主动断开）、超出最大连接数。";
                    break;
                case "System.Net.Http.HttpRequestException":
                    _Ex_Description = "HttpClient请求的核心异常（.NET Framework 4.5+），涵盖 HTTP 请求的通用失败（如网络中断、服务端无响应），内部会嵌套WebException或SocketException。";
                    break;
                case "System.IO.EndOfStreamException":
                    _Ex_Description = "网络流读取时提前到达流末尾，如服务端返回的响应流不完整、网络传输中数据丢失，调用Stream.Read时触发。";
                    break;
                case "System.Threading.ThreadAbortException":
                    _Ex_Description = "线程被强制中止，如调用Thread.Abort()方法终止线程，该异常会自动重新抛出（即使捕获也会再次触发），.NET 中不推荐使用Abort。";
                    break;
                case "System.Threading.SynchronizationLockException":
                    _Ex_Description = "未获取锁的情况下执行释放锁操作，如调用Monitor.Exit(obj)但未先调用Monitor.Enter(obj)，或在lock代码块外释放锁。";
                    break;
                case "System.Threading.Tasks.TaskCanceledException":
                    _Ex_Description = "异步任务被取消，如调用CancellationTokenSource.Cancel()后，等待的Task触发该异常，或HttpClient请求超时导致任务取消。";
                    break;
                case "System.AggregateException":
                    _Ex_Description = "异步任务中发生的聚合异常，当Task.WhenAll/Task.WhenAny中多个任务抛出异常时，会将所有异常封装到该异常的InnerExceptions集合中（单个任务异常也可能嵌套其中）。";
                    break;
                case "System.Threading.Deadlock":
                    _Ex_Description = "死锁（非直接抛出的异常，但会导致程序卡死），两个或多个线程互相持有对方需要的锁，导致所有线程无法继续执行（如线程 A 持有锁 1 等待锁 2，线程 B 持有锁 2 等待锁 1）。";
                    break;
                case "System.Windows.Forms.InvalidEnumArgumentException":
                    _Ex_Description = "向 WinForm 控件传递无效的枚举值，如设置Button.DialogResult为不存在的枚举值、Form.WindowState传递无效的窗口状态。";
                    break;
                case "System.ComponentModel.Win32Exception":
                    _Ex_Description = "调用 Windows API 时的底层错误，WinForm 控件大量依赖 Win32 API，如创建窗口失败（系统资源不足）、打开文件对话框时的系统 API 调用失败，内部包含原生的 Windows 错误码。";
                    break;
                case "System.Windows.Forms.LayoutException":
                    _Ex_Description = "WinForm 控件布局计算失败，如嵌套的Panel/GroupBox控件布局循环依赖、Anchor/Dock属性设置冲突导致布局引擎计算异常。";
                    break;
                case "System.Drawing.InvalidImageFormatException":
                    _Ex_Description = "加载无效的图片格式，如将非图片文件（如 txt）当作 PNG/JPG 加载、图片文件损坏、不支持的图片格式（如 WebP 在旧版 GDI + 中不支持）。";
                    break;
                case "System.Drawing.Printing.PrintException":
                    _Ex_Description = "打印操作失败，如打印机未连接、打印机缺纸、打印队列已满、打印驱动程序损坏、无打印权限。";
                    break;
                case "System.Runtime.InteropServices.ExternalException":
                    _Ex_Description = "GDI + 操作失败的通用异常，如创建超大位图（超出系统 GDI 资源限制）、向已释放的Graphics对象绘图、GDI + 资源泄漏导致系统资源不足。";
                    break;
                case "System.IO.FileLoadException":
                    _Ex_Description = "加载程序集 / 资源文件失败，如引用的 DLL 文件损坏、DLL 版本不兼容（如 x86 DLL 在 x64 程序中加载）、资源文件（如 resx）格式错误。";
                    break;
                case "System.Reflection.TargetInvocationException":
                    _Ex_Description = "通过反射调用方法 / 构造函数时，目标方法内部抛出异常，该异常会封装原异常到InnerException中。";
                    break;
                case "System.Reflection.AmbiguousMatchException":
                    _Ex_Description = "反射查找方法 / 属性时找到多个匹配项，如类中有重载的方法且参数类型模糊、属性名存在大小写不同的重复项（非大小写敏感环境）。";
                    break;
                case "System.MissingMethodException":
                    _Ex_Description = "反射查找不存在的方法，如调用类中未定义的方法、构造函数参数不匹配且无默认构造函数。";
                    break;
                case "System.MissingFieldException":
                    _Ex_Description = "反射查找不存在的字段，如访问类中未定义的公共 / 私有字段。";
                    break;
                case "System.TypeLoadException":
                    _Ex_Description = "加载类型失败，如引用的程序集中不存在目标类型、程序集依赖缺失、类型的访问修饰符限制（如私有类型被外部反射访问）。";
                    break;
                case "System.Configuration.ConfigurationErrorsException":
                    _Ex_Description = "读取app.config/web.config配置文件失败，如配置节格式错误（XML 语法错误）、配置值类型不匹配（如将字符串赋值给数值型配置）、缺失必需的配置节。";
                    break;
                case "System.Xml.XmlException":
                    _Ex_Description = "XML 解析错误，如 XML 文件缺少闭合标签、特殊字符未转义、根节点重复，在读取 XML 配置 / 序列化时触发。";
                    break;
                case "System.Runtime.Serialization.SerializationException":
                    _Ex_Description = "二进制或 XML 序列化失败，如序列化未标记[Serializable]的类、序列化包含循环引用的对象、反序列化时类结构发生变化（如字段名修改）。";
                    break;
                case "System.Text.RegularExpressions.RegexMatchTimeoutException":
                    _Ex_Description = "正则表达式匹配超时，通常是正则表达式复杂度高或目标字符串过长导致匹配操作超出设定的超时时间。";
                    break;

                case "System.Linq.Enumerable.EmptyPartitionException":
                    _Ex_Description = "LINQ分区操作中遇到空分区，对空集合执行Skip/Take等分区操作时触发。";
                    break;

                case "System.Security.Cryptography.CryptographicException":
                    _Ex_Description = "加密操作失败，如密钥无效、加密算法不支持、数据损坏或签名验证失败。";
                    break;
                case "System.Security.Cryptography.HashAlgorithmException":
                    _Ex_Description = "哈希计算异常，哈希算法初始化失败或对空数据执行哈希操作时触发。";
                    break;

                case "System.Xml.XPath.XPathException":
                    _Ex_Description = "XPath表达式解析失败，传入的XPath语法错误或无法匹配XML节点。";
                    break;
                case "System.Xml.Schema.XmlSchemaException":
                    _Ex_Description = "XML架构验证失败，XML文档不符合指定的XSD架构约束（如节点类型、长度不匹配）。";
                    break;

                case "System.Runtime.InteropServices.InvalidComObjectException":
                    _Ex_Description = "COM对象无效，如使用未初始化的COM对象、COM对象已被释放或注册失败。";
                    break;
                case "System.Runtime.InteropServices.InvalidOleVariantTypeException":
                    _Ex_Description = "OLE变体类型无效，COM交互中传递的变体类型与目标接口要求不兼容。";
                    break;

                case "System.ServiceModel.CommunicationException":
                    _Ex_Description = "WCF通信异常，如服务端未启动、网络中断或绑定配置不匹配导致通信失败。";
                    break;
                case "System.ServiceModel.FaultException":
                    _Ex_Description = "WCF服务抛出故障异常，服务端业务逻辑执行失败并返回自定义故障信息。";
                    break;
                case "System.ServiceModel.EndpointNotFoundException":
                    _Ex_Description = "找不到WCF服务终结点，配置的服务地址错误或终结点未暴露。";
                    break;

                case "System.Collections.Generic.KeyNotFoundException":
                    _Ex_Description = "字典中找不到指定的键，使用Dictionary[]访问不存在的键时触发。";
                    break;

                case "System.Reflection.TargetParameterCountException":
                    _Ex_Description = "反射调用方法时参数数量不匹配，传入的参数个数与目标方法的形参个数不一致。";
                    break;
                case "System.Reflection.MemberAccessException":
                    _Ex_Description = "反射访问成员失败，如尝试访问私有成员、静态成员被当作实例成员访问。";
                    break;

                case "System.Threading.LockRecursionException":
                    _Ex_Description = "锁递归异常，递归获取非递归锁时触发（如Monitor锁不允许同一线程重复获取）。";
                    break;
                case "System.Threading.WaitHandleCannotBeOpenedException":
                    _Ex_Description = "等待句柄无法打开，如尝试打开不存在的命名互斥体、信号量。";
                    break;
                case "System.Threading.SemaphoreFullException":
                    _Ex_Description = "信号量已满，对已达到最大计数的信号量执行Release操作时触发。";
                    break;

                case "System.Net.Mail.SmtpException":
                    _Ex_Description = "SMTP邮件发送失败，如邮件服务器地址错误、端口被占用、认证失败或收件人地址无效。";
                    break;
                case "System.Net.FtpWebRequestException":
                    _Ex_Description = "FTP操作失败，如FTP服务器登录失败、文件上传/下载路径不存在或权限不足。";
                    break;
                case "System.Net.DnsException":
                    _Ex_Description = "DNS域名解析失败，如域名不存在、DNS服务器无响应或网络配置错误。";
                    break;

                case "System.Data.DataException":
                    _Ex_Description = "ADO.NET数据操作异常，如DataTable约束冲突、数据行状态无效或数据集序列化失败。";
                    break;
                case "System.Globalization.CultureNotFoundException":
                    _Ex_Description = "找不到指定的区域性信息，如传入无效的区域文化代码（如\"zh-CN-Invalid\"）。";
                    break;
                case "System.Resources.MissingManifestResourceException":
                    _Ex_Description = "找不到清单资源，如资源文件（resx）未嵌入程序集或资源名称拼写错误。";
                    break;
                case "System.ApplicationException":
                    _Ex_Description = "应用程序自定义异常，通常是业务逻辑中手动抛出的通用应用程序错误。";
                    break;
                case "System.IO.PathTooLongException":
                    _Ex_Description = "指定的路径或文件名超过了系统定义的最大长度，Windows系统中路径通常最大支持260个字符。";
                    break;
                case "System.IO.DriveNotFoundException":
                    _Ex_Description = "找不到指定的驱动器，如访问未挂载的U盘、映射的网络驱动器断开或光驱无介质。";
                    break;
                case "System.IO.FileExistsException":
                    _Ex_Description = "尝试创建的文件已存在，执行File.Create等操作时目标路径已有同名文件且未指定覆盖。";
                    break;
                case "System.IO.DirectoryExistsException":
                    _Ex_Description = "尝试创建的目录已存在，执行Directory.CreateDirectory时目标路径已有同名文件夹。";
                    break;

                case "System.Runtime.Serialization.InvalidDataContractException":
                    _Ex_Description = "数据契约无效，WCF数据契约序列化时类未正确标记[DataContract]或[DataMember]特性，或特性配置错误。";
                    break;
                case "System.Runtime.Serialization.SurrogateSelectorException":
                    _Ex_Description = "代理选择器异常，序列化时代理选择器无法找到匹配的序列化代理，或代理实现逻辑错误。";
                    break;

                case "System.Windows.Forms.DataGridViewException":
                    _Ex_Description = "DataGridView控件异常，如数据源绑定类型不匹配、单元格编辑时数据转换失败、列配置冲突。";
                    break;
                case "System.Windows.Forms.TreeViewException":
                    _Ex_Description = "TreeView控件异常，如节点添加时父节点不存在、节点数据绑定为空或递归添加节点导致层级溢出。";
                    break;
                case "System.Windows.Markup.XamlParseException":
                    _Ex_Description = "WPF的XAML解析异常，XAML代码语法错误、控件命名冲突、绑定路径无效或资源引用不存在。";
                    break;
                case "System.Windows.Markup.XamlXmlReaderException":
                    _Ex_Description = "WPF的XAML XML读取异常，XAML文件损坏、编码错误或包含不支持的XAML特性。";
                    break;
                case "System.Data.SqlClient.SqlTimeoutException":
                    _Ex_Description = "SQL Server超时异常，执行SQL语句时超出设定的超时时间（如复杂查询、数据库负载过高）。";
                    break;
                case "System.Data.SqlClient.SqlDependencyException":
                    _Ex_Description = "SQL依赖项异常，配置SQL Server依赖通知失败，或依赖的表结构发生变化导致通知失效。";
                    break;
                case "System.Data.Entity.Core.EntityCommandCompilationException":
                    _Ex_Description = "EF命令编译异常，LINQ to Entities查询转换为SQL语句失败（如使用不支持的LINQ方法）。";
                    break;
                case "MySql.Data.MySqlClient.MySqlException":
                    _Ex_Description = "MySQL操作异常，如SQL语法错误、连接超时、主键冲突或MySQL服务未启动（需引用MySQL Connector）。";
                    break;
                case "Npgsql.NpgsqlException":
                    _Ex_Description = "PostgreSQL操作异常，如连接字符串错误、表不存在、权限不足或PostgreSQL服务端口被占用。";
                    break;

                case "System.Net.Mail.SmtpFailedRecipientException":
                    _Ex_Description = "SMTP收件人异常，单个收件人地址无效或被邮件服务器拒收（如邮箱不存在、列入黑名单）。";
                    break;
                case "System.Net.Mail.SmtpFailedRecipientsException":
                    _Ex_Description = "SMTP多个收件人异常，批量发送邮件时多个收件人地址无效或被拒收。";
                    break;
                case "System.Net.FtpWebResponseException":
                    _Ex_Description = "FTP响应异常，FTP服务器返回错误码（如450文件被占用、550权限不足）。";
                    break;
                case "System.Net.WebSockets.WebSocketException":
                    _Ex_Description = "WebSocket通信异常，如连接握手失败、连接被断开、数据帧格式错误或心跳超时。";
                    break;

                case "System.Security.Cryptography.CryptographicUnexpectedOperationException":
                    _Ex_Description = "加密操作意外异常，如加密算法未正确初始化、解密时密钥与加密密钥不匹配或哈希值长度不匹配。";
                    break;
                case "System.Security.Cryptography.X509Certificates.CertificateException":
                    _Ex_Description = "X509证书异常，如证书过期、证书链验证失败、证书文件损坏或私钥丢失。";
                    break;
                case "System.Security.SecurityException":
                    _Ex_Description = "安全权限异常，程序集缺少必要的安全权限（如访问注册表、网络、文件系统的权限）。";
                    break;
                case "System.Security.Authentication.AuthenticationException":
                    _Ex_Description = "身份验证异常，如SSL/TLS握手失败、用户名密码错误或令牌过期。";
                    break;

                case "StackExchange.Redis.RedisConnectionException":
                    _Ex_Description = "Redis连接异常，如Redis服务器未启动、网络断开、密码错误或端口被占用（需引用StackExchange.Redis）。";
                    break;
                case "StackExchange.Redis.RedisServerException":
                    _Ex_Description = "Redis服务器异常，执行Redis命令时语法错误、键不存在或服务器内存不足。";
                    break;
                case "RabbitMQ.Client.Exceptions.BrokerUnreachableException":
                    _Ex_Description = "RabbitMQ代理不可达，如MQ服务器未启动、网络配置错误或虚拟主机不存在。";
                    break;
                case "RabbitMQ.Client.Exceptions.OperationInterruptedException":
                    _Ex_Description = "RabbitMQ操作中断，如连接被关闭、信道被销毁或发布消息时交换机不存在。";
                    break;

                case "System.Text.EncoderFallbackException":
                    _Ex_Description = "编码回退异常，字符编码转换时无法将某个字符转换为目标编码（如将中文转换为ASCII）。";
                    break;
                case "System.Text.DecoderFallbackException":
                    _Ex_Description = "解码回退异常，字符解码时遇到无效的字节序列，无法转换为目标字符。";
                    break;
                case "System.Net.CookieException":
                    _Ex_Description = "Cookie异常，如Cookie格式错误、域名不匹配、过期时间无效或Cookie大小超过限制。";
                    break;
                case "System.Configuration.SettingsPropertyNotFoundException":
                    _Ex_Description = "配置属性未找到，读取应用程序设置时指定的属性名在config文件中不存在。";
                    break;
                case "System.Configuration.SettingsPropertyWrongTypeException":
                    _Ex_Description = "配置属性类型错误，读取配置时属性值的类型与代码中定义的类型不匹配（如字符串转整数失败）。";
                    break;
                case "System.Numerics.BigIntegerParseException":
                    _Ex_Description = "大整数解析异常，将字符串转换为BigInteger时格式错误（如包含非数字字符）。";
                    break;
                case "Rox.Runtimes.IException.Steam.SteamServiceError":
                    _Ex_Description = "Steam 服务不可用, 无法访问 Steam 相关服务, 可以访问 https://steamstat.us/ 查看 Steam 服务当前情况";
                    break;
                case "Rox.Runtimes.IException.Steam.UnAuthenticatedSteamKey":
                    _Ex_Description = "提供的Steam Web API Key 无效或已过期";
                    break;
                case "Rox.Runtimes.IException.EpicGames.EpicGamesServerError":
                    _Ex_Description = "Epic Games 免费游戏服务不可用, 无法访问 Epic Games 相关服务, 可以访问 https://status.epicgames.com/ 查看 Epic Online Services 当前情况";
                    break;
                case "Rox.Runtimes.IException.Weather.WeatherAPIServerError":
                    _Ex_Description = "UAPI 服务器内部错误, 在处理天气数据时发生了未知问题";
                    break;
                case "Rox.Runtimes.IException.Weather.WeatherServiceError":
                    _Ex_Description = "天气供应商API不可用, 无法访问天气供应商提供的API";
                    break;
                default:
                    _Ex_Description = "发生未识别的异常，建议查看异常详细信息排查问题。";
                    break;

            }
            string _Title = (_Ex_Description != "" ? "已" : "未") + $"识别的异常类型: {exception.GetType().FullName}";
            AntdUI.Modal.open(new AntdUI.Modal.Config(this, _Title, $"错误类型: {exception.GetType().FullName}\n\n {_Ex_Description}", AntdUI.TType.Error)
            {
                Font = new Font("微软雅黑", 10, FontStyle.Regular),
                OkFont = new Font("微软雅黑", 10, FontStyle.Regular),
                OkText = "我知道了",
                MaskClosable = false,
                Draggable = false,
            });

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void _Crush_Path_Click(object sender, EventArgs e) => Process.Start("explorer.exe", Application.StartupPath);
    }
}
