using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using AIRMAM5.Models;
using System.Net.Mail;
using System.Net.Configuration;
using System.Configuration;
using Serilog;
using Newtonsoft.Json;

namespace AIRMAM5
{
    public class EmailService : IIdentityMessageService
    {
        private ILogger _logger = Log.Logger;
        public Task SendAsync(IdentityMessage message)
        {
            //// 將您的電子郵件服務外掛到這裡以傳送電子郵件。
            //return Task.FromResult(0);
            var smtp = new SmtpClient();
            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            Task t = null;
            try
            {
                var mail = new MailMessage(smtpSection.From, message.Destination)
                {
                    IsBodyHtml = true,
                    Subject = message.Subject,
                    Body = message.Body
                };
                //mail.From = new MailAddress(smtpSection.From);
                //mail.To.Add(message.Destination)
                smtp.Timeout = 1000; ;

                #region Serilog
                string _logmsg = string.Format($"【EmailService】寄送郵件驗證....完成 \n  寄送 {message.Destination} ");
                _logger.Information(_logmsg);
                #endregion
                t = Task.Run(() => smtp.SendAsync(mail, null));
            }
            catch (Exception ex)
            {
                #region Serilog
                string _logmsg = string.Format($"【EmailService】寄送郵件驗證....Exception: {ex.ToString()} 。");
                _logger.Information(_logmsg);
                #endregion
            }

            return t;
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // 將您的 SMS 服務外掛到這裡以傳送簡訊。
            return Task.FromResult(0);
        }
    }

    // 設定此應用程式中使用的應用程式使用者管理員。UserManager 在 ASP.NET Identity 中定義且由應用程式中使用。
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // 設定使用者名稱的驗證邏輯
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false, //允許使用者名稱只有數字
                //RequireUniqueEmail = true
            };

            // 設定密碼的驗證邏輯
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,             //要求長度
                RequireNonLetterOrDigit = false, //是否 要有特殊字元
                RequireDigit = false,            //是否 要有數字
                RequireLowercase = false,        //是否 小寫字母
                RequireUppercase = false,        //是否 大寫字母
            };

            // 設定使用者鎖定詳細資料
            manager.UserLockoutEnabledByDefault = Config.LockedEnable;//true; //建立帳號時是否啟用鎖定
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(Config.LockoutTimeSpan); //鎖定時間
            manager.MaxFailedAccessAttemptsBeforeLockout = Config.AccessFailMax; //最多失敗次數

            // 註冊雙因素驗證提供者。此應用程式使用手機和電子郵件接收驗證碼以驗證使用者
            // 您可以撰寫專屬提供者，並將它外掛到這裡。
            manager.RegisterTwoFactorProvider("電話代碼", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "您的安全碼為 {0}"
            });
            manager.RegisterTwoFactorProvider("電子郵件代碼", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "安全碼",
                BodyFormat = "您的安全碼為 {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // 設定在此應用程式中使用的應用程式登入管理員。
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    /// <summary>
    /// The role manager used by the application to store roles and their connections to users
    /// 增加角色管理員相關的設定
    /// 一、於 IdentityConfig.cs 先建立角色管理員
    /// 二、於App_Start \ Startup.Auth.cs加入 "//增加腳色的OwinContext"    
    /// (以上這兩個步驟，你的identity 就具備Role的功能)
    /// </summary>
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
        }
    }

    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            //註解自動重建資料庫
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public async static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            //管理者
            string name = "admin";
            string email = "admin@example.com";
            string password = "P@ssw0rd";
            string roleName = "Administrator";
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new IdentityRole(roleName);
                var roleresult = roleManager.Create(role);
            }
            var user = new ApplicationUser { UserName = name, Email = email };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                result = userManager.SetLockoutEnabled(user.Id, false);
                var rolesForUser = userManager.GetRoles(user.Id);
                if (!rolesForUser.Contains(role.Name))
                {
                    var r = userManager.AddToRole(user.Id, role.Name);
                }
            }
        }
    }
}
