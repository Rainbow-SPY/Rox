![Icon](docs/logo+Text.png)

Rox 是一个使用 C# .NET Framework 4.7.2 编写，并使用 Microsoft Visual Studio 2026 编译的跨平台动态链接库。它提供了多种功能模块，包括日志记录、文件操作、网络检查、API请求多种功能、Windows 系统配置等。
> 更新到 2026年1月4日 6:53 PM.

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
## 常用功能
- [控制台日志输出](#控制台打印彩色日志)
- [Windows 安全中心 身份验证](#7-windows-身份验证)
- [Steam 个人信息公开摘要查询](#Steam个人信息查询兼容v1可等待)
- [当地天气查询](#天气查询可等待)

## 目录
### [操作手册](#操作手册-1)
- [日志](#1-日志)
	- [日志输出](#控制台打印彩色日志)
	- [写入日志到文件](#写入日志到文件)
	- [清空日志](#清空日志)
- [自定义下载](#2-自定义下载)
- [安全软件检测](#3-安全软件检测)
- [网络](#4-网络)
	- [网络可用性检查](#网络可用性检查)
- [Windows系统相关配置](#5-Windows系统相关配置)
	- [休眠](#启用禁用休眠)
	- [卓越性能](#启用卓越性能)
	- [启用禁用 Windows 安全中心 与 Windows Defender](#启用禁用windows-安全中心与windows-defender)
	- [检查 Windows 更新状态](#检查-windows-update状态)
	- [写入注册表](#写入注册表)
	- [读取注册表项值](#读取注册表项值)
- [文件](#6-文件)
	- [文件属性修改](#文件属性修改)
	- [MD5哈希值对比](##md5哈希值对比)
	- [获取文件MD5哈希值](#获取文件md5哈希值)
- [Windows 身份验证](#7-windows-身份验证)
- [文本类处理](#8-文本类处理)
	- [Json反序列化](#json反序列化)
	- [Json序列化](#json序列化)
- [API查询](#9-api查询)
	- [Steam 个人信息公开摘要(v1)](#steam个人信息查询兼容v1可等待)
	- [天气查询(v2)](#天气查询v2可等待)
- [游戏娱乐](#10-游戏娱乐)
	- [获取 Steam 安装路径](#获取-steam-安装路径)
	- [获取CS2安装路径](#获取cs2安装路径)
	- [Minecraft Java版 村庄英雄Buff加成的交易价格计算](#minecraft-java版-村庄英雄buff加成的交易价格计算)
	- [Epic Games 拉取免费游戏](#获取-Epic-免费游戏列表)

### [开发环境](#开发环境-1)

## 操作手册
### 1.日志
#### 控制台打印彩色日志
```csharp 
Rox.Runtimes.LogLibraries.WriteLog.Info(LogKind logkind,string message);
Rox.Runtimes.LogLibraries.WriteLog.Warning(string logkind,string message);
Rox.Runtimes.LogLibraries.WriteLog.Error(string message);
Rox.Runtimes.LogLibraries.WriteLog.Debug(string message);
```
支持3种重载, 可以自定义`LogKind`的类型, 显式使用字符串作为
* **`LogKind`: 日志报告的类型**   可用枚举详见 `Rox.Runtimes.LogLibraries.LogKind`
___
#### 写入日志到文件

```csharp
Rox.Runtimes.LogLibraries.LogToFile(LogLevel loglevel,LogKind logkind, string message);
Rox.Runtimes.LogLibraries.LogToFile
Rox.Runtimes.LogLibraries.LogToFile(LogLevel loglevel, string message);
```
调用此方法时,会程序目录下创建`log.ralog`日志文件,并会以下格式写入文件

 ```plaintext
 $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] [{logkind}]: {message}";
 ```
___
#### 清空日志

```csharp
Rox.Runtimes.LogLibraries.ClearFile(string filePath);
```

* **`filepath`:** 日志文件路径

### 2. 自定义下载

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

当`log`为`true`时,日志会输出到程序目录下的`aria2c.log`文件内,反之`false`则不会.
___
### 3. 安全软件检测

```csharp
bool Rox.Security.Is360SafeRunning();
bool Rox.Security.IsHuorongSecurityRunning();
```

* **返回值:** `true` 表示安全软件正在运行，`false` 表示未运行。
### 4. 网络
#### 网络可用性检查

```csharp
bool Rox.Runtimes.Network_I.IsNetworkAvailable();
```

- **返回值:** `true` 表示网络可用，`false` 表示网络不可用。

### 5. Windows系统相关配置

> [!WARNING]
> 此操作执行后可能会影响系统性能和安全。

#### 启用/禁用休眠

```csharp
Rox.Windows.Hibernate.Enable(); //启用休眠
Rox.Windows.Hibernate.Disable(); //禁用休眠
```
___
#### 启用卓越性能

```csharp
Rox.Windows.EnableHighPowercfg(); //启用卓越性能
```
___
#### 启用/禁用Windows 安全中心与Windows Defender

```csharp
Rox.Security.WindowsSecurityCenter.Enable() //启用
Rox.Security.WindowsSecurityCenter.Disable() //禁用
```
___
#### 检查 Windows Update状态

```csharp
bool Rox.Windows.WindowsUpdate.CheckStatus();
```
* **返回类型:** `bool`
* **返回值:** 已禁用更新返回`false`.已启用更新返回`true`,键值不存在或遇到未知错误返回`false`
___
#### 写入注册表

```csharp
Rox.Runtimes.Registry_I.Write(string keyPath,string valueName,object valueData,RegistryValueKind valueType);
```

* **`keyPath`: 设定注册表路径**

* **`valueName`: 设定注册表项名称**

* **`RegistryKind`可用枚举:** `DWord`,`QWord`,`String`等,详细帮助请查看[官方界面](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.win32.registryvaluekind?view=net-8.0)

* **`valueType`:设定注册表项内数据类型**

* **`valueData`:设定注册表项内数据**
<br>
___
#### 读取注册表项值

```csharp
Rox.Runtimes.Registry_I.GetRegistryValue(string keyName, string valueName);
```

* **`keyname`: 注册表项的路径**

* **`valuename`: 注册表项的名称**

* **返回类型:** `string`

### 6. 文件

#### 文件属性修改

```csharp
Rox.Runtimes.File_I.FileProperties(string path, Properties key, bool Enable);
```

* **`path`: 文件的路径**
* **`Properties` 可用枚举:** `System`(设置文件为系统文件),`Hidden`(设置文件为受保护的隐藏文件),`Readonly`(设置文件为只读),`Archive`(设置文件为可存档文件).
* **`key`: 文件属性**
* **`Enable`: 启用或取消属性:** 设置为`true`时,给出的命令为`+r`(示例);设置为`false`时,给出的命令为`-r`(示例).

#### MD5哈希值验证
```csharp
bool Rox.Runtimes.File_I.CheckFileHash(string filePath, string expectedMD5);
```

* **`filePath`: 文件路径**
* **`expectedMD5`: 期望的MD5哈希值**
* **返回类型: `bool`**
* **返回值: 文件的MD5哈希值与期望的MD5哈希值相同时,返回`true`,反之则为`false`.**

#### 获取文件MD5哈希值
```csharp
Rox.Runtimes.File_I.CalculateMD5(string filePath);
```

* **`filePath`: 文件路径**
* **返回类型: `string`**
* **返回值: 文件的MD5哈希值**
### 7. Windows 身份验证

```csharp
bool Rox.Windows.Authentication();
```
* **返回类型: `bool`**
* **返回值:** `true` 表示验证成功，`false` 表示取消操作
### 8. 文本类处理

#### Json反序列化
```csharp
Rox.Text.Json.DeserializeObject<T>(string json);
Rox.Text.Json.DeserializeObject(string json);
```

* **`json`:** Json字符串
* **返回类型:** `<T> JObject`
* **返回值:** 返回反序列化后的对象
> 注： `<dynamic>` 已经包含在 DeserializeObject(string json) 方法中，因为返回类型是 `<dynamic>`，所以不需要额外的方法。
___

#### Json序列化
```csharp
Rox.Text.Json.SerializeObject(object obj);
```

* **`obj`:** 对象
* **返回类型:** `string`
* **返回值:** 返回序列化后的Json字符串
### 9. API查询

#### Steam个人信息查询_v1(可等待)
```csharp
await Rox.GameExpansionFeatures.SteamUserData_v1.GetDataJson_v1(string SteamID);

var type = await Rox.GameExpansionFeatures.SteamUserData_v1.GetDataJson_v1(SteamID);
var info = type.$SteamType$;
```

* **`steamID`:** SteamID,支持SteamID3,ID64,个人主页链接,自定义URL,好友代码
<details><summary><code>$SteamType$</code>:</strong> 实际的 SteamType 属性</summary>

> | 属性  | 注释 |
> | :------------: |:------------: |
> | **`long`** steamid | SteamID64 ( 7656xxxxxxxx ) |
> | **`int`** communityvisibilitystate | 社区隐私状态, 1 为可见 3为隐藏 |
> | **`int`** profilestate| 如果属性返回 1 代表用户已经填写了个人资料 |
> | personaname | 用户名 |
> | profileurl | **带有转义符**的个人主页链接(https:\\/\\/) |
> | profileurl_1 | **无转义符**的个人主页链接 |
> | avator | **带有转义字符**的头像链接(https:\\/\\/) |
> | avator_1 | **无转义符**的头像链接 |
> | **`int`** personastate | 在线状态, 0-离线, 1-在线<br> 2-忙碌, 3-离开, 4-打盹, 5-想交易, 6-想玩。 |
> | realname | 真实姓名 |
> | primaryclanid | 主要组ID |
> | timecreated_str | 创建账号时间 |
> | loccountrycode | 账号绑定区域 (US/CN/HK)|
> | friendcode | 好友代码 |
> | steamID3 | SteamID3 ( [U:1:xxxxxxx] ) |

</details> 

* **返回类型:** `Json`
* **返回值:** 返回Steam用户信息

**附: SteamType类属性**
<!----| code | 返回值 |---->

___
#### 天气查询_v2(可等待)
```csharp
//获取返回的Json
await Rox.API.Weather_v1.GetWeatherDataJson(string city,bool extended = false, bool indices = false, bool forecast = false);
await Rox.API.Weather_v1.GetWeatherDataJson(int adcode,bool extended = false, bool indices = false, bool forecast = false);

string advice = [$WeatherObject$].life_indices.[$Life_Indices$].[$IndicesLevel$];


举个例子: 
var allweather = await Rox.API.Weather_v1.GetWeatherDataJson("东城区");
var allweather = await Rox.API.Weather_v1.GetWeatherDataJson(101101);
string temperature = allweather.temperature_1; //获取气温属性值
foreach (var _data in allweather.forecast)
{
	WriteLog.Info("Weather Forcast", $"{_data.date} 的天气预报:\n" +
		$"白天天气: {_data.weather_day}, 夜间天气: {_data.weather_night}\n" +
		$"最高温度: {_data.temp_max} ℃, 最低温度: {_data.temp_min} ℃\n" +
		$"降水量: {_data.precip} mm, 能见度: {_data.visibility} km, 紫外线指数: {_data.uv_index}");
}
string advice = allweather.life_indices.uv.advice;

```

> [!INFO]
> 提示:
>
> 单击展开属性列表
 <details> <summary><strong><code>$IndicesLevel$</code></strong>: 实际的 <strong>IndicesLevel</strong> 属性</summary>

> | 属性 | 注释 |
> | :--: | :--: |
> | level | 指数等级 |
> | brief | 指数简述 |
> | advice   | 指数建议 |
> 
</details>


<details><summary><strong><code>$WeatherType$</code>: </strong>实际的 <strong>WeatherType</strong> 属性</summary>

> | 属性  | 注释 |
> | :------------: |:------------: |
> | code | 错误代码 |
> | province | 省份名称 |
> | city | 城市名称 |
> | **`int`** adcode | 高德6位数字城市编码 |
> | weather | 天气状况 |
> | **`double`** temperature | 气温 |
> |  wind_direction | 风向 |
> |  wind_power | 风力等级 |
> | **`int`** humidty | 湿度 % |
> | report_time | 天气的更新时间 |
> | message | 错误信息 |
> | **`double`** temp_max | 最高气温 |
> | **`double`** temp_min | 最低气温 |

</details>


<details><summary><strong><code>extended</code>: 是否返回扩展气象字段（体感温度、能见度、气压、紫外线指数、空气质量、降水量、云量）</strong>: </summary>

> | 属性  | 注释 |
> | :------------: |:------------: |
> | **`double`** feels_like | 体感温度 |
> | **`int`** visibility | 能见度 km |
> | **`int`** pressure | 气压 hPa |
> | **`double`** uv | 紫外线指数 |
> | **`int`** aqi | 空气质量指数 |
> | **`int`** precipitation | 降水量 mm |
> | **`int`** cloud | 云量 % |
> 
</details>

<details><summary><strong><code>indices</code>: 是否返回生活指数（穿衣、紫外线、洗车、晾晒、空调、感冒、运动、舒适度）</strong></summary>

> [!INFO]
> 提示: 位于 `Life_Indices` 的所有属性均为 `<IndicesLevel>`类, 请参考README的Json反序列化步骤
> | 属性 | 注释 |
> | :--: | :--:|
> | clothing | 穿衣指数
> | uv | 紫外线指数 |
> | car_wash | 洗车指数 |
> | drying | 晾晒指数 |
> | air_conditioner | 空调指数 |
> | cold_risk | 感冒指数 |
> | exercise | 运动指数 |
> | comfort | 舒适度指数 |

</details>

<details><summary><strong><code>forecast</code>: 是否返回预报数据（当日最高/最低气温及未来3天天气预报）</strong></summary>

> | 属性 | 注释 |
> | :---------------: | :-----------------------:|
> | **`List<Forcast>`** forcast | 未来三天的天气预报 |
> | date | 预告日期|
> | **`double`** temp_max | 最高气温 |
> | **`double`** temp_mix | 最低气温 |
> | weather_day | 白天天气 |
> | weather_night | 夜间天气 |
> | **`int`** humidity | 湿度 % |
> | **`int`** precip | 降水量 mm |
> | **`int`** visibility | 能见度 km |
> | **`double`** uv_index | 紫外线指数 0-11+ |

</details>

* **`$WeatherObject$`:** 实际的 **WeatherType** Jobject 对象
* **`$Life_Indices$`:** 实际的 **life_indices** 属性
* **`city`:** 指定的地区
* **`adcode`:** 高德地图的6位数字城市编码
* **返回类型:** `JObject`
* **返回值:** 天气信息
___
### 10.游戏娱乐

#### 获取 Steam 安装路径
```csharp
Rox.GameExpansionFeatures.Steam.GetSteamPath();
```
* **返回类型:** `string`
* **返回值:** Steam 安装文件夹



#### 获取CS2安装路径
```csharp
Rox.GameExpansionFeatures.CSGO.GetCS2Path();
```
* **返回类型:** `string`
* **返回值:** CS2的存放文件夹路径

#### Minecraft Java版 村庄英雄Buff加成的交易价格计算
```csharp
Rox.GameExpansionFeatures.Minecraft.TradingWithHeroOfVillage_Calculator(int BasePrice, int HearoOfVillage_Level);
```
* **BasePrice:** 基础价格
* **HearoOfVillage_Level:** "村庄英雄"效果等级, 范围 1-5
* **返回类型:** `int`
* **返回值:** 计算后的交易价格

#### 获取 Epic Games 免费游戏列表
``` csharp
await Rox.GameExpansionFeatures.EpicGames.GetFreeGames.GetDataJson()
```
* **返回类型:** `Json`
* **返回值:** 一个或多个免费游戏的详细列表

<details><summary><strong>附: EpicType属性</strong></summary>

> | 属性  | 注释 |
> | :------------: |:------------: |
> | id | Epic游戏的唯一标识符 |
> | title | 游戏的完整标题名称 |
> | cover | 封面图片的URL地址 |
> | **`int`** original_price |  游戏原价 单位 CNY¥ |
> | original_price_desc | 格式化后的原价描述字符串 |
> | description | 游戏的简介描述 |
> | seller | 发行商 |
> | **`bool`** is_free_now | 当前是否免费 |
> | free_start | 免费开始时间的可读字符串格式 |
> | free_end | 免费结束时间的可读字符串格式 |
> | link | 游戏在Epic Games商店的详情页链接 |
> | message | 错误信息 |

</details>

## 开发环境
[Visual Studio 2026](https://visualstudio.microsoft.com/zh-hans/vs)
- 系统要求
	- [Windows 11 版本 21H2 或更高版本：家庭版、专业版、专业教育版、专业工作站版、企业版和教育版](https://learn.microsoft.com/zh-cn/visualstudio/releases/2026/vs-system-requirements)
	- Windows 10 版本 1909 或更高版本：家庭版、专业版、教育版和企业版。
	- 64 位操作系统, 基于 x64 的处理器
- 工作负荷
	- 桌面应用和移动应用
		- .NET 桌面开发
- 编译语言
	- C# .NET Framework 4.7.2
- 依赖项
	- [视觉窗体库 AntdUI](https://www.nuget.org/packages/AntdUI)
	- [Json处理 Newtonsoft.Json ](https://www.nuget.org/packages/newtonsoft.json)
    	> (部分使用, 大部分使用项目内的 `Rox.Text.Json` 进行简单反/序列化)
- 扩展
	> 以下扩展均为 Visual Studio 2026 版本适用的扩展
    - [ClaudiaIDE **(视觉 更改文本编辑器的背景)**](https://marketplace.visualstudio.com/items?itemName=kbuchi.ClaudiaIDE)
    - [Hide Main Menu, Title Bar, and Tabs 2026 **(视觉 隐藏Tab栏,主菜单)**](https://marketplace.visualstudio.com/items?itemName=ChrisTorng.MinimalisticView)
    - [IntelliSense Extender 2022 **(代码辅助 IntelliSense增强版)**](https://marketplace.visualstudio.com/items?itemName=Dreamescaper.IntelliSenseExtender2022)
    - [IntelliSense汉语拼音拓展 **(代码辅助 支持汉语拼音拓展)**](https://marketplace.visualstudio.com/items?itemName=stratos.ChinesePinyinIntelliSenseExtender)
    - [Markdown Editor v2 **(编辑器 支持编辑和实时显示Markdown)**](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.MarkdownEditor2)
    - [One Dark Pro 2026 **(视觉 主题)**](https://marketplace.visualstudio.com/items?itemName=Bayaraa.OneDarkPro2026)