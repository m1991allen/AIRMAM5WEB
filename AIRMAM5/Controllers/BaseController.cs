using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using AIRMAM5.Repository;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.Utility.Common;
using AIRMAM5.Models;
using AIRMAM5.DBEntity.Procedure;
using AIRMAM5.DBEntity.Interface;
using System;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Enums;
using System.Threading.Tasks;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 基底 Controller
    /// </summary>
    public class BaseController : Controller
    {
        public AuthRepository _authRepository;
        public ApplicationSignInManager _signInManager;
        public ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }
        public ApplicationRoleManager AppRoleManager
        {
            get { return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>(); }
            private set { _roleManager = value; }
        }

        /// <summary>
        /// 專案紀錄 Serilog Service
        /// </summary>
        public ISerilogService _serilogService;
        /// <summary>
        /// 系統操作紀錄 Service
        /// </summary>
        public ITblLogService _tblLogService;
        /// <summary>
        /// 系統代碼資料 Service
        /// </summary>
        public ICodeService _tbzCodeService;
        /// <summary>
        /// 功能項目 Service
        /// </summary>
        public IFunctionsService _funcsService;
        /// <summary>
        /// 專案是否使用TSM服務
        /// </summary>
        public readonly string IsUseTSM = Config.IsUseTSM;//ConfigurationManager.AppSettings["IsUseTSM"].ToString();
        /// <summary>
        /// 系統是否啟用"目錄末端節點Queue"
        /// </summary>
        public readonly bool IsUsingQueue = true;

        #region 【目前使用者 ApplicationUser】
        /// <summary>
        /// 目前使用者 ApplicationUser
        /// </summary>
        public ApplicationUser CurrentUser { get; }

        #endregion

        public BaseController(ISerilogService serilogService, IFunctionsService functionsService)
        {
            _serilogService = serilogService;
            _funcsService = functionsService;

            var userFromAuthCookie = System.Threading.Thread.CurrentPrincipal;
            if (userFromAuthCookie != null && userFromAuthCookie.Identity.IsAuthenticated)
            {
                string userNameFromAuthCookie = userFromAuthCookie.Identity.Name;
                _authRepository = new AuthRepository();
                CurrentUser = _authRepository.GetUser(userNameFromAuthCookie);

                var _configService = new ConfigService();
                var cong = _configService.GetConfigBy("DIRECTORIES_USING_QUEUE").FirstOrDefault();  //設定系統目錄維護功能中，是否啟用Queue節點操作
                if (cong != null) { IsUsingQueue = cong.fsVALUE == "1" ? true : false; }
            }
        }

        /// <summary>
        /// 判斷使用者驗證、是否可使用頁面
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public bool CheckUserAuth(string controller)
        {
            bool idAuthority = User.Identity.IsAuthenticated;
            string _loginID = User.Identity.Name;

            ////判斷該使用者是否可以使用此頁面
            ////idAuthority = clsSECURITY.GetUserFunctionAuth(_loginID, controller);
            //_funcsService = new FunctionsService();
            idAuthority = _funcsService.CheckUserFuncAuth(_loginID, controller);

            return idAuthority;
        }

        /// <summary>
        /// 取 使用者帳號當次登入的 cookie : UserName-ExpiredTime-SecurityKey-ValidateCode-IsActive-FileSecret-loginLogid
        /// </summary>
        /// <returns></returns>
        public List<string> GetUserLoginLogId()
        {
            //long _loginLogid = -1;

            if (Request == null || Request.Cookies["User"] == null) return null;//return new List<string>();
            List<string> _ckUser = CommonSecurity.CookieDecrypt(Request.Cookies["User"].Value).Split(new char[] { '-' }).ToList();
            //long.TryParse(_ckUser[6], out _loginLogid); //tbmLOGIN.id
            //return _loginLogid;
            return _ckUser;
        }

        /// <summary>
        /// 使用者 SignalR ConnectID
        /// </summary>
        /// <returns></returns>
        public string GetUserSignalrConnectID(string userid)
        {
            UsersService _usersService = new UsersService(_serilogService);
            var urex = _usersService.GetById(userid).tbmUSER_EXTEND;

            return urex == null ? string.Empty : urex.fsSIGNALR_CONNECT_ID;
        }

        /// <summary>
        /// 角色檢查,不存在就新建
        /// </summary>
        /// <param name="rolename">角色定義名稱 ex: GeneralUser, MediaUser.... </param>
        /// <returns></returns>
        public async Task RoleCheck(string rolename, string desc)
        {
            try
            {
                bool roleExists = await AppRoleManager.RoleExistsAsync(rolename);

                if (!roleExists)
                {
                    ApplicationRole role = new ApplicationRole
                    {
                        Name = rolename,
                        fsCREATED_BY = "SYS",
                        fdCREATED_DATE = DateTime.Now,
                        fsUPDATED_BY = string.Empty,
                        fdUPDATED_DATE = DateTime.Now,
                        fsDESCRIPTION = desc,
                        Discriminator = "ApplicationRole",
                        fsTYPE = string.Empty,
                    };
                    var roleResult = await AppRoleManager.CreateAsync(role);
                    //var roleResult = await AppRoleManager.CreateAsync(new IdentityRole("GenericUser"));
                }
            }
            catch (Exception ex)
            {
                #region >>> Serilog-Error
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Base",
                    Method = "RoleCheck",
                    LogString = "Exception",
                    Input = new { RoleName = rolename, Exception = ex },
                    ErrorMessage = ex.Message.ToString(),
                    EventLevel = SerilogLevelEnum.Error
                });
                #endregion
            }
        }

    }
}