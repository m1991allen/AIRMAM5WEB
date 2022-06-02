using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.Models
{
    /*public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }
    }*/

    /*public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }*/

    /*public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }*/

    /*public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "代碼")]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "記住此瀏覽器?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }*/

    /*public class ForgotViewModel
    {
        [Required]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }
    }*/

    /// <summary>
    /// 登入欄位
    /// </summary>
    public class LoginViewModel
    {
        //[Required]
        //[Display(Name = "電子郵件")]
        //[EmailAddress]
        //public string Email { get; set; }

        [Required]
        [Display(Name = "帳號")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [Display(Name = "記住我的帳號")]
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// 註冊Model
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// 電子郵件
        /// </summary>
        //[Required]
        [EmailAddress]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        [Required]
        [Display(Name = "帳號")]
        public string UserName { get; set; }

        /// <summary>
        /// 使用者姓名
        /// </summary>
        [Required]
        [Display(Name = "姓名")]
        public string RealName { get; set; }

        /// <summary>
        /// 帳號角色 Role
        /// </summary>
        [Required]
        [Display(Name = "帳號角色")]
        public string RoleName { get; set; }
        public List<System.Web.Mvc.SelectListItem> RoleList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        /// <summary>
        /// 隸屬部門
        /// </summary>
        public string DeptId { get; set; }
        public List<System.Web.Mvc.SelectListItem> DeptList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        [Required]
        [StringLength(100, ErrorMessage = "{0} 的長度至少必須為 {2} 個字元。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        [Compare("Password", ErrorMessage = "密碼和確認密碼不相符。")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// 重設密碼
    /// </summary>
    public class ResetPasswordViewModel
    {
        /// <summary>
        /// 新密碼
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "{0} 的長度至少必須為 {2} 個字元。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string Password { get; set; }
        /// <summary>
        /// 確認密碼
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        [Compare("Password", ErrorMessage = "密碼和確認密碼不相符。")]
        public string ConfirmPassword { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 使用者ID
        /// </summary>
        public string UserId { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }
    }

}
