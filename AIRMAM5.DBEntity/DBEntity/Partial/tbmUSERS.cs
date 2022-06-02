using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 使用者資料表 tbmUSERS (dotnet原生= dbo.AspNetUsers)
    /// </summary>
    [MetadataType(typeof(tbmUSERSMetadata))]
    public partial class tbmUSERS
    {
        /// <summary>
        /// 所屬群組 (資料傳遞用)
        /// </summary>
        public string GroupIds { get; set; }

        //Marked_20200423_未被參考/// <summary>
        ///// 詳細內容/編輯 頁面資料 -> tbmUSERS
        ///// </summary>
        ///// <param name="m"></param>
        //public tbmUSERS(UserEditViewModel m)
        //{
        //    //fsUSER_ID = m.fsUSER_ID;
        //    //fsLOGIN_ID = m.fsLOGIN_ID;
        //    ////fsNAME = m.fsNAME;
        //    //fsENAME = m.fsENAME ?? string.Empty;
        //    //fsTITLE = m.fsTITLE;
        //    //fsDEPT_ID = m.fsDEPT_ID;
        //    fsDESCRIPTION = m.fsDESCRIPTION ?? string.Empty;
        //    fsFILE_SECRET = string.Join(";", m.FSecretList);
        //    fsBOOKING_TARGET_PATH = m.fsBOOKING_TARGET_PATH;
        //    //fsEMAIL = m.fsEMAIL ?? string.Empty;
        //    //fsIS_ACTIVE = m.fsIS_ACTIVE ?? true;
        //    fsPHONE = m.fsPHONE ?? string.Empty;
        //    GroupIds = string.Join(";", m.GroupList);
        //}

        /// <summary>
        /// 使用者 Metadata
        /// </summary>
        public class tbmUSERSMetadata
        {
            /// <summary>
            /// 使用者識別Id
            /// </summary>
            [Display(Name = "使用者編號")]
            public string fsUSER_ID { get; set; }
            /// <summary>
            /// 使用者帳號
            /// </summary>
            [Display(Name = "帳 號")]
            public string fsLOGIN_ID { get; set; }
            /// <summary>
            /// 使用者姓名/顯示名稱
            /// </summary>
            [Display(Name = "姓 名")]
            public string fsNAME { get; set; } = string.Empty;

            [Display(Name = "英文名")]
            public string fsENAME { get; set; } = string.Empty;

            [Display(Name = "職稱")]
            public string fsTITLE { get; set; } = string.Empty;
            /// <summary>
            /// 隸屬單位
            /// </summary>
            [Display(Name = "隸屬單位")]
            public string fsDEPT_ID { get; set; }

            [Display(Name = "描述/備註")]
            public string fsDESCRIPTION { get; set; } = string.Empty;

            /// <summary>
            /// 檔案機密權限
            /// </summary>
            [Display(Name = "檔案機密權限")]
            public string fsFILE_SECRET { get; set; }

            /// <summary>
            /// 預設調用路徑(如：\\SERVER\FOLDER)
            /// </summary>
            [Display(Name = "預設調用路徑")]
            public string fsBOOKING_TARGET_PATH { get; set; } = string.Empty;

            [Display(Name = "電子郵件")]
            public string fsEMAIL { get; set; }

            /// <summary>
            /// 帳號狀態: 啟用 / 不啟用
            /// </summary>
            [Display(Name = "帳號狀態")]
            public bool? fsIS_ACTIVE { get; set; }

            /// <summary>
            /// 電子郵件驗證
            /// </summary>
            public bool fsEmailConfirmed { get; set; }

            /// <summary>
            /// PasswordHash
            /// </summary>
            [Display(Name = "密碼")]
            public string fsPASSWORD { get; set; } = string.Empty;

            public string fsSecurityStamp { get; set; } = string.Empty;

            [Display(Name = "電話")]
            public string fsPHONE { get; set; } = string.Empty;
            /// <summary>
            /// 電話驗證
            /// </summary>
            public bool fbPhoneConfirmed { get; set; } = false;

            public bool fbTwoFactorEnabled { get; set; } = false;

            public DateTime? fdLockoutEndDateUtc { get; set; }

            /// <summary>
            /// 是否啟用自動鎖定
            /// </summary>
            public bool fbLockoutEnabled { get; set; }

            public int fnAccessFailedCount { get; set; } = 0;

            [Display(Name = "建立時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime? fdCREATED_DATE { get; set; }

            [Display(Name = "建立帳號")]
            public string fsCREATED_BY { get; set; }

            [Display(Name = "最後異動時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }

            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }
            
            public string Discriminator { get; set; }

            [JsonIgnore]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<tbmUSER_GROUP> tbmUSER_GROUP { get; set; }

            [JsonIgnore]    //Tip: 在特定導覽屬性上套用 [JsonIgnore] 屬性(Attribute)即可防止參考循環問題發生
            public virtual tbmUSER_EXTEND tbmUSER_EXTEND { get; set; }
        }
    }
}
