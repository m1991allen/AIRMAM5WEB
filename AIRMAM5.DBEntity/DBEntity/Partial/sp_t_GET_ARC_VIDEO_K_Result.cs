using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 依照查詢條件取出t_tbmARC_K主檔資料 結果
    /// </summary>
    [MetadataType(typeof(sp_t_GET_ARC_VIDEO_K_ResultMetadata))]
    public partial class sp_t_GET_ARC_VIDEO_K_Result
    {
        /// <summary>
        /// 依照查詢條件取出t_tbmARC_K主檔資料 結果
        /// </summary>
        public class sp_t_GET_ARC_VIDEO_K_ResultMetadata
        {
            /// <summary>
            /// 檔案編號
            /// </summary>
            [Display(Name = "檔案編號")]
            public string fsFILE_NO { get; set; }
            /// <summary>
            /// 標題
            /// </summary>
            [Display(Name = "標題")]
            public string fsTITLE { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            [Display(Name = "描述")]
            public string fsDESCRIPTION { get; set; }
            /// <summary>
            /// 檔案路徑
            /// </summary>
            [Display(Name = "檔案路徑")]
            public string fsFILE_PATH { get; set; }
            /// <summary>
            /// 檔案大小
            /// </summary>
            [Display(Name = "檔案大小")]
            public string fsFILE_SIZE { get; set; }
            /// <summary>
            /// 副檔名
            /// </summary>
            [Display(Name = "副檔名")]
            public string fsFILE_TYPE { get; set; }
            /// <summary>
            /// 是否為代表圖
            /// </summary>
            [Display(Name = "是否為代表圖")]
            public string fcHEAD_FRAME { get; set; }

            /// <summary>
            /// 建立時間
            /// </summary>
            [Display(Name = "建立時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime fdCREATED_DATE { get; set; }
            /// <summary>
            /// 建立帳號
            /// </summary>
            [Display(Name = "建立帳號")]
            public string fsCREATED_BY { get; set; }
            /// <summary>
            /// 最後異動時間
            /// </summary>
            [Display(Name = "最後異動時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }
            /// <summary>
            /// 最後異動帳號
            /// </summary>
            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }

            /// <summary>
            /// 關鍵影格時間
            /// </summary>
            [Display(Name = "關鍵影格時間")]
            public string fsTIME { get; set; }
            /// <summary>
            /// 檔案完整路徑
            /// </summary>
            [Display(Name = "檔案完整路徑")]
            public string C_sIMAGE_URL { get; set; }
            /// <summary>
            /// 檔案資訊
            /// </summary>
            [Display(Name = "檔案資訊")]
            public string C_sFILE_INFO { get; set; }
            /// <summary>
            /// 影片長度
            /// </summary>
            [Display(Name = "影片長度")]
            public Nullable<decimal> C_sVIDEO_MAX_TIME { get; set; }
        }
        
    }
}
