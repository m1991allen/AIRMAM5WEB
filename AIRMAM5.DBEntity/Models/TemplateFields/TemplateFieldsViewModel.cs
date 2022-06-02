using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Template;
using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.TemplateFields
{
    /// <summary>
    /// 樣板-自訂欄位 檢視 欄位定義。 繼承參考 <see cref="TemplateIdModel"/>
    /// </summary>
    public class TemplateFieldsViewModel : TemplateIdModel
    {
        /// <summary>
        /// 樣板-自訂欄位 檢視 欄位定義。
        /// </summary>
        public TemplateFieldsViewModel() : base()
        {
            TemplateMain = new tbmTEMPLATE();
            CustomFieldList = new List<ChooseTypeViewModel>();
        }

        /// <summary>
        /// 自訂樣板主資料表 <see cref="tbmTEMPLATE"/>
        /// </summary>
        public tbmTEMPLATE TemplateMain { get; set; }

        /// <summary>
        /// 自訂樣板欄位-選擇資料類型+欄位屬性 清單 <see cref="tbmTEMPLATE_FIELDS"/>
        /// </summary>
        public List<ChooseTypeViewModel> CustomFieldList { get; set; }
    }

}
