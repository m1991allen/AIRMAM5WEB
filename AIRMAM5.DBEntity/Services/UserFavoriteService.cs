using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Function;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 使用者帳號 我的最愛紀錄
    /// </summary>
    public class UserFavoriteService
    {
        readonly ISerilogService _serilogService;
        //protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "無效的資料內容");
        readonly IGenericRepository<tbmUSER_FAVORITE> _userFavoriteRepository = new GenericRepository<tbmUSER_FAVORITE>();

        public UserFavoriteService()
        {
            _serilogService = new SerilogService();
            //this._db = new AIRMAM5DBEntities();
        }

        /// <summary>
        /// 取回 使用者 我的最愛 [tbmUSER_FAVORITE]
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<tbmUSER_FAVORITE> GetByUserId(string userid)
        {
            var query = _userFavoriteRepository.FindBy(x => x.fsUSER_ID == userid).ToList();

            return query ?? new List<tbmUSER_FAVORITE>();
        }

        /// <summary>
        /// 使用者ID IsExists [tbmUSER_FAVORITE]
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="funcid"></param>
        /// <returns></returns>
        public bool IsExists(string userid, string funcid)
        {
            bool result = false;
            var query = _userFavoriteRepository.FindBy(x => x.fsUSER_ID == userid && x.fsFUNC_ID == funcid);
            if (query.Any()) result = true;

            return result;
        }

        /// <summary>
        /// 取回 使用者ID 我的最愛項目
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<UserFavoriteModel> GetUserFav(string userid)
        {
            List<UserFavoriteModel> query = new List<UserFavoriteModel>();
            try
            {
                using (var ent = new AIRMAM5DBEntities())
                {
                    var _urfav = new GenericRepository<tbmUSER_FAVORITE>(ent);
                    var _funs = new GenericRepository<tbmFUNCTIONS>(ent);

                    query = (from a in _urfav.FindBy(x => x.fsUSER_ID == userid).AsEnumerable()
                             join b in _funs.FindBy(x => x.fsTYPE != FunctionTypeEnum.X.ToString()).AsEnumerable() on a.fsFUNC_ID equals b.fsFUNC_ID
                             select new UserFavoriteModel
                             {
                                 FuncId = a.fsFUNC_ID,
                                 FunctionName = b.fsNAME,
                                 ControllerName = b.fsCONTROLLER,
                                 ActionName = b.fsACTION,
                                 FavoriteUrl = string.Format($"/AIRMAM5/{b.fsCONTROLLER}/{b.fsACTION}")
                             }).ToList();
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "UserFavoriteService",
                    Method = "[GetUserFav]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return query;
        }

        #region 【使用者帳號 我的最愛】新增/ 刪除
        /// <summary>
        /// Create 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult CreateBy(tbmUSER_FAVORITE rec)
        {
            if (rec == null) return result;

            try
            {
                _userFavoriteRepository.Create(rec);

                result = new VerifyResult(true, "新增完成")
                {
                    Data = rec
                };
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "UserFavoriteService",
                    Method = "[CreateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"我的最愛新增失敗. {ex.Message}")
                });
                #endregion
                result = new VerifyResult(false, string.Format($"我的最愛新增失敗. {ex.Message}"));
            }

            return result;
        }

        /// <summary>
        /// Delete 【EF Delete】: 單筆與多筆
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public VerifyResult DeleteBy(List<tbmUSER_FAVORITE> rec)
        {
            if (rec == null) return result;
            try
            {
                _userFavoriteRepository.RemoveRange(rec);
                result = new VerifyResult(true, "我的最愛已移除");
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "UserFavoriteService",
                    Method = "[DeleteBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"我的最愛移除失敗. {ex.Message}")
                });
                #endregion
                result = new VerifyResult(false, string.Format($"我的最愛移除失敗. {ex.Message}"));
            }

            return result;
        }
        #endregion

    }
}
