using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Role
{
    /// <summary>
    /// tbmGROUP 群組/角色管理 檢視模型
    /// </summary>
    /// 
    public class GroupsViewModel
    {
        /// <summary>
        /// 角色群組Id
        /// </summary>
        [DisplayName("角色群組代號")]
        [Required]
        public string RoleId { get; set; }

        /// <summary>
        /// 群組/角色名稱
        /// </summary>
        [DisplayName("角色群組名稱")]
        [Required]
        public string RoleName { get; set; }

        /// <summary>
        /// 群組/角色描述
        /// </summary>
        [DisplayName("系統群組描述")]
        public string Description { get; set; } = string.Empty;
    }

}
