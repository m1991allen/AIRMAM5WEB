using AIRMAM5.DBEntity.Models.ArcPre;
using AIRMAM5.DBEntity.Models.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Subject
{
    /// <summary>
    /// (節點)主題 _Details/_Edit  繼承參考 <see cref="TableUserDateByNameModel"/>
    /// </summary>
    public class SubjectViewModel : TableUserDateByNameModel
    {
        public SubjectViewModel() { }

        /// <summary>
        /// 主題編號
        /// </summary>
        [Display(Name = "主題編號")]
        public string SubjectId { set; get; } = string.Empty;

        /// <summary>
        /// 標題
        /// </summary>
        [Display(Name = "標題")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 描述 
        /// </summary>
        [Display(Name = "描述")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 主題(所在目錄節點)欄位內容(變動欄位) 清單 <see cref="ArcPreAttributeModel"/>
        /// </summary>
        //*無[FieldValue]值 *//public List<TemplateFieldsModel> SubjectAttributes { get; set; } = new List<TemplateFieldsModel>();
        public List<ArcPreAttributeModel> SubjectAttributes { get; set; } = new List<ArcPreAttributeModel>();
    }

}
