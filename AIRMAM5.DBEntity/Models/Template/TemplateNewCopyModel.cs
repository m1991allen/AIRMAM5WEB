using AIRMAM5.DBEntity.DBEntity;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Template
{
    // 【_Copy.cshtml】

    /// <summary>
    /// 自訂樣版:新增/複製 選擇Model (_Copy.cshtml)。 繼承參考 <see cref="TemplateSearchModel"/>
    /// </summary>
    public class TemplateNewCopyModel : TemplateSearchModel
    {
        /// <summary>
        /// 自訂樣版:新增/複製 選擇Model (_Copy.cshtml)
        /// </summary>
        public TemplateNewCopyModel() { }

        /* marked_&_modified_20211007 */
        /// <summary>
        /// [tbmTEMPLATE]樣版資料填入 TemplateNewCopyModel()
        /// </summary>
        /// <param name="m"></param>
        public TemplateNewCopyModel(tbmTEMPLATE m)
        {
            fnTEMP_ID = m.fnTEMP_ID;
            fsTABLE = m.fsTABLE;
            fsNAME = m.fsNAME;
            fsDESCRIPTION = m.fsDESCRIPTION;
            IsSearch = m.IsSearch;
        }

        /// <summary>
        /// 自訂樣版:新增/複製 選擇Model (_Copy.cshtml)。  資料轉換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public virtual void ConvertData<T>(T data)
        {
            if (data != null)
            {
                var properties = typeof(T).GetProperties();

                foreach (var pp in properties)
                {
                    var val = pp.GetValue(data) ?? string.Empty;

                    if (pp.Name == "fnTEMP_ID" || pp.Name.ToUpper() == "TEMPID")
                    {
                        if (int.TryParse(val.ToString(), out int idx)) { this.fnTEMP_ID = idx; }
                    }

                    if (pp.Name == "fsTABLE" || pp.Name.ToUpper() == "TEMPTABLE" || pp.Name.ToUpper() == "TABLE")
                    { this.fsTABLE = val.ToString(); }

                    if (pp.Name == "fsNAME" || pp.Name.ToUpper() == "TEMPNAME")
                    { this.fsNAME = val.ToString(); }

                    if (pp.Name == "fsDESCRIPTION" || pp.Name.ToUpper() == "DESCRIPTION" || pp.Name.ToUpper() == "DESC")
                    { this.fsDESCRIPTION = val.ToString(); }

                    if (pp.Name == "IsSearch" || pp.Name.ToUpper() == "ISSEARCH")
                    {
                        if (bool.TryParse(val.ToString(), out bool chk)) { this.IsSearch = chk; }
                    }

                }
            }
        }

        #region >>>>> 欄位參數
        /// <summary>
        /// 樣板名稱
        [Required]
        [Display(Name = "樣板名稱")]
        public string fsNAME { get; set; } = string.Empty;

        /// <summary>
        /// 樣板描述
        /// </summary>
        [Display(Name = "樣板描述")]
        public string fsDESCRIPTION { get; set; } = string.Empty;

        /// <summary>
        /// 是否進階查詢 [fcIS_SEARCH]
        /// </summary>
        [Display(Name = "進階查詢")]
        public bool IsSearch { get; set; } = false;
        #endregion
    }

}
