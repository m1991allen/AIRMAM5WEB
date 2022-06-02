using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net;
using AIRMAM5.DBEntity.Services;
using System.Threading.Tasks;
using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.Filters;
using AIRMAM5.DBEntity.Models.Role;
using AIRMAM5.DBEntity.Models.Function;
using AIRMAM5.DBEntity.Interface;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 角色群組維護 Controller
    /// </summary>
    [Authorize]
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class GroupController : BaseController
    {
        readonly GroupsService _rolesService;

        public GroupController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _rolesService = new GroupsService(serilogService);
            _serilogService = serilogService;
            _tblLogService = tblLogService;
        }

        /// <summary>
        /// 角色群組維護Index
        /// <para> Tips: 整個Controller有限制授權,所以會直接導向至登入頁,需要AllowAnonymous,才會進入動作檢查Index授權,才會導到無權限頁面 </para>
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (!CheckUserAuth("Group")) return PartialView("NoAuth");
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "角色群組維護"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 查詢功能
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Search()
        {
            ResponseResultModel result = new ResponseResultModel(true, "", "");
            if (!CheckUserAuth("Group"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var get = await _rolesService.RolesCounter();
            result.IsSuccess = true;
            result.Message = "OK";
            result.Data = get;
            result.StatusCode = HttpStatusCode.OK;
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 顯示 角色群組的帳號資料
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowAccount(string id)
        {
            ResponseResultModel result = new ResponseResultModel(true);
            if (!CheckUserAuth("Group"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var get = _rolesService.RoleAccount(id);
            result.IsSuccess = true;
            result.Message = "OK";
            result.Data = get;
            result.StatusCode = HttpStatusCode.OK;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 詳細內容
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>2020.9.2：未使用。</remarks>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult _Details(string id)
        {
            if (!CheckUserAuth("Group")) return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });
            var vmodel = _rolesService.FindById(id);
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "角色群組維護", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { GroupID = id }),
                User.Identity.Name);
            #endregion
            return PartialView("_Details", vmodel);
        }

        /// <summary>
        /// 群組/角色  新增Modal頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult _Create()
        {
            if (!CheckUserAuth("Group")) return RedirectToAction("NoAuthModal", "Error", new { @id = "CreateModal" });
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "角色群組維護-新增"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion
            tbmGROUPS m = new tbmGROUPS();
            return PartialView("_Create", m);
        }

        /// <summary>
        /// 群組/角色  新增 POST
        /// </summary>
        /// <param name="clsGROUPS"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create(tbmGROUPS groups)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, groups);
            VerifyResult res = new VerifyResult();
            try
            {
                #region _檢查_
                if (!CheckUserAuth("Group"))
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

                groups.fsGROUP_ID = Guid.NewGuid().ToString();
                groups.fsTYPE = string.Empty;
                groups.fsCREATED_BY = User.Identity.Name;
                groups.fdCREATED_DATE = DateTime.Now;
                groups.Discriminator = "ApplicationRole";
                //Tips: dbo.tbmGROUPS.[Discriminator] ='ApplicationRole' / 'IdentityRole'  AppRoleManager 才會讀得到.

                res = _rolesService.Create(groups);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "角色群組", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(groups),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = groups,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
            }
            catch(Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Group",
                    Method = "[Create]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = groups, Result = res, Exception = ex},
                    LogString = "新增角色.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                ModelState.AddModelError("", ex.Message);
            }

            return await Task.Run(() => Json(result, JsonRequestBehavior.DenyGet));
        }

        /// <summary>
        /// 群組/角色 編輯
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Edit(string id)
        {
            if (!CheckUserAuth("Group")) return RedirectToAction("NoAuthModal", "Error", new { @id = "EditModal" });
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "角色群組維護-編輯"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion

            var get = _rolesService.FindById(id);
            var m = new GroupsViewModel
            {
                RoleId = get.fsGROUP_ID,
                RoleName = get.fsNAME,
                Description = get.fsDESCRIPTION
            };
            return View("_Edit", m);
        }

        /// <summary>
        /// 群組/角色 編輯 POST
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(GroupsViewModel groups)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, groups);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢查_
                if (!CheckUserAuth("Group"))
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

                var _upd = _rolesService.FindById(groups.RoleId);
                if (_upd == null)
                {
                    result.IsSuccess = false;
                    result.Message = "查無角色群組資料";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (_rolesService.IsExistsByName(groups.RoleId, groups.RoleName))
                {
                    result.IsSuccess = false;
                    result.Message = "角色群組名稱重複";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                _upd.fsNAME = groups.RoleName;
                _upd.fsDESCRIPTION = groups.Description;
                _upd.fdUPDATED_DATE = DateTime.Now;
                _upd.fsUPDATED_BY = User.Identity.Name;
                //var g = new tbmGROUPS(groups) { fsUPDATED_BY = User.Identity.Name };
                res = _rolesService.Update(_upd);
                string _res = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "角色群組", _res),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(groups),
                    User.Identity.Name);
                #endregion
                result = new ResponseResultModel(res)
                {
                    Records = groups,
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
                    Controller = "Group",
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = groups, Result = res, Exception = ex },
                    LogString = "修改角色.Exception",
                    ErrorMessage = string.Format($"修改角色失敗. {ex.Message}")
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 群組/角色 刪除 Modal頁面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Delete(string id) {
            if (!CheckUserAuth("Group")) return RedirectToAction("NoAuthModal", "Error", new { @id = "DeleteModal" });
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "角色群組維護-刪除檢視"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion

            var vmodel = _rolesService.FindById(id);
            return PartialView("_Delete", vmodel);
        }

        /// <summary>
        /// 群組/角色 刪除 POST
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Delete(string id)
        {
            var _param = new { id };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢查_
                if (!CheckUserAuth("Group"))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (string.IsNullOrEmpty(id))
                {
                    result.IsSuccess = false;
                    result.Message = "請選擇要刪除的群組";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                res = _rolesService.Delete(id, User.Identity.Name);
                string _res = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M003",     //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "角色群組", _res),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(id),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
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
                    Controller = "Group",
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Result = res, Exception = ex },
                    LogString = "刪除角色.Exception",
                    ErrorMessage = string.Format($"刪除角色失敗. {ex.Message}")
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 角色群組與功能項目
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Funcs(string id)
        {
            RoleFuncMenuViewModel funcItems = new RoleFuncMenuViewModel();

            try
            {
                if (!CheckUserAuth("Group")) return RedirectToAction("NoAuthModal", "Error", new { @id = "RoleFuncsModal" });
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                    string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "角色群組與功能項目-操作"),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(new { }),
                    User.Identity.Name);
                #endregion

                //Marked_20210830//_funcsService = new FunctionsService();
                funcItems = _funcsService.GetFunctionsForRole(id);
            }
            catch (Exception ex)
            {
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Group",
                    Method = "[Funcs]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { RoleID = id, Exception = ex },
                    LogString = "角色群組與功能項目.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            
            return PartialView("Funcs", funcItems);
        }

        /// <summary>
        /// 角色群組與功能項目 更新POST
        /// </summary>
        [HttpPost]
        public ActionResult FuncsSave(RoleFuncUpdateModel roleFunc)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, roleFunc);
            VerifyResult res = new VerifyResult();

            try
            {
                res = _rolesService.UpdateRoleFuncs(roleFunc);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M019",     //[@USER_ID(@USER_NAME)] 設定 [@TARGET] 的 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, "功能項目", "角色群組", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(roleFunc),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = roleFunc,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : (HttpStatusCode)400
                };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = (HttpStatusCode)500;
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Group",
                    Method = "[FuncsSave]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = roleFunc, Result = res, Exception = ex },
                    LogString = "角色群組與功能項目更新.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

    }
}