
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Rule
{
    /// <summary>
    /// 編輯 規則定義表(tbmRULE) MODEL。　繼承參考<see cref="RuleCategoryModel"/>
    /// </summary>
    public class EditRuleModel : RuleCategoryModel
    {
        /// <summary>
        /// 編輯 規則定義表(tbmRULE) MODEL
        /// </summary>
        public EditRuleModel() { }

        #region >>> 欄位參數
        /// <summary>
        /// fsRULENAME 規則名稱
        /// </summary>
        [Display(Name = "規則名稱")]
        public string RuleName { get; set; } = string.Empty;

        /// <summary>
        /// 是否啟用
        /// </summary>
        [Display(Name = "是否啟用")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// fsNOTE 備註
        /// </summary>
        [Display(Name = "備註")]
        public string Note { get; set; } = string.Empty;
        #endregion

        /// <summary>
        /// 編輯 規則定義表(tbmRULE) MODEL - 資料格式轉換
        /// </summary>
        /// <typeparam name="T">來源資料 類型Model </typeparam>
        /// <param name="data">來源資料 內容Data </param>
        /// <returns></returns>
        public EditRuleModel DataConvert<T>(T data)
        {
            if (data == null) { return this; }

            var properties = typeof(T).GetProperties();
            foreach (var pp in properties)
            {
                object val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fsRULECATEGORY") { this.RuleCategory = val.ToString(); }
                if (pp.Name == "fsRULENAME") { this.RuleName = val.ToString(); }
                if (pp.Name == "fbISENABLED")
                {
                    if (bool.TryParse(val.ToString(), out bool chk)) { IsEnabled = chk; }
                }

                if (pp.Name == "fsNOTE") { Note = val.ToString() ?? string.Empty; }
            }
            return this;
        }
    }

}
