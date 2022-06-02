using AIRMAM5.APIServer;
using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.ArcPre;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Search;
using AIRMAM5.DBEntity.Models.SearchResponse;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Subject;
using AIRMAM5.DBEntity.Models.Template;
using AIRMAM5.DBEntity.Procedure;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.Filters;
using AIRMAM5.HubServer;
using AIRMAM5.Models.TSMapi;
//using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 全文檢索
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class SearchController : BaseController
    {
        private ProcedureGetService _getService;
        private readonly SearchAPIService _searchAPIService;
        private TblSRHService _sRHService;
        private ArcVideoService _arcVideoSer;
        private ArcAudioService _arcAudioSer;
        private ArcPhotoService _arcPhotoSer;
        private ArcDocService _arcDocService;
        private TemplateService _templateService;
        private UsersService _usersService;

        /// <summary>
        /// API URL (預設初始值=Search api URL)
        /// </summary>
        private string _ApiUrl = string.Format($"{Config.SearchUrl}Search/SearchCount");

        /// <summary>
        /// 
        /// </summary>
        public SearchController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService, ICodeService codeService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _tbzCodeService = codeService;
            //
            _getService = new ProcedureGetService();
            _searchAPIService = new SearchAPIService();
            _sRHService = new TblSRHService();
            _arcVideoSer = new ArcVideoService(serilogService);
            _arcAudioSer = new ArcAudioService(serilogService);
            _arcPhotoSer = new ArcPhotoService(serilogService);
            _arcDocService = new ArcDocService(serilogService);
            _usersService = new UsersService(serilogService);
        }

        /// <summary>
        /// 最熱門top5 檢索關鍵字
        /// </summary>
        /// <param name="word">關鍵字</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PopularKeywords(string word)
        {
            ResponseResultModel result = new ResponseResultModel();
            List<PopularKeywordsViewModel> topwords = new List<PopularKeywordsViewModel>();
            var get = _getService.GetHotKW(5);
            //if (get != null && get.FirstOrDefault() != null)
            //{
                topwords = get.Where(x => string.IsNullOrEmpty(word) ? true : x.fsKEYWORD.IndexOf(word) >= 0)
                    .Select(s => new PopularKeywordsViewModel
                    {
                        word = s.fsKEYWORD
                    })//.Take(5)
                    .ToList();

                result.IsSuccess = true;
                result.Message = "熱索關鍵字";
                result.Data = topwords;
                result.StatusCode = HttpStatusCode.OK;
            //}

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 首頁(檢索結果)
        /// </summary>
        /// <param name="id">JSON stringify的SearchParameterViewModel,由iframe src回傳 </param>
        /// <returns></returns>
        [InterceptorOfController(Keyword = "AuthCookie")]
        public ActionResult Index(string id) {
            /* TODO: 測試用,前端串接時請註解。 */
            // model = new SearchParameterViewModel(true);
            /**************************/
            SearchParameterViewModel model = new SearchParameterViewModel();
            try
            {
                id = @id.Replace("^", "\\\"");
                model = JsonConvert.DeserializeObject<SearchParameterViewModel>(id);
            }
            catch (Exception ex)
            {
                return RedirectToAction("NotFound", "Error");
            }

            return View(model);
        }

        #region ==========【 記錄檢索 >> 開啟檢索頁 】==========
        /// <summary>
        /// 檢索前,先記錄 搜索參數 。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SearchParameter(SearchParameterViewModel model)
        {
            //記錄: 1檢索條件,寫入tblSRH，轉成JSON放入fsSTATEMENT；2檢索關鍵字,寫入tbSRH_KW。
            var _res = _sRHService.Create_SRH_KW(model, User.Identity.Name);
            string _str = _res.IsSuccess ? "成功" : "失敗";

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "全文檢索-搜索參數", _str),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(model),
                User.Identity.Name);
            #endregion
            // Success: 更新Dashboard「熱索關鍵字」區塊
            if (!_res.IsSuccess)
            {
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[SearchParameter]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Params = model, _res.Message },
                    LogString = "記錄:全文檢索-搜索參數.Result"
                });
                #endregion
            }
            return Json(_res, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 檢索結果頁(檢索id查詢)
        /// </summary>
        /// <param name="id">檢索記錄Id </param>
        /// <returns></returns>
        public ActionResult Index2(long id)
        {
            var _param = new { tblSRH_ID = id };
            SearchParameterViewModel model = new SearchParameterViewModel();

            try
            {
                //var get = _sRHService.GetByParam(id, string.Empty, string.Empty, User.Identity.Name).FirstOrDefault();
                var get = _sRHService.GetByParam(id, string.Empty, string.Empty, string.Empty).FirstOrDefault();
                if (get == null)
                {
                    return View("Index", model);
                }
                //
                model = JsonConvert.DeserializeObject<SearchParameterViewModel>(get.fsSTATEMENT);
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M014",     //[@USER_ID(@USER_NAME)] 檢視 [@TARGET] 的 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, "全文檢索", "檢索記錄Id:" + id.ToString(), "完成"),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion
            }
            catch (Exception ex)
            {
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[Index2]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "檢索結果頁.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            /* TODO: 測試用,前端串接時請註解。 */
            // model = new SearchParameterViewModel(true);
            /**************************/
            return View("Index", model);
        }
        #endregion

        #region ==========【檢索結果 頁面區塊】==========
        /// <summary>
        /// 一、回覆 搜尋後 {影音圖文}符合的資料筆數
        /// </summary>
        /// <param name="model">檢索參數 <see cref="SearchParameterViewModel"/></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SearchCounts(SearchParameterViewModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(false, string.Empty, _param);
            
            try
            {
                //【檢索結果-Basic-3】符合筆數api資料
                SearchCountResponseModel rtnmd = await this.GetSearchCount(model);

                result.IsSuccess = true;
                result.Data = rtnmd;
                result.Message = "檢索符合資料數";
                result.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.ErrorException = ex;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[SearchCounts]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "檢索符合資料數.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 二、 搜尋後 {影音圖文}資料清單(LSIT) + 包含TSM狀態資料
        /// <para>　　1.透過搜尋引擎(API),取回符合的檔案編號清單 </para>
        /// <para>　　2.依符合的檔案編號清單,取回資料列表內容 </para>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mediaType">媒體類型(V,A,P,D)</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SearchListAsync(SearchParameterViewModel model, string mediaType)
        {
            var _param = model;
            #region _Serilog.Info
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "Search",
                Method = "[SearchListAsync]",
                EventLevel = SerilogLevelEnum.Information,
                Input = _param,
                LogString = "資料清單.Parameter"
            });
            #endregion
            ResponseResultModel result = new ResponseResultModel(false, string.Empty, _param);

            try
            {
                var _type = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), mediaType);
                SearchTypeEnum _searchType = SearchTypeEnum.Video_DEV;
                switch (_type)
                {
                    case FileTypeEnum.V:
                        model.fsINDEX = SearchTypeEnum.Video_DEV.ToString(); //只取"影片"的Metadata
                        _searchType = SearchTypeEnum.Video_DEV;
                        break;
                    case FileTypeEnum.A:
                        model.fsINDEX = SearchTypeEnum.Audio_DEV.ToString(); //只取"聲音"的Metadata
                        _searchType = SearchTypeEnum.Audio_DEV;
                        break;
                    case FileTypeEnum.P:
                        model.fsINDEX = SearchTypeEnum.Photo_DEV.ToString(); //只取"圖片"的Metadata
                        _searchType = SearchTypeEnum.Photo_DEV;
                        break;
                    case FileTypeEnum.D:
                        model.fsINDEX = SearchTypeEnum.Doc_DEV.ToString(); //只取"文件"的Metadata
                        _searchType = SearchTypeEnum.Doc_DEV;
                        break;
                    case FileTypeEnum.S:
                        model.fsINDEX = SearchTypeEnum.Subject_DEV.ToString(); //只取"主題"的Metadata
                        _searchType = SearchTypeEnum.Subject_DEV;
                        break;
                    default:
                        _searchType = SearchTypeEnum.Video_DEV;
                        break;
                }
                var _metaData = await this.GetSearchMeata(model);
                var rtnmd = _metaData.Count()==0? null: _getService.GetArcSearchTList(_searchType, _metaData);

                result.IsSuccess = true;
                result.Data = rtnmd;
                result.Message = "檢索符合資料清單";
                result.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.ErrorException = ex;
                #region _Serilog.Error
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[SearchListAsync]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "檢索符合資料清單.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 搜尋條件 文字化 ConditionModel()
        /// </summary>
        /// <param name="model">檢索參數 <see cref="SearchParameterViewModel"/> </param>
        /// <returns></returns>
        private SearchResponseBaseModel.ConditionModel TransConditionStr(SearchParameterViewModel model)
        {
            var _conditionStr = new SearchResponseBaseModel.ConditionModel();
            #region >>> 搜尋條件 文字化 ConditionModel()
            //檢索類型：影片、聲音、圖片、文字。查詢方式：同音、同義。建立日期區間：2019/01/01~2019/12/31 新到舊。
            string _srtype = model.fsINDEX.Replace("Video_DEV", "影片").Replace("Audio_DEV", "聲音").Replace("Photo_DEV", "圖片").Replace("Doc_DEV", "文字").Replace("Subject_DEV", "主題").Replace(",", "、")
                , _srqry1 = model.fnSEARCH_MODE == 2 ? "同義" : string.Empty
                , _srqry2 = model.fnHOMO == 1 ? "同音" : string.Empty
                , _srqry0 = string.IsNullOrEmpty(_srqry1) || string.IsNullOrEmpty(_srqry2) ? string.Empty : "、"
                , _srdate = string.Format("{0}區間：{1}~{2} {3} "
                , model.clsDATE.fsCOLUMN == "fdCREATED_DATE" ? "建立日期" : "日期"
                , model.clsDATE.fdSDATE, model.clsDATE.fdEDATE
                , model.lstCOLUMN_ORDER.FirstOrDefault().fsVALUE == "2" ? "新到舊" : "舊到新");

            _conditionStr = new SearchResponseBaseModel.ConditionModel
            {
                SearchType = string.Format($"檢索類型：{_srtype} "),
                SearchMode = string.Format($"查詢方式：{_srqry2}{_srqry0}{_srqry1} "),
                DateInterval = _srdate,
                AdvancedQry = string.Empty
            };
            #endregion

            return _conditionStr;
        }
        #region ========= Marked_2020.03.11 --->>>> GetCountData(SearchParameterViewModel model), 可參考使用SearchCounts
        /*/// <summary>
        /// 取得各種類查詢筆數與資訊
        /// </summary>
        /// <returns></returns>
        [Obsolete("可參考使用SearchCounts", true)]
        [HttpPost]
        public async Task<ActionResult> GetCountData(SearchParameterViewModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            SearchResponseBaseModel rtnmodel = new SearchResponseBaseModel();

            try {
                //【檢索結果-Basic-3】符合筆數api資料
                var _countData = await this.GetSearchCount(model);
                rtnmodel.CountData = _countData;
                rtnmodel.ConditionStr = this.TransConditionStr(model);
                rtnmodel.SearchParam = model;

                //【檢索結果-Basic-4】資料筆數api資料(依指定每頁筆數、起始筆數)
                if (_countData.fnVIDEO_COUNT > 0) {
                    model.fsINDEX = SearchTypeEnum.Video_DEV.ToString(); //這裡只取"影片"的Metadata
                    var _metaData = await this.GetSearchMeata(model);
                    var search = _getService.GetArcSearchTList(SearchTypeEnum.Video_DEV, _metaData);
                    rtnmodel.MetaDataList = search;
                }
                else if (_countData.fnAUDIO_COUNT > 0) {
                    model.fsINDEX = SearchTypeEnum.Audio_DEV.ToString(); 
                    var _metaData = await this.GetSearchMeata(model);
                    var search = _getService.GetArcSearchTList(SearchTypeEnum.Audio_DEV, _metaData);
                    rtnmodel.MetaDataList = search;
                }
                else if (_countData.fnPHOTO_COUNT > 0) {
                    model.fsINDEX = SearchTypeEnum.Photo_DEV.ToString();
                    var _metaData = await this.GetSearchMeata(model);
                    var search = _getService.GetArcSearchTList(SearchTypeEnum.Photo_DEV, _metaData);
                    rtnmodel.MetaDataList = search;
                }
                else if (_countData.fnDOC_COUNT > 0)  {
                    model.fsINDEX = SearchTypeEnum.Doc_DEV.ToString();
                    var _metaData = await this.GetSearchMeata(model);
                    var search = _getService.GetArcSearchTList(SearchTypeEnum.Doc_DEV, _metaData);
                    rtnmodel.MetaDataList = search;
                }
             
                result.IsSuccess = true;
                result.StatusCode = HttpStatusCode.OK;
                result.Data = rtnmodel;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[GetCountData]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }   */
        #endregion

        /// <summary>
        /// 查詢條件顯示共用頁面
        /// </summary>
        /// <param name="model">檢索結果 <see cref="SearchResponseBaseModel"/></param>
        /// <returns></returns>
        //[ChildActionOnly]
        [HttpPost]
        public ActionResult _Condition(SearchResponseBaseModel model)//(SearchResponseVideoModel model)
        {
            //全文檢索.按下搜尋 -> Index() -> _Videio(),_Audio(),_Photo(),_Doc()
            // Index()呼叫 SearchCount API 取回符合筆數統計，將  符合筆數、查詢條件參數 提供給{影音圖文}頁面
            // 再判斷: {影音圖文}頁面 顯示一個Metadata內容。
            return PartialView("_Condition", model);
        }

        /// <summary>
        /// 檢索後, {影/音/圖/文}檔案編號使用的 樣板ID List
        /// </summary>
        /// <param name="param"> 檢索參數 <see cref="SearchParameterViewModel"/> </param>
        /// <param name="mediaType">媒體類型(V,A,P,D)</param>
        public async Task<JsonResult> TemplateListAsync(SearchParameterViewModel param, string mediaType)
        {
            #region _Serilog.Info
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "Search",
                Method = "[TemplateListAsync]",
                EventLevel = SerilogLevelEnum.Information,
                Input = param,
                LogString = "檢索資料樣版清單.Parameter"
            });
            #endregion
            ResponseResultModel result = new ResponseResultModel(false, string.Empty, param);
            List<TemplateIdModel> templates = new List<TemplateIdModel>();
            List<SelectListItem> listItems = new List<SelectListItem>();

            try
            {
                FileTypeEnum _type = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), mediaType); //媒體類型Enum: V,A,P,D

                //指定檢索參數的 索引庫名稱[fsINDEX]
                switch (_type)
                {
                    case FileTypeEnum.A:
                        param.fsINDEX = SearchTypeEnum.Audio_DEV.ToString(); //只取"聲音"的Metadata
                        ////searchMetas = await this.GetSearchMeataFileNo(param);
                        //templates = await this.GetSearchMeataTemplate(param);
                        //listItems = await _arcAudioSer.GetSearchMetaAudioTemplates(searchMetas);
                        break;
                    case FileTypeEnum.P:
                        param.fsINDEX = SearchTypeEnum.Photo_DEV.ToString(); //只取"圖片"的Metadata
                        //searchMetas = await this.GetSearchMeataFileNo(param);
                        //listItems = await _arcPhotoSer.GetSearchMetaPhotoTemplates(searchMetas);
                        break;
                    case FileTypeEnum.D:
                        param.fsINDEX = SearchTypeEnum.Doc_DEV.ToString(); //只取"文件"的Metadata
                        //searchMetas = await this.GetSearchMeataFileNo(param);
                        //listItems = await _arcDocService.GetSearchMetaDocTemplates(searchMetas);
                        break;
                    //case FileTypeEnum.S:
                    //    //
                    //    break;
                    case FileTypeEnum.V:
                    default:        //預設顯示{影片}
                        param.fsINDEX = SearchTypeEnum.Video_DEV.ToString(); //只取"影片"的Metadata
                        //searchMetas = await this.GetSearchMeataFileNo(param);
                        //listItems = await _arcVideoSer.GetSearchMetaVideoTemplates(searchMetas); //取回檢索資料的樣板list                                                                   
                        break;
                }

                templates = await this.GetSearchMeataTemplate(param);
                _templateService = new TemplateService();
                listItems = _templateService.GetTemplateListItem(templates);

                result.IsSuccess = true;
                result.Data = listItems;
                result.Message = "檢索資料樣版清單";
                result.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.ErrorException = ex;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[TemplateListAsync]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = param, Exception = ex },
                    LogString = "檢索資料樣版清單.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region ========= Marked_2020.03.12 --->> _ListAsync(List<GetArcSearchResult> model, string mediaType), 可參考使用SearchListAsync
        /*/// <summary>
        /// 結果列表共用頁面 : 判斷是否讀TSM API
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mediaType">媒體類型(V,A,P,D)</param>
        /// <returns></returns>
        //[ChildActionOnly]
        [Obsolete("可參考使用SearchListAsync", true)]
        public async Task<ActionResult> _ListAsync(List<GetArcSearchResult> model, string mediaType)
        {
            var _param = new { model, mediaType };
            ResponseResultModel result = new ResponseResultModel(false, "", _param);
            try
            {
                //檢索video，判斷專案有無TSM，是否需要取得檔案狀態 TSMApi
                List<GetFileStatusResult> _fileStatus = new List<GetFileStatusResult>();
                //if (IsUseTSM.ToUpper() == "TRUE" && mediaType == FileTypeEnum.V.ToString())
                //{
                //    //TODO(21091224): 直接讀TSM API 取回資料過久,改由前端呼叫
                //    #region 【Call TSM Api】
                //    //_ApiUrl = string.Format($"{_tsmUrl}Tsm/GetFileStatusInTsm");
                //    //List<string> _fnoList = model.Select(s => s.fsFILE_NO).ToList();  //檔案編號list
                //    //var _tsmFilePath = _getService.GetTSMFilePath(_fnoList).Select(s => new FILE_NO_TSM_PATH(s)).ToList();
                //    //clsFILE_STATUS_ARGS _agrs = new clsFILE_STATUS_ARGS(_tsmFilePath);
                //    ////
                //    //var _apiResult = await new CallAPIService().ApiPostAsync(_ApiUrl, _agrs);
                //    //string _str = _apiResult.IsSuccess ? string.Format($"GetFileStatusInTsm成功: {_apiResult.Message}") : string.Format($"GetFileStatusInTsm失敗: {_apiResult.Message}");
                //    //string _apiData = JsonConvert.SerializeObject(_apiResult.Data);
                //    //_fileStatus = JsonConvert.DeserializeObject<List<GetFileStatusResult>>(_apiData);
                //    #endregion
                //}
                //
                model = (from a in model
                         join b in _fileStatus on a.fsFILE_NO equals b.FILE_NO into obj
                         from bb in obj.DefaultIfEmpty()
                         select new GetArcSearchResult()
                         {
                             fsSUBJECT_ID = a.fsSUBJECT_ID,
                             fsFILE_NO = a.fsFILE_NO,
                             Title = a.Title,
                             SubjectTitle = a.SubjectTitle,
                             CreateDate = a.CreateDate,
                             FileType = a.FileType,
                             Duration = a.Duration,
                             HeadFrame = a.HeadFrame,
                             SearchType = a.SearchType,
                             TSMFileStatus = bb == null ? -1 : bb.FILE_STATUS,
                             TSMFileStatusStr = bb == null ? " - " : GetEnums.GetDescriptionText((TSMFileStatus)bb.FILE_STATUS)
                         }).ToList();

                result.IsSuccess = true;
                result.Data = model;
                result.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.ErrorException = ex;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[_ListAsync]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "結果列表.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }   

            return await Task.Run(() => Json(result, JsonRequestBehavior.DenyGet));
            //return PartialView("_List", model);
        }   */
        #endregion

        /// <summary>
        /// 預覽共用頁面(圖片或影片的顯示)
        /// </summary>
        /// <param name="fsSUBJECT_ID">主題編號 </param>
        /// <param name="mediaType">媒體類型(V,A,P,D) </param>
        /// <param name="fileNo">檔案編號, 預設 空值 </param>
        /// <returns></returns>
        //[ChildActionOnly]
        [HttpPost]
        public ActionResult _Preview(string fsSUBJECT_ID, string mediaType, string fileNo = "")
        {
            var _param = new { fsSUBJECT_ID, mediaType, fileNo };
            ShowViewerModel rtnmodel = new ShowViewerModel()
            {
                fsSUBJECT_ID = fsSUBJECT_ID,
                fsFILE_NO = fileNo,
                FileType = mediaType
            };
            #region _Serilog
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "Search",
                Method = "[_Preview]",
                EventLevel = SerilogLevelEnum.Information,
                Input = _param,
                LogString = "預覽.Parameter"
            });
            #endregion

            try
            {
                FileTypeEnum _mediaType = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), mediaType);
                switch (_mediaType)
                {
                    case FileTypeEnum.V:
                        rtnmodel = _arcVideoSer.GetVideioBySubjectId(fsSUBJECT_ID, fileNo)
                            .Select(s => new ShowViewerModel().FormatConversion(s, _mediaType))
                            .FirstOrDefault();
                        break;
                    case FileTypeEnum.A:
                        rtnmodel = _arcAudioSer.GetArcAudioBySubjectId(fsSUBJECT_ID, fileNo)
                            .Select(s => new ShowViewerModel().FormatConversion(s, _mediaType))
                            .FirstOrDefault();
                        break;
                    case FileTypeEnum.P:
                        rtnmodel = _arcPhotoSer.GetArcPhotoBySubjectId(fsSUBJECT_ID, fileNo)
                            .Select(s => new ShowViewerModel().FormatConversion(s, _mediaType))
                            .FirstOrDefault();
                        break;
                    case FileTypeEnum.D:
                        rtnmodel = _arcDocService.GetArcDocBySubjectId(fsSUBJECT_ID, fileNo)
                            .Select(s => new ShowViewerModel().FormatConversion(s, _mediaType))
                            .FirstOrDefault();
                        break;
                    default:
                        //
                        break;
                }

                //Marked_20201026:直接通知前端,檔案是否可以預覽(open DocViewer)
                ////(DocViewer)可預覽的檔案類型 (多種類型以分號; 為分隔符號, EX: doc;docx;txt;html;)
                //var _Previewable = new ConfigService().GetConfigBy("DV_VIEW_FILETYPE").FirstOrDefault();
                //rtnmodel.PreviewableFileType = _Previewable == null ? string.Empty : _Previewable.fsVALUE;
            }
            catch (Exception ex)
            {
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[_Preview]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "預覽.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return PartialView("_Preview", rtnmodel);
        }

        /// <summary>
        /// 基本資料
        /// </summary>
        /// <param name="fsSUBJECT_ID">主題編號 </param>
        /// <param name="mediaType">媒體類型(V,A,P,D) </param>
        /// <param name="fileNo">檔案編號 </param>
        /// <returns></returns>
        //[ChildActionOnly]
        [HttpPost]
        public ActionResult _BasicMedia(string fsSUBJECT_ID, string mediaType, string fileNo)
        {
            var _param = new { fsSUBJECT_ID, mediaType, fileNo };
            SubjectFileMetaViewModel metaModel = new SubjectFileMetaViewModel(); //續承使用「檢索結果.基本資料頁model」//ArcBasicMetaModel();

            try
            {
                FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), mediaType);
                switch (mediatype)
                {
                    case FileTypeEnum.V:
                        metaModel = _arcVideoSer.GetVideoBySubjectFile(fsSUBJECT_ID, fileNo)
                                    .Select(s => new SubjectFileMetaViewModel().FormatConversion(s, mediatype)).FirstOrDefault();
                        break;
                    case FileTypeEnum.A:
                        metaModel = _arcAudioSer.GetArcAudioByIdFile(fsSUBJECT_ID, fileNo)
                                    .Select(s => new SubjectFileMetaViewModel().FormatConversion(s, mediatype)).FirstOrDefault();
                        break;
                    case FileTypeEnum.P:
                        metaModel = _arcPhotoSer.GetArcPhotoByIdFile(fsSUBJECT_ID, fileNo)
                                    .Select(s => new SubjectFileMetaViewModel().FormatConversion(s, mediatype)).FirstOrDefault();
                        break;
                    case FileTypeEnum.D:
                        metaModel = _arcDocService.GetArcDocByIdFile(fsSUBJECT_ID, fileNo)
                                    .Select(s => new SubjectFileMetaViewModel().FormatConversion(s, mediatype)).FirstOrDefault();
                        break;
                    default:
                        //
                        break;
                }
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[_BasicMedia]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "檢索基本資料.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return PartialView("_BasicMedia", metaModel ?? new SubjectFileMetaViewModel());
        }

        /// <summary>
        /// 動態欄位資料
        /// </summary>
        /// <param name="fsSUBJECT_ID">主題編號 </param>
        /// <param name="mediaType">媒體類型(V,A,P,D) </param>
        /// <param name="fileNo">檔案編號 </param>
        /// <returns></returns>
        //[ChildActionOnly]
        [HttpPost]
        public ActionResult _DynamicMedia(string fsSUBJECT_ID, string mediaType, string fileNo)
        {
            //自訂欄位
            var _param = new { fsSUBJECT_ID, mediaType, fileNo };
            List<ArcPreAttributeModel> ArcPreAttributes = new List<ArcPreAttributeModel>();

            try
            {
                FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), mediaType);
                switch (mediatype)
                {
                    case FileTypeEnum.V:
                        ArcPreAttributes = _arcVideoSer.GetVideoAttrByFile(fileNo).Select(s => new ArcPreAttributeModel()
                                            .FormatConversion(s, mediatype)).ToList();
                        break;
                    case FileTypeEnum.A:
                        ArcPreAttributes = _arcAudioSer.GetAudioAttrByFile(fileNo).Select(s => new ArcPreAttributeModel()
                                            .FormatConversion(s, mediatype)).ToList();
                        break;
                    case FileTypeEnum.P:
                        ArcPreAttributes = _arcPhotoSer.GetPhotoAttrByFile(fileNo).Select(s => new ArcPreAttributeModel()
                                            .FormatConversion(s, mediatype)).ToList();
                        break;
                    case FileTypeEnum.D:
                        ArcPreAttributes = _arcDocService.GetDocAttrByFile(fileNo).Select(s => new ArcPreAttributeModel()
                                            .FormatConversion(s, mediatype)).ToList();
                        break;
                    default:
                        //
                        break;
                }
            }
            catch (Exception ex)
            {
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[_DynamicMedia]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "檢索動態欄位.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return PartialView("_DynamicMedia", ArcPreAttributes);
        }

        /// <summary>
        /// 關鍵影格(只有影片類型有)
        /// </summary>
        /// <param name="fileNo">檔案編號 </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _KeyFrame(string fileNo)
        {
            string _fno = (!string.IsNullOrEmpty(fileNo)) ? fileNo : "0";
            
            //VideoKeyFrameModel<spGET_ARC_VIDEO_K_Result> _vkf = new VideoKeyFrameModel<spGET_ARC_VIDEO_K_Result>();
            var _keyFrame = _arcVideoSer.GetKeyFrame(_fno).Select(s => new VideoKeyFrameModel().FormatConvert(s, FileTypeEnum.V)).ToList();

            return PartialView(_keyFrame);
        }

        /// <summary>
        /// 段落描述
        /// </summary>
        /// <param name="fileNo">檔案編號 </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _ParaDescription(string mediaType, string fileNo)
        {
            string _fno = (!string.IsNullOrEmpty(fileNo)) ? fileNo : "0";
            FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), mediaType);
            List<SubjectFileSeqmentModel> model = new List<SubjectFileSeqmentModel>();

            if (mediatype == FileTypeEnum.V)
            {
                //SubjectFileSeqmentModel<spGET_ARC_VIDEO_D_Result> _vdp = new SubjectFileSeqmentModel<spGET_ARC_VIDEO_D_Result>();
                model = _arcVideoSer.GetVideoSeqment(_fno).Select(s => new SubjectFileSeqmentModel().FormatConvert(s, mediatype)).ToList();
            }
            else //if (mediatype == FileTypeEnum.A)
            {
                //SubjectFileSeqmentModel<spGET_ARC_AUDIO_D_Result> _vdp = new SubjectFileSeqmentModel<spGET_ARC_AUDIO_D_Result>();
                model = _arcAudioSer.GetAudioSeqment(_fno).Select(s => new SubjectFileSeqmentModel().FormatConvert(s, mediatype)).ToList();
            }

            return PartialView(model);
        }

        /// <summary>
        /// 聲音檔專輯資訊
        /// </summary>
        /// <param name="fsSUBJECT_ID">主題編號 </param>
        /// <param name="fileNo">檔案編號 </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _AudioAlbumInfo(string fsSUBJECT_ID, string fileNo)
        {
            string _subj = (!string.IsNullOrEmpty(fsSUBJECT_ID)) ? fsSUBJECT_ID : "0";
            string _fno = (!string.IsNullOrEmpty(fileNo)) ? fileNo : "0";

            var _album = _arcAudioSer.GetArcAudioByIdFile(_subj, _fno)
                .Select(s => new SearchResponseAudioModel.AudioInfoModel
                {
                    Album = s.fsALBUM ?? string.Empty,
                    AlbumTitle = s.fsALBUM_TITLE ?? string.Empty,
                    AlbumArtists = s.fsALBUM_ARTISTS ?? string.Empty,
                    AlbumPerformers = s.fsALBUM_PERFORMERS ?? string.Empty,
                    AlbumComposers = s.fsALBUM_COMPOSERS ?? string.Empty,
                    AlbumCopyright = s.fsALBUM_COPYRIGHT ?? string.Empty,
                    AlbumYear = s.fnALBUM_YEAR ?? 0,
                    AlbumGenres = s.fsALBUM_GENRES ?? string.Empty,
                    AlbumComment = s.fsALBUM_COMMENT ?? string.Empty
                }).FirstOrDefault();

            return PartialView(_album);
        }
        
        /// <summary>
        /// 取得圖片 EXIF資訊
        /// </summary>
        /// <param name="fileNo">檔案編號 </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _PhotoExifInfo(string fileNo) {
            string _fno = (!string.IsNullOrEmpty(fileNo)) ? fileNo : "0";
            var _exif = _arcPhotoSer.GetPhotoExif(_fno);

            return PartialView(_exif);
        }

        /// <summary>
        /// 取得文件資訊
        /// </summary>
        /// <param name="fileNo">檔案編號</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _DocInfo(string fileNo)
        {
            string _fno = (!string.IsNullOrEmpty(fileNo)) ? fileNo : "0";
            var _info = _arcDocService.GetDocumentInfo(_fno);

            return PartialView(_info);
        }

        /// <summary>
        /// 編輯: 基本欄位+自訂欄位(參照「主題與檔案維護」分頁中的編輯功能)
        /// </summary>
        /// <param name="fileId">檔案Id</param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult _Edit(int fileId)
        {
            return PartialView("_Edit");
        }
        #endregion

        #region ==========【SearchAPI: Count, Meatadata】==========
        /// <summary>
        /// 檢索符合筆數
        /// </summary>
        /// <param name="model">檢索參數 <see cref="SearchParameterViewModel"/></param>
        /// <returns></returns>
        [NonAction]
        public async Task<SearchCountResponseModel> GetSearchCount(SearchParameterViewModel model)
        {
            var _param = model;
            SearchCountResponseModel rtnmodel = new SearchCountResponseModel();
            _ApiUrl = string.Format($"{Config.SearchUrl}Search/SearchCount");

            try
            {
                #region 使用者資訊: 可查詢目錄節點權限,可查詢機密權限,是否為管理者+媒資管理員
                //var _userss = new UsersService();
                var _ur = _usersService.GetBy("", User.Identity.Name);
                string _directory = _usersService.CurrentUserDirAuth, _secret = _ur.FirstOrDefault().fsFILE_SECRET;
                bool _isadmin = _usersService.IsAdminOrMediaManager;
                //Tips_2020/04/01: 全文檢索的fbIS_ADMIN欄位 => 為true(Administrator、MediaManager)
                #endregion

                var _apiParam = new SearchApiParameterModel(model)
                {
                    fsAUTH_DIR = _directory,
                    fsSECRET = _secret,
                    fbIS_ADMIN = _isadmin
                };
                var _apiResult = await _searchAPIService.SearchApiAsync(_ApiUrl, _apiParam);
                string _str = _apiResult.IsSuccess ? string.Format($"SearchCount成功: {_apiResult.Message}") : string.Format($"SearchCount失敗: {_apiResult.Message}");

                string _apiData = JsonConvert.SerializeObject(_apiResult.Data);
                rtnmodel = string.IsNullOrEmpty(_apiData) ? new SearchCountResponseModel() : JsonConvert.DeserializeObject<SearchCountResponseModel>(_apiData);
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[GetSearchCount]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return rtnmodel;
        }

        /// <summary>
        /// 檢索符合 {VIDEO/AUDIO/PHOTO/DOC} 資料列表(檔案編號、附近內容文字) 【Search/SearchMeta】
        /// </summary>
        /// <param name="model">檢索參數 <see cref="SearchParameterViewModel"/></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<SearchMetaResponseModel>> GetSearchMeata(SearchParameterViewModel model)
        {
            var _param = model;
            List<SearchMetaResponseModel> rtnmodel = new List<SearchMetaResponseModel>();
            _ApiUrl = string.Format($"{Config.SearchUrl}Search/SearchMeta");

            try
            {
                #region 使用者資訊: 可查詢目錄節點權限,可查詢機密權限,是否為管理者+媒資管理員
                //var _userss = new UsersService();
                var _ur = _usersService.GetBy("", User.Identity.Name);
                string _directory = _usersService.CurrentUserDirAuth, _secret = _ur.FirstOrDefault().fsFILE_SECRET;
                bool _isadmin = _usersService.IsAdminOrMediaManager;
                //Tips_2020/04/01: 全文檢索的fbIS_ADMIN欄位 => 為true(Administrator、MediaManager)
                #endregion

                var _apiParam = new SearchApiParameterModel(model)
                {
                    fsAUTH_DIR = _directory,//"1879;1888;1889;1903;1890;1868;1899;1873;",//
                    fsSECRET = _secret,//"0;1;2",//
                    fbIS_ADMIN = _isadmin
                };
                var _apiResult = await _searchAPIService.SearchApiAsync(_ApiUrl, _apiParam);
                string _str = _apiResult.IsSuccess ? string.Format($"SearchMeta成功: {_apiResult.Message}") : string.Format($"SearchMeta失敗: {_apiResult.Message}");
                string _apiData = JsonConvert.SerializeObject(_apiResult.Data);

                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[GetSearchMeata]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Params = _param, Result = _apiData },
                    LogString = "檢索符合資料.apiResponse"
                });
                #endregion
                rtnmodel = JsonConvert.DeserializeObject<List<SearchMetaResponseModel>>(_apiData);
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[GetSearchMeata]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return rtnmodel;
        }

        /// <summary>
        /// 檢索符合 {VIDEO/AUDIO/PHOTO/DOC} 檔案編號資料【Search/SearchMetaExport】
        /// <para> TIP: 與 GetSearchMeata() 差別: 回傳內容只有檔案編號，沒有"關鍵字附近的字"。 </para>
        /// </summary>
        /// <param name="model">檢索參數 <see cref="SearchParameterViewModel"/></param>
        [NonAction]
        public async Task<List<SearchMetaResponseModel>> GetSearchMeataFileNo(SearchParameterViewModel model)
        {
            var _param = model;
            List<SearchMetaResponseModel> _list = new List<SearchMetaResponseModel>();
            _ApiUrl = string.Format($"{Config.SearchUrl}Search/SearchMetaExport");

            try
            {
                #region 使用者資訊: 可查詢目錄節點權限,可查詢機密權限,是否為管理者+媒資管理員
                //var _userss = new UsersService();
                var _ur = _usersService.GetBy("", User.Identity.Name);
                string _directory = _usersService.CurrentUserDirAuth, _secret = _ur.FirstOrDefault().fsFILE_SECRET;
                bool _isadmin = _usersService.IsAdminOrMediaManager;
                //Tip: 全文檢索的fbIS_ADMIN欄位 => 為true(Administrator、MediaManager)
                #endregion

                var _apiParam = new SearchApiParameterModel(model)
                {
                    fsAUTH_DIR = _directory, //可用的目錄id
                    fsSECRET = _secret,      //檔案機密等級
                    fbIS_ADMIN = _isadmin
                };
                var _apiResult = await _searchAPIService.SearchApiAsync(_ApiUrl, _apiParam);
                string _str = _apiResult.IsSuccess ? string.Format($"SearchMetaExport成功: {_apiResult.Message}") : string.Format($"SearchMetaExport失敗: {_apiResult.Message}");

                string _apiData = JsonConvert.SerializeObject(_apiResult.Data);
                _list = JsonConvert.DeserializeObject<List<SearchMetaResponseModel>>(_apiData);
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[GetSearchMeataFileNo]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return _list;
        }
        
        /// <summary>
        /// 檢索符合 {VIDEO/AUDIO/PHOTO/DOC} 檔案編號資料使用的樣版編號
        /// </summary>
        /// <param name="model">檢索參數 <see cref="SearchParameterViewModel"/></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<TemplateIdModel>> GetSearchMeataTemplate(SearchParameterViewModel param)
        {
            //var _param = model;
            List<TemplateIdModel> _list = new List<TemplateIdModel>();
            _ApiUrl = string.Format($"{Config.SearchUrl}Search/SearchMetaTemplate");

            try
            {
                #region 使用者資訊: 可查詢目錄節點權限,可查詢機密權限,是否為管理者+媒資管理員
                //var _userss = new UsersService();
                var _ur = _usersService.GetBy("", User.Identity.Name);
                string _directory = _usersService.CurrentUserDirAuth, _secret = _ur.FirstOrDefault().fsFILE_SECRET;
                bool _isadmin = _usersService.IsAdminOrMediaManager;
                //Tip: 全文檢索的fbIS_ADMIN欄位 => 為true(Administrator、MediaManager)
                #endregion

                var _apiParam = new SearchApiParameterModel(param)
                {
                    fsAUTH_DIR = _directory, //可用的目錄id
                    fsSECRET = _secret,      //檔案機密等級
                    fbIS_ADMIN = _isadmin
                };
                var _apiResult = await _searchAPIService.SearchApiAsync(_ApiUrl, _apiParam);
                string _str = _apiResult.IsSuccess ? string.Format($"SearchMetaTemplate成功: {_apiResult.Message}") : string.Format($"SearchMetaTemplate失敗: {_apiResult.Message}");

                string _apiData = JsonConvert.SerializeObject(_apiResult.Data);
                _list = JsonConvert.DeserializeObject<List<TemplateIdModel>>(_apiData);
            }
            catch (Exception ex)
            {
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[GetSearchMeataTemplate]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = param, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return _list;
        }
        #endregion

        #region 【Call TSM Api】
        /// <summary>
        /// 取得影片類型的檔案的TSM狀態
        /// <para> 2020/6/2 : 雲端走S3儲存，要注意web.config設定的API來源，回傳參數有不同。</para>
        /// </summary>
        /// <param name="mediaType">媒體檔案類型</param>
        /// <param name="fileNos">多個檔案編號</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetTsmStatus(string mediaType, string[] fileNos)
        {
            var _param = new { mediaType, fileNos };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param)
            {
                Data = new GetTsmFileStatusResult()
            };

            /** ========== 2020.6.2 ========== TSM API / TSM.S3 API
             * 雲端走S3儲存，新增專案(AIRMAM5.Tsm.S3)
             * TSM.S3 API URL : Http://.../AIRMAM5.Tsm.S3/Tsm/GetFileStatusInTsm
             * 變更時，可於 web.config 設定參數即可。
             * 
             * API 回傳格式與原TSM API相同。
             * TSM API URL : Http://.../AIRMAM5.Tsm/Tsm/GetFileStatusInTsm
             * */
            //_ApiUrl = string.Format($"{_tsmUrl}Tsm/GetFileStatusInTsm");
            _ApiUrl = string.Format($"{Config.TsmUrl}Tsm/GetFileStatusInTsm");

            try
            {
                FileTypeEnum _searchType = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), mediaType);
                if (_searchType != FileTypeEnum.V)
                {
                    result.IsSuccess = false;
                    result.Message = "只允許查詢影片類型";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                //modified_20200103_調整回覆資料格式
                GetTsmFileStatusResult _getTsmFile = new GetTsmFileStatusResult();

                #region >>> 判斷專案有無TSM，是否需要取得檔案狀態 TSMApi 【Call TSM Api】
                if (IsUseTSM.ToUpper() == "TRUE")
                {
                    _getTsmFile.IsUseTSM = true;

                    List<string> _fnoList = fileNos.ToList();  //檔案編號list
                    var _tsmFilePath = _getService.GetTSMFilePath(_fnoList).Select(s => new FILE_NO_TSM_PATH(s)).ToList();
                    clsFILE_STATUS_ARGS _agrs = new clsFILE_STATUS_ARGS(_tsmFilePath);

                    var _apiResult = await new CallAPIService().ApiPostAsync(_ApiUrl, _agrs);
                    string _str = _apiResult.IsSuccess ? string.Format($"GetFileStatusInTsm成功: {_apiResult.Message}") : string.Format($"GetFileStatusInTsm失敗: {_apiResult.Message}");
                    string _apiData = JsonConvert.SerializeObject(_apiResult.Data);

                    var _fileStatus = JsonConvert.DeserializeObject<List<GetFileStatusResult>>(_apiData);
                    if (_fileStatus != null && _fileStatus.Count() > 0)
                    {
                        _getTsmFile.TsmFileStatus = _fileStatus;
                    }
                    //
                    result.IsSuccess = _apiResult.IsSuccess;
                    result.Message = _str;
                    result.StatusCode = _apiResult.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed;
                    result.Data = _getTsmFile;
                }
                #endregion
            }
            catch (Exception ex) {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[GetTmsStatus]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { TSMapi = _ApiUrl, Params = _param, Exception = ex },
                    LogString = "檔案TSM狀態.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region 【檢索結果匯至檔案.csv】

        /// <summary>
        /// 檢索結果匯至檔案 async
        /// </summary>
        /// <param name="param"> 檢索參數 <see cref="SearchParameterViewModel"/> </param>
        /// <param name="mediaType">媒體類型(V,A,P,D)</param>
        /// <remarks> 20201216_TIP: 前端提供"檢索參數"時，要指定[fnTEMP_ID]
        /// </remarks>
        [HttpPost]
        public async Task<JsonResult> SearchExportAsync(SearchParameterViewModel param, string mediaType)
        {
            #region _Serilog.Info
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "Search",
                Method = "[SearchExportAsync]",
                EventLevel = SerilogLevelEnum.Information,
                Input = new { Params = param, MediaType = mediaType },
                LogString = "檢索結果匯出.Parameter"
            });
            #endregion
            ResponseResultModel result = new ResponseResultModel(false, string.Empty, param);

            var watch = new Stopwatch();
            watch.Start();
            ConfigService _configSer = new ConfigService();

            try
            {
                FileTypeEnum _type = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), mediaType);  //媒體類型Enum: V,A,P,D
                param.fsINDEX = (_type == FileTypeEnum.V) ? SearchTypeEnum.Video_DEV.ToString()
                    : (_type == FileTypeEnum.A) ? SearchTypeEnum.Audio_DEV.ToString()
                    : (_type == FileTypeEnum.P) ? SearchTypeEnum.Photo_DEV.ToString()
                    : (_type == FileTypeEnum.D) ? SearchTypeEnum.Doc_DEV.ToString()
                    : (_type == FileTypeEnum.S) ? SearchTypeEnum.Subject_DEV.ToString() : SearchTypeEnum.Video_DEV.ToString();  //檢索分類列舉值

                //A:檢索符合的檔案編號資料與筆數
                List<SearchMetaResponseModel> searchMetas = await this.GetSearchMeataFileNo(param);
                string filenos = string.Join(",", searchMetas.Select(s => s.fsFILE_NO));

                //從A資料取匯出的內容資料(csv文字資料)  【spRPT_GET_SEARCH_EXPORT_DATA_Result】
                #region _Serilog.Debug
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[SearchExportAsync]",
                    EventLevel = SerilogLevelEnum.Debug,
                    Input = new
                    {
                        TempID = param.fnTEMP_ID,
                        FileNos = filenos,
                        MediaType = mediaType
                    },
                    LogString = "檢索結果匯出call預存"
                });
                #endregion
                List<spRPT_GET_SEARCH_EXPORT_DATA_Result> searchResult = _getService.GetSearchExportData(true, int.Parse(param.fnTEMP_ID.ToString()), filenos, mediaType);
                
                //檢索匯出檔案路徑
                spGET_CONFIG_Result getPath = _configSer.GetConfigBy("SEARCH_EXPORT_FILE_PATH").FirstOrDefault()
                                    , getUrl = _configSer.GetConfigBy("SEARCH_EXPORT_FILE_URL").FirstOrDefault();

                string exportFilePath = getPath == null ? string.Empty : string.Format($@"{getPath.fsVALUE}{User.Identity.Name}\")
                    , exportFileName = getPath == null ? string.Empty : string.Format($@"{DateTime.Now:yyyyMMddHHmmss}.csv")
                    , exportFileUrl = getUrl == null ? string.Empty : string.Format($@"{getUrl.fsVALUE}{User.Identity.Name}/");

                if (!Directory.Exists(exportFilePath)) { Directory.CreateDirectory(exportFilePath); }
                ///* TEST */exportFilePath = "C:\\logs\\SearchTest_" + string.Format($"{User.Identity.Name}_{DateTime.Now:yyyyMMddHHmmss}.csv");
                //Write to a file.
                using (TextWriter writer = new StreamWriter(exportFilePath + exportFileName, false, System.Text.Encoding.UTF8))
                    foreach (var rec in searchResult)
                    {
                        await writer.WriteLineAsync(rec.fsCONTENT);
                    }

                //Notice SignalR - 新增通知訊息資料
                await Task.Run(() =>
                {
                    tbmNOTIFY notify = new tbmNOTIFY
                    {
                        fsTITLE = "檢索匯出作業",
                        fsCONTENT = string.Format($"下載路徑：<br /><a target='_blank' href='{exportFileUrl}{exportFileName}' download>{exportFileUrl}{exportFileName}</a>"),
                        fnCATEGORY = (int)NotifyCategoryEnum.預設,
                        fsNOTICE_TARGET = string.Empty,
                        fdEXPIRE_DATE = DateTime.Now.AddDays(7),
                        fdCREATED_DATE = DateTime.Now,
                        fsCREATED_BY = User.Identity.Name
                    };

                    NotifyService notifyService = new NotifyService();
                    VerifyResult verify = notifyService.Create(notify, "SearchExport", CurrentUser.Id);
                    string signalrID = this.GetUserSignalrConnectID(CurrentUser.Id);

                    if (result.IsSuccess)
                    {   //call SignalR 
                        Task.Run(() =>
                        {
                            var _hubs = new BroadcastHub2();
                            _hubs.MyNotifyByUser(CurrentUser.Id, signalrID);
                        });
                    }

                    #region _Serilog.Debug
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "Search",
                        Method = "[SearchExportAsync]",
                        EventLevel = SerilogLevelEnum.Debug,
                        Input = new
                        {
                            CreateResult = result.IsSuccess,
                            ToUser = CurrentUser.UserName,
                            ToUserSignarlR = signalrID,
                            MESSAGE = string.Format($" ({Thread.CurrentThread.ManagedThreadId}) 檢索結果匯出: 通知訊息_DONE. ")
                        },
                        LogString = "檢索結果匯出.通知"
                    });
                    #endregion
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.ErrorException = ex;
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[SearchExportAsync]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = param, MediaType = mediaType, Exception = ex },
                    LogString = "檢索結果匯出.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            result.Data = new { watchtime = watch.ElapsedMilliseconds };
            result.IsSuccess = true;

            watch.Stop();
            ViewBag.WatchMilliseconds = watch.ElapsedMilliseconds;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}