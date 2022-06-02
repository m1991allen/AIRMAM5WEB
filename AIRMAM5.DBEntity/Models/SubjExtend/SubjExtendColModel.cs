using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.SubjExtend
{
    /// <summary>
    /// 擴充功能{新聞文稿/公文對應} View : 動態的查詢條件欄位選單
    /// </summary>
    public class SubjExtendColModel : SelectListItem
    {
        /// <summary>
        /// 資料型態
        /// </summary>
        public string DataType { get; set; }

        //public bool Disabled { get; set; }
        //public SelectListGroup Group { get; set; }
        //public bool Selected { get; set; }
        //public string Text { get; set; }
        //public string Value { get; set; }

    }

}
