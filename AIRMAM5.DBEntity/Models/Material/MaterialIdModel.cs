using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Material
{
    /// <summary>
    /// 借調檔案編號 [fnMATERIAL_ID]
    /// </summary>
    public class MaterialIdModel
    {
        /// <summary>
        /// 借調ID
        /// </summary>
        [Display(Name = "借調編號")]
        public long MaterialId { get; set; }
    }

}
