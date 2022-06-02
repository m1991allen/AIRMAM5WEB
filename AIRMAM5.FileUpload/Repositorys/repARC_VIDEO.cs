// ============================================= 
// 描述: ARC VIDEO 資料介接 
// 記錄: <2016/11/15><David.Sin><新增本程式>
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
    public class repARC_VIDEO
    {
        /// <summary>ARC_VDO的標準新增函數</summary>
        public string fnINSERT_ARC_VDO(clsARC_VIDEO clsARC_VIDEO)
        {
            string fsRESULT = string.Empty;
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();

            dicParameters.Add("fsFILE_NO", clsARC_VIDEO.fsFILE_NO);
            dicParameters.Add("fsTITLE", clsARC_VIDEO.fsTITLE);
            dicParameters.Add("fsDESCRIPTION", clsARC_VIDEO.fsDESCRIPTION);
            dicParameters.Add("fsSUBJECT_ID", clsARC_VIDEO.fsSUBJECT_ID);
            dicParameters.Add("fsFILE_STATUS", clsARC_VIDEO.fsFILE_STATUS);
           
            dicParameters.Add("fsFILE_TYPE", clsARC_VIDEO.fsFILE_TYPE);
            dicParameters.Add("fsFILE_TYPE_H", clsARC_VIDEO.fsFILE_TYPE_H);
            dicParameters.Add("fsFILE_TYPE_L", clsARC_VIDEO.fsFILE_TYPE_L);
            dicParameters.Add("fsFILE_SIZE", clsARC_VIDEO.fsFILE_SIZE);
            dicParameters.Add("fsFILE_SIZE_H", clsARC_VIDEO.fsFILE_SIZE_H);
            dicParameters.Add("fsFILE_SIZE_L", clsARC_VIDEO.fsFILE_SIZE_L);
            dicParameters.Add("fsFILE_PATH", clsARC_VIDEO.fsFILE_PATH);
            dicParameters.Add("fsFILE_PATH_H", clsARC_VIDEO.fsFILE_PATH_H);
            dicParameters.Add("fsFILE_PATH_L", clsARC_VIDEO.fsFILE_PATH_L);
            dicParameters.Add("fxMEDIA_INFO", clsARC_VIDEO.fxMEDIA_INFO);
            dicParameters.Add("fdBEG_TIME", clsARC_VIDEO.fdBEG_TIME.ToString());
            dicParameters.Add("fdEND_TIME", clsARC_VIDEO.fdEND_TIME.ToString());
            dicParameters.Add("fdDURATION", clsARC_VIDEO.fdDURATION.ToString());
            dicParameters.Add("fsRESOL_TAG", clsARC_VIDEO.fsRESOL_TAG);
            dicParameters.Add("fnPRE_ID", clsARC_VIDEO.fnPRE_ID.ToString());
            dicParameters.Add("fsCREATED_BY", clsARC_VIDEO.fsCREATED_BY);


            fsRESULT = clsDB.Do_Tran("spINSERT_ARC_VIDEO", dicParameters);
            return fsRESULT;
        }

        /// <summary>ARC_VDO的標準置換函數</summary>
        public string fnUPDATE_ARC_VDO_CHANGE(clsARC_VIDEO clsARC_VIDEO)
        {
            string fsRESULT = string.Empty;
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();

            dicParameters.Add("fsFILE_NO", clsARC_VIDEO.fsFILE_NO);
            dicParameters.Add("fsFILE_TYPE", clsARC_VIDEO.fsFILE_TYPE);
            dicParameters.Add("fsFILE_TYPE_H", clsARC_VIDEO.fsFILE_TYPE_H);
            dicParameters.Add("fsFILE_TYPE_L", clsARC_VIDEO.fsFILE_TYPE_L);
            dicParameters.Add("fsFILE_SIZE", clsARC_VIDEO.fsFILE_SIZE);
            dicParameters.Add("fsFILE_SIZE_H", clsARC_VIDEO.fsFILE_SIZE_H);
            dicParameters.Add("fsFILE_SIZE_L", clsARC_VIDEO.fsFILE_SIZE_L);
            dicParameters.Add("fsFILE_PATH", clsARC_VIDEO.fsFILE_PATH);
            dicParameters.Add("fsFILE_PATH_H", clsARC_VIDEO.fsFILE_PATH_H);
            dicParameters.Add("fsFILE_PATH_L", clsARC_VIDEO.fsFILE_PATH_L);
            dicParameters.Add("fxMEDIA_INFO", clsARC_VIDEO.fxMEDIA_INFO);
            dicParameters.Add("fdBEG_TIME", clsARC_VIDEO.fdBEG_TIME.ToString());
            dicParameters.Add("fdEND_TIME", clsARC_VIDEO.fdEND_TIME.ToString());
            dicParameters.Add("fdDURATION", clsARC_VIDEO.fdDURATION.ToString());
            dicParameters.Add("fsRESOL_TAG", clsARC_VIDEO.fsRESOL_TAG);
            dicParameters.Add("fsUPDATED_BY", clsARC_VIDEO.fsUPDATED_BY);
            dicParameters.Add("fcDELETE_KF", (clsARC_VIDEO.fbDELETE_KF ? "Y" : "N"));

            fsRESULT = clsDB.Do_Tran("spUPDATE_ARC_VIDEO_CHANGE", dicParameters);

            return fsRESULT;
        }

    }
}