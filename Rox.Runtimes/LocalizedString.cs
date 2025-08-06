using System;
using System.Net.Http;
using System.Text.RegularExpressions;
namespace Rox
{
    namespace Runtimes
    {
        /// <summary>
        /// 本地化字符串类
        /// </summary>
        public class LocalizedString
        {
            #region 本地化字符串
            /// <summary>
            /// 语言
            /// </summary>
            public static string Language = GetLocalizedString("Language");
            /// <summary>
            /// 版本
            /// </summary>
            public static readonly string Version = GetLocalizedString("Version");
            /// <summary>
            /// 作者
            /// </summary>
            public static readonly string Author = GetLocalizedString("Author");
            /// <summary>
            /// 版权所有
            /// </summary>
            public static readonly string Copyright = GetLocalizedString("Copyright");
            /// <summary>
            /// 产品
            /// </summary>
            public static readonly string Product = GetLocalizedString("Product");
            /// <summary>
            /// 文件存在
            /// </summary>
            public static readonly string _FILE_EXIST = GetLocalizedString("_FILE_EXIST");
            /// <summary>
            /// 写入文件
            /// </summary>
            public static readonly string _FILE_WRITING = GetLocalizedString("_FILE_WRITING");
            /// <summary>
            /// 文件已保存到
            /// </summary>
            public static readonly string _FILE_EXIST_PATH = GetLocalizedString("_FILE_EXIST_PATH");
            /// <summary>
            /// 资源文件未找到
            /// </summary>
            public static readonly string _RES_FILE_NOT_FIND = GetLocalizedString("_RES_FILE_NOT_FIND");
            /// <summary>
            /// 未能写入日志文件
            /// </summary>
            public static readonly string _CANNOT_WRITE_LOGFILE = GetLocalizedString("_CANNOT_WRITE_LOGFILE");
            /// <summary>
            /// 清空日志
            /// </summary>
            public static readonly string _CLEAR_LOGFILE = GetLocalizedString("_CLEAR_LOGFILE");
            /// <summary>
            /// 未能清空日志文件
            /// </summary>
            public static readonly string _CANNOT_CLEAR_LOGFILE = GetLocalizedString("_CANNOT_CLEAR_LOGFILE");
            /// <summary>
            /// 获取输出目录
            /// </summary>
            public static readonly string _GET_OUTPUT_DIRECTORY = GetLocalizedString("_GET_OUTPUT_DIRECTORY");
            /// <summary>
            /// 获取输出名称
            /// </summary>
            public static readonly string _GET_OUTPUT_NAME = GetLocalizedString("_GET_OUTPUT_NAME");
            /// <summary>
            /// 创建目录
            /// </summary>
            public static readonly string _CREATE_DIRECTORY = GetLocalizedString("_CREATE_DIRECTORY");
            /// <summary>
            /// 获取目录
            /// </summary>
            public static readonly string _GET_DIRECTORY = GetLocalizedString("_GET_DIRECTORY");
            /// <summary>
            /// 网络不可用
            /// </summary>
            public static readonly string _NOTAVAILABLE_NETWORK = GetLocalizedString("_NOTAVAILABLE_NETWORK");
            /// <summary>
            /// 网络不可用, 是否执行步骤?
            /// </summary>
            public static readonly string _NOTAVAILABLE_NETWORK_TIPS = GetLocalizedString("_NOTAVAILABLE_NETWORK_TIPS");
            /// <summary>
            /// 提示
            /// </summary>
            public static readonly string _TIPS = GetLocalizedString("_TIPS");
            /// <summary>
            /// 错误
            /// </summary>
            public static readonly string _ERROR = GetLocalizedString("_ERROR");
            /// <summary>
            /// 警告
            /// </summary>
            public static readonly string _WARNING = GetLocalizedString("_WARNING");
            /// <summary>
            /// 获取 URL
            /// </summary>
            public static readonly string _GET_URL = GetLocalizedString("_GET_URL");
            /// <summary>
            /// 获取临时目录
            /// </summary>
            public static readonly string _GET_TEMP = GetLocalizedString("_GET_TEMP");
            /// <summary>
            /// 正则表达式获取文件
            /// </summary>
            public static readonly string _REGEX_GET_FILE = GetLocalizedString("_REGEX_GET_FILE");
            /// <summary>
            /// 获取目录内的文件
            /// </summary>
            public static readonly string _GET_FILES_IN_DIRECTORY = GetLocalizedString("_GET_FILES_IN_DIRECTORY");
            /// <summary>
            /// 获取系统位数
            /// </summary>
            public static readonly string _GET_SYSTEM_BIT = GetLocalizedString("_GET_SYSTEM_BIT");
            /// <summary>
            /// 获取网页文件下载链接
            /// </summary>
            public static readonly string _FINDING_HTML_DOWNLOAD_LINK = GetLocalizedString("_FINDING_HTML_DOWNLOAD_LINK");
            /// <summary>
            /// 获取 HTML 代码
            /// </summary>
            public static readonly string _FIND_HTML_CODE = GetLocalizedString("_FIND_HTML_CODE");
            /// <summary>
            /// 安全软件正在运行
            /// </summary>
            public static readonly string _SECURITY_RUNNING = GetLocalizedString("_SECURITY_RUNNING");
            /// <summary>
            /// 进程已启动
            /// </summary>
            public static readonly string _PROCESS_STARTED = GetLocalizedString("_PROCESS_STARTED");
            /// <summary>
            /// 进程已退出
            /// </summary>
            public static readonly string _PROCESS_EXITED = GetLocalizedString("_PROCESS_EXITED");
            /// <summary>
            /// 未能禁用休眠
            /// </summary>
            public static readonly string _CANNOT_DISENABLE_HIBERNATE = GetLocalizedString("_CANNOT_DISENABLE_HIBERNATE");
            /// <summary>
            /// 已禁用休眠
            /// </summary>
            public static readonly string _DISENABLE_HIBERNATE = GetLocalizedString("_DISENABLE_HIBERNATE");
            /// <summary>
            /// 未能启用卓越性能模式
            /// </summary>
            public static readonly string _CANNOT_ENABLE_HIGHPOWERCFG = GetLocalizedString("_CANNOT_ENABLE_HIGHPOWERCFG");
            /// <summary>
            /// 启用卓越性能模式
            /// </summary>
            public static readonly string _ENABLE_HIGHPOWERCFG = GetLocalizedString("_ENABLE_HIGHPOWERCFG");
            /// <summary>
            /// 未能禁用安全中心
            /// </summary>
            public static readonly string _CANNOT_DISABLE_SECURITY_CENTER = GetLocalizedString("_CANNOT_DISABLE_SECURITY_CENTER");
            /// <summary>
            /// 未能启用安全中心
            /// </summary>
            public static readonly string _CANNOT_ENABLE_SECURITY_CENTER = GetLocalizedString("_CANNOT_ENABLE_SECURITY_CENTER");
            /// <summary>
            /// 已禁用安全中心
            /// </summary>
            public static readonly string _DISABLE_SECURITY_CENTER = GetLocalizedString("_DISABLE_SECURITY_CENTER");
            /// <summary>
            /// 已启用安全中心
            /// </summary>
            public static readonly string _ENABLE_SECURITY_CENTER = GetLocalizedString("_ENABLE_SECURITY_CENTER");
            /// <summary>
            /// 写入注册表
            /// </summary>
            public static readonly string _WRITE_REGISTRY = GetLocalizedString("_WRITE_REGISTRY");
            /// <summary>
            /// 未能禁用 Windows 更新
            /// </summary>
            public static readonly string _CANNOT_DISABLE_WINDOWS_UPDATER = GetLocalizedString("_CANNOT_DISABLE_WINDOWS_UPDATER");
            /// <summary>
            /// 未能启用 Windows 更新
            /// </summary>
            public static readonly string _CANNOT_ENABLE_WINDOWS_UPDATER = GetLocalizedString("_CANNOT_ENABLE_WINDOWS_UPDATER");
            /// <summary>
            /// 已禁用 Windows 更新
            /// </summary>
            public static readonly string _DISABLE_WINDOWS_UPDATER = GetLocalizedString("_DISABLE_WINDOWS_UPDATER");
            /// <summary>
            /// 已启用 Windows 更新
            /// </summary>
            public static readonly string _ENABLE_WINDOWS_UPDATER = GetLocalizedString("_ENABLE_WINDOWS_UPDATER");
            /// <summary>
            /// Windows 已激活
            /// </summary>
            public static readonly string _ACTIVE_WINDOWS = GetLocalizedString("_ACTIVE_WINDOWS");
            /// <summary>
            /// 未能激活 Windows
            /// </summary>
            public static readonly string _CANNOT_ACTIVE_WINDOWS = GetLocalizedString("_CANNOT_ACTIVE_WINDOWS");
            /// <summary>
            /// 成功写入注册表
            /// </summary>
            public static readonly string _SUCESS_WRITE_REGISTRY = GetLocalizedString("_SUCESS_WRITE_REGISTRY");
            /// <summary>
            /// 未能写入注册表
            /// </summary>
            public static readonly string _WRITE_REGISTRY_FAILED = GetLocalizedString("_WRITE_REGISTRY_FAILED");
            /// <summary>
            /// 获取 Aria2c 参数
            /// </summary>
            public static readonly string _GET_ARIA2C_ARGS = GetLocalizedString("_GET_ARIA2C_ARGS");
            /// <summary>
            /// 获取 Aria2c 路径
            /// </summary>
            public static readonly string _GET_ARIA2C_PATH = GetLocalizedString("_GET_ARIA2C_PATH");
            /// <summary>
            /// 获取 Aria2c 退出代码
            /// </summary>
            public static readonly string _GET_ARIA2C_EXITCODE = GetLocalizedString("_GET_ARIA2C_EXITCODE");
            /// <summary>
            /// 已启用 Aria2c 日志输出
            /// </summary>
            public static readonly string _ENABLE_ARIA2C_LOG_OUTPUT = GetLocalizedString("_ENABLE_ARIA2C_LOG_OUTPUT");
            /// <summary>
            /// 已禁用 Aria2c 日志输出
            /// </summary>
            public static readonly string _DISABLE_ARIA2C_LOG_OUTPUT = GetLocalizedString("_DISABLE_ARIA2C_LOG_OUTPUT");
            /// <summary>
            /// 正在下载文件
            /// </summary>
            public static readonly string _DOWNLOADING_FILE = GetLocalizedString("_DOWNLOADING_FILE");
            /// <summary>
            /// 下载完成
            /// </summary>
            public static readonly string _DOWNLOADING_COMPLETE = GetLocalizedString("_DOWNLOADING_COMPLETE");
            /// <summary>
            /// 下载失败
            /// </summary>
            public static readonly string _DOWNLOADING_FAILED = GetLocalizedString("_DOWNLOADING_FAILED");
            /// <summary>
            /// 获取 64 位下载链接
            /// </summary>
            public static readonly string _GET_64_LINK = GetLocalizedString("_GET_64_LINK");
            /// <summary>
            /// 获取 32 位下载链接
            /// </summary>
            public static readonly string _GET_32_LINK = GetLocalizedString("_GET_32_LINK");
            /// <summary>
            /// 获取 ResourceManager 名称
            /// </summary>
            public static readonly string _GET_RM_NAME = GetLocalizedString("_GET_RM_NAME");
            /// <summary>
            /// 获取 ResourceManager 对象
            /// </summary>
            public static readonly string _GET_RM_OBJ = GetLocalizedString("_GET_RM_OBJ");
            /// <summary>
            /// 创建新的 ResourceManager 实例
            /// </summary>
            public static readonly string _NEW_RM = GetLocalizedString("_NEW_RM");
            /// <summary>
            /// 获取 HTML 页面文件
            /// </summary>
            public static readonly string _GET_HTML = GetLocalizedString("_GET_HTML");
            /// <summary>
            /// 32 位
            /// </summary>
            public static readonly string _32 = GetLocalizedString("_32");
            /// <summary>
            /// 64 位
            /// </summary>
            public static readonly string _64 = GetLocalizedString("_64");
            /// <summary>
            /// 获取文件
            /// </summary>
            public static readonly string _GET_FILE = GetLocalizedString("_GET_FILE");
            /// <summary>
            /// 等待下载
            /// </summary>
            public static readonly string _WAIT_DOWNLOADING = GetLocalizedString("_WAIT_DOWNLOADING");
            /// <summary>
            /// 重试下载
            /// </summary>
            public static readonly string _RETRY_DOWNLOAD = GetLocalizedString("_RETRY_DOWNLOAD");
            /// <summary>
            /// 错误代码
            /// </summary>
            public static readonly string _ERROR_CODE = GetLocalizedString("_ERROR_CODE");
            /// <summary>
            /// 登录失败: 用户名未知或密码错误
            /// </summary>
            public static readonly string _LOGIN_ERROR_USER_OR_PASSWORD = GetLocalizedString("_LOGIN_ERROR_USER_OR_PASSWORD");
            /// <summary>
            /// 请验证您的身份
            /// </summary>
            public static readonly string _LOGIN_VERIFY = GetLocalizedString("_LOGIN_VERIFY");
            /// <summary>
            /// 用户取消了操作
            /// </summary>
            public static readonly string _CANCEL_OP = GetLocalizedString("_CANCEL_OP");
            /// <summary>
            /// 未知错误
            /// </summary>
            public static readonly string _UNKNOW_ERROR = GetLocalizedString("_UNKNOW_ERROR");
            /// <summary>
            /// 接受到响应
            /// </summary>
            public static readonly string _GET_RESPONSE = GetLocalizedString("_GET_RESPONSE");
            /// <summary>
            /// 回答
            /// </summary>
            public static readonly string _ANSWER = GetLocalizedString("_ANSWER");
            /// <summary>
            /// 发送请求
            /// </summary>
            public static readonly string _SEND_REQUEST = GetLocalizedString("_SEND_REQUEST");
            /// <summary>
            /// 验证失败
            /// </summary>
            public static readonly string _LOGIN_VERIFY_ERROR = GetLocalizedString("_LOGIN_VERIFY_ERROR");
            /// <summary>
            /// 请输入您的凭据
            /// </summary>
            public static readonly string _ENTER_CREDENTIALS = GetLocalizedString("_ENTER_CREDENTIALS");
            /// <summary>
            /// 验证成功
            /// </summary>
            public static readonly string _SUCCESS_VERIFY = GetLocalizedString("_SUCCESS_VERITY");
            /// <summary>
            /// 最新版本
            /// </summary>
            public static readonly string _LATEST_VERSION = GetLocalizedString("_LATEST_VERSION");
            /// <summary>
            /// 不支持的平台
            /// </summary>
            public static readonly string _UNSUPPORT_PLATFORM = GetLocalizedString("_UNSUPPORT_PLATFORM");
            /// <summary>
            /// 解析 JSON 失败
            /// </summary>
            public static readonly string _JSON_PARSING_FAILED = GetLocalizedString("_JSON_PARSING_FAILED");
            /// <summary>
            /// 新版本可用
            /// </summary>
            public static readonly string _NEW_VERSION_AVAILABLE = GetLocalizedString("_NEW_VERSION_AVAILABLE");
            /// <summary>
            /// 当前版本为最新版本
            /// </summary>
            public static readonly string _NON_NEW_VER = GetLocalizedString("_NON_NEW_VER");
            /// <summary>
            /// 当前版本
            /// </summary>
            public static readonly string _CURRENT_VER = GetLocalizedString("_CURRENT_VER");
            /// <summary>
            /// 添加新行
            /// </summary>
            public static readonly string _ADD_NEW_LINE = GetLocalizedString("_ADD_NEW_LINE");
            /// <summary>
            /// 更新行值
            /// </summary>
            public static readonly string _UPDATE_LINE = GetLocalizedString("_UPDATE_LINE");
            /// <summary>
            /// 读取文件
            /// </summary>
            public static readonly string _READ_FILE = GetLocalizedString("_READ_FILE");
            /// <summary>
            /// 写入文件
            /// </summary>
            public static readonly string _WRITE_FILE = GetLocalizedString("_WRITE_FILE");
            /// <summary>
            /// Windows 更新 已禁用
            /// </summary>
            public static readonly string _WINDOWS_UPDATER_DISABLED = GetLocalizedString("_WINDOWS_UPDATER_DISABLED");
            /// <summary>
            /// Windows 更新 已启用
            /// </summary>
            public static readonly string _WINDOWS_UPDATER_ENABLED = GetLocalizedString("_WINDOWS_UPDATER_ENABLED");
            /// <summary>
            /// 读取注册表失败
            /// </summary>
            public static readonly string _READ_REGISTRY_FAILED = GetLocalizedString("_READ_REGISTRY_FAILED");
            #endregion
            #region 错误代码
            /// <summary>
            /// 不符合 17 位 SteamID64 格式
            /// </summary>
            public static readonly string Not_Allow_17_SteamID64 = "Not_Allow_17_SteamID64 (6003)";
            /// <summary>
            /// 未找到 Steam 账户或完全私密了个人资料
            /// </summary>
            public static readonly string _Steam_Not_Found_Account = "_Steam_Not_Found_Account (6006)";
            /// <summary>
            /// 查询 Steam 账户信息时发生未知异常
            /// </summary>
            public static readonly string _Steam_Unknow_Exception = "_Steam_Unknow_Exception (6007)";
            /// <summary>
            /// 查询 Steam 账户信息时上游服务错误, 在向 Steam 的官方 API 请求数据时遇到了问题, 这可能是他们的服务暂时中断，请稍后重试。
            /// </summary>
            public static readonly string _Steam_Service_Error = "_Steam_Service_Error (6008)";
            /// <summary>
            /// 查询 Steam 账户信息时认证失败, 提供的 Steam Web API Key 无效或已过期，或者没有提供 Key
            /// </summary>
            public static readonly string _Steam_Server_UnAuthenticated = "_Steam_Server_UnAuthenticated (6009)";
            /// <summary>
            /// 处理 Json 时发生未知异常
            /// </summary>
            public static readonly string _Json_Unknow_Exception = "_Json_Unknow_Exception (6001)";
            /// <summary>
            /// 解析 SteamType 对象时发生错误 或 无法解析 SteamID64
            /// </summary>
            public static readonly string _Json_Parse_SteamID64 = "_Json_Parse_SteamID64 (6002)";
            /// <summary>
            /// 指定的 Json 数据在反序列化过程中出现未知异常
            /// </summary>
            public static readonly string _Json_DeObject_Unknow_Exception = "_Json_DeObject_Unknow_Exception (6201)";
            /// <summary>
            /// 指定的字符串为 <see cref="string.Empty"/> 或 <see langword="null"/>
            /// </summary>
            public static readonly string _String_NullOrEmpty = "_String_NullOrEmpty (1002)";
            /// <summary>
            /// 无效的字符串输入, 通常由API返回响应时回复错误
            /// </summary>
            public static readonly string Invaid_String_Input = "Invaid_String_Input (1001)";
            /// <summary>
            /// 处理 正则表达式 时发生未知异常
            /// </summary>
            public static readonly string _Regex_Match_Unknow_Exception = "_Regex_Match_Unknow_Exception (4002)";
            /// <summary>
            /// 指定的 正则表达式 <see cref="Regex.Match(string)"/> 未匹配出结果而导致输出字符串为 <see cref="string.Empty"/> 或 <see langword="null"/>
            /// </summary>
            public static readonly string _Regex_Match_Not_Found_Any = "_Regex_Match_Not_Found_Any (4001)";
            /// <summary>
            /// 使用 <see cref="HttpClient.GetAsync(string)"/> 发送请求时出现错误
            /// </summary>
            public static readonly string _HttpClient_Request_Failed = "_HttpClient_Request_Failed (1301)";
            /// <summary>
            /// 检测到非法/不安全的请求, 服务器访问已拒绝
            /// </summary>
            public static readonly string _HttpClient_Request_UnsafeOrIllegal_Denied = "_HttpClient_Request_UnsafeOrIllegal_Denied (1302)";
            /// <summary>
            /// 请求的天气名称不存在或未找到
            /// </summary>
            public static readonly string _Weather_City_Not_Found = "_Weather_City_Not_Found (1201)";
            /// <summary>
            /// 查询天气时发生未知异常
            /// </summary>
            public static readonly string _Weather_Unknow_Exception = "_Weather_Unknow_Exception (1202)";
            /// <summary>
            /// 上游服务错误, 天气服务提供商的API暂时不可用或返回了错误。
            /// </summary>
            public static readonly string _Weather_Service_Error = "_Weather_Service_Error (1203)";
            #endregion

            /// <param name="_void"> 方法名 </param>
            /// <param name="_type"> 类型名 </param>
            /// <returns> 方法 <see langword="%s"/> 传入的 <see langword="%s"/> 类型的值为 <see langword="null"/> </returns>
            public static string _void_value_null(string _void, string _type) => $"方法 {_void} 传入的 {_type} 类型的值为 null";
            /// <param name="value"> 方法名 </param>
            /// <returns>传递的参数 <see langword="%s"/> 不能为 <see langword="null"/> 或 <see langword="Empty"/> </returns>
            public static string _value_Not_Is_NullOrEmpty(string value) => $"传递的参数 {value} 不能为 null 或空白。";
            /// <param name="value"> 输入的值 </param>
            /// <param name="type"> 类型 </param>
            /// <returns> 输入的值 <see langword="%s"/> 不是 <see langword="%s"/> 类型 </returns>
            public static string _input_value_Not_Is_xType(string value, string type) => $"输入的值 {value} 不是 {type} 类型";
            /// <param name="kind1"> 转换前的类型 </param>
            /// <param name="kind2"> 转换后的类型 </param>
            /// <returns> 转换 <see langword="%s"/> 到 <see langword="%s"/> 时发生错误 </returns>
            public static string _Convert_Kind_To_Kind(string kind1, string kind2) => $"转换 {kind1} 到 {kind2} 时发生错误";
            /// <param name="kind"> 返回值的类型 </param>
            /// <param name="value"> 返回值 </param>
            /// <returns> 返回 <see langword="%s"/> 值: <see langword="%s"/> </returns>
            public static string _Return_xKind_value(string kind, string value) => $"返回 {kind} 值: {value}";
            /// <param name="kind"> 异常的类型 </param>
            /// <returns> <see langword="%s"/> 遭遇未知的异常, 异常类型: <see cref="Exception.GetType"/> - <see cref="Exception.Message"/></returns>
            public static string _Exception_With_xKind(string kind) => _Exception_With_xKind(kind, new Exception("未知异常"));
            /// <param name="kind"> 异常的类型 </param>
            /// <param name="ex"> 异常对象 </param>
            /// <returns> <see langword="%s"/> 遭遇未知的异常, 异常类型: <see cref="Exception.GetType"/> - <see cref="Exception.Message"/> </returns>
            public static string _Exception_With_xKind(string kind, Exception ex) => $"{kind} 遭遇未知的异常, 异常类型: {ex.GetType().Name ?? "Unknow"} - {ex.Message}\n{ex.StackTrace ?? "Unknow"}";
            /// <summary>
            /// 获取本地化字符串
            /// </summary>
            /// <param name="key">字符串常量</param>
            /// <returns>指定语言文件中的字符串</returns>
            public static string GetLocalizedString(string key)
            {
                return ResourceHelper.GetString(key, System.Globalization.CultureInfo.InstalledUICulture.Name.ToString());
            }

        }
    }
}
