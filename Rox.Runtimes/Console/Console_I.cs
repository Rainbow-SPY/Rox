using System;
using static Rox.Runtimes.LogLibraries;
using static Rox.Runtimes.LocalizedString;

namespace Rox.Runtimes
{
    /// <summary>
    /// 控制台动态输出数字类
    /// </summary>
    public class Console_I
    {
        // 记录上一次输出的数字长度，用于清空残留位数
        private static int _lastNumberLength = 0;

        /// <summary>
        /// 动态覆盖输出数字（固定前缀+动态数字+固定后缀）
        /// </summary>
        /// <param name="dynamicNumber">要显示的动态数字</param>
        /// <param name="prefixLength">前缀文本的长度（光标起始列位置）</param>
        /// <param name="suffixText">后缀固定文本（如"个文件"）</param>
        /// <param name="fixedLine">固定输出的行号（-1表示当前行）</param>
        public static void DynamicNumberOutput(
            int dynamicNumber,
            int prefixLength,
            string suffixText,
            int fixedLine = -1)
        {
            try
            {
                // 参数校验
                if (prefixLength < 0) throw new ArgumentOutOfRangeException(nameof(prefixLength), "前缀长度不能小于0");
                suffixText = suffixText ?? string.Empty;

                // 转换数字为字符串，获取当前数字长度
                string numberStr = dynamicNumber.ToString();
                int currentNumberLength = numberStr.Length;

                // 确定输出行：固定行或当前光标行
                int outputLine = fixedLine >= 0 ? fixedLine : Console.CursorTop;
                // 光标定位到前缀后的位置（动态数字的起始列）
                Console.SetCursorPosition(prefixLength, outputLine);

                // 输出动态数字 + 后缀文本
                Console.Write(numberStr + suffixText);

                // 处理数字长度变化的残留（如从100→99，需要清空第3位的0）
                if (currentNumberLength < _lastNumberLength)
                {
                    // 计算需要清空的残留位数，用空格填充
                    int residualLength = _lastNumberLength - currentNumberLength;
                    Console.Write(new string(' ', residualLength));
                    // 填充空格后，将光标回退到后缀文本开头（可选，不影响视觉）
                    Console.SetCursorPosition(prefixLength + currentNumberLength, outputLine);
                }

                // 更新上一次的数字长度
                _lastNumberLength = currentNumberLength;

                // 强制刷新输出缓冲区
                Console.Out.Flush();
            }
            catch (Exception ex)
            {
                // 异常输出到新行，避免覆盖原有内容
                WriteLog.Error(_Exception_With_xKind($"\n[动态输出异常]", ex));
            }
        }
    }
}
