using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.Function
{
    /// <summary>
    /// 角色與功能項目清單列表
    /// </summary>
    public class RoleFuncMenuViewModel
    {
        /// <summary>
        /// 角色群組Id
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 角色群組名稱
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 功能項目列表 <see cref="FunctionMenuViewModel"/>
        /// </summary>
        public List<FunctionMenuViewModel> FuncItemList { get; set; }
    }

}
