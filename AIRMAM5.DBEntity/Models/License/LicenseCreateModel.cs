using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.License
{
    /// <summary>
    /// 版權資料 新增 ViewModel
    /// </summary>
    public class LicenseCreateModel: LicenseListModel
    {
        public LicenseCreateModel() : base() { }

        /// <summary>
        /// 排序 fnORDER
        /// </summary>
        [Display(Name = "排序")]
        public int Order { get; set; }

        /// <summary>
        /// 資料格式轉換
        /// </summary>
        /// <typeparam name="T">資料型別參數 </typeparam>
        /// <param name="data">資料內容 </param>
        /// <returns></returns>
        public new LicenseCreateModel ConvertData<T>(T data)
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
                if (pp.Name == "fnORDER" || pp.Name == "Order")
                {
                    if (int.TryParse(val.ToString(), out int num))
                    {
                        this.Order = num;
                    }
                }
            }

            return this;
        }
    }

}
