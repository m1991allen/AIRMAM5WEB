using AIRMAM5.DBEntity.Models.Shared;
using System;

namespace AIRMAM5.DBEntity.Models.Works
{
    /// <summary>
    /// 取出上傳轉檔主檔資料 【spGET_L_WORK_BY_TRANSCODE】參數。 繼承參考 <see cref="SearchByDate"/>
    /// </summary>
    public class GetLWorkByTranscodeParam : SearchByDate
    {
        public GetLWorkByTranscodeParam() { }

        /// <summary>
        /// 指定區間天數。例: 5表示 區間: 前五日~後一日
        /// </summary>
        /// <param name="workid">指定轉檔工作編號 </param>
        /// <param name="days">指定區間的天數 </param>
        public GetLWorkByTranscodeParam(long workid, int days)
        {
            WorkId = workid;
            DateTime dt = DateTime.Now.AddDays(-days);
            StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            dt = DateTime.Now.AddDays(+1);
            EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
        }

        /// <summary>
        /// 【spGET_L_WORK_BY_TRANSCODE】參數: 指定 [fsWORK_ID]
        /// </summary>
        /// <param name="workid">指定轉檔工作編號 </param>
        public GetLWorkByTranscodeParam(long workid) { WorkId = workid; StartDate = string.Empty; EndDate = string.Empty; }

        /// <summary>
        /// 工作編號
        /// </summary>
        public long WorkId { get; set; }

        /// <summary>
        /// 工作/轉檔狀態 , fsSTATUS 是 CODE.WORK_TC 這個代碼
        /// </summary>
        public string WorkStatus { get; set; } = string.Empty;
    }

}
