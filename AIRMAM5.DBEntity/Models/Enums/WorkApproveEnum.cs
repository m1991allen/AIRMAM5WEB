using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// WORK_APPROVE 審核狀態 Enum
    /// </summary>
    public enum WorkApproveEnum
    {
        /// <summary>
        /// 待審核
        /// </summary>
        [Description("待審核")]
        _A,
        /// <summary>
        /// 審核過
        /// </summary>
        [Description("審核通過")]
        _C,
        /// <summary>
        /// 無須審核
        /// </summary>
        [Description("無須審核")]
        _N,
        /// <summary>
        /// 審核拒絕
        /// </summary>
        [Description("審核拒絕")]
        _R
    }
}
