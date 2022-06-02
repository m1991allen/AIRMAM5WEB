using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.User
{
    /// <summary>
    /// 使用者資料可編輯欄位 Model,　繼承參考 <see cref="UserListViewModel"/> 
    /// </summary>
    public class UserEditViewModel : UserListViewModel
    {
        /// <summary>
        /// 使用者資料可編輯欄位 Model
        /// </summary>
        public UserEditViewModel() { }

        #region >>> 屬性/欄位參數
        /// <summary>
        /// 英文名稱
        /// </summary>
        [Display(Name = "英文名稱")]
        public string fsENAME { get; set; } = string.Empty;

        /// <summary>
        /// 職稱
        /// </summary>
        [Display(Name = "職稱")]
        public string fsTITLE { get; set; } = string.Empty;

        [Display(Name = "連絡電話")]
        public string fsPHONE { get; set; } = string.Empty;

        /// <summary>
        /// 檔案機密等級權限 (複選)
        /// </summary>
        [Display(Name = "檔案機密等級權限")]
        public string[] FSecretList { get; set; }

        /// <summary>
        /// 預設調用路徑
        /// </summary>
        [Display(Name = "預設調用路徑")]
        public string fsBOOKING_TARGET_PATH { get; set; } = string.Empty;

        /// <summary>
        /// 使用者所屬群組/角色 (複選)
        /// </summary>
        [Display(Name = "所屬群組")]
        public string[] GroupList { get; set; }
        #endregion

        #region >>> 屬性/欄位參數.SelectListItem
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

        /// <summary>
        /// 使用者資料可編輯欄位資料, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_USERS_Result"/> </typeparam>
        /// <param name="m"></param>
        /// <returns> <see cref="UserEditViewModel"/> </returns>
        public new UserEditViewModel FormatConversion<T>(T m)
        {
            if (m == null) { return this; }

            var _Properties = typeof(T).GetProperties();
            foreach (var info in _Properties)
            {
                var val = info.GetValue(m) ?? string.Empty;

                if (info.Name == "fsUSER_ID") this.fsUSER_ID = val.ToString();
                if (info.Name == "fsLOGIN_ID") this.fsLOGIN_ID = val.ToString();
                if (info.Name == "fsNAME") this.fsNAME = val.ToString();
                if (info.Name == "fsENAME") this.fsENAME = val.ToString();
                if (info.Name == "fsTITLE") this.fsTITLE = val.ToString();
                if (info.Name == "fsDEPT_ID") this.fsDEPT_ID = val.ToString();
                if (info.Name == "fsEMAIL") this.fsEMAIL = val.ToString();
                if (info.Name == "fsPHONE") this.fsPHONE = val.ToString();
                if (info.Name == "fsDESCRIPTION") this.fsDESCRIPTION = val.ToString();
                if (info.Name == "fsBOOKING_TARGET_PATH") this.fsBOOKING_TARGET_PATH = val.ToString();

                if (info.Name == "fsIS_ACTIVE")
                {
                    bool.TryParse(val.ToString(), out bool b);
                    this.fsIS_ACTIVE = b;
                }
                if (info.Name == "fsGROUPs")
                {
                    this.GroupList = val.ToString().Split(new char[] { ';' });
                    //this.RoleGroupSelect = _grp.GroupListItemSelected(val.ToString());
                }
                if (info.Name == "fsFILE_SECRET")
                {
                    this.FSecretList = val.ToString().Split(new char[] { ';' });
                    //this.FileSecretSelect = _ser.CodeListItemSelected(TbzCodeIdEnum.FILESECRET.ToString(), val.ToString());
                }

            }

            //this.DeptSelect = _ser.GetCodeItemList(TbzCodeIdEnum.DEPT001.ToString(), true);
            return this;
        }
    }

}
