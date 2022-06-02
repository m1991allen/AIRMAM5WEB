using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 同義詞 spGET_SYNONYMS 回覆model
    /// </summary>
    [MetadataType(typeof(spGET_SYNONYMS_ResultMetadata))]
    public partial class spGET_SYNONYMS_Result
    {
        /// <summary>
        /// 原始資料:同義詞詞彙
        /// </summary>
        public string OrigSynonyms { get; set; }

        /// <summary>
        /// 同義詞 spGET_SYNONYMS 回覆model
        /// </summary>
        public class spGET_SYNONYMS_ResultMetadata
        {
            [Display(Name = "ID")]
            public long fnINDEX_ID { get; set; }
            /// <summary>
            /// 同義詞詞彙
            /// </summary>
            [Display(Name = "同義詞詞彙")]
            [Required]
            public string fsTEXT_LIST { get; set; }
            /// <summary>
            /// 類別
            /// </summary>
            [Display(Name = "類別")]
            public string fsTYPE { get; set; }
            /// <summary>
            /// 類別 fsCODEID= SYNO_TYPE
            /// </summary>
            public string fsTYPE_NAME { get; set; }

            [Display(Name = "備註")]
            public string fsNOTE { get; set; }

            [Display(Name = "建立時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
            public System.DateTime fdCREATED_DATE { get; set; }
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
        }
    }
}
