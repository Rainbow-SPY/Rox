using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using static Rox.Runtimes.LogLibraries;

namespace Rox.Runtimes
{
    /// <summary>
    /// 异常报告助手
    /// </summary>
    public partial class Reporter : Form
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
                    // 忽略无��获取名称的程序集
                }
            }


            WriteLog.Error(log);
            _Crush_Path.Text = _Crush_File_Path;
            LogToCrushFile(_Crush_File_Path, log);
            richTextBox1.Text += log;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void _Crush_Path_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Application.StartupPath);
        }
    }
}
