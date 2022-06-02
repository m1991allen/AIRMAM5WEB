
namespace AIRMAM5.DBEntity.Models.ArchiveMove
{
    /// <summary>
    /// 目錄節點搬移存檔 MODEL
    /// </summary>
    public class DirMoveSaveModel
    {
        /// <summary>
        /// 搬移目的地的 父層(目錄)編號
        /// </summary>
        public long TargetParentId { get; set; }
        
        /// <summary>
        /// 搬移的目錄節點編號
        /// </summary>
        public long MoveDirId { get; set; }

    }

}
