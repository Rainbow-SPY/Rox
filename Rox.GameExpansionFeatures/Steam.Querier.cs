using Rox.Text;
using System.Threading.Tasks;
using static Rox.API;

namespace Rox.GameExpansionFeatures
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
            /// <param name="SteamID64">SteamID64</param>
            /// <returns><see cref="SteamUserData_v1.SteamType"/> 格式的 <see cref="Json"/> 文本</returns>
            public static async Task<SteamUserData_v1.SteamType> UserData_v1(string SteamID64) => await SteamUserData_v1.GetDataJson_v1(SteamID64);
            /// <summary>
            /// 请求Steam Web API Json的方法, 此方法重定向到 <see cref="SteamUserData.GetDataJson(string)"/> 方法
            /// </summary>
            /// <param name="SteamID64">SteamID64</param>
            /// <returns><see cref="SteamUserData.SteamType"/> 格式的 <see cref="Json"/> 文本</returns>
            public static async Task<API.SteamUserData.SteamType> UserData(string SteamID64) => await SteamUserData.GetDataJson(SteamID64);
        }
    }
}
