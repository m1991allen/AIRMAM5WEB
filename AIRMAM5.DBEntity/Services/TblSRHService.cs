using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Search;
using AIRMAM5.DBEntity.Models.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 檢索記錄 tblSRH , 檢索關鍵字[tblSRH_KW]
    /// </summary>
    public class TblSRHService
    {
        protected AIRMAM5DBEntities _db;
        readonly ISerilogService _serilogService;

        public TblSRHService()
        {
            //_db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }

        /// <summary>
        /// 指定條件 查詢 檢索紀錄 【spGET_L_SRH】
        /// </summary>
        /// <param name="id">檢索紀錄編號 fnSRH_ID</param>
        /// <param name="sdate">起始時間 </param>
        /// <param name="edate">結束時間 </param>
        /// <param name="username">系統帳號 </param>
        /// <returns></returns>
        public List<spGET_L_SRH_Result> GetByParam(long id = 0, string sdate = "", string edate = "", string username = "")
        {
            List<spGET_L_SRH_Result> result = new List<spGET_L_SRH_Result>();

            using (_db = new AIRMAM5DBEntities())
            {
                result = _db.spGET_L_SRH(id, sdate, edate, username).DefaultIfEmpty().ToList();
                if (result == null || result.FirstOrDefault() == null) result = new List<spGET_L_SRH_Result>();

                return result;
            }
        }

        /// <summary>
        /// Dashboard 熱索關鍵字統計
        /// </summary>
        /// <param name="days">統計天數, 預設7日 </param>
        /// <param name="tops">最熱門筆數, 預設10筆 </param>
        /// <returns></returns>
        public List<DashboardViewModel.HotkeyModel> GetHotKey(int days = 7, int tops = 10)
        {
            #region//Tips: 方法1
            //var query = _db.tblSRH_KW.AsEnumerable()
            //    .Where(x => x.fdCREATED_DATE >= DateTime.Now.AddDays(-days) && x.fdCREATED_DATE < DateTime.Now.AddDays(1)).ToList();
            //var queryby = query.GroupBy(g => g.fsKEYWORD)
            //    .Select(s => new DashboardViewModel.HotkeyModel
            //    {
            //        word = s.Key,
            //        Counts = s.Count(),
            //        LastTime = string.Format("0:yyyy-MM-dd HH:mm:ss", s.Max(m => m.fdCREATED_DATE))
            //    }).OrderByDescending(o => o.Counts).Take(tops).ToList();
            #endregion

            //Tips: 方法2
            using (_db = new AIRMAM5DBEntities())
            {
                var queryby = (from a in _db.tblSRH_KW.AsEnumerable().Where(x => x.fdCREATED_DATE >= DateTime.Now.AddDays(-days) && x.fdCREATED_DATE < DateTime.Now.AddDays(1))
                               group a by a.fsKEYWORD into g
                               select new DashboardViewModel.HotkeyModel
                               {
                                   word = g.Key,
                                   Counts = g.Count(),
                                   LastTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", g.Max(m => m.fdCREATED_DATE))
                               }).OrderByDescending(o => o.Counts).ThenByDescending(b=>b.LastTime)
                               .Take(tops).ToList();

                return queryby;
            }
        }

        #region ---------- CURD 【tblSRH】: 新 修 刪
        /// <summary>
        /// 新建 (1)檢索紀錄 tblSRH、(2)熱索關鍵字 tblSRH_KW : 【EF Create】
        /// </summary>
        /// <param name="md">(前端)檢索參數 </param>
        /// <param name="createby">建立者 </param>
        /// <returns></returns>
        public VerifyResult Create_SRH_KW(SearchParameterViewModel md, string createby)
        { 
            //記錄: 1檢索條件,寫入tblSRH，轉成JSON放入fsSTATEMENT；2檢索關鍵字,寫入tbSRH_KW。
            VerifyResult result = new VerifyResult();

            using (_db = new AIRMAM5DBEntities())
            {
                using (var _trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        #region =====熱索關鍵字 記錄===== (20200106_關鍵字空值, 不記錄)
                        List<tblSRH_KW> kWs = new List<tblSRH_KW>();
                        if (!string.IsNullOrEmpty(md.fsKEYWORD))
                        {
                            string _kwrd = string.IsNullOrEmpty(md.fsKEYWORD.Replace(" ", ""))
                                ? string.Empty
                                : Regex.Replace(md.fsKEYWORD, "\\s+", " ").Replace(" ", ",").Replace("&", ",").Replace("|", ",").Replace("!", ",").Replace("AND", ",").Replace("OR", ",").Replace("NOT", ",");

                            var _ary = _kwrd.Split(new char[] { ',' });
                            foreach (var t in _ary)
                            {
                                kWs.Add(new tblSRH_KW
                                {
                                    fsKEYWORD = t,
                                    fsCREATED_BY = createby,
                                    fdCREATED_DATE = DateTime.Now
                                });
                            }

                            //var _srhkwRepository = new GenericRepository<tblSRH_KW>();
                            //_srhkwRepository.CreateRange(kWs);
                            _db.tblSRH_KW.AddRange(kWs);
                        }
                        #endregion

                        #region =====檢索參數 記錄=====
                        md.fsKEYWORD = md.fsKEYWORD ?? string.Empty; //不要寫入null,轉為空值。
                        tblSRH sRH = new tblSRH
                        {
                            fsSTATEMENT = JsonConvert.SerializeObject(md),
                            fsCREATED_BY = createby,
                            fdCREATED_DATE = DateTime.Now
                        };

                        //var _srhRepository = new GenericRepository<tblSRH>();
                        //_srhRepository.Create(sRH);
                        _db.tblSRH.Add(sRH);
                        #endregion

                        _db.SaveChanges();
                        _trans.Commit();

                        result.IsSuccess = true;
                        result.Message = "檢索紀錄已新增.";
                        result.Data = new { tblSRH = sRH, tblSRH_KW = kWs };
                    }
                    catch (Exception ex)
                    {
                        _trans.Rollback();
                        result.IsSuccess = false;
                        result.Message = string.Format($"檢索紀錄新增失敗【{ex.Message}】");
                        #region Serilog.Err
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "TblSRHService",
                            Method = "[Create_SRH_KW]",
                            EventLevel = SerilogLevelEnum.Error,
                            Input = new { Params = md, Exception = ex },
                            LogString = "Exception",
                            ErrorMessage = string.Format($"檢索紀錄新增失敗【{ex.Message}】 ")
                        });
                        #endregion
                    }
                    finally
                    {
                        _trans.Dispose();
                    }
                }
            }

            return result;
        }

        #endregion

    }
}
