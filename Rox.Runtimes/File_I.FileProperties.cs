using System.Diagnostics;
using System.Windows.Forms;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;

namespace Rox.Runtimes
{
    public partial class File_I
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
                MessageBox_I.Error("Unsupported property type.", _ERROR);
                WriteLog.Error("_UNSUPPORT_PROPERTY_TYPE");
                return;
            }
            Process process = new Process();
            process.StartInfo.FileName = "attrib";
            process.StartInfo.Arguments = $"{arg} {path}";
            process.Start();
            WriteLog.Info($"{_PROCESS_STARTED}: {process.Id}");
            process.WaitForExit();
            WriteLog.Info($"{_PROCESS_EXITED}: {process.ExitCode}");
            process.Close();
        }
    }
}