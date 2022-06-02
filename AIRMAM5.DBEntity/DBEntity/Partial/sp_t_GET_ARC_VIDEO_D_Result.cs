using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 依照查詢條件取出t_tbmARC_D主檔資料 結果
    /// </summary>
    [MetadataType(typeof(sp_t_GET_ARC_VIDEO_D_ResultMetadata))]
    public partial class sp_t_GET_ARC_VIDEO_D_Result
    {
        /// <summary>
        /// 依照查詢條件取出t_tbmARC_D主檔資料 結果
        /// </summary>
        public class sp_t_GET_ARC_VIDEO_D_ResultMetadata
        {
            /// <summary>
            /// 檔案編號
            /// </summary>
            [Display(Name = "檔案編號")]
            public string fsFILE_NO { get; set; }
            /// <summary>
            /// 段落序號
            /// </summary>
            [Display(Name = "段落序號")]
            public int fnSEQ_NO { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            [Display(Name = "描述")]
            public string fsDESCRIPTION { get; set; }
            /// <summary>
            /// 段落開始時間
            /// </summary>
            [Display(Name = "段落開始時間")]
            public decimal fdBEG_TIME { get; set; }
            /// <summary>
            /// 段落結束時間
            /// </summary>
            [Display(Name = "段落結束時間")]
            public decimal fdEND_TIME { get; set; }
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
            /// 建立者
            /// </summary>
            [Display(Name = "建立者")]
            public string fsCREATED_BY_NAME { get; set; }
            /// <summary>
            /// 最後異動者
            /// </summary>
            [Display(Name = "最後異動者")]
            public string fsUPDATED_BY_NAME { get; set; }
        }
    }
}
