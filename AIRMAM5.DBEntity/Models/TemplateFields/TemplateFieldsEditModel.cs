using System.Collections.Generic;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.TemplateFields
{
    /// <summary>
    /// 樣板-自訂欄位 編輯頁-欄位定義。 繼承參考 <see cref="TemplateFieldsViewModel"/>
    /// </summary>
    /// <remarks> 檢視頁 [_Cog.cshtml] </remarks>
    public class TemplateFieldsEditModel : TemplateFieldsViewModel
    {
        //static CodeService _ser = new CodeService();

        /// <summary>
        /// 樣板-自訂欄位 編輯頁-欄位定義。檢視頁 [_Cog.cshtml]。
        /// </summary>
        public TemplateFieldsEditModel()
        {
            //FieldTypes = _ser.GetCodeItemList(TbzCodeIdEnum.TEMP002.ToString(), true, false);
            //TableList = _ser.GetCodeItemList(TbzCodeIdEnum.TEMP001.ToString());
        }

        /// <summary>
        /// 自訂欄位資料型別下拉選單
        /// </summary>
        public List<SelectListItem> FieldTypes { get; set; }

        /// <summary>
        /// 樣板類別:提供使用的目的資料表 下拉選單 fsCODE_ID='TEMP001'
        /// </summary>
        public List<SelectListItem> TableList { get; set; }
    }

}
