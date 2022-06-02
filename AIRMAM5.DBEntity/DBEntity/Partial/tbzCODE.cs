using AIRMAM5.DBEntity.Models.CodeSet;
using AIRMAM5.DBEntity.Models.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 代碼表(明細)  tbzCODE
    /// </summary>
    [MetadataType(typeof(tbzCODEMetadata))]
    public partial class tbzCODE
    {
        /// <summary>
        /// 初始 (fsTYPE=C)
        /// </summary>
        public tbzCODE() : base()
        {
            fsCODE_ID = string.Empty;
            fsCODE = string.Empty;
            fsNAME = string.Empty;
            fsENAME = string.Empty;
            fnORDER = 99;
            fsSET = string.Empty;
            fsIS_ENABLED = IsTrueFalseEnum.Y.ToString();
            IsEnabled = true;
            fsTYPE = Enum.GetName(typeof(CodeSetTypeEnum), CodeSetTypeEnum.C);
            fdCREATED_DATE = DateTime.Now;
            fsCREATED_BY = string.Empty;
            fdUPDATED_DATE = null;
            fsUPDATED_BY = string.Empty;
        }

        /// <summary>
        ///  TO tbzCODE()
        /// </summary>
        /// <param name="r"></param>
        public tbzCODE(CodeDataModel r)//(spGET_CODE_Result r)
        {
            if (r == null) return;
            fsCODE_ID = r.fsCODE_ID;
            fsCODE = r.fsCODE;
            fsNAME = r.fsNAME ?? string.Empty;
            fsENAME = r.fsENAME ?? string.Empty;
            fnORDER = r.fnORDER;
            fsSET = r.fsSET ?? string.Empty;
            fsNOTE = r.fsNOTE ?? string.Empty;
            fsIS_ENABLED = r.IsEnabled ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString(); //r.fsIS_ENABLED;
            IsEnabled = r.IsEnabled; //r.fsIS_ENABLED.ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
            //fsTYPE = r.fsTYPE;
            //fdCREATED_DATE = r.fdCREATED_DATE;
            //fsCREATED_BY = r.fsCREATED_BY;
            //fdUPDATED_DATE = r.fdUPDATED_DATE;
            //fsUPDATED_BY = r.fsUPDATED_BY;
        }

        #region 額外欄位參數
        /// <summary>
        /// 是否啟用 bool
        /// </summary>
        [Display(Name = "可選")]
        public bool IsEnabled { get; set; } = true;
        #endregion

        /// <summary>
        /// 代碼表(明細)  tbzCODEMetadata
        /// </summary>
        public class tbzCODEMetadata
        {
            /// <summary>
            /// 代碼(主表)項目
            /// </summary>
            [Required]
            [Display(Name = "代碼項目")]
            public string fsCODE_ID { get; set; }

            /// <summary>
            /// 代碼
            /// </summary>
            [Required]
            [Display(Name = "子代碼")]
            public string fsCODE { get; set; }

            /// <summary>
            /// 名稱
            /// </summary>
            [Required]
            [Display(Name = "名稱")]
            public string fsNAME { get; set; }

            /// <summary>
            /// 英文名稱
            /// </summary>
            [Display(Name = "英文名稱")]
            public string fsENAME { get; set; }

            /// <summary>
            /// 顯示順序
            /// </summary>
            [Required]
            [Display(Name = "顯示順序")]
            public int fnORDER { get; set; }

            /// <summary>
            /// 設定
            /// </summary>
            [Display(Name = "設定")]
            public string fsSET { get; set; }

            /// <summary>
            /// 備註
            /// </summary>
            [Display(Name = "備註")]
            public string fsNOTE { get; set; }

            /// <summary>
            /// 是否啟用
            /// </summary>
            [Display(Name = "可選")]
            public string fsIS_ENABLED { get; set; }

            /// <summary>
            /// 代碼種類
            /// </summary>
            [Display(Name = "代碼種類")]
            [Required]
            public string fsTYPE { get; set; }

            [Display(Name = "建立時間")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public DateTime fdCREATED_DATE { get; set; }

            [Display(Name = "建立者")]
            public string fsCREATED_BY { get; set; }

            [Display(Name = "修改時間")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public DateTime? fdUPDATED_DATE { get; set; }

            [Display(Name = "修改者")]
            public string fsUPDATED_BY { get; set; }

            [JsonIgnore]    //Tips: 在特定導覽屬性上套用 [JsonIgnore] 屬性(Attribute)即可防止參考循環問題發生
            public virtual tbzCODE_SET tbzCODE_SET { get; set; }
        }
    }
}
