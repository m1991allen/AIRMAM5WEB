using AIRMAM5.DBEntity.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Synonym
{
    /// <summary>
    /// 同義詞 檢視, 繼承參考 <see cref="TableUserDateByNameModel"/>
    /// </summary>
    public class SynonymViewModel : TableUserDateByNameModel
    {
        /// <summary>
        /// 同義詞 檢視
        /// </summary>
        public SynonymViewModel() { }

        #region >>> 欄位定義
        /// <summary>
        /// 同義詞ID
        /// </summary>
        [Display(Name = "ID")]
        public long fnINDEX_ID { get; set; }

        /// <summary>
        /// 同義詞字串(以;分隔)
        /// </summary>
        [Display(Name = "同義詞詞彙")]
        public string fsTEXT_LIST { get; set; } = string.Empty;

        /// <summary>
        /// 同義詞詞彙 Array
        /// </summary>
        [Display(Name = "同義詞詞彙")]
        public string[] TextList { get; set; }

        /// <summary>
        /// 分類
        /// </summary>
        [Display(Name = "分類")]
        public string fsTYPE { get; set; }

        /// <summary>
        /// 類別 fsCODEID= SYNO_TYPE
        /// </summary>
        [Display(Name = "類別")]
        public string fsTYPE_NAME { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [Display(Name = "備註")]
        public string fsNOTE { get; set; } = string.Empty;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public SynonymViewModel ConvertData<T>(T data)
        {
            if (data == null) { return this; }

            var _Properties = typeof(T).GetProperties();

            foreach (var info in _Properties)
            {
                var val = info.GetValue(data) ?? string.Empty;

                if (info.Name == "fnINDEX_ID")
                    this.fnINDEX_ID = string.IsNullOrEmpty(val.ToString()) ? 0 : int.Parse(val.ToString());

                if (info.Name == "fsTYPE") this.fsTYPE = val.ToString();
                if (info.Name == "fsTEXT_LIST")
                {
                    this.fsTEXT_LIST = val.ToString();
                    this.TextList = val.ToString().Split(new char[] { ';' });
                }

                if (info.Name == "fsNOTE") this.fsNOTE = val.ToString();
                if (info.Name == "fsTYPE_NAME") this.fsTYPE_NAME = val.ToString();

                if (info.Name == "fsCREATED_BY") this.CreatedBy = val.ToString();
                if (info.Name == "fsCREATED_BY_NAME") this.CreatedByName = val.ToString();
                if (info.Name == "fdCREATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime dt);
                    this.CreatedDate = dt;
                }

                if (info.Name == "fsUPDATED_BY") this.UpdatedBy = val.ToString();
                if (info.Name == "fsUPDATED_BY_NAME") this.UpdatedByName = val.ToString();
                if (info.Name == "fdUPDATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime dt);
                    this.UpdatedDate = dt;
                    if (string.IsNullOrEmpty(val.ToString())) this.UpdatedDate = null; else this.UpdatedDate = dt;
                }
            }

            this.CreatedBy = string.Format("{0}{1}"
                    , string.IsNullOrEmpty(this.CreatedBy) ? string.Empty : this.CreatedBy
                    , string.IsNullOrEmpty(this.CreatedByName) ? string.Empty : string.Format($"({this.CreatedByName})"));
            this.UpdatedBy = string.Format("{0}{1}"
                    , string.IsNullOrEmpty(this.UpdatedBy) ? string.Empty : this.UpdatedBy
                    , string.IsNullOrEmpty(this.UpdatedByName) ? string.Empty : string.Format($"({this.UpdatedByName})"));

            return this;
        }
        
    }

}
