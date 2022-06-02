using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Function;
using AIRMAM5.DBEntity.Models.Role;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.User;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// tbmGROUPS 群組/角色資料
    /// </summary>
    public class GroupsService
    {
        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "fail");
        private readonly IGenericRepository<tbmGROUPS> _groupRepository = new GenericRepository<tbmGROUPS>();

        public GroupsService()
        {
            //this._db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }

        public GroupsService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        /// <summary>
        /// 依fsGROUP_ID 查群組/角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public tbmGROUPS FindById(string id)
        {
            var query = _groupRepository.FindBy(x => x.fsGROUP_ID == id);
            if (query.Any()) return query.FirstOrDefault();

            return null;
        }

        /// <summary>
        /// 角色群組(fsGROUP_ID)是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsExistsById(string id)
        {
            var query = _groupRepository.FindBy(x => x.fsGROUP_ID == id);
            if (query.Any()) { return true; }
            return false;
        }

        /// <summary>
        /// 角色群組名稱(fsNAME)是否存在
        /// </summary>
        /// <param name="nm"></param>
        /// <returns></returns>
        public bool IsExistsByName(string id, string nm)
        {
            var query = _groupRepository.FindBy(x => x.fsNAME.Equals(nm) && !x.fsGROUP_ID.Equals(id));
            if (query.Any()) { return true; }

            return false;
        }

        /// <summary>
        /// 系統群組角色 帳號數統計 【spGET_GROUPS】
        /// </summary>
        /// <param name="groupid">fsGROUP_ID, 不指定值會取回所有資料。</param>
        /// <returns></returns>
        public async Task<List<GroupsCounterModel>> RolesCounter(string groupid = "")
        {
            using (_db = new AIRMAM5DBEntities())
            {
                List<GroupsCounterModel> counters = _db.spGET_GROUPS(groupid).DefaultIfEmpty()
                    .Select(s => new GroupsCounterModel
                    {
                        RoleId = s.fsGROUP_ID,
                        RoleName = s.fsNAME,
                        Description = s.fsDESCRIPTION,
                        AccountCounts = s.fnUSER_COUNT
                    }).ToList();

                return await Task.Run(() => counters);
            }
        }

        /// <summary>
        /// 角色群組帳號資料 
        /// </summary>
        /// <param name="id"> 角色群組ID </param>
        public List<UserListViewModel> RoleAccount(string id)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = (from a in _db.tbmGROUPS
                             join b in _db.tbmUSER_GROUP on a.fsGROUP_ID equals b.fsGROUP_ID
                             join u in _db.tbmUSERS on b.fsUSER_ID equals u.fsUSER_ID
                             join d in _db.tbzCODE.Where(z => z.fsCODE_ID == TbzCodeIdEnum.DEPT001.ToString()) on u.fsDEPT_ID equals d.fsCODE into obj
                             from dd in obj.DefaultIfEmpty()
                             where a.fsGROUP_ID == id
                             select new UserListViewModel
                             {
                                 fsUSER_ID = u.fsUSER_ID,
                                 fsLOGIN_ID = u.fsLOGIN_ID,
                                 fsNAME = u.fsNAME ?? string.Empty,
                                 fsDEPT_ID = u.fsDEPT_ID,
                                 C_sDEPTNAME = dd.fsNAME ?? string.Empty,
                                 fsIS_ACTIVE = u.fsIS_ACTIVE ?? false,
                                 fsDESCRIPTION = u.fsDESCRIPTION ?? string.Empty,
                                 fsEMAIL = u.fsEMAIL ?? string.Empty,
                                 fsEmailConfirmed = u.fsEmailConfirmed == true ? true : false,
                                 EmailConfirmedStr = u.fsEmailConfirmed == true ? "已驗證" : "未驗證"
                             })
                             .Distinct()
                             .DefaultIfEmpty();

                return query.ToList();
            }
        }

        #region __CURD
        /// <summary>
        /// Create 新增  tbmGROUPS: 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult Create(tbmGROUPS rec)
        {
            try
            {
                if (rec == null) return result;
                var isExist = _groupRepository.FindBy(x => x.fsNAME == rec.fsNAME).Count()>0;
                if (isExist)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"群組資料已經存在【{rec.fsNAME}】");
                    result.Data = rec;
                    return result;
                }
                #region -- spINSERT_GROUPS --✘✘✗
                //string _exec = _db.spINSERT_GROUPS(
                //    rec.fsGROUP_ID,
                //    rec.fsNAME,
                //    rec.fsDESCRIPTION,
                //    rec.fsTYPE,
                //    rec.fsCREATED_BY).FirstOrDefault();

                //if (_exec.IndexOf("ERROR") == -1)
                //{
                //    result = new VerifyResult(true, "群組資料已建立.");
                //}
                //else
                //{
                //    result = new VerifyResult(false, "群組資料新增失敗【" + _exec.Split(':')[1] + "】");
                //}
                #endregion

                _groupRepository.Create(rec);

                result.IsSuccess = true;
                result.Message = string.Format($"群組資料新增成功【{rec.fsNAME}】");
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "GroupsService",
                    Method = "[Create]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"群組新增失敗. {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"群組資料新增失敗【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// Update 編輯更新 tbmGROUPS: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult Update(tbmGROUPS rec)
        {
            try
            {
                if (rec == null) return result;
                // -- spINSERT_GROUPS --✘✘✗

                _groupRepository.Update(rec);
                //result = new VerifyResult(true, string.Format($"群組({rec.fsNAME}) 資料已更新"))
                //{
                //    Data = new GroupsViewModel
                //    {
                //        Description = rec.fsDESCRIPTION,
                //        RoleId = rec.fsGROUP_ID,
                //        RoleName = rec.fsNAME
                //    }
                //};
                result.IsSuccess = true;
                result.Message = string.Format($"群組({rec.fsNAME}) 資料已更新");
                result.Data = new GroupsViewModel
                {
                    Description = rec.fsDESCRIPTION,
                    RoleId = rec.fsGROUP_ID,
                    RoleName = rec.fsNAME
                };
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "GroupsService",
                    Method = "[Update]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"群組更新失敗. {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"群組資料修改失敗【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// Delete (相關資料表資料要一併刪除) EXECUTE dbo.spDELETE_GROUPS
        /// </summary>
        /// <param name="groupid">群組/角色 fsGROUP_ID</param>
        /// <returns></returns>
        public VerifyResult Delete(string groupid, string deluser)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    #region -- spDELETE_GROUPS --
                    var _exec = _db.spDELETE_GROUPS(groupid, deluser).FirstOrDefault().ToString();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = string.Format($"群組資料已刪除");
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = string.Format($"群組資料刪除失敗【{_exec.Split(':')[1]}】");
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "GroupsService",
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"群組刪除失敗. {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"群組資料刪除失敗【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// 更新 角色可用功能項目資料 : EXECUTE dbo.spUPDATE_FUNC_GROUP
        /// </summary>
        /// <returns></returns>
        public VerifyResult UpdateRoleFuncs(RoleFuncUpdateModel rec)
        {
            if (rec == null) { return result; }

            #region _SP已修正:: spUPDATE_FUNC_GROUP
            //using (var trans = _db.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        #region 刪除原本可使用的功能項目
            //        var _rolefunc = _db.tbmFUNC_GROUP.Where(x => x.fsGROUP_ID == rec.RoleId);
            //        var _remove = _db.tbmFUNC_GROUP.RemoveRange(_rolefunc);
            //        _db.SaveChanges();
            //        #endregion

            //        #region 新建可使用的功能項目
            //        var _data = rec.FunctionIds.Select(s => new tbmFUNC_GROUP
            //        {
            //            fsFUNC_ID = s.FuncId,
            //            fsGROUP_ID = rec.RoleId,
            //            fsCREATED_BY = HttpContext.Current.User.Identity.Name,
            //            fdCREATED_DATE = DateTime.Now,
            //            fsUPDATED_BY = string.Empty
            //        }).ToList();
            //        var _inster = _db.tbmFUNC_GROUP.AddRange(_data);
            //        _db.SaveChanges();
            //        #endregion

            //        trans.Commit();
            //        result = new VerifyResult(true, "角色功能已更新.");
            //    }
            //    catch (Exception ex)
            //    {
            //        trans.Rollback();
            //        _serilogService.SerilogWriter(new SerilogInputModel
            //        {
            //            Controller = "GroupsService",
            //            Method = "UpdateRoleFuncs",
            //            EventLevel = SerilogLevelEnum.Error,
            //            Input = ex,
            //            LogString = "Exception",
            //            ErrorMessage = string.Format($"角色功能更新失敗. {ex.Message}")
            //        });
            //        result = new VerifyResult(false, ex.Message);
            //    }
            //    finally
            //    {
            //        trans.Dispose();
            //    }
            //}
            #endregion

            try
            {
                string _ids = string.Join(",", rec.FunctionIds.Select(s => s.FuncId).ToList());

                using (_db = new AIRMAM5DBEntities())
                {
                    #region -- spUPDATE_FUNC_GROUP --
                    var _exec = _db.spUPDATE_FUNC_GROUP(_ids, rec.RoleId, HttpContext.Current.User.Identity.Name).ToString();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = "角色功能已修改";
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = string.Format($"角色功能修改失敗【{_exec.Split(':')[1]}】");
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "GroupsService",
                    Method = "[UpdateRoleFuncs]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"角色功能更新失敗. {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"角色功能修改失敗【{ex.Message}】");
            }
            return result;
        }
        #endregion

        #region 【SelectListItem】 角色群組 下拉清單
        /// <summary>
        /// 傳入 多筆群組ID 符合的id 註記selected 回傳 SelectListItem 
        /// </summary>
        /// <param name="ids">使用者群組ids(多筆;分隔)</param>
        /// <param name="none">是否顯示"請選擇"項目,預設false</param>
        /// <returns></returns>
        public List<SelectListItem> GroupListItemSelected(string ids, bool none = false)
        {
            var idsList = ids.Split(new char[] { ';' });

            var query = _groupRepository.GetAll();
            if (query.Any())
            {
                var list = (from a in query.AsEnumerable()
                            select new SelectListItem
                            {
                                Value = a.fsGROUP_ID,
                                Text = string.Format($"{a.fsNAME} {a.fsDESCRIPTION}"),
                                Selected = ids.IndexOf(a.fsGROUP_ID, 0) > -1 ? true : false
                            }).ToList();

                if (none)
                {
                    if (string.IsNullOrEmpty(ids))
                    { list.Insert(0, new SelectListItem { Value = "", Text = " 全部 - ", Selected = true }); }
                    else { list.Insert(0, new SelectListItem { Value = "", Text = " 全部 - ", Selected = false }); }
                }

                return list;
            }

            return new List<SelectListItem>();
        }

        /// <summary>
        /// 群組角色 SelectListItem
        /// </summary>
        /// <param name="none">是否顯示"請選擇"項目,預設false</param>
        /// <returns></returns>
        public List<SelectListItem> GetUserRoles(bool none = false)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            var query = _groupRepository.GetAll();
            if (query.Any())
            {
                items = query.AsEnumerable()
                .Select(s => new SelectListItem
                {
                    Value = s.fsGROUP_ID,//s.fsNAME,
                    Text = string.Format($"{s.fsNAME} {s.fsDESCRIPTION}")
                }).ToList();
            }

            if (none) items.Insert(0, new SelectListItem { Text = " 請選擇- ", Value = "" });
            return items;
        }

        /// <summary>
        /// 依 系統目錄取出未設定過權限的角色群組 : EXECUTE dbo.spGET_GROUPS_NOTIN_DIR_GROUP_BY_DIR_ID
        /// </summary>
        /// <param name="dirid">系統目錄編號 fnDIR_ID </param>
        /// <returns></returns>
        public List<SelectListItem> GetRolesByDirId(long dirid)
        {
            List<SelectListItem> result = new List<SelectListItem>();

            using (_db =  new AIRMAM5DBEntities())
            {
                var query = _db.spGET_GROUPS_NOTIN_DIR_GROUP_BY_DIR_ID(dirid).DefaultIfEmpty().ToList();
                if (query != null && query.FirstOrDefault() != null)
                {
                    result = query.Select(s => new SelectListItem
                    {
                        Value = s.fsGROUP_ID,
                        Text = s.fsNAME
                    }).ToList();
                }

                return result;
            }
        }
        
        #endregion
    }
}