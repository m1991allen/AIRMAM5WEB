using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Procedure;
using System.Net;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.Filters;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 查詢作業 > 資料異動
    /// </summary>
    [Authorize]
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class L_TranController : BaseController
    {
        readonly string CONTR_NAEM = "L_Tran";
        readonly ProcedureGetService _procedureGetService;
        readonly UsersService _usersService;

        public L_TranController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _usersService = new UsersService(serilogService);
            _procedureGetService = new ProcedureGetService();
        }

        /// <summary>
        ///  資料異動首頁
        /// <para> Tips: 整個Controller有限制授權,所以會直接導向至登入頁,需要AllowAnonymous,才會進入動作檢查Index授權,才會導到無權限頁面 </para>
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            if(!CheckUserAuth(CONTR_NAEM)) return View("NoAuth");
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "資料異動查詢"),
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
            return PartialView("_Search");
        }

        /// <summary>
        /// 查詢功能
        /// <para> 有開放使用，就是顯示全部使用者帳號資料. </para>
        /// </summary>
        /// <returns></returns>
        public ActionResult Search(DateSerarchModel m)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, m);
            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            string _sdate = m.BeginDate.Year == 1 ? string.Format($"{DateTime.Now.AddDays(-3):yyyy/MM/dd}") : string.Format($"{m.BeginDate:yyyy/MM/dd}")
                , _edate = m.EndDate.Year == 1 ? string.Format($"{DateTime.Now.AddDays(+1):yyyy/MM/dd}") : string.Format($"{m.EndDate:yyyy/MM/dd}");

            var get = _procedureGetService.GetLTranBy(_sdate, _edate);
            ////✘-TIPS_2020/01/30: 非"系統管理員"帳號,開啟此頁面,只顯示登入帳號的資料。
            //Modified_2020/05/18: 有開放使用，就是顯示全部使用者帳號資料
            //if (!_usersService.CurrentUserIsAdmin)
            //{
            //    get = get.Where(x => x.fsACTION_BY == CurrentUser.UserName).ToList();
            //}

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "資料異動查詢", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(m),
                User.Identity.Name);
            #endregion

            result.IsSuccess = true;
            result.StatusCode = HttpStatusCode.OK;
            result.Data = get;

            #region Serilog.INFO
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "L_Tran",
                Method = "[Search]",
                EventLevel = SerilogLevelEnum.Information,
                Input = new { DataCount = get.Count(), Param = m },
                LogString = "資料異動查詢.Result"
            });
            #endregion
            //return Json(result, JsonRequestBehavior.DenyGet);
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented,
               new JsonSerializerSettings
               {   //視自己需求可以拿掉
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore   //指定如何處理迴圈引用: Ignore--不序列化,Error-丟擲異常,Serialize--仍要序列化
                   //NullValueHandling = NullValueHandling.Ignore   //去除null 的Property
                   , DateFormatString = "yyyy/MM/dd HH:mm:ss"       //將日期專換成固定格式
               }), "application/json");
        }

        /// <summary>
        ///  異動紀錄內容 Modal
        /// </summary>
        /// <returns></returns>
       [AllowAnonymous]
        public ActionResult _Details(string log)
        {
            if (!CheckUserAuth(CONTR_NAEM))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });

            //TODO-20190911: 目前沒有規劃要顯示這麼詳,要前後比對效能不好  先把主表顯示出來就好,暫時不顯示詳細資料
            return PartialView("_Details");
        }

    }
}
