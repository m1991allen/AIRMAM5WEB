using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.User
{
    /// <summary>
    /// 使用者 ID,帳號
    /// </summary>
    public class UserIdModel
    {
        /// <summary>
        /// 使用者識別Id
        /// </summary>
        [Display(Name = "使用者編號")]
        public string fsUSER_ID { get; set; }

        /// <summary>
        /// 帳號 
        /// </summary>
        [Display(Name = "使用者帳號")]
        public string fsLOGIN_ID { get; set; }
    }
}
