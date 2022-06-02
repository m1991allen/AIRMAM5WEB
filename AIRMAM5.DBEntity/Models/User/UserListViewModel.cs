using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.User
{
    /// <summary>
    /// 使用者列表 model,　繼承參考 <see cref="UserIdModel"/> 
    /// </summary>
    public class UserListViewModel : UserIdModel
    {
        /// <summary>
        /// 使用者列表 model
        /// </summary>
        public UserListViewModel() { }

        #region >>>>>[欄位參數]
        /// <summary>
        /// 使用者姓名
        /// </summary>
        [Required]
        [Display(Name = "顯示名稱")]
        public string fsNAME { get; set; } = string.Empty;

        /// <summary>
        /// 隸屬單位 [fsDEPT_ID]
        /// </summary>
        [Display(Name = "隸屬單位")]
        public string fsDEPT_ID { get; set; } = string.Empty;

        [Display(Name = "隸屬單位")] // C_sDEPTNAME
        public string C_sDEPTNAME { get; set; } = string.Empty;

        /// <summary>
        /// 帳號狀態: 1啟用 / 0不啟用 [fsIS_ACTIVE]
        /// </summary>
        [Display(Name = "帳號狀態")]
        public bool fsIS_ACTIVE { get; set; } = false;

        /// <summary>
        /// 描述/備註 [fsDESCRIPTION]
        /// </summary>
        [Display(Name = "描述/備註")]
        public string fsDESCRIPTION { get; set; } = string.Empty;

        /// <summary>
        /// 電子郵件 [fsEMAIL]
        /// </summary>
        [Display(Name = "電子郵件")]
        public string fsEMAIL { get; set; } = string.Empty;
        /// <summary>
        /// 電子郵件是否驗證 [fsEmailConfirmed]
        /// </summary>
        public bool fsEmailConfirmed { get; set; }

        /// <summary>
        /// 電子郵件是否驗證
        /// </summary>
        public string EmailConfirmedStr { get; set; } = string.Empty;
        #endregion

        /// <summary>
        /// 取得使用者列表 資料內容, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_USERS_Result"/> </typeparam>
        /// <param name="m">資料來源資料集合 <typeparamref name="T"/></param>
        public UserListViewModel FormatConversion<T>(T m)
        {
            if (m == null) { return this; }

            var _Properties = typeof(T).GetProperties();
            foreach (var info in _Properties)
            {
                var val = info.GetValue(m) ?? string.Empty;

                if (info.Name == "fsUSER_ID") this.fsUSER_ID = val.ToString();
                if (info.Name == "fsLOGIN_ID") this.fsLOGIN_ID = val.ToString();
                if (info.Name == "fsNAME") this.fsNAME = val.ToString();
                if (info.Name == "fsDEPT_ID") this.fsDEPT_ID = val.ToString();
                if (info.Name == "C_sDEPTNAME") this.C_sDEPTNAME = val.ToString();
                if (info.Name == "fsIS_ACTIVE")
                {
                    bool.TryParse(val.ToString(), out bool b);
                    this.fsIS_ACTIVE = b;
                }

                if (info.Name == "fsDESCRIPTION") this.fsDESCRIPTION = val.ToString();

                if (info.Name == "fsEMAIL") this.fsEMAIL = val.ToString();
                if (info.Name == "fsEmailConfirmed")
                {
                    bool.TryParse(val.ToString(), out bool b);
                    this.fsEmailConfirmed = b;
                    this.EmailConfirmedStr = b ? "已驗證" : "未驗證";
                }
            }

            return this;
        }
    }

}
