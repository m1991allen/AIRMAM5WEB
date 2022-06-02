using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Works
{
    /// <summary>
    /// 轉檔工作編號 tblWORK.[fnWORK_ID]
    /// </summary>
    public class LWorkIdModel
    {
        /// <summary>
        /// 轉檔工作編號 fnWORK_ID
        /// </summary>
        [Display(Name = "工作編號")]
        public long fnWORK_ID { get; set; }
    }

}
