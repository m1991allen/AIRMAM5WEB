using System.Collections.Generic;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.CodeSet
{
    /// <summary>
    /// 主代碼帶子代碼清單model
    /// </summary>
    public class MainSubCodeListModel
    {
        public MainSubCodeListModel() { }

        /// <summary>
        /// 主代碼
        /// </summary>
        public string MainCodeId { get; set; } = string.Empty;

        /// <summary>
        /// 主代碼名稱
        /// </summary>
        public string MainCodeName { get; set; } = string.Empty;

        /// <summary>
        /// 子代碼列表 <see cref="SelectListItem"/>
        /// </summary>
        public List<SelectListItem> SubCodeList { get; set; } = new List<SelectListItem>();
    }

}
