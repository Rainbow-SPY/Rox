using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    public partial class Windows
    {
        /// <summary>
        /// 用于显示一个凭据提示框以验证用户身份
        /// </summary>
        /// <returns> <see langword="true"/> 表示验证成功，<see langword="false"/> 表示验证失败。</returns>
        public static bool Authentication()
        {
            string _au = "Windows Authentication";
            try
            {
                Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Lsa", true)?.SetValue("limitblankpassworduse", 0, RegistryValueKind.DWord);
                WriteLog.Info(_au, "空密码登录限制已成功禁用！");
            }
            catch (UnauthorizedAccessException)
            {
                WriteLog.Error(_au, "错误：请以管理员身份运行此程序！");
            }
            catch (Exception ex)
            {
                WriteLog.Error(_au, "修改失败：" + _Exception_With_xKind("Windows.Authentication", ex));
            }
            CREDUI_INFO credUI = new CREDUI_INFO
            {
                cbSize = Marshal.SizeOf(typeof(CREDUI_INFO)),
                pszCaptionText = _LOGIN_VERIFY,
                pszMessageText = $"{_ENTER_CREDENTIALS}\n" +
                                $"1. 本地账户：Administrator（无密码请留空密码框）\n" +
                                $"2. Microsoft账户：完整邮箱（如xxx@outlook.com）\n" +
                                $"当前计算机名：{Environment.MachineName}",
                hwndParent = IntPtr.Zero,
                hbmBanner = IntPtr.Zero
            };

            bool isAuthenticated = false;
            bool userCancelled = false;
            string logAccount = "用户未尝试验证";

            do
            {
                uint authPackage = 0;
                bool save = false;
                int flags = 0x1;  //| 0x20 | 0x8000; // CREDUIWIN_GENERIC + ALLOW_EMPTY_PASSWORD + FORCE_PROMPT（强制显示窗口）

                // 调用验证窗口（捕获API返回值，排查错误）
                int result = CredUIPromptForWindowsCredentials(
                    ref credUI,
                    0,
                    ref authPackage,
                    IntPtr.Zero,
                    0,
                    out IntPtr outCredBuffer,
                    out uint outCredBufferSize,
                    ref save,
                    flags);

                // 打印API返回值（用于调试：正常弹出窗口并点击取消返回1223，窗口创建失败返回其他值）
                WriteLog.Info(_au, $"CredUIPromptForWindowsCredentials 返回值：{result}");

                if (result == 0) // 用户点击确定
                {
                    int maxUserName = 256;
                    int maxDomainName = 256;
                    int maxPassword = 256;
                    StringBuilder userNameSb = new StringBuilder(maxUserName);
                    StringBuilder domainNameSb = new StringBuilder(maxDomainName);
                    StringBuilder passwordSb = new StringBuilder(maxPassword);

                    bool unpackSuccess = CredUnPackAuthenticationBuffer(
                        0,
                        outCredBuffer,
                        outCredBufferSize,
                        userNameSb,
                        ref maxUserName,
                        domainNameSb,
                        ref maxDomainName,
                        passwordSb,
                        ref maxPassword);

                    if (unpackSuccess)
                    {
                        string actualUserName = userNameSb.ToString().Trim();
                        string actualDomain = domainNameSb.ToString().Trim();
                        string actualPassword = passwordSb.ToString().Trim();
                        logAccount = $"{actualDomain}\\{actualUserName}";

                        bool isLocalEmptyPwd = string.IsNullOrEmpty(actualPassword) &&
                                             (string.IsNullOrEmpty(actualDomain) || actualDomain.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase));
                        bool isMicrosoftAccount = actualUserName.Contains("@") && string.IsNullOrEmpty(actualDomain);

                        if (isMicrosoftAccount)
                        {
                            actualDomain = "MicrosoftAccount";
                            logAccount = $"{actualDomain}\\{actualUserName}";
                        }
                        else if (string.IsNullOrEmpty(actualDomain))
                        {
                            actualDomain = Environment.MachineName;
                            logAccount = $"{actualDomain}\\{actualUserName}";
                        }

                        int logonType = isLocalEmptyPwd ? 4 : 2;
                        bool isValid = LogonUser(
                            actualUserName,
                            actualDomain,
                            actualPassword,
                            logonType,
                            0,
                            out IntPtr userToken);

                        if (isValid)
                        {
                            isAuthenticated = true;
                            CloseHandle(userToken);
                        }
                        else
                        {
                            int errorCode = Marshal.GetLastWin32Error();
                            string extraMessage = _UNKNOW_ERROR;
                            switch (errorCode)
                            {
                                case 1326:
                                    extraMessage = _LOGIN_ERROR_USER_OR_PASSWORD;
                                    break;
                                case 1385:
                                    extraMessage = _LOGIN_ERROR_1385;
                                    break;
                                case 1793:
                                    extraMessage = _LOGIN_ERROR_ACCOUNT_EXPIRED;
                                    break;
                                case 1382:
                                    extraMessage = _LOGIN_ERROR_1382;
                                    break;
                                case 1383:
                                    extraMessage = _LOGIN_ERROR_1383;
                                    break;
                            }

                            MessageBox.Show($"{_LOGIN_VERIFY_ERROR}\n错误码：{errorCode}\n原因：{extraMessage}\n{_TIP_EMPTY_PASSWORD_POLICY}", _ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            WriteLog.Error(_au, $"{_LOGIN_VERIFY_ERROR}\n错误码：{errorCode}\n原因：{extraMessage}\n{_TIP_EMPTY_PASSWORD_POLICY}");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"{_CREDENTIAL_PARSE_ERROR}\n{_TIP_MICROSOFT_ACCOUNT}", _ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }

                    CoTaskMemFree(outCredBuffer);
                }
                else if (result == 1223) // 明确的用户取消（API标准返回值）
                {
                    userCancelled = true;
                }
                else // 窗口创建失败（返回非0非1223）
                {
                    MessageBox.Show($"{_WINDOW_CREATE_ERROR}\nAPI返回错误码：{result}", _ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    userCancelled = true; // 无法创建窗口，直接退出
                }
            } while (!isAuthenticated && !userCancelled);

            // 日志记录
            if (isAuthenticated)
            {
                WriteLog.Info($"{_SUCCESS_VERIFY} - 账户：{logAccount}");
                return true;
            }
            else
            {
                WriteLog.Info($"{_CANCEL_OP} - 最后验证账户：{logAccount}");
                return false;
            }
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CREDUI_INFO
        {
            public int cbSize;
            public IntPtr hwndParent;
            public string pszMessageText;
            public string pszCaptionText;
            public IntPtr hbmBanner;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
        private static extern void CoTaskMemFree(IntPtr ptr);

        [DllImport("credui.dll", CharSet = CharSet.Unicode, EntryPoint = "CredUIPromptForWindowsCredentialsW")]
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

        [DllImport("credui.dll", CharSet = CharSet.Unicode, EntryPoint = "CredUnPackAuthenticationBufferW")]
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



        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken);
    }
}