
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Directory
{
    /// <summary>
    /// 系統目錄編號 Model
    /// </summary>
    public class DirIdModel
    {
        public DirIdModel() { DirId = 0; }

        /// <summary>
        /// 系統目錄編號 fsDIR_ID
        /// </summary>
        [Display(Name = "系統目錄編號")]
        [Required]
        public long DirId { get; set; }
    }

}
