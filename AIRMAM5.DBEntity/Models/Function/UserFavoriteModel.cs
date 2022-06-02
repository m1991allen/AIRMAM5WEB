
namespace AIRMAM5.DBEntity.Models.Function
{
    /// <summary>
    /// 使用者帳號 我的最愛 項目。　繼承參考 <see cref="FunctionIdModel"/>
    /// </summary>
    public class UserFavoriteModel : FunctionIdModel
    {
        /// <summary>
        /// 使用者帳號 我的最愛 項目。
        /// </summary>
        public UserFavoriteModel() { }

        #region >>> 欄位參數
        /// <summary>
        /// 功能項目名稱
        /// </summary>
        public string FunctionName { get; set; } = string.Empty;

        /// <summary>
        ///  [fsCONTROLLER]
        /// </summary>
        public string ControllerName { set; get; } = string.Empty;

        /// <summary>
        ///  [fsACTION]
        /// </summary>
        public string ActionName { set; get; } = string.Empty;

        /// <summary>
        /// 我的最愛URL [fsFAVORITE]
        /// </summary>
        public string FavoriteUrl { get; set; } = string.Empty;

        /// <summary>
        /// 功能項目圖示
        /// </summary>
        public string Icon { get; set; } = string.Empty;
        #endregion
    }
}
