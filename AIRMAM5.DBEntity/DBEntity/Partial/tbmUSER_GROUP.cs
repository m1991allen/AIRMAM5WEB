using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 使用者角色對應資料表 tbmUSER_GROUP
    /// </summary>
    [MetadataType(typeof(tbmUSER_GROUPMetadata))]
    public partial class tbmUSER_GROUP
    {
        public class tbmUSER_GROUPMetadata
        {
            public string fsUSER_ID { get; set; }
            public string fsGROUP_ID { get; set; }
            public Nullable<System.DateTime> fdCREATED_DATE { get; set; }
            public string fsCREATED_BY { get; set; }
            public Nullable<System.DateTime> fdUPDATED_DATE { get; set; }
            public string fsUPDATED_BY { get; set; }
            public string Discriminator { get; set; }
            public string IdentityUser_Id { get; set; }

            [JsonIgnore]    //Tip: 在特定導覽屬性上套用 [JsonIgnore] 屬性(Attribute)即可防止參考循環問題發生
            public virtual tbmUSERS tbmUSERS { get; set; }
            [JsonIgnore]
            public virtual tbmGROUPS tbmGROUPS { get; set; }
        }
    }
}
