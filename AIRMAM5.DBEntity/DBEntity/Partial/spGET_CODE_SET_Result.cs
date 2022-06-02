using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    [MetadataType(typeof(spGET_CODE_SET_ResultMetadata))]
    public partial class spGET_CODE_SET_Result
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        [Display(Name = "可選")]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// spGET_CODE_SET 代碼主檔資料
        /// </summary>
        public class spGET_CODE_SET_ResultMetadata
        {
            /// <summary>
            /// 代碼項目(主檔:代碼Id)
            /// </summary>
            [Display(Name = "代碼編號")]
            public string fsCODE_ID { get; set; }
            /// <summary>
            /// 代碼項目名稱
            /// </summary>
            [Display(Name = "名稱")]
            public string fsTITLE { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string fsTBCOL { get; set; }

            /// <summary>
            /// 備註
            /// </summary>
            [Display(Name = "備註")]
            public string fsNOTE { get; set; }

            /// <summary>
            /// 是否有效
            /// </summary>
            [Display(Name = "可選")]
            public string fsIS_ENABLED { get; set; }

            /// <summary>
            /// 分類
            /// </summary>
            [Display(Name = "分類")]
            public string fsTYPE { get; set; }

            /// <summary>
            /// 是否有效(中文)
            /// </summary>
            public string C_sIS_ENABLED { get; set; }

            [Display(Name = "建立時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime fdCREATED_DATE { get; set; }

            [Display(Name = "建立帳號")]
            public string fsCREATED_BY { get; set; }

            [Display(Name = "最後異動時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }

            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }

            /// <summary>
            /// 代碼數量 (明細檔)
            /// </summary>
            [Display(Name = "代碼數量")]
            public int? C_nCNT_CODE { get; set; }

            /// <summary>
            /// 建立者角色群組
            /// </summary>
            public string fsCREATED_BY_NAME { get; set; }

            /// <summary>
            /// 最後異動者角色群組
            /// </summary>
            public string fsUPDATED_BY_NAME { get; set; }
        }
    }
}
