using System;

namespace AIRMAM5.DBEntity.Models.Notify
{
    /// <summary>
    /// 訊息通知資料欄位
    /// </summary>
    public class NotifyDataModel
    {
        /// <summary>
        /// 訊息通知編號
        /// </summary>
        public long NOTIFY_ID { get; set; }

        /// <summary>
        /// 訊息通知標題
        /// </summary>
        public string TITLE { get; set; }

        /// <summary>
        /// 訊息通知內容
        /// </summary>
        public string CONTENT { get; set; }

        /// <summary>
        /// 是否讀取
        /// </summary>
        public bool IsRead { get; set; }

        public DateTime CREATED_DATE { get; set; }

        public string CREATED_BY { get; set; }
    }

}
