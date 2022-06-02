using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.ArcPre
{
    /// <summary>
    /// 預編詮釋 _Search Model
    /// </summary>
    public class ArcPreSearchModel
    {
        /// <summary>
        /// 預編詮釋 _Search Model
        /// </summary>
        public ArcPreSearchModel()
        {
            ArcPreTypeList = new List<SelectListItem>();
        }

        /// <summary>
        /// 預編名稱 fsNAME
        /// </summary>
        [Required]
        [Display(Name = "預編名稱")]
        public string ArcPreName { get; set; } = string.Empty;

        /// <summary>
        /// 類型 fsTYPE = S、V、A、P、D
        /// </summary>
        [Required]
        [Display(Name = "類型")]
        public List<SelectListItem> ArcPreTypeList { get; set; }
    }

}
