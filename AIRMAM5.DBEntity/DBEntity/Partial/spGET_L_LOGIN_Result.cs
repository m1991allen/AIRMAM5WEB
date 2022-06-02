using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// spGET_L_LOGIN Procedure回覆結果
    /// </summary>
    [MetadataType(typeof(spGET_L_LOGIN_ResultMetadata))]
    public partial class spGET_L_LOGIN_Result
    {
        /// <summary>
        /// 登入紀錄查詢- Metadata欄位
        /// </summary>
        public class spGET_L_LOGIN_ResultMetadata
        {
            /// <summary>
            /// LginId
            /// </summary>
            [Display(Name = "編號")]
            public long fnLOGIN_ID { get; set; }

            /// <summary>
            /// 使用者帳號
            /// </summary>
            [Display(Name = "登入帳號")]
            public string fsLOGIN_ID { get; set; }

            /// <summary>
            /// 登入時間
            /// </summary>
            [Display(Name = "登入時間")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", NullDisplayText = "", ApplyFormatInEditMode = true)]
            public DateTime fdSTIME { get; set; }

            /// <summary>
            /// 登出時間
            /// </summary>
            [Display(Name = "登出時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", NullDisplayText = "", ApplyFormatInEditMode = true)]
            public DateTime? fdETIME { get; set; }

            /// <summary>
            /// 備註
            /// </summary>
            [Display(Name = "備註")]
            public string fsNOTE { get; set; }

            [Display(Name = "新增時間")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", NullDisplayText = "", ApplyFormatInEditMode = true)]
            public DateTime fdCREATED_DATE { get; set; }

            [Display(Name = "新增人員")]
            public string fsCREATED_BY { get; set; }

            [Display(Name = "修改時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", NullDisplayText = "", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }

            [Display(Name = "修改人員")]
            public string fsUPDATED_BY { get; set; }

            public string fsCREATED_BY_NAME { get; set; }

            public string fsUPDATED_BY_NAME { get; set; }
        }
    }

    /// <summary>
    /// 查詢作業/登入紀錄 Index 資料
    /// </summary>
    public class spGET_L_LOGIN_ResultViewModel : spGET_L_LOGIN_Result
    {
        public spGET_L_LOGIN_ResultViewModel() { }

        public spGET_L_LOGIN_ResultViewModel(spGET_L_LOGIN_Result m)
        {
            fnLOGIN_ID = m.fnLOGIN_ID;
            fsLOGIN_ID = m.fsLOGIN_ID;
            fdSTIME = m.fdSTIME;
            fdETIME = m.fdETIME == null ? m.fdETIME : (((DateTime)m.fdETIME).Year == 1900 ? null : m.fdETIME);
            fsNOTE = m.fsNOTE;
            fdCREATED_DATE = m.fdCREATED_DATE;
            fsCREATED_BY = m.fsCREATED_BY;
            fdUPDATED_DATE = m.fdUPDATED_DATE;
            fsUPDATED_BY = m.fsUPDATED_BY;
            fsCREATED_BY_NAME = m.fsCREATED_BY_NAME;
            fsUPDATED_BY_NAME = m.fsUPDATED_BY_NAME;
            fsLOGIN_NAME = m.fsLOGIN_NAME;      //帳號顯示名稱
        }

        /// <summary>
        /// 登入使用時間 , 00D:00H:35m:25s
        /// </summary>
        [Display(Name = "登入使用時間")]
        public string UsageTime { get; set; } = string.Empty;
    }
}
