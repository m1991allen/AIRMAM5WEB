using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Subject;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.BatchBooking
{
    /// <summary>
    /// 批次調用:主題檔案列表 Model。 繼承參考 <see cref="SubjFileNoModel"/>
    /// </summary>
    public class BatchBookingShowListModel : SubjFileNoModel
    {
        /// <summary>
        /// 批次調用:主題檔案列表 Model
        /// </summary>
        public BatchBookingShowListModel() { }

        #region >>>>> 欄位參數 
        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 預覽圖(URL)
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 媒資檔案類別: A聲音, D文件, P圖片, S主題, V影片
        /// </summary>
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// 版權 [fsLICENSE] 搭配'版權代碼表 dbo.[tbmLICENSE]'
        /// </summary>
        /// <remarks> Added_20210909 </remarks>
        [Display(Name = "版權")]
        public string LicenseCode { get; set; } = string.Empty;
        /// <summary>
        /// 版權 LicenseCode中文
        /// </summary>
        /// <remarks> Added_20210909 </remarks>
        [Display(Name = "版權")]
        public string LicenseStr { get; set; } = string.Empty;
        /// <summary>
        /// 版權.是否調用提醒
        /// </summary>
        /// <remarks> Added_20210914 </remarks>
        [Display(Name = "是否調用提醒")]
        public bool IsAlert { get; set; }
        /// <summary>
        /// 版權.是否調用禁止
        /// </summary>
        /// <remarks> Added_20210914 </remarks>
        [Display(Name = "是否調用禁止")]
        public bool IsForBid { get; set; }
        /// <summary>
        /// 版權.提醒訊息內容
        /// </summary>
        /// <remarks> Added_20210914 </remarks>
        [Display(Name = "提醒訊息")]
        public string LicenseMessage { get; set; } = string.Empty;
        /// 版權.是否到期
        /// </summary>
        /// <remarks> Added_20211005 </remarks>
        public bool IsExpired { get; set; }
        /// <summary>
        /// 版權.授權到期日期
        /// </summary>
        /// <remarks> Added_20211005 </remarks>
        public DateTime? LicenseEndDate { get; set; }
        #endregion

    }

    /// <summary>
    /// 批次調用:主題檔案列表 Model。
    /// </summary>
    /// <typeparam name="T">來源資料型態 </typeparam>
    public class BatchBookingShowListModel<T> : BatchBookingShowListModel
    {
        /// <summary>
        /// 批次調用:主題檔案列表 資料, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <param name="data">來源資料 </param>
        /// <param name="fileCategory">媒資檔案類別: A聲音, D文件, P圖片, S主題, V影片 </param>
        public BatchBookingShowListModel(T data, FileTypeEnum fileCategory)
        {
            if (data != null)
            {
                //媒資檔案類別: A聲音, D文件, P圖片, S主題, V影片
                this.FileType = fileCategory.ToString();
                var _properties = typeof(T).GetProperties();

                foreach (var p in _properties)
                {
                    var _val = p.GetValue(data) ?? string.Empty;

                    if (p.Name == "fsSUBJECT_ID") this.fsSUBJECT_ID = _val.ToString();
                    if (p.Name == "fsFILE_NO") this.fsFILE_NO = _val.ToString();
                    if (p.Name == "fsTITLE") this.Title = _val.ToString();
                    if (p.Name == "fsDESCRIPTION") this.Description = _val.ToString();

                    //預覽圖URL
                    if (p.Name == "fsHEAD_FRAME_URL" || p.Name == "fsHEAD_FRAME" || p.Name == "C_sIMAGE_URL")
                    {
                        string _img = (fileCategory == FileTypeEnum.V)
                            ? "~/Images/videopreview.png" : (fileCategory == FileTypeEnum.A)
                                ? "~/Images/audiopreview.png" : (fileCategory == FileTypeEnum.P)
                                    ? "~/Images/imagepreview.png" : (fileCategory == FileTypeEnum.D)
                                        ? "~/Images/docpreview.png" : string.Empty;

                        this.ImageUrl = string.IsNullOrEmpty(_val.ToString()) ? _img : _val.ToString();
                    }

                    //20210909_ADDED_版權欄位
                    if (p.Name == "fsLICENSE") { this.LicenseCode = _val.ToString(); }
                    if (p.Name == "fsLICENSE_NAME") { this.LicenseStr = _val.ToString(); }

                    // 版權.是否調用提醒、是否調用禁止、提醒訊息
                    if (p.Name == "fcIS_ALERT" || p.Name == "IS_ALERT")
                    {
                        if (bool.TryParse(_val.ToString(), out bool chk)) { this.IsAlert = chk; }
                    }
                    if (p.Name == "fcIS_FORBID" || p.Name == "IS_FORBID")
                    {
                        if (bool.TryParse(_val.ToString(), out bool chk)) { this.IsForBid = chk; }
                    }
                    if (p.Name == "fsLICENSE_MESSAGE" || p.Name == "MESSAGE") { this.LicenseMessage = _val.ToString() ?? string.Empty; }
                    //20211005_ADDED)_版權授權日期
                    if (p.Name == "fcIS_LICENSE_EXPIRED" || p.Name == "IS_EXPIRED" || p.Name == "IsExpired")
                    {
                        if (bool.TryParse(_val.ToString(), out bool chk)) { this.IsExpired = chk; }
                    }
                    if (p.Name == "fdENDDATE" || p.Name == "LicenseEndDate")
                    {
                        if (DateTime.TryParse(_val.ToString(), out DateTime dt)) { this.LicenseEndDate = dt; }
                    }
                }

            }
        }
    }
}
