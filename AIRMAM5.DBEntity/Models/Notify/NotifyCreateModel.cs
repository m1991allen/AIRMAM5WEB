using AIRMAM5.DBEntity.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Notify
{
    /// <summary>
    /// 新增訊息通知 Model
    /// </summary>
    public class NotifyCreateModel
    {
        /// <summary>
        /// 標題
        /// </summary>
        [Required]
        [DisplayName("標題")]
        public string Title { get; set; }

        /// <summary>
        /// 內容
        /// </summary>
        [Required]
        [DisplayName("內容")]
        public string Content { get; set; }

        /// <summary>
        /// 類別
        /// </summary>
        [DisplayName("類別")]
        public int Category { get; set; } = (int)NotifyCategoryEnum.預設;
        public List<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 通知對象(人or群組or部門,測試暫用userid)
        /// </summary>
        [Required]
        [DisplayName("通知對象")]
        public string NoticeTo { get; set; }

        /// <summary>
        /// 有效日期
        /// </summary>
        [Required]
        [DisplayName("有效日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ExpireDate { get; set; } = DateTime.Now.AddDays(5);
    }

}
