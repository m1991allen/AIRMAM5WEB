
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Rule
{
    /// <summary>
    /// 規則條件 啟用/停用 參數。　繼承參考<see cref="RuleCategoryModel"/>
    /// </summary>
    public class RuleFilterActiveModel : RuleCategoryModel
    {
        /// <summary>
        /// 條件資料表 fsTABLE
        /// </summary>
        [Display(Name = "規則資料表")]
        public string Table { get; set; } = string.Empty;

        /// <summary>
        /// 條件篩選欄位 fsCOLUMN
        /// </summary>
        [Display(Name = "欄位")]
        public string Column { get; set; } = string.Empty;

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsActive { get; set; }

    }
}
