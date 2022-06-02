
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Subject
{
    /// <summary>
    /// 段落描述_新增/編輯 ViewMODEL
    /// </summary>
    public class ParagraphCUViewModel
    {
        /// <summary>
        /// 段落描述_新增/編輯 ViewMODEL
        /// </summary>
        public ParagraphCUViewModel() { }

        #region >>>>>欄位參數
        /// <summary>
        /// 檔案類型: A聲音, D文件, P圖片, S主題, V影片 FileCategory
        /// </summary>
        [Display(Name = "檔案類型")]
        public string FileCategory { get; set; } = string.Empty;

        /// <summary>
        /// 檔案編號
        /// </summary>
        public string fsFILE_NO { set; get; } = string.Empty;

        /// <summary>
        /// 流水號 fnSEQ_NO
        /// </summary>
        public int SeqNo { set; get; } = 0;

        /// <summary>
        /// 開始時間 (ex: 4.521)
        /// </summary>
        public decimal BegTime { get; set; } = 0M;

        /// <summary>
        /// 結束時間 (ex: 27.879)
        /// </summary>
        public decimal EndTime { get; set; } = 0M;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
        #endregion
    }

}
