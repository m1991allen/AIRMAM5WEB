using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.DBEntity.Procedure;
using System.Net;
using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.Common;
using AIRMAM5.Filters;
using AIRMAM5.Utility.Common;
using AIRMAM5.HubServer;
using AIRMAM5.DBEntity.Models.Works;
using AIRMAM5.DBEntity.Models.Subject;
using AIRMAM5.DBEntity.Models.SubjectUpload;
using AIRMAM5.DBEntity.Models.Directory;
using AIRMAM5.DBEntity.Models.Template;
using AIRMAM5.DBEntity.Models.TemplateFields;
using AIRMAM5.DBEntity.Models.ArcPre;
using AIRMAM5.DBEntity.Interface;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 媒資管理 > 主題與檔案維護
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class SubjectController : BaseController
    {
        readonly string CONTR_NAEM = "Subject";
        readonly DirectoriesService _directoriesService;
        readonly ISubjectService _SubjectService;
        readonly ArcVideoService _arcVideoService;
        readonly ArcAudioService _arcAudioService;
        readonly ArcPhotoService _arcPhotoService;
        readonly ArcDocService _arcDocService;
        private TblWorkService _tblWorkService;
        private ConfigService _configService;
        private TemplateService _templateService;
        private ArcPreService _arcPreService;

        /// <summary>
        /// 媒資管理 > 主題與檔案維護
        /// </summary>
        public SubjectController(ISerilogService serilogService, ISubjectService subjectService, ITblLogService tblLogService, ICodeService codeService
            , IFunctionsService functionsService)
            : base(serilogService, functionsService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _tbzCodeService = codeService;
            _SubjectService = subjectService;
            //
            _directoriesService = new DirectoriesService();
            _arcVideoService = new ArcVideoService(serilogService);
            _arcAudioService = new ArcAudioService(serilogService);
            _arcPhotoService = new ArcPhotoService(serilogService);
            _arcDocService = new ArcDocService(serilogService);
            _arcPreService = new ArcPreService(serilogService);
            _tblWorkService = new TblWorkService();
            _configService = new ConfigService();
            _templateService = new TemplateService();
        }

        /// <summary>
        /// 主題與檔案維護
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return View("NoAuth");
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "主題與檔案維護"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 系統目錄樹狀節點 (id=0，Root directory)
        /// <para> Tips: Dir的GetDir會防角色,但Subject這支不會,目前前端統一用這支 </para>
        /// </summary>
        /// <param name="id">系統目錄編號 fnDIR_ID</param>
        /// <param name="fsKEYWORD"></param>
        /// <param name="showcount">是否顯示主題數量, 預設:false 不顯示</param>
        /// <param name="showhide">是否顯示隱藏的dir, 預設:false 不顯示</param>
        /// <returns></returns>
        public JsonResult GetDir(long id, string fsKEYWORD, bool showcount = false, bool showhide = false)
        {
            //取得樹狀
            var param = new GetDirLoadOnDemandSearchModel
            {
                DirId = id,
                KeyWord = fsKEYWORD,
                UserName = User.Identity.Name,
                ShowSubJ = showcount,     //是否顯示主題數量
                IsShowAll = showhide
            };

            var get = _directoriesService.DirSubList(param);

            return Json(get, JsonRequestBehavior.AllowGet);
        }

        #region -----(使用者 VS 節點/主題/檔案編號 的操作權限 method)-----
        /// <summary>
        /// 取得使用者在該節點對於主題/影/音/圖/文的權限
        /// </summary>
        /// <param name="id">Queue節點的 DirId </param>
        /// <param name="type">檔案類型 : S,V,A,P,D </param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetUserDirAuth(long id)//, string type)
        {
            var _param = new { DirId = id };
            UserUseDirAuthModel result = new UserUseDirAuthModel();

            try
            {
                var _get = _directoriesService.GetDirAuthByLoginId(id, User.Identity.Name);
                //Tips: 使用者會有兩筆資料狀況(群組G、使用者U)，要將兩筆權限欄位(LIMIT_SUBJECT,LIMIT_VIDEO,LIMIT_AUDIO,LIMIT_PHOTO,LIMIT_DOC)做聯集
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[GetUserDirAuth]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = _get },
                    LogString = "使用者目錄檔案權限.Result"
                });
                #endregion

                result = (_get.Count() == 0 || _get.FirstOrDefault() == null) ? result : _get.FirstOrDefault();
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[GetUserDirAuth]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "使用者目錄檔案權限.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  使用者在主題編號 {主題/影/音/圖/文}的操作權限
        /// </summary>
        /// <param name="id">主題編號 fsSUBJECT_ID </param>
        /// <param name="type">檔案類型 S,V,A,P,D </param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetUserSubjAuth(string id)//, string type)
        {
            var _param = new { SubjId = id };
            UserUseDirAuthModel result = new UserUseDirAuthModel();

            try
            {
                var _get = _directoriesService.GetSubjectAuthByLoginId(id, User.Identity.Name);
                //Tips: 使用者會有兩筆資料狀況(群組G、使用者U)，要將兩筆權限欄位(LIMIT_SUBJECT,LIMIT_VIDEO,LIMIT_AUDIO,LIMIT_PHOTO,LIMIT_DOC)做聯集
                //Marked
                //var result = new UserUseSubjFileAuthModel(_get, type)
                //{
                //    SubjectId = id,
                //    LoginId = User.Identity.Name,
                //    FileCategory = type
                //};
                //result = result;
                result = (_get.Count() == 0 || _get.FirstOrDefault() == null) ? result : _get.FirstOrDefault();

                #region //多筆(不建議使用)
                //List<UserUseSubjFileAuthModel> result = new List<UserUseSubjFileAuthModel>();
                //foreach (var id in ids )
                //{
                //    var _get = _directoriesService.GetSubjectAuthByLoginId(id, User.Identity.Name);
                //    result.Add(new UserUseSubjFileAuthModel(_get, type)
                //    {
                //        SubjectId = id,
                //        LoginId = User.Identity.Name
                //    });
                //}
                #endregion
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[GetUserSubjAuth]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = "使用者主題權限.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[GetUserSubjAuth]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "使用者主題權限.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  使用者在檔案編號 {主題/影/音/圖/文}的操作權限
        /// </summary>
        /// <param name="id">檔案編號 fsFILE_ID </param>
        /// <param name="type">檔案類型 S,V,A,P,D </param>
        /// <returns></returns>
        public JsonResult GetUserFileAuth(string id)//, string type)
        {
            var _param = new { FileNo = id };
            UserUseDirAuthModel result = new UserUseDirAuthModel();

            try
            {
                var _get = _directoriesService.GetFilenoAuthByLoginId(id, User.Identity.Name);
                //Tips: 會有兩筆資料狀況(群組G、使用者U)，要將兩筆權限欄位(LIMIT_SUBJECT,LIMIT_VIDEO,LIMIT_AUDIO,LIMIT_PHOTO,LIMIT_DOC)做聯集
                //Marked
                //var result = new UserUseSubjFileAuthModel(_get, type)
                //{
                //    FileNo = id,
                //    LoginId = User.Identity.Name,
                //    FileCategory = type
                //};
                result = (_get.Count() == 0 || _get.FirstOrDefault() == null) ? result : _get.FirstOrDefault();

                #region//多筆(不建議使用)
                //List<UserUseSubjFileAuthModel> result = new List<UserUseSubjFileAuthModel>();
                //foreach (var id in ids)
                //{
                //    var _get = _directoriesService.GetFilenoAuthByLoginId(id, User.Identity.Name);
                //    result.Add(new UserUseSubjFileAuthModel(_get, type)
                //    {
                //        FileNo = id,
                //        LoginId = User.Identity.Name
                //    });
                //}
                #endregion
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[GetUserFileAuth]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = "使用者檔案權限.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[GetUserFileAuth]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "使用者檔案權限.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ----------------------((系統目錄節點-操作: 新/修/刪/檢視))----------------------
        /// <summary>
        /// (系統目錄節點)主題列表
        /// </summary>
        /// <param name="id">Queue節點的Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(long id)
        {
            var _param = new { id };
            var get = _directoriesService.GetDirSubjectsById(id)
                .Select(s => new SubjectListViewModel().FormatConvert(s))
                .ToList();

            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param)
            {
                Data = get,
                StatusCode = HttpStatusCode.OK
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// (系統目錄節點)新增主題
        /// </summary>
        /// <param name="id"> 系統目錄Id [fnDIR_ID] </param>
        /// <returns></returns>
        public ActionResult _Create(long id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "CreateSubjectModal" });
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "主題與檔案維護-新增"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion
            //
            var _dir = new DirectoriesService().GetDirectoriesById(id);
            //取 目錄主題樣板的預編詮釋資料-選單LISt (註: 樣板ID無 預編詮釋資料,就無選單.)
            //var arcpreList = new ArcPreService().GetArcPreListBy(FileTypeEnum.S.ToString(), _dir.fnTEMP_ID_SUBJECT);
            //arcpreList.Insert(0, new SelectListItem { Value = "0", Text = "無", Selected = true });
            var arcpreList = _arcPreService.GetByParam(new spGET_ARC_PRE_Param
            {
                PreType = FileTypeEnum.S.ToString(),
                PreTempId = _dir.fnTEMP_ID_SUBJECT
            });
            arcpreList.Insert(0, new spGET_ARC_PRE_Result { });

            //取回目錄-主題樣版動態欄位
            //List<ArcPreAttributeModel> attrFields = new List<ArcPreAttributeModel>();
            var _getFields = _templateService.GetTemplateFieldsById(_dir.fnTEMP_ID_SUBJECT)
                .Select(s => new TemplateFieldsModel().DataConvert(s)).ToList();

            SubjectCreateModel model = new SubjectCreateModel
            {
                DirId = id,
                TemplateId = _dir.fnTEMP_ID_SUBJECT,
                ArcPreList = arcpreList,
                ArcPreAttributes = _getFields
            };

            return PartialView("_Create", model);
        }

        /// <summary>
        /// (系統目錄節點)新增主題 POST 
        /// </summary>
        /// <param name="form">表單內容 </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            var _param = form;
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
                if (string.IsNullOrEmpty(form["Title"].ToString()))
                {
                    result.IsSuccess = false;
                    result.Message = "\"標題\" 為必填欄位。";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (string.IsNullOrEmpty(form["DateInSubjId"].ToString()))
                {   //20201211_前端增加傳入:主題編號日期
                    result.IsSuccess = false;
                    result.Message = "\"主題編號日期\" 為必填欄位。";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                bool _trydir = int.TryParse(form["DirId"], out int _dirid)
                    , _trytemp = int.TryParse(form["TemplateId"], out int _tempid)
                    , _trypre = int.TryParse(form["SubjectPreId"], out int _preid);
                if (!(_trydir && _trytemp && _trypre))
                {
                    result.IsSuccess = false;
                    result.Message = "目錄節點/樣板ID錯誤!";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                tbmSUBJECT _newRow = new tbmSUBJECT(_dirid)
                {
                    //fnDIR_ID = _dirid,
                    fsTITLE = form["Title"].ToString(),
                    fsDESCRIPTION = form["Description"].ToString(),
                    fdCREATED_DATE = DateTime.Now,
                    fsCREATED_BY = User.Identity.Name
                };

                //選擇:預編詮釋資料 檢查
                var _dir = new DirectoriesService().GetDirectoriesById(_dirid);
                var _arcpre = _arcPreService.GetById(_preid);
                if (_preid > 0 && _arcpre != null)
                {
                    #region 【判斷此預編用的樣板是否與此主題樣板相同】
                    if (_dir.fnTEMP_ID_SUBJECT != _arcpre.fnTEMP_ID)
                    {
                        result.IsSuccess = false;
                        result.Message = "此主題需要的樣板與預編詮釋資料的樣板不符!";
                        result.StatusCode = HttpStatusCode.BadRequest;
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                    #endregion

                    //使用預編欄位資料
                    _newRow = new tbmSUBJECT(_arcpre)
                    {
                        fnDIR_ID = _dirid,
                        fsTITLE = form["Title"].ToString(),
                        fsDESCRIPTION = form["Description"].ToString(),
                        fdCREATED_DATE = DateTime.Now,
                        fsCREATED_BY = User.Identity.Name
                    };
                }
                else
                {
                    // 樣板自訂欄位資料(依欄位屬性給值).SetValue 
                    //Tips_20200409:增加參數判斷批次修改"接口名_IsEdit=on",預設=false
                    _templateService.AttriFieldsSetValue<tbmSUBJECT>(_tempid, _newRow, form);
                }
                
                string subjidHead = form["DateInSubjId"].ToString(); //20201211_前端增加傳入:主題編號日期
                DateTime.TryParse(subjidHead, out DateTime dt);
                subjidHead = string.Format($"{dt:yyyyMMdd}");
                res = _SubjectService.CreateBy(_newRow, subjidHead);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M001",        //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "主題", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Create]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = "新增主題.Result"
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
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "新增主題.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 主題詳細頁
        /// </summary>
        /// <param name="id">主題檔案編號 fsSUBJECT </param>
        /// <returns></returns>
        public ActionResult _Details(string id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailtModal" });
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M004",        //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "主題", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { SubjectId = id }),
                User.Identity.Name);
            #endregion

            var getSubject = _SubjectService.GetBy(id);
            SubjectViewModel subject = new SubjectViewModel
            {
                SubjectId = getSubject.fsSUBJ_ID,
                Title = getSubject.fsTITLE,
                Description = getSubject.fsDESCRIPTION
            };

            #region 主題-樣版動態欄位:屬性設定值
            var _getAttrs = _SubjectService.GetSubjAttribute(id);
            _getAttrs.ForEach(f =>
            {
                ArcPreAttributeModel _arrtF = new ArcPreAttributeModel().FormatConversion(f, FileTypeEnum.S);
                subject.SubjectAttributes.Add(_arrtF);
            });
            #endregion

            return PartialView("_Details", subject);
        }

        /// <summary>
        /// 主題編輯頁
        /// </summary>
        /// <param name="id">主題檔案編號 fsSUBJ_ID  </param>
        /// <returns></returns>
        public ActionResult _Edit(string id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "EditModal" });

            var getSubject = _SubjectService.GetBy(id);
            SubjectViewModel subject = new SubjectViewModel
            {
                SubjectId = getSubject.fsSUBJ_ID,
                Title = getSubject.fsTITLE,
                Description = getSubject.fsDESCRIPTION
            };

            #region 主題-樣版動態欄位數:屬性設定值
            var _getAttrs = _SubjectService.GetSubjAttribute(id);

            _getAttrs.ForEach(f =>
            {
                ArcPreAttributeModel _arrtF = new ArcPreAttributeModel().FormatConversion(f, FileTypeEnum.S);
                subject.SubjectAttributes.Add(_arrtF);
            });
            #endregion

            return PartialView("_Edit", subject);
        }

        /// <summary>
        /// 刪除主題檔案 Model View
        /// </summary>
        /// <param name="subjid"></param>
        /// <returns></returns>
        public ActionResult _Delete(string subjid)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "DeleteModal" });
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "主題-刪除檢視"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { SubjectId = subjid }),
                User.Identity.Name);
            #endregion

            var getSubject = _SubjectService.GetBy(subjid);
            SubjectViewModel subject = new SubjectViewModel
            {
                SubjectId = getSubject.fsSUBJ_ID,
                Title = getSubject.fsTITLE,
                Description = getSubject.fsDESCRIPTION
            };

            #region 主題-樣版動態欄位數:屬性設定值
            var _getAttrs = _SubjectService.GetSubjAttribute(subjid);

            _getAttrs.ForEach(f =>
            {
                ArcPreAttributeModel _arrtF = new ArcPreAttributeModel().FormatConversion(f, FileTypeEnum.S);
                subject.SubjectAttributes.Add(_arrtF);
            });
            #endregion

            return PartialView("_Delete", subject);
        }

        /// <summary>
        /// 主題編輯頁 POST
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            var _param = form;
            //var _param = new
            //{
            //    SubjectId = form["SubjectId"].ToString(),
            //    Title = form["Title"].ToString(),
            //    Description = form["Description"].ToString()
            //};
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                res = _SubjectService.UpdateBy(form, User.Identity.Name);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",        //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "主題", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog .Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Edit]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "編輯主題.Result"
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
                    LogString = "編輯主題.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 刪除主題檔案POST
        /// </summary>
        /// <param name="subjid">主題檔案編號 fsSUBJ_ID </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string subjid)
        {
            var _param = new { subjid };
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
                if (string.IsNullOrEmpty(subjid))
                {
                    result.IsSuccess = false;
                    result.Message = "主題編號有誤";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
                res = _SubjectService.DeleteBy(subjid, User.Identity.Name);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M003",        //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "主題", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = "刪除主題.Result"
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
                    LogString = "刪除主題.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion ----------------------((系統目錄節點-操作))----------------------

        #region ----------------------((影音圖文-內頁顯示))----------------------
        ///<summary>
        /// 內頁: 預覽(影音圖文)區塊
        /// </summary>
        /// <param name="fsSUBJECT_ID">主題編號</param>
        /// <param name="type">媒體的樣板類型: V, A, P, D, S </param>
        /// <param name="fileNo">預覽檔案編號 : 檔案編號如果給空值,依照類型回傳預設圖 </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _Preview(string fsSUBJECT_ID, string type, string fileNo = "")
        {
            var _param = new { fsSUBJECT_ID, type, fileNo };
            ShowViewerModel rtnmodel = new ShowViewerModel();

            try
            {
                FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), type);
                string _typestr = GetEnums.GetDescriptionText<FileTypeEnum>(type);
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M014",     //[@USER_ID(@USER_NAME)] 檢視 [@TARGET] 的 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, "主題檔案", _typestr, "OK"),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                //顯示list第一筆(影音圖文)檔案
                string _fileUrl = string.Empty, _imgUrl = string.Empty, _subjPath = string.Empty, _filePath = string.Empty;

                ShowViewerModel vw = new ShowViewerModel();
                switch (mediatype)
                {
                    case FileTypeEnum.V:
                        var getvideo = (string.IsNullOrEmpty(fileNo)) ? _arcVideoService.GetVideioBySubjectId(fsSUBJECT_ID) : _arcVideoService.GetVideioBySubjectId(fsSUBJECT_ID, fileNo);
                        var _videio = getvideo.Select(s => vw.FormatConversion(s, mediatype)).FirstOrDefault();
                        rtnmodel = _videio;
                        break;
                    case FileTypeEnum.A:
                        var getaudio = (string.IsNullOrEmpty(fileNo)) ? _arcAudioService.GetArcAudioBySubjectId(fsSUBJECT_ID) : _arcAudioService.GetArcAudioBySubjectId(fsSUBJECT_ID, fileNo);
                        var _audio = getaudio.Select(s => vw.FormatConversion(s, mediatype)).FirstOrDefault();
                        rtnmodel = _audio;
                        break;
                    case FileTypeEnum.P:
                        var getphoto = (string.IsNullOrEmpty(fileNo)) ? _arcPhotoService.GetArcPhotoBySubjectId(fsSUBJECT_ID) : _arcPhotoService.GetArcPhotoBySubjectId(fsSUBJECT_ID, fileNo);
                        var _photo = getphoto.Select(s => vw.FormatConversion(s, mediatype)).FirstOrDefault();
                        rtnmodel = _photo;
                        break;
                    case FileTypeEnum.D:
                        var getdoc = (string.IsNullOrEmpty(fileNo)) ? _arcDocService.GetArcDocBySubjectId(fsSUBJECT_ID) : _arcDocService.GetArcDocBySubjectId(fsSUBJECT_ID, fileNo);
                        var _doc = getdoc.Select(s => vw.FormatConversion(s, mediatype)).FirstOrDefault();
                        rtnmodel = _doc;
                        break;
                    default:
                        //
                        break;
                }

                //Marked_20201026:直接通知前端,檔案是否可以預覽(open DocViewer)
                ////(DocViewer)可預覽的檔案類型 (多種類型以分號; 為分隔符號, EX: doc;docx;txt;html;)
                //var _Previewable = _configService.GetConfigBy("DV_VIEW_FILETYPE").FirstOrDefault();
                //rtnmodel.PreviewableFileType = _Previewable == null ? string.Empty : _Previewable.fsVALUE;
                rtnmodel.fsSUBJECT_ID = fsSUBJECT_ID;
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[_Preview]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "預覽主題當案.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return PartialView("_Preview", rtnmodel);
        }

        ///<summary>
        /// 媒體列表
        /// </summary>
        /// <param name="fsSUBJECT_ID">主題編號</param>
        /// <param name="type">媒體的樣板類型: V, A, P, D, S</param>
        /// <param name="fileNo">檔案編號</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _List(string fsSUBJECT_ID, string type, string fileNo = "")
        {
            var _param = new { fsSUBJECT_ID, type };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);

            try
            {
                if (!CheckUserAuth(CONTR_NAEM)) return View("NoAuth");
                FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), type);

                string _fileUrl = string.Empty, _imgUrl = string.Empty, _subjPath = string.Empty, _filePath = string.Empty;
                ShowListsModel smd = new ShowListsModel() { fsSUBJECT_ID = fsSUBJECT_ID };
                switch (mediatype)
                {
                    case FileTypeEnum.V:
                        var _video = _arcVideoService.GetVideioBySubjectId(fsSUBJECT_ID).Select(s => smd.FormatConversion(s, mediatype));
                        result.Data = _video;
                        break;
                    case FileTypeEnum.A:
                        var _audio = _arcAudioService.GetArcAudioBySubjectId(fsSUBJECT_ID).Select(s => smd.FormatConversion(s, mediatype));
                        result.Data = _audio;
                        break;
                    case FileTypeEnum.P:
                        var _photo = _arcPhotoService.GetArcPhotoBySubjectId(fsSUBJECT_ID).Select(s => smd.FormatConversion(s, mediatype));
                        result.Data = _photo;
                        break;
                    case FileTypeEnum.D:
                        var _doc = _arcDocService.GetArcDocBySubjectId(fsSUBJECT_ID).Select(s => smd.FormatConversion(s, mediatype));
                        result.Data = _doc;
                        break;
                    default:
                        //
                        break;
                }

                result.StatusCode = HttpStatusCode.OK;
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
                    Method = "[_List]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "主題檔案列表.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        ///<summary>
        /// 媒體資料 Metadata(+變動欄位)
        /// </summary>
        /// <param name="fsSUBJECT_ID">主題編號</param>
        /// <param name="type">媒體的樣板類型</param>
        /// <param name="fileno">檔案編號(選填) 空值將會顯示第一筆fileno的metadat</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _MetaData(string fsSUBJECT_ID, string type, string fileno = "")
        {
            var _param = new { fsSUBJECT_ID, type, fileno };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);

            try
            {
                if (!CheckUserAuth(CONTR_NAEM)) return View("NoAuth");
                FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), type);

                #region Added_媒體類別.基本資料內容 參考「檢索結果頁」
                SubjectFileMetaViewModel metaModel = new SubjectFileMetaViewModel(); //續承使用「檢索結果.基本資料頁model」//ArcBasicMetaModel();
                List<ArcPreAttributeModel> _customField = new List<ArcPreAttributeModel>();

                switch (mediatype)
                {
                    case FileTypeEnum.V:
                        metaModel = _arcVideoService.GetVideoBySubjectFile(fsSUBJECT_ID, fileno)
                                    .Select(s => metaModel.FormatConversion(s, mediatype)).FirstOrDefault();

                        /* 顯示基本欄位、自訂欄位 內容值, 自訂欄位值需叫用預存() */
                        fileno = string.IsNullOrEmpty(fileno) ? metaModel.fsFILE_NO : fileno;

                        metaModel.ArcPreAttributes = _arcVideoService.GetVideoAttrByFile(string.IsNullOrEmpty(fileno) ? metaModel.fsFILE_NO : fileno)
                                        .Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();
                        break;
                    case FileTypeEnum.A:
                        metaModel = _arcAudioService.GetArcAudioByIdFile(fsSUBJECT_ID, fileno)
                                    .Select(s => metaModel.FormatConversion(s, mediatype)).FirstOrDefault();

                        /* 顯示基本欄位、自訂欄位 內容值, 自訂欄位值需叫用預存() */
                        fileno = string.IsNullOrEmpty(fileno) ? metaModel.fsFILE_NO : fileno;

                        _customField = _arcAudioService.GetAudioAttrByFile(string.IsNullOrEmpty(fileno) ? metaModel.fsFILE_NO : fileno)
                                        .Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();

                        #region---/*  聲音資訊+專輯資訊_addedBy_20191231 */
                        var _a = _arcAudioService.GetArcAudioByIdFile(fsSUBJECT_ID, fileno).FirstOrDefault();
                        if (_a != null)
                        {
                            _customField.Add(new ArcPreAttributeModel("fsALBUM", (_a.fsALBUM ?? string.Empty), "專輯名稱"));
                            _customField.Add(new ArcPreAttributeModel("fsALBUM_TITLE", (_a.fsALBUM_TITLE ?? string.Empty), "專輯標題"));
                            _customField.Add(new ArcPreAttributeModel("fsALBUM_ARTISTS", (_a.fsALBUM_ARTISTS ?? string.Empty), "專輯演出者"));
                            _customField.Add(new ArcPreAttributeModel("fsALBUM_PERFORMERS", (_a.fsALBUM_PERFORMERS ?? string.Empty), "歌曲演出者"));
                            _customField.Add(new ArcPreAttributeModel("fsALBUM_COMPOSERS", (_a.fsALBUM_COMPOSERS ?? string.Empty), "歌曲作曲者"));
                            _customField.Add(new ArcPreAttributeModel("fnALBUM_YEAR", (_a.fnALBUM_YEAR.ToString()), "專輯年份"));
                            _customField.Add(new ArcPreAttributeModel("fsALBUM_COPYRIGHT", (_a.fsALBUM_COPYRIGHT ?? string.Empty), "著作權"));
                            _customField.Add(new ArcPreAttributeModel("fsALBUM_GENRES", (_a.fsALBUM_GENRES ?? string.Empty), "內容類型"));
                            _customField.Add(new ArcPreAttributeModel("fsALBUM_COMMENT", (_a.fsALBUM_COMMENT ?? string.Empty), "備註"));
                        }
                        #endregion
                        metaModel.ArcPreAttributes = _customField;
                        break;
                    case FileTypeEnum.P:
                        metaModel = _arcPhotoService.GetArcPhotoByIdFile(fsSUBJECT_ID, fileno)
                                    .Select(s => metaModel.FormatConversion(s, mediatype)).FirstOrDefault();

                        /* 顯示基本欄位、自訂欄位 內容值, 自訂欄位值需叫用預存() */
                        fileno = string.IsNullOrEmpty(fileno) ? metaModel.fsFILE_NO : fileno;

                        _customField = _arcPhotoService.GetPhotoAttrByFile(string.IsNullOrEmpty(fileno) ? metaModel.fsFILE_NO : fileno)
                                        .Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();

                        #region---/* 圖片資訊+EXIF資訊_addedBy_20191231 */
                        var _p = _arcPhotoService.GetPhotoExif(fileno);
                        if (_p != null)
                        {
                            _customField.Add(new ArcPreAttributeModel("fnWIDTH", _p.PhotoWidth.ToString(), "圖片寬"));
                            _customField.Add(new ArcPreAttributeModel("fnHEIGHT", _p.PhotoHeight.ToString(), "圖片高"));
                            _customField.Add(new ArcPreAttributeModel("fnXDPI", _p.PhotoXdpi.ToString(), "XDPI"));
                            _customField.Add(new ArcPreAttributeModel("fnYDPI", _p.PhotoYdpi.ToString(), "YDPI"));
                            _customField.Add(new ArcPreAttributeModel("fsCAMERA_MAKE", _p.CameraMake ?? string.Empty, "相機廠牌"));
                            _customField.Add(new ArcPreAttributeModel("fsCAMERA_MODEL", _p.CameraModel ?? string.Empty, "相機型號"));
                            _customField.Add(new ArcPreAttributeModel("fsFOCAL_LENGTH", _p.FocalLength ?? string.Empty, "焦距"));
                            _customField.Add(new ArcPreAttributeModel("fsEXPOSURE_TIME", _p.ExposureTime ?? string.Empty, "曝光時間"));
                            _customField.Add(new ArcPreAttributeModel("fsAPERTURE", _p.Aperture ?? string.Empty, "光圈"));
                            _customField.Add(new ArcPreAttributeModel("fnISO", _p.ISO.ToString(), "ISO"));
                        }
                        #endregion
                        metaModel.ArcPreAttributes = _customField;
                        break;
                    case FileTypeEnum.D:
                        metaModel = _arcDocService.GetArcDocByIdFile(fsSUBJECT_ID, fileno)
                                    .Select(s => metaModel.FormatConversion(s, mediatype)).FirstOrDefault();

                        /* 顯示基本欄位、自訂欄位 內容值, 自訂欄位值需叫用預存() */
                        fileno = string.IsNullOrEmpty(fileno) ? metaModel.fsFILE_NO : fileno;

                        _customField = _arcDocService.GetDocAttrByFile(string.IsNullOrEmpty(fileno) ? metaModel.fsFILE_NO : fileno)
                                        .Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();

                        #region---/* 文件額外資訊_addedBy_20191231 */
                        var _d = _arcDocService.GetDocumentInfo(fileno);
                        if (_d != null)
                        {
                            _customField.Add(new ArcPreAttributeModel("Content", _d.Content ?? string.Empty, "文件內容"));
                            _customField.Add(new ArcPreAttributeModel("FileCreatedDate", string.Format($"{_d.FileCreatedDate:yyyy-MM-dd HH:mm:ss}"), "文件建立日期"));
                            _customField.Add(new ArcPreAttributeModel("FileUpdatedDate", string.Format($"{_d.FileUpdatedDate:yyyy-MM-dd HH:mm:ss}"), "文件修改日期"));
                        }
                        #endregion
                        metaModel.ArcPreAttributes = _customField;
                        break;
                    default:
                        //
                        break;
                }
                #endregion

                result.StatusCode = HttpStatusCode.OK;
                result.Data = metaModel;
                result.Message = string.Empty;
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
                    Method = "[_MetaData]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "主題檔案資料.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 新增關鍵影格的資料頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult _KeyFrameAdd()
        {
            return PartialView();
        }

        /// <summary>
        /// 關鍵影格編輯頁面
        /// </summary>
        /// <param name="fileno"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public ActionResult _KeyFrameEdit(string fileno, string time)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return View("NoAuth");
            var _param = new { fileno };
            VideoKeyFrameModel model = new VideoKeyFrameModel();

            try
            {
                //VideoKeyFrameModel<spGET_ARC_VIDEO_K_Result> _vkf = new VideoKeyFrameModel<spGET_ARC_VIDEO_K_Result>();
                //model = _arcVideoService.GetKeyFrame(fileno, time).Select(s => _vkf.FormatConvert(s, FileTypeEnum.V)).FirstOrDefault();
                model = _arcVideoService.GetKeyFrame(fileno, time)
                            .Select(s => new VideoKeyFrameModel().FormatConvert(s, FileTypeEnum.V)).FirstOrDefault();

                // 轉成時間格式 00:00:00;000
                //model.Time = CommonTimeCode.FarmeToTimeCode((double.Parse(model.Time) / 1000) * 29.97);
                model.Time = CommonTimeCode.CurrentTimesToTimeCode((double.Parse(model.Time) / 1000));
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[_KeyFrame]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "關鍵影格編輯頁.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return PartialView(model);

        }
        /// <summary>
        /// 關鍵影格刪除頁面
        /// </summary>
        /// <param name="fileno"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public ActionResult _KeyFrameDelete(string fileno, string time)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return View("NoAuth");
            var _param = new { fileno };
            VideoKeyFrameModel model = new VideoKeyFrameModel();

            try
            {
                model = _arcVideoService.GetKeyFrame(fileno, time)
                            .Select(s => new VideoKeyFrameModel().FormatConvert(s, FileTypeEnum.V))
                            .FirstOrDefault();

                // 轉成時間格式 00:00:00;000
                //model.Time = CommonTimeCode.FarmeToTimeCode((double.Parse(model.Time) / 1000) * 29.97);
                model.Time = CommonTimeCode.CurrentTimesToTimeCode((double.Parse(model.Time) / 1000));
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[_KeyFrame]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "關鍵影格刪除頁.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return PartialView(model);
        }

        ///<summary>
        /// 關鍵影格 KeyFarem (只有影片 才有
        /// </summary>
        /// <param name="fileno">檔案編號fsFILE_NO </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _KeyFrame(string fileno)
        {
            var _param = new { fileno };
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

                var _videio = _arcVideoService.GetKeyFrame(fileno)
                                .Select(s => new VideoKeyFrameModel().FormatConvert(s, FileTypeEnum.V));

                result.Data = _videio;
                result.StatusCode = HttpStatusCode.OK;
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
                    Method = "[_KeyFrame]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "關鍵影格.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        ///<summary>
        ///段落描述 Seqment (只有影、音 才有
        /// </summary>
        /// <param name="type">媒體的樣板類型 fsTYPE </param>
        /// <param name="fileno">檔案編號fsFILE_NO </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _ParaDescription(string type, string fileno)
        {
            var _param = new { type, fileno };
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

                FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), type);
                switch (mediatype)
                {
                    case FileTypeEnum.V:
                        result.Data = _arcVideoService.GetVideoSeqment(fileno)
                                        .Select(s => new SubjectFileSeqmentModel().FormatConvert(s, mediatype));
                        break;
                    case FileTypeEnum.A:
                        result.Data = _arcAudioService.GetAudioSeqment(fileno)
                                        .Select(s => new SubjectFileSeqmentModel().FormatConvert(s, mediatype));
                        break;
                    default:
                        //
                        break;
                }

                result.StatusCode = HttpStatusCode.OK;
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
                    Method = "[_ParaDescription]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "段落描述.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion ----------------------((影音圖文-內頁))----------------------

        #region ----------------------((系統目錄節點-操作: 上傳檔案))----------------------
        /// <summary>
        /// 上傳 View 
        /// </summary>
        /// <param name="id">目錄節點Id [fnDIR_ID]</param>
        /// <param name="subjid">主題編號 [fsSUBJ_ID]</param>
        /// <returns></returns>
        public ActionResult _Upload(int id, string subjid)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "UploadModal" });
            var param = new { id, subjid };

            #region >>>>> 依使用者的目錄檔案操作權限 決定上傳檔案選單的項目
            //上傳檔案類型: UPLOAD_MEDIATYPE = MEDIATYPE_TO_V, MEDIATYPE_TO_A, MEDIATYPE_TO_P, MEDIATYPE_TO_D
            var _code = _tbzCodeService.GetCodeDetail(TbzCodeIdEnum.UPLOAD_MEDIATYPE.ToString());
            List<SelectListItem> mediaType = new List<SelectListItem>();
            var _dirAuth = _directoriesService.GetDirAuthByLoginId(id, User.Identity.Name);

            if (_dirAuth.FirstOrDefault().LimitVideo.IndexOf("I") >= 0)
            {
                mediaType.Add(
                    _code.Where(z => z.fsCODE.IndexOf("_TO_V") >= 0).Select(s => new SelectListItem
                    {
                        Value = s.fsCODE,
                        Text = s.fsNAME
                    }).FirstOrDefault());
            }
            if (_dirAuth.FirstOrDefault().LimitAudio.IndexOf("I") >= 0)
            {
                mediaType.Add(
                    _code.Where(z => z.fsCODE.IndexOf("_TO_A") >= 0).Select(s => new SelectListItem
                    {
                        Value = s.fsCODE,
                        Text = s.fsNAME
                    }).FirstOrDefault());
            }
            if (_dirAuth.FirstOrDefault().LimitPhoto.IndexOf("I") >= 0)
            {
                mediaType.Add(
                    _code.Where(z => z.fsCODE.IndexOf("_TO_P") >= 0).Select(s => new SelectListItem
                    {
                        Value = s.fsCODE,
                        Text = s.fsNAME
                    }).FirstOrDefault());
            }
            if (_dirAuth.FirstOrDefault().LimitDoc.IndexOf("I") >= 0)
            {
                mediaType.Add(
                    _code.Where(z => z.fsCODE.IndexOf("_TO_D") >= 0).Select(s => new SelectListItem
                    {
                        Value = s.fsCODE,
                        Text = s.fsNAME
                    }).FirstOrDefault());
            }
            mediaType.Insert(0, new SelectListItem { Value = "", Text = "(未選擇)" });
            #endregion

            /*
             * Tips_20200217:(1)取得目錄媒資的自訂欄位樣版編號[fnTEMP_ID], 
             *               (2)再依自訂欄位樣版編號[fnTEMP_ID] 取得預編詮釋資料 清單List
             * */
            var _dirTemplate = _templateService.GetDirTemplateById(id).ToList()
                .Select(s => new TemplateBaseModel().DataConvert(s)).ToList();
                //{ 
                //    fnTEMP_ID = s.fnTEMP_ID,
                //    fsNAME = s.C_TEMPNAME,
                //    fsTABLE = s.fsTABLE,
                //    IsSearch = s.fcIS_SEARCH.ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false,
                //    SearchType = s.fsTABLE == FileTypeEnum.V.ToString() ? SearchTypeEnum.Video_DEV.ToString()
                //            : s.fsTABLE == FileTypeEnum.A.ToString() ? SearchTypeEnum.Audio_DEV.ToString()
                //                : s.fsTABLE == FileTypeEnum.P.ToString() ? SearchTypeEnum.Photo_DEV.ToString()
                //                    : s.fsTABLE == FileTypeEnum.D.ToString() ? SearchTypeEnum.Doc_DEV.ToString()
                //                        : s.fsTABLE == FileTypeEnum.S.ToString() ? SearchTypeEnum.Subject_DEV.ToString()
                //                            : SearchTypeEnum.Video_DEV.ToString()
                //}).ToList();

            /*
             * Tips: 取 目錄主題樣板的預編詮釋資料-選單LISt (Tips: 無樣板ID 預編詮釋資料,就無選單.)
             * Tips: 前端依上傳媒體分類(影音圖文) 動態取用清單內容 Shared/GetCodeItemList
             * 
             * */
            var arcpreList = _arcPreService.GetArcPreListBy(FileTypeEnum.V.ToString(), _dirTemplate.Find(z => z.fsTABLE == FileTypeEnum.V.ToString()).fnTEMP_ID);
            arcpreList.Insert(0, new SelectListItem { Value = "0", Text = "無", Selected = true });

            //媒體類型副檔名 List
            List<FileExtensionViewModel> fileExtension = new List<FileExtensionViewModel>();
            foreach (var f in mediaType)
            {
                var get = _configService.GetConfigBy(f.Value).FirstOrDefault();
                if (get == null) continue;
                string val = get == null ? string.Empty : get.fsVALUE;
                fileExtension.Add(new FileExtensionViewModel
                {
                    MediaType = f.Value,
                    FileExtension = val
                });
            }

            #region 檔案上傳元件使用的參數資訊_&_更新UPLOAD_ASHX_CURRENT_SWITCH
            var ashxUrls = _configService.GetConfigBy("UPLOAD_ASHX").FirstOrDefault().fsVALUE.Split(new char[] { ';' });// ;為分隔符號
            var currentSwitch = _configService.GetConfigBy("UPLOAD_ASHX_CURRENT_SWITCH").FirstOrDefault();
            int.TryParse(currentSwitch.fsVALUE, out int _switch);//目前要傳至那一個ASHX
            int.TryParse(_configService.GetConfigBy("UPLOAD_ASHX_MAX_COUNT").FirstOrDefault().fsVALUE, out int _maxCount);
            int.TryParse(_configService.GetConfigBy("UPLOAD_FILE_BUFFER").FirstOrDefault().fsVALUE, out int _fileBuffer);
            int.TryParse(_configService.GetConfigBy("UPLOAD_SIMULTANEOUS_FILEs").FirstOrDefault().fsVALUE, out int _simultaneous);

            //更新UPLOAD_ASHX_CURRENT_SWITCH
            _switch = (_switch + 1 > _maxCount) ? 1 : (_switch += 1);
            tbzCONFIG upd = new tbzCONFIG(currentSwitch)
            {
                fsVALUE = _switch.ToString(),
                fsUPDATED_BY = User.Identity.Name
            };
            var res = _configService.UpdateBy(upd);
            string _str = res.IsSuccess ? "成功" : "失敗";
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M021",        //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "上傳(更新UPLOAD_ASHX_CURRENT_SWITCH) ", _str),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(upd),
                User.Identity.Name);
            #endregion
            #region _Serilog.Info
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = CONTR_NAEM,
                Method = "[_Upload]",
                EventLevel = SerilogLevelEnum.Information,
                Input = new { Param = upd, Result = res },
                LogString = "上傳燈箱.更新Config.Result"
            });
            #endregion

            //Tips: 測試期,可暫固定寫為本機api位址。
            string _uploadUrl = ashxUrls[_switch - 1].ToString();//"http://localhost/AIRMAM5.FileUpload/api/Upload/UploadFile";//

            var configInfo = new UploadConfigViewModel
            {
                TargetUrl = _uploadUrl,
                UploadFileBuffer = _fileBuffer,
                SimultaneousUploads = _simultaneous < 1 ? 3 : _simultaneous,
                LoginId = User.Identity.Name,
                TempFolder = Guid.NewGuid().ToString().Replace("-", "")
            };
            #endregion

            //int.TryParse(ConfigurationManager.AppSettings["UploadTimeout"].ToString(), out int _timeoutsec);
            configInfo.TimeoutSec = Config.UploadTimeOut;//_timeoutsec;
            SubjectUploadViewModel model = new SubjectUploadViewModel()
            {
                DirId = id,
                DirTemplate = _dirTemplate,
                fsSUBJECT_ID = subjid,
                MediaTypeList = mediaType,
                ArcPreTempList = arcpreList,
                MediaFileExtension = fileExtension,
                UploadConfig = configInfo,
                FileSecretList = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.FILESECRET.ToString(), true, false, false),
                FileLicenseList = new LicenseService().GetListItem(null, true) //TIP_20211008_決議:不用特別限制版權條件
            };

            return PartialView("_Upload", model);
        }
        #endregion

        #region ----------------------((影音圖文-資料分頁按鈕功能:加入借調,刪除,修改,批次修改,重轉,置換))----------------------
        /*
         * 加入借調
         * Controller: Booking
         * Method    : AddBooking
         * Parameter : List<MaterialCreateModel>
         * */

        /// <summary>
        /// 指定媒體 刪除
        /// </summary>
        /// <param name="type">媒體的樣板類型: V, A, P, D </param>
        /// <param name="fileno">檔案編號 </param>
        public ActionResult _DeleteMedia(string type, string fileno)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "DeleteMediaModal" });
            var _param = new { type, fileno };

            DeleteMetadataViewModel model = new DeleteMetadataViewModel(fileno, type, string.Empty);
            return PartialView("_DeleteMedia", model);
        }

        /// <summary>
        /// 指定媒體 刪除POST
        /// <para> 2020-06-03 : 刪除媒資檔案,會影響DashBoard的入庫統計值與圖表的統計數值,加入更新處理(SignalR) </para>
        /// </summary>
        /// <param name="model"> </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMedia(DeleteMetadataViewModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region 【檢查】
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (string.IsNullOrEmpty(model.FileNo))
                {
                    result.IsSuccess = false;
                    result.Message = "檔案編號有誤";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                //判斷是否可以刪除
                bool _is = _tblWorkService.IsWorkOnTranscode(model.FileCategory, model.FileNo, "DELETE");
                if (_is)
                {
                    result.IsSuccess = false;
                    result.Message = "檔案已進入轉檔作業，不可刪除!";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
                string _nm = model.FileCategory == FileTypeEnum.V.ToString() ? "影片檔案" :
                        (model.FileCategory == FileTypeEnum.A.ToString()) ? "聲音檔案" :
                            (model.FileCategory == FileTypeEnum.P.ToString()) ? "圖片檔案" :
                                (model.FileCategory == FileTypeEnum.D.ToString()) ? "文件檔案" : string.Empty;

                res = new ProcedureDeleteService(_serilogService).DeleteArcByTypeFileno(model.FileCategory, model.FileNo, model.Reason, User.Identity.Name);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M003",     //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, _nm, _str),
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
                    Method = "[DeleteMedia]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = string.Format($"刪除{_nm}.Result")
                });
                #endregion

                //refresh dashboard
                //new BroadcastHub().RefreshCountsOfDashBoard(CurrentUser.Id);
                new BroadcastHub2().RefreshCountsOfDashBoard(CurrentUser.Id);
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
                    Method = "[DeleteMedia]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, Exception = ex },
                    LogString = "刪除媒資檔案.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 指定媒體{影音圖文}內容頁 編輯
        /// </summary>
        /// <param name="subjid">主題節點id </param>
        /// <param name="type">媒體的樣板類型: V, A, P, D </param>
        /// <param name="fileno">預案編號: 在批次修改,[fileno]代表前端回傳的多批檔案編號.請以逗號(,)為分隔符號 </param>
        public ActionResult _EditMedia(string subjid, string type, string fileno)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "EditMediaModal" });
            var _param = new { subjid, type, fileno };

            SubjectFileMetaViewModel model = new SubjectFileMetaViewModel();
            //↑model裡面的fsFILE_NO 在批次修改裡,代表前端回傳的多批檔案編號.

            string _fileno = fileno.Split(new char[] { ',' })[0];
            //↑在批次修改裡,[fileno]代表前端回傳的多筆檔案編號.(請以逗號(,)為分隔符號)

            #region 前端所需資料---------------------------- 多筆:預設取第一筆Metadata
            FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), type);
            string _fileUrl = string.Empty, _imgUrl = string.Empty, _subjPath = string.Empty, _filePath = string.Empty;
            switch (mediatype)
            {
                case FileTypeEnum.V:
                    model = _arcVideoService.GetVideoBySubjectFile(subjid, _fileno).Select(s => new SubjectFileMetaViewModel().FormatConversion(s, mediatype)).FirstOrDefault();
                    
                    model.ArcPreAttributes = _arcVideoService.GetVideoAttrByFile(_fileno).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();
                    break;
                case FileTypeEnum.A:
                    model = _arcAudioService.GetArcAudioByIdFile(subjid, _fileno).Select(s => new SubjectFileMetaViewModel().FormatConversion(s, mediatype)).FirstOrDefault();
                    
                    model.ArcPreAttributes = _arcAudioService.GetAudioAttrByFile(_fileno).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();
                    break;
                case FileTypeEnum.P:
                    model = _arcPhotoService.GetArcPhotoByIdFile(subjid, _fileno).Select(s => new SubjectFileMetaViewModel().FormatConversion(s, mediatype)).FirstOrDefault();
                    
                    model.ArcPreAttributes = _arcPhotoService.GetPhotoAttrByFile(_fileno).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();
                    break;
                case FileTypeEnum.D:
                    model = _arcDocService.GetArcDocByIdFile(subjid, _fileno).Select(s => new SubjectFileMetaViewModel().FormatConversion(s, mediatype)).FirstOrDefault();
                    
                    model.ArcPreAttributes = _arcDocService.GetDocAttrByFile(_fileno).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();
                    break;
                default:
                    break;
            }

            model.FileSecretList = _tbzCodeService.CodeListItemSelected(TbzCodeIdEnum.FILESECRET.ToString(), model.FileSecret.ToString());
            model.FileLicenseList = new LicenseService().GetListItem(null, true);  //TIP_20211008_決議:不用特別限制版權條件
            model.fsFILE_NO = fileno;
            ////Tips: ↑ model[fsFILE_NO] 在批次修改裡,代表前端回傳的多筆檔案編號.(請以逗號(,)為分隔符號)
            #endregion -----------------------------------------

            return PartialView("_EditMedia", model);
        }

        /// <summary>
        /// 指定媒體{影音圖文}內容頁 編輯POST
        /// <para>(1)單筆=>全部都會傳"接口名_IsEdit=on"(但其實可以強迫全部更新), 如: fsATTRIBUTE4_IsEdit=on</para>
        /// <para>(2)多筆=>只有存在"接口名_IsEdit=on"的欄位才能更新</para>
        /// <para>  如果沒有存在"接口名_IsEdit=on",就不能更新  </para>
        /// </summary>
        /// <param name="form"></param>
        /// <remarks>20210910_ADDED_新增[版權]欄位 </remarks>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditMedia(FormCollection form)
        {
            var _param = form;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            string _type = form["FileCategory"].ToString(), _typenm = form["FileCategoryTxt"].ToString();
            //↑form裡面的fsFILE_NO 在批次修改裡,代表前端回傳的多批檔案編號.
            VerifyResult res = new VerifyResult();

            try
            {
                FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), _type);
                switch (mediatype)
                {
                    case FileTypeEnum.V:
                        res = _arcVideoService.UpdateMultiple(form, User.Identity.Name);
                        break;
                    case FileTypeEnum.A:
                        res = _arcAudioService.UpdateMultiple(form, User.Identity.Name);
                        break;
                    case FileTypeEnum.P:
                        res = _arcPhotoService.UpdateMultiple(form, User.Identity.Name);
                        break;
                    case FileTypeEnum.D:
                        res = _arcDocService.UpdateMultiple(form, User.Identity.Name);
                        break;
                    default:
                        //
                        break;
                }

                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",     //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, _typenm, _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[EditMedia]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = string.Format($"編輯{_typenm}.Result")
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
                    Method = "[EditMedia]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, Exception = ex },
                    LogString = string.Format($"編輯{_typenm}.Exception"),//"Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 指定媒體{影片,聲音} 重轉檔
        /// </summary>
        /// <param name="model"> </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReTransfer(ReTransferViewModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            UpdateLWorkReTranResult res = new UpdateLWorkReTranResult();

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
                if (string.IsNullOrEmpty(model.fsFILE_NO))
                {
                    result.IsSuccess = false;
                    result.Message = "檔案編號有誤";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                //STEP-1: 取得檔案編號的工作編號[fsWORK_ID]
                var _work = _tblWorkService.GetByTypeFileno("TRANSCODE", model.fsFILE_NO);
                if (_work == null)
                {
                    result.IsSuccess = false;
                    result.Message = "找不到對應的工作編號";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                List<string> workIds = new List<string>
                {
                    _work.fnWORK_ID.ToString()
                };

                //STEP-2: 工作編號[fsWORK_ID]執行重轉SP
                res = _tblWorkService.UpdateLWorkReTran(workIds, CurrentUser.UserName);
                //Tips: 轉檔,只要沒有Exception,皆為True；多筆的成功/失敗都會記錄在res
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M021",     //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, model.FileCategory + "重轉檔", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(workIds),
                    User.Identity.Name);
                #endregion
                result.IsSuccess = res.IsSuccess;
                //result.Message = _str;
                result.Records = _param;
                result.StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed;

                result.Message = res.IsSuccess ? string.Empty : res.Message + "\n ";
                result.Message = res.Processed.Count() > 0
                    ? string.Format($"{result.Message} 檔案【{string.Join(",", res.Processed)}】新增轉檔資料成功，下次更新資料時就會進行排程。 \n") : result.Message;
                result.Message = res.UnProcessed.Count() > 0
                    ? string.Format($"{result.Message} 檔案【{string.Join(",", res.UnProcessed)}】正在轉檔中！ \n") : result.Message;
                result.Message = res.Failure.Count() > 0
                    ? string.Format($"{result.Message} 檔案【{string.Join(",", res.Failure)}】新增轉檔資料失敗！") : result.Message;

                #region Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[ReTransfer]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = model.FileCategory + "重轉檔.Result"
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
                    Method = "[ReTransfer]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, Exception = ex },
                    LogString = model.FileCategory + "重轉檔.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 指定媒體{影音}內容頁 置換檔案
        /// </summary>
        /// <param name="subjid">主題id </param>
        /// <param name="type">媒體的樣板類型: MEDIATYPE_TO_V, MEDIATYPE_TO_A (只有影,音 可置換) </param>
        /// <param name="fileno">檔案編號 </param>
        public ActionResult _ChangeMedia(string subjid, string type, string fileno)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "ReplacementModal" });
            VerifyResult res = new VerifyResult();

            #region 媒體類型副檔名 List
            //置換只能針對單一媒體類別,這裡固定為單一媒體類別可上傳的副檔名資料
            List<FileExtensionViewModel> fileExtension = new List<FileExtensionViewModel>();
            var get = _configService.GetConfigBy(type).FirstOrDefault();
            string val = get == null ? string.Empty : get.fsVALUE;
            fileExtension.Add(new FileExtensionViewModel
            {
                MediaType = type,
                FileExtension = val
            });
            #endregion

            #region 檔案上傳元件使用的參數資訊_&_更新UPLOAD_ASHX_CURRENT_SWITCH
            string[] ashxUrls = _configService.GetConfigBy("UPLOAD_ASHX").FirstOrDefault().fsVALUE.Split(new char[] { ';' }); // 上傳路徑ashx1~3。;為分隔符號
            var currentSwitch = _configService.GetConfigBy("UPLOAD_ASHX_CURRENT_SWITCH").FirstOrDefault(); //目前要傳至那一個ASHX,使用者開啓上傳視窗就會+1 (1~3跑)
            int.TryParse(currentSwitch.fsVALUE, out int _switch);   //目前要傳至那一個ASHX
            int.TryParse(_configService.GetConfigBy("UPLOAD_ASHX_MAX_COUNT").FirstOrDefault().fsVALUE, out int _maxCount); //上傳主機台數
            int.TryParse(_configService.GetConfigBy("UPLOAD_FILE_BUFFER").FirstOrDefault().fsVALUE, out int _fileBuffer);  //分塊上傳檔案大小(MB)
            int.TryParse(_configService.GetConfigBy("UPLOAD_SIMULTANEOUS_FILEs").FirstOrDefault().fsVALUE, out int _simultaneous); //同時上傳數（默認值null：3）

            //更新UPLOAD_ASHX_CURRENT_SWITCH
            _switch = (_switch + 1 > _maxCount) ? 1 : (_switch += 1);
            tbzCONFIG upd = new tbzCONFIG(currentSwitch)
            {
                fsVALUE = _switch.ToString(),
                fsUPDATED_BY = User.Identity.Name
            };
            res = _configService.UpdateBy(upd);
            string _str = res.IsSuccess ? "成功" : "失敗";

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M021",        //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "置換(更新UPLOAD_ASHX_CURRENT_SWITCH) ", _str),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(upd),
                User.Identity.Name);
            #endregion
            #region _Serilog.Verbose
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = CONTR_NAEM,
                Method = "[_ChangeMedia]",
                EventLevel = SerilogLevelEnum.Verbose,
                Input = new { Param = upd, Result = res },
                LogString = "置換操作.更新Config.Result"
            });
            #endregion

            //Tips: 測試期,暫固定寫為本機api位址。
            string _uploadUrl = ashxUrls[_switch - 1].ToString();//"http://localhost/AIRMAM5.FileUpload/api/Upload/UploadFile";//
            UploadConfigViewModel configInfo = new UploadConfigViewModel
            {
                TargetUrl = _uploadUrl,
                UploadFileBuffer = _fileBuffer,
                SimultaneousUploads = _simultaneous < 1 ? 3 : _simultaneous,
                LoginId = User.Identity.Name,
                TempFolder = Guid.NewGuid().ToString().Replace("-", "")
            };
            //int.TryParse(ConfigurationManager.AppSettings["UploadTimeout"].ToString(), out int _timeoutsec);
            configInfo.TimeoutSec = Config.UploadTimeOut;//_timeoutsec;
            #endregion

            #region 檔案機密等級 Added_20200302
            string /*_fileSecret = "0", */_fileCategory = string.Empty, _ques = string.Empty, fileLicense = string.Empty;
            short fileSecret = 0;
            //上傳檔案類型: UPLOAD_MEDIATYPE = MEDIATYPE_TO_V, MEDIATYPE_TO_A, MEDIATYPE_TO_P, MEDIATYPE_TO_D
            if (type.Contains("_TO_V"))
            {
                var _VIDEO = _arcVideoService.GetByFileno(fileno);
                fileSecret = _VIDEO.fnFILE_SECRET;
                _fileCategory = FileTypeEnum.V.ToString();
                _ques = "1.是否保留關鍵影格描述與段落描述?";
                fileLicense = _VIDEO.fsLICENSE;
            }
            else if (type.Contains("_TO_A"))
            {
                var _AUDIO = _arcAudioService.GetByFileno(fileno);
                fileSecret = _AUDIO.fnFILE_SECRET;
                _fileCategory = FileTypeEnum.A.ToString();
                _ques = "1.是否保留段落描述?";
                fileLicense = _AUDIO.fsLICENSE;
            }
            else if (type.Contains("_TO_P"))
            {
                var _PHOTO = _arcPhotoService.GetByFileno(fileno);
                fileSecret = _PHOTO.fnFILE_SECRET;
                _fileCategory = FileTypeEnum.P.ToString();
                fileLicense = _PHOTO.fsLICENSE;
            }
            else if (type.IndexOf("_TO_D") >= 0)
            {
                var _DOC = _arcDocService.GetByFileno(fileno);
                fileSecret = _DOC.fnFILE_SECRET;
                _fileCategory = FileTypeEnum.D.ToString();
                fileLicense = _DOC.fsLICENSE;
            }
            #endregion
            ChangeUploadViewModel model = new ChangeUploadViewModel
            {
                fsSUBJECT_ID = subjid,
                fsFILE_NO = fileno,
                MediaFileExtension = fileExtension,
                UploadConfig = configInfo,
                //Added_20200302:機密等級。
                FileSecretList = _tbzCodeService.CodeListItemSelected(TbzCodeIdEnum.FILESECRET.ToString(), fileSecret.ToString() + ";"),
                FileSecret = fileSecret,
                FileCategory = _fileCategory,
                DisplayQuestion = _ques,
                FileLicenseList = new LicenseService().GetListItem(null,true),  //TIP_20211008_決議:不用特別限制版權條件//20210914_ADDED:版權。
                FileLicense = fileLicense,
            };

            return PartialView(model);
        }
        #endregion
        /// <summary>
        /// 第三方系統頁面(文稿,公文等等)
        /// </summary>
        /// <returns></returns>

        public ActionResult _DocSystem() {
            return PartialView("_DocSystem");
        }


        #region ----------------------((影-關鍵影格 按鈕功能))----------------------
        /// <summary>
        /// 將關鍵影格截圖 設為代表圖
        /// </summary>
        /// <param name="fileno"></param>
        /// <param name="time"> [fsTIME] 格式: 002760040(表示 002760.040秒) </param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetHeadFrame(string fileno, string time)
        {
            var _param = new { fileno, time };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();
            #region _檢查_
            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            if (string.IsNullOrEmpty(fileno))
            {
                result.IsSuccess = false;
                result.Message = "檔案編號有誤";
                result.StatusCode = HttpStatusCode.BadRequest;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            try
            {
                res = _arcVideoService.UpdateHeadFrameBy(fileno, time, User.Identity.Name);
                string _str = res.IsSuccess ? "成功" : "失敗";
                //
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",        //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "關鍵影格代表圖", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[SetHeadFrame]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = "關鍵影格代表圖.Result"
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
                    Method = "[SetHeadFrame]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "關鍵影格代表圖.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        
        /// <summary>
        /// 關鍵影格_新增 送出存檔POST 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult KeyFrameAdd(KeyFrameCUViewModel model)
        {
            //TIPS: 關鍵影格新增成功後,前端會直接使用原本的資料內容顯示在頁面中(但前端會傳null)
            var _param = model;//new KeyFrameCUViewModel().FormatConvert(model);
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();
            #region _檢查_
            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            if (string.IsNullOrEmpty(model.fsFILE_NO))
            {
                result.IsSuccess = false;
                result.Message = "檔案編號有誤";
                result.StatusCode = HttpStatusCode.BadRequest;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            try
            {
                //取得相關資訊
                var _arcVideo = _arcVideoService.GetByFileno(model.fsFILE_NO);

                //關鍵影格原則(初始)
                string _kfRule = "";

                if (double.Parse(_arcVideo.fnWIDTH.ToString()) / double.Parse(_arcVideo.fnHEIGHT.ToString()) > 1.7)
                    _kfRule = _configService.GetConfigBy("KEYFRAME_RULE2").FirstOrDefault().fsVALUE;
                else
                    _kfRule = _configService.GetConfigBy("KEYFRAME_RULE4").FirstOrDefault().fsVALUE;

                string[] _kfRuleAry = _kfRule.Split(';');

                string _targetPath = _configService.GetConfigBy("MEDIA_FOLDER_V_K").FirstOrDefault().fsVALUE;

                #region >>>>> 開始擷取單張KF
                double.TryParse(model.SetTime, out double _time);
                string _timeLab = _time.ToString("000000.000").Replace(".", "");

                /* 20200910 : 移除AIRMAM5.KeyFrame 專案,整合到AIRMAM5。
                 *            AIRMAM5.Common.clsKEYFRAME.cs 改使用 AIRMAM5.Common.KeyFrame.cs。 */
                //clsKEYFRAME _keyframe = new clsKEYFRAME
                //{
                //    fsSOURCE_FILE = _arcVideo.fsFILE_PATH_L + _arcVideo.fsFILE_NO + "_L." + _arcVideo.fsFILE_TYPE_L,
                //    fsTARGET_PATH = string.Format($@"{_targetPath}{_arcVideo.fsFILE_NO.Substring(0, 4)}\{_arcVideo.fsFILE_NO.Substring(4, 2)}\{_arcVideo.fsFILE_NO.Substring(6, 2)}\{_arcVideo.fsFILE_NO}\"),
                //    fsFILE_NAME = string.Format($"{_arcVideo.fsFILE_NO}_{_timeLab}"),
                //    fsEXT = _kfRule.Split(';')[0],
                //    fnTIME = _time,//double.Parse(clsARC_VIDEO_K.fdTIME),
                //    fnQUALITY = 100,
                //    fnWIDTH = int.Parse(_kfRule.Split(';')[3]),
                //    fnHEIGHT = int.Parse(_kfRule.Split(';')[4])
                //};
                //res = _keyframe.fnGET_KEYFRAME();
                
                KeyFrame _keyframe = new KeyFrame
                {
                    SourceFile = _arcVideo.fsFILE_PATH_L + _arcVideo.fsFILE_NO + "_L." + _arcVideo.fsFILE_TYPE_L,
                    TargetPath = string.Format($@"{_targetPath}{_arcVideo.fsFILE_NO.Substring(0, 4)}\{_arcVideo.fsFILE_NO.Substring(4, 2)}\{_arcVideo.fsFILE_NO.Substring(6, 2)}\{_arcVideo.fsFILE_NO}\"),
                    FileName = string.Format($"{_arcVideo.fsFILE_NO}_{_timeLab}"),
                    FeileEXT = _kfRule.Split(';')[0],
                    FileTime = _time,
                    FileQuality = 100,
                    FileWidth = int.Parse(_kfRule.Split(';')[3]),
                    FileHeight = int.Parse(_kfRule.Split(';')[4])
                };
                res = _keyframe.CaptureConvert();  //_keyframe.CaptureKeyFrame();

                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M021",        //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "擷取關鍵影格", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                if (!res.IsSuccess)
                {
                    result = new ResponseResultModel(res) { Records = _param, StatusCode = HttpStatusCode.BadRequest };
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[KeyFrameAdd]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = res },
                    LogString = "擷取單張KF.Result"
                });
                #endregion
                #endregion >>>>> 開始擷取單張KF_end

                #region >>>>> Create
                tbmARC_VIDEO_K _video_k = new tbmARC_VIDEO_K
                {
                    fsFILE_NO = model.fsFILE_NO,
                    fsTIME = _timeLab,
                    fsTITLE = string.IsNullOrEmpty(model.Title) ? string.Empty : model.Title,
                    fsDESCRIPTION = model.Description ?? string.Empty,
                    fsFILE_PATH = _keyframe.TargetPath,
                    //fsFILE_SIZE = "",
                    fsFILE_TYPE = _keyframe.FeileEXT,
                    //fcHEAD_FRAME = "",
                    fsCREATED_BY = User.Identity.Name,
                    fdCREATED_DATE = DateTime.Now,
                    fsUPDATED_BY = string.Empty,
                    fdUPDATED_DATE = null
                };

                if (System.IO.File.Exists(_keyframe.TargetPath + _arcVideo.fsFILE_NO + "_" + _timeLab.Replace(".", "") + "." + _kfRuleAry[0]))
                    _video_k.fsFILE_SIZE = new System.IO.FileInfo(_keyframe.TargetPath + _arcVideo.fsFILE_NO + "_" + _timeLab.Replace(".", "") + "." + _kfRuleAry[0]).Length.ToString();
                else
                    _video_k.fsFILE_SIZE = "0";

                res = _arcVideoService.InsertKeyFrameBy(_video_k);
                _str = res.IsSuccess ? "成功" : "失敗";
                #endregion
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M001",        //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "關鍵影格", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };

                //成功:至資料庫取回實際資料內容
                if (result.IsSuccess)
                {
                    result.Records = _arcVideoService.GetKeyFrame(model.fsFILE_NO, _timeLab).Select(s => new KeyFrameCUViewModel().FormatConvert(s)).FirstOrDefault();
                }
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[InsertKeyFrame]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = "新增關鍵影格.Result"
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
                    Method = "[KeyFrameAdd]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "新增關鍵影格.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 關鍵影格_編輯 送出存檔POST (UI提供可修改:標題、描述)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult KeyFrameEdit(KeyFrameCUViewModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();
            #region _檢查_
            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            if (string.IsNullOrEmpty(model.fsFILE_NO))
            {
                result.IsSuccess = false;
                result.Message = "檔案編號有誤";
                result.StatusCode = HttpStatusCode.BadRequest;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            try
            {
                var get = _arcVideoService.GetKeyFrameBy(model.fsFILE_NO, model.SetTime);
                if (get == null)
                {
                    result.IsSuccess = false;
                    result.Message = "查無記錄";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                get.fsDESCRIPTION = model.Description ?? string.Empty;
                get.fsTITLE = model.Title;
                get.fdUPDATED_DATE = DateTime.Now;
                get.fsUPDATED_BY = User.Identity.Name;

                res = _arcVideoService.UpdateKeyFrameBy(get);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",        //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "關鍵影格", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[KeyFrameEdit]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = "編輯關鍵影格.Result"
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
                    Method = "[KeyFrameEdit]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "編輯關鍵影格.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 關鍵影格_刪除 送出POST : 指定fsFILE_NO+ fsTime(可多筆)
        /// </summary>
        /// <param name="fileno">檔案編號 [fsFILE_NO] </param>
        /// <param name="time">時間 [fsTIME] : 單筆or多筆刪除 </param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult KeyFrameDelete(string fileno, string[] time)
        {
            var _param = new { fileno, time };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            #region _檢查_
            if (!CheckUserAuth(CONTR_NAEM))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            if (string.IsNullOrEmpty(fileno))
            {
                result.IsSuccess = false;
                result.Message = "檔案編號有誤";
                result.StatusCode = HttpStatusCode.BadRequest;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            try
            {
                string _msg = string.Empty;
                List<tbmARC_VIDEO_K> _kfs = new List<tbmARC_VIDEO_K>();
                foreach (var i in time)
                {
                    var get = _arcVideoService.GetKeyFrameBy(fileno, i);
                    if (get == null)
                    {
                        _msg = string.Concat(_msg, i);
                        continue;
                    }
                    _kfs.Add(get);
                }

                res = _arcVideoService.DeleteKeyFrameMultiple(_kfs);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M003",        //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "關鍵影格", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[KeyFrameDelete]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result, NoDel = _msg },
                    LogString = "刪除關鍵影格.Result"
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
                    Method = "[KeyFrameDelete]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "刪除關鍵影格.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region ----------------------((影音-段落描述 按鈕功能))----------------------
        /// <summary>
        /// 新增段落描述的資料頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult _ParagraphAdd()
        {
            if (!CheckUserAuth(CONTR_NAEM))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "ParagraphAddModal" });

            return PartialView();
        }

        /// <summary>
        ///  指定媒體{影音} 新增段落描述
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ParagraphAdd(ParagraphCUViewModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (string.IsNullOrEmpty(model.fsFILE_NO))
                {
                    result.IsSuccess = false;
                    result.Message = "檔案編號有誤";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), model.FileCategory);
                switch (mediatype)
                {
                    case FileTypeEnum.V:
                        var _vd = new tbmARC_VIDEO_D(model)
                        {
                            fdCREATED_DATE = DateTime.Now,
                            fsCREATED_BY = User.Identity.Name
                        };
                        res = _arcVideoService.InsertParagraph(_vd);
                        break;
                    case FileTypeEnum.A:
                        var _ad = new tbmARC_AUDIO_D(model)
                        {
                            fdCREATED_DATE = DateTime.Now,
                            fsCREATED_BY = User.Identity.Name
                        };
                        res = _arcAudioService.InsertParagraph(_ad);
                        break;
                    default:
                        //
                        break;
                }

                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M001",        //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "段落描述", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };

                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[ParagraphAdd]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = "新增段落描述.Result"
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
                    Method = "[ParagraphAdd]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "新增段落描述.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 取得指定段落描述的編輯資料頁面
        /// </summary>
        /// <param name="type">媒體類型</param>
        /// <param name="fileno">檔案編號</param>
        /// <param name="seqno">段落序號</param>
        /// <returns></returns>
        public ActionResult _ParagraphEdit(string type, string fileno, int seqno)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "ParagraphEditModal" });

            var _param = new { type, fileno };
            FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), type);
            SubjectFileSeqmentModel model = new SubjectFileSeqmentModel();

            switch (mediatype)
            {
                case FileTypeEnum.V:
                    model = _arcVideoService.GetVideoSeqment(fileno, seqno)
                                .Select(s => new SubjectFileSeqmentModel().FormatConvert(s, mediatype)).FirstOrDefault();
                    break;
                case FileTypeEnum.A:
                    model = _arcAudioService.GetAudioSeqment(fileno, seqno)
                                .Select(s => new SubjectFileSeqmentModel().FormatConvert(s, mediatype)).FirstOrDefault();
                    break;
                default:
                    //
                    break;
            }

            return PartialView(model);
        }

        /// <summary>
        ///  指定媒體{影音} 修改段落描述 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ParagraphEdit(ParagraphCUViewModel model)
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
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (string.IsNullOrEmpty(model.fsFILE_NO))
                {
                    result.IsSuccess = false;
                    result.Message = "檔案編號有誤";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), model.FileCategory);
                switch (mediatype)
                {
                    case FileTypeEnum.V:
                        var _vd = _arcVideoService.GetParagraphBy(model.fsFILE_NO, model.SeqNo).FirstOrDefault();
                        _vd.fsDESCRIPTION = model.Description;
                        _vd.fdBEG_TIME = model.BegTime;
                        _vd.fdEND_TIME = model.EndTime;
                        _vd.fdUPDATED_DATE = DateTime.Now;
                        _vd.fsUPDATED_BY = User.Identity.Name;
                        res = _arcVideoService.EditParagraph(_vd);

                        res.Data = new SubjectFileSeqmentModel().FormatConvert(_vd, mediatype);
                        break;
                    case FileTypeEnum.A:
                        var _ad = _arcAudioService.GetParagraphBy(model.fsFILE_NO, model.SeqNo).FirstOrDefault();
                        _ad.fsDESCRIPTION = model.Description;
                        _ad.fdBEG_TIME = model.BegTime;
                        _ad.fdEND_TIME = model.EndTime;
                        _ad.fdUPDATED_DATE = DateTime.Now;
                        _ad.fsUPDATED_BY = User.Identity.Name;
                        res = _arcAudioService.EditParagraph(_ad);

                        res.Data = new SubjectFileSeqmentModel().FormatConvert(_ad, mediatype);
                        break;
                    default:
                        //
                        break;
                }

                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M002",        //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "段落描述", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[ParagraphEdit]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = "修改段落描述.Result"
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
                    Method = "[ParagraphEdit]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "修改段落描述.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 取得指定段落描述的刪除資料頁面
        /// </summary>
        /// <param name="type">媒體類型</param>
        /// <param name="fileno">檔案編號</param>
        /// <param name="seqno">段落序號</param>
        /// <returns></returns>
        public ActionResult _ParagraphDelete(string type, string fileno, int seqno)
        {
            if (!CheckUserAuth(CONTR_NAEM))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "ParagraphDeleteModal" });

            var _param = new { type, fileno };
            FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), type);
            SubjectFileSeqmentModel model = new SubjectFileSeqmentModel();

            switch (mediatype)
            {
                case FileTypeEnum.V:
                    model = _arcVideoService.GetVideoSeqment(fileno, seqno).Select(s => new SubjectFileSeqmentModel().FormatConvert(s, mediatype)).FirstOrDefault();
                    break;
                case FileTypeEnum.A:
                    model = _arcAudioService.GetAudioSeqment(fileno, seqno).Select(s => new SubjectFileSeqmentModel().FormatConvert(s, mediatype)).FirstOrDefault();
                    break;
                default:
                    //
                    break;
            }

            return PartialView(model);
        }

        /// <summary>
        ///  指定媒體{影音} 刪除段落描述 
        /// </summary>
        /// <param name="type">媒體檔案類型: V, A (只有影,音) </param>
        /// <param name="fileno">檔案編號 </param>
        /// <param name="seqno">段落資料序號 </param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ParagraphDelete(string type, string fileno, int seqno)
        {
            var _param = new { type, fileno, seqno };
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
                if (string.IsNullOrEmpty(fileno))
                {
                    result.IsSuccess = false;
                    result.Message = "檔案編號有誤";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), type);
                switch (mediatype)
                {
                    case FileTypeEnum.V:
                        var _vd = _arcVideoService.GetParagraphBy(fileno, seqno).FirstOrDefault();
                        res = _arcVideoService.DeleteParagraph(_vd);
                        break;
                    case FileTypeEnum.A:
                        var _ad = _arcAudioService.GetParagraphBy(fileno, seqno).FirstOrDefault();
                        res = _arcAudioService.DeleteParagraph(_ad);
                        break;
                    default:
                        //
                        break;
                }

                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M003",        //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "段落描述", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Verbose
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[ParagraphDelete]",
                    EventLevel = SerilogLevelEnum.Verbose,
                    Input = new { Param = _param, Result = result },
                    LogString = "刪除段落描述.Result"
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
                    Method = "[ParagraphDelete]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "刪除段落描述.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion
    }
}
