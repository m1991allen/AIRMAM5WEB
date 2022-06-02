using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 主題檔 資料表 tbmSUBJECT
    /// </summary>
    [MetadataType(typeof(tbmSUBJECTMetadata))]
    public partial class tbmSUBJECT
    {
        /// <summary>
        /// 初始
        /// </summary>
        public tbmSUBJECT(long dirid)
        {
            fsSUBJ_ID = string.Empty;
            fsTITLE = string.Empty;
            fnDIR_ID = dirid;
            fsTYPE1 = string.Empty;
            fsTYPE2 = string.Empty;
            fsTYPE3 = string.Empty;
            fsDESCRIPTION = string.Empty;
            fdCREATED_DATE = DateTime.Now;
            fsCREATED_BY = string.Empty;
            fdUPDATED_DATE = null;
            fsUPDATED_BY = string.Empty;
            fsATTRIBUTE1 = string.Empty;
            fsATTRIBUTE2 = string.Empty;
            fsATTRIBUTE3 = string.Empty;
            fsATTRIBUTE4 = string.Empty;
            fsATTRIBUTE5 = string.Empty;
            fsATTRIBUTE6 = string.Empty;
            fsATTRIBUTE7 = string.Empty;
            fsATTRIBUTE8 = string.Empty;
            fsATTRIBUTE9 = string.Empty;
            fsATTRIBUTE10 = string.Empty;
            fsATTRIBUTE11 = string.Empty;
            fsATTRIBUTE12 = string.Empty;
            fsATTRIBUTE13 = string.Empty;
            fsATTRIBUTE14 = string.Empty;
            fsATTRIBUTE15 = string.Empty;
            fsATTRIBUTE16 = string.Empty;
            fsATTRIBUTE17 = string.Empty;
            fsATTRIBUTE18 = string.Empty;
            fsATTRIBUTE19 = string.Empty;
            fsATTRIBUTE20 = string.Empty;
            fsATTRIBUTE21 = string.Empty;
            fsATTRIBUTE22 = string.Empty;
            fsATTRIBUTE23 = string.Empty;
            fsATTRIBUTE24 = string.Empty;
            fsATTRIBUTE25 = string.Empty;
            fsATTRIBUTE26 = string.Empty;
            fsATTRIBUTE27 = string.Empty;
            fsATTRIBUTE28 = string.Empty;
            fsATTRIBUTE29 = string.Empty;
            fsATTRIBUTE30 = string.Empty;
            fsATTRIBUTE31 = string.Empty;
            fsATTRIBUTE32 = string.Empty;
            fsATTRIBUTE33 = string.Empty;
            fsATTRIBUTE34 = string.Empty;
            fsATTRIBUTE35 = string.Empty;
            fsATTRIBUTE36 = string.Empty;
            fsATTRIBUTE37 = string.Empty;
            fsATTRIBUTE38 = string.Empty;
            fsATTRIBUTE39 = string.Empty;
            fsATTRIBUTE40 = string.Empty;
            fsATTRIBUTE41 = string.Empty;
            fsATTRIBUTE42 = string.Empty;
            fsATTRIBUTE43 = string.Empty;
            fsATTRIBUTE44 = string.Empty;
            fsATTRIBUTE45 = string.Empty;
            fsATTRIBUTE46 = string.Empty;
            fsATTRIBUTE47 = string.Empty;
            fsATTRIBUTE48 = string.Empty;
            fsATTRIBUTE49 = string.Empty;
            fsATTRIBUTE50 = string.Empty;
        }

        /// <summary>
        /// 預編詮釋資料 copy: fsATTRIBUTE1 ~ fsATTRIBUTE50
        /// </summary>
        /// <param name="m"></param>
        public tbmSUBJECT(tbmARC_PRE m)
        {
            fsTITLE = m.fsTITLE;
            fsDESCRIPTION = m.fsDESCRIPTION;
            fsTYPE1 = string.Empty;
            fsTYPE2 = string.Empty;
            fsTYPE3 = string.Empty;
            fdCREATED_DATE = DateTime.Now;
            fsCREATED_BY = string.Empty;
            fdUPDATED_DATE = null;
            fsUPDATED_BY = string.Empty;
            fsATTRIBUTE1 = m.fsATTRIBUTE1;
            fsATTRIBUTE2 = m.fsATTRIBUTE2;
            fsATTRIBUTE3 = m.fsATTRIBUTE3;
            fsATTRIBUTE4 = m.fsATTRIBUTE4;
            fsATTRIBUTE5 = m.fsATTRIBUTE5;
            fsATTRIBUTE6 = m.fsATTRIBUTE6;
            fsATTRIBUTE7 = m.fsATTRIBUTE7;
            fsATTRIBUTE8 = m.fsATTRIBUTE8;
            fsATTRIBUTE9 = m.fsATTRIBUTE9;
            fsATTRIBUTE10 = m.fsATTRIBUTE10;
            fsATTRIBUTE11 = m.fsATTRIBUTE11;
            fsATTRIBUTE12 = m.fsATTRIBUTE12;
            fsATTRIBUTE13 = m.fsATTRIBUTE13;
            fsATTRIBUTE14 = m.fsATTRIBUTE14;
            fsATTRIBUTE15 = m.fsATTRIBUTE15;
            fsATTRIBUTE16 = m.fsATTRIBUTE16;
            fsATTRIBUTE17 = m.fsATTRIBUTE17;
            fsATTRIBUTE18 = m.fsATTRIBUTE18;
            fsATTRIBUTE19 = m.fsATTRIBUTE19;
            fsATTRIBUTE20 = m.fsATTRIBUTE20;
            fsATTRIBUTE21 = m.fsATTRIBUTE21;
            fsATTRIBUTE22 = m.fsATTRIBUTE22;
            fsATTRIBUTE23 = m.fsATTRIBUTE23;
            fsATTRIBUTE24 = m.fsATTRIBUTE24;
            fsATTRIBUTE25 = m.fsATTRIBUTE25;
            fsATTRIBUTE26 = m.fsATTRIBUTE26;
            fsATTRIBUTE27 = m.fsATTRIBUTE27;
            fsATTRIBUTE28 = m.fsATTRIBUTE28;
            fsATTRIBUTE29 = m.fsATTRIBUTE29;
            fsATTRIBUTE30 = m.fsATTRIBUTE30;
            fsATTRIBUTE31 = m.fsATTRIBUTE31;
            fsATTRIBUTE32 = m.fsATTRIBUTE32;
            fsATTRIBUTE33 = m.fsATTRIBUTE33;
            fsATTRIBUTE34 = m.fsATTRIBUTE34;
            fsATTRIBUTE35 = m.fsATTRIBUTE35;
            fsATTRIBUTE36 = m.fsATTRIBUTE36;
            fsATTRIBUTE37 = m.fsATTRIBUTE37;
            fsATTRIBUTE38 = m.fsATTRIBUTE38;
            fsATTRIBUTE39 = m.fsATTRIBUTE39;
            fsATTRIBUTE40 = m.fsATTRIBUTE40;
            fsATTRIBUTE41 = m.fsATTRIBUTE41;
            fsATTRIBUTE42 = m.fsATTRIBUTE42;
            fsATTRIBUTE43 = m.fsATTRIBUTE43;
            fsATTRIBUTE44 = m.fsATTRIBUTE44;
            fsATTRIBUTE45 = m.fsATTRIBUTE45;
            fsATTRIBUTE46 = m.fsATTRIBUTE46;
            fsATTRIBUTE47 = m.fsATTRIBUTE47;
            fsATTRIBUTE48 = m.fsATTRIBUTE48;
            fsATTRIBUTE49 = m.fsATTRIBUTE49;
            fsATTRIBUTE50 = m.fsATTRIBUTE50;
        }

        /// <summary>
        /// 主題檔資料表 Matadata: tbmSUBJECT
        /// </summary>
        public class tbmSUBJECTMetadata
        {
            [Display(Name = "主題檔案編號")]
            public string fsSUBJ_ID { get; set; }
            [Display(Name = "標題")]
            public string fsTITLE { get; set; }
            [Display(Name = "描述")]
            public string fsDESCRIPTION { get; set; }
            [Display(Name = "所屬目錄")]
            public long fnDIR_ID { get; set; }
            [Display(Name = "分類1")]
            public string fsTYPE1 { get; set; }
            [Display(Name = "分類2")]
            public string fsTYPE2 { get; set; }
            [Display(Name = "分類3")]
            public string fsTYPE3 { get; set; }

            #region 自訂欄位1~50
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
            #endregion

            [Display(Name = "建立時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime fdCREATED_DATE { get; set; }
            [Display(Name = "建立帳號")]
            public string fsCREATED_BY { get; set; }
            [Display(Name = "最後異動時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }
            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }

            [JsonIgnore]
            public virtual ICollection<tbmARC_AUDIO> tbmARC_AUDIO { get; set; }
            [JsonIgnore]
            public virtual ICollection<tbmARC_DOC> tbmARC_DOC { get; set; }
            [JsonIgnore]
            public virtual ICollection<tbmARC_PHOTO> tbmARC_PHOTO { get; set; }
            [JsonIgnore]
            public virtual ICollection<tbmARC_VIDEO> tbmARC_VIDEO { get; set; }
        }
    }
}
