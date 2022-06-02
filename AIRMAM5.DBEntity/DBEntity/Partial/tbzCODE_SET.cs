using AIRMAM5.DBEntity.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 代碼主表  tbzCODE_SET
    /// </summary>
    [MetadataType(typeof(tbzCODE_SETMetadata))]
    public partial class tbzCODE_SET
    {
        /// <summary>
        /// 初始 (fsTYPE=C)
        /// </summary>
        public tbzCODE_SET(string codeid = null)
        {
            fsCODE_ID = string.IsNullOrEmpty(codeid) ? string.Empty : codeid;
            fsTITLE = string.Empty;
            fsTBCOL = string.Empty;
            fsNOTE = string.Empty;
            fsIS_ENABLED = IsTrueFalseEnum.Y.ToString();
            IsEnabled = true;
            fsTYPE = Enum.GetName(typeof(CodeSetTypeEnum), CodeSetTypeEnum.C);
            fdCREATED_DATE = DateTime.Now;
            fsCREATED_BY = string.Empty;
            fdUPDATED_DATE = null;
            fsUPDATED_BY = string.Empty;
        }

        /// <summary>
        /// spGET_CODE_SET_Result() TO tbzCODE_SET()
        /// </summary>
        /// <param name="r"></param>
        public tbzCODE_SET(spGET_CODE_SET_Result r)
        {
            if (r == null) new tbzCODE_SET();
            fsCODE_ID = r.fsCODE_ID;
            fsTITLE = r.fsTITLE;
            fsTBCOL = r.fsTBCOL;
            fsNOTE = r.fsNOTE;
            fsIS_ENABLED = r.fsIS_ENABLED;
            IsEnabled = r.fsIS_ENABLED.ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
            fsTYPE = r.fsTYPE;
            fdCREATED_DATE = r.fdCREATED_DATE;
            fsCREATED_BY = r.fsCREATED_BY;
            fdUPDATED_DATE = r.fdUPDATED_DATE;
            fsUPDATED_BY = r.fsUPDATED_BY;
        }

        #region 額外欄位參數
        /// <summary>
        /// 是否啟用 bool
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        #endregion

        /// <summary>
        /// 代碼主表 tbzCODE_SET Metadata
        /// </summary>
        public class tbzCODE_SETMetadata
        {
            /// <summary>
            /// 代碼編號
            /// </summary>
            [Required]
            [Display(Name = "代碼編號")]
            public string fsCODE_ID { get; set; }

            /// <summary>
            /// 名稱
            /// </summary>
            [Required]
            [Display(Name = "名稱")]
            public string fsTITLE { get; set; } = string.Empty;

            /// <summary>
            /// 自訂欄位(目前不使用)
            /// </summary>
            //[Display(Name = "")]
            public string fsTBCOL { get; set; } = string.Empty;

            /// <summary>
            /// 備註
            /// </summary>
            [Display(Name = "備註")]
            public string fsNOTE { get; set; } = string.Empty;

            /// <summary>
            /// 是否啟用
            /// </summary>
            [Display(Name = "是否啟用")]
            public string fsIS_ENABLED { get; set; } = IsTrueFalseEnum.Y.ToString();

            /// <summary>
            /// 代碼種類
            /// </summary>
            [Display(Name = "代碼種類")]
            public string fsTYPE { get; set; } = Enum.GetName(typeof(CodeSetTypeEnum), CodeSetTypeEnum.C);

            [Display(Name = "建立者")]
            public string fsCREATED_BY { get; set; } = string.Empty;

            [Display(Name = "建立時間")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public DateTime fdCREATED_DATE { get; set; }

            [Display(Name = "修改者")]
            public string fsUPDATED_BY { get; set; } = string.Empty;

            [Display(Name = "修改時間")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public DateTime? fdUPDATED_DATE { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            [JsonIgnore]    //Tip: 在特定導覽屬性上套用 [JsonIgnore] 屬性(Attribute)即可防止參考循環問題發生
            public virtual ICollection<tbzCODE> tbzCODE { get; set; }
        }
    }
}
