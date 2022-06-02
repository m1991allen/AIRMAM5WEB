using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// TEMP002	自訂欄位.資料型態 Enum
    /// </summary>
    public enum CodeTEMP002Enum
    {
        /// <summary>
        /// Text 文字
        /// </summary>
        [Description("文字")]
        NVARCHAR = 1,
        /// <summary>
        /// Number 數字
        /// </summary>
        [Description("數字")]
        INTEGER = 2,
        /// <summary>
        /// DateTime 日期時間
        /// </summary>
        [Description("日期")]
        DATETIME = 3,
        /// <summary>
        /// Custom 自訂代碼
        /// </summary>
        [Description("自訂代碼")]
        CODE = 5
    }

}
