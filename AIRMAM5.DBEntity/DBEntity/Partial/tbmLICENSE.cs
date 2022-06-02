using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 版權資料  tbmLICENSE
    /// </summary>
    [MetadataType(typeof(tbmLICENSEMetadata))]
    public partial class tbmLICENSE
    {
        public tbmLICENSE()
        {
            fdCREATED_DATE = DateTime.Now;
            fcIS_ACTIVE = true;
            fcIS_ALERT = false;
            fcIS_FORBID = false;
            fsMESSAGE = string.Empty;
            fsDESCRIPTION = string.Empty;
        }

        public class tbmLICENSEMetadata
        {
            [Required]
            [Display(Name = "版權代碼")]
            public string fsCODE { get; set; }

            [Display(Name = "版權名稱")]
            [Required]
            [MaxLength(50, ErrorMessage = "版權名稱長度限制50")]
            public string fsNAME { get; set; }

            [Display(Name = "備註")]
            public string fsDESCRIPTION { get; set; }

            [Display(Name = "授權到期日")]
            public Nullable<System.DateTime> fdENDDATE { get; set; }

            [Display(Name = "提醒訊息內容")]
            public string fsMESSAGE { get; set; }

            [Display(Name = "是否調用提醒")]
            public bool fcIS_ALERT { get; set; }

            [Display(Name = "是否調用禁止")]
            public bool fcIS_FORBID { get; set; }

            [Display(Name = "是否啟用")]
            public bool fcIS_ACTIVE { get; set; }

            [Display(Name = "排序")]
            public int fnORDER { get; set; }

            public System.DateTime fdCREATED_DATE { get; set; }

            public string fsCREATED_BY { get; set; }

            public Nullable<System.DateTime> fdUPDATED_DATE { get; set; }

            public string fsUPDATED_BY { get; set; }
        }

        /// <summary>
        /// 資料格式轉換至 <see cref="tbmLICENSE"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public tbmLICENSE DataConvert<T>(T data)
        {
            if (data == null) { return this; }

            //string insuser = null, insusernm = null, upduser = null, updusernm = null;
            var propreties = typeof(T).GetProperties();

            foreach (var pp in propreties)
            {
                var val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fsCODE" || pp.Name == "LicenseCode") { this.fsCODE = val.ToString(); }
                if (pp.Name == "fsNAME" || pp.Name == "LicenseName") { this.fsNAME = val.ToString(); }
                if (pp.Name == "fsDESCRIPTION" || pp.Name == "LicenseDesc") { this.fsDESCRIPTION = val.ToString(); }
                if (pp.Name == "fdENDDATE" || pp.Name == "EndDate")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt))
                    {
                        this.fdENDDATE = dt;
                    }
                }

                if (pp.Name == "fsMESSAGE" || pp.Name == "AlertMessage" || pp.Name == "AlertMsg") { this.fsMESSAGE = val.ToString(); }
                if (pp.Name == "fcIS_ALERT" || pp.Name == "IsBookingAlert")
                {
                    if (bool.TryParse(val.ToString(), out bool chk))
                    {
                        this.fcIS_ALERT = chk;
                    }
                }
                if (pp.Name == "fcIS_FORBID" || pp.Name == "IsNotBooking")
                {
                    if (bool.TryParse(val.ToString(), out bool chk))
                    {
                        this.fcIS_FORBID = chk;
                    }
                }
                if (pp.Name == "fcIS_ACTIVE" || pp.Name == "IsActive")
                {
                    if (bool.TryParse(val.ToString(), out bool chk))
                    {
                        this.fcIS_ACTIVE = chk;
                    }
                }
                if (pp.Name == "fnORDER" || pp.Name == "Order")
                {
                    if (int.TryParse(val.ToString(), out int num))
                    {
                        this.fnORDER = num;
                    }
                }

                if (pp.Name == "fsCREATED_BY") { this.fsCREATED_BY = val.ToString(); }//insuser = val.ToString();
                //if (pp.Name == "fsCREATED_BY_NAME") { insusernm = val.ToString(); }
                if (pp.Name == "fdCREATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime insDate);
                    this.fdCREATED_DATE = insDate;
                }

                if (pp.Name == "fsUPDATED_BY") { this.fsUPDATED_BY = val.ToString(); }//upduser = val.ToString();
                //if (pp.Name == "fsUPDATED_BY_NAME") updusernm = val.ToString();
                if (pp.Name == "fdUPDATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime updDate);
                    if (string.IsNullOrEmpty(val.ToString())) this.fdUPDATED_DATE = null; else this.fdUPDATED_DATE = updDate;
                }

            }

            //this.CreatedBy = string.Format("{0}{1}"
            //        , string.IsNullOrEmpty(insuser) ? string.Empty : insuser
            //        , string.IsNullOrEmpty(insusernm) ? string.Empty : string.Format($"({insusernm})"));
            //this.UpdatedBy = string.Format("{0}{1}"
            //        , string.IsNullOrEmpty(upduser) ? string.Empty : upduser
            //        , string.IsNullOrEmpty(updusernm) ? string.Empty : string.Format($"({updusernm})"));

            return this;
        }
    
    }
}
