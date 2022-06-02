using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Works;
using AIRMAM5.DBEntity.Models.Enums;
using System.Net;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.Filters;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 查詢作業 > 手動上傳轉檔紀錄(入庫轉檔)
    /// </summary>
    //[InterceptorOfException]
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class L_UploadController : BaseController
    {
        readonly string CONTR_NAEM = "L_Upload";
        readonly TblWorkService _tblWorkService;

        public L_UploadController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;// new SerilogService();
            _tbzCodeService = new CodeService();
            _tblLogService = tblLogService;// new TblLogService();
            _tblWorkService = new TblWorkService();
        }

        /// <summary>
        /// 手動上傳轉檔首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return View("NoAuth");

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "入庫轉檔查詢"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(string.Empty),
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
            WorkStatusDateSerarchModel m = new WorkStatusDateSerarchModel(3);

            return PartialView("_Search", m);
        }

        /// <summary>
        /// 查詢功能
        /// <para> 有開放使用，就是顯示全部使用者帳號資料. </para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(WorkStatusDateSerarchModel m)
        {
            var _param = m;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);

            try
            {
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                var b = new GetLWorkByTranscodeParam()
                {
                    StartDate = string.Format($"{m.BeginDate:yyyy/MM/dd}"),
                    EndDate = string.Format($"{m.EndDate:yyyy/MM/dd}"),
                    WorkStatus = string.IsNullOrEmpty(m.WorkStatus) || m.WorkStatus == "*" ? string.Empty : m.WorkStatus
                };
                var get = _tblWorkService.GetLWorkByTranscode(b);//.Select(s => new UploadWorkEditModel(s)).ToList();

                #region >>> Tips: 可操作功能者,不限制帳號清單資料。
                //Tips_2020/01/31: 非"系統管理員"帳號,開啟此頁面,只顯示登入帳號的資料。
                //var _usersService = new UsersService();
                //if (_usersService.CurrentUserIsAdmin)
                //{
                //    result.Data = get.Select(s => new UploadWorkEditModel(s)).OrderByDescending(r => r.fnWORK_ID).ToList();
                //}
                //else
                //{
                //    result.Data = get.Where(x => x.fsCREATED_BY == CurrentUser.UserName)
                //        .Select(s => new UploadWorkEditModel(s)).OrderByDescending(r => r.fnWORK_ID).ToList();
                //}

                result.Data = get.Select(s => new UploadWorkEditModel().FormatConversion(s))
                    .OrderByDescending(r => r.fnWORK_ID).ToList();
                #endregion

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "入庫轉檔紀錄", "OK"),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(m),
                    User.Identity.Name);
                #endregion

                result.IsSuccess = true;
                result.StatusCode = HttpStatusCode.OK;
                #region Serilog.INFO
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[Search]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { DataCount = get.Count(), Param = _param },
                    LogString = "入庫轉檔查詢.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[Search]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "入庫轉檔紀錄.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            //return Json(result, JsonRequestBehavior.DenyGet);
            //Tips: JsonConvert效能較佳，且沒有字串長度上限
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented,
               new JsonSerializerSettings
               {   //視自己需求可以拿掉
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore //指定如何處理迴圈引用: Ignore--不序列化,Error-丟擲異常,Serialize--仍要序列化
                   , DateFormatString = "yyyy/MM/dd HH:mm:ss"
               }), "application/json");
        }
        
        /// <summary>
        ///  手動上傳轉檔詳細內容Modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult _Details(long id)
        {
            if (!CheckUserAuth(CONTR_NAEM))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });

            var b = new GetLWorkByTranscodeParam(id);
            var get = _tblWorkService.GetLWorkByTranscode(b)
                    .Select(s => new UploadWorkViewModel().FormatConversion(s))
                    .FirstOrDefault();

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "入庫轉檔紀錄", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { tblLog_ID = id }),
                User.Identity.Name);
            #endregion
            return PartialView("_Details", get);
        }

        /// <summary>
        /// 編輯: 設定優先權、備註內容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult _Edit(long id)
        {
            if (!CheckUserAuth(CONTR_NAEM))
            { return RedirectToAction("NoAuthModal", "Error", new { @id = "EditModal" }); }
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "入庫轉檔查詢-編輯"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { ID = id }),
                User.Identity.Name);
            #endregion

            var b = new GetLWorkByTranscodeParam(id);
            var get = _tblWorkService.GetLWorkByTranscode(b)
                    .Select(s => new UploadWorkEditModel().FormatConversion(s))
                    .FirstOrDefault();

            return PartialView("_Edit", get);
        }

        /// <summary>
        /// 編輯: 設定優先權、備註內容 POST 
        /// </summary>
        /// <param name="workid">工作編號 fnWORK_ID </param>
        /// <param name="priority">優先權 fsPRIORITY </param>
        /// <param name="note">備註 fsNOTE </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(long workid, int priority, string note)
        {
            var _param = new { workid, priority, note };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢查_
                if (!CheckUserAuth("L_Upload"))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                var _upd = _tblWorkService.GetById(workid);
                if (_upd == null)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"查無工作編號({workid})資料");
                    result.StatusCode = HttpStatusCode.NotFound;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                _upd.fsPRIORITY = priority.ToString();
                _upd.fsNOTE = note ?? string.Empty;
                res = _tblWorkService.UpdateBy(_upd);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "入庫轉檔查詢-轉檔優先權", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    //Message = string.Format($"ID: {workid} 更新轉檔優先權 {_str}. "),
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.INFO
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { param = new { workid, priority }, Result = result },
                    LogString = "編輯轉檔優先權.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "Edit",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Result = res, Exception = ex },
                    LogString = "編輯轉檔優先權.Exception",
                    ErrorMessage = string.Format($"轉檔優先權更新失敗. {ex.Message}")
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 選擇資料列 執行重新轉檔
        /// </summary>
        /// <param name="id">轉檔工作編號 fnWORK_ID</param>
        /// <returns></returns>
        //[InterceptorOfController(Method = "ReTran")]
        [HttpPost]
        public ActionResult ReTran(string[] id)
        {
            var _param = JsonConvert.SerializeObject(id);
            ResponseResultModel result = new ResponseResultModel(false, "檔案編號錯誤.", _param) { Data = new { } };
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
                    res = _tblWorkService.UpdateLWorkReTran(workIds, CurrentUser.UserName);
                    //Tips: 多批轉檔,只要沒有Exception,皆為True；多筆的成功/失敗都會記錄在res
                    string _str = res.IsSuccess ? "成功" : "失敗";
                    #region _DB LOG
                    _tblLogService.Insert_L_Log(
                        TbzCodeIdEnum.MSG001.ToString(),
                        "M021",     //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                        string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "重新轉檔", _str),
                        string.Format($"位置: {Request.UserHostAddress} "),
                        JsonConvert.SerializeObject(id),
                        User.Identity.Name);
                    #endregion

                    result.IsSuccess = res.IsSuccess;
                    result.Message = _str;
                    result.Data = res;
                    result.StatusCode = HttpStatusCode.OK;

                    result.Message = res.IsSuccess ? string.Empty : res.Message;
                    result.Message = res.Processed.Count() > 0
                        ? string.Format($"{result.Message}\n 檔案【{string.Join(",", res.Processed)}】新增轉檔資料成功，下次更新資料時就會進行排程!") : result.Message;
                    result.Message = res.UnProcessed.Count() > 0
                        ? string.Format($"{result.Message}\n 檔案【{string.Join(",", res.UnProcessed)}】正在轉檔中！") : result.Message;
                    result.Message = res.Failure.Count() > 0
                        ? string.Format($"{result.Message}\n 檔案【{string.Join(",", res.Failure)}】新增轉檔資料失敗！") : result.Message;

                    #region Serilog.Info
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "L_Upload",
                        Method = "[ReTran]",
                        EventLevel = SerilogLevelEnum.Information,
                        Input = new { Param = _param, Result = result },
                        LogString = "執行重新轉檔.Result"
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
                    Controller = "L_Upload",
                    Method = "[ReTran]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Result = res, Exception = ex },
                    LogString = "重新轉檔.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #region 【取 轉檔編號 目前進度%】
        /// <summary>
        /// 取 轉檔編號 目前進度%
        /// </summary>
        /// <param name="id">轉檔工作編號 fnWORK_ID</param>
        /// <returns></returns>
        [HttpPost]
        //[InterceptorOfController(Method = "GetCurrentProgress")]
        public JsonResult GetCurrentProgress(long[] ids)
        {
            var _param = JsonConvert.SerializeObject(ids);
            ResponseResultModel result = new ResponseResultModel(false, "檔案編號錯誤.", _param);
            #region _檢查_
            if (!User.Identity.IsAuthenticated)
            {
                result.IsSuccess = false;
                result.Message = "使用者未驗證";
                result.StatusCode = HttpStatusCode.Unauthorized;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            try
            {
                if (ids != null && ids.Length > 0)
                {
                    var get = _tblWorkService.GetMultipleById(ids.ToList());
                        //.Select(s => new LWorkProgressModel(s)
                        //{
                        //    //fnWORK_ID = s.fnWORK_ID,
                        //    //Progress = s.fsPROGRESS,
                        //    //fsSTATUS = s.fsSTATUS,    //Tips:此欄位有多個工作類別的狀態定義,要看[fsTYPE]決定是哪一個定義代碼
                        //    WorkStatusName = s.fsTYPE.ToUpper() == "TRANSCODE" 
                        //        ? _tbzCodeService.GetCodeName(TbzCodeIdEnum.WORK_TC, s.fsSTATUS)
                        //        : _tbzCodeService.GetCodeName(TbzCodeIdEnum.WORK_BK, s.fsSTATUS)
                        //}).ToList();

                    /* --- 工作類別 ---
                     * WORK001	AVID	    調用AVID作業
                     * WORK001	BOOKING	    調用轉檔作業
                     * WORK001	COPYFILE	調用複製作業
                     * WORK001	NAS	        調用NAS作業
                     * WORK001	TRANSCODE	上傳入庫轉檔作業
                     * */
                    result.IsSuccess = true;
                    result.Message = "OK";
                    result.StatusCode = HttpStatusCode.OK;
                    result.Data = get;
                }
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "轉檔編號 目前進度%", "OK"),
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
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[GetCurrentProgress]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Exception = ex },
                    LogString = "轉檔進度.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

    }
}
