using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// 代碼主表 代碼類別： S:系統代碼、C:自訂代碼
    /// </summary>
    public enum CodeSetTypeEnum
    {
        /// <summary>
        /// S:系統代碼
        /// </summary>
        [Description("系統代碼")]
        S,
        /// <summary>
        /// C:自訂代碼
        /// </summary>
        [Description("自訂代碼")]
        C
    }

}
