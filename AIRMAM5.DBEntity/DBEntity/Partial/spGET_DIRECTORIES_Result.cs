using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 
    /// </summary>
    [MetadataType(typeof(spGET_DIRECTORIES_ResultMetadata))]
    public partial class spGET_DIRECTORIES_Result
    {
        /// <summary>
        /// 系統目錄維護功能畫面使用
        /// </summary>
        public class spGET_DIRECTORIES_ResultMetadata
        {
            /// <summary>
            /// 目錄編號
            /// </summary>
            [Display(Name = "目錄編號")]
            public long fnDIR_ID { get; set; }
            /// <summary>
            /// 目錄名稱
            /// </summary>
            [Display(Name = "目錄名稱")]
            public string fsNAME { get; set; }
            /// <summary>
            /// 母節點
            /// </summary>
            public long fnPARENT_ID { get; set; }

            /// <summary>
            /// 目錄描述
            /// </summary>
            [Display(Name = "目錄描述")]
            public string fsDESCRIPTION { get; set; }

            /// <summary>
            /// 目錄類型 (Q:末端節點，可新增主題)
            /// </summary>
            [Display(Name = "目錄類型")]
            public string fsDIRTYPE { get; set; }

            /// <summary>
            /// 目錄顯示順序
            /// </summary>
            [Display(Name = "目錄顯示順序")]
            public int fnORDER { get; set; }

            /// <summary>
            /// 主題檔樣板編號
            /// </summary>
            [Display(Name = "主題檔樣板")]
            public int fnTEMP_ID_SUBJECT { get; set; }
            /// <summary>
            /// 影片檔樣板編號
            /// </summary>
            [Display(Name = "影片檔樣板")]
            public int fnTEMP_ID_VIDEO { get; set; }
            /// <summary>
            /// 聲音檔樣板編號
            /// </summary>
            [Display(Name = "聲音檔樣板")]
            public int fnTEMP_ID_AUDIO { get; set; }
            /// <summary>
            /// 圖片檔樣板編號
            /// </summary>
            [Display(Name = "圖片檔樣板")]
            public int fnTEMP_ID_PHOTO { get; set; }
            /// <summary>
            /// 文件檔樣板編號
            /// </summary>
            [Display(Name = "文件檔樣板")]
            public int fnTEMP_ID_DOC { get; set; }
            /// <summary>
            /// 目錄管理群組
            /// </summary>
            [Display(Name = "目錄管理群組")]
            public string fsADMIN_GROUP { get; set; }
            /// <summary>
            /// 目錄管理帳號
            /// </summary>
            [Display(Name = "目錄管理帳號")]
            public string fsADMIN_USER { get; set; }
            /// <summary>
            /// 目錄開放類型
            /// </summary>
            [Display(Name = "目錄開放類型")]
            public string fsSHOWTYPE { get; set; }

            [Display(Name = "建立時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime fdCREATED_DATE { get; set; }

            [Display(Name = "建立帳號")]
            public string fsCREATED_BY { get; set; }

            [Display(Name = "最後異動時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }

            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }

            public string fsCREATED_BY_NAME { get; set; }

            public string fsUPDATED_BY_NAME { get; set; }

            /// <summary>
            /// 主題檔欄位樣版 名稱
            /// </summary>
            public string C_SUBJECT_NAME { get; set; }
            /// <summary>
            /// 影片檔欄位樣版 名稱
            /// </summary>
            public string C_VIDEO_NAME { get; set; }
            /// <summary>
            /// 聲音檔欄位樣版 名稱
            /// </summary>
            public string C_AUDIO_NAME { get; set; }
            /// <summary>
            /// 圖片檔欄位樣版 名稱
            /// </summary>
            public string C_PHOTO_NAME { get; set; }
            /// <summary>
            /// 文件檔欄位樣版 名稱
            /// </summary>
            public string C_DOC_NAME { get; set; }
            /// <summary>
            /// 目錄管理群組 
            /// </summary>
            public string C_sGROUP_NAME_LIST { get; set; }
            /// <summary>
            /// 目錄管理帳號
            /// </summary>
            public string C_sUSER_NAME_LIST { get; set; }
            /// <summary>
            /// 目錄路徑
            /// </summary>
            public string C_sDIR_PATH { get; set; }
        }
    }
}
