using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Synonym
{
    /// <summary>
    /// 同義詞 編輯 ViewModel
    /// </summary>
    public class SynonymEditModel : SynonymCreateModel
    {
        //static CodeService _ser = new CodeService();

        public SynonymEditModel()
        {
            //SynonymTypeList = _ser.GetCodeItemList(Enums.TbzCodeIdEnum.SYNO_TYPE.ToString());
            //SynonymTypeList.Insert(0, new SelectListItem { Value = "", Text = "請選擇分類" });
            //SynonymTypeList.Insert(1, new SelectListItem { Value = "*", Text = "全部" });
        }

        ///// <summary>
        ///// 編輯 同義詞
        ///// </summary>
        //public SynonymEditModel(tbmSYNONYMS m) : base()
        //{
        //    fnINDEX_ID = m.fnINDEX_ID;
        //    fsTEXT_LIST = m.fsTEXT_LIST;
        //    fsTYPE = m.fsTYPE;
        //    fsNOTE = m.fsNOTE;
        //}

        #region >>>>>欄位參數
        //fsTEXT_LIST
        //fsTYPE
        //fsNOTE
        //SynonymTypeList

        /// <summary>
        /// 同義詞ID
        /// </summary>
        [Display(Name = "ID")]
        public long fnINDEX_ID { get; set; }

        /// <summary>
        /// 編輯前的同義詞字串
        /// </summary>
        [Display(Name = "同義詞詞彙")]
        public string OrigSynonyms { get; set; }
        #endregion

        public SynonymEditModel ConvertData<T>(T data)
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

}
