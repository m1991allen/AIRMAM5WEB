
using System.ComponentModel;


namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// true與false (資料庫)文字表示: 1, 0
    /// </summary>
    public enum IsTrueFalseEnum
    {
        /// <summary>
        /// false
        /// </summary>
        [Description("否")]
        N = 0,

        /// <summary>
        /// true
        /// </summary>
        [Description("是")]
        Y = 1
    }

    /// <summary>
    /// 布林值
    /// </summary>
    public enum BoolTrueFalseEnum
    {
        /// <summary>
        /// False
        /// </summary>
        [Description("False")]
        FALSE,

        /// <summary>
        /// True
        /// </summary>
        [Description("True")]
        TRUE
    }
}
