using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRMAM5.Models.TSMapi
{
    /// <summary>
    /// 磁帶下架參數 (要參照 AIRMAM5.TSM專案中的 clsTAPE_CHECK_OUT_ARGS())
    /// </summary>
    public class TapeCheckOutArgs
    {
        /// <summary>
        /// 磁帶編號
        /// </summary>
        public List<string> lstTAPE_NO { get; set; }
    }
}