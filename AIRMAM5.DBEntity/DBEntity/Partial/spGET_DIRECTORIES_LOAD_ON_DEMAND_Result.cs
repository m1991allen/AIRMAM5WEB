using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 依照查詢條件取出load on demand 目錄資料 結果
    /// </summary>
    [Metadatatype(typeof(spGET_DIRECTORIES_LOAD_ON_DEMAND_ResultMetadata))]
    public partial class spGET_DIRECTORIES_LOAD_ON_DEMAND_Result
    {
        public class spGET_DIRECTORIES_LOAD_ON_DEMAND_ResultMetadata
        {
            /// <summary>
            /// 目錄編號
            /// </summary>
            [Display(Name = "目錄編號")]
            public Nullable<long> fnDIR_ID { get; set; }
            /// <summary>
            /// 母節點
            /// </summary>
            public Nullable<long> fnPARENT_ID { get; set; }
            /// <summary>
            /// 目錄名稱
            /// </summary>
            [Display(Name = "目錄標題名稱")]
            public string fsNAME { get; set; }
            /// <summary>
            /// 目錄類型 (Q:末端節點，可新增主題)
            /// </summary>
            [Display(Name = "目錄標題名稱")]
            public string fsDIRTYPE { get; set; }
            /// <summary>
            /// 目錄開放類型
            /// </summary>
            [Display(Name = "目錄開放類型")]
            public string fsSHOWTYPE { get; set; }
            // <summary>
            /// 目錄管理權限 Y=直接 , y=繼承
            /// </summary>
            [Display(Name = "目錄管理權限")]
            public string C_ADMIN { get; set; }
            /// <summary>
            /// 主題/檔案權限 Y=直接 , y=繼承
            /// </summary>
            [Display(Name = "主題/檔案權限")]
            public string C_USER { get; set; }
            /// <summary>
            /// 路徑 如: "AIRMAM>>測試2>>測試(複)_1155"
            /// </summary>
            public string fsPATH_NAME { get; set; }
        }
    }
    //2019/12/11: 預存修改更新。
    //[MetadataType(typeof(spGET_DIRECTORIES_LOAD_ON_DEMAND_ResultMetadata))]
    //public partial class spGET_DIRECTORIES_LOAD_ON_DEMAND_Result
    //{
    //    public class spGET_DIRECTORIES_LOAD_ON_DEMAND_ResultMetadata
    //    {
    //        /// <summary>
    //        /// 目錄編號
    //        /// </summary>
    //        [Display(Name = "目錄編號")]
    //        public long fnDIR_ID { get; set; }
    //        /// <summary>
    //        /// 目錄名稱
    //        /// </summary>
    //        [Display(Name = "目錄標題名稱")]
    //        public string fsNAME { get; set; }
    //        /// <summary>
    //        /// 母節點
    //        /// </summary>
    //        public long fnPARENT_ID { get; set; }

    //        /// <summary>
    //        /// 目錄描述
    //        /// </summary>
    //        [Display(Name = "目錄描述")]
    //        public string fsDESCRIPTION { get; set; }

    //        /// <summary>
    //        /// 目錄類型 (Q:末端節點，可新增主題)
    //        /// </summary>
    //        [Display(Name = "目錄標題名稱")]
    //        public string fsDIRTYPE { get; set; }

    //        /// <summary>
    //        /// 目錄開放類型
    //        /// </summary>
    //        [Display(Name = "目錄開放類型")]
    //        public string fsSHOWTYPE { get; set; }
    //        // <summary>
    //        /// 目錄管理權限 Y=直接 , y=繼承
    //        /// </summary>
    //        [Display(Name = "目錄管理權限")]
    //        public string C_ADMIN { get; set; }
    //        /// <summary>
    //        /// 主題/檔案權限 Y=直接 , y=繼承
    //        /// </summary>
    //        [Display(Name = "主題/檔案權限")]
    //        public string C_USER { get; set; }
    //    }
    //}
}
