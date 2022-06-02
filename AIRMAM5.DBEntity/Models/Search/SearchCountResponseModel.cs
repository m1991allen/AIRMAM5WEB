
namespace AIRMAM5.DBEntity.Models.Search
{
    /// <summary>
    /// 檢索符合筆數統計 API Response Model 
    /// <para>　　API: AIRMAM5.Search.Lucene/Search/SearchCount </para>
    /// </summary>
    public class SearchCountResponseModel
    {
        /// <summary>
        /// 主題分類筆數 fnSUBJECT_COUNT
        /// </summary>
        public int fnSUBJECT_COUNT { get; set; } = 0;

        /// <summary>
        /// 影片分類筆數 fnVIDEO_COUNT
        /// </summary>
        public int fnVIDEO_COUNT { get; set; } = 0;

        /// <summary>
        /// 聲音分類筆數 fnAUDIO_COUNT
        /// </summary>
        public int fnAUDIO_COUNT { get; set; } = 0;

        /// <summary>
        /// 圖片分類筆數 fnPHOTO_COUNT
        /// </summary>
        public int fnPHOTO_COUNT { get; set; } = 0;

        /// <summary>
        /// 圖片分類筆數 fnDOC_COUNT
        /// </summary>
        public int fnDOC_COUNT { get; set; } = 0;
    }

}
