// ============================================= 
// 描述: WORK資料介接 
// 記錄: <2016/09/07><David.Sin><新增本程式> 
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
    public class repL_WORK
    {
        /// <summary>L_WORK的標準新增函數</summary>
        public string fnINSERT_L_WORK(clsL_WORK clsL_WORK)
        {
            string fsRESULT = string.Empty;
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();

            dicParameters.Add("fsTYPE", clsL_WORK.fsTYPE);
            dicParameters.Add("fsPARAMETERS", clsL_WORK.fsPARAMETERS);
            dicParameters.Add("fsSTATUS", clsL_WORK.fsSTATUS);
            dicParameters.Add("fsPROGRESS", clsL_WORK.fsPROGRESS);
            dicParameters.Add("fsPRIORITY", clsL_WORK.fsPRIORITY);
            dicParameters.Add("fsRESULT", clsL_WORK.fsRESULT);
            dicParameters.Add("fsNOTE", clsL_WORK.fsNOTE);
            dicParameters.Add("fsCREATED_BY", clsL_WORK.fsCREATED_BY);
            dicParameters.Add("_ITEM_ID", clsL_WORK._ITEM_ID);
            fsRESULT = clsDB.Do_Tran("spINSERT_L_WORK", dicParameters);

            return fsRESULT;
        }
        
    }
}