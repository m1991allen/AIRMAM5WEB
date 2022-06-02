using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    [MetadataType(typeof(tbmBOOKING_TMetadata))]
    public partial class tbmBOOKING_T
    {
        public class tbmBOOKING_TMetadata
        {

            [JsonIgnore]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<tbmBOOKING> tbmBOOKING { get; set; }
        }
    }
}
