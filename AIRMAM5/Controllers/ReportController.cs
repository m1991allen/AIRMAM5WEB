using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.Filters;
using Newtonsoft.Json;
using System;
//using System.Configuration;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 系統報表
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class ReportController : BaseController
    {
        /// <summary>
        /// SQL Report Server URL
        /// </summary>
        private readonly static string _rptServer = Config.ReportServer;//ConfigurationManager.AppSettings["ReportServer"].ToString();

        public ReportController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
        }

        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 查詢頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult _Search(string rpt)
        {
            //ReportSearchModel m = new ReportSearchModel(-200);
            ReportParameterModel m = new ReportParameterModel(-200, rpt);
            return PartialView("_Search", m);
        }

        /// <summary>
        /// 回傳檔案下載URL
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Search(ReportParameterModel m)//(ReportSearchModel m)
        {
            /*
             回傳URL
                http://172.20.142.36/ReportServer_MSSQL2016?/AIRMAM5.RPT/rptGET_BOOKING_LIST_DETAIL&fdSDATE={sDATE}&fdEDATE={eDATE}&rs:Format=EXCELOPENXML&rc:parameters=false&rs:Command=Render
            明細表顯示 Excel : &rs:Format=EXCELOPENXML
            統計表顯示 pdf   : &rs:Format=pdf
             */
            var _param = m;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            if (!CheckUserAuth("Report"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = System.Net.HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            /* 不提供指定類型
             * 明細表顯示 Excel : &rs:Format=EXCELOPENXML), 
             * 統計表顯示 pdf   : &rs:Format=pdf) 
             * */
            //string _kind = m.RptItem.IndexOf("_DETAIL") >= 0 ? "EXCELOPENXML" : "pdf";
            bool isSdateOK = DateTime.TryParseExact(m.StartDate, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out DateTime sdate);
            bool isEdateOK = DateTime.TryParseExact(m.EndDate, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out DateTime edate);
            if (isSdateOK && isEdateOK) {
                string _sdate = string.Format("{0:yyyy-MM-dd}", sdate),
                       _edate = string.Format("{0:yyyy-MM-dd}", edate),
                        // rptUrl = string.Format($"{_rptServer}?/AIRMAM5.RPT/{m.RptItem}&fdSDATE={_sdate}&fdEDATE={_edate}&rs:Format={m.RptKind}&rc:parameters=false&rs:Command=Render");
                        //rptUrl = string.Format($"{_rptServer}?%2fAIRMAM5.RPT%2f{m.RptItem}&fdSDATE={_sdate}&fdEDATE={_edate}&rc:parameters=false&rs:Command=Render");
                       //2021-07-26 david modify 報表專案名稱須用設定的， 
                       rptUrl = string.Format($"{_rptServer}{m.RptItem}&fdSDATE={_sdate}&fdEDATE={_edate}&rc:parameters=false&rs:Command=Render");

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統報表檔案下載", "OK"),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
                result.IsSuccess = true;
                result.Data = new { rptUrl };
                result.Message = "檔案下載URL OK";
                result.StatusCode = System.Net.HttpStatusCode.OK;
            } else {
                result.IsSuccess = false;
                result.Data = new { rptUrl = string.Empty };
                result.Message = "日期格式錯誤";
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
            }

            #region _Serilog(Debug)
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "Report",
                Method = "[Search]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { Param = _param, Result = result },
                LogString = "系統報表檔案下載"
            });
            #endregion
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 每日入庫統計表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetArcListSum()
        {
            if (!CheckUserAuth("Report")) return View("NoAuth");
            //預存 dbo.spRPT_GET_ARC_LIST_SUM
            //ReportServer 識別 : rptGET_ARC_LIST_SUM
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "每日入庫統計表"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 每日入庫明細表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetArcListDetail()
        {
            if (!CheckUserAuth("Report")) return View("NoAuth");
            //預存 dbo.spRPT_GET_ARC_LIST_DETAIL
            //ReportServer 識別 : rptGET_ARC_LIST_DETAIL
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "每日入庫明細表"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 每日調用統計表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBookingListSum()
        {
            if (!CheckUserAuth("Report")) return View("NoAuth");
            //預存 dbo.spRPT_GET_BOOKING_LIST_SUM
            //ReportServer 識別 : rptGET_BOOKING_LIST_SUM
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "每日調用統計表"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 每日調用明細表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBookingListDetail()
        {
            if (!CheckUserAuth("Report")) return View("NoAuth");
            //預存 dbo.spRPT_GET_BOOKING_LIST_DETAIL
            //ReportServer 識別 : rptGET_BOOKING_LIST_DETAIL
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "每日調用明細表"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

    }
}