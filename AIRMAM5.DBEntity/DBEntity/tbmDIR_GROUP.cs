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
    
    public partial class tbmDIR_GROUP
    {
        public long fnDIR_ID { get; set; }
        public string fsGROUP_ID { get; set; }
        public string fsLIMIT_SUBJECT { get; set; }
        public string fsLIMIT_VIDEO { get; set; }
        public string fsLIMIT_AUDIO { get; set; }
        public string fsLIMIT_PHOTO { get; set; }
        public string fsLIMIT_DOC { get; set; }
        public System.DateTime fdCREATED_DATE { get; set; }
        public string fsCREATED_BY { get; set; }
        public Nullable<System.DateTime> fdUPDATED_DATE { get; set; }
        public string fsUPDATED_BY { get; set; }
    }
}
