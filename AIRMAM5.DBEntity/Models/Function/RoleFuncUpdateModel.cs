using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.Function
{
    /// <summary>
    /// 角色可用功能項目 更新Model
    /// </summary>
    public class RoleFuncUpdateModel
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 可使用 功能項目Id List <see cref="FunctionIdModel"/>
        /// </summary>
        public List<FunctionIdModel> FunctionIds { get; set; }
    }

}
