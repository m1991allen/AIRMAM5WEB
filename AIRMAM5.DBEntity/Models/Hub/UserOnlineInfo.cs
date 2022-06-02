using System;

namespace AIRMAM5.DBEntity.Models.Hub
{
    /// <summary>
    /// Client端連線成功資訊。　繼承參考 <see cref="SignalrClientUpdateModel"/>
    /// </summary>
    public class UserOnlineInfo : SignalrClientUpdateModel
    {
        /// <summary>
        /// Client端連線成功資訊
        /// </summary>
        public UserOnlineInfo() { }

        /// <summary>
        /// Client端連線成功資訊
        /// </summary>
        /// <param name="m"> 用戶端更新資料model <see cref="SignalrClientUpdateModel"/> </param>
        public UserOnlineInfo(SignalrClientUpdateModel m)
        {
            this.UserId = m.UserId;
            this.LoginId = m.LoginId;
            this.GroupId = m.GroupId;
            this.SignalrConnectionId = m.SignalrConnectionId;
            this.LoginLogId = m.LoginLogId;
            this.ConnectTime = DateTime.Now;
        }

        /// <summary>
        /// 連線(起始)時間
        /// </summary>
        public DateTime ConnectTime { get; set; }
    }
}
