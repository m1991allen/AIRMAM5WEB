
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Announce
{
    /// <summary>
    /// 公告識別碼 fnANN_ID 資料MOdel
    /// </summary>
    public class AnnounceIdModel
    {
        public AnnounceIdModel() { }

        /// <summary>
        /// 公告識別碼 fnANN_ID
        /// </summary>
        [Display(Name = "公告識別碼")]
        public long AnnounceId { get; set; }
    }

}
