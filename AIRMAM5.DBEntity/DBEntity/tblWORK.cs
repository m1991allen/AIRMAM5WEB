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
    
    public partial class tblWORK
    {
        public long fnWORK_ID { get; set; }
        public long fnGROUP_ID { get; set; }
        public string fsTYPE { get; set; }
        public string fsPARAMETERS { get; set; }
        public string fsSTATUS { get; set; }
        public string fsPROGRESS { get; set; }
        public string fsPRIORITY { get; set; }
        public System.DateTime fdSTART_WORK_TIME { get; set; }
        public Nullable<System.DateTime> fdSTIME { get; set; }
        public Nullable<System.DateTime> fdETIME { get; set; }
        public string fsRESULT { get; set; }
        public string fsNOTE { get; set; }
        public System.DateTime fdCREATED_DATE { get; set; }
        public string fsCREATED_BY { get; set; }
        public Nullable<System.DateTime> fdUPDATED_DATE { get; set; }
        public string fsUPDATED_BY { get; set; }
        public string C_ITEM_TYPE { get; set; }
        public string C_ITEM_ID { get; set; }
        public string C_sDESCRIPTION { get; set; }
        public string C_SM_VOLUME_NAME { get; set; }
        public string C_ITEM_SET1 { get; set; }
        public string C_ITEM_SET2 { get; set; }
        public string C_ITEM_SET3 { get; set; }
        public string C_ITEM_SET4 { get; set; }
        public string C_APPROVE_STATUS { get; set; }
        public Nullable<System.DateTime> C_APPROVE_DATE { get; set; }
        public string C_APPROVE_BY { get; set; }
        public string C_ARC_TYPE { get; set; }
        public string C_APPROVE_MEMO { get; set; }
    }
}
