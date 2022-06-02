using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.ArchiveMove;
using AIRMAM5.DBEntity.Models.Directory;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Subject;
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
    /// 歸檔搬遷功能
    /// </summary>
    //[Authorize(Roles = "Administrator,MediaManager")]
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class ArchiveMoveController : BaseController
    {
        readonly DirectoriesService _directoriesService;
        readonly ISubjectService _subjectService;
        private readonly ArcVideoService _arcVideoService;
        private readonly ArcAudioService _arcAudioService;
        private readonly ArcPhotoService _arcPhotoService;
        private readonly ArcDocService _arcDocService;

        public ArchiveMoveController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService, ISubjectService subjectService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _subjectService = subjectService;
            _directoriesService = new DirectoriesService();
            _arcVideoService = new ArcVideoService(serilogService);
            _arcAudioService = new ArcAudioService(serilogService);
            _arcPhotoService = new ArcPhotoService(serilogService);
            _arcDocService = new ArcDocService(serilogService);
        }

        /// <summary>
        /// 首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckUserAuth("ArchiveMove")) return View("NoAuth");

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "歸檔搬遷"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 主題編號 取 {影/音/圖/文}資料
        /// </summary>
        /// <param name="subjid">主題編號 </param>
        /// <returns></returns>
        public JsonResult GetSubjFiles(string subjid)
        {
            var _param = new { subjid };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);

            #region _檢查__
            if (!CheckUserAuth("ArchiveMove"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var _getSubj = _subjectService.GetBy(subjid);
            if (_getSubj == null)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"主題編號({subjid}) 不存在,請重新操作!");
                result.StatusCode = HttpStatusCode.BadRequest;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            #endregion

            try
            {
                /* modified_2010901 *///model = model.SetSubjectFilesData(subjid);
                //model.SubjectId = subjid;
                //model.DirId = _getSubj.fnDIR_ID;
                List<SubjFileModel> _v = _arcVideoService.GetVideioBySubjectId(subjid)
                    .Select(s => new SubjFileModel().DataConvert(s, FileTypeEnum.V)).ToList();

                var _a = _arcAudioService.GetArcAudioBySubjectId(subjid)
                    .Select(s => new SubjFileModel().DataConvert(s, FileTypeEnum.A)).ToList();

                var _p = _arcPhotoService.GetArcPhotoBySubjectId(subjid)
                    .Select(s => new SubjFileModel().DataConvert(s, FileTypeEnum.P)).ToList();

                var _d = _arcDocService.GetArcDocBySubjectId(subjid)
                    .Select(s => new SubjFileModel().DataConvert(s, FileTypeEnum.D)).ToList();

                SubjectFilesViewModel model = new SubjectFilesViewModel
                {
                    SubjectId = subjid,
                    DirId = _getSubj.fnDIR_ID,
                    VideoFiles = _v,//_v.Select(s => new SubjFileModel().FormatConversion(s, FileTypeEnum.V)).ToList(),
                    AudioFiles = _a,//_a.Select(s => new SubjFileModel().FormatConversion(s, FileTypeEnum.A)).ToList(),
                    PhotoFiles = _p,//_p.Select(s => new SubjFileModel().FormatConversion(s, FileTypeEnum.P)).ToList(),
                    DocFiles = _d,//_d.Select(s => new SubjFileModel().FormatConversion(s, FileTypeEnum.D)).ToList()
                };

                result.IsSuccess = true;
                result.StatusCode = HttpStatusCode.OK;
                result.Data = model;
                result.Message = "OK";
                #region _Serilog.Debug
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArchiveMove",
                    Method = "[GetSubjFiles]",
                    EventLevel = SerilogLevelEnum.Debug,
                    Input = new { param = _param, Result = result },
                    LogString = "主題取資料.Result"
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
                    Controller = "ArchiveMove",
                    Method = "[GetSubjFiles]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "主題取資料.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 依檔案編號 取得{影/音/圖/文} 相同樣版的目錄節點樹狀資料
        /// <para> Tips: 前端若選擇多筆檔案, 傳入時僅以第一筆檔案編號為查詢值。</para>
        /// </summary>
        /// <param name="m"> 參數(FileType,FileNo,DirId,UserName,KeyWord) </param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetTargetDir(GetDirByFileNoLoadOnDemand m)
        {
            var _param = m;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);

            #region _檢查__
            if (!CheckUserAuth("ArchiveMove"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            #endregion
            
            try
            {
                m.UserName = string.IsNullOrEmpty(m.UserName) ? User.Identity.Name : m.UserName;
                m.KeyWord = m.KeyWord ?? string.Empty;
                //取得樹狀 List<DirectoriesItemModel>
                var get = _directoriesService.GetDirMatchTemplate(m)
                    .Select(s => new DirectoriesItemModel().FormatConversion(s))
                    .ToList();

                result.IsSuccess = true;
                result.StatusCode = HttpStatusCode.OK;
                result.Data = get;
                result.Message = "OK";
                #region _Serilog.Debug//
                //_serilogService.SerilogWriter(new SerilogInputModel
                //{
                //    Controller = "ArchiveMove",
                //    Method = "[GetTargetDirMatch]",
                //    EventLevel = SerilogLevelEnum.Debug,
                //    Input = new { param = _param, Result = result },
                //    LogString = "檔案搬移目的地目錄.Result"
                //});
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
                    Controller = "ArchiveMove",
                    Method = "[GetTargetDirMatch]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = result, Exception = ex },
                    LogString = "檔案搬移目的地目錄.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //TIP: 「目錄節點Queue 啟用/不啟用」設定, 呼叫不同預存
        public JsonResult GetSubjectsByDirFilter(GetDirAndSubjectsByDirFilter m)
        {
            var _param = m;
            var get = _directoriesService.GetDirSubjectsByIdFilter(m)
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
        /// 檔案變更主題位置 存檔
        /// <para> 前端選取的檔案編號修改其對應的[tbmARC_xxxxx].主題編號。</para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MoveSave(MoveSaveModel m)
        {
            var _param = m;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();
            #region _檢查__
            if (!CheckUserAuth("ArchiveMove"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            try
            {
                FileTypeEnum _type = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), m.FileType);
                switch (_type)
                {
                    case FileTypeEnum.V:
                        var _v = _arcVideoService.GetByFileNos(m.MoveFileNos);
                        if (!_v.Any())
                        {
                            result.IsSuccess = false;
                            result.Message = "檔案編號錯誤,請重新操作";
                            result.StatusCode = HttpStatusCode.NotFound;
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }

                        _v.ForEach(f =>
                        {
                            f.fsSUBJECT_ID = m.TargetSubjId;
                            f.fdUPDATED_DATE = DateTime.Now;
                            f.fsUPDATED_BY = CurrentUser.UserName;
                        });
                        res = _arcVideoService.UpdateByMove(_v);
                        break;
                    case FileTypeEnum.A:
                        var _a = _arcAudioService.GetByFileNos(m.MoveFileNos);
                        if (!_a.Any())
                        {
                            result.IsSuccess = false;
                            result.Message = "檔案編號錯誤,請重新操作";
                            result.StatusCode = HttpStatusCode.NotFound;
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }

                        _a.ForEach(f =>
                        {
                            f.fsSUBJECT_ID = m.TargetSubjId;
                            f.fdUPDATED_DATE = DateTime.Now;
                            f.fsUPDATED_BY = CurrentUser.UserName;
                        });
                        res = _arcAudioService.UpdateByMove(_a);
                        break;
                    case FileTypeEnum.P:
                        var _p = _arcPhotoService.GetByFileNos(m.MoveFileNos);
                        if (!_p.Any())
                        {
                            result.IsSuccess = false;
                            result.Message = "檔案編號錯誤,請重新操作";
                            result.StatusCode = HttpStatusCode.NotFound;
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }

                        _p.ForEach(f =>
                        {
                            f.fsSUBJECT_ID = m.TargetSubjId;
                            f.fdUPDATED_DATE = DateTime.Now;
                            f.fsUPDATED_BY = CurrentUser.UserName;
                        });
                        res = _arcPhotoService.UpdateByMove(_p);
                        break;
                    case FileTypeEnum.D:
                        var _d = _arcDocService.GetByFileNos(m.MoveFileNos);
                        if (!_d.Any())
                        {
                            result.IsSuccess = false;
                            result.Message = "檔案編號錯誤,請重新操作";
                            result.StatusCode = HttpStatusCode.NotFound;
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }

                        _d.ForEach(f =>
                        {
                            f.fsSUBJECT_ID = m.TargetSubjId;
                            f.fdUPDATED_DATE = DateTime.Now;
                            f.fsUPDATED_BY = CurrentUser.UserName;
                        });
                        res = _arcDocService.UpdateByMove(_d);
                        break;
                    default:
                        //
                        break;
                }

                string _str = res.IsSuccess ? "成功" : "失敗", _datatype = string.Format($"歸檔搬遷_{GetEnums.GetDescriptionText(_type)}");
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M012",     //[@USER_ID(@USER_NAME)] 修改 [@TARGET] 的 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, _datatype, "主題編號", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(m),
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
                    Controller = "ArchiveMove",
                    Method = "[MoveSave]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { param = _param, Result = result },
                    LogString = "檔案搬移主題存檔.Result"
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
                    Controller = "ArchiveMove",
                    Method = "[MoveSave]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "檔案搬移主題存檔.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 主題變更目錄節點位置 存檔
        /// <para>  ※ 每次只針對一個主題搬遷。</para>
        /// <para>  ※ 變更後的目錄節點 樣板要與原本的目錄節點樣板 相同。 </para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubjMoveSave(SubjMoveSaveModel m)
        {
            var _param = m;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            #region _檢查_
            if (!CheckUserAuth("ArchiveMove"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            if (m.MoveSubjIds.Length < 1)
            {
                result.IsSuccess = false;
                result.Message = "請先選擇搬移的主題";
                result.StatusCode = HttpStatusCode.BadRequest;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            //取目的地目錄節點的父層目錄ID
            long _TargetParentId = _directoriesService.GetDirById(m.TargetDirId).fnPARENT_ID;
            //
            var get = _directoriesService.GetDirMatchTemplateBySubj(new GetDirBySubjIdLoadOnDemand
            {
                SubjId = m.MoveSubjIds[0],
                DirId = _TargetParentId,
                UserName = CurrentUser.UserName,
                KeyWord = string.Empty
            }).Where(x => x.fnDIR_ID == m.TargetDirId).Any();
            if (!get)
            {
                result.IsSuccess = false;
                result.Message = "目的地-目錄節點的樣板不符合";
                result.StatusCode = HttpStatusCode.BadRequest;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            VerifyResult res = new VerifyResult();
            try
            {
                var _subj = _subjectService.GetByIds(m.MoveSubjIds);
                if (!_subj.Any())
                {
                    result.IsSuccess = false;
                    result.Message = "主題編號錯誤,請重新操作";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                _subj.ForEach(f =>
                {
                    f.fnDIR_ID = m.TargetDirId;
                    f.fdUPDATED_DATE = DateTime.Now;
                    f.fsUPDATED_BY = CurrentUser.UserName;
                });

                res = _subjectService.UpdateByMove(_subj);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M012",     //[@USER_ID(@USER_NAME)] 修改 [@TARGET] 的 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, "主題(歸檔搬遷)", "目錄節點", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(m),
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
                    Controller = "ArchiveMove",
                    Method = "[SubjMoveSave]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { param = _param, Result = result },
                    LogString = "主題搬移目錄節點存檔.Result"
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
                    Controller = "ArchiveMove",
                    Method = "[SubjMoveSave]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "主題搬移目錄節點存檔.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 目錄節點移動
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MoveTreeNode(DirMoveSaveModel model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(false, string.Empty, _param);
            #region _檢查_
            if (!CheckUserAuth("ArchiveMove"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            VerifyResult res = new VerifyResult();
            try
            {
                res = _directoriesService.MoveNodeUpdate(model.MoveDirId, model.TargetParentId, User.Identity.Name);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M012",     //[@USER_ID(@USER_NAME)] 修改 [@TARGET] 的 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogTargetParams, CurrentUser.UserName, CurrentUser.fsNAME, "系統目位置(歸檔搬遷)", "目錄節點", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(model),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = model,
                    StatusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                };
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArchiveMove",
                    Method = "[MoveTreeNode]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = model, Result = result },
                    LogString = "目錄節點移動存檔.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"目錄節點移動異常【{ex.Message}】");
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArchiveMove",
                    Method = "[MoveTreeNode]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"目錄節點移動失敗【{ex.Message}】")
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

    }
}