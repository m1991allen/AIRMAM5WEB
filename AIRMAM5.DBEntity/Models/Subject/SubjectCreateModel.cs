using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Directory;
using AIRMAM5.DBEntity.Models.TemplateFields;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Subject
{
    /// <summary>
    /// (節點)主題新增 _Create Model。 繼承參考 <see cref="DirIdModel"/>
    /// </summary>
    public class SubjectCreateModel : DirIdModel
    {
        public SubjectCreateModel()
        {
            this.DateInSubjId = string.Format($"{DateTime.Now:yyyy-MM-dd}");
        }

        ///// <summary>
        ///// 系統目錄Id
        ///// </summary>
        //public long DirId { get; set; }

        /// <summary>
        /// 主題編號中的日期(預設當日yyyyMMdd)
        /// </summary>
        [Display(Name = "主題編號日期")]
        public string DateInSubjId { get; set; }

        /// <summary>
        /// 樣板Id 
        /// </summary>
        public int TemplateId { get; set; }

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
        /// 下拉清單: 主題-預編詮釋  <see cref="spGET_ARC_PRE_Result"/>
        /// </summary>
        [Display(Name = "預編詮釋資料")]
        //public IEnumerable<SelectListItem> ArcPreList { get; set; }
        public IEnumerable<spGET_ARC_PRE_Result> ArcPreList { get; set; }

        /// <summary>
        /// 主題(所在目錄節點)樣版欄位  (變動欄位)  <see cref="TemplateFieldsModel"/>
        /// </summary>
        public List<TemplateFieldsModel> ArcPreAttributes { get; set; } = new List<TemplateFieldsModel>();
    }

}
