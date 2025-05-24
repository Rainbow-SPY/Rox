using Microsoft.Win32;
using Rox.Runtimes;
using Rox.Text;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    /// <summary>
    /// 检测特定安全软件是否在运行
    /// </summary>
    public class Security
    {
        /// <summary>
        /// 检测360安全卫士是否在运行
        /// </summary>
        /// <returns>运行返回 <see langword="true"></see> 未运行返回 <see langword="false"></see></returns>
        public static bool Anti360Security()
        {
            Process[] processes = Process.GetProcessesByName("360Tray");
            if (processes.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 检测火绒安全软件是否在运行
        /// </summary>
        /// <returns> 运行返回 <see langword="true"></see> 未运行返回 <see langword="false"></see></returns>
        public static bool AntiHuoRongSecurity()
        {
            Process[] processes = Process.GetProcessesByName("HipsTray");
            if (processes.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public class WindowsSecurity
        {
            public static void Enable()
            {
                Windows.WindowsSecurityCenter.Enable();
            }

            public static void Disable()
            {
                Windows.WindowsSecurityCenter.Disable();
            }
        }
    }
    /// <summary>
    /// 用于处理注册表操作
    /// </summary>
    public class Registry
    {
        /// <summary>
        /// 用于写入注册表项的值
        /// </summary>
        /// <param name="keyPath"> 注册表项路径</param>
        /// <param name="valueName"> 注册表项名称</param>
        /// <param name="valueType"> 注册表项类型</param>
        /// <param name="valueData"> 注册表项数据</param>
        public static void Write(string keyPath, string valueName, object valueData, RegistryValueKind valueType)
        {
            try
            {
                // 打开注册表项
                using (RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    // 写入值
                    WriteLog(LogLevel.Info, $"{_WRITE_REGISTRY}");
                    key.SetValue(valueName, valueData, valueType);
                    key.Close();
                }
                LogLibraries.WriteLog(LogLibraries.LogLevel.Info, $"{_SUCESS_WRITE_REGISTRY}");
            }
            catch (Exception ex)
            {
                LogLibraries.WriteLog(LogLibraries.LogLevel.Error, $"{_WRITE_REGISTRY_FAILED}: {ex.Message}");
            }
        }
        /// <summary>
        /// 用于读取注册表项的值
        /// </summary>
        /// <param name="keyName"> 注册表项路径</param>
        /// <param name="valueName"> 注册表项名称</param>
        /// <returns> 返回注册表项的值</returns>
        internal static string GetRegistryValue(string keyName, string valueName)
        {
            string value = "";
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(keyName))
            {
                if (key != null)
                {
                    value = key.GetValue(valueName) as string;
                    key.Close();
                }
            }
            return value;
        }
    }
    /// <summary>
    /// 用于处理文件操作
    /// </summary>
    public class File
    {
        /// <summary>
        /// 用于处理文件的加密和解密操作
        /// </summary>
        public class AESEncryption
        {
            /// <summary>
            /// 用于使用指定的 256 位密钥和 128 位初始化向量对给定的明文字符串进行 AES 加密，并返回加密后的字符串。
            /// </summary>
            /// <param name="plainText"> 要加密的明文字符串</param>
            /// <param name="Key"> 256位密钥</param>
            /// <param name="IV"> 128位初始化向量</param>
            /// <returns> 返回加密后的字符串</returns>
            public static string Encrypt(string plainText, byte[] Key/*256-bit*/, byte[] IV/*128-bit*/)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (var msEncrypt = new System.IO.MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }
                            return Convert.ToBase64String(msEncrypt.ToArray());
                        }
                    }
                }
            }// 加密方法
            /// <summary>
            /// 用于使用指定的 256 位密钥和 128  位初始化向量对给定的密文字符串进行 AES 解密，并返回解密后的字符串。
            /// </summary>
            /// <param name="cipherText"> 要解密的密文字符串</param>
            /// <param name="Key"> 256位密钥</param>
            /// <param name="IV"> 128位初始化向量</param>
            /// <returns> 返回解密后的字符串</returns>
            public static string Decrypt(string cipherText, byte[] Key/*256-bit*/, byte[] IV/*128-bit*/)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (var msDecrypt = new System.IO.MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }// 解密方法
        }
        /// <summary>
        /// 定义了文件的厘性选项，包括只读、系统、隐藏和归档。
        /// </summary>

        public enum AtOp
        {
            Readonly,
            System,
            Hidden,
            Archive,
        }
        /// <summary>
        /// 用于设置文件的属性
        /// </summary>
        /// <param name="path"> 文件路径</param>
        /// <param name="Key"> 属性选项</param>
        /// <param name="Switch"> 开关</param>
        public void Attrib(string path, AtOp Key, bool Switch)
        {
            string key;
            if (Switch)
            {
                key = "+";
            }
            else
            {
                key = "-";
            }
            if (Key == AtOp.Readonly)
            {
                string arg = $"{key}r";
                Process process = new Process();
                process.StartInfo.FileName = "attrib";
                process.StartInfo.Arguments = $"{arg} {path}";
                process.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process.Id}");
                process.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process.ExitCode}");
                process.Close();
            }
            if (Key == AtOp.System)
            {
                string arg = $"{key}s";
                Process process = new Process();
                process.StartInfo.FileName = "attrib";
                process.StartInfo.Arguments = $"{arg} {path}";
                process.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process.Id}");
                process.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process.ExitCode}");
                process.Close();
            }
            if (Key == AtOp.Hidden)
            {
                string arg = $"{key}h";
                Process process = new Process();
                process.StartInfo.FileName = "attrib";
                process.StartInfo.Arguments = $"{arg} {path}";
                process.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process.Id}");
                process.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process.ExitCode}");
                process.Close();
            }
            if (Key == AtOp.Archive)
            {
                string arg = $"{key}a";
                Process process = new Process();
                process.StartInfo.FileName = "attrib";
                process.StartInfo.Arguments = $"{arg} {path}";
                process.Start();
                WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process.Id}");
                process.WaitForExit();
                WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process.ExitCode}");
                process.Close();
            }
        }
        /// <summary>
        /// 用于检查文件的哈希值是否与预期的哈希值匹配
        /// </summary>
        /// <param name="filePath"> 文件路径</param>
        /// <param name="expectedMD5"> 预期的MD5哈希值</param>
        /// <returns> 如果哈希值匹配，则返回 <see langword="true"/>，否则返回 <see langword="false"/></returns>
        public static bool CheckFileHash(string filePath, string expectedMD5)
        {
            try
            {
                // 检查文件是否存在
                if (!System.IO.File.Exists(filePath))
                {
                    MessageBox.Show($"文件 {filePath} 不存在。");
                    return false;
                }

                // 计算文件的MD5哈希值
                string actualMD5 = CalculateMD5(filePath);

                // 检查哈希值是否匹配
                if (actualMD5 != expectedMD5)
                {
                    MessageBox.Show($"文件 {filePath} 的MD5哈希值不匹配。\n预期: {expectedMD5}\n实际: {actualMD5}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"检查文件 {filePath} 时发生错误: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// 计算文件的MD5哈希值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>MD5哈希值</returns>
        public static string CalculateMD5(string filePath)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = System.IO.File.OpenRead(filePath))
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
                }
            }
        }
        /// <summary>
        /// 用于加密和解密字符串的 JavaScript 代码,需要在Node.Js环境中运行
        /// </summary>
        private readonly static string NodeJsEnDecryptJavaScript = @"const _0x4b1552=_0x29e3;(function(_0x4e59cd,_0x1dccd2){const _0x248214=_0x29e3,_0x31cb7=_0x4e59cd();while(!![]){try{const _0x271097=-parseInt(_0x248214(0x103))/0x1+-parseInt(_0x248214(0xf7))/0x2+parseInt(_0x248214(0xef))/0x3*(parseInt(_0x248214(0x101))/0x4)+-parseInt(_0x248214(0xe5))/0x5*(-parseInt(_0x248214(0x104))/0x6)+parseInt(_0x248214(0xf4))/0x7*(-parseInt(_0x248214(0xf6))/0x8)+-parseInt(_0x248214(0xf5))/0x9+parseInt(_0x248214(0xe9))/0xa*(parseInt(_0x248214(0xff))/0xb);if(_0x271097===_0x1dccd2)break;else _0x31cb7['push'](_0x31cb7['shift']());}catch(_0x46f0bd){_0x31cb7['push'](_0x31cb7['shift']());}}}(_0x15f9,0x2d30b));const args=process['argv'][_0x4b1552(0xea)](0x2);function parseArgs(_0x496144){const _0x25b410=_0x4b1552,_0x13484f={};for(const _0x21b904 of _0x496144){if(_0x21b904[_0x25b410(0xed)]('-')){const [_0x520905,_0xab69d6]=_0x21b904['replace'](/^-+/,'')[_0x25b410(0xe6)]('=');_0x13484f[_0x520905]=_0xab69d6||!![];}}return _0x13484f;}function _0x29e3(_0xd5a7b6,_0x405ab1){const _0x15f97b=_0x15f9();return _0x29e3=function(_0x29e389,_0x2396f3){_0x29e389=_0x29e389-0xe5;let _0x1e4e56=_0x15f97b[_0x29e389];return _0x1e4e56;},_0x29e3(_0xd5a7b6,_0x405ab1);}function encrypt(_0xac5b6){const _0x122d1b=_0x4b1552;let _0xa35ecc=Buffer[_0x122d1b(0xe8)](_0xac5b6)[_0x122d1b(0xf0)](_0x122d1b(0xfd)),_0x312aaa='';for(let _0x5c910f=0x0;_0x5c910f<_0xa35ecc['length'];_0x5c910f++){const _0x1e27e1=_0xa35ecc[_0x122d1b(0xfc)](_0x5c910f);_0x312aaa+=_0x1e27e1['toString'](0x2)['padStart'](0x8,'0');}let _0x4d9846='';for(let _0x502a9c=0x0;_0x502a9c<_0x312aaa['length'];_0x502a9c++){const _0x7fed9=parseInt(_0x312aaa[_0x502a9c],0xa);_0x4d9846+=_0x7fed9<0x9?(_0x7fed9+0x1)[_0x122d1b(0xf0)]():'0';}const _0x29ee89=Buffer[_0x122d1b(0xe8)](_0x4d9846)[_0x122d1b(0xf0)](_0x122d1b(0xfd));let _0x5b2289='';for(let _0x1e4c82=0x0;_0x1e4c82<_0x29ee89[_0x122d1b(0xe7)];_0x1e4c82++){const _0x2a7211=_0x29ee89[_0x122d1b(0xfc)](_0x1e4c82)['toString'](0x10)[_0x122d1b(0xeb)](0x2,'0');_0x5b2289+=_0x2a7211;}return _0x5b2289;}function decrypt(_0x39a8da){const _0x2f6aa0=_0x4b1552;let _0x2c3cdc='';for(let _0x118530=0x0;_0x118530<_0x39a8da[_0x2f6aa0(0xe7)];_0x118530+=0x2){const _0x4de302=_0x39a8da['substr'](_0x118530,0x2);_0x2c3cdc+=String[_0x2f6aa0(0xee)](parseInt(_0x4de302,0x10));}let _0x5d6345=Buffer['from'](_0x2c3cdc,_0x2f6aa0(0xfd))[_0x2f6aa0(0xf0)](_0x2f6aa0(0xf8)),_0x329f3a='';for(let _0x38b4ba=0x0;_0x38b4ba<_0x5d6345['length'];_0x38b4ba++){const _0x5d24c2=parseInt(_0x5d6345[_0x38b4ba],0xa);_0x329f3a+=_0x5d24c2>0x0?(_0x5d24c2-0x1)['toString']():'9';}let _0x569555='';for(let _0x36ce13=0x0;_0x36ce13<_0x329f3a[_0x2f6aa0(0xe7)];_0x36ce13+=0x8){const _0x8f35a6=_0x329f3a[_0x2f6aa0(0xfa)](_0x36ce13,0x8);_0x569555+=String['fromCharCode'](parseInt(_0x8f35a6,0x2));}const _0x26d914=Buffer[_0x2f6aa0(0xe8)](_0x569555,_0x2f6aa0(0xfd))[_0x2f6aa0(0xf0)](_0x2f6aa0(0x100));return _0x26d914;}function main(){const _0x35c4d5=_0x4b1552,_0x175b13=parseArgs(args);(!_0x175b13[_0x35c4d5(0xec)]||!_0x175b13[_0x35c4d5(0xf3)]&&!_0x175b13['Decrypt'])&&(console[_0x35c4d5(0xfe)](_0x35c4d5(0xf2)),console[_0x35c4d5(0xfe)](_0x35c4d5(0x102)),console[_0x35c4d5(0xfe)]('Example\x20for\x20decryption:\x20node\x201.js\x20-string=\x2248656c6c6f\x22\x20-Decrypt'),process[_0x35c4d5(0xf1)](0x1));const _0x3802c4=_0x175b13[_0x35c4d5(0xec)];let _0xe8125f;if(_0x175b13['Encrypt'])return _0xe8125f=encrypt(_0x3802c4),console['log'](_0x35c4d5(0xfb),_0xe8125f),_0xe8125f;else{if(_0x175b13[_0x35c4d5(0xf9)])return _0xe8125f=decrypt(_0x3802c4),console[_0x35c4d5(0xfe)]('Decrypted\x20result:',_0xe8125f),_0xe8125f;}}function _0x15f9(){const _0x4e4352=['Example\x20for\x20encryption:\x20node\x201.js\x20-string=\x22sk-7656s6c8193hc786ca87sd901h\x22\x20-Encrypt','6777GHufMg','16788MMQLrU','390YMzmye','split','length','from','10060uozIcO','slice','padStart','string','startsWith','fromCharCode','6oYQWAX','toString','exit','Usage:\x20node\x201.js\x20-string=\x22your_string\x22\x20[-Encrypt]\x20[-Decrypt]','Encrypt','16996UTJabQ','1973916VjHyQP','936vmmYDP','360436wXGUNP','ascii','Decrypt','substr','Encrypted\x20result:','charCodeAt','base64','log','4411IzgmHn','utf8','507688uMJLZZ'];_0x15f9=function(){return _0x4e4352;};return _0x15f9();}main();";

        public static void EncryptString(string str)
        {
            string jsPath = $"{Directory.GetCurrentDirectory()}\\temp\\encrypt.js";
            if (!System.IO.File.Exists(jsPath))
            {
                WriteJavaScriptOnTemp();
            }
            Process process = new Process();
            process.StartInfo.FileName = "node";
            process.StartInfo.Arguments = $"{jsPath} -string={str} -Encrypt";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.WaitForExit();
            process.Close();
        }
        //初始化操作
        private static void WriteJavaScriptOnTemp()
        {
            string jsPath = $"{Directory.GetCurrentDirectory()}\\temp\\encrypt.js";
            if (!System.IO.File.Exists(jsPath))
            {
                System.IO.File.Create(jsPath).Close();
            }
            using (StreamWriter sw = new StreamWriter(jsPath, true, Encoding.Default))
            {
                sw.Write(NodeJsEnDecryptJavaScript);
                sw.Close();
            }
        }

    }
    /// <summary>
    /// 用于更新软件
    /// </summary>
    public class Update
    {
        /// <summary>
        /// 用于根据加载状态创建或更新一个批处理文件，以便在 NinjaMagisk 自动更新过程中执行特定命令
        /// </summary>
        /// <param name="Loading"> 加载状态</param>
        /// <param name="code"> 命令</param>
        private static void BatchWriter(bool Loading, string code)
        {
            string batPath = $"{Directory.GetCurrentDirectory()}\\temp\\update.bat";
            if (!System.IO.File.Exists(batPath))
            {
                System.IO.File.Create(batPath).Close();
            }
            if (Loading)
            {
                ClearFile(batPath);
                string command = $"@echo off&echo ==================================&echo =   NinjaMagisk Auto Updater     =&echo =                                =&echo =   Version 0.0.1                =&echo ==================================&cd /d {AppDomain.CurrentDomain.BaseDirectory}\n";
                using (StreamWriter sw = new StreamWriter(batPath, true, Encoding.Default))
                {
                    sw.Write(command);
                    sw.Close();
                }
                return;
            }
            // copy /y C:\123.r D:\123.r & start D:\123.r & pause
            using (StreamWriter sw = new StreamWriter(batPath, true, Encoding.Default))
            {
                sw.WriteLine(code);
                sw.Close();
            }
            return;
        }
        /// <summary>
        /// 用于检查更新, 比较两个版本号字符串并返回更新的版本或指示版本相同
        /// </summary>
        /// <param name="version1"> 版本1</param>
        /// <param name="version2"> 版本2</param>
        /// <returns> 返回更新的版本或指示版本相同</returns>
        internal static string NewerVersions(string version1, string version2)
        {
            // 将版本号按小数点分割成数组
            string[] v1Parts = version1.Split('.');
            string[] v2Parts = version2.Split('.');

            // 获取两个版本号的最大长度
            int maxLength = Math.Max(v1Parts.Length, v2Parts.Length);

            for (int i = 0; i < maxLength; i++)
            {
                // 如果当前部分超出数组范围，则补0
                int v1Part = i < v1Parts.Length ? int.Parse(v1Parts[i]) : 0;
                int v2Part = i < v2Parts.Length ? int.Parse(v2Parts[i]) : 0;

                // 比较当前部分的大小
                if (v1Part > v2Part)
                {
                    return version1; // version1 更新
                }
                else if (v1Part < v2Part)
                {
                    return version2; // version2 更新
                }
            }

            // 如果所有部分都相同，则版本号相同
            return "same";
        }
        /// <summary>
        /// 发送HTTP请求的HttpClient实例
        /// </summary>
        private static readonly HttpClient _httpClient = new HttpClient();
        /// <summary>
        /// 用于检查更新的平台
        /// </summary>
        public enum Platform
        {
            /// <summary>
            /// Github
            /// </summary>
            Github,
            /// <summary>
            /// Gitee
            /// </summary>
            Gitee,
        }
        /// <summary>
        /// 用于检查更新
        /// </summary>
        /// <param name="CheckUpdateUrl"> 检查更新的URL</param>
        /// <param name="platform"> 平台</param>
        /// <returns> 如果有新版本可用，则返回 <see langword="true"/>，否则返回 <see langword="false"/></returns>
        public static async Task<bool> CheckUpdate(string CheckUpdateUrl, Platform platform)
        {
            /* Github API规定的Release最新发行版查询地址为       https://api/github.com/repos/{用户名}/{仓库}/releases/latest
             * 
             * Gitee API规定的Release最新发行版查询地址为       https://gitee.com/api/v5/repos/{用户名}/{仓库}/releases/latest
             * 
             * 返回的json中包含了最新发行版的信息，包括版本号、发布时间、下载地址等 例如,最新的版本号为 "tag_name": "v1.4",
             */
            string jsonResponse = await FetchJsonFromUrl(CheckUpdateUrl);
            if (!string.IsNullOrEmpty(jsonResponse))
            {
                var (TagName, Name) = ExtractTagAndName(jsonResponse, platform);
                WriteLog(LogLevel.Info, $"{_LATEST_VERSION}: {TagName} - {Name}");
                string[] strings = TagName.Split('v');
                string res = NewerVersions(LocalizedString.Version, strings[1]);
                if (res == "same" || res == LocalizedString.Version)
                {
                    WriteLog(LogLevel.Info, _NON_NEW_VER);
                    return false;
                }
                else
                {
                    WriteLog(LogLevel.Info, $"{_NEW_VERSION_AVAILABLE}: {res} {_CURRENT_VER}: {LocalizedString.Version}");
                    return true;
                }
            }
            else
            {
                WriteLog(LogLevel.Error, _JSON_PARSING_FAILED);
                return false;
            }
        }
        /// <summary>
        /// 用于从指定的URL获取<see cref="Json"/> 数据,并在请求失败时返回<see langword="null"/>
        /// </summary>
        /// <param name="CheckUpdateUrl"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static string GetUpdateJson(string CheckUpdateUrl, Platform platform)
        {
            try
            {
                string jsonResponse = FetchJsonFromUrl(CheckUpdateUrl).Result;
                var (TagName, Name) = ExtractTagAndName(jsonResponse, platform);
                WriteLog(LogLevel.Info, $"{_LATEST_VERSION}: {TagName} - {Name}");
                string strings1 = $"{TagName};{Name}";
                return strings1;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }
        /// <summary>
        /// 用于从指定的URL获取<see cref="Json"/> 数据,并在请求失败时返回<see langword="null"/>
        /// </summary>
        /// <param name="url"> URL</param>
        /// <returns> 返回<see cref="Json"/>字符串</returns>
        private static async Task<string> FetchJsonFromUrl(string url)
        {
            try
            {
                // GitHub API 需要 User-Agent 头
                if (url.Contains("github.com") && !_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
                {
                    _httpClient.DefaultRequestHeaders.Add("User-Agent", "C# HttpClient");
                }

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // 确保请求成功
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                WriteLog(LogLevel.Error, $"{_ERROR}: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// 用于提取标签和名称
        /// </summary>
        /// <param name="json">  <see cref="Json"/> 格式的数据</param>
        /// <param name="platform"> 平台</param>
        /// <returns> 返回标签和名称</returns>
        private static (string TagName, string Name) ExtractTagAndName(string json, Platform platform)
        {
            try
            {
                // 解析 JSON
                Json.JObject jsonObject = Json.JObject.Parse(json);

                // 提取 tag_name
                string tagName = jsonObject["tag_name"]?.ToString();

                // 根据平台提取 name
                string name;
                switch (platform)
                {
                    case Platform.Github:
                        name = jsonObject["name"]?.ToString(); // GitHub 的 name 在根节点
                        break;
                    case Platform.Gitee:
                        name = jsonObject["name"]?.ToString(); // Gitee 的 name 在 prerelease 对象中
                        break;
                    default:
                        throw new ArgumentException(_UNSUPPORT_PLATFORM);
                }
                return (tagName, name);
            }
            catch (Exception ex)
            {
                WriteLog(LogLevel.Error, $"{_JSON_PARSING_FAILED}: {ex.Message}");
                return (null, null);
            }
        }
        /*        
    规定 在压缩包内包含了 `update.ini` 和 `filehash.ini` 文件,以及更新文件

    {version} 为版本号

    ```
    压缩包文件目录:
    Update_{ version}.zip          // 更新文件压缩包    
    ├── update.ini             // 更新信息    
    ├── filehash.ini           // 文件哈希值        
    └── #(update files)        // 更新文件 
    ```

    规定 ``update.ini`` 规格:
    ```
    1 > version = ""                              // 版本号     
    2 > type = [Release / HotFix / bugFix]        // 更新类型       
    3 > description = ""                          // 更新说明        
    4 > updatefilecount = ""                      // 更新文件数量       
    5 > hashurl = ""                              // 哈希值文件下载地址       
    6 > hash = ""                                 // 文件数量 
    ```

    规定 ``filehash.ini`` 规格:
    ```
    > { fileName},{ fileHash}
    示例:
    1 > Library.dll,4CC1ED4D70DFC8A7455822EC8339D387
    2 > Library.pdb, FDFA7596701DCC2E96D462DBC35E7823
    ```           
        */
        /// <summary>
        /// 用于处理应用程序的自我更新，包括创建更新目录和选择更新文件。
        /// </summary>
        public static void SelfUpdater()
        {
            try
            {
                string updateDir = Path.Combine(Directory.GetCurrentDirectory(), "update");
                if (!Directory.Exists(updateDir))
                {
                    Directory.CreateDirectory(updateDir);
                    WriteLog(LogLevel.Info, $"{_CREATE_DIRECTORY}: {updateDir}");
                }
                var temp = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp";
                var update = $"{Directory.GetCurrentDirectory()}\\update";
                string batPath = $"{Directory.GetCurrentDirectory()}\\temp\\update.bat";
                OpenFileDialog fileDialog = new OpenFileDialog
                {
                    DefaultExt = "",
                    Title = "选择一个更新文件",
                    Filter = "更新文件压缩包(*.zip)|*.zip"
                };
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {

                    string UpdateFile = fileDialog.FileName;
                    WriteLog(LogLevel.Info, $"Selected update file: {UpdateFile}");
                    if (CheckFilesInArchive(UpdateFile))
                    {
                        WriteLog(LogLevel.Info, $"Archive Passed");
                        string path = ExtractUpdateFiles(update, UpdateFile);
                        if (string.IsNullOrEmpty(path))
                        {
                            WriteLog(LogLevel.Error, "Failed to extract update files.");
                            return;
                        }

                        WriteLog(LogLevel.Info, $"Files extracted to: {path}");
                        // 读取 filehash.ini 文件
                        string updateIniPath = Path.Combine(path, "filehash.ini");
                        if (!System.IO.File.Exists(updateIniPath))
                        {
                            WriteLog(LogLevel.Error, $"update.ini not found in: {path}");
                            return;
                        }

                        string[] lines = System.IO.File.ReadAllLines(updateIniPath);
                        WriteLog(LogLevel.Info, $"Reading filehash.ini from: {updateIniPath}");
                        BatchWriter(true, "");
                        foreach (string line in lines)
                        {
                            WriteLog(LogLevel.Info, "Reading line: " + line);
                            string[] parts = line.Split(',');
                            if (parts.Length < 2)
                            {
                                WriteLog(LogLevel.Error, $"Invalid line in filehash.ini: {line}");
                                break;
                            }
                            string fileName = parts[0].Trim();
                            string expectedHash = parts[1].Trim();

                            BatchWriter(false, $"copy /y {Directory.GetCurrentDirectory()}\\update\\{fileName} {Directory.GetCurrentDirectory()}");
                            string filePath = Path.Combine(path, fileName);
                            string destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "update", fileName);

                            WriteLog(LogLevel.Info, $"Verifying file: {filePath}, Expected Hash: {expectedHash}");

                            // 验证文件哈希值

                            if (Rox.File.CheckFileHash(filePath, expectedHash))
                            {
                                WriteLog(LogLevel.Info, $"File {fileName} Passed");

                                // 确保目标目录存在
                                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                                // 移动文件
                                System.IO.File.Move(filePath, destinationPath);
                                WriteLog(LogLevel.Info, $"File {fileName} moved to: {destinationPath}");
                            }
                            else
                            {
                                WriteLog(LogLevel.Error, $"File {fileName} Failed");
                                MessageBox.Show($"{_ERROR}: The file {fileName} did not pass MD5 verification.", _ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            }
                        }
                        BatchWriter(false, $"echo     Update successful!&pause&start {Application.ExecutablePath}");
                        Process.Start(batPath);
                        Application.Exit();
                    }
                    else
                    {
                        WriteLog(LogLevel.Error, $"Archive Failed");
                        MessageBox.Show($"{_ERROR}: The archive not passed structure verify.", _ERROR, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogLevel.Error, $"{_ERROR}: {ex.Message}");
            }
        }
        /// <summary>
        /// 用于提取更新文件
        /// </summary>
        /// <param name="path"> 更新文件的储存路径</param>
        /// <param name="UpdateFile"> 更新文件</param>  
        /// <returns></returns>
        private static string ExtractUpdateFiles(string path, string UpdateFile)
        {
            Process process = new Process();
            process.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\7za.exe";
            process.StartInfo.Arguments = $"x \"{UpdateFile}\" -o{path} -y -aoa";
            process.Start();
            process.WaitForExit();
            // 获取解压后的文件夹路径
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (file.Contains("update.ini"))
                {
                    return Path.GetDirectoryName(file);
                }
            }
            return null;
        }
        /// <summary>
        /// 用于检查压缩包中是否包含指定的文件
        /// </summary>
        /// <param name="UpdateFile"> 更新文件</param>
        /// <returns> 如果包含指定的文件，则返回 <see langword="true"/>，否则返回 <see langword="false"/></returns>
        private static bool CheckFilesInArchive(string UpdateFile)
        {
            string[] filesToCheck = { "update.ini", "filehash.ini" };
            DownloadAssistant.ModuleDownloader(DownloadAssistant.Module.zip);
            Process process = new Process();
            process.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\7za.exe";
            process.StartInfo.Arguments = $"l \"{UpdateFile}\"";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            StreamReader streamReader = process.StandardOutput;
            string output = streamReader.ReadToEnd();
            foreach (string file in filesToCheck)
            {
                if (output.Contains(file))
                {
                    process.Close();
                    return true;
                }
            }
            if (process.ExitCode != 0)
            {
                WriteLog(LogLevel.Error, $"{_ERROR}: {process.ExitCode}");
                process.Close();
                return false;
            }
            process.Close();
            return false;
        }
    }
}