using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 依照查詢條件取出t_tbmARC主檔資料 結果
    /// </summary>
    [MetadataType(typeof(sp_t_GET_ARC_VIDEO_ResultMetadata))]
    public partial class sp_t_GET_ARC_VIDEO_Result
    {
        /// <summary>
        /// 依照查詢條件取出t_tbmARC主檔資料 結果
        /// </summary>
        public class sp_t_GET_ARC_VIDEO_ResultMetadata
        {
            /// <summary>
            /// 編號
            /// </summary>
            [Display(Name = "編號")]
            public long fnINDEX { get; set; }
            /// <summary>
            /// 檔案編號
            /// </summary>
            [Display(Name = "檔案編號")]
            public string fsFILE_NO { get; set; }
            /// <summary>
            /// 標題
            /// </summary>
            [Display(Name = "標題")]
            public string fsTITLE { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            [Display(Name = "描述")]
            public string fsDESCRIPTION { get; set; }
            /// <summary>
            /// 主題編號
            /// </summary>
            [Display(Name = "主題編號")]
            public string fsSUBJECT_ID { get; set; }
            /// <summary>
            /// 檔案狀態
            /// </summary>
            [Display(Name = "檔案狀態")]
            public string fsFILE_STATUS { get; set; }
            /// <summary>
            /// 機密等級
            /// </summary>
            [Display(Name = "機密等級")]
            public short fnFILE_SECRET { get; set; }
            /// <summary>
            /// 原始副檔名
            /// </summary>
            [Display(Name = "原始副檔名")]
            public string fsFILE_TYPE { get; set; }
            /// <summary>
            /// 高解副檔名
            /// </summary>
            [Display(Name = "高解副檔名")]
            public string fsFILE_TYPE_H { get; set; }
            /// <summary>
            /// 低解副檔名
            /// </summary>
            [Display(Name = "低解副檔名")]
            public string fsFILE_TYPE_L { get; set; }
            /// <summary>
            /// 原始檔案大小
            /// </summary>
            [Display(Name = "原始檔案大小")]
            public string fsFILE_SIZE { get; set; }
            /// <summary>
            /// 高解檔案大小
            /// </summary>
            [Display(Name = "高解檔案大小")]
            public string fsFILE_SIZE_H { get; set; }
            /// <summary>
            /// 低解檔案大小
            /// </summary>
            [Display(Name = "低解檔案大小")]
            public string fsFILE_SIZE_L { get; set; }
            /// <summary>
            /// 原始檔案路徑
            /// </summary>
            [Display(Name = "原始檔案路徑")]
            public string fsFILE_PATH { get; set; }
            /// <summary>
            /// 高解檔案路徑
            /// </summary>
            [Display(Name = "高解檔案路徑")]
            public string fsFILE_PATH_H { get; set; }
            /// <summary>
            /// 低解檔案路徑
            /// </summary>
            [Display(Name = "低解檔案路徑")]
            public string fsFILE_PATH_L { get; set; }
            /// <summary>
            /// 數位檔資訊
            /// </summary>
            [Display(Name = "數位檔資訊")]
            public string fxMEDIA_INFO { get; set; }
            /// <summary>
            /// 代表圖
            /// </summary>
            [Display(Name = "代表圖")]
            public string fsHEAD_FRAME { get; set; }
            /// <summary>
            /// 起始Timecode
            /// </summary>
            [Display(Name = "編號")]
            public decimal fdBEG_TIME { get; set; }
            /// <summary>
            /// 結束Timecode
            /// </summary>
            [Display(Name = "編號")]
            public decimal fdEND_TIME { get; set; }
            /// <summary>
            /// 影片長度
            /// </summary>
            [Display(Name = "影片長度")]
            public decimal fdDURATION { get; set; }
            /// <summary>
            /// 畫質
            /// </summary>
            [Display(Name = "畫質")]
            public string fsRESOL_TAG { get; set; }
            /// <summary>
            /// 建立時間
            /// </summary>
            [Display(Name = "建立時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime fdCREATED_DATE { get; set; }
            /// <summary>
            /// 建立帳號
            /// </summary>
            [Display(Name = "建立帳號")]
            public string fsCREATED_BY { get; set; }
            /// <summary>
            /// 最後異動時間
            /// </summary>
            [Display(Name = "最後異動時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }
            /// <summary>
            /// 最後異動帳號
            /// </summary>
            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }

            public string fsATTRIBUTE1 { get; set; }
            public string fsATTRIBUTE2 { get; set; }
            public string fsATTRIBUTE3 { get; set; }
            public string fsATTRIBUTE4 { get; set; }
            public string fsATTRIBUTE5 { get; set; }
            public string fsATTRIBUTE6 { get; set; }
            public string fsATTRIBUTE7 { get; set; }
            public string fsATTRIBUTE8 { get; set; }
            public string fsATTRIBUTE9 { get; set; }
            public string fsATTRIBUTE10 { get; set; }
            public string fsATTRIBUTE11 { get; set; }
            public string fsATTRIBUTE12 { get; set; }
            public string fsATTRIBUTE13 { get; set; }
            public string fsATTRIBUTE14 { get; set; }
            public string fsATTRIBUTE15 { get; set; }
            public string fsATTRIBUTE16 { get; set; }
            public string fsATTRIBUTE17 { get; set; }
            public string fsATTRIBUTE18 { get; set; }
            public string fsATTRIBUTE19 { get; set; }
            public string fsATTRIBUTE20 { get; set; }
            public string fsATTRIBUTE21 { get; set; }
            public string fsATTRIBUTE22 { get; set; }
            public string fsATTRIBUTE23 { get; set; }
            public string fsATTRIBUTE24 { get; set; }
            public string fsATTRIBUTE25 { get; set; }
            public string fsATTRIBUTE26 { get; set; }
            public string fsATTRIBUTE27 { get; set; }
            public string fsATTRIBUTE28 { get; set; }
            public string fsATTRIBUTE29 { get; set; }
            public string fsATTRIBUTE30 { get; set; }
            public string fsATTRIBUTE31 { get; set; }
            public string fsATTRIBUTE32 { get; set; }
            public string fsATTRIBUTE33 { get; set; }
            public string fsATTRIBUTE34 { get; set; }
            public string fsATTRIBUTE35 { get; set; }
            public string fsATTRIBUTE36 { get; set; }
            public string fsATTRIBUTE37 { get; set; }
            public string fsATTRIBUTE38 { get; set; }
            public string fsATTRIBUTE39 { get; set; }
            public string fsATTRIBUTE40 { get; set; }
            public string fsATTRIBUTE41 { get; set; }
            public string fsATTRIBUTE42 { get; set; }
            public string fsATTRIBUTE43 { get; set; }
            public string fsATTRIBUTE44 { get; set; }
            public string fsATTRIBUTE45 { get; set; }
            public string fsATTRIBUTE46 { get; set; }
            public string fsATTRIBUTE47 { get; set; }
            public string fsATTRIBUTE48 { get; set; }
            public string fsATTRIBUTE49 { get; set; }
            public string fsATTRIBUTE50 { get; set; }
            public string fsATTRIBUTE51 { get; set; }
            public string fsATTRIBUTE52 { get; set; }
            public string fsATTRIBUTE53 { get; set; }
            public string fsATTRIBUTE54 { get; set; }
            public string fsATTRIBUTE55 { get; set; }
            public string fsATTRIBUTE56 { get; set; }
            public string fsATTRIBUTE57 { get; set; }
            public string fsATTRIBUTE58 { get; set; }
            public string fsATTRIBUTE59 { get; set; }
            public string fsATTRIBUTE60 { get; set; }
            /// <summary>
            /// 建立者
            /// </summary>
            [Display(Name = "建立者")]
            public string fsCREATED_BY_NAME { get; set; }
            /// <summary>
            /// 最後異動者
            /// </summary>
            [Display(Name = "最後異動者")]
            public string fsUPDATED_BY_NAME { get; set; }
            /// <summary>
            /// 主題路徑
            /// </summary>
            [Display(Name = "主題路徑")]
            public string C_sSUBJ_PATH { get; set; }
            /// <summary>
            /// 低解播放路徑
            /// </summary>
            [Display(Name = "低解播放路徑")]
            public string C_sFILE_URL_L { get; set; }
            /// <summary>
            /// 代表圖路徑
            /// </summary>
            [Display(Name = "代表圖路徑")]
            public string fsHEAD_FRAME_URL { get; set; }
        }
    }
}
