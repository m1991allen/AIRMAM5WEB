using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Rule
{
    /// <summary>
    /// 新建規則 View MODEL
    /// </summary>
    public class CreateRuleViewModel
    {
        //static CodeService _codeSer = new CodeService();
        //static RuleService _ruleSer = new RuleService();

        public CreateRuleViewModel() { }

        #region >>>Marked_BY_20210902 加入DI而調整, 
        ///// <summary>  
        ///// 新建規則 初始
        ///// </summary>
        ///// <param name="type"> 流程類型 </param>
        //public CreateRuleViewModel(string type = null)
        //{
        //    //_流程類型 List
        //    var ruleLst = _ruleSer.GetRuleListForCreate();
        //    if (ruleLst == null) { return; }
        //    RuleCategoryList = ruleLst;

        //    //未指定流程類型, 以 ruleLst第一筆為預設
        //    type = string.IsNullOrEmpty(type) ? ruleLst.FirstOrDefault().Value : type;
        //    var _rule = _ruleSer.GetRuleBy(type).FirstOrDefault();
        //    RuleCategory = type;
        //    RuleName = _rule == null ? string.Empty : _rule.fsRULENAME;

        //    //_規則資料表 List
        //    var _ruletable = _ruleSer.UnspecifyRuleTableColumns(type);
        //    List<FilterTableModel> _tableList = _ruletable.GroupBy(g => new { g.fsTABLE })
        //        .Select(s => new FilterTableModel
        //        {
        //            TableName = s.Key.fsTABLE,
        //            TableDesc = _ruletable.Where(x => x.fsTABLE == s.Key.fsTABLE).FirstOrDefault().fsTABLE_NAME ?? string.Empty,
        //            Properties = _ruletable.Where(x => x.fsTABLE == s.Key.fsTABLE).Select(f => new FieldInfo(f)).ToList()
        //        }).ToList();

        //    TableList = _tableList;
        //}
        #endregion

        #region >>> 欄位定義
        /// <summary>
        /// fsRULECATEGORY 規則類別: 參考代碼"RULE", 調用 BOOKING、入庫 UPLOAD、轉檔 TRANSCODE
        /// </summary>
        [Display(Name = "流程類型")]
        public string RuleCategory { get; set; } = string.Empty;

        /// <summary>
        /// 規則名稱
        /// </summary>
        [Display(Name = "規則名稱")]
        public string RuleName { get; set; } = string.Empty;

        /// <summary>
        /// 套用規則表/ 規則資料表
        /// </summary>
        [Display(Name = "套用規則表")]
        public string RuleTable { get; set; } = string.Empty;

        /// <summary>
        /// 備註
        /// </summary>
        [Display(Name = "備註")]
        public string Note { get; set; } = string.Empty;
        #endregion

        #region >>> 欄位定義:下拉清單
        /// <summary>
        /// 規則資料表 List
        /// </summary>
        [Display(Name = "套用規則表")]
        public List<FilterTableModel> TableList { get; set; } = new List<FilterTableModel>();

        /// <summary>
        ///  fsRULECATEGORY 規則類別 下拉清單
        /// </summary>
        public List<SelectListItem> RuleCategoryList { get; set; } = new List<SelectListItem>();
        #endregion
    }

}
