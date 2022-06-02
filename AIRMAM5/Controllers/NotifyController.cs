using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AIRMAM5.Filters;
using System.Net;
using Newtonsoft.Json;
using AIRMAM5.DBEntity.Models.Notify;
using AIRMAM5.HubServer;
using AIRMAM5.DBEntity.Interface;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 訊息通知管理 controller
    /// </summary>
    //[InterceptorOfException]
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class NotifyController : BaseController
    {
        readonly NotifyService _notifyService;
        private readonly IHubContext _hubContext;

        public NotifyController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _notifyService = new NotifyService();
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<BroadcastHub2>();
        }

        // GET: Notify
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// FOR TEST
        /// </summary>
        /// <returns></returns>
        public ActionResult NotifyNew()
        {
            return View();
        }


        /// <summary>
        /// 新增 訊息通知
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateNotify()
        {
            NotifyCreateModel m = new NotifyCreateModel
            {
                CategoryList = GetEnums.GetNotifyCategory()
            };
            return View(m);
        }

        /// <summary>
        /// 新增 訊息通知 -> 送給SingalR Client
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateNotify(NotifyCreateModel model)
        {
            try
            {
                var r = _notifyService.NotifyCreate(model, CurrentUser.UserName);
                if (r.IsSuccess)
                {
                    //List<NotifyDataModel> result = new List<NotifyDataModel>();
                    var result = _notifyService.GetByUser(model.NoticeTo);
                    int unread = 0;
                    if (result.Count() > 0)
                    {
                        var get = result.Where(x => x.IsRead == false);
                        if (get.Any()) unread = get.Count();
                    }

                    UserNotifyModel json = new UserNotifyModel
                    {
                        UserId = model.NoticeTo,
                        UnRead = unread,
                        DataList = result
                    };

                    ////呼叫所有客戶端 showMyNotify(), 
                    ////var hubContext = GlobalHost.ConnectionManager.GetHubContext<BroadcastHub>();
                    //_hubContext.Clients.All.showMyNotify(json);
                    switch (model.Category)
                    {
                        case (int)NotifyCategoryEnum.指定帳號:
                            //_hubContext.Clients.Client(clientid).showMyNotify(json);
                            //_hubContext.Clients.Clients()....
                            break;
                        case (int)NotifyCategoryEnum.角色群組:
                            //_hubContext.Clients.Group(groupname).showMyNotify(json);
                            //_hubContext.Clients.Groups()....
                            break;
                        case (int)NotifyCategoryEnum.預設:
                        default:
                            _hubContext.Clients.All.showMyNotify(json);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Notify",
                    Method = "[CreateNotify]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "新增訊息通知.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                ModelState.AddModelError("", ex.Message);
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 訊息通知 類別 List
        /// </summary>
        /// <returns></returns>
        public JsonResult Get_NotifyCategorys()
        {
            List<SelectListItem> CategoryList = GetEnums.GetNotifyCategory();
            return Json(CategoryList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 讀取訊息 (只會處理"未讀")
        /// </summary>
        /// <param name="ids">通知id </param>
        /// <param name="readall">是否全部已讀</param>
        /// <returns></returns>
        [HttpPost]
        //[InterceptorOfController(Method = "ReadNotify")]
        public JsonResult ReadNotify(long[] ids, bool readall)
        {
            var _param = new { NotifyIds = ids };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param) { StatusCode = HttpStatusCode.OK };

            try
            {
                #region _檢查_
                if (!User.Identity.IsAuthenticated)
                {
                    result.IsSuccess = false;
                    result.Message = "使用者未驗證";
                    result.StatusCode = HttpStatusCode.Unauthorized;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                //var get = _notifyService.GetBy(id).FirstOrDefault();
                //if (get == null)
                //{
                //    result.IsSuccess = false;
                //    result.Message = "訊息資料異常(NULL)";
                //    result.StatusCode = HttpStatusCode.NotFound;
                //    return Json(result, JsonRequestBehavior.DenyGet);
                //}
                //if (_notifyService.IsExistsByUser(id, CurrentUser.Id))
                //{
                //    result.IsSuccess = false;
                //    result.Message = "非使用者訊息資料";
                //    result.StatusCode = HttpStatusCode.NotFound;
                //    return Json(result, JsonRequestBehavior.DenyGet);
                //}
                #endregion

                //使用者未讀狀態的訊息資料  //全部已讀註記 優先處理
                var _get = _notifyService.GetUserNoitfy(CurrentUser.Id).Where(x => x.fbIS_READ == false).ToList();
                VerifyResult res = new VerifyResult();

                if ((ids != null && ids.Length > 0 && readall == false) || readall == true)
                {
                    if (!readall)
                    { //非 全部已讀, 依id 取回資料
                        if (ids != null && ids.Length > 0)
                        {
                            _get = _get.Where(x => ids.ToList().IndexOf(x.fnNOTIFY_ID) >= 0).ToList();
                        }
                    }
                    _get.ForEach(x =>
                    {
                        x.fbIS_READ = true;
                        x.fdREAD_TIME = DateTime.Now;
                        x.fdUPDATED_DATE = DateTime.Now;
                        x.fsUPDATED_BY = User.Identity.Name;
                    });
                    res = _notifyService.UpdateMultiple(_get);
                    string _str = res.IsSuccess ? "成功" : "失敗";

                    #region _DB LOG
                    _tblLogService.Insert_L_Log(
                        TbzCodeIdEnum.MSG001.ToString(),
                        "M021",     //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                        string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "讀取提醒訊息", _str),
                        string.Format($"位置: {Request.UserHostAddress} "),
                        JsonConvert.SerializeObject(_param),
                        User.Identity.Name);
                    #endregion
                    result = new ResponseResultModel(res)
                    {
                        Records = _param,
                        StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                    };
                    //TODO_2020/01/22: result.Data已有更新資料，是否要再推SingalR Clients 更新。

                    #region _Serilog.Debug//
                    //_serilogService.SerilogWriter(new SerilogInputModel
                    //{
                    //    Controller = "Notify",
                    //    Method = "[ReadNotify]",
                    //    EventLevel = SerilogLevelEnum.Debug,
                    //    Input = new { Param = _param, Result = res },
                    //    LogString = "讀取提醒訊息.Result"
                    //});
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Notify",
                    Method = "[ReadNotify]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "讀取提醒訊息.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}