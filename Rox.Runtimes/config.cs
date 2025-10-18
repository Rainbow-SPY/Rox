using Microsoft.Win32;
using static Rox.Runtimes.LogLibraries;

// gugugaga
// Created by Rainbow-SPY
// Copyright (C) 2019-2025 Rainbow-SPY, All Rights Reserved.
namespace Rox.Runtimes
{
    /// <summary>
    /// 配置相关操作
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 注册 Rox 相关文件扩展名
        /// </summary>
        public static void RegisteredFileExt()
        {
            GetSystemInfo.InitializeSystemInfo();
            string getlan = GetSystemInfo.SystemLanguage;
            WriteLog.Info($"Current System Language: {getlan}");
            // ------------------------------------------------------------------
            #region registered cfg file
            RegistryKey key = Registry.ClassesRoot.CreateSubKey(".rxcfg");
            key.SetValue("", "rxcfgfile");
            WriteLog.Info(LogKind.Registry, "Registered .rxcfg file extension");
            key.Close();
            RegistryKey key2 = Registry.ClassesRoot.CreateSubKey("rxcfgfile");
            if (getlan != "zh-CN")
            {
                key2.SetValue("", "Rox Config File");
                WriteLog.Info(LogKind.Registry, "Registered .rxcfg file extension as Rox Config File");
            }
            else
            {
                key2.SetValue("", "Rox 配置文件");
                WriteLog.Info(LogKind.Registry, "Registered .rxcfg 文件扩展名为 Rox 配置文件");
            }
            key2.Close();
            #endregion
            // ------------------------------------------------------------------
            #region registered temp file
            RegistryKey a = Registry.ClassesRoot.CreateSubKey(".rxtemp");
            a.SetValue("", "rxtempfile");
            WriteLog.Info(LogKind.Registry, "Registered .rxtemp file extension");
            a.Close();
            RegistryKey b = Registry.ClassesRoot.CreateSubKey("rxtempfile");
            if (getlan != "zh-CN")
            {
                b.SetValue("", "Rox Temporary File");
                WriteLog.Info(LogKind.Registry, "Registered .rxtemp file extension as Rox Temporary File");
            }
            else
            {
                b.SetValue("", "Rox 缓存文件");
                WriteLog.Info(LogKind.Registry, "Registered .rxtemp 文件扩展名为 Rox 缓存文件");
            }
            b.Close();
            #endregion
            // ------------------------------------------------------------------
            #region registered ralog file
            RegistryKey c = Registry.ClassesRoot.CreateSubKey(".ralog");
            c.SetValue("", "ralogfile");
            WriteLog.Info(LogKind.Registry, "Registered .ralog file extension");
            c.Close();
            RegistryKey d = Registry.ClassesRoot.CreateSubKey("ralogfile");
            if (getlan != "zh-CN")
            {
                d.SetValue("", "Rox Log File");
                WriteLog.Info(LogKind.Registry, "Registered .ralog file extension as Rox Log File");
            }
            else
            {
                d.SetValue("", "Rox 日志文件");
                WriteLog.Info(LogKind.Registry, "Registered .ralog 文件扩展名为 Rox 日志文件");
            }
            d.Close();
            // 将其关联到记事本
            RegistryKey e = Registry.ClassesRoot.CreateSubKey("ralogfile\\shell\\open\\command");
            e.SetValue("", $"notepad.exe \"%1\"");
            WriteLog.Info(LogKind.Registry, "Associated .ralog file extension with Notepad");
            e.Close();
            #endregion
        }
        /// <summary>
        /// 注册自定义文件扩展名
        /// </summary>
        /// <param name="extension"> 文件扩展名, 例如 .abc 或 abc </param>
        /// <param name="description"> 文件类型描述 </param>
        public static void RegisteredCustomFileExt(string extension, string description)
        {
            string ext = "";
            if (!extension.StartsWith("."))
            {
                ext = extension;
                extension = "." + extension;
            }
            else
                ext = extension.Replace(".", "");

            RegistryKey a = Registry.ClassesRoot.CreateSubKey(extension);
            a.SetValue("", ext);
            WriteLog.Info(LogKind.Registry, $"Registered {extension} file extension");
            a.Close();
            RegistryKey b = Registry.ClassesRoot.CreateSubKey(ext);
            b.SetValue("", description);
            WriteLog.Info(LogKind.Registry, $"Registered {extension} file extension as {description}");
            b.Close();
        }
        /// <summary>
        /// 注册自定义文件扩展名并关联打开命令
        /// </summary>
        /// <param name="extension"> 文件扩展名, 例如 .abc 或 abc </param>
        /// <param name="description"> 文件类型描述 </param>
        /// <param name="opencommand"> 打开命令, 例如 "notepad.exe \"%1\"" </param>
        public static void RegisteredCustomFileExt(string extension, string description, string opencommand)
        {
            string ext = "";
            if (!extension.StartsWith("."))
            {
                ext = extension;
                extension = "." + extension;
            }
            else
                ext = extension.Replace(".", "");
            RegisteredCustomFileExt(extension, description);
            RegistryKey a = Registry.ClassesRoot.CreateSubKey($"{ext}\\shell\\open\\command");
            a.SetValue("", opencommand);
            WriteLog.Info(LogKind.Registry, $"Associated {extension} file extension with {opencommand}");
            a.Close();
        }
    }
}
