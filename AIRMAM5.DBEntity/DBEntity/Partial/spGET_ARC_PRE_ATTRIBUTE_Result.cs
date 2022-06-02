using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRMAM5.DBEntity.DBEntity
{
    [MetadataType(typeof(spGET_ARC_PRE_ATTRIBUTE_ResultMetadata))]
    public partial class spGET_ARC_PRE_ATTRIBUTE_Result
    {
        public class spGET_ARC_PRE_ATTRIBUTE_ResultMetadata
        {
            /// <summary>
            /// 自訂欄位代號, EX: fsATTRIBUTE1, fsATTRIBUTE2, ....
            /// </summary>
            [Display(Name = "欄位代號")]
            public string fsFIELD { get; set; }

            /// <summary>
            /// 欄位名稱
            /// </summary>
            [Display(Name = "欄位名稱")]
            public string fsFIELD_NAME { get; set; }

            /// <summary>
            /// 欄位型別
            /// </summary>
            [Display(Name = "欄位型別")]
            public string fsFIELD_TYPE { get; set; }

            /// <summary>
            /// 排序
            /// </summary>
            [Display(Name = "排序")]
            public Nullable<int> fnORDER { get; set; }

            [Display(Name = "代碼編號")]
            public string fsCODE_ID { get; set; }

            [Display(Name = "控制項類型")]
            public string fsCODE_CTRL { get; set; }

            /// <summary>
            /// 欄位值(資料內容)
            /// </summary>
            [Display(Name = "")]
            public string fsVALUE { get; set; }

            /// <summary>
            /// 是否可為NULL
            /// </summary>
            [Display(Name = "可為空值")]
            public string fsISNULLABLE { get; set; }

            /// <summary>
            /// 單選或多選
            /// </summary>
            [Display(Name = "選擇方式")]
            public Nullable<int> fnCODE_CNT { get; set; }

            /// <summary>
            /// 是否多行
            /// </summary>
            [Display(Name = "是否多行")]
            public string fsMULTILINE { get; set; }
        }
    }
}
