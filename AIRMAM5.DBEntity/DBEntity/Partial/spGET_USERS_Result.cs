using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks> UserDetailViewModel 參考使用。 </remarks>
    [MetadataType(typeof(spGET_USERS_ResultMetadata))]
    public partial class spGET_USERS_Result
    {
        public class spGET_USERS_ResultMetadata
        {
            /// <summary>
            /// 使用者識別Id
            /// </summary>
            [Display(Name = "使用者編號")]
            public string fsUSER_ID { get; set; }

            /// <summary>
            /// 使用者帳號
            /// </summary>
            [Display(Name = "系統帳號")]
            [Required]
            public string fsLOGIN_ID { get; set; }

            /// <summary>
            /// PasswordHash
            /// </summary>
            [Display(Name = "密碼")]
            public string fsPASSWORD { get; set; }

            /// <summary>
            /// 使用者姓名
            /// </summary>
            [Display(Name = "顯示名稱")]
            public string fsNAME { get; set; }

            [Display(Name = "英文名稱")]
            public string fsENAME { get; set; }

            [Display(Name = "職稱")]
            public string fsTITLE { get; set; }
            /// <summary>
            /// 隸屬單位
            /// </summary>
            [Display(Name = "隸屬單位")]
            public string fsDEPT_ID { get; set; }

            [DataType(DataType.EmailAddress)]
            [Display(Name = "電子郵件")]
            public string fsEMAIL { get; set; }

            [Display(Name = "連絡電話")]
            public string fsPHONE { get; set; }

            [Display(Name = "描述/備註")]
            public string fsDESCRIPTION { get; set; }
            /// <summary>
            /// 檔案機密權限
            /// </summary>
            [Display(Name = "檔案機密權限")]
            //[Required]
            public string fsFILE_SECRET { get; set; }

            /// <summary>
            /// 預設調用路徑
            /// </summary>
            [Display(Name = "預設調用路徑")]
            public string fsBOOKING_TARGET_PATH { get; set; }

            /// <summary>
            /// 帳號狀態: 1啟用 / 0不啟用
            /// </summary>
            [Display(Name = "帳號狀態")]
            public bool? fsIS_ACTIVE { get; set; }

            /// <summary>
            /// 電子郵件驗證狀態 true/ false/ null
            /// </summary>
            [Display(Name = "郵件驗證狀態")]
            public bool? fsEmailConfirmed { get; set; }

            [Display(Name = "建立時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime? fdCREATED_DATE { get; set; }

            [Display(Name = "建立帳號")]
            public string fsCREATED_BY { get; set; }

            [Display(Name = "最後異動時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }

            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }

            [Display(Name = "隸屬單位")]
            public string C_sDEPTNAME { get; set; }

            /// <summary>
            /// 群組角色(多筆;分隔)
            /// </summary>
            [Display(Name = "群組角色")]
            //[Required]
            public string fsGROUPs { get; set; }

            /// <summary>
            /// 是否為系統管理嗎 Y / N
            /// </summary>
            [Display(Name = "系統管理員")]
            public string C_sIS_ADMINS { get; set; }
        }
    }
}
