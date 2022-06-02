// ============================================= 
// 描述: repL_NO資料介接
// 記錄: <2016/11/04><David.Sin><新增本程式> 
// ============================================= 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AIRMAM5.FileUpload.Common;
using AIRMAM5.FileUpload.Models;
using System.Data;

namespace AIRMAM5.FileUpload.Repositorys
{
    public class repL_NO
    {
        /// <summary>L_NO的取出函數</summary>
        public string fnGET_L_NO(String _fsTYPE, String _fsNAME, String _fsHEAD, String _fsBODY, int _fsNO_L, String _strUID) //get的要另外傳入執行者
        {
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();
            string fsRESULT = "";

            dicParameters.Add("fsTYPE", _fsTYPE);
            dicParameters.Add("fsNAME", _fsNAME);
            dicParameters.Add("fsHEAD", _fsHEAD);
            dicParameters.Add("fsBODY", _fsBODY);
            dicParameters.Add("fsNO_L", _fsNO_L.ToString());
            dicParameters.Add("BY", _strUID.ToString());
            
            DataTable dtNO = clsDB.Do_Query("spGET_L_NO_NEW_NO", dicParameters);

            if (dtNO != null && dtNO.Rows.Count > 0)
            {
                fsRESULT = dtNO.Rows[0]["NEW_NO"].ToString();
            }

            return fsRESULT;
        }

    }
}