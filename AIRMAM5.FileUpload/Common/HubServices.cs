using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AIRMAM5.FileUpload.Common
{
    /// <summary>
    /// SignalR Hub Service 溝通
    /// </summary>
    public class HubServices
    {
        readonly ISerilogService _serilogService = new SerilogService();
        readonly UsersService _usersService;// = new UsersService();
        readonly string signalrHub = ConfigurationManager.AppSettings["SignalrHub"].ToString();
        private HubConnection hubConnection = null;
        private IHubProxy broadcastHubProxy = null;

        public HubServices()
        {
            hubConnection = new HubConnection(signalrHub, false);
            broadcastHubProxy = hubConnection.CreateHubProxy("broadcastHub");
            //Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} 【HubServices】 HubConnection ----- BEGN ----- "));
            _usersService = new UsersService(_serilogService);
        }

        //public async Task Connection()
        //{
        //    //using (var hubConnection = new HubConnection("http://www.contoso.com/"))
        //    //{
        //    //    IHubProxy stockTickerHubProxy = hubConnection.CreateHubProxy("StockTickerHub");
        //    //    stockTickerHubProxy.On<Stock>("UpdateStockPrice", stock => Console.WriteLine("Stock update for {0} new price {1}", stock.Symbol, stock.Price));
        //    //    await hubConnection.Start();
        //    //}
        //    using (var hubConnection = new HubConnection(signalrHub))
        //    {
        //        hubConnection.CreateHubProxy("broadcastHub");
        //        ServicePointManager.DefaultConnectionLimit = 10;  //設定 WPF 用戶端中的最大併發連線數
        //        await hubConnection.Start();
        //    }
        //}

        /// <summary>
        /// 上傳檔案成功通知 SignalR
        /// <para> 透過SignalR 更新前端通知訊息。</para>
        /// </summary>
        public void Upload2Notify(string touserid)
        {
            var urex = _usersService.GetById(touserid).tbmUSER_EXTEND;
            #region _Serilog(Debug)
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "HubServices",
                Method = "[Upload2Notify]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { MESSAGE = string.Format($" 【HubServices.Upload2Notify】: touserid= {touserid} , signalrHub= {signalrHub}  ") },
                LogString = ""
            });
            #endregion

            try
            {
                if (urex != null)
                {
                    Task.Run(()=>
                    {
                        //hubConnection = new HubConnection(signalrHub, false);
                        //broadcastHubProxy = hubConnection.CreateHubProxy("broadcastHub");
                        var init = hubConnection.Start().ContinueWith<bool>(task =>
                        {
                            //若連線失敗，顯示錯誤並return false;
                            if (task.IsFaulted)
                            {
                                foreach (var e in task.Exception.Flatten().InnerExceptions)
                                {
                                    Debug.WriteLine(string.Format($" 【HubServices】 HubConnection Error: " + e.Message));
                                    #region _Serilog.Warning
                                    _serilogService.SerilogWriter(new SerilogInputModel
                                    {
                                        Controller = "HubServices",
                                        Method = "[Upload2Notify]",
                                        EventLevel = SerilogLevelEnum.Warning,
                                        Input = new { MESSAGE = string.Format($" 【HubServices】 HubConnection Error: " + e.Message) },
                                        LogString = "即時通知.HubConnection"
                                    });
                                    #endregion
                                }
                                return false;
                            }

                            return true;
                        });

                        if (!init.Result) return;

                        //用戶端呼叫伺服器方法 .Invoke
                        var hubp = broadcastHubProxy.Invoke("MyNotifyByUser", touserid, urex.fsSIGNALR_CONNECT_ID).ContinueWith<bool>(t =>
                        {
                            if (t.IsFaulted)
                            {
                                foreach (var ex in t.Exception.Flatten().InnerExceptions)
                                {
                                    Console.WriteLine("Error: {0}", ex.Message);
                                    #region _Serilog.Warning
                                    _serilogService.SerilogWriter(new SerilogInputModel
                                    {
                                        Controller = "HubServices",
                                        Method = "[Upload2Notify]",
                                        EventLevel = SerilogLevelEnum.Warning,
                                        Input = new { MESSAGE = string.Format($" 【HubServices】 broadcastHubProxy.Invoke Error: " + ex.Message) },
                                        LogString = "即時通知.HubConnection"
                                    });
                                    #endregion
                                }
                                return false;
                            }
                            return true;
                        });

                        #region _Serilog(Debug)
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "HubServices",
                            Method = "[Upload2Notify]",
                            EventLevel = SerilogLevelEnum.Debug,
                            Input = new { MESSAGE = string.Format($" 【HubServices.Upload2Notify】: touserid= {touserid} , Call \"MyNotifyByUser\" Success!! ") },
                            LogString = "即時通知.Result"
                        });
                        #endregion
                        
                        //HubDispose();
                    });
                }
            }
            catch (Exception ex)
            {
                HubDispose();
                //Debug.WriteLine(string.Format($" 【HubServices】- [Upload2Notify] - Exception: {ex.Message} \n {ex.StackTrace}"));
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "HubServices",
                    Method = "[Upload2Notify]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { to = touserid, ConnectID = urex.fsSIGNALR_CONNECT_ID, Exception = ex, innerEx = ex.InnerException },
                    LogString = "通知訊息.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw;
            }
        }

        /// <summary>
        /// 更新DashBoard 入庫/調用 統計數字區塊
        /// <para> 入庫/調用 統計標準: WorkStatus=90 轉檔完成 才算+1。 </para>
        /// </summary>
        public void RefreshDashBoardCounter(string userid)
        {
            var urex = _usersService.GetById(userid).tbmUSER_EXTEND;
            #region _Serilog(Debug)
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "HubServices",
                Method = "[RefreshDashBoardCounter]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { MESSAGE = string.Format($" 【HubServices.Upload2Notify】: userid= {userid} , signalrHub= {signalrHub}  ") },
                LogString = ""
            });
            #endregion

            try
            {
                if (urex != null)
                {
                    Task.Run(() =>
                    {
                        var init = hubConnection.Start().ContinueWith<bool>(task =>
                        {
                            //若連線失敗，顯示錯誤並return false;
                            if (task.IsFaulted)
                            {
                                foreach (var e in task.Exception.Flatten().InnerExceptions)
                                {
                                    Debug.WriteLine(string.Format($" 【HubServices】 HubConnection Error: " + e.Message));
                                    #region _Serilog.Warning
                                    _serilogService.SerilogWriter(new SerilogInputModel
                                    {
                                        Controller = "HubServices",
                                        Method = "[RefreshDashBoardCounter]",
                                        EventLevel = SerilogLevelEnum.Warning,
                                        Input = new { MESSAGE = string.Format($" 【HubServices】 HubConnection Error: " + e.Message) },
                                        LogString = "更新DashBoard.HubConnection",
                                    });
                                    #endregion
                                }
                                return false;
                            }

                            return true;
                        });

                        if (!init.Result) return;

                        //用戶端呼叫伺服器方法 .Invoke
                        var hubp = broadcastHubProxy.Invoke("RefreshCountsOfDashBoard", userid).ContinueWith<bool>(t =>
                        {
                            if (t.IsFaulted)
                            {
                                foreach (var ex in t.Exception.Flatten().InnerExceptions)
                                {
                                    Console.WriteLine("Error: {0}", ex.Message);
                                    #region _Serilog.Warning
                                    _serilogService.SerilogWriter(new SerilogInputModel
                                    {
                                        Controller = "HubServices",
                                        Method = "[RefreshDashBoardCounter]",
                                        EventLevel = SerilogLevelEnum.Warning,
                                        Input = new { MESSAGE = string.Format($" 【HubServices】 broadcastHubProxy.Invoke Error: " + ex.Message) },
                                        LogString = "更新DashBoard.HubConnection",
                                    });
                                    #endregion
                                }
                                return false;
                            }
                            return true;
                        });

                        #region _Serilog(Debug)
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "HubServices",
                            Method = "[RefreshDashBoardCounter]",
                            EventLevel = SerilogLevelEnum.Debug,
                            Input = new { MESSAGE = string.Format($" 【HubServices.Upload2Notify】: UserID= {userid} , Call \"RefreshCountsOfDashBoard\" Success!! ") },
                            LogString = "更新DashBoard.Result"
                        });
                        #endregion

                        //HubDispose();
                    });
                }
            }
            catch (Exception ex)
            {
                HubDispose();
                //Debug.WriteLine(string.Format($" 【HubServices】- [RefreshDashBoardCounter] - Exception: {ex.Message} \n {ex.StackTrace}"));
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "HubServices",
                    Method = "[RefreshDashBoard_Counter]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { UserId = userid, ConnectID = urex.fsSIGNALR_CONNECT_ID, Exception = ex, innerEx = ex.InnerException },
                    LogString = "更新儀表板.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw;
            }
        }

        public void HubDispose()
        {
            if (hubConnection.State == ConnectionState.Disconnected)
            {
                hubConnection.Stop();
                hubConnection.Dispose();
            }
        }
    }
}