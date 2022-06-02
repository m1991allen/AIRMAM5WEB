using System;
using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.SubjExtend
{
    /// <summary>
    /// 擴充功能{新聞文稿/公文系統} (共用/統一)查詢結果 model
    /// </summary>
    /// <typeparam name="T">資料內容的資料型態 指定某Model格式 </typeparam>
    public class SearchResultModel<T>
    {
        /// <summary>
        /// 資料主鍵欄位 ;分隔符號
        /// </summary>
        public string PKeyCol { get; set; }

        /// <summary>
        /// 資料欄位名稱 <typeparamref name="T"/>
        /// </summary>
        public T DataTitle { get; set; }

        /// <summary>
        /// 資料內容 泛型<typeparamref name="T"/>
        /// </summary>
        public List<T> DataList { get; set; }
    }
}
