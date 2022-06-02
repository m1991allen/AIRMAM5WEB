using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 依照查詢條件取出使用者所屬群組資料 結果
    /// </summary>
    [MetadataType(typeof(spGET_USERS_GROUP_BY_USER_ID_ResultMetadata))]
    public partial class spGET_USERS_GROUP_BY_USER_ID_Result
    {
        /// <summary>
        /// 依照查詢條件取出使用者所屬群組資料 結果
        /// </summary>
        public class spGET_USERS_GROUP_BY_USER_ID_ResultMetadata
        {
            /// <summary>
            /// 群組識別碼
            /// </summary>
            [Display(Name = "群組識別碼")]
            public string fsGROUP_ID { get; set; }
            /// <summary>
            /// 群組名稱
            /// </summary>
            [Display(Name = "群組名稱")]
            public string fsNAME { get; set; }
            /// <summary>
            /// 群組描述
            /// </summary>
            [Display(Name = "群組描述")]
            public string fsDESCRIPTION { get; set; }
        }
        
    }
}
