using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using AIRMAM5.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 登入記錄資料 dbo.tblLOGIN
    /// </summary>
    public class TblLoginService
    {
        protected AIRMAM5DBEntities _db;
        readonly ISerilogService _serilogService;
        readonly IGenericRepository<tblLOGIN> _tblLoginRepostory = new GenericRepository<tblLOGIN>();

        public TblLoginService()
        {
            //_db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }

        ///// <summary>
        ///// 測試資料
        ///// </summary>
        ///// <param name="rows">回傳資料筆數</param>
        ///// <returns></returns>
        //public List<spGET_L_LOGIN_Result> GetTempData(int rows)
        //{
        //    List<spGET_L_LOGIN_Result> result = new List<spGET_L_LOGIN_Result>();
        //    for (int i = 1; i < rows; i=i+2)
        //    {
        //        result.Add(new spGET_L_LOGIN_Result
        //        {
        //            fnLOGIN_ID = rows + i,
        //            fsLOGIN_ID = "user.ass",
        //            fdSTIME = DateTime.Now.AddMinutes(-85),
        //            //fdETIME = DateTime.Now.AddDays(-1).AddHours(+5),
        //            fsNOTE = "操作系統中",
        //            fdCREATED_DATE = DateTime.Now.AddMinutes(-85).AddSeconds(5),
        //            fsCREATED_BY = "user.ass",
        //            //fdUPDATED_DATE = DateTime.Now.AddDays(-1).AddHours(+5).AddSeconds(10),
        //            //fsUPDATED_BY = "user.ass",
        //            fsCREATED_BY_NAME = "助理",
        //            //fsUPDATED_BY_NAME = "助理"
        //        });
        //        result.Add(new spGET_L_LOGIN_Result
        //        {
        //            fnLOGIN_ID = rows + i + 1,
        //            fsLOGIN_ID = "lisa.shien",
        //            fdSTIME = DateTime.Now.AddDays(-1).AddHours(+3),
        //            fdETIME = DateTime.Now.AddDays(-1).AddHours(+5),
        //            fsNOTE = "已登出",
        //            fdCREATED_DATE = DateTime.Now.AddDays(-1).AddHours(+3).AddSeconds(5),
        //            fsCREATED_BY = "lisa.shien",
        //            fdUPDATED_DATE = DateTime.Now.AddDays(-1).AddHours(+5).AddSeconds(10),
        //            fsUPDATED_BY = "lisa.shien",
        //            fsCREATED_BY_NAME = "通用會員",
        //            fsUPDATED_BY_NAME = "通用會員"
        //        });
        //    }
        //    return result;
        //}

        /// <summary>
        /// 指定條件 查詢 使用登入紀錄
        /// </summary>
        /// <param name="username">使用者帳號</param>
        /// <param name="sdate">查詢起始日 yyyy/MM/dd</param>
        /// <param name="edate">查詢結束日 yyyy/MM/dd</param>
        /// <param name="loginlogid">登入登出紀錄編號 fnLOGIN_ID</param>
        /// <returns></returns>
        public List<spGET_L_LOGIN_ResultViewModel> GetByParam(string username, string sdate = "", string edate = "", long loginlogid = 0)
        {
            List<spGET_L_LOGIN_ResultViewModel> result = new List<spGET_L_LOGIN_ResultViewModel>();
            
            try
            {
                //var _ser = new UsersService(_serilogService);

                using (_db = new AIRMAM5DBEntities())
                {
                    result = _db.spGET_L_LOGIN(loginlogid, sdate, edate, username).DefaultIfEmpty()
                    .Select(s => new spGET_L_LOGIN_ResultViewModel(s)
                    {
                        UsageTime = DateTimeExtensions.DiffDate(s.fdSTIME, s.fdETIME)
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "tblLoginService",
                    Method = "GetByParam",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"登入紀錄查詢【{ex.Message}】")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 取得目前可能在線人數並修改狀態 spGET_L_LOGIN_ALIVE
        /// </summary>
        /// <param name="loginlogid"></param>
        /// <returns></returns>
        public List<spGET_L_LOGIN_ALIVE_Result> GetLoginAlive(long loginlogid = 0)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var get = _db.spGET_L_LOGIN_ALIVE(loginlogid).DefaultIfEmpty().ToList();

                return get;
            }
        }

        /// <summary>
        /// 新增 登入紀錄, 回傳fnLOGIN_ID.
        /// </summary>
        /// <param name="username">使用者帳號</param>
        /// <param name="sdate"></param>
        /// <param name="edate"></param>
        /// <param name="note"></param>
        public decimal CreateLogin(tblLOGIN rec)//(string username, DateTime? sdate, DateTime? edate, string note)
        {
            VerifyResult result = new VerifyResult();
            long loginLogid = -1;
            //string result = string.Empty;

            try
            {
                //spINSERT_L_LOGIN
                //loginLogid = _db.spINSERT_L_LOGIN(username, sdate, edate, note, username).FirstOrDefault();

                _tblLoginRepostory.Create(rec);
                result.IsSuccess = true;
                result.Message = "新增登入紀錄";
                result.Data = rec;
                loginLogid = rec.fnLOGIN_ID;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"新增登入紀錄 【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "tblLoginService",
                    Method = "[CreateLogin]",
                    EventLevel = SerilogLevelEnum.Error,
                    ErrorMessage = string.Format($"新增登入紀錄 【{ex.Message}】"),
                    Input = new { Param = rec, Exception = ex},
                    LogString = "Exception",
                });
                #endregion
            }

            return loginLogid;
        }

        /// <summary>
        /// 記錄 使用者帳號 登出時間
        /// </summary>
        /// <param name="loginlogid">登入登出紀錄編號 fnLOGIN_ID</param>
        /// <param name="username">使用者帳號</param>
        public int UpdateLogout(long loginLogid, string username)
        {
            int? rownum = -1;
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    rownum = _db.spUPDATE_L_LOGIN_BY_ETIME(loginLogid, username).FirstOrDefault();
                    //var _try = int.TryParse(result, out rownum);
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "tblLoginService",
                    Method = "UpdateLogout",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"記錄登出時間 【{ex.Message}】")
                });
                #endregion
            }

            return rownum ?? -1;
        }
    }
}
