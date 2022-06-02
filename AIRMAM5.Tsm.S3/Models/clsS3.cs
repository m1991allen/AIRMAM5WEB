using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AIRMAM5.Tsm.S3.Models
{
    public class clsS3
    {
        public string Restore { get; set; }
        public string AcceptRanges { get; set; }
        public string ContentType { get; set; }
        public string LastModified { get; set; }
        public string ETag { get; set; }
        public string StorageClass { get; set; }
        //     "Restore": "ongoing-request=\"true\"",
        //"AcceptRanges": "bytes",
        //"ContentType": "video/mpeg",
        //"LastModified": "Fri, 22 May 2020 09:22:42 GMT",
        //"ContentLength": 17096704,
        //"ETag": "\"f33199eeae14200348bfea9d7f9e7ed9-3\"",
        //"StorageClass": "GLACIER",
        //"Metadata": {}
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


    public class clsRECALL_ARGS
    {
        public string fsFILE_PATH { get; set; }
        public int fnRESERVE_DAYS { get; set; }
        public string fsRECALL_MODE { get; set; }
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
}