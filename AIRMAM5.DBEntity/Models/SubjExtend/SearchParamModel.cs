
namespace AIRMAM5.DBEntity.Models.SubjExtend
{
    /// <summary>
    /// 擴充功能{新聞文稿/公文對應} 動態的查詢參數 model
    /// </summary>
    public class SearchParam
    {
        /// <summary>
        /// 欄位 名
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 欄位 值 (適用單一值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 欄位 資料型態
        /// </summary>
        public string FieldType { get; set; }

    }
    /// <summary>
    /// 泛型: 擴充功能{新聞文稿/公文對應} 動態的查詢參數 model
    /// </summary>
    /// <typeparam name="T">資料型態。用於指定屬性GenericValue </typeparam>
    public class SearchParam<T> : SearchParam
    {
        /// <summary>
        /// 泛型欄位值 (適用多個值組合, 如.字串陣列值 ["22","ko","5j"], ["2021-07-23","2021-07-25"]
        /// </summary>
        /// <remarks>
        ///   1、文稿對應查詢時, <typeparamref name="T"/>會使用到字串陣列型態(用於日期區間)。 
        /// </remarks>
        public T GenericValue { get; set; }
    }

}
