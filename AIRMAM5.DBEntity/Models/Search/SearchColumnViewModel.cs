using AIRMAM5.DBEntity.Models.Template;
using AIRMAM5.DBEntity.Models.Enums;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Search
{
    /// <summary>
    /// (提供前端)檢索欄位資料 _LoginPartial
    /// </summary>
    public class SearchColumnViewModel
    {
        /// <summary>
        /// 初始 : 全文檢索固定為0,龍捲風的模糊搜尋固定為1 
        /// </summary>
        public SearchColumnViewModel()
        {
            //不顯示"主題 Subject_DEV"項目
            SearchType = GetEnums.GetEnumList<SearchTypeEnum>(SearchTypeEnum.Subject_DEV);
            QueryType = new List<SelectListItem>
                {
                    new SelectListItem{ Value = "0", Text = "全文檢索" },
                    //new SelectListItem{ Value = "1", Text = "模糊搜尋" }
                };
            DateType = new List<SelectListItem>
                {
                    new SelectListItem{ Value = "fdCREATED_DATE", Text = "建立日期" },
                    //new SelectListItem{ Value = "fdUPDATED_DATE", Text = "建立日期" }
                };
            ////不顯示"主題 S" 樣板
            //var _tmpsr = new TemplateService();
            //SearchTemplate = _tmpsr.GetByParam().Where(x => x.fsTABLE.IndexOf(FileTypeEnum.S.ToString()) <= 0).Select(s => new TemplateBaseModel(s)).ToList();
        }

        /// <summary>
        /// 檢索類型選單
        /// </summary>
        public List<SelectListItem> SearchType { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 查詢方式
        /// </summary>
        public List<SelectListItem> QueryType { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 日期選單: 
        /// </summary>
        public List<SelectListItem> DateType { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 進階檢索樣板選單
        /// </summary>
        //public List<SelectListItem> SearchTemplate { get; set; } = new List<SelectListItem>();
        public List<TemplateBaseModel> SearchTemplate { get; set; }

        /// <summary>
        /// 最多可使用幾個欄位查詢 'SEARCH_MAX_COLUMN'
        /// </summary>
        public int SearchMaxColumn { get; set; } = 3;
    }

}
