using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Hub;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using AIRMAM5.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// tblHUBCONNECTION 使用者hub connection id
    /// </summary>
    public class TblHubConnectionService
    {
        readonly IGenericRepository<tblHUBCONNECTION> _hubConnRepository;
        readonly ISerilogService _serilogService;

        public TblHubConnectionService()
        {
            _hubConnRepository = new GenericRepository<tblHUBCONNECTION>();
            _serilogService = new SerilogService();
        }

        /// <summary>
        /// 使用者+連線Id 是否存在
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="connid"></param>
        /// <returns></returns>
        public bool IsExtist(string userid, string connid)
        {
            var query = _hubConnRepository.FindBy(x => x.fsUSER_ID == userid && x.fsHUB_CONNECTION_ID == connid);

            return query.Any();
        }

        /// <summary>
        /// 取得 使用者hub連線ID資料
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="connid"></param>
        /// <returns></returns>
        public IEnumerable<tblHUBCONNECTION> GetBy(string uid, string connid="")
        {
            bool isEmpty = string.IsNullOrWhiteSpace(connid);
            var query = _hubConnRepository
                .FindBy(x => x.fsUSER_ID == uid && (isEmpty ? true : x.fsHUB_CONNECTION_ID == connid));

            return query;
        }

        /// <summary>
        /// 取得 線上ConnectionID (不包括 逾期回報的id)
        /// </summary>
        public List<UserOnlineInfo> GetAllNoOverdue()
        {
            //Hub未回報逾期時間(分鐘)
            double.TryParse(ConfigurationManager.AppSettings["HubConnectionOverdueTime"].ToString(), out double _overdue);

            var tmp = new List<UserOnlineInfo>();
            DateTime dtime1 = DateTime.Now;

            _hubConnRepository.GetAll().ToList().ForEach(f =>
            {
                dtime1 = f.fdUPDATED_TIME ?? f.fdONLINEED_TIME;
                double diffMin = DateTimeExtensions.DiffDateTo(dtime1, DateTime.Now);
                if (diffMin < _overdue)
                {
                    tmp.Add(new UserOnlineInfo
                    {
                        UserId = f.fsUSER_ID,
                        SignalrConnectionId = f.fsHUB_CONNECTION_ID,
                        ConnectTime = f.fdONLINEED_TIME,
                        LoginId = f.fsUSERNAME,
                        LoginLogId = f.fnLOGIN_ID
                    });
                }
            });

            return tmp;
        }

        /// <summary>
        /// 取 指定角色群組的線上ConnectionID  (不包括 逾期回報的id)
        /// </summary>
        /// <param name="roles">角色群組ID(多筆;分隔)</param>
        /// <returns></returns>
        public List<UserOnlineInfo> GetAssignConnectID(string roles)
        {
            //Hub未回報逾期時間(分鐘)
            double.TryParse(ConfigurationManager.AppSettings["HubConnectionOverdueTime"].ToString(), out double _overdue);

            var tmp = new List<UserOnlineInfo>();
            DateTime dtime1 = DateTime.Now;

            using (var _db = new AIRMAM5DBEntities())
            {
                var query = (from g in _db.tbmGROUPS
                             join urg in _db.tbmUSER_GROUP on g.fsGROUP_ID equals urg.fsGROUP_ID
                             join ur in _db.tbmUSERS on urg.fsUSER_ID equals ur.fsUSER_ID
                             join hub in _db.tblHUBCONNECTION on ur.fsUSER_ID equals hub.fsUSER_ID
                             where roles.IndexOf(roles) > -1 
                             select hub).DefaultIfEmpty();

                query.ToList().ForEach(f =>
                {
                    dtime1 = f.fdUPDATED_TIME ?? f.fdONLINEED_TIME;
                    double diffMin = DateTimeExtensions.DiffDateTo(dtime1, DateTime.Now);
                    tmp.Add(
                     new UserOnlineInfo
                     {
                         UserId = f.fsUSER_ID,
                         SignalrConnectionId = f.fsHUB_CONNECTION_ID,
                         ConnectTime = f.fdONLINEED_TIME,
                         LoginId = f.fsUSERNAME,
                         LoginLogId = f.fnLOGIN_ID
                     });
                });
            }

            return tmp;
        }

        #region >>> CURD
        /// <summary>
        /// 新增 tblHUBCONNECTION 資料
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult Create(tblHUBCONNECTION rec)
        {
            VerifyResult result = new VerifyResult(false, "無效的資料內容");

            try
            {
                if (rec != null)
                {
                    _hubConnRepository.Create(rec);

                    result.IsSuccess = true;
                    result.Message = "使用者連線Id 新增成功";
                    result.Data = rec;
                }
            }
            catch (Exception ex)
            {
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TblHubConnectionService",
                    Method = "Create",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = rec, Result = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"使用者連線Id 新增失敗【{ex.Message}】 ")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 更新 tblHUBCONNECTION 資料
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult Update(tblHUBCONNECTION rec)
        {
            VerifyResult result = new VerifyResult(false, "無效的資料內容");

            try
            {
                if (rec != null)
                {
                    _hubConnRepository.Update(rec);

                    result.IsSuccess = true;
                    result.Message = "使用者連線Id 修改成功";
                    result.Data = rec;
                }
            }
            catch (Exception ex)
            {
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TblHubConnectionService",
                    Method = "Update",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = rec, Result = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"使用者連線Id 修改失敗【{ex.Message}】 ")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 移除 使用者+登入記錄編號 的連線id 資料。dbo.tblHUBCONNECTION
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="llogid"></param>
        /// <returns></returns>
        public void RemoveByLoginLogId(string uid, long llogid)
        {
            VerifyResult result = new VerifyResult(false, string.Empty);

            var query = this.GetBy(uid);
            if (query.Any())
            {
                var gets = query.Where(x => x.fnLOGIN_ID == llogid).ToList();
                if (gets != null && gets.FirstOrDefault() != null)
                {
                    _hubConnRepository.RemoveRange(gets);
                }

                // 移除久未回報的ConnectionID
                RemoveOverdue();
            }
        }

        /// <summary>
        /// 移除-固定時間-未回報的ConnectionID
        /// <para>config檔參數"HubConnectionOverdueTime"</para>
        /// </summary>
        public void RemoveOverdue()
        {
            //Hub未回報逾期時間(分鐘)
            //double.TryParse(ConfigurationManager.AppSettings["HubConnectionOverdueTime"].ToString(), out double _overdue);
            double.TryParse(Config.HubConnectOverdueTime, out double _overdue);

            // 移除久未回報的ConnectionID
            var tmp = new List<tblHUBCONNECTION>();
            DateTime dtime1 = DateTime.Now;

            _hubConnRepository.GetAll().ToList().ForEach(f =>
            {
                dtime1 = f.fdUPDATED_TIME ?? f.fdONLINEED_TIME;
                double diffMin = DateTimeExtensions.DiffDateTo(dtime1, DateTime.Now);
                if (diffMin >= _overdue)
                {
                    tmp.Add(f);
                }
            });

            if (tmp.Count() > 0)
            {
                _hubConnRepository.RemoveRange(tmp);
            }
        }
        #endregion

    }
}
