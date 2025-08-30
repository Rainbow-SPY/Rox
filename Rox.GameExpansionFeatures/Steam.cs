using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rox.GameExpansionFeatures
{
    public partial class Steam
    {
        /// <summary>
        /// 通过注册表 <see cref="Registry"/> 获取Steam的安装路径。
        /// </summary>
        /// <returns> Steam的安装路径，如果未找到则返回null。</returns>
        public static string GetSteamPath()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
            {
                if (key != null)
                {
                    object value = key.GetValue("SteamPath");
                    if (value != null)
                    {
                        return value.ToString();
                    }
                    return null;
                }
                return null;
            }
        }
    }
}
