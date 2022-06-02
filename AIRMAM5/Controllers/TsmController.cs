using AIRMAM5.APIServer;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Procedure;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.Filters;
using AIRMAM5.Models.TSMapi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// TSM檔案
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class TsmController : BaseController
    {
        static string _apiURL = string.Empty;
        readonly CallAPIService _callAPIService = new CallAPIService();
        readonly ConfigService _configService = new ConfigService();
        readonly ProcedureGetService _procedureGetService = new ProcedureGetService();

        public TsmController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
        }

        /// <summary>
        /// 磁帶上架
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexCheckIn()
        {
            if (!CheckUserAuth("TSM")) return PartialView("NoAuth");
            
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "磁帶上架"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 磁帶下架
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexCheckOut()
        {
            if (!CheckUserAuth("TSM")) return PartialView("NoAuth");
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "磁帶下架"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion

            //勾選可下架數量不可超過tbzCONFIG.TSM_MAX_CHECKOUT設定的值
            var _get = _configService.GetConfigBy("TSM_MAX_CHECKOUT");
            //單次最大下架磁帶數量
            int _tsmCheckoutMax = (_get == null || _get.FirstOrDefault() == null) ? 6 : int.Parse(_get.FirstOrDefault().fsVALUE);

            ViewData["TapeMax"] = _tsmCheckoutMax;
            return View();
        }

        /// <summary>
        /// 取得待上架磁帶資料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPendingTape()
        {
            ResponseResultModel result = new ResponseResultModel(true)
            {
                Data = new List<GetPendingTapeMode>()
            };
            #region _檢查_
            if (!CheckUserAuth("TSM"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            #endregion

            try
            {
                var get = _procedureGetService.GetPendingTapes();
                if (get == null || get.FirstOrDefault() == null)
                {
                    result.IsSuccess = false;
                    result.Message = "無待上架磁帶資料";
                    result.StatusCode = HttpStatusCode.NoContent;
                    result.Data = new List<GetPendingTapeMode>();
                }
                else
                {
                    result.StatusCode = HttpStatusCode.OK;
                    result.Data = get.Select(s => new GetPendingTapeMode(s)).ToList();
                }
                //
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M005",  //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "待上架磁帶資訊 ", "OK"),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(""),
                    User.Identity.Name);
                #endregion

            }
            catch (Exception ex )
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TSM",
                    Method = "[GetPendingTape]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { TSMapi = _apiURL, Exception = ex },
                    LogString = "待上架磁帶資訊.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region 【Call TSM Api】
        /// <summary>
        /// 取 架上所有磁帶資訊
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetTapeInfoInLib()
        {
            ResponseResultModel result = new ResponseResultModel(true)
            {
                Data = new TapeInfoModel()
            };
            #region _檢查_
            if (!CheckUserAuth("TSM"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            #endregion

            try
            {
                //取得在架上所有磁帶的資訊
                _apiURL = string.Format($"{Config.TsmUrl}Tsm/GetTapeInfoInLib");

                var _apiResult = await _callAPIService.ApiGetAsync(_apiURL);
                string _str = _apiResult.IsSuccess 
                    ? string.Format($"取得櫃中磁帶列表成功: {_apiResult.Message}") 
                    : string.Format($"取得櫃中磁帶列表失敗: {_apiResult.Message}");

                string _apiData = JsonConvert.SerializeObject(_apiResult.Data);
                var _tapeInfo = JsonConvert.DeserializeObject<List<TapeInfoModel>>(_apiData);
                //
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M005",  //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "架上磁帶資訊 ", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(""),
                    User.Identity.Name);
                #endregion

                result.IsSuccess = _apiResult.IsSuccess;
                result.Message = _str;
                result.StatusCode = _apiResult.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed;
                result.Data = _tapeInfo;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TSM",
                    Method = "[GetTapeInfoInLib]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { TSMapi = _apiURL, Exception = ex },
                    LogString = "架上磁帶資訊.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取 納管過的所有磁帶的資訊
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetAllTapeInfo()
        {
            ResponseResultModel result = new ResponseResultModel(true)
            {
                Data = new TapeInfoModel()
            };
            #region _檢查_
            if (!CheckUserAuth("TSM"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            #endregion

            try
            {
                //取得納管過的所有磁帶的資訊
                _apiURL = string.Format($"{Config.TsmUrl}Tsm/GetTapeInfoAll");

                var _apiResult = await _callAPIService.ApiGetAsync(_apiURL);
                string _str = _apiResult.IsSuccess
                    ? string.Format($"取得納管磁帶列表成功: {_apiResult.Message}")
                    : string.Format($"取得納管磁帶列表失敗: {_apiResult.Message}");

                string _apiData = JsonConvert.SerializeObject(_apiResult.Data);
                var _tapeInfo = JsonConvert.DeserializeObject<List<TapeInfoModel>>(_apiData);
                //
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M005",  //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "所有磁帶資訊 ", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    string.Empty,
                    User.Identity.Name);
                #endregion

                result.IsSuccess = _apiResult.IsSuccess;
                result.Message = _str;
                result.StatusCode = _apiResult.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed;
                result.Data = _tapeInfo;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TSM",
                    Method = "[GetTapeInfoInLib]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { TSMapi = _apiURL, Exception = ex },
                    LogString = "所有磁帶資訊.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 磁帶下架
        /// </summary>
        /// <param name="tapes">磁帶編號</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> TapeCheckOut(string[] tapes)
        {
            ResponseResultModel result = new ResponseResultModel(true);
            #region _檢查_
            if (!CheckUserAuth("TSM"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            try
            {
                //磁帶下架
                _apiURL = string.Format($"{Config.TsmUrl}Tsm/TapeCheckOut");
                //磁帶下架TSM api 參數格式
                TapeCheckOutArgs _args = new TapeCheckOutArgs { lstTAPE_NO = tapes.ToList() };

                var _apiResult = await _callAPIService.ApiPostAsync(_apiURL, _args);
                string _str = _apiResult.IsSuccess
                    ? string.Format($"磁帶下架成功")
                    : string.Format($"磁帶下架失敗: {_apiResult.Message}");

                string _apiData = JsonConvert.SerializeObject(_apiResult.Data);
                var _tapeInfo = JsonConvert.DeserializeObject<List<TapeInfoModel>>(_apiData);
                //
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M021",  //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "磁帶下架", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(tapes),
                    User.Identity.Name);
                #endregion

                result.IsSuccess = _apiResult.IsSuccess;
                result.Message = _str;
                result.StatusCode = _apiResult.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed;
                result.Data = _tapeInfo;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TSM",
                    Method = "[TapeCheckOut]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { TSMapi = _apiURL, Param = tapes.ToString(), Exception = ex },
                    LogString = "磁帶下架.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 磁帶上架
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> TapeCheckIn()
        {
            ResponseResultModel result = new ResponseResultModel(true);
            #region _檢查_
            if (!CheckUserAuth("TSM"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            try
            {
                //磁帶上架
                _apiURL = string.Format($"{Config.TsmUrl}Tsm/TapeCheckIn");

                var _apiResult = await _callAPIService.ApiPostAsync(_apiURL);
                string _str = _apiResult.IsSuccess
                    ? string.Format($"磁帶上架成功")
                    : string.Format($"磁帶上架失敗: {_apiResult.Message}");

                string _apiData = JsonConvert.SerializeObject(_apiResult.Data);
                var _tapeInfo = JsonConvert.DeserializeObject<List<TapeInfoModel>>(_apiData);
                //
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M021",  //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "磁帶上架", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(""),
                    User.Identity.Name);
                #endregion

                result.IsSuccess = _apiResult.IsSuccess;
                result.Message = _str;
                result.StatusCode = _apiResult.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed;
                result.Data = _tapeInfo;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TSM",
                    Method = "[TapeCheckIn]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { TSMapi = _apiURL, Exception = ex },
                    LogString = "磁帶上架.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 查詢是否有上架工作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> TapeCheckInWorks()
        {
            ResponseResultModel result = new ResponseResultModel(true);
            #region _檢查_
            if (!CheckUserAuth("TSM"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            #endregion

            try
            {
                //檢查有無上架程序正在執行
                _apiURL = string.Format($"{Config.TsmUrl}Tsm/IsTapeCheckInWorking");

                var _apiResult = await _callAPIService.ApiGetAsync(_apiURL);
                string _str = _apiResult.IsSuccess
                    ? string.Format($"取得是否有上架作業成功")
                    : string.Format($"取得是否有上架作業失敗: {_apiResult.Message}");

                string _apiData = JsonConvert.SerializeObject(_apiResult.Data); // false OR true
                bool isPending = _apiData.ToUpper() == "TRUE" ? true : false;
                //
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M005",  //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "是否有上架工作", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(""),
                    User.Identity.Name);
                #endregion

                result.IsSuccess = _apiResult.IsSuccess;
                result.Message = _str;
                result.StatusCode = _apiResult.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed;
                result.Data = isPending;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TSM",
                    Method = "[TapeCheckInWorks]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { TSMapi = _apiURL, Exception = ex },
                    LogString = "查詢是否有上架工作.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}