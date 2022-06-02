using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AIRMAM5.DBEntity.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AIRMAM5.Models;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.Utility.Common;
using Newtonsoft.Json;
using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;

namespace AIRMAM5.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private TblLoginService _tblLoginService;
        readonly UsersService _usersServices;
        readonly UserExtendService _userExtendService;

        public AccountController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            :base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _tblLoginService = new TblLoginService();
            _usersServices = new UsersService(serilogService);
            _userExtendService = new UserExtendService();
        }
        //public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //    //
        //    _serilogService = new SerilogService();
        //    _tblLoginService = new TblLoginService();
        //    _tblLogService = new TblLogService();
        //    _usersServices = new UsersService();
        //    _userExtendService = new UserExtendService();
        //}

        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = string.Empty;//returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) { return View(model); }

            var appUser = await UserManager.FindByNameAsync(model.UserName);
            var _loginlog = new tblLOGIN(); // 登入紀錄 tblLOGIN

            /* 帳號鎖定中 */
            if (appUser != null && appUser.LockoutEndDateUtc > DateTime.UtcNow)
            {
                string msg = string.Format($"帳號鎖定中 - {appUser.LockoutEndDateUtc.Value.ToLocalTime():yyyy-MM-dd HH:mm:ss} ");
                #region  _DB LOG [tblLOG]
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "S001",
                    string.Format(FormatString.LoginParams, model.UserName, appUser.fsNAME, "失敗"),
                    string.Format($"位置: {Request.UserHostAddress} / 失敗原因:{msg}."),
                    Request.UserHostAddress,
                    model.UserName);
                #endregion
                #region LoginLog [tblLOGIN]
                _tblLoginService.CreateLogin(
                    new tblLOGIN
                    {
                        fsUSER_ID = appUser.Id,
                        fsLOGIN_ID = appUser.UserName,
                        fdSTIME = DateTime.Now,
                        fdETIME = null,
                        fsNOTE = string.Format($"登入失敗:{msg} "),
                        fdCREATED_DATE = DateTime.Now,
                        fsCREATED_BY = appUser.UserName
                    });
                #endregion

                //return View("Lockout");
                ModelState.AddModelError("RememberMe", string.Format($"帳號 {appUser.UserName} 已遭鎖定,請稍後再試一次。"));
                return View("Login", model);
            }

            /* 帳號是否為LDAP */
            bool ldapAtuh = (appUser != null && appUser.PasswordHash.Equals("//")) ? _usersServices.LDAPAuth(model.UserName, model.Password) : false;
            #region >>>  LDAP驗證登入
            if (ldapAtuh)
            {
                #region >>> LDAP帳號第一次登入&&無系統帳號資料, 建立帳號資料
                if (appUser == null)
                {
                    appUser = new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = model.UserName,
                        Email = string.Format($"{model.UserName}@ftv.com.tw"),
                        EmailConfirmed = true,
                        PasswordHash = string.Empty,  //LDAP帳號不填入密碼
                        fdCREATED_DATE = DateTime.Now,
                        fsCREATED_BY = model.UserName,
                        fsNAME = model.UserName,
                        fsENAME = model.UserName.Replace(".", " "),
                    };
                    var insRes = await UserManager.CreateAsync(appUser);
                    if (!insRes.Succeeded)
                    {
                        string errmsg = "帳號建立失敗.(";
                        foreach (var err in insRes.Errors) { errmsg += err + " "; }
                        ModelState.AddModelError("UserName", errmsg + ")");
                        return View(model);
                    }

                    var getUser = await UserManager.FindByNameAsync(appUser.UserName);
                    appUser = getUser;
                    /* UserExtend 新增 */
                    var userExtend = new tbmUSER_EXTEND
                    {
                        fsUSER_ID = appUser.Id,
                        fsSIGNALR_CONNECT_ID = string.Empty,
                        fdCREATED_DATE = DateTime.Now,
                        fsCREATED_BY = appUser.UserName,
                        fdUPDATED_DATE = DateTime.Now,
                        fsUPDATED_BY = string.Empty,
                    };
                    var exRes = _userExtendService.Create(userExtend);
                }
                #endregion

                // ★驗證新需求處理_20211117
                await ResetAccessFailed(appUser, SignInStatus.Success);
                // ★預設一律為: GeneralUser 一般使用者。
                await RoleCheck("GeneralUser", "通用使用者");
                if (!await UserManager.IsInRoleAsync(appUser.Id, "GeneralUser"))
                {
                    await UserManager.AddToRoleAsync(appUser.Id, "GeneralUser");
                }
                
                // 登入實作
                await SignInManager.SignInAsync(appUser, model.RememberMe, model.RememberMe);
                //SetCookieValue(appUser);
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Account",
                    Method = "[Login]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { returnUrl, appUser },
                    LogString = "LDAP登入.Result"
                });
                #endregion
            }
            #endregion
            else
            {
                if (appUser == null)
                {
                    ModelState.AddModelError("UserName", string.Format($"帳號錯誤 {model.UserName}"));
                    return View(model);
                }
                _loginlog = new tblLOGIN
                {
                    fsUSER_ID = appUser.Id,
                    fsLOGIN_ID = appUser.UserName,
                    fdSTIME = DateTime.Now,
                    fdETIME = null,
                    fsNOTE = string.Empty,
                    fdCREATED_DATE = DateTime.Now,
                    fsCREATED_BY = appUser.UserName
                };

                #region >>> ★ 是LDAP帳號,但驗證失敗次敗 處理
                if (!ldapAtuh && appUser.PasswordHash.Equals("//"))
                {
                    // ★驗證新需求處理_20211117
                    await ResetAccessFailed(appUser, SignInStatus.Failure);
                    /* ★調用一次 AccessFailedAsync()，AccessFailedCount 累加一，等於/大於定義次數，AccessFailedCount 歸零，寫入 LockoutEndDateUtc */
                    await UserManager.AccessFailedAsync(appUser.Id);
                    /* ★設置有5次, 當滿第5次錯誤時，執行上面後，FailedCount會轉為0. */
                    int failedCnt = await UserManager.GetAccessFailedCountAsync(appUser.Id);
                    if (failedCnt == 0)
                    {
                        await UserManager.SetLockoutEndDateAsync(appUser.Id, DateTimeOffset.UtcNow.AddMinutes(Config.LockoutTimeSpan)); //鎖定結束UTC時間

                        #region  _DB LOG [tblLOG]
                        _tblLogService.Insert_L_Log(
                            TbzCodeIdEnum.MSG001.ToString(),
                            "S001",
                            string.Format(FormatString.LoginParams, model.UserName, appUser.fsNAME, "失敗"),
                            string.Format($"位置: {Request.UserHostAddress} / 失敗原因:LDAP嘗試登入多次暫被鎖定"),
                            Request.UserHostAddress,
                            model.UserName);
                        #endregion
                        #region LoginLog [tblLOGIN]
                        _loginlog.fsNOTE = string.Format($"登入失敗:LDAP嘗試登入多次暫被鎖定 - {appUser.LockoutEndDateUtc.Value.ToLocalTime()}");
                        _tblLoginService.CreateLogin(_loginlog);
                        #endregion
                        
                        return View("Lockout");
                    }
                    
                    failedCnt = await UserManager.GetAccessFailedCountAsync(appUser.Id);
                    ModelState.AddModelError("RememberMe", string.Format($"登入嘗試失敗{failedCnt}次,請確認LDAP帳號密碼。"));

                    #region  _DB LOG [tblLOG]
                    _tblLogService.Insert_L_Log(
                        TbzCodeIdEnum.MSG001.ToString(),
                        "S001",
                        string.Format(FormatString.LoginParams, model.UserName, appUser.fsNAME, "失敗"),
                        string.Format($"位置: {Request.UserHostAddress} / 失敗原因::LDAP帳號密碼不正確 {failedCnt}次."),
                        Request.UserHostAddress,
                        model.UserName);
                    #endregion
                    #region LoginLog [tblLOGIN]
                    _loginlog.fsNOTE = string.Format($"登入失敗:LDAP帳號密碼不正確 {failedCnt}次.");
                    _tblLoginService.CreateLogin(_loginlog);
                    #endregion
                    return View("Login", model);
                }
                #endregion

                // shouldLockout: false 不會計算為帳戶鎖定的登入失敗, 若要啟用密碼失敗來觸發帳戶鎖定，請變更為 shouldLockout: true
                var result = SignInManager.PasswordSignIn(model.UserName, model.Password, model.RememberMe, shouldLockout: Config.LockedEnable);
                switch (result)
                {
                    case SignInStatus.Success:
                        if (appUser != null && !appUser.fsIS_ACTIVE)
                        {
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            ModelState.AddModelError("RememberMe", "使用者帳號 已停用。");

                            #region  _DB LOG [tblLOG]
                            _tblLogService.Insert_L_Log(
                                TbzCodeIdEnum.MSG001.ToString(),
                                "S001",
                                string.Format(FormatString.LoginParams, model.UserName, appUser.fsNAME, "失敗"),
                                string.Format($"位置: {Request.UserHostAddress} / 失敗原因:帳號已停用"),
                                Request.UserHostAddress,
                                model.UserName);
                            #endregion
                            #region LoginLog [tblLOGIN]
                            _loginlog.fsNOTE = "登入失敗:帳號已停用";
                            _tblLoginService.CreateLogin(_loginlog);
                            #endregion
                            return View("Login");
                        }
                        if (!_userExtendService.IsExists(appUser.Id))
                        {
                            var userExtend = new tbmUSER_EXTEND
                            {
                                fsUSER_ID = appUser.Id,
                                fsSIGNALR_CONNECT_ID = string.Empty,
                                fdCREATED_DATE = DateTime.Now,
                                fsCREATED_BY = appUser.UserName,
                                fdUPDATED_DATE = DateTime.Now,
                                fsUPDATED_BY = string.Empty,
                            };
                            var exRes = _userExtendService.Create(userExtend);
                        }

                        // ★驗證新需求處理_20211117
                        await ResetAccessFailed(appUser, SignInStatus.Success);
                        //SetCookieValue(appUser);
                        //return RedirectToLocal(returnUrl); 
                        break;
                    case SignInStatus.LockedOut:
                        //帳號已被鎖定: __分鐘後可再登入。
                        string msg = string.Format($"帳號鎖定中 - {appUser.LockoutEndDateUtc.Value.ToLocalTime():yyyy-MM-dd HH:mm:ss} ");

                        #region  _DB LOG [tblLOG]
                        _tblLogService.Insert_L_Log(
                            TbzCodeIdEnum.MSG001.ToString(),
                            "S001",
                            string.Format(FormatString.LoginParams, model.UserName, appUser.fsNAME, "失敗"),
                            string.Format($"位置: {Request.UserHostAddress} / 失敗原因:帳號已被鎖定"),
                            Request.UserHostAddress,
                            model.UserName);
                        #endregion
                        #region LoginLog [tblLOGIN]
                        _loginlog.fsNOTE = string.Format($"登入失敗:{msg}");
                        _tblLoginService.CreateLogin(_loginlog);
                        #endregion

                        //return View("Lockout");
                        ModelState.AddModelError("RememberMe", string.Format($"帳號 {appUser.UserName} 已遭鎖定,請稍後再試一次。"));
                        return View("Login", model);
                    case SignInStatus.RequiresVerification:
                        //傳送驗證碼
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
                    case SignInStatus.Failure:
                        ////LockoutEnabled 是否啟用自動鎖定, LockoutEndUtc 鎖定結束時間, AccessFailedCount 登入失敗次數
                        int failedCnt = UserManager.GetAccessFailedCount(appUser.Id);
                        ModelState.AddModelError("RememberMe", string.Format($"登入嘗試失敗{failedCnt}次,請確認帳號密碼。"));

                        #region  _DB LOG [tblLOG]
                        _tblLogService.Insert_L_Log(
                            TbzCodeIdEnum.MSG001.ToString(),
                            "S001",
                            string.Format(FormatString.LoginParams, model.UserName, appUser.fsNAME, "失敗"),
                            string.Format($"位置: {Request.UserHostAddress} / 失敗原因:帳號密碼不正確 {failedCnt}次."),
                            Request.UserHostAddress,
                            model.UserName);
                        #endregion
                        #region LoginLog [tblLOGIN]
                        _loginlog.fsNOTE = string.Format($"登入失敗:帳號密碼不正確 {failedCnt}次.");
                        _tblLoginService.CreateLogin(_loginlog);
                        #endregion
                        return View("Login", model);
                    default:
                        ModelState.AddModelError("RememberMe", "登入嘗試失試。");
                        #region  _DB LOG [tblLOG]
                        _tblLogService.Insert_L_Log(
                            TbzCodeIdEnum.MSG001.ToString(),
                            "S001",
                            string.Format(FormatString.LoginParams, model.UserName, appUser.fsNAME, "失敗"),
                            string.Format($"位置: {Request.UserHostAddress} / 失敗原因: empty."),
                            Request.UserHostAddress,
                            model.UserName);
                        #endregion
                        #region LoginLog [tblLOGIN]
                        _loginlog.fsNOTE = "登入失敗: empty.";
                        _tblLoginService.CreateLogin(_loginlog);
                        #endregion
                        return View("Login", model);
                }
            }

            #region _處理記住我功能 HttpCookie
            HttpCookie cookie = Request.Cookies["RememberMe"] ?? new HttpCookie("RememberMe")
            {
                //HttpOnly = true,//避免Cookie被JavaScript存取
                Path = "/",
                Value = model.UserName
            };
            cookie.Expires = (model.RememberMe) ? DateTime.Now.AddDays(30) : DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);
            #endregion
            SetCookieValue(appUser);
            return RedirectToLocal(returnUrl);
        }

        /// <summary>
        /// ★驗證新需求處理_20211117
        ///   <para>①記錄驗證失敗日期 ②驗證失敗次數欄位歸零: 登入成功後、登入失敗且與上次失敗非同一日. </para>
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        protected async Task ResetAccessFailed(ApplicationUser appUser, SignInStatus signInStatus)
        {
            bool chg = false;

            switch (signInStatus)
            {
                case SignInStatus.Success:
                    // 登入成功:驗證失敗次數欄位歸零
                    appUser.AccessFailedCount = 0;
                    chg = !chg ? true : chg;

                    // 若帳號停用,改為"啟用"
                    if (!appUser.fsIS_ACTIVE) { appUser.fsIS_ACTIVE = true; chg = !chg ? true : chg; }

                    break;
                case SignInStatus.Failure:
                    var userex = _userExtendService.GetById(appUser.Id);
                    // 1.驗證失敗日期非同一日:驗證失敗次數欄位歸零,再計次數.
                    var b = userex.fdAccessFailedDate.Equals(DateTime.Now.Date);
                    if (appUser.AccessFailedCount > 0 && (!userex.fdAccessFailedDate.Equals(DateTime.Now.Date)))
                    {
                        appUser.AccessFailedCount = 0;
                        chg = !chg ? true : chg;
                    }
                    //2.更新-驗證失敗日期
                    userex.fdAccessFailedDate = DateTime.Now.Date;
                    userex.fdUPDATED_DATE = DateTime.Now;
                    userex.fsUPDATED_BY = appUser.UserName;
                    _userExtendService.Update(userex);

                    break;
            }

            if (chg)
            {
                appUser.fdUPDATED_DATE = DateTime.Now;
                appUser.fsUPDATED_BY = appUser.UserName;
                await UserManager.UpdateAsync(appUser);
            }
        }

        /// <summary>
        /// user cookie加密資訊
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        private void SetCookieValue(ApplicationUser appuser)
        {
            #region  _DB LOG [tblLOG]
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S001",
                string.Format(FormatString.LoginParams, appuser.UserName, appuser.fsNAME, "成功"),
                string.Format($"位置: {Request.UserHostAddress} "),
                Request.UserHostAddress,
                appuser.UserName);
            #endregion
            #region LoginLog [tblLOGIN]
            tblLOGIN _loginlog = new tblLOGIN
            {
                fsUSER_ID = appuser.Id,
                fsLOGIN_ID = appuser.UserName,
                fdSTIME = DateTime.Now,
                fdETIME = null,
                fsNOTE = "操作系統中",
                fdCREATED_DATE = DateTime.Now,
                fsCREATED_BY = appuser.UserName//User.Identity.Name
            };

            long loginLogid = (long)_tblLoginService.CreateLogin(_loginlog); //(appuser.UserName, DateTime.Now, null, "操作系統中");
            #endregion

            var _user = appuser;
            string fsENC_KEY = Config.fsENC_KEY;//ConfigurationManager.AppSettings["fsENC_KEY"].ToString();
            

            //取得有效的TimeSpan日期
            long fdEXPIRED_TIME = Convert.ToInt64(DateTime.Now.AddYears(99).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

            //GUID
            string fsSECURITY_KEY = Guid.NewGuid().ToString().Replace("-", "");

            //檢查碼
            //string fsVALIDATE_CODE = clsSECURITY.Encrypt(_user.UserName + fdEXPIRED_TIME.ToString() + fsSECURITY_KEY);
            string fsVALIDATE_CODE = CommonSecurity.Encrypt(_user.UserName + fdEXPIRED_TIME.ToString() + fsSECURITY_KEY);

            var _ur = _usersServices.GetBy(string.Empty, _user.UserName, string.Empty)[0];
            //增加登入紀錄編號 fnLOGIN_ID
            var _keystr = string.Format($"{_user.UserName}-{fdEXPIRED_TIME}-{fsSECURITY_KEY}-{fsVALIDATE_CODE}-{_user.fsIS_ACTIVE}-{_user.fsFILE_SECRET}-{loginLogid}");
            //UserName-ExpiredTime-SecurityKey-ValidateCode-IsActive-FileSecret-loginLogid

            //cookie加密
            string fsVALUE = CommonSecurity.CookieEncrypt(_keystr);

            HttpCookie UserCookie = new HttpCookie("User", fsVALUE)
            {
                Expires = DateTime.Now.AddDays(1)//.AddYears(99)
            };
            Response.Cookies.Add(UserCookie);

            #region _Serilog.Debug
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "Account",
                Method = "[SetCookieValue]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { appuser.UserName, UserCookie = fsVALUE },
                LogString = "使用者登入Cookies"
            });
            #endregion
        }

        /*// GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // 需要使用者已透過使用者名稱/密碼或外部登入進行登入
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }*/

        /*// POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 下列程式碼保護兩個因素碼不受暴力密碼破解攻擊。 
            // 如果使用者輸入不正確的代碼來表示一段指定的時間，則使用者帳戶 
            // 會有一段指定的時間遭到鎖定。 
            // 您可以在 IdentityConfig 中設定帳戶鎖定設定
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "代碼無效。");
                    return View(model);
            }
        }*/

        /// <summary>
        /// 使用者註冊頁面---未完
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            var deptList = new CodeService().GetCodeItemList(TbzCodeIdEnum.DEPT001.ToString());
            deptList.Insert(0, new SelectListItem { Value = "", Text = "-- 隸屬部門 --"});
            RegisterViewModel model = new RegisterViewModel
            {
                DeptList = deptList,
                RoleList = RolesList()
            };
            return View(model);
        }

        /// <summary>
        /// 使用者註冊頁面 POST---未完
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName, fsNAME = model.RealName, Email = model.Email, 
                    fsDEPT_ID = model.DeptId
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    #region _TODO: 帳號角色設定
                    string rolename = model.RoleName;
                    var _role = await this.AppRoleManager.FindByNameAsync(rolename);
                    if (this.AppRoleManager.RoleExists(rolename) == false)
                    {
                        var role = new ApplicationRole(rolename)
                        {
                            fdCREATED_DATE = DateTime.Now,
                            fsCREATED_BY = User.Identity.Name
                        };
                        await this.AppRoleManager.CreateAsync(role);
                    }
                    //將使用者加入該角色
                    await UserManager.AddToRoleAsync(user.Id, rolename);
                    #endregion

                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            model.RoleList = RolesList();
            return View(model);
        }
        
        /// <summary>
        /// 角色
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> RolesList()
        {
            var _serg = new GroupsService(_serilogService);
            return _serg.GetUserRoles(true);
        }

        #region =====電子郵件信箱驗證 =====
        /// <summary>
        /// 使用者電子郵件驗證: 檢查與寄發驗證信 處理 (包含密碼重設)  TIPS: 與UserController().CheckSendEmailVerification() 相同。
        /// <para>　　🔔 true = 已驗證；false = 未驗證、或處理錯誤 </para>
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task<VerifyResult> CheckSendEmailVerification(string userid)
        {
            VerifyResult result = new VerifyResult();
            if (await UserManager.IsEmailConfirmedAsync(userid))
            {
                return new VerifyResult(true, "電子郵件已驗證.");
            }

            var user = await UserManager.FindByIdAsync(userid);
            try
            {
                //↓↓↓↓↓ 未驗證
                var _codeResult = _userExtendService.GetVerifyCodeAndUpdate(userid, 6);
                if (!_codeResult.IsSuccess)
                {
                    //return _codeResult.IsSuccess;   //資料更新異常錯誤
                    return _codeResult;
                }

                string _verifyCode = _codeResult.Data.ToString();

                string mailToken = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);//token時效預設為1日(24小時)
                string pwToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);//產生密碼重置token
                string _callbackUrl = Url.Action("CreateConfirmChange", "User", new { userId = user.Id, code = pwToken, code2 = mailToken, set = _verifyCode }, protocol: Request.Url.Scheme);
                
                //CreateConfirmChange: 電子郵件驗證頁面+密碼變更。
                string _subject = string.Format("【AIRMAM媒資管理系統】電子郵件驗證");
                string _body = FormatString.RegisterContent("AIRMAM媒資管理系統", _callbackUrl, user.Email, _verifyCode, user.UserName);
                await UserManager.SendEmailAsync(userid, _subject, _body);
                //
                result.IsSuccess = false;
                //result.Message = "📌 電子郵件未驗證, 已寄出驗證信, 請先完成電子郵件驗證。";
                result.Message = string.Format($"{user.UserName} 電子郵件未驗證, 已寄出驗證信, 請先完成電子郵件驗證.");
            }
            catch (Exception ex)
            {
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Account",
                    Method = "[CheckSendEmailVerification]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { userid, user.UserName, Exception = ex },
                    LogString = "電子郵件檢查與寄發.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// (郵件內容連結)Confirm: 電子郵件驗證操作
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code">電子郵件驗證用Token </param>
        /// <param name="set">驗證碼 VerifyCode </param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code, string set)
        {
            if (userId == null || code == null || set == null)
            {
                return View("ConfirmEmailInvalid");
            }

            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                var _ur = _usersServices.GetById(userId);
                _ur.fsEmailConfirmed = true;
                _ur.fdUPDATED_DATE = DateTime.Now;
                _ur.fsUPDATED_BY = userId;

                var _urex = _ur.tbmUSER_EXTEND;
                if (_urex.fsVerifyCode == set)
                {
                    _urex.fdVerifyDate = DateTime.Now;          //驗證日期更新
                    _urex.fdEmailConfirmDate = DateTime.Now;    //電子郵件驗證日期更新
                    _urex.fdUPDATED_DATE = DateTime.Now;
                    _urex.fsUPDATED_BY = _ur.fsLOGIN_ID;

                    var _res = _usersServices.ConfirmEmailUpdate(_ur, _urex);
                    if (_res.IsSuccess)
                    {
                        return View("ConfirmEmail");
                    }
                }
            }
            return View("ConfirmEmailInvalid");
            //return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
        #endregion

        #region ---登入頁---【忘記密碼】
        /// <summary>
        /// 忘記密碼
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// 忘記密碼: 寄送電子郵件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, model);
            
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                #region _檢查_
                if (user == null)
                {
                    ModelState.AddModelError("Email", "資料不正確,請確認!");
                    return View(model);
                }
                //LDAP帳號不可在此使用忘記密碼!
                if (user.PasswordHash.Equals("//"))
                {
                    ModelState.AddModelError("Email", "使用LDAP帳號, 不提供密碼變更。");
                    return View(model);
                }
                // 🔔檢查電子郵件驗證狀態
                if (!(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    //ModelState.AddModelError("Email", "電子郵件未驗證,請洽系統管理員。");
                    //return View(model);
                    var _chk = await CheckSendEmailVerification(user.Id);
                    if (!_chk.IsSuccess)
                    {
                        ModelState.AddModelError("Email", _chk.Message);
                        return View(model);
                    }
                }
                #endregion

                // 如需如何進行帳戶確認及密碼重設的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=320771
                // 傳送包含此連結的電子郵件
                string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);//token時效預設為1日(24小時)
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = token }, protocol: Request.Url.Scheme);

                string _subject = string.Format("【AIRMAM媒資管理系統】重設密碼");
                string _body = FormatString.ForgetPwdContent("AIRMAM媒資管理系統", callbackUrl);
                await UserManager.SendEmailAsync(user.Id, _subject, _body);

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M021",      //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogParams, user.UserName, user.fsNAME, "忘記密碼(Mail)", "成功"),
                    Request.UserHostAddress,
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Account",
                    Method = "[ForgotPassword]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { param = model, Result = "OK" },
                    LogString = "忘記密碼.寄送電子郵件"
                });
                #endregion
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        /// <summary>
        /// 忘記密碼 寄送郵件確認頁
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        #endregion

        #region 【忘記密碼->郵件連結->重設密碼】
        /// <summary>
        /// 重設變更密碼
        /// </summary>
        /// <param name="code"> Token </param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            //return code == null ? View("Error") : View();
            try
            {
                // 驗證Token : By default, the generated tokens are single-use and expire in 1 day
                if (UserManager.VerifyUserToken(userId, "ResetPassword", code))
                {
                    var model = new ResetPasswordViewModel
                    {
                        UserId = userId,
                        Code = code //Token
                    };

                    return View(model);
                }
                return View("ResetPasswordInvalid");
            }
            catch (InvalidOperationException)
            {
                return View("ResetPasswordInvalid");
            }
        }

        /// <summary>
        /// POST 忘記密碼->郵件連結----> 重設密碼
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, model);
            #region _檢查_
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(model.UserId);
            //if (user == null)
            //{
            //    ModelState.AddModelError("UserId", "使用者不存在。");
            //    return View(model);
            //}
            #endregion

            try
            {
                var res = await UserManager.ResetPasswordAsync(model.UserId, model.Code, model.Password);
                var ur = _usersServices.GetById(model.UserId);
                string _str = res.Succeeded ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M021",      //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogParams, (user == null ? string.Empty : user.UserName), (user == null ? string.Empty : user.fsNAME), "重設密碼", _str),
                    Request.UserHostAddress,
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion
                #region _Serilog.Debug
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Account",
                    Method = "[ResetPassword]",
                    EventLevel = SerilogLevelEnum.Debug,
                    Input = new { param = model, Result = res },
                    LogString = "重設密碼.Result"
                });
                #endregion

                if (res.Succeeded)
                {
                    var _urex = _userExtendService.GetById(model.UserId);//.FindByUserId(model.UserId);
                    _urex.fbPWD_RESTORE = false;            //註記使用者帳號目前 非「還原密碼中」
                    _urex.fdRESTORE_DATE = DateTime.Now;    //還原密碼操作 最後更新時間
                    _urex.fsRESTORE_BY = ur.fsLOGIN_ID;      //還原密碼操作 最後更新使用者
                    _urex.fdUPDATED_DATE = DateTime.Now;
                    _urex.fsUPDATED_BY = ur.fsLOGIN_ID;
                    _userExtendService.Update(_urex);

                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }

                AddErrors(res);
            }
            catch (Exception ex)
            {
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Account",
                    Method = "[ResetPassword]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "重設密碼.Exception",
                    ErrorMessage = string.Format($"重設密碼. {ex.Message}")
                });
                #endregion
                ModelState.AddModelError("UserId", ex.Message);
            }

            return View();
        }

        /// <summary>
        /// 密碼已重設 確認轉頁view
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        /// <summary>
        /// 忘記密碼-重設密碼 失敗View
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordInvalid()
        {
            return View();
        }
        #endregion

        /* POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 要求重新導向至外部登入提供者
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }
        */

        /* GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }
        */

        /* POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // 產生並傳送 Token
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }
        */

        /* GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // 若使用者已經有登入資料，請使用此外部登入提供者登入使用者
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // 若使用者沒有帳戶，請提示使用者建立帳戶
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }
        */

        /* POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // 從外部登入提供者處取得使用者資訊
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }
        */

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            var _kies = GetUserLoginLogId();

            if (_kies != null)
            {
                long.TryParse(_kies[6], out long _loginLogid);

                //DB Login log : 使用者帳號 登出時間
                int rownum = _tblLoginService.UpdateLogout(_loginLogid, _kies[0]);

                //+移除 使用者+登入記錄編號 的連線id 資料
                new TblHubConnectionService().RemoveByLoginLogId(CurrentUser.Id, _loginLogid);
            }

            //登出時 清除domain下所有cookie
            string[] myCookies = Request.Cookies.AllKeys;
            foreach (string cookie in myCookies)
            {
                Response.Cookies[cookie].Expires = DateTime.Now.AddYears(-100);
            }

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();

            #region  _DB LOG [tblLOG]
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S002",     //[@USER_ID(@USER_NAME)] 登出系統 @RESULT
                string.Format(FormatString.LoginParams, CurrentUser.UserName, CurrentUser.fsNAME, "成功"),
                string.Format($"位置: {Request.UserHostAddress} "),
                Request.UserHostAddress,
                CurrentUser.UserName);
            #endregion

            return RedirectToAction("Login", new { ReturnUrl = "" });
        }

        /*// GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        */

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helper
        // 新增外部登入時用來當做 XSRF 保護
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}