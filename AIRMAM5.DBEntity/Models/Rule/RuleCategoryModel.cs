
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Rule
{
    /// <summary>
    /// 規則定義資料表(tbmRULE) PKEY [fsRULECATEGORY]
    /// </summary>
    public class RuleCategoryModel
    {
        /// <summary>
        /// fsRULECATEGORY 規則類別 : 0調用 BOOKING、1入庫 UPLOAD、2轉檔 TRANSCODE
        /// </summary>
        [Display(Name = "流程類型")]
        public string RuleCategory { get; set; } = string.Empty;
    }

}
