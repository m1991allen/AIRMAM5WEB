using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 取出TEMPLATE_FIELDS 主檔 資料 : spGET_TEMPLATE_FIELDS_BY_TEMP_ID
    /// </summary>
    [MetadataType(typeof(spGET_TEMPLATE_FIELDS_BY_TEMP_ID_ResultMetadata))]
    public partial class spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result
    {
        /// <summary>
        /// 取出TEMPLATE_FIELDS 主檔 資料 : spGET_TEMPLATE_FIELDS_BY_TEMP_ID
        /// </summary>
        public class spGET_TEMPLATE_FIELDS_BY_TEMP_ID_ResultMetadata
        {
            [Display(Name = "樣板編號")]
            public int fnTEMP_ID { get; set; }

            [Display(Name = "使用欄位")]
            public string fsFIELD { get; set; }

            [Display(Name = "欄位名稱")]
            public string fsFIELD_NAME { get; set; }

            [Display(Name = "內容上限")]
            public Nullable<int> fnFIELD_LENGTH { get; set; }

            [Display(Name = "備註")]
            public string fsDESCRIPTION { get; set; }

            [Display(Name = "排序")]
            public int fnORDER { get; set; }
            
            public int? fnCTRL_WIDTH { get; set; }
            /// <summary>
            /// 是否多行
            /// </summary>
            public string fsMULTILINE { get; set; }

            /// <summary>
            /// 必要 isNullable
            /// </summary>
            [Display(Name = "必要")]
            public string fsISNULLABLE { get; set; }
            /// <summary>
            /// 代碼編號
            /// </summary>
            public string fsCODE_ID { get; set; }
            /// <summary>
            /// 單選或多選 : 0:多選、1:單選
            /// </summary>
            public Nullable<int> fnCODE_CNT { get; set; }
            /// <summary>
            /// 控制項類型
            /// </summary>
            public string fsCODE_CTRL { get; set; }
            /// <summary>
            /// 是否要列為進階搜尋
            /// </summary>
            [Display(Name = "進階檢索")]
            public string fsIS_SEARCH { get; set; }
            /// <summary>
            /// 預設值
            /// </summary>
            [Display(Name = "預設值")]
            public string fsDEFAULT { get; set; }
            /// <summary>
            /// 內容型別: tbzCODE.TEMP002
            /// </summary>
            [Display(Name = "內容型別")]
            public string fsFIELD_TYPE { get; set; }

            public string C_sDEFAULT { get; set; }

            public int? C_sLIMIT { get; set; }
            /// <summary>
            /// 內容型別
            /// </summary>
            public string C_sFIELD_TYPE_NAME { get; set; }

            public string C_sCODE_SET_NAME { get; set; }

            [Display(Name = "建立時間")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime fdCREATED_DATE { get; set; }
            [Display(Name = "建立人員")]
            public string fsCREATED_BY { get; set; }

            [Display(Name = "最後異動時間")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }
            [Display(Name = "最後異動人員")]
            public string fsUPDATED_BY { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string C_VALUE { get; set; }
            public string fsCREATED_BY_NAME { get; set; }
            public string fsUPDATED_BY_NAME { get; set; }
        }
    }
}
