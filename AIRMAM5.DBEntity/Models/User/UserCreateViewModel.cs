using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.User
{
    /// <summary>
    /// 使用者 新建帳號欄位Model
    /// <para>　　1、新建帳號,畫面不須輸入密碼。透過電子郵件驗證信,登入系統、再強制使用者變更密碼。</para>
    /// </summary>
    public class UserCreateViewModel
    {
        public UserCreateViewModel() { }

        #region >>> 欄位參數
        /// <summary>
        /// 使用者帳號
        /// </summary>
        [Required]
        [Display(Name = "使用者帳號")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 密碼 (新建帳號密碼使用系統預設密碼,前端表單不用輸入)
        /// </summary>
        //[Required]
        //[StringLength(100, ErrorMessage = "{0} 的長度至少必須為 {2} 個字元。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 確認密碼 (新建帳號密碼使用系統預設密碼,前端表單不用輸入)
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        //[System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "密碼和確認密碼不相符。")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// 使用者姓名
        /// </summary>
        [Required]
        [Display(Name = "顯示名稱")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 使用者英文名
        /// </summary>
        [Display(Name = "英文名")]
        public string EName { get; set; } = string.Empty;
        /// <summary>
        /// 使用者職稱
        /// </summary>
        [Display(Name = "職稱")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 隸屬單位
        /// </summary>
        [Required]
        [Display(Name = "隸屬單位")]
        public string DeptId { get; set; } = string.Empty;

        /// <summary>
        /// 所屬群組角色
        /// </summary>
        [Display(Name = "群組角色")]
        //[Required]
        public string GroupIds { get; set; } = string.Empty;
        public List<string> RoleList { get; set; } = new List<string>();

        /// <summary>
        /// 電子郵件
        /// </summary>
        [EmailAddress]
        [Display(Name = "電子郵件")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "連絡電話")]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// 預設調用路徑(如：\\SERVER\FOLDER)
        /// </summary>
        [Display(Name = "預設調用路徑")]
        public string BookingTargetPath { get; set; } = string.Empty;

        /// <summary>
        /// 檔案機密權限
        /// </summary>
        [Display(Name = "檔案機密權限")]
        //[Required]
        public string FileSecret { get; set; } = string.Empty;
        public List<string> SecretList { get; set; } = new List<string>();

        [Display(Name = "備註")]
        public string Description { get; set; } = string.Empty;
        #endregion
        
        #region DropDownItemList
        /// <summary>
        /// 單位/部門選單
        /// </summary>
        public List<SelectListItem> DeptList { get; set; } = new List<SelectListItem>();
        /// <summary>
        /// 檔案機密選單
        /// </summary>
        public List<SelectListItem> FileSecretList { get; set; } = new List<SelectListItem>();
        /// <summary>
        /// 角色群組選單
        /// </summary>
        public List<SelectListItem> RoleGroupLst { get; set; } = new List<SelectListItem>();
        #endregion
    }
}
