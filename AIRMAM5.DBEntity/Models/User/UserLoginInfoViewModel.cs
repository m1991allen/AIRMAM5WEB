using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.User
{
    /// <summary>
    /// 使用者 登入資訊
    /// </summary>
    public class UserLoginInfoViewModel : UserIdModel
    {
        public UserLoginInfoViewModel() { }

        #region >>> 欄位參數
        /// <summary>
        /// 使用者姓名
        /// </summary>
        [Display(Name = "顯示名稱")]
        public string RealName { get; set; }

        /// <summary>
        /// 隸屬部門ID
        /// </summary>
        [Display(Name = "隸屬單位")]
        public string DeptId { get; set; }

        /// <summary>
        /// 隸屬部門
        /// </summary>
        [Display(Name = "隸屬單位")]
        public string DeptName { get; set; }

        /// <summary>
        /// 電子郵件
        /// </summary>
        [Display(Name = "電子郵件")]
        public string Email { get; set; }

        /// <summary>
        /// 使用者所屬群組/角色
        /// </summary>
        [Display(Name = "所屬群組")]
        //public List<UserRolesViewModel> UserRoles { get; set; } = new List<UserRolesViewModel>();
        public List<string> UserRoles { get; set; } = new List<string>();

        /// <summary>
        /// 登入記錄編號
        /// </summary>
        public long LoginLogid { get; set; } = 0;
        #endregion
    }

}
