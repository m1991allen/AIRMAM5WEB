using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.DBEntity;
using System.Net;
using AIRMAM5.HubServer;
using AIRMAM5.DBEntity.Models.Announce;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.Filters;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 系統公告
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class AnnController : BaseController
    {
        readonly string CONTR_NAEM = "Ann";
        readonly IAnnounceService _announceService;

        public AnnController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService, ICodeService codeService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _tbzCodeService = codeService;
            _announceService = new AnnounceService(serilogService);
        }

        #region ---公告資料異動後,透過SignalR 更新前端DashBoard"公告區塊內容"---
        /// <summary>
        /// 通知Server TO Client 公告資料有異動/更新
        /// </summary>
        public void NotifyClientOfUpdats(string msg)
        {
            //new BroadcastHub().RefreshAnnOfDashBoard(msg);
            new BroadcastHub2().RefreshAnnOfDashBoard(msg);
        }
        #endregion

        /// <summary>
        /// 登入頁左側公告資料
        /// </summary>
        /// <returns></returns>
        public JsonResult LoginAnn()
        {
            var get = _announceService.GetLoginAnn();

            return Json(get, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 系統公告維護頁面
        /// </summary>
        /// <param name="sdate">上架日期 ≧ sdate</param>
        /// <param name="edate">下架日期 ≦ edate</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult Matain()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return PartialView("NoAuth");

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統公告維護"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 系統公告維護頁面 指定查詢
        /// </summary>
        /// <param name="sdate"></param>
        /// <param name="edate"></param>
        /// <param name="type"></param>
        /// <param name="dept"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Matain(string sdate, string edate, string type, string dept)
        {
            var _param = new { sdate, edate, type, dept };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);

            #region _檢查_
            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            try
            {
                sdate = sdate ?? string.Empty;
                edate = edate ?? string.Empty;
                type = string.IsNullOrEmpty(type) || type == "*" ? string.Empty : type;
                dept = string.IsNullOrEmpty(dept) || dept == "*" ? string.Empty : dept;

                var get = _announceService.GetBy4Parament(0, sdate, edate, type)
                    .Where(x => string.IsNullOrEmpty(dept) ? true : x.fsDEPT == dept)
                    .Select(s => new AnnouncePublicViewModel().DataConvert(s))
                    .ToList();

                result.IsSuccess = true;
                result.Message = "OK";
                result.StatusCode = HttpStatusCode.OK;
                result.Data = get;
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M005",  //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統公告", "OK"),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
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
                    Method = "[Matain]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                ModelState.AddModelError("", ex.Message);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 公告查詢頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult _Search()
        {
            AnnounceSearchModel md = new AnnounceSearchModel
            {
                AnnTypeList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.ANN001.ToString()),
                AnnDeptList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.DEPT001.ToString())
        };

            return PartialView("_Search", md);
        }

        /// <summary>
        /// 公告詳細內容Modal頁面
        /// </summary>
        /// <param name="id">公告Id</param>
        /// <returns></returns>
        public ActionResult _Details(long id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal","Error", new { @id = "DetailModal" });

            var get = _announceService.GetBy4Parament(id)
                .Select(s => new AnnounceDetailViewModel().DataConvert(s))
                .FirstOrDefault();
            
            GroupsService _gser = new GroupsService(_serilogService);
            get.PostTo = string.Join(",",
                        _gser.GroupListItemSelected(get.PostTo)
                                .Where(x => x.Selected == true).Select(x => x.Text).ToList());

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M004",  //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統公告", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { AnnounceId = id }),
                User.Identity.Name);
            #endregion
            return View("_Details",get);
        }

        /// <summary>
        /// 新增公告Modal頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult _Create()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "CreateModal" });

            AnnounceCreateModel md = new AnnounceCreateModel
            {
                GroupListItems = new GroupsService().GetUserRoles(),
                AnnTypeList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.ANN001.ToString()),
                AnnDeptList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.DEPT001.ToString()),
                HiddenList = _tbzCodeService.GetBoolItemList(new string[2] { "隱藏", "顯示" })
            };

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統公告維護-新增"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion
            
            return PartialView("_Create", md);
        }

        /// <summary>
        /// 新增公告 POST
        /// </summary>
        /// <param name="tbmANNOUNCE"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(AnnounceCreateModel announce)
        {
            var _param = announce;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();
            #region _檢查_
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
                {
                    if (ModelState[item].Errors.Count > 0)
                    {
                        result.IsSuccess = false;
                        result.Message = ModelState[item].Errors[0].ErrorMessage;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                }
            }
            #endregion

            try
            {
                //var _create = new tbmANNOUNCE(announce)
                //{
                //    fsCREATED_BY = User.Identity.Name,
                //    fdCREATED_DATE = DateTime.Now
                //};
                tbmANNOUNCE add = new tbmANNOUNCE
                {
                    fsCREATED_BY = User.Identity.Name,
                    fdCREATED_DATE = DateTime.Now
                };
                add.ConvertGet(announce);

                res = _announceService.Create(add);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統公告", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(announce),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };

                if (res.IsSuccess) { NotifyClientOfUpdats("-系統公告新增-"); }
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
                    Method = "[Create]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Result = res, Exception = ex },
                    LogString = "新增系統公告.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                ModelState.AddModelError("", ex.Message);
            }
            
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 系統公告 編輯Modal頁面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Edit(long id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "EditModal" });

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統公告維護-編輯"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion

            AnnounceCreateModel get = _announceService.GetBy4Parament(id)
                .Select(s => new AnnounceCreateModel().DataConvert(s))
                .FirstOrDefault();

            get.GroupListItems = new GroupsService().GetUserRoles();
            get.AnnTypeList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.ANN001.ToString());
            get.AnnDeptList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.DEPT001.ToString());
            get.HiddenList = _tbzCodeService.GetBoolItemList(new string[2] { "隱藏", "顯示" });

            return View("_Edit", get);
        }

        /// <summary>
        /// 系統公告 編輯 POST
        /// </summary>
        /// <param name="clsANN"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(AnnounceCreateModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            #region _檢查
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
                        result = new ResponseResultModel(false, ModelState[item].Errors[0].ErrorMessage, model)
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                        };
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
            }
            if (model.fdEDATE < model.fdSDATE)
            {
                result.IsSuccess = false;
                result.Message = "日期迄 不可小於起始日期! ";
                result.StatusCode = HttpStatusCode.BadRequest;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            try
            {
                var _upd = _announceService.GetById(model.AnnounceId);
                if (_upd == null)
                {
                    result.IsSuccess = false;
                    result.Message = "查無公告資料";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #region _set values
                _upd.fsTITLE = model.fsTITLE;
                _upd.fdSDATE = model.fdSDATE;
                _upd.fdEDATE = model.fdEDATE;
                _upd.fsDEPT = model.fsDEPT;
                _upd.fsTYPE = model.fsTYPE;
                _upd.fnORDER = model.fnORDER;
                _upd.fsIS_HIDDEN = model.fsIS_HIDDEN;
                _upd.fsGROUP_LIST = model.GroupList == null ? string.Empty : string.Join(";", model.GroupList);
                _upd.fsCONTENT = model.fsCONTENT ?? string.Empty;
                _upd.fsNOTE = model.fsNOTE ?? string.Empty;
                _upd.fdUPDATED_DATE = DateTime.Now;
                _upd.fsUPDATED_BY = User.Identity.Name;
                #endregion

                res = _announceService.Update(_upd);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統公告", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = model,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                if (res.IsSuccess)
                {
                    NotifyClientOfUpdats("-系統公告更新-");
                    //Response Data: AnnouncePublicViewModel{}
                    res.Data = _announceService.GetBy4Parament(model.AnnounceId)
                        .Select(s => new AnnouncePublicViewModel().DataConvert(s))
                        .FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "系統公告修改.Exception",
                    ErrorMessage = string.Format($"系統公告修改失敗. {ex.Message}")
                });
                #endregion
                ModelState.AddModelError("", ex.Message);
            }
            
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 系統公告 刪除Modal頁面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Delete(long id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "DeleteModal" });
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統公告維護-刪除檢視"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion

            var get = _announceService.GetBy4Parament(id)
                    .Select(s => new AnnounceDetailViewModel().DataConvert(s))
                    .FirstOrDefault();

            GroupsService _gser = new GroupsService(_serilogService);
            get.PostTo = string.Join(",",
                        _gser.GroupListItemSelected(get.PostTo)
                                .Where(x => x.Selected == true).Select(x => x.Text).ToList());

            return PartialView("_Delete", get);
        }

        /// <summary>
        /// 公告資料 刪除 POST
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(long id)
        {
            var _param = new { id };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            #region _檢查__
            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            if (id <= 0)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"公告編號不正確 ({id})");// "公告編號不正確";
                result.StatusCode = HttpStatusCode.BadRequest;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            if (!_announceService.IsExists(id))
            {
                result.IsSuccess = false;
                result.Message = string.Format($"公告資料不存在({id})");
                result.StatusCode = HttpStatusCode.BadRequest;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            try
            {
                res = _announceService.Delete(id);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M003",     //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統公告", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(id),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };

                if (res.IsSuccess)
                {
                    NotifyClientOfUpdats("-系統公告更新-");
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "刪除系統公告.Exception",
                    ErrorMessage = string.Format($"系統公告刪除失敗. {ex.Message}")
                });
                #endregion
                ModelState.AddModelError("", ex.Message);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

    }
}
