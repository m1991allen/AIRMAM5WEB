using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    [MetadataType(typeof(tbmBOOKINGMetadata))]
    public partial class tbmBOOKING
    {
        public class tbmBOOKINGMetadata
        {
            [JsonIgnore]
            public virtual tbmBOOKING_T tbmBOOKING_T { get; set; }
        }
    }
}
