using Microsoft.Win32;
using System;
using static Rox.Runtimes.LocalizedString;
using static Rox.Runtimes.LogLibraries;
namespace Rox
{
    namespace Runtimes
    {
        /// <summary>
        /// 用于处理注册表操作
        /// </summary>
        public class Registry_I
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
                        WriteLog.Info($"{_WRITE_REGISTRY}");
                        key.SetValue(valueName, valueData, valueType);
                        key.Close();
                    }
                    LogLibraries.WriteLog.Info($"{_SUCESS_WRITE_REGISTRY}");
                }
                catch (Exception ex)
                {
                    LogLibraries.WriteLog.Error($"{_WRITE_REGISTRY_FAILED}: {ex.Message}");
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
            /// <summary>
            /// 用于注册文件扩展名
            /// </summary>
            public class Extension
            {
                internal static bool CommonRegisteredVoid(string _ext, string _des, string _ico)
                {
                    if (!CommonRegisteredVoid(_ext, _des))
                        return false;
                    try
                    {
                        string bindKeyName = FindBindKey(_ext);
                        if (string.IsNullOrEmpty(bindKeyName))
                            return false;

                        using (RegistryKey bindKey = Registry.ClassesRoot.OpenSubKey(bindKeyName, true))
                        {
                            bindKey?.SetValue("DefaultIcon", _ico);
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        WriteLog.Error(_Exception_With_xKind("CommonRegisteredVoid", ex));
                        return false;
                    }
                }
                internal static bool CommonRegisteredVoidWithKey(string _ext, string _des, string _key, string _ico)
                {
                    if (!CommonRegisteredVoidWithKey(_ext, _des, _key))
                        return false;
                    try
                    {
                        string bindKeyName = FindBindKey(_ext);
                        if (string.IsNullOrEmpty(bindKeyName))
                            return false;

                        using (RegistryKey bindKey = Registry.ClassesRoot.OpenSubKey(bindKeyName, true))
                        {
                            bindKey?.SetValue("DefaultIcon", _ico);
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        WriteLog.Error(_Exception_With_xKind("CommonRegisteredVoid", ex));
                        return false;
                    }
                }
                internal static bool CommonRegisteredVoid(string _ext, string _des)
                {
                    try
                    {
                        string _key = FindBindKey(_ext);
                        if (_key != null)
                        {
                            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(_key, writable: true))
                            {
                                key.SetValue("", _des);
                            }
                        }
                        else
                        {
                            // 创建唯一关联键名 (如 "MyApp.txtfile")
                            string newKeyName = $"New.{_ext.TrimStart('.')}file";
                            using (RegistryKey extKey = Registry.ClassesRoot.CreateSubKey(_ext))
                            {
                                extKey.SetValue("", newKeyName);
                            }
                            using (RegistryKey bindKey = Registry.ClassesRoot.CreateSubKey(newKeyName))
                            {
                                bindKey.SetValue("", _des);
                            }
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        WriteLog.Error(_Exception_With_xKind("CommonRegisteredVoid", ex));
                        return false;
                    }
                }
                internal static bool CommonRegisteredVoidWithKey(string _ext, string _des, string _key)
                {
                    try
                    {
                        if (_key != null)
                        {
                            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(_key, writable: true))
                            {
                                key.SetValue("", _des);
                            }
                        }
                        else
                        {
                            // 创建唯一关联键名 (如 "MyApp.txtfile")
                            string newKeyName = $"New.{_ext.TrimStart('.')}file";
                            using (RegistryKey extKey = Registry.ClassesRoot.CreateSubKey(_ext))
                            {
                                extKey.SetValue("", newKeyName);
                            }
                            using (RegistryKey bindKey = Registry.ClassesRoot.CreateSubKey(newKeyName))
                            {
                                bindKey.SetValue("", _des);
                            }
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        WriteLog.Error(_Exception_With_xKind("CommonRegisteredVoid", ex));
                        return false;
                    }
                }

                internal static string FindBindKey(string _ext)
                {
                    try
                    {
                        using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(_ext))
                        {
                            return key?.GetValue("")?.ToString(); // 安全访问
                        }
                    }
                    catch (Exception e)
                    {
                        WriteLog.Error(_Exception_With_xKind("FindBindKey", e));
                        return null;
                    }
                }
                internal static string CreateExtKey(string _ext)
                {
                    RegistryKey a = Registry.ClassesRoot.CreateSubKey(_ext);
                    var _path = a.ToString();
                    a.Close();
                    return _path;
                }

                /// <summary>
                /// 用于注册 Adobe 产品类文件扩展名
                /// </summary>
                public class Adobe
                {
                    /// <summary>
                    /// 注册 Adobe After Effects 项目文件(.aep)文件扩展名
                    /// </summary>
                    /// <returns><see langword="true"/>/<see langword="false"/></returns>
                    public static bool Aep()
                    {
                        try
                        {
                            string _aep_name = @"Adobe.AfterEffects.Project.16";
                            string _aep_des = @"Adobe After Effects 项目文件";
                            string _aep_ico = @"C:\Windows\aep.ico,0";
                            return CommonRegisteredVoidWithKey(".aep", _aep_des, _aep_name, _aep_ico);
                        }
                        catch (Exception ex)
                        {
                            WriteLog.Error(_Exception_With_xKind("Extension.Adobe.Aep", ex));
                            return false;
                        }
                    }
                    /// <summary>
                    /// 注册 Adobe Dreamweaver 网页模板(.dwt)文件扩展名
                    /// </summary>
                    /// <returns><see langword="true"/>/<see langword="false"/></returns>
                    public static bool Dwt()
                    {
                        try
                        {
                            string _dwt_name = @"Dreamweaver.Template.21";
                            string _dwt_des = @"Adobe Dreamweaver 网页模版";
                            string _dwt_ico = @"C:\Windows\dwt.ico,0";
                            return CommonRegisteredVoidWithKey(".dwt", _dwt_des, _dwt_name, _dwt_ico);
                        }
                        catch (Exception ex)
                        {
                            WriteLog.Error(_Exception_With_xKind("Extension.Adobe.Dwt", ex));
                            return false;
                        }
                    }
                    /// <summary>
                    /// 注册 Adobe Illustrator 矢量图文件(.ai)文件扩展名
                    /// </summary>
                    /// <returns><see langword="true"/>/<see langword="false"/></returns>
                    public static bool Ai()
                    {
                        try
                        {
                            string _ai_name = @"Illustrator.Document.27";
                            string _ai_des = @"Adobe Illustrator 矢量图文件";
                            string _ai_ico = @"C:\Windows\ai.ico,0";
                            return CommonRegisteredVoidWithKey(".ai", _ai_des, _ai_name, _ai_ico);
                        }
                        catch (Exception ex)
                        {
                            WriteLog.Error(_Exception_With_xKind("Extension.Adobe.Ai", ex));
                            return false;
                        }
                    }
                    /// <summary>
                    /// 注册 Adobe Animate 文档(.fla)文件扩展名
                    /// </summary>
                    /// <returns><see langword="true"/>/<see langword="false"/></returns>
                    public static bool Fla()
                    {
                        try
                        {
                            string _fla_name = @"Flash.Document150";
                            string _fla_des = @"Adobe Animate 文档";
                            string _fla_ico = @"C:\Windows\fla.ico,0";
                            return CommonRegisteredVoidWithKey(".fla", _fla_des, _fla_name, _fla_ico);
                        }
                        catch (Exception ex)
                        {
                            WriteLog.Error(_Exception_With_xKind("Extension.Adobe.Fla", ex));
                            return false;
                        }
                    }
                    /// <summary>
                    /// 注册 Adobe Photoshop 图像文件(.psd)文件扩展名
                    /// </summary>
                    /// <returns><see langword="true"/>/<see langword="false"/></returns>
                    public static bool Psd()
                    {
                        try
                        {
                            string _psd_name = @"Photoshop.Image.23";
                            string _psd_des = @"Adobe Photoshop 图像文件";
                            string _psd_ico = @"C:\Windows\psd.ico,0";
                            return CommonRegisteredVoidWithKey(".psd", _psd_des, _psd_name, _psd_ico);
                        }
                        catch (Exception ex)
                        {
                            WriteLog.Error(_Exception_With_xKind("Extension.Adobe.Psd", ex));
                            return false;
                        }
                    }
                    /// <summary>
                    /// 注册 Adobe Premiere Pro 项目文件(.prproj)文件扩展名
                    /// </summary>
                    /// <returns><see langword="true"/>/<see langword="false"/></returns>
                    public static bool Prproj()
                    {
                        try
                        {
                            string _prproj_name = @"Adobe.Premiere.Pro.Project.13";
                            string _prproj_des = @"Adobe Premiere Pro 项目文件";
                            string _prproj_ico = @"C:\Windows\prproj.ico,0";
                            return CommonRegisteredVoidWithKey(".prproj", _prproj_des, _prproj_name, _prproj_ico);
                        }
                        catch (Exception ex)
                        {
                            WriteLog.Error(_Exception_With_xKind("Extension.Adobe.Prproj", ex));
                            return false;
                        }
                    }
                }
            }
        }
    }
}
