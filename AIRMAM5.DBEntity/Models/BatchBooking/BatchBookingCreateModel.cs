using AIRMAM5.DBEntity.Models.Material;

namespace AIRMAM5.DBEntity.Models.BatchBooking
{
    /// <summary>
    /// 批次調用 送出存檔model 。繼承參考 <see cref="BookingCreateBase"/>
    /// </summary>
    public class BatchBookingCreateModel : BookingCreateBase
    {
        #region >>>> 欄位參數
        // 調用原因/樣板Id dbo.[tbmBOOKING_T].[fsID]
        //public int ResonId { get; set; } = -1;

        // 調用原因/樣板 dbo.[tbmBOOKING_T].[fsNAME] / dbo.[tbmBOOKING].[fsRESON]
        //public string ResonStr { get; set; } = string.Empty;

        // 調用描述 dbo.[tbmBOOKING].[fsDESCRIPT]
        //public string DescStr { get; set; } = string.Empty;

        // 轉出格式:影片 dbo.[tbmBOOKING].[fsPROFILE_V]
        //public string ProfileVideo { get; set; } = string.Empty;

        // 轉出格式:聲音 dbo.[tbmBOOKING].[fsPROFILE_A]
        //public string ProfileAudio { get; set; } = string.Empty;

        // 浮水印 dbo.[tbmBOOKING].[fsWATERMARK]
        //public string WaterMark { get; set; } = string.Empty;

        // 調用路徑 dbo.[tbmBOOKING].[fsPATH]
        //public string PathStr { get; set; } = string.Empty;

        /// <summary>
        /// 媒資檔案編號s [fsFILE_NOs] (多筆以"^"為分隔符號
        /// </summary>
        public string FileNos { get; set; } = null;
        #endregion
    }
}
