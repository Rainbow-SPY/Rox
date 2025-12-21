using System;
using System.Text.RegularExpressions;

namespace Rox.Runtimes
{
    /// <summary>
    /// 各种东西的互相转换
    /// </summary>
    public class Convert
    {

        private const long Base = 1024;
        private const long KbFactor = Base;
        private const long MbFactor = Base * KbFactor;
        private const long GbFactor = Base * MbFactor;
        private const long TbFactor = Base * GbFactor;

        /// <summary>
        /// 将带单位的字符串（如"2KB"）转换为字节数
        /// </summary>
        public static long ToBytes(string valueWithUnit)
        {
            if (string.IsNullOrWhiteSpace(valueWithUnit))
                throw new ArgumentNullException(nameof(valueWithUnit));

            var match = Regex.Match(valueWithUnit.Trim(), @"^(\d+(\.\d+)?)\s*([BKMGTP]B?)$", RegexOptions.IgnoreCase);
            if (!match.Success)
                throw new ArgumentException($"无效的格式: {valueWithUnit}，示例：2KB、3.5MB", nameof(valueWithUnit));

            double value = double.Parse(match.Groups[1].Value);

            switch (match.Groups[3].Value.ToUpperInvariant())
            {
                case "B":
                    return (long)Math.Round(value);
                case "KB":
                    return (long)Math.Round(value * KbFactor);
                case "MB":
                    return (long)Math.Round(value * MbFactor);
                case "GB":
                    return (long)Math.Round(value * GbFactor);
                case "TB":
                    return (long)Math.Round(value * TbFactor);
                default:
                    throw new ArgumentException($"不支持的单位: {match.Groups[3].Value.ToUpperInvariant()}", nameof(valueWithUnit));
            }
        }

        /// <summary>
        /// 将字节数转换为KB
        /// </summary>
        public static double ToKB(long bytes) => (double)bytes / KbFactor;

        /// <summary>
        /// 将字节数转换为MB
        /// </summary>
        public static double ToMB(long bytes) => (double)bytes / MbFactor;

        /// <summary>
        /// 将字节数转换为GB
        /// </summary>
        public static double ToGB(long bytes) => (double)bytes / GbFactor;

        /// <summary>
        /// 将字节数转换为TB
        /// </summary>
        public static double ToTB(long bytes) => (double)bytes / TbFactor;

        /// <summary>
        /// 根据字节数自动转换为最大合适单位（保留两位小数）
        /// </summary>
        public static string AutoConvert(long bytes)
        {
            if (bytes < 0)
                throw new ArgumentException("字节数不能为负数", nameof(bytes));

            // 替换原来的 switch 表达式，使用 if-else 语句
            if (bytes >= TbFactor)
                return $"{Math.Round(ToTB(bytes), 2)} TB";
            else if (bytes >= GbFactor)
                return $"{Math.Round(ToGB(bytes), 2)} GB";
            else if (bytes >= MbFactor)
                return $"{Math.Round(ToMB(bytes), 2)} MB";
            else if (bytes >= KbFactor)
                return $"{Math.Round(ToKB(bytes), 2)} KB";
            else
                return $"{bytes} B";
        }

        /// <summary>
        /// 将带单位的字符串转换为目标单位（如"2KB"转换为"MB"）
        /// </summary>
        public static double ConvertToUnit(string valueWithUnit, string targetUnit)
        {
            long bytes = ToBytes(valueWithUnit);
            switch (targetUnit.ToUpperInvariant())
            {
                case "B":
                    return bytes;
                case "KB":
                    return ToKB(bytes);
                case "MB":
                    return ToMB(bytes);
                case "GB":
                    return ToGB(bytes);
                case "TB":
                    return ToTB(bytes);
                default:
                    throw new ArgumentException($"不支持的目标单位: {targetUnit}", nameof(targetUnit));
            }
        }
    }
}
