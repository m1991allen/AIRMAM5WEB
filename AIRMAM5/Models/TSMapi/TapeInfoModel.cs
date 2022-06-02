using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRMAM5.Models.TSMapi
{
    /// <summary>
    /// 架上磁帶資訊 TapeInfoInLib
    /// <para> 參照 AIRMAM5.Tsm Models\\clsTSM.cs : clsTAPE_INFO_RESULT() </para>
    /// </summary>
    public class TapeInfoModel
    {
        /// <summary>
        /// 磁帶編號
        /// </summary>
        public string VOL_ID { get; set; } = string.Empty;

        /// <summary>
        /// 磁帶類型
        /// </summary>
        public string VOL_TYPE { get; set; } = string.Empty;

        /// <summary>
        /// 使用狀態
        /// </summary>
        public string VOL_USE_STATUS { get; set; } = string.Empty;

        /// <summary>
        /// 已存放資料(GB)
        /// </summary>
        public double USED_GB { get; set; }

        /// <summary>
        /// 最後讀取日期
        /// </summary>
        public string READ_DATE { get; set; } = string.Empty;

        /// <summary>
        /// 最後寫入日期
        /// </summary>
        public string WRITE_DATE { get; set; } = string.Empty;

        /// <summary>
        /// 儲存池
        /// </summary>
        public string POOL_NAME { get; set; } = string.Empty;

        /// <summary>
        /// 讀寫狀態
        /// </summary>
        public string VOL_RW_STATUS { get; set; } = string.Empty;

        /// <summary>
        /// 寫入錯誤次數
        /// </summary>
        public string WRITE_ERRORS { get; set; }

        /// <summary>
        /// 讀取錯誤次數
        /// </summary>
        public string READ_ERRORS { get; set; }
    }
}