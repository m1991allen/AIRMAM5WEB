using AIRMAM5.DBEntity.Models.Rule;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 
    /// </summary>
    [MetadataType(typeof(tbmRULE_FILTERMetadata))]
    public partial class tbmRULE_FILTER
    {
        public tbmRULE_FILTER() { }

        public tbmRULE_FILTER(EditRuleFilterModel m)
        {
            fsRULECATEGORY = m.RuleCategory;
            fsTABLE = m.TargetTable;
            fsCOLUMN = m.FilterField;
            fsOPERATOR = m.Operator;
            fsFILTERVALUE = m.FilterValue;
            fnPRIORITY = m.Priority;
            fbISENABLED = m.IsEnabled;
            fsNOTE = m.Note ?? string.Empty;
            fsWHERE_CLAUSE = m.WhereClause;
            fsSCRIPTS = string.Empty;
            fdCREATED_DATE = DateTime.Now;
        }
    }

    public class tbmRULE_FILTERMetadata
    {
        public string fsRULECATEGORY { get; set; }
        public string fsTABLE { get; set; }
        public string fsCOLUMN { get; set; }
        public string fsOPERATOR { get; set; }
        public string fsFILTERVALUE { get; set; }
        public int fnPRIORITY { get; set; }
        public bool? fbISENABLED { get; set; }
        public string fsNOTE { get; set; }
        public string fsWHERE_CLAUSE { get; set; }
        public string fsSCRIPTS { get; set; }
        public DateTime fdCREATED_DATE { get; set; }
        public string fsCREATED_BY { get; set; } = string.Empty;
        public DateTime? fdUPDATED_DATE { get; set; }
        public string fsUPDATED_BY { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual tbmRULE tbmRULE { get; set; }
    }
}
