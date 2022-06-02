
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 依 系統目錄fnDIR_ID 取 DIRECTORIES主檔資料&其主題檔 (影/音/圖/文)數量統計 結果
    /// </summary>
    [MetadataType(typeof(spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID_ResultMetadata))]
    public partial class spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID_Result
    {
        /// <summary>
        /// 依 系統目錄fnDIR_ID 取 DIRECTORIES主檔資料&其主題檔 (影/音/圖/文)數量統計 結果 -Metadata
        /// </summary>
        public class spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID_ResultMetadata
        {
            [Display(Name ="主題檔案編號")]
            public string fsSUBJ_ID { get; set; }
            [Display(Name = "標題")]
            public string fsTITLE { get; set; }
            [Display(Name = "描述")]
            public string fsDESCRIPTION { get; set; }
            /// <summary>
            /// 影片數量
            /// </summary>
            [Display(Name = "影")]
            public int? C_nVideo { get; set; }
            /// <summary>
            /// 聲音數量
            /// </summary>
            [Display(Name = "音")]
            public int? C_nAudio { get; set; }
            /// <summary>
            /// 圖片數量
            /// </summary>
            [Display(Name = "圖")]
            public int? C_nPhoto { get; set; }
            /// <summary>
            /// 文件數量
            /// </summary>
            [Display(Name = "文")]
            public int? C_nDocument { get; set; }
        }
    }
}
