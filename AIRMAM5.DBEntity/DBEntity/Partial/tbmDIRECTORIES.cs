using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /* 前端 CreateDir view 使用 */
    /// <summary>
    /// 系統目錄資料表 tbmDIRECTORIES
    /// </summary>
    [MetadataType(typeof(tbmDIRECTORIESMetadata))]
    public partial class tbmDIRECTORIES
    {
        /// <summary>
        /// 初始
        /// </summary>
        public tbmDIRECTORIES()
        {
            this.fnDIR_ID = 0;
            fsNAME = string.Empty;
            fnPARENT_ID = 0;
            fsDESCRIPTION = string.Empty;
            fsDIRTYPE = string.Empty;
            fnORDER = 99;
            fnTEMP_ID_SUBJECT = 0;
            fnTEMP_ID_VIDEO = 0;
            fnTEMP_ID_AUDIO = 0;
            fnTEMP_ID_PHOTO = 0;
            fnTEMP_ID_DOC = 0;
            fsADMIN_GROUP = string.Empty;
            fsADMIN_USER = string.Empty;
            fsSHOWTYPE = string.Empty;
            fdCREATED_DATE = DateTime.Now;
            fsCREATED_BY = string.Empty;
            fdUPDATED_DATE = null;
            fsUPDATED_BY = string.Empty;
            //
            IsQueue = false;
        }
        
        /// <summary>
        /// Procedure[spGET_DIRECTORIES] 取得的資料轉存至 [tbmDIRECTORIES]
        /// </summary>
        /// <param name="m"></param>
        public tbmDIRECTORIES(spGET_DIRECTORIES_Result m)
        {
            this.fnDIR_ID = m.fnDIR_ID;
            fsNAME = m.fsNAME ?? string.Empty;
            fnPARENT_ID = m.fnPARENT_ID;
            fsDESCRIPTION = m.fsDESCRIPTION ?? string.Empty;
            fsDIRTYPE = m.fsDIRTYPE ?? string.Empty;
            fnORDER = m.fnORDER;
            fnTEMP_ID_SUBJECT = m.fnTEMP_ID_SUBJECT;
            fnTEMP_ID_VIDEO = m.fnTEMP_ID_VIDEO;
            fnTEMP_ID_AUDIO = m.fnTEMP_ID_AUDIO;
            fnTEMP_ID_PHOTO = m.fnTEMP_ID_PHOTO;
            fnTEMP_ID_DOC = m.fnTEMP_ID_DOC;
            fsADMIN_GROUP = m.fsADMIN_GROUP ?? string.Empty;
            fsADMIN_USER = m.fsADMIN_USER ?? string.Empty;
            fsSHOWTYPE = m.fsSHOWTYPE ?? string.Empty;
            fdCREATED_DATE = m.fdCREATED_DATE;
            fsCREATED_BY = m.fsCREATED_BY ?? string.Empty;
            fdUPDATED_DATE = m.fdUPDATED_DATE;
            fsUPDATED_BY = m.fsUPDATED_BY ?? string.Empty;
            //
            IsQueue = m.fsDIRTYPE == "Q" ? true : false;
        }
        
        #region 額外資訊欄位
        /// <summary>
        /// 是否為末端節點，可新增主題
        /// </summary>
        public bool IsQueue { get; set; }
        #endregion

        /* 前端view: _CreateDir.cshtml 會使用 tbmDIRECTORIES Meatadata */
        /// <summary>
        /// 系統目錄資料表 tbmDIRECTORIES Meatadata
        /// </summary>
        public class tbmDIRECTORIESMetadata
        {
            /// <summary>
            /// 目錄編號
            /// </summary>
            [Display(Name = "目錄編號")]
            public long fnDIR_ID { get; set; }

            [Required]
            [Display(Name = "目錄標題名稱")]
            public string fsNAME { get; set; }

            [Display(Name = "父節點")]
            public long fnPARENT_ID { get; set; }

            [Display(Name = "目錄描述")]
            public string fsDESCRIPTION { get; set; }

            /// <summary>
            /// 目錄類型: Q=末端節點，可新增主題
            /// </summary>
            [Display(Name = "目錄內容類型")]
            public string fsDIRTYPE { get; set; }

            [Display(Name = "顯示順序")]
            public int fnORDER { get; set; }

            [Display(Name = "主題檔樣板編號")]
            public int fnTEMP_ID_SUBJECT { get; set; }

            [Display(Name = "影片檔樣板編號")]
            public int fnTEMP_ID_VIDEO { get; set; }

            [Display(Name = "聲音檔樣板編號")]
            public int fnTEMP_ID_AUDIO { get; set; }

            [Display(Name = "圖片檔樣板編號")]
            public int fnTEMP_ID_PHOTO { get; set; }

            [Display(Name = "文件檔樣板編號")]
            public int fnTEMP_ID_DOC { get; set; }

            [Display(Name = "目錄管理群組")]
            public string fsADMIN_GROUP { get; set; }

            [Display(Name = "目錄管理使用者")]
            public string fsADMIN_USER { get; set; }

            /// <summary>
            /// fsCODE_ID=DIR002(目錄開放類型)
            /// </summary>
            [Display(Name = "目錄開放類型")]
            public string fsSHOWTYPE { get; set; }

            [Display(Name = "建立時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public System.DateTime fdCREATED_DATE { get; set; }

            [Display(Name = "建立帳號")]
            public string fsCREATED_BY { get; set; }

            [Display(Name = "最後異動時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }

            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }
        }
    }
}
