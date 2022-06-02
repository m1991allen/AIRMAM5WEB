using AIRMAM5.DBEntity.DBEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AIRMAM5.Models.TSMapi
{
    /// <summary>
    /// 取得待上架磁帶資料model
    /// </summary>
    /// <remarks>
    ///  資料來源: spTSM_GET_L_WAIT_VOL_ACTIVE_ALL_Result
    /// </remarks>
    public class GetPendingTapeMode
    {
        /// <summary>
        /// 取得待上架磁帶資料model
        /// </summary>
        public GetPendingTapeMode() { }

        /// <summary>
        /// 取得待上架磁帶資料model
        /// </summary>
        /// <param name="m">預存資料: spTSM_GET_L_WAIT_VOL_ACTIVE_ALL_Result </param>
        public GetPendingTapeMode(spTSM_GET_L_WAIT_VOL_ACTIVE_ALL_Result m)
        {
            this.TapeNumber = m.fsVOL_ID;
            this.StatusName = m.fsSATUS_NAME;
            this.CreatedDate = m.fdCREATED_DATE;
            this.CreatedBy = m.fsCREATED_BY;
            this.WrokId = m.fnWORK_ID;
            this.Priority = m.fsPRIORITY;
            this.BookingReason = m.fsBOOKING_REASON;
        }

        #region 欄位資訊
        /// <summary>
        /// 磁帶編號 fsVOL_ID
        /// </summary>
        [Display(Name = "磁帶編號")]
        public string TapeNumber { get; set; }

        /// <summary>
        /// 狀態 fsSATUS_NAME
        /// </summary>
        [Display(Name = "狀態")]
        public string StatusName { get; set; }

        /// <summary>
        /// 要求日期 fdCREATED_DATE 
        /// </summary>
        [Display(Name = "要求日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 調用人員 fsCREATED_BY
        /// </summary>
        [Display(Name = "調用人員")]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// 轉檔編號 fnWORK_ID
        /// </summary>
        [Display(Name = "轉檔編號")]
        public long WrokId { get; set; }

        /// <summary>
        /// 優先序號 fsPRIORITY
        /// </summary>
        [Display(Name = "優先序")]
        public string Priority { get; set; }

        /// <summary>
        /// 調用原因 fsBOOKING_REASON
        /// </summary>
        [Display(Name = "調用原因")]
        public string BookingReason { get; set; } = string.Empty;
        #endregion
    }
}