using AIRMAM5.DBEntity.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.AcrDelete
{
    /// <summary>
    /// 媒資刪除紀錄 取出t_tbmARC_INDEX主檔資料 參數。繼承參考 <see cref="SearchByDate"/>
    /// </summary>
    public class SP_TGetARCIndexByCondition : SearchByDate
    {
        /// <summary>
        /// 初始 區間: 前3日~後一日
        /// </summary>
        public SP_TGetARCIndexByCondition()
        {
            //預存程序 日期查詢 格式:yyyy/MM/dd
            DateTime dt = DateTime.Now.AddDays(-3);
            StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            dt = DateTime.Now.AddDays(+1);
            EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
        }

        /// <summary>
        /// t_tbmARC_INDEX主檔資料 參數
        /// </summary>
        /// <param name="sdate">查詢起始日. 若為null值, 則為系統日-3日 </param>
        /// <param name="edate"></param>
        /// <param name="statue"></param>
        /// <param name="type"></param>
        /// <param name="idx">刪除紀錄ID [fnINDEX_ID]</param>
        public SP_TGetARCIndexByCondition(string sdate, string edate, string statue, string type, long idx = 0)
        {
            StartDate = sdate ?? string.Format($"{DateTime.Now.AddDays(-3):yyyy/MM/dd}");
            EndDate = edate ?? string.Format($"{DateTime.Now.AddDays(+1):yyyy/MM/dd}");
            //-1暫刪除選項(資料欄位實際是空值)
            Status = (statue == "*" || statue == "-1") ? string.Empty : statue;
            Type = type == "*" ? string.Empty : type;
            IndexId = idx;
        }

        /// <summary>
        /// 狀態 @fsSTATUS
        /// </summary>
        [Display(Name = "狀態")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 類別 @fsTYPE
        /// </summary>
        [Display(Name = "類別")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 編號 @fnINDEX_ID (媒資刪除記錄)
        /// </summary>
        public long IndexId { get; set; } = 0;
    }

}
