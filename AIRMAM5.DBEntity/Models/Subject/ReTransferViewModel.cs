using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Subject
{
    /// <summary>
    /// 影片,聲音 重轉檔。　繼承參考 <see cref="SubjFileNoModel"/> 。
    /// </summary>
    public class ReTransferViewModel : SubjFileNoModel
    {
        /// <summary>
        /// 分類: A聲音, D文件, P圖片, S主題, V影片 FileCategory
        /// </summary>
        [Display(Name = "檔案類型")]
        public string FileCategory { get; set; } = string.Empty;
    }
}
