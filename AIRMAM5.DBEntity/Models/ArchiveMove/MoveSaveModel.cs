
namespace AIRMAM5.DBEntity.Models.ArchiveMove
{
    /// <summary>
    /// 檔案搬移存檔 Model
    /// </summary>
    public class MoveSaveModel
    {
        /// <summary>
        /// 搬移目的地的主題編號
        /// </summary>
        public string TargetSubjId { get; set; }

        /// <summary>
        /// 媒資類別: A聲音, D文件, P圖片, S主題, V影片
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 搬移的檔案編號 Array
        /// </summary>
        public string[] MoveFileNos { get; set; }
    }

}
