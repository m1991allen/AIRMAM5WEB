//using AIRMAM5.DBEntity.DBEntity;
using Newtonsoft.Json;

namespace AIRMAM5.DBEntity.Models.Template
{
    /// <summary>
    /// 自訂樣版:新增/複製 新建Model 。 繼承參考 <see cref="TemplateNewCopyModel"/>
    /// </summary>
    public class TemplateCreateModel : TemplateNewCopyModel
    {
        /// <summary>
        /// 自訂樣版:新增/複製 新建Model 
        /// </summary>
        public TemplateCreateModel() : base() { }

        /* marked_&_modified_20211007 (未曾被使用 */
        ///// <summary>
        ///// [tbmTEMPLATE]樣版資料填入 TemplateCreateModel()
        ///// </summary>
        ///// <param name="m">自訂樣板(主)資料表 </param>
        ///// <param name="choose">新增樣版選擇別: NEW或COPY, 預設=NEW </param>
        //public TemplateCreateModel(tbmTEMPLATE m, string choose = "NEW")
        //{
        //    fnTEMP_ID = m.fnTEMP_ID;
        //    fsNAME = m.fsNAME;
        //    fsTABLE = m.fsTABLE;
        //    IsSearch = m.IsSearch;
        //    fsDESCRIPTION = m.fsDESCRIPTION;
        //    Template = choose;
        //}

        /// <summary>
        /// NEW全新樣板 或 COPY複製樣板
        /// </summary>
        [JsonProperty(PropertyName = "template")]
        public string Template { get; set; } = string.Empty;

        /// <summary>
        /// 既有樣板編號fnTEMP_ID(New樣板則為0)
        /// </summary>
        [JsonProperty(PropertyName = "existtemplate")]
        public int ExistTemplate { get; set; } = 0;
    }

}
