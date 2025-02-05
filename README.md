# NinjaMagisk 动态链接库
NinjaMagisk 是使用C# .NET Framework 4.7.2 编写, 使用 Microsoft Visual Studio 2022 编译的跨平台动态链接库

## 全局引用方法

NinjaMagisk 给出了多种方法
```csharp
using NinjaMagisk;
```

```csharp
using static NinjaMagisk.LogLibraries;
```

```csharp
using static NinjaMagisk.Software;
```

```csharp
using static NinjaMagisk.Windows;
```

## 所有方法

<!-- 这里使用javascript只是为了方便可视化代码-->

### 控制台打印彩色日志

<!-- 这里使用javascript只是为了方便可视化代码-->

```javascript 
NinjaMagisk.LogLibries.WriteLog(LogLevel loglevel, LogKind logkind,string message);
NinjaMagisk.LogLibries.WriteLog(LogLevel loglevel,string message);
```



**`LogLevel` 可用枚举:** `Info`,`Warning`,`Error`.

> [!NOTE]
> 当您使用`LogLevel.Error`时,除`LogKind`的显示字符区域外,所有字符均为红色.

**`LogKind` 可用枚举:** `Process`,`Task`,`Service`,`Rgistry`,`Network`,`PowerShell`,`Form`,`System`,`Thread`.

### 写入日志
```javascript
NinjaMagisk.LogLibries.LogToFile(LogLevel loglevel,LogKind logkind, string message);
NinjaMagisk.LogLibries.LogToFile(LogLevel loglevel, string message);
```
> [!NOTE]
> 调用此方法时,会在当前目录下创建`Assistant.log`日志文件,并会以下格式写入文件
> ```javascript
> $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logkind}] [{logLevel}]: {message}";
> ```


**`LogLevel` 可用枚举:** `Info`,`Warning`,`Error`.

**`LogKind` 可用枚举:** `Process`,`Task`,`Service`,`Rgistry`,`Network`,`PowerShell`,`Form`,`System`,`Thread`.
### 清空日志
```javascript
NinjaMagisk.LogLibries.ClearFile(string filePath);
```
> [!NOTE]
> 调用此方法时,请确保`filePath`的文件路径有效,否则会提示`$"fail to clear log file: {ex.Message}"`

### 高速下载
> [!WARNING]
> 此处使用了aria2c库,作者遵循GPL 2.0协议,内容为未修改或修改过的程序,均为在[Github - aria2/aria2](github.com/aria2/ari2)上下载.作者仅提供方法脚本,对使用者进行的任何操作均不承担任何责任,特此声明.

```JavaScript
NinjaMagisk.Software.DownloadAssistant.Downloader(string url);
NinjaMagisk.Software.DownloadAssistant.Downloader(string url,string Downloadvocation);
NinjaMagisk.Software.DownloadAssistant.Downloader(string url,string Downloadvocation,bool log);
NinjaMagisk.Software.DownloadAssistant.Downloader(string url,string Downloadvocation,string outputName);
NinjaMagisk.Software.DownloadAssistant.Downloader(string url,string Downloadvocation,string outputName,bool log);
```
**`url` 下载链接**

**`Downloadvocation` 下载位置:** 请确保位置有效

**`log` 是否启用日志输出:** 当`bool`为`true`时,日志会输出到`"{Directory.GetCurrentDirectory()}\\aria2c.log\`文件内,反之为`false`则不会.

**`outputName` 文件的输出名称:** 下载完后会以`outputName`为命名储存在`Downloadvocation`文件夹内.

### 模块下载

```JavaScript
NinjaMagisk.Software.DownloadAssistant.ModuleDownloader(Module module);
```

> [!WARNING]
> 使用时请注意,在无网络的情况下,`MessageBox`会弹出提示询问是否进行下一步操作.
> 
> Visual++运行库产品来自MicroSoftware,© MicroSoftware 2025,作者未对程序和DLL进行修改.作者仅提供方法脚本,对使用者进行的任何操作均不承担任何责任,特此声明.
>
> `7-zip` 产品来自[ww.7-zip.org](www.7-zip.org),Copyright (C) 2024 Igor Pavlov.作者未对此进行任何修改,遵循 GNU LGPL license.作者仅提供方法脚本,对使用者进行的任何操作均不承担任何责任,特此声明.

**`Module`可用枚举:** `zip`,`VC`.

> [!NOTE]
> 当您使用`DownloadAssistant.ModuleDownlaoder(DownloadAssistan.Nodule.VC)`时,请注意! 此文件会保存在 `%USERPROFILLE%\Appdata\Local\Temp`文件夹内.
> 
> 当您使用`DownloadAssistant.ModuleDownlaoder(DownloadAssistan.Nodule.zip)`时,请注意! 此文件会保存在`$"{Directory.GetCurrentDirectory()}\\bin"`文件夹内.

### 应用下载

```JavaScript
NinjaMagisk.Software.DownloadAssistant.ApplicationDownloader(App app);
```
**`App`可用枚举:** `EasiNote5`,`EasiCamera`,`SeewoService`,`WeChat`,`ToDesk`.

> [!WARNING]
> 请注意! `EasiNote5`,`EasiCamera`,`SeewoService`产品来自广州视源电子科技股份有限公司,Copyright © 2023 seewo. All Rights Reserved. Shirui Electronics.作者只提供了直链下载链接,未对产品做出任何修改行为.作者仅提供方法脚本,对使用者进行的任何操作均不承担任何责任,特此声明.
>
> `WeChat`产品来自腾讯公司,Copyright © 1998-2025 Tencent All Rights Reserved.作者只提供了网页链接,未对产品做出任何修改.
>
> `ToDesk`产品来自海南有趣科技有限公司, Copyright © 海南有趣科技有限公司 版权所有 ,作者只提供了直链下载链接,未对产品做出任何修改行为.作者仅提供方法脚本,对使用者进行的任何操作均不承担任何责任,特此声明.

> [!NOTE]
> 请注意!文件会保存在 `%USERPROFILLE%\Appdata\Local\Temp`文件夹内

### 安全软件检测

> [!WARNING]
> 此举未对火绒安全软件和360安全软件的任意进程、程序，组件、驱动做出任何修改、抹黑、分发,仅对相关进程检测提示防止安全软件误杀其他程序或组件.作者仅提供方法脚本,对使用者进行的任何操作均不承担任何责任,特此声明.

```javascript
NinjaMagisk.Software.AntiSecurity.Anti360Security();
NinjaMagisk.Software.AntiSecurity.AntiHuoRongSecurity();
```
> 示例:
> ```javascript
> if (NinjaMagisk.Software.AntiSecurity.Anti360Security())
> {
>    return true;
> }
> else
> {
>    return false;
> }
> ```

返回`true`即代表`360Tray.exe`或`HipsTray.exe`正在运行.
返回`false`即代表`360Tray.exe`或`HipsTray.exe`未运行.

### 网络可用性检查

```javascript
NinjaMagisk.Network.IsNetworkAvailable();
```

> 示例
> ```javascript
> if (NinjaMagisk.Network.IsNetworkAvailable())
> {
>    return true;
> }
> else
> {
>    return false;
> }
> ```

通过`NetworkInterface.GetIsNetworkAvailable()`检查网络是否可用,并进一步使用`PingReply.Send("8.8.8.8",2000);`检查是否能够向 Google 的公共 DNS 通信.

返回`true`即代表网络可用.
返回`false`即代表网络不可用.

### 启用/禁用休眠

```javascript
NinjaMagisk.Windows.Hibernate.Enable(); //启用休眠
NinjaMagisk.Windows.Hibernate.Disable(); //禁用休眠
```
通过向`powercfg.exe`发送`"/hibernate {on/off}"`指令实现操作.

### 启用卓越性能

> [!WARNING]
> 此操作需要性能较高配置的电脑,此操作执行后可能会存在耗电过快等情况,较低配置的电脑启用后可能会出现卡顿等问题,建议非高配电脑请使用`高性能`电源方案,请谨慎选择!作者仅提供方法脚本,对使用者进行的任何操作均不承担任何责任,特此声明.

```javascript
NinjaMagisk.Windows.EnableHighPowercfg();
```
通过向`powercfg.exe`发送`"-duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61"` GUID 指令实现操作.

### 启用/禁用Windows 安全中心与Windows Defender

> [!WARNING]
> 此操作会禁用Windows Defender防间谍软件和实时监控等行为,计算机安全系数会下降.
>
> 此方法仅操作注册表进行读取写入,未对Microsoft Windows操作系统的任何其他组件、驱动、程序、进程做出任何修改,作者仅提供方法脚本,对使用者进行的任何操作均不承担任何责任,特此声明.

```JavaScript
NinjaMagisk.Windows.WindowsSecurityCenter.Enable() //启用Windows 安全中心 和 Windows Defender
NinjaMagisk.Windows.WindowsSecurityCenter.Disable() //禁用Windows 安全中心 和 Windows Defender
```
通过读取写入组策略和注册表来实现操作.

> [!NOTE]
> 在第三方安全软件(非Windows Defender)运行时,执行步骤后,MessageBox会提示您需要关闭安全软件以进行下一步操作

### 启用/禁用 Windows Update

> [!WARNING]
> 此操作会禁用Windows 更新,您将无法接收到最新的更新和补丁.
>
> 此方法仅使用第三方程序操作注册表进行读取写入,未对Microsoft Windows操作系统的任何其他组件、驱动、程序、进程做出任何修改,作者仅提供方法脚本,对使用者进行的任何操作均不承担任何责任,特此声明.

```JavaScript
NinjaMagisk.Windows.WindowsUpdate.Enable() //启用Windows 更新
NinjaMagisk.Windows.WindowsUpdate.Disable() //禁用Windows 更新
```
通过使用第三方程序读取写入注册表来实现操作.

> [!NOTE]
> 在第三方安全软件(非Windows Defender)运行时,执行步骤后,MessageBox会提示您需要关闭安全软件以进行下一步操作

### 激活 Windows

> [!WARNING]
> 此操作会使用第三方程序激活Windows,如果您是正版用户,请不要使用此方法.
>
> 此方法仅使用第三方程序操作,未对Microsoft Windows操作系统的任何其他组件、驱动、程序、进程做出任何修改,作者仅提供方法脚本,对使用者进行的任何操作均不承担任何责任,特此声明.

```javascript
NinjaMagisk.Windows.ActiveWindows();
```
通过使用第三方程序来实现操作.

> [!NOTE]
> 在第三方安全软件(非Windows Defender)运行时,执行步骤后,MessageBox会提示您需要关闭安全软件以进行下一步操作

### 写入注册表

> [!WARNING]
> 此方法仅操作注册表读取写入,未对Microsoft Windows操作系统的任何其他组件、驱动、程序、进程做出任何修改,作者仅提供方法脚本,对使用者进行的任何操作均不承担任何责任,特此声明.

```JavaScript
NinjaMagisk.Registry.Write(string keyPath,string valueName,RegistryValueKind valueType,object valueData);
```

**`keyPath`: 设定注册表路径**

**`valueName`: 设定注册表项名称**

**`RegistryKind`可用枚举:** `DWord`,`QWord`,`String`等,详细帮助请查看[官方界面](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.win32.registryvaluekind?view=net-8.0)

**`valueType`:设定注册表项内数据类型**

**`valueData`:设定注册表项内数据**
