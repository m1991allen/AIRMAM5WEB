using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AIRMAM5.Tsm.Models
{
    public class clsTSM
    {
    }

    /// <summary>
    /// 查詢磁帶是否在架上參數
    /// </summary>
    public class clsTAPE_IS_IN_LIB_ARGS
    {
        /// <summary>
        /// 磁帶編號(可多筆)
        /// </summary>
        public List<string> lstTAPE_NO { get;set; }
    }

    /// <summary>
    /// 查詢磁帶是否在架上結果
    /// </summary>
    public class clsTAPE_IS_IN_LIB_RESULT
    {
        /// <summary>
        /// 磁帶編號
        /// </summary>
        public string fsTAPE_NO { get; set; }
        /// <summary>
        /// 是否在架上
        /// </summary>
        public bool fbIS_IN_LIB { get; set; }
    }

    /// <summary>
    /// 查詢檔案狀態參數
    /// </summary>
    public class clsFILE_STATUS_ARGS
    {
        /// <summary>
        /// 檔案路徑(TSM)
        /// </summary>
        public List<FILE_NO_TSM_PATH> lstFILE_TSM_PATH { get; set; }

        public class FILE_NO_TSM_PATH
        {
            /// <summary>
            /// 檔案編號
            /// </summary>
            public string fsFILE_NO { get; set; }
            /// <summary>
            /// 檔案在TSM路徑
            /// </summary>
            public string fsFILE_TSM_PATH { get; set; }
        }
    }

    /// <summary>
    /// 查詢檔案狀態結果
    /// </summary>
    public class clsFILE_STATUS_RESULT
    {
        /// <summary>
        /// 檔案編號
        /// </summary>
        public string FILE_NO { get; set; }
        /// <summary>
        /// 檔案狀態【0(檔案在磁帶);1(檔案在磁碟);2(檔案不存在);3(處理中)】
        /// </summary>
        public FileStatus FILE_STATUS { get; set; }
    }

    /// <summary>
    /// 檔案狀態列舉
    /// </summary>
    public enum FileStatus
    {
        /// <summary>
        /// 檔案在Tape
        /// </summary>
        [Description("檔案在磁帶")]
        Tape,
        /// <summary>
        /// 檔案在Nearline
        /// </summary>
        [Description("檔案在磁碟")]
        NearLine,
        /// <summary>
        /// 錯誤
        /// </summary>
        [Description("錯誤")]
        Error,
        /// <summary>
        /// 處理中
        /// </summary>
        [Description("處理中")]
        Processing,
        /// <summary>
        /// 無檔案路徑
        /// </summary>
        [Description("檔案不存在")]
        NotExist,
        /// <summary>
        /// 檔案在線
        /// </summary>
        [Description("檔案在線")]
        Online,
        /// <summary>
        /// 檔案離線
        /// </summary>
        [Description("檔案離線")]
        Offline,
        /// <summary>
        /// 檔案深度離線
        /// </summary>
        [Description("檔案深度離線")]
        Offline_Deep
    }

    /// <summary>
    /// 磁帶狀態結果
    /// </summary>
    public class clsTAPE_INFO_RESULT
    {
        /// <summary>
        /// 磁帶編號
        /// </summary>
        public string VOL_ID { get; set; }
        /// <summary>
        /// 磁帶類型
        /// </summary>
        public string VOL_TYPE { get; set; }
        /// <summary>
        /// 使用狀態
        /// </summary>
        public string VOL_USE_STATUS { get; set; }
        /// <summary>
        /// 已存放資料(GB)
        /// </summary>
        public double USED_GB { get; set; }
        /// <summary>
        /// 最後讀取日期
        /// </summary>
        public string READ_DATE { get; set; }
        /// <summary>
        /// 最後寫入日期
        /// </summary>
        public string WRITE_DATE { get; set; }
        /// <summary>
        /// 儲存池
        /// </summary>
        public string POOL_NAME { get; set; }
        /// <summary>
        /// 讀寫狀態
        /// </summary>
        public string VOL_RW_STATUS { get; set; }
        /// <summary>
        /// 寫入錯誤
        /// </summary>
        public string WRITE_ERRORS { get; set; }
        /// <summary>
        /// 讀取錯誤
        /// </summary>
        public string READ_ERRORS { get; set; }
    }

    /// <summary>
    /// 磁帶下架參數
    /// </summary>
    public class clsTAPE_CHECK_OUT_ARGS
    {
        /// <summary>
        /// 磁帶編號
        /// </summary>
        public List<string> lstTAPE_NO { get; set; }
    }

    /// <summary>
    /// 待上架磁帶清單結果
    /// </summary>
    public class clsWAIT_VOL_RESULT
    {
        /// <summary>
        /// 待上架編號
        /// </summary>
        public long fnWAIT_ID { get; set; }
        /// <summary>
        /// 磁帶編號
        /// </summary>
        public string fsVOL_ID { get; set; }
        /// <summary>
        /// 工作編號
        /// </summary>
        public long fnWORK_ID { get; set; }
        /// <summary>
        /// 狀態名稱
        /// </summary>
        public string _sTATUS_NAME { get; set; }
        /// <summary>
        /// 調用原因
        /// </summary>
        public string _sBOOKING_REASON { get; set; }
        /// <summary>
        /// 優先權
        /// </summary>
        public string _sPRIORITY { get; set; }
        /// <summary>
        /// 調用人員
        /// </summary>
        public string fsCREATED_BY_NAME { get; set; }
    }

    /// <summary>
    /// 更新待上架磁帶狀態參數
    /// </summary>
    public class clsUPDATE_WAIT_VOL_ARGS
    {
        public long fnWAIT_ID { get; set; }
        public string fsVOL_ID { get; set; }
        public long fnWORK_ID { get; set; }
        public string fsSTATUS { get; set; }
        public string fsUPDATED_BY { get; set; }
    }

    /// <summary>
    /// 更新已上架磁帶WORK參數
    /// </summary>
    public class clsUPDATE_L_WORK_ARGS
    {
        public long fnWORK_ID { get; set; }
        public string fsSTATUS { get; set; }
        public string fsUPDATED_BY { get; set; }
    }
}