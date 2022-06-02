using System.Collections.Generic;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Announce
{
    /// <summary>
    /// 公告維護-搜尋條件
    /// </summary>
    public class AnnounceSearchModel
    {
        /// <summary>
        /// 公告維護-搜尋條件 初始
        /// </summary>
        public AnnounceSearchModel() { }

        /// <summary>
        /// 公告分類 選單
        /// </summary>
        public List<SelectListItem> AnnTypeList { get; set; }

        /// <summary>
        /// 發佈單位 選單
        /// </summary>
        public List<SelectListItem> AnnDeptList { get; set; }
    }

}
