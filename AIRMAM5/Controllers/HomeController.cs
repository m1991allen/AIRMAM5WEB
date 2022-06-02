using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.DBEntity.Models.Shared;
using System.Net;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.Filters;
using AIRMAM5.DBEntity.Procedure;
using System.Diagnostics;
using AIRMAM5.DBEntity.Models.Function;
using AIRMAM5.DBEntity.Interface;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 登入後主頁、功能項目資料
    /// </summary>
    [Authorize]
    [InterceptorOfController(Keyword ="")]
    public class HomeController : BaseController
    {
        readonly UserFavoriteService _userFavoriteService;
        readonly ReportGetService _rptService;

        public HomeController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _funcsService = functionService;
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _userFavoriteService = new UserFavoriteService();
            _rptService = new ReportGetService();
        }

        /// <summary>
        /// 成功登入後開啟首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) return View("Login");
            if (Request.Cookies["User"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ////新增使用者可查詢的節點
            new UsersService(_serilogService).UpdateUserDirAuthr();
            return View();
        }

        /// <summary>
        /// 功能項目列表 _PartialPageFunctionList.cshtml
        /// </summary>
        /// <returns></returns>
        public ActionResult WebFunctionMenu()
        {
            List<FunctionMenuViewModel> rootnode = new List<FunctionMenuViewModel>();
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user == null) { return PartialView("_PartialPageFunctionList", rootnode); }

            try
            {
                //
                rootnode = _funcsService.GetFunctionsMenu(user.UserName);
            }
            catch (Exception ex)
            {
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Home",
                    Method = "[WebFunctionMenu]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Username = User.Identity.Name, Exception = ex },
                    LogString = "功能項目列表.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return PartialView("_PartialPageFunctionList", rootnode);
        }

        #region 【儲存工作紀錄:我的最愛】
        /// <summary>
        /// 取得 我的最愛紀錄
        /// </summary>
        [HttpGet]
        public JsonResult GetFavorite()
        {
            var _param = new { urnm = User.Identity.Name };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);

            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    result.IsSuccess = false;
                    result.Message = "使用者未驗證";
                    result.StatusCode = HttpStatusCode.Unauthorized;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var res = _userFavoriteService.GetUserFav(CurrentUser.Id);
                result.IsSuccess = true;
                result.Data = res;
                result.StatusCode = HttpStatusCode.OK;
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "我的最愛", "OK"),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Home",
                    Method = "[GetFavorite]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Exception = ex },
                    LogString = "我的最愛.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 加入 我的最愛紀錄
        /// </summary>
        /// <param name="id">功能項目id [fsFUNC_ID]</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddFavorite(string id)
        {
            var _param = new { funcID = id, urnm = User.Identity.Name };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();
            #region _檢查_
            if (!User.Identity.IsAuthenticated)
            {
                result.IsSuccess = false;
                result.Message = "使用者未驗證";
                result.StatusCode = HttpStatusCode.Unauthorized;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (_userFavoriteService.IsExists(User.Identity.GetUserId(), id))
            {
                result.Message = "已存在";
                result.StatusCode = HttpStatusCode.OK;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            try
            {
                tbmUSER_FAVORITE _FAVORITE = new tbmUSER_FAVORITE
                {
                    fsUSER_ID = User.Identity.GetUserId(),
                    fsLOGIN_ID = User.Identity.Name,
                    fsFUNC_ID = id,
                    fdCREATED_DATE = DateTime.Now,
                    fsCREATED_BY = User.Identity.Name
                };
                res = _userFavoriteService.CreateBy(_FAVORITE);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M001",        //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "我的最愛", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    Data = res.Data,
                    StatusCode = res.IsSuccess ? HttpStatusCode.Created : HttpStatusCode.ExpectationFailed
                };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Home",
                    Method = "[AddFavorite]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, Exception = ex },
                    LogString = "加入我的最愛.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 移除 我的最愛紀錄
        /// </summary>
        /// <param name="id">功能項目id [fsFUNC_ID]</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelFavorite(string id)
        {
            var _param = new { funcID = id, urnm = User.Identity.Name };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    result.IsSuccess = false;
                    result.Message = "使用者未驗證";
                    result.StatusCode = HttpStatusCode.Unauthorized;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                if (_userFavoriteService.IsExists(CurrentUser.Id, id))
                {
                    var get = _userFavoriteService.GetByUserId(CurrentUser.Id).Where(x => x.fsFUNC_ID == id).ToList();
                    res = _userFavoriteService.DeleteBy(get);
                    string _str = res.IsSuccess ? "成功" : "失敗";
                    #region _DB LOG
                    _tblLogService.Insert_L_Log(
                        TbzCodeIdEnum.MSG001.ToString(), 
                        "M003",     //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                        string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "我的最愛", _str),
                        string.Format($"位置: {Request.UserHostAddress} "),
                        JsonConvert.SerializeObject(_param),
                        User.Identity.Name);
                    #endregion

                    result = new ResponseResultModel(res)
                    {
                        Message=_str,
                        Records = _param,
                        StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                    };
                }
                ///刪除'我的最愛',回傳目前'我的最愛'項目list
                result.Data = _userFavoriteService.GetUserFav(CurrentUser.Id);
                #region _Serilog(Debug)
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Home",
                    Method = "[DelFavorite]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { param = _param, Result = result },
                    LogString = "移除我的最愛.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Home",
                    Method = "[DelFavorite]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, Exception = ex },
                    LogString = "移除我的最愛.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 【Dashboard】
        /// <summary>
        /// 儀錶板主頁
        /// </summary>
        /// <returns></returns>
        public ActionResult DashBoard()
        {
            if (!User.Identity.IsAuthenticated) return View("Login");

            DashboardViewModel model = new DashboardViewModel
            {
                StatisticsData = new List<DashboardViewModel.StatisticsModel>
                {
                    _rptService.GetStatistics("upload", "day"),//GetStatistics(1),//
                    _rptService.GetStatistics("booking", "day"),//GetStatistics(2),//
                    _rptService.GetStatistics("upload", "month"),//GetStatistics(3),//
                    _rptService.GetStatistics("booking", "month"),//GetStatistics(4),//
                    _rptService.GetStatistics("upload", "yesterday"),//GetStatistics(5),//
                    _rptService.GetStatistics("booking", "yesterday"),//GetStatistics(6)//
                }
            };

            #region >>>>> Tips: 圖表資料(過去12個月)
            int m = 12;
            string[] _month = new string[m];
            int[] _upload = new int[m], _booking = new int[m];
            DateTime sysdt = DateTime.Now,
                _sdt = new DateTime(sysdt.Year, sysdt.Month, 1, 0, 0, 0),
                _edt = _sdt;// new DateTime(sysdt.AddDays(1).Year, sysdt.AddDays(1).Month, 1, 0, 0, 0);

            for (int i = 0; i < m; i++)
            {
                sysdt = DateTime.Now.AddMonths(-(m - i));
                _month[i] = string.Format($"{sysdt:yyyy}-{sysdt:MM}");
                Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} m= {m} , i= {i} , sysdt = {sysdt} "));

                //單月入庫數量
                _sdt = new DateTime(sysdt.Year, sysdt.Month, 1, 0, 0, 0);
                _edt = new DateTime(sysdt.Year, sysdt.Month, _sdt.AddMonths(1).Date.AddDays(-1).Day, 0, 0, 0); //new DateTime(sysdt.AddMonths(1).Year, sysdt.AddMonths(1).Month, 1, 0, 0, 0);
                _upload[i] = _rptService.GetUploadSum(string.Format("{0:yyyy-MM-dd}", _sdt), string.Format("{0:yyyy-MM-dd}", _edt))
                    .Select(s => new
                    {
                        s.fsSUBJ_TITLE,
                        Counts = (s.fnCOUNT_V ?? 0) + (s.fnCOUNT_A ?? 0) + (s.fnCOUNT_P ?? 0) + (s.fnCOUNT_D ?? 0)
                    }).Sum(s => s.Counts);

                //單月調用數量
                _booking[i] = _rptService.GetBookingSum(string.Format("{0:yyyy-MM-dd}", _sdt), string.Format("{0:yyyy-MM-dd}", _edt))
                    .Select(s => new
                    {
                        s.fdDATE,
                        Counts = (s.fnCOUNT_V ?? 0) + (s.fnCOUNT_A ?? 0) + (s.fnCOUNT_P ?? 0) + (s.fnCOUNT_D ?? 0)
                    }).Sum(s => s.Counts);

                Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} m= {m} , i={i} , sysdt= {sysdt} , _sdt= {_sdt}, _edt= {_edt} "));
            }

            DashboardViewModel.ChartModel _char = new DashboardViewModel.ChartModel()
            {
                Months = _month,//new string[] { "1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月" },
                BranchData = new List<DashboardViewModel.ChartModel.Branch>
                {
                    new DashboardViewModel.ChartModel.Branch{ LabelStr = "入庫 ", Counts = _upload } , //Counts = new int[] { 11, 102, 96, 84, 66, 39, 55, 66, 23, 27, 29, 35 } },
                    new DashboardViewModel.ChartModel.Branch{ LabelStr = "調用 ", Counts = _booking } //new int[] { 33, 66, 23, 27, 29, 35, 21, 96, 84, 66, 39, 55 } }
                }
            };
            model.Charts = _char;
            #endregion

            #region ////最近入庫資料(10筆) //Marked_20200331:決議不顯示
            //DateTime sysdt = DateTime.Now,
            //    _sdt = new DateTime(sysdt.Year, sysdt.Month, sysdt.Day, 0, 0, 0).AddDays(-14),
            //    _edt = new DateTime(sysdt.AddDays(1).Year, sysdt.AddDays(1).Month, sysdt.AddDays(1).Day, 0, 0, 0);
            //model.NewUploadData = _rptService.GetUploadDetail(string.Format("{0:yyyy-MM-dd}", _sdt), string.Format("{0:yyyy-MM-dd}", _edt))
            //    .OrderByDescending(o => o.fdCREATED_DATE)
            //    .Select(s => new DashboardViewModel.LastUploadModel(s)).Take(10).ToList();
            #endregion

            #region >>>>> 熱索關鍵字TOP10
            var _srhService = new TblSRHService();
            model.HotkeyData = _srhService.GetHotKey(30, 10);
            #endregion

            #region >>>>> 系統公告
            var _annService = new AnnounceService(_serilogService);
            model.AnnounceData = _annService.GetAnnounceInfo(User.Identity.Name);
            #endregion

            #region >>>>> 今日前10調用者_ADDED_20210519
            //TIP: 預存結果的分類會有三種：轉檔調用, 複製調用, AVID調用，前端只顯示兩種分類統計值(轉檔調用, 複製調用)
            var get = _rptService.GetBookingTodayTop();
            if (get.Count() > 0)
            {
                var getType = get.Select(s => new { s.fsTYPE, s.fsBOOKING_TYPE }).Distinct().ToList();
                var usr = get.Select(s => s.fsUSER).OrderBy(b => b).Distinct().ToList();

                List<DashboardViewModel.BookingTop10.Branch> bookingTopsVal = new List<DashboardViewModel.BookingTop10.Branch>();
                List<string> colorList = GetEnums.GetEnumString<ChartColorEnum>();

                //TIP:分類種類可能會變動!! 。因應後端會有兩個以上的分類統計,改成變動方式產生統計圖表所需的資料格式。
                var branchList = new List<DashboardViewModel.BookingTop10.Branch>();
                int k = 0;
                getType.ForEach(r => {
                    int[] typeVal = new int[usr.Count()]; //分類值
                    int n = 0;

                    usr.ForEach(fr => {
                        var qry = get.Where(x => x.fsUSER == fr && x.fsTYPE == r.fsTYPE).FirstOrDefault();
                        typeVal[n] = qry == null ? 0 : qry.fnBOOKING_QTY ?? 0;
                        n = n + 1;
                    });

                    //var branch = new DashboardViewModel.BookingTop10.Branch
                    //{
                    //    Type = r.fsTYPE,
                    //    TypeStr = r.fsBOOKING_TYPE,
                    //    Values = typeVal
                    //};
                    branchList.Add(new DashboardViewModel.BookingTop10.Branch
                    {
                        Type = r.fsTYPE,
                        TypeStr = r.fsBOOKING_TYPE,
                        Values = typeVal,
                        BarColorHex = colorList[k].Replace("Color_", "#")
                    });
                    k = (k + 1) > colorList.Count() ? 0 : (k + 1);
                });

                model.BookingTodayTOP = new DashboardViewModel.BookingTop10
                {
                    UserLabels = usr.ToArray(),
                    BookWorkVals = branchList
                };
            }
            else
            {
                var emptyBarnch = new DashboardViewModel.BookingTop10.Branch { Type = string.Empty, TypeStr = string.Empty, Values = new int[1] { 0 } };
                var branchList = new List<DashboardViewModel.BookingTop10.Branch> { emptyBarnch };

                model.BookingTodayTOP = new DashboardViewModel.BookingTop10
                {
                    UserLabels = new string[] { },
                    BookWorkVals = branchList
                };
            }
            #endregion

            #region >>>>> 主機調用作業量、主機入庫作業量_ADDED_20210519
            var workArcQty = _rptService.GetWorkArcQty();
            int arcingQ = workArcQty.Where(x => x.fsTYPE == "ARC").Select(s => s.fnCOUNT).FirstOrDefault() ?? 0;    //目前入庫數
            int arcmaxQ = workArcQty.Where(x => x.fsTYPE == "MAX_ARC").Select(s => s.fnCOUNT).FirstOrDefault() ?? 0;//最大數
            int[] arcData = new int[2] { arcingQ, (arcmaxQ - arcingQ) };    //[ 執行路數, 未執行路數 ]
            DashboardViewModel.GaugeViewData arcQty = new DashboardViewModel.GaugeViewData
            {
                GaugeId = "guageUpload",
                GaugeTitle = "主機入庫作業",
                GaugeColor = ChartColorEnum.Color_9966ff.ToString().Replace("Color_", "#"),//"#9966ff", //purple
                GaugeData = arcData
            };

            var workBookQty = _rptService.GetWorkBookQty();
            int runQ = workBookQty.Where(x => x.fsTYPE == "BOOK").Select(s => s.fnCOUNT).FirstOrDefault() ?? 0;     //目前調用數
            int maxQ = workBookQty.Where(x => x.fsTYPE == "MAX_BOOK").Select(s => s.fnCOUNT).FirstOrDefault() ?? 0; //最大數
            int[] bkData = new int[2] { runQ, (maxQ - runQ) };  //[ 執行路數, 未執行路數 ]
            DashboardViewModel.GaugeViewData bookQty = new DashboardViewModel.GaugeViewData
            {
                GaugeId = "guageBoooking",
                GaugeTitle = "主機調用作業",
                GaugeColor = ChartColorEnum.Color_ff9f40.ToString().Replace("Color_", "#"),//"#ff9f40", //orange
                GaugeData = bkData
            };

            model.WorkArcQty = arcQty;
            model.WorkBookQty = bookQty;
            #endregion

            return View(model);
        }

        #region //移至 ReportGetService
        ///// <summary>
        ///// DashBoard 統計{入庫/調出}值區塊資料
        ///// </summary>
        ///// <param name="category">✘✘✘-統計值區塊分類: 1今日入庫,2今日調用,3本月入庫,4本月調用 </param>
        ///// <param name="act"> 操作別: upload 入庫, 調用 Booking </param>
        ///// <param name="type"> 統計別: 今日 today , 本月 month , 昨日 yesterday </param>
        ///// <returns></returns>
        //public DashboardViewModel.StatisticsModel GetStatistics(string act, string type)//(int category)//
        //{
        //    DashboardViewModel.StatisticsModel model = new DashboardViewModel.StatisticsModel();
        //    string _lab = "今日";
        //    DateTime sysdt = DateTime.Now,
        //        _sdt = new DateTime(sysdt.Year, sysdt.Month, sysdt.Day, 0, 0, 0),   //DAY, 當日
        //        _edt = _sdt;// new DateTime(sysdt.AddDays(1).Year, sysdt.AddDays(1).Month, sysdt.AddDays(1).Day, 0, 0, 0);

        //    switch (type.ToUpper())
        //    {
        //        case "YESTERDAY":
        //            _sdt = new DateTime(sysdt.AddDays(-1).Year, sysdt.AddDays(-1).Month, sysdt.AddDays(-1).Day, 0, 0, 0);
        //            _edt = _sdt;// new DateTime(sysdt.Year, sysdt.Month, sysdt.Day, 0, 0, 0);
        //            _lab = "昨日";
        //            break;
        //        case "MONTH":
        //            _sdt = new DateTime(sysdt.Year, sysdt.Month, 1, 0, 0, 0);
        //            _edt = _sdt;// new DateTime(sysdt.AddMonths(1).Year, sysdt.AddMonths(1).Month, 1, 0, 0, 0);
        //            _lab = "本月";
        //            break;
        //        case "TODAY":
        //        default:
        //            _sdt = new DateTime(sysdt.Year, sysdt.Month, sysdt.Day, 0, 0, 0);
        //            _edt = _sdt;
        //            _lab = "今日";
        //            break;
        //    }

        //    switch (act.ToUpper())
        //    {
        //        case "UPLOAD":
        //            model.Category = type.ToUpper() == "YESTERDAY" ? 5 : (type.ToUpper() == "MONTH" ? 3 : 1);
        //            model.Counts = _rptService.GetUploadSum(string.Format("{0:yyyy-MM-dd}", _sdt), string.Format("{0:yyyy-MM-dd}", _edt))
        //                .Select(s => new
        //                {
        //                    s.fsSUBJ_TITLE,
        //                    Counts = (s.fnCOUNT_V ?? 0) + (s.fnCOUNT_A ?? 0) + (s.fnCOUNT_P ?? 0) + (s.fnCOUNT_D ?? 0)
        //                }).Sum(s => s.Counts);
        //            model.LabelStr = _lab + "入庫";
        //            break;
        //        case "BOOKING":
        //            model.Category = type.ToUpper() == "YESTERDAY" ? 6 : (type.ToUpper() == "MONTH" ? 4 : 2);
        //            model.Counts = _rptService.GetBookingSum(string.Format("{0:yyyy-MM-dd}", _sdt), string.Format("{0:yyyy-MM-dd}", _edt))
        //                .Select(s => new
        //                {
        //                    s.fdDATE,
        //                    Counts = (s.fnCOUNT_V ?? 0) + (s.fnCOUNT_A ?? 0) + (s.fnCOUNT_P ?? 0) + (s.fnCOUNT_D ?? 0)
        //                }).Sum(s => s.Counts);
        //            model.LabelStr = _lab + "調用";
        //            break;
        //        default:
        //            break;
        //    }
            
        //    return model;
        //}
        #endregion

        /// <summary>
        /// 熱索關鍵字指定天數、TOP 資料
        /// </summary>
        /// <param name="d"> d 統計天數  </param>
        /// <param name="t"> t 最熱門筆數 </param>
        /// <returns></returns>
        public JsonResult BoardHotKey(int d, int t)
        {
            var _param = new { Days = d, Top = t };
            ResponseResultModel result = new ResponseResultModel(false, "無權限~!", _param);
            if (!User.Identity.IsAuthenticated) return Json(result, JsonRequestBehavior.AllowGet);

            //熱索關鍵字
            var _srhService = new TblSRHService();
            var get = _srhService.GetHotKey(d, t);

            result.IsSuccess = true;
            result.Message = "OK";
            result.StatusCode = HttpStatusCode.OK;
            result.Data = get;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
 