using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Services;
using System.Net;
using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.Utility.Extensions;
using AIRMAM5.DBEntity.Models.ArcPre;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.Filters;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 預編詮釋資料 Controller
    /// </summary>
    //[InterceptorOfException]
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class ArcPreController : BaseController
    {
        readonly string CONTR_NAEM = "ArcPre";
        readonly TemplateService _templateService;
        readonly ArcPreService _arcPreService;

        /// <summary>
        /// 
        /// </summary>
        public ArcPreController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService, ICodeService codeService)
            : base(serilogService, functionService)
        {
            _tblLogService = tblLogService;
            _serilogService = serilogService;
            _tbzCodeService = codeService;
            _templateService = new TemplateService();
            _arcPreService = new ArcPreService(serilogService);
        }
        ///* Marked_20210830 */
        //public ArcPreController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}

        /// <summary>
        /// 預編詮釋資料 主頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return PartialView("NoAuth");

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "預編詮釋資料維護"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 查詢頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult _Search()
        {
            ArcPreSearchModel m = new ArcPreSearchModel
            {
                ArcPreTypeList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.TEMP001.ToString(), true, true, true)
            };

            return PartialView("_Search", m);
        }

        /// <summary>
        ///  新增Modal-1: 選擇類別、樣版
        /// </summary>
        /// <returns></returns>
        public ActionResult _Create()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "CreateModal" });

            var ArcPreTypeList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.TEMP001.ToString(), true, true, false);
            ArcPreTypeList.Insert(0, new SelectListItem { Value = "-1", Text = "請選擇類型 -" });

            var ArcPreTemplateList = _templateService.GetTemplateList(ArcPreTypeList[0].Value);
            ArcPreTemplateList.Insert(0, new SelectListItem { Value = "-1", Text = "未選擇 -" });

            ArcPreCreateModel m = new ArcPreCreateModel
            {
                ArcPreTypeList = ArcPreTypeList,
                ArcPreTemplateList = ArcPreTemplateList
            };

            return PartialView("_Create", m);

        }
        /// <summary>
        /// 新增Modal-2: 依前一步驟選擇的類別、樣版, 取回詮釋資料欄位(變更欄位內容值), 
        /// </summary>
        /// <param name="type">類別 [fsTYPE]</param>
        /// <param name="tempid">樣板Id [fnTEMPID]</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _SubCreate(string type, int tempid)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "SubCreateModal" });
            var _param = new { type, tempid };

            var _table = _tbzCodeService.GetCodeName(TbzCodeIdEnum.TEMP001, type);
            var _template = _templateService.GetById(tempid);

            FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), _template.fsTABLE);
            ArcPreModel arcPre = new ArcPreModel()
            {
                fsTYPE = type,
                fsTYPE_NAME = _table,
                fnTEMP_ID = tempid,
                fsTEMP_NAME = _template.fsNAME
            };

            #region 樣版動態欄位數:屬性設定值
            List<ArcPreAttributeModel> attrFields = new List<ArcPreAttributeModel>();
            var getFields = _templateService.GetTemplateFieldsById(tempid);

            //foreach (var fd in getFields)
            //{
            //    if (fd == null) { continue; }
            //    ArcPreAttributeModel _arrtF = new ArcPreAttributeModel().FormatConversion(fd, mediatype);
            //    attrFields.Add(_arrtF);
            //}
            getFields.ForEach(f =>
            {
                if (f == null) { return; }
                attrFields.Add(new ArcPreAttributeModel().FormatConversion(f, mediatype));
            });

            arcPre.ArcPreAttributes = attrFields;
            #endregion

            #region _Serilog.Debug
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = CONTR_NAEM,
                Method = "[_SubCreate]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { Params = _param, Result = arcPre },
                LogString = "Parameter"
            });
            #endregion
            return PartialView("_SubCreate", arcPre);
        }

        /// <summary>
        /// 新增Modal: 儲存POST
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ArcPreModel model, FormCollection form)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();
            #region _Serilog//get值 forTEST
            //List<string> _vals = new List<string>();
            //for (int i = 0; i < form.Count; i++)
            //{
            //    _vals.Insert(i, form[i]);
            //}
            //_serilogService.SerilogWriter(new SerilogInputModel
            //{
            //    Controller = CONTR_NAEM,
            //    Method = "Create",
            //    EventLevel = SerilogLevelEnum.Debug,
            //    Input = new { Params = _param, Forms = form, Vaues = _vals },
            //    LogString = "Parameter"
            //});
            #endregion

            try
            {
                #region _檢查_
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                
                if (!ModelState.IsValid)
                {
                    foreach (var item in ModelState.Keys)
                        if (ModelState[item].Errors.Count > 0)
                        {
                            result.IsSuccess = false;
                            result.Message = ModelState[item].Errors[0].ErrorMessage;
                            result.StatusCode = HttpStatusCode.BadRequest;
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                }
                #endregion
                //#region _樣板欄位必填檢查 TODO: //Tips: 2020/02/20 前端判斷

                tbmARC_PRE _newRow = new tbmARC_PRE(model) { fdCREATED_DATE = DateTime.Now, fsCREATED_BY = User.Identity.Name };
                //【樣板自訂欄位資料(依欄位屬性給值).SetValue 】//↑改寫在資料model裡。
                //_templateService.AttriFieldsSetValue<tbmARC_PRE>(model.fnTEMP_ID, _newRow, form);

                res = _arcPreService.CreateBy(_newRow);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M001",        //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "預編詮釋資料", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Create]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = model, Result = result },
                    LogString = "新增樣板預編詮釋資料.Result"
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
                    Controller = CONTR_NAEM,
                    Method = "[Create]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, Exception = ex },
                    LogString = "新增樣板預編詮釋資料.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        ///  詳細Modal
        /// </summary>
        /// <param name="fnPREID">預編資料Id</param>
        /// <returns></returns>
        public ActionResult _Details(long fnPREID)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });
            ArcPreViewModel arcPre = new ArcPreViewModel();

            #region Main
            var getArcPre = _arcPreService.GetByParam(new spGET_ARC_PRE_Param(fnPREID, string.Empty, string.Empty, 0)).FirstOrDefault();
            if (getArcPre == null) return PartialView("_Details", arcPre);

            arcPre = new ArcPreViewModel().ConvertData(getArcPre);
            FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), getArcPre.fsTYPE);
            #endregion

            //// 樣版動態欄位【樣板自訂欄位資料(依欄位屬性 取值).GetValue 】
            //int _tempid = getArcPre == null ? 0 : getArcPre.fnTEMP_ID;
            //arcPre.ArcPreAttributes = _templateService.AttriFieldsGetValue(_tempid, getArcPre);

            #region 樣版動態欄位數:屬性設定值
            var _preAttrs = _arcPreService.GetAttributeByParam(new spGET_ARC_PRE_ATTRIBUTE_Param
            {
                fnPRE_ID = fnPREID,
                PreTempId = getArcPre == null ? 0 : getArcPre.fnTEMP_ID
            });

            //foreach (var a in _preAttrs)
            //{
            //    ArcPreAttributeModel _arrtF = new ArcPreAttributeModel().FormatConversion(a, mediatype);
            //    arcPre.ArcPreAttributes.Add(_arrtF);
            //}
            _preAttrs.ForEach(f =>
            {
                arcPre.ArcPreAttributes.Add(new ArcPreAttributeModel().FormatConversion(f, mediatype));
            });
            #endregion.

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(), 
                "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "樣板預編詮釋資料", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { ArcPreId = fnPREID }),
                User.Identity.Name);
            #endregion
            return PartialView("_Details", arcPre);
        }

        /// <summary>
        ///  刪除Modal
        /// </summary>
        /// <param name="fnpreid">預編資料Id [fnPRE_ID]</param>
        /// <returns></returns>
        public ActionResult _Delete(long fnpreid)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "DeleteModal" });
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "預編詮釋資料維護-刪除檢視"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion

            ArcPreViewModel arcPre = new ArcPreViewModel();
            #region Main
            var getArcPre = _arcPreService.GetByParam(new spGET_ARC_PRE_Param(fnpreid, string.Empty, string.Empty, 0)).FirstOrDefault();
            if (getArcPre == null) return PartialView("_Edit", arcPre);

            arcPre = new ArcPreViewModel().ConvertData(getArcPre);
            FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), getArcPre.fsTYPE);
            #endregion

            #region 樣版動態欄位數:屬性設定值
            var _preAttrs = _arcPreService.GetAttributeByParam(new spGET_ARC_PRE_ATTRIBUTE_Param
            {
                fnPRE_ID = fnpreid,
                PreTempId = getArcPre == null ? 0 : getArcPre.fnTEMP_ID
            });

            //foreach (var a in _preAttrs)
            //{
            //    ArcPreAttributeModel _arrtF = new ArcPreAttributeModel().FormatConversion(a, mediatype);
            //    arcPre.ArcPreAttributes.Add(_arrtF);
            //}
            _preAttrs.ForEach(f =>
            {
                arcPre.ArcPreAttributes.Add(new ArcPreAttributeModel().FormatConversion(f, mediatype));
            });
            #endregion

            return PartialView("_Delete", arcPre);
        }

        /// <summary>
        /// 刪除 POST
        /// </summary>
        /// <param name="fnpreid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(long fnpreid)
        {
            var _param = new { fnpreid };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢查_
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (fnpreid < 1)
                {
                    result.IsSuccess = false;
                    result.Message = "刪除編號有誤";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                res = _arcPreService.DeleteBy(fnpreid, User.Identity.Name);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M003",     //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "預編詮釋資料", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "刪除預編詮釋.Result"
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
                    Controller = CONTR_NAEM,
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, Exception = ex },
                    LogString = "刪除預編詮釋.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 查詢功能
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[InterceptorOfController(Method = "Search")]
        public ActionResult Search(string fsNAME, string fsTYPE)
        {
            fsTYPE = (fsTYPE == "*") ? "" : fsTYPE;
            var _param = new { fsNAME, fsTYPE };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);

            try
            {
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                fsNAME = fsNAME ?? string.Empty;
                fsTYPE = fsTYPE ?? string.Empty;
                var p = new spGET_ARC_PRE_Param(0, fsNAME, fsTYPE, 0);
                var get = _arcPreService.GetByParam(p)
                    .Select(s => new ArcPreMainModel().DataConvert(s)).ToList();

                result.IsSuccess = true;
                result.Message = "OK";
                result.Data = get;
                result.StatusCode = HttpStatusCode.OK;

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M005",        //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "預編詮釋資料", "OK"),
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
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Search]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        ///  編輯Modal
        /// </summary>
        /// <param name="fnPREID">預編資料Id</param>
        /// <returns></returns>
        //[InterceptorOfController(Method = "_Edit")]
        public ActionResult _Edit(long fnPREID)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "EditModal" });
            ArcPreModel arcPre = new ArcPreModel();

            #region 預編詮釋資料-Main
            var getArcPre = _arcPreService.GetByParam(new spGET_ARC_PRE_Param(fnPREID, string.Empty, string.Empty, 0)).FirstOrDefault();
            if (getArcPre == null) return PartialView("_Edit", arcPre);

            arcPre = arcPre.ConvertData(getArcPre);
            FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), getArcPre.fsTYPE);
            #endregion

            ////樣版動態欄位【樣板自訂欄位資料(依欄位屬性 取值).GetValue 】
            //int _tempid = getArcPre == null ? 0 : getArcPre.fnTEMP_ID;
            //arcPre.ArcPreAttributes = _templateService.AttriFieldsGetValue(_tempid, getArcPre);
            #region 樣版動態欄位數:屬性設定值
            var _preAttrs = _arcPreService.GetAttributeByParam(new spGET_ARC_PRE_ATTRIBUTE_Param
            {
                fnPRE_ID = fnPREID, PreTempId = getArcPre == null ? 0 : getArcPre.fnTEMP_ID
            });
            
            //foreach (var a in _preAttrs)
            //{
            //    ArcPreAttributeModel _arrtF = new ArcPreAttributeModel().FormatConversion(a, mediatype);
            //    arcPre.ArcPreAttributes.Add(_arrtF);
            //}
            _preAttrs.ForEach(f => {
                arcPre.ArcPreAttributes.Add(new ArcPreAttributeModel().FormatConversion(f, mediatype));
            });
            #endregion

            #region _Serilog.Debug
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = CONTR_NAEM,
                Method = "[_Edit]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { Params = new { fnPREID }, Result = arcPre },
                LogString = "ShowFieldsResult"
            });
            #endregion
            return PartialView("_Edit", arcPre);
        }

        /// <summary>
        /// 編輯: 儲存POST
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[InterceptorOfController(Method = "Edit")]
        public ActionResult Edit(ArcPrePostModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢核_
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (!ModelState.IsValid)
                {
                    foreach (var item in ModelState.Keys)
                    {
                        if (ModelState[item].Errors.Count > 0)
                        {
                            result.IsSuccess = false;
                            result.Message = ModelState[item].Errors[0].ErrorMessage;
                            result.StatusCode = HttpStatusCode.BadRequest;
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                    }
                }

                var _updRow = _arcPreService.GetById(model.fnPRE_ID);
                if (_updRow == null)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"無對應資料(ID={model.fnPRE_ID})");
                    result.StatusCode = HttpStatusCode.NotFound;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                //UI: 類型,使用樣板, 不可編輯。
                //樣板自訂欄位資料(欄位屬性設定值)
                var getFields = _templateService.GetTemplateFieldsById(model.fnTEMP_ID);

                _updRow.fsNAME = StringExtensions.ReplaceStr(model.fsNAME);
                _updRow.fsTITLE = StringExtensions.ReplaceStr(model.fsTITLE);
                _updRow.fsDESCRIPTION = StringExtensions.ReplaceStr(model.fsDESCRIPTION);
                _updRow.fsHASH_TAG = model.fsHashTag ?? string.Empty;
                _updRow.fdUPDATED_DATE = DateTime.Now;
                _updRow.fsUPDATED_BY = User.Identity.Name;
                foreach (var r in model.ArcPreAttributes)
                {
                    typeof(tbmARC_PRE).GetProperties().FirstOrDefault(x => x.Name == r.Field).SetValue(_updRow, r.FieldValue ?? string.Empty);
                }

                res = _arcPreService.UpdateBy(_updRow);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "預編詮釋資料", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { param = _param, Result = result },
                    LogString = "修改預編詮釋.Result"
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
                    Controller = CONTR_NAEM,
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "修改預編詮釋.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

    }
}
