using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.Search
{
    /// <summary>
    /// (前端)檢索參數(參考檢索API的參數)
    /// </summary>
    public class SearchParameterViewModel
    {
        public SearchParameterViewModel() { }

        #region >>>>> 欄位參數 
        /// <summary>
        /// 關鍵字 [fsKEYWORD]
        /// </summary>
        public string fsKEYWORD { set; get; }

        /// <summary>
        /// 要查詢的索引庫名稱，以逗號分隔 [fsINDEX] 例: Video_DEV,Audio_DEV,Photo_DEV,Doc_DEV, <see cref="SearchTypeEnum"/>
        /// </summary>
        public string fsINDEX { set; get; }

        /// <summary>
        /// 搜尋模式(0:一般模式、2:同義詞搜尋) [fnSEARCH_MODE]
        /// </summary>
        public int fnSEARCH_MODE { set; get; } = 0;

        /// <summary>
        /// 同音(0:關閉、1:開啟) [fnHOMO]
        /// </summary>
        public int fnHOMO { set; get; } = 0;

        /// <summary>
        /// 日期區間查詢 [clsDATE]
        /// </summary>
        public DateSearchModel clsDATE { get; set; } = new DateSearchModel();

        /// <summary>
        /// 欄位排序, 例:1:升冪(ASC)、2:降冪(Desc) [clsCOLUMN_ORDER]
        /// </summary>
        /// <remarks> (應該)日期查詢與DateSearchModel 配合一起使用的 </remarks>
        public List<ColumnSort> lstCOLUMN_ORDER { get; set; } = new List<ColumnSort>();

        /// <summary>
        /// 樣板編號 [fnTEMP_ID]
        /// </summary>
        public long fnTEMP_ID { get; set; } = 0;
        /// <summary>
        /// 進階查詢.欄位條件 列表 [clsCOLUMN_SEARCH]
        /// </summary>
        /// <remarks> 有選擇"樣板" 就會有欄位可選擇. </remarks>
        public List<AdvancedQryModel> lstCOLUMN_SEARCH { get; set; } = new List<AdvancedQryModel>();

        /// <summary>
        /// 每頁筆數(前端指定) [fnPAGE_SIZE]
        /// </summary>
        public int fnPAGE_SIZE { get; set; } = 10;

        /// <summary>
        /// 開始筆數(前端指定) [fnSTART_INDEX]
        /// </summary>
        public int fnSTART_INDEX { get; set; } = 1;
        #endregion

        #region  Search Parameter Model
        /// <summary>
        /// 日期查詢 [clsDATE_SEARCH]
        /// </summary>
        public class DateSearchModel
        {
            /// <summary>
            /// 日期欄位 [fsCOLUMN]
            /// </summary>
            public string fsCOLUMN { get; set; }

            /// <summary>
            /// 日期(起) [fdSDATE]
            /// </summary>
            /// <remarks> 格式: yyyy/MM/dd EX: 2019/01/01 </remarks>
            public string fdSDATE { set; get; }

            /// <summary>
            /// 日期(迄) [fdEDATE]
            /// </summary>
            /// <remarks> 格式: yyyy/MM/dd EX: 2019/12/01 </remarks>
            public string fdEDATE { set; get; }
        }

        /// <summary>
        /// 欄位排序 [clsCOLUMN_ORDER]
        /// </summary>
        /// <remarks> (應該)日期查詢與DateSearchModel 配合一起使用的 </remarks>
        public class ColumnSort
        {
            /// <summary>
            /// 欄位名稱 [fsCOLUMN]
            /// </summary>
            public string fsCOLUMN { get; set; } = string.Empty;

            /// <summary>
            /// 值 例:1:升冪(ASC)、2:降冪(Desc) [fsVALUE]
            /// </summary>
            /// <remarks> 例: ASC, Desc</remarks>
            public string fsVALUE { get; set; }
        }

        /// <summary>
        /// 進階查詢:欄位檢索類別 [clsCOLUMN_SEARCH]
        /// </summary>
        public class AdvancedQryModel
        {
            /// <summary>
            /// 是否全文檢索(true:檢索、false:區間或比對) [fbIS_FULLTEXT]
            /// </summary>
            public bool fbIS_FULLTEXT { get; set; } = true;

            /// <summary>
            /// 欄位名稱 例:fsATTRIBUTE1 [fsCOLUMN]
            /// </summary>
            public string fsCOLUMN { get; set; } = string.Empty;

            /// <summary>
            /// 值, 例:大時代 [fsVALUE]
            /// </summary>
            public string fsVALUE { get; set; } = string.Empty;
        }
        #endregion
    }

}
