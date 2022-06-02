using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Procedure
{
    /// <summary>
    /// DB Procedure 報表 : spRPT_GET_xxxxxxxxx
    /// </summary>
    public class ReportGetService
    {
        protected AIRMAM5DBEntities _db;
        readonly SerilogService _serilogService;

        public ReportGetService()
        {
            //_db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }
        
        /// <summary>
        /// 每日入庫統計表  【dbo.spGET_ARC_LIST_SUM】
        /// <para> 日期區間使用 between 抓資料 </para>
        /// </summary>
        /// <param name="sdate">開始日期 yyyy-MM-dd </param>
        /// <param name="edate">結束日期 yyyy-MM-dd </param>
        /// <returns></returns>
        public List<spGET_ARC_LIST_SUM_Result> GetUploadSum(string sdate, string edate)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_LIST_SUM(sdate, edate).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null) return new List<spGET_ARC_LIST_SUM_Result>();

                return query;
            }
        }

        /// <summary>
        /// 每日入庫明細表  【dbo.spGET_ARC_LIST_DETAIL】
        /// </summary>
        /// <param name="sdate">開始日期 yyyy-MM-dd </param>
        /// <param name="edate">結束日期 yyyy-MM-dd </param>
        /// <returns></returns>
        public List<spGET_ARC_LIST_DETAIL_Result> GetUploadDetail(string sdate, string edate)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_LIST_DETAIL(sdate, edate).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null) return new List<spGET_ARC_LIST_DETAIL_Result>();

                return query;
            }
        }

        /// <summary>
        /// 每日調用統計表  【dbo.spRPT_GET_BOOKING_LIST_SUM】
        /// <para> 日期區間使用 between 抓資料 </para>
        /// </summary>
        /// <param name="sdate">開始日期 yyyy-MM-dd </param>
        /// <param name="edate">結束日期 yyyy-MM-dd </param>
        /// <returns></returns>
        public List<spGET_BOOKING_LIST_SUM_Result> GetBookingSum(string sdate, string edate)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_BOOKING_LIST_SUM(sdate, edate).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null) return new List<spGET_BOOKING_LIST_SUM_Result>();

                return query;
            }
        }

        /// <summary>
        /// 每日調用明細表  【dbo.spGET_BOOKING_LIST_DETAIL】
        /// <para> 日期區間使用 between 抓資料 </para>
        /// </summary>
        /// <param name="sdate">開始日期 yyyy-MM-dd </param>
        /// <param name="edate">結束日期 yyyy-MM-dd </param>
        /// <returns></returns>
        public List<spGET_BOOKING_LIST_DETAIL_Result> GetBookingDetail(string sdate, string edate)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_BOOKING_LIST_DETAIL(sdate, edate).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null) return new List<spGET_BOOKING_LIST_DETAIL_Result>();

                return query;
            }
        }

        #region >>>>> DashBoard
        /// <summary>
        /// DashBoard 統計{入庫/調出}值區塊資料
        /// </summary>
        /// <param name="category">✘✘✘-統計值區塊分類: 1今日入庫,2今日調用,3本月入庫,4本月調用 </param>
        /// <param name="act"> 操作別: upload 入庫, 調用 Booking </param>
        /// <param name="type"> 統計別: 今日 today , 本月 month , 昨日 yesterday </param>
        /// <returns></returns>
        public DashboardViewModel.StatisticsModel GetStatistics(string act, string type)//(int category)//
        {
            DashboardViewModel.StatisticsModel model = new DashboardViewModel.StatisticsModel();
            string _lab = "今日";

            DateTime sysdt = DateTime.Now,
                _sdt = new DateTime(sysdt.Year, sysdt.Month, sysdt.Day, 0, 0, 0),   //DAY, 當日
                _edt = _sdt;// new DateTime(sysdt.AddDays(1).Year, sysdt.AddDays(1).Month, sysdt.AddDays(1).Day, 0, 0, 0);

            switch (type.ToUpper())
            {
                case "YESTERDAY":
                    _sdt = new DateTime(sysdt.AddDays(-1).Year, sysdt.AddDays(-1).Month, sysdt.AddDays(-1).Day, 0, 0, 0);
                    _edt = _sdt;// new DateTime(sysdt.Year, sysdt.Month, sysdt.Day, 0, 0, 0);
                    _lab = "昨日";
                    break;
                case "MONTH":
                    _sdt = new DateTime(sysdt.Year, sysdt.Month, 1, 0, 0, 0);
                    _edt = _sdt.AddMonths(1).AddDays(-1);// new DateTime(sysdt.AddMonths(1).Year, sysdt.AddMonths(1).Month, 1, 0, 0, 0);
                    _lab = "本月";
                    break;
                case "TODAY":
                default:
                    _sdt = new DateTime(sysdt.Year, sysdt.Month, sysdt.Day, 0, 0, 0);
                    _edt = _sdt;
                    _lab = "今日";
                    break;
            }

            switch (act.ToUpper())
            {
                case "UPLOAD":
                    model.Category = type.ToUpper() == "YESTERDAY" ? 5 : (type.ToUpper() == "MONTH" ? 3 : 1);
                    model.Counts = this.GetUploadSum(string.Format("{0:yyyy-MM-dd}", _sdt), string.Format("{0:yyyy-MM-dd}", _edt))
                        .Select(s => new
                        {
                            s.fsSUBJ_TITLE,
                            Counts = (s.fnCOUNT_V ?? 0) + (s.fnCOUNT_A ?? 0) + (s.fnCOUNT_P ?? 0) + (s.fnCOUNT_D ?? 0)
                        }).Sum(s => s.Counts);
                    model.LabelStr = _lab + "入庫";
                    break;
                case "BOOKING":
                    model.Category = type.ToUpper() == "YESTERDAY" ? 6 : (type.ToUpper() == "MONTH" ? 4 : 2);
                    model.Counts = this.GetBookingSum(string.Format("{0:yyyy-MM-dd}", _sdt), string.Format("{0:yyyy-MM-dd}", _edt))
                        .Select(s => new
                        {
                            s.fdDATE,
                            Counts = (s.fnCOUNT_V ?? 0) + (s.fnCOUNT_A ?? 0) + (s.fnCOUNT_P ?? 0) + (s.fnCOUNT_D ?? 0)
                        }).Sum(s => s.Counts);
                    model.LabelStr = _lab + "調用";
                    break;
                default:
                    //
                    break;
            }

            return model;
        }

        /// <summary>
        /// 取得今日前10名調用者 
        /// </summary>
        /// <remarks> added_20210519<br/>
        ///     [fsTYPE] : BOOKING	轉檔調用, COPYFILE 複製調用
        /// </remarks>
        /// <returns> </returns>
        public List<spGET_BOOKING_TODAY_TOP_10_Result> GetBookingTodayTop()
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_BOOKING_TODAY_TOP_10().DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_BOOKING_TODAY_TOP_10_Result>();

                return query ?? null;
            }
        }
        /// <summary>
        /// 主機入庫作業量
        /// </summary>
        /// <remark> added_20210519 </remark>
        /// <returns></returns>
        public List<spGET_WORK_ARC_CURRENT_QTY_Result> GetWorkArcQty()
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_WORK_ARC_CURRENT_QTY().DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_WORK_ARC_CURRENT_QTY_Result>();

                return query ?? null;
            }
        }

        /// <summary>
        /// 主機調用作業量
        /// </summary>
        /// <remark> added_20210519 </remark>
        /// <returns></returns>
        public List<spGET_WORK_BOOK_CURRENT_QTY_Result> GetWorkBookQty()
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_WORK_BOOK_CURRENT_QTY().DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_WORK_BOOK_CURRENT_QTY_Result>();

                return query ?? null;
            }
        }
        #endregion


    }
}
