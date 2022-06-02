using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Enums;
using System.Net;
using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.Filters;
using AIRMAM5.DBEntity.Models.Template;
using AIRMAM5.DBEntity.Models.TemplateFields;
using AIRMAM5.DBEntity.Interface;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 系統設定 > 自訂欄位樣板維護
    /// </summary>
    //[InterceptorOfException]
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class ColTemplateController : BaseController
    {
        readonly TemplateService _tbmTemplateService;
        readonly TemplateFieldsService _tempFieldsService;

        public ColTemplateController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _tbzCodeService = new CodeService();
            _tbmTemplateService = new TemplateService();
            _tempFieldsService = new TemplateFieldsService();
        }

        /// <summary>
        /// 自訂欄位樣板維護 Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckUserAuth("ColTemplate")) return View("NoAuth");
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "自訂欄位樣板維護"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 查詢功能 
        /// </summary>
        /// <param name="fnTEMP_ID">編號</param>
        /// <param name="fsTABLE">樣板分類:提供使用的目的資料表 fsCODE_ID='TEMP001'</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(int fnTEMP_ID, string fsTABLE)
        {
            var _param = new { fnTEMP_ID, fsTABLE };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            if (!CheckUserAuth("ColTemplate"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var get = _tbmTemplateService.GetByParam(fnTEMP_ID, fsTABLE);
            result.IsSuccess = true;
            result.Data = get;
            result.Message = "OK";

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M005",  //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "自訂欄位樣板", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(_param),
                User.Identity.Name);
            #endregion
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #region ==========【自訂樣板】========== 新增/複製 _Copy -> [POST]Copy -> _Cog
        /// <summary>
        /// 選擇全新樣板or複制樣板Modal
        /// </summary>
        /// <returns></returns>
        public ActionResult _Copy()
        {
            if (!CheckUserAuth("ColTemplate")) return RedirectToAction("NoAuthModal", "Error", new { @id = "SelectTempleteModal" });
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "新增自訂欄位樣板"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion

            var _table = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.TEMP001.ToString(), true);
            _table.Insert(0, new SelectListItem { Value = "", Text = " 未選擇 - " });

            var _template = _tbmTemplateService.GetByParam().Select(s => new TemplateBaseModel().DataConvert(s)).ToList();

            var get = new TemplateNewCopy
            {
                TempTableList = _table,
                TemplateList = _template
            };
            return PartialView("_Copy", get);
        }

        /// <summary>
        /// 依照傳入狀態(NEW,COPY)新增Modal : 進行儲存,且Data要回傳新增的樣板Id
        /// </summary>
        /// <param name="m"> 新建Model  </param>
        [HttpPost]
        public ActionResult Copy(TemplateCreateModel m)
        {
            var _param = new { m };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢查欄位
                if (!CheckUserAuth("ColTemplate")) return View("NoAuth");
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
                if (string.IsNullOrEmpty(m.Template) || (m.Template.ToUpper() == "COPY" && m.ExistTemplate == 0))
                {
                    result.IsSuccess = false;
                    result.Message = "未選取既有樣板";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                tbmTEMPLATE _tmplate = new tbmTemplateGeneric<TemplateCreateModel>().FormatConversion(m);//new tbmTEMPLATE(m)
                //{
                //    fsCREATED_BY = User.Identity.Name,
                //    fdCREATED_DATE = DateTime.Now
                //};
                _tmplate.fsCREATED_BY = User.Identity.Name;
                _tmplate.fdCREATED_DATE = DateTime.Now;
                res = _tbmTemplateService.CreateBy(_tmplate, m.Template, m.ExistTemplate);//新增樣板儲存
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M001",        //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "COPY新增自訂樣板", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ColTemplate",
                    Method = "[Copy]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "樣板新建.Result"
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
                    Controller = "ColTemplate",
                    Method = "[Copy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region ==========【自訂樣板】========== 詳細/編輯/刪除
        /// <summary>
        /// 自訂樣板 詳細Modal
        /// </summary>
        /// <returns></returns>
        public ActionResult _Details(int id)
        {
            if (!CheckUserAuth("ColTemplate")) return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "自訂欄位樣板", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion

            var get = _tbmTemplateService.GetByParam(id)
                    .Select(s => new TemplateViewModel().FormatConversion(s))
                    //.Select(s => new TemplateViewGeneric<spGET_TEMPLATE_Result>().FormatConversion(s))
                    .FirstOrDefault();
            return PartialView("_Details", get);
        }

        /// <summary>
        /// 自訂樣板 編輯Model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Edit(int id)
        {
            if (!CheckUserAuth("ColTemplate")) return RedirectToAction("NoAuthModal", "Error", new { @id = "EditModal" });

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "自訂樣板編輯"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion

            TemplateEditModel md = new TemplateEditModel
            {
                TableList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.TEMP001.ToString()),
            };

            md.TableList.Insert(0, new SelectListItem { Value = "-1", Text = "(未選擇)" });

            var get = _tbmTemplateService.GetByParam(id)
                //.Select(s => new TemplateEditGeneric<spGET_TEMPLATE_Result>().FormatConversion(s))
                .Select(s => md.FormatConversion(s))
                .FirstOrDefault();

            return PartialView("_Edit", get);
        }

        /// <summary>
        /// 自訂樣板 編輯Model 存檔POST
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(TemplateEditModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, "", _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢查_
                if (!CheckUserAuth("ColTemplate")) return View("NoAuth");
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

                var get = _tbmTemplateService.GetByParam(model.fnTEMP_ID)//.Select(s => new tbmTEMPLATE(s))
                    .Select(s => new tbmTemplateGeneric<spGET_TEMPLATE_Result>().FormatConversion(s))
                    .FirstOrDefault();

                if (get == null)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"樣板編號查無資料({model.fnTEMP_ID}) ");
                    result.StatusCode = HttpStatusCode.NotFound;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                get.fsNAME = model.fsNAME;
                get.fcIS_SEARCH = model.IsSearch ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString();
                get.fsDESCRIPTION = model.fsDESCRIPTION ?? string.Empty;
                get.fdUPDATED_DATE = DateTime.Now;
                get.fsUPDATED_BY = User.Identity.Name;

                res = _tbmTemplateService.UpdateBy(get);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",    //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "自訂樣板", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    Data = res.Data,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ColTemplateCode",
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { param = _param, Result = result },
                    LogString = "樣板編輯.Result"
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
                    Controller = "ColTemplateCode",
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = model, Result = res, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 自訂樣板 刪除Modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Delete(int id)
        {
            if (!CheckUserAuth("ColTemplate"))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "DeleteModal" });

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "自訂樣板刪除"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion

            var get = _tbmTemplateService.GetByParam(id)
                    //.Select(s => new TemplateEditModel().FormatConversion(s))
                    .Select(s => new TemplateViewModel().FormatConversion(s))
                    .FirstOrDefault();

            return PartialView("_Delete", get);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var _param = new { id };
            ResponseResultModel result = new ResponseResultModel(true, "", _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢查_
                if (!CheckUserAuth("ColTemplate"))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden; //403-Forbidden：禁止使用。沒有權限。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (id < 1)
                {
                    result.IsSuccess = false;
                    result.Message = "刪除編號有誤";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
                res = _tbmTemplateService.DeleteBy(id, User.Identity.Name);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M003",    //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "自訂樣板", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ColTemplate",
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "刪除樣板.Result"
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
                    Controller = "ColTemplate",
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Result = res, exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion 

        #region ==========【自訂樣板欄位】========== 詳細/新增/編輯/刪除
        /// <summary>
        /// 自訂樣板欄位詳細Modal
        /// </summary>
        /// <param name="id"> 樣版編號 fnTEMP_ID </param>
        /// <returns></returns>
        public ActionResult _Cog(int id)
        {
            var _param = new { id };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            if (!CheckUserAuth("ColTemplate")) return View("NoAuth");
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "編輯樣板欄位"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion

            TemplateFieldsEditModel get = _tbmTemplateService.GetFieldsEditById(id);
            get.FieldTypes = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.TEMP002.ToString(), true, false);
            get.TableList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.TEMP001.ToString());

            /* return Model : TemplateFieldsEditModel()
             {
                TemplateMain      : new tbmTEMPLATE,                //自訂樣板主資料表
                CustomFieldList   : List<new ChooseTypeViewModel>,  //自訂樣板欄位資料清單
                ✘-CustomField       : new ChooseTypeViewModel         //自訂樣板欄位
                FieldTypes        : List<SelectListItem>()          //自訂欄位資料型別下拉選單
                TableList         : List<SelectListItem>            //自訂樣板欄位資料
             }
             */

            #region _Serilog.Debug
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "ColTemplate",
                Method = "[_Cog]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { Param = new { TempID = id }, Result = get },
                LogString = "樣版-設定鈕[_Cog].Result"
            });
            #endregion
            return PartialView("_Cog", get);
        }

        /// <summary>
        /// 新增欄位表單前 依據選擇種類顯示
        /// </summary>
        /// <param name="fnTEMP_ID">自訂樣板ID </param>
        /// <param name="FieldType">欄位資料型態 </param>
        /// <returns></returns>
        public ActionResult _ChooseType(int fnTEMP_ID, string FieldType)
        {
            var _fields = _tbmTemplateService.GetTemplateFieldsById(fnTEMP_ID).ToList();
            int _orderNum = (_fields.Any() && _fields.First() != null) ? _fields.OrderByDescending(b => b.fnORDER).FirstOrDefault().fnORDER : 1;

            ChooseTypeViewModel res = new ChooseTypeViewModel
            {
                FieldOrder = _orderNum + 1,
                FieldType = FieldType,
                FieldTypeEnum = (CodeTEMP002Enum)Enum.Parse(typeof(CodeTEMP002Enum), FieldType)
            };
            res.FieldLen = res.FieldTypeEnum == CodeTEMP002Enum.INTEGER ? 5 
                : res.FieldTypeEnum == CodeTEMP002Enum.DATETIME ? 22 
                    : res.FieldTypeEnum == CodeTEMP002Enum.CODE ? 20 : 100;
            res.IsSearch = false;

            if (res.FieldTypeEnum == CodeTEMP002Enum.CODE)
            {
                res.CustomerCodeList = _tbzCodeService.GetMainSubList(CodeSetTypeEnum.C.ToString(), true, false, false).ToList();
            }

            #region 取目前可用的 fsFIELD 欄位值序號
            var fieldsList = (_fields.Any() && _fields.First() != null) ? _fields.OrderBy(b => b.fsFIELD).Select(s => s.fsFIELD).ToList() : new List<string>();
            for (int i = 1; i <= 60; i++)
            {
                string _field = string.Format($"fsATTRIBUTE{i}");
                if (fieldsList.Exists(x => x == _field)) { continue; } //欄位名稱已存在
                res.Field = _field;//res.fsFIELD = _field;
                break;
            }
            #endregion

            #region _Serilog.debug
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "ColTemplate",
                Method = "[_ChooseType]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { Param = new { fnTEMP_ID, FieldType }, Result = res },
                LogString = "樣板欄位屬性資訊.Result"
            });
            #endregion
            return PartialView("_ChooseType", res);
        }

        /// <summary>
        /// 新增 樣板欄位 存檔POST
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddField(TemplateFieldsModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();
            if (model.IsNullable == false && string.IsNullOrEmpty(model.FieldDef))
            {
                result.IsSuccess = false;
                result.Message = "不可為空值時,必須填寫預設值!";
                result.StatusCode = HttpStatusCode.BadRequest;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            try
            {
                #region _檢核_
                if (!CheckUserAuth("ColTemplate")) return View("NoAuth");
                //{
                //    result.IsSuccess = false;
                //    result.Message = "您無權限使用此網頁";
                //    result.StatusCode = HttpStatusCode.Forbidden; //403-Forbidden：禁止使用。沒有權限。
                //    return Json(result, JsonRequestBehavior.DenyGet);
                //}
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
                #region _樣板欄位名稱是否重複
                bool isHad = _tempFieldsService.ChkTemplateFieldName(model.fnTEMP_ID, model.FieldName);
                if (isHad)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"欄位名稱:{model.FieldName} 已存在! ");
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
                #region _僅有文字欄位可使用進階查詢//
                //if(model.IsSearch && model.FieldType != "NVARCHAR")
                //{
                //    result.IsSuccess = false;
                //    result.Message = string.Format($"欄位名稱:{model.FieldName} 僅有文字欄位可進階查詢! ");
                //    result.StatusCode = HttpStatusCode.BadRequest;
                //    return Json(result, JsonRequestBehavior.DenyGet);
                //}
                #endregion
                // 2021/06/25 david 進階檢索數量放寬6個
                #region _樣板欄位中,進階檢索數量不可超過6個
                if (model.IsSearch)
                {
                    bool isExceed = _tempFieldsService.ChkSearchFieldOverNum(model.fnTEMP_ID, model.Field, model.IsSearch);
                    if (isExceed)
                    {
                        result.IsSuccess = false;
                        result.Message = "進階搜尋不可超過6個欄位! ";
                        result.StatusCode = HttpStatusCode.BadRequest;
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                }
                #endregion

                //tbmTEMPLATE_FIELDS _fields = new tbmTEMPLATE_FIELDS(model)
                //{
                //    fnTEMP_ID = model.fnTEMP_ID, //必需有fnTEMP_ID,才能存自訂欄位.
                //    fsCREATED_BY = User.Identity.Name
                //};  /* modified_20210719 */
                tbmTEMPLATE_FIELDS _fields = new tbmTEMPLATE_FIELDS().FormatConvert(model);
                _fields.fnTEMP_ID = model.fnTEMP_ID; //必需有fnTEMP_ID,才能存自訂欄位.
                _fields.fsCREATED_BY = User.Identity.Name;

                //TIPS_2020/4/7:自訂欄位=DATETIME、預設值=空值時,自動帶入系統日期時間
                var _type = (CodeTEMP002Enum)Enum.Parse(typeof(CodeTEMP002Enum), model.FieldType);
                switch (_type)
                {
                    case CodeTEMP002Enum.DATETIME:
                        _fields.fsDEFAULT = model.FieldDef ?? string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                        break;
                    case CodeTEMP002Enum.CODE:
                        //"必填"又無指定預設值, 取代碼資料第一筆.
                        _fields.fsDEFAULT = string.IsNullOrEmpty(model.FieldDef)
                            ? (model.IsNullable ? string.Empty : _tbzCodeService.GetCodeDetail(model.FieldCodeId).FirstOrDefault().fsCODE)
                            : model.FieldDef;
                        break;
                    case CodeTEMP002Enum.INTEGER:
                        //_fields.fsDEFAULT = (string.IsNullOrEmpty(model.FieldDef)) ? "1" : model.FieldDef;
                        _fields.fsDEFAULT = (model.IsNullable == false ? (string.IsNullOrEmpty(model.FieldDef) ? "1" : model.FieldDef) : model.FieldDef);
                        break;
                    case CodeTEMP002Enum.NVARCHAR:
                    default:
                        //_fields.fsDEFAULT = (string.IsNullOrEmpty(model.FieldDef)) ? "(預設值)" :model.FieldDef;
                        _fields.fsDEFAULT = (model.IsNullable == false ? (string.IsNullOrEmpty(model.FieldDef) ? "(預設值)" : model.FieldDef) : model.FieldDef);
                        break;
                }

                res = _tempFieldsService.CreateField(_fields);
                string _str = res.IsSuccess ? "成功" : "失敗";

                //提供前端顯示
                _param.Field = ((tbmTEMPLATE_FIELDS)res.Data).fsFIELD;
                res.Data = new ChooseTypeViewModel().FormatConversion(_fields);

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M001",    //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "樣板欄位", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ColTemplate",
                    Method = "[AddField]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "新增樣板欄位.Result"
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
                    Controller = "ColTemplate",
                    Method = "[AddField]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 編輯 樣板欄位 存檔POST
        /// <para>前端可編輯欄位: 排序,欄位名稱,是否必填(是否可為NULL),內容上限,預設值,進階檢索</para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditField(TemplateFieldsModel model)
        {
            if (!CheckUserAuth("ColTemplate")) return View("NoAuth");

            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢查欄位
                if (!ModelState.IsValid)
                {
                    foreach (var item in ModelState.Keys)
                        if (ModelState[item].Errors.Count > 0)
                        {
                            result = new ResponseResultModel(false, ModelState[item].Errors[0].ErrorMessage, _param)
                            {
                                StatusCode = HttpStatusCode.BadRequest,
                            };
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                }
                #endregion
                #region _樣板欄位名稱是否重複
                bool isHad = _tempFieldsService.ChkTemplateFieldName(model.fnTEMP_ID, model.FieldName, model.Field);
                if (isHad)
                {
                    result = new ResponseResultModel(false, string.Format($"位名稱:{model.FieldName} 已存在! "), model)
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                    };
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
                #region _僅有文字欄位可使用進階查詢
                if (model.IsSearch && model.FieldType == "CODE")
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"欄位名稱:{model.FieldName} 自訂代碼欄位不可進階查詢! ");
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                // 2021/06/25 david 進階檢索數量放寬6個
                #region _樣板欄位中,進階檢索數量不可超過6個
                if (model.IsSearch)
                {
                    bool isExceed = _tempFieldsService.ChkSearchFieldOverNum(model.fnTEMP_ID, model.Field, model.IsSearch);
                    if (isExceed)
                    {
                        result = new ResponseResultModel(false, "進階搜尋不可超過6個欄位! ", model)
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                        };
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                }
                #endregion

                var _upd = _tempFieldsService.GetFieldBy(model.fnTEMP_ID, model.Field);
                _upd.fsFIELD_NAME = model.FieldName;
                _upd.fnORDER = model.FieldOrder;
                _upd.fsISNULLABLE = model.IsNullable ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString();
                _upd.fnFIELD_LENGTH = model.FieldLen;
                _upd.fnCTRL_WIDTH = model.FieldWidth;
                //_upd.fsDEFAULT = model.FieldDef ?? string.Empty;

                //TIPS_2020/4/7:自訂欄位=DATETIME、預設值=空值時,自動帶入系統日期時間
                var _type = (CodeTEMP002Enum)Enum.Parse(typeof(CodeTEMP002Enum), model.FieldType);
                switch (_type)
                {
                    case CodeTEMP002Enum.DATETIME:
                        _upd.fsDEFAULT = model.FieldDef ?? string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                        break;
                    case CodeTEMP002Enum.CODE:
                        //代碼資料第一筆
                        _upd.fsDEFAULT = model.FieldDef ?? _tbzCodeService.GetCodeDetail(_upd.fsCODE_ID).FirstOrDefault().fsCODE;
                        break;
                    case CodeTEMP002Enum.INTEGER:
                        _upd.fsDEFAULT = model.FieldDef ?? "0";
                        break;
                    case CodeTEMP002Enum.NVARCHAR:
                        //
                    default:
                        _upd.fsDEFAULT = model.FieldDef ?? string.Empty;
                        break;
                }
                _upd.fsIS_SEARCH = model.IsSearch ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString();
                _upd.fdUPDATED_DATE = DateTime.Now;
                _upd.fsUPDATED_BY = User.Identity.Name;

                res = _tempFieldsService.UpdateField(_upd); //res.Data => tbmTEMPLATE_FIELDS()
                string _str = res.IsSuccess ? "成功" : "失敗";
                res.Data = new ChooseTypeViewModel(_upd.fsCODE_ID).FormatConversion(_upd);

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",    //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "樣板欄位", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ColTemplateCode",
                    Method = "[EditField]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { param = _param, Result = result },
                    LogString = "編輯樣板欄位.Result"
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
                    Controller = "ColTemplate",
                    Method = "[EditField]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 刪除 樣板欄位 存檔POST
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteField(int id, string field)
        {
            var _param = new { id, field };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢核_
                if (!CheckUserAuth("ColTemplate"))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden; //403-Forbidden：禁止使用。沒有權限。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (id < 1)
                {
                    result.IsSuccess = false;
                    result.Message = "刪除編號有誤";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (string.IsNullOrEmpty(field))
                {
                    result.IsSuccess = false;
                    result.Message = "刪除欄位名稱有誤";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                string _urname = User.Identity.Name;
                res = _tempFieldsService.DeleteField(id, field, _urname);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M003",    //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "自訂樣板", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ColTemplate",
                    Method = "[DeleteField]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "刪除樣板欄位.Result"
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
                    Controller = "ColTemplate",
                    Method = "[DeleteField]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion


        /// <summary>
        /// 樣板選單
        /// </summary>
        /// <param name="table">提供使用的目的資料表 fsCODE_ID= TEMP001</param>
        /// <param name="issearch">是否可檢索</param>
        /// <returns></returns>
        public JsonResult GetTemplateList(string table)
        {
            var get = _tbmTemplateService.GetTemplateList(table);
            return Json(get, JsonRequestBehavior.AllowGet);
        }
    }
}
