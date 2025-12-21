using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Rox.Runtimes
{
    public partial class Reporter
    {
        /// <summary>
        /// 应用程序初始化与崩溃处理接口
        /// </summary>
        public class InitializeReportInterface
        {
            // 主窗体创建委托
            private readonly Func<Form> _mainFormFactory;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="mainFormFactory">创建主窗体的工厂方法</param>
            public InitializeReportInterface(Func<Form> mainFormFactory) => _mainFormFactory = mainFormFactory ?? throw new ArgumentNullException(nameof(mainFormFactory));

            /// <summary>
            /// 启动应用程序
            /// </summary>
            public void Run()
            {
                string[] args = Environment.GetCommandLineArgs();

                // 检查是否是崩溃处理进程
                if (args.Length > 1 && args[1] == "-crash")
                {
                    RunMainApplication();
                    return;
                }

                // 启动主应用程序实例
                Process.Start(Application.ExecutablePath, "-crash");
            }

            /// <summary>
            /// 运行主应用程序逻辑
            /// </summary>
            private void RunMainApplication()
            {
                // 注册全局异常处理
                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    if (e.ExceptionObject is Exception ex)
                        HandleCrash(ex);
                };

                Application.ThreadException += (sender, e) =>
                {
                    HandleCrash(e.Exception);
                };

                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    // 使用工厂方法创建主窗体
                    using (var mainForm = _mainFormFactory())
                        Application.Run(mainForm);
                }
                catch (Exception ex)
                {
                    HandleCrash(ex);
                }
            }

            /// <summary>
            /// 处理崩溃逻辑
            /// </summary>
            private void HandleCrash(Exception ex)
            {
                using (Form f = new Reporter(ex))
                {
                    f.TopMost = true;   
                    f.ShowDialog();
                }
                Application.Exit();
            }
        }
    }
}
