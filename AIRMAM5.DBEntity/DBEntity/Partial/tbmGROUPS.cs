using AIRMAM5.DBEntity.Models.Role;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 群組/角色資料表 tbmGROUPS 
    /// </summary>
    [MetadataType(typeof(tbmGROUPSMetadata))]
    public partial class tbmGROUPS
    {
        public tbmGROUPS(GroupsViewModel m) { }

        /// <summary>
        /// 角色群組維護 view Metadata
        /// </summary>
        public class tbmGROUPSMetadata
        {
            [Display(Name = "群組識別碼")]
            public string fsGROUP_ID { get; set; } = string.Empty;

            [Display(Name = "群組名稱")]
            [MaxLength(50)]
            [Required]
            public string fsNAME { get; set; } = string.Empty;

            [Display(Name = "群組描述")]
            public string fsDESCRIPTION { get; set; } = string.Empty;

            [Display(Name = "群組類別")]
            public string fsTYPE { get; set; } = string.Empty;

            [Display(Name = "新增時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public Nullable<System.DateTime> fdCREATED_DATE { get; set; } = DateTime.Now;

            [Display(Name = "新增人員")]
            public string fsCREATED_BY { get; set; }

            [Display(Name = "修改時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public Nullable<System.DateTime> fdUPDATED_DATE { get; set; }

            [Display(Name = "修改人員")]
            public string fsUPDATED_BY { get; set; }

            public string Discriminator { get; set; } = string.Empty;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            [JsonIgnore]
            public virtual ICollection<tbmUSER_GROUP> tbmUSER_GROUP { get; set; }
        }
    }


}
