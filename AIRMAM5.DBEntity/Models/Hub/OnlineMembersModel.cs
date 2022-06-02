
namespace AIRMAM5.DBEntity.Models.Hub
{
    /// <summary>
    /// 在線人數清單
    /// </summary>
    public class OnlineMembersModel
    {
        /// <summary>
        /// 登入記錄編號 dbo.tblLOGIN.[fnLOGIN_ID]
        /// </summary>
        public long LoginLogid { get; set; }

        /// <summary>
        /// 使用者ID [fsUSER_ID]
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 使用者帳號 [fsLOGIN_ID]
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// [fdSTIME]
        /// </summary>
        public string LoginDTime { get; set; }

        /// <summary>
        /// [fdETIME]
        /// </summary>
        public string EndDTime { get; set; }

        public string Note { get; set; }
        /// <summary>
        /// 最後異動時間
        /// </summary>
        public string UpdateDTime { get; set; }
    }
}
