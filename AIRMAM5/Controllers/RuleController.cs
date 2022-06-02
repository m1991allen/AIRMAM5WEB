using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Rule;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 規則庫
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class RuleController : BaseController
    {
        readonly string CONTR_NAEM = "Rule";
        private RuleService _ruleService;

        public RuleController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService, ICodeService codeService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _tbzCodeService = codeService;
            _ruleService = new RuleService(serilogService);
        }

        /// <summary>
        /// 首頁        
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckUserAuth(CONTR_NAEM)) { return View("NoAuth"); }
            
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "流程規則設定"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 規則查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult _Search()
        {
            return PartialView();
        }

        /// <summary>
        /// 新增規則燈箱：規則主表[tbmRULE]+規則條件[tbmRULE_FILTER]
        /// </summary>
        /// <returns></returns>
        public ActionResult _Create()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "CreateModal" });
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "流程規則設定-新增"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion

            #region >>> Model初始
            //_流程類型 List
            var ruleLst = _ruleService.GetRuleListForCreate();
            //未指定流程類型, 以 ruleLst第一筆為預設
            string type = ruleLst.FirstOrDefault().Value;
            var _rule = _ruleService.GetRuleBy(type).FirstOrDefault();
            //_規則資料表 List
            var _ruletable = _ruleService.UnspecifyRuleTableColumns(type);
            List<FilterTableModel> _tableList = _ruletable.GroupBy(g => new { g.fsTABLE })
                .Select(s => new FilterTableModel
                {
                    TableName = s.Key.fsTABLE,
                    TableDesc = _ruletable.Where(x => x.fsTABLE == s.Key.fsTABLE).FirstOrDefault().fsTABLE_NAME ?? string.Empty,
                    Properties = _ruletable.Where(x => x.fsTABLE == s.Key.fsTABLE).Select(f => new FieldInfo().DataConvert(f, _tbzCodeService)).ToList()
                }).ToList();

            //TIPS: 已建立的流程規則,不提供選項。
            CreateRuleViewModel md = new CreateRuleViewModel
            {
                RuleCategory = type,
                RuleCategoryList = ruleLst ?? new List<SelectListItem>(),
                RuleName = _rule == null ? string.Empty : _rule.fsRULENAME,
                TableList = _tableList
            };
            #endregion

            #region _Serilog.Debug
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = CONTR_NAEM,
                Method = "[_Create]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { ViewModel = md },
                LogString = "新增規則燈箱.Result"
            });
            #endregion
            return PartialView(md);
        }

        #region ==========【新增流程+規則 頁面欄位資料】==========
        /// <summary>
        /// 新增 流程+規則 頁面欄位資料
        /// </summary>
        /// <param name="ruletype">規則類別(RULE): 參考代碼"RULE" </param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CreateRuleParams(string ruletype)
        {
            var _param = new { ruletype };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);

            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = (HttpStatusCode)403.8; //403.8 - 網站存取遭拒。
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            #region /* ↓↓↓ Modified_20210902 *///CreateRuleViewModel md = new CreateRuleViewModel(ruletype);
            //_流程類型 List
            var ruleLst = _ruleService.GetRuleListForCreate();

            //未指定流程類型, 以 ruleLst第一筆為預設
            ruletype = string.IsNullOrEmpty(ruletype) ? ruleLst.FirstOrDefault().Value : ruletype;
            var _rule = _ruleService.GetRuleBy(ruletype).FirstOrDefault();

            //_規則資料表 List
            var _ruletable = _ruleService.UnspecifyRuleTableColumns(ruletype);
            List<FilterTableModel> _tableList = _ruletable.GroupBy(g => new { g.fsTABLE })
                .Select(s => new FilterTableModel
                {
                    TableName = s.Key.fsTABLE,
                    TableDesc = _ruletable.Where(x => x.fsTABLE == s.Key.fsTABLE).FirstOrDefault().fsTABLE_NAME ?? string.Empty,
                    Properties = _ruletable.Where(x => x.fsTABLE == s.Key.fsTABLE).Select(f => new FieldInfo().DataConvert(f, _tbzCodeService)).ToList()
                }).ToList();

            //TIPS: 已建立的流程規則,不提供選項。
            CreateRuleViewModel md = new CreateRuleViewModel
            {
                RuleCategory = ruletype,
                RuleCategoryList = ruleLst ?? new List<SelectListItem>(),
                RuleName = _rule == null ? string.Empty : _rule.fsRULENAME,
                TableList = _tableList
            };
            #endregion

            result.IsSuccess = true;
            result.Message = "OK";
            result.Data = md;
            result.StatusCode = (HttpStatusCode)200;

            #region _Serilog.Verbose
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = CONTR_NAEM,
                Method = "[CreateRuleParams]",
                EventLevel = SerilogLevelEnum.Verbose,
                Input = new { Param = _param, Result = md },
                LogString = "新增規則條件資料.Result"
            });
            #endregion
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增 流程+規則條件 POST
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(CreateRuleModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢查_
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden; //403 - 禁止使用 //(HttpStatusCode)403.8; //403.8 - 網站存取遭拒。 //
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                //
                if (_ruleService.RuleIsExists(model.RuleMaster.RuleCategory))
                {
                    result.IsSuccess = false;
                    result.Message = "流程規則已存在[tbmRule]";
                    result.StatusCode = HttpStatusCode.ExpectationFailed; //417 - 執行失敗
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                res = _ruleService.CreateNewRule(model, CurrentUser.UserName);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "規則:" + model.RuleMaster.RuleCategory, _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Create]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = "新建流程規則.Result"
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
                    Input = new { param = _param, res, exception = ex },
                    LogString = "新建流程規則.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region ---【流程規則主表 [tbmRULE]】---
        /// <summary>
        /// 編輯:流程規則(Main)燈箱
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult _CategoryEdit(string category)
        {
            if (!CheckUserAuth(CONTR_NAEM))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "EditCategoryModal" });

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "編輯:流程規則"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { category }),
                User.Identity.Name);
            #endregion
            var get = _ruleService.GetRuleBy(category).Select(s => new EditRuleModel().DataConvert(s)).FirstOrDefault();

            #region _Serilog.Debug
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = CONTR_NAEM,
                Method = "[_CategoryEdit]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { category, Result = get },
                LogString = "編輯規則燈箱.Result"
            });
            #endregion
            return PartialView(get);
        }

        /// <summary>
        ///  編輯:流程規則(Main) SAVE
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CategoryEdit(EditRuleModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢查_
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden; //403 - 禁止使用 //(HttpStatusCode)403.8; //403.8 - 網站存取遭拒。 //
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                //
                if (!_ruleService.RuleIsExists(model.RuleCategory))
                {
                    result.IsSuccess = false;
                    result.Message = "流程規則已不存在,請重新查詢.";
                    result.StatusCode = HttpStatusCode.ExpectationFailed; //417 - 執行失敗
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
                var get = _ruleService.GetRuleBy(model.RuleCategory).FirstOrDefault();
                get.fsRULENAME = model.RuleName;
                get.fsNOTE = model.Note;
                get.fdUPDATED_DATE = DateTime.Now;
                get.fsUPDATED_BY = CurrentUser.UserName;

                res = _ruleService.Update(get);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "流程規則:" + model.RuleCategory, _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[CategoryEdit]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = "編輯流程規則存檔.Result"
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
                    Controller = CONTR_NAEM,
                    Method = "[CategoryEdit]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, exception = ex },
                    LogString = "編輯規則存檔.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 流程規則 是否啟用 (Master)  ---------------------TODO----------------
        /// </summary>
        /// <param name="category">流程代碼</param>
        /// <param name="isActive">是否啟用</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ActiveCategory(string category, bool isActive)
        {
            var _param = new { category, isActive };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            #region _檢查
            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden; //403 - 禁止使用 //(HttpStatusCode)403.8; //403.8 - 網站存取遭拒。 //
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            //
            if (!_ruleService.RuleIsExists(category))
            {
                result.IsSuccess = false;
                result.Message = "流程規則已不存在,請重新查詢.";
                result.StatusCode = HttpStatusCode.ExpectationFailed; //417 - 執行失敗
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "是否啟用:流程"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { category }),
                User.Identity.Name);
            #endregion

            try
            {
                var get = _ruleService.GetRuleBy(category).FirstOrDefault();
                get.fbISENABLED = isActive;
                get.fdUPDATED_DATE = DateTime.Now;
                get.fsUPDATED_BY = CurrentUser.UserName;

                res = _ruleService.Update(get);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "是否啟用流程規則:" + category, _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[CategoryEdit]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "是否啟用流程規則存檔.Result"
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
                    Controller = CONTR_NAEM,
                    Method = "[ActiveCategory]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, exception = ex },
                    LogString = "是否啟用流程規則存檔.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region  ---【流程規則條件 [tbmRULE_FILTER]】---
        /// <summary>
        /// 流程的規則條件 是否啟用 (Detail)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ActiveRuleFilter(RuleFilterActiveModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();
            var ruleInfo = _ruleService.GetRule(model.RuleCategory, model.Table, model.Column).FirstOrDefault();
            string ruleName = ruleInfo.fsCOLUMN_NAME;
            string _act = model.IsActive ? string.Format($"{model.RuleCategory}流程{ruleName}規則-啟用") : string.Format($"{model.RuleCategory}流程{ruleName}規則-停用");

            #region --檢查--
            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden; //403 - 禁止使用 //(HttpStatusCode)403.8; //403.8 - 網站存取遭拒。 //
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            //
            if (!_ruleService.IsExists(model.RuleCategory, model.Table, model.Column))
            {
                result.IsSuccess = false;
                result.Message = "流程規則條件已不存在,請重新查詢.";
                result.StatusCode = HttpStatusCode.ExpectationFailed; //417 - 執行失敗
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "是否啟用:流程規則條件"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(model),
                User.Identity.Name);
            #endregion

            try
            {
                var get = _ruleService.GetRuleFilterBy(model.RuleCategory, model.Table, model.Column).FirstOrDefault();
                get.fbISENABLED = model.IsActive;
                get.fdUPDATED_DATE = DateTime.Now;
                get.fsUPDATED_BY = CurrentUser.UserName;
                res = _ruleService.UpdateSignleFilter(get);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, _act, _str),
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
                    Method = "[CategoryEdit]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = _act + ".Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                //result.ErrorException = ex;
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[ActiveCategory]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, exception = ex },
                    LogString = _act + ".Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 新增:流程規則燈箱
        /// </summary>
        /// <param name="category">key值:流程類別</param>
        /// <returns></returns>
        public ActionResult _SubCreate(string category)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "CreateSubModal" });

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "流程規則:新增子規則燈箱"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { category }),
                User.Identity.Name);
            #endregion
            return PartialView();
        }

        /// <summary>
        /// 新增:流程規則條件 SAVE (子規則)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubCreate(EditRuleFilterModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(false, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢查_
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden; //403 - 禁止使用 //(HttpStatusCode)403.8; //403.8 - 網站存取遭拒。 //
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                //確認流程規則中,是否有相同的條件: table+column
                if (_ruleService.IsExists(model.RuleCategory, model.TargetTable, model.FilterField))
                {
                    result.IsSuccess = false;
                    result.Message = "流程條件已存在,請操作'編輯'調整邏輯條件";
                    result.StatusCode = HttpStatusCode.ExpectationFailed; //417 - 執行失敗
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                //當為全站審資料時,給予其他預設值
                if (model.TargetTable =="*" && model.FilterField == "*")
                {
                    _param.Operator = "*";
                    _param.FieldType = "STRING";
                    _param.Note = "全站審";
                    _param.Priority = 0;
                }
                #endregion

                List<tbmRULE_FILTER> _ins = new List<tbmRULE_FILTER>
                {
                    new tbmRULE_FILTER(model)
                    {
                        fsCREATED_BY = CurrentUser.UserName
                    }
                };

                res = _ruleService.Create_RuleFilters(_ins);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "規則條件:" + model.RuleCategory, _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };

                //回傳資料
                var _get = _ruleService.GetRule(model.RuleCategory, model.TargetTable, model.FilterField)
                    .Select(e => new RuleListModel().ConvertData(e)).FirstOrDefault();

                result.Data = _get;
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[SubCreate]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "新增流程規則條件.Result"
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
                    Method = "[SubCreate]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Result = res, exception = ex },
                    LogString = "新增流程規則條件.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        ///  編輯:流程規則條件燈箱
        /// </summary>
        /// <param name="category">key值:流程類別</param>
        /// <param name="table">key值:表單類別</param>
        /// <param name="field">key值:欄位類別</param>
        /// <returns></returns>
        public ActionResult _Edit(string category, string table, string field)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "EditModal" });
            var _param = new { category, table, field };

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "編輯:流程規則條件"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(_param),
                User.Identity.Name);
            #endregion
            var md = _ruleService.GetRule(category, table, field)
                .Select(s => new EditRuleFilterViewModel().DataConvert(s))
                .FirstOrDefault();

            //_規則資料表欄位屬性 List
            md.Properties = _ruleService.GetRuleTableBy(md.RuleCategory, md.TargetTable)
                .Where(x => x.fsCOLUMN == md.FilterField)
                .Select(s => new FieldInfo().DataConvert(s, _tbzCodeService)).FirstOrDefault();

            #region _Serilog.Debug
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = CONTR_NAEM,
                Method = "[_Edit]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { Param = _param, Model = md },
                LogString = "編輯規則條件.Result"
            });
            #endregion
            return PartialView(md);
        }

        /// <summary>
        /// 編輯:流程條件[tbmRULE_FILTER]
        /// <para> 可編輯欄位: fsPRIORITY, fsWHERE_CLAUSE, fsOPERATOR, fbISENABLED, fsFILTERVALUE </para>   
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(EditRuleFilterModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(false, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = (HttpStatusCode)403.8; //403.8 - 網站存取遭拒。
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var get = _ruleService.GetRuleFilterBy(model.RuleCategory, model.TargetTable, model.FilterField);//.FirstOrDefault();
                if (get == null)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"流程規則條件已不存在,請重新查詢。({model.RuleCategory}-{model.TargetTable}-{model.FilterField})");
                    result.StatusCode = HttpStatusCode.ExpectationFailed; //417 - 執行失敗
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                get.ForEach(a => {
                    a.fnPRIORITY = model.Priority;
                    a.fsWHERE_CLAUSE = model.WhereClause;
                    a.fsOPERATOR = model.Operator;
                    a.fbISENABLED = model.IsEnabled;
                    a.fsFILTERVALUE = model.FilterValue;
                    a.fsNOTE = model.Note ?? string.Empty;
                    a.fdUPDATED_DATE = DateTime.Now;
                    a.fsUPDATED_BY = "user.ass";
                });

                res = _ruleService.Update_RuleFilters(get);
                string _str = res.IsSuccess ? "成功" : "失敗"
                    , _s2 = string.Format($"{model.RuleCategory} 規則表:{model.TargetTable}")
                    , _s3 = string.Format($"【{model.FilterField}】 規則條件");
                //
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M012",     //[@USER_ID(@USER_NAME)] 修改 [@TARGET] 的 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, _s2, _s3, _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                //回傳資料
                var _get = _ruleService
                            .GetRule(model.RuleCategory, model.TargetTable, model.FilterField)
                            .Select(e => new RuleListModel().ConvertData(e)).FirstOrDefault();

                result.Data = _get;
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "編輯規則條件.Result"
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
                    Input = new { param = _param, Result = res, exception = ex },
                    LogString = "編輯規則條件.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        ///  刪除:流程規則條件燈箱
        /// </summary>
        /// <param name="category">key值:流程類別</param>
        /// <param name="table">key值:表單類別</param>
        /// <param name="field">key值:欄位類別</param>
        /// <returns></returns>
        public ActionResult _Delete(string category, string table, string field)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "DeleteModal" });
            var _param = new { category, table, field };
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "刪除:流程規則條件"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(_param),
                User.Identity.Name);
            #endregion

            var md = _ruleService.GetRule(category, table, field)
                .Select(s => new EditRuleFilterViewModel().DataConvert(s))
                .FirstOrDefault();

            return PartialView(md);
        }

        /// <summary>
        /// 刪除:流程規則條件燈箱 SAVE
        /// </summary>
        /// <param name="category">key值:流程類別</param>
        /// <param name="table">key值:表單類別</param>
        /// <param name="field">key值:欄位類別</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Delete(string category, string table, string field)
        {
            var _param = new { category, table, field };
            ResponseResultModel result = new ResponseResultModel(false, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = (HttpStatusCode)403.8; //403.8 - 網站存取遭拒。
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var get = _ruleService.GetRuleFilterBy(category, table, field);
                if (get == null || get.FirstOrDefault() == null)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"流程規則條件已不存在,請重新查詢。({category}-{table}-{field})");
                    result.StatusCode = HttpStatusCode.ExpectationFailed; //417 - 執行失敗
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                res = _ruleService.Delete(get.FirstOrDefault());
                string _str = res.IsSuccess ? "成功" : "失敗"
                    , _s2 = string.Format($"{category} 規則表:{table}")
                    , _s3 = string.Format($"【{field}】 規則條件");
                //
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M013",     //[@USER_ID(@USER_NAME)] 刪除 [@TARGET] 的 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, _s2, _s3, _str),
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
                    LogString = "刪除規則條件.Result"
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
                    Input = new { param = _param, Result = res, exception = ex },
                    LogString = "刪除規則條件.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        /// <summary>
        /// 查詢結果
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(RuleCategoryModel m)
        {
            var _param = m;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);

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
                #endregion

                //規則主資料
                var get = _ruleService.GetRuleBy(m.RuleCategory ?? string.Empty)
                            .Select(s => new RuleListModel().ConvertData(s)).Distinct().ToList();

                //規則條件(篩選)資料
                get.ForEach(a => a.RuleFilters = _ruleService.GetRule(a.RuleCategory, a.RuleTable)
                    .Select(e => new RuleListFilterModel().DataConvert(e))
                    .OrderBy(o => o.RuleCategory).ThenBy(t => t.Priority).ToList());

                result.IsSuccess = true;
                result.Message = "OK";
                result.StatusCode = HttpStatusCode.OK;
                result.Data = get;

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "規則庫資料", "OK"),
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
                result.ErrorException = ex;
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Search]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "規則庫.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #region >>> 頁面下拉選單API
        /// <summary>
        /// 流程可選用的規則資料表
        /// </summary>
        /// <param name="category"> 流程類別 </param>
        /// <returns> List: <see cref="FilterTableModel"/> </returns>
        public JsonResult ProcessTableList(string category)
        {
            var _param = new { category };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);

            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = (HttpStatusCode)403.8; //403.8 - 網站存取遭拒。
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //_規則資料表 List
            var _ruletable = _ruleService.UnspecifyRuleTableColumns(category);
            List<FilterTableModel> filterTables = _ruletable.GroupBy(g => new { g.fsTABLE })
                .Select(s => new FilterTableModel
                {
                    TableName = s.Key.fsTABLE,
                    TableDesc = _ruletable.Where(x => x.fsTABLE == s.Key.fsTABLE).FirstOrDefault().fsTABLE_NAME ?? string.Empty,
                    //Properties = _ruletable.Where(x => x.fsTABLE == s.Key.fsTABLE).Select(f => new FieldInfo(f)).ToList()
                    Properties = _ruletable.Where(x => x.fsTABLE == s.Key.fsTABLE).Select(f => new FieldInfo().DataConvert(f, _tbzCodeService)).ToList()
                }).ToList();

            result.IsSuccess = true;
            result.Message = "OK";
            result.Data = filterTables;
            result.StatusCode = (HttpStatusCode)200;

            #region _Serilog.Verbose
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = CONTR_NAEM,
                Method = "[ProcessTableList]",
                EventLevel = SerilogLevelEnum.Verbose,
                Input = new { Param = _param, Result = filterTables },
                LogString = "流程可選用的規則資料表.Result"
            });
            #endregion
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}