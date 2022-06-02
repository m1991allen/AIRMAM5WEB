using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.DBEntity;
using System.Net;
using AIRMAM5.Filters;
using AIRMAM5.DBEntity.Models.Directory;
using AIRMAM5.DBEntity.Interface;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 媒資管理 > 系統目錄維護
    /// <para> Tips_20200518: 只會開放給 Administrator, MediaManager </para>
    /// <para> Tips_20200518: root節點的管理群組設定: Administrator, MediaManager </para>
    /// </summary>
    [Authorize(Roles = "Administrator,MediaManager")]
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class DirController : BaseController
    {
        readonly string CONTR_NAEM = "Dir";
        readonly ISubjectService _subjectService;
        readonly DirectoriesService _directoriesService;
        readonly DirectoriesUserService _directoriesUserService;
        readonly DirectoriesGroupService _directoriesGroupService;
        readonly GroupsService _groupService;
        readonly UsersService _userService;
        /// <summary>
        /// 
        /// </summary>
        public DirController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService, ISubjectService subjectService
            , ICodeService codeService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _tbzCodeService = codeService;
            _directoriesService = new DirectoriesService();
            _subjectService = subjectService;
            _directoriesUserService = new DirectoriesUserService();
            _directoriesGroupService = new DirectoriesGroupService();
            _groupService = new GroupsService(serilogService);
            _userService = new UsersService(serilogService);
        }
        ///* Marked_20210830 */
        //public DirController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}

        /// <summary>
        /// 系統目錄維護 首頁
        /// <para> Tips: 整個Controller有限制授權,所以會直接導向至登入頁,需要AllowAnonymous,才會進入動作檢查Index授權,才會導到無權限頁面 </para>
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return View("NoAuth");
            //var g = GetDir(0, "");
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統目錄維護"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 系統目錄樹狀節點 (id=0，Root directory)
        /// </summary>
        /// <param name="id">系統目錄編號 fnDIR_ID</param>
        /// <param name="fsKEYWORD"></param>
        /// <param name="showcount">是否顯示主題數量, 預設:false 不顯示</param>
        /// <returns></returns>
        public JsonResult GetDir(long id, string fsKEYWORD, bool showcount = false)
        {
            ////取得樹狀
            var param = new GetDirLoadOnDemandSearchModel
            {
                DirId = id,
                KeyWord = fsKEYWORD,
                UserName = User.Identity.Name,
                ShowSubJ = showcount        //是否顯示主題數量
                , IsShowAll = true          //20200904_維護功能要 顯示全部的目錄資料
            };

            List<DirectoriesItemModel> get = _directoriesService.DirSubList(param);

            #region _Serilog(Verbose)
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = CONTR_NAEM,
                Method = "[GetDir]",
                EventLevel = SerilogLevelEnum.Verbose,
                Input = new { Param = param, Result = get },
                LogString = "系統目錄.Result"
            });
            #endregion
            return Json(get, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 節點內容 : 目錄資訊與欄位
        /// </summary>
        /// <param name="id">系統目錄編號 fnDIR_ID</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult _DirInfo(long id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return View("NoAuth");
            var get = _directoriesService.GetDirectoriesById(id);
            /* 20201116_TIPS: 系統不啟用"目錄未節點queue"時, 欄位:目錄類型 [fsDIRTYPE] 一律顯示空值，從資料來源預存程序處理。 */

            //目錄開放類型 fsSHOWTYPE
            //_tbzCodeService = new CodeService();
            get.fsSHOWTYPE = _tbzCodeService.GetCodeName(TbzCodeIdEnum.DIR002, get.fsSHOWTYPE);

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "目錄節點內容:資訊與欄位", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { SubjectId = id }),
                User.Identity.Name);
            #endregion
            return PartialView("_DirInfo", get);
        }

        /// <summary>
        /// 節點內容 : 目錄使用權限
        /// </summary>
        /// <param name="id">系統目錄編號 fnDIR_ID</param>
        /// <param name="type">頁面上的「欄位類型」：G群組、U使用者 </param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult _DirAuth(long id, string type)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return View("NoAuth");
            var _param = new GetDirAuthoritySearchModel(id, type);

            DirectoriesAuthorithModel result = new DirectoriesAuthorithModel(id, type);
            var get = _directoriesService.GetDirectoriesAuthById(_param);
            result.DirAuthority = (get == null || get.FirstOrDefault() == null) ? new List<spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result>() : get;
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "目錄節點內容:使用權限("+type+")", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { SubjectId = id }),
                User.Identity.Name);
            #endregion
            return PartialView("_DirAuth", result);
        }

        #region 目錄使用權限: G群組、U使用者 新增
        /// <summary>
        /// 新增 目錄-{G群組/U使用者}-使用權限 View
        /// </summary>
        /// <param name="id">系統目錄編號 fnDIR_ID</param>
        /// <param name="type">指定新增的欄位類型 : G群組 / U使用者 </param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult _DirAuthCreate(long id, string type)
        {
            if (type.ToUpper() == "G")
            {
                if (!CheckUserAuth(CONTR_NAEM))
                    return RedirectToAction("NoAuthModal", "Error", new { @id = "CreateGroupModal" });

                //tbmDIR_GROUP dirgroup = new tbmDIR_GROUP(id) { AuthType = type };
                /* → ↓ marked_&_modified_20210902 *///CreateGroupDirAuthModel dirgroup = new CreateGroupDirAuthModel(id); //20201228_ViewModel調整
                CreateGroupDirAuthModel dirgroup = new CreateGroupDirAuthModel()
                {
                    fnDIR_ID = id,
                    RoleList = _groupService.GetRolesByDirId(id),
                    OperationList = GetEnums.GetOperationAuthority()
                };

                return PartialView("_DirGroupAuthCreate", dirgroup); //群組權限view
            }

            if (!CheckUserAuth(CONTR_NAEM))
            { return RedirectToAction("NoAuthModal", "Error", new { @id = "CreateUserModal" }); }

            //tbmDIR_USER diruser = new tbmDIR_USER(id) { AuthType = type };
            CreateUserDirAuthModel diruser = new CreateUserDirAuthModel /*Marked_&_Modified_20210903 */
            {
                fnDIR_ID = id,
                UserList = _userService.GetRolesByDirId(id, string.Empty, false),
                OperationList = GetEnums.GetOperationAuthority()
            };

            return PartialView("_DirUserAuthCreate", diruser); //使用者權限view
        }

        /// <summary>
        /// 新增 目錄-使用者權限 SAVE
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateUserAuth(tbmDIR_USER dirUser)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, dirUser);
            VerifyResult res = new VerifyResult();

            try
            {
                #region ___檢查___
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (!ModelState.IsValid)
                {
                    foreach (var item in ModelState.Keys)
                        if (ModelState[item].Errors.Count > 0)
                        {
                            result.IsSuccess = false;
                            result.Message = ModelState[item].Errors[0].ErrorMessage;
                            result.StatusCode = HttpStatusCode.BadRequest; //400-Bad Request：錯誤的要求。
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                }
                #endregion

                dirUser.fdCREATED_DATE = DateTime.Now;
                dirUser.fsCREATED_BY = User.Identity.Name;
                res = _directoriesUserService.CreateBy(dirUser);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region --成功回覆前端"目錄使用權限"資料列內容: spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result--
                if (res.IsSuccess)
                {
                    var _p = new GetDirAuthoritySearchModel(dirUser.fnDIR_ID, "U");
                    var get = _directoriesService.GetDirectoriesAuthById(_p).Where(x => x.LOGIN_ID == dirUser.fsLOGIN_ID).FirstOrDefault();
                    /*Tip_20201228_回覆內容調整*///res.Data = get; //_DirAuthority;

                    //Tip_20201228_回覆內容調整為: "目錄使用權限"資料列內容+系統目錄未設定過權限的帳號List(提供前端更新下拉內容使用)
                    List<SelectListItem> _ddlUser = _userService.GetRolesByDirId(dirUser.fnDIR_ID, string.Empty, false);
                    RoleOrUserDirAuthResponse resp = new RoleOrUserDirAuthResponse(get) { RoleOrUserList = _ddlUser };
                    res.Data = resp; 
                }
                #endregion
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "目錄使用者權限", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(dirUser),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = dirUser,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[CreateUserAuth]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = dirUser, Result = res, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 新增 目錄-角色群組權限 SAVE
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateGroupAuth(tbmDIR_GROUP dirGroup)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, dirGroup);
            VerifyResult res = new VerifyResult();

            try
            {
                #region ___檢查___
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (!ModelState.IsValid)
                {
                    foreach (var item in ModelState.Keys)
                        if (ModelState[item].Errors.Count > 0)
                        {
                            result.IsSuccess = false;
                            result.Message = ModelState[item].Errors[0].ErrorMessage;
                            result.StatusCode = HttpStatusCode.BadRequest; //400-Bad Request：錯誤的要求。
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                }
                #endregion

                dirGroup.fdCREATED_DATE = DateTime.Now;
                dirGroup.fsCREATED_BY = User.Identity.Name;
                res = _directoriesGroupService.CreateBy(dirGroup);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region --成功回覆前端"目錄使用權限"資料列內容: spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result--
                if (res.IsSuccess)
                {
                    var _p = new GetDirAuthoritySearchModel(dirGroup.fnDIR_ID);
                    var get = _directoriesService.GetDirectoriesAuthById(_p).Where(x => x.GROUP_ID == dirGroup.fsGROUP_ID).FirstOrDefault();
                    /*Tip_20201228_回覆內容調整*/
                    //var _DirAuthority = get ?? new spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result();
                    //res.Data = _DirAuthority;

                    //TIP_20201228_回覆內容調整為: "目錄使用權限"資料列內容+系統目錄未設定過權限的角色List(提供前端更新下拉內容使用)
                    List<SelectListItem> _ddlrole = _groupService.GetRolesByDirId(dirGroup.fnDIR_ID);
                    RoleOrUserDirAuthResponse resp = new RoleOrUserDirAuthResponse(get) { RoleOrUserList = _ddlrole };
                    res.Data = resp;
                }
                #endregion

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "目錄群組權限", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(dirGroup),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = dirGroup,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[CreateGroupAuth]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = dirGroup, Result = res, Exception = ex },
                    LogString = "新增目錄-角色群組權限.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 目錄使用權限: G群組、U使用者 修改
        /// <summary>
        /// 修改 目錄-{G群組/U使用者}-權限 View
        /// </summary>
        /// <param name="id">系統目錄編號 fnDIR_ID</param>
        /// <param name="type">指定新增的欄位類型 : G群組/U使用者 </param>
        /// <param name="idvalue">群組ID / 使用者帳號 </param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult _DirAuthEdit(long id, string type, string idvalue="")
        {
            if (!CheckUserAuth(CONTR_NAEM))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "EditModal" });

            var _param = new GetDirAuthoritySearchModel(id, type);// { DirId = id, AuthType = type };
            var get = _directoriesService.GetDirectoriesAuthById(_param);

            DirAuthEditModel result = get.Select(s => new DirAuthEditModel().ConvertData(s)).FirstOrDefault(x => x.UserId == idvalue);

            if (type.ToUpper() == "G")
            {
                result = get.Select(s => new DirAuthEditModel().ConvertData(s)).FirstOrDefault(x => x.GroupId == idvalue);
                return PartialView("_DirGroupAuthEdit", result);
            }

            return PartialView("_DirUserAuthEdit", result);
        }

        /// <summary>
        /// 修改 目錄-群組-權限 SAVE
        /// </summary>
        /// <param name="clsDIR_AUTHORITY"></param>
        [HttpPost]
        public ActionResult EditGroupAuth(DirAuthEditModel model)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, model);
            VerifyResult res = new VerifyResult();
            try
            {
                #region ___檢查___
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (!ModelState.IsValid)
                {
                    foreach (var item in ModelState.Keys)
                        if (ModelState[item].Errors.Count > 0)
                        {
                            result.IsSuccess = false;
                            result.Message = ModelState[item].Errors[0].ErrorMessage;
                            result.StatusCode = HttpStatusCode.BadRequest;
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                }
                #endregion

                #region --查無資料, 權限若是"繼承"上層,資料表dbo.tbmDIR_GROUP 不會有資料, 要新增進行修改。
                var _upd = _directoriesGroupService.GetBy(model.DirId, model.GroupId).FirstOrDefault();
                if (_upd == null)
                {
                    _upd = new tbmDIR_GROUP(model)
                    {
                        fdCREATED_DATE = DateTime.Now,
                        fsCREATED_BY = User.Identity.Name
                    };
                    res = _directoriesGroupService.CreateBy(_upd);
                }
                else
                {
                    _upd.fsLIMIT_SUBJECT = string.Join(",", model.LimitSubject);
                    _upd.fsLIMIT_VIDEO = string.Join(",", model.LimitVideo);
                    _upd.fsLIMIT_AUDIO = string.Join(",", model.LimitAudio);
                    _upd.fsLIMIT_PHOTO = string.Join(",", model.LimitPhoto);
                    _upd.fsLIMIT_DOC = string.Join(",", model.LimitDoc);
                    _upd.fdUPDATED_DATE = DateTime.Now;
                    _upd.fsUPDATED_BY = User.Identity.Name;
                    res = _directoriesGroupService.UpdateBy(_upd);
                }
                #endregion

                string _str = res.IsSuccess ? "成功" : "失敗";
                #region --成功回覆前端"目錄使用權限"資料列內容: spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result--
                if (res.IsSuccess)
                {
                    var _p = new GetDirAuthoritySearchModel(model.DirId);
                    //{
                    //    DirId = model.DirId,
                    //    AuthType = "G"
                    //};
                    var get = _directoriesService.GetDirectoriesAuthById(_p).Where(x => x.GROUP_ID == model.GroupId).FirstOrDefault();
                    var _DirAuthority = get ?? new spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result();
                    res.Data = _DirAuthority;
                }
                #endregion

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "目錄群組權限", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = model,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[EditGroupAuth]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = model, Result = res, Exception = ex },
                    LogString = "更新目錄-群組權限.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 更新 目錄-使用者-權限 SAVE
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditUserAuth(DirAuthEditModel model)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, model);
            VerifyResult res = new VerifyResult();

            try
            {
                #region ___檢查___
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (!ModelState.IsValid)
                {
                    foreach (var item in ModelState.Keys)
                        if (ModelState[item].Errors.Count > 0)
                        {
                            result.IsSuccess = false;
                            result.Message = ModelState[item].Errors[0].ErrorMessage;
                            result.StatusCode = HttpStatusCode.BadRequest;
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                }
                #endregion

                #region --查無資料, 權限若是"繼承"上層,資料表dbo.tbmDIR_USER 不會有資料, 要新增進行修改。
                var _upd = _directoriesUserService.GetBy(model.DirId, model.LoginId).FirstOrDefault();
                if (_upd == null)
                {
                    //權限若是"繼承"上層,資料表dbo.tbmDIR_USER 不會有資料, 要新增進行修改。
                    _upd = new tbmDIR_USER(model)
                    {
                        fdCREATED_DATE = DateTime.Now,
                        fsCREATED_BY = User.Identity.Name
                    };
                    res = _directoriesUserService.CreateBy(_upd);
                    res.Message = "系統目錄使用者 權限已更新.";
                }
                else
                {
                    _upd.fsLIMIT_SUBJECT = string.Join(",", model.LimitSubject);
                    _upd.fsLIMIT_VIDEO = string.Join(",", model.LimitVideo);
                    _upd.fsLIMIT_AUDIO = string.Join(",", model.LimitAudio);
                    _upd.fsLIMIT_PHOTO = string.Join(",", model.LimitPhoto);
                    _upd.fsLIMIT_DOC = string.Join(",", model.LimitDoc);
                    _upd.fdUPDATED_DATE = DateTime.Now;
                    _upd.fsUPDATED_BY = User.Identity.Name;
                    res = _directoriesUserService.UpdateBy(_upd);
                }
                #endregion

                string _str = res.IsSuccess ? "成功" : "失敗";
                #region --成功回覆前端"目錄使用權限"資料列內容: spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result--
                if (res.IsSuccess)
                {
                    var _p = new GetDirAuthoritySearchModel(model.DirId, "U");
                    //{
                    //    DirId = model.DirId,
                    //    AuthType = "U"
                    //};
                    var get = _directoriesService.GetDirectoriesAuthById(_p).Where(x => x.LOGIN_ID == model.LoginId).FirstOrDefault();
                    var _DirAuthority = get ?? new spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result();
                    res.Data = _DirAuthority;
                }
                #endregion

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "目錄使用者權限", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = model,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[EditUserAuth]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = model, Result = res, exception = ex },
                    LogString = "更新目錄-使用者權限.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 目錄使用權限: G群組、U使用者 刪除
        /// <summary>
        /// 刪除 目錄-群組/使用者-權限 SAVE
        /// </summary>
        /// <param name="id">系統目錄編號 fnDIR_ID</param>
        /// <param name="type">指定新增的欄位類型 : G群組/U使用者 </param>
        /// <param name="idvalue">群組ID / 使用者ID </param>
        [HttpPost]
        public ActionResult DeleteOperationAuth(long id, string type, string idvalue)
        {
            var _param = new { id, type, idvalue };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region ___檢查___
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(idvalue))
                {
                    result.IsSuccess = false;
                    result.Message = "刪除條件錯誤";
                    result.StatusCode = HttpStatusCode.BadRequest; //400-Bad Request：錯誤的要求。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                //TIP_20201228_回覆內容調整為: "目錄使用權限"資料列內容+系統目錄未設定過權限的角色List(提供前端更新下拉內容使用)
                List<SelectListItem> _ddlList = _groupService.GetRolesByDirId(id);

                string _typeStr = type.ToUpper() == "G" ? "群組" : "使用者";
                if (type.ToUpper() == "G")
                {
                    //_typeStr = "群組";
                    res = _directoriesGroupService.DeleteBy(id, idvalue, User.Identity.Name);

                    _ddlList = _groupService.GetRolesByDirId(id);
                    RoleOrUserDirAuthResponse resp = new RoleOrUserDirAuthResponse() { RoleOrUserList = _ddlList };
                    res.Data = resp;
                }
                else if (type.ToUpper() == "U")
                {
                    //_typeStr = "使用者";
                    string _urnm = _userService.GetBy(idvalue).FirstOrDefault().fsLOGIN_ID;
                    res = _directoriesUserService.DeleteBy(id, _urnm, User.Identity.Name);

                    _ddlList = _userService.GetRolesByDirId(id, string.Empty, false);
                    RoleOrUserDirAuthResponse resp = new RoleOrUserDirAuthResponse() { RoleOrUserList = _ddlList };
                    res.Data = resp;
                }


                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M003",        //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "目錄" + _typeStr + "權限", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[DeleteOperationAuth]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region (樹狀)目錄: 新增、編輯、刪除
        /// <summary>
        /// 新增 目錄  View
        /// </summary>
        /// <remarks>
        /// <para> Tips_20200518: 只會開放給 Administrator, MediaManager </para>
        /// </remarks>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult _CreateDir(long id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "CreateDirModal" });

            // ↓↓設定系統目錄維護功能中，是否啟用Queue節點操作
            var cong = new ConfigService().GetConfigBy("DIRECTORIES_USING_QUEUE").FirstOrDefault();

            // ↓↓取父節點資訊
            spGET_DIRECTORIES_Result dirParent = _directoriesService.GetDirectoriesById(id);
            DirectoryEditModel m = new DirectoryEditModel
            {
                UserList = _userService.GetUsersList(),
                GroupList = _groupService.GetUserRoles(),
                TemplateList = new TemplateService().GetByParam(0, ""),
                DirShowTypeList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.DIR002.ToString(), true),
                UsingQueue = cong == null ? false : (cong.fsVALUE == "1" ? true : false)
            };

            if (dirParent != null)
            {
                //m = new DirectoryEditModel(dirParent)
                //{
                //    /*新增預設不用填值: 目錄標題名稱,目錄管理群組,目錄管理使用者,目錄描述。*/
                //    fsNAME = string.Empty,//string.Format($"{dirParent.fsNAME}(複)"), 
                //    fsDESCRIPTION = string.Empty,
                //    DirGroupsAry = new string[] { },
                //    DirUsersAry = new string[] { },
                //    fnPARENT_ID = dirParent.fnDIR_ID //設定父層DirId
                //    , UsingQueue = this.IsUsingQueue
                //};
                m = m.FormatConversion(dirParent);

                /*新增預設不用填值: 目錄標題名稱,目錄管理群組,目錄管理使用者,目錄描述。*/
                m.fsNAME = string.Empty;
                m.fsDESCRIPTION = string.Empty;
                m.DirGroupsAry = new string[] { };
                m.DirUsersAry = new string[] { };
                m.fnPARENT_ID = dirParent.fnDIR_ID; //設定父層DirId
                m.UsingQueue = this.IsUsingQueue;
            }

            return PartialView("_CreateDir", m);//目錄新增View
        }

        /// <summary>
        /// 新增 目錄  SAVE
        /// <para> Tips_20200518: 目錄管理群組、使用者=空值, 表示繼承自母節點管理權限 </para>
        /// <para> Tips_20200518: root節點的管理群組設定: Administrator, MediaManager </para>
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateDir(tbmDIRECTORIES model)
        {
            //Tips_20200518：目錄管理使用者/群組 選單沒有 -全部- 選項
            //if (model.fsADMIN_GROUP == "*") { model.fsADMIN_GROUP = string.Empty; }
            //if (model.fsADMIN_USER == "*") { model.fsADMIN_USER = string.Empty; }
            ResponseResultModel result = new ResponseResultModel(true, "", model);
            VerifyResult res = new VerifyResult();

            try
            {
                #region ___檢查___
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden; //403-Forbidden：禁止使用。沒有權限。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (!ModelState.IsValid)
                {
                    foreach (var item in ModelState.Keys)
                        if (ModelState[item].Errors.Count > 0)
                        {
                            result.IsSuccess = false;
                            result.Message = ModelState[item].Errors[0].ErrorMessage;
                            result.StatusCode = HttpStatusCode.BadRequest;
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                }
                #endregion

                model.fsADMIN_USER = string.IsNullOrEmpty(model.fsADMIN_USER) ? string.Empty : string.Format($"{model.fsADMIN_USER};");
                model.fsADMIN_GROUP = string.IsNullOrEmpty(model.fsADMIN_GROUP) ? string.Empty : string.Format($"{model.fsADMIN_GROUP};");
                model.fsDIRTYPE = model.IsQueue ? "Q" : string.Empty;
                model.fsDESCRIPTION = model.fsDESCRIPTION ?? string.Empty;
                model.fdCREATED_DATE = DateTime.Now;
                model.fsCREATED_BY = User.Identity.Name;
                res = _directoriesService.CreateBy(model);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), "M001",
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統目錄", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion
                result = new ResponseResultModel(res)
                {
                    Records = model,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog.ERR
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[CreateDir]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = model, Result = res, Exception = ex },
                    LogString = "新增目錄.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 編輯 目錄  View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult _EditDir(long id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "EditDirModal" });

            //取節點資訊
            spGET_DIRECTORIES_Result get = _directoriesService.GetDirectoriesById(id);
            //DirectoryEditModel m = get == null ? new DirectoryEditModel() : new DirectoryEditModel(_serilogService).FormatConversion(get);

            // ↓↓設定系統目錄維護功能中，是否啟用Queue節點操作
            var cong = new ConfigService().GetConfigBy("DIRECTORIES_USING_QUEUE").FirstOrDefault();

            DirectoryEditModel m = new DirectoryEditModel
            {
                UserList = _userService.GetUsersList(),
                GroupList = _groupService.GetUserRoles(),
                TemplateList = new TemplateService().GetByParam(0, ""),
                DirShowTypeList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.DIR002.ToString(), true),
                UsingQueue = cong == null ? false : (cong.fsVALUE == "1" ? true : false)
            };

            if (get != null) {  m = m.FormatConversion(get); }

            return PartialView("_EditDir", m);//目錄編輯View
        }

        /// <summary>
        /// 編輯 目錄  SAVE
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditDir(tbmDIRECTORIES model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region ___檢查___
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden; //403-Forbidden：禁止使用。沒有權限。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (!ModelState.IsValid)
                {
                    foreach (var item in ModelState.Keys)
                        if (ModelState[item].Errors.Count > 0)
                        {
                            result.IsSuccess = false;
                            result.Message = ModelState[item].Errors[0].ErrorMessage;
                            result.StatusCode = HttpStatusCode.BadRequest;
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                }
                
                var _rec = _directoriesService.GetDirById(model.fnDIR_ID);
                if (_rec == null)
                {
                    result.IsSuccess = false;
                    result.Message = "目錄資料不存在,請確認";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                //若此節點已經有主題，則不可變更各類型樣板
                var _subj = _subjectService.GetBy(null, model.fnDIR_ID);
                if (_subj != null && _subj.fnDIR_ID == model.fnDIR_ID)
                {
                    if (_rec.fnTEMP_ID_SUBJECT != model.fnTEMP_ID_SUBJECT || _rec.fnTEMP_ID_VIDEO != model.fnTEMP_ID_VIDEO ||
                        _rec.fnTEMP_ID_AUDIO != model.fnTEMP_ID_AUDIO || _rec.fnTEMP_ID_PHOTO != model.fnTEMP_ID_PHOTO || _rec.fnTEMP_ID_DOC != model.fnTEMP_ID_DOC)
                    {
                        result.IsSuccess = false;
                        result.Message = "此節點已存在主題，不可變更樣板";
                        result.StatusCode = HttpStatusCode.BadRequest;
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                }
                #endregion

                _rec.fsNAME = model.fsNAME;
                _rec.fnORDER = model.fnORDER;
                //_rec.fsDIRTYPE = model.IsQueue ? "Q" : string.Empty; //20201119:目錄類型不能編輯修改.
                _rec.fnTEMP_ID_SUBJECT = model.fnTEMP_ID_SUBJECT;
                _rec.fnTEMP_ID_VIDEO = model.fnTEMP_ID_VIDEO;
                _rec.fnTEMP_ID_AUDIO = model.fnTEMP_ID_AUDIO;
                _rec.fnTEMP_ID_PHOTO = model.fnTEMP_ID_PHOTO;
                _rec.fnTEMP_ID_DOC = model.fnTEMP_ID_DOC;
                _rec.fsSHOWTYPE = model.fsSHOWTYPE;
                _rec.fsADMIN_USER = string.IsNullOrEmpty(model.fsADMIN_USER) ? string.Empty : string.Format($"{model.fsADMIN_USER};");
                _rec.fsADMIN_GROUP = string.IsNullOrEmpty(model.fsADMIN_GROUP) ? string.Empty : string.Format($"{model.fsADMIN_GROUP};");
                _rec.fsDESCRIPTION = model.fsDESCRIPTION;
                _rec.fdUPDATED_DATE = DateTime.Now;
                _rec.fsUPDATED_BY = User.Identity.Name;

                res = _directoriesService.UpdateBy(_rec);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統目錄", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion
                result = new ResponseResultModel(res)
                {
                    Records = model,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[EditDir]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = model, Result = res, Exception = ex },
                    LogString = "編輯目錄.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 刪除 目錄  View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult _DeleteDir(long id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "DeleteDirModal" });

            //取節點資訊
            spGET_DIRECTORIES_Result get = _directoriesService.GetDirectoriesById(id);

            //DirectoryEditModel m = get == null ? new DirectoryEditModel(_serilogService) : new DirectoryEditModel(_serilogService).FormatConversion(get);

            // ↓↓設定系統目錄維護功能中，是否啟用Queue節點操作
            var cong = new ConfigService().GetConfigBy("DIRECTORIES_USING_QUEUE").FirstOrDefault();

            DirectoryEditModel m = new DirectoryEditModel
            {
                UserList = _userService.GetUsersList(),
                GroupList = _groupService.GetUserRoles(),
                TemplateList = new TemplateService().GetByParam(0, ""),
                DirShowTypeList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.DIR002.ToString(), true),
                UsingQueue = cong == null ? false : (cong.fsVALUE == "1" ? true : false)
            };

            if (get != null) { m = m.FormatConversion(get); }

            return PartialView("_DeleteDir", m);
        }

        /// <summary>
        /// 刪除 目錄  POST
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteDir(long id)
        {
            var _param = new { id };
            ResponseResultModel result = new ResponseResultModel(true, "", _param);
            VerifyResult res = new VerifyResult();

            try
            {
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden; //403-Forbidden：禁止使用。沒有權限。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                res = _directoriesService.DeleteBy(id, User.Identity.Name);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M003",        //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統目錄", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[DeleteDir]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

    }
}
