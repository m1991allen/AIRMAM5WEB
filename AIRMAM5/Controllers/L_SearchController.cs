using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.Filters;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 查詢作業 > 檢索記錄查詢
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class L_SearchController : BaseController
    {
        readonly string CONTR_NAEM = "L_Search";
        readonly TblSRHService _tblSRHService;
        readonly UsersService _usersService;

        public L_SearchController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _tblSRHService = new TblSRHService();
            _usersService = new UsersService(serilogService);
        }

        /// <summary>
        /// 檢索記錄首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return PartialView("NoAuth");
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "檢索記錄查詢"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 查詢頁面
        /// <para> [系統帳號]選單只顯示登入帳號自己，但若是administrator角色群組的帳號，就是顯示所有使用者帳號清單 </para>
        /// </summary>
        /// <returns></returns>
        public ActionResult _Search()
        {
            LoginIdDateSerarchModel m = new LoginIdDateSerarchModel()
            {
                UserId = User.Identity.GetUserId()
            };
            //TIPS_20200518: 帳號=系統管理者角色 下拉選單會有"全部"項目
            //  [系統帳號]選單只顯示登入帳號自己，但若是administrator角色群組的帳號，就是顯示所有使用者帳號清單
            var _b = _usersService.CurrentUserIsAdmin;

            m.LoginIdList = _usersService.GetUsersList(User.Identity.GetUserId(), _b);
            ////Tips: 檢索紀錄不提供"全部使用者"查詢. 系統管理者 顯示到全部使用者清單.
            //m.LoginIdList.RemoveRange(0, 1);
            return PartialView("_Search", m);
        }

        /// <summary>
        /// 查詢功能
        /// </summary>
        /// <returns></returns>
        public ActionResult Search(LoginIdDateSerarchModel m)
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

                string _sdate = string.Format($"{m.BeginDate:yyyy-MM-dd}"), _edate = string.Format($"{m.EndDate:yyyy-MM-dd}");
                var _urnm = string.IsNullOrEmpty(m.UserId) || m.UserId == "*" ? User.Identity.GetUserId() : UserManager.FindById(m.UserId).UserName;
                //Tips: 檢索紀錄不提供"全部使用者"查詢.
                var get = _tblSRHService.GetByParam(0, _sdate, _edate, _urnm);
                get = get.Count() > 0 ? get.OrderByDescending(s => s.fdCREATED_DATE).ToList() : get;

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "檢索記錄查詢", "完成"),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
                result.IsSuccess = true;
                result.StatusCode = HttpStatusCode.OK;
                result.Data = get;

                #region Serilog.INFO
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Search",
                    Method = "[Search]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { DataCount = get.Count(), Param = _param },
                    LogString = "檢索記錄查詢.Result"
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
                    Controller = "L_Search",
                    Method = "[Search]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Exception = ex },
                    LogString = "檢索記錄查詢.Exception",
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

        ///// <summary>
        /////  檢索記錄詳細內容 Modal --> 要顯示全文檢索的結果內容頁
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult _Details()
        //{
        //    return PartialView("_Details");
        //}
    }
}