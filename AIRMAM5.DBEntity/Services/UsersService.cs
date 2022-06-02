using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.User;
using AIRMAM5.DBEntity.Repository;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 使用者帳號資訊  tbmUSERS
    /// </summary>
    public class UsersService : IGenericInterface<tbmUSERS>
    {
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "fail");
        readonly ISerilogService _serilogService;
        //readonly ICodeService _tbzCodeService;
        readonly IGenericRepository<tbmUSERS> _usersRepository = new GenericRepository<tbmUSERS>();
        readonly IGenericRepository<tbmUSER_EXTEND> _userExtendRepository = new GenericRepository<tbmUSER_EXTEND>();

        public UsersService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        public UsersService()
        {
            _serilogService = new SerilogService();
            //this._db = new AIRMAM5DBEntities();
            //_tbzCodeService = new CodeService(_db);
        }

        #region --- Current User Info ---
        /// <summary>
        /// 目前使用者ID
        /// </summary>
        public string CurrentUID
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (!String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                    {
                        return this.FindUserByName(HttpContext.Current.User.Identity.Name).fsUSER_ID;
                    }
                }
                return string.Empty;
            }
            //set { value = _CurrentUID; }
        }
        /// <summary>
        /// 是否為 系統管理員角色
        /// </summary>
        public bool CurrentUserIsAdmin
        {
            get
            {
                return (HttpContext.Current.User.IsInRole("Administrator") || HttpContext.Current.User.IsInRole("系統管理員"));
            }
        }
        /// <summary>
        /// 是否為 媒資管理員 角色
        /// </summary>
        public bool CurrentUserIsMediaManager
        {
            get
            {
                return (HttpContext.Current.User.IsInRole("MediaManager"));
            }
        }
        /// <summary>
        /// 系統管理員OR媒資管理員
        /// </summary>
        public bool IsAdminOrMediaManager
        {
            get
            {
                return (HttpContext.Current.User.IsInRole("Administrator") || HttpContext.Current.User.IsInRole("MediaManager"));
            }
        }
        /// <summary>
        /// 目前使用者可用的目錄節點權限(ex: 1899;1873;)
        /// </summary>
        public string CurrentUserDirAuth
        {
            get
            {
                var users = _usersRepository.FindBy(x => x.fsUSER_ID == CurrentUID).ToList();
                if (users.Any())
                {
                    string loginID = users.FirstOrDefault().fsLOGIN_ID;
                    using (_db = new AIRMAM5DBEntities())
                    {
                        IQueryable<tbmUSER_DIR> dirauth = _db.tbmUSER_DIR.Where(x => x.fsLOGIN_ID == loginID);
                        return dirauth.Any() ? dirauth.FirstOrDefault().fsDIR_LIST : string.Empty;
                    }
                }
                return string.Empty;
            }
        }
        #endregion

        /// <summary>
        /// non-use ✘✘✘
        /// </summary>
        /// <returns></returns>
        public bool IsExists(int IndexId)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 帳號(fsLOGIN_ID) OR fsUSER_ID 是否存在
        /// </summary>
        /// <param name="IndexStr"></param>
        /// <returns></returns>
        public bool IsExists(string IndexStr)
        {
            var query = _usersRepository.FindBy(x => x.fsLOGIN_ID == IndexStr || x.fsUSER_ID == IndexStr);
            
            return query.Any();
        }
        /// <summary>
        /// tbmUSERS 全部資料
        /// </summary>
        /// <returns></returns>
        public List<tbmUSERS> GetAll()
        {
            var query = _usersRepository.GetAll();
            if (query == null || query.FirstOrDefault() == null)
                return new List<tbmUSERS>();

            return query.ToList();
        }

        /// <summary>
        /// non-use ✘✘✘
        /// </summary>
        /// <param name="IndexId"></param>
        /// <returns></returns>
        public tbmUSERS GetById(int IndexId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 依User_ID / fsLOGIN_ID 取得使用者資料
        /// </summary>
        /// <param name="IndexStr"></param>
        /// <returns></returns>
        public tbmUSERS GetById(string IndexStr)
        {
            var query = _usersRepository.FindBy(x => x.fsUSER_ID == IndexStr || x.fsLOGIN_ID == IndexStr);
            if (query.Any())
                return query.FirstOrDefault();

            return null;
        }

        //--------------------------------------------------------(↑Interface)

        /// <summary>
        /// 登入時新增/更新 使用者有權限的DIR  【spINSERT_USER_DIR_AUTH】
        /// </summary>
        public void UpdateUserDirAuthr()
        {
            var _ur = this.GetById(CurrentUID);
            if (_ur == null) return;

            using (_db = new AIRMAM5DBEntities())
            {
                var _exec = _db.spINSERT_USER_DIR_AUTH(_ur.fsLOGIN_ID).FirstOrDefault();

                if (_exec.IndexOf("ERROR") == -1)
                {
                    result.IsSuccess = true;
                    result.Message = string.Format($"使用者({_ur.fsLOGIN_ID}) 已更新可使用的DIR權限.");
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"使用者({_ur.fsLOGIN_ID}) 更新可使用的DIR權限失敗【{_exec.Split(':')[1]}】");
                }
            }

            #region Serilog.Verbose
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "TbmUsersService",
                Method = "[UpdateUserDirAuthr]",
                EventLevel = SerilogLevelEnum.Verbose,
                Input = result,
                LogString = "Result",
                ErrorMessage = "登入時新增/更新 使用者有權限的DIR"
            });
            #endregion
        }

        /// <summary>
        /// 電子郵件(fsEMAIL)是否存在
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool ExistsUserEmail(string email)
        {
            var query = _usersRepository.FindBy(x => x.fsEMAIL == email);
            //if (query.Any()) { return true; }

            return query.Any();
        }

        /// <summary>
        /// 依UserName(fsLOGIN_ID) 查使用者 (Repository)
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public tbmUSERS FindUserByName(string username)
        {
            var query = _usersRepository.FindBy(x => x.fsLOGIN_ID == username);
            if (query.Any())
            {
                return query.FirstOrDefault();
            }

            return null;
        }

        #region --- Procedure ---
        /// <summary>
        /// 指定條件(userid, loginid, name) 取回 使用者帳號資料. 全部不指定,回傳全部資料。 【spGET_USERS】
        /// </summary>
        /// <param name="userid"> Id </param>
        /// <param name="loginid"> 帳號 </param>
        /// <param name="name"> 顯示名稱/姓名 </param>
        /// <returns></returns>
        //public List<UserDetailViewModel> GetBy(string userid = "", string loginid = "", string name = "")
        public List<spGET_USERS_Result> GetBy(string userid = "", string loginid = "", string name = "")
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_USERS(userid, loginid, name).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null) query = new List<spGET_USERS_Result>();

                return query;
            }
        }

        /// <summary>
        /// 目前or指定 使用者的基本資料(群組/角色清單) 【spGET_USERS】
        /// </summary>
        /// <param name="userid">UserId, 預設=null依目前使用者userid查詢</param>
        /// <returns></returns>
        public UserLoginInfoViewModel GetUserInfo(string userid = null)
        {
            userid = userid ?? CurrentUID;
            var get = this.GetBy(userid, string.Empty, string.Empty)
                .Select(s => new UserLoginInfoViewModel
                {
                    fsUSER_ID = s.fsUSER_ID,
                    fsLOGIN_ID = s.fsLOGIN_ID,
                    RealName = s.fsNAME,
                    Email = s.fsEMAIL,
                    DeptId = s.fsDEPT_ID,
                    DeptName = s.C_sDEPTNAME,
                }).FirstOrDefault();

            if (get == null) return new UserLoginInfoViewModel();

            //使用者:角色 = 1對多
            using (_db = new AIRMAM5DBEntities())
            {
                var role = _db.spGET_USERS_GROUP_BY_USER_ID(userid)//.Select(s => s.fsNAME).ToList();
                        .Select(s => s.fsDESCRIPTION)
                        .ToList();  //Tips: 顯示角色中文說明(fsNAME為英文)

                if (role.Count() > 0) get.UserRoles = role;
            }

            return get;
        }
        #endregion

        #region ---使用者帳號 下拉清單---
        /// <summary>
        /// 使用者帳號 SelectListItem 
        /// <para> ✘✘系統管理者會有"全部"項目；非系統管理員,僅顯示自己的資料。 </para>
        /// <para> 指定參數 getall 是否需要 "全部"項目 </para>
        /// </summary>
        /// <param name="selectvalue">指定"fsUSER_ID" 為選取, 預設空白不指定</param>
        /// <param name="getall">取回全部使用者資訊, 預設false </param>
        /// <returns></returns>
        public List<SelectListItem> GetUsersList(string selectvalue = "", bool getall = false)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if(getall)
            {
                items.AddRange(new List<SelectListItem>
                {
                    new SelectListItem { Text = "- 全部 -", Value = "*" }
                });
            }
            var query = _usersRepository.FindBy(x => (this.CurrentUserIsAdmin || getall) ? true : x.fsUSER_ID == this.CurrentUID);
            if (query.Any())
            {
                items.AddRange(query.AsEnumerable()
                    .Select(s => new SelectListItem
                    {
                        Value = s.fsUSER_ID,
                        Text = string.Format($"{s.fsLOGIN_ID} {s.fsNAME}"),
                        Selected = (s.fsUSER_ID == selectvalue) ? true : false
                    }).ToList());
            }
            return items;
        }
        
        /// <summary>
        /// 依 系統目錄取出未設定過權限的帳號 dbo.spGET_USERS_NOTIN_DIR_USER_BY_DIR_ID
        /// </summary>
        /// <param name="dirid">系統目錄編號 fnDIR_ID </param>
        /// <param name="username">使用者帳號 fsLOGIN_ID </param>
        /// <param name="useridValue">下拉清單的[value]值的來源設定。預設/True: 取UserId。 false: 取LoginID(username) </param>
        /// <returns></returns>
        public List<SelectListItem> GetRolesByDirId(long dirid, string username, bool useridValue = true)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_USERS_NOTIN_DIR_USER_BY_DIR_ID(dirid, username).DefaultIfEmpty()
                .Select(s => new SelectListItem
                {
                    Value = useridValue ? s.fsUSER_ID : s.fsLOGIN_ID,
                    Text = string.Format($"{s.fsLOGIN_ID} {s.fsNAME}")
                }).ToList();

                return query;
            }
        }
        #endregion

        #region------------------------------------------【CURD】----------------
        /// <summary>
        /// Create tbmUSERS //TODO: 還要再確認實作內容。
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult Create(tbmUSERS rec)
        {
            result.Message = "無效的資料內容";
            if (rec == null) return result;

            try
            {
                _usersRepository.Create(rec);

                result.IsSuccess = true;
                result.Message = string.Format($"使用者帳號({rec.fsLOGIN_ID})已新增.");

                // -- Execute dbo.spINSERT_USERS -- ✘✘✘
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"使用者帳號({rec.fsLOGIN_ID})新增失敗. {ex.Message}");
                #region Serilog.ERR
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TbmUsersService",
                    Method = "[Create]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = rec, exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"使用者帳號({rec.fsLOGIN_ID})新增異常. {ex.Message}")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// CreateRange tbmUSERS //TODO: 未實作內容。
        /// </summary>
        /// <param name="ranges"></param>
        /// <returns></returns>
        public VerifyResult CreateRange(List<tbmUSERS> ranges)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Update tbmUSERS 
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult Update(tbmUSERS rec)
        {
            result.Message = "無效的資料內容";
            if (rec == null) return result;

            using (_db = new AIRMAM5DBEntities())
            {
                using (var _trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        // ✘✘--- Execute dbo.spUPDATE_USERS 
                        #region (1)__Update dbo.tbmUSER_GROUP__
                        if (!string.IsNullOrEmpty(rec.GroupIds))
                        {
                            var remove = _db.tbmUSER_GROUP.Where(x => x.fsUSER_ID == rec.fsUSER_ID);
                            _db.tbmUSER_GROUP.RemoveRange(remove);
                            _db.SaveChanges();

                            var upds = rec.GroupIds.Split(new char[] { ';' }).Distinct()
                                .Select(s => new tbmUSER_GROUP
                                {
                                    fsUSER_ID = rec.fsUSER_ID,
                                    fsGROUP_ID = s,
                                    fdCREATED_DATE = DateTime.Now,
                                    fsCREATED_BY = rec.fsUPDATED_BY
                                });
                            _db.tbmUSER_GROUP.AddRange(upds);
                        }
                        #endregion

                        //  (2)__Updaet dbo.tbmUSER__
                        _usersRepository.Update(rec);

                        _db.SaveChanges();
                        _trans.Commit();

                        result.IsSuccess = true;
                        result.Message = string.Format($"帳號({rec.fsLOGIN_ID})資料已修改.");
                        result.Data = GetBy(rec.fsUSER_ID, rec.fsLOGIN_ID)
                            .Select(s => new UserListViewModel().FormatConversion(s))
                            .FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        _trans.Rollback();
                        result.IsSuccess = false;
                        result.Message = string.Format($"帳號({rec.fsLOGIN_ID})更新失敗. {ex.Message}");
                        #region Serilog.Err
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "TbmUsersService",
                            Method = "[Update]",
                            EventLevel = SerilogLevelEnum.Error,
                            Input = new { param = rec, exception = ex },
                            LogString = "Exception",
                            ErrorMessage = string.Format($"帳號({rec.fsLOGIN_ID})更新發生異常. {ex.Message}")
                        });
                        #endregion
                    }
                    finally
                    {
                        _trans.Dispose();
                    }
                }
            }

            return result;
        }
        /// <summary>
        ///  UpdateRange tbmUSERS //TODO: 未實作內容。
        /// </summary>
        /// <param name="ranges"></param>
        /// <returns></returns>
        public VerifyResult UpdateRange(List<tbmUSERS> ranges)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  RemoveRange tbmUSERS //TODO: 未實作內容。
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult Delete(tbmUSERS rec)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  RemoveRange tbmUSERS //TODO: 未實作內容。
        /// </summary>
        /// <param name="ranges"></param>
        /// <returns></returns>
        public VerifyResult RemoveRange(List<tbmUSERS> ranges)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region --- 還原帳號密碼 ---
        /// <summary>
        /// 還原帳號密碼
        /// <para> (1)[tbmUSERS]密碼還原預設值= 帳號。
        /// <br>  2020.03.12 : 流程與"忘記密碼"相同, 項目(1)取消、只執行項目(2)。</br>
        /// <br>  2020.08.06 : 恢復項目(1)，預設密碼以config為主。</br>
        /// </para>
        /// <para>　　(2)[tbmUSER_EXTEND]註記使用者目前「還原密碼中」。 </para>
        /// </summary>
        /// <param name="userid">使用者id </param>
        /// <param name="updateby">更新使用者id </param>
        /// <param name="def"> +20200806+密碼還原預設值,未指定則以帳號為預設值。</param>
        public VerifyResult RestorePwdUpdate(string userid, string updateby, string def = "")
        {
            using (_db = new AIRMAM5DBEntities())
            {
                using (var _trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        //var query = _usersRepository.FindBy(x => x.fsUSER_ID == userid);
                        var query = _db.tbmUSERS.Where(x=>x.fsUSER_ID == userid);
                        if (query.Any() && query.FirstOrDefault() != null)
                        {
                            tbmUSERS _ur = query.First();
                            string defps = string.IsNullOrWhiteSpace(def) ? _ur.fsLOGIN_ID.Trim() : def; //+20200806+

                            #region 還原成預設密碼: dbo.tbmUSERS
                            PasswordHasher hasher = new PasswordHasher();
                            string _defPwd = hasher.HashPassword(defps).ToString();

                            _ur.fsPASSWORD = _defPwd;
                            _ur.fdUPDATED_DATE = DateTime.Now;
                            _ur.fsUPDATED_BY = updateby;
                            //_usersRepository.Update(_ur);
                            _db.Entry(_ur).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                            #endregion

                            #region 註記「還原密碼中」: dbo.tbmUSER_EXTEND
                            //var _urex = _ur.tbmUSER_EXTEND;
                            var _urex = _db.tbmUSER_EXTEND.Where(x=>x.fsUSER_ID == userid).FirstOrDefault();
                            if (_urex != null)
                            {
                                _urex.fsVerifyCode = string.IsNullOrWhiteSpace(def) ? _urex.fsVerifyCode : def; //+20200806+
                                _urex.fdVerifyDate = null;              //+20200806+這裡不是採用隨機碼產生
                                _urex.fbPWD_RESTORE = true;             //註記使用者帳號目前「還原密碼中」
                                _urex.fsRESTORE_BY = updateby;          //註記最後執行「還原密碼」的操作使用者
                                _urex.fdRESTORE_DATE = DateTime.Now;    //註記最後執行「還原密碼」時間
                                _urex.fdUPDATED_DATE = DateTime.Now;
                                _urex.fsUPDATED_BY = updateby;

                                //_userExtendRepository.Update(_urex);
                                _db.Entry(_urex).State = System.Data.Entity.EntityState.Modified;
                                _db.SaveChanges();
                            }
                            #endregion

                            _trans.Commit();
                            result.IsSuccess = true;
                            result.Message = string.Format($"{_ur.fsLOGIN_ID} 還原密碼中!");
                            result.Data = new { };
                        }
                    }
                    catch (Exception ex)
                    {
                        _trans.Rollback();
                        result.IsSuccess = false;
                        result.Message = string.Format($"密碼還原記錄失敗. {ex.Message}");
                        result.Data = new { };
                        #region Serilog.Err
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "TbmUsersService",
                            Method = "RestorePwd",
                            EventLevel = SerilogLevelEnum.Error,
                            Input = new { Param = new { userid, updateby }, Exception = ex },
                            LogString = "Exception",
                            ErrorMessage = string.Format($"密碼還原失敗. {ex.Message}")
                        });
                        #endregion
                    }
                    finally
                    {
                        _trans.Dispose();
                    }
                }
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 電子郵件信箱驗證 資料update
        /// </summary>
        /// <param name="ur"></param>
        /// <param name="urex"></param>
        /// <returns></returns>
        public VerifyResult ConfirmEmailUpdate(tbmUSERS ur, tbmUSER_EXTEND urex)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                using (var _trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        _usersRepository.Update(ur);
                        _userExtendRepository.Update(urex);

                        _trans.Commit();
                        result.IsSuccess = true;
                        result.Message = "電子郵件信箱驗證 完成!";
                        result.Data = new { };
                    }
                    catch (Exception ex)
                    {
                        _trans.Rollback();
                        result.IsSuccess = false;
                        result.Message = string.Format($"電子郵件信箱驗證失敗. {ex.Message}");
                        result.Data = new { };
                        #region Serilog.Err
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "TbmUsersService",
                            Method = "ConfirmEmailUpdate",
                            EventLevel = SerilogLevelEnum.Error,
                            Input = new { Param = new { ur, urex }, Exception = ex },
                            LogString = "Exception",
                            ErrorMessage = string.Format($"電子郵件信箱驗證失敗. {ex.Message}")
                        });
                        #endregion
                        throw ex;
                    }
                    finally
                    {
                        _trans.Dispose();
                    }
                }
            }

            return result;
        }

        #region --- LDAP驗證 ---
        /// <summary>
        /// LDAP驗證
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public bool LDAPAuth(string username, string pwd)
        {
            var query = new ConfigService().GetConfigBy("LDAP_ADDRESS");
            if (query != null)
            {
                var config = query.FirstOrDefault();
                if (config == null) return false;

                string[] strLDAP = config.fsVALUE.Trim().Split(';');
                foreach (string val in strLDAP)
                {
                    try
                    {
                        DirectoryEntry entry = new DirectoryEntry(
                            val,
                            string.Format($"uid={username},ou=people,dc=ftv,dc=com,dc=tw"),
                            pwd,
                            AuthenticationTypes.ServerBind);

                        object obj = entry.NativeObject;
                        result.IsSuccess = true;
                        result.Message = string.Empty;
                        result.Data = obj;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        #region Serilog.Err
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "UsersService",
                            Method = "[LDAPAuth]",
                            EventLevel = SerilogLevelEnum.Error,
                            Input = new { Param = new { username, pwd }, LDAPValue = val },
                            LogString = "Exception",
                            ErrorMessage = string.Format($"LDAP驗證. {ex.Message}")
                        });
                        #endregion
                        //LDAP 登入失敗
                        continue;
                    }
                }
            }

            return false;
        }
        #endregion
    }
}
