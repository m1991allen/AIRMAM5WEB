using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRMAM5.Search.Lucene.Models
{
    public class clsPARAMETER
    {
        /// <summary>
        /// 要查詢的索引庫名稱，以逗號分隔
        /// </summary>
        public string fsINDEX { set; get; }
        /// <summary>
        /// 關鍵字
        /// </summary>
        public string fsKEYWORD { set; get; }
        /// <summary>
        /// 日期區間查詢
        /// </summary>
        public clsDATE_SEARCH clsDATE { get; set; }
        /// <summary>
        /// 欄位查詢
        /// </summary>
        public List<clsCOLUMN_SEARCH> lstCOLUMN_SEARCH { get; set; }
        /// <summary>
        /// 搜尋模式(0:一般模式、2:同義詞搜尋)
        /// </summary>
        public int fnSEARCH_MODE { set; get; }
        /// <summary>
        /// 同音(0:關閉、1:開啟)
        /// </summary>
        public int fnHOMO { set; get; }
        /// <summary>
        /// 樹狀節點
        /// </summary>
        public string fsTREE { set; get; }
        /// <summary>
        /// 欄位排序(1:升冪、2:降冪)
        /// </summary>
        public List<clsCOLUMN_ORDER> lstCOLUMN_ORDER { set; get; }
        /// <summary>
        /// 每頁筆數
        /// </summary>
        public int fnPAGE_SIZE { set; get; }
        /// <summary>
        /// 開始筆數
        /// </summary>
        public int fnSTART_INDEX { set; get; }
        /// <summary>
        /// 可查詢節點權限(以分號分開)
        /// </summary>
        public string fsAUTH_DIR { set; get; }
        /// <summary>
        /// 可查詢機密權限(以分號分開)
        /// </summary>
        public string fsSECRET { set; get; }
        /// <summary>
        /// 是否為管理者
        /// </summary>
        public bool fbIS_ADMIN { get; set; }
        /// <summary>
        /// 樣板編號
        /// </summary>
        public int fnTEMP_ID { get; set; }

        /// <summary>
        /// 日期查詢
        /// </summary>
        public class clsDATE_SEARCH
        {
            /// <summary>
            /// 日期欄位
            /// </summary>
            public string fsCOLUMN { get; set; }
            /// <summary>
            /// 日期(起)
            /// </summary>
            public string fdSDATE { set; get; }
            /// <summary>
            /// 日期(迄)
            /// </summary>
            public string fdEDATE { set; get; }
        }
        /// <summary>
        /// 欄位檢索類別
        /// </summary>
        public class clsCOLUMN_SEARCH
        {
            /// <summary>
            /// 是否全文檢索(true:檢索、false:區間或比對)
            /// </summary>
            public bool fbIS_FULLTEXT { get; set; }
            /// <summary>
            /// 欄位名稱
            /// </summary>
            public string fsCOLUMN { get; set; }
            /// <summary>
            /// 值
            /// </summary>
            public string fsVALUE { get; set; }
        }
        /// <summary>
        /// 排序類別
        /// </summary>
        public class clsCOLUMN_ORDER
        {
            /// <summary>
            /// 欄位名稱
            /// </summary>
            public string fsCOLUMN { get; set; }
            /// <summary>
            /// 值(1:升冪、2:降冪)
            /// </summary>
            public string fsVALUE { get; set; }
        }
    }
}