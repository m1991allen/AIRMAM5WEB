using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Template
{
    /// <summary>
    /// 自訂欄位樣板 搜尋Model。 繼承參考 <see cref="TemplateIdModel"/>
    /// </summary>
    public class TemplateSearchModel : TemplateIdModel
    {
        /// <summary>
        /// 自訂欄位樣板 搜尋Model
        /// </summary>
        public TemplateSearchModel() { }

        /// <summary>
        /// 樣板類別:提供使用的目的資料表 fsCODE_ID='TEMP001'
        /// </summary>
        [Required]
        [Display(Name = "樣板類別")]
        public string fsTABLE { get; set; } = string.Empty;
    }

}
