
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Serilog;
using System;
using AIRMAM5.Models;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace AIRMAM5.Repository
{
    /// <summary>
    /// 自訂 : 註冊與尋找使用者
    /// </summary>
    public class AuthRepository : IDisposable
    {
        private ApplicationDbContext _ctx;
        private UserManager<ApplicationUser> _userManager;
        readonly UserStore<IdentityUser> userStore = new UserStore<IdentityUser>(new ApplicationDbContext());

        private ILogger _logger = Log.Logger;

        /// <summary>
        /// AuthRepository
        /// </summary>
        public AuthRepository()
        {
            _ctx = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_ctx));
            _userManager.UserValidator = new UserValidator<ApplicationUser>(_userManager) { AllowOnlyAlphanumericUserNames = false };
        }

        public ApplicationUser GetUser(string username)
        {
            var UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return UserManager.FindByName(username);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }

        public bool UserIsAdmin(string userid)
        {
            var UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return UserManager.IsInRole(userid, "Administrator"); ;
        }

        public bool UserIsMediaManager(string userid)
        {
            var UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return UserManager.IsInRole(userid, "MediaManager"); ;
        }


        ///// <summary>
        ///// 註冊:新增使用者資料
        ///// </summary>
        ///// <param name="umodel"></param>
        ///// <returns></returns>
        //public async Task<IdentityResult> RegisterUser(RegisterViewModel umodel)//(UserModel umodel)
        //{
        //    ApplicationUser user = new ApplicationUser
        //    {
        //        UserName = umodel.UserName, //fsLOGIN_ID
        //        fsNAME = umodel.RealName,//RealName = umodel.RealName,
        //        //RegisterTime = DateTime.Now,
        //        fsIS_ACTIVE = true,//IsStopAuthority = false,
        //        Email = umodel.Email
        //    };

        //    try
        //    {
        //        var result = await _userManager.CreateAsync(user, umodel.Password);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error("AuthRepository Exception: {0} \n {1}", ex.Message.ToString(), ex.ToString());
        //        throw ex;
        //    }
        //}

        //public async Task<IdentityResult> UpdateSignalrClientByUser(SignalrClientIdModel model)
        //{
        //    try
        //    {
        //        var user = await _userManager.FindByIdAsync(model.UserId);
        //        //user.SignalConnectId = model.SignalrConnectionId;
        //        var result = await _userManager.UpdateAsync(user);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error("AuthRepository Exception: {0} \n {1}", ex.Message.ToString(), ex.ToString());
        //        throw ex;
        //    }
        //}

    }
}