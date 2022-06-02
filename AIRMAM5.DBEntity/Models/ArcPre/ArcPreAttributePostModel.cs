using AIRMAM5.DBEntity.Models.Template;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.ArcPre
{
    /// <summary>
    /// 預編詮釋tbmARC_PRE 自訂欄位值(編輯預編自訂欄位內容值 存檔回傳使用)。 繼承參考 <see cref="TemplateIdModel"/>
    /// </summary>
    public class ArcPreAttributePostModel : TemplateIdModel
    {
        /// <summary>
        /// 預編詮釋tbmARC_PRE 自訂欄位值(編輯預編自訂欄位內容值 存檔回傳使用)
        /// </summary>
        public ArcPreAttributePostModel() { }

        /**預編詮釋資料 編號 fnPRE_ID *///fnTEMP_ID

        /// <summary>
        /// fsFIELD (樣板)欄位代號, EX: fsATTRIBUTE1, fsATTRIBUTE2, ....
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// fsFIELD_NAME 欄位名稱
        /// </summary>
        [Display(Name = "名稱")]
        [Required]
        public string FieldName { get; set; } = string.Empty;

        /// <summary>
        /// 欄位值
        /// </summary>
        [Display(Name = "欄位值")]
        public string FieldValue { get; set; } = string.Empty;
    }

}
