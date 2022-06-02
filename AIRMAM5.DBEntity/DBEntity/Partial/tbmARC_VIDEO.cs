using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 媒資-影片主檔資料表 tbmARC_VIDEO
    /// </summary>
    [MetadataType(typeof(tbmARC_VIDEOMetadata))]
    public partial class tbmARC_VIDEO
    {
        /// <summary>
        /// 
        /// </summary>
        public class tbmARC_VIDEOMetadata
        {
            //public string fsFILE_NO { get; set; }
            //public string fsTITLE { get; set; }
            //public string fsDESCRIPTION { get; set; }
            //public string fsSUBJECT_ID { get; set; }
            //public string fsFILE_STATUS { get; set; }
            //public short fnFILE_SECRET { get; set; }
            //public string fsFILE_TYPE { get; set; }
            //public string fsFILE_TYPE_H { get; set; }
            //public string fsFILE_TYPE_L { get; set; }
            //public string fsFILE_SIZE { get; set; }
            //public string fsFILE_SIZE_H { get; set; }
            //public string fsFILE_SIZE_L { get; set; }
            //public string fsFILE_PATH { get; set; }
            //public string fsFILE_PATH_H { get; set; }
            //public string fsFILE_PATH_L { get; set; }
            //public string fxMEDIA_INFO { get; set; }
            //public string fsHEAD_FRAME { get; set; }
            //public decimal fdBEG_TIME { get; set; }
            //public decimal fdEND_TIME { get; set; }
            //public decimal fdDURATION { get; set; }

            //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            //public string fsRESOL_TAG { get; set; }

            //public System.DateTime fdCREATED_DATE { get; set; }
            //public string fsCREATED_BY { get; set; }
            //public Nullable<System.DateTime> fdUPDATED_DATE { get; set; }
            //public string fsUPDATED_BY { get; set; }

            //public string fsATTRIBUTE1 { get; set; }
            //public string fsATTRIBUTE2 { get; set; }
            //public string fsATTRIBUTE3 { get; set; }
            //public string fsATTRIBUTE4 { get; set; }
            //public string fsATTRIBUTE5 { get; set; }
            //public string fsATTRIBUTE6 { get; set; }
            //public string fsATTRIBUTE7 { get; set; }
            //public string fsATTRIBUTE8 { get; set; }
            //public string fsATTRIBUTE9 { get; set; }
            //public string fsATTRIBUTE10 { get; set; }
            //public string fsATTRIBUTE11 { get; set; }
            //public string fsATTRIBUTE12 { get; set; }
            //public string fsATTRIBUTE13 { get; set; }
            //public string fsATTRIBUTE14 { get; set; }
            //public string fsATTRIBUTE15 { get; set; }
            //public string fsATTRIBUTE16 { get; set; }
            //public string fsATTRIBUTE17 { get; set; }
            //public string fsATTRIBUTE18 { get; set; }
            //public string fsATTRIBUTE19 { get; set; }
            //public string fsATTRIBUTE20 { get; set; }
            //public string fsATTRIBUTE21 { get; set; }
            //public string fsATTRIBUTE22 { get; set; }
            //public string fsATTRIBUTE23 { get; set; }
            //public string fsATTRIBUTE24 { get; set; }
            //public string fsATTRIBUTE25 { get; set; }
            //public string fsATTRIBUTE26 { get; set; }
            //public string fsATTRIBUTE27 { get; set; }
            //public string fsATTRIBUTE28 { get; set; }
            //public string fsATTRIBUTE29 { get; set; }
            //public string fsATTRIBUTE30 { get; set; }
            //public string fsATTRIBUTE31 { get; set; }
            //public string fsATTRIBUTE32 { get; set; }
            //public string fsATTRIBUTE33 { get; set; }
            //public string fsATTRIBUTE34 { get; set; }
            //public string fsATTRIBUTE35 { get; set; }
            //public string fsATTRIBUTE36 { get; set; }
            //public string fsATTRIBUTE37 { get; set; }
            //public string fsATTRIBUTE38 { get; set; }
            //public string fsATTRIBUTE39 { get; set; }
            //public string fsATTRIBUTE40 { get; set; }
            //public string fsATTRIBUTE41 { get; set; }
            //public string fsATTRIBUTE42 { get; set; }
            //public string fsATTRIBUTE43 { get; set; }
            //public string fsATTRIBUTE44 { get; set; }
            //public string fsATTRIBUTE45 { get; set; }
            //public string fsATTRIBUTE46 { get; set; }
            //public string fsATTRIBUTE47 { get; set; }
            //public string fsATTRIBUTE48 { get; set; }
            //public string fsATTRIBUTE49 { get; set; }
            //public string fsATTRIBUTE50 { get; set; }
            //public string fsATTRIBUTE51 { get; set; }
            //public string fsATTRIBUTE52 { get; set; }
            //public string fsATTRIBUTE53 { get; set; }
            //public string fsATTRIBUTE54 { get; set; }
            //public string fsATTRIBUTE55 { get; set; }
            //public string fsATTRIBUTE56 { get; set; }
            //public string fsATTRIBUTE57 { get; set; }
            //public string fsATTRIBUTE58 { get; set; }
            //public string fsATTRIBUTE59 { get; set; }
            //public string fsATTRIBUTE60 { get; set; }

            //public Nullable<int> fnWIDTH { get; set; }

            //public Nullable<int> fnHEIGHT { get; set; }

            //public string fsORI_FILE_NAME { get; set; }

            //public string fsS2T_CONTENT { get; set; }

            //public string fsLICENSE { get; set; }

            //[JsonIgnore]
            //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            //public virtual ICollection<tbmARC_VIDEO_D> tbmARC_VIDEO_D { get; set; }
            //[JsonIgnore]
            //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            //public virtual ICollection<tbmARC_VIDEO_K> tbmARC_VIDEO_K { get; set; }
            //[JsonIgnore]
            //public virtual tbmSUBJECT tbmSUBJECT { get; set; }
        }

    }
}
