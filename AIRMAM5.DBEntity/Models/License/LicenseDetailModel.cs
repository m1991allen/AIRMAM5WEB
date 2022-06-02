using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.License
{
    /// <summary>
    /// 版權資料 內容
    /// </summary>
    public class LicenseDetailModel : LicenseCreateModel
    {
        #region >>> 欄位定義
        /// <summary>
        /// 建立者
        /// </summary>
        [Display(Name = "新增人員")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Display(Name = "新增時間")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 建立者
        /// </summary>
        [Display(Name = "修改人員")]
        public string UpdatedBy { get; set; } = string.Empty;

        /// <summary>
        /// 修改時間
        /// </summary>
        [Display(Name = "修改時間")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public DateTime? UpdatedTime { get; set; }
        #endregion

        /// <summary>
        /// 資料格式轉換
        /// </summary>
        /// <typeparam name="T">資料型別參數 </typeparam>
        /// <param name="data">資料內容 </param>
        /// <returns></returns>
        public new LicenseDetailModel ConvertData<T>(T data)
        {
            if (data == null) { return this; }

            string insuser = null, insusernm = null, upduser = null, updusernm = null;
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

                if (pp.Name == "fsCREATED_BY") insuser = val.ToString();
                if (pp.Name == "fsCREATED_BY_NAME") insusernm = val.ToString();
                if (pp.Name == "fdCREATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime insDate);
                    this.CreatedTime = insDate;
                }

                if (pp.Name == "fsUPDATED_BY") upduser = val.ToString();
                if (pp.Name == "fsUPDATED_BY_NAME") updusernm = val.ToString();
                if (pp.Name == "fdUPDATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime updDate);
                    if (string.IsNullOrEmpty(val.ToString())) this.UpdatedTime = null; else this.UpdatedTime = updDate;
                }
            }

            this.CreatedBy = string.Format("{0}{1}"
                    , string.IsNullOrEmpty(insuser) ? string.Empty : insuser
                    , string.IsNullOrEmpty(insusernm) ? string.Empty : string.Format($"({insusernm})"));

            this.UpdatedBy = string.Format("{0}{1}"
                    , string.IsNullOrEmpty(upduser) ? string.Empty : upduser
                    , string.IsNullOrEmpty(updusernm) ? string.Empty : string.Format($"({updusernm})"));

            return this;
        }
    }

}
