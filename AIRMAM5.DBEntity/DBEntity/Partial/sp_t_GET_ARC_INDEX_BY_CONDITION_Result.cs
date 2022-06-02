using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /*20200925_調整_已無前端view 直接使用 */
    /// <summary>
    /// 依照查詢條件取出t_tbmARC_INDEX主檔資料 結果
    /// </summary>
    [MetadataType(typeof(sp_t_GET_ARC_INDEX_BY_CONDITION_ResultMetadata))]
    public partial class sp_t_GET_ARC_INDEX_BY_CONDITION_Result
    {
        /// <summary>
        /// 刪除紀錄功能-刪除檔案/還原檔案 viewmodel
        /// </summary>
        public class sp_t_GET_ARC_INDEX_BY_CONDITION_ResultMetadata
        {
            /// <summary>
            /// 編號
            /// </summary>
            [Display(Name = "編號")]
            public long fnINDEX_ID { get; set; }
            /// <summary>
            /// 檔案編號
            /// </summary>
            [Display(Name = "檔案編號")]
            public string fsFILE_NO { get; set; }
            /// <summary>
            /// 類別
            /// </summary>
            [Display(Name = "類別")]
            public string fsTYPE { get; set; }
            /// <summary>
            /// 刪除原因
            /// </summary>
            [Display(Name = "刪除原因")]
            public string fsREASON { get; set; }
            /// <summary>
            /// 狀態
            /// </summary>
            [Display(Name = "狀態")]
            public string fsSTATUS { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string C_sSTATUS { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string C_sTYPE { get; set; }
            /// <summary>
            /// 標題
            /// </summary>
            [Display(Name = "標題")]
            public string C_sTITLE { get; set; }
            /// <summary>
            /// 建立時間
            /// </summary>
            [Display(Name = "刪除時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", NullDisplayText = "", ApplyFormatInEditMode = true)]
            public DateTime fdCREATED_DATE { get; set; }
            /// <summary>
            /// 建立者
            /// </summary>
            [Display(Name = "刪除者")]
            public string fsCREATED_BY { get; set; }
            /// <summary>
            /// 最後異動時間
            /// </summary>
            [Display(Name = "清空/還原時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", NullDisplayText = "", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }
            /// <summary>
            /// 最後異動帳號
            /// </summary>
            [Display(Name = "修改人員")]
            public string fsUPDATED_BY { get; set; }
            /// <summary>
            /// 建立者 名稱
            /// </summary>
            [Display(Name = "新增人員")]
            public string fsCREATED_BY_NAME { get; set; }
            /// <summary>
            /// 異動者 名稱
            /// </summary>
            [Display(Name = "新增時間")]
            public string fsUPDATED_BY_NAME { get; set; }
        }
    }
}
