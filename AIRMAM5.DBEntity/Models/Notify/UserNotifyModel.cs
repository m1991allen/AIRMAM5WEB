using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.Notify
{
    /// <summary>
    /// 使用者通知訊息與數量 Model
    /// </summary>
    /// <remarks> 主頁面上方快捷列: 未讀訊息,訊息列表 </remarks>
    public class UserNotifyModel
    {
        /// <summary>
        /// 使用者userid
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 未讀訊息數量
        /// </summary>
        public int UnRead { get; set; } = 0;

        /// <summary>
        /// 訊息清單(僅顯示未刪除)
        /// </summary>
        public List<NotifyDataModel> DataList { get; set; } = new List<NotifyDataModel>();
    }
}
