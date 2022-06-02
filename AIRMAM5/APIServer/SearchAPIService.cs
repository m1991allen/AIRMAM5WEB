using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Synonym;
using AIRMAM5.DBEntity.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AIRMAM5.APIServer
{
    /// <summary>
    ///  龍捲風/Lucene 全文檢索Search API
    /// </summary>
    public class SearchAPIService
    {
        readonly SerilogService _serilogService = new SerilogService();

        #region 【同義詞 Synonym】
        /// <summary>
        /// 同義詞: 新增、刪除、重建API叫用 ( InsertSynonym, DeleteSynonym, RebuildSynonym)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiurl"></param>
        /// <param name="synonyms"></param>
        /// <returns></returns>
        public async Task<SearchAPIResponse> SynonymApiAsync<T>(string apiurl, T synonyms)
        {
            HttpClient httpClient = new HttpClient();
            /*舊回傳格式*///SearchAPIReturn responseJson = new SearchAPIReturn();
            SearchAPIResponse apiReturn = new SearchAPIResponse();

            try
            {
                httpClient = new HttpClient { BaseAddress = new Uri(apiurl) };
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.Timeout = TimeSpan.FromSeconds(60 * 5);

                var serializer = new JavaScriptSerializer();
                var jsonText = serializer.Serialize(synonyms);
                StringContent content = new StringContent(jsonText, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(apiurl, content);
                string responseBody = response.Content.ReadAsStringAsync().Result;
                /*舊回傳格式*///responseJson = JsonConvert.DeserializeObject<SearchAPIReturn>(responseBody);
                apiReturn = JsonConvert.DeserializeObject<SearchAPIResponse>(responseBody);
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "SearchAPIService",
                    Method = "SynonymApiAsync",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { apiurl, Param = JsonConvert.SerializeObject(synonyms), Result = apiReturn },
                    LogString = "Result"
                });
                #endregion

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "SearchAPIService",
                    Method = "[SynonymApiAsync]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { apiurl, Param = JsonConvert.SerializeObject(synonyms), Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw ex;
            }
            finally
            {
                httpClient.Dispose();
            }

            return apiReturn;
        }

        /// <summary>
        /// Search API: InsertSynonym, DeleteSynonym
        /// </summary>
        /// <param name="apiurl">API URL</param>
        /// <param name="synonyms">同義詞List</param>
        /// <returns></returns>
        [Obsolete("已改使用 SynonymApiAsync<T>()", true)]
        public async Task<SearchAPIResponse> RunSearchApiAsync(string apiurl, List<SynonymStrModel> synonyms)
        {
            HttpClient httpClient = new HttpClient();
            /*舊回傳格式*///SearchAPIReturn responseJson = new SearchAPIReturn();
            SearchAPIResponse apiReturn = new SearchAPIResponse();

            try
            {
                httpClient = new HttpClient { BaseAddress = new Uri(apiurl) };
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var serializer = new JavaScriptSerializer();
                var jsonText = serializer.Serialize(synonyms);
                StringContent content = new StringContent(jsonText, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(apiurl, content);
                string responseBody = response.Content.ReadAsStringAsync().Result;
                /*舊回傳格式*///responseJson = JsonConvert.DeserializeObject<SearchAPIReturn>(responseBody);
                apiReturn = JsonConvert.DeserializeObject<SearchAPIResponse>(responseBody);
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "SearchAPIService",
                    Method = "RunSearchApiAsync",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { apiurl, synonyms, Result = apiReturn, Lists = synonyms },
                    LogString = "Result"
                });
                #endregion

                response.EnsureSuccessStatusCode();
                //return response.Headers.Location;
            }
            catch (Exception ex)
            {
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "SearchAPIService",
                    Method = "RunSearchApiAsync",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { apiurl, synonyms, Lists = synonyms, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw ex;
            }
            finally
            {
                httpClient.Dispose();
            }
            return apiReturn;
        }

        /// <summary>
        /// Search API: 重建同義詞
        /// </summary>
        /// <param name="apiurl"></param>
        /// <param name="synonyms"></param>
        /// <returns></returns>
        [Obsolete("已改使用 SynonymApiAsync<T>()", true)]
        public async Task<SearchAPIResponse> RebuildsynonymsApiAsync(string apiurl, List<List<SynonymStrModel>> synonyms)
        {
            HttpClient httpClient = new HttpClient();
            /*舊回傳格式*///SearchAPIReturn responseJson = new SearchAPIReturn();
            SearchAPIResponse apiReturn = new SearchAPIResponse();

            try
            {
                httpClient = new HttpClient { BaseAddress = new Uri(apiurl) };
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.Timeout = TimeSpan.FromSeconds(60 * 5);

                var serializer = new JavaScriptSerializer();
                var jsonText = serializer.Serialize(synonyms);
                StringContent content = new StringContent(jsonText, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(apiurl, content);
                string responseBody = response.Content.ReadAsStringAsync().Result;
                /*舊回傳格式*///responseJson = JsonConvert.DeserializeObject<SearchAPIReturn>(responseBody);
                apiReturn = JsonConvert.DeserializeObject<SearchAPIResponse>(responseBody);
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "SearchAPIService",
                    Method = "RebuildsynonymsApiAsync",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { apiurl, synonyms, Result = apiReturn, Lists = synonyms },
                    LogString = "Result"
                });
                #endregion

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "SearchAPIService",
                    Method = "RebuildsynonymsApiAsync",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { apiurl, synonyms, Result = apiReturn, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw ex;
            }
            finally
            {
                httpClient.Dispose();
            }
            return apiReturn;
        }
        #endregion -----同義詞 Synonym-----

        #region 【檢索 Search】
        /// <summary>
        /// 檢索結果: 數量、符合資料列表 (SearchCount、SearchMeta)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiurl"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<SearchAPIResponseCode> SearchApiAsync<T>(string apiurl, T search)
        {
            HttpClient httpClient = new HttpClient();
            SearchAPIResponse apiReturn = new SearchAPIResponse();
            SearchAPIResponseCode result = new SearchAPIResponseCode(); //包含api的 HttpStatusCode

            try
            {
                httpClient = new HttpClient { BaseAddress = new Uri(apiurl) };
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.Timeout = TimeSpan.FromSeconds(60 * 5);

                var serializer = new JavaScriptSerializer();
                var jsonText = serializer.Serialize(search);
                StringContent content = new StringContent(jsonText, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(apiurl, content);
                string responseBody = response.Content.ReadAsStringAsync().Result;
                apiReturn = JsonConvert.DeserializeObject<SearchAPIResponse>(responseBody);

                #region +++++增加記錄API的 HttpStatusCode 再回覆
                result = new SearchAPIResponseCode(apiReturn) { StatusCode = response.StatusCode };
                #endregion

                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "SearchAPIService",
                    Method = "[SearchApiAsync]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Result = apiReturn },
                    LogString = "Result"
                });
                #endregion
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "SearchAPIService",
                    Method = "[SearchApiAsync]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { apiurl, Param = JsonConvert.SerializeObject(search), Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw ex;
            }

            return await Task.Run(() => result);//return apiReturn;
        }

        #endregion -----檢索 Search-----

    }
}