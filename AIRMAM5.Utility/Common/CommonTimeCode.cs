using System;

namespace AIRMAM5.Utility.Common
{
    public class CommonTimeCode
    {
        #region =====【影格/幀速率 說明】====
        /*
         * 技術目的：利用丟時間碼來減少彩色副載波一半掃描頻率，進而減少彩色副載波對黑白視訊所產生的干擾，讓同一視訊中能兼容黑白與彩色視訊載波的一種妥協方法
         *          稱為:Drop Frame Timecode (丟棄影格時間碼) 
         *          ※但是並沒有視訊影格真正被丟棄，而是採用（跳過時間標籤）的方式來計算。
         * 定義　　：NTSC電視系統：1秒鐘＝30格 (格為剪輯單位)，1格＝2圖場。
         * 
         * Non-Drop-Frame-Timecode  ：30 fps/s 。
         * Drop-Frame-Timecode      ：29.97 fps/s (因為電視掃瞄線有水平遮沒期與垂直遮沒期之時間差之故)
         * 
         * 如果要減少彩色副載波對黑白視訊所產生的干擾，每小時的影片檔案需要丟棄/跳過的影格為: 60分鐘 *60秒 * (30-29.97)=108格 
         * 就是每10分鐘要跳過: 108格/6 =18格, 18格/30(fps/s)=0.6秒；每60分鐘要跳過: 0.6秒*6= 3.6秒。
         * (代表NTSC 525系統, 每60分鐘要跳過108(fps)/30(fps/s)=3.6秒)
         * 
         * ※ Drop Frame Timecode 的計算定義：
         *  1. 在前9分鐘內，每一分鐘都要跳過最前面的兩個影格。
         *      例如：【20：12：59；29】→【20：13：00；02】。
         *  2. 分鐘數可以被十整除，即保留第0和1影格的時間碼。 
         *      （例如：10分鐘、20分鐘、30分鐘、40分鐘、50分鐘、...）=【20：09：59；29】→【20：10：00；00】→【20：10：00；01】。→【20：10：00；02】。 
         * */
        #endregion

        /// <summary>
        /// 每秒影片禎數/影格 29.97
        /// </summary>
        public static readonly double VideoFrames = 29.97;
        /// <summary>
        /// 每小時影片幀數= 60*60*29.97= 107892
        /// </summary>
        static readonly double FramesAtHour = 60 * 60 * VideoFrames;
        /// <summary>
        /// 每分鐘影片幀數= 60*29.97= 1798.2
        /// </summary>
        static readonly double FramesAtMin = 60 * VideoFrames;

        /// <summary>
        /// 轉成時間格式 00:00:00;000 
        /// </summary>
        /// <param name="fnFRAMES">影片禎數 </param>
        /// <returns></returns>
        public static string FarmeToTimeCode(double fnFRAMES)
        {
            double HH = 0, MM = 0, SS = 0, FF = 0, itmp = 0;

            //frames = frames - 1;
            HH = Math.Floor(fnFRAMES / FramesAtHour);
            itmp = fnFRAMES % FramesAtHour;

            //2011/05/05 by Mike
            MM = Math.Floor(itmp / 17982) * 10;
            itmp = itmp % 17982;
            if (itmp > 2)
            {
                MM = MM + (Math.Floor((itmp - 2) / 1798));
                itmp = (itmp - 2) % 1798;
                itmp = itmp + 2;
            }
            SS = Math.Floor(itmp / 30);
            FF = Math.Floor(itmp % 30);

            //return ((HH < 10 ? "0" + HH.ToString() : HH.ToString()) + ":" + (MM < 10 ? "0" + MM.ToString() : MM.ToString()) + ":" + (SS < 10 ? "0" + SS.ToString() : SS.ToString()) + ";" + (FF < 10 ? "0" + FF.ToString() : FF.ToString()));
            return string.Format($"{Convert2Digits(HH)}:{Convert2Digits(MM)}:{Convert2Digits(SS)};{Convert2Digits(FF)}");
        }

        /// <summary>
        /// 秒數轉換成時間格式 00:00:00;000
        /// </summary>
        /// <param name="seconds">秒數 (若為毫秒,請先處理 ex: 秒數/1000 </param>
        public static string CurrentTimesToTimeCode(double seconds)
        {
            double HH = 0, MM = 0, SS = 0, FF = 0;
            double _frames = seconds * VideoFrames; //指定秒數的總幀數(Total fps)

            double itmp = _frames % FramesAtHour;   //每小時取完後,未滿一小時的有多少影格(fps)
            HH = Math.Floor(_frames / FramesAtHour);

            MM = Math.Floor(itmp / 17982) * 10;  //未滿一小時的影格,每10分鐘為單位取值, 再*10 (為何*10)
            //TIP: 每分鐘影格(fps/@Minute)= 60*29.97= 1,798.2. 所以17982=10分鐘總影格數

            itmp = itmp % 17982;    //未滿一小時的影格,每10分鐘為單位計算後,剩餘影格數 =>未滿10分鐘的影格數=>要跳過0和1影格(下方有扣2)
            if (itmp > 2)
            {
                MM = MM + (Math.Floor((itmp - 2) / 1798));
                itmp = (itmp - 2) % 1798;
                itmp = itmp + 2;
            }
            SS = Math.Floor(itmp / 30);
            FF = Math.Floor(itmp % 30);

            return string.Format($"{Convert2Digits(HH)}:{Convert2Digits(MM)}:{Convert2Digits(SS)};{Convert2Digits(FF)}");
        }

        static string Convert2Digits(double val)
        {
            return val < 10 ? "0" + val.ToString() : val.ToString();
        }
    }
}
