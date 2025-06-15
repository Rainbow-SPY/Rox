![Icon](Rox/logo+Text.png)
___
Rox 是一个使用 C# .NET Framework 4.7.2 编写，并使用 Microsoft Visual Studio 2022 编译的跨平台动态链接库。它提供了多种功能模块，包括日志记录、文件操作、网络检查、Windows 系统配置、AI 集成,**音频解密**等。

## 提示! 
> [!WARNING]
> [5f6e7e44](https://github.com/Rainbow-SPY/Rox/commit/5f6e7e443fd479705cd078bc9bd5bac9d79b45df) 提交在Main.cs中将`LocalizedString`本地化字符串 和 `LogLibraries`日志库组件迁移到`Runtimes.cs`中, v1.5版本后的主模块和其他模块需要引用运行库进行操作.
> ```csharp
> using Rox.Runtimes;
> using static Rox.Runtimes.LogLibraries;
> using static Rox.Runtimes.LocalizedString;
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
11. [文本类处理](#11-文本类处理)
12. [API查询](#12-api查询)
13. [音乐解密](#13-音乐解密)
14. [Node.js](#14-nodejs)
15. [发送Windows通知](#15-发送Windows通知)
> 此方法要求系统版本为 Windows 10 以上

## 1. 全局引用方法

Rox 提供了多种引用方式，您可以根据需求选择合适的方式：

```csharp
using Rox;
using static Rox.Runtimes.LogLibraries;
using static Rox.Runtimes.LocalizedString;
...
```
## 2. 日志
### 控制台打印彩色日志
```csharp 
Rox.Runtimes.LogLibries.WriteLog(LogLevel loglevel, LogKind logkind,string message);
Rox.Runtimes.LogLibries.WriteLog(LogLevel loglevel,string message);
```

* **`LogLevel` 可用枚举:** `Info`,`Warning`,`Error`.
* **`LogKind` 可用枚举:** `Process`,`Task`,`Service`,`Registry`,`Network`,`PowerShell`,`Form`,`System`,`Thread`.
___
### 写入日志到文件

```csharp
Rox.Runtimes.LogLibries.LogToFile(LogLevel loglevel,LogKind logkind, string message);
Rox.Runtimes.LogLibries.LogToFile(LogLevel loglevel, string message);
```
调用此方法时,会在当前目录下创建`Assistant.log`日志文件,并会以下格式写入文件

 ```plaintext
 $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logkind}] [{logLevel}]: {message}";
 ```
___
### 清空日志

```csharp
Rox.Runtimes.LogLibries.ClearFile(string filePath);
```

* **`filepath`:** 日志文件路径

## 3. 自定义下载

> [!NOTE]
> 使用时请注意,在无网络的情况下,`MessageBox`会弹出提示询问是否进行下一步操作.

```csharp
Rox.DownloadAssistant.Downloader(string url);
Rox.DownloadAssistant.Downloader(string url,string Downloadvocation);
Rox.DownloadAssistant.Downloader(string url,string Downloadvocation,bool log);
Rox.DownloadAssistant.Downloader(string url,string Downloadvocation,string outputName);
Rox.DownloadAssistant.Downloader(string url,string Downloadvocation,string outputName,bool log);
```
* **`url`:** 下载链接

* **`Downloadvocation`**: 下载位置

* **`log`** : 是否启用日志输出

当`bool`为`true`时,日志会输出到`"{Directory.GetCurrentDirectory()}\\aria2c.log\`文件内,反之为`false`则不会.

* **`outputName`**: 文件的输出名称

文件下载完后会以`outputName`为命名储存在`Downloadvocation`文件夹内.
___
### 模块下载

```csharp
Rox.DownloadAssistant.ModuleDownloader(Module module);
```

* **`Module`可用枚举:** `zip`,`VC`.

当您使用`Module.VC`下载微软常用运行库时,此文件会保存在 `%USERPROFILLE%\Appdata\Local\Temp`文件夹内.  

当您使用`Module.zip`下载7-zip组件时, 此文件会保存在`$"{Directory.GetCurrentDirectory()}\\bin"`文件夹内.
<br>
___
### 应用下载

```csharp
Rox.DownloadAssistant.ApplicationDownloader(App app);
```
* **`App`可用枚举:** `EasiNote5`,`EasiCamera`,`SeewoService`,`WeChat`,`ToDesk`.

文件会保存在 `%USERPROFILLE%\Appdata\Local\Temp`文件夹内

## 4. 安全软件检测

```csharp
Rox.Security.Anti360Security();
Rox.Security.AntiHuoRongSecurity();
```

* **返回值:** `true` 表示安全软件正在运行，`false` 表示未运行。
## 5. 网络
### 网络可用性检查

```csharp
Rox.Runtimes.Network_I.IsNetworkAvailable();
```

- **返回值:** `true` 表示网络可用，`false` 表示网络不可用。

## 6. Windows系统相关配置

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
### 启用/禁用 Windows Update

```csharp
Rox.Windows.WindowsUpdate.Enable() //启用Windows 更新
Rox.Windows.WindowsUpdate.Disable() //禁用Windows 更新
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
bool _return = Rox.Windows.BoolActiveWindows(); //激活Windows并返回值
```
* **返回类型:** bool
* **返回值:** 激活成功返回`true`,激活失败或遇到未知错误返回`false`
___
### 写入注册表

```csharp
Rox.Runtimes.Registry_1.Write(string keyPath,string valueName,object valueData,RegistryValueKind valueType);
```

* **`keyPath`: 设定注册表路径**

* **`valueName`: 设定注册表项名称**

