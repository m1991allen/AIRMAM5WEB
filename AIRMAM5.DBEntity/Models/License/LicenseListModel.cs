
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.License
{
    /// <summary>
    /// 版權資料 清單 Model
    /// </summary>
    public class LicenseListModel : LicenseCodeModel
    {
        #region >>> 參數/屬性
        /// <summary>
        /// 版權名稱
        /// </summary>
        [Required]
        [Display(Name = "版權名稱")]
        public string LicenseName { get; set; } = string.Empty;
        /// <summary>
        /// 版權備註
        /// </summary>
        [Display(Name = "備註")]
        public string LicenseDesc { get; set; } = string.Empty;
        /// <summary>
        /// 授權到期日期, 預設無日期
        /// </summary>
        [Display(Name = "授權到期日")]
        public string EndDate { get; set; } = string.Empty;
        /// <summary>
        /// 提醒訊息內容
        /// </summary>
        [Display(Name = "提醒訊息")]
        public string AlertMessage { get; set; } = string.Empty;
        /// <summary>
        /// 是否調用提醒
        /// </summary>
        [Display(Name = "是否調用提醒")]
        public bool IsBookingAlert { get; set; }
        /// <summary>
        /// 是否調用禁止
        /// </summary>
        [Display(Name = "是否調用禁止")]
        public bool IsNotBooking { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        [Display(Name = "是否啟用")]
        public bool IsActive { get; set; }
        #endregion

        /// <summary>
        /// 資料格式轉換
        /// </summary>
        /// <typeparam name="T">資料型別參數 </typeparam>
        /// <param name="data">資料內容 </param>
        /// <returns></returns>
        public LicenseListModel ConvertData<T>(T data)
        {
            if (data == null) { return this; }

            var properties = typeof(T).GetProperties();

            foreach (var pp in properties)
            {
                var val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fsCODE" || pp.Name == "LicenseCode") { this.LicenseCode = val.ToString(); }
                if (pp.Name == "fsNAME" || pp.Name == "LicenseName") { this.LicenseName = val.ToString(); }
                if (pp.Name == "fsDESCRIPTION" || pp.Name == "LicenseDesc") { this.LicenseDesc = val.ToString(); }
                if (pp.Name == "fdENDDATE" || pp.Name == "EndDate")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt))
                    {
                        this.EndDate = string.Format($"{dt:yyyy-MM-dd}");
                    }
                }

                if (pp.Name == "fsMESSAGE" || pp.Name == "AlertMessage" || pp.Name == "AlertMsg") { this.AlertMessage = val.ToString(); }
                if (pp.Name == "fcIS_ALERT" || pp.Name == "IsBookingAlert")
                {
                    if (bool.TryParse(val.ToString(), out bool chk))
                    {
                        this.IsBookingAlert = chk;
                    }
                }
                if (pp.Name == "fcIS_FORBID" || pp.Name == "IsNotBooking")
                {
                    if (bool.TryParse(val.ToString(), out bool chk))
                    {
                        this.IsNotBooking = chk;
                    }
                }
                if (pp.Name == "fcIS_ACTIVE" || pp.Name == "IsActive")
                {
                    if (bool.TryParse(val.ToString(), out bool chk))
                    {
                        this.IsActive = chk;
                    }
                }
            }

            return this;
        }
    }

}
