using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    namespace Runtimes
    {
        /// <summary>
        /// Windows 目录操作类
        /// </summary>
        public class Directory_I
        {
            #region COM 接口定义 (修正版)
            /// <summary>
            /// 文件复制操作接口
            /// </summary>
            [ComImport]
            [Guid("947aab5f-0a5c-4c13-b4d6-4bf7836fc9f8")] // 正确的IFileOperation IID
            [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IFileOperation
            {
                /// <summary>
                /// 注册文件操作事件监听器
                /// </summary>
                /// <param name="pfops"> 文件操作事件监听器接口</param>
                /// <param name="pdwCookie"> 事件监听器的唯一标识符</param>
                /// <returns> 返回事件监听器的唯一标识符</returns>
                uint Advise(IntPtr pfops, out uint pdwCookie);
                /// <summary>
                /// 取消注册文件操作事件监听器
                /// </summary>
                /// <param name="dwCookie"> 事件监听器的唯一标识符</param>
                void Unadvise(uint dwCookie);
                /// <summary>
                /// 设置操作标志
                /// </summary>
                /// <param name="dwOperationFlags"> 操作标志</param>
                void SetOperationFlags(FileOperationFlags dwOperationFlags);
                /// <summary>
                /// 设置进度消息
                /// </summary>
                /// <param name="pszMessage"> 进度消息</param>
                void SetProgressMessage([MarshalAs(UnmanagedType.LPWStr)] string pszMessage);
                /// <summary>
                /// 设置进度对话框
                /// </summary>
                /// <param name="popd"> 进度对话框对象</param>
                void SetProgressDialog([MarshalAs(UnmanagedType.Interface)] object popd);
                /// <summary>
                /// 设置属性数组
                /// </summary>
                /// <param name="pproparray">属性数组对象</param>
                void SetProperties([MarshalAs(UnmanagedType.Interface)] object pproparray);
                /// <summary>
                /// 设置所有者窗口
                /// </summary>
                /// <param name="hwndParent"> 所有者窗口的句柄</param>
                void SetOwnerWindow(uint hwndParent);
                /// <summary>
                /// 应用属性到单个项目
                /// </summary>
                /// <param name="psiItem"> 要应用属性的ShellItem</param>
                void ApplyPropertiesToItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem);
                /// <summary>
                /// 应用属性到多个项目
                /// </summary>
                /// <param name="punkItems"> 要应用属性的项目集合</param>
                void ApplyPropertiesToItems([MarshalAs(UnmanagedType.Interface)] object punkItems);
                /// <summary>
                /// 重命名单个项目
                /// </summary>
                /// <param name="psiItem"> 要重命名的ShellItem</param>
                /// <param name="pszNewName"> 新名称</param>
                /// <param name="pfopsItem"> 操作标志</param>
                void RenameItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem,
                               [MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
                               IntPtr pfopsItem);
                /// <summary>
                /// 重命名多个项目
                /// </summary>
                /// <param name="pUnkItems"> 要重命名的项目集合</param>
                /// <param name="pszNewName"> 新名称</param>
                void RenameItems([MarshalAs(UnmanagedType.Interface)] object pUnkItems,
                                [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
                /// <summary>
                /// 移动单个项目到指定目录
                /// </summary>
                /// <param name="psiItem"> 要移动的ShellItem</param>
                /// <param name="psiDestinationFolder"> 目标目录的ShellItem</param>
                /// <param name="pszNewName"> 新名称</param>
                /// <param name="pfopsItem"> 操作标志</param>
                void MoveItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem,
                             [MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder,
                             [MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
                             IntPtr pfopsItem);
                /// <summary>
                /// 移动多个项目到指定目录
                /// </summary>
                /// <param name="punkItems"> 要移动的项目集合</param>
                /// <param name="psiDestinationFolder"> 目标目录的ShellItem</param>
                void MoveItems([MarshalAs(UnmanagedType.Interface)] object punkItems,
                              [MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder);
                /// <summary>
                /// 复制单个项目到指定目录
                /// </summary>
                /// <param name="psiItem"> 要复制的ShellItem</param>
                /// <param name="psiDestinationFolder"> 目标目录的ShellItem</param>
                /// <param name="pszNewName"> 新名称</param>
                /// <param name="pfopsItem"> 操作标志</param>
                void CopyItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem,
                             [MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder,
                             [MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
                             IntPtr pfopsItem);
                /// <summary>
                /// 复制多个项目到指定目录
                /// </summary>
                /// <param name="punkItems"> 要复制的项目集合</param>
                /// <param name="psiDestinationFolder"> 目标目录的ShellItem</param>
                void CopyItems([MarshalAs(UnmanagedType.Interface)] object punkItems,
                              [MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder);
                /// <summary>
                /// 删除单个项目
                /// </summary>
                /// <param name="psiItem"> 要删除的ShellItem</param>
                /// <param name="pfopsItem"> 操作标志</param>
                void DeleteItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem,
                               IntPtr pfopsItem);
                /// <summary>
                /// 删除多个项目
                /// </summary>
                /// <param name="punkItems"> 要删除的项目集合</param>
                void DeleteItems([MarshalAs(UnmanagedType.Interface)] object punkItems);
                /// <summary>
                /// 创建新项目
                /// </summary>
                /// <param name="psiDestinationFolder"> 目标目录的ShellItem</param>
                /// <param name="dwFileAttributes"> 文件属性</param>
                /// <param name="pszName"> 新项目的名称</param>
                /// <param name="pszTemplateName"> 模板名称</param>
                /// <param name="pfopsItem"> 操作标志</param>
                /// <returns></returns>
                uint NewItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder,
                            uint dwFileAttributes,
                            [MarshalAs(UnmanagedType.LPWStr)] string pszName,
                            [MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName,
                            IntPtr pfopsItem);
                /// <summary>
                /// 执行所有已排队的文件操作
                /// </summary>
                void PerformOperations();
                /// <summary>
                /// 检查是否有任何操作被中止
                /// </summary>
                /// <returns> 如果有任何操作被中止，则返回 <see langword="true"/>，否则返回 <see langword="false"/></returns>
                [return: MarshalAs(UnmanagedType.Bool)]
                bool GetAnyOperationsAborted();
            }
            /// <summary>
            /// 文件操作类的 COM 实现
            /// </summary>
            [ComImport]
            [Guid("3ad05575-8857-4850-9277-11b85bdb8e09")] // IFileOperation 的 CLSID
            [ClassInterface(ClassInterfaceType.None)]
            public class FileOperation { }
            /// <summary>
            /// 文件操作标志枚举
            /// </summary>
            [Flags]
            public enum FileOperationFlags : uint
            {
                /// <summary>
                /// 多目标文件操作标志
                /// </summary>
                FOF_MULTIDESTFILES = 0x0001,
                /// <summary>
                /// 确认鼠标操作标志
                /// </summary>
                FOF_CONFIRMMOUSE = 0x0002,
                /// <summary>
                /// 静默操作标志，不显示任何对话框或消息框
                /// </summary>
                FOF_SILENT = 0x0004,
                /// <summary>
                /// 重命名冲突时重命名文件
                /// </summary>
                FOF_RENAMEONCOLLISION = 0x0008,
                /// <summary>
                /// 不显示确认对话框
                /// </summary>
                FOF_NOCONFIRMATION = 0x0010,
                /// <summary>
                /// 希望获取映射句柄
                /// </summary>
                FOF_WANTMAPPINGHANDLE = 0x0020,
                /// <summary>
                /// 允许撤销操作
                /// </summary>
                FOF_ALLOWUNDO = 0x0040,
                /// <summary>
                /// 仅操作文件，不操作目录
                /// </summary>
                FOF_FILESONLY = 0x0080,
                /// <summary>
                /// 显示简单进度条，不显示详细进度信息
                /// </summary>
                FOF_SIMPLEPROGRESS = 0x0100,
                /// <summary>
                /// 不确认创建目录
                /// </summary>
                FOF_NOCONFIRMMKDIR = 0x0200,
                /// <summary>
                /// 不显示错误消息框
                /// </summary>
                FOF_NOERRORUI = 0x0400,
                /// <summary>
                /// 不复制安全属性
                /// </summary>
                FOF_NOCOPYSECURITYATTRIBS = 0x0800,
                /// <summary>
                /// 不递归操作子目录
                /// </summary>
                FOF_NORECURSION = 0x1000,
                /// <summary>
                /// 不处理连接的元素（例如符号链接或快捷方式）
                /// </summary>
                FOF_NO_CONNECTED_ELEMENTS = 0x2000,
                /// <summary>
                /// 希望在删除文件时显示警告对话框
                /// </summary>
                FOF_WANTNUKEWARNING = 0x4000,
                /// <summary>
                /// 不递归处理重解析点（例如符号链接或挂载点）
                /// </summary>
                FOF_NORECURSEREPARSE = 0x8000
            }
            /// <summary>
            /// ShellItem 接口定义
            /// </summary>
            [ComImport]
            [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
            [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IShellItem
            {
                /// <summary>
                /// 绑定到处理程序
                /// </summary>
                /// <param name="pbc"> 绑定上下文</param>
                /// <param name="bhid"> 处理程序标识符</param>
                /// <param name="riid"> 请求的接口标识符</param>
                /// <param name="ppv"> 返回的接口指针</param>
                void BindToHandler(IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid bhid,
                                  [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppv);
                /// <summary>
                /// 获取父项
                /// </summary>
                /// <param name="ppsi"> 父项的ShellItem</param>
                void GetParent(out IShellItem ppsi);
                /// <summary>
                /// 获取显示名称
                /// </summary>
                /// <param name="sigdnName"> 显示名称的类型</param>
                /// <param name="ppszName"> 显示名称字符串</param>
                void GetDisplayName(SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
                /// <summary>
                /// 获取属性
                /// </summary>
                /// <param name="sfgaoMask"> 属性标志掩码</param>
                /// <param name="psfgaoAttribs"> 属性标志掩码</param>
                void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);
                /// <summary>
                /// 获取图标索引
                /// </summary>
                /// <param name="psi"> ShellItem</param>
                /// <param name="hint"> 提示</param>
                /// <param name="piOrder"> 图标索引</param>
                void Compare(IShellItem psi, uint hint, out int piOrder);
            }
            /// <summary>
            /// ShellItem 显示名称类型枚举
            /// </summary>
            public enum SIGDN : uint
            {
                /// <summary>
                /// 正常显示名称
                /// </summary>
                NORMALDISPLAY = 0,
                /// <summary>
                /// 全路径显示名称
                /// </summary>
                PARENTRELATIVEPARSING = 0x80018001,
                /// <summary>
                /// 父相对解析显示名称
                /// </summary>
                PARENTRELATIVEFORADDRESSBAR = 0x8001c001,
                /// <summary>
                /// 父相对地址栏显示名称
                /// </summary>
                DESKTOPABSOLUTEPARSING = 0x80028000,
                /// <summary>
                /// 桌面绝对解析显示名称
                /// </summary>
                PARENTRELATIVEEDITING = 0x80031001,
                /// <summary>
                /// 父相对编辑显示名称
                /// </summary>
                DESKTOPABSOLUTEEDITING = 0x8004c000,
                /// <summary>
                /// 桌面绝对编辑显示名称
                /// </summary>
                FILESYSPATH = 0x80058000,
                /// <summary>
                /// 文件系统路径显示名称
                /// </summary>
                URL = 0x80068000
            }
            /// <summary>
            /// 创建 ShellItem 的方法
            /// </summary>
            /// <param name="pszPath"> 要解析的路径</param>
            /// <param name="pbc"> 绑定上下文</param>
            /// <param name="riid"> 请求的接口标识符</param>
            /// <param name="ppv"> 返回的接口指针</param>
            /// <returns> HRESULT</returns>
            [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern int SHCreateItemFromParsingName(
                [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
                IntPtr pbc,
                [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                out IShellItem ppv);
            #endregion

            /// <summary>
            /// 复制目录方法（保持原始调用方式不变）
            /// </summary>
            public static bool CopyDirectory(string sourceDirectory, string destinationDirectory, IWin32Window ownerWindow = null,
                FileOperationFlags flags = FileOperationFlags.FOF_NOCONFIRMMKDIR | FileOperationFlags.FOF_SIMPLEPROGRESS)
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    MessageBox.Show("源目录不存在: " + sourceDirectory, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    WriteLog(LogLevel.Error, $"CopyDirectory failed: Source directory does not exist: {sourceDirectory}");
                    return false;
                }

                try
                {
                    // 创建文件操作对象（正确方式）
                    var fileOperation = (IFileOperation)new FileOperation();

                    // 设置操作标志
                    fileOperation.SetOperationFlags(flags);

                    // 设置所有者窗口
                    if (ownerWindow != null)
                    {
                        fileOperation.SetOwnerWindow((uint)ownerWindow.Handle);
                    }

                    // 创建源目录的ShellItem
                    IShellItem sourceItem;
                    int hr = SHCreateItemFromParsingName(sourceDirectory, IntPtr.Zero, typeof(IShellItem).GUID, out sourceItem);
                    if (hr != 0) Marshal.ThrowExceptionForHR(hr);

                    // 创建目标父目录的ShellItem
                    string destParent = Path.GetDirectoryName(destinationDirectory);
                    string newFolderName = Path.GetFileName(destinationDirectory);

                    IShellItem destParentItem;
                    hr = SHCreateItemFromParsingName(destParent, IntPtr.Zero, typeof(IShellItem).GUID, out destParentItem);
                    if (hr != 0)
                    {
                        Marshal.ReleaseComObject(sourceItem);
                        Marshal.ThrowExceptionForHR(hr);
                    }

                    // 执行复制操作
                    fileOperation.CopyItem(sourceItem, destParentItem, newFolderName, IntPtr.Zero);
                    fileOperation.PerformOperations();

                    bool aborted = fileOperation.GetAnyOperationsAborted();

                    // 释放资源
                    Marshal.ReleaseComObject(sourceItem);
                    Marshal.ReleaseComObject(destParentItem);
                    Marshal.ReleaseComObject(fileOperation);

                    return !aborted;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"复制目录时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    WriteLog(LogLevel.Error, $"CopyDirectory failed: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
