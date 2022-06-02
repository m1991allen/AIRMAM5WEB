using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.DBEntity.Models.Shared;
using Microsoft.AspNet.Identity;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.Filters;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 查詢作業 > 系統操作紀錄
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class L_LogController : BaseController
    {
        readonly string CONTR_NAEM = "L_Log";
        readonly UsersService _usersService;

        public L_LogController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _usersService = new UsersService(serilogService);
        }

        /// <summary>
        /// 操作首頁
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
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統操作紀錄"),
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
            bool _selectAll = _usersService.CurrentUserIsAdmin;
            m.LoginIdList = _usersService.GetUsersList(User.Identity.GetUserId(), _selectAll);

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
            #region _檢查_
            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = System.Net.HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            string _sdate = string.Format($"{m.BeginDate:yyyy/MM/dd}"), _edate = string.Format($"{m.EndDate:yyyy/MM/dd}");
            var _urnm = string.IsNullOrEmpty(m.UserId) ? string.Empty : UserManager.FindById(m.UserId).UserName;
            var get = _tblLogService.GetByParam(0, _sdate, _edate, _urnm);
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統操作紀錄", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(_param),
                User.Identity.Name);
            #endregion

            result.IsSuccess = true;
            result.Data = get;
            result.StatusCode = System.Net.HttpStatusCode.OK;
            #region _Serilog.INFO
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "L_Log",
                Method = "[Search]",
                EventLevel = SerilogLevelEnum.Information,
                Input = new { DataCount = get.Count(), Param = _param },
                LogString = "系統操作紀錄.Result"
            });
            #endregion

            //Tips: JsonConvert效能較佳，且沒有字串長度上限
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented,
               new JsonSerializerSettings
               {   //視自己需求可以拿掉
                   //指定如何處理迴圈引用: Ignore--不序列化,Error-丟擲異常,Serialize--仍要序列化
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                   DateFormatString = "yyyy/MM/dd HH:mm:ss"
               }), "application/json");
        }

        /// <summary>
        /// 操作紀錄 詳細Modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Details(long id)
        {
            if (!CheckUserAuth(CONTR_NAEM))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });

            var get = _tblLogService.GetByParam(id).FirstOrDefault();

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統操作紀錄", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { tblLog_ID = id }),
                User.Identity.Name);
            #endregion
            return PartialView("_Details", get);
        }

    }
}
