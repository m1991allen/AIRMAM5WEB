using AIRMAM5.DBEntity.Models.Announce;
using AIRMAM5.DBEntity.Models.Search;
using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.Shared
{
    /// <summary>
    /// DashBoard 資料model
    /// </summary>
    public class DashboardViewModel
    {
        public DashboardViewModel() { }

        #region ---主要區塊model ---
        public List<StatisticsModel> StatisticsData { get; set; } = new List<StatisticsModel>();
        /// <summary>
        /// 統計圖Chart資料model
        /// </summary>
        public ChartModel Charts { get; set; } = new ChartModel();

        /*  Marked_20200331:決議不顯示「最新入庫資料」
        /// <summary>
        /// 最新入庫資料model
        /// </summary>
        public List<LastUploadModel> NewUploadData { get; set; } = new List<LastUploadModel>(); */

        /// <summary>
        /// 熱索關鍵字統計資料model
        /// </summary>
        public List<HotkeyModel> HotkeyData { get; set; } = new List<HotkeyModel>();

        /// <summary>
        /// 系統公告資料model
        /// </summary>
        public List<AnnouncePublicViewModel> AnnounceData { get; set; } = new List<AnnouncePublicViewModel>();

        /// <summary>
        /// Dashboard.今日前10調用者資料
        /// </summary>
        /// <remarks> added_20210519 </remarks>
        public BookingTop10 BookingTodayTOP { get; set; } = new BookingTop10();
        /// <summary>
        /// Dashboard.主機調用作業量(目前) --透過SignalR更新
        /// </summary>
        public GaugeViewData WorkBookQty { get; set; }
        /// <summary>
        /// Dashboard.主機入庫作業量(目前) --透過SignalR更新
        /// </summary>
        public GaugeViewData WorkArcQty { get; set; }
        #endregion

        /// <summary>
        /// 統計圖表資料內容
        /// </summary>
        public class ChartModel
        {
            /// <summary>
            /// 初始(預設值-FOR TEST)
            /// </summary>
            public ChartModel()
            {
                Months = new string[] { "1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月" };
                //BranchData = new List<Branch>() {
                //    new Branch
                //    {
                //        LabelStr = "入庫 ",
                //        Counts = new int[] { 22, 33, 44, 55, 66, 77, 88, 99, 110, 122, 133, 144 }
                //    },
                //    new Branch
                //    {
                //        LabelStr = "調用 ",
                //        Counts = new int[] { 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 122, 133 }
                //    }
                //};
            }
            /// <summary>
            /// 月分Array
            /// </summary>
            public string[] Months { get; set; } //= { "1月", "2月", "3月", "4月", "5月", "6月" };
            /// <summary>
            /// 統計值
            /// </summary>
            public List<Branch> BranchData { get; set; }

            /// <summary>
            /// 統計圖表資料內容.分類與值
            /// </summary>
            public class Branch
            {
                /// <summary>
                /// 分類: 1今日入庫,2今日調用,3本月入庫,4本月調用,昨日入庫,昨日調用
                /// </summary>
                public string LabelStr { get; set; } = string.Empty;

                public int[] Counts { get; set; } = new int[] { };
            }
        }

        /// <summary>
        /// (入庫/調用)統計值
        /// </summary>
        public class StatisticsModel
        {
            public StatisticsModel() { }

            public StatisticsModel(int category, string labstr)
            {
                this.Category = category;
                this.LabelStr = labstr;
            }

            /// <summary>
            /// 分類: 1今日入庫,2今日調用,3本月入庫,4本月調用
            /// </summary>
            public int Category { get; set; }

            /// <summary>
            /// 分類: 1今日入庫,2今日調用,3本月入庫,4本月調用
            /// </summary>
            public string LabelStr { get; set; } = string.Empty;

            /// <summary>
            /// 數量(影/音/圖/文 加總)
            /// </summary>
            public int Counts { get; set; } = 0;
            //明細: spRPT_GET_ARC_LIST_SUM
        }

        /// <summary>
        /// 熱索關鍵字統計
        /// </summary>
        public class HotkeyModel : PopularKeywordsViewModel
        {
            /// <summary>
            /// 次數
            /// </summary>
            public decimal Counts { get; set; } = 0;

            /// <summary>
            /// 最近時間
            /// </summary>
            public string LastTime { get; set; } = string.Empty;
        }

        /* Marked_20200331:決議不顯示「最新入庫資料」
        /// <summary>
        /// 最新入庫資料
        /// </summary>
        public class LastUploadModel : SubjFileNoModel
        {
            public LastUploadModel() { }
            public LastUploadModel(spGET_ARC_LIST_DETAIL_Result m)
            {                
                //fsSUBJECT_ID = m.fsSUBJECT_ID;
                //fsFILE_NO = m.fsFILE_NO;
                Title = m.fsTITLE ?? string.Empty;
                FileCategory = GetEnums.GetDescriptionText<FileTypeEnum>(m.fsARC_TYPE); //m.fsARC_TYPE;
                FileExtension = m.fsFILE_TYPE;
                SubjectPath = m.Path_Name ?? string.Empty;
                CreatedDate = string.Format($"{m.fdCREATED_DATE:yyyy-MM-dd HH:mm:ss}"); //m.fdCREATED_DATE;
                CreatedByName = m.fsCREATED_BY_NAME ?? string.Empty;
            }

            //fsSUBJECT_ID
            //fsFILE_NO
            /// <summary>
            /// 標題
            /// </summary>
            public string Title { get; set; } = string.Empty;
            /// <summary>
            /// 分類: A聲音, D文件, P圖片, S主題, V影片 
            /// </summary>
            public string FileCategory { get; set; } = string.Empty;
            /// <summary>
            /// 檔案副檔名: mp4, jpg, docx, .....
            /// </summary>
            public string FileExtension { get; set; } = string.Empty;
            /// <summary>
            /// 主題檔案Path 路徑字串
            /// </summary>
            public string SubjectPath { get; set; } = string.Empty;
            /// <summary>
            /// 建立時間 fdCREATED_DATE
            /// </summary>
            public string CreatedDate { get; set; } = string.Empty;
            /// <summary>
            /// 建立帳號顯示名稱 fsCREATED_BY_NAME
            /// </summary>
            public string CreatedByName { get; set; } = string.Empty;
        }           */

        /// <summary>
        /// 今日前10調用者資料
        /// </summary>
        /// <remarks> added_20210519 </remarks>
        public class BookingTop10
        {
            /// <summary>
            /// 調用使用者 Array ["帳號(姓名)", "Admin(系統管理員)", ...]
            /// </summary>
            public string[] UserLabels { get; set; } = new string[] { };

            /// <summary>
            /// 調用者調用分類與值_List清單 
            /// </summary>
            /// <remarks> TIP: 分類會有多種 </remarks>
            public List<Branch> BookWorkVals { get; set; }

            /// <summary>
            /// 今日前10調用者資料.分類與值
            /// </summary>
            public class Branch
            {
                /// <summary>
                /// 分類: BOOKING, COPYFILE
                /// </summary>
                public string Type { get; set; }
                /// <summary>
                /// 分類: BOOKING轉檔調用, COPYFILE複製調用
                /// </summary>
                public string TypeStr { get; set; } = string.Empty;
                /// <summary>
                /// 值 Array [727, 589, 537, 543, 574,...]
                /// </summary>
                public int[] Values { get; set; } = new int[] { };
                /// <summary>
                /// 圖表資料BAR 顏色HEX, eg. red='#ff6384', grey='#e7e9ed', blue='#36a2eb', ...
                /// </summary>
                public string BarColorHex { get; set; } = string.Empty;
            }
        }

        /// <summary>
        /// Dashboard 測量圖表資料Model
        /// </summary>
        /// <remarks> added_20210519 </remarks>
        public class GaugeViewData
        {
            public string GaugeId { get; set; }
            public int[] GaugeData { get; set; } = new int[] { };
            public string GaugeColor { get; set; }
            public string GaugeTitle { get; set; }

        }
    }
}
