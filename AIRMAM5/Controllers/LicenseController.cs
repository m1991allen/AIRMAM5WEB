using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.License;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 版權資料維護功能
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class LicenseController : BaseController
    {
        readonly string CONTR_NAEM = "License";
        private readonly ILicenseService _licenseService;


        public LicenseController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _licenseService = new LicenseService();
        }

        // GET: License
        public ActionResult Index()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return PartialView("NoAuth");

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "版權資料維護"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion

            return View();
        }

        /// <summary>
        /// 查詢 PartialView
        /// </summary>
        /// <returns></returns>
        public ActionResult _Search()
        {

            return PartialView("_Search");
        }

        /// <summary>
        /// 搜尋結果
        /// </summary>
        /// <param name="name">版權名稱 (模糊比對) </param>
        /// <param name="edt">授權結束日期 (可空值) </param>
        /// <returns></returns>
        public ActionResult Search(string name, string edt)
        {
            var _param = new { LicenseName = name, EndDate = edt };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);

            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            try
            {
                name = string.IsNullOrEmpty(name) ? "" : name;
                edt = string.IsNullOrEmpty(edt) ? string.Empty : edt;

                var get = _licenseService.SearchBy(name, null, edt)  //TIP_20211008_決議:不用特別限制版權條件
                    .Select(s => new LicenseListModel().ConvertData(s))
                    .ToList();

                result.IsSuccess = true;
                result.Message = "OK";
                result.StatusCode = HttpStatusCode.OK;
                result.Data = get;
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M005",  //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "版權資料", "OK"),
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
                    Controller = "License",
                    Method = "[Search]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                ModelState.AddModelError("", ex.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 版權內容 View
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult _Details(string code)
        {
            if (!CheckUserAuth(CONTR_NAEM))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });

            var get = _licenseService.GetBy(code)
                .Select(s => new LicenseDetailModel().ConvertData(s))
                .FirstOrDefault();

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M004",  //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統公告", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { LicenseCode = code }),
                User.Identity.Name);
            #endregion
            return View("_Details", get);
        }

        /// <summary>
        /// 編輯版權資料 View
        /// </summary>
        /// <param name="code">版權代碼 fsCODE </param> 
        /// <returns></returns>
        public ActionResult _Edit(string id)
        {
            if (!CheckUserAuth(CONTR_NAEM))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "EditModal" });

            var get = _licenseService.GetBy(id)
                .Select(s => new LicenseCreateModel().ConvertData(s))
                .FirstOrDefault();

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "版權資料-編輯"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(get),
                User.Identity.Name);
            #endregion

            return View("_Edit", get);
        }
        /// <summary>
        /// 版權資料編輯存檔 POST
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(LicenseCreateModel model)
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
            #endregion

            try
            {
                var get = _licenseService.GetBy(model.LicenseCode);
                if (!get.Any() || get.FirstOrDefault() == null || get == null)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"查無版權資料({model.LicenseCode})");
                    result.StatusCode = HttpStatusCode.NotFound;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                DateTime? licenseEndDate = string.IsNullOrEmpty(model.EndDate) ? (DateTime?)null : DateTime.Parse(model.EndDate);
                
                #region update column values
                var upd = get.FirstOrDefault();
                string mkstr = (upd.fdENDDATE == licenseEndDate) ? "": "(授權日期異動)";

                upd.fsDESCRIPTION = model.LicenseDesc;
                upd.fdENDDATE = licenseEndDate;// string.IsNullOrEmpty(model.EndDate) ? (DateTime?)null : DateTime.Parse(model.EndDate);
                upd.fsMESSAGE = model.AlertMessage;
                upd.fcIS_ACTIVE = model.IsActive;
                upd.fcIS_ALERT = model.IsBookingAlert;
                upd.fcIS_FORBID = model.IsNotBooking;
                upd.fdUPDATED_DATE = DateTime.Now;
                upd.fsUPDATED_BY = User.Identity.Name;
                #endregion

                res = _licenseService.Update(upd);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "版權資料", _str+ mkstr),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion

                //return LicenseListModel
                result = new ResponseResultModel(res)
                {
                    Records = model,
                    Data = new LicenseListModel().ConvertData(upd),
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "License",
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "版權資料修改.Exception",
                    ErrorMessage = string.Format($"版權資料修改失敗. {ex.Message}")
                });
                #endregion
                ModelState.AddModelError("", ex.Message);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 新增 版權資料 View
        /// </summary>
        /// <returns></returns>
        public ActionResult _Create()
        {
            if (!CheckUserAuth(CONTR_NAEM))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "CreateModal" });

            var get = _licenseService.GetBy().OrderByDescending(b => b.fnORDER).FirstOrDefault();
            LicenseCreateModel md = new LicenseCreateModel
            {
                IsActive = true,
                IsBookingAlert = false,
                IsNotBooking = false,
                Order = get == null ? 0 : (get.fnORDER + 1)
            };

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "版權資料-新增"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion

            return PartialView("_Create", md);
        }

        /// <summary>
        /// 新增 版權資料 POST
        /// </summary>
        /// <returns></returns>
        public ActionResult Create(LicenseCreateModel model)
        {
            var _param = model;
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
                if (_licenseService.IsExists(model.LicenseCode))
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("版權代碼({0}) 重複，請重新輸入版權代碼。", model.LicenseCode);
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (_licenseService.IsExistsByName(model.LicenseName))
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("已有相同的版權名稱({0})，請重新輸入版權名稱。", model.LicenseName);
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                var _INS = new tbmLICENSE()
                {
                    fsCREATED_BY = User.Identity.Name,
                    fdCREATED_DATE = DateTime.Now
                }.DataConvert(model);

                res = _licenseService.Create(_INS);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "版權資料", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    Data = new LicenseListModel().ConvertData(_INS), //return
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
                    Controller = "Ann",
                    Method = "[Create]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Result = res, Exception = ex },
                    LogString = "新增版權.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                ModelState.AddModelError("", ex.Message);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

    }
}