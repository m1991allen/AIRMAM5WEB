using AIRMAM5.DBEntity.DBEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.User
{
    /// <summary>
    /// 詳細內容 頁面資料model,　繼承參考 <see cref="spGET_USERS_Result"/> 
    /// </summary>
    public class UserDetailViewModel : spGET_USERS_Result
    {
        /// <summary>
        /// 詳細內容 頁面資料model
        /// </summary>
        public UserDetailViewModel() { }

        #region >>> 屬性/欄位參數
        /// <summary>
        /// 使用者所屬群組/角色 (複選)
        /// </summary>
        [Display(Name = "所屬群組")]
        public string[] GroupList { get; set; }

        /// <summary>
        /// 檔案機密等級權限 (複選)
        /// </summary>
        [Display(Name = "檔案機密等級權限")]
        public string[] FSecretList { get; set; }

        /// <summary>
        /// 電子郵件是否驗證
        /// </summary>
        public string EmailConfirmed { get; set; } = string.Empty;

        #region >>> 屬性/欄位參數.DropDownList
        /// <summary>
        /// 單位部門.下拉清單
        /// </summary>
        public List<SelectListItem> DeptSelect { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 機密等級權限.下拉清單
        /// </summary>
        public List<SelectListItem> FileSecretSelect { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 群組角色.下拉清單
        /// </summary>
        public List<SelectListItem> RoleGroupSelect { get; set; } = new List<SelectListItem>();
        #endregion

        #endregion

        /* marked_20211001_取消類別中的方法 */
        /// <summary>
        /// 取得 使用者詳細內容 資料 , 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_USERS_Result"/> </typeparam>
        /// <param name="m"></param>
        /// <returns> <see cref="UserDetailViewModel"/> </returns>
        public UserDetailViewModel FormatConversion<T>(T m)
        {
            if (m == null) { return this; }

            var _Properties = typeof(T).GetProperties();
            foreach (var info in _Properties)
            {
                var val = info.GetValue(m) ?? string.Empty;

                if (info.Name == "fsUSER_ID") this.fsUSER_ID = val.ToString();
                if (info.Name == "fsLOGIN_ID") this.fsLOGIN_ID = val.ToString();
                //if (info.Name == "fsPASSWORD") { }
                if (info.Name == "fsNAME") this.fsNAME = val.ToString();
                if (info.Name == "fsENAME") this.fsENAME = val.ToString();
                if (info.Name == "fsTITLE") this.fsTITLE = val.ToString();
                if (info.Name == "fsDEPT_ID") this.fsDEPT_ID = val.ToString();
                if (info.Name == "fsEMAIL") this.fsEMAIL = val.ToString();
                if (info.Name == "fsPHONE") this.fsPHONE = val.ToString();
                if (info.Name == "fsDESCRIPTION") this.fsDESCRIPTION = val.ToString();
                if (info.Name == "fsFILE_SECRET")
                {
                    this.fsFILE_SECRET = val.ToString();
                    this.FSecretList = val.ToString().Split(new char[] { ';' });
                    //this.FileSecretSelect = _ser.CodeListItemSelected(TbzCodeIdEnum.FILESECRET.ToString(), val.ToString());
                }

                if (info.Name == "fsBOOKING_TARGET_PATH") this.fsBOOKING_TARGET_PATH = val.ToString();

                if (info.Name == "fsIS_ACTIVE")
                {
                    bool.TryParse(val.ToString(), out bool b);
                    this.fsIS_ACTIVE = b;
                }
                if (info.Name == "fsEmailConfirmed")
                {
                    bool.TryParse(val.ToString(), out bool b);
                    this.fsEmailConfirmed = b;
                    this.EmailConfirmed = b ? "已驗證" : "未驗證";
                }

                if (info.Name == "fsCREATED_BY") this.fsCREATED_BY = val.ToString();
                if (info.Name == "fsCREATED_BY_NAME") this.fsCREATED_BY_NAME = val.ToString();
                if (info.Name == "fdCREATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime dt);
                    this.fdCREATED_DATE = dt;
                }

                if (info.Name == "fsUPDATED_BY") this.fsUPDATED_BY = val.ToString();
                if (info.Name == "fsUPDATED_BY_NAME") this.fsUPDATED_BY_NAME = val.ToString();
                if (info.Name == "fdUPDATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime dt);
                    if (string.IsNullOrEmpty(val.ToString())) this.fdUPDATED_DATE = null; else this.fdUPDATED_DATE = dt;
                }

                if (info.Name == "C_sDEPTNAME") this.C_sDEPTNAME = val.ToString();
                if (info.Name == "fsGROUPs")
                {
                    this.fsGROUPs = val.ToString();
                    this.GroupList = val.ToString().Split(new char[] { ';' });
                    //this.RoleGroupSelect = _grp.GroupListItemSelected(val.ToString());
                }

                if (info.Name == "C_sIS_ADMINS") this.C_sIS_ADMINS = val.ToString();
            }

            this.fsCREATED_BY = string.Format("{0}{1}"
                    , string.IsNullOrEmpty(this.fsCREATED_BY) ? string.Empty : this.fsCREATED_BY
                    , string.IsNullOrEmpty(this.fsCREATED_BY_NAME) ? string.Empty : string.Format($"({this.fsCREATED_BY_NAME})"));
            this.fsUPDATED_BY = string.Format("{0}{1}"
                    , string.IsNullOrEmpty(this.fsUPDATED_BY) ? string.Empty : this.fsUPDATED_BY
                    , string.IsNullOrEmpty(this.fsUPDATED_BY_NAME) ? string.Empty : string.Format($"({this.fsUPDATED_BY_NAME})"));

            return this;
        }
    }

}
