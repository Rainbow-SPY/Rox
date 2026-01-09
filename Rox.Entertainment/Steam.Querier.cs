using Rox.Text;
using System.Threading.Tasks;

namespace Rox.Entertainment
{
    public partial class Steam
    {
        /// <summary>
        /// Steam 相关数据查询器
        /// </summary>
        public class Querier
        {
            /// <summary>
            /// 新版请求Steam Web API Json的方法, 此方法重定向到 <see cref="SteamUserData_v1.GetDataJson_v1(string)"/> 方法
            /// </summary>
            /// <param name="AnySteamID">支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns><see cref="SteamUserData_v1.SteamType"/> 格式的 <see cref="Json"/> 文本</returns>
            public static async Task<SteamUserData_v1.SteamType> UserData_v1(string AnySteamID) => await SteamUserData_v1.GetDataJson_v1(AnySteamID);
            /// <summary>
            /// 请求Steam Web API Json的方法, 此方法重定向到 <see cref="SteamUserData.GetDataJson(string)"/> 方法
            /// </summary>
            /// <param name="AnySteamID">支持SteamID3,ID64,个人主页链接,自定义URL,好友代码</param>
            /// <returns><see cref="SteamUserData.SteamType"/> 格式的 <see cref="Json"/> 文本</returns>
            public static async Task<SteamUserData.SteamType> UserData(string AnySteamID) => await SteamUserData.GetDataJson(AnySteamID);
        }
    }
}
