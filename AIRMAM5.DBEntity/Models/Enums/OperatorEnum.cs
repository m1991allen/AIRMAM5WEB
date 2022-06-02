using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// 運算子(規則設定)
    /// </summary>
    public enum OperatorEnum
    {
        /// <summary>
        /// 包含 Include
        /// </summary>
        [Description("包含")]
        Include,
        /// <summary>
        /// 不包含 Exclude
        /// </summary>
        [Description("不包含")]
        Exclude,
        /// <summary>
        /// 等於
        /// </summary>
        [Description("等於")]
        Equal,
        /// <summary>
        /// 介於
        /// </summary>
        [Description("介於")]
        Between
    }

}
