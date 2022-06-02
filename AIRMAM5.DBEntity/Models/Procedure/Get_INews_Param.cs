
namespace AIRMAM5.DBEntity.Models.Procedure
{
    /// <summary>
    /// 查詢文稿 預存程序參數 dbo.spGET_INEWS
    /// </summary>
    public class Get_INews_Param
    {
        /// <summary>
        /// 動態欄位名稱字串, 分號(;)為分隔符號
        /// </summary>
        public string Columns { get; set; }

        /// <summary>
        /// 動態欄位值字串, 分號(;)為分隔符號
        /// </summary>
        public string Values { get; set; }
    }
}
