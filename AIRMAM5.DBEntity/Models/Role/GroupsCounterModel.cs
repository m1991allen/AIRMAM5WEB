using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Role
{
    /// <summary>
    /// 角色管理帳號數量統計 Model。　繼承參考<see cref="GroupsViewModel"/>
    /// </summary>
    public class GroupsCounterModel : GroupsViewModel
    {
        /// <summary>
        /// 角色帳號數量
        /// </summary>
        [DisplayName("帳號數量")]
        public int AccountCounts { get; set; }
    }
}
