using System.Threading.Tasks;

namespace Rox.SocialMedia.QQ
{
    /// <summary>
    /// 获取QQ用户公开摘要
    /// </summary>
    public class GetQQUserData
    {
        /// <summary>
        /// 获取QQ用户公开摘要, 此方法重定向自 <see cref="Rox.API.GetQQUserData.GetQQUserDataJson(string)"/>
        /// </summary>
        /// <param name="QQ">QQ号</param>
        /// <returns> <see cref="Rox.API.GetQQUserData.QQType"/> 对象 </returns>
        public static Task<API.GetQQUserData.QQType> GetQQUserDataJson(string QQ) => API.GetQQUserData.GetQQUserDataJson(QQ);
    }
}
