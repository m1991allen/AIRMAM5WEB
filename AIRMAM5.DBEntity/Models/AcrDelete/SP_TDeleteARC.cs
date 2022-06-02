
namespace AIRMAM5.DBEntity.Models.AcrDelete
{
    /// <summary>
    /// 刪除媒體實體檔案 參數。繼承參考 <see cref="TtbmARCIndexIdModel"/>
    /// </summary>
    public class SP_TDeleteARC : TtbmARCIndexIdModel
    {
        /// <summary>
        /// 媒體分類 : P,D,V,A
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 檔案編號
        /// </summary>
        public string FileNo { get; set; } = string.Empty;

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
