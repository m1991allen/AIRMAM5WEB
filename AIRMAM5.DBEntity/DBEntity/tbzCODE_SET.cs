//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace AIRMAM5.DBEntity.DBEntity
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbzCODE_SET
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbzCODE_SET()
        {
            this.tbzCODE = new HashSet<tbzCODE>();
        }
    
        public string fsCODE_ID { get; set; }
        public string fsTITLE { get; set; }
        public string fsTBCOL { get; set; }
        public string fsNOTE { get; set; }
        public string fsIS_ENABLED { get; set; }
        public string fsTYPE { get; set; }
        public string fsCREATED_BY { get; set; }
        public System.DateTime fdCREATED_DATE { get; set; }
        public string fsUPDATED_BY { get; set; }
        public Nullable<System.DateTime> fdUPDATED_DATE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbzCODE> tbzCODE { get; set; }
    }
}
