using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.CodeSet
{
    /// <summary>
    /// (系統/自訂)主代碼 設定子代碼頁 ViewModel
    /// </summary>
    public class CodeEditViewModel
    {
        /// <summary>
        /// 代碼主檔
        /// </summary>
        public CodeSetEditModel CodeSet { get; set; } = new CodeSetEditModel();

        /// <summary>
        /// 子代碼單筆
        /// </summary>
        public CodeDataModel Code { get; set; } = new CodeDataModel();

        /// <summary>
        /// 代碼主檔.子檔清單
        /// </summary>
        public List<CodeDataModel> CodeList { get; set; } = new List<CodeDataModel>();
    }

}
