using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.CodeSet
{
    /// <summary>
    /// 代碼主表 檢視 MODEL。　繼承參考 <see cref="CodeSetEditModel"/>
    /// </summary>
    public class CodeSetViewModel : CodeSetEditModel
    {
        /// <summary>
        /// 代碼主表 檢視 MODEL
        /// </summary>
        public CodeSetViewModel() { }

        /* Marked_BY_20211012 */
        ///// <summary>
        ///// 代碼主表 檢視 MODEL
        ///// </summary>
        ///// <param name="m">預存回覆資料 <see cref="spGET_CODE_SET_Result"/> </param>
        //public CodeSetViewModel(spGET_CODE_SET_Result m)
        //{
        //    fsCODE_ID = m.fsCODE_ID;
        //    fsTITLE = m.fsTITLE;
        //    fsNOTE = m.fsNOTE ?? string.Empty;
        //    IsEnabled = m.fsIS_ENABLED.ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
        //    fdCREATED_DATE = m.fdCREATED_DATE;
        //    fsCREATED_BY = //m.fsCREATED_BY;
        //        string.Format("{0}{1}"
        //        , string.IsNullOrEmpty(m.fsCREATED_BY) ? string.Empty : m.fsCREATED_BY
        //        , string.IsNullOrEmpty(m.fsCREATED_BY_NAME) ? string.Empty : string.Format($"({m.fsCREATED_BY_NAME})"));

        //    fsCREATED_BY_NAME = m.fsCREATED_BY_NAME ?? string.Empty;
        //    fdUPDATED_DATE = m.fdUPDATED_DATE;
        //    fsUPDATED_BY = //m.fsUPDATED_BY ?? string.Empty;
        //        string.Format("{0}{1}"
        //        , string.IsNullOrEmpty(m.fsUPDATED_BY) ? string.Empty : m.fsUPDATED_BY
        //        , string.IsNullOrEmpty(m.fsUPDATED_BY_NAME) ? string.Empty : string.Format($"({m.fsUPDATED_BY_NAME})"));

        //    fsUPDATED_BY_NAME = m.fsUPDATED_BY_NAME ?? string.Empty;
        //}

        #region >>> 欄位參數
        [Display(Name = "新增時間")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime fdCREATED_DATE { get; set; }

        [Display(Name = "新增人員")]
        public string fsCREATED_BY { get; set; } = string.Empty;

        [Display(Name = "修改時間")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? fdUPDATED_DATE { get; set; }

        [Display(Name = "修改人員")]
        public string fsUPDATED_BY { get; set; } = string.Empty;

        /// <summary>
        /// 建立者角色群組
        /// </summary>
        public string fsCREATED_BY_NAME { get; set; } = string.Empty;

        /// <summary>
        /// 最後異動者角色群組
        /// </summary>
        public string fsUPDATED_BY_NAME { get; set; } = string.Empty;
        #endregion

        /// <summary>
        /// 型別<typeparamref name="T"/> 資料轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 </typeparam>
        /// <param name="data">資料 </param>
        /// <returns></returns>
        public new CodeSetViewModel ConvertData<T>(T data)
        {
            if (data == null) { return this; }

            var properties = typeof(T).GetProperties();

            foreach (var pp in properties)
            {
                var val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fsCODE_ID" || pp.Name.ToUpper() == "CODEID") { this.fsCODE_ID = val.ToString(); }

                if (pp.Name == "fsTITLE" || pp.Name.ToUpper() == "TITLE") { this.fsTITLE = val.ToString(); }

                if (pp.Name == "fsNOTE" || pp.Name.ToUpper() == "NOTE") { this.fsNOTE = val.ToString(); }

                if (pp.Name == "fsIS_ENABLED" || pp.Name.ToUpper() == "ISENABLED")
                {
                    if (bool.TryParse(val.ToString(), out bool chk)) { this.IsEnabled = chk; }
                }

                if (pp.Name == "fdCREATED_DATE" || pp.Name.ToUpper() == "CREATEDDATE")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt)) { this.fdCREATED_DATE = dt; }
                }
                if (pp.Name == "fsCREATED_BY" || pp.Name.ToUpper() == "CREATEDBY") { this.fsCREATED_BY = val.ToString(); }
                if (pp.Name == "fsCREATED_BY_NAME" || pp.Name.ToUpper() == "CREATEDBYNAME") { this.fsCREATED_BY = val.ToString(); }

                if (pp.Name == "fdUPDATED_DATE" || pp.Name.ToUpper() == "UPDATEDDATE")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt)) { this.fdUPDATED_DATE = dt; }
                }
                if (pp.Name == "fsUPDATED_BY" || pp.Name.ToUpper() == "UPDATEBY") { this.fsUPDATED_BY = val.ToString(); }
                if (pp.Name == "fsUPDATED_BY_NAME" || pp.Name.ToUpper() == "UPDATEDBYNAME") { this.fsUPDATED_BY_NAME = val.ToString(); }
            }

            this.fsCREATED_BY = string.Format("{0}{1}"
                , string.IsNullOrEmpty(this.fsCREATED_BY) ? string.Empty : this.fsCREATED_BY
                , string.IsNullOrEmpty(this.fsCREATED_BY_NAME) ? string.Empty : string.Format($"({this.fsCREATED_BY_NAME})"));
            this.fsUPDATED_BY = string.Format("{0}{1}"
                , string.IsNullOrEmpty(this.fsUPDATED_BY) ? string.Empty : this.fsUPDATED_BY
                , string.IsNullOrEmpty(this.fsUPDATED_BY_NAME) ? string.Empty : string.Format($"({this.fsUPDATED_BY_NAME})"));

            return this;
        }
    }

}
