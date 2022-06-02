using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Booking
{
    /// <summary>
    /// 調用-預借編號 [fnBOOKING_ID]
    /// </summary>
    public class BookingIdModel
    {
        /// <summary>
        /// 預借編號ID
        /// </summary>
        [Display(Name = "預借編號")]
        public long BookingId { get; set; }
    }

}
