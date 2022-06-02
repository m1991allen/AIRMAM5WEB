// ============================================= 
// 描述: LOG資料介接 
// 記錄: <2016/08/31><David.Sin><新增本程式> 
// ============================================= 

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using AIRMAM5.FileUpload.Common;
using AIRMAM5.FileUpload.Models;

namespace AIRMAM5.FileUpload.Repositorys
{
    public class repL_LOG
    {
        /// <summary>L_LOG的新增函數by parameters</summary>
        public static string fnINSERT_L_LOG_BY_PARAMETERS(String _fsCODE_ID, String _fsCODE, String _fsPARAMETERS, String _fsNOTE, String _DATA_KEY, String _strUID)
        {
            string fsRESULT = string.Empty;

            Dictionary<string, string> dicParameters = new Dictionary<string, string>();

            dicParameters.Add("fsCODE_ID", _fsCODE_ID);
            dicParameters.Add("fsCODE", _fsCODE);
            dicParameters.Add("fsPARAMETERS", _fsPARAMETERS);
            dicParameters.Add("fsNOTE", _fsNOTE);
            dicParameters.Add("fsDATA_KEY", _DATA_KEY);
            dicParameters.Add("fsCREATED_BY", _strUID);

            fsRESULT = clsDB.Do_Tran("spINSERT_L_LOG_BY_PARAMETERS", dicParameters);

            return fsRESULT;
        }
        
    }
}