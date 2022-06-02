using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AIRMAM5.Utility.Enums
{
    /// <summary>
    /// 時間單位
    /// </summary>
    public enum TimeUnitEnum
    {
        /// <summary>
        /// 毫秒數
        /// </summary>
        [Description("毫秒數")]
        Milliseconds,
        /// <summary>
        /// 分鐘數
        /// </summary>
        [Description("分鐘數")]
        Seconds,
        /// <summary>
        /// 分鐘數
        /// </summary>
        [Description("分鐘數")]
        Minutes,
        /// <summary>
        /// 時數
        /// </summary>
        [Description("時數")]
        Hours,
        /// <summary>
        /// 天數
        /// </summary>
        [Description("天數")]
        Days,
    }
}
