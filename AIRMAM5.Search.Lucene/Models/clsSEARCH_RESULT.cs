using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRMAM5.Search.Lucene.Models
{

    public class clsSEARCH_RESULT
    {
        /// <summary>
        /// 結果筆數
        /// </summary>
        public class clsCOUNT
        {
            public int fnSUBJECT_COUNT { get; set; }
            public int fnVIDEO_COUNT { get; set; }
            public int fnAUDIO_COUNT { get; set; }
            public int fnPHOTO_COUNT { get; set; }
            public int fnDOC_COUNT { get; set; }
        }
        /// <summary>
        /// 查詢結果
        /// </summary>
        public class clsARC
        {
            /// <summary>
            /// 檔案編號
            /// </summary>
            public string fsFILE_NO { get; set; }
            /// <summary>
            /// 符合關鍵字附近的字
            /// </summary>
            public string fsMATCH { get; set; }
        }
        /// <summary>
        /// 主題結果
        /// </summary>
        public class clsSUBJ
        {
            /// <summary>
            /// 主題編號
            /// </summary>
            public string fsSUBJ_ID { get; set; }
            /// <summary>
            /// 符合關鍵字附近的字
            /// </summary>
            public string fsMATCH { set; get; }
        }
        /// <summary>
        /// 影片結果
        /// </summary>
        public class clsVIDEO
        {
            /// <summary>
            /// 檔案編號
            /// </summary>
            public string fsFILE_NO { get; set; }
            /// <summary>
            /// 符合關鍵字附近的字
            /// </summary>
            public string fsMATCH { get; set; }
        }
        /// <summary>
        /// 聲音結果
        /// </summary>
        public class clsAUDIO
        {
            /// <summary>
            /// 檔案編號
            /// </summary>
            public string fsFILE_NO { get; set; }
            /// <summary>
            /// 符合關鍵字附近的字
            /// </summary>
            public string fsMATCH { set; get; }

        }
        /// <summary>
        /// 圖片結果
        /// </summary>
        public class clsPHOTO
        {
            /// <summary>
            /// 檔案編號
            /// </summary>
            public string fsFILE_NO { get; set; }
            /// <summary>
            /// 符合關鍵字附近的字
            /// </summary>
            public string fsMATCH { set; get; }

        }
        /// <summary>
        /// 文件結果
        /// </summary>
        public class clsDOC
        {
            /// <summary>
            /// 檔案編號
            /// </summary>
            public string fsFILE_NO { get; set; }
            /// <summary>
            /// 符合關鍵字附近的字
            /// </summary>
            public string fsMATCH { set; get; }
        }
        /// <summary>
        /// 群組結果
        /// </summary>
        public class clsGROUP
        {
            /// <summary>
            /// 編號
            /// </summary>
            public string fsGROUP { get; set; }
        }
        /// <summary>
        /// 樣板結果
        /// </summary>
        public class clsTEMP
        {
            /// <summary>
            /// 樣板編號
            /// </summary>
            public int fnTEMP_ID { get; set; }
        }
    }
}