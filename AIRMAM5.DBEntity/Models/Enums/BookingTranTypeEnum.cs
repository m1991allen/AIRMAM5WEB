using AIRMAM5.DBEntity.DBEntity;
using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// 調用檔案樣版.轉檔類別 <see cref="tbmBOOKING_T.fsTRAN_TYPE"/>
    /// </summary>
    public enum BookingTranTypeEnum
    {
        /// <summary>
        /// AVID調用
        /// </summary>
        [Description("AVID調用")]
        AVID,
        /// <summary>
        /// 一般調用
        /// </summary>
        [Description("一般調用")]
        BOOKING,
        /// <summary>
        /// 批次調用
        /// </summary>
        [Description("批次調用")]
        BOOKING_BATCH,

        /// <summary>
        /// 複製調用  (spGET_BOOKING_TODAY_TOP_10 有用到)
        /// </summary> 
        [Description("複製調用")]
        COPYFILE,
        /// <summary>
        /// NAS調用 (spGET_BOOKING_TODAY_TOP_10 有用到, 被歸類到'複製調用')
        /// </summary>
        NAS,
        /// <summary>
        /// DUPLICATE調用 (spGET_BOOKING_TODAY_TOP_10 有用到, 被歸類到'AVID調用')
        /// </summary>
        DUPLICATE,
    }
}
