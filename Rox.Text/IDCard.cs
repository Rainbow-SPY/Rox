using System;
using System.Text.RegularExpressions;

namespace Rox.Text
{
    /// <summary>
    /// 身份证信息类
    /// </summary>
    public class IDCard
    {
        /// <summary>
        /// 验证身份证号码是否合法
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns> 合法返回true，否则返回false </returns>
        public static bool IsValidIdCard(string idCard)
        {
            // 1. 空值/长度校验：必须是18位
            if (string.IsNullOrWhiteSpace(idCard) || idCard.Length != 18)
            {
                return false;
            }

            // 先通过正则初步验证格式
            if (!Regex.IsMatch(idCard, @"^\d{6}(18|19|20)\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])\d{3}[\dXx]$"))
                return false;

            // 进一步验证出生日期的合法性（正则无法完全校验日期有效性）
            try
            {
                // 提取出生日期部分（第7-14位）
                string birthDateStr = idCard.Substring(6, 8);
                DateTime birthDate = DateTime.ParseExact(birthDateStr, "yyyyMMdd", null);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 校验18位身份证的校验码（按国家标准算法）
        /// </summary>
        /// <param name="idCard">18位身份证号</param>
        /// <returns>校验码是否合法</returns>
        private static bool CheckIdCardCheckCode(string idCard)
        {
            // 权重数组
            int[] weight = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            // 校验码对应值
            char[] checkCode = { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };

            // 计算前17位的加权和
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                if (!int.TryParse(idCard[i].ToString(), out int num))
                {
                    return false;
                }
                sum += num * weight[i];
            }

            // 计算余数并匹配校验码
            int remainder = sum % 11;
            char actualCheckCode = char.ToUpper(idCard[17]); // 统一转为大写
            return actualCheckCode == checkCode[remainder];
        }
        /// <summary>
        /// 从身份证号码中提取出生日期
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns> 出生日期字符串，格式为yyyyMMdd；如果身份证号码不合法则返回null </returns>
        public static string GetBirthDate(string idCard)
        {
            if (!IsValidIdCard(idCard))
                return null;
            return idCard.Substring(6, 8); // 返回格式为yyyyMMdd
        }
        /// <summary>
        /// 计算身份证持有者的年龄
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public static int GetAge(string idCard)
        {
            string birthDateStr = GetBirthDate(idCard);
            if (birthDateStr == null)
                return -1;
            DateTime birthDate = DateTime.ParseExact(birthDateStr, "yyyyMMdd", null);
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;
            return age;
        }
        /// <summary>
        /// 判断身份证持有者是否为成年人（18岁及以上）
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns> 是成年人返回true，否则返回false </returns>
        public static bool IsAdult(string idCard) => GetAge(idCard) >= 18;
        /// <summary>
        /// 判断身份证持有者性别是否为男性
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns> 是男性返回true，否则返回false </returns>
        public static bool IsMan(string idCard)
        {
            if (!IsValidIdCard(idCard))
                return false;
            // 倒数第二位奇数为男性，偶数为女性
            return int.Parse(idCard.Substring(16, 1)) % 2 == 1;
        }

    }
}
