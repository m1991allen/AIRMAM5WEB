using AIRMAM5.DBEntity.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Rule
{
    /// <summary>
    /// 規則資料表欄位(條件(篩選) 設定 。　繼承參考<see cref="RuleCategoryModel"/>
    /// </summary>
    public class RuleListFilterModel : RuleCategoryModel
    {
        /// <summary>
        /// 規則資料表欄位(條件(篩選) 設定 
        /// </summary>
        public RuleListFilterModel() { }
        
        /// <summary>
        /// 流程規則條件key值組合: [fsCategory]_[fsTable]_[fsColumn]
        /// </summary>
        public string FilterKey { get; set; }

        #region >>> 欄位參數
        //RuleCategory
        /// <summary>
        /// fsTABLE 規則資料表
        /// </summary>
        [Display(Name = "規則資料表")]
        public string RuleTable { get; set; } = string.Empty;

        /// <summary>
        /// fsCOLUMN 資料表欄位
        /// </summary>
        [Display(Name = "欄位")]
        public string RuleColumn { get; set; } = string.Empty;

        /// <summary>
        /// fsCOLUMN_NAME 欄位名稱
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// fsOPERATOR 運算子: 0包含 Include、1不包含 Exclude、等於 Equal、2介於 Beetween、....
        /// </summary>
        [Display(Name = "運算子")]
        public string Operator { get; set; } = string.Empty;
        public string OperatorStr { get; set; } = string.Empty;

        /// <summary>
        /// fsFILTERVALUE 篩選值: 分號(;)為分隔符號
        /// </summary>
        [Display(Name = "篩選值")]
        public string FilterValue { get; set; } = string.Empty;

        /// <summary>
        /// fsPRIORITY 優先序
        /// </summary>
        [Display(Name = "優先序")]
        public int Priority { get; set; } = 1;

        /// <summary>
        /// fsISENABLED 是否啟用
        /// </summary>
        [Display(Name = "是否啟用")]
        //public string IsEnabled { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 條件邏輯: AND, OR
        /// </summary>
        public string WhereClause { get; set; }
        #endregion

        /// <summary>
        /// 規則資料表欄位(條件(篩選) 設定  - 資料格式轉換
        /// </summary>
        /// <typeparam name="T">來源資料 類型Model </typeparam>
        /// <param name="data">來源資料 內容Data </param>
        /// <returns></returns>
        public RuleListFilterModel DataConvert<T>(T data)
        {
            if (data == null) { return this; }

            var properties = typeof(T).GetProperties();
            foreach (var pp in properties)
            {
                var val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fsRULECATEGORY") { RuleCategory = val.ToString() ?? string.Empty; }
                if (pp.Name == "fsTABLE") { RuleTable = val.ToString() ?? string.Empty; }
                if (pp.Name == "fsCOLUMN") { RuleColumn = val.ToString() ?? string.Empty; }
                if (pp.Name == "fsCOLUMN_NAME") { ColumnName = val.ToString(); }
                if (pp.Name == "fsOPERATOR") { Operator = val.ToString(); }
                if (pp.Name == "fsOPERATOR") { OperatorStr = GetEnums.GetDescriptionText<OperatorEnum>(val.ToString()); }
                if (pp.Name == "C_sFILTERVALUE") { FilterValue = val.ToString(); }

                if (pp.Name == "fnPRIORITY")
                {
                    if (int.TryParse(val.ToString(), out int num)) { Priority = num; }
                }

                if (pp.Name == "fbISENABLED")
                {
                    if (bool.TryParse(val.ToString(), out bool chk)) { IsEnabled = chk; }
                }

                if (pp.Name == "fsWHERE_CLAUSE") { WhereClause = val.ToString() ?? "AND"; }

            }

            this.FilterKey = string.Format($"{this.RuleCategory}_{this.RuleTable}_{this.RuleColumn}");

            return this;
        }
    }

}