* **`RegistryKind`可用枚举:** `DWord`,`QWord`,`String`等,详细帮助请查看[官方界面](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.win32.registryvaluekind?view=net-8.0)

* **`valueType`:设定注册表项内数据类型**

* **`valueData`:设定注册表项内数据**
<br>

## 7. AI

### ChatGPT API引用

```csharp
Rox.AI.ChatGPT.Chat(string text,string api); //ChatGPT API引用
```
* **`text`: 向 `api.openai.com/v1/completions` 发送的请求**

* **`api`: OpenAI Platform 网站申请的API**
<br>

### DeepSeek API引用

```csharp
Rox.AI.DeepSeek.Chat(string text,string api); //DeepSeek API引用
```

* **`text`: 向 `api.deepseek.com/v1/completions` 发送的请求**

* **`api`: DeepSeek Platform 网站申请的API**

## 8. 文件

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

## 9. Windows 身份验证

```csharp
Rox.Windows.Authentication();
```
* **返回类型: `bool`**
* **返回值:** `true` 表示验证成功，`false` 表示取消操作。

## 10. 检查更新模块
> [!NOTE]
> **此类因长时间未进行维护而暂停使用**


### 检查更新
```csharp
await Rox.Update.CheckUpdate(string CheckUpdateUrl,Platform platform);
```

* **`CheckUpdateUrl`: 检查更新的API链接**
* **`Platform`可用枚举:** `Github`,`Gitee`.
___
 Github API规定的Release最新发行版查询地址为`https://api/github.com/repos/{用户名}/{仓库}/releases/latest`

 Gitee API规定的Release最新发行版查询地址为`https://gitee.com/api/v5/repos/{用户名}/{仓库}/releases/latest`

 返回的json中包含了最新发行版的信息，包括版本号、发布时间、下载地址等 例如,最新的版本号为 "tag_name": "v1.4","name": "新版本发布...."

 当检测出新版本时,会返回`true`,反之则为`false`,当Json解析错误时也会返回`false`.
 ___
 ### 自动更新
 ```csharp
 Rox.Update.SelfUpdate(); // 自动更新
 ```
 * **返回类型:** `MessageBox`
 ___
规定 在压缩包内包含了 `update.ini` 和 `filehash.ini` 文件,以及更新文件     
___
规定 `Update_{version}.zip` 规格:
```
 压缩包文件目录:  
    Update_{version}.zip       // 更新文件压缩包    
    ├── update.ini             // 更新信息    
    ├── filehash.ini           // 文件哈希值        
    └── #(update files)        // 更新文件 
```
___
 规定 ``update.ini`` 规格:
```
    1 > version = ""                              // 版本号     
    2 > type = [Release / HotFix / bugFix]        // 更新类型       
    3 > description = ""                          // 更新说明        
    4 > updatefilecount = ""                      // 更新文件数量       
    5 > hashurl = ""                              // 哈希值文件下载地址       
    6 > hash = ""                                 // 文件数量 
```
___
 规定 ``filehash.ini`` 规格:
```
    > {fileName},{fileHash}         
示例: 
    1 > Library.dll,4CC1ED4D70DFC8A7455822EC8339D387
    2 > Library.pdb,FDFA7596701DCC2E96D462DBC35E7823
```           
## 11. 文本类处理
### 加密 / 解密字符串
```csharp
Rox.Text.EncryptString(string str); //加密
Rox.Text.DecryptString(string str); //解密
```
* **`str`:** 要加密或解密的字符串
* **返回类型:** `string`
* **返回值:** 加密或解密后的字符串
  
在加密或解密之前, Rox会先解包 Node.js, 使用Node.js执行js脚本进行加解密

### 读取 / 写入配置文件
```csharp
Rox.Text.Config.ReadConfig(string iniPath,string HeadText); //读取配置文件
Rox.Text.Config.WriteConfig(string iniPath,string HeadText,string Value); //写入配置文件
```

* **`iniPath`:** 配置文件路径
* **`HeadText`:** 配置文件头部文本
* **`Value`:** 配置的值
* **返回类型:** `string`
* **返回值:** 返回配置文件头部文本对应的值
___
规定配置文件规格:
```plainText
    1> HeadText1 = value1
    2> HeadText2 = value2
    3> HeadText3 = value3
```
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
## 12. API查询

### Steam个人信息查询(可等待)
```csharp
Rox.API.SteamUserData.GetDataJson(string SteamID);

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

| 属性  | 注释 |
| :------------: |:------------: |
| code | ⚠️**此属性为int类型** HttpClient返回值 (200成功,432无玩家信息,443无效的输入) |
| communitystate | 社区隐私状态 |
| steamID | SteamID ( STEAM_0:1:xxxxxxxx ) |
| steamID3 |  SteamID3 ( [U:1:xxxxxxx] ) |
| steamID64 | SteamID64 ( 7656xxxxxxxx ) |
| username | 用户名 |
| realname | 真实姓名 |
| profileurl | ⚠️个人主页链接, 原生属性使用会**带有转义字符**(https:\\/\\/) |
| profileurl_1 | ✔️个人主页链接, 使用此属性可输出**无转义符的网址** | 
| avator | ⚠️头像链接, 原生属性使用会**带有转义字符**(https:\\/\\/) |
| avator_1 | ✔️个人主页链接, 使用此属性可输出**无转义符的网址** | 
| accountcreationdate | 账号创建时间 |
| lastlogoff | 上次登出时间 |
| location | 账号绑定区域 |
| onlinestatus | 在线状态 |
| friendcode | 好友代码 |
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
