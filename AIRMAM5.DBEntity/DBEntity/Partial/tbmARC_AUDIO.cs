using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 媒資-聲音主檔資料表 tbmARC_AUDIO
    /// </summary>
    [MetadataType(typeof(tbmARC_AUDIOMeatadata))]
    public partial class tbmARC_AUDIO
    {
        //public tbmARC_AUDIO()
        //{
        //    fsTITLE = string.Empty;
        //    fsDESCRIPTION = string.Empty;
        //    fsSUBJECT_ID = string.Empty;
        //    fsFILE_STATUS = string.Empty;
        //    fnFILE_SECRET = 0;
        //    fsFILE_TYPE = string.Empty;
        //    fsFILE_TYPE_H = string.Empty;
        //    fsFILE_TYPE_L = string.Empty;
        //    fsFILE_SIZE = string.Empty;
        //    fsFILE_SIZE_H = string.Empty;
        //    fsFILE_SIZE_L = string.Empty;
        //    fsFILE_PATH = string.Empty;
        //    fsFILE_PATH_H = string.Empty;
        //    fsFILE_PATH_L = string.Empty;
        //    fxMEDIA_INFO = string.Empty;
        //    fsALBUM = string.Empty;
        //    fsALBUM_TITLE = string.Empty;
        //    fsALBUM_ARTISTS = string.Empty;
        //    fsALBUM_PERFORMERS = string.Empty;
        //    fsALBUM_COMPOSERS = string.Empty;
        //    fnALBUM_YEAR = 0;
        //    fsALBUM_COPYRIGHT = string.Empty;
        //    fsALBUM_GENRES = string.Empty;
        //    fsALBUM_COMMENT = string.Empty;
        //    fcALBUM_PICTURE = "N";
        //    fdBEG_TIME = 0.0M;
        //    fdEND_TIME = 0.0M;
        //    fdDURATION = 0.0M;
        //    fdCREATED_DATE = DateTime.Now;
        //    fsCREATED_BY = string.Empty;
        //    fdUPDATED_DATE = null;
        //    fsUPDATED_BY = string.Empty;
        //    //
        //    fsATTRIBUTE1 = string.Empty;
        //    fsATTRIBUTE2 = string.Empty;
        //    fsATTRIBUTE3 = string.Empty;
        //    fsATTRIBUTE4 = string.Empty;
        //    fsATTRIBUTE5 = string.Empty;
        //    fsATTRIBUTE6 = string.Empty;
        //    fsATTRIBUTE7 = string.Empty;
        //    fsATTRIBUTE8 = string.Empty;
        //    fsATTRIBUTE9 = string.Empty;
        //    fsATTRIBUTE10 = string.Empty;
        //    fsATTRIBUTE11 = string.Empty;
        //    fsATTRIBUTE12 = string.Empty;
        //    fsATTRIBUTE13 = string.Empty;
        //    fsATTRIBUTE14 = string.Empty;
        //    fsATTRIBUTE15 = string.Empty;
        //    fsATTRIBUTE16 = string.Empty;
        //    fsATTRIBUTE17 = string.Empty;
        //    fsATTRIBUTE18 = string.Empty;
        //    fsATTRIBUTE19 = string.Empty;
        //    fsATTRIBUTE20 = string.Empty;
        //    fsATTRIBUTE21 = string.Empty;
        //    fsATTRIBUTE22 = string.Empty;
        //    fsATTRIBUTE23 = string.Empty;
        //    fsATTRIBUTE24 = string.Empty;
        //    fsATTRIBUTE25 = string.Empty;
        //    fsATTRIBUTE26 = string.Empty;
        //    fsATTRIBUTE27 = string.Empty;
        //    fsATTRIBUTE28 = string.Empty;
        //    fsATTRIBUTE29 = string.Empty;
        //    fsATTRIBUTE30 = string.Empty;
        //    fsATTRIBUTE31 = string.Empty;
        //    fsATTRIBUTE32 = string.Empty;
        //    fsATTRIBUTE33 = string.Empty;
        //    fsATTRIBUTE34 = string.Empty;
        //    fsATTRIBUTE35 = string.Empty;
        //    fsATTRIBUTE36 = string.Empty;
        //    fsATTRIBUTE37 = string.Empty;
        //    fsATTRIBUTE38 = string.Empty;
        //    fsATTRIBUTE39 = string.Empty;
        //    fsATTRIBUTE40 = string.Empty;
        //    fsATTRIBUTE41 = string.Empty;
        //    fsATTRIBUTE42 = string.Empty;
        //    fsATTRIBUTE43 = string.Empty;
        //    fsATTRIBUTE44 = string.Empty;
        //    fsATTRIBUTE45 = string.Empty;
        //    fsATTRIBUTE46 = string.Empty;
        //    fsATTRIBUTE47 = string.Empty;
        //    fsATTRIBUTE48 = string.Empty;
        //    fsATTRIBUTE49 = string.Empty;
        //    fsATTRIBUTE50 = string.Empty;
        //    fsATTRIBUTE51 = string.Empty;
        //    fsATTRIBUTE52 = string.Empty;
        //    fsATTRIBUTE53 = string.Empty;
        //    fsATTRIBUTE54 = string.Empty;
        //    fsATTRIBUTE55 = string.Empty;
        //    fsATTRIBUTE56 = string.Empty;
        //    fsATTRIBUTE57 = string.Empty;
        //    fsATTRIBUTE58 = string.Empty;
        //    fsATTRIBUTE59 = string.Empty;
        //    fsATTRIBUTE60 = string.Empty;
        //}

        public class tbmARC_AUDIOMeatadata
        {
            public string fsFILE_NO { get; set; }
            public string fsTITLE { get; set; }
            public string fsDESCRIPTION { get; set; }
            public string fsSUBJECT_ID { get; set; }
            public string fsFILE_STATUS { get; set; }
            public short fnFILE_SECRET { get; set; }
            public string fsFILE_TYPE { get; set; }
            public string fsFILE_TYPE_H { get; set; }
            public string fsFILE_TYPE_L { get; set; }
            public string fsFILE_SIZE { get; set; }
            public string fsFILE_SIZE_H { get; set; }
            public string fsFILE_SIZE_L { get; set; }
            public string fsFILE_PATH { get; set; }
            public string fsFILE_PATH_H { get; set; }
            public string fsFILE_PATH_L { get; set; }
            public string fxMEDIA_INFO { get; set; }
            public string fsALBUM { get; set; }
            public string fsALBUM_TITLE { get; set; }
            public string fsALBUM_ARTISTS { get; set; }
            public string fsALBUM_PERFORMERS { get; set; }
            public string fsALBUM_COMPOSERS { get; set; }
            public Nullable<short> fnALBUM_YEAR { get; set; }
            public string fsALBUM_COPYRIGHT { get; set; }
            public string fsALBUM_GENRES { get; set; }
            public string fsALBUM_COMMENT { get; set; }
            public string fcALBUM_PICTURE { get; set; }
            public decimal fdBEG_TIME { get; set; }
            public decimal fdEND_TIME { get; set; }
            public decimal fdDURATION { get; set; }
            public System.DateTime fdCREATED_DATE { get; set; }
            public string fsCREATED_BY { get; set; }
            public Nullable<System.DateTime> fdUPDATED_DATE { get; set; }
            public string fsUPDATED_BY { get; set; }
            public string fsATTRIBUTE1 { get; set; }
            public string fsATTRIBUTE2 { get; set; }
            public string fsATTRIBUTE3 { get; set; }
            public string fsATTRIBUTE4 { get; set; }
            public string fsATTRIBUTE5 { get; set; }
            public string fsATTRIBUTE6 { get; set; }
            public string fsATTRIBUTE7 { get; set; }
            public string fsATTRIBUTE8 { get; set; }
            public string fsATTRIBUTE9 { get; set; }
            public string fsATTRIBUTE10 { get; set; }
            public string fsATTRIBUTE11 { get; set; }
            public string fsATTRIBUTE12 { get; set; }
            public string fsATTRIBUTE13 { get; set; }
            public string fsATTRIBUTE14 { get; set; }
            public string fsATTRIBUTE15 { get; set; }
            public string fsATTRIBUTE16 { get; set; }
            public string fsATTRIBUTE17 { get; set; }
            public string fsATTRIBUTE18 { get; set; }
            public string fsATTRIBUTE19 { get; set; }
            public string fsATTRIBUTE20 { get; set; }
            public string fsATTRIBUTE21 { get; set; }
            public string fsATTRIBUTE22 { get; set; }
            public string fsATTRIBUTE23 { get; set; }
            public string fsATTRIBUTE24 { get; set; }
            public string fsATTRIBUTE25 { get; set; }
            public string fsATTRIBUTE26 { get; set; }
            public string fsATTRIBUTE27 { get; set; }
            public string fsATTRIBUTE28 { get; set; }
            public string fsATTRIBUTE29 { get; set; }
            public string fsATTRIBUTE30 { get; set; }
            public string fsATTRIBUTE31 { get; set; }
            public string fsATTRIBUTE32 { get; set; }
            public string fsATTRIBUTE33 { get; set; }
            public string fsATTRIBUTE34 { get; set; }
            public string fsATTRIBUTE35 { get; set; }
            public string fsATTRIBUTE36 { get; set; }
            public string fsATTRIBUTE37 { get; set; }
            public string fsATTRIBUTE38 { get; set; }
            public string fsATTRIBUTE39 { get; set; }
            public string fsATTRIBUTE40 { get; set; }
            public string fsATTRIBUTE41 { get; set; }
            public string fsATTRIBUTE42 { get; set; }
            public string fsATTRIBUTE43 { get; set; }
            public string fsATTRIBUTE44 { get; set; }
            public string fsATTRIBUTE45 { get; set; }
            public string fsATTRIBUTE46 { get; set; }
            public string fsATTRIBUTE47 { get; set; }
            public string fsATTRIBUTE48 { get; set; }
            public string fsATTRIBUTE49 { get; set; }
            public string fsATTRIBUTE50 { get; set; }
            public string fsATTRIBUTE51 { get; set; }
            public string fsATTRIBUTE52 { get; set; }
            public string fsATTRIBUTE53 { get; set; }
            public string fsATTRIBUTE54 { get; set; }
            public string fsATTRIBUTE55 { get; set; }
            public string fsATTRIBUTE56 { get; set; }
            public string fsATTRIBUTE57 { get; set; }
            public string fsATTRIBUTE58 { get; set; }
            public string fsATTRIBUTE59 { get; set; }
            public string fsATTRIBUTE60 { get; set; }

            [JsonIgnore]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<tbmARC_AUDIO_D> tbmARC_AUDIO_D { get; set; }
            [JsonIgnore]
            public virtual tbmSUBJECT tbmSUBJECT { get; set; }
        }
    }
}
