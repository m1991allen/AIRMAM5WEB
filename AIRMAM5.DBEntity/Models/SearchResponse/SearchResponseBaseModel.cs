
using AIRMAM5.DBEntity.Models.Search;
using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.SearchResponse
{
    /// <summary>
    /// 檢索結果-Basic: 1檢索參數、2查詢條件顯示文字、3檢索符合筆數、4檢索符合檔案號清單
    /// </summary>
    public class SearchResponseBaseModel
    {
        /// <summary>
        /// 檢索結果-Basic
        /// </summary>
        public SearchResponseBaseModel() { }

        /// <summary>
        /// 1檢索參數 <see cref="SearchParameterViewModel"/>
        /// </summary>
        public SearchParameterViewModel SearchParam { get; set; } = new SearchParameterViewModel();

        /// <summary>
        /// 2查詢條件文字化顯示 <see cref="ConditionModel"/>
        /// </summary>
        /// <remarks>
        ///   例, 檢索類型：影片、聲音、圖片、文字。 查詢方式：同音、同義。 建立日期區間：2019/01/01~2019/12/31 新到舊。 
        /// </remarks>
        public ConditionModel ConditionStr { get; set; } = new ConditionModel();

        /// <summary>
        /// 3檢索符合筆數 <see cref="SearchCountResponseModel"/>
        /// </summary>
        public SearchCountResponseModel CountData { get; set; } = new SearchCountResponseModel();

        /// <summary>
        /// 4檢索符合檔案編號清單 <see cref="GetArcSearchResult"/>
        /// </summary>
        public List<GetArcSearchResult> MetaDataList { get; set; } = new List<GetArcSearchResult>();

        /// <summary>
        /// 查詢條件文字化顯示。
        /// </summary>
        /// <remarks>
        ///   例, 檢索類型：影片、聲音、圖片、文字。 查詢方式：同音、同義。 建立日期區間：2019/01/01~2019/12/31 新到舊。 
        /// </remarks>
        public class ConditionModel
        {
            /// <summary>
            /// 檢索類型
            /// </summary>
            public string SearchType { get; set; } = string.Empty;

            /// <summary>
            /// 查詢方式
            /// </summary>
            public string SearchMode { get; set; } = string.Empty;

            /// <summary>
            /// {}日期區間
            /// </summary>
            public string DateInterval { get; set; } = string.Empty;

            /// <summary>
            /// 進階查詢
            /// </summary>
            public string AdvancedQry { get; set; } = string.Empty;
        }
    }

}
