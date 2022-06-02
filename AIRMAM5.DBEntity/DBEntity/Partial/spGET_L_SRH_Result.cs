using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 取出檢索紀錄 : spGET_L_SRH
    /// </summary>
    [MetadataType(typeof(spGET_L_SRH_ResultMetadata))]
    public partial class spGET_L_SRH_Result
    {
        /// <summary>
        /// 取出檢索紀錄 : spGET_L_SRH
        /// </summary>
        public class spGET_L_SRH_ResultMetadata
        {
            /// <summary>
            /// 檢索紀錄編號
            /// </summary>
            [Display(Name = "編號")]
            public long fnSRH_ID { get; set; }

            /// <summary>
            /// 檢索紀錄查詢條件
            /// </summary>
            [Display(Name = "查詢條件")]
            public string fsSTATEMENT { get; set; }

            /// <summary>
            /// 建立時間
            /// </summary>
            [Display(Name = "建立時間")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true, NullDisplayText = "")]
            public DateTime fdCREATED_DATE { get; set; }

            [Display(Name = "建立人員")]
            public string fsCREATED_BY { get; set; }

            public string fsCREATED_BY_NAME { get; set; }
        }
    }
}
