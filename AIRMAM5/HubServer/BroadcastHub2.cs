using AIRMAM5.APIServer;
using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Announce;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Hub;
using AIRMAM5.DBEntity.Models.Notify;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Procedure;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.Models.TSMapi;
using AIRMAM5.Utility.Extensions;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AIRMAM5.HubServer
{
    /// <summary>
    /// SignalR Server 
    /// </summary>
    [HubName("broadcastHub")]
    public class BroadcastHub2 : Hub
    {
        readonly IHubContext _hubContext;
        readonly SerilogService _serilogService;
        readonly TblHubConnectionService _tblHubConnService;
        readonly NotifyService _notifyServer;
        readonly TblLoginService _tblLoginService;
        readonly AnnounceService _annService;
        readonly UsersService _usersService;

        /// <summary>
        /// 目前連線使用者 tbmUsers
        /// </summary>
        private tbmUSERS ConntectedUser { get; }

        public BroadcastHub2()
        {
            // 獲取所有連接的句柄，方便后面進行消息廣播
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<BroadcastHub2>();
            //
            _serilogService = new SerilogService();
            _tblHubConnService = new TblHubConnectionService();
            _notifyServer = new NotifyService();
            _tblLoginService = new TblLoginService();
            _annService = new AnnounceService();
            _usersService = new UsersService();

            #region >>>目前使用者
            var userFromAuthCookie = Thread.CurrentPrincipal;
            if (userFromAuthCookie != null && userFromAuthCookie.Identity.IsAuthenticated)
            {
                string userNameFromAuthCookie = userFromAuthCookie.Identity.Name; //登入帳號
                ConntectedUser = _usersService.FindUserByName(userNameFromAuthCookie);
            }
            #endregion
        }

        /// <summary>
        /// 靜態類別 儲存上線清單
        /// </summary>
        public static class UserHandler
        {
            /// <summary>
            /// 用戶端-帳號-資訊: Key = UserId, Value = List:UserOnlineInfo
            /// </summary>
            public static Dictionary<string, List<UserOnlineInfo>> ConnectedUsers = new Dictionary<string, List<UserOnlineInfo>>();
            
            /// <summary>
            /// 檢查 使用者ID(ConnectedUsers)是否存在
            /// </summary>
            /// <param name="userid">使用者ID</param>
            public static bool UserIsExists(string userid)
            {
                var get = ConnectedUsers.TryGetValue(userid, out List<UserOnlineInfo> user);
                return get;
            }

            /// <summary>
            /// 更新使用者資訊內容: UserOnlineInfo List
            /// </summary>
            /// <param name="userOnlines">記憶體中線上使用者 UserOnlineInfo List </param>
            /// <param name="m">更新資料 UserOnlineInfo </param>
            /// <returns></returns>
            public static List<UserOnlineInfo> UpdateOnlineInfo(List<UserOnlineInfo> userOnlines, UserOnlineInfo m)
            {
                var tempdata = userOnlines;
                //檢查上線時間(太久未更新) 移除 Dictionary List資料
                tempdata.ForEach(info =>
                {
                    //var diff = new TimeSpan(DateTime.Now.Ticks - info.ConnectTime.Ticks).TotalHours;
                    var diff = DateTimeExtensions.DiffDateTo(info.ConnectTime, DateTime.Now, Utility.Enums.TimeUnitEnum.Hours);
                    if (Math.Abs(diff) >= Config.HubKeepTimes)
                    {
                        userOnlines.Remove(info);
                    }
                });

                var get =  userOnlines.Where(x => x.SignalrConnectionId == m.SignalrConnectionId);
                if (get.Any())
                {
                    userOnlines.Remove(get.First());
                }
                userOnlines.Add(m);

                return userOnlines;
            }

            /// <summary>
            /// 使用者ID 取連線的ConnectionID (可能會有多筆)
            /// </summary>
            /// <param name="userid">使用者ID</param>
            public static List<UserOnlineInfo> getUserConnectId(string userid)
            {
                List<UserOnlineInfo> onlineInfo = new List<UserOnlineInfo>();

                if (UserIsExists(userid))
                {
                    onlineInfo = ConnectedUsers[userid];
                }

                return onlineInfo;
            }
        }

        #region >>> Update 更新處理
        /// <summary>
        /// 更新/新增 用戶端連線使用者資訊: UserHandler.ConnectedUsers[UserId, List]
        /// </summary>
        /// <param name="model">更新資料 SignalrClientIdModel </param>
        /// <remarks>
        /// <br> 1.更新記憶體記錄 </br>
        /// <br> 2.Connection Id 寫到資料表 </br>
        /// </remarks>
        private void UpdateConnectedUsers(SignalrClientUpdateModel model)
        {
            List<UserOnlineInfo> connectUsers = new List<UserOnlineInfo>();
            UserOnlineInfo urInfo = new UserOnlineInfo(model);

            if (UserHandler.UserIsExists(model.UserId))
            {
                connectUsers = UserHandler.UpdateOnlineInfo(UserHandler.ConnectedUsers[model.UserId], urInfo);
                UserHandler.ConnectedUsers[model.UserId] = connectUsers;
            }
            else
            {
                connectUsers.Add(urInfo);
                UserHandler.ConnectedUsers.Add(model.UserId, connectUsers);
            }
        }

        /// <summary>
        /// 更新資料 寫入dbo.tblHUBCONNECTION
        /// </summary>
        /// <param name="m"></param>
        private VerifyResult UpdateHubConnection(tblHUBCONNECTION m)
        {
            VerifyResult result = new VerifyResult(true, string.Empty);

            if (_tblHubConnService.IsExtist(m.fsUSER_ID, m.fsHUB_CONNECTION_ID))
            {
                var get = _tblHubConnService.GetBy(m.fsUSER_ID, m.fsHUB_CONNECTION_ID).FirstOrDefault();
                if (get == null)
                {
                    result = _tblHubConnService.Create(m);
                }
                else
                {
                    get.fdUPDATED_TIME = DateTime.Now;
                    get.fsUSERNAME = ConntectedUser.fsLOGIN_ID; //fsLOGIN_ID 使用者帳號
                    get.fnLOGIN_ID = m.fnLOGIN_ID;
                    get.fdUPDATED_DATE = DateTime.Now;
                    get.fsUPDATED_BY = ConntectedUser.fsLOGIN_ID; //使用者帳號

                    _tblHubConnService.Update(get);
                }
            }
            else
            {
                result = _tblHubConnService.Create(m);
            }

            return result;
        }
        #endregion

        /// <summary>
        /// Client 使用者連線時呼叫: 
        /// <br> 1、新增目前使用者至上線清單；</br>
        /// <br> 2、新增使用者至群組；</br>
        /// <br> 3、更新/紀錄使用者Signal Client Connection Id； </br>
        /// <br> 4、取回使用者通知紀錄資料； </br>
        /// </summary>
        /// <param name="m"></param>
        public void UserConnected(SignalrClientUpdateModel m)
        {
            #region Serilog.Info
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "BroadcastHub",
                Method = "[UserConnected]",
                EventLevel = SerilogLevelEnum.Information,
                Input = m,
                LogString = "SignalR"
            });
            #endregion

            //進行編碼，防止XSS攻擊
            m.LoginId = HttpUtility.HtmlEncode(m.LoginId);

            // 新增使用者至群組
            //Groups.Add(Context.ConnectionId, m.GroupId);

            // 新增目前使用者至上線清單: UserHandler.ConnectedUsers[UserId, List] (使用者會有多個ConnectionID).
            UpdateConnectedUsers(m);

            // 更新重新整理在線人數
            UpdateLastTime(m.UserId, m.SignalrConnectionId, m.LoginLogId);

            // 取使用者訊息清單 : 顯示近7天的訊息(已讀7天，未讀一率顯示)。
            MyNotifyByUser(m.UserId, m.SignalrConnectionId);
        }

        /// <summary>
        /// 使用者更新最後在線時間 tblLOGIN.fdETIME
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="loginlogid"></param>
        /// <remarks> 用戶端固定時間回call method :回報在線
        /// </remarks>
        public void UpdateLastTime(string userid, string connectId, long loginlogid)
        {
            var _param = new { userid, connectId, loginlogid };

            try
            {
                // 更新資料 寫入dbo.tblHUBCONNECTION
                var _res = UpdateHubConnection(new tblHUBCONNECTION
                {
                    fsUSER_ID = userid,
                    fsHUB_CONNECTION_ID = connectId,
                    fdONLINEED_TIME = DateTime.Now,
                    fdUPDATED_TIME = DateTime.Now,
                    fsUSERNAME = ConntectedUser.fsLOGIN_ID, //fsLOGIN_ID 使用者帳號
                    fnLOGIN_ID = loginlogid,
                    fdCREATED_DATE = DateTime.Now,
                    fsCREATED_BY = ConntectedUser.fsLOGIN_ID,
                    fdUPDATED_DATE = DateTime.Now,
                    fsUPDATED_BY = string.Empty
                });

                //取得目前可能在線人數並修改狀態
                //TIP_20201112: 發生預存沒有回傳資料而造成Exception(500 Error),先判斷預存資料結果再處理, 影響：不會更新在線人數清單(其它在線者的線上人數清單不會更新)
                var _getLoginAlive = _tblLoginService.GetLoginAlive(loginlogid);
                if (_getLoginAlive != null && _getLoginAlive.Count() > 0)
                {
                    var _list = _getLoginAlive
                        .Select(s => new OnlineMembersModel
                        {
                            LoginLogid = s.fnLOGIN_ID,
                            UserId = s.fsUSER_ID,
                            UserName = s.fsLOGIN_ID,
                            LoginDTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", s.fdSTIME),
                            EndDTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", s.fdETIME),
                            Note = s.fsNOTE ?? string.Empty,
                            UpdateDTime = s.fdUPDATED_DATE == null ? string.Empty : string.Format("{0:yyyy-MM-dd HH:mm:ss}", s.fdUPDATED_DATE)
                        }).ToList();

                    OnlineDataModel json = new OnlineDataModel { Number = _list.Count(), DataList = _list };

                    // 通知所有人連線,更新在線人數.(ConnectionID從資料表獲取)
                    //_hubContext.Clients.All.showOnline(json);
                    // TIP: 2020-07-29-連線ConnectionID由資料表取得
                    var _onlines = _tblHubConnService.GetAllNoOverdue();
                    _onlines.ForEach(r =>
                    {
                        _hubContext.Clients.Client(r.SignalrConnectionId).showOnline(json);
                    });
                }
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "BroadcastHub2",
                    Method = "[UpdateLastTime]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Exception = ex },
                    LogString = "更新最後在線時間.Result",
                    ErrorMessage = ex.Message
                });
                #endregion
                //throw;
            }
        }

        #region >>> 取資料 Notify
        /// <summary>
        /// 提供 SignalR Client 呼叫: 用戶端取回自己的通知訊息資料清單
        /// </summary>
        /// <param name="userid"></param>
        public void MyNotifyByUser(string userid, string clientid)
        {
            var result = _notifyServer.GetByUser(userid, false);

            int unread = 0;
            if (result.Count() > 0)
            {
                var get = result.Where(x => x.IsRead == false);
                if (get.Any()) unread = get.Count();
            }

            UserNotifyModel json = new UserNotifyModel
            {
                UserId = userid,
                UnRead = unread,
                DataList = result
            };

            // 呼叫所有客戶端 
            //Clients.All.showMyNotify(json);
            // 呼叫特定使用者
            Clients.Client(clientid).showMyNotify(json);
        }
        #endregion

        #region >>> 更新DashBoard、公告訊息
        /// <summary>
        /// To Client : 更新 DashBoard 公告區塊內容
        /// <para> 1.通知Client有公告更新。2.Client回傳ClientID、UserId 取回可看的公告資料。 </para>
        /// </summary>
        /// <param name="msg"></param>
        public void RefreshAnnOfDashBoard(string msg)
        {
            try
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    List<AnnouncePublicViewModel> annData = new List<AnnouncePublicViewModel>();

                    //通知所有連線的ConnectID (依使用者各自取得公告資料)
                    //UserHandler.ConnectedUsers.ToList().ForEach(f =>
                    //{
                    //    f.Value.ForEach(r =>
                    //    {
                    //        if ((DateTime.Now - r.ConnectTime).TotalHours > 23) return;

                    //        annData = _annService.GetAnnounceInfo(r.LoginId);
                    //        if (annData.Any())
                    //        {
                    //            // 呼叫特定ConnectionID
                    //            _hubContext.Clients.Client(r.SignalrConnectionId).refreshAnnounce(annData);
                    //        }
                    //    });
                    //});
                    //TIP: 2020-07-29-連線ConnectionID由資料表取得
                    var _onlines = _tblHubConnService.GetAllNoOverdue();
                    _onlines.ForEach(r =>
                    {
                        annData = _annService.GetAnnounceInfo(r.LoginId);
                        if (annData.Any())
                        {
                            _hubContext.Clients.Client(r.SignalrConnectionId).refreshAnnounce(annData);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "BroadcastHub2",
                    Method = "[RefreshAnnOfDashBoard]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Exception = ex },
                    LogString = "更新公告通知.Result",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw;
            }
        }

        /// <summary>
        /// To Client : 更新 DashBoard 入庫/調用統計、圖表Chart值
        /// </summary>
        /// <param name="userid"> 觸發更新的使用者id </param>
        public void RefreshCountsOfDashBoard(string userid)
        {
            //DashBoard 頁面資料MODEL：DashboardViewModel
            try
            {
                if (_usersService.IsExists(userid))
                {
                    var _rptSer = new ReportGetService();
                    // >>>>> 01.入庫,調用統計值區塊資料
                    var counters = new List<DashboardViewModel.StatisticsModel>
                    {
                        _rptSer.GetStatistics("upload", "day"),
                        _rptSer.GetStatistics("booking", "day"),
                        _rptSer.GetStatistics("upload", "month"),
                        _rptSer.GetStatistics("booking", "month"),
                        _rptSer.GetStatistics("upload", "yesterday"),
                        _rptSer.GetStatistics("booking", "yesterday")
                    };

                    #region >>>>> 02.入庫,調用統計圖表區塊資料(過去12個月)
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

                        //單月入庫數量
                        _sdt = new DateTime(sysdt.Year, sysdt.Month, 1, 0, 0, 0);
                        _edt = new DateTime(sysdt.Year, sysdt.Month, _sdt.AddMonths(1).Date.AddDays(-1).Day, 0, 0, 0); //new DateTime(sysdt.AddMonths(1).Year, sysdt.AddMonths(1).Month, 1, 0, 0, 0);
                        _upload[i] = _rptSer.GetUploadSum(string.Format("{0:yyyy-MM-dd}", _sdt), string.Format("{0:yyyy-MM-dd}", _edt))
                            .Select(s => new
                            {
                                s.fsSUBJ_TITLE,
                                Counts = (s.fnCOUNT_V ?? 0) + (s.fnCOUNT_A ?? 0) + (s.fnCOUNT_P ?? 0) + (s.fnCOUNT_D ?? 0)
                            }).Sum(s => s.Counts);

                        //單月調用數量
                        _booking[i] = _rptSer.GetBookingSum(string.Format("{0:yyyy-MM-dd}", _sdt), string.Format("{0:yyyy-MM-dd}", _edt))
                            .Select(s => new
                            {
                                s.fdDATE,
                                Counts = (s.fnCOUNT_V ?? 0) + (s.fnCOUNT_A ?? 0) + (s.fnCOUNT_P ?? 0) + (s.fnCOUNT_D ?? 0)
                            }).Sum(s => s.Counts);
                    }

                    DashboardViewModel.ChartModel _chart = new DashboardViewModel.ChartModel()
                    {
                        Months = _month,
                        BranchData = new List<DashboardViewModel.ChartModel.Branch>
                        {
                            new DashboardViewModel.ChartModel.Branch{ LabelStr = "入庫 ", Counts = _upload } ,
                            new DashboardViewModel.ChartModel.Branch{ LabelStr = "調用 ", Counts = _booking }
                        }
                    };
                    #endregion

                    DashboardViewModel dashboard = new DashboardViewModel
                    {
                        StatisticsData = counters,
                        Charts = _chart
                    };

                    //通知所有連線的ConnectID
                    //_hubContext.Clients.All.refreshCounts(dashboard);
                    //TIP: 2020-07-29-連線ConnectionID由資料表取得
                    var _onlines = _tblHubConnService.GetAllNoOverdue();
                    _onlines.ForEach(r =>
                    {
                        _hubContext.Clients.Client(r.SignalrConnectionId).refreshCounts(dashboard);
                    });
                }
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "BroadcastHub2",
                    Method = "[RefreshCountsOfDashBoard]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Exception = ex },
                    LogString = "更新入庫調用統計值.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw;
            }
        }
        #endregion

        #region >>> override
        /// <summary>
        ///  連線
        /// </summary>
        /// <remarks> 成功連線後, 前端立即呼叫hub Method: UserConnected() </remarks>
        /// <returns></returns>
        public override async Task OnConnected()
        {
            #region _Serilog.Debug//
            //_serilogService.SerilogWriter(new SerilogInputModel
            //{
            //    Controller = "BroadcastHub",
            //    Method = "[OnConnected]",
            //    EventLevel = SerilogLevelEnum.Debug,
            //    Input = new { Context.ConnectionId, UserHandler.ConnectedIds },
            //    LogString = "連線",
            //});
            #endregion
            _tblHubConnService.RemoveOverdue();
            await base.OnConnected();
        }
        // 斷線
        public override async Task OnDisconnected(bool stopCalled)
        {
            #region _Serilog.Debug//
            //_serilogService.SerilogWriter(new SerilogInputModel
            //{
            //    Controller = "BroadcastHub",
            //    Method = "[OnConnected]",
            //    EventLevel = SerilogLevelEnum.Debug,
            //    Input = new { Context.ConnectionId, UserHandler.ConnectedUsers },
            //    LogString = "斷線",
            //});
            #endregion

            #region >>>移除 Dictionary資料
            //string _connId = Context.ConnectionId;
            //await Task.Run(() =>
            //{
            //    var urConnList = UserHandler.ConnectedUsers[ConntectedUser.fsUSER_ID];
            //    List<UserOnlineInfo> upd = urConnList;

            //    urConnList.ToList().ForEach(f =>
            //    {
            //        if (f.SignalrConnectionId == Context.ConnectionId)
            //        {
            //            var _res = upd.Remove(f);
            //            #region _Serilog.Verbose
            //            _serilogService.SerilogWriter(new SerilogInputModel
            //            {
            //                Controller = "BroadcastHub",
            //                Method = "[OnConnected]",
            //                EventLevel = SerilogLevelEnum.Verbose,
            //                Input = new { UserOnlineInfo = f, RemoveResult = _res },
            //                LogString = "斷線.Info",
            //            });
            //            #endregion
            //            return;
            //        }
            //    });

            //    UserHandler.ConnectedUsers[ConntectedUser.fsUSER_ID] = upd;
            //}).ConfigureAwait(false);
            #endregion
            await base.OnDisconnected(stopCalled);
        }
        #endregion

        #region >>> 檔案上架TSM
        /// <summary>
        /// 取待上架磁帶資料
        /// </summary>
        /// <remarks>
        ///   回call client method: refreshTapeWaitCheckIn(List val)
        ///   參數: val = GetPendingTapeMode 清單資料
        ///   <br>
        ///     通知系統管理員(以 dbo.tbzCONFIG.fsKEY='ADMIN_GROUPS' 指定的群組為通知對象)
        ///   </br>
        /// </remarks>
        public void TapePendingRecord()
        {
            ProcedureGetService _procedureGet = new ProcedureGetService();

            try
            {
                var get = _procedureGet.GetPendingTapes();

                List<GetPendingTapeMode> getPending = (get == null || get.FirstOrDefault() == null)
                    ? new List<GetPendingTapeMode>()
                    : get.Select(s => new GetPendingTapeMode(s)).ToList();

                var sysAdm = new ConfigService().GetConfigBy("ADMIN_GROUPS").FirstOrDefault();
                if (sysAdm == null)
                {
                    #region _Serilog.Err
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "BroadcastHub2",
                        Method = "[TapePendingRecord]",
                        EventLevel = SerilogLevelEnum.Error,
                        Input = new { Message = "tbzCONFIG.fsKEY='ADMIN_GROUPS' 指定的群組未設定." },
                        LogString = "待上架磁帶資料.Result"
                    });
                    #endregion
                }
                else
                {
                    //TIP: 連線ConnectionID由資料表取得
                    //var _onlines = _tblHubConnService.GetAllNoOverdue();
                    var _onlines = _tblHubConnService.GetAssignConnectID(sysAdm.fsVALUE);
                    _onlines.ForEach(r =>
                    {
                        _hubContext.Clients.Client(r.SignalrConnectionId).refreshTapeWaitCheckIn(getPending);
                    });
                }
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "BroadcastHub2",
                    Method = "[TapePendingRecord]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Exception = ex },
                    LogString = "待上架磁帶資料.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw;
            }
        }

        /// <summary>
        /// 目前是否有上架作業 (意指TSM IO Port的磁帶移至磁帶櫃中動作)
        /// </summary>
        /// <remarks>
        ///   回call client method: checkPendingWorks(string val)
        ///   參數: val = false 或 true
        ///   <br>
        ///     通知系統管理員(以 dbo.tbzCONFIG.fsKEY='ADMIN_GROUPS' 指定的群組為通知對象)
        ///   </br>
        /// </remarks>
        public async Task WhetherPendingWork()
        {
            CallAPIService _callAPIService = new CallAPIService();

            try
            {
                //檢查有無上架程序正在執行
                var _apiURL = string.Format($"{Config.TsmUrl}Tsm/IsTapeCheckInWorking");
                var _apiResult = await _callAPIService.ApiGetAsync(_apiURL);

                string _str = _apiResult.IsSuccess
                    ? string.Format($"IsTapeCheckInWorking 成功")
                    : string.Format($"IsTapeCheckInWorking 失敗: {_apiResult.Message}");

                string _apiData = JsonConvert.SerializeObject(_apiResult.Data); // false OR true
                //bool isPending = _apiData.ToUpper() == "TRUE" ? true : false;
                
                var sysAdm = new ConfigService().GetConfigBy("ADMIN_GROUPS").FirstOrDefault();
                if (sysAdm == null)
                {
                    #region _Serilog.Err
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "BroadcastHub2",
                        Method = "[WhetherPendingWork]",
                        EventLevel = SerilogLevelEnum.Error,
                        Input = new { Message = "tbzCONFIG.fsKEY='ADMIN_GROUPS' 指定的群組未設定." },
                        LogString = "目前是否有上架作業.Result"
                    });
                    #endregion
                }
                else
                {
                    //TIP: 連線ConnectionID由資料表取得
                    //var _onlines = _tblHubConnService.GetAllNoOverdue();
                    var _onlines = _tblHubConnService.GetAssignConnectID(sysAdm.fsVALUE);
                    _onlines.ForEach(r =>
                    {
                        _hubContext.Clients.Client(r.SignalrConnectionId).checkPendingWorks(_apiData);
                    });
                }
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "BroadcastHub2",
                    Method = "[WhetherPendingWork]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Exception = ex },
                    LogString = "目前是否有上架作業.Result",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw;
            }
        }
        #endregion

        #region >>> 主機調用、入庫作業 執行路數
        /// <summary>
        /// To Client : 更新 DashBoard 主機入庫作業 執行路數資料
        /// </summary>
        /// <remarks> 通知所有連線的ConnectID </remarks>
        public void ServerArcWorkQty()
        {
            ReportGetService _rptService = new ReportGetService();
            try
            {
                var get = _rptService.GetWorkArcQty();

                int arcingQ = get.Where(x => x.fsTYPE == "ARC").Select(s => s.fnCOUNT).FirstOrDefault() ?? 0;
                int arcmaxQ = get.Where(x => x.fsTYPE == "MAX_ARC").Select(s => s.fnCOUNT).FirstOrDefault() ?? 0;
                int[] arcData = new int[2] { arcingQ, (arcmaxQ - arcingQ) };

                DashboardViewModel.GaugeViewData arcQty = new DashboardViewModel.GaugeViewData
                {
                    GaugeId = "guageUpload",
                    GaugeTitle = "主機入庫作業",
                    GaugeColor = ChartColorEnum.Color_9966ff.ToString().Replace("Color_", "#"),//"#9966ff", //purple
                    GaugeData = arcData  //[ 執行路數, 未執行路數 ]
                };

                //通知所有連線的ConnectID
                var _onlines = _tblHubConnService.GetAllNoOverdue();
                _onlines.ForEach(r =>
                {
                    _hubContext.Clients.Client(r.SignalrConnectionId).refreshBookArcWorkQty(arcQty);
                });
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "BroadcastHub2",
                    Method = "[ServerArcWorkQty]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Exception = ex },
                    LogString = "更新主機入庫作業值.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw;
            }
        }

        /// <summary>
        /// To Client : 更新 DashBoard 主機調用作業 執行路數資料
        /// </summary>
        public void ServerBookWorkQty()
        {
            ReportGetService _rptService = new ReportGetService();
            try
            {
                var get = _rptService.GetWorkBookQty();

                int runQ = get.Where(x => x.fsTYPE == "BOOK").Select(s => s.fnCOUNT).FirstOrDefault() ?? 0;
                int maxQ = get.Where(x => x.fsTYPE == "MAX_BOOK").Select(s => s.fnCOUNT).FirstOrDefault() ?? 0;
                int[] bkData = new int[2] { runQ, (maxQ - runQ) };

                DashboardViewModel.GaugeViewData bookQty = new DashboardViewModel.GaugeViewData
                {
                    GaugeId = "guageBoooking",
                    GaugeTitle = "主機調用作業",
                    GaugeColor = ChartColorEnum.Color_ff9f40.ToString().Replace("Color_", "#"),//"#ff9f40", //orange
                    GaugeData = bkData  //[ 執行路數, 未執行路數 ]
                };

                //通知所有連線的ConnectID
                var _onlines = _tblHubConnService.GetAllNoOverdue();
                _onlines.ForEach(r =>
                {
                    _hubContext.Clients.Client(r.SignalrConnectionId).refreshBookArcWorkQty(bookQty);
                });
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "BroadcastHub2",
                    Method = "[ServerBookWorkQty]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Exception = ex },
                    LogString = "更新主機調用作業值.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw;
            }
        }
        #endregion
    }
}