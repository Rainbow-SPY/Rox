using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    namespace Runtimes
    {
        /// <summary>
        /// Node.Js 类
        /// </summary>
        public class NodeJs
        {
            /// <summary>
            /// 提取 Node.Js,版本号: node-v22.15.1-win-x86
            /// </summary>
            /// <param name="ExtraedFolder"> 存放的文件夹</param>
            /// <returns> 返回提取结果, 如果提取成功则返回文件路径, 否则返回错误信息或返回值</returns>
            public static string ExtractNodeJs(string ExtraedFolder)
            {
                //检查文件夹是否合法
                // 伪代码：
                // 1. 检查 ExtraedFolder 是否为 null 或空字符串 或 不是有效路径（如只包含无效字符或为根目录等）
                // 2. 如果非法，则爆出错误返回
                // 3. 否则，使用传入的 ExtraedFolder

                // 替换原有判断逻辑如下：
                if (string.IsNullOrWhiteSpace(ExtraedFolder) || Path.GetFileName(ExtraedFolder) == string.Empty)
                {
                    WriteLog(LogLevel.Error, $"{ExtraedFolder}值为null或空字符串");
                    MessageBox.Show($"{ExtraedFolder}值为null或空字符串", "错误的路径! - Rox", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return $"Error";
                }
                else
                {
                    LogLibraries.WriteLog(LogLevel.Info, $"{_GET_DIRECTORY}: {ExtraedFolder}");
                }
                // 检查参数是否为文件夹格式
                // 检查文件夹是否存在
                if (!Directory.Exists(ExtraedFolder))
                {
                    // 创建文件夹
                    Directory.CreateDirectory(ExtraedFolder);
                    WriteLog(LogLevel.Info, $"{_CREATE_DIRECTORY}: {ExtraedFolder}");
                }


                // 检查文件是否存在
                if (System.IO.File.Exists(Path.Combine(ExtraedFolder, "node-v22.15.1-win-x86", "node.exe")))
                {
                    LogLibraries.WriteLog(LogLibraries.LogLevel.Info, "Node.Js 已经提取");
                    LogLibraries.WriteLog(LogLevel.Info, $"{_FILE_EXIST}: {Path.Combine(ExtraedFolder, "node.exe")}");
                    return $"{Path.Combine(ExtraedFolder, "node-v22.15.1-win-x86", "node.exe")}"; // 返回文件路径
                }
                else
                {
                    // 从Resources.resx文件中提取NodeJs
                    // 这里假设你已经有一个资源文件，里面包含了NodeJs的压缩包
                    // 获取当前正在执行的类库的程序集
                    Assembly assembly = Assembly.GetExecutingAssembly();

                    // 假设Node.Js.zip是嵌入在"Namespace.Resources"命名空间中的

                    string resourceName = "Rox.Runtimes.Properties.Resources"; // 替换为你的资源路径

                    // 创建 ResourceManager 实例
                    ResourceManager rm = new ResourceManager(resourceName, assembly);
                    LogLibraries.WriteLog(LogLevel.Info, $"{_NEW_RM}");
                    // 从资源中获取Node.Js.zip文件的字节数据
                    byte[] NodeJsZipData = (byte[])rm.GetObject("Node_js");
                    LogLibraries.WriteLog(LogLevel.Info, $"{_GET_RM_OBJ}: Node_js");
                    if (NodeJsZipData != null)
                    {
                        // 将文件保存到当前目录
                        string outputDirectory = Path.GetTempPath();
                        // 检查并创建目录
                        if (!Directory.Exists(outputDirectory))
                        {
                            Directory.CreateDirectory(outputDirectory);
                            LogLibraries.WriteLog(LogLevel.Info, $"{_CREATE_DIRECTORY}");
                        }
                        LogLibraries.WriteLog(LogLevel.Info, $"{_GET_OUTPUT_DIRECTORY}: {outputDirectory}");
                        // 保存文件路径
                        string outputFilePath = Path.Combine(outputDirectory, "Node.Js.zip");
                        LogLibraries.WriteLog(LogLevel.Info, $"{_GET_OUTPUT_NAME}: {outputDirectory}");
                        // 写入文件，确保保存为二进制数据
                        LogLibraries.WriteLog(LogLevel.Info, $"{_FILE_WRITING}");
                        System.IO.File.WriteAllBytes(outputFilePath, NodeJsZipData);
                        LogLibraries.WriteLog(LogLevel.Info, $"Node.Js.zip {_FILE_EXIST_PATH} {outputFilePath}");
                    }
                    else
                    {
                        LogLibraries.WriteLog(LogLevel.Error, $"{_RES_FILE_NOT_FIND}");
                        return _RES_FILE_NOT_FIND;
                    }
                    // 解压缩文件
                    string zipFilePath = Path.Combine(Path.GetTempPath(), "Node.Js.zip");
                    Process zip = new Process();
                    zip.StartInfo.FileName = "powershell.exe";
                    zip.StartInfo.Arguments = $"-Command \"Expand-Archive -Path '{zipFilePath}' -DestinationPath '{ExtraedFolder}'\"";
                    zip.StartInfo.UseShellExecute = false;
                    zip.StartInfo.CreateNoWindow = true;
                    zip.Start();
                    zip.WaitForExit();
                    if (zip.ExitCode == 0)
                    {
                        LogLibraries.WriteLog(LogLevel.Info, $"Node.js {_DOWNLOADING_COMPLETE}");
                        LogLibraries.WriteLog(LogLevel.Info, $"{_FILE_EXIST_PATH} {ExtraedFolder}");
                        return $"{Path.Combine(ExtraedFolder, "node-v22.15.1-win-x86", "node.exe")}";
                    }
                    else
                    {
                        LogLibraries.WriteLog(LogLevel.Error, $"Node.js {_DOWNLOADING_FAILED}");
                        LogLibraries.WriteLog(LogLevel.Error, $"{zip.ExitCode}");
                        return $"{zip.ExitCode}";
                    }
                }
            }
            /// <summary>
            /// 检查 Node.Js 是否存在
            /// </summary>
            /// <param name="ExtraedFolder"></param>
            /// <returns> 返回提取结果, 如果提取成功则返回文件路径, 否则返回错误信息或返回值</returns>
            public static string CheckNodeJs(string ExtraedFolder)
            {
                // 检查文件夹是否合法
                // 伪代码：
                // 1. 检查 ExtraedFolder 是否为 null 或空字符串 或 不是有效路径（如只包含无效字符或为根目录等）
                // 2. 如果非法，则爆出错误返回
                // 3. 否则，使用传入的 ExtraedFolder
                // 替换原有判断逻辑如下：
                if (string.IsNullOrWhiteSpace(ExtraedFolder) || Path.GetFileName(ExtraedFolder) == string.Empty)
                {
                    WriteLog(LogLevel.Error, $"{ExtraedFolder}值为null或空字符串");
                    MessageBox.Show($"{ExtraedFolder}值为null或空字符串", "错误的路径! - Rox", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return $"Error";
                }
                else
                {
                    LogLibraries.WriteLog(LogLevel.Info, $"{_GET_DIRECTORY}: {ExtraedFolder}");
                }
                // 检查参数是否为文件夹格式
                // 检查文件夹是否存在
                if (!Directory.Exists(ExtraedFolder))
                {
                    // 创建文件夹
                    Directory.CreateDirectory(ExtraedFolder);
                    WriteLog(LogLevel.Info, $"{_CREATE_DIRECTORY}: {ExtraedFolder}");
                }
                // 检查文件是否存在
                if (System.IO.File.Exists(Path.Combine(ExtraedFolder, "node-v22.15.1-win-x86", "node.exe")))
                {
                    LogLibraries.WriteLog(LogLevel.Info, "Node.Js 已经提取");
                    LogLibraries.WriteLog(LogLevel.Info, $"{_FILE_EXIST}: {Path.Combine(ExtraedFolder, "node.exe")}");
                    return $"{Path.Combine(ExtraedFolder, "node-v22.15.1-win-x86", "node.exe")}"; // 返回文件路径
                }
                else
                {
                    LogLibraries.WriteLog(LogLevel.Error, $"File Not Exist");
                    string _returnValue = Rox.Runtimes.NodeJs.ExtractNodeJs(ExtraedFolder);
                    char drive = _returnValue[0];
                    if ((drive >= 'A' && drive <= 'Z') || (drive >= 'a' && drive <= 'z') && _returnValue[1] == ':')
                    {
                        WriteLog(LogLevel.Info, $"{_FILE_EXIST} {_returnValue}");
                        return _returnValue; // 返回文件路径
                    }

                    if (_returnValue == "Error")
                    {
                        WriteLog(LogLevel.Error, "Node.js 在 ResourceManager中提取资源包失败.");
                        return "Error"; //返回错误信息
                    }
                    // 检查返回值是否为int
                    else if (int.TryParse(_returnValue, out int exitCode))
                    {
                        // 如果返回值是int类型，则表示提取失败
                        WriteLog(LogLevel.Error, $"Node.js 在 PowerShell 中解压缩资源包时失败，错误代码: {exitCode}");
                        return exitCode.ToString(); // 返回错误代码
                    }
                    else
                    {
                        WriteLog(LogLevel.Info, $"Node.js 在 ResourceManager中返回了 {_RES_FILE_NOT_FIND}, 请检查资源文件是否存在.");
                        return _RES_FILE_NOT_FIND; // 返回错误信息
                    }
                }
                // 检查返回值是否为路径
            }
        }
    }
}
