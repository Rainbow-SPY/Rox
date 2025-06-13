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
        /// <summary>
        /// 开启或关闭 Windows 安全中心 , 此方法重定向到 <see cref="Windows.WindowsSecurityCenter.Enable()"/>  和 <see cref="Windows.WindowsSecurityCenter.Disable()"/>
        /// </summary>
        public class WindowsSecurity
        {
            /// <summary>
            /// 开启 Windows 安全中心 , 此方法重定向到 <see cref="Windows.WindowsSecurityCenter.Enable()"/>
            /// </summary>
            public static void Enable()
            {
                Windows.WindowsSecurityCenter.Enable();
            }
            /// <summary>
            /// 关闭 Windows 安全中心 , 此方法重定向到 <see cref="Windows.WindowsSecurityCenter.Disable()"/>
            /// </summary>
            public static void Disable()
            {
                Windows.WindowsSecurityCenter.Disable();
            }
        }
    }
    /// <summary>
    /// 用于处理文件操作
    /// </summary>
    public class File
    {
        /// <summary>
        /// 定义了文件的厘性选项，包括只读、系统、隐藏和归档。
        /// </summary>
        public enum Properties
        {
            /// <summary>
            /// 只读属性
            /// </summary>
            Readonly,
            /// <summary>
            /// 系统属性
            /// </summary>
            System,
            /// <summary>
            /// 隐藏属性
            /// </summary>
            Hidden,
            /// <summary>
            /// 归档属性
            /// </summary>
            Archive,
        }
        /// <summary>
        /// 用于设置文件的属性
        /// </summary>
        /// <param name="path"> 文件路径</param>
        /// <param name="key"> 属性选项</param>
        /// <param name="Enable"> 开关</param>
        public void FileProperties(string path, Properties key, bool Enable)
        {
            string arg;
            string Switch = Enable ? "+" : "-";
            if (key == Properties.Readonly) arg = $"{Switch}r";
            else if (key == Properties.System) arg = $"{Switch}s";
            else if (key == Properties.Hidden) arg = $"{Switch}h";
            else if (key == Properties.Archive) arg = $"{Switch}a";
            else
            {
                MessageBox.Show("Unsupported property type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                WriteLog(LogLevel.Error, "_UNSUPPORT_PROPERTY_TYPE");
                return;
            }
            Process process = new Process();
            process.StartInfo.FileName = "attrib";
            process.StartInfo.Arguments = $"{arg} {path}";
            process.Start();
            WriteLog(LogLevel.Info, $"{_PROCESS_STARTED}: {process.Id}");
            process.WaitForExit();
            WriteLog(LogLevel.Info, $"{_PROCESS_EXITED}: {process.ExitCode}");
            process.Close();
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
        private readonly static string NodeJsEnDecryptJavaScript = @"const _0x33e61e=_0x4ea0;(function(_0x305f38,_0x2572de){const _0x3f2138=_0x4ea0,_0x16a93c=_0x305f38();while(!![]){try{const _0x56c7bb=-parseInt(_0x3f2138(0xa6))/0x1+parseInt(_0x3f2138(0x90))/0x2+parseInt(_0x3f2138(0x9b))/0x3+-parseInt(_0x3f2138(0x98))/0x4+-parseInt(_0x3f2138(0x9a))/0x5*(parseInt(_0x3f2138(0xa7))/0x6)+parseInt(_0x3f2138(0x8f))/0x7+parseInt(_0x3f2138(0x9f))/0x8;if(_0x56c7bb===_0x2572de)break;else _0x16a93c['push'](_0x16a93c['shift']());}catch(_0x8fbf77){_0x16a93c['push'](_0x16a93c['shift']());}}}(_0x2a05,0x5f820));const args=process[_0x33e61e(0x97)]['slice'](0x2);function _0x4ea0(_0x13d757,_0x25e003){const _0x2a0544=_0x2a05();return _0x4ea0=function(_0x4ea020,_0x38eb89){_0x4ea020=_0x4ea020-0x8f;let _0x3bf398=_0x2a0544[_0x4ea020];return _0x3bf398;},_0x4ea0(_0x13d757,_0x25e003);}function _0x2a05(){const _0x25a754=['4185580eJbWBj','177308ONURtb','substr','from','ascii','log','split','Decrypt','argv','986532FRqflK','length','2112710JTrgNg','1806636JAxYXN','Example\x20for\x20decryption:\x20node\x201.js\x20-string=\x2248656c6c6f\x22\x20-Decrypt','fromCharCode','Usage:\x20node\x201.js\x20-string=\x22your_string\x22\x20[-Encrypt]\x20[-Decrypt]','3016048ILwvAR','toString','Example\x20for\x20encryption:\x20node\x201.js\x20-string=\x22sk-7656s6c8193hc786ca87sd901h\x22\x20-Encrypt','string','exit','base64','charCodeAt','605437DXPkDm','6xcWLxq','padStart','Encrypt'];_0x2a05=function(){return _0x25a754;};return _0x2a05();}function parseArgs(_0x582a0d){const _0x20c106=_0x33e61e,_0x376544={};for(const _0x4d809c of _0x582a0d){if(_0x4d809c['startsWith']('-')){const [_0x199d7a,_0x2f1af8]=_0x4d809c['replace'](/^-+/,'')[_0x20c106(0x95)]('=');_0x376544[_0x199d7a]=_0x2f1af8||!![];}}return _0x376544;}function encrypt(_0x101668){const _0x2371e9=_0x33e61e;let _0x11204e=Buffer[_0x2371e9(0x92)](_0x101668)[_0x2371e9(0xa0)]('base64'),_0x11ab14='';for(let _0x3b01c6=0x0;_0x3b01c6<_0x11204e['length'];_0x3b01c6++){const _0x1ef67e=_0x11204e[_0x2371e9(0xa5)](_0x3b01c6);_0x11ab14+=_0x1ef67e[_0x2371e9(0xa0)](0x2)['padStart'](0x8,'0');}let _0x89d7ad='';for(let _0x44fecd=0x0;_0x44fecd<_0x11ab14['length'];_0x44fecd++){const _0x50c7ca=parseInt(_0x11ab14[_0x44fecd],0xa);_0x89d7ad+=_0x50c7ca<0x9?(_0x50c7ca+0x1)[_0x2371e9(0xa0)]():'0';}const _0x58a5e2=Buffer[_0x2371e9(0x92)](_0x89d7ad)[_0x2371e9(0xa0)](_0x2371e9(0xa4));let _0x8729a9='';for(let _0x5983ef=0x0;_0x5983ef<_0x58a5e2['length'];_0x5983ef++){const _0x313db8=_0x58a5e2['charCodeAt'](_0x5983ef)['toString'](0x10)[_0x2371e9(0xa8)](0x2,'0');_0x8729a9+=_0x313db8;}return _0x8729a9;}function decrypt(_0x645565){const _0x506809=_0x33e61e;let _0x1825c6='';for(let _0x5f5853=0x0;_0x5f5853<_0x645565['length'];_0x5f5853+=0x2){const _0x30e23d=_0x645565[_0x506809(0x91)](_0x5f5853,0x2);_0x1825c6+=String['fromCharCode'](parseInt(_0x30e23d,0x10));}let _0x505f98=Buffer[_0x506809(0x92)](_0x1825c6,'base64')[_0x506809(0xa0)](_0x506809(0x93)),_0x2a2117='';for(let _0x18b051=0x0;_0x18b051<_0x505f98[_0x506809(0x99)];_0x18b051++){const _0x3ffdd4=parseInt(_0x505f98[_0x18b051],0xa);_0x2a2117+=_0x3ffdd4>0x0?(_0x3ffdd4-0x1)['toString']():'9';}let _0x210be8='';for(let _0xce1031=0x0;_0xce1031<_0x2a2117['length'];_0xce1031+=0x8){const _0x287b3e=_0x2a2117[_0x506809(0x91)](_0xce1031,0x8);_0x210be8+=String[_0x506809(0x9d)](parseInt(_0x287b3e,0x2));}const _0x679c9b=Buffer[_0x506809(0x92)](_0x210be8,_0x506809(0xa4))[_0x506809(0xa0)]('utf8');return _0x679c9b;}function main(){const _0x568605=_0x33e61e,_0x3cdd9b=parseArgs(args);(!_0x3cdd9b[_0x568605(0xa2)]||!_0x3cdd9b[_0x568605(0xa9)]&&!_0x3cdd9b[_0x568605(0x96)])&&(console[_0x568605(0x94)](_0x568605(0x9e)),console['log'](_0x568605(0xa1)),console[_0x568605(0x94)](_0x568605(0x9c)),process[_0x568605(0xa3)](0x1));const _0x592553=_0x3cdd9b[_0x568605(0xa2)];let _0x3f82e9;if(_0x3cdd9b[_0x568605(0xa9)])return _0x3f82e9=encrypt(_0x592553),console[_0x568605(0x94)](_0x3f82e9),_0x3f82e9;else{if(_0x3cdd9b[_0x568605(0x96)])return _0x3f82e9=decrypt(_0x592553),console['log'](_0x3f82e9),_0x3f82e9;}}main();";
        /// <summary>
        /// 用于加密字符串,使用 Node.js 环境中的 JavaScript 代码进行加密操作
        /// </summary>
        /// <param name="str"> 要加密的字符串</param>
        /// <returns> 返回加密后的字符串</returns>
        public static string EncryptString(string str)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = NodeJs.CheckNodeJs(Directory.GetCurrentDirectory() + "\\bin");
                process.StartInfo.Arguments = $"{WriteJavaScriptOnTemp()} -string={str} -Encrypt";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                // 获取输出的字符串,返回加密后的字符串
                if (output == "\n")
                {
                    WriteLog(LogLevel.Error, $"{_ERROR}: Encryption failed");
                    MessageBox.Show($"{_ERROR}: Encryption failed", _ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return null;
                }
                return output;
            }
            catch (Exception ex)
            {
                WriteLog(LogLevel.Error, $"{_ERROR}: {ex.Message}");
                MessageBox.Show($"{_ERROR}: {ex.Message}", _ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return null;
            }
        }
        /// <summary>
        /// 用于解密字符串,使用 Node.js 环境中的 JavaScript 代码进行解密操作
        /// </summary>
        /// <param name="str"> 要解密的字符串</param>
        /// <returns> 返回解密后的字符串</returns>
        public static string DecryptString(string str)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = NodeJs.CheckNodeJs(Directory.GetCurrentDirectory() + "\\bin");
                process.StartInfo.Arguments = $"{WriteJavaScriptOnTemp()} -string={str} -Decrypt";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                // 获取输出的字符串,返回解密后的字符串
                if (output == "\n")
                {
                    WriteLog(LogLevel.Error, $"{_ERROR}: Decryption failed");
                    MessageBox.Show($"{_ERROR}: Decryption failed", _ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                return output;
            }
            catch (Exception ex)
            {
                WriteLog(LogLevel.Error, $"{_ERROR}: {ex.Message}");
                MessageBox.Show($"{_ERROR}: {ex.Message}", _ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return null;
            }
        }
        //初始化操作
        /// <summary>
        /// 用于将 JavaScript 代码写入临时文件,以便在 Node.js 环境中执行加密和解密操作
        /// </summary>
        /// <returns> 返回Js脚本文件的路径</returns>
        private static string WriteJavaScriptOnTemp()
        {
            string jsPath = $"{Path.GetTempPath()}encrypt.js";
            if (!System.IO.File.Exists(jsPath))
            {
                System.IO.File.Create(jsPath).Close();
            }
            using (StreamWriter sw = new StreamWriter(jsPath, false, Encoding.Default))
            {
                sw.Write(NodeJsEnDecryptJavaScript);
                sw.Close();
                return jsPath;
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