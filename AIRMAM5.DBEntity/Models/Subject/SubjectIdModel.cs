
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Subject
{
    /// <summary>
    /// 主題編號  fsSUBJECT_ID 
    /// </summary>
    public class SubjectIdModel
    {
        public SubjectIdModel() { }
        /// <summary>
        /// 主題編號
        /// </summary>
        [Display(Name = "主題編號")]
        public string fsSUBJECT_ID { set; get; } = string.Empty;
    }

}
