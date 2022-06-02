using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// spGET_L_LOGIN_ALIVE Procedure回覆結果
    /// </summary>
    [MetadataType(typeof(spGET_L_LOGIN_ALIVE_ResultMetadata))]
    public partial class spGET_L_LOGIN_ALIVE_Result
    {
        public class spGET_L_LOGIN_ALIVE_ResultMetadata
        {
            /// <summary>
            /// LginId
            /// </summary>
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
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public DateTime fdSTIME { get; set; }

            /// <summary>
            /// 登出時間
            /// </summary>
            [Display(Name = "登出時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public DateTime? fdETIME { get; set; }

            /// <summary>
            /// 備註
            /// </summary>
            [Display(Name = "備註")]
            public string fsNOTE { get; set; }

            [Display(Name = "建立時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public DateTime fdCREATED_DATE { get; set; }

            [Display(Name = "建立帳號")]
            public string fsCREATED_BY { get; set; }

            [Display(Name = "最後異動時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public DateTime? fdUPDATED_DATE { get; set; }

            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }

            /// <summary>
            /// 登入使用時間 , 00D:00H:35m:25s
            /// </summary>
            [Display(Name = "使用時間")]
            public string C_sOnlineTime { get; set; }
        }
    }
}
