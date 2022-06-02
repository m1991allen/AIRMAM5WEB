using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// 檔案直向/橫向 [fsDIRECTION]
    /// </summary>
    public enum FileDirection
    {
        /// <summary>
        /// 橫向(Default)
        /// </summary>
        [Description("橫向")]
        H,

        /// <summary>
        /// 直向
        /// </summary>
        [Description("直向")]
        V,
    }
}
