using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRMAM5.DBEntity.DBEntity
{
    [MetadataType(typeof(tblSRHMetadata))]
    public partial class tblSRH
    {
        public tblSRH() { }
        public tblSRH(long srhId)
        {
            fnSRH_ID = srhId;
            fsSTATEMENT = string.Empty;
            fdCREATED_DATE = new DateTime();
            fsCREATED_BY = string.Empty;
        }

        public class tblSRHMetadata
        {
            public long fnSRH_ID { get; set; }
            public string fsSTATEMENT { get; set; }
            public DateTime fdCREATED_DATE { get; set; }
            public string fsCREATED_BY { get; set; }
        }
    }
}
