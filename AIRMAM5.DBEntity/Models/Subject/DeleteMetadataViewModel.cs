using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Subject
{
    /// <summary>
    /// 刪除-影音圖文 Metadata Model
    /// </summary>
    public class DeleteMetadataViewModel
    {
        /// <summary>
        /// 刪除-影音圖文 Metadata Model
        /// </summary>
        public DeleteMetadataViewModel() { }

        /// <summary>
        /// 刪除-影音圖文 Metadata Model
        /// </summary>
        /// <param name="no">檔案編號 </param>
        /// <param name="type">檔案類型 </param>
        /// <param name="reson">刪除原因 </param>
        public DeleteMetadataViewModel(string no, string type, string reson)
        {
            this.FileCategory = type;
            this.FileNo = no;
            this.Reason = reson;
        }

        #region >>>>>欄位參數
        /// <summary>
        /// 檔案編號
        /// </summary>
        [Display(Name = "檔案編號")]
        public string FileNo { set; get; }

        /// <summary>
        /// 媒體檔案分類: V, A, P, D
        /// </summary>
        [Display(Name = "檔案類型")]
        public string FileCategory { get; set; }

        /// <summary>
        /// 刪除原因
        /// </summary>
        [Display(Name = "刪除原因")]
        public string Reason { get; set; } = string.Empty;
        #endregion
    }

}
