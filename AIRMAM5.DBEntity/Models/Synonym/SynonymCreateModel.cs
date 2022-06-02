using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Synonym
{
    /// <summary>
    /// 新建 同義詞
    /// </summary>
    public class SynonymCreateModel
    {
        //static CodeService _ser = new CodeService();

        /// <summary>
        /// 新建 同義詞
        /// </summary>
        public SynonymCreateModel()
        {
            //SynonymTypeList = _ser.GetCodeItemList(Enums.TbzCodeIdEnum.SYNO_TYPE.ToString());
            //SynonymTypeList.Insert(0, new SelectListItem { Value = "", Text = "請選擇分類" });
            //SynonymTypeList.Insert(1, new SelectListItem { Value = "*", Text = "全部" });
        }

        #region >>>>>欄位參數
        /// <summary>
        /// 同義詞字串(以;分隔)
        /// </summary>
        [Display(Name = "同義詞詞彙")]
        public string fsTEXT_LIST { get; set; } = string.Empty;

        /// <summary>
        /// 分類
        /// </summary>
        [Display(Name = "分類")]
        public string fsTYPE { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [Display(Name = "備註")]
        public string fsNOTE { get; set; } = string.Empty;

        /// <summary>
        /// 同義詞分類選單
        /// </summary>
        [Display(Name = "分類")]
        public List<SelectListItem> SynonymTypeList { get; set; }
        #endregion
    }

}
