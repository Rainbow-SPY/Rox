using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
using Registry = Rox.Runtimes.Registry_I;
namespace Rox
{
    /// <summary>
    /// Windows 相关操作
    /// </summary>
    public class Windows
    {
        /// <summary>
        /// Windows 资源管理器操作 及 桌面相关操作
        /// </summary>
        public class Explorer
        {
            /// <summary>
            /// 显示/隐藏文件拓展名
            /// </summary>
            /// <param name="Switch">显示/隐藏文件拓展名</param>
            public static void ShowFileExtension(bool Switch)
            {
                string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
                using (RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    key.SetValue("HideFileExt", Switch ? 0 : 1, RegistryValueKind.DWord); // 显示扩展名
                    WriteLog.Info(_SUCESS_WRITE_REGISTRY + keyPath);
                    RefreshExplorer();
                    return;
                }
            } //0显示 1隐藏
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
                        WriteLog.Info(_SUCESS_WRITE_REGISTRY + keyPath);
                        RefreshExplorer();
                        return;
                    }
                    else
                    {
                        key.SetValue("Hidden", 0, RegistryValueKind.DWord); // 隐藏隐藏文件
                                                                            //key.SetValue("ShowSuperHidden", 0, RegistryValueKind.DWord); // 隐藏系统文件
                        WriteLog.Info(_SUCESS_WRITE_REGISTRY + keyPath);
                        RefreshExplorer();
                        return;
                    }
                }
            } //0隐藏 1显示
            /// <summary>
            /// 设置文件夹的可见性
            /// </summary>
            /// <param name="registryPath"> 注册表位置</param>
            /// <param name="visibility"> 可见性</param>
            private static void SetFolderVisibility(string registryPath, string visibility)
            {
                try
                {
                    Registry.Write(registryPath, "ThisPCPolicy", visibility, RegistryValueKind.String);
                    WriteLog.Info($"成功设置 {registryPath} 的可见性为 {visibility}");
                }
                catch (Exception ex)
                {
                    WriteLog.Error($"设置 {registryPath} 的可见性时出错: {ex.Message}");
                }
            }
            #region 刷新资源管理器
            // 导入 Windows API
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll", SetLastError = true)]
            private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            private static extern IntPtr SendMessageTimeout(
                IntPtr hWnd,
                uint Msg,
                IntPtr wParam,
                string lParam,
                uint fuFlags,
                uint uTimeout,
                out IntPtr lpdwResult);

            // 定义消息常量
            private const uint WM_COMMAND = 0x0111;
            private const int SC_REFRESH = 0xF720;
            private const uint WM_SETTINGCHANGE = 0x001A;
            private const uint SMTO_ABORTIFHUNG = 0x0002;
            private const int HWND_BROADCAST = 0xFFFF;

            /// <summary>
            /// 综合刷新方法：刷新资源管理器窗口、桌面，并通知系统设置已更改
            /// </summary>
            public static void RefreshExplorer()
            {
                try
                {
                    // 1. 刷新已打开的资源管理器窗口
                    RefreshExplorerWindows();

                    // 2. 刷新桌面
                    RefreshDesktop();

                    // 3. 通知系统设置已更改
                    NotifySettingChange();

                    WriteLog.Info("资源管理器、桌面和系统设置已刷新。");
                }
                catch (Exception ex)
                {
                    WriteLog.Error($"刷新过程中出错: {ex.Message}");
                }
            }

            /// <summary>
            /// 刷新已打开的资源管理器窗口
            /// </summary>
            private static void RefreshExplorerWindows()
            {
                try
                {
                    // 查找所有 Explorer 窗口
                    Process[] explorerProcesses = Process.GetProcessesByName("explorer");
                    foreach (var process in explorerProcesses)
                    {
                        IntPtr hWnd = process.MainWindowHandle;
                        if (hWnd != IntPtr.Zero)
                        {
                            // 发送刷新命令
                            SendMessage(hWnd, WM_COMMAND, (IntPtr)SC_REFRESH, IntPtr.Zero);
                        }
                    }

                    WriteLog.Info("已刷新所有资源管理器窗口。");
                }
                catch (Exception ex)
                {
                    WriteLog.Error($"刷新资源管理器窗口时出错: {ex.Message}");
                }
            }

            /// <summary>
            /// 刷新桌面
            /// </summary>
            private static void RefreshDesktop()
            {
                try
                {
                    // 查找桌面窗口
                    IntPtr hWnd = FindWindow("Progman", null);
                    if (hWnd != IntPtr.Zero)
                    {
                        // 发送刷新命令
                        SendMessage(hWnd, WM_COMMAND, (IntPtr)SC_REFRESH, IntPtr.Zero);
                        WriteLog.Info("桌面已刷新。");
                    }
                    else
                    {
                        WriteLog.Warning("未找到桌面窗口。");
                    }
                }
                catch (Exception ex)
                {
                    WriteLog.Error($"刷新桌面时出错: {ex.Message}");
                }
            }

            /// <summary>
            /// 通知系统设置已更改
            /// </summary>
            private static void NotifySettingChange()
            {
                try
                {
                    // 发送 WM_SETTINGCHANGE 消息
                    SendMessageTimeout(
                        (IntPtr)HWND_BROADCAST, // 广播到所有窗口
                        WM_SETTINGCHANGE,      // 消息类型
                        IntPtr.Zero,           // 未使用
                        "Environment",         // 表示环境变量或设置已更改
                        SMTO_ABORTIFHUNG,      // 超时标志
                        5000,                  // 超时时间（毫秒）
                        out IntPtr result);           // 返回值

                    WriteLog.Info("已通知系统设置更改。");
                }
                catch (Exception ex)
                {
                    WriteLog.Error($"通知系统设置更改时出错: {ex.Message}");
                }
            }
            #endregion
            /// <summary>
            /// 用于安装和卸载 Windows 11 的小部件
            /// </summary>
            public static void UninstallWindows11Widgets()
            {
                Process.Start("winget", "uninstall MicrosoftWindows.Client.WebExperience_cw5n1h2txyewy");
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
                            //MessageBox_I.($"已{(value == 0 ? "添加" : "移除")}“个人文件夹”图标", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox_I.Error("无法访问注册表！", _ERROR);
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
                        //    MessageBox_I.(output, "PowerShell 输出", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //}
                        if (!string.IsNullOrEmpty(error))
                        {
                            MessageBox_I.Error(error, "PowerShell 错误");
                        }
                    }
                }
            }
            /// <summary>
            /// 用于修改桌面图标小箭头的类
            /// </summary>
            public class DesktopIconsArrow
            {
                // 隐藏桌面图标小箭头
                /// <summary>
                /// 隐藏桌面图标小箭头
                /// </summary>
                public static void HideDesktopIconArrow()
                {
                    // 隐藏桌面图标小箭头
                    Registry.Write(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Icons", "29", @"%systemroot%\system32\imageres.dll,197", RegistryValueKind.String);
                    RefreshExplorer();
                    return;
                }
                // 显示桌面图标小箭头
                /// <summary>
                /// 显示桌面图标小箭头
                /// </summary>
                public static void ShowDesktopIconArrow()
                {
                    // 显示桌面图标小箭头
                    Microsoft.Win32.Registry.LocalMachine.DeleteSubKeyTree(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Icons");
                    RefreshExplorer();
                    return;
                }
            }
            /// <summary>
            /// 用于修改桌面系统图标是否在桌面上显示的类
            /// </summary>
            public class DesktopIconSettings
            {
                // 导入 ShellExecute 函数
                [DllImport("shell32.dll", SetLastError = true)]
                private static extern IntPtr ShellExecute(
                    IntPtr hwnd,
                    string lpOperation,
                    string lpFile,
                    string lpParameters,
                    string lpDirectory,
                    int nShowCmd);

                // 定义窗口显示方式常量
                private const int SW_SHOWNORMAL = 1;

                /// <summary>
                /// 打开“桌面图标设置”窗口
                /// </summary>
                public static void OpenDesktopIconSettings()
                {
                    try
                    {
                        // 使用 rundll32.exe 打开“桌面图标设置”窗口
                        ShellExecute(
                            IntPtr.Zero,                     // 父窗口句柄
                            "open",                          // 操作类型
                            "rundll32.exe",                  // 文件名
                            "shell32.dll,Control_RunDLL desk.cpl,,0", // 参数
                            null,                            // 工作目录
                            SW_SHOWNORMAL);                  // 窗口显示方式

                        WriteLog.Info("已打开“桌面图标设置”窗口。");
                    }
                    catch (Exception ex)
                    {
                        WriteLog.Info($"打开“桌面图标设置”窗口时出错: {ex.Message}");
                    }
                }
            }
            /// <summary>
            /// 用于管理此电脑中系统文件夹的可见性的类
            /// </summary>
            public class ThisPCFolders
            {
                /// <summary>
                /// 设置在此电脑中的“3D 对象”文件夹可见性
                /// </summary>
                public static void _3DObject(bool visibility)
                {
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{31C0DD25-9439-4F12-BF41-7FF4EDA38722}", true).CreateSubKey("PropertyBag").Close();
                    SetFolderVisibility(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{31C0DD25-9439-4F12-BF41-7FF4EDA38722}", visibility ? "Show" : "Hide");
                }
                /// <summary>
                /// 设置在此电脑中的“桌面”文件夹可见性
                /// </summary>
                /// <param name="visibility"> 可见性</param>
                public static void Desktop(bool visibility)
                {
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", true).CreateSubKey("PropertyBag").Close();
                    SetFolderVisibility(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", visibility ? "Show" : "Hide");
                }
                /// <summary>
                /// 设置在此电脑中的“文档”文件夹可见性
                /// </summary>
                /// <param name="visibility"> 可见性</param>
                public static void Documents(bool visibility)
                {
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{f42ee2d3-909f-4907-8871-4c22fc0bf756}", true).CreateSubKey("PropertyBag").Close();
                    SetFolderVisibility(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{f42ee2d3-909f-4907-8871-4c22fc0bf756}", visibility ? "Show" : "Hide");
                }
                /// <summary>
                /// 设置在此电脑中的“下载”文件夹可见性
                /// </summary>
                /// <param name="visibility"> 可见性</param>
                public static void Downloads(bool visibility)
                {
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{7d83ee9b-2244-4e70-b1f5-5393042af1e4}", true).CreateSubKey("PropertyBag").Close();
                    SetFolderVisibility(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{7d83ee9b-2244-4e70-b1f5-5393042af1e4}", visibility ? "Show" : "Hide");
                }
                /// <summary>
                /// 设置在此电脑中的“音乐”文件夹可见性
                /// </summary>
                /// <param name="visibility"> 可见性</param>
                public static void Music(bool visibility)
                {
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{a0c69a99-21c8-4671-8703-7934162fcf1d}", true).CreateSubKey("PropertyBag").Close();
                    SetFolderVisibility(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{a0c69a99-21c8-4671-8703-7934162fcf1d}", visibility ? "Show" : "Hide");
                }
                /// <summary>
                /// 设置在此电脑中的“图片”文件夹可见性
                /// </summary>
                /// <param name="visibility"></param>
                public static void Pictures(bool visibility)
                {
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{0ddd015d-b06c-45d5-8c4c-f59713854639}", true).CreateSubKey("PropertyBag").Close();
                    SetFolderVisibility(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{0ddd015d-b06c-45d5-8c4c-f59713854639}", visibility ? "Show" : "Hide");
                }
                /// <summary>
                /// 设置在此电脑中的“视频”文件夹可见性
                /// </summary>
                /// <param name="visibility"> 可见性</param>
                public static void Videos(bool visibility)
                {
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{35286a68-3c57-41a1-bbb1-0eae73d76c95}", true).CreateSubKey("PropertyBag").Close();
                    SetFolderVisibility(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{35286a68-3c57-41a1-bbb1-0eae73d76c95}", visibility ? "Show" : "Hide");
                }
            }
            /// <summary>
            /// 一个用于修改右键菜单的类，提供了静态方法 <see cref="Windows10Menu"/>和<see cref="Windows11Menu"/>来控制右键菜单。
            /// </summary>
            public class RightClickMenu
            {
                /// <summary>
                /// 切换 Windows 10 右键菜单
                /// </summary>
                public static void Windows10Menu()
                {
                    try
                    {
                        // 切换 Windows 10 右键菜单
                        Registry.Write(@"HKEY_CURRENT_USER\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32", "", "", RegistryValueKind.String);
                    }
                    catch (Exception e)
                    {
                        MessageBox_I.Error(e.Message, _ERROR);
                    }
                }
                /// <summary>
                /// 恢复 Windows 11 右键菜单
                /// </summary>
                public static void Windows11Menu()
                {
                    try
                    {
                        // 恢复 Windows 11 右键菜单
                        Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}");
                    }
                    catch (Exception e)
                    {
                        MessageBox_I.Error(e.Message, _ERROR);
                    }
                }
            }
            /// <summary>
            /// 一个用于修改默认访问位置的类，提供了静态方法 <see cref="ThisPC"/>和<see cref="QuickAcess"/>来控制默认打开的文件夹。
            /// </summary>
            public class ExplorerLaunchTo
            {
                /// <summary>
                /// 设置资源管理器默认打开位置为此电脑
                /// </summary>
                public static void ThisPC()
                {
                    Switch(1);
                }
                /// <summary>
                /// 设置资源管理器默认打开位置为快速访问
                /// </summary>
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
                            WriteLog.Info(_SUCESS_WRITE_REGISTRY + keyPath);
                            RefreshExplorer();
                            return;
                        }
                        else
                        {
                            MessageBox_I.Error("无法访问注册表！", _ERROR);
                            return;
                        }
                    }
                }

            }
            /// <summary>
            /// 在资源管理器中打开指定路径的文件夹
            /// </summary>
            /// <param name="path">要打开的路径</param>
            public static void OpenFolderInExplorer(string path)
            {
                try
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        MessageBox_I.Error(_value_Not_Is_NullOrEmpty("path"), _ERROR);
                        return;
                    }
                    // 检查路径是否存在
                    if (!Directory.Exists(path) && !System.IO.File.Exists(path))
                    {
                        MessageBox_I.Error($"路径不存在: {path}", _ERROR);
                        return;
                    }

                    // 检查路径是否是一个没有后缀名的文件
                    if (System.IO.File.Exists(path))
                    {
                        string extension = Path.GetExtension(path);
                        if (string.IsNullOrEmpty(extension))
                        {
                            MessageBox_I.Error(_value_Not_Is_NullOrEmpty("extension"), _ERROR);
                            return;
                        }
                    }

                    // 打开文件夹
                    if (Directory.Exists(path))
                    {
                        Process.Start("explorer.exe", path);
                    }
                    else
                    {
                        // 如果是文件，打开其所在文件夹并选中文件
                        string folderPath = Path.GetDirectoryName(path);
                        Process.Start("explorer.exe", $"/select,{path}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox_I.Error($"打开文件夹时出错: {ex.Message}", _ERROR);
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
                WriteLog.Info($"{_PROCESS_STARTED}: {Sleep.Id}");
                Sleep.WaitForExit();
                WriteLog.Info($"{_PROCESS_EXITED}: {Sleep.ExitCode}");
                if (Sleep.ExitCode != 0)
                {
                    WriteLog.Error(LogKind.System, $"{_CANNOT_DISENABLE_HIBERNATE}: {Sleep.ExitCode}");
                }
                else
                {
                    WriteLog.Info(LogKind.System, $"{_DISENABLE_HIBERNATE}");
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
            WriteLog.Info($"{_PROCESS_STARTED}: {p.Id}");
            p.WaitForExit();
            WriteLog.Info($"{_PROCESS_EXITED}: {p.ExitCode}");
            if (p.ExitCode != 0)
            {
                WriteLog.Error($"{_CANNOT_ENABLE_HIGHPOWERCFG}: {p.ExitCode}");
            }
            else
            {
                WriteLog.Info(LogKind.Process, $"{_ENABLE_HIGHPOWERCFG}");
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
                if (Security.Is360SafeRunning() || Security.IsHuorongSecurityRunning())
                {
                    DialogResult dialogResult = MessageBox.Show($"{_SECURITY_RUNNING}", $"{_WARNING}", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    if (dialogResult != DialogResult.OK)
                    {
                        return;
                    }
                }
                RegistryKey key;
                try
                {
                    // 修改 HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender
                    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender");
                    WriteLog.Info($"{_WRITE_REGISTRY}");
                    key.SetValue("DisableAntiSpyware", value, RegistryValueKind.DWord);  // 禁用防间谍软件
                    WriteLog.Info($"{_SUCESS_WRITE_REGISTRY}");
                    key.Close();  // 关闭注册表项

                    // 修改 HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection
                    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection");
                    WriteLog.Info($"{_WRITE_REGISTRY}");
                    key.SetValue("DisableBehaviorMonitoring", value, RegistryValueKind.DWord);  // 禁用行为监控
                    WriteLog.Info($"{_SUCESS_WRITE_REGISTRY}");
                    WriteLog.Info($"{_WRITE_REGISTRY}");
                    key.SetValue("DisableIOAVProtection", value, RegistryValueKind.DWord);  // 禁用文件扫描
                    WriteLog.Info($"{_SUCESS_WRITE_REGISTRY}");
                    WriteLog.Info($"{_WRITE_REGISTRY}");
                    key.SetValue("DisableOnAccessProtection", value, RegistryValueKind.DWord);  // 禁用访问保护
                    WriteLog.Info($"{_SUCESS_WRITE_REGISTRY}");
                    WriteLog.Info($"{_WRITE_REGISTRY}");
                    key.SetValue("DisableRealtimeMonitoring", value, RegistryValueKind.DWord);  // 禁用实时监控
                    WriteLog.Info($"{_SUCESS_WRITE_REGISTRY}");
                    key.Close();  // 关闭注册表项

                    // 修改 HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SecurityHealthService
                    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\SecurityHealthService");
                    WriteLog.Info($"{_WRITE_REGISTRY}");
                    key.SetValue("Start", value + 2, RegistryValueKind.DWord);  // 设置服务启动类型为2自动 3手动 
                    WriteLog.Info($"{_SUCESS_WRITE_REGISTRY}");
                    key.Close();
                    // 关闭注册表项
                }
                catch
                {
                    WriteLog.Error($"{_WRITE_REGISTRY_FAILED}");
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
            /// 检查 Windows 更新服务的状态
            /// </summary>
            public static bool CheckStatus()
            {
                RegistryKey key;
                try
                {
                    // 读取 HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU
                    key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU");
                    if (key != null)
                    {
                        int value = (int)key.GetValue("NoAutoUpdate", 0);
                        if (value == 1)
                        {
                            WriteLog.Info($"{_WINDOWS_UPDATER_DISABLED}");
                            key.Close();
                            return false;
                        }
                        else
                        {
                            WriteLog.Info($"{_WINDOWS_UPDATER_ENABLED}");
                            key.Close();
                            return true;
                        }
                    }
                    else
                    {
                        WriteLog.Info($"{_WINDOWS_UPDATER_ENABLED}");
                        return false;
                    }
                }
                catch
                {
                    WriteLog.Error($"{_READ_REGISTRY_FAILED}");
                    Thread.Sleep(1000);
                    return false;
                }

            }
        }
        /// <summary>
        /// 用于激活 Windows 系统，首先检查安全软件的状态，然后启动指定的激活程序。
        /// </summary>
        public static void ActiveWindows()//Windows激活
        {
            Process powerShell = new Process();
            powerShell.StartInfo.FileName = "powershell.exe";
            powerShell.StartInfo.Arguments = "irm https://get.activated.win | iex";
            powerShell.Start();
            WriteLog.Info(LogKind.Process, $"{_PROCESS_STARTED}: {powerShell.Id}");
            powerShell.WaitForExit();
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
                            MessageBox_I.Info(_SUCCESS_VERIFY, _TIPS);
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
                            MessageBox_I.Error($"{_LOGIN_VERIFY_ERROR}（{_ERROR_CODE}：{errorCode} {ExtraMessage}）", _ERROR);
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
                WriteLog.Info(_SUCCESS_VERIFY);
                return true;
            }
            else
            {
                WriteLog.Info(_CANCEL_OP);
                return false;
            }
        }//Windows安全中心身份验证
        #endregion
    }
}