using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Function;
using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Interface
{
    public interface IFunctionsService
    {
        #region 【判斷使用者驗證、是否可使用頁面】
        /// <summary>
        /// 判斷使用者驗證、是否可使用頁面
        /// </summary>
        /// <param name="username"> 使用者帳號 fsLOGIN_ID </param>
        /// <param name="controller"> 功能頁面控制項名稱 fsCONTROLLER </param>
        /// <returns></returns>
        bool CheckUserFuncAuth(string username, string controller);
        #endregion

        /// <summary>
        /// 取 平台功能 父層級項目資料 /或帳號可用功能資料.  【spGET_FUNCTIONS_BY_TYPE】
        /// </summary>
        /// <param name="parentid">預設空值, 僅回傳父層級功能項目. 指定fsLOGIN_ID,取回使用者可用的功能項目資料 </param>
        /// <returns></returns>
        List<FunctionViewModel> GetFunctionsByParent(string loginid = "");

        /// <summary>
        /// 依父層ParentId 取 子層級項目資料 【spGET_FUNCTIONS_BY_PARENT_ID】
        /// </summary>
        /// <param name="parentiid">父層級Id</param>
        /// <returns></returns>
        List<FunctionViewModel> WebFunctionSub(string parentid);

        /// <summary>
        /// 指定fsLOGIN_ID(username) 取 平台功能項目資料 父+子 項目列表
        /// </summary>
        /// <param name="login_id"></param>
        /// <returns></returns>
        List<FunctionMenuViewModel> GetFunctionsMenu(string loginid);

        /// <summary>
        /// 角色群組 對應所有功能項目(是否可使用)
        /// </summary>
        RoleFuncMenuViewModel GetFunctionsForRole(string roleid, bool? isdel = null);

        /// <summary>
        /// 取回系統設定的"快捷功能項目"資料
        /// </summary>
        /// <returns></returns>
        List<tbmFUNCTIONS> GetQuickMenu();

    }
}
