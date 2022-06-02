using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AIRMAM5.Models
{
    // 您可將更多屬性新增至 ApplicationUser 類別，藉此為使用者新增設定檔資料，如需深入了解，請瀏覽 https://go.microsoft.com/fwlink/?LinkID=317594。
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 注意 authenticationType 必須符合 CookieAuthenticationOptions.AuthenticationType 中定義的項目
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在這裡新增自訂使用者宣告
            return userIdentity;
        }

        #region extended property 自訂欄位
        /// <summary>
        /// 使用者姓名 
        /// </summary>
        public string fsNAME { get; set; } = "";
        /// <summary>
        /// 使用者英文名
        /// </summary>
        public string fsENAME { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public string fsTITLE { get; set; } = "";
        /// <summary>
        /// 部門
        /// </summary>
        public string fsDEPT_ID { get; set; } = "";
        /// <summary>
        /// 備註
        /// </summary>
        public string fsDESCRIPTION { get; set; } = "";
        /// <summary>
        /// 文件機密等級
        /// </summary>
        [MaxLength(30), DataType(DataType.Text)]
        public string fsFILE_SECRET { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public string fsBOOKING_TARGET_PATH { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public bool fsIS_ACTIVE { get; set; } = true;

        public DateTime fdCREATED_DATE { get; set; } = DateTime.Now;
        public string fsCREATED_BY { get; set; } = "";
        public DateTime? fdUPDATED_DATE { get; set; }
        [MaxLength(128)]
        public string fsUPDATED_BY { get; set; } = "";
        #endregion
        [NotMapped]
        public string Discriminator { get; set; } = "";
    }

    /// <summary>
    /// 角色
    /// </summary>
    /// <remarks>Adding a Customized Role to the Identity Samples Project</remarks>
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }

        public ApplicationRole(string roleName, string description, DateTime createdDate)
            : base(roleName)
        {
            base.Name = roleName;

            this.fsDESCRIPTION = description;
            this.fdCREATED_DATE = createdDate;
            this.fsCREATED_BY = "IdentityRole";
        }

        #region extended property 自訂欄位
        public string fsDESCRIPTION { get; set; } = "";
        public string fsTYPE { get; set; } = "";
        public DateTime fdCREATED_DATE { get; set; } = DateTime.Now;
        public string fsCREATED_BY { get; set; } = "";
        public DateTime? fdUPDATED_DATE { get; set; }
        public string fsUPDATED_BY { get; set; } = "";
        #endregion

        [NotMapped]
        public string Discriminator { get; set; } = "ApplicationRole";
    }

    /// <summary>
    /// 使用者角色對應
    /// </summary>
    public class ApplicationUserRole : IdentityUserRole
    {
        public ApplicationUserRole() : base() { }

        #region extended property 自訂欄位
        public DateTime fdCREATED_DATE { get; set; } = DateTime.Now;
        
        public string fsCREATED_BY { get; set; } = "";
        public DateTime? fdUPDATED_DATE { get; set; }
        public string fsUPDATED_BY { get; set; } = "";
        #endregion

        [NotMapped]
        public string Discriminator { get; set; } = "IdentityUserRole";
        [NotMapped]
        public string IdentityUser_Id { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        //static ApplicationDbContext()
        //{
        //    // Set the database intializer which is run once during application start
        //    // This seeds the database with admin user credentials and admin role
        //    Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        //}

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        /// <summary>
        /// Identity 預設資料表更名
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(DbModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //AspNetUsers
            var TbmUsers = builder.Entity<IdentityUser>().ToTable("tbmUSERS", "dbo");
            TbmUsers.Property(p => p.Id).HasColumnName("fsUSER_ID");
            TbmUsers.Property(p => p.UserName).HasColumnName("fsLOGIN_ID").IsRequired();//.HasMaxLength(50);
            builder.Entity<ApplicationUser>().ToTable("tbmUSERS", "dbo").Property(u => u.fsNAME).IsRequired().HasMaxLength(50);
            builder.Entity<ApplicationUser>().ToTable("tbmUSERS", "dbo").Property(u => u.fsENAME).IsOptional().HasMaxLength(50);
            builder.Entity<ApplicationUser>().ToTable("tbmUSERS", "dbo").Property(u => u.fsTITLE).IsOptional().HasMaxLength(50);
            builder.Entity<ApplicationUser>().ToTable("tbmUSERS", "dbo").Property(u => u.fsDEPT_ID).IsOptional().HasMaxLength(10);
            builder.Entity<ApplicationUser>().ToTable("tbmUSERS", "dbo").Property(u => u.fsDESCRIPTION).IsOptional().HasMaxLength(1024);
            builder.Entity<ApplicationUser>().ToTable("tbmUSERS", "dbo").Property(u => u.fsFILE_SECRET).IsRequired().HasMaxLength(30);
            builder.Entity<ApplicationUser>().ToTable("tbmUSERS", "dbo").Property(u => u.fsBOOKING_TARGET_PATH).IsOptional().HasMaxLength(500);
            builder.Entity<ApplicationUser>().ToTable("tbmUSERS", "dbo").Property(u => u.fsIS_ACTIVE).IsRequired();

            TbmUsers.Property(p => p.Email).HasColumnName("fsEMAIL").IsOptional();
            TbmUsers.Property(p => p.EmailConfirmed).HasColumnName("fsEmailConfirmed");
            TbmUsers.Property(p => p.PasswordHash).HasColumnName("fsPASSWORD").IsRequired();//.HasMaxLength(255);
            TbmUsers.Property(p => p.SecurityStamp).HasColumnName("fsSecurityStamp");
            TbmUsers.Property(p => p.PhoneNumber).HasColumnName("fsPHONE").IsOptional();//.HasMaxLength(20);
            TbmUsers.Property(p => p.PhoneNumberConfirmed).HasColumnName("fbPhoneConfirmed");
            TbmUsers.Property(p => p.TwoFactorEnabled).HasColumnName("fbTwoFactorEnabled");
            TbmUsers.Property(p => p.LockoutEndDateUtc).HasColumnName("fdLockoutEndDateUtc");
            TbmUsers.Property(p => p.LockoutEnabled).HasColumnName("fbLockoutEnabled");
            TbmUsers.Property(p => p.AccessFailedCount).HasColumnName("fnAccessFailedCount");

            builder.Entity<ApplicationUser>().ToTable("tbmUSERS", "dbo").Property(u => u.fdCREATED_DATE).IsRequired();
            builder.Entity<ApplicationUser>().ToTable("tbmUSERS", "dbo").Property(u => u.fsCREATED_BY).IsRequired().HasMaxLength(128);
            builder.Entity<ApplicationUser>().ToTable("tbmUSERS", "dbo").Property(u => u.fsUPDATED_BY).HasMaxLength(128);

            //builder.Entity<ApplicationUser>().HasMany(u => u.Roles);

            //AspNetRoles
            var TbmGroups = builder.Entity<IdentityRole>().HasKey(r => r.Id).ToTable("tbmGROUPS", "dbo");
            TbmGroups.Property(p => p.Id).HasColumnName("fsGROUP_ID");
            TbmGroups.Property(p => p.Name).HasColumnName("fsNAME").IsRequired().HasMaxLength(50);

            builder.Entity<ApplicationRole>().ToTable("tbmGROUPS", "dbo").Property(r => r.fsDESCRIPTION).IsOptional().HasMaxLength(512);
            builder.Entity<ApplicationRole>().ToTable("tbmGROUPS","dbo").Property(r => r.fsTYPE).IsRequired().HasMaxLength(1);
            builder.Entity<ApplicationRole>().ToTable("tbmGROUPS", "dbo").Property(r => r.fdCREATED_DATE).IsRequired();
            builder.Entity<ApplicationRole>().ToTable("tbmGROUPS", "dbo").Property(r => r.fsCREATED_BY).IsRequired().HasMaxLength(128);
            builder.Entity<ApplicationRole>().ToTable("tbmGROUPS", "dbo").Property(r => r.fsUPDATED_BY).HasMaxLength(128);

            //AspNetUserLogins
            var login = builder.Entity<IdentityUserLogin>().HasKey(iul => new { iul.UserId, iul.LoginProvider, iul.ProviderKey }).ToTable("tbmUserLogins", "dbo");
            login.Property(i => i.UserId).HasColumnName("UserId");
            login.Property(i => i.LoginProvider).HasColumnName("LoginProvider");
            login.Property(i => i.ProviderKey).HasColumnName("ProviderKey");

            //AspNetUserClaims
            //builder.Entity<IdentityUserClaim>().ToTable("tbmUserClaims");//.HasRequired<IdentityUser>(u => u.User);
            var claim = builder.Entity<IdentityUserClaim>().HasKey(iuc => iuc.Id).ToTable("tbmUserClaims", "dbo");
            claim.Property(c => c.Id).HasColumnName("Id");
            claim.Property(c => c.ClaimType).HasColumnName("ClaimType");
            claim.Property(c => c.ClaimValue).HasColumnName("ClaimValue");
            claim.Property(c => c.UserId).HasColumnName("UserId");

            //AspNetUserRoles
            //builder.Entity<IdentityUserRole>().HasKey(r => new { r.UserId, r.RoleId }).ToTable("tbmUSER_GROUP", "dbo");
            var userRole = builder.Entity<IdentityUserRole>().HasKey(iur => new { iur.UserId, iur.RoleId }).ToTable("tbmUSER_GROUP", "dbo");
            userRole.Property(p => p.UserId).HasColumnName("fsUSER_ID").HasMaxLength(128);
            userRole.Property(p => p.RoleId).HasColumnName("fsGROUP_ID").HasMaxLength(128);

            builder.Entity<ApplicationUserRole>().ToTable("tbmUSER_GROUP", "dbo").Property(r => r.fdCREATED_DATE).IsRequired();
            builder.Entity<ApplicationUserRole>().ToTable("tbmUSER_GROUP", "dbo").Property(r => r.fsCREATED_BY).IsRequired().HasMaxLength(128);
            builder.Entity<ApplicationUserRole>().ToTable("tbmUSER_GROUP", "dbo").Property(r => r.fsUPDATED_BY).IsOptional().HasMaxLength(128);

            builder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
        }
    }

}