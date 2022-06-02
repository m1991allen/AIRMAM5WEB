using AIRMAM5.DBEntity.DBEntity;
using System;

namespace AIRMAM5.DBEntity.Models.Booking
{
    /// <summary>
    /// 審核調用(預借檔案) 列表List Model。 繼承參考 <see cref="BookingIdModel"/>
    /// </summary>
    public class VerifyBookingListModel : BookingIdModel
    {
        /// <summary>
        /// 審核調用(預借檔案) 列表List Model。 繼承參考 <see cref="BookingIdModel"/>
        /// </summary>
        public VerifyBookingListModel() { }

        #region >>> 欄位參數
        //VerifyId: 18,
        /// <summary>
        /// 轉檔工作編號 fnWORK_ID
        /// </summary>
        public long WorkId { get; set; }
        //BookingId: 34,

        /// <summary>
        /// 調用日期
        /// </summary>
        public string BookingDate { get; set; } = string.Empty;

        /// <summary>
        /// 調用者
        /// </summary>
        public string BookingLoginId { get; set; } = string.Empty;

        /// <summary>
        /// 調用者顯示名稱
        /// </summary>
        public string BookingUserName { get; set; } = string.Empty;

        /// <summary>
        /// 審核狀態(代碼 WORK_APPROVE)
        /// </summary>
        public string ApproveStatus { get; set; } = string.Empty;

        /// <summary>
        /// 審核狀態(中文)
        /// </summary>
        public string ApproveStatusStr { get; set; } = string.Empty;

        /// <summary>
        /// 媒資檔案類別: V,A,P,D
        /// </summary>
        public string MediaType { get; set; } = string.Empty;

        /// <summary>
        /// 預借原因
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// 預借檔案標題
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// (剪輯)起始時間
        /// </summary>
        public string MarkInTimeStr { get; set; } = string.Empty;

        /// <summary>
        /// (剪輯)結束時間
        /// </summary>
        public string MarkOutTimeStr { get; set; } = string.Empty;

        /// <summary>
        /// 審核人員
        /// </summary>
        public string ConfirmLoginId { get; set; } = string.Empty;

        /// <summary>
        /// 最後審核時間
        /// </summary>
        public string ConfirmTime { get; set; } = string.Empty;

        /// <summary>
        /// 審核結果備註
        /// </summary>
        public string ApproveMemo { get; set; } = string.Empty;
        #endregion

        /// <summary>
        /// 審核調用(預借檔案) 列表List Model - 資料格式轉換 
        /// </summary>
        /// <typeparam name="T">來源資料 類型Model  <see cref="spGET_L_WORK_BY_BOOKING_APPROVE_Result"/> </typeparam>
        /// <param name="data">來源資料 內容Data </param>
        /// <returns></returns>
        public VerifyBookingListModel DataConvert<T>(T data)
        {
            if (data == null) { return this; }

            string approveBy = string.Empty, approveByName = string.Empty;

            var properties = typeof(T).GetProperties();
            foreach (var pp in properties)
            {
                var val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fnWORK_ID" || pp.Name.ToUpper() == "WORKID")
                {
                    if (long.TryParse(val.ToString(), out long idx))
                    {
                        WorkId = idx;
                    }
                }
                if (pp.Name == "fnBOOKING_ID" || pp.Name.ToUpper() == "BOOKINGID")
                {
                    if (long.TryParse(val.ToString(), out long idx))
                    {
                        BookingId = idx;
                    }
                }

                if (pp.Name == "fdCREATED_DATE")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt))
                    {
                        BookingDate = string.Format($"{dt:yyyy/MM/dd HH:mm:ss}");
                    }
                }
                if (pp.Name == "fsCREATED_BY") { BookingLoginId = val.ToString(); }
                if (pp.Name == "fsCREATED_BY_NAME") { BookingUserName = val.ToString(); }
                if (pp.Name == "C_APPROVE_STATUS") { ApproveStatus = val.ToString(); }

                if (pp.Name == "C_APPROVE_STATUS_NAME") { ApproveStatusStr = val.ToString(); }
                if (pp.Name == "C_ARC_TYPE") { MediaType = val.ToString(); }
                if (pp.Name == "fsBOOKING_REASON") { Reason = val.ToString(); }
                if (pp.Name == "fsTITLE") { Title = val.ToString(); }

                if (pp.Name == "fdMARKIN")
                {
                    //TIP: 資料已是TimeCode格式,不需轉換!
                    MarkInTimeStr = val.ToString(); //string.IsNullOrEmpty(val.ToString()) ? string.Empty : CommonTimeCode.CurrentTimesToTimeCode(double.Parse(val.ToString()));
                }
                if (pp.Name == "fdMARKOUT")
                {
                    //TIP: 資料已是TimeCode格式,不需轉換!
                    MarkOutTimeStr = val.ToString(); //string.IsNullOrEmpty(m.fdMARKOUT) ? string.Empty : CommonTimeCode.CurrentTimesToTimeCode(double.Parse(m.fdMARKOUT));
                }

                if (pp.Name == "C_APPROVE_BY") { approveBy = val.ToString(); }
                if (pp.Name == "C_APPROVE_BY_NAME") { approveByName = val.ToString(); }

                if (pp.Name == "C_APPROVE_DATE")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt))
                    {
                        ConfirmTime = string.Format($"{dt:yyyy-MM-dd HH:mm:ss}");
                    }
                }

                if (pp.Name == "C_APPROVE_MEMO") { ApproveMemo = val.ToString() ?? string.Empty; }
            }

            this.ConfirmLoginId = string.IsNullOrEmpty(approveByName) ? approveBy : string.Format($"{approveBy}({approveByName})");

            return this;
        }
    }

}
