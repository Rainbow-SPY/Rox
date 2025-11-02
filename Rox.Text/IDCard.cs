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
            if (string.IsNullOrEmpty(idCard))
                return false;

            // 先通过正则初步验证格式
            if (!Regex.IsMatch(idCard, @"^\d{6}(19|20)\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])\d{3}[\dX]$"))
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
        public static bool IsAdult(string idCard)
        {
            string birthDateStr = GetBirthDate(idCard);
            if (birthDateStr == null)
                return false;
            DateTime birthDate = DateTime.ParseExact(birthDateStr, "yyyyMMdd", null);
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;
            return age >= 18;
        }
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
            int genderDigit = int.Parse(idCard.Substring(16, 1));
            return genderDigit % 2 == 1;
        }

    }
}
