using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Booking;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
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
    /// 檔案調用 >> 我的調用狀態
    /// </summary>
    /// <remarks>
    /// 主頁列表-資料來源: spGET_L_WORK_BY_BOOKING. 讀取 tblWORK,tbmBOOKING 資料。
    /// 查看內容-資料來源: spGET_L_WORK_BY_TRANSCODE. 讀取 tblWORK 資料。
    /// </remarks>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class MyBookingController : BaseController
    {
        /// <summary>
        /// 工作表資料
        /// </summary>
        private readonly TblWorkService _tblWorkService;
        public MyBookingController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService, ICodeService codeService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _tbzCodeService = codeService;
            _tblWorkService = new TblWorkService();
        }

        /// <summary>
        /// 首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckUserAuth("MyBooking")) return View("NoAuth");
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "我的調用狀態"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
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
            //List<MyBookingModel> get = new List<MyBookingModel>();

            try
            {
                if (!CheckUserAuth("MyBooking"))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden; //403-Forbidden：禁止使用。沒有權限。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                m.WorkStatus = (m.WorkStatus == "*") ? string.Empty : m.WorkStatus;
                m.LoginId = User.Identity.Name;
                //TIPS_20200518_只顯示登入帳號自己的資料, Administrator也是看自己的資料

                DateTime dt = DateTime.Now.AddDays(-3);
                m.StartDate = m.StartDate ?? string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
                dt = DateTime.Now.AddDays(+1);
                m.EndDate = m.EndDate ?? string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");

                var get = _tblWorkService.GetLWorkByBooking(m);
                result.IsSuccess = true;
                result.Data = get.Select(s => new MyBookingModel().DataConvert(s)).ToList();
                result.StatusCode = HttpStatusCode.OK;

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "我的調用狀態", "OK"),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(new { UserName = User.Identity.Name }),
                    User.Identity.Name);
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.ErrorException = ex;
                #region _Serilog.ERR
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "MyBooking",
                    Method = "[Search]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "我的調用狀態.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        ///  詳細內容 Modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Details(long id)
        {
            if (!CheckUserAuth("MyBooking")) return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });

            var m = new GetLWorkByBookingParam(id, User.Identity.Name);
            BookingDetModel get = _tblWorkService.GetLWorkByBooking(m)
                    .Select(s => new BookingDetModel().DataConvert(s)).FirstOrDefault();

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "我的調用狀態", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { tblLog_ID = id }),
                User.Identity.Name);
            #endregion
            return PartialView("_Details", get);
        }

    }
}