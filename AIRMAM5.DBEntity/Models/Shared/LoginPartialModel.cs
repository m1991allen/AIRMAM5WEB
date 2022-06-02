using AIRMAM5.DBEntity.Models.Function;
using AIRMAM5.DBEntity.Models.Search;
using AIRMAM5.DBEntity.Models.User;
using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.Shared
{
    /// <summary>
    /// _LoginPartial 資料model
    /// </summary>
    public class LoginPartialModel
    {
        public LoginPartialModel()
        {
            SearchColumn = new SearchColumnViewModel();
            UserInfo = new UserLoginInfoViewModel();
            QuickMenu = new List<UserFavoriteModel>();
        }

        /// <summary>
        /// (提供前端)檢索欄位資料
        /// </summary>
        public SearchColumnViewModel SearchColumn { get; set; }

        /// <summary>
        /// 使用者 登入資訊
        /// </summary>
        public UserLoginInfoViewModel UserInfo { get; set; }

        /// <summary>
        /// UI上方右側常駐之功能項目
        /// </summary>
        public List<UserFavoriteModel> QuickMenu { get; set; }
    }
}
