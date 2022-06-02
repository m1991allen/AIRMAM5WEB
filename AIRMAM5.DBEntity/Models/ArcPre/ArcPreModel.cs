using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.ArcPre
{
    /// <summary>
    /// 預編詮釋: 編輯Model。 繼承參考 <see cref="ArcPreMainModel"/>
    /// </summary>
    public class ArcPreModel : ArcPreMainModel
    {
        /// <summary>
        /// 預編詮釋: 編輯Model
        /// </summary>
        public ArcPreModel() { }

        /// <summary>
        /// 樣板欄位內容  Attribute List(TemplateFields)
        /// </summary>
        //public List<spGET_ARC_PRE_ATTRIBUTE_Result> ArcPreAttributes { get; set; }
        public List<ArcPreAttributeModel> ArcPreAttributes { get; set; } = new List<ArcPreAttributeModel>();

        /// <summary>
        /// 資料格式<typeparamref name="T"/>轉換 
        /// </summary>
        /// <typeparam name="T">資料型別參數 </typeparam>
        /// <param name="data">資料 </param>
        /// <returns></returns>
        public ArcPreModel ConvertData<T>(T data)
        {
            if (data == null) { return this; }

            this.ArcPreAttributes = new List<ArcPreAttributeModel>();
            var properties = typeof(T).GetProperties();

            foreach (var pp in properties)
            {
                var val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fnPRE_ID")
                {
                    if (long.TryParse(val.ToString(), out long idx)) { this.fnPRE_ID = idx; }
                }

                if (pp.Name == "fsNAME") { fsNAME = val.ToString(); }
                if (pp.Name == "fsTYPE") { fsTYPE = val.ToString(); }
                if (pp.Name == "fsTYPE_NAME") { fsTYPE_NAME = val.ToString(); }
                if (pp.Name == "fnTEMP_ID")
                {
                    if (int.TryParse(val.ToString(), out int idx)) { this.fnTEMP_ID = idx; }
                }

                if (pp.Name == "fsTEMP_NAME") { fsTEMP_NAME = val.ToString(); }
                if (pp.Name == "fsTITLE") { fsTITLE = val.ToString(); }
                if (pp.Name == "fsDESCRIPTION") { fsDESCRIPTION = val.ToString(); }
                //20211123_ADDED)_自訂標籤, TIP: ^為分隔符號。
                if (pp.Name == "fsHASH_TAG")
                {
                    var b = val.ToString().Replace("#", "").Split(new char[] { '^' });
                    this.HashTag = b;
                    this.fsHashTag = val.ToString();
                }
            }

            return this;
        }
    }

}
