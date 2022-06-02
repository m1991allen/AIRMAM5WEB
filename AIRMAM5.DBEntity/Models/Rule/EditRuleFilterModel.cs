using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Rule
{
    /// <summary>
    /// 編輯 規則定義條件(tbmRULE_FILTER) MODEL。　繼承參考<see cref="RuleCategoryModel"/>
    /// </summary>
    public class EditRuleFilterModel : RuleCategoryModel
    {
        /// <summary>
        /// 編輯 規則定義條件(tbmRULE_FILTER) MODEL
        /// </summary>
        public EditRuleFilterModel() { }

        #region >>> 欄位參數 <<<
        /// <summary>
        /// fsTARGETTABLE 目標資料表
        /// </summary>
        [Display(Name = "規則資料表")]
        public string TargetTable { get; set; } = string.Empty;

        /// <summary>
        /// fsFILTERFIELD 篩選欄位
        /// </summary>
        [Display(Name = "欄位")]
        public string FilterField { get; set; } = string.Empty;

        /// <summary>
        /// fsOPERATOR 運算子: 0包含 include、1不包含 exclude、2等於 equal、3介於 BETWEEN、....
        /// </summary>
        [Display(Name = "運算子")]
        public string Operator { get; set; } = string.Empty;

        /// <summary>
        /// fsFILTERVALUE 篩選值: 分號(;)為分隔符號
        /// </summary>
        [Display(Name = "篩選值")]
        public string FilterValue { get; set; } = string.Empty;

        /// <summary>
        /// 資料型別
        /// </summary>
        [Display(Name = "資料型別")]
        public string FieldType { get; set; } = string.Empty;

        /// <summary>
        /// fsPRIORITY 優先序
        /// </summary>
        [Display(Name = "優先序")]
        public int Priority { get; set; } = 1;

        /// <summary>
        /// fsISENABLED 是否啟用
        /// </summary>
        [Display(Name = "是否啟用")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// fsNOTE 備註
        /// </summary>
        public string Note { get; set; } = string.Empty;

        /// <summary>
        /// 條件邏輯: AND, OR
        /// </summary>
        public string WhereClause { get; set; } = "AND";
        #endregion

        /// <summary>
        /// 編輯 規則定義條件(tbmRULE_FILTER) MODEL  - 資料格式轉換
        /// </summary>
        /// <typeparam name="T">來源資料 類型Model </typeparam>
        /// <param name="data">來源資料 內容Data </param>
        /// <returns></returns>
        public EditRuleFilterModel DataConvert<T>(T data)
        {
            if (data == null) { return this; }

            var properties = typeof(T).GetProperties();
            foreach (var pp in properties)
            {
                object val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fsRULECATEGORY") { this.RuleCategory = val.ToString(); }
                if (pp.Name == "fsTABLE") { this.TargetTable = val.ToString(); }
                if (pp.Name == "fsCOLUMN") { this.FilterField = val.ToString(); }
                if (pp.Name == "fsOPERATOR") { this.Operator = val.ToString(); }
                if (pp.Name == "fsFILTERVALUE") { this.FilterValue = val.ToString(); }
                if (pp.Name == "fnPRIORITY")
                {
                    if (int.TryParse(val.ToString(), out int num)) { this.Priority = num; }
                }
                if (pp.Name == "fbISENABLED")
                {
                    if (bool.TryParse(val.ToString(), out bool chk)) { IsEnabled = chk; }
                }
                if (pp.Name == "fsNOTE") { Note = val.ToString(); }
            }

            return this;
        }
    }

}
