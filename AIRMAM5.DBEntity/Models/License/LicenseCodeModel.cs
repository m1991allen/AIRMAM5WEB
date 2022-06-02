
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.License
{
    /// <summary>
    /// 版權代碼
    /// </summary>
    public class LicenseCodeModel
    {
        /// <summary>
        /// 版權代碼
        /// </summary>
        [Required]
        [Display(Name = "版權代碼")]
        public string LicenseCode { get; set; } = string.Empty;
    }
}
