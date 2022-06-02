using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Enums;
using System.Net;
using AIRMAM5.DBEntity.Models.CodeSet;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.Filters;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 系統代碼維護 
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class SysCodeController : BaseController
    {
        readonly string CONTR_NAEM = "SysCode";

        public SysCodeController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService, ICodeService codeService)
            : base(serilogService, functionService)
        {
            _tbzCodeService = codeService;
            _tblLogService = tblLogService;
            _serilogService = serilogService;
        }

        /// <summary>
        /// 系統代碼維護 Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return View("NoAuth");

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統代碼維護 "),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(string.Empty),
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
            return PartialView("_Search");
        }

        /// <summary>
        /// 系統代碼查詢
        /// </summary>
        /// <param name="fsCODE_ID">系統代碼</param>
        /// <param name="fsTITLE">代碼標題</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(string fsCODE_ID, string fsTITLE)
        {
            var _param = new { fsCODE_ID, fsTITLE };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var get = _tbzCodeService.GetCodeMaster(fsCODE_ID, fsTITLE, CodeSetTypeEnum.S.ToString());
            result.IsSuccess = true;
            result.StatusCode = HttpStatusCode.OK;
            result.Data = get;
            result.Message = "OK";

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統代碼", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(_param),
                User.Identity.Name);
            #endregion
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #region ==========【代碼主檔】==========
        /// <summary>
        /// 新增代碼主檔
        /// </summary>
        /// <returns></returns>
        public ActionResult _Create()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "CreateModal" });

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "新增系統代碼-主檔燈箱"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(string.Empty),
                User.Identity.Name);
            #endregion
            return PartialView("_Create", new CodeSetEditModel());
        }

        /// <summary>
        /// 新增代碼主檔 POST
        /// </summary>
        /// <param name="CodeSetEditModel"> modified_BY_20200121 : 資料model改用 CodeSetEditModel()。 </param>
        [HttpPost]
        public ActionResult Create(CodeSetEditModel m)
        {
            var _param = m;
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

                tbzCODE_SET _ins = new tbzCODE_SET
                {
                    fsCODE_ID = m.fsCODE_ID,
                    fsTITLE = m.fsTITLE ?? string.Empty,
                    fsIS_ENABLED = m.IsEnabled ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString(),
                    fsTBCOL = string.Empty,
                    fsNOTE = m.fsNOTE ?? string.Empty,
                    fsTYPE = CodeSetTypeEnum.S.ToString(),
                    fdCREATED_DATE = DateTime.Now,
                    fsCREATED_BY = User.Identity.Name,
                    IsEnabled = m.IsEnabled
                };

                res = _tbzCodeService.CreateCodeSet(_ins);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統代碼", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                };
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Create]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { param = _param, Result = result },
                    LogString = "新增代碼主檔.Result"
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
                    LogString = "新增代碼主檔.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 詳細Modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Details(string id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });

            var get = _tbzCodeService.GetCodeMaster(id)
                    //.Select(s => new CodeSetViewModel(s)).FirstOrDefault();
                    .Select(s => new CodeSetViewModel().ConvertData(s)).FirstOrDefault();

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統代碼主檔", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { CodeID = id }),
                User.Identity.Name);
            #endregion
            return PartialView("_Details", get);
        }

        #region (***定義: 不開放從ui操作修改,刪除系統代碼***)
        /// <summary>
        /// 編輯代碼主檔 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Edit(string id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "EditModal" });

            var get = _tbzCodeService.GetCodeMaster(id).Select(s=> new CodeSetEditModel().ConvertData(s)).FirstOrDefault();

            return PartialView("_Edit", get);
        }

        /// <summary>
        /// 編輯代碼主檔 POST (***定義: 不開放從ui操作修改 系統代碼***)
        /// </summary>
        /// <param name="CodeSetEditModel">
        ///      modified_BY_20200121 : 資料model改用 CodeSetEditModel()。</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(CodeSetEditModel m)//(tbzCODE_SET model)
        {
            var _param = m;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
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

            VerifyResult res = new VerifyResult();
            try
            {
                var _upd = _tbzCodeService.GetCodeSetById(m.fsCODE_ID);
                if (_upd == null)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"代碼資料錯誤，請重新查詢({m.fsCODE_ID})。");
                    result.StatusCode = HttpStatusCode.NotFound;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                _upd.fsIS_ENABLED = m.IsEnabled ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString();
                _upd.fsNOTE = m.fsNOTE ?? string.Empty;
                _upd.fsTITLE = m.fsTITLE ?? string.Empty;
                _upd.fdUPDATED_DATE = DateTime.Now;
                _upd.fsUPDATED_BY = User.Identity.Name;

                res = _tbzCodeService.UpdateCodeSet(_upd);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統代碼主檔", _str),
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
                    Controller = CONTR_NAEM,
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { param = _param, Result = result },
                    LogString = "編輯系統代碼主檔.Result"
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
                    Input = new { param = _param, Result = res, Exception = ex },
                    LogString = "編輯系統代碼主檔.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 刪除代碼主檔Modal  (***定義: 不開放從ui操作刪除 系統代碼***)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Delete(string id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "DeleteModal" });

            //var get = _tbzCodeService.GetCodeMaster(id).FirstOrDefault();
            //get.IsEnabled = get.fsIS_ENABLED == "Y" ? true : false;
            var get = _tbzCodeService.GetCodeMaster(id).Select(s => new CodeSetViewModel().ConvertData(s)).FirstOrDefault();
            
            return PartialView("_Delete", get);
        }

        /// <summary>
        /// 刪除代碼主檔 POST  (***定義: 不開放從ui操作刪除 系統代碼***)
        /// </summary>
        /// <param name="fsCODE_ID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string fsCODE_ID)
        {
            var _param = new { fsCODE_ID };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            #region _檢查_
            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            if (string.IsNullOrEmpty(fsCODE_ID))
            {
                result.IsSuccess = false;
                result.Message = "刪除代碼編號有誤";
                result.StatusCode = HttpStatusCode.BadRequest;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            VerifyResult res = new VerifyResult();
            try
            {
                res = _tbzCodeService.DeleteCodeSet(fsCODE_ID, User.Identity.Name);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M003",     //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統代碼主檔", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(new { fsCODE_ID }), 
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
                    Controller = CONTR_NAEM,
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "刪除系統代碼主檔.Result"
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
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Result = res, Exception = ex },
                    LogString = "刪除系統代碼主檔.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion
        #endregion

        #region ==========【子代碼明細】==========
        /// <summary>
        /// 顯示子檔列表Modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Cog(string id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "CogModal" });
            var get = _tbzCodeService.GetEditById(id);
            /* return Model : CodeEditViewModel()
             {
                CodeSet  : CodeSetEditModel(),      //代碼主檔
                Code     : CodeDataModel(),         //代碼子檔單筆
                CodeList : List<CodeDataModel()>,   //代碼主檔.子檔清單
             }
             */

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統代碼-設定子代碼 燈箱"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(string.Empty),
                User.Identity.Name);
            #endregion
            return PartialView("_Cog", get);
        }

        /// <summary>
        /// 新增 子代碼 POST
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateCode(CodeDataModel det)//(tbzCODE det)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, det);
            VerifyResult res = new VerifyResult();
            try
            {
                #region _檢查欄位
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
                #region _子代碼是否存在
                bool isHad = _tbzCodeService.ChkCodeIsHad(det.fsCODE_ID, det.fsCODE);
                if (isHad)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"代碼{det.fsCODE} 已存在! ");
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                tbzCODE _create = new tbzCODE(det)
                {
                    fsIS_ENABLED = det.IsEnabled ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString(),
                    fsTYPE = Enum.GetName(typeof(CodeSetTypeEnum), CodeSetTypeEnum.S),
                    fsCREATED_BY = User.Identity.Name,
                    fdCREATED_DATE = DateTime.Now
                };

                res = _tbzCodeService.CreateCodeDet(_create);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統代碼子代碼", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(det),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = det,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[CreateCode]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = det, Result = result },
                    LogString = "新增系統代碼子代碼.Result"
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
                    Method = "[CreateCode]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = det, Result = res, Exception = ex },
                    LogString = "新增系統代碼子代碼.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 刪除 子代碼 POST
        /// </summary>
        /// <param name="param"> { codeid, code } 代碼主檔代碼, 子代碼 </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteCode(CodeIdsModel param)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, param);
            VerifyResult res = new VerifyResult();
            try
            {
                #region _檢查參數值
                if (string.IsNullOrEmpty(param.CodeId) || string.IsNullOrEmpty(param.Code))
                {
                    result.IsSuccess = false;
                    result.Message = "請選擇要刪除的子代碼項目";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
                #region _只要樣版有用到此自訂代碼，就不可以刪除子項
                bool isUsed = _tbzCodeService.ChkCodeIsUsedFromTemplateFields(param.CodeId, param.Code);
                if (isUsed)
                {
                    result.IsSuccess = false;
                    result.Message = "樣版有用到此自訂代碼，不可以刪除子項";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
                #region _子代碼是否存在
                bool isHad = _tbzCodeService.ChkCodeIsHad(param.CodeId, param.Code);
                if (!isHad)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"子代碼{param.Code} 已不存在! ");
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                res = _tbzCodeService.DeleteCodeDet(param);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region  _DB LOG [tblLOG]
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M003",     //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統代碼:子代碼", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[EditCode]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = param, Result = result },
                    LogString = "刪除系統代碼子代碼.Result"
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
                    Method = "[DeleteCode]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = param, Result = res, Exception = ex },
                    LogString = "刪除系統代碼子代碼.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 修改編輯 子代碼 POST
        /// </summary>
        /// <param name="det"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditCode(CodeDataModel det)
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, det);
            VerifyResult res = new VerifyResult();

            try
            {
                #region _檢查欄位
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
                #region _子代碼是否存在
                bool isHad = _tbzCodeService.ChkCodeIsHad(det.fsCODE_ID, det.fsCODE);
                if (!isHad)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"子代碼{det.fsCODE} 已不存在! ");
                    result.StatusCode = HttpStatusCode.NotFound;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                var _upd = _tbzCodeService.GetCodeById(det.fsCODE_ID, det.fsCODE).FirstOrDefault();
                _upd.fsNAME = det.fsNAME;
                _upd.fsENAME = det.fsENAME ?? string.Empty;
                _upd.fnORDER = det.fnORDER;
                _upd.fsNOTE = det.fsNOTE ?? string.Empty;
                _upd.fsIS_ENABLED = det.IsEnabled ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString();
                _upd.fsSET = det.fsSET ?? string.Empty;
                _upd.fdUPDATED_DATE = DateTime.Now;
                _upd.fsUPDATED_BY = User.Identity.Name;

                res = _tbzCodeService.UpdateCodeDet(_upd);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統代碼:子代碼", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(det),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = det,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[EditCode]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = det, Result = result },
                    LogString = "編輯系統代碼子代碼.Result"
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
                    Method = "[EditCode]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = det, Result = res, Exception = ex },
                    LogString = "編輯系統代碼子代碼.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        
        #endregion

    }
}
