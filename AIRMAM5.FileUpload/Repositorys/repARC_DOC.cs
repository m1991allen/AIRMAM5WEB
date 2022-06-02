// ============================================= 
// 描述: ARC DOC 資料介接 
// 記錄: <2016/11/23><David.Sin><新增本程式>
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
    public class repARC_DOC 
    {
        /// <summary>ARC_DOC的標準新增函數</summary>
        public string fnINSERT_ARC_DOC(clsARC_DOC clsARC_DOC)
        {
            string fsRESULT = string.Empty;
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();

            dicParameters.Add("fsFILE_NO", clsARC_DOC.fsFILE_NO);
            dicParameters.Add("fsTITLE", clsARC_DOC.fsTITLE);
            dicParameters.Add("fsDESCRIPTION", clsARC_DOC.fsDESCRIPTION);
            dicParameters.Add("fsSUBJECT_ID", clsARC_DOC.fsSUBJECT_ID);
            dicParameters.Add("fsFILE_STATUS", clsARC_DOC.fsFILE_STATUS);
            dicParameters.Add("fsFILE_TYPE", clsARC_DOC.fsFILE_TYPE);
            //dicParameters.Add("fsFILE_TYPE_1", clsARC_DOC.fsFILE_TYPE_1);
            //dicParameters.Add("fsFILE_TYPE_2", clsARC_DOC.fsFILE_TYPE_2);
            dicParameters.Add("fsFILE_SIZE", clsARC_DOC.fsFILE_SIZE);
            //dicParameters.Add("fsFILE_SIZE_1", clsARC_DOC.fsFILE_SIZE_1);
            //dicParameters.Add("fsFILE_SIZE_2", clsARC_DOC.fsFILE_SIZE_2);
            dicParameters.Add("fsFILE_PATH", clsARC_DOC.fsFILE_PATH);
            //dicParameters.Add("fsFILE_PATH_1", clsARC_DOC.fsFILE_PATH_1);
            //dicParameters.Add("fsFILE_PATH_2", clsARC_DOC.fsFILE_PATH_2);
            dicParameters.Add("fxMEDIA_INFO", clsARC_DOC.fxMEDIA_INFO);
            dicParameters.Add("fsCONTENT", clsARC_DOC.fsCONTENT);
            dicParameters.Add("fdFILE_CREATED_DATE", clsARC_DOC.fdFILE_CREATED_DATE);
            dicParameters.Add("fdFILE_UPDATED_DATE", clsARC_DOC.fdFILE_UPDATED_DATE);
            dicParameters.Add("fnPRE_ID", clsARC_DOC.fnPRE_ID.ToString());
            dicParameters.Add("fsATTRIBUTE1", clsARC_DOC.fsATTRIBUTE1);
            dicParameters.Add("fsATTRIBUTE2", clsARC_DOC.fsATTRIBUTE2);
            dicParameters.Add("fsATTRIBUTE3", clsARC_DOC.fsATTRIBUTE3);
            dicParameters.Add("fsATTRIBUTE4", clsARC_DOC.fsATTRIBUTE4);
            dicParameters.Add("fsATTRIBUTE5", clsARC_DOC.fsATTRIBUTE5);
            dicParameters.Add("fsATTRIBUTE6", clsARC_DOC.fsATTRIBUTE6);
            dicParameters.Add("fsATTRIBUTE7", clsARC_DOC.fsATTRIBUTE7);
            dicParameters.Add("fsATTRIBUTE8", clsARC_DOC.fsATTRIBUTE8);
            dicParameters.Add("fsATTRIBUTE9", clsARC_DOC.fsATTRIBUTE9);
            dicParameters.Add("fsATTRIBUTE10", clsARC_DOC.fsATTRIBUTE10);
            dicParameters.Add("fsATTRIBUTE11", clsARC_DOC.fsATTRIBUTE11);
            dicParameters.Add("fsATTRIBUTE12", clsARC_DOC.fsATTRIBUTE12);
            dicParameters.Add("fsATTRIBUTE13", clsARC_DOC.fsATTRIBUTE13);
            dicParameters.Add("fsATTRIBUTE14", clsARC_DOC.fsATTRIBUTE14);
            dicParameters.Add("fsATTRIBUTE15", clsARC_DOC.fsATTRIBUTE15);
            dicParameters.Add("fsATTRIBUTE16", clsARC_DOC.fsATTRIBUTE16);
            dicParameters.Add("fsATTRIBUTE17", clsARC_DOC.fsATTRIBUTE17);
            dicParameters.Add("fsATTRIBUTE18", clsARC_DOC.fsATTRIBUTE18);
            dicParameters.Add("fsATTRIBUTE19", clsARC_DOC.fsATTRIBUTE19);
            dicParameters.Add("fsATTRIBUTE20", clsARC_DOC.fsATTRIBUTE20);
            dicParameters.Add("fsATTRIBUTE21", clsARC_DOC.fsATTRIBUTE21);
            dicParameters.Add("fsATTRIBUTE22", clsARC_DOC.fsATTRIBUTE22);
            dicParameters.Add("fsATTRIBUTE23", clsARC_DOC.fsATTRIBUTE23);
            dicParameters.Add("fsATTRIBUTE24", clsARC_DOC.fsATTRIBUTE24);
            dicParameters.Add("fsATTRIBUTE25", clsARC_DOC.fsATTRIBUTE25);
            dicParameters.Add("fsATTRIBUTE26", clsARC_DOC.fsATTRIBUTE26);
            dicParameters.Add("fsATTRIBUTE27", clsARC_DOC.fsATTRIBUTE27);
            dicParameters.Add("fsATTRIBUTE28", clsARC_DOC.fsATTRIBUTE28);
            dicParameters.Add("fsATTRIBUTE29", clsARC_DOC.fsATTRIBUTE29);
            dicParameters.Add("fsATTRIBUTE30", clsARC_DOC.fsATTRIBUTE30);
            dicParameters.Add("fsATTRIBUTE31", clsARC_DOC.fsATTRIBUTE31);
            dicParameters.Add("fsATTRIBUTE32", clsARC_DOC.fsATTRIBUTE32);
            dicParameters.Add("fsATTRIBUTE33", clsARC_DOC.fsATTRIBUTE33);
            dicParameters.Add("fsATTRIBUTE34", clsARC_DOC.fsATTRIBUTE34);
            dicParameters.Add("fsATTRIBUTE35", clsARC_DOC.fsATTRIBUTE35);
            dicParameters.Add("fsATTRIBUTE36", clsARC_DOC.fsATTRIBUTE36);
            dicParameters.Add("fsATTRIBUTE37", clsARC_DOC.fsATTRIBUTE37);
            dicParameters.Add("fsATTRIBUTE38", clsARC_DOC.fsATTRIBUTE38);
            dicParameters.Add("fsATTRIBUTE39", clsARC_DOC.fsATTRIBUTE39);
            dicParameters.Add("fsATTRIBUTE40", clsARC_DOC.fsATTRIBUTE40);
            dicParameters.Add("fsATTRIBUTE41", clsARC_DOC.fsATTRIBUTE41);
            dicParameters.Add("fsATTRIBUTE42", clsARC_DOC.fsATTRIBUTE42);
            dicParameters.Add("fsATTRIBUTE43", clsARC_DOC.fsATTRIBUTE43);
            dicParameters.Add("fsATTRIBUTE44", clsARC_DOC.fsATTRIBUTE44);
            dicParameters.Add("fsATTRIBUTE45", clsARC_DOC.fsATTRIBUTE45);
            dicParameters.Add("fsATTRIBUTE46", clsARC_DOC.fsATTRIBUTE46);
            dicParameters.Add("fsATTRIBUTE47", clsARC_DOC.fsATTRIBUTE47);
            dicParameters.Add("fsATTRIBUTE48", clsARC_DOC.fsATTRIBUTE48);
            dicParameters.Add("fsATTRIBUTE49", clsARC_DOC.fsATTRIBUTE49);
            dicParameters.Add("fsATTRIBUTE50", clsARC_DOC.fsATTRIBUTE50);
            dicParameters.Add("fsATTRIBUTE51", clsARC_DOC.fsATTRIBUTE51);
            dicParameters.Add("fsATTRIBUTE52", clsARC_DOC.fsATTRIBUTE52);
            dicParameters.Add("fsATTRIBUTE53", clsARC_DOC.fsATTRIBUTE53);
            dicParameters.Add("fsATTRIBUTE54", clsARC_DOC.fsATTRIBUTE54);
            dicParameters.Add("fsATTRIBUTE55", clsARC_DOC.fsATTRIBUTE55);
            dicParameters.Add("fsATTRIBUTE56", clsARC_DOC.fsATTRIBUTE56);
            dicParameters.Add("fsATTRIBUTE57", clsARC_DOC.fsATTRIBUTE57);
            dicParameters.Add("fsATTRIBUTE58", clsARC_DOC.fsATTRIBUTE58);
            dicParameters.Add("fsATTRIBUTE59", clsARC_DOC.fsATTRIBUTE59);
            dicParameters.Add("fsATTRIBUTE60", clsARC_DOC.fsATTRIBUTE60);
            dicParameters.Add("fsCREATED_BY", clsARC_DOC.fsCREATED_BY);

            fsRESULT = clsDB.Do_Tran("spINSERT_ARC_DOC", dicParameters);

            return fsRESULT;
        }

        /// <summary>ARC_DOC的標準置換函數</summary>
        public string fnUPDATE_ARC_DOC_CHANGE(clsARC_DOC clsARC_DOC)
        {
            string fsRESULT = string.Empty;
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();

            dicParameters.Add("fsFILE_NO", clsARC_DOC.fsFILE_NO);
            dicParameters.Add("fsFILE_TYPE", clsARC_DOC.fsFILE_TYPE);
            //dicParameters.Add("fsFILE_TYPE_1", clsARC_DOC.fsFILE_TYPE_1);
            //dicParameters.Add("fsFILE_TYPE_2", clsARC_DOC.fsFILE_TYPE_2);
            dicParameters.Add("fsFILE_SIZE", clsARC_DOC.fsFILE_SIZE);
            //dicParameters.Add("fsFILE_SIZE_1", clsARC_DOC.fsFILE_SIZE_1);
            //dicParameters.Add("fsFILE_SIZE_2", clsARC_DOC.fsFILE_SIZE_2);
            dicParameters.Add("fsFILE_PATH", clsARC_DOC.fsFILE_PATH);
            //dicParameters.Add("fsFILE_PATH_1", clsARC_DOC.fsFILE_PATH_1);
            //dicParameters.Add("fsFILE_PATH_2", clsARC_DOC.fsFILE_PATH_2);
            dicParameters.Add("fxMEDIA_INFO", clsARC_DOC.fxMEDIA_INFO);
            dicParameters.Add("fdFILE_CREATED_DATE", clsARC_DOC.fdFILE_CREATED_DATE);
            dicParameters.Add("fdFILE_UPDATED_DATE", clsARC_DOC.fdFILE_UPDATED_DATE);
            dicParameters.Add("fsUPDATED_BY", clsARC_DOC.fsUPDATED_BY);

            fsRESULT = clsDB.Do_Tran("spUPDATE_ARC_DOC_CHANGE", dicParameters);

            return fsRESULT;
        }
    }
}