using System.Collections.Generic;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Template
{
    //【_Copy.cshtml】
    /// <summary>
    /// 選擇全新樣板or複制樣板Modal (_Copy.cshtml)。 繼承參考 <see cref="TemplateNewCopyModel"/>
    /// </summary>
    public class TemplateNewCopy : TemplateNewCopyModel
    {
        /// <summary>
        /// 樣板類別下拉選單
        /// </summary>
        public List<SelectListItem> TempTableList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 系統樣板選單 tbmTEMPLATE
        /// </summary>
        public List<TemplateBaseModel> TemplateList { get; set; } = new List<TemplateBaseModel>();
    }
}
