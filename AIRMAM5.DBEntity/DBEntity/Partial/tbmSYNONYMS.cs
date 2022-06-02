using AIRMAM5.DBEntity.Models;
using AIRMAM5.DBEntity.Models.Synonym;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 同義詞資料表 tbmSYNONYMS
    /// </summary>
    //[MetadataType(typeof(tbmSYNONYMSMetadata))]
    public partial class tbmSYNONYMS
    {
        public tbmSYNONYMS()
        {
            fnINDEX_ID = 0;
            fsTEXT_LIST = string.Empty;
            fsTYPE = string.Empty;
            fsNOTE = string.Empty;
            fdCREATED_DATE = DateTime.Now;
            fsCREATED_BY = string.Empty;
            fdUPDATED_DATE = null;
            fsUPDATED_BY = string.Empty;
        }

        /* marked_&_modified_20211007 */
        //public tbmSYNONYMS(SynonymCreateModel m)
        //{
        //    fnINDEX_ID = 0;
        //    fsTEXT_LIST = m.fsTEXT_LIST;
        //    fsTYPE = m.fsTYPE ?? string.Empty;
        //    fsNOTE = m.fsNOTE ?? string.Empty;
        //    fdCREATED_DATE = DateTime.Now;
        //    fsCREATED_BY = string.Empty;
        //    fdUPDATED_DATE = null;
        //    fsUPDATED_BY = string.Empty;
        //}

        /* marked_&_modified_20211007 (也未被參考使用 */
        ///// <summary>
        ///// 預存spGET_SYNONYMS_Result 結果轉 tbmSYNONYMS
        ///// </summary>
        ///// <param name="m"></param>
        //public tbmSYNONYMS(spGET_SYNONYMS_Result m)
        //{
        //    fnINDEX_ID = m.fnINDEX_ID;
        //    fsTEXT_LIST = m.fsTEXT_LIST;
        //    fsTYPE = m.fsTYPE ?? string.Empty;
        //    fsNOTE = m.fsNOTE ?? string.Empty;
        //    fdCREATED_DATE = m.fdCREATED_DATE;
        //    fsCREATED_BY = m.fsCREATED_BY;
        //    fdUPDATED_DATE = m.fdUPDATED_DATE;
        //    fsUPDATED_BY = m.fsUPDATED_BY;
        //}

        /// <summary>
        /// 同義詞資料表Metadata tbmSYNONYMS Metadata
        /// </summary>
        public class tbmSYNONYMSMetadata
        {
            [Display(Name = "序號")]
            public long fnINDEX_ID { get; set; }
            [Display(Name = "同義詞組")]
            public string fsTEXT_LIST { get; set; }
            [Display(Name = "分類")]
            public string fsTYPE { get; set; }
            [Display(Name = "備註")]
            public string fsNOTE { get; set; }

            [Display(Name = "建立時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime fdCREATED_DATE { get; set; }
            [Display(Name = "建立帳號")]
            public string fsCREATED_BY { get; set; }
            [Display(Name = "最後異動時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }
            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }
        }

        /// <summary>
        /// 同義詞資料表 資料轉換
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="data"> </param>
        public void ConvertGet<T>(T data)
        {
            if (data != null)
            {
                var properties = typeof(T).GetProperties();

                foreach (var pp in properties)
                {
                    var val = pp.GetValue(data) ?? string.Empty;

                    if (pp.Name == "fnINDEX_ID" || pp.Name.ToUpper() == "INDEXID")
                    {
                        if (long.TryParse(val.ToString(), out long idx)) { this.fnINDEX_ID = idx; }
                    }

                    if (pp.Name == "fsTEXT_LIST" || pp.Name.ToUpper() == "TEXTLIST") { this.fsTEXT_LIST = val.ToString(); }
                    if (pp.Name == "fsTYPE" || pp.Name == "AnnType") { this.fsTYPE = val.ToString(); }
                    if (pp.Name == "fsNOTE" || pp.Name == "AnnNote") { this.fsNOTE = val.ToString(); }

                    if (pp.Name == "fdCREATED_DATE" || pp.Name == "CreatedDate")
                    {
                        if (DateTime.TryParse(val.ToString(), out DateTime dt))
                        {
                            this.fdCREATED_DATE = dt;
                        }
                    }
                    if (pp.Name == "fsCREATED_BY" || pp.Name == "CreatedBy") { this.fsCREATED_BY = val.ToString(); }

                    if (pp.Name == "fdUPDATED_DATE" || pp.Name == "UpdatedDate")
                    {
                        if (DateTime.TryParse(val.ToString(), out DateTime dt))
                        {
                            this.fdUPDATED_DATE = dt;
                        }
                    }
                    if (pp.Name == "fsUPDATED_BY" || pp.Name == "UpdatedBy") { this.fsUPDATED_BY = val.ToString(); }
                }
            }
        }
    }
}
