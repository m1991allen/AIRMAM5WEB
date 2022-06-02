using System;
using System.Linq;
using System.Web.Mvc;
using AIRMAM5.Models;
using Newtonsoft.Json;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.DBEntity.Models.Shared;
using System.Threading.Tasks;
using AIRMAM5.DBEntity.Models.Enums;
using System.Net;
using Microsoft.AspNet.Identity;
using AIRMAM5.Utility.Extensions;
using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.User;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.Filters;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 使用者帳號維護
    /// </summary>
    [Authorize]
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class UserController : BaseController
    {
        readonly UsersService _usersService;
        readonly UserExtendService _userExtendService;
        readonly GroupsService _groupsService;

        public UserController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService, ICodeService codeService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _tbzCodeService = codeService;
            _usersService = new UsersService(serilogService);
            _userExtendService = new UserExtendService();
            _groupsService = new GroupsService(serilogService);
        }

        /// <summary>
        /// 使用者帳號維護
        /// <para> Tips: 整個Controller有限制授權,所以會直接導向至登入頁,需要AllowAnonymous,才會進入動作檢查Index授權,才會導到無權限頁面 </para>
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (!CheckUserAuth("User")) return View("NoAuth");

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "使用者帳號維護"),
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
            return PartialView("_Search");
        }

        /// <summary>
        /// 使用者帳號維護 條件查詢
        /// </summary>
        /// <param name="fsUSER_ID">使用者id </param>
        /// <param name="fsLOGIN_ID">使用者帳號 </param>
        /// <param name="fsNAME">顯示名稱 </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(string userid, string loginid, string name)
        {
            var _param = new { userid, loginid, name };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            if (!CheckUserAuth("User"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                //203 Non-Authoritative Information - 非授權資訊。
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            userid = userid ?? string.Empty;
            loginid = loginid ?? string.Empty;
            name = name ?? string.Empty;

            var res = _usersService.GetBy(userid, loginid, name)
                    .Select(s => new UserListViewModel().FormatConversion(s))
                    .ToList();

            result.IsSuccess = true;
            result.Data = res;
            result.StatusCode = HttpStatusCode.OK;

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "使用者帳號維護", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(_param),
                User.Identity.Name);
            #endregion
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #region 【(列表-指定使用者)還原密碼】 - 透過電子郵件由帳號使用者自行重設密碼 - 20200806_Marked_改還原為預設值。
        ///// <summary>
        ///// 指定使用者帳號 還原密碼 POST
        ///// </summary>
        ///// <param name="userid"></param>
        ///// <remarks>
        /////  檢查-電子郵件是否驗證 (未驗證: 取驗證碼-> 寄郵件-> 檢查電子信箱-> 透過郵件連結,完成電子郵件驗證)
        /////    (1)未驗證處理：取驗證碼-> 寄郵件-> 檢查電子信箱-> 透過郵件連結,完成電子郵件驗證(尚未重設密碼)
        /////      TIP: 取驗證碼,會更新資料表tbmUSER_EXTEN, 失敗就返回false。
        /////    (2)已驗證處理：直接寄送「重設密碼」通知電子郵件
        ///// </remarks>
        //[HttpPost]
        //public async Task<ActionResult> RestorePwd(string userid)
        //{
        //    var _param = new { userid };
        //    ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
        //    VerifyResult res = new VerifyResult();
        //    string token = string.Empty, callbackUrl = string.Empty, _subject = string.Empty, _body = string.Empty;

        //    try
        //    {
        //        #region _檢查_
        //        var _ur = await UserManager.FindByIdAsync(userid);
        //        if (_ur == null)
        //        {
        //            result.IsSuccess = false;
        //            result.Message = string.Format($"使用者帳號資料錯誤/查無帳號資料!");
        //            result.StatusCode = HttpStatusCode.NotFound;  //404 Not Found -找不到。
        //            return Json(result, JsonRequestBehavior.DenyGet);
        //        }
        //        if (_ur.PasswordHash.Equals("//"))
        //        {
        //            result.IsSuccess = false;
        //            result.Message = string.Format($"{_ur.UserName} 使用LDAP帳號, 不提供密碼變更.");
        //            result.StatusCode = HttpStatusCode.Unauthorized;  //401 Unauthorized - 拒絕存取。
        //            return Json(result, JsonRequestBehavior.DenyGet);
        //        }
        //        #endregion

        //        // 電子郵件驗證
        //        var v = await this.CheckSendEmailVerification(userid);
        //        if (!v.IsSuccess)
        //        {
        //            result = new ResponseResultModel(v)
        //            {
        //                Records = _param,
        //                Message = v.Message,
        //                StatusCode = HttpStatusCode.Unauthorized 
        //            };
        //            return await Task.Run(() => Json(result, JsonRequestBehavior.DenyGet));
        //        }

        //        #region >>>>> 電子郵件已驗證: 以寄送郵件進行重新設定密碼
        //        // 傳送包含此連結的電子郵件
        //        token = await UserManager.GeneratePasswordResetTokenAsync(_ur.Id);//token時效預設為1日(24小時)
        //        callbackUrl = Url.Action("ResetPassword", "Account", new { userId = _ur.Id, code = token }, protocol: Request.Url.Scheme);

        //        _subject = string.Format("【AIRMAM媒資管理系統】重設密碼");
        //        _body = FormatString.RestorePwdContent("AIRMAM媒資管理系統", callbackUrl);
        //        await UserManager.SendEmailAsync(_ur.Id, _subject, _body);
        //        #endregion

        //        res = _usersService.RestorePwdUpdate(userid, User.Identity.Name);
        //        string _str = res.IsSuccess ? "成功" : "失敗", _log = string.Format($"帳號({_ur.UserName}) 還原密碼(Mail)");

        //        #region DB_LOG
        //        _tblLogService.Insert_L_Log(
        //            TbzCodeIdEnum.MSG001.ToString(),
        //            "M021",      //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
        //            string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, _log, _str),
        //            string.Format($"位置: {Request.UserHostAddress} "),
        //            JsonConvert.SerializeObject(_param),
        //            User.Identity.Name);
        //        #endregion

        //        result = new ResponseResultModel(res)
        //        {
        //            Records = _param,
        //            Message = res.IsSuccess ? string.Format($"{_ur.UserName} 重設密碼電子郵件已寄出.") : res.Message,
        //            StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
        //        };
        //        #region Serilog
        //        _serilogService.SerilogWriter(new SerilogInputModel
        //        {
        //            Controller = "User",
        //            Method = "[RestorePwd]",
        //            EventLevel = SerilogLevelEnum.Information,
        //            Input = new { Param = _param, Result = result },
        //            LogString = "帳號還原密碼.寄送電子郵件"
        //        });
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccess = false;
        //        result.Message = ex.Message;
        //        result.StatusCode = HttpStatusCode.InternalServerError;
        //        #region _Serilog
        //        _serilogService.SerilogWriter(new SerilogInputModel
        //        {
        //            Controller = "User",
        //            Method = "[RestorePwd]",
        //            EventLevel = SerilogLevelEnum.Error,
        //            Input = new { Param = _param, Result = res, Exception = ex },
        //            LogString = "帳號還原密碼.Exception",
        //            ErrorMessage = ex.Message
        //        });
        //        #endregion
        //    }
        //    return await Task.Run(() => Json(result, JsonRequestBehavior.DenyGet));
        //}

        ///// <summary>
        ///// 使用者電子郵件驗證: 檢查與寄發驗證信 處理 (包含密碼重設)
        ///// <para>　　🔔 true = 已驗證；false = 未驗證、或處理錯誤 </para>
        ///// </summary>
        ///// <param name="userid"></param>
        ///// <remarks> 驗證處理步驟
        /////  (1)產生使用者驗證碼
        /////  (2)產生密碼重置之有效期token、
        /////  (3)產生使用者密碼重置之有效期Token -->主要是使用這個
        /////  (4)寄出'電子郵件驗證' Mail
        /////  
        /////    TIPS: 與AccountController().CheckSendEmailVerification() 相同。
        ///// </remarks>
        //private async Task<VerifyResult> CheckSendEmailVerification(string userid)
        //{
        //    VerifyResult result = new VerifyResult();
        //    if (await UserManager.IsEmailConfirmedAsync(userid))
        //    {
        //        return new VerifyResult(true, "電子郵件已驗證.");
        //    }

        //    try
        //    {
        //        //↓↓↓↓↓ 電子郵件未驗證
        //        var _codeResult = _userExtendService.GetVerifyCodeAndUpdate(userid, 6);
        //        if (!_codeResult.IsSuccess)
        //        {
        //            //return _codeResult.IsSuccess;   //資料更新異常錯誤
        //            return _codeResult;
        //        }

        //        string _verifyCode = _codeResult.Data.ToString();
        //        var user = await UserManager.FindByIdAsync(userid);

        //        string mailToken = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);//token時效預設為1日(24小時)
        //        string pwToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        string _callbackUrl = Url.Action("CreateConfirmChange", "User", new { userId = user.Id, code = pwToken, code2 = mailToken, set = _verifyCode }, protocol: Request.Url.Scheme);

        //        //CreateConfirmChange: 電子郵件驗證頁面+密碼變更。
        //        string _subject = string.Format("【AIRMAM媒資管理系統】電子郵件驗證");
        //        string _body = FormatString.RegisterContent("AIRMAM媒資管理系統", _callbackUrl, user.Email, _verifyCode, user.UserName);
        //        await UserManager.SendEmailAsync(userid, _subject, _body);

        //        //
        //        result.IsSuccess = false;
        //        //result.Message = "📌 電子郵件未驗證, 已寄出驗證信, 請先完成電子郵件驗證。";
        //        result.Message = string.Format($"{user.UserName} 電子郵件未驗證, 已寄出驗證信, 請先完成電子郵件驗證.");
        //    }
        //    catch (Exception ex)
        //    {
        //        #region _Serilog
        //        _serilogService.SerilogWriter(new SerilogInputModel
        //        {
        //            Controller = "User",
        //            Method = "[CheckSendEmailVerification]",
        //            EventLevel = SerilogLevelEnum.Error,
        //            Input = new { userid, Exception = ex },
        //            LogString = "使用者電子郵件驗證.Exception",
        //            ErrorMessage = ex.Message
        //        });
        //        #endregion
        //        result.IsSuccess = false;
        //        result.Message = ex.Message;
        //    }

        //    #region Serilog
        //    _serilogService.SerilogWriter(new SerilogInputModel
        //    {
        //        Controller = "User",
        //        Method = "[CheckSendEmailVerification]",
        //        EventLevel = SerilogLevelEnum.Information,
        //        Input = new { UserID = userid, Result = result },
        //        LogString = "電子郵件檢查與寄發.Result"
        //    });
        //    #endregion
        //    return result;
        //}
        #endregion
        
        #region 【(列表-指定使用者)還原密碼】 - 還原為系統預設密碼值(Config)
        /// <summary>
        /// 指定使用者帳號 還原密碼 POST
        /// </summary>
        /// <param name="userid"></param>
        [HttpPost]
        public async Task<ActionResult> RestorePwd(string userid)
        {
            var _param = new { userid };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢查_
                var _ur = await UserManager.FindByIdAsync(userid);
                if (_ur == null)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"使用者帳號資料錯誤/查無帳號資料!");
                    result.StatusCode = HttpStatusCode.NoContent;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (_ur.PasswordHash.Equals("//"))
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"{_ur.UserName} 使用LDAP帳號, 不提供密碼變更.");
                    result.StatusCode = HttpStatusCode.Unauthorized;  //401 Unauthorized - 拒絕存取。
                    //203 Non-Authoritative Information - 非授權資訊。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                // 帳號的密碼還原為系統預設密碼
                res = _usersService.RestorePwdUpdate(userid, User.Identity.Name, Config.DefaultPaswd);
                string _str = res.IsSuccess ? "成功" : "失敗", _log = string.Format($"帳號({_ur.UserName}) 密碼還原為預設值.");

                #region DB_LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M021",      //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, _log, _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    Message = res.IsSuccess ? string.Format($"{_ur.UserName} 密碼已還原為預設值.") : res.Message,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "User",
                    Method = "[RestorePwd]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "帳號還原密碼.OK"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "User",
                    Method = "[RestorePwd]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Result = res, Exception = ex },
                    LogString = "帳號還原密碼.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return await Task.Run(() => Json(result, JsonRequestBehavior.DenyGet));
        }
        #endregion

        #region 【變更密碼】
        [AllowAnonymous]
        public ActionResult _ChangePassword()
        {
            if (CheckUserAuth("User"))
            {
                var user = UserManager.FindByName(User.Identity.Name);
                if (user.PasswordHash.Equals("//"))
                {
                    ModelState.AddModelError("", "使用LDAP帳號, 不提供密碼變更.");
                    // LDAP帳號就不要顯示「變更密碼」功能: OK
                    return View("NoAuth");
                }
                return View();
            }
            return View("NoAuth");
        }

        /// <summary>
        /// POST 變更密碼
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            ResponseResultModel result = new ResponseResultModel(false, string.Empty, model);
            IdentityResult res = new IdentityResult();
            #region _檢查_
            if (!CheckUserAuth("User"))
            {
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            if (!User.Identity.IsAuthenticated)
            {
                result.IsSuccess = false;
                result.Message = "使用者未驗證";
                result.StatusCode = HttpStatusCode.Unauthorized;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState.Keys)
                {
                    if (ModelState[item].Errors.Count > 0)
                    {
                        result.IsSuccess = false;
                        result.Message = ModelState[item].Errors[0].ErrorMessage;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                }
            }
            #endregion

            try
            {
                res = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.CurrentPassword, model.NewPassword);
                string _str = res.Succeeded ? "成功" : "失敗", _log = string.Format($"帳號({CurrentUser.UserName})變更密碼");
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    //"M009",     //[@USER_ID(@USER_NAME)] 設定 [@DATA_TYPE] 資料 @RESULT
                    "M021",     //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "變更密碼", _str),
                    Request.UserHostAddress,
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res.Succeeded)
                {
                    Message = res.Succeeded ? "密碼已變更" : res.Errors.FirstOrDefault().ToString(),
                    Records = model,
                    StatusCode = res.Succeeded ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                };
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "User",
                    Method = "[ChangePassword]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { param = model, Result = result },
                    LogString = "變更密碼.Result"
                });
                #endregion

                if (res.Succeeded)
                {
                    var _urex = _userExtendService.GetById(User.Identity.GetUserId());
                    if (_urex != null)
                    {
                        _urex.fbPWD_RESTORE = false;            //註記使用者帳號目前 非「還原密碼中」
                        _urex.fsVerifyCode = "Changed";         //註記使用者自行變更密碼
                        _urex.fdVerifyDate = null;
                        _urex.fdRESTORE_DATE = DateTime.Now;    //還原密碼操作 最後更新時間
                        _urex.fsRESTORE_BY = User.Identity.Name;//還原密碼操作 最後更新使用者
                        _urex.fdUPDATED_DATE = DateTime.Now;
                        _urex.fsUPDATED_BY = User.Identity.Name;
                        _userExtendService.Update(_urex);
                    }
                    /*不適用*///return RedirectToAction("ResetPasswordConfirmation", "Account");
                }

            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("", ex.ToString());
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "User",
                    Method = "[ChangePassword]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = model, Result = res, Exception = ex},
                    LogString = "Exception",
                    ErrorMessage = string.Format($"變更密碼. {ex.Message}")
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 【變更電子信箱  】 
        //TODO 20200521 只有先新增儲存邏輯, 沒有檢查LDAP帳號檢查,變更成功後目前是前端再呼叫sendEmailVerify api 看有沒有需要也在 controller中做完 by susie!!!!!!!!!!!!!!!!!!!!!!

        /// <summary>
        /// 變更電子信箱 ->成功:寄送驗證信
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ChangeEmail(string userid, string email) 
        {
            var _param = new { userid,email};
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region __檢查__
                if (!CheckUserAuth("User"))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden; //403-Forbidden：禁止使用。沒有權限。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                if (!ModelState.IsValid)
                {
                    foreach (var item in ModelState.Keys)
                    {
                        if (ModelState[item].Errors.Count > 0)
                        {
                            result.IsSuccess = false;
                            result.Message = ModelState[item].Errors[0].ErrorMessage;
                            result.StatusCode = HttpStatusCode.BadRequest; //400-Bad Request：錯誤的要求。
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                    }
                }

                if (string.IsNullOrEmpty(email))
                {
                    result.IsSuccess = false;
                    result.Message = "電子郵件不可為空值！";  //400 Bad Request - 錯誤的要求。
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (_usersService.ExistsUserEmail(email))
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"電子郵件({email}) 已被使用. ");
                    result.StatusCode = HttpStatusCode.PreconditionFailed;  //412 Precondition Failed - 指定條件失敗。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
                
                var ur = _usersService.GetById(userid);
                if (ur.fsEMAIL.Equals(email))
                {
                    result.IsSuccess = false;
                    result.Message = "輸入的電子郵件未變更！";
                    result.StatusCode = HttpStatusCode.PreconditionFailed; //412 Precondition Failed - 指定條件失敗。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                ur.fsEMAIL = email?? string.Empty;
                ur.fsEmailConfirmed = false;
                res = _usersService.Update(ur);
                string _str = res.IsSuccess ? "成功" : "失敗", _msg = string.Format($"變更電子信箱: {_str} ");
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",    //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "電子郵件", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                //電子郵件變更存檔成功->寄送新郵件驗證信
                if (res.IsSuccess)
                {
                    string _verifyCode = StringExtensions.GenerateRandomStr(6);

                    #region >>>>> 電子郵件變更存檔成功->寄送新郵件驗證信
                    string token = await UserManager.GenerateEmailConfirmationTokenAsync(userid);//token時效預設為1日(24小時)
                    string callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userid, code = token, set = _verifyCode }, protocol: Request.Url.Scheme);
                    //
                    string _subject = string.Format("【AIRMAM媒資管理系統】電子郵件驗證");
                    string _body = FormatString.ConfirmChangeEmailContent("AIRMAM媒資管理系統", callbackUrl, email, _verifyCode, ur.fsLOGIN_ID);
                    await UserManager.SendEmailAsync(userid, _subject, _body);
                    #endregion
                    _msg = string.Format($"{_msg}. 電子郵件驗證信已寄出, 請先完成電子郵件驗證.");
                }

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    Message = res.IsSuccess ? _msg : string.Format($"{_msg} ({res.Message})"),//string.Format($"變更電子信箱: {_str} "),
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region __Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "User",
                    Method = "[ChangeEmail]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "編輯使用者電子信箱.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region __Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "User",
                    Method = "[ChangeEmail]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param =_param, Result = res, exception = ex },
                    LogString = "編輯使用者電子信箱.Exception",
                    ErrorMessage = string.Format($"修改失敗. {ex.Message}")
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        /// <summary>
        /// 使用者帳號詳細內容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[InterceptorOfController(Method = "_Details")]
        public ActionResult _Details(string id)
        {
            //UserDetailViewModel m = new UserDetailViewModel
            //{
            //    DeptSelect = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.DEPT001.ToString(), true),
            //    FileSecretSelect = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.FILESECRET.ToString(), true),
            //    RoleGroupSelect = _groupsService.GetUserRoles()
            //};

            if (CheckUserAuth("User"))
            {
                var m = _usersService.GetBy(id).Select(s => new UserDetailViewModel()
                    //.Select(s => new UserDetailViewModel<spGET_USERS_Result>(s)
                    {
                    DeptSelect = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.DEPT001.ToString(), true),
                        FileSecretSelect = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.FILESECRET.ToString(), true),
                        RoleGroupSelect = _groupsService.GetUserRoles()
                    }.FormatConversion(s)).FirstOrDefault();

                m.FileSecretSelect = _tbzCodeService.CodeListItemSelected(TbzCodeIdEnum.FILESECRET.ToString(), m.fsFILE_SECRET);
                m.RoleGroupSelect = _groupsService.GroupListItemSelected(m.fsGROUPs);

                #region DB_LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "使用者帳號內容", "OK"),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(new { UserId = id }),
                    User.Identity.Name);
                #endregion
                return View("_Details", m);
            }

            return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });
        }

        /// <summary>
        /// 寄送使用者帳號電子郵件驗證信
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SendEmailVerify(string userid)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, new { userid });

            #region _檢查_
            if (!CheckUserAuth("User"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            var user = await UserManager.FindByIdAsync(userid);
            string _verifyCode = StringExtensions.GenerateRandomStr(6);

            VerifyResult _res = new VerifyResult();
            #region >>>>> Update tbmUserExtend
            var userex = _userExtendService.GetById(user.Id);//.FindByUserId(user.Id);
            userex.fdVerifyDate = DateTime.Now;
            userex.fsVerifyCode = _verifyCode;
            userex.fdUPDATED_DATE = DateTime.Now;
            userex.fsUPDATED_BY = CurrentUser.Id;

            _res = _userExtendService.Update(userex);
            #endregion

            string _str = _res.IsSuccess ? "成功" : "失敗", _msg = string.Format($"寄送 使用者:{user.UserName} 電子郵件驗證信。");
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M021", //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, _msg, _str),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(userid),
                User.Identity.Name);
            #endregion

            if (!_res.IsSuccess)
            {
                result.IsSuccess = _res.IsSuccess;
                result.Message = _res.Message;
                result.StatusCode = HttpStatusCode.BadRequest;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region >>>>> 電子郵件驗證 寄發
            string token = await UserManager.GenerateEmailConfirmationTokenAsync(userid);//token時效預設為1日(24小時)
            string callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userid, code = token, set = _verifyCode }, protocol: Request.Url.Scheme);
            //
            string _subject = string.Format("【AIRMAM媒資管理系統】電子郵件驗證");
            string _body = FormatString.ConfirmEmailContent("AIRMAM媒資管理系統", callbackUrl, user.Email, _verifyCode, user.UserName);
            await UserManager.SendEmailAsync(userid, _subject, _body);
            #endregion

            result.IsSuccess = true;
            result.Message = string.Format($"帳號:{user.UserName} 電子郵件驗證信 已寄出, 請先完成電子郵件驗證.");
            result.StatusCode = HttpStatusCode.OK;

            #region _Serilog.Info
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "User",
                Method = "[SendEmailVerify]",
                EventLevel = SerilogLevelEnum.Information,
                Input = new { User = user.UserName, VerifyCode = _verifyCode, Result = result },
                LogString = "電子郵件驗證信.Result"
            });
            #endregion
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #region 【帳號新增: 電子郵件驗證與密碼設定】
        /// <summary>
        /// 使用者帳號 新建頁面
        /// </summary>
        /// <returns></returns>
        //[InterceptorOfController(Method = "_Create")]
        public ActionResult _Create()
        {
            if (CheckUserAuth("User"))
            {
                UserCreateViewModel model = new UserCreateViewModel
                {
                    DeptList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.DEPT001.ToString()),
                    FileSecretList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.FILESECRET.ToString()),
                    RoleGroupLst = _groupsService.GetUserRoles(true)
                };

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                    string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "使用者帳號新增燈箱"),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(string.Empty),
                    User.Identity.Name);
                #endregion
                return PartialView("_Create", model);
            }

            return PartialView("NoAuth");
        }

        /// <summary>
        /// 使用者帳號 新建 POST
        /// <para></para>
        /// <para></para>
        /// </summary>
        /// <param name="umodel"></param>
        /// <remarks>
        /// 　　🔔 新建帳號,畫面不須輸入密碼。透過電子郵件驗證信,登入系統、再強制使用者變更密碼。
        /// 　　🔔 驗證碼=預設密碼、新建資料表: tbmUSER, tbmUSER_EXTEND。
        /// 　　💡 20200806_變更：新建帳號密碼=系統預設密碼
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult> Create(UserCreateViewModel umodel)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, umodel);
            IdentityResult idresult = new IdentityResult();

            try
            {
                #region _檢查_
                if (!CheckUserAuth("User"))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (!ModelState.IsValid)
                {
                    foreach (var item in ModelState.Keys)
                    {
                        if (ModelState[item].Errors.Count > 0)
                        {
                            result.IsSuccess = false;
                            result.Message = ModelState[item].Errors[0].ErrorMessage;
                            result.StatusCode = HttpStatusCode.BadRequest;
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                    }
                }
                if (_usersService.IsExists(umodel.UserName))//.ExistsUserName(umodel.UserName))
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"帳號({umodel.UserName}) 已被使用. ");
                    result.StatusCode = HttpStatusCode.PreconditionFailed;  //412 Precondition Failed - 指定條件失敗。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (_usersService.ExistsUserEmail(umodel.Email))
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"電子郵件({umodel.Email}) 已被使用. ");
                    result.StatusCode = HttpStatusCode.PreconditionFailed;  //412 Precondition Failed - 指定條件失敗。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                ApplicationUser newuser = new ApplicationUser
                {
                    UserName = umodel.UserName,
                    fsNAME = umodel.Name,
                    fsENAME = umodel.EName ?? string.Empty,
                    fsTITLE = umodel.Title ?? string.Empty,
                    fsDEPT_ID = umodel.DeptId,
                    fsIS_ACTIVE = true,
                    Email = umodel.Email ?? string.Empty,
                    PhoneNumber = umodel.Phone ?? string.Empty,
                    fsBOOKING_TARGET_PATH = umodel.BookingTargetPath ?? string.Empty,
                    fsFILE_SECRET = string.Join(";", umodel.SecretList),
                    fsDESCRIPTION = umodel.Description ?? string.Empty,
                    fsCREATED_BY = User.Identity.Name
                };

                ////臨時密碼(=驗證碼)
                //string _verifyCode = StringExtensions.GenerateRandomStr(6);
                string _verifyCode = Config.DefaultPaswd;
                idresult = await UserManager.CreateAsync(newuser, _verifyCode);//(newuser, umodel.Password);
                string _str = idresult.Succeeded ? "成功" : "失敗";

                //新增成功,才寄出驗證信
                // TIP:沒有完成電子郵件驗證也是可以登入系統, 若使用者操作忘記密碼就必須要完成電子信箱驗證
                if (idresult.Succeeded)
                {
                    result.Data = _usersService.GetBy(newuser.Id)
                            .Select(s => new UserListViewModel().FormatConversion(s))
                            .FirstOrDefault();

                    #region >>>>> Create tbmUserExtend
                    tbmUSER_EXTEND _exten = new tbmUSER_EXTEND
                    {
                        fsUSER_ID = newuser.Id,
                        fsSIGNALR_CONNECT_ID = string.Empty,
                        fsVerifyCode = _verifyCode,
                        fdCREATED_DATE = DateTime.Now,
                        fsCREATED_BY = CurrentUser.Id
                    };
                    _userExtendService.Create(_exten);
                    #endregion

                    #region >>>>> 電子郵件驗證 + 設定密碼
                    string mailToken = await UserManager.GenerateEmailConfirmationTokenAsync(newuser.Id);//token時效預設為1日(24小時)
                    string tokenpw = await UserManager.GeneratePasswordResetTokenAsync(newuser.Id);  
                    string callbackUrl = Url.Action("CreateConfirmChange", "User", new { userId = newuser.Id, code = tokenpw, code2 = mailToken, set = _verifyCode }, protocol: Request.Url.Scheme);
                    
                    //TODO_2020.3.13: 新建帳號的電子郵件驗證頁面要+密碼變更。
                    string _subject = string.Format("【AIRMAM媒資管理系統】電子郵件驗證");
                    string _body = FormatString.RegisterContent("AIRMAM媒資管理系統", callbackUrl, newuser.Email, _verifyCode, newuser.UserName);
                    await UserManager.SendEmailAsync(newuser.Id, _subject, _body);
                    //
                    _str = string.Format($"{_str}, 電子郵件驗證信 已寄出, 請先完成電子郵件驗證.");
                    //result.IsSuccess = idresult.Succeeded;
                    //result.Message = string.Format($"帳號:{newuser.UserName} 建立{_str}, 電子郵件驗證信 已寄出, 請先完成電子郵件驗證.");
                    //result.StatusCode = HttpStatusCode.OK;
                    //return Json(result, JsonRequestBehavior.DenyGet);
                    #endregion
                }

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "使用者:" + newuser.UserName, _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(umodel),
                    User.Identity.Name);
                #endregion
                result.IsSuccess = idresult.Succeeded;
                result.Message = string.Format($"帳號({umodel.UserName}) 新增{_str}");
                result.StatusCode = idresult.Succeeded ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed;
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "User",
                    Method = "[Create]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { param = umodel, Result = result, AppUser = newuser },
                    LogString = "新建使用者.Result"
                });
                #endregion

                #region 將使用者加入該角色
                if (idresult.Succeeded)
                {
                    foreach (var k in umodel.RoleList)
                    {
                        // TIP: dbo.tbmGROUPS.[Discriminator] ='ApplicationRole' / 'IdentityRole' 才會讀得到.
                        var _role = await AppRoleManager.FindByIdAsync(k);
                        if (_role != null)
                        {
                            var _r = await UserManager.AddToRoleAsync(newuser.Id, _role.Name);
                            #region _DB LOG
                            _tblLogService.Insert_L_Log(
                                TbzCodeIdEnum.MSG001.ToString(),
                                "M019",     //[@USER_ID(@USER_NAME)] 設定 [@TARGET] 的 [@DATA_TYPE] @RESULT
                                string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, "角色群組", "使用者:" + newuser.UserName, _str),
                                string.Format($"位置: {Request.UserHostAddress} "),
                                JsonConvert.SerializeObject(umodel),
                                User.Identity.Name);
                            #endregion
                        }
                    }
                }
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
                    Controller = "User",
                    Method = "[Create]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = umodel, IdResult = idresult, exception = ex },
                    LogString = "新建使用者.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return await Task.Run(() => Json(result, JsonRequestBehavior.DenyGet));
        }

        /// <summary>
        /// (郵件內容連結)Confirm: 電子郵件驗證與密碼設定
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code">設定密碼驗證用Token </param>
        /// <param name="code2">電子郵件驗證用Token </param>
        /// <param name="set">驗證碼 VerifyCode </param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> CreateConfirmChange(string userId, string code, string code2, string set)
        {
            var _param = new { userId, code, code2, set };
            try
            {
                var result = await UserManager.ConfirmEmailAsync(userId, code2);
                if (result.Succeeded)
                {
                    //完成電子郵件驗證。
                    var _urex = _userExtendService.GetById(userId);
                    if (_urex.fsVerifyCode == set)
                    {
                        _urex.fdVerifyDate = DateTime.Now;          //驗證日期更新
                        _urex.fdEmailConfirmDate = DateTime.Now;    //電子郵件驗證日期更新
                        _urex.fdUPDATED_DATE = DateTime.Now;
                        _urex.fsUPDATED_BY = userId;
                        _userExtendService.Update(_urex);
                    }

                    #region _Serilog.Info
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "User",
                        Method = "[CreateConfirmChange]",
                        EventLevel = SerilogLevelEnum.Information,
                        Input = new { param = _param, Result = result },
                        LogString = "帳號新增_電子郵件驗證.Result"
                    });
                    #endregion
                    // 驗證Token : By default, the generated tokens are single-use and expire in 1 day
                    if (UserManager.VerifyUserToken(userId, "ResetPassword", code))
                    {
                        var model = new ResetPasswordViewModel
                        {
                            UserId = userId,
                            Code = code2
                        };
                        #region _Serilog.Info
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "User",
                            Method = "[CreateConfirmChange]",
                            EventLevel = SerilogLevelEnum.Information,
                            Input = new { param = model, Result = result },
                            LogString = "帳號新增_密碼設定"
                        });
                        #endregion
                        return View(model);
                    }
                    return View("CreateConfirmChangeInvalid");
                }

                return View("CreateConfirmChangeInvalid");
            }
            catch (InvalidOperationException)
            {
                return View("CreateConfirmChangeInvalid");
            }
        }

        /// <summary>
        /// (郵件內容連結)Confirm: 電子郵件驗證與密碼設定, 確定save
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateConfirmChange(ResetPasswordViewModel model)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, model);
            #region _檢查_
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError("UserId", "使用者不存在。");
                return View(model);
            }
            #endregion

            IdentityResult res = new IdentityResult();
            VerifyResult _upd = new VerifyResult();
            try
            {
                res = await UserManager.ResetPasswordAsync(model.UserId, model.Code, model.Password);
                string _str = res.Succeeded ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    //"M009",      //[@USER_ID(@USER_NAME)] 設定 [@DATA_TYPE] 資料 @RESULT
                    "M019",     //[@USER_ID(@USER_NAME)] 設定 [@TARGET] 的 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogTargetParams, user.UserName, user.fsNAME, "新建帳號密碼", "使用者("+user.UserName+")", _str),
                    Request.UserHostAddress,
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion

                if (res.Succeeded)
                {
                    var _urex = _userExtendService.GetById(model.UserId);//.FindByUserId(model.UserId);
                    _urex.fbPWD_RESTORE = false;            //註記使用者帳號目前 非「還原密碼中」
                    _urex.fsVerifyCode = model.Password;
                    _urex.fdRESTORE_DATE = DateTime.Now;    //還原/變更密碼操作 最後更新時間
                    _urex.fsRESTORE_BY = model.UserId;      //還原/變更密碼操作 最後更新使用者
                    _urex.fdUPDATED_DATE = DateTime.Now;
                    _urex.fsUPDATED_BY = model.UserId;
                    _upd = _userExtendService.Update(_urex);

                    #region _Serilog.Info
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "User",
                        Method = "[CreateConfirmChange]",
                        EventLevel = SerilogLevelEnum.Information,
                        Input = new { param = model, IdResult = res, UpdResult = _upd },
                        LogString = "帳號新增_UserExtend.Result"
                    });
                    #endregion
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                //
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "User",
                    Method = "[CreateConfirmChange]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { param = model, Result = res },
                    LogString = "帳號新增_密碼設定.Result"
                });
                #endregion
                foreach (var error in res.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "User",
                    Method = "[CreateConfirmChange]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = model, IdResult = res, UpdResult = _upd, Exception = ex },
                    LogString = "帳號新增_密碼設定.Exception",
                    ErrorMessage = string.Format($"帳號新增_密碼設定. {ex.Message}")
                });
                #endregion
                ModelState.AddModelError("UserId", ex.Message);
            }

            return View();
        }
        
        /// <summary>
        /// 帳號新增-電子郵件驗證 失敗View
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult CreateConfirmChangeInvalid()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// 使用者帳號編輯
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[InterceptorOfController(Method = "_Edit")]
        public ActionResult _Edit(string id)
        {
            if (!CheckUserAuth("User")) return RedirectToAction("NoAuthModal", "Error", new { @id = "EditModal" });
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "使用者帳號維護-編輯"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { UserId = id }),
                User.Identity.Name);
            #endregion

            //var newmd = new UserEditViewModel();
            UserEditViewModel m = _usersService.GetBy(id).Select(s => new UserEditViewModel().FormatConversion(s))
                .FirstOrDefault();

            m.DeptSelect = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.DEPT001.ToString(), true);
            m.FileSecretSelect = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.FILESECRET.ToString(), true);
            m.RoleGroupSelect = _groupsService.GetUserRoles();

            return PartialView("_Edit",m);
        }

        /// <summary>
        /// 使用者帳號編輯 POST
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        //[InterceptorOfController(Method = "Edit")]
        public ActionResult Edit(UserEditViewModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region __檢查__
                if (!CheckUserAuth("User"))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden; //403-Forbidden：禁止使用。沒有權限。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (!ModelState.IsValid)
                {
                    foreach (var item in ModelState.Keys)
                    {
                        if (ModelState[item].Errors.Count > 0)
                        {
                            result.IsSuccess = false;
                            result.Message = ModelState[item].Errors[0].ErrorMessage;
                            result.StatusCode = HttpStatusCode.BadRequest; //400-Bad Request：錯誤的要求。
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                    }
                }
                #endregion

                var ur = _usersService.GetById(model.fsUSER_ID);//.FindUserByUserId(model.fsUSER_ID);

                ur.fsPHONE = model.fsPHONE ?? string.Empty;
                ur.fsDESCRIPTION = model.fsDESCRIPTION ?? string.Empty;
                ur.fsFILE_SECRET = string.Join(";", model.FSecretList);
                ur.fsBOOKING_TARGET_PATH = model.fsBOOKING_TARGET_PATH ?? string.Empty;
                ur.GroupIds = string.Join(";", model.GroupList);
                ur.fdUPDATED_DATE = DateTime.Now;
                ur.fsUPDATED_BY = User.Identity.Name;
                #region 210-系統帳號維護追蹤 : 增加可編輯欄位
                ur.fsNAME = model.fsNAME;
                ur.fsENAME = model.fsENAME ?? string.Empty;
                ur.fsTITLE = model.fsTITLE ?? string.Empty;
                ur.fsDEPT_ID = model.fsDEPT_ID;
                //ur.fsEMAIL = model.fsEMAIL ?? string.Empty;
                //TODO_2020.3.13: 變更電子郵件,會需要重新驗證電子郵件。電子郵件異動另開metohd處理。
                #endregion 210-系統帳號維護追蹤 : 增加可編輯欄位

                res = _usersService.Update(ur);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "使用者:" + model.fsLOGIN_ID, _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion
                
                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region __Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "User",
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "編輯使用者.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region __Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "User",
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = model, Result = res, exception = ex },
                    LogString = "編輯使用者.Exception",
                    ErrorMessage = string.Format($"修改失敗. {ex.Message}")
                });
                #endregion
            }
            
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #region 【使用者帳號 啟用/停用】
        /// <summary>
        /// 使用者帳號 啟用/停用
        /// </summary>
        /// <param name="id">USERID</param>
        /// <param name="active">是否啟用 true/false</param>
        /// <returns></returns>
        //[InterceptorOfController(Method = "UpdateActive")]
        [HttpPost]
        public async Task<ActionResult> UpdateActive(string id, bool active)
        {
            var _param = new { id, active };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            IdentityResult idresult = new IdentityResult();

            try
            {
                #region _檢查_
                if (!CheckUserAuth("User"))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden; //403-Forbidden：禁止使用。沒有權限。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                var _ur = await UserManager.FindByIdAsync(id);
                if (_ur == null)
                {
                    result.IsSuccess = false;
                    result.Message = "使用者帳號資料錯誤/查無帳號資料!";
                    result.StatusCode = HttpStatusCode.NotFound;  //404 Not Found - 找不到。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                _ur.fsIS_ACTIVE = active;
                _ur.fdUPDATED_DATE = DateTime.Now;
                _ur.fsUPDATED_BY = User.Identity.Name;

                idresult = await UserManager.UpdateAsync(_ur);
                string _str = idresult.Succeeded ? "帳號已" + (active ? "啟用" : "停用") : "帳號" + (active ? "啟用" : "停用") + " 失敗"
                , _str2 = idresult.Succeeded ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M019",     //[@USER_ID(@USER_NAME)] 設定 [@TARGET] 的 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, ("帳號" + (active ? "啟用" : "停用")), "使用者:" + _ur.UserName, _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(new { UserId = id, IsActive = active }),
                    User.Identity.Name);
                #endregion

                result.IsSuccess = idresult.Succeeded;
                result.Message = _str;
                result.StatusCode = idresult.Succeeded ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed;
                #region __Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "User",
                    Method = "[UpdateActive]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "帳號啟用/停用.Result"
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
                    Controller = "User",
                    Method = "[UpdateActive]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, IdResult = idresult, exception = ex },
                    LogString = "帳號啟用/停用.Exception",
                    ErrorMessage = string.Format($"啟用/停用 失敗. {ex.Message}")
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

    }
}
