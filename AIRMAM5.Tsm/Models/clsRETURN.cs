using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRMAM5.Tsm.Models
{
    public class clsRETURN
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; } = false;
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 回傳資訊
        /// </summary>
        public object Data { get; set; } = null;
        /// <summary>
        /// 回傳時間
        /// </summary>
        public string ResponseTime { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        /// <summary>
        /// 例外訊息
        /// </summary>
        public Exception ErrorException { get; set; } = null;
    }
}