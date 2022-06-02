using AIRMAM5.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AIRMAM5.Utility.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 西元年轉民國年
        /// </summary>
        /// <param name="dt">指定日期</param>
        /// <returns></returns>
        public static string ToRocDate(DateTime dt)
        {
            return string.Format("{0}-{1}-{2} ", dt.Year - 1911, dt.Month, dt.Day);
        }

        /// <summary>
        /// 兩個日期相差值, 格式: 00D:00H:03m:21S
        /// </summary>
        /// <param name="dt1">較早的時間 </param>
        /// <param name="dt2">較晚的時間 </param>
        /// <returns></returns>
        public static string DiffDate(DateTime dt1, DateTime? dt2)
        {

            DateTime _dt2 = dt2 == null ? DateTime.Now : (DateTime)dt2;
            if (_dt2.Year == 1900) _dt2 = DateTime.Now;

            TimeSpan _diffTimeSpan = _dt2.Subtract(dt1);

            string diffTime = string.Format("{0:00}日:{1}時:{2}分:{3}秒", 
                _diffTimeSpan.Days, 
                _diffTimeSpan.Hours,
                _diffTimeSpan.Minutes,
                _diffTimeSpan.Seconds);

            return diffTime;
        }

        /// <summary>
        /// 兩個日期相差值, 回傳指定時間單位
        /// </summary>
        /// <param name="dt1">較早的時間 </param>
        /// <param name="dt2">較晚的時間 </param>
        /// <param name="timeUnit">指定回傳的時間單位, 預設:分鐘數 </param>
        /// <returns></returns>
        public static double DiffDateTo(DateTime dt1, DateTime? dt2, TimeUnitEnum timeUnit = TimeUnitEnum.Minutes)
        {
            DateTime _dt2 = dt2 == null ? DateTime.Now : (DateTime)dt2;
            if (_dt2.Year == 1900) _dt2 = DateTime.Now;

            TimeSpan _diffTimeSpan = _dt2.Subtract(dt1);

            double diffTimes = 0;
            switch (timeUnit)
            {
                case TimeUnitEnum.Milliseconds:
                    diffTimes = _diffTimeSpan.TotalMilliseconds;
                    break;

                case TimeUnitEnum.Seconds:
                    diffTimes = _diffTimeSpan.TotalSeconds;
                    break;

                case TimeUnitEnum.Hours:
                    diffTimes = _diffTimeSpan.TotalHours;
                    break;

                case TimeUnitEnum.Days:
                    diffTimes = _diffTimeSpan.TotalDays;
                    break;

                case TimeUnitEnum.Minutes:
                default:
                    diffTimes = _diffTimeSpan.TotalMinutes;
                    break;
            }

            return diffTimes;
        }

        /// <summary>
        /// 檢查日期字串格式 i.e. yyyy-MM-dd
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckDateIsMatch(string str, string cht="-")
        {
            // yyyy-MM-dd
            Regex rgx = new Regex(@"^[0-9]{4}-[0-9]{2}-[0-9]{2}$");

            string pattern = @"^[0-9]{4}" + cht + "[0-9]{2}" + cht + "[0-9]{2}$";
            rgx = new Regex(pattern);
            var r = rgx.IsMatch(str);

            return r;
        }

    }
}
