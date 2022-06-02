using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Function
{
    /// <summary>
    /// 平台功能項目 檢視模型(Base)。　繼承參考 <see cref="FunctionIdModel"/>
    /// </summary>
    public class FunctionViewModel : FunctionIdModel
    {
        /// <summary>
        /// 平台功能項目 檢視模型(Base)
        /// </summary>
        public FunctionViewModel() : base() { }

        #region >>> 欄位參數
        /// <summary>
        /// fsNAME
        /// </summary>
        [Display(Name = "功能名稱")]
        public string FuncName { get; set; } = string.Empty;
        /// <summary>
        /// fsDESCRIPTION
        /// </summary>
        [Display(Name = "備註")]
        public string FuncDescription { get; set; } = string.Empty;
        /// <summary>
        /// fsTYPE
        /// </summary>
        [Display(Name = "類別")]
        public string FuncType { get; set; }
        /// <summary>
        /// fnORDER
        /// </summary>
        [Display(Name = "排序")]
        public int FuncOrder { get; set; }
        /// <summary>
        /// fsICON
        /// </summary>
        [Display(Name = "編號")]
        public string FuncIcon { get; set; }
        /// <summary>
        /// fsPARENT_ID
        /// </summary>
        [Display(Name = "父層")]
        public string ParentId { get; set; }
        /// <summary>
        /// fsHEADER
        /// </summary>
        [Display(Name = "Header")]
        public string Header { get; set; } = string.Empty;
        /// <summary>
        /// fsCONTROLLER
        /// </summary>
        [Display(Name = "Controller")]
        public string ControllerName { set; get; } = string.Empty;
        /// <summary>
        /// fsACTION
        /// </summary>
        [Display(Name = "ActionName")]
        public string ActionName { set; get; } = string.Empty;

        /// <summary>
        /// 功能項目是否可使用: true / false
        /// </summary>
        [Display(Name = "功能項目是否可使用")]
        public bool Usable { get; set; } = true;
        #endregion
    }

}
