using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    [MetadataType(typeof(tbmRULEMetadata))]
    public partial class tbmRULE
    {
        public class tbmRULEMetadata
        {
            [JsonIgnore]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<tbmRULE_FILTER> tbmRULE_FILTER { get; set; }
        }
    }
}
