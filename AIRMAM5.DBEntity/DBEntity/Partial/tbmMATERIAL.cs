using AIRMAM5.DBEntity.Models.Material;
using System;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 預借清單 tbmMATERIAL
    /// </summary>
    //[MetadataType(typeof(tbmMATERIALMetadata))]
    public partial class tbmMATERIAL
    {
        public tbmMATERIAL() { }

        public tbmMATERIAL(MaterialCreateModel m)
        {
            fsTYPE = m.FileCategory;
            fsFILE_NO = m.FileNo;
            fsDESCRIPTION = m.MaterialDesc ?? string.Empty;
            fsNOTE = m.MaterialNote ?? string.Empty;
            fsPARAMETER = m.ParameterStr ?? string.Empty;
            fdCREATED_DATE = DateTime.Now;
            fsCREATED_BY = string.Empty;
            fdUPDATED_DATE = null;
            fsUPDATED_BY = string.Empty;
        }

        //public class tbmMATERIALMetadata
        //{
        //    public long fnMATERIAL_ID { get; set; }
        //    public string fsMARKED_BY { get; set; }
        //    public string fsTYPE { get; set; }
        //    public string fsFILE_NO { get; set; }
        //    public string fsDESCRIPTION { get; set; }
        //    public string fsNOTE { get; set; }
        //    public string fsPARAMETER { get; set; }
        //    public DateTime fdCREATED_DATE { get; set; }
        //    public string fsCREATED_BY { get; set; }
        //    public DateTime? fdUPDATED_DATE { get; set; }
        //    public string fsUPDATED_BY { get; set; }
        //}
    }
}
