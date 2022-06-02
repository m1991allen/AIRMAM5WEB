using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Rule
{
    /// <summary>
    /// 流程規則條件資料表 pk Model。　繼承參考<see cref="RuleCategoryModel"/>
    /// </summary>
    public class RuleFilterKeyModel : RuleCategoryModel
    {
        /// <summary>
        /// 流程規則條件資料表
        /// </summary>
        public RuleFilterKeyModel() { }

        /// <summary>
        /// [tbmRULE_FILTER].fsTARGETTABLE 目標資料表
        /// </summary>
        [Display(Name = "規則資料表")]
        public string RuleTable { get; set; } = string.Empty;

        /// <summary>
        /// [tbmRULE_FILTER].fsFILTERFIELD 篩選欄位
        /// </summary>
        [Display(Name = "欄位")]
        public string RuleColumn { get; set; } = string.Empty;
    }

}
