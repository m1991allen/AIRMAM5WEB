using AIRMAM5.DBEntity.DBEntity;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Directory
{
    /// <summary>
    /// 系統目錄: 新增使用者權限 回傳前端Model: "目錄使用權限"資料列內容+系統目錄未設定過權限的帳號
    /// </summary>
    public class RoleOrUserDirAuthResponse
    {
        /// <summary>
        /// 系統目錄: 新增使用者權限 回傳前端Model: "目錄使用權限"資料列內容+系統目錄未設定過權限的帳號
        /// </summary>
        public RoleOrUserDirAuthResponse() { }

        /// <summary>
        /// 系統目錄: 新增使用者權限 回傳前端Model: "目錄使用權限"資料列內容+系統目錄未設定過權限的帳號
        /// </summary>
        /// <param name="m"> <see cref="spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result"/> </param>
        public RoleOrUserDirAuthResponse(spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result m)
        {
            this.DirAuthority = m;
        }

        /// <summary>
        /// 單一目錄/節點{G群組/U使用者} 權限資料
        /// </summary>
        public spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result DirAuthority { get; set; }

        /// <summary>
        /// 帳號資料選單(取出 系統目錄未設定過權限的帳號)
        /// </summary>
        public List<SelectListItem> RoleOrUserList { get; set; }
    }
}
