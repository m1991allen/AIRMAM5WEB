using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 自訂欄位樣板 spGET_TEMPLATE 結果
    /// </summary>
    [MetadataType(typeof(spGET_TEMPLATE_ResultMetadata))]
    public partial class spGET_TEMPLATE_Result
    {
        public spGET_TEMPLATE_Result() : base() { }

        //public spGET_TEMPLATE_Result(spGET_TEMPLATE_Result get)
        //{
        //    this.fnTEMP_ID = get.fnTEMP_ID;
        //    this.fsNAME = get.fsNAME;
        //    this.fsTABLE = get.fsTABLE;
        //    this.C_sTABLENAME = get.C_sTABLENAME;
        //    this.fsDESCRIPTION = get.fsDESCRIPTION;
        //    this.fcIS_SEARCH = get.fcIS_SEARCH;
        //    this.IsSearch = get.fcIS_SEARCH == "Y" ? true : false;
        //    fdCREATED_DATE = get.fdCREATED_DATE;
        //    fsCREATED_BY = get.fsCREATED_BY;
        //    fsCREATED_BY_NAME = get.fsCREATED_BY_NAME;
        //    fdUPDATED_DATE = get.fdUPDATED_DATE;
        //    fsUPDATED_BY = get.fsUPDATED_BY;
        //    fsUPDATED_BY_NAME = get.fsUPDATED_BY_NAME;
        //}

        /// <summary>
        /// 是否要列為進階搜尋 (對應原欄位[fsIS_SEARCH])
        /// </summary>
        [Display(Name = "進階搜尋")]
        public bool IsSearch { get; set; } = false;

        /// <summary>
        /// 自訂欄位樣板 spGET_TEMPLATE 結果
        /// </summary>
        public class spGET_TEMPLATE_ResultMetadata
        {
            /// <summary>
            /// 編號
            /// </summary>
            [Display(Name = "編號")]
            public int fnTEMP_ID { get; set; }

            [Display(Name = "樣板名稱")]
            public string fsNAME { get; set; }

            /// <summary>
            /// 樣板分類:提供使用的目的資料表 fsCODE_ID='TEMP001'
            /// </summary>
            [Display(Name = "樣板分類")]
            public string fsTABLE { get; set; }
            /// <summary>
            /// 樣板分類:提供使用的目的資料表 fsCODE_ID='TEMP001'
            /// </summary>
            public string C_sTABLENAME { get; set; }

            [Display(Name = "樣板描述")]
            public string fsDESCRIPTION { get; set; }
            /// <summary>
            /// 是否進階查詢
            /// </summary>
            [Display(Name = "進階查詢")]
            public string fcIS_SEARCH { get; set; }

            [Display(Name = "建立時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime fdCREATED_DATE { get; set; }

            [Display(Name = "建立帳號")]
            public string fsCREATED_BY { get; set; }

            [Display(Name = "最後異動時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }

            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }

            public string fsCREATED_BY_NAME { get; set; }

            public string fsUPDATED_BY_NAME { get; set; }
        }
    }
}
