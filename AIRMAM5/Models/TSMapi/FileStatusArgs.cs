using AIRMAM5.DBEntity.DBEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace AIRMAM5.Models.TSMapi
{
    /// <summary>
    /// 查詢檔案狀態參數(參照AIRMAM5.TSM API)
    /// <para> 參照 AIRMAM5.Tsm Models\\clsTSM.cs : clsFILE_STATUS_ARGS() </para>
    /// </summary>
    public class clsFILE_STATUS_ARGS //FileStatusArgs
    {
        /// <summary>
        /// 查詢檔案狀態參數
        /// </summary>
        public clsFILE_STATUS_ARGS() { }

        /// <summary>
        /// 查詢檔案狀態參數
        /// </summary>
        /// <param name="paths"></param>
        public clsFILE_STATUS_ARGS(List<FILE_NO_TSM_PATH> paths) { lstFILE_TSM_PATH = paths; }

        /// <summary>
        /// 檔案路徑(TSM)  lstFILE_TSM_PATH
        /// </summary>
        [JsonPropertyName("lstFILE_TSM_PATH")]
        public List<FILE_NO_TSM_PATH> lstFILE_TSM_PATH { get; set; }

    }

    /// <summary>
    /// 檔案編號對應的TSM路徑
    /// </summary>
    public class FILE_NO_TSM_PATH
    {
        /// <summary>
        /// 檔案編號對應的TSM路徑
        /// </summary>
        /// <param name="m">預存資料: spTSM_GET_FILE_UNCPATH_TO_TSMPATH_Result </param>
        public FILE_NO_TSM_PATH(spTSM_GET_FILE_UNCPATH_TO_TSMPATH_Result m)
        {
            this.fsFILE_NO = m.fsFILE_NO;
            this.fsFILE_TSM_PATH = m.fsFILE_TSM_PATH;
        }

        /// <summary>
        /// 檔案編號 fsFILE_NO
        /// </summary> 
        [JsonPropertyName("fsFILE_NO")]
        public string fsFILE_NO { get; set; }

        /// <summary>
        /// 檔案在TSM路徑 fsFILE_TSM_PATH
        /// </summary>
        [JsonPropertyName("fsFILE_TSM_PATH")]
        public string fsFILE_TSM_PATH { get; set; } = string.Empty;
    }
}