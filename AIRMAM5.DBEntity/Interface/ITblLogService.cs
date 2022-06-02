using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Shared;
using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Interface
{
    public interface ITblLogService
    {
        /// <summary>
        /// 指定條件 查詢 系統操作紀錄
        /// </summary>
        /// <param name="llogid"></param>
        /// <param name="sdate"></param>
        /// <param name="edate"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        List<spGET_L_LOG_BY_LOGID_DATES_LOGINID_Result> GetByParam(long llogid = 0, string sdate = "", string edate = "", string username = "");

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
        VerifyResult Insert_L_Log(string codeid, string code, string parameters, string note, string datakey, string creater);

    }
}
