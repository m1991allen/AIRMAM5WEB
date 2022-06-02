using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Booking
{
    /// <summary>
    /// 調用詳細內容 Model。 繼承參考 <see cref="MyBookingModel"/>
    /// </summary>
    public class BookingDetModel : MyBookingModel
    {
        /// <summary>
        /// 調用詳細內容Model
        /// </summary>
        public BookingDetModel() { }

        #region >>> 欄位參數
        /// <summary>
        /// 建立時間 fdCREATED_DATE
        /// </summary>
        [Display(Name ="新增時間")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 建立帳號 fsCREATED_BY
        /// </summary>
        [Display(Name = "新增人員")]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// 最後異動時間 fdUPDATED_DATE
        /// </summary>
        [Display(Name = "修改時間")]
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// 最後異動帳號 fsUPDATED_BY
        /// </summary>
        [Display(Name = "修改人員")]
        public string UpdatedBy { get; set; } = string.Empty;

        /// <summary>
        /// 建立帳號顯示名稱 fsCREATED_BY_NAME
        /// </summary>
        [Display(Name = "新增人員")]
        public string CreatedByName { get; set; } = string.Empty;

        /// <summary>
        /// 最後異動帳號顯示名稱 fsUPDATED_BY_NAME
        /// </summary>
        [Display(Name = "修改人員")]
        public string UpdatedByName { get; set; } = string.Empty;
        #endregion

        public new BookingDetModel DataConvert<T>(T data)
        {
            if (data == null) { return this; }

            var properties = typeof(T).GetProperties();

            foreach (var pp in properties)
            {
                var val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fnBOOKING_ID" || pp.Name.ToUpper() == "BOOKINGID")
                {
                    if (long.TryParse(val.ToString(), out long idx)) { BookingId = idx; }
                }
                if (pp.Name == "fnWORK_ID")
                {
                    if (long.TryParse(val.ToString(), out long idx)) { WorkId = idx; }
                }
                if (pp.Name == "fsPROGRESS") { Progress = val.ToString(); }
                if (pp.Name == "fsRESULT") { Result = val.ToString(); }
                if (pp.Name == "C_ARC_TYPE") { ArcType = val.ToString(); }
                if (pp.Name == "C_ARC_TYPE_NAME") { ArcTypeName = val.ToString(); }
                if (pp.Name == "fsTYPE_NAME") { BookingTypeName = val.ToString(); }
                if (pp.Name == "fsSTATUS") { WorkStatus = val.ToString() ?? string.Empty; }
                if (pp.Name == "fsSTATUS_NAME") { StatusName = val.ToString() ?? string.Empty; }
                if (pp.Name == "fsTITLE") { Title = val.ToString() ?? string.Empty; }
                if (pp.Name == "fdCREATED_DATE")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt)) { BookingDate = string.Format($"{dt:yyyy/MM/dd HH:mm:ss}"); }
                }

                if (pp.Name == "fdMARKIN") { MarkInTime = val.ToString() ?? string.Empty; }
                if (pp.Name == "fdMARKOUT") { MarkOutTime = val.ToString() ?? string.Empty; }
                if (pp.Name == "fsNOTE") { NoteStr = val.ToString() ?? string.Empty; }

                if (pp.Name == "fdSTIME")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt))
                        StartTime = string.Format($"{dt:yyyy/MM/dd HH:mm:ss}");
                }
                if (pp.Name == "fdETIME")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt))
                        EndTime = string.Format($"{dt:yyyy/MM/dd HH:mm:ss}");
                }
                if (pp.Name == "fsCREATED_BY") { CreatedBy = val.ToString(); }
                if (pp.Name == "fsCREATED_BY_NAME") { CreatedByName = val.ToString(); }
                if (pp.Name == "fdCREATED_DATE")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt))
                        CreatedDate = dt;
                }

                if (pp.Name == "fsUPDATED_BY") { UpdatedBy = val.ToString(); }
                if (pp.Name == "fsUPDATED_BY_NAME") { UpdatedByName = val.ToString(); }
                if (pp.Name == "fdUPDATED_DATE")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt))
                        UpdatedDate = dt;
                }
            }

            return this;
        }
    }

}
