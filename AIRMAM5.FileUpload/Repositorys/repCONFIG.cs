// ============================================= 
// 描述: Config資料介接 
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
    public class repCONFIG
    {
        /// <summary>CONFIG的標準取出函數</summary>
        public List<clsCONFIG> fnGET_CFG(string fsKEY)
        {
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();
            List<clsCONFIG> clsCONFIGs = new List<clsCONFIG>();

            dicParameters.Add("fsKEY", fsKEY);
            DataTable dtL_LOG = clsDB.Do_Query("spGET_CONFIG", dicParameters);

            if (dtL_LOG != null && dtL_LOG.Rows.Count > 0)
            {
                for (int i = 0; i < dtL_LOG.Rows.Count; i++)
                {
                    clsCONFIGs.Add(new clsCONFIG {
                        fsKEY = dtL_LOG.Rows[i]["fsKEY"].ToString(),
                        fsVALUE = dtL_LOG.Rows[i]["fsVALUE"].ToString(),
                        fsTYPE = dtL_LOG.Rows[i]["fsTYPE"].ToString(),
                        fsDESCRIPTION = dtL_LOG.Rows[i]["fsDESCRIPTION"].ToString(),

                        fdCREATED_DATE = DateTime.Parse(dtL_LOG.Rows[i]["fdCREATED_DATE"].ToString()),
                        fsCREATED_BY = dtL_LOG.Rows[i]["fsCREATED_BY"].ToString(),
                        fdUPDATED_DATE = DateTime.Parse(dtL_LOG.Rows[i]["fdUPDATED_DATE"].ToString()),
                        fsUPDATED_BY = dtL_LOG.Rows[i]["fsUPDATED_BY"].ToString()
                    });
                }
            }
            return clsCONFIGs;
        }
    }
}