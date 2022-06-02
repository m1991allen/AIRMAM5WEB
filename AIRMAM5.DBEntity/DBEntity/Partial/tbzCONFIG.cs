using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 系統設定檔-tbzCONFIG
    /// </summary>
    //[MetadataType(typeof(tbzCONFIGMetadata))]
    public partial class tbzCONFIG
    {
        public tbzCONFIG() { }

        public tbzCONFIG(spGET_CONFIG_Result m)
        {
            fsKEY = m.fsKEY;
            fsVALUE = m.fsVALUE;
            fsTYPE = m.fsTYPE;
            fsDESCRIPTION = m.fsDESCRIPTION ?? string.Empty;
            fdCREATED_DATE = m.fdCREATED_DATE;
            fsCREATED_BY = m.fsCREATED_BY;
            fdUPDATED_DATE = m.fdUPDATED_DATE;
            fsUPDATED_BY = m.fsUPDATED_BY;
        }

        public class tbzCONFIGMetadata
        {
            /// <summary>
            /// 設定主鍵
            /// </summary>
            public string fsKEY { get; set; }
            /// <summary>
            /// 設定內容
            /// </summary>
            public string fsVALUE { get; set; }
            /// <summary>
            /// 使用類型
            /// </summary>
            public string fsTYPE { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            public string fsDESCRIPTION { get; set; }

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
        }
    }
}
