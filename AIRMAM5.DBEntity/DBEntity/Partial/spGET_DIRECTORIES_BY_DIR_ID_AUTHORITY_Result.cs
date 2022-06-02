using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 單一目錄/節點{G群組/U使用者} 權限資料 結果 spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY
    /// </summary>
    [MetadataType(typeof(spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_ResultMetadata))]
    public partial class spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result
    {
        public spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result()
        {
            fnDIR_ID = -1;
            DATATYPE = "G";
            GROUP_ID = string.Empty;
            GROUP_NAME = string.Empty;
            USER_ID = string.Empty;
            LOGIN_ID = string.Empty;
            USER_NAME = string.Empty;
            C_ADMIN = string.Empty;
            C_USER = string.Empty;
            LIMIT_SUBJECT = string.Empty;
            LIMIT_VIDEO = string.Empty;
            LIMIT_AUDIO = string.Empty;
            LIMIT_PHOTO = string.Empty;
            LIMIT_DOC = string.Empty;
            fnPARENT_ID = -1;
            C_sDIR_PATH = string.Empty;
        }
        /// <summary>
        /// 單一目錄/節點{G群組/U使用者} 權限資料 結果 spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY
        /// </summary>
        public class spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_ResultMetadata
        {
            /// <summary>
            /// 欄位類別 : G群組/U使用者
            /// </summary>
            [Display(Name = "角色群組")]
            public string DATATYPE { get; set; }
            /// <summary>
            /// 角色群組ID
            /// </summary>
            public string GROUP_ID { get; set; }
            /// <summary>
            /// 角色群組 名稱
            /// </summary>
            [Display(Name = "角色群組")]
            public string GROUP_NAME { get; set; }
            /// <summary>
            /// 使用者ID
            /// </summary>
            [Display(Name = "使用者")]
            public string USER_ID { get; set; }
            /// <summary>
            /// 使用者帳號
            /// </summary>
            [Display(Name = "使用者帳號")]
            public string LOGIN_ID { get; set; }
            /// <summary>
            /// 使用者顯示名稱
            /// </summary>
            [Display(Name = "顯示名稱")]
            public string USER_NAME { get; set; }
            /// <summary>
            /// 目錄管理權限 Y=直接 , y=繼承
            /// </summary>
            [Display(Name = "目錄管理權限")]
            public string C_ADMIN { get; set; }
            /// <summary>
            /// 主題/檔案權限 Y=直接 , y=繼承
            /// </summary>
            [Display(Name = "主題/檔案權限")]
            public string C_USER { get; set; }

            /// <summary>
            /// 主題 權限內容
            /// </summary>
            [Display(Name = "主題")]
            [Required]
            public string LIMIT_SUBJECT { get; set; }
            /// <summary>
            /// 影片 權限內容
            /// </summary>
            [Display(Name = "影片")]
            [Required]
            public string LIMIT_VIDEO { get; set; }
            /// <summary>
            /// 聲音 權限內容
            /// </summary>
            [Display(Name = "聲音")]
            [Required]
            public string LIMIT_AUDIO { get; set; }
            /// <summary>
            /// 圖片 權限內容
            /// </summary>
            [Display(Name = "圖片")]
            [Required]
            public string LIMIT_PHOTO { get; set; }
            /// <summary>
            /// 文件 權限內容
            /// </summary>
            [Display(Name = "文件")]
            [Required]
            public string LIMIT_DOC { get; set; }

            public long? fnPARENT_ID { get; set; }

            public string C_sDIR_PATH { get; set; }
            /// <summary>
            /// 系統目錄編號
            /// </summary>
            public long? fnDIR_ID { get; set; }
        }
    }
}
