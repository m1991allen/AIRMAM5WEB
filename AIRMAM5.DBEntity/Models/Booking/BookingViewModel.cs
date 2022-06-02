using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Booking
{
    /// <summary>
    /// 管理調用狀態 資料Model 。 繼承參考 <see cref="MyBookingModel"/>
    /// </summary>
    public class BookingViewModel : MyBookingModel
    {
        public BookingViewModel() { }

        #region >>>屬性/欄位
        /// <summary>
        /// 優先權
        /// </summary>
        [Display(Name = "優先權")]
        public string Priority { get; set; } = "9";

        /// <summary>
        /// 調用者
        /// </summary>
        [Display(Name = "調用者")]
        public string CreateBy { get; set; } = string.Empty;
        #endregion

        public new BookingViewModel DataConvert<T>(T data)
        {
            if (data == null) { return this; }

            var properties = typeof(T).GetProperties();

            foreach (var pp in properties)
            {
                var val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fnBOOKING_ID" || pp.Name.ToUpper() == "BOOKINGID")
                {
                    if (long.TryParse(val.ToString(), out long idx))
                    {
                        BookingId = idx;
                    }
                }
                if (pp.Name == "fnWORK_ID" || pp.Name.ToUpper() == "WORKID")
                {
                    if (long.TryParse(val.ToString(), out long idx))
                    {
                        WorkId = idx;
                    }
                }
                if (pp.Name == "fsPROGRESS" || pp.Name.ToUpper() == "PROGRESS") { Progress = val.ToString(); }
                if (pp.Name == "fsRESULT") { Result = val.ToString(); }
                if (pp.Name == "C_ARC_TYPE") { ArcType = val.ToString(); }
                if (pp.Name == "C_ARC_TYPE_NAME") { ArcTypeName = val.ToString(); }
                if (pp.Name == "fsTYPE_NAME") { BookingTypeName = val.ToString(); }
                if (pp.Name == "fsSTATUS") { WorkStatus = val.ToString() ?? string.Empty; }
                if (pp.Name == "fsSTATUS_NAME") { StatusName = val.ToString() ?? string.Empty; }

                if (pp.Name == "fsSTATUS_COLOR") { StatusColor = val.ToString() ?? "grey"; }//grey:排程中
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

                if (pp.Name == "fsPRIORITY") { Priority = val.ToString() ?? string.Empty; }

                if (pp.Name == "fsCREATED_BY") { CreateBy = val.ToString(); }
                if (pp.Name == "fsCREATED_BY_NAME") { }
                if (pp.Name == "fdCREATED_DATE") { }

                if (pp.Name == "fsUPDATED_BY") { }
                if (pp.Name == "fsUPDATED_BY_NAME") { }
                if (pp.Name == "fdUPDATED_DATE") { }
            }

            return this;
        }
    }

}
