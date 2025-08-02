![Icon](Rox/logo+Text.png)

Rox 是一个使用 C# .NET Framework 4.7.2 编写，并使用 Microsoft Visual Studio 2022 编译的跨平台动态链接库。它提供了多种功能模块，包括日志记录、文件操作、网络检查、Windows 系统配置、AI 集成,**音频解密**等。

---
## 📜 License / 许可证  
This project is licensed under **AGPL-3.0 + Attribution + Non-Commercial terms**.  
- 🔍 **You must**:  
  - Keep original author attribution and repository link.  
  - Open-source any modified versions under AGPL-3.0.  
- 🚫 **You cannot**:  
  - Use this code (or derivatives) for commercial purposes.  
- 📂 See [LICENSE](LICENSE) for full terms.  

本项目采用 **AGPL-3.0 + 署名 + 非商业附加条款** 许可协议。  
- 🔍 **您必须**：  
  - 保留原始作者署名及仓库链接。  
  - 任何修改后的版本必须以 AGPL-3.0 协议开源。  
- 🚫 **您不得**：  
  - 将此代码（或衍生作品）用于商业用途。  
- 📂 完整条款参见 [LICENSE](LICENSE)。  

---

## 目录
1. [日志](#2-日志)
2. [自定义下载](#3-自定义下载)
3. [安全软件检测](#4-安全软件检测)
4. [网络](#5-网络)
5. [Windows系统相关配置](#6-Windows系统相关配置)
6. [AI](#7-AI)
7. [文件](#8-文件)
8. [Windows身份验证](#9-Windows身份验证)
9. [检查更新模块](#10-检查更新模块)
10. [文本类处理](#11-文本类处理)
11. [API查询](#12-api查询)
12. [音乐解密](#13-音乐解密)
13. [Node.js](#14-nodejs)
14. [发送Windows通知](#15-发送Windows通知)
> 此方法要求系统版本为 Windows 10 以上

## 1. 日志
### 控制台打印彩色日志
```csharp 
Rox.Runtimes.LogLibries.WriteLog.Info(LogKind logkind,string message);
Rox.Runtimes.LogLibries.WriteLog.Warning(LogKind logkind,string message);
Rox.Runtimes.LogLibries.WriteLog.Error(string message);
Rox.Runtimes.LogLibries.WriteLog.Debug(string message);
```

* **`LogKind` 可用枚举:** `Process`,`Task`,`Service`,`Registry`,`Network`,`PowerShell`,`Form`,`System`,`Thread`, `Json`, `Regex`, `Downloader`.
___
### 写入日志到文件

```csharp
Rox.Runtimes.LogLibries.LogToFile(LogLevel loglevel,LogKind logkind, string message);
Rox.Runtimes.LogLibries.LogToFile(LogLevel loglevel, string message);
```
调用此方法时,会程序目录下创建`Assistant.log`日志文件,并会以下格式写入文件

 ```plaintext
 $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] [{logkind}]: {message}";
 ```
___
### 清空日志

```csharp
Rox.Runtimes.LogLibries.ClearFile(string filePath);
```

* **`filepath`:** 日志文件路径

## 2. 自定义下载

```csharp
Rox.DownloadAssistant.Downloader(string url);
Rox.DownloadAssistant.Downloader(string[] url);
Rox.DownloadAssistant.Downloader(string url,string location);
Rox.DownloadAssistant.Downloader(string url,string location);
Rox.DownloadAssistant.Downloader(string url,string location,bool log);
+2 重载...
```

* **`url(s)`:** 下载链接

* **`location`**: 下载位置

* **`log`** : 是否启用日志输出

当`bool`为`true`时,日志会输出到程序目录下的`aria2c.log`文件内,反之`false`则不会.
___
## 3. 安全软件检测

```csharp
Rox.Security.Is360SafeRunning();
Rox.Security.IsHuorongSecurityRunning();
```

* **返回值:** `true` 表示安全软件正在运行，`false` 表示未运行。
## 4. 网络
### 网络可用性检查

```csharp
Rox.Runtimes.Network_I.IsNetworkAvailable();
```

- **返回值:** `true` 表示网络可用，`false` 表示网络不可用。

## 5. Windows系统相关配置

> [!WARNING]
> 此操作执行后可能会影响系统性能和安全。
### 启用/禁用休眠

```csharp
Rox.Windows.Hibernate.Enable(); //启用休眠
Rox.Windows.Hibernate.Disable(); //禁用休眠
```
___
### 启用卓越性能

```csharp
Rox.Windows.EnableHighPowercfg(); //启用卓越性能
```
___
### 启用/禁用Windows 安全中心与Windows Defender

```csharp
Rox.Security.WindowsSecurityCenter.Enable() //启用
Rox.Security.WindowsSecurityCenter.Disable() //禁用
```
___
### 检查 Windows Update状态

```csharp
bool status = Rox.Windows.WindowsUpdate.CheckStatus();
```
* **返回类型:** bool
* **返回值:** 已禁用更新返回`false`.已启用更新返回`true`,键值不存在或遇到未知错误返回`false`
___
### 激活 Windows

```csharp
Rox.Windows.ActiveWindows(); //激活Windows
```
通过 "irmhttps://get.activated.win | iex" 下载激活脚本完成激活
___
### 写入注册表

```csharp
Rox.Runtimes.Registry_I.Write(string keyPath,string valueName,object valueData,RegistryValueKind valueType);
```

* **`keyPath`: 设定注册表路径**

* **`valueName`: 设定注册表项名称**

* **`RegistryKind`可用枚举:** `DWord`,`QWord`,`String`等,详细帮助请查看[官方界面](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.win32.registryvaluekind?view=net-8.0)

* **`valueType`:设定注册表项内数据类型**

* **`valueData`:设定注册表项内数据**
<br>
## 6. 文件

### 文件属性修改

```csharp
Rox.Runtimes.File_I.FileProperties(string path, Properties key, bool Enable);
```

* **`path`: 文件的路径**
* **`Properties` 可用枚举:** `System`(设置文件为系统文件),`Hidden`(设置文件为受保护的隐藏文件),`Readonly`(设置文件为只读),`Archive`(设置文件为可存档文件).
* **`key`: 文件属性**
* **`Enable`: 启用或取消属性:** 设置为`true`时,给出的命令为`+r`(示例);设置为`false`时,给出的命令为`-r`(示例).

### MD5哈希值验证
```csharp
Rox.Runtimes.File_I.CheckFileHash(string filePath, string expectedMD5);
```

* **`filePath`: 文件路径**
* **`expectedMD5`: 期望的MD5哈希值**
* **返回类型: `bool`**
* **返回值: 文件的MD5哈希值与期望的MD5哈希值相同时,返回`true`,反之则为`false`.**

### 获取文件MD5哈希值
```csharp
Rox.Runtimes.File_I.CalculateMD5(string filePath);
```

* **`filePath`: 文件路径**
* **返回类型: `string`**
* **返回值: 文件的MD5哈希值**
## 7. Windows 身份验证

```csharp
Rox.Windows.Authentication();
```
* **返回类型: `bool`**
* **返回值:** `true` 表示验证成功，`false` 表示取消操作
## 8. 文本类处理
### 加密 / 解密字符串
```csharp
Rox.Text.EncryptString(string str); //加密
Rox.Text.DecryptString(string str); //解密
```
* **`str`:** 要加密或解密的字符串
* **返回类型:** `string`
* **返回值:** 加密或解密后的字符串
  
在加密或解密之前, Rox会先解包 Node.js, 使用Node.js执行js脚本进行加解密
___
### Json反序列化
```csharp
Rox.Text.Json.DeserializeObject<T>(string json);
Rox.Text.Json.DeserializeObject(string json);
```

* **`json`:** Json字符串*
* **返回类型:** `<T>`  `<dynamic>`
* **返回值:** 返回反序列化后的对象
> 注： `<dynamic>` 已经包含在 DeserializeObject(string json) 方法中，因为返回类型是 `<dynamic>`，所以不需要额外的方法。
___
### Json序列化
```csharp
Rox.Text.Json.SerializeObject(object obj);
```

* **`obj`:** 对象
* **返回类型:** `string`
* **返回值:** 返回序列化后的Json字符串
## 9. API查询

### Steam个人信息查询(兼容v1)(可等待)
```csharp
Rox.API.SteamUserData.GetDataJson(string SteamID);
Rox.API.SteamUserData_v1.GetDataJson_v1(string SteamID);

var type = await Rox.API.SteamUserData.GetDataJson(SteamID);
var info = type.$SteamType$;
```

* **`steamID`:** SteamID,支持SteamID3,ID64,个人主页链接,自定义URL,好友代码
* **`$SteamType$`:** 实际的 **SteamType** 属性
* **返回类型:** `Json`
* **返回值:** 返回Steam用户信息
<br><br>
举个例子: 获取好友代码及多个数值

```csharp
var type = await Rox.API.SteamUserData.GetDataJson("7656xxxxxxxx"); // 先获取返回的Json

string FriendCode = type.friendcode; // 提取好友代码属性值
strike username = type.username; //提取用户名属性值
...
```


**附: SteamType类属性**

| 属性  | 注释 | 返回类型 |
| :------------: |:------------: | :------------: |
| code | HttpClient返回值 (200成功,432无玩家信息,443无效的输入) | int |
| communityvisibilitystate | ⚠**仅v1** 社区隐私状态 | int |
| communitystate | ⚠**不兼容v1** 社区隐私状态 | string |
| steamID | ⚠**不兼容v1** SteamID ( STEAM_0:1:xxxxxxxx ) | string |
| steamID3 | ⚠**不兼容v1** SteamID3 ( [U:1:xxxxxxx] ) | string |
| steamID64 | ⚠**不兼容v1** SteamID64 ( 7656xxxxxxxx ) | string |
| steamid | ⚠**仅v1** SteamID64 ( 7656xxxxxxxx ) | string |
| username | ⚠**不兼容v1** 用户名 | string |
| personaname | ⚠**仅v1** 用户名 | string |
| realname | 真实姓名 | string |
| profileurl | ⚠️个人主页链接, 原生属性使用会**带有转义字符**(https:\\/\\/) |
| profileurl_1 | ✔️个人主页链接, 使用此属性可输出**无转义符的网址** | 
| avator | ⚠️头像链接, 原生属性使用会**带有转义字符**(https:\\/\\/) |
| avator_1 | ✔️个人主页链接, 使用此属性可输出**无转义符的网址** | 
| accountcreationdate | 账号创建时间 |
| lastlogoff | ⚠**不兼容v1** 上次登出时间 |
| location | 账号绑定区域 |
| onlinestatus | 在线状态 |
| friendcode | ⚠**不兼容v1** 好友代码 |
| profilestate| ⚠**仅v1** 如果属性返回 1 代表用户已经填写了个人资料 |
___
### Steam个人信息 - 直接方法调用 (可等待)

> **以下内容返回类型均为`string`**

```csharp
string text = await Rox.API.SteamUserData.$void$(string SteamID);

举个例子:
 string name = await Rox.API.SteamUserData.GetUserNameString("7656xxxxxxxx"); // 获取用户名
 string avator = await Rox.API.SteamUserData.GetAvatarString("7656xxxxxxxx"); // 获取头像链接
 ...
```
* **`$void$`:** 实际的直接调用方法  
* **`steamID`:** SteamID,支持SteamID3,ID64,个人主页链接,自定义URL,好友代码
  
  
通过对此类方法组的调用可以直接获取到字符串, 不用进一步解析Json

**附: 直接调用方法列表**

| 方法  |  返回值 | 注释 |
| :------------: | :------------: |:------------: |
| GetCommunityState | 目前状态 | 获取社区状态 |
| GetSteamIDString | SteamID | 获取SteamID |
| GetSteamID3String | SteamID3 | 获取SteamID3 |
| GetUserNameString | 用户名 | 获取Steam用户名 |
| GetSteamID64String | SteamID64 | 获取SteamID64 |
| GetSteamProfileUrlString | URL | 获取Steam个人主页链接地址 |
| GetAvatarString | URL | 获取Steam头像链接地址 |
| GetAccountCreationDateString | 日期 | 获取账号创建日期 |
| GetLastLogoffString | 日期 | 获取账号最后登出时间 |
| GetLocationString | 国家或地区 | 获取账号绑定区域 |
| GetOnlineStatusString | 目前状态 | 获取在线状态 |
| GetFriendCodeString | 好友代码 | 获取好友代码 |
| GetRealNameString | 真实姓名 | 获取真实姓名 |









___
### 天气查询(可等待)
```csharp
var allweather = await Rox.API.Weather.GetWeatherDataJson(string city);//获取返回的Json
string type = allweather.$WeatherType$; //获取属性值

举个例子:
var allweather = await Rox.API.Weather.GetWeatherDataJson("东城区");
string temperature = allweather.temperature_1; //获取气温属性值
```

* **`$WeatherType$`:** 实际的 **WeatherType** 属性
* **`city`:** 指定的地区
* **返回类型:** `Json`
* **返回值:** 天气信息

**附: WeatherType属性**

| 属性  | 注释 |
| :------------: |:------------: |
| code | ⚠️**此属性为int类型** HttpClient返回值 (200成功,500未查询到城市,400空输入,0非法/不安全的请求) |
| province | 省份名称 |
| city | 城市名称 |
| temperature | 气温⚠️**此属性不输出单位** 请使用`temperature_1`获取更好体验 |
| temperature_1 | 气温 ✔️ **此属性输出带有单位的字符串** |
| weather | 天气状况 |
| wind_direction | 风向 ⚠️**此属性不输出单位** 请使用`wind_direction_1`获取更好体验 |
| wind_direction_1 | 风向 ✔️**此属性输出带有单位的字符串** |
| wind_power | 风力等级 ⚠️**此属性不输出单位** 请使用`wind_power_1`获取更好体验|
| wind_power_1 | 风力等级 ✔️**此属性输出带有单位的字符串** |
| humidty | 湿度 ⚠️**此属性不输出单位** 请使用`humidty_1`获取更好体验|
| humidty_1 | 湿度 ✔️**此属性输出带有单位的字符串** |
| reporttime | 天气的更新时间 |
| msg | 错误信息 |
___
### 天气查询 - 直接方法调用

> **以下内容返回类型均为`string`**

```csharp
var text = await Rox.API.Weather.$void$(string city);

举个例子:
var temperature = await Rox.API.Weather.GetTemperature("东城区"); // 获取北京市东城区的气温
var weather = await Rox.API.Weather.GetWeather("黑河市"); //获取黑龙江省黑河市的天气状况
...
```
* **`$void$`:** 实际的直接调用方法
* **`city`:** 指定的地区  
  
  
通过对此类方法组的调用可以直接获取到字符串, 不用进一步解析Json

**附: 直接调用方法列表**

| 方法  |  返回值 | 注释 |
| :------------: | :------------: |:------------: |
| GetTemperature | 温度 ℃ | 获取指定地区的温度 |
| GetWeather | 天气 | 获取指定地区的天气 |
| GetWindDirection | 风向 风 | 获取指定地区的风向 |
| GetWindPower | 风力 级 | 获取指定地区的风力 |
| GetHumidity | 湿度 % | 获取SteamID64 |



## 13. 音乐解密

### 裤猫音乐解密
> [!WARNING]
> 请勿将此项目用于商业用途, 如造成的财产和版权损失与开发者改不相关

```csharp
Rox.Audio.ParseKGG.ChooseSigleKGGFile(); // 选择单一文件进行解密.kgg加密格式音频
Rox.Audio.ParseKGG.ReadKGGFiles(string filepath); //选择文件夹进行识别格式解密.kgg加密格式音频
Rox.Audio.ParseKGM.ChooseSigleKGMFile(); // 虚着呢单一文件进行解密.kgm加密格式音频
Rox.Audio.ParseKGM.ReadKGMFiles(string filepath); //选择文件夹进行识别格式解密.kgm加密格式音频
```

* **`filepath`:** 文件夹
## 14. Node.js

### 提取Node.js
```csharp
string _return = Rox.Runtimes.NodeJs.ExtractNodeJs(string ExtarctFolder); //提取Node.js到指定文件夹
```
> Node.js版本为node-v22.15.1-win-x86,使用32位
* **`ExtractFolder`:** 指定被提取的文件夹
* **返回类型:** `string`
* **返回值:** 参数为`null`或路径不合法返回`Error`并弹出`MessageBox`, 如果文件夹已经存在`node.exe`或提取完成返回**Node.js文件路径**, 资源未找到返回`_RES_FILE_NOT_FIND`本地化字符串, 提取失败返回PowerShell进程退出码.

### 检查Node.Js是否存在
```csharp
Rox.Runtimes.NodeJs.CheckNodeJs(string ExtraedFolder); //检查文件夹内有没有提取过的Node.js
```
* **`ExtractedFolder`:** 指定提取过的文件夹
* **返回类型:** `string`
* **返回值:** 参数为`null`或路径不合法返回`Error`并弹出`MessageBox`, 如果文件夹已经存在`node.exe`或提取完成返回**Node.js文件路径**, 资源未找到返回`_RES_FILE_NOT_FIND`本地化字符串, 提取失败返回PowerShell进程退出码.

## 15. 发送Windows通知

> 此功能要求系统版本为Windows 10以上

```csharp
Rox.Runtimes.WindowsToast.PostToastNotification(string title, string content);
```
* **`title`:** 发送通知的标题
* **`content`:** 发送通知的内容
> 暂不支持图片, 后续会加入

发送通知前会在本地的 `bin` 文件夹下生成 `WindowsToast.exe` 用来发送通知
