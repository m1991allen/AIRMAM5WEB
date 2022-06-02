
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AIRMAM5.DBEntity.Models.Rule
{
    /// <summary>
    /// 編輯流程規則條件 MODEL。　繼承參考<see cref="RuleCategoryModel"/>
    /// </summary>
    public class EditRuleFilterViewModel : RuleCategoryModel//: EditRuleFilterModel
    {
        /// <summary>
        /// 編輯流程規則條件 MODEL
        /// </summary>
        public EditRuleFilterViewModel() { }

        #region ---tbmRULE
        /// <summary>
        /// 規則名稱 [tbmRULE].fsRULENAME 
        /// </summary>
        [Display(Name = "規則名稱")]
        public string RuleName { get; set; } = string.Empty;

        /// <summary>
        /// 是否啟用 [tbmRULE].IsEnabled
        /// </summary>
        public bool RuleIsEnabled { get; set; } = true;
        /// <summary>
        /// 備註 [tbmRULE].fsNOTE
        /// </summary>
        [Display(Name = "備註")]
        public string RuleNote { get; set; } = string.Empty;
        #endregion

        #region ---tbmRULE_FILTER
        /// <summary>
        /// [tbmRULE_FILTER].fsTARGETTABLE 目標資料表
        /// </summary>
        [Display(Name = "套用規則表")]
        public string TargetTable { get; set; } = string.Empty;

        /// <summary>
        /// [tbmRULE_FILTER].fsFILTERFIELD 篩選欄位
        /// </summary>
        [Display(Name = "欄位")]
        public string FilterField { get; set; } = string.Empty;

        /// <summary>
        /// fsTABLE 規則資料表
        /// </summary>
        [Display(Name = "規則資料表")]
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// fsCOLUMN_NAME 欄位名稱
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// [tbmRULE_FILTER].fsPRIORITY 優先序
        /// </summary>
        [Display(Name = "優先序")]
        public int Priority { get; set; } = 1;

        /// <summary>
        /// 條件邏輯: AND, OR
        /// </summary>
        [Display(Name = "條件邏輯")]
        public string WhereClause { get; set; } = "AND";

        /// <summary>
        /// [tbmRULE_FILTER].fsOPERATOR 
        /// <para> 運算子(Enum): 0包含 include、1不包含 exclude、2等於 equal、3介於 BETWEEN、....</para>
        /// </summary>
        [Display(Name = "運算子")]
        public string Operator { get; set; } = string.Empty;

        /// <summary>
        /// fsISENABLED 是否啟用
        /// </summary>
        [Display(Name = "是否啟用")]
        public bool IsEnabled { get; set; } = true;
        #endregion

        #region ------ 欄位屬性與篩選值
        /// <summary>
        /// [tbmRULE_FILTER].fsFILTERVALUE 篩選值: 分號(;)為分隔符號
        /// </summary>
        [Display(Name = "篩選值")]
        public string FilterValue { get; set; } = string.Empty;

        /// <summary>
        /// [tbmRULE_FILTER].fsFILTERVALUE 篩選值Array
        /// </summary>
        public string[] FilterValueAry { get; set; } = new string[] { };

        /// <summary>
        /// 規則資料表欄位屬性清單
        /// </summary>
        public FieldInfo Properties { get; set; }
        #endregion

        /* marked_20211001_取消類別中的方法 */
        /// <summary>
        /// 資料轉換 <see cref="EditRuleFilterViewModel"/>類型格式 
        /// </summary>
        /// <typeparam name="T">來源資料 格式 eg. 預存回傳結果 <see cref="spGET_RULE_BY_RULE_TABLE_COLUMN_Result"/> </typeparam>
        /// <param name="data">來源資料 內容 </param>
        /// <returns></returns>
        public EditRuleFilterViewModel DataConvert<T>(T data)
        {
            if (data == null) { return this; }

            var properies = typeof(T).GetProperties();
            foreach (var p in properies)
            {
                string val = p.GetValue(data) == null ? string.Empty : p.GetValue(data).ToString();

                if (p.Name == "fsRULECATEGORY") { this.RuleCategory = val; }
                if (p.Name == "fsRULENAME") { this.RuleName = val; }
                if (p.Name == "RuleIsEnabled")
                {
                    if (bool.TryParse(val, out bool chk)) { this.RuleIsEnabled = chk; }
                }

                if (p.Name == "RuleNote") { this.RuleNote = val; }
                if (p.Name == "fsTABLE") { this.TargetTable = val; }
                if (p.Name == "fsTABLE_NAME") { this.TableName = val; }
                if (p.Name == "fsCOLUMN") { this.FilterField = val; }
                if (p.Name == "fsCOLUMN_NAME") { this.ColumnName = val; }

                if (p.Name == "fnPRIORITY")
                {
                    if (int.TryParse(val, out int num)) { this.Priority = num; }
                }

                if (p.Name == "fsWHERE_CLAUSE") { this.WhereClause = val; }
                if (p.Name == "fsOPERATOR") { Operator = val; }

                if (p.Name == "fbISENABLED")
                {
                    if (bool.TryParse(val, out bool chk)) { IsEnabled = chk; }
                }

                if (p.Name == "fsFILTERVALUE")
                {
                    FilterValue = val;
                    FilterValueAry = val.Split(new char[] { ';', '^' }).ToArray();
                }
            }

            return this;
        }
    }

}
