// ============================================= 
// 描述: ARC AUDIO 資料介接 
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
    public class repARC_AUDIO
    {
        /// <summary>ARC_AUD的標準新增函數</summary>
        public string fnINSERT_ARC_AUDIO(clsARC_AUDIO clsARC_AUDIO)
        {
            string fsRESULT = string.Empty;
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();

            dicParameters.Add("fsFILE_NO", clsARC_AUDIO.fsFILE_NO);
            dicParameters.Add("fsTITLE", clsARC_AUDIO.fsTITLE);
            dicParameters.Add("fsDESCRIPTION", clsARC_AUDIO.fsDESCRIPTION);
            dicParameters.Add("fsSUBJECT_ID", clsARC_AUDIO.fsSUBJECT_ID);
            dicParameters.Add("fsFILE_STATUS", clsARC_AUDIO.fsFILE_STATUS);
            dicParameters.Add("fsFILE_TYPE", clsARC_AUDIO.fsFILE_TYPE);
            dicParameters.Add("fsFILE_TYPE_H", clsARC_AUDIO.fsFILE_TYPE_H);
            dicParameters.Add("fsFILE_TYPE_L", clsARC_AUDIO.fsFILE_TYPE_L);
            dicParameters.Add("fsFILE_SIZE", clsARC_AUDIO.fsFILE_SIZE);
            dicParameters.Add("fsFILE_SIZE_H", clsARC_AUDIO.fsFILE_SIZE_H);
            dicParameters.Add("fsFILE_SIZE_L", clsARC_AUDIO.fsFILE_SIZE_L);
            dicParameters.Add("fsFILE_PATH", clsARC_AUDIO.fsFILE_PATH);
            dicParameters.Add("fsFILE_PATH_H", clsARC_AUDIO.fsFILE_PATH_H);
            dicParameters.Add("fsFILE_PATH_L", clsARC_AUDIO.fsFILE_PATH_L);
            dicParameters.Add("fxMEDIA_INFO", clsARC_AUDIO.fxMEDIA_INFO);
            dicParameters.Add("fsALBUM", clsARC_AUDIO.fsALBUM);
            dicParameters.Add("fsALBUM_TITLE", clsARC_AUDIO.fsALBUM_TITLE);
            dicParameters.Add("fsALBUM_ARTISTS", clsARC_AUDIO.fsALBUM_ARTISTS);
            dicParameters.Add("fsALBUM_PERFORMERS", clsARC_AUDIO.fsALBUM_PERFORMERS);
            dicParameters.Add("fsALBUM_COMPOSERS", clsARC_AUDIO.fsALBUM_COMPOSERS);
            dicParameters.Add("fnALBUM_YEAR", clsARC_AUDIO.fnALBUM_YEAR.ToString());
            dicParameters.Add("fsALBUM_COPYRIGHT", clsARC_AUDIO.fsALBUM_COPYRIGHT);
            dicParameters.Add("fsALBUM_GENRES", clsARC_AUDIO.fsALBUM_GENRES);
            dicParameters.Add("fsALBUM_COMMENT", clsARC_AUDIO.fsALBUM_COMMENT);
            dicParameters.Add("fcALBUM_PICTURE", (clsARC_AUDIO.fcALBUM_PICTURE ? "Y" : "N"));
            dicParameters.Add("fdBEG_TIME", clsARC_AUDIO.fdBEG_TIME.ToString());
            dicParameters.Add("fdEND_TIME", clsARC_AUDIO.fdEND_TIME.ToString());
            dicParameters.Add("fdDURATION", clsARC_AUDIO.fdDURATION.ToString());
            dicParameters.Add("fnPRE_ID", clsARC_AUDIO.fnPRE_ID.ToString());
            dicParameters.Add("fsATTRIBUTE1", clsARC_AUDIO.fsATTRIBUTE1);
            dicParameters.Add("fsATTRIBUTE2", clsARC_AUDIO.fsATTRIBUTE2);
            dicParameters.Add("fsATTRIBUTE3", clsARC_AUDIO.fsATTRIBUTE3);
            dicParameters.Add("fsATTRIBUTE4", clsARC_AUDIO.fsATTRIBUTE4);
            dicParameters.Add("fsATTRIBUTE5", clsARC_AUDIO.fsATTRIBUTE5);
            dicParameters.Add("fsATTRIBUTE6", clsARC_AUDIO.fsATTRIBUTE6);
            dicParameters.Add("fsATTRIBUTE7", clsARC_AUDIO.fsATTRIBUTE7);
            dicParameters.Add("fsATTRIBUTE8", clsARC_AUDIO.fsATTRIBUTE8);
            dicParameters.Add("fsATTRIBUTE9", clsARC_AUDIO.fsATTRIBUTE9);
            dicParameters.Add("fsATTRIBUTE10", clsARC_AUDIO.fsATTRIBUTE10);
            dicParameters.Add("fsATTRIBUTE11", clsARC_AUDIO.fsATTRIBUTE11);
            dicParameters.Add("fsATTRIBUTE12", clsARC_AUDIO.fsATTRIBUTE12);
            dicParameters.Add("fsATTRIBUTE13", clsARC_AUDIO.fsATTRIBUTE13);
            dicParameters.Add("fsATTRIBUTE14", clsARC_AUDIO.fsATTRIBUTE14);
            dicParameters.Add("fsATTRIBUTE15", clsARC_AUDIO.fsATTRIBUTE15);
            dicParameters.Add("fsATTRIBUTE16", clsARC_AUDIO.fsATTRIBUTE16);
            dicParameters.Add("fsATTRIBUTE17", clsARC_AUDIO.fsATTRIBUTE17);
            dicParameters.Add("fsATTRIBUTE18", clsARC_AUDIO.fsATTRIBUTE18);
            dicParameters.Add("fsATTRIBUTE19", clsARC_AUDIO.fsATTRIBUTE19);
            dicParameters.Add("fsATTRIBUTE20", clsARC_AUDIO.fsATTRIBUTE20);
            dicParameters.Add("fsATTRIBUTE21", clsARC_AUDIO.fsATTRIBUTE21);
            dicParameters.Add("fsATTRIBUTE22", clsARC_AUDIO.fsATTRIBUTE22);
            dicParameters.Add("fsATTRIBUTE23", clsARC_AUDIO.fsATTRIBUTE23);
            dicParameters.Add("fsATTRIBUTE24", clsARC_AUDIO.fsATTRIBUTE24);
            dicParameters.Add("fsATTRIBUTE25", clsARC_AUDIO.fsATTRIBUTE25);
            dicParameters.Add("fsATTRIBUTE26", clsARC_AUDIO.fsATTRIBUTE26);
            dicParameters.Add("fsATTRIBUTE27", clsARC_AUDIO.fsATTRIBUTE27);
            dicParameters.Add("fsATTRIBUTE28", clsARC_AUDIO.fsATTRIBUTE28);
            dicParameters.Add("fsATTRIBUTE29", clsARC_AUDIO.fsATTRIBUTE29);
            dicParameters.Add("fsATTRIBUTE30", clsARC_AUDIO.fsATTRIBUTE30);
            dicParameters.Add("fsATTRIBUTE31", clsARC_AUDIO.fsATTRIBUTE31);
            dicParameters.Add("fsATTRIBUTE32", clsARC_AUDIO.fsATTRIBUTE32);
            dicParameters.Add("fsATTRIBUTE33", clsARC_AUDIO.fsATTRIBUTE33);
            dicParameters.Add("fsATTRIBUTE34", clsARC_AUDIO.fsATTRIBUTE34);
            dicParameters.Add("fsATTRIBUTE35", clsARC_AUDIO.fsATTRIBUTE35);
            dicParameters.Add("fsATTRIBUTE36", clsARC_AUDIO.fsATTRIBUTE36);
            dicParameters.Add("fsATTRIBUTE37", clsARC_AUDIO.fsATTRIBUTE37);
            dicParameters.Add("fsATTRIBUTE38", clsARC_AUDIO.fsATTRIBUTE38);
            dicParameters.Add("fsATTRIBUTE39", clsARC_AUDIO.fsATTRIBUTE39);
            dicParameters.Add("fsATTRIBUTE40", clsARC_AUDIO.fsATTRIBUTE40);
            dicParameters.Add("fsATTRIBUTE41", clsARC_AUDIO.fsATTRIBUTE41);
            dicParameters.Add("fsATTRIBUTE42", clsARC_AUDIO.fsATTRIBUTE42);
            dicParameters.Add("fsATTRIBUTE43", clsARC_AUDIO.fsATTRIBUTE43);
            dicParameters.Add("fsATTRIBUTE44", clsARC_AUDIO.fsATTRIBUTE44);
            dicParameters.Add("fsATTRIBUTE45", clsARC_AUDIO.fsATTRIBUTE45);
            dicParameters.Add("fsATTRIBUTE46", clsARC_AUDIO.fsATTRIBUTE46);
            dicParameters.Add("fsATTRIBUTE47", clsARC_AUDIO.fsATTRIBUTE47);
            dicParameters.Add("fsATTRIBUTE48", clsARC_AUDIO.fsATTRIBUTE48);
            dicParameters.Add("fsATTRIBUTE49", clsARC_AUDIO.fsATTRIBUTE49);
            dicParameters.Add("fsATTRIBUTE50", clsARC_AUDIO.fsATTRIBUTE50);
            dicParameters.Add("fsATTRIBUTE51", clsARC_AUDIO.fsATTRIBUTE51);
            dicParameters.Add("fsATTRIBUTE52", clsARC_AUDIO.fsATTRIBUTE52);
            dicParameters.Add("fsATTRIBUTE53", clsARC_AUDIO.fsATTRIBUTE53);
            dicParameters.Add("fsATTRIBUTE54", clsARC_AUDIO.fsATTRIBUTE54);
            dicParameters.Add("fsATTRIBUTE55", clsARC_AUDIO.fsATTRIBUTE55);
            dicParameters.Add("fsATTRIBUTE56", clsARC_AUDIO.fsATTRIBUTE56);
            dicParameters.Add("fsATTRIBUTE57", clsARC_AUDIO.fsATTRIBUTE57);
            dicParameters.Add("fsATTRIBUTE58", clsARC_AUDIO.fsATTRIBUTE58);
            dicParameters.Add("fsATTRIBUTE59", clsARC_AUDIO.fsATTRIBUTE59);
            dicParameters.Add("fsATTRIBUTE60", clsARC_AUDIO.fsATTRIBUTE60);
            dicParameters.Add("fsCREATED_BY", clsARC_AUDIO.fsCREATED_BY);
            
            fsRESULT = clsDB.Do_Tran("spINSERT_ARC_AUDIO", dicParameters);

            return fsRESULT;
        }
        
        /// <summary>ARC_AUDIO的標準置換函數</summary>
        public string fnUPDATE_ARC_AUDIO_CHANGE(clsARC_AUDIO clsARC_AUDIO)
        {
            string fsRESULT = string.Empty;
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();

            dicParameters.Add("fsFILE_NO", clsARC_AUDIO.fsFILE_NO);
            dicParameters.Add("fsFILE_TYPE", clsARC_AUDIO.fsFILE_TYPE);
            dicParameters.Add("fsFILE_TYPE_H", clsARC_AUDIO.fsFILE_TYPE_H);
            dicParameters.Add("fsFILE_TYPE_L", clsARC_AUDIO.fsFILE_TYPE_L);
            dicParameters.Add("fsFILE_SIZE", clsARC_AUDIO.fsFILE_SIZE);
            dicParameters.Add("fsFILE_SIZE_H", clsARC_AUDIO.fsFILE_SIZE_H);
            dicParameters.Add("fsFILE_SIZE_L", clsARC_AUDIO.fsFILE_SIZE_L);
            dicParameters.Add("fsFILE_PATH", clsARC_AUDIO.fsFILE_PATH);
            dicParameters.Add("fsFILE_PATH_H", clsARC_AUDIO.fsFILE_PATH_H);
            dicParameters.Add("fsFILE_PATH_L", clsARC_AUDIO.fsFILE_PATH_L);
            dicParameters.Add("fxMEDIA_INFO", clsARC_AUDIO.fxMEDIA_INFO);
            dicParameters.Add("fsALBUM", clsARC_AUDIO.fsALBUM);
            dicParameters.Add("fsALBUM_TITLE", clsARC_AUDIO.fsALBUM_TITLE);
            dicParameters.Add("fsALBUM_ARTISTS", clsARC_AUDIO.fsALBUM_ARTISTS);
            dicParameters.Add("fsALBUM_PERFORMERS", clsARC_AUDIO.fsALBUM_PERFORMERS);
            dicParameters.Add("fsALBUM_COMPOSERS", clsARC_AUDIO.fsALBUM_COMPOSERS);
            dicParameters.Add("fnALBUM_YEAR", clsARC_AUDIO.fnALBUM_YEAR.ToString());
            dicParameters.Add("fsALBUM_COPYRIGHT", clsARC_AUDIO.fsALBUM_COPYRIGHT);
            dicParameters.Add("fsALBUM_GENRES", clsARC_AUDIO.fsALBUM_GENRES);
            dicParameters.Add("fsALBUM_COMMENT", clsARC_AUDIO.fsALBUM_COMMENT);
            dicParameters.Add("fcALBUM_PICTURE", (clsARC_AUDIO.fcALBUM_PICTURE ? "Y" : "N"));
            dicParameters.Add("fdBEG_TIME", clsARC_AUDIO.fdBEG_TIME.ToString());
            dicParameters.Add("fdEND_TIME", clsARC_AUDIO.fdEND_TIME.ToString());
            dicParameters.Add("fdDURATION", clsARC_AUDIO.fdDURATION.ToString());
            dicParameters.Add("fsUPDATED_BY", clsARC_AUDIO.fsUPDATED_BY);

            fsRESULT = clsDB.Do_Tran("spUPDATE_ARC_AUDIO_CHANGE", dicParameters);

            return fsRESULT;
        }

    }
}