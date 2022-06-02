using AIRMAM5.DBEntity.Models.Subject;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 媒資-影片.段落描述資料表 tbmARC_VIDEO_D
    /// </summary>
    [MetadataType(typeof(tbmARC_VIDEO_DMetadata))]
    public partial class tbmARC_VIDEO_D
    {
        /// <summary>
        /// 媒資-影片.段落描述資料表 tbmARC_VIDEO_D
        /// </summary>
        public tbmARC_VIDEO_D()
        {
            fnSEQ_NO = 0;
            fsDESCRIPTION = string.Empty;
            fdBEG_TIME = 0M;
            fdEND_TIME = 0M;
            fsCREATED_BY = string.Empty;
            fdCREATED_DATE = DateTime.Now;
            fsUPDATED_BY = string.Empty;
            fdUPDATED_DATE = null;
        }

        /// <summary>
        /// 媒資-影片.段落描述資料表 tbmARC_VIDEO_D
        /// </summary>
        /// <param name="m">新增/編輯段落描述Model <see cref="ParagraphCUViewModel"/></param>
        public tbmARC_VIDEO_D(ParagraphCUViewModel m)
        {
            fsFILE_NO = m.fsFILE_NO ?? string.Empty;
            fnSEQ_NO = m.SeqNo;
            fsDESCRIPTION = m.Description;
            fdBEG_TIME = m.BegTime;
            fdEND_TIME = m.EndTime;
            fsCREATED_BY = string.Empty;
            fdCREATED_DATE = DateTime.Now;
            fsUPDATED_BY = string.Empty;
            fdUPDATED_DATE = null;
        }

        public class tbmARC_VIDEO_DMetadata
        {
            public string fsFILE_NO { get; set; }
            public int fnSEQ_NO { get; set; }
            public string fsDESCRIPTION { get; set; }
            public decimal fdBEG_TIME { get; set; }
            public decimal fdEND_TIME { get; set; }
            public DateTime fdCREATED_DATE { get; set; }
            public string fsCREATED_BY { get; set; }
            public DateTime? fdUPDATED_DATE { get; set; }
            public string fsUPDATED_BY { get; set; }

            [JsonIgnore]
            public virtual tbmARC_VIDEO tbmARC_VIDEO { get; set; }
        }
    }
}
