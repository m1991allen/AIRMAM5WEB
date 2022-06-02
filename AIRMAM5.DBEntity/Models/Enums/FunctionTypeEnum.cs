using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// 功能項目類別
    /// </summary>
    public enum FunctionTypeEnum
    {
        /// <summary>
        /// G 父層級
        /// </summary>
        [Description("父層級")]
        G,

        /// <summary>
        /// M 
        /// </summary>
        [Description("功能項")]
        M,

        /// <summary>
        /// X 刪除
        /// </summary>
        [Description("刪除")]
        X
    }

}
