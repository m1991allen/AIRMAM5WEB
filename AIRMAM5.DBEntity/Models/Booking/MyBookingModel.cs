
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Booking
{
    /// <summary>
    /// 我的調用狀態 資料Model。 繼承參考 <see cref="BookingIdModel"/>
    /// </summary>
    public class MyBookingModel : BookingIdModel
    {
        /// <summary>
        /// 我的調用狀態資料Model
        /// </summary>
        public MyBookingModel() { }

        #region >>> 欄位參數
        ///// <summary>
        ///// 調用編號 [fnBOOKING_ID]
        ///// </summary>
        //public long BookingId { get; set; } = 0;

        /// <summary>
        /// 轉檔編號 [fnWORK_ID]
        /// </summary>
        [Display(Name = "轉檔編號")]
        public long WorkId { get; set; } = 0;

        /// <summary>
        /// 轉檔進度 %
        /// </summary>
        [Display(Name = "進度")]
        public string Progress { get; set; } = string.Empty;

        /// <summary>
        /// 調用結果 [fsRESULT]
        /// </summary>
        [Display(Name = "調用結果")]
        public string Result { get; set; } = string.Empty;

        /// <summary>
        /// 檔案類型 [_ARC_TYPE] : 影音圖文
        /// </summary>
        [Display(Name = "檔案類型")]
        public string ArcType { get; set; } = string.Empty;

        /// <summary>
        /// 檔案類型名稱 [_ARC_TYPE_NAME]: fsCODE_ID = 'MTRL001'
        /// </summary>
        [Display(Name = "檔案類型")]
        public string ArcTypeName { get; set; } = string.Empty;

        /// <summary>
        /// 調用類別名稱 [fsTYPE_NAME]: fsCODE_ID = 'WORK001'
        /// </summary>
        [Display(Name = "調用類別")]
        public string BookingTypeName { get; set; } = string.Empty;

        /// <summary>
        /// 狀態 [fsSTATUS]
        /// </summary>
        [Display(Name = "狀態")]
        public string WorkStatus { get; set; } = string.Empty;

        /// <summary>
        /// 狀態說明 [fsSTATUS_NAME] 
        /// </summary>
        [Display(Name = "狀態說明")]
        public string StatusName { get; set; } = string.Empty;

        /// <summary>
        /// 狀態顏色表示
        /// </summary>
        [Display(Name = "狀態顏色")]
        public string StatusColor { get; set; } = string.Empty;

        /// <summary>
        /// 標題 [fsTITLE]
        /// </summary>
        [Display(Name = "標題")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 調用日期 [fdCREATED_DATE]
        /// </summary>
        [Display(Name = "調用日期")]
        public string BookingDate { get; set; } = string.Empty;

        /// <summary>
        /// 起始時間 [fdMARKIN] : 已為TimeCode格式
        /// </summary>
        [Display(Name = "起始時間")]
        public string MarkInTime { get; set; } = string.Empty;

        /// <summary>
        /// 結束時間 [fdMARKOUT] : 已為TimeCode格式
        /// </summary>
        [Display(Name = "結束時間")]
        public string MarkOutTime { get; set; } = string.Empty;

        /// <summary>
        /// 調用備註 [fsNOTE]
        /// </summary>
        [Display(Name = "備註")]
        public string NoteStr { get; set; } = string.Empty;

        /// <summary>
        /// 轉檔開始時間 [fdSTIME]
        /// </summary>
        [Display(Name = "轉檔開始時間")]
        public string StartTime { get; set; } = string.Empty;

        /// <summary>
        /// 轉檔結束時間 [fdETIME]
        /// </summary>
        [Display(Name = "轉檔結束時間")]
        public string EndTime { get; set; } = string.Empty;
        #endregion

        /// <summary>
        /// 我的調用狀態 資料Model - 資料格式轉換 
        /// </summary>
        /// <typeparam name="T">來源資料 類型Model  <see cref="spGET_L_WORK_BY_BOOKING_Result"/> </typeparam>
        /// <param name="data">來源資料 內容Data </param>
        /// <returns></returns>
        public MyBookingModel DataConvert<T>(T data)
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
                if (pp.Name == "fsRESULT" || pp.Name.ToUpper() == "RESULT") { Result = val.ToString(); }
                if (pp.Name == "C_ARC_TYPE") { ArcType = val.ToString(); }
                if (pp.Name == "C_ARC_TYPE_NAME") { ArcTypeName = val.ToString(); }
                if (pp.Name == "fsTYPE_NAME") { BookingTypeName = val.ToString(); }
                if (pp.Name == "fsSTATUS") { WorkStatus = val.ToString() ?? string.Empty; }
                if (pp.Name == "fsSTATUS_NAME") { StatusName = val.ToString() ?? string.Empty; }

                if (pp.Name == "fsSTATUS_COLOR") { StatusColor = val.ToString() ?? "grey";  }//grey:排程中
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

                if (pp.Name == "fsCREATED_BY") { }
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
