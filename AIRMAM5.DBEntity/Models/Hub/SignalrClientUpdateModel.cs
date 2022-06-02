
namespace AIRMAM5.DBEntity.Models.Hub
{
    /// <summary>
    /// User Client Connection Hub Id UpdateModel 用戶端資料更新。
    /// 　繼承參考 <see cref="SignalrUserConnectId"/>
    /// </summary>
    public class SignalrClientUpdateModel : SignalrUserConnectId
    {
        /// <summary>
        /// 角色群組id
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// 使用者帳號
        /// </summary>
        public string LoginId { get; set; }

        /// <summary>
        /// 使用者登入當次 tblLOGIN.[fnLOGIN_ID]
        /// </summary>
        public long LoginLogId { get; set; }
    }

}
