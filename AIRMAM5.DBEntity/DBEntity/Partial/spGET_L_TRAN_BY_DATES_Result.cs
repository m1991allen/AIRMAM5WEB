using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 資料異動查詢結果 spGET_L_TRAN_BY_DATES
    /// </summary>
    [MetadataType(typeof(spGET_L_TRAN_BY_DATES_ResultMetadata))]
    public partial class spGET_L_TRAN_BY_DATES_Result
    {
        /// <summary>
        /// 資料異動查詢結果 spGET_L_TRAN_BY_DATES
        /// </summary>
        public class spGET_L_TRAN_BY_DATES_ResultMetadata
        {
            /// <summary>
            /// 編號
            /// </summary>
            [Display(Name = "編號")]
            public long fnID { get; set; }
            /// <summary>
            /// 資料表
            /// </summary>
            [Display(Name = "資料表")]
            public string fsTABLE { get; set; }
            /// <summary>
            /// 資料表名稱
            /// </summary>
            [Display(Name = "資料表名稱")]
            public string fsTABLE_NAME { get; set; }
            /// <summary>
            /// 檔案編號
            /// </summary>
            [Display(Name = "檔案編號")]
            public string fsFILE_NO { get; set; }
            /// <summary>
            /// 檔案標題
            /// </summary>
            [Display(Name = "檔案標題")]
            public string fsTITLE { get; set; }
            /// <summary>
            /// 異動時間
            /// </summary>
            [Display(Name = "異動時間")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public DateTime? fdACTION_DATE { get; set; }
            /// <summary>
            /// 異動帳號
            /// </summary>
            [Display(Name = "異動帳號")]
            public string fsACTION_BY { get; set; }
            /// <summary>
            /// 異動帳號角色群組
            /// </summary>
            public string fsACTION_BY_NAME { get; set; }
            /// <summary>
            /// 執行動作
            /// </summary>
            [Display(Name = "執行動作")]
            public string fsACTION { get; set; }
        }
    }
}
