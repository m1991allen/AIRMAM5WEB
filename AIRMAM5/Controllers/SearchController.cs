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
    /// �����˯�
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
        /// API URL (�w�]��l��=Search api URL)
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
        /// �̼���top5 �˯�����r
        /// </summary>
        /// <param name="word">����r</param>
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
                result.Message = "��������r";
                result.Data = topwords;
                result.StatusCode = HttpStatusCode.OK;
            //}

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ����(�˯����G)
        /// </summary>
        /// <param name="id">JSON stringify��SearchParameterViewModel,��iframe src�^�� </param>
        /// <returns></returns>
        [InterceptorOfController(Keyword = "AuthCookie")]
        public ActionResult Index(string id) {
            /* TODO: ���ե�,�e�ݦ걵�ɽе��ѡC */
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

        #region ==========�i �O���˯� >> �}���˯��� �j==========
        /// <summary>
        /// �˯��e,���O�� �j���Ѽ� �C
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SearchParameter(SearchParameterViewModel model)
        {
            //�O��: 1�˯�����,�g�JtblSRH�A�নJSON��JfsSTATEMENT�F2�˯�����r,�g�JtbSRH_KW�C
            var _res = _sRHService.Create_SRH_KW(model, User.Identity.Name);
            string _str = _res.IsSuccess ? "���\" : "����";

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M001",     //[@USER_ID(@USER_NAME)] �s�W [@DATA_TYPE] ��� @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "�����˯�-�j���Ѽ�", _str),
                string.Format($"��m: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(model),
                User.Identity.Name);
            #endregion
            // Success: ��sDashboard�u��������r�v�϶�
            if (!_res.IsSuccess)
            {
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[SearchParameter]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Params = model, _res.Message },
                    LogString = "�O��:�����˯�-�j���Ѽ�.Result"
                });
                #endregion
            }
            return Json(_res, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// �˯����G��(�˯�id�d��)
        /// </summary>
        /// <param name="id">�˯��O��Id </param>
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
                    "M014",     //[@USER_ID(@USER_NAME)] �˵� [@TARGET] �� [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, "�����˯�", "�˯��O��Id:" + id.ToString(), "����"),
                    string.Format($"��m: {Request.UserHostAddress} "),
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
                    LogString = "�˯����G��.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            /* TODO: ���ե�,�e�ݦ걵�ɽе��ѡC */
            // model = new SearchParameterViewModel(true);
            /**************************/
            return View("Index", model);
        }
        #endregion

        #region ==========�i�˯����G �����϶��j==========
        /// <summary>
        /// �@�B�^�� �j�M�� {�v���Ϥ�}�ŦX����Ƶ���
        /// </summary>
        /// <param name="model">�˯��Ѽ� <see cref="SearchParameterViewModel"/></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SearchCounts(SearchParameterViewModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(false, string.Empty, _param);
            
            try
            {
                //�i�˯����G-Basic-3�j�ŦX����api���
                SearchCountResponseModel rtnmd = await this.GetSearchCount(model);

                result.IsSuccess = true;
                result.Data = rtnmd;
                result.Message = "�˯��ŦX��Ƽ�";
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
                    LogString = "�˯��ŦX��Ƽ�.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// �G�B �j�M�� {�v���Ϥ�}��ƲM��(LSIT) + �]�tTSM���A���
        /// <para>�@�@1.�z�L�j�M����(API),���^�ŦX���ɮ׽s���M�� </para>
        /// <para>�@�@2.�̲ŦX���ɮ׽s���M��,���^��ƦC���e </para>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mediaType">�C������(V,A,P,D)</param>
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
                LogString = "��ƲM��.Parameter"
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
                        model.fsINDEX = SearchTypeEnum.Video_DEV.ToString(); //�u��"�v��"��Metadata
                        _searchType = SearchTypeEnum.Video_DEV;
                        break;
                    case FileTypeEnum.A:
                        model.fsINDEX = SearchTypeEnum.Audio_DEV.ToString(); //�u��"�n��"��Metadata
                        _searchType = SearchTypeEnum.Audio_DEV;
                        break;
                    case FileTypeEnum.P:
                        model.fsINDEX = SearchTypeEnum.Photo_DEV.ToString(); //�u��"�Ϥ�"��Metadata
                        _searchType = SearchTypeEnum.Photo_DEV;
                        break;
                    case FileTypeEnum.D:
                        model.fsINDEX = SearchTypeEnum.Doc_DEV.ToString(); //�u��"���"��Metadata
                        _searchType = SearchTypeEnum.Doc_DEV;
                        break;
                    case FileTypeEnum.S:
                        model.fsINDEX = SearchTypeEnum.Subject_DEV.ToString(); //�u��"�D�D"��Metadata
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
                result.Message = "�˯��ŦX��ƲM��";
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
                    LogString = "�˯��ŦX��ƲM��.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// �j�M���� ��r�� ConditionModel()
        /// </summary>
        /// <param name="model">�˯��Ѽ� <see cref="SearchParameterViewModel"/> </param>
        /// <returns></returns>
        private SearchResponseBaseModel.ConditionModel TransConditionStr(SearchParameterViewModel model)
        {
            var _conditionStr = new SearchResponseBaseModel.ConditionModel();
            #region >>> �j�M���� ��r�� ConditionModel()
            //�˯������G�v���B�n���B�Ϥ��B��r�C�d�ߤ覡�G�P���B�P�q�C�إߤ���϶��G2019/01/01~2019/12/31 �s���¡C
            string _srtype = model.fsINDEX.Replace("Video_DEV", "�v��").Replace("Audio_DEV", "�n��").Replace("Photo_DEV", "�Ϥ�").Replace("Doc_DEV", "��r").Replace("Subject_DEV", "�D�D").Replace(",", "�B")
                , _srqry1 = model.fnSEARCH_MODE == 2 ? "�P�q" : string.Empty
                , _srqry2 = model.fnHOMO == 1 ? "�P��" : string.Empty
                , _srqry0 = string.IsNullOrEmpty(_srqry1) || string.IsNullOrEmpty(_srqry2) ? string.Empty : "�B"
                , _srdate = string.Format("{0}�϶��G{1}~{2} {3} "
                , model.clsDATE.fsCOLUMN == "fdCREATED_DATE" ? "�إߤ��" : "���"
                , model.clsDATE.fdSDATE, model.clsDATE.fdEDATE
                , model.lstCOLUMN_ORDER.FirstOrDefault().fsVALUE == "2" ? "�s����" : "�¨�s");

            _conditionStr = new SearchResponseBaseModel.ConditionModel
            {
                SearchType = string.Format($"�˯������G{_srtype} "),
                SearchMode = string.Format($"�d�ߤ覡�G{_srqry2}{_srqry0}{_srqry1} "),
                DateInterval = _srdate,
                AdvancedQry = string.Empty
            };
            #endregion

            return _conditionStr;
        }
        #region ========= Marked_2020.03.11 --->>>> GetCountData(SearchParameterViewModel model), �i�ѦҨϥ�SearchCounts
        /*/// <summary>
        /// ���o�U�����d�ߵ��ƻP��T
        /// </summary>
        /// <returns></returns>
        [Obsolete("�i�ѦҨϥ�SearchCounts", true)]
        [HttpPost]
        public async Task<ActionResult> GetCountData(SearchParameterViewModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            SearchResponseBaseModel rtnmodel = new SearchResponseBaseModel();

            try {
                //�i�˯����G-Basic-3�j�ŦX����api���
                var _countData = await this.GetSearchCount(model);
                rtnmodel.CountData = _countData;
                rtnmodel.ConditionStr = this.TransConditionStr(model);
                rtnmodel.SearchParam = model;

                //�i�˯����G-Basic-4�j��Ƶ���api���(�̫��w�C�����ơB�_�l����)
                if (_countData.fnVIDEO_COUNT > 0) {
                    model.fsINDEX = SearchTypeEnum.Video_DEV.ToString(); //�o�̥u��"�v��"��Metadata
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
        /// �d�߱�����ܦ@�έ���
        /// </summary>
        /// <param name="model">�˯����G <see cref="SearchResponseBaseModel"/></param>
        /// <returns></returns>
        //[ChildActionOnly]
        [HttpPost]
        public ActionResult _Condition(SearchResponseBaseModel model)//(SearchResponseVideoModel model)
        {
            //�����˯�.���U�j�M -> Index() -> _Videio(),_Audio(),_Photo(),_Doc()
            // Index()�I�s SearchCount API ���^�ŦX���Ʋέp�A�N  �ŦX���ơB�d�߱���Ѽ� ���ѵ�{�v���Ϥ�}����
            // �A�P�_: {�v���Ϥ�}���� ��ܤ@��Metadata���e�C
            return PartialView("_Condition", model);
        }

        /// <summary>
        /// �˯���, {�v/��/��/��}�ɮ׽s���ϥΪ� �˪OID List
        /// </summary>
        /// <param name="param"> �˯��Ѽ� <see cref="SearchParameterViewModel"/> </param>
        /// <param name="mediaType">�C������(V,A,P,D)</param>
        public async Task<JsonResult> TemplateListAsync(SearchParameterViewModel param, string mediaType)
        {
            #region _Serilog.Info
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "Search",
                Method = "[TemplateListAsync]",
                EventLevel = SerilogLevelEnum.Information,
                Input = param,
                LogString = "�˯���Ƽ˪��M��.Parameter"
            });
            #endregion
            ResponseResultModel result = new ResponseResultModel(false, string.Empty, param);
            List<TemplateIdModel> templates = new List<TemplateIdModel>();
            List<SelectListItem> listItems = new List<SelectListItem>();

            try
            {
                FileTypeEnum _type = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), mediaType); //�C������Enum: V,A,P,D

                //���w�˯��Ѽƪ� ���ޮw�W��[fsINDEX]
                switch (_type)
                {
                    case FileTypeEnum.A:
                        param.fsINDEX = SearchTypeEnum.Audio_DEV.ToString(); //�u��"�n��"��Metadata
                        ////searchMetas = await this.GetSearchMeataFileNo(param);
                        //templates = await this.GetSearchMeataTemplate(param);
                        //listItems = await _arcAudioSer.GetSearchMetaAudioTemplates(searchMetas);
                        break;
                    case FileTypeEnum.P:
                        param.fsINDEX = SearchTypeEnum.Photo_DEV.ToString(); //�u��"�Ϥ�"��Metadata
                        //searchMetas = await this.GetSearchMeataFileNo(param);
                        //listItems = await _arcPhotoSer.GetSearchMetaPhotoTemplates(searchMetas);
                        break;
                    case FileTypeEnum.D:
                        param.fsINDEX = SearchTypeEnum.Doc_DEV.ToString(); //�u��"���"��Metadata
                        //searchMetas = await this.GetSearchMeataFileNo(param);
                        //listItems = await _arcDocService.GetSearchMetaDocTemplates(searchMetas);
                        break;
                    //case FileTypeEnum.S:
                    //    //
                    //    break;
                    case FileTypeEnum.V:
                    default:        //�w�]���{�v��}
                        param.fsINDEX = SearchTypeEnum.Video_DEV.ToString(); //�u��"�v��"��Metadata
                        //searchMetas = await this.GetSearchMeataFileNo(param);
                        //listItems = await _arcVideoSer.GetSearchMetaVideoTemplates(searchMetas); //���^�˯���ƪ��˪Olist                                                                   
                        break;
                }

                templates = await this.GetSearchMeataTemplate(param);
                _templateService = new TemplateService();
                listItems = _templateService.GetTemplateListItem(templates);

                result.IsSuccess = true;
                result.Data = listItems;
                result.Message = "�˯���Ƽ˪��M��";
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
                    LogString = "�˯���Ƽ˪��M��.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region ========= Marked_2020.03.12 --->> _ListAsync(List<GetArcSearchResult> model, string mediaType), �i�ѦҨϥ�SearchListAsync
        /*/// <summary>
        /// ���G�C��@�έ��� : �P�_�O�_ŪTSM API
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mediaType">�C������(V,A,P,D)</param>
        /// <returns></returns>
        //[ChildActionOnly]
        [Obsolete("�i�ѦҨϥ�SearchListAsync", true)]
        public async Task<ActionResult> _ListAsync(List<GetArcSearchResult> model, string mediaType)
        {
            var _param = new { model, mediaType };
            ResponseResultModel result = new ResponseResultModel(false, "", _param);
            try
            {
                //�˯�video�A�P�_�M�צ��LTSM�A�O�_�ݭn���o�ɮת��A TSMApi
                List<GetFileStatusResult> _fileStatus = new List<GetFileStatusResult>();
                //if (IsUseTSM.ToUpper() == "TRUE" && mediaType == FileTypeEnum.V.ToString())
                //{
                //    //TODO(21091224): ����ŪTSM API ���^��ƹL�[,��ѫe�ݩI�s
                //    #region �iCall TSM Api�j
                //    //_ApiUrl = string.Format($"{_tsmUrl}Tsm/GetFileStatusInTsm");
                //    //List<string> _fnoList = model.Select(s => s.fsFILE_NO).ToList();  //�ɮ׽s��list
                //    //var _tsmFilePath = _getService.GetTSMFilePath(_fnoList).Select(s => new FILE_NO_TSM_PATH(s)).ToList();
                //    //clsFILE_STATUS_ARGS _agrs = new clsFILE_STATUS_ARGS(_tsmFilePath);
                //    ////
                //    //var _apiResult = await new CallAPIService().ApiPostAsync(_ApiUrl, _agrs);
                //    //string _str = _apiResult.IsSuccess ? string.Format($"GetFileStatusInTsm���\: {_apiResult.Message}") : string.Format($"GetFileStatusInTsm����: {_apiResult.Message}");
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
                    LogString = "���G�C��.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }   

            return await Task.Run(() => Json(result, JsonRequestBehavior.DenyGet));
            //return PartialView("_List", model);
        }   */
        #endregion

        /// <summary>
        /// �w���@�έ���(�Ϥ��μv�������)
        /// </summary>
        /// <param name="fsSUBJECT_ID">�D�D�s�� </param>
        /// <param name="mediaType">�C������(V,A,P,D) </param>
        /// <param name="fileNo">�ɮ׽s��, �w�] �ŭ� </param>
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
                LogString = "�w��.Parameter"
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

                //Marked_20201026:�����q���e��,�ɮ׬O�_�i�H�w��(open DocViewer)
                ////(DocViewer)�i�w�����ɮ����� (�h�������H����; �����j�Ÿ�, EX: doc;docx;txt;html;)
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
                    LogString = "�w��.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return PartialView("_Preview", rtnmodel);
        }

        /// <summary>
        /// �򥻸��
        /// </summary>
        /// <param name="fsSUBJECT_ID">�D�D�s�� </param>
        /// <param name="mediaType">�C������(V,A,P,D) </param>
        /// <param name="fileNo">�ɮ׽s�� </param>
        /// <returns></returns>
        //[ChildActionOnly]
        [HttpPost]
        public ActionResult _BasicMedia(string fsSUBJECT_ID, string mediaType, string fileNo)
        {
            var _param = new { fsSUBJECT_ID, mediaType, fileNo };
            SubjectFileMetaViewModel metaModel = new SubjectFileMetaViewModel(); //��ӨϥΡu�˯����G.�򥻸�ƭ�model�v//ArcBasicMetaModel();

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
                    LogString = "�˯��򥻸��.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return PartialView("_BasicMedia", metaModel ?? new SubjectFileMetaViewModel());
        }

        /// <summary>
        /// �ʺA�����
        /// </summary>
        /// <param name="fsSUBJECT_ID">�D�D�s�� </param>
        /// <param name="mediaType">�C������(V,A,P,D) </param>
        /// <param name="fileNo">�ɮ׽s�� </param>
        /// <returns></returns>
        //[ChildActionOnly]
        [HttpPost]
        public ActionResult _DynamicMedia(string fsSUBJECT_ID, string mediaType, string fileNo)
        {
            //�ۭq���
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
                    LogString = "�˯��ʺA���.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return PartialView("_DynamicMedia", ArcPreAttributes);
        }

        /// <summary>
        /// ����v��(�u���v��������)
        /// </summary>
        /// <param name="fileNo">�ɮ׽s�� </param>
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
        /// �q���y�z
        /// </summary>
        /// <param name="fileNo">�ɮ׽s�� </param>
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
        /// �n���ɱM���T
        /// </summary>
        /// <param name="fsSUBJECT_ID">�D�D�s�� </param>
        /// <param name="fileNo">�ɮ׽s�� </param>
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
        /// ���o�Ϥ� EXIF��T
        /// </summary>
        /// <param name="fileNo">�ɮ׽s�� </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _PhotoExifInfo(string fileNo) {
            string _fno = (!string.IsNullOrEmpty(fileNo)) ? fileNo : "0";
            var _exif = _arcPhotoSer.GetPhotoExif(_fno);

            return PartialView(_exif);
        }

        /// <summary>
        /// ���o����T
        /// </summary>
        /// <param name="fileNo">�ɮ׽s��</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _DocInfo(string fileNo)
        {
            string _fno = (!string.IsNullOrEmpty(fileNo)) ? fileNo : "0";
            var _info = _arcDocService.GetDocumentInfo(_fno);

            return PartialView(_info);
        }

        /// <summary>
        /// �s��: �����+�ۭq���(�ѷӡu�D�D�P�ɮ׺��@�v���������s��\��)
        /// </summary>
        /// <param name="fileId">�ɮ�Id</param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult _Edit(int fileId)
        {
            return PartialView("_Edit");
        }
        #endregion

        #region ==========�iSearchAPI: Count, Meatadata�j==========
        /// <summary>
        /// �˯��ŦX����
        /// </summary>
        /// <param name="model">�˯��Ѽ� <see cref="SearchParameterViewModel"/></param>
        /// <returns></returns>
        [NonAction]
        public async Task<SearchCountResponseModel> GetSearchCount(SearchParameterViewModel model)
        {
            var _param = model;
            SearchCountResponseModel rtnmodel = new SearchCountResponseModel();
            _ApiUrl = string.Format($"{Config.SearchUrl}Search/SearchCount");

            try
            {
                #region �ϥΪ̸�T: �i�d�ߥؿ��`�I�v��,�i�d�߾��K�v��,�O�_���޲z��+�C��޲z��
                //var _userss = new UsersService();
                var _ur = _usersService.GetBy("", User.Identity.Name);
                string _directory = _usersService.CurrentUserDirAuth, _secret = _ur.FirstOrDefault().fsFILE_SECRET;
                bool _isadmin = _usersService.IsAdminOrMediaManager;
                //Tips_2020/04/01: �����˯���fbIS_ADMIN��� => ��true(Administrator�BMediaManager)
                #endregion

                var _apiParam = new SearchApiParameterModel(model)
                {
                    fsAUTH_DIR = _directory,
                    fsSECRET = _secret,
                    fbIS_ADMIN = _isadmin
                };
                var _apiResult = await _searchAPIService.SearchApiAsync(_ApiUrl, _apiParam);
                string _str = _apiResult.IsSuccess ? string.Format($"SearchCount���\: {_apiResult.Message}") : string.Format($"SearchCount����: {_apiResult.Message}");

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
        /// �˯��ŦX {VIDEO/AUDIO/PHOTO/DOC} ��ƦC��(�ɮ׽s���B���񤺮e��r) �iSearch/SearchMeta�j
        /// </summary>
        /// <param name="model">�˯��Ѽ� <see cref="SearchParameterViewModel"/></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<SearchMetaResponseModel>> GetSearchMeata(SearchParameterViewModel model)
        {
            var _param = model;
            List<SearchMetaResponseModel> rtnmodel = new List<SearchMetaResponseModel>();
            _ApiUrl = string.Format($"{Config.SearchUrl}Search/SearchMeta");

            try
            {
                #region �ϥΪ̸�T: �i�d�ߥؿ��`�I�v��,�i�d�߾��K�v��,�O�_���޲z��+�C��޲z��
                //var _userss = new UsersService();
                var _ur = _usersService.GetBy("", User.Identity.Name);
                string _directory = _usersService.CurrentUserDirAuth, _secret = _ur.FirstOrDefault().fsFILE_SECRET;
                bool _isadmin = _usersService.IsAdminOrMediaManager;
                //Tips_2020/04/01: �����˯���fbIS_ADMIN��� => ��true(Administrator�BMediaManager)
                #endregion

                var _apiParam = new SearchApiParameterModel(model)
                {
                    fsAUTH_DIR = _directory,//"1879;1888;1889;1903;1890;1868;1899;1873;",//
                    fsSECRET = _secret,//"0;1;2",//
                    fbIS_ADMIN = _isadmin
                };
                var _apiResult = await _searchAPIService.SearchApiAsync(_ApiUrl, _apiParam);
                string _str = _apiResult.IsSuccess ? string.Format($"SearchMeta���\: {_apiResult.Message}") : string.Format($"SearchMeta����: {_apiResult.Message}");
                string _apiData = JsonConvert.SerializeObject(_apiResult.Data);

                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Search",
                    Method = "[GetSearchMeata]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Params = _param, Result = _apiData },
                    LogString = "�˯��ŦX���.apiResponse"
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
        /// �˯��ŦX {VIDEO/AUDIO/PHOTO/DOC} �ɮ׽s����ơiSearch/SearchMetaExport�j
        /// <para> TIP: �P GetSearchMeata() �t�O: �^�Ǥ��e�u���ɮ׽s���A�S��"����r���񪺦r"�C </para>
        /// </summary>
        /// <param name="model">�˯��Ѽ� <see cref="SearchParameterViewModel"/></param>
        [NonAction]
        public async Task<List<SearchMetaResponseModel>> GetSearchMeataFileNo(SearchParameterViewModel model)
        {
            var _param = model;
            List<SearchMetaResponseModel> _list = new List<SearchMetaResponseModel>();
            _ApiUrl = string.Format($"{Config.SearchUrl}Search/SearchMetaExport");

            try
            {
                #region �ϥΪ̸�T: �i�d�ߥؿ��`�I�v��,�i�d�߾��K�v��,�O�_���޲z��+�C��޲z��
                //var _userss = new UsersService();
                var _ur = _usersService.GetBy("", User.Identity.Name);
                string _directory = _usersService.CurrentUserDirAuth, _secret = _ur.FirstOrDefault().fsFILE_SECRET;
                bool _isadmin = _usersService.IsAdminOrMediaManager;
                //Tip: �����˯���fbIS_ADMIN��� => ��true(Administrator�BMediaManager)
                #endregion

                var _apiParam = new SearchApiParameterModel(model)
                {
                    fsAUTH_DIR = _directory, //�i�Ϊ��ؿ�id
                    fsSECRET = _secret,      //�ɮ׾��K����
                    fbIS_ADMIN = _isadmin
                };
                var _apiResult = await _searchAPIService.SearchApiAsync(_ApiUrl, _apiParam);
                string _str = _apiResult.IsSuccess ? string.Format($"SearchMetaExport���\: {_apiResult.Message}") : string.Format($"SearchMetaExport����: {_apiResult.Message}");

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
        /// �˯��ŦX {VIDEO/AUDIO/PHOTO/DOC} �ɮ׽s����ƨϥΪ��˪��s��
        /// </summary>
        /// <param name="model">�˯��Ѽ� <see cref="SearchParameterViewModel"/></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<TemplateIdModel>> GetSearchMeataTemplate(SearchParameterViewModel param)
        {
            //var _param = model;
            List<TemplateIdModel> _list = new List<TemplateIdModel>();
            _ApiUrl = string.Format($"{Config.SearchUrl}Search/SearchMetaTemplate");

            try
            {
                #region �ϥΪ̸�T: �i�d�ߥؿ��`�I�v��,�i�d�߾��K�v��,�O�_���޲z��+�C��޲z��
                //var _userss = new UsersService();
                var _ur = _usersService.GetBy("", User.Identity.Name);
                string _directory = _usersService.CurrentUserDirAuth, _secret = _ur.FirstOrDefault().fsFILE_SECRET;
                bool _isadmin = _usersService.IsAdminOrMediaManager;
                //Tip: �����˯���fbIS_ADMIN��� => ��true(Administrator�BMediaManager)
                #endregion

                var _apiParam = new SearchApiParameterModel(param)
                {
                    fsAUTH_DIR = _directory, //�i�Ϊ��ؿ�id
                    fsSECRET = _secret,      //�ɮ׾��K����
                    fbIS_ADMIN = _isadmin
                };
                var _apiResult = await _searchAPIService.SearchApiAsync(_ApiUrl, _apiParam);
                string _str = _apiResult.IsSuccess ? string.Format($"SearchMetaTemplate���\: {_apiResult.Message}") : string.Format($"SearchMetaTemplate����: {_apiResult.Message}");

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

        #region �iCall TSM Api�j
        /// <summary>
        /// ���o�v���������ɮת�TSM���A
        /// <para> 2020/6/2 : ���ݨ�S3�x�s�A�n�`�Nweb.config�]�w��API�ӷ��A�^�ǰѼƦ����P�C</para>
        /// </summary>
        /// <param name="mediaType">�C���ɮ�����</param>
        /// <param name="fileNos">�h���ɮ׽s��</param>
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
             * ���ݨ�S3�x�s�A�s�W�M��(AIRMAM5.Tsm.S3)
             * TSM.S3 API URL : Http://.../AIRMAM5.Tsm.S3/Tsm/GetFileStatusInTsm
             * �ܧ�ɡA�i�� web.config �]�w�ѼƧY�i�C
             * 
             * API �^�Ǯ榡�P��TSM API�ۦP�C
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
                    result.Message = "�u���\�d�߼v������";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                //modified_20200103_�վ�^�и�Ʈ榡
                GetTsmFileStatusResult _getTsmFile = new GetTsmFileStatusResult();

                #region >>> �P�_�M�צ��LTSM�A�O�_�ݭn���o�ɮת��A TSMApi �iCall TSM Api�j
                if (IsUseTSM.ToUpper() == "TRUE")
                {
                    _getTsmFile.IsUseTSM = true;

                    List<string> _fnoList = fileNos.ToList();  //�ɮ׽s��list
                    var _tsmFilePath = _getService.GetTSMFilePath(_fnoList).Select(s => new FILE_NO_TSM_PATH(s)).ToList();
                    clsFILE_STATUS_ARGS _agrs = new clsFILE_STATUS_ARGS(_tsmFilePath);

                    var _apiResult = await new CallAPIService().ApiPostAsync(_ApiUrl, _agrs);
                    string _str = _apiResult.IsSuccess ? string.Format($"GetFileStatusInTsm���\: {_apiResult.Message}") : string.Format($"GetFileStatusInTsm����: {_apiResult.Message}");
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
                    LogString = "�ɮ�TSM���A.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region �i�˯����G�צ��ɮ�.csv�j

        /// <summary>
        /// �˯����G�צ��ɮ� async
        /// </summary>
        /// <param name="param"> �˯��Ѽ� <see cref="SearchParameterViewModel"/> </param>
        /// <param name="mediaType">�C������(V,A,P,D)</param>
        /// <remarks> 20201216_TIP: �e�ݴ���"�˯��Ѽ�"�ɡA�n���w[fnTEMP_ID]
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
                LogString = "�˯����G�ץX.Parameter"
            });
            #endregion
            ResponseResultModel result = new ResponseResultModel(false, string.Empty, param);

            var watch = new Stopwatch();
            watch.Start();
            ConfigService _configSer = new ConfigService();

            try
            {
                FileTypeEnum _type = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), mediaType);  //�C������Enum: V,A,P,D
                param.fsINDEX = (_type == FileTypeEnum.V) ? SearchTypeEnum.Video_DEV.ToString()
                    : (_type == FileTypeEnum.A) ? SearchTypeEnum.Audio_DEV.ToString()
                    : (_type == FileTypeEnum.P) ? SearchTypeEnum.Photo_DEV.ToString()
                    : (_type == FileTypeEnum.D) ? SearchTypeEnum.Doc_DEV.ToString()
                    : (_type == FileTypeEnum.S) ? SearchTypeEnum.Subject_DEV.ToString() : SearchTypeEnum.Video_DEV.ToString();  //�˯������C�|��

                //A:�˯��ŦX���ɮ׽s����ƻP����
                List<SearchMetaResponseModel> searchMetas = await this.GetSearchMeataFileNo(param);
                string filenos = string.Join(",", searchMetas.Select(s => s.fsFILE_NO));

                //�qA��ƨ��ץX�����e���(csv��r���)  �ispRPT_GET_SEARCH_EXPORT_DATA_Result�j
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
                    LogString = "�˯����G�ץXcall�w�s"
                });
                #endregion
                List<spRPT_GET_SEARCH_EXPORT_DATA_Result> searchResult = _getService.GetSearchExportData(true, int.Parse(param.fnTEMP_ID.ToString()), filenos, mediaType);
                
                //�˯��ץX�ɮ׸��|
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

                //Notice SignalR - �s�W�q���T�����
                await Task.Run(() =>
                {
                    tbmNOTIFY notify = new tbmNOTIFY
                    {
                        fsTITLE = "�˯��ץX�@�~",
                        fsCONTENT = string.Format($"�U�����|�G<br /><a target='_blank' href='{exportFileUrl}{exportFileName}' download>{exportFileUrl}{exportFileName}</a>"),
                        fnCATEGORY = (int)NotifyCategoryEnum.�w�],
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
                            MESSAGE = string.Format($" ({Thread.CurrentThread.ManagedThreadId}) �˯����G�ץX: �q���T��_DONE. ")
                        },
                        LogString = "�˯����G�ץX.�q��"
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
                    LogString = "�˯����G�ץX.Exception",
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