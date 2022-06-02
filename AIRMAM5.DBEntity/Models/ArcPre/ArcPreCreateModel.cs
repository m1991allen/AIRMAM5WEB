using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.ArcPre
{
    /// <summary>
    /// 預編詮釋 _Create Model
    /// </summary>
    public class ArcPreCreateModel
    {
        /// <summary>   ↓ ↑ Marked_&_Modified_20210903
        /// 初始 下拉選單
        /// </summary>
        public ArcPreCreateModel() { }

        /// <summary>
        /// 類型 fsTYPE = S、V、A、P、D
        /// </summary>
        [Required]
        [Display(Name = "類型")]
        public List<SelectListItem> ArcPreTypeList { get; set; }

        /// <summary>
        /// 樣板 fnTEMP_ID
        /// </summary>
        [Display(Name = "樣板")]
        public List<SelectListItem> ArcPreTemplateList { get; set; }
    }
}
