using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 依照CREATE日期或LOG ID 取出L_LOG主檔資料 欄位
    /// </summary>
    [MetadataType(typeof(spGET_L_LOG_BY_LOGID_DATES_LOGINID_ResultMetadata))]
    public partial class spGET_L_LOG_BY_LOGID_DATES_LOGINID_Result
    {
        /// <summary>
        /// 系統操作紀錄內容- Metadata欄位
        /// </summary>
        public class spGET_L_LOG_BY_LOGID_DATES_LOGINID_ResultMetadata
        {
            /// <summary>
            /// 系統操作紀錄編號 fnlLOG_ID
            /// </summary>
            [Display(Name = "編號")]
            public long fnlLOG_ID { get; set; }
            
            /// <summary>
            /// 
            /// </summary>
            [Display(Name = "類別")]
            public string fsTYPE { get; set; }

            [Display(Name = "群組")]
            public string fsGROUP { get; set; }

            [Display(Name = "描述")]
            public string fsDESCRIPTION { get; set; }

            [Display(Name = "備註")]
            public string fsNOTE { get; set; }

            [Display(Name = "資料主鍵")]
            public string fsDATA_KEY { get; set; }

            [Display(Name = "新增時間")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public DateTime fdCREATED_DATE { get; set; }

            [Display(Name = "新增人員")]
            public string fsCREATED_BY { get; set; }

            public string fsCREATED_BY_NAME { get; set; }

            [Display(Name = "類別")]
            public string C_sTYPE_NAME { get; set; }

            [Display(Name = "群組")]
            public string C_sGROUP_NAME { get; set; }
        }
    }
}
