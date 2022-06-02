using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.Rule
{
    /// <summary>
    /// 資料表資訊 MODEL
    /// </summary>
    public class FilterTableModel
    {
        /// <summary>
        /// 資料表資訊 MODEL
        /// </summary>
        public FilterTableModel() { }

        /// <summary>
        /// 資料表名稱 ex: dbo.tbmGROUPS
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// 資料表說明
        /// </summary>
        public string TableDesc { get; set; } = string.Empty;

        /// <summary>
        /// 規則資料表欄位屬性清單
        /// </summary>
        public List<FieldInfo> Properties { get; set; } = new List<FieldInfo>();
    }

}
