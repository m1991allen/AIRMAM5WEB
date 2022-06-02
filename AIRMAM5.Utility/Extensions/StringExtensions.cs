using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRMAM5.Utility.Extensions
{
    public class StringExtensions
    {
        /// <summary>
        /// 過濾特殊字元
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceStr(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;

            str = str.Replace("~", "～");
            str = str.Replace("!", "！");
            str = str.Replace("@", "＠");
            str = str.Replace("#", "＃");
            str = str.Replace("$", "＄");
            str = str.Replace("%", "％");
            str = str.Replace("^", "");
            str = str.Replace("&", "＆");
            str = str.Replace("*", "");
            str = str.Replace("<", "");
            str = str.Replace(">", "");
            str = str.Replace(":", "");
            str = str.Replace("\"", "");
            str = str.Replace("/", "");
            str = str.Replace("\\", "");
            str = str.Replace("?", "？");
            str = str.Replace("|", "");
            str = str.Replace("?", "？");
            return str;
        }

        /// <summary>
        /// 產生隨機數字
        /// </summary>
        /// <param name="digitsNumber">所需的位數</param>
        /// <returns></returns>
        public static int GenerateRandomNumber(int digitsNumber)
        {
            Random random = new Random();
            double num = Math.Pow(10, digitsNumber);
            bool isInt = int.TryParse(num.ToString(), out int newNumber);
            var newNumberStr = random.Next(0, newNumber);
            newNumberStr = takeNDigits(newNumberStr, 1);

            if (newNumberStr >= 4)
                newNumberStr = random.Next(0, 4);
            return newNumberStr; //newNumberStr.ToString();
        }

        /// <summary>
        /// 取得數字到第幾位數
        /// </summary>
        /// <param name="number"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        private static int takeNDigits(int number, int N)
        {
            //為了處理負數
            number = Math.Abs(number);

            // 0的特殊情況，因為0的Log將是無窮大
            if (number == 0) return number;

            //獲取此輸入數字的位數
            int numberOfDigits = (int)Math.Floor(Math.Log10(number) + 1);

            //檢查輸入數字的位數是否超過所需的前N位數
            if (numberOfDigits >= N)
                return (int)Math.Truncate((number / Math.Pow(10, numberOfDigits - N)));
            else
                return number;
        }

        private static int rep = 0;
        /// <summary>
        /// 產生 隨機字母字串(數字字母混和)
        /// </summary>
        /// <param name="codeCount">產生的位數 </param>
        /// <returns></returns>
        public static string GenerateRandomStr(int codeCount)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }

    }
}
