using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rox.SocialMedia
{
    public class GetQQUserData
    {
        /// <summary>
        /// 获取QQ用户信息, 此方法重定向自 <see cref="Rox.API.GetQQUserData.GetQQUserDataJson(string)"/>
        /// </summary>
        /// <param name="QQ"></param>
        /// <returns> <see cref="Rox.API.GetQQUserData.QQType"/> 对象 </returns>
        public static Task<API.GetQQUserData.QQType> GetQQUserDataJson(string QQ)=> API.GetQQUserData.GetQQUserDataJson(QQ);
    }
}
