
namespace AIRMAM5.DBEntity.Models.Material
{
    /// <summary>
    /// 調用紀錄 新增model。繼承參考 <see cref="BookingCreateBase"/>
    /// </summary>
    public class BookingCreateModel :BookingCreateBase
    {
        /// <summary>
        /// 調用紀錄 新增model
        /// </summary>
        public BookingCreateModel() { }

        #region >>>> 欄位參數
        /*20200904 *///-因應批次調用需求，調整新增調用檔案model
        ///// <summary>
        ///// 調用原因/樣板Id dbo.[tbmBOOKING_T].[fsID]
        ///// </summary>
        //public int ResonId { get; set; } = -1;
        ///// <summary>
        ///// 調用原因/樣板 dbo.[tbmBOOKING_T].[fsNAME] / dbo.[tbmBOOKING].[fsRESON]
        ///// </summary>
        //public string ResonStr { get; set; } = string.Empty;
        ///// <summary>
        ///// 調用描述 dbo.[tbmBOOKING].[fsDESCRIPT]
        ///// </summary>
        //public string DescStr { get; set; } = string.Empty;
        ///// <summary>
        ///// 轉出格式:影片 dbo.[tbmBOOKING].[fsPROFILE_V]
        ///// </summary>
        //public string ProfileVideo { get; set; } = string.Empty;
        ///// <summary>
        ///// 轉出格式:聲音 dbo.[tbmBOOKING].[fsPROFILE_A]
        ///// </summary>
        //public string ProfileAudio { get; set; } = string.Empty;
        ///// <summary>
        ///// 浮水印 dbo.[tbmBOOKING].[fsWATERMARK]
        ///// </summary>
        //public string WaterMark { get; set; } = string.Empty;
        ///// <summary>
        ///// 調用路徑 dbo.[tbmBOOKING].[fsPATH]
        ///// </summary>
        //public string PathStr { get; set; } = string.Empty;

        /// <summary>
        /// 調用編號s [fsMATERIAL_ID] (多筆以"^"為分隔符號
        /// </summary>
        public string MaterialIds { get; set; } = null;
        #endregion
    }

    /// <summary>
    /// 調用紀錄 新增model_Base
    /// </summary>
    /// <remarks> 20200904: 因應批次調用需求，調整新增調用檔案model，將相同參數設為共用model。
    /// </remarks>
    public class BookingCreateBase
    {
        /// <summary>
        /// 調用紀錄 新增model_Base
        /// </summary>
        public BookingCreateBase() { }

        #region >>>> 欄位參數
        /// <summary>
        /// 調用原因/樣板Id dbo.[tbmBOOKING_T].[fsID]
        /// </summary>
        public int ResonId { get; set; } = -1;

        /// <summary>
        /// 調用原因/樣板 dbo.[tbmBOOKING_T].[fsNAME] / dbo.[tbmBOOKING].[fsRESON]
        /// </summary>
        public string ResonStr { get; set; } = string.Empty;

        /// <summary>
        /// 調用描述 dbo.[tbmBOOKING].[fsDESCRIPT]
        /// </summary>
        public string DescStr { get; set; } = string.Empty;

        /// <summary>
        /// 轉出格式:影片 dbo.[tbmBOOKING].[fsPROFILE_V]
        /// </summary>
        public string ProfileVideo { get; set; } = string.Empty;

        /// <summary>
        /// 轉出格式:聲音 dbo.[tbmBOOKING].[fsPROFILE_A]
        /// </summary>
        public string ProfileAudio { get; set; } = string.Empty;

        /// <summary>
        /// 浮水印 dbo.[tbmBOOKING].[fsWATERMARK]
        /// </summary>
        public string WaterMark { get; set; } = string.Empty;

        /// <summary>
        /// 調用路徑 dbo.[tbmBOOKING].[fsPATH]
        /// </summary>
        public string PathStr { get; set; } = string.Empty;
        #endregion
    }
}
