using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 依照SUBJ_ID取出ARC_VIDEO 入庫項目-影片檔主檔 資料
    /// </summary>
    [MetadataType(typeof(spGET_ARC_VIDEO_BY_SUBJ_ID_ResultMetadata))]
    public partial class spGET_ARC_VIDEO_BY_SUBJ_ID_Result
    {
        public class spGET_ARC_VIDEO_BY_SUBJ_ID_ResultMetadata
        {
            [Display(Name ="檔案編號")]
            public string fsFILE_NO { get; set; }
            [Display(Name = "標題")]
            public string fsTITLE { get; set; }
            [Display(Name = "描述")]
            public string fsDESCRIPTION { get; set; }
            /// <summary>
            /// 圖檔路徑
            /// </summary>
            [Display(Name = "圖檔路徑")]
            public string C_sIMAGE_URL { get; set; }
            /// <summary>
            /// 檔案路徑
            /// </summary>
            [Display(Name = "檔案路徑")]
            public string C_sFILE_URL_L { get; set; }

            /// <summary>
            /// 主題檔案路徑 _sSUBJ_PATH
            /// </summary>
            [Display(Name = "主題檔案路徑")]
            public string C_sSUBJ_PATH { get; set; }

            /// <summary>
            /// 關鍵影格數量 _sKEYFRAME_COUNT
            /// </summary>
            [Display(Name = "關鍵影格數量")]
            public Nullable<int> C_sKEYFRAME_COUNT { get; set; }

            /// <summary>
            /// 段落描述數量 _sSEGMENT_COUNT
            /// </summary>
            [Display(Name = "段落描述數量")]
            public Nullable<int> C_sSEGMENT_COUNT { get; set; }

            /// <summary>
            /// 檔案是否可置換 (值= Y/N)
            /// </summary>
            [Display(Name = "是否可置換")]
            public string C_sCHANGE { get; set; }
        }
    }
}
