using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRMAM5.Models.TSMapi
{
    /// <summary>
    /// 查詢檔案TSM狀態結果
    /// </summary>
    public class GetFileStatusResult
    {
        /// <summary>
        /// 檔案編號
        /// </summary>
        public string FILE_NO { get; set; }

        /// <summary>
        /// 檔案狀態【0(檔案在磁帶);1(檔案在磁碟);2(檔案不存在);3(處理中)】
        /// <para> 20200103_Added: 4(檔案不存在) </para>
        /// <para> 參考 TSMFileStatus </para>
        /// </summary>
        public int FILE_STATUS { get; set; }
    }

}