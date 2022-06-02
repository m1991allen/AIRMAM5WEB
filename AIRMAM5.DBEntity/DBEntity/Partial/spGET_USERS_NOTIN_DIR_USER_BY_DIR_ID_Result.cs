using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 依照查詢條件取出尚未在目錄使用者資料 結果
    /// </summary>
    [MetadataType(typeof(spGET_USERS_NOTIN_DIR_USER_BY_DIR_ID_ResultMetadata))]
    public partial class spGET_USERS_NOTIN_DIR_USER_BY_DIR_ID_Result
    {
        /// <summary>
        /// 依照查詢條件取出尚未在目錄使用者資料 結果
        /// </summary>
        public class spGET_USERS_NOTIN_DIR_USER_BY_DIR_ID_ResultMetadata
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
            /// 使用者姓名
            /// </summary>
            [Display(Name = "姓 名")]
            public string fsNAME { get; set; }
        }
        
    }
}
