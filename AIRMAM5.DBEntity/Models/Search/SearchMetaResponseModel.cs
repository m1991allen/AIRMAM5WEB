
namespace AIRMAM5.DBEntity.Models.Search
{
    /// <summary>
    /// 檢索符合結果 API Response Model 
    /// <para>　　API: AIRMAM5.Search.Lucene/Search/SearchMeta </para>
    /// </summary>
    public class SearchMetaResponseModel
    {
        /// <summary>
        /// 檔案編號: 影音圖文 回傳內容皆相同
        /// </summary>
        public string fsFILE_NO { get; set; }

        /// <summary>
        /// 符合關鍵字附近的字
        /// </summary>
        public string fsMATCH { get; set; } = string.Empty;
    }

}
