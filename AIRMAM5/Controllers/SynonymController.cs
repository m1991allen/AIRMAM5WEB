using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Enums;
using System.Threading.Tasks;
using AIRMAM5.APIServer;
//using System.Configuration;
using AIRMAM5.Filters;
using AIRMAM5.DBEntity.Models.Synonym;
using AIRMAM5.DBEntity.Interface;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 系統設定 > 同義詞維護
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class SynonymController : BaseController
    {
        readonly SynonymsService _tbmSynonymsService = new SynonymsService();
        readonly SearchAPIService _searchAPIService = new SearchAPIService();

        /// <summary>
        /// Search API URL
        /// </summary>
        readonly static string _searchUrl = Config.SearchUrl;//ConfigurationManager.AppSettings["fsSEARCH_API"].ToString();//Properties.Settings.Default.fsSEARCH_API;//
        readonly string InsertSynonymApiUrl = string.Format($"{_searchUrl}Search/InsertSynonym");
        readonly string DeleteSynonymApiUrl = string.Format($"{_searchUrl}Search/DeleteSynonym");
        readonly string RebuildSynonymApiUrl = string.Format($"{_searchUrl}Search/RebuildSynonym");

        public SynonymController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService, ICodeService codeService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _tbzCodeService = codeService;
        }

        /// <summary>
        /// 同義詞維護首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckUserAuth("Synonym")) return View("NoAuth");

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "同義詞維護"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 同義詞維護查詢頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult _Search()
        {
            SynonymSearchModel rtnModel = new SynonymSearchModel();
            var synoList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.SYNO_TYPE.ToString());

            rtnModel.SynoList = synoList;
            return PartialView("_Search", rtnModel);
        }

        /// <summary>
        /// 同義詞維護查詢REsult
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(string type, string word)
        {
            var _param = new { type, word };
            type = (type == "*") ? string.Empty : type;
            ResponseResultModel result = new ResponseResultModel(true, "OK", _param);

            if (!CheckUserAuth("Synonym"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var get = _tbmSynonymsService.GetByParam(0, type, word).Select(s => new SynonymListModel().ConvertData(s));

            result.IsSuccess = true;
            result.StatusCode = HttpStatusCode.OK;
            result.Data = get;

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(), 
                "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "同義詞", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(_param),
                User.Identity.Name);
            #endregion
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 同義詞維護 > 詳細資料Modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Details(long id)
        {
            if (!CheckUserAuth("Synonym")) return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });

            var get = _tbmSynonymsService.GetByParam(id, string.Empty, string.Empty)
                .Select(s => new SynonymViewModel().ConvertData(s))
                .FirstOrDefault();

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "同義詞", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { tbmSynonyms_ID = id }),
                User.Identity.Name);
            #endregion
            return PartialView("_Details", get);
        }

        /// <summary>
        ///  同義詞維護 > 新增Modal
        /// </summary>
        /// <returns></returns>
        public ActionResult _Create()
        {
            if (!CheckUserAuth("Synonym"))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "CreateModal" });

            var _SynonymTypeList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.SYNO_TYPE.ToString());
            _SynonymTypeList.Insert(0, new SelectListItem { Value = "", Text = "請選擇分類" });
            _SynonymTypeList.Insert(1, new SelectListItem { Value = "*", Text = "全部" });

            SynonymCreateModel model = new SynonymCreateModel
            {
                SynonymTypeList = _SynonymTypeList
            };

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "同義詞-新增"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion
            return PartialView("_Create", model);
        }

        /// <summary>
        /// 新增同義詞 POST
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create(SynonymCreateModel model)
        {
            var _param = model;
            model.fsTYPE = (model.fsTYPE == "*") ? string.Empty : model.fsTYPE;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            if (!CheckUserAuth("Synonym"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            VerifyResult res = new VerifyResult();
            /*舊回傳格式*///SearchAPIReturn _apiResult = new SearchAPIReturn();
            SearchAPIResponse _apiResult = new SearchAPIResponse();
            bool _isBool = false;

            try
            {
                #region Insert tbmSynonym
                var _create = new tbmSYNONYMS()
                {
                    fsCREATED_BY = User.Identity.Name
                };
                _create.ConvertGet(model);

                if (string.IsNullOrEmpty(model.fsTEXT_LIST) || model.fsTEXT_LIST.Split(new char[] { ';' }).Count() < 2)
                {
                    result.IsSuccess = false;
                    result.Message = "至少定義二個同義詞";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                res = _tbmSynonymsService.CreateSynonyms(_create);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #endregion
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Synonym",
                    Method = "[Create]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = model, Result = result },
                    LogString = "新增同義詞(DB).Result"
                });
                #endregion

                if (res.IsSuccess)
                {
                    //加入
                    var addList = model.fsTEXT_LIST.Split(new char[] { ';' })
                            .Where(x => x != "")
                            .Select(s => new SynonymStrModel { fsSYNONYM = s }).ToList();

                    if (addList.Count() > 0)
                    {
                        _apiResult = await _searchAPIService.SynonymApiAsync(InsertSynonymApiUrl, addList);
                        #region _Serilog
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "Synonym",
                            Method = "[Create]",
                            EventLevel = SerilogLevelEnum.Information,
                            Input = new { APIurl = "InsertSynonymApiUrl", Result = _apiResult, AddList = addList },
                            LogString = "新增同義詞(API).Result"
                        });
                        #endregion

                        _isBool = _apiResult.IsSuccess;
                        _str = _isBool ? "成功" : "失敗";
                        #region _DB LOG
                        _tblLogService.Insert_L_Log(
                            TbzCodeIdEnum.MSG001.ToString(), 
                            "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                            string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "同義詞(API)", _str),
                            string.Format($"位置: {Request.UserHostAddress} "),
                            JsonConvert.SerializeObject(model),
                            User.Identity.Name);
                        #endregion
                        if (_isBool == false)
                        {
                            result.IsSuccess = _isBool;
                            result.Message = _apiResult.Message;
                            result.StatusCode = HttpStatusCode.ExpectationFailed;//417-Expectation Failed：執行失敗。
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                    }
                }

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "同義詞", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion
                result = new ResponseResultModel(res)
                {
                    Records = model,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
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
                    Controller = "Synonym",
                    Method = "[Create]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "新增同義詞.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 同義詞維護 > 編輯資料Modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Edit(long id)
        {
            if (!CheckUserAuth("Synonym")) return RedirectToAction("NoAuthModal", "Error", new { @id = "EditModal" });
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "編輯:同義詞"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { id }),
                User.Identity.Name);
            #endregion

            var _SynonymTypeList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.SYNO_TYPE.ToString());
            _SynonymTypeList.Insert(0, new SelectListItem { Value = "", Text = "請選擇分類" });
            _SynonymTypeList.Insert(1, new SelectListItem { Value = "*", Text = "全部" });

            var get = _tbmSynonymsService.GetById(id);
            SynonymEditModel model = new SynonymEditModel()
            {
                OrigSynonyms = get == null ? string.Empty : get.fsTEXT_LIST,
                SynonymTypeList = _SynonymTypeList
            }.ConvertData(get);


            return PartialView("_Edit", model);
        }

        /// <summary>
        /// 編輯存檔 post
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Edit(SynonymEditModel model)//(spGET_SYNONYMS_Result model)
        {
            if (model.fsTYPE == "*") { model.fsTYPE = string.Empty; }
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, model);
            if (!CheckUserAuth("Synonym"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            VerifyResult res = new VerifyResult();
            /*舊回傳格式*///SearchAPIReturn _apiResult = new SearchAPIReturn();
            SearchAPIResponse _apiResult = new SearchAPIResponse();
            bool _isBool = false;

            try
            {
                if (string.IsNullOrEmpty(model.fsTEXT_LIST) || model.fsTEXT_LIST.Split(new char[] { ';' }).Count() < 2)
                {
                    result.IsSuccess = false;
                    result.Message = "至少定義二個同義詞";
                    result.StatusCode = HttpStatusCode.BadRequest; //400-Bad Request：錯誤的要求。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                var get = _tbmSynonymsService.GetById(model.fnINDEX_ID);
                get.fsTEXT_LIST = model.fsTEXT_LIST;
                get.fsTYPE = model.fsTYPE ?? string.Empty;
                get.fsNOTE = model.fsNOTE ?? string.Empty;
                get.fdUPDATED_DATE = DateTime.Now;
                get.fsUPDATED_BY = User.Identity.Name;

                //(1)同義詞資料表內容更新
                res = _tbmSynonymsService.UpdateSynonyms(get);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "同義詞(DB)", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion

                //成功後, (2)更新搜索引擎
                if (res.IsSuccess)
                {
                    #region (2)更新搜索引擎
                    //刪除
                    var delList = model.OrigSynonyms.Split(new char[] { ';' })
                            .Where(x => x != "")
                            .Select(s => new SynonymStrModel { fsSYNONYM = s }).ToList();

                    if (delList.Count() > 0)
                    {
                        _apiResult = await _searchAPIService.SynonymApiAsync(DeleteSynonymApiUrl, delList);
                        #region _Serilog(Verbose)
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "Synonym",
                            Method = "[Edit]",
                            EventLevel = SerilogLevelEnum.Verbose,
                            Input = new { APIurl = "DeleteSynonymApiUrl", Result = _apiResult, DelList = delList },
                            LogString = "刪除同義詞(API).Result"
                        });
                        #endregion

                        _isBool = _apiResult.IsSuccess;
                        _str = _isBool ? "成功" : "失敗";
                        #region _DB LOG
                        _tblLogService.Insert_L_Log(
                            TbzCodeIdEnum.MSG001.ToString(), 
                            "M013",     //[@USER_ID(@USER_NAME)] 刪除 [@TARGET] 的 [@DATA_TYPE] @RESULT
                            string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, "同義詞", "搜索引擎(API)", _str),
                            string.Format($"位置: {Request.UserHostAddress} "),
                            JsonConvert.SerializeObject(model),
                            User.Identity.Name);
                        #endregion

                        if (_isBool == false)
                        {
                            result.IsSuccess = _isBool;
                            result.Message = _apiResult.Message;
                            result.StatusCode = HttpStatusCode.ExpectationFailed;
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                    }

                    //加入新的
                    var addList = model.fsTEXT_LIST
                        .Split(new char[] { ';' })
                        .Where(x => x != "")
                        .Select(s => new SynonymStrModel { fsSYNONYM = s }).ToList();

                    if (addList.Count() > 0)
                    {
                        _apiResult = await _searchAPIService.SynonymApiAsync(InsertSynonymApiUrl, addList);
                        #region _Serilog(Verbose)
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "Synonym",
                            Method = "[Edit]",
                            EventLevel = SerilogLevelEnum.Verbose,
                            Input = new { APIurl = "InsertSynonymApiUrl", Result = _apiResult, AddList = addList },
                            LogString = "加入同義詞(API).Result"
                        });
                        #endregion

                        _isBool = _apiResult.IsSuccess;
                        _str = _isBool ? "成功" : "失敗";
                        #region _DB LOG
                        _tblLogService.Insert_L_Log(
                            TbzCodeIdEnum.MSG001.ToString(), 
                            "M011",        //[@USER_ID(@USER_NAME)] 新增 [@TARGET] 的 [@DATA_TYPE] @RESULT
                            string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, "同義詞", "搜索引擎(API)", _str),
                            string.Format($"位置: {Request.UserHostAddress} "),
                            JsonConvert.SerializeObject(model),
                            User.Identity.Name);
                        #endregion
                        if (_isBool == false)
                        {
                            result.IsSuccess = _isBool;
                            result.Message = _apiResult.Message;
                            result.StatusCode = HttpStatusCode.ExpectationFailed;
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                    }
                    #endregion
                }

                result = new ResponseResultModel(res)
                {
                    Records = res.Data,//model,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Synonym",
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = model, Result = result },
                    LogString = "編輯同義詞.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Synonym",
                    Method = "Edit",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = model, Result = res},
                    LogString = "編輯同義詞.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        ///  同義詞維護 >刪除Modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Delete(long id)
        {
            if (!CheckUserAuth("Synonym"))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "DeleteModal" });
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "刪除:同義詞"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { id }),
                User.Identity.Name);
            #endregion

            var get = _tbmSynonymsService.GetByParam(id, string.Empty, string.Empty)
                        .Select(s => new SynonymViewModel().ConvertData(s))
                        .FirstOrDefault();

            return PartialView("_Delete", get);
        }

        /// <summary>
        /// 同義詞維護 >刪除 [POST]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(long id)
        {
            var _param = new { id };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();
            if (!CheckUserAuth("Synonym"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            /*舊回傳格式*///SearchAPIReturn _apiResult = new SearchAPIReturn();
            SearchAPIResponse _apiResult = new SearchAPIResponse();
            bool _isBool = false;

            try
            {
                // ---- 取得同義詞
                List<SynonymStrModel> synonymsList = new List<SynonymStrModel>();
                var get = _tbmSynonymsService.GetByParam(id);
                synonymsList = get.FirstOrDefault()
                        .fsTEXT_LIST.Split(new char[] { ';' })
                        .Where(x => x != "")
                        .Select(s => new SynonymStrModel { fsSYNONYM = s }).ToList();

                #region (1)從搜索引擎刪除同義詞
                _apiResult = await _searchAPIService.SynonymApiAsync(DeleteSynonymApiUrl, synonymsList);
                _isBool = _apiResult.IsSuccess;
                string _str = _isBool ? "成功" : "失敗";
                result.IsSuccess = _isBool;
                result.Message = _apiResult.Message;
                result.StatusCode = _isBool ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed; //417-Expectation Failed：執行失敗。
                
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M013",     //[@USER_ID(@USER_NAME)] 刪除 [@TARGET] 的 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, "同義詞", "搜索引擎(API)", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
                #endregion

                //搜索引擎刪除成功, 再刪除資料表資料
                if (_isBool)
                {
                    //(2)刪除資料表的同義詞
                    res = _tbmSynonymsService.DeleteSynonyms(id, User.Identity.Name);
                    _str = res.IsSuccess ? "成功" : "失敗";
                    result = new ResponseResultModel(res)
                    {
                        StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                    };

                    #region _DB LOG
                    _tblLogService.Insert_L_Log(
                        TbzCodeIdEnum.MSG001.ToString(),
                        "M003",     //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                        string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "同義詞(DB)", _str),
                        string.Format($"位置: {Request.UserHostAddress} "),
                        JsonConvert.SerializeObject(_param),
                        User.Identity.Name);
                    #endregion
                }
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Synonym",
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { synonymId = id, APIurl = "DeleteSynonymApiUrl", APIResult = _apiResult, Result = result },
                    LogString = "刪除同義詞.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Synonym",
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { synonymsID = id, Result = res, Exception = ex },
                    LogString = "刪除同義詞.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 全部重建
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Rebuild()
        {
            ResponseResultModel result = new ResponseResultModel(true, "OK", "");
            if (!CheckUserAuth("Synonym"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "同義詞:全部重建"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion

            /*舊回傳格式*///SearchAPIReturn _apiResult = new SearchAPIReturn();
            SearchAPIResponse _apiResult = new SearchAPIResponse();
            try
            {
                List<List<SynonymStrModel>> synonymsList = new List<List<SynonymStrModel>>();
                // (1)取所有同義詞
                var get = _tbmSynonymsService.GetByParam(0);
                if (get.Any())
                {
                    synonymsList = get.Select(s => s.fsTEXT_LIST.Split(new char[] { ';' })
                                    .Where(x => x != "")
                                    .Select(r => new SynonymStrModel { fsSYNONYM = r }).ToList())
                                .ToList();
                }

                // (2)呼叫AIRMAM5.Search.Lucene 重建
                _apiResult = await _searchAPIService.SynonymApiAsync(RebuildSynonymApiUrl, synonymsList);
                string _str = _apiResult.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M021",     //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "同義詞全部重建(API)", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(new { }),
                    User.Identity.Name);
                #endregion

                result.IsSuccess = _apiResult.IsSuccess;
                result.Message = _apiResult.Message;
                result.StatusCode = _apiResult.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Synonym",
                    Method = "[Rebuild]",
                    EventLevel = SerilogLevelEnum.Debug,
                    Input = new { APIurl = "RebuildSynonymApiUrl", APIResult = _apiResult },
                    LogString = "全部重建同義詞(API).Result"
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
                    Controller = "Synonym",
                    Method = "[Rebuild]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { APIurl = "RebuildSynonymApiUrl", APIResult = _apiResult, Exception = ex },
                    LogString = "全部重建同義詞(API).Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
