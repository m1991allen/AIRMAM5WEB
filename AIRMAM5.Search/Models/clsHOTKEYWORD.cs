using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRMAM5.Search.Models
{
    public class clsGET_HOTKEYWORD
    {
        public string fsRESULT { get; set; }
        public string fsMESSAGE { get; set; }
        public List<clsHOTKEYWORD> lstHOTKEYWORD { get; set; }
    }

    public class clsHOTKEYWORD
    {
        /// <summary>
        /// 熱門關鍵字
        /// </summary>
        public string fsKEYWORD { set; get; }
        /// <summary>
        /// 次數
        /// </summary>
        public int fnCOUNT { set; get; }
    }
}