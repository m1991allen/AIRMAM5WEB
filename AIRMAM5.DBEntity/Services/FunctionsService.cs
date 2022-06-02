using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Function;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 平台功能項目
    /// </summary>
    public class FunctionsService : IFunctionsService
    {
        private IGenericRepository<tbmFUNCTIONS> _funcsRepository = new GenericRepository<tbmFUNCTIONS>();
        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        readonly string LOGTEXT = "FunctionsService";

        public FunctionsService()
        {
            //this._db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }
        public FunctionsService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        #region 【判斷使用者驗證、是否可使用頁面】
        /// <summary>
        /// 判斷使用者驗證、是否可使用頁面
        /// </summary>
        /// <param name="username"> 使用者帳號 fsLOGIN_ID </param>
        /// <param name="controller"> 功能頁面控制項名稱 fsCONTROLLER </param>
        /// <returns></returns>
        public bool CheckUserFuncAuth(string username, string controller)
        {
            bool result = false;
            var query = this.GetFunctionsByParent(username).Where(x => x.FuncType == FunctionTypeEnum.M.ToString()).ToList();

            if (query != null && query.FirstOrDefault(x => x.ControllerName == controller) != null)
            {
                result = true;
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 取 平台功能 父層級項目資料 /或帳號可用功能資料.  【spGET_FUNCTIONS_BY_TYPE】
        /// </summary>
        /// <param name="parentid">預設空值, 僅回傳父層級功能項目. 指定fsLOGIN_ID,取回使用者可用的功能項目資料 </param>
        /// <returns></returns>
        public List<FunctionViewModel> GetFunctionsByParent(string loginid = "")
        {
            List<FunctionViewModel> funcList = new List<FunctionViewModel>();

            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    funcList = _db.spGET_FUNCTIONS_BY_TYPE(loginid)
                        .Select(a => new FunctionViewModel
                        {
                            FuncId = a.fsFUNC_ID,
                            FuncName = a.fsNAME,
                            FuncDescription = a.fsDESCRIPTION,
                            FuncType = a.fsTYPE,
                            FuncOrder = a.fnORDER,
                            FuncIcon = a.fsICON,
                            ParentId = a.fsPARENT_ID,
                            Header = a.fsHEADER,
                            ControllerName = a.fsCONTROLLER,
                            ActionName = a.fsACTION
                        }).ToList();

                    #region 帳號=LDAP, 不顯示「Y0009	變更密碼」
                    if (!string.IsNullOrEmpty(loginid))
                    {
                        var user = _db.tbmUSERS.FirstOrDefault(x => x.fsLOGIN_ID == loginid);
                        if (user.fsPASSWORD.Equals("//"))
                        {
                            //使用LDAP帳號, 不提供密碼變更操作. //不要顯示「Y0009 變更密碼」功能
                            var _remove = funcList.Where(x => x.FuncId == "Y0009").FirstOrDefault();
                            if (_remove != null) funcList.Remove(_remove);
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = LOGTEXT,
                    Method = "[GetFunctionsByParent]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"取平台功能. {ex.Message}")
                });
                #endregion
            }

            return funcList;
        }

        /// <summary>
        /// 依父層ParentId 取 子層級項目資料 【spGET_FUNCTIONS_BY_PARENT_ID】
        /// </summary>
        /// <param name="parentiid">父層級Id</param>
        /// <returns></returns>
        public List<FunctionViewModel> WebFunctionSub(string parentid)
        {
            List<FunctionViewModel> funcList = new List<FunctionViewModel>();

            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    funcList = _db.spGET_FUNCTIONS_BY_PARENT_ID(parentid)
                        .Select(a => new FunctionViewModel
                        {
                            FuncId = a.fsFUNC_ID,
                            FuncName = a.fsNAME,
                            FuncDescription = a.fsDESCRIPTION,
                            FuncType = a.fsTYPE,
                            FuncOrder = a.fnORDER,
                            FuncIcon = a.fsICON,
                            ParentId = a.fsPARENT_ID,
                            Header = a.fsHEADER,
                            ControllerName = a.fsCONTROLLER,
                            ActionName = a.fsACTION
                        }).ToList();

                    if (funcList == null || funcList.FirstOrDefault() == null)
                        return new List<FunctionViewModel>();
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = LOGTEXT,
                    Method = "[WebFunctionSub]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"取平台子層級功能. {ex.Message}")
                });
                #endregion
            }

            return funcList;
        }

        /// <summary>
        /// 指定fsLOGIN_ID(username) 取 平台功能項目資料 父+子 項目列表
        /// </summary>
        /// <param name="login_id"></param>
        /// <returns></returns>
        public List<FunctionMenuViewModel> GetFunctionsMenu(string loginid)
        {
            List<FunctionMenuViewModel> funcMenu = new List<FunctionMenuViewModel>();
            List<FunctionViewModel> parentList = this.GetFunctionsByParent(loginid);

            //foreach (var r in parentList.Where(x => x.FuncType == FunctionTypeEnum.G.ToString()).OrderBy(o => o.FuncOrder))
            //{
            //    if (r != null)
            //    {
            //        FunctionMenuViewModel p = new FunctionMenuViewModel().DataConvert(r);
            //        var getsub = parentList.Where(x => x.ParentId == r.FuncId);
            //        p.SubList = getsub.Any() ? getsub.OrderBy(g => g.FuncOrder).ToList() : new List<FunctionViewModel>();
            //        funcMenu.Add(p);
            //    }
            //}
            parentList.Where(x => x.FuncType == FunctionTypeEnum.G.ToString()).OrderBy(o => o.FuncOrder)
                .ToList().ForEach(f =>
                {
                    FunctionMenuViewModel p = new FunctionMenuViewModel().DataConvert(f);
                    var getsub = parentList.Where(x => x.ParentId == f.FuncId);
                    p.SubList = getsub.Any() ? getsub.OrderBy(g => g.FuncOrder).ToList() : new List<FunctionViewModel>();
                    funcMenu.Add(p);
                });

            return funcMenu;
        }

        /// <summary>
        /// 角色群組 對應所有功能項目(是否可使用)
        /// </summary>
        public RoleFuncMenuViewModel GetFunctionsForRole(string roleid, bool? isdel = null)
        {
            List<FunctionMenuViewModel> funclist = new List<FunctionMenuViewModel>();
            RoleFuncMenuViewModel result = new RoleFuncMenuViewModel();

            try
            {
                //var _role = new GroupsService(_serilogService).FindById(roleid);
                //string rolename = _role == null ? string.Empty : _role.fsNAME;
                
                using (_db = new AIRMAM5DBEntities())
                {
                    var _role = _db.tbmGROUPS.FirstOrDefault(x => x.fsGROUP_ID == roleid);
                    string rolename = _role == null ? string.Empty : _role.fsNAME;

                    var _funcs = _db.spGET_FUNCTIONS_ALL().ToList();
                    var _funcsGroup = _db.spGET_FUNC_GROUP(roleid, "");

                    var get = (from a in _funcs.Where(x => x.fsTYPE != FunctionTypeEnum.X.ToString())
                               join b in _funcsGroup.Where(x => x.fsGROUP_ID == roleid) on new { id = a.fsFUNC_ID, rid = roleid } equals new { id = b.fsFUNC_ID, rid = b.fsGROUP_ID } into objtmp
                               from bb in objtmp.DefaultIfEmpty()
                               select new FunctionViewModel
                               {
                                   FuncId = a.fsFUNC_ID,
                                   FuncName = a.fsNAME,
                                   FuncDescription = a.fsDESCRIPTION ?? "",
                                   FuncType = a.fsTYPE,
                                   FuncOrder = a.fnORDER,
                                   FuncIcon = a.fsICON,
                                   ParentId = a.fsPARENT_ID,
                                   Header = a.fsHEADER,
                                   ControllerName = a.fsCONTROLLER,
                                   ActionName = a.fsACTION,
                                   Usable = bb == null ? false : true
                               }).ToList();

                    ////子層項目加入父層
                    //foreach (var r in get.Where(x => x.FuncType == FunctionTypeEnum.G.ToString()).OrderBy(o => o.FuncOrder))
                    //{
                    //    if (r != null)
                    //    {
                    //        FunctionMenuViewModel p = new FunctionMenuViewModel().DataConvert(r);
                    //        var getsub = get.Where(x => x.ParentId == r.FuncId);
                    //        p.SubList = getsub.Any() ? getsub.OrderBy(g => g.FuncOrder).ToList() : new List<FunctionViewModel>();
                    //        funclist.Add(p);
                    //    }
                    //}
                    get.Where(x => x.FuncType == FunctionTypeEnum.G.ToString()).OrderBy(o => o.FuncOrder)
                        .ToList().ForEach(f =>
                        {
                            FunctionMenuViewModel p = new FunctionMenuViewModel().DataConvert(f);
                            var getsub = get.Where(x => x.ParentId == f.FuncId);
                            p.SubList = getsub.Any() ? getsub.OrderBy(g => g.FuncOrder).ToList() : new List<FunctionViewModel>();
                            funclist.Add(p);
                        });

                    result.RoleId = roleid;
                    result.RoleName = rolename;
                    result.FuncItemList = funclist;
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = LOGTEXT,
                    Method = "[GetFunctionsForRole]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"角色與功能. {ex.Message}")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 取回系統設定的"快捷功能項目"資料
        /// </summary>
        /// <returns></returns>
        public List<tbmFUNCTIONS> GetQuickMenu()
        {
            var query = _funcsRepository.FindBy(x => x.fsTYPE == FunctionTypeEnum.M.ToString() && x.fbIS_QUICK_MENU == true);

            return query.ToList();
        }

    }
}
