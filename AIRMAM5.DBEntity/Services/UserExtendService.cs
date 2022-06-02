using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using AIRMAM5.Utility.Extensions;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 使用者帳號額外資訊資料表 tbmUSER_EXTEND
    /// </summary>
    public class UserExtendService : IGenericInterface<tbmUSER_EXTEND>
    {
        readonly SerilogService _serilogService;
        //protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "fail");
        readonly IGenericRepository<tbmUSER_EXTEND> _userExtendRepository = new GenericRepository<tbmUSER_EXTEND>();
        public static string CurrentUserId;
        private string _currentUserId = string.Empty;

        public UserExtendService()
        {
            _serilogService = new SerilogService();
            //this._db = new AIRMAM5DBEntities();
            this._currentUserId = CurrentUserId;
        }

        #region --- Interface ---
        ///// <summary>
        ///// non-use ✘✘✘
        ///// </summary>
        ///// <returns></returns>
        //public bool IsExists(int IndexId)
        //{
        //    throw new NotImplementedException();
        //}
        /// <summary>
        /// 使用者帳號是否存在 UserExtend
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool IsExists(string userid)
        {
            return _userExtendRepository.FindBy(x => x.fsUSER_ID == userid).Any();
        }

        /// <summary>
        /// tbmUSER_EXTEND 全部資料
        /// </summary>
        /// <returns></returns>
        public List<tbmUSER_EXTEND> GetAll()
        {
            var query = _userExtendRepository.GetAll();
            if (query == null || query.FirstOrDefault() == null)
                return new List<tbmUSER_EXTEND>();

            return query.ToList();
        }

        /// <summary>
        /// non-use ✘✘✘
        /// </summary>
        /// <param name="IndexId"></param>
        /// <returns></returns>
        public tbmUSER_EXTEND GetById(int IndexId)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 依UserId(fsUSER_ID) 查使用者資料(dbo.tbmUSER_EXTEND)
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public tbmUSER_EXTEND GetById(string userid)
        {
            var query = _userExtendRepository.FindBy(x => x.fsUSER_ID == userid);
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            return null;
        }
        #endregion

        #region---【CURD】----------------
        /// <summary>
        /// Create tbmUSER_EXTEND
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult Create(tbmUSER_EXTEND rec)
        {
            if (rec == null)
            {
                result.IsSuccess = false ;
                result.Message = "[tbmUSER_EXTEND]新增失敗";
                return result;
            }

            _userExtendRepository.Create(rec);
            result.IsSuccess = true;
            result.Message = "[tbmUSER_EXTEND]新增完成";
            return result;
        }

        /// <summary>
        /// CreateRange tbmUSER_EXTEND //TODO: 未實作內容。
        /// </summary>
        /// <param name="ranges"></param>
        /// <returns></returns>
        public VerifyResult CreateRange(List<tbmUSER_EXTEND> ranges)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update tbmUSER_EXTEND
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult Update(tbmUSER_EXTEND rec)
        {
            result = new VerifyResult { IsSuccess = false, Message = "[tbmUSER_EXTEND]更新: fail" };

            if (rec != null)
            {
                _currentUserId = HttpContext.Current.User.Identity.GetUserId();

                var get = _userExtendRepository.FindBy(x => x.fsUSER_ID == rec.fsUSER_ID);
                if (get.Any())
                {
                    var r = get.First();
                    r.fsSIGNALR_CONNECT_ID = rec.fsSIGNALR_CONNECT_ID;
                    r.fdUPDATED_DATE = DateTime.Now;
                    r.fsUPDATED_BY = _currentUserId;
                    _userExtendRepository.Update(rec);
                    result.IsSuccess = true;
                    result.Message = "[tbmUSER_EXTEND]更新完成";
                }
                else
                {
                    result.IsSuccess = true;
                    result.Message = "[tbmUSER_EXTEND]無符合資料";
                }
            }

            return result;
        }

        /// <summary>
        /// UpdateRange tbmUSER_EXTEND  //TODO: 未實作內容。
        /// </summary>
        /// <param name="ranges"></param>
        /// <returns></returns>
        public VerifyResult UpdateRange(List<tbmUSER_EXTEND> ranges)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete tbmUSER_EXTEND  //TODO: 未實作內容。
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult Delete(tbmUSER_EXTEND rec)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// RemoveRange tbmUSER_EXTEND  //TODO: 未實作內容。
        /// </summary>
        /// <param name="ranges"></param>
        /// <returns></returns>
        public VerifyResult RemoveRange(List<tbmUSER_EXTEND> ranges)
        {
            throw new NotImplementedException();
        }
        #endregion

        //----------------------------------
        ///// <summary>
        ///// 依UserId(fsUSER_ID) 查使用者資料(dbo.tbmUSERS)
        ///// </summary>
        ///// <param name="username"></param>
        ///// <returns></returns>
        //public tbmUSER_EXTEND FindByUserId(string userid)
        //{
        //    var query = _userExtendRepository.FindBy(x => x.fsUSER_ID == userid);
        //    if (query.Any())
        //    {
        //        return query.FirstOrDefault();
        //    }
        //    return null;
        //}

        /// <summary>
        /// 依使用者 產生驗證碼, Update dbo.tbmUSER_EXTEND
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="len">驗證通長度 </param>
        public VerifyResult GetVerifyCodeAndUpdate(string userid, int len)
        {
            string _verifyCode = StringExtensions.GenerateRandomStr(len);

            try
            {
                _currentUserId = HttpContext.Current.User.Identity.GetUserId();

                if (this.IsExists(userid))
                {
                    var _urex = this.GetById(userid);//.FindByUserId(userid);
                    _urex.fsVerifyCode = _verifyCode;
                    _urex.fdVerifyDate = null;
                    _urex.fdUPDATED_DATE = DateTime.Now;
                    _urex.fsUPDATED_BY = _currentUserId;

                    result = this.Update(_urex);
                }
                else
                {  //Insert
                    tbmUSER_EXTEND _create = new tbmUSER_EXTEND
                    {
                        fsUSER_ID = userid,
                        fsSIGNALR_CONNECT_ID = string.Empty,
                        fsVerifyCode = _verifyCode,
                        fsCREATED_BY = _currentUserId,
                        fdCREATED_DATE = DateTime.Now
                    };
                    result = this.Create(_create);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.ErrorException = ex;
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "UserExtendService",
                    Method = "[GetVerifyCodeAndUpdate]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = new { userid }, Result = result },
                    LogString = "產生使用者驗證碼.Exception"
                });
                #endregion
            }

            result.Data = result.IsSuccess ? _verifyCode : string.Empty;
            return result;
        }
    }
}
