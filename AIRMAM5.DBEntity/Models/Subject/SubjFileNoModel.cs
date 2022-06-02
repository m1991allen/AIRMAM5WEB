
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Subject
{
    /// <summary>
    /// 主題編號+檔案編號  繼承參考 <see cref="SubjectIdModel"/>
    /// <para>fsSUBJECT_ID ，fsFILE_NO </para>
    /// </summary>
    public class SubjFileNoModel : SubjectIdModel
    {
        /// <summary>
        /// 主題編號+檔案編號  繼承參考 <see cref="SubjectIdModel"/>
        /// </summary>
        public SubjFileNoModel() { }

        /// <summary>
        /// 檔案編號
        /// </summary>
        [Display(Name = "檔案編號")]
        public string fsFILE_NO { set; get; } = string.Empty;
    }
}
