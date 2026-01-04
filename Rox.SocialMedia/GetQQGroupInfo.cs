using System.Threading.Tasks;

namespace Rox.SocialMedia.QQ
{
    /// <summary>
    /// 获取QQ群公开摘要
    /// </summary>
    public class GetQQGroupInfo
    {
        /// <summary>
        ///  获取QQ群公开摘要, 此方法重定向自 <see cref="Rox.API.GetQQGroupData.GetQQGroupDataJson(string)"/>
        /// </summary>
        /// <param name="QGroupID">QQ群ID</param>
        /// <returns><see cref="API.GetQQGroupData.QGroupType"/> 对象</returns>
        public static Task<API.GetQQGroupData.QGroupType> GetQQGroupDataJson(string QGroupID) => API.GetQQGroupData.GetQQGroupDataJson(QGroupID);
    }
}
