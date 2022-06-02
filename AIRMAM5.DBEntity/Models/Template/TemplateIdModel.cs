using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Template
{
    /// <summary>
    /// 樣版編號 Model
    /// </summary>
    public class TemplateIdModel
    {
        /// <summary>
        /// 樣版編號 Model
        /// </summary>
        public TemplateIdModel() : base() { fnTEMP_ID = 0; }

        /// <summary>
        /// 樣版編號 fnTEMP_ID
        /// </summary>
        [Display(Name = "樣板編號")]
        //[JsonProperty(PropertyName = "fnTEMP_ID")]
        public int fnTEMP_ID { get; set; }
    }

}
