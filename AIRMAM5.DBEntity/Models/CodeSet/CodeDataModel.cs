using AIRMAM5.DBEntity.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.CodeSet
{
    /// <summary>
    /// 子代碼
    /// </summary>
    public class CodeDataModel
    {
        public CodeDataModel() { }

        #region >>>>> 欄位參數
        /// <summary>
        /// 代碼主表.代碼 fsCODE_ID
        /// </summary>
        [Display(Name = "主代碼")]
        public string fsCODE_ID { get; set; }
        /// <summary>
        /// 代碼子表.子代碼 fsCODE
        /// </summary>
        [Display(Name = "子代碼")]
        public string fsCODE { get; set; }
        /// <summary>
        /// 代碼子表.子代碼名稱 fsNAME
        /// </summary>
        [Display(Name = "代碼名稱")]
        public string fsNAME { get; set; } = string.Empty;
        /// <summary>
        /// 子代碼英文名稱 fsENAME
        /// </summary>
        [Display(Name = "英文名稱")]
        public string fsENAME { get; set; } = string.Empty;
        /// <summary>
        /// 排序 fnORDER
        /// </summary>
        [Display(Name = "排序")]
        public int fnORDER { get; set; } = 99;
        /// <summary>
        /// 備註 fsNOTE
        /// </summary>
        [Display(Name = "備註")]
        public string fsNOTE { get; set; } = string.Empty;
        /// <summary>
        /// 設定 fsSET
        /// </summary>
        [Display(Name = "設定")]
        public string fsSET { get; set; } = string.Empty;
        /// <summary>
        /// 子代碼是否可選
        /// </summary>
        [Display(Name = "可選")]
        public bool IsEnabled { get; set; } = true;
        #endregion

        /// <summary>
        /// 指定T 回傳CodeDataModel, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <param name="m"> 資料來源 </param>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_CODE_Result"/>, <see cref="tbzCODE"/> </typeparam>
        /// <returns></returns>
        public CodeDataModel FormatConversion<T>(T m)
        {
            if (m == null) { return this; }

            var _properties = typeof(T).GetProperties();

            foreach (var p in _properties)
            {
                var _val = p.GetValue(m) ?? string.Empty;
                if (p.Name == "fsCODE_ID") { this.fsCODE_ID = _val.ToString(); }
                if (p.Name == "fsCODE") { this.fsCODE = _val.ToString(); }
                if (p.Name == "fsNAME") { this.fsNAME = _val.ToString(); }
                if (p.Name == "fsENAME") { this.fsENAME = _val.ToString(); }
                if (p.Name == "fnORDER")
                {
                    int.TryParse(_val.ToString(), out int v);
                    this.fnORDER = v;
                }
                if (p.Name == "fsNOTE") { this.fsNOTE = _val.ToString(); }
                if (p.Name == "fsSET") { this.fsSET = _val.ToString(); }
                if (p.Name == "fsIS_ENABLED")
                {
                    this.IsEnabled = _val.ToString() == IsTrueFalseEnum.Y.ToString() ? true : false;
                }
            }

            return this;
        }
    }

}
