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
            if (string.IsNullOrWhiteSpace(idCard) || idCard.Length != 18)
                return false;

            return Regex.IsMatch(idCard, @"^\d{6}(18|19|20)\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])\d{3}[\dXx]$");
        }

        /// <summary>
        /// 校验18位身份证的校验码（按国家标准算法）
        /// </summary>
        /// <param name="idCard">18位身份证号</param>
        /// <returns>校验码是否合法</returns>
        public static bool CheckIdCardCheckCode(string idCard)
        {
            int[] weight = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };

            var sum = 0;
            for (var i = 0; i < 17; i++)
            {
                if (!int.TryParse(idCard[i].ToString(), out var num))
                    return false;
                sum += num * weight[i];
            }

            return char.ToUpper(idCard[17]) ==
                   new[] { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' }[sum % 11];
        }

        /// <summary>
        /// 从身份证号码中提取出生日期
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns> 出生日期字符串，格式为yyyyMMdd；如果身份证号码不合法则返回null </returns>
        public static string GetBirthDate(string idCard) => IsValidIdCard(idCard) ? idCard.Substring(6, 8) : null;

        /// <summary>
        /// 计算身份证持有者的年龄
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns>年龄</returns>
        public static int GetAge(string idCard)
        {
            if (GetBirthDate(idCard) == null)
                return -1;
            var birthDate = DateTime.ParseExact(GetBirthDate(idCard), "yyyyMMdd", null);
            var age = DateTime.Today.Year - birthDate.Year;
            if (birthDate > DateTime.Today.AddYears(-age))
                age--;
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
        public static bool IsMan(string idCard) => IsValidIdCard(idCard) && int.Parse(idCard.Substring(16, 1)) % 2 == 1;
    }
}