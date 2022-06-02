using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.ArcPre
{
    /// <summary>
    /// 預編詮釋: 編輯存檔POST Model。 繼承參考 <see cref="ArcPreMainModel"/>
    /// </summary>
    public class ArcPrePostModel : ArcPreMainModel
    {
        //public ArcPrePostModel() { }

        /// <summary>
        /// 樣板欄位內容  Attribute List(TemplateFields)
        /// </summary>
        //public List<spGET_ARC_PRE_ATTRIBUTE_Result> ArcPreAttributes { get; set; }

        public List<ArcPreAttributePostModel> ArcPreAttributes { get; set; } = new List<ArcPreAttributePostModel>();
    }

}
