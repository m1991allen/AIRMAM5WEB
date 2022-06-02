using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;
using Newtonsoft.Json;
using System; 
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AIRMAM5.APIServer
{
    /// <summary>
    /// AIRMAM5.Tsm API服務
    /// </summary>
    public class CallAPIService
    {
        readonly SerilogService _serilogService = new SerilogService();
        HttpClient httpClient = new HttpClient();

        /// <summary>
        ///終止PostAsync Task 時間(秒)
        /// </summary>
        private readonly static string _cancelAfter = Config.CancelPostAsync.ToString();//ConfigurationManager.AppSettings["CancelPostAsync"].ToString();

        /// <summary>
        /// API POST叫用，有參數。(TSMAPI, FileUploadAPI
        /// +2020/1/2   : 加入終止任務token.
        /// </summary>
        /// <typeparam name="T"> API參數 資料類型</typeparam>
        /// <param name="apiurl">指定API URL </param>
        /// <param name="fileNos">API參數 </param>
        public async Task<SearchAPIResponse> ApiPostAsync<T>(string apiurl, T fileNos)
        {
            SearchAPIResponse apiReturn = new SearchAPIResponse();
            #region _Serilog
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "CallAPIService",
                Method = "[ApiPostAsync]",
                EventLevel = SerilogLevelEnum.Information,
                Input = new { apiurl, Param = JsonConvert.SerializeObject(fileNos) },
                LogString = "POSTAPI",
                ErrorMessage = apiReturn.Message,
            });
            #endregion

            try
            {
                httpClient = new HttpClient { BaseAddress = new Uri(apiurl) };
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.Timeout = TimeSpan.FromSeconds(60 * 5); //5 minutes
                //
                var cts = new CancellationTokenSource();
                int.TryParse(_cancelAfter, out int cancelsec); // 任務取消秒數
                cts.CancelAfter(1000 * cancelsec);

                var serializer = new JavaScriptSerializer();
                var jsonText = serializer.Serialize(fileNos);
                StringContent content = new StringContent(jsonText, Encoding.UTF8, "application/json");

                //HttpResponseMessage response = await httpClient.PostAsync(apiurl, content);
                HttpResponseMessage response = await httpClient.PostAsync(apiurl, content, cts.Token); //加入:終止任務token

                string responseBody = response.Content.ReadAsStringAsync().Result;
                apiReturn = JsonConvert.DeserializeObject<SearchAPIResponse>(responseBody);
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "CallAPIService",
                    Method = "[ApiPostAsync]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Result = apiReturn },
                    LogString = "POSTAPI.Result",
                    ErrorMessage = apiReturn.Message,
                });
                #endregion

                response.EnsureSuccessStatusCode();
                //Tip_20200817: TSM api回覆400會拋出Exception
                //var _scode = response.StatusCode;
            }
            catch (OperationCanceledException ex)//(Exception ex)
            {
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "CallAPIService",
                    Method = "[ApiPostAsync]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { apiurl, Param = JsonConvert.SerializeObject(fileNos), Exception = ex },
                    LogString = "POSTAPI.OperationCanceledException",
                    ErrorMessage = ex.Message
                });
                #endregion
                //throw ex;
            }

            return await Task.Run(() => apiReturn);
        }

        /// <summary>
        /// API POST叫用，無參數
        /// </summary>
        /// <param name="apiurl">指定API URL </param>
        /// <returns></returns>
        public async Task<SearchAPIResponse> ApiPostAsync(string apiurl)
        {
            SearchAPIResponse apiReturn = new SearchAPIResponse();
            #region _Serilog
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "CallAPIService",
                Method = "[ApiPostAsync]",
                EventLevel = SerilogLevelEnum.Information,
                Input = new { apiurl, Param = "(無參數)" },
                LogString = "POSTAPI",
                ErrorMessage = apiReturn.Message,
            });
            #endregion

            try
            {
                httpClient = new HttpClient { BaseAddress = new Uri(apiurl) };
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.Timeout = TimeSpan.FromSeconds(60 * 5); //5 minutes
                //
                var cts = new CancellationTokenSource();
                int.TryParse(_cancelAfter, out int cancelsec); // 任務取消秒數
                cts.CancelAfter(1000 * cancelsec);

                StringContent content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(apiurl, content, cts.Token); //加入:終止任務token

                string responseBody = response.Content.ReadAsStringAsync().Result;
                apiReturn = JsonConvert.DeserializeObject<SearchAPIResponse>(responseBody);
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "CallAPIService",
                    Method = "[ApiPostAsync]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Result = apiReturn },
                    LogString = "POSTAPI.Result",
                    ErrorMessage = apiReturn.Message,
                });
                #endregion

                response.EnsureSuccessStatusCode();
                //Tip_20200817: TSM api回覆400會拋出Exception
                //var _scode = response.StatusCode;
            }
            catch (OperationCanceledException ex)//(Exception ex)
            {
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "CallAPIService",
                    Method = "[ApiPostAsync]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { apiurl, Param = "(無參數)", Exception = ex },
                    LogString = "POSTAPI.OperationCanceledException",
                    ErrorMessage = ex.Message
                });
                #endregion
                //throw ex;
            }

            return await Task.Run(() => apiReturn);
        }

        /// <summary>
        /// API GET叫用，無參數。(TSMAPI, FileUploadAPI
        /// </summary>
        /// <param name="apiurl">指定API URL </param>
        public async Task<SearchAPIResponse> ApiGetAsync(string apiurl)
        {
            SearchAPIResponse apiReturn = new SearchAPIResponse();
            #region _Serilog
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "CallAPIService",
                Method = "[ApiGetAsync]",
                EventLevel = SerilogLevelEnum.Information,
                Input = new { apiurl, Param = "(無參數)" },
                LogString = "GETAPI",
                ErrorMessage = apiReturn.Message,
            });
            #endregion

            try
            {
                httpClient = new HttpClient { BaseAddress = new Uri(apiurl) };
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.Timeout = TimeSpan.FromSeconds(60 * 5); //5 minutes
                //
                var cts = new CancellationTokenSource();
                int.TryParse(_cancelAfter, out int cancelsec); // 任務取消秒數
                cts.CancelAfter(1000 * cancelsec);

                HttpResponseMessage response = await httpClient.GetAsync(apiurl, cts.Token); //加入:終止任務token
                string responseBody = response.Content.ReadAsStringAsync().Result;
                apiReturn = JsonConvert.DeserializeObject<SearchAPIResponse>(responseBody);
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "CallAPIService",
                    Method = "[ApiGetAsync]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Result = apiReturn },
                    LogString = "GETAPI.Result",
                    ErrorMessage = apiReturn.Message,
                });
                #endregion

                response.EnsureSuccessStatusCode();
            }
            catch (OperationCanceledException ex)//(Exception ex)
            {
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "CallAPIService",
                    Method = "[ApiGetAsync]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { apiurl, Param = "(無參數)", Exception = ex },
                    LogString = "GETAPI.OperationCanceledException",
                    ErrorMessage = ex.Message
                });
                #endregion
                //throw ex;
            }

            return await Task.Run(() => apiReturn);
        }
    }
}