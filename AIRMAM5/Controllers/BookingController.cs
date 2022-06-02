using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Booking;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Material;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Works;
using AIRMAM5.DBEntity.Procedure;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 管理調用狀態
    /// </summary>
    /// <remarks>
    /// 主頁列表-資料來源: spGET_L_WORK_BY_BOOKING. 讀取 tblWORK,tbmBOOKING 資料。
    /// 查看內容-資料來源: spGET_L_WORK_BY_BOOKING.
    /// 編輯優先權-資料來源: spGET_L_WORK_BY_BOOKING. 
    /// 編輯優先權-存檔   : spUPDATE_L_WORK_PRIORITY. 更新 tblWORK
    /// 執行重設借調      : spUPDATE_REBOOKING(多筆). 更新 (1)tblWORK (2)tbmBOOKING
    /// ✚加入借調       : (多筆)資料新增 tbmMATERIAL
    /// </remarks>
    //[InterceptorOfException]
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class BookingController : BaseController
    {
        readonly string CONTR_NAEM = "Booking";
        /// <summary>
        /// 工作表資料
        /// </summary>
        private readonly TblWorkService _tblWorkService;
        private readonly ProcedureGetService _getService;
        private readonly MaterialService _materialService;

        public BookingController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService, ICodeService codeService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _tbzCodeService = codeService;
            _materialService = new MaterialService(serilogService);
            _tblWorkService = new TblWorkService();
            _getService = new ProcedureGetService();
        }
        ///* Marked_20210830 */
        //public BookingController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}

        /// <summary>
        /// 首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return View("NoAuth");
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "管理調用狀態"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 查詢頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult _Search()
        {
            //Tips: 預設查詢３日內的資料
            BookingDateSearchModel model = new BookingDateSearchModel(3);

            var tclist = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.WORK_BK.ToString(), true, false, true);
            //tclist.Insert(0, new SelectListItem { Value = "*", Text = " -全部- ", Selected = true });
            model.WorkStatusList = tclist;

            return PartialView("_Search", model);
        }

        /// <summary>
        /// 查詢功能
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(GetLWorkByBookingParam m)
        {
            var _param = m;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            //List<BookingViewModel> get = new List<BookingViewModel>();

            try
            {
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #region _參數_
                m.WorkStatus = (m.WorkStatus == "*") ? string.Empty : m.WorkStatus;
                m.LoginId = User.Identity.Name;
                //Tips: 預設查詢３日內的資料
                DateTime dt = DateTime.Now.AddDays(-3); 
                m.StartDate = m.StartDate ?? string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
                dt = DateTime.Now.AddDays(+1);
                m.EndDate = m.EndDate ?? string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
                #endregion

                //2020/02/04: 不限定"系統管理員"帳號, 都可看到全部的資料。
                m.LoginId = string.Empty;   //有開放使用，就是顯示全部使用者帳號資料
                //var _usersService = new UsersService();
                //if (_usersService.CurrentUserIsAdmin) { m.LoginId = string.Empty; }

                var get = _tblWorkService.GetLWorkByBooking(m).Select(s => new BookingViewModel().DataConvert(s)).ToList();

                result.IsSuccess = true;
                result.Message = string.Empty;
                //result.Data = get.Select(s => new BookingViewModel().DataConvert(s)).ToList();
                result.Data = get;
                result.StatusCode = HttpStatusCode.OK;

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "調用狀態資料", "OK"),
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
                result.ErrorException = ex;
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Search]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "管理調用狀態.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 詳細內容 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Details(long id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });

            var m = new GetLWorkByBookingParam(id, string.Empty);
            BookingDetModel get = _tblWorkService.GetLWorkByBooking(m)
                            .Select(s => new BookingDetModel().DataConvert(s)).FirstOrDefault();

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(), 
                "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "調用狀態資料", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(m),
                User.Identity.Name);
            #endregion
            return PartialView("_Details", get);
        }

        /// <summary>
        /// 編輯: 設定優先權
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
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "管理調用狀態-編輯"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion

            var m = new GetLWorkByBookingParam(id, string.Empty);
            BookingViewModel get = _tblWorkService.GetLWorkByBooking(m)
                .Select(s => new BookingViewModel().DataConvert(s)).FirstOrDefault();

            return PartialView("_Edit", get);
        }
        
        /// <summary>
        /// 編輯: 設定優先權 POST
        /// </summary>
        /// <param name="workid">工作編號 fnWORK_ID </param>
        /// <param name="priority">優先權 fsPRIORITY </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(long workid, int priority)
        {
            var _param = new { workid, priority };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                 res = _tblWorkService.UpdateLWorkPriority(workid, priority);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M012",     //[@USER_ID(@USER_NAME)] 修改 [@TARGET] 的 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, "調用優先權", "管理調用狀態", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Message = string.Format($"ID: {workid} 更新調用優先權 {_str}. "),
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { param = _param, Result = result },
                    LogString = "編輯調用優先權.Result"
                });
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
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, Exception = ex },
                    LogString = "編輯調用優先權.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);

        }
        /// <summary>
        /// 選擇資料列 執行重設借調
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReBooking(string[] id)
        {
            var _param = JsonConvert.SerializeObject(id);
            ResponseResultModel result = new ResponseResultModel(false, "檔案編號錯誤.", _param);
            UpdateLWorkReTranResult res = new UpdateLWorkReTranResult();

            try
            {
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                if (id != null && id.Length > 0)
                {
                    List<string> workIds = id.ToList();
                    res = _tblWorkService.UpdateLWorkReBooking(workIds, CurrentUser.UserName);
                    //Tips: 多筆編輯,只要沒有Exception,皆為True；多筆的成功/失敗都會記錄在res
                    string _str = res.IsSuccess ? "成功" : "失敗";
                    #region _DB LOG
                    _tblLogService.Insert_L_Log(
                        TbzCodeIdEnum.MSG001.ToString(),
                        "M021",     //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                        string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "管理檔案狀態-重設借調", _str),
                        string.Format($"位置: {Request.UserHostAddress} "),
                        JsonConvert.SerializeObject(id),
                        User.Identity.Name);
                    #endregion

                    result.IsSuccess = res.IsSuccess;
                    result.Message = _str;
                    result.Data = string.Format($"重設借調 {_str}");
                    result.StatusCode = HttpStatusCode.OK;

                    result.Message = res.IsSuccess ? "" : res.Message;
                    result.Message = res.Processed.Count() > 0
                        ? string.Format($"{result.Message}\n 檔案【{string.Join(",", res.Processed)}】重設借調成功，下次更新資料時就會進行排程。!") : result.Message;
                    result.Message = res.UnProcessed.Count() > 0
                        ? string.Format($"{result.Message}\n 檔案【{string.Join(",", res.UnProcessed)}】轉檔中，不可重設借調！") : result.Message;
                    result.Message = res.Failure.Count() > 0
                        ? string.Format($"{result.Message}\n 檔案【{string.Join(",", res.Failure)}】重設借調失敗！") : result.Message;
                    
                    #region Serilog.Info
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = CONTR_NAEM,
                        Method = "[ReBooking]",
                        EventLevel = SerilogLevelEnum.Information,
                        Input = new { Param = _param, Result = result },
                        LogString = "重設借調.Result"
                    });
                    #endregion
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
                    Method = "[ReBooking]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, Exception = ex },
                    LogString = "重設借調.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 選擇資料列 取消調用
        /// </summary>
        /// <param name="id"></param>
        /// <remarks> fsSTATUS IN ('_C','00') 才可取消。 </remarks>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BookingCancel(string[] id)
        {
            var _param = JsonConvert.SerializeObject(id);
            ResponseResultModel result = new ResponseResultModel(false, "檔案編號錯誤.", _param);
            UpdateLWorkReTranResult res = new UpdateLWorkReTranResult();

            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            try
            {
                if (id != null && id.Length > 0)
                {
                    List<string> workIds = id.ToList();
                    res = _tblWorkService.CancelBooking(workIds, CurrentUser.UserName);

                    //Tips: 多筆編輯,只要沒有Exception,皆為True；多筆的成功/失敗都會記錄在res
                    string _str = res.IsSuccess ? "成功" : "失敗";
                    #region _DB LOG
                    _tblLogService.Insert_L_Log(
                        TbzCodeIdEnum.MSG001.ToString(),
                        "M021",     //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                        string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "管理檔案狀態-取消調用", _str),
                        string.Format($"位置: {Request.UserHostAddress} "),
                        JsonConvert.SerializeObject(id),
                        User.Identity.Name);
                    #endregion

                    result.IsSuccess = res.IsSuccess;
                    result.Message = _str;
                    result.Data = string.Format($"取消調用 {_str}");
                    result.StatusCode = HttpStatusCode.OK;

                    result.Message = res.IsSuccess ? string.Empty : res.Message;
                    result.Message = res.Processed.Count() > 0
                        ? string.Format($"{result.Message}\n 檔案【{string.Join(",", res.Processed)}】取消借調成功!") : result.Message;
                    result.Message = res.UnProcessed.Count() > 0
                        ? string.Format($"{result.Message}\n 檔案【{string.Join(",", res.UnProcessed)}】轉檔中或已完成，不可取消借調！") : result.Message;
                    result.Message = res.Failure.Count() > 0
                        ? string.Format($"{result.Message}\n 檔案【{string.Join(",", res.Failure)}】取消借調失敗！") : result.Message;
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
                    Method = "[BookingCancel]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, Exception = ex },
                    LogString = "取消調用.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #region ==========【加入借調 tbmMATERIAL】==========
        /// <summary>
        /// 加入借調 (檢索結果頁/ 主題與檔案內頁) ***(參考IE版本,只有影片可以操作 借調)***
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddBooking(List<MaterialCreateModel> models)
        {
            var _param = new { models, urnm = User.Identity.Name };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    result.IsSuccess = false;
                    result.Message = "使用者未驗證";
                    result.StatusCode = HttpStatusCode.Unauthorized;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                //Tips: 「檢索結果頁✚借調」: 後端檢查帳號是否有借調=B "檔案編號"權限
                string _has = string.Empty, _nohas = string.Empty, _prohibit = string.Empty;
                List<tbmMATERIAL> _materials = new List<tbmMATERIAL>();

                foreach (var r in models)
                {
                    var _fileAuth = _getService.GetDirFileNoAuthorityByUser(CurrentUser.UserName, r.FileNo, r.FileCategory);
                    if (string.IsNullOrEmpty(_fileAuth)) //檔案編號沒有權限設定
                    {
                        _nohas = string.IsNullOrEmpty(_nohas) ? string.Format($"無借調權限: {r.FileNo}") : string.Format($"{_nohas},{r.FileNo}");
                        continue;
                    }
                    if (_fileAuth.IndexOf("B") < 0)
                    {
                        _nohas = string.IsNullOrEmpty(_nohas) ? string.Format($"無借調權限: {r.FileNo}") : string.Format($"{_nohas},{r.FileNo}");
                        continue;
                    }
                    #region >>> 版權設定 是否調用禁止 20210913_ADDED
                    var urFileAuth = _getService.GetDirFileNoAuthorityByUser(CurrentUser.UserName, r.FileNo);
                    var fLicense = (urFileAuth.Any() && urFileAuth.FirstOrDefault() != null) ? urFileAuth.FirstOrDefault() : null;
                    //fLicense.IS_ALERT //是否調用提示
                    //fLicense.IS_FORBID //是否調用禁止
                    if (fLicense.IS_FORBID == true)
                    {
                        string msg = string.IsNullOrEmpty(fLicense.MESSAGE) ? string.Empty : string.Format($"({fLicense.MESSAGE})");
                        _prohibit = string.IsNullOrEmpty(_prohibit)
                                ? string.Format($"禁止調用: {r.FileNo}{msg}")
                                : string.Format($"{_prohibit},{r.FileNo}{msg}");
                        continue;
                    }
                    //可調用,是否有版權提示訊息
                    string altMSG = fLicense.IS_ALERT == true ? string.Format($"({fLicense.MESSAGE})") : string.Empty;
                    #endregion

                    //可借調
                    _has = string.IsNullOrEmpty(_has) ? string.Format($"可借調: {r.FileNo}{altMSG}") : string.Format($"{_has},{r.FileNo}{altMSG}");
                    _materials.Add(new tbmMATERIAL(r)
                    {
                        fsMARKED_BY = User.Identity.Name,
                        fsCREATED_BY = User.Identity.Name
                    });
                }
                
                if (_materials.Count > 0) { res = _materialService.CreateRange(_materials, User.Identity.Name); }

                //Tips: 回覆"我的調用清單"列表資料格式 MaterialListModel
                string _str = res.IsSuccess ? "成功" : "失敗";
                /* 【TIP】 目前並不會有多筆傳入。
                 * 【TODO】
                 *      當有多筆傳入(eg. 10筆)，調用成功、失敗的檔案編號會顯示在Message中「可借調:..(8筆) ; 無借調權限:.(2筆); 禁止調用:.(0筆)」
                 *      Response IsSuccess = true, 要確認這種狀況，前端是否都把10筆檔案標示為"已加入清單"!!!
                 *      前端是不會顯示"訊息"。
                 * 
                 * */

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), //"M001",
                    "M021",     //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "管理檔案狀態-加入調閱", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    //Tips: Message = string.Format($"{_has} ; {_nohas}"), //顯示 "可借調: .... ; 無借調權限: .....; 禁止調用: ....."
                    Message = string.IsNullOrEmpty(_has)
                                //? _nohas
                                ? (string.IsNullOrEmpty(_nohas) ? _prohibit : _nohas + (string.IsNullOrEmpty(_prohibit) ? _prohibit : "; " + _prohibit))
                                : _has + (string.IsNullOrEmpty(_nohas) ? _nohas : "; " + _nohas),
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[AddBooking]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result, Str = new { _has, _nohas } },
                    LogString = "加入借調.Result"
                });
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
                    Method = "[AddBooking]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, Exception = ex },
                    LogString = "加入借調.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion
    }
}