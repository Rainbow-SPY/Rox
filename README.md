# NinjaMagisk 动态链接库
NinjaMagisk 是使用C# .NET Framework 4.7.2 编写, 使用 Microsoft Visual Studio 2022 编译的跨平台动态链接库

## 目录

1. [全局引用方法](#1-全局引用方法)
2. [日志](#2-日志)
3. [自定义下载](#3-自定义下载)
4. [安全软件检测](#4-安全软件检测)
5. [网络](#5-网络)
6. [Windows系统相关配置](#6-Windows系统相关配置)
7. [AI](#7-AI)
8. [文件](#8-文件)
9. [Windows身份验证](#9-Windows身份验证)
10. [检查更新模块](#10-检查更新模块)

## 1. 全局引用方法

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

## 2. 日志

### 控制台打印彩色日志
<!-- 这里使用javascript只是为了方便可视化代码-->


```javascript 
NinjaMagisk.LogLibries.WriteLog(LogLevel loglevel, LogKind logkind,string message);
NinjaMagisk.LogLibries.WriteLog(LogLevel loglevel,string message);
```


* **`LogLevel` 可用枚举:** `Info`,`Warning`,`Error`.

* **`LogKind` 可用枚举:** `Process`,`Task`,`Service`,`Registry`,`Network`,`PowerShell`,`Form`,`System`,`Thread`.
<br>

### 写入日志到文件

```javascript
NinjaMagisk.LogLibries.LogToFile(LogLevel loglevel,LogKind logkind, string message);
NinjaMagisk.LogLibries.LogToFile(LogLevel loglevel, string message);
```
调用此方法时,会在当前目录下创建`Assistant.log`日志文件,并会以下格式写入文件

> ```javascript
> $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logkind}] [{logLevel}]: {message}";
> ```


* **`LogLevel` 可用枚举:** `Info`,`Warning`,`Error`.

* **`LogKind` 可用枚举:** `Process`,`Task`,`Service`,`Rgistry`,`Network`,`PowerShell`,`Form`,`System`,`Thread`.

<br>

### 清空日志

```javascript
NinjaMagisk.LogLibries.ClearFile(string filePath);
```

* **`filepath`:** 设置文本长度为 0 字节实现清空文本
<br>

## 3. 自定义下载

> [!NOTE]
> 使用时请注意,在无网络的情况下,`MessageBox`会弹出提示询问是否进行下一步操作.

```JavaScript
NinjaMagisk.Software.DownloadAssistant.Downloader(string url);
NinjaMagisk.Software.DownloadAssistant.Downloader(string url,string Downloadvocation);
NinjaMagisk.Software.DownloadAssistant.Downloader(string url,string Downloadvocation,bool log);
NinjaMagisk.Software.DownloadAssistant.Downloader(string url,string Downloadvocation,string outputName);
NinjaMagisk.Software.DownloadAssistant.Downloader(string url,string Downloadvocation,string outputName,bool log);
```
* **`url` 下载链接**

* **`Downloadvocation` 下载位置:** 请确保位置有效

* **`log` 是否启用日志输出:** 当`bool`为`true`时,日志会输出到`"{Directory.GetCurrentDirectory()}\\aria2c.log\`文件内,反之为`false`则不会.

* **`outputName` 文件的输出名称:** 下载完后会以`outputName`为命名储存在`Downloadvocation`文件夹内.

<br>

### 模块下载

```JavaScript
NinjaMagisk.Software.DownloadAssistant.ModuleDownloader(Module module);
```

* **`Module`可用枚举:** `zip`,`VC`.

当您使用`Module.VC`下载微软常用运行库时,此文件会保存在 `%USERPROFILLE%\Appdata\Local\Temp`文件夹内.  

当您使用`Module.zip`下载7-zip组件时, 此文件会保存在`$"{Directory.GetCurrentDirectory()}\\bin"`文件夹内.
<br>

### 应用下载

```JavaScript
NinjaMagisk.Software.DownloadAssistant.ApplicationDownloader(App app);
```
* **`App`可用枚举:** `EasiNote5`,`EasiCamera`,`SeewoService`,`WeChat`,`ToDesk`.

请注意!文件会保存在 `%USERPROFILLE%\Appdata\Local\Temp`文件夹内
<br>

## 4. 安全软件检测

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

## 5. 网络

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

## 6. Windows系统相关配置

> [!WARNING]
> 此系列操作可能需要性能较高配置的电脑,执行后可能会存在耗电过快、无法接收到最新补丁、计算机安全系数下降等情况.  较低配置的电脑启用后可能会出现卡顿等问题,建议非高配电脑请使用`高性能`电源方案,请谨慎选择!
### 启用/禁用休眠

```javascript
NinjaMagisk.Windows.Hibernate.Enable(); //启用休眠
NinjaMagisk.Windows.Hibernate.Disable(); //禁用休眠
```
通过向`powercfg.exe`发送`"/hibernate {on/off}"`指令实现操作.
<br>

### 启用卓越性能

```javascript
NinjaMagisk.Windows.EnableHighPowercfg();
```
通过向`powercfg.exe`发送`"-duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61"` GUID 指令实现操作.
<br>

### 启用/禁用Windows 安全中心与Windows Defender

```JavaScript
NinjaMagisk.Windows.WindowsSecurityCenter.Enable() //启用Windows 安全中心 和 Windows Defender
NinjaMagisk.Windows.WindowsSecurityCenter.Disable() //禁用Windows 安全中心 和 Windows Defender
```
通过读取写入组策略和注册表来实现操作.
<br>

### 启用/禁用 Windows Update

```JavaScript
NinjaMagisk.Windows.WindowsUpdate.Enable() //启用Windows 更新
NinjaMagisk.Windows.WindowsUpdate.Disable() //禁用Windows 更新
```
通过使用第三方程序读取写入注册表来实现操作.
<br>

### 激活 Windows

```javascript
NinjaMagisk.Windows.ActiveWindows();
```
通过使用第三方程序来实现操作.
<br>

### 写入注册表

```JavaScript
NinjaMagisk.Registry.Write(string keyPath,string valueName,RegistryValueKind valueType,object valueData);
```

* **`keyPath`: 设定注册表路径**

* **`valueName`: 设定注册表项名称**

* **`RegistryKind`可用枚举:** `DWord`,`QWord`,`String`等,详细帮助请查看[官方界面](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.win32.registryvaluekind?view=net-8.0)

* **`valueType`:设定注册表项内数据类型**

* **`valueData`:设定注册表项内数据**
<br>

## 7. AI

### ChatGPT API引用

```javascript
NinjaMagisk.AI.ChatGPT.Chat(string text,string api);
```
* **`text`: 向 `api.openai.com/v1/completions` 发送的请求**

* **`api`: OpenAI Platform 网站申请的API**
<br>

### DeepSeek API引用

```JavaScript
NinjaMagisk.AI.DeepSeek.Chat(string text,string api);
```

* **`text`: 向 `api.deepseek.com/v1/completions` 发送的请求**

* **`api`: DeepSeek Platform 网站申请的API**

## 8. 文件

### AES加密/解密

> [!NOTE]
> 请妥善保管好您的`IV`算法初始化向量和您的`Key`密钥.

```javascript
NinjaMagisk.File.AESEncryption.Decrypt(string cipherText, byte[] Key /*256-bit*/ , byte[] IV /*128-bit*/ );//解密
NinjaMagisk.File.AESEncryption.Encrypt(string cipherText, byte[] Key /*256-bit*/ , byte[] IV /*128-bit*/ );//加密
```
* **`cipherText`: 要加密/解密的文本**
* **`key`: AES加密/解密密钥(256位)**
* **`IV`: AES加密/解密初始化向量(128位)**
<br>

### 文件属性修改

```javascript
NinjaMagisk.File.Attrib(string path, AtOp Key, bool Switch);
```

* **`path`: 文件的路径**
* **`AtOp`(Attrib Option) 可用枚举:** `System`(设置文件为系统文件),`Hidden`(设置文件为受保护的隐藏文件),`Readonly`(设置文件为只读),`Archive`(设置文件为可存档文件).
* **`Key`: 文件属性**
* **`Switch`: 启用或取消属性:** 设置为`true`时,给出的命令为`+r`(示例);设置为`false`时,给出的命令为`-r`(示例).

### MD5哈希值验证
```javascript
NinjaMagisk.File.CheckFileHash(string filePath, string expectedMD5);
```

* **`filePath`: 文件路径**
* **`expectedMD5`: 期望的MD5哈希值**
* **返回值: 文件的MD5哈希值**
* **返回类型: `bool`**
* **返回值: 文件的MD5哈希值与期望的MD5哈希值相同时,返回`true`,反之则为`false`.**

### 获取文件哈希值
```javascript
NinjaMagisk.File.CalculateMD5(string filePath);
```

* **`filePath`: 文件路径**
* **返回值: 文件的MD5哈希值**
* **返回类型: `string`**

## 9. Windows 身份验证

```javascript
NinjaMagisk.Windows.Authentication();
```

使用原生自带的Windows 安全中心身份验证UI验证用户名密码,暂不支持生物验证.

当验证成功时,返回`true`,取消操作则为`false`.

## 10. 检查更新模块

### 检查更新
```javascript
await NinjaMagisk.Update.CheckUpdate(string CheckUpdateUrl,Platform platform);
```

* **`CheckUpdateUrl`: 检查更新的API链接**
* **`Platform`可用枚举:** `Github`,`Gitee`.

 Github API规定的Release最新发行版查询地址为`https://api/github.com/repos/{用户名}/{仓库}/releases/latest`

 Gitee API规定的Release最新发行版查询地址为`https://gitee.com/api/v5/repos/{用户名}/{仓库}/releases/latest`

 返回的json中包含了最新发行版的信息，包括版本号、发布时间、下载地址等 例如,最新的版本号为 "tag_name": "v1.4","name": "新版本发布...."

 当检测出新版本时,会返回`true`,反之则为`false`,当Json解析错误时也会返回`false`.
 ___
 ### 自动更新
 ```javascript

 ```



该方法用于自动更新程序,通过云端下载更新文件并自动更新
           
规定 更新文件为 `Update_{version}.zip` ,并且在压缩包内包含了 `update.ini` 和 `filehash.ini` 文件,以及更新文件     
* **\{version}** 为版本号
```
 压缩包文件目录:  
    Update_{version}.zip       // 更新文件压缩包    
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
    > {fileName},{fileHash}         
示例: 
    1 > Library.dll,4CC1ED4D70DFC8A7455822EC8339D387
    2 > Library.pdb,FDFA7596701DCC2E96D462DBC35E7823
```           
      


