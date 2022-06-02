
namespace AIRMAM5.DBEntity.Models.ArchiveMove
{
    /// <summary>
    /// 主題搬移存檔 Model
    /// </summary>
    public class SubjMoveSaveModel
    {
        /// <summary>
        /// 搬移目的地的 目錄節點編號
        /// </summary>
        public long TargetDirId { get; set; }

        /// <summary>
        /// 搬移的主題編號 Array
        /// </summary>
        public string[] MoveSubjIds { get; set; }
    }

}
