using System.Diagnostics;
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
        public static bool Is360SafeRunning() => Process.GetProcessesByName("360Tray").Length > 0;
        /// <summary>
        /// 检测火绒安全软件是否在运行
        /// </summary>
        /// <returns> 运行返回 <see langword="true"></see> 未运行返回 <see langword="false"></see></returns>
        public static bool IsHuorongSecurityRunning() => Process.GetProcessesByName("HipsTray").Length > 0;
        /// <summary>
        /// 开启或关闭 Windows 安全中心 , 此方法重定向到 <see cref="Rox.Windows.WindowsSecurityCenter"/>
        /// </summary>
        public class WindowsSecurity
        {
            /// <summary>
            /// 开启 Windows 安全中心 , 此方法重定向到 <see cref="Rox.Windows.WindowsSecurityCenter.Enable()"/>
            /// </summary>
            public static void Enable() => Windows.WindowsSecurityCenter.Enable();
            /// <summary>
            /// 关闭 Windows 安全中心 , 此方法重定向到 <see cref="Rox.Windows.WindowsSecurityCenter.Disable()"/>
            /// </summary>
            public static void Disable() => Windows.WindowsSecurityCenter.Disable();
        }
    }
    #region 此类废弃, 等我去写新代码, 精力不够 QAQ
    ///// <summary>
    ///// 用于更新软件
    ///// </summary>
    //public class Update
    //{
    //    /// <summary>
    //    /// 用于检查更新, 比较两个版本号字符串并返回更新的版本或指示版本相同
    //    /// </summary>
    //    /// <param name="version1"> 版本1</param>
    //    /// <param name="version2"> 版本2</param>
    //    /// <returns> 返回更新的版本或指示版本相同</returns>
    //    internal static string NewerVersions(string version1, string version2)
    //    {
    //        // 将版本号按小数点分割成数组
    //        string[] v1Parts = version1.Split('.');
    //        string[] v2Parts = version2.Split('.');

    //        // 获取两个版本号的最大长度
    //        int maxLength = Math.Max(v1Parts.Length, v2Parts.Length);

    //        for (int i = 0; i < maxLength; i++)
    //        {
    //            // 如果当前部分超出数组范围，则补0
    //            int v1Part = i < v1Parts.Length ? int.Parse(v1Parts[i]) : 0;
    //            int v2Part = i < v2Parts.Length ? int.Parse(v2Parts[i]) : 0;

    //            // 比较当前部分的大小
    //            if (v1Part > v2Part)
    //            {
    //                return version1; // version1 更新
    //            }
    //            else if (v1Part < v2Part)
    //            {
    //                return version2; // version2 更新
    //            }
    //        }

    //        // 如果所有部分都相同，则版本号相同
    //        return "same";
    //    }
    //    /// <summary>
    //    /// 发送HTTP请求的HttpClient实例
    //    /// </summary>
    //    private static readonly HttpClient _httpClient = new HttpClient();
    //    /// <summary>
    //    /// 用于检查更新的平台
    //    /// </summary>
    //    public enum Platform
    //    {
    //        /// <summary>
    //        /// Github
    //        /// </summary>
    //        Github,
    //        /// <summary>
    //        /// Gitee
    //        /// </summary>
    //        Gitee,
    //    }
    //    /// <summary>
    //    /// 用于检查更新
    //    /// </summary>
    //    /// <param name="CheckUpdateUrl"> 检查更新的URL</param>
    //    /// <param name="platform"> 平台</param>
    //    /// <returns> 如果有新版本可用，则返回 <see langword="true"/>，否则返回 <see langword="false"/></returns>
    //    public static async Task<bool> CheckUpdate(string CheckUpdateUrl, Platform platform)
    //    {
    //        /* Github API规定的Release最新发行版查询地址为       https://api/github.com/repos/{用户名}/{仓库}/releases/latest
    //         * 
    //         * Gitee API规定的Release最新发行版查询地址为       https://gitee.com/api/v5/repos/{用户名}/{仓库}/releases/latest
    //         * 
    //         * 返回的json中包含了最新发行版的信息，包括版本号、发布时间、下载地址等 例如,最新的版本号为 "tag_name": "v1.4",
    //         */
    //        string jsonResponse = await FetchJsonFromUrl(CheckUpdateUrl);
    //        if (!string.IsNullOrEmpty(jsonResponse))
    //        {
    //            var (TagName, Name) = ExtractTagAndName(jsonResponse, platform);
    //            WriteLog.Info($"{_LATEST_VERSION}: {TagName} - {Name}");
    //            string[] strings = TagName.Split('v');
    //            string res = NewerVersions(LocalizedString.Version, strings[1]);
    //            if (res == "same" || res == LocalizedString.Version)
    //            {
    //                WriteLog.Info(_NON_NEW_VER);
    //                return false;
    //            }
    //            else
    //            {
    //                WriteLog.Info($"{_NEW_VERSION_AVAILABLE}: {res} {_CURRENT_VER}: {LocalizedString.Version}");
    //                return true;
    //            }
    //        }
    //        else
    //        {
    //            WriteLog.Error(_JSON_PARSING_FAILED);
    //            return false;
    //        }
    //    }
    //    /// <summary>
    //    /// 用于从指定的URL获取<see cref="Json"/> 数据,并在请求失败时返回<see langword="null"/>
    //    /// </summary>
    //    /// <param name="CheckUpdateUrl"></param>
    //    /// <param name="platform"></param>
    //    /// <returns></returns>
    //    public static string GetUpdateJson(string CheckUpdateUrl, Platform platform)
    //    {
    //        try
    //        {
    //            string jsonResponse = FetchJsonFromUrl(CheckUpdateUrl).Result;
    //            var (TagName, Name) = ExtractTagAndName(jsonResponse, platform);
    //            WriteLog.Info($"{_LATEST_VERSION}: {TagName} - {Name}");
    //            string strings1 = $"{TagName};{Name}";
    //            return strings1;
    //        }
    //        catch (Exception e)
    //        {
    //            MessageBox_I.(e.ToString());
    //            return null;
    //        }
    //    }
    //    /// <summary>
    //    /// 用于从指定的URL获取<see cref="Json"/> 数据,并在请求失败时返回<see langword="null"/>
    //    /// </summary>
    //    /// <param name="url"> URL</param>
    //    /// <returns> 返回<see cref="Json"/>字符串</returns>
    //    private static async Task<string> FetchJsonFromUrl(string url)
    //    {
    //        try
    //        {
    //            // GitHub API 需要 User-Agent 头
    //            if (url.Contains("github.com") && !_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
    //            {
    //                _httpClient.DefaultRequestHeaders.Add("User-Agent", "C# HttpClient");
    //            }

    //            HttpResponseMessage response = await _httpClient.GetAsync(url);
    //            response.EnsureSuccessStatusCode(); // 确保请求成功
    //            return await response.Content.ReadAsStringAsync();
    //        }
    //        catch (HttpRequestException ex)
    //        {
    //            WriteLog.Error($"{_ERROR}: {ex.Message}");
    //            return null;
    //        }
    //    }
    //    /// <summary>
    //    /// 用于提取标签和名称
    //    /// </summary>
    //    /// <param name="json">  <see cref="Json"/> 格式的数据</param>
    //    /// <param name="platform"> 平台</param>
    //    /// <returns> 返回标签和名称</returns>
    //    private static (string TagName, string Name) ExtractTagAndName(string json, Platform platform)
    //    {
    //        try
    //        {
    //            // 解析 JSON
    //            Json.JObject jsonObject = Json.JObject.Parse(json);

    //            // 提取 tag_name
    //            string tagName = jsonObject["tag_name"]?.ToString();

    //            // 根据平台提取 name
    //            string name;
    //            switch (platform)
    //            {
    //                case Platform.Github:
    //                    name = jsonObject["name"]?.ToString(); // GitHub 的 name 在根节点
    //                    break;
    //                case Platform.Gitee:
    //                    name = jsonObject["name"]?.ToString(); // Gitee 的 name 在 prerelease 对象中
    //                    break;
    //                default:
    //                    throw new ArgumentException(_UNSUPPORT_PLATFORM);
    //            }
    //            return (tagName, name);
    //        }
    //        catch (Exception ex)
    //        {
    //            WriteLog.Error($"{_JSON_PARSING_FAILED}: {ex.Message}");
    //            return (null, null);
    //        }
    //    }
    //    /*        
    //规定 在压缩包内包含了 `update.ini` 和 `filehash.ini` 文件,以及更新文件

    //{version} 为版本号

    //```
    //压缩包文件目录:
    //Update_{ version}.zip          // 更新文件压缩包    
    //├── update.ini             // 更新信息    
    //├── filehash.ini           // 文件哈希值        
    //└── #(update files)        // 更新文件 
    //```

    //规定 ``update.ini`` 规格:
    //```
    //1 > version = ""                              // 版本号     
    //2 > type = [Release / HotFix / bugFix]        // 更新类型       
    //3 > description = ""                          // 更新说明        
    //4 > updatefilecount = ""                      // 更新文件数量       
    //5 > hashurl = ""                              // 哈希值文件下载地址       
    //6 > hash = ""                                 // 文件数量 
    //```

    //规定 ``filehash.ini`` 规格:
    //```
    //> { fileName},{ fileHash}
    //示例:
    //1 > Library.dll,4CC1ED4D70DFC8A7455822EC8339D387
    //2 > Library.pdb, FDFA7596701DCC2E96D462DBC35E7823
    //```           
    //    */
    //    /// <summary>
    //    /// 用于处理应用程序的自我更新，包括创建更新目录和选择更新文件。
    //    /// </summary>
    //    public static void SelfUpdater()
    //    {
    //        try
    //        {
    //            string updateDir = Path.Combine(Directory.GetCurrentDirectory(), "update");
    //            if (!Directory.Exists(updateDir))
    //            {
    //                Directory.CreateDirectory(updateDir);
    //                WriteLog.Info($"{_CREATE_DIRECTORY}: {updateDir}");
    //            }
    //            var temp = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp";
    //            var update = $"{Directory.GetCurrentDirectory()}\\update";
    //            string batPath = $"{Directory.GetCurrentDirectory()}\\temp\\update.bat";
    //            OpenFileDialog fileDialog = new OpenFileDialog
    //            {
    //                DefaultExt = "",
    //                Title = "选择一个更新文件",
    //                Filter = "更新文件压缩包(*.zip)|*.zip"
    //            };
    //            if (fileDialog.ShowDialog() == DialogResult.OK)
    //            {

    //                string UpdateFile = fileDialog.FileName;
    //                WriteLog.Info($"Selected update file: {UpdateFile}");
    //                if (CheckFilesInArchive(UpdateFile))
    //                {
    //                    WriteLog.Info($"Archive Passed");
    //                    string path = ExtractUpdateFiles(update, UpdateFile);
    //                    if (string.IsNullOrEmpty(path))
    //                    {
    //                        WriteLog.Error("Failed to extract update files.");
    //                        return;
    //                    }

    //                    WriteLog.Info($"Files extracted to: {path}");
    //                    // 读取 filehash.ini 文件
    //                    string updateIniPath = Path.Combine(path, "filehash.ini");
    //                    if (!System.IO.File.Exists(updateIniPath))
    //                    {
    //                        WriteLog.Error($"update.ini not found in: {path}");
    //                        return;
    //                    }

    //                    string[] lines = System.IO.File.ReadAllLines(updateIniPath);
    //                    WriteLog.Info($"Reading filehash.ini from: {updateIniPath}");
    //                    BatchWriter(true, "");
    //                    foreach (string line in lines)
    //                    {
    //                        WriteLog.Info("Reading line: " + line);
    //                        string[] parts = line.Split(',');
    //                        if (parts.Length < 2)
    //                        {
    //                            WriteLog.Error($"Invalid line in filehash.ini: {line}");
    //                            break;
    //                        }
    //                        string fileName = parts[0].Trim();
    //                        string expectedHash = parts[1].Trim();

    //                        BatchWriter(false, $"copy /y {Directory.GetCurrentDirectory()}\\update\\{fileName} {Directory.GetCurrentDirectory()}");
    //                        string filePath = Path.Combine(path, fileName);
    //                        string destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "update", fileName);

    //                        WriteLog.Info($"Verifying file: {filePath}, Expected Hash: {expectedHash}");

    //                        // 验证文件哈希值

    //                        if (File_I.CheckFileHash(filePath, expectedHash))
    //                        {
    //                            WriteLog.Info($"File {fileName} Passed");

    //                            // 确保目标目录存在
    //                            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

    //                            // 移动文件
    //                            System.IO.File.Move(filePath, destinationPath);
    //                            WriteLog.Info($"File {fileName} moved to: {destinationPath}");
    //                        }
    //                        else
    //                        {
    //                            WriteLog.Error($"File {fileName} Failed");
    //                            MessageBox_I.($"{_ERROR}: The file {fileName} did not pass MD5 verification.", _ERROR);
    //                        }
    //                    }
    //                    BatchWriter(false, $"echo     Update successful!&pause&start {Application.ExecutablePath}");
    //                    Process.Start(batPath);
    //                    Application.Exit();
    //                }
    //                else
    //                {
    //                    WriteLog.Error($"Archive Failed");
    //                    MessageBox_I.($"{_ERROR}: The archive not passed structure verify.", _ERROR, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            WriteLog.Error($"{_ERROR}: {ex.Message}");
    //        }
    //    }
    //    /// <summary>
    //    /// 用于提取更新文件
    //    /// </summary>
    //    /// <param name="path"> 更新文件的储存路径</param>
    //    /// <param name="UpdateFile"> 更新文件</param>  
    //    /// <returns></returns>
    //    private static string ExtractUpdateFiles(string path, string UpdateFile)
    //    {
    //        Process process = new Process();
    //        process.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\7za.exe";
    //        process.StartInfo.Arguments = $"x \"{UpdateFile}\" -o{path} -y -aoa";
    //        process.Start();
    //        process.WaitForExit();
    //        // 获取解压后的文件夹路径
    //        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
    //        foreach (var file in files)
    //        {
    //            if (file.Contains("update.ini"))
    //            {
    //                return Path.GetDirectoryName(file);
    //            }
    //        }
    //        return null;
    //    }
    //    /// <summary>
    //    /// 用于检查压缩包中是否包含指定的文件
    //    /// </summary>
    //    /// <param name="UpdateFile"> 更新文件</param>
    //    /// <returns> 如果包含指定的文件，则返回 <see langword="true"/>，否则返回 <see langword="false"/></returns>
    //    private static bool CheckFilesInArchive(string UpdateFile)
    //    {
    //        string[] filesToCheck = { "update.ini", "filehash.ini" };
    //        DownloadAssistant.ModuleDownloader(DownloadAssistant.Module.zip);
    //        Process process = new Process();
    //        process.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}\\bin\\7za.exe";
    //        process.StartInfo.Arguments = $"l \"{UpdateFile}\"";
    //        process.StartInfo.CreateNoWindow = true;
    //        process.StartInfo.RedirectStandardOutput = true;
    //        process.StartInfo.UseShellExecute = false;
    //        process.Start();
    //        StreamReader streamReader = process.StandardOutput;
    //        string output = streamReader.ReadToEnd();
    //        foreach (string file in filesToCheck)
    //        {
    //            if (output.Contains(file))
    //            {
    //                process.Close();
    //                return true;
    //            }
    //        }
    //        if (process.ExitCode != 0)
    //        {
    //            WriteLog.Error($"{_ERROR}: {process.ExitCode}");
    //            process.Close();
    //            return false;
    //        }
    //        process.Close();
    //        return false;
    //    }
    //}
    #endregion
}