// ============================================= 
// 描述: SUBJ資料介接 
// 記錄: <2016/09/14><David.Sin><新增本程式>
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
    public class repSUBJ
    {
        /// <summary>SUBJ的標準取出函數</summary>
        public List<clsSUBJ> fnGET_SUBJ(string fsSUBJ_ID) //get的要另外傳入執行者
        {
            List<clsSUBJ> clsSUBJs = new List<clsSUBJ>();
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();

            dicParameters.Add("fsSUBJ_ID", fsSUBJ_ID);

            DataTable dtSUBJ = clsDB.Do_Query("spGET_SUBJECT", dicParameters);

            if (dtSUBJ != null && dtSUBJ.Rows.Count > 0)
            {
                for (int i = 0; i <= dtSUBJ.Rows.Count - 1; i++)
                {
                    clsSUBJs.Add(
                        new clsSUBJ
                        {

                            fsSUBJ_ID = dtSUBJ.Rows[i]["fsSUBJ_ID"].ToString(),
                            fsTITLE = dtSUBJ.Rows[i]["fsTITLE"].ToString(),
                            fsDESCRIPTION = dtSUBJ.Rows[i]["fsDESCRIPTION"].ToString(),
                            fnDIR_ID = long.Parse(dtSUBJ.Rows[i]["fnDIR_ID"].ToString()),
                            fsTYPE1 = dtSUBJ.Rows[i]["fsTYPE1"].ToString(),
                            fsTYPE2 = dtSUBJ.Rows[i]["fsTYPE2"].ToString(),
                            fsTYPE3 = dtSUBJ.Rows[i]["fsTYPE3"].ToString(),
                            fsATTRIBUTE1 = dtSUBJ.Rows[i]["fsATTRIBUTE1"].ToString(),
                            fsATTRIBUTE2 = dtSUBJ.Rows[i]["fsATTRIBUTE2"].ToString(),
                            fsATTRIBUTE3 = dtSUBJ.Rows[i]["fsATTRIBUTE3"].ToString(),
                            fsATTRIBUTE4 = dtSUBJ.Rows[i]["fsATTRIBUTE4"].ToString(),
                            fsATTRIBUTE5 = dtSUBJ.Rows[i]["fsATTRIBUTE5"].ToString(),
                            fsATTRIBUTE6 = dtSUBJ.Rows[i]["fsATTRIBUTE6"].ToString(),
                            fsATTRIBUTE7 = dtSUBJ.Rows[i]["fsATTRIBUTE7"].ToString(),
                            fsATTRIBUTE8 = dtSUBJ.Rows[i]["fsATTRIBUTE8"].ToString(),
                            fsATTRIBUTE9 = dtSUBJ.Rows[i]["fsATTRIBUTE9"].ToString(),
                            fsATTRIBUTE10 = dtSUBJ.Rows[i]["fsATTRIBUTE10"].ToString(),
                            fsATTRIBUTE11 = dtSUBJ.Rows[i]["fsATTRIBUTE11"].ToString(),
                            fsATTRIBUTE12 = dtSUBJ.Rows[i]["fsATTRIBUTE12"].ToString(),
                            fsATTRIBUTE13 = dtSUBJ.Rows[i]["fsATTRIBUTE13"].ToString(),
                            fsATTRIBUTE14 = dtSUBJ.Rows[i]["fsATTRIBUTE14"].ToString(),
                            fsATTRIBUTE15 = dtSUBJ.Rows[i]["fsATTRIBUTE15"].ToString(),
                            fsATTRIBUTE16 = dtSUBJ.Rows[i]["fsATTRIBUTE16"].ToString(),
                            fsATTRIBUTE17 = dtSUBJ.Rows[i]["fsATTRIBUTE17"].ToString(),
                            fsATTRIBUTE18 = dtSUBJ.Rows[i]["fsATTRIBUTE18"].ToString(),
                            fsATTRIBUTE19 = dtSUBJ.Rows[i]["fsATTRIBUTE19"].ToString(),
                            fsATTRIBUTE20 = dtSUBJ.Rows[i]["fsATTRIBUTE20"].ToString(),
                            fsATTRIBUTE21 = dtSUBJ.Rows[i]["fsATTRIBUTE21"].ToString(),
                            fsATTRIBUTE22 = dtSUBJ.Rows[i]["fsATTRIBUTE22"].ToString(),
                            fsATTRIBUTE23 = dtSUBJ.Rows[i]["fsATTRIBUTE23"].ToString(),
                            fsATTRIBUTE24 = dtSUBJ.Rows[i]["fsATTRIBUTE24"].ToString(),
                            fsATTRIBUTE25 = dtSUBJ.Rows[i]["fsATTRIBUTE25"].ToString(),
                            fsATTRIBUTE26 = dtSUBJ.Rows[i]["fsATTRIBUTE26"].ToString(),
                            fsATTRIBUTE27 = dtSUBJ.Rows[i]["fsATTRIBUTE27"].ToString(),
                            fsATTRIBUTE28 = dtSUBJ.Rows[i]["fsATTRIBUTE28"].ToString(),
                            fsATTRIBUTE29 = dtSUBJ.Rows[i]["fsATTRIBUTE29"].ToString(),
                            fsATTRIBUTE30 = dtSUBJ.Rows[i]["fsATTRIBUTE30"].ToString(),
                            fsATTRIBUTE31 = dtSUBJ.Rows[i]["fsATTRIBUTE31"].ToString(),
                            fsATTRIBUTE32 = dtSUBJ.Rows[i]["fsATTRIBUTE32"].ToString(),
                            fsATTRIBUTE33 = dtSUBJ.Rows[i]["fsATTRIBUTE33"].ToString(),
                            fsATTRIBUTE34 = dtSUBJ.Rows[i]["fsATTRIBUTE34"].ToString(),
                            fsATTRIBUTE35 = dtSUBJ.Rows[i]["fsATTRIBUTE35"].ToString(),
                            fsATTRIBUTE36 = dtSUBJ.Rows[i]["fsATTRIBUTE36"].ToString(),
                            fsATTRIBUTE37 = dtSUBJ.Rows[i]["fsATTRIBUTE37"].ToString(),
                            fsATTRIBUTE38 = dtSUBJ.Rows[i]["fsATTRIBUTE38"].ToString(),
                            fsATTRIBUTE39 = dtSUBJ.Rows[i]["fsATTRIBUTE39"].ToString(),
                            fsATTRIBUTE40 = dtSUBJ.Rows[i]["fsATTRIBUTE40"].ToString(),
                            fsATTRIBUTE41 = dtSUBJ.Rows[i]["fsATTRIBUTE41"].ToString(),
                            fsATTRIBUTE42 = dtSUBJ.Rows[i]["fsATTRIBUTE42"].ToString(),
                            fsATTRIBUTE43 = dtSUBJ.Rows[i]["fsATTRIBUTE43"].ToString(),
                            fsATTRIBUTE44 = dtSUBJ.Rows[i]["fsATTRIBUTE44"].ToString(),
                            fsATTRIBUTE45 = dtSUBJ.Rows[i]["fsATTRIBUTE45"].ToString(),
                            fsATTRIBUTE46 = dtSUBJ.Rows[i]["fsATTRIBUTE46"].ToString(),
                            fsATTRIBUTE47 = dtSUBJ.Rows[i]["fsATTRIBUTE47"].ToString(),
                            fsATTRIBUTE48 = dtSUBJ.Rows[i]["fsATTRIBUTE48"].ToString(),
                            fsATTRIBUTE49 = dtSUBJ.Rows[i]["fsATTRIBUTE49"].ToString(),
                            fsATTRIBUTE50 = dtSUBJ.Rows[i]["fsATTRIBUTE50"].ToString(),

                            fdCREATED_DATE = DateTime.Parse(dtSUBJ.Rows[i]["fdCREATED_DATE"].ToString()),
                            fsCREATED_BY = dtSUBJ.Rows[i]["fsCREATED_BY"].ToString(),
                            fdUPDATED_DATE = DateTime.Parse(dtSUBJ.Rows[i]["fdUPDATED_DATE"].ToString()),
                            fsUPDATED_BY = dtSUBJ.Rows[i]["fsUPDATED_BY"].ToString(),

                            _sDIR_PATH = dtSUBJ.Rows[i]["_sDIR_PATH"].ToString(),
                            _sSUBJ_PATH = dtSUBJ.Rows[i]["_sSUBJ_PATH"].ToString(),
                            _sTYPE1 = dtSUBJ.Rows[i]["_sTYPE1"].ToString(),
                            _sTYPE2 = dtSUBJ.Rows[i]["_sTYPE2"].ToString(),
                            _sTYPE3 = dtSUBJ.Rows[i]["_sTYPE3"].ToString()
                        }
                    );
                }
            }

            return clsSUBJs;
        }

    }
}