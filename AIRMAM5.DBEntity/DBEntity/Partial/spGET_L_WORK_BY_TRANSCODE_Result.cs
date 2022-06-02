using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 取出上傳轉檔主檔資料 Procedure查詢結果
    /// </summary>
    [MetadataType(typeof(spGET_L_WORK_BY_TRANSCODE_ResultMetadata))]
    public partial class spGET_L_WORK_BY_TRANSCODE_Result
    {
        /// <summary>
        /// 取出上傳轉檔主檔資料 Procedure查詢結果
        /// </summary>
        public class spGET_L_WORK_BY_TRANSCODE_ResultMetadata
        {
            /// <summary>
            /// 轉檔編號
            /// </summary>
            [Display(Name = "編號")]
            public long fnWORK_ID { get; set; }
            /// <summary>
            /// 工作/轉檔類別 , dbo.tbzCODE.fsCODE_ID = WORK001
            /// </summary>
            public string fsTYPE { get; set; }
            /// <summary>
            /// 工作/轉檔狀態 (手動上傳) , tbzCODE.fsCODE_ID = WORK_TC
            /// </summary>
            [Display(Name = "狀態")]
            public string fsSTATUS { get; set; }
            /// <summary>
            /// 進度 %
            /// </summary>
            [Display(Name = "轉檔進度")]
            public string fsPROGRESS { get; set; }
            /// <summary>
            /// 優先順序
            /// </summary>
            [Display(Name = "優先順序")]
            [Range(1,9)]
            public string fsPRIORITY { get; set; }
            /// <summary>
            /// 開始轉檔時間
            /// </summary>
            [Display(Name = "開始轉檔時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public System.DateTime fdSTIME { get; set; }
            /// <summary>
            /// 結束轉檔時間
            /// </summary>
            [Display(Name = "結束轉檔時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public DateTime? fdETIME { get; set; }
            /// <summary>
            /// 轉檔結果
            /// </summary>
            [Display(Name = "轉檔結果")]
            public string fsRESULT { get; set; }
            /// <summary>
            /// 備註
            /// </summary>
            [Display(Name = "備註")]
            public string fsNOTE { get; set; }
            /// <summary>
            /// 建立時間
            /// </summary>
            [Display(Name = "建立時間")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public System.DateTime fdCREATED_DATE { get; set; }
            /// <summary>
            /// 建立者
            /// </summary>
            [Display(Name = "建立者")]
            public string fsCREATED_BY { get; set; }
            /// <summary>
            /// 最後異動時間
            /// </summary>
            [Display(Name = "異動時間")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public DateTime? fdUPDATED_DATE { get; set; }
            /// <summary>
            /// 最後異動者
            /// </summary>
            [Display(Name = "異動者")]
            public string fsUPDATED_BY { get; set; }
            /// <summary>
            /// 工作/轉檔類別 中文
            /// </summary>
            public string C_sTYPENAME { get; set; }
            /// <summary>
            /// 轉檔狀態
            /// </summary>
            [Display(Name = "轉檔狀態")]
            public string C_sSTATUSNAME { get; set; }
            /// <summary>
            /// 轉檔參數
            /// </summary>
            [Display(Name = "轉檔參數")]
            public string fsPARAMETERS { get; set; }
            /// <summary>
            /// 檔案資訊
            /// </summary>
            [Display(Name = "檔案資訊")]
            public string C_sFILE_INFO { get; set; }
            /// <summary>
            /// fnGROUP_ID
            /// </summary>
            public long fnGROUP_ID { get; set; }
            /// <summary>
            /// C_ITEM_TYPE
            /// </summary>
            public string C_ITEM_TYPE { get; set; }
            /// <summary>
            /// C_ITEM_ID
            /// </summary>
            public string C_ITEM_ID { get; set; }
            /// <summary>
            /// 建立者 名稱
            /// </summary>
            public string fsCREATED_BY_NAME { get; set; }
            /// <summary>
            /// 最後異動者 名稱
            /// </summary>
            public string fsUPDATED_BY_NAME { get; set; }
        }
    }
}
