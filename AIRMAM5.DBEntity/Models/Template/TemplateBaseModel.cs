using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Template
{
    /// <summary>
    /// 自訂樣板基本資訊Model。 繼承參考 <see cref="TemplateSearchModel"/>
    /// </summary>
    public class TemplateBaseModel : TemplateSearchModel
    {
        /// <summary>
        /// 自訂樣板基本資訊Model
        /// </summary>
        public TemplateBaseModel() { }

        /*  Modified_20210903: 改 DataConvert<T>(T data) */
        ///// <summary>
        ///// 自訂樣板基本資訊Model
        ///// </summary>
        //public TemplateBaseModel(spGET_TEMPLATE_Result m)
        //{
        //    fnTEMP_ID = m.fnTEMP_ID;
        //    fsNAME = m.fsNAME;
        //    fsTABLE = m.fsTABLE;
        //    IsSearch = m.fcIS_SEARCH.ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
        //    SearchType = fsTABLE == FileTypeEnum.V.ToString() ? SearchTypeEnum.Video_DEV.ToString()
        //        : fsTABLE == FileTypeEnum.A.ToString() ? SearchTypeEnum.Audio_DEV.ToString()
        //            : fsTABLE == FileTypeEnum.P.ToString() ? SearchTypeEnum.Photo_DEV.ToString()
        //                : fsTABLE == FileTypeEnum.D.ToString() ? SearchTypeEnum.Doc_DEV.ToString()
        //                    : fsTABLE == FileTypeEnum.S.ToString() ? SearchTypeEnum.Subject_DEV.ToString() : SearchTypeEnum.Audio_DEV.ToString();
        //}

        #region >>>欄位參數
        /// <summary>
        /// 樣板名稱
        [Required]
        [Display(Name = "樣板名稱")]
        public string fsNAME { get; set; } = string.Empty;

        /// <summary>
        /// 是否進階查詢 [fcIS_SEARCH]
        /// </summary>
        [Display(Name = "進階查詢")]
        public bool IsSearch { get; set; } = false;

        /// <summary>
        /// 樣板類別對應的 檢索類別(SearchTypeEnum) : 
        /// </summary>
        public string SearchType { get; set; }
        #endregion

        /// <summary>
        /// 自訂樣板基本資訊Model - 資料格式轉換
        /// </summary>
        /// <typeparam name="T">來源資料 類型Model eg. <see cref="spGET_TEMPLATE_Result"/> </typeparam>
        /// <param name="data">來源資料 內容Data </param>
        /// <returns></returns>
        public TemplateBaseModel DataConvert<T>(T data)
        {
            if (data == null) { return this; }

            var properties = typeof(T).GetProperties();
            foreach (var pp in properties)
            {
                object val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fnTEMP_ID")
                {
                    if(int.TryParse(val.ToString(), out int idx)) { fnTEMP_ID = idx; }
                }

                if (pp.Name == "fsNAME") { fsNAME = val.ToString(); }
                if (pp.Name == "fsTABLE")
                {
                    this.fsTABLE = val.ToString();

                    SearchType = this.fsTABLE == FileTypeEnum.V.ToString() ? SearchTypeEnum.Video_DEV.ToString()
                        : fsTABLE == FileTypeEnum.A.ToString() ? SearchTypeEnum.Audio_DEV.ToString()
                            : fsTABLE == FileTypeEnum.P.ToString() ? SearchTypeEnum.Photo_DEV.ToString()
                                : fsTABLE == FileTypeEnum.D.ToString() ? SearchTypeEnum.Doc_DEV.ToString()
                                    : fsTABLE == FileTypeEnum.S.ToString() ? SearchTypeEnum.Subject_DEV.ToString() 
                                        : SearchTypeEnum.Audio_DEV.ToString();
                }
                if (pp.Name == "fcIS_SEARCH")
                {
                    IsSearch = val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
                }

            }

            return this;
        }
    }

}
