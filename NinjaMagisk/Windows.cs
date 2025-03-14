using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static NinjaMagisk.LocalizedString;
using static NinjaMagisk.LogLibraries;
namespace NinjaMagisk
{
    /// <summary>
    /// Windows 相关操作
    /// </summary>
    public class Windows
    {
        /// <summary>
        /// 显示/隐藏文件拓展名
        /// </summary>
        /// <param name="Switch">显示/隐藏文件拓展名</param>
        public static void ShowFileExtension(bool Switch)
        {
            string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
            using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if (Switch)
                {
                    key.SetValue("HideFileExt", 0, RegistryValueKind.DWord); // 显示扩展名
                    return;
                }
                else
                {
                    key.SetValue("HideFileExt", 1, RegistryValueKind.DWord); // 隐藏扩展名
                    return;
                }
            }
        }
        /// <summary>
        /// 显示/隐藏被隐藏的文件和系统文件
        /// </summary>
        /// <param name="Switch">显示/隐藏被隐藏的文件和系统文件</param>
        public static void ShowHiddenFile(bool Switch)
        {
            string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
            using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if (Switch)
                {
                    key.SetValue("Hidden", 1, RegistryValueKind.DWord); // 显示隐藏文件
                    //key.SetValue("ShowSuperHidden", 1, RegistryValueKind.DWord); // 显示系统文件
                    return;
                }
                else
                {
                    key.SetValue("Hidden", 0, RegistryValueKind.DWord); // 隐藏隐藏文件
                    //key.SetValue("ShowSuperHidden", 0, RegistryValueKind.DWord); // 隐藏系统文件
                    return;
                }
            }
        }
        /// <summary>
        /// 向桌面添加系统位置快捷方式
        /// </summary>
        public class AddDesktopLink
        {
            /// <summary>
            /// 添加/移除"此电脑"图标
            /// </summary>
            /// <param name="Switch">指定添加或移除的图标</param>
            public static void AddThisPC(bool Switch)
            {
                string script;
                if (Switch)
                {
                    script = @"
            $keyPath = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel'
            if (-not (Test-Path $keyPath)) {
                New-Item -Path $keyPath -Force
            }
            Set-ItemProperty -Path $keyPath -Name '{20D04FE0-3AEA-1069-A2D8-08002B30309D}' -Value 0
        ";
                }
                else
                {
                    script = @"
            $keyPath = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel'
            Set-ItemProperty -Path $keyPath -Name '{20D04FE0-3AEA-1069-A2D8-08002B30309D}' -Value 1
        ";
                }

                // 执行 PowerShell 脚本
                RunPowerShellScript(script);
            }
            /// <summary>
            /// 添加/移除"网络"图标
            /// </summary>
            /// <param name="Switch">指定添加或移除的开关</param>
            public static void AddInternet(bool Switch)
            {
                string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel";
                using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    if (Switch)
                    {
                        key.SetValue("{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}", 0, RegistryValueKind.DWord); // 显示网络图标
                    }
                    else
                    {
                        key.SetValue("{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}", 1, RegistryValueKind.DWord); // 隐藏网络图标
                    }
                }
            }
            /// <summary>
            /// 添加/移除"控制面板"图标
            /// </summary>
            /// <param name="Switch">指定添加或移除的开关</param>
            public static void AddControlPan(bool Switch)
            {
                string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel";
                using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    if (Switch)
                    {
                        key.SetValue("{5399E694-6CE5-4D6C-8FCE-1D8870FDCBA0}", 0, RegistryValueKind.DWord); // 显示控制面板图标
                    }
                    else
                    {
                        key.SetValue("{5399E694-6CE5-4D6C-8FCE-1D8870FDCBA0}", 1, RegistryValueKind.DWord); // 隐藏控制面板图标
                    }
                }
            }
            /// <summary>
            /// 添加/移除"个人文件夹"图标
            /// </summary>
            /// <param name="Switch">指定添加或移除的开关</param>
            public static void AddUserFolder(bool Switch)
            {
                int value;
                if (Switch)
                    value = 0;
                else
                    value = 1;
                string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel";
                using (RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    if (key != null)
                    {
                        key.SetValue("{59031a47-3f72-44a7-89c5-5595fe6b30ee}", value, RegistryValueKind.DWord);
                        //MessageBox.Show($"已{(value == 0 ? "添加" : "移除")}“个人文件夹”图标", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("无法访问注册表！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            /// <summary>
            /// 运行PowerShell脚本
            /// </summary>
            /// <param name="script">指定的PowerShell脚本</param>
            private static void RunPowerShellScript(string script)
            {
                // 创建一个 ProcessStartInfo 对象
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe", // 指定启动 PowerShell
                    Arguments = $"-NoProfile -ExecutionPolicy unrestricted -Command \"{script}\"", // 传递脚本
                    RedirectStandardOutput = true, // 重定向标准输出
                    RedirectStandardError = true, // 重定向错误输出
                    UseShellExecute = false, // 不使用操作系统 shell 启动进程
                    CreateNoWindow = true // 不创建新窗口
                };

                // 创建并启动进程
                using (Process process = new Process())
                {
                    process.StartInfo = psi;
                    process.Start();

                    // 读取输出
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit(); // 等待进程结束

                    // 显示输出或错误
                    //if (!string.IsNullOrEmpty(output))
                    //{
                    //    MessageBox.Show(output, "PowerShell 输出", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //}
                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageBox.Show(error, "PowerShell 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        /// <summary>
        /// 一个用于修改默认访问位置的类，提供了静态方法 <see cref="ThisPC"/>和<see cref="QuickAcess"/>来控制默认打开的文件夹。
        /// </summary>
        public class ExplorerLaunchTo
        {
            public static void ThisPC()
            {
                Switch(1);
            }
            public static void QuickAcess()
            {
                Switch(2);
            }
            static void Switch(int value)
            {
                string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
                using (RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    if (key != null)
                    {
                        key.SetValue("LaunchTo", value, RegistryValueKind.DWord);
                    }
                    else
                    {
                        MessageBox.Show("无法访问注册表！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }
        /// <summary>
        /// 一个用于启用和禁用系统休眠功能的类，提供了静态方法 <see cref="Enable"/>和<see cref="Disable"/>来控制休眠状态。
        /// </summary>
        public class Hibernate
        {
            /// <summary>
            /// 启用系统休眠功能。
            /// </summary>
            public static void Enable()
            {
                Switch("on");
            }
            /// <summary>
            /// 禁用系统休眠功能。
            /// </summary>
            public static void Disable()
            {
                Switch("off");
            }
            /// <summary>
            /// 用于启用或禁用系统休眠功能。
            /// </summary>
            /// <param name="key"> 指定启用或禁用休眠功能的关键字。</param>
            static void Switch(string key)
            {
                Process Sleep = new Process();
                Sleep.StartInfo.FileName = "powercfg.exe";
                Sleep.StartInfo.Arguments = "/hibernate " + key;
                Sleep.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {Sleep.Id}");
                Sleep.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {Sleep.ExitCode}");
                if (Sleep.ExitCode != 0)
                {
                    WriteLog(LogLevel.Error, LogKind.System, $"{_CANNOT_DISENABLE_HIBERNATE}: {Sleep.ExitCode}");
                }
                else
                {
                    WriteLog(LogLevel.Info, LogKind.System, $"{_DISENABLE_HIBERNATE}");
                }
            }
        }//休眠
        /// <summary>
        /// 用于通过调用 <see langword="powercfg"/> 命令来启用卓越性能电源配置方案，并记录相关的进程信息和执行结果。
        /// </summary>
        public static void EnableHighPowercfg()
        {
            Process p = new Process();
            p.StartInfo.FileName = "powercfg";
            p.StartInfo.Arguments = "-duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61";
            p.Start();
            WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {p.Id}");
            p.WaitForExit();
            WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {p.ExitCode}");
            if (p.ExitCode != 0)
            {
                WriteLog(LogLevel.Error, $"{_CANNOT_ENABLE_HIGHPOWERCFG}: {p.ExitCode}");
            }
            else
            {
                WriteLog(LogLevel.Info, LogKind.Process, $"{_ENABLE_HIGHPOWERCFG}");
            }
        }//卓越性能电源方案
        /// <summary>
        /// 用于启用或禁用 Windows 安全中心的功能，并在操作过程中处理与安全软件的冲突
        /// </summary>
        public class WindowsSecurityCenter//Windows安全中心
        {
            /// <summary>
            /// 启用 Windows 安全中心
            /// </summary>
            public static void Enable()
            {
                Switch(0);
            }
            /// <summary>
            /// 禁用 Windows 安全中心
            /// </summary>
            public static void Disable()
            {
                Switch(1);
            }
            /// <summary>
            /// 用于启用或禁用 Windows 安全中心的功能
            /// </summary>
            /// <param name="value"> 指定启用或禁用 Windows 安全中心的值</param>
            static void Switch(int value)
            {
                while (AntiSecurity.Anti360Security() || AntiSecurity.AntiHuoRongSecurity())
                {
                    DialogResult dialogResult = MessageBox.Show($"{_SECURITY_RUNNING}", $"{_WARNING}", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    if (dialogResult == DialogResult.OK)
                    {
                        continue;
                    }
                }
                RegistryKey key;
                try
                {
                    // 修改 HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender
                    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender");
                    WriteLog(LogLevel.Info, $"{_WRITE_REGISTRY}");
                    key.SetValue("DisableAntiSpyware", value, RegistryValueKind.DWord);  // 禁用防间谍软件
                    WriteLog(LogLevel.Info, $"{_SUCESS_WRITE_REGISTRY}");
                    key.Close();  // 关闭注册表项

                    // 修改 HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection
                    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection");
                    WriteLog(LogLevel.Info, $"{_WRITE_REGISTRY}");
                    key.SetValue("DisableBehaviorMonitoring", value, RegistryValueKind.DWord);  // 禁用行为监控
                    WriteLog(LogLevel.Info, $"{_SUCESS_WRITE_REGISTRY}");
                    WriteLog(LogLevel.Info, $"{_WRITE_REGISTRY}");
                    key.SetValue("DisableIOAVProtection", value, RegistryValueKind.DWord);  // 禁用文件扫描
                    WriteLog(LogLevel.Info, $"{_SUCESS_WRITE_REGISTRY}");
                    WriteLog(LogLevel.Info, $"{_WRITE_REGISTRY}");
                    key.SetValue("DisableOnAccessProtection", value, RegistryValueKind.DWord);  // 禁用访问保护
                    WriteLog(LogLevel.Info, $"{_SUCESS_WRITE_REGISTRY}");
                    WriteLog(LogLevel.Info, $"{_WRITE_REGISTRY}");
                    key.SetValue("DisableRealtimeMonitoring", value, RegistryValueKind.DWord);  // 禁用实时监控
                    WriteLog(LogLevel.Info, $"{_SUCESS_WRITE_REGISTRY}");
                    key.Close();  // 关闭注册表项

                    // 修改 HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SecurityHealthService
                    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\SecurityHealthService");
                    WriteLog(LogLevel.Info, $"{_WRITE_REGISTRY}");
                    key.SetValue("Start", value + 2, RegistryValueKind.DWord);  // 设置服务启动类型为2自动 3手动 
                    WriteLog(LogLevel.Info, $"{_SUCESS_WRITE_REGISTRY}");
                    key.Close();
                    // 关闭注册表项
                }
                catch
                {
                    WriteLog(LogLevel.Error, $"{_WRITE_REGISTRY_FAILED}");
                    Thread.Sleep(1000);
                }
            }//开关
        }
        /// <summary>
        /// 用于启用或禁用 Windows 更新服务，并在操作过程中处理与安全软件的冲突
        /// </summary>
        public class WindowsUpdate//Windows更新服务
        {
            /// <summary>
            /// 启用 Windows 更新服务
            /// </summary>
            public static void Enable()
            {
                Switch("/E");
            }
            /// <summary>
            /// 禁用 Windows 更新服务
            /// </summary>
            public static void Disable()
            {
                Switch("/D");
            }
            /// <summary>
            /// 用于启用或禁用 Windows 更新服务
            /// </summary>
            /// <param name="value"> 指定启用或禁用 Windows 更新服务的值</param>
            /// <returns> 返回启用或禁用 Windows 更新服务的字符串</returns>
            private static string IsEnable(string value)
            {
                if (value == "/D")
                {
                    return _DISABLE_WINDOWS_UPDATER;
                }
                if (value == "/E")
                {
                    return _ENABLE_WINDOWS_UPDATER;
                }
                return null;
            }
            /// <summary>
            /// 用于处理启用或禁用 Windows 更新服务的错误
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private static string ErrorEnable(string value)
            {
                if (value == "/D")
                {
                    return _CANNOT_DISABLE_WINDOWS_UPDATER;
                }
                if (value == "/E")
                {
                    return _CANNOT_ENABLE_WINDOWS_UPDATER;
                }
                return null;
            }
            /// <summary>
            /// 用于启用或禁用 Windows 更新服务
            /// </summary>
            /// <param name="value"> 指定启用或禁用 Windows 更新服务的值</param>
            static void Switch(string value)
            {
                while (AntiSecurity.Anti360Security() || AntiSecurity.AntiHuoRongSecurity())
                {
                    DialogResult dialogResult = MessageBox.Show($"{_NOTAVAILABLE_NETWORK_TIPS}", $"{_WARNING}", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    if (dialogResult == DialogResult.OK)
                    {
                        continue;
                    }
                }
                try
                {
                    string modulePath = $"{AppDomain.CurrentDomain.BaseDirectory}\\bin";
                    bool is64Bit = Environment.Is64BitOperatingSystem;
                    string fileName;
                    if (is64Bit)
                    {
                        fileName = "Wub_x64.exe";
                    }
                    else
                    {
                        fileName = "Wubx32.exe";
                    }
                    DownloadAssistant.ModuleDownloader(DownloadAssistant.Module.Wub);
                    Process p = new Process();
                    p.StartInfo.FileName = Path.Combine(modulePath, fileName);
                    p.StartInfo.Arguments = value;
                    p.Start();
                    WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {p.Id}");
                    p.WaitForExit();
                    WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {p.ExitCode}");
                    if (p.ExitCode != 0)
                    {
                        WriteLog(LogLevel.Warning, $"{ErrorEnable(value)}: ExitCode= {p.ExitCode}");
                    }
                    else
                    {
                        WriteLog(LogLevel.Info, $"{IsEnable(value)}");
                    }
                    p.Close();
                }
                catch (Exception e)
                {
                    WriteLog(LogLevel.Error, $"{ErrorEnable(value)}: {e}");
                }
            }//开关
        }
        /// <summary>
        /// 用于激活 Windows 系统，首先检查安全软件的状态，然后启动指定的激活程序。
        /// </summary>
        public static void ActiveWindows()//Windows激活
        {
            while (AntiSecurity.Anti360Security() || AntiSecurity.AntiHuoRongSecurity())
            {
                DialogResult dialogResult = MessageBox.Show($"{_NOTAVAILABLE_NETWORK_TIPS}", $"{_WARNING}", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                if (dialogResult == DialogResult.OK)
                {
                    continue;
                }
            }
            DownloadAssistant.ModuleDownloader(DownloadAssistant.Module.Activator);
            try
            {
                Process process12 = new Process();
                process12.StartInfo.FileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\bin\\HEU_KMS_Activator_v19.6.0.exe";
                process12.StartInfo.Arguments = "/kms38";
                process12.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process12.Id}");
                WriteLog(LogLevel.Info, LogKind.Process, "process started");
                WriteLog(LogLevel.Info, LogKind.Process, $"Args: {AppDomain.CurrentDomain.BaseDirectory}\\bin\\HEU_KMS_Activator_v19.6.0.exe /kms38");
                process12.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process12.ExitCode}");
                if (process12.ExitCode != 0)
                {
                    WriteLog(LogLevel.Error, $"{_CANNOT_ACTIVE_WINDOWS}: {process12.ExitCode}");
                }
                else
                {
                    WriteLog(LogLevel.Info, $"{_ACTIVE_WINDOWS}");
                }
                process12.Close();
            }
            catch (Exception exception)
            {
                WriteLog(LogLevel.Error, $"{_CANNOT_ACTIVE_WINDOWS}: {exception}");
            }
        }
        #region Windows身份验证

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CREDUI_INFO
        {
            public int cbSize;
            public IntPtr hwndParent;
            public string pszMessageText;
            public string pszCaptionText;
            public IntPtr hbmBanner;
        }
        [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
        private static extern void CoTaskMemFree(IntPtr ptr);
        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        private static extern int CredUIPromptForWindowsCredentials(
                ref CREDUI_INFO pUiInfo,
                int dwAuthError,
                ref uint pulAuthPackage,
                IntPtr pvInAuthBuffer,
                uint ulInAuthBufferSize,
                out IntPtr ppvOutAuthBuffer,
                out uint pulOutAuthBufferSize,
                ref bool pfSave,
                int dwFlags);
        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        private static extern bool CredUnPackAuthenticationBuffer(
            int dwFlags,
            IntPtr pAuthBuffer,
            uint cbAuthBuffer,
            StringBuilder pszUserName,
            ref int pcchMaxUserName,
            StringBuilder pszDomainName,
            ref int pcchMaxDomainName,
            StringBuilder pszPassword,
            ref int pcchMaxPassword);
        // 导入 LogonUser API
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);
        // 验证用户名和密码

        /// <summary>
        /// 用于显示一个凭据提示框以验证用户身份。
        /// </summary>
        /// <returns> <see langword="true"/> 表示验证成功，<see langword="false"/> 表示验证失败。</returns>
        public static bool Authentication()
        {
            CREDUI_INFO credUI = new CREDUI_INFO
            {
                cbSize = Marshal.SizeOf(typeof(CREDUI_INFO)),
                pszCaptionText = _LOGIN_VERIFY,
                pszMessageText = _ENTER_CREDENTIALS,
                hwndParent = IntPtr.Zero,
                hbmBanner = IntPtr.Zero
            };
            bool isAuthenticated = false;
            bool userCancelled = false;
            do
            {
                uint authPackage = 0;
                bool save = false;

                int result = CredUIPromptForWindowsCredentials(
                    ref credUI,
                    0,
                    ref authPackage,
                    IntPtr.Zero,
                    0,
                    out IntPtr outCredBuffer,
                    out uint outCredBufferSize,
                    ref save,
                    0x1); // CREDUIWIN_GENERIC

                if (result == 0)
                {
                    int maxUserName = 100;
                    int maxDomainName = 100;
                    int maxPassword = 100;
                    StringBuilder userName = new StringBuilder(maxUserName);
                    StringBuilder domainName = new StringBuilder(maxDomainName);
                    StringBuilder password = new StringBuilder(maxPassword);

                    if (CredUnPackAuthenticationBuffer(0, outCredBuffer, outCredBufferSize, userName, ref maxUserName, domainName, ref maxDomainName, password, ref maxPassword))
                    {
                        // 验证用户名和密码
                        bool isValid = LogonUser(
                            userName.ToString(),
                            domainName.ToString(),
                            password.ToString(),
                            2, // LOGON32_LOGON_INTERACTIVE
                            0, // LOGON32_PROVIDER_DEFAULT
                            out IntPtr userToken);
                        string ExtraMessage;
                        if (isValid)
                        {
                            isAuthenticated = true;
                            CloseHandle(userToken);
                            MessageBox.Show(_SUCCESS_VERIFY, _TIPS, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            // 验证失败，显示错误提示
                            int errorCode = Marshal.GetLastWin32Error();
                            if (errorCode == 1326)
                            {
                                ExtraMessage = _LOGIN_ERROR_USER_OR_PASSWORD;
                            }
                            else
                            {
                                ExtraMessage = _UNKNOW_ERROR;
                            }
                            MessageBox.Show($"{_LOGIN_VERIFY_ERROR}（{_ERROR_CODE}：{errorCode} {ExtraMessage}）", _ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        // 调用验证方法
                    }
                    CoTaskMemFree(outCredBuffer);
                }
                else
                {
                    userCancelled = true;
                }
            }
            while (!isAuthenticated && !userCancelled); // 未成功且未取消时循环
            if (isAuthenticated)
            {
                // 执行后续操作（例如打开受保护的功能）
                WriteLog(LogLevel.Info, _SUCCESS_VERIFY);
                return true;
            }
            else
            {
                WriteLog(LogLevel.Info, _CANCEL_OP);
                return false;
            }
        }//Windows安全中心身份验证
        #endregion
    }
}