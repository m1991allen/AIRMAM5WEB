using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 系統操作記錄 dbo.tblLOG
    /// </summary>
    public class TblLogService : ITblLogService
    {
        protected AIRMAM5DBEntities _db;
        readonly ISerilogService _serilogService;

        public TblLogService()
        {
            //_db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }

        public TblLogService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        /// <summary>
        /// 指定條件 查詢 系統操作紀錄
        /// </summary>
        /// <param name="llogid"></param>
        /// <param name="sdate"></param>
        /// <param name="edate"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<spGET_L_LOG_BY_LOGID_DATES_LOGINID_Result> GetByParam(long llogid = 0, string sdate = "", string edate = "", string username = "")
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_L_LOG_BY_LOGID_DATES_LOGINID(llogid, sdate, edate, username).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null)
                {
                    query = new List<spGET_L_LOG_BY_LOGID_DATES_LOGINID_Result>();
                }

                return query;
            }
        }

        /// <summary>
        /// 新增 系統操作記錄 【spINSERT_L_LOG_BY_PARAMETERS】
        /// </summary>
        /// <param name="codeid"> tbzCODE 的 key fsCODE_ID : 'MSG001' </param>
        /// <param name="code"> tbzCODE 的 key fsCODE (MSG_ID)</param>
        /// <param name="parameters"> 參數 </param>
        /// <param name="note"> 備註 </param>
        /// <param name="datakey"></param>
        /// <param name="creater"></param>
        /// <returns></returns>
        public VerifyResult Insert_L_Log(string codeid , string code, string parameters, string note, string datakey, string creater)
        {
            VerifyResult result = new VerifyResult(true);
            using (_db = new AIRMAM5DBEntities())
            {
                var _exec = _db.spINSERT_L_LOG_BY_PARAMETERS(codeid, code, parameters, note, datakey, creater).ToString();

                if (_exec.IndexOf("ERROR") == -1)
                {
                    result = new VerifyResult(true, "操作紀錄新增成功!");
                }
                else
                {
                    result = new VerifyResult(false, "操作紀錄新增失敗【" + _exec.Split(':')[1] + "】");
                }

                return result;
            }
        }
    }
}
