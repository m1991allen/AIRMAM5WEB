using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Material
{
    /// <summary>
    /// 調用參數(調用樣板選項)
    /// </summary>
    public class BookingOptionModel
    {
        /// <summary>
        /// 調用參數(調用樣板選項)
        /// </summary>
        public BookingOptionModel() { }

        #region >>>> 欄位參數
        /// <summary>
        /// 調用原因 dbo.[tbmBOOKING_T].[fsNAME]
        /// </summary>
        [Display(Name = "調用原因")]
        public string ResonStr { get; set; } = string.Empty;
        public List<SelectListItem> ResonList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 調用路徑  dbo.[tbzCODE].[fsCODE_ID]= 'BOOKING_PATH'
        /// </summary>
        [Display(Name = "調用路徑")]
        public string PathStr { get; set; } = string.Empty;
        public List<SelectListItem> PathList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 轉出格式(for 影片)
        /// </summary>
        [Display(Name = "轉出格式-影片")]
        public string ProfileV { get; set; } = string.Empty;
        public List<SelectListItem> VideoProfileList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 轉出格式(for 聲音)
        /// </summary>
        [Display(Name = "轉出格式-聲音")]
        public string ProfileA { get; set; } = string.Empty;
        public List<SelectListItem> AudioProfileList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 浮水印
        /// </summary>
        [Display(Name = "浮水印")]
        public string WatermarkStr { get; set; } = string.Empty;
        public List<SelectListItem> WatermarkList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 調用說明是否可為空值 : true-空值、false-不可空值。
        /// </summary>
        public bool DescIsNullable { get; set; } = true;

        /// <summary>
        /// 調用編號s [fsMATERIAL_ID] (多筆以"^"為分隔符號
        /// </summary>
        public string MaterialIds { get; set; } = null;
        #endregion
    }

}
