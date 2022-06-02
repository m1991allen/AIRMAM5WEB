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
    /// 查詢作業 > 登入登出紀錄
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class L_LoginController : BaseController
    {
        readonly string CONTR_NAEM = "L_Login";
        readonly TblLoginService _tblLoginService;
        readonly UsersService _usersService;

        public L_LoginController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _tblLoginService = new TblLoginService();
            _usersService = new UsersService(serilogService);
        }

        /// <summary>
        /// 首頁
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
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "登入登出紀錄"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        ///  查詢頁面
        /// <para> [系統帳號]選單只顯示登入帳號自己，但若是administrator角色群組的帳號，就是顯示所有使用者帳號清單 </para>
        /// </summary>
        /// <returns></returns>
        public ActionResult _Search()
        {
            LoginIdDateSerarchModel m = new LoginIdDateSerarchModel()
            {
                UserId = User.Identity.GetUserId()
            };
            //var roles = AppRoleManager.Roles.ToList();
            //var _b = User.IsInRole("Administrator");
            //TIPS_20200518: 帳號=系統管理者角色 下拉選單會有"全部"項目
            //  [系統帳號]選單只顯示登入帳號自己，但若是administrator角色群組的帳號，就是顯示所有使用者帳號清單
            var _b = _usersService.CurrentUserIsAdmin;

            m.LoginIdList = _usersService.GetUsersList(User.Identity.GetUserId(), _b);
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
            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = System.Net.HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            string _sdate = string.Format($"{m.BeginDate:yyyy/MM/dd}"), _edate = string.Format($"{m.EndDate:yyyy/MM/dd}");
            var _urnm = string.IsNullOrEmpty(m.UserId) || m.UserId == "*" ? string.Empty : UserManager.FindById(m.UserId).UserName;
            var get = _tblLoginService.GetByParam(_urnm, _sdate, _edate);
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "登入登出紀錄", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(_param),
                User.Identity.Name);
            #endregion

            result.IsSuccess = true;
            result.StatusCode = System.Net.HttpStatusCode.OK;
            result.Data = get;
            #region Serilog(Debug)
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "L_Login",
                Method = "[Search]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { DataCount = get.Count(), Param = _param },
                LogString = "登入紀錄查詢.Result"
            });
            #endregion
            //Tips: JsonConvert效能較佳，且沒有字串長度上限
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented,
               new JsonSerializerSettings
               {   //視自己需求可以拿掉
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore //指定如何處理迴圈引用: Ignore--不序列化,Error-丟擲異常,Serialize--仍要序列化
                   , DateFormatString = "yyyy/MM/dd HH:mm:ss"       //將日期專換成固定格式
               }), "application/json");
        }

        /// <summary>
        /// 登入紀錄詳細資料 Modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Details(long id)
        {
            if (!CheckUserAuth(CONTR_NAEM))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });

            var get = _tblLoginService.GetByParam(string.Empty, string.Empty, string.Empty, id).FirstOrDefault();

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "登入登出紀錄", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { tblLog_ID = id }),
                User.Identity.Name);
            #endregion
            return PartialView("_Details", get);
        }
        
    }
}
