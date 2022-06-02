using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.Hub
{
    /// <summary>
    /// 在線人數與清單
    /// </summary>
    public class OnlineDataModel
    {
        /// <summary>
        /// 數量
        /// </summary>
        public int Number { get; set; } = 0;

        /// <summary>
        /// 在線使用者清單 <see cref="OnlineMembersModel"/>
        /// </summary>
        public List<OnlineMembersModel> DataList { get; set; } = new List<OnlineMembersModel>();
    }
}
