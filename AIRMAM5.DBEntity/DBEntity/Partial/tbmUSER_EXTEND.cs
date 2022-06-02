using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 使用者附加資料表 tbmUSER_EXTEND
    /// </summary>
    [MetadataType(typeof(tbmUSER_EXTENDMetadata))]
    public partial class tbmUSER_EXTEND
    {

        public class tbmUSER_EXTENDMetadata
        {
            public string fsUSER_ID { get; set; }
            public string fsSIGNALR_CONNECT_ID { get; set; }
            public bool? fbPWD_RESTORE { get; set; }
            public string fsRESTORE_BY { get; set; }
            public DateTime? fdRESTORE_DATE { get; set; }
            public string fsVerifyCode { get; set; }
            public DateTime? fdVerifyDate { get; set; }
            public DateTime? fdChangeEmailDate { get; set; }
            public DateTime? fdEmailConfirmDate { get; set; }
            public System.DateTime fdCREATED_DATE { get; set; }
            public string fsCREATED_BY { get; set; }
            public DateTime? fdUPDATED_DATE { get; set; }
            public string fsUPDATED_BY { get; set; }

            [JsonIgnore]    //Tip: 在特定導覽屬性上套用 [JsonIgnore] 屬性(Attribute)即可防止參考循環問題發生
            public virtual tbmUSERS tbmUSERS { get; set; }
        }
    }
}
