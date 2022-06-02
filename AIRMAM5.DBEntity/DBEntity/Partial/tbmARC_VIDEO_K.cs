using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 媒資-影片.關鍵影格資料表 tbmARC_VIDEO_K
    /// </summary>
    [MetadataType(typeof(tbmARC_VIDEO_KMetadata))]
    public partial class tbmARC_VIDEO_K
    {
        public tbmARC_VIDEO_K()
        {
            fsTIME = string.Empty;
            fsTITLE = string.Empty;
            fsDESCRIPTION = string.Empty;
            fsFILE_PATH = string.Empty;
            fsFILE_SIZE = string.Empty;
            fsFILE_TYPE = string.Empty;
            fsCREATED_BY = string.Empty;
            fdCREATED_DATE = DateTime.Now;
            fsUPDATED_BY = string.Empty;
            fdUPDATED_DATE = null;
        }

        public class tbmARC_VIDEO_KMetadata
        {
            public string fsFILE_NO { get; set; }
            public string fsTIME { get; set; }
            public string fsTITLE { get; set; }
            public string fsDESCRIPTION { get; set; }
            public string fsFILE_PATH { get; set; }
            public string fsFILE_SIZE { get; set; }
            public string fsFILE_TYPE { get; set; }
            public string fcHEAD_FRAME { get; set; }
            public DateTime fdCREATED_DATE { get; set; }
            public string fsCREATED_BY { get; set; }
            public DateTime? fdUPDATED_DATE { get; set; }
            public string fsUPDATED_BY { get; set; }

            [JsonIgnore]
            public virtual tbmARC_VIDEO tbmARC_VIDEO { get; set; }
        }
    }
}
