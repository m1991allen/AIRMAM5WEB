using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 預編詮釋資料 : spGET_ARC_PRE
    /// </summary>
    [MetadataType(typeof(spGET_ARC_PRE_ResultMetadata))]
    public partial class spGET_ARC_PRE_Result
    {
        public spGET_ARC_PRE_Result()
        {
            fnPRE_ID = -1;
            fsNAME = "無";
            fsTYPE = string.Empty;
            fnTEMP_ID = -1;
            fsTITLE = string.Empty;
            fsDESCRIPTION = string.Empty;
        }
        
        /// <summary>
        /// 預編詮釋資料 spGET_ARC_PRE欄位參數
        /// </summary>
        public class spGET_ARC_PRE_ResultMetadata
        {
            /// <summary>
            /// 預編詮釋資料 編號 fnPRE_ID
            /// </summary>
            [Display(Name = "編號")]
            public long fnPRE_ID { get; set; }
            /// <summary>
            /// 預編名稱 fsNAME
            /// </summary>
            [Required]
            [Display(Name = "預編名稱")]
            public string fsNAME { get; set; }

            /// <summary>
            /// 類型 fsTYPE = S、V、A、P、D
            /// </summary>
            [Required]
            [Display(Name = "類型")]
            public string fsTYPE { get; set; }

            /// <summary>
            /// 樣板 fnTEMP_ID
            /// </summary>
            [Required]
            [Range(1, int.MaxValue)]
            [Display(Name = "樣板")]
            public int fnTEMP_ID { get; set; }

            /// <summary>
            /// 標題 fsTITLE
            /// </summary>
            [Required]
            [Display(Name = "標題")]
            public string fsTITLE { get; set; }

            /// <summary>
            /// 描述 fsDESCRIPTION
            /// </summary>
            [Display(Name = "描述")]
            public string fsDESCRIPTION { get; set; }

            /// <summary>
            /// 建立時間
            /// </summary>
            [Display(Name = "建立時間")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public DateTime fdCREATED_DATE { get; set; }
            [Display(Name = "建立人員")]
            public string fsCREATED_BY { get; set; }
            [Display(Name = "最後異動時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }
            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }
            /// <summary>
            /// 建立人員顯示名稱
            /// </summary>
            public string fsCREATED_BY_NAME { get; set; }
            /// <summary>
            /// 最後異動帳號顯示名稱
            /// </summary>
            public string fsUPDATED_BY_NAME { get; set; }

            /// <summary>
            /// 類型名稱 fsTYPE_NAME
            /// </summary>
            [Display(Name = "類型")]
            public string fsTYPE_NAME { get; set; }
            /// <summary>
            /// 樣板名稱 fsTEMP_NAME
            /// </summary>
            [Display(Name = "使用樣板")]
            public string fsTEMP_NAME { get; set; }

            #region 自訂欄位1~60
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
            #endregion
        }
    }
}
