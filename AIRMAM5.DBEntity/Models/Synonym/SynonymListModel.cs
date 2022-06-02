using AIRMAM5.DBEntity.DBEntity;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Synonym
{
    /// <summary>
    /// 同義詞字串清單
    /// </summary>
    public class SynonymListModel
    {
        /// <summary>
        /// 同義詞字串清單
        /// </summary>
        public SynonymListModel() { }

        #region >>>>>欄位參數
        /// <summary>
        /// 同義詞ID
        /// </summary>
        public long fnINDEX_ID { get; set; }

        /// <summary>
        /// 同義詞字串(以;分隔)
        /// </summary>
        public string fsTEXT_LIST { get; set; } = string.Empty;

        /// <summary>
        /// 分類
        /// </summary>
        public string fsTYPE { get; set; } = string.Empty;

        /// <summary>
        /// 備註
        /// </summary>
        public string fsNOTE { get; set; } = string.Empty;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public SynonymListModel ConvertData<T>(T data)
        {
            if (data == null) { return this; }

            var properties = typeof(T).GetProperties();
            foreach (var pp in properties)
            {
                var val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fnINDEX_ID")
                {
                    if (long.TryParse(val.ToString(), out long idx)) { this.fnINDEX_ID = idx; }
                }
                if (pp.Name == "fsTEXT_LIST") { this.fsTEXT_LIST = val.ToString(); }
                if (pp.Name == "fsTYPE") { this.fsTYPE = val.ToString(); }
                if (pp.Name == "fsNOTE") { this.fsNOTE = val.ToString(); }
            }

            return this;
        }
    }

    /// <summary>
    /// 同義詞維護查詢頁面-參數
    /// </summary>
    public class SynonymSearchModel
    {
        public List<SelectListItem> SynoList = new List<SelectListItem>();
    }

}
