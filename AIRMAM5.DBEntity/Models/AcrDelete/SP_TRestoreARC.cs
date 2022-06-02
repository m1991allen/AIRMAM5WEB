
namespace AIRMAM5.DBEntity.Models.AcrDelete
{
    /// <summary>
    /// 回復媒體檔案 參數。繼承參考 <see cref="TtbmARCIndexIdModel"/>
    /// </summary>
    public class SP_TRestoreARC : TtbmARCIndexIdModel
    {
        /// <summary>
        /// 執行人員帳號
        /// </summary>
        public string UseNameBy { get; set; }
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
