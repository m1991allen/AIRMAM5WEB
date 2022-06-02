// ============================================= 
// 描述: ARC PHOTO 資料介接 
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
    public class repARC_PHOTO
    {
        /// <summary>ARC_PHO的標準新增函數</summary>
        public string fnINSERT_ARC_PHOTO(clsARC_PHOTO clsARC_PHOTO)
        {
            string fsRESULT = string.Empty;
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();

            dicParameters.Add("fsFILE_NO", clsARC_PHOTO.fsFILE_NO);
            dicParameters.Add("fsTITLE", clsARC_PHOTO.fsTITLE);
            dicParameters.Add("fsDESCRIPTION", clsARC_PHOTO.fsDESCRIPTION);
            dicParameters.Add("fsSUBJECT_ID", clsARC_PHOTO.fsSUBJECT_ID);
            dicParameters.Add("fsFILE_STATUS", clsARC_PHOTO.fsFILE_STATUS);
            dicParameters.Add("fsFILE_TYPE", clsARC_PHOTO.fsFILE_TYPE);
            dicParameters.Add("fsFILE_TYPE_H", clsARC_PHOTO.fsFILE_TYPE_H);
            dicParameters.Add("fsFILE_TYPE_L", clsARC_PHOTO.fsFILE_TYPE_L);
            dicParameters.Add("fsFILE_SIZE", clsARC_PHOTO.fsFILE_SIZE);
            dicParameters.Add("fsFILE_SIZE_H", clsARC_PHOTO.fsFILE_SIZE_H);
            dicParameters.Add("fsFILE_SIZE_L", clsARC_PHOTO.fsFILE_SIZE_L);
            dicParameters.Add("fsFILE_PATH", clsARC_PHOTO.fsFILE_PATH);
            dicParameters.Add("fsFILE_PATH_H", clsARC_PHOTO.fsFILE_PATH_H);
            dicParameters.Add("fsFILE_PATH_L", clsARC_PHOTO.fsFILE_PATH_L);
            dicParameters.Add("fxMEDIA_INFO", clsARC_PHOTO.fxMEDIA_INFO);
            dicParameters.Add("fnWIDTH", clsARC_PHOTO.fnWIDTH);
            dicParameters.Add("fnHEIGHT", clsARC_PHOTO.fnHEIGHT);
            dicParameters.Add("fnXDPI", clsARC_PHOTO.fnXDPI);
            dicParameters.Add("fnYDPI", clsARC_PHOTO.fnYDPI);
            dicParameters.Add("fsCAMERA_MAKE", clsARC_PHOTO.fsCAMERA_MAKE);
            dicParameters.Add("fsCAMERA_MODEL", clsARC_PHOTO.fsCAMERA_MODEL);
            dicParameters.Add("fsFOCAL_LENGTH", clsARC_PHOTO.fsFOCAL_LENGTH);
            dicParameters.Add("fsEXPOSURE_TIME", clsARC_PHOTO.fsEXPOSURE_TIME);
            dicParameters.Add("fsAPERTURE", clsARC_PHOTO.fsAPERTURE);
            dicParameters.Add("fnISO", clsARC_PHOTO.fnISO.ToString());
            dicParameters.Add("fnPRE_ID", clsARC_PHOTO.fnPRE_ID.ToString());
            dicParameters.Add("fsATTRIBUTE1", clsARC_PHOTO.fsATTRIBUTE1);
            dicParameters.Add("fsATTRIBUTE2", clsARC_PHOTO.fsATTRIBUTE2);
            dicParameters.Add("fsATTRIBUTE3", clsARC_PHOTO.fsATTRIBUTE3);
            dicParameters.Add("fsATTRIBUTE4", clsARC_PHOTO.fsATTRIBUTE4);
            dicParameters.Add("fsATTRIBUTE5", clsARC_PHOTO.fsATTRIBUTE5);
            dicParameters.Add("fsATTRIBUTE6", clsARC_PHOTO.fsATTRIBUTE6);
            dicParameters.Add("fsATTRIBUTE7", clsARC_PHOTO.fsATTRIBUTE7);
            dicParameters.Add("fsATTRIBUTE8", clsARC_PHOTO.fsATTRIBUTE8);
            dicParameters.Add("fsATTRIBUTE9", clsARC_PHOTO.fsATTRIBUTE9);
            dicParameters.Add("fsATTRIBUTE10", clsARC_PHOTO.fsATTRIBUTE10);
            dicParameters.Add("fsATTRIBUTE11", clsARC_PHOTO.fsATTRIBUTE11);
            dicParameters.Add("fsATTRIBUTE12", clsARC_PHOTO.fsATTRIBUTE12);
            dicParameters.Add("fsATTRIBUTE13", clsARC_PHOTO.fsATTRIBUTE13);
            dicParameters.Add("fsATTRIBUTE14", clsARC_PHOTO.fsATTRIBUTE14);
            dicParameters.Add("fsATTRIBUTE15", clsARC_PHOTO.fsATTRIBUTE15);
            dicParameters.Add("fsATTRIBUTE16", clsARC_PHOTO.fsATTRIBUTE16);
            dicParameters.Add("fsATTRIBUTE17", clsARC_PHOTO.fsATTRIBUTE17);
            dicParameters.Add("fsATTRIBUTE18", clsARC_PHOTO.fsATTRIBUTE18);
            dicParameters.Add("fsATTRIBUTE19", clsARC_PHOTO.fsATTRIBUTE19);
            dicParameters.Add("fsATTRIBUTE20", clsARC_PHOTO.fsATTRIBUTE20);
            dicParameters.Add("fsATTRIBUTE21", clsARC_PHOTO.fsATTRIBUTE21);
            dicParameters.Add("fsATTRIBUTE22", clsARC_PHOTO.fsATTRIBUTE22);
            dicParameters.Add("fsATTRIBUTE23", clsARC_PHOTO.fsATTRIBUTE23);
            dicParameters.Add("fsATTRIBUTE24", clsARC_PHOTO.fsATTRIBUTE24);
            dicParameters.Add("fsATTRIBUTE25", clsARC_PHOTO.fsATTRIBUTE25);
            dicParameters.Add("fsATTRIBUTE26", clsARC_PHOTO.fsATTRIBUTE26);
            dicParameters.Add("fsATTRIBUTE27", clsARC_PHOTO.fsATTRIBUTE27);
            dicParameters.Add("fsATTRIBUTE28", clsARC_PHOTO.fsATTRIBUTE28);
            dicParameters.Add("fsATTRIBUTE29", clsARC_PHOTO.fsATTRIBUTE29);
            dicParameters.Add("fsATTRIBUTE30", clsARC_PHOTO.fsATTRIBUTE30);
            dicParameters.Add("fsATTRIBUTE31", clsARC_PHOTO.fsATTRIBUTE31);
            dicParameters.Add("fsATTRIBUTE32", clsARC_PHOTO.fsATTRIBUTE32);
            dicParameters.Add("fsATTRIBUTE33", clsARC_PHOTO.fsATTRIBUTE33);
            dicParameters.Add("fsATTRIBUTE34", clsARC_PHOTO.fsATTRIBUTE34);
            dicParameters.Add("fsATTRIBUTE35", clsARC_PHOTO.fsATTRIBUTE35);
            dicParameters.Add("fsATTRIBUTE36", clsARC_PHOTO.fsATTRIBUTE36);
            dicParameters.Add("fsATTRIBUTE37", clsARC_PHOTO.fsATTRIBUTE37);
            dicParameters.Add("fsATTRIBUTE38", clsARC_PHOTO.fsATTRIBUTE38);
            dicParameters.Add("fsATTRIBUTE39", clsARC_PHOTO.fsATTRIBUTE39);
            dicParameters.Add("fsATTRIBUTE40", clsARC_PHOTO.fsATTRIBUTE40);
            dicParameters.Add("fsATTRIBUTE41", clsARC_PHOTO.fsATTRIBUTE41);
            dicParameters.Add("fsATTRIBUTE42", clsARC_PHOTO.fsATTRIBUTE42);
            dicParameters.Add("fsATTRIBUTE43", clsARC_PHOTO.fsATTRIBUTE43);
            dicParameters.Add("fsATTRIBUTE44", clsARC_PHOTO.fsATTRIBUTE44);
            dicParameters.Add("fsATTRIBUTE45", clsARC_PHOTO.fsATTRIBUTE45);
            dicParameters.Add("fsATTRIBUTE46", clsARC_PHOTO.fsATTRIBUTE46);
            dicParameters.Add("fsATTRIBUTE47", clsARC_PHOTO.fsATTRIBUTE47);
            dicParameters.Add("fsATTRIBUTE48", clsARC_PHOTO.fsATTRIBUTE48);
            dicParameters.Add("fsATTRIBUTE49", clsARC_PHOTO.fsATTRIBUTE49);
            dicParameters.Add("fsATTRIBUTE50", clsARC_PHOTO.fsATTRIBUTE50);
            dicParameters.Add("fsATTRIBUTE51", clsARC_PHOTO.fsATTRIBUTE51);
            dicParameters.Add("fsATTRIBUTE52", clsARC_PHOTO.fsATTRIBUTE52);
            dicParameters.Add("fsATTRIBUTE53", clsARC_PHOTO.fsATTRIBUTE53);
            dicParameters.Add("fsATTRIBUTE54", clsARC_PHOTO.fsATTRIBUTE54);
            dicParameters.Add("fsATTRIBUTE55", clsARC_PHOTO.fsATTRIBUTE55);
            dicParameters.Add("fsATTRIBUTE56", clsARC_PHOTO.fsATTRIBUTE56);
            dicParameters.Add("fsATTRIBUTE57", clsARC_PHOTO.fsATTRIBUTE57);
            dicParameters.Add("fsATTRIBUTE58", clsARC_PHOTO.fsATTRIBUTE58);
            dicParameters.Add("fsATTRIBUTE59", clsARC_PHOTO.fsATTRIBUTE59);
            dicParameters.Add("fsATTRIBUTE60", clsARC_PHOTO.fsATTRIBUTE60);
            dicParameters.Add("fsCREATED_BY", clsARC_PHOTO.fsCREATED_BY);

            fsRESULT = clsDB.Do_Tran("spINSERT_ARC_PHOTO", dicParameters);

            return fsRESULT;
        }

        /// <summary>ARC_PHO的標準置換函數</summary>
        public string fnUPDATE_ARC_PHOTO_CHANGE(clsARC_PHOTO clsARC_PHOTO)
        {
            string fsRESULT = string.Empty;
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();

            dicParameters.Add("fsFILE_NO", clsARC_PHOTO.fsFILE_NO);
            dicParameters.Add("fsFILE_TYPE", clsARC_PHOTO.fsFILE_TYPE);
            dicParameters.Add("fsFILE_TYPE_H", clsARC_PHOTO.fsFILE_TYPE_H);
            dicParameters.Add("fsFILE_TYPE_L", clsARC_PHOTO.fsFILE_TYPE_L);
            dicParameters.Add("fsFILE_SIZE", clsARC_PHOTO.fsFILE_SIZE);
            dicParameters.Add("fsFILE_SIZE_H", clsARC_PHOTO.fsFILE_SIZE_H);
            dicParameters.Add("fsFILE_SIZE_L", clsARC_PHOTO.fsFILE_SIZE_L);
            dicParameters.Add("fsFILE_PATH", clsARC_PHOTO.fsFILE_PATH);
            dicParameters.Add("fsFILE_PATH_H", clsARC_PHOTO.fsFILE_PATH_H);
            dicParameters.Add("fsFILE_PATH_L", clsARC_PHOTO.fsFILE_PATH_L);
            dicParameters.Add("fxMEDIA_INFO", clsARC_PHOTO.fxMEDIA_INFO);
            dicParameters.Add("fnWIDTH", clsARC_PHOTO.fnWIDTH);
            dicParameters.Add("fnHEIGHT", clsARC_PHOTO.fnHEIGHT);
            dicParameters.Add("fnXDPI", clsARC_PHOTO.fnXDPI);
            dicParameters.Add("fnYDPI", clsARC_PHOTO.fnYDPI);
            dicParameters.Add("fsCAMERA_MAKE", clsARC_PHOTO.fsCAMERA_MAKE);
            dicParameters.Add("fsCAMERA_MODEL", clsARC_PHOTO.fsCAMERA_MODEL);
            dicParameters.Add("fsFOCAL_LENGTH", clsARC_PHOTO.fsFOCAL_LENGTH);
            dicParameters.Add("fsEXPOSURE_TIME", clsARC_PHOTO.fsEXPOSURE_TIME);
            dicParameters.Add("fsAPERTURE", clsARC_PHOTO.fsAPERTURE);
            dicParameters.Add("fnISO", clsARC_PHOTO.fnISO.ToString());
            dicParameters.Add("fsUPDATED_BY", clsARC_PHOTO.fsUPDATED_BY);

            fsRESULT = clsDB.Do_Tran("spUPDATE_ARC_PHOTO_CHANGE", dicParameters);

            return fsRESULT;
        }
    }
}