using AIRMAM5.DBEntity.Models.Works;
using System;

namespace AIRMAM5.DBEntity.Models.Booking
{
    /// <summary>
    /// 取出調用轉檔資料 【spGET_L_WORK_BY_BOOKING】參數。 繼承參考 <see cref="GetLWorkByTranscodeParam"/>
    /// </summary>
    public class GetLWorkByBookingParam : GetLWorkByTranscodeParam
    {
        public GetLWorkByBookingParam() { }

        /// <summary>
        /// 僅指定 工作轉檔編號[fsWORK_ID]
        /// </summary>
        /// <param name="workid">轉檔工作id </param>
        /// <param name="loginid">使用者帳號 </param>
        public GetLWorkByBookingParam(long workid, string loginid)
        {
            this.WorkId = workid;
            this.LoginId = loginid;
            StartDate = string.Empty;
            EndDate = string.Empty;
            WorkStatus = string.Empty;
        }

        /// <summary>
        /// 指定 【spGET_L_WORK_BY_BOOKING】參數值
        /// </summary>
        /// <param name="days">指定區間天數。例: 5表示 區間: 前五日~後一日 </param>
        /// <param name="workid">工作轉檔編號[fsWORK_ID] </param>
        /// <param name="loginid">使用者帳號 </param>
        /// <param name="status">工作轉檔狀態[fsSTATUS] </param>
        public GetLWorkByBookingParam(int days, long workid, string loginid, string status)
        {
            //預存程序 日期查詢 格式:yyyy/MM/dd
            DateTime dt = DateTime.Now.AddDays(-days);
            StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            dt = DateTime.Now.AddDays(+1);
            EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            //
            this.WorkId = workid;
            this.LoginId = loginid;
            this.WorkStatus = status;
        }

        /// <summary>
        /// 使用者帳號
        /// </summary>
        public string LoginId { get; set; } = string.Empty;
    }

}
