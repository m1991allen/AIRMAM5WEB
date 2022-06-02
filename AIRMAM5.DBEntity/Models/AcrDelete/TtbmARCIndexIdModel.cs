
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.AcrDelete
{
    /// <summary>
    /// 媒資刪除記錄主表Id
    /// </summary>
    public class TtbmARCIndexIdModel
    {
        /// <summary>
        /// 編號 fnINDEX_ID (媒資刪除記錄)
        /// </summary>
        [Display(Name = "編號")]
        public long IndexId { get; set; }
    }

    /* 刪除(媒資檔案)記錄表
     * t_tbmARC_INDEX(主檔
     * t_tbmARC_VIDEO 
     * t_tbmARC_AUDIO
     * t_tbmARC_PHOTO
     * t_tbmARC_DOC
     * 
     * */

}
