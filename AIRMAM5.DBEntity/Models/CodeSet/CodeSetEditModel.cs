using AIRMAM5.DBEntity.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.CodeSet
{
    /// <summary>
    /// 代碼主表 編輯/新增 MODEL
    /// </summary>
    public class CodeSetEditModel
    {
        /// <summary>
        /// 代碼主表 編輯/新增 MODEL
        /// </summary>
        public CodeSetEditModel() { }

        #region >>> 欄位參數
        /// <summary>
        /// 代碼項目(主檔:代碼Id)
        /// </summary>
        [Display(Name = "代碼編號")]
        public string fsCODE_ID { get; set; } = string.Empty;
        /// <summary>
        /// 代碼項目名稱
        /// </summary>
        [Display(Name = "名稱")]
        public string fsTITLE { get; set; } = string.Empty;

        /// <summary>
        /// 備註
        /// </summary>
        [Display(Name = "備註")]
        public string fsNOTE { get; set; } = string.Empty;

        /// <summary>
        /// 代碼主表.代碼是否有效
        /// </summary>
        [Display(Name = "是否啟用")]
        public bool IsEnabled { get; set; } = true;
        #endregion

        /// <summary>
        /// 代碼主表 編輯/新增 MODEL - 資料格式轉換 
        /// </summary>
        /// <typeparam name="T">來源資料 類型Model  <see cref="spGET_CODE_SET_Result"/> </typeparam>
        /// <param name="data">來源資料 內容Data </param>
        /// <returns></returns>
        public CodeSetEditModel ConvertData<T>(T data)
        {
            if (data == null) { return this; }

            var properites = typeof(T).GetProperties();
            foreach (var pp in properites)
            {
                var val = pp.GetValue(data);

                if (pp.Name == "fsCODE_ID") { fsCODE_ID = val.ToString(); }
                if (pp.Name == "fsTITLE") { fsTITLE = val.ToString(); }
                if (pp.Name == "fsNOTE") { fsNOTE = val.ToString(); }
                if (pp.Name == "fsIS_ENABLED")
                {
                    IsEnabled = val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
                }
            }

            return this;
        }
    }
}
