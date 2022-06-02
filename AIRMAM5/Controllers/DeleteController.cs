using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Enums;
using System.Net;
using AIRMAM5.Filters;
using AIRMAM5.HubServer;
using AIRMAM5.DBEntity.Models.AcrDelete;
using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Subject;
using AIRMAM5.DBEntity.Models.ArcPre;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 媒資管理 > 刪除紀錄管理
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class DeleteController : BaseController
    {
        /// <summary>
        /// T_tbmARC_IndexService 檔案刪除資料桶主檔.Service
        /// </summary>
        readonly T_tbmARC_IndexService _TbmARC_IndexService;

        public DeleteController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _TbmARC_IndexService = new T_tbmARC_IndexService(serilogService, tblLogService);
        }

        /// <summary>
        /// 刪除紀錄管理首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if(!CheckUserAuth("Delete")) return View("NoAuth");
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "刪除紀錄管理"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
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
            DeleteDateSearchModel m = new DeleteDateSearchModel(3);

            return PartialView("_Search", m);
        }

        /// <summary>
        /// 查詢功能
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(SP_TGetARCIndexByCondition param)
        {
            var _param = param;
            bool IsTempDelete = param.Status == "-1" ? true : false;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);

            if (!CheckUserAuth("Delete"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            //-1暫刪除選項
            param.Status = (param.Status == "*" || param.Status=="-1" || param.Status == null) ? string.Empty : param.Status;
            param.Type = (param.Type == "*" || param.Type == null) ? string.Empty : param.Type;
            var get = _TbmARC_IndexService.GetARCIndexByCondition(param);

            //狀態=暫刪除, [fsSTATUS]=" ".
            if (IsTempDelete) get = get.Where(x => x.fsSTATUS.Trim() == string.Empty).ToList();

            result.IsSuccess = true;
            result.Data = get.OrderByDescending(s => s.fdCREATED_DATE).ToList();
            result.StatusCode = HttpStatusCode.OK;

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M005",  //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "刪除紀錄", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(_param),
                User.Identity.Name);
            #endregion
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 刪除紀錄管理-資料內容
        /// </summary>
        /// <param name="id">fnINDEX </param>
        /// <returns></returns>
        public ActionResult _Details(int id)
        {
            if (!CheckUserAuth("Delete"))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });

            var param = new SP_TGetARCIndexByCondition
            {
                StartDate = string.Empty,
                EndDate = string.Empty,
                Status = string.Empty,
                Type = string.Empty,
                IndexId = id
            };

            //DeleteArcViewModel model = new DeleteArcViewModel().GetDetailsData(param);
            /* ↑↓ modified_20210831 */// sp_t_GET_ARC_INDEX_BY_CONDITION_Result
            var get = _TbmARC_IndexService.GetARCIndexByCondition(param).FirstOrDefault();
            FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), get.fsTYPE);

            /*modified_20211005*///DeleteArcViewModel model = new DeleteArcViewModel().DataConvert(get, mediatype, _TbmARC_IndexService);
            DeleteArcViewModel model = new DeleteArcViewModel();

            switch (mediatype)
            {
                case FileTypeEnum.V:
                    var _v = _TbmARC_IndexService.GetArcVideoById(get.fnINDEX_ID);//_TbmARC_IndexService.GetArcVideoById(model.IndexId);
                    //model.Viewer = new ShowViewerModel().FormatConversion(_v, mediatype);
                    //model.BasicMeta = new ArcBasicMetaModel().FormatConversion(_v, mediatype);
                    model = new DeleteArcViewModel<sp_t_GET_ARC_INDEX_BY_CONDITION_Result, sp_t_GET_ARC_VIDEO_Result>(get, mediatype, _v);
                    model.KeyFrame = _TbmARC_IndexService.GetArcVideoKeyById(model.IndexId).Select(s => new VideoKeyFrameModel().FormatConvert(s, mediatype)).ToList();
                    model.FileSeqment = _TbmARC_IndexService.GetArcVideoDescrById(model.IndexId).Select(s => new SubjectFileSeqmentModel().FormatConvert(s, mediatype)).ToList();
                    model.DynamicInfo = _TbmARC_IndexService.GetArcVideoAttributeById(model.fsFILE_NO).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();
                    break;
                case FileTypeEnum.A:
                    var _a = _TbmARC_IndexService.GetArcAudioById(get.fnINDEX_ID);//arcIndexSer.GetArcAudioById(this.IndexId);
                    //this.Viewer = new ShowViewerModel().FormatConversion(_a, mediatype);
                    //this.BasicMeta = new ArcBasicMetaModel().FormatConversion(_a, mediatype);
                    model = new DeleteArcViewModel<sp_t_GET_ARC_INDEX_BY_CONDITION_Result, sp_t_GET_ARC_AUDIO_Result>(get, mediatype, _a)
                    {
                        FileSeqment = _TbmARC_IndexService.GetArcAudioDescrById(get.fnINDEX_ID).Select(s => new SubjectFileSeqmentModel().FormatConvert(s, mediatype)).ToList(),
                        DynamicInfo = _TbmARC_IndexService.GetArcAudioAttributeById(get.fsFILE_NO).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList()
                    };

                    // >>>>> 聲音資訊+專輯資訊: model已設置。
                    break;
                case FileTypeEnum.P:
                    var _p = _TbmARC_IndexService.GetArcPhotoById(get.fnINDEX_ID);
                    //this.Viewer = new ShowViewerModel().FormatConversion(_p, mediatype);
                    //this.BasicMeta = new ArcBasicMetaModel().FormatConversion(_p, mediatype);
                    model = new DeleteArcViewModel<sp_t_GET_ARC_INDEX_BY_CONDITION_Result, sp_t_GET_ARC_PHOTO_Result>(get, mediatype, _p)
                    {
                        DynamicInfo = _TbmARC_IndexService.GetArcPhotoAttributeById(get.fsFILE_NO).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList()
                    };

                    // >>>>> 圖片資訊+EXIF資訊: model已設置。
                    break;
                case FileTypeEnum.D:
                    var _d = _TbmARC_IndexService.GetArcDocById(get.fnINDEX_ID);
                    //this.Viewer = new ShowViewerModel().FormatConversion(_d, mediatype);
                    //this.BasicMeta = new ArcBasicMetaModel().FormatConversion(_d, mediatype);
                    model = new DeleteArcViewModel<sp_t_GET_ARC_INDEX_BY_CONDITION_Result, sp_t_GET_ARC_DOC_Result>(get, mediatype, _d)
                    {
                        DynamicInfo = _TbmARC_IndexService.GetArcDocAttributeById(get.fsFILE_NO).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList()
                    };

                    // >>>>> 文件額外資訊: model已設置。
                    break;
                default:
                    //
                    break;
            }

            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(), 
                "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "刪除紀錄管理", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { fnIndex_ID = id }),
                User.Identity.Name);
            #endregion
            return PartialView("_Details", model);
        }

        /// <summary>
        /// 刪除Modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Delete(long id)
        {
            if (!CheckUserAuth("Delete")) return RedirectToAction("NoAuthModal", "Error", new { @id = "DeleteModal" });

            var get = _TbmARC_IndexService.GetById(id);
            var _param = new SP_TGetARCIndexByCondition(
                string.Format("{0:yyyy/MM/dd}", get.fdCREATED_DATE.AddDays(-1)),
                string.Format("{0:yyyy/MM/dd}", get.fdCREATED_DATE.AddDays(1)),
                get.fsSTATUS, get.fsTYPE, id);

            //var result = _TbmARC_IndexService.GetARCIndexByCondition(_param).FirstOrDefault();
            /*20200925_調整_減少非必要資料傳遞 */
            var result = _TbmARC_IndexService.GetARCIndexByCondition(_param)
                        .Select(s => new DeleteConfirmViewModel().DataConvert(s))
                        .FirstOrDefault();

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "刪除紀錄管理-刪除檢視"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { SubjectId = id }),
                User.Identity.Name);
            #endregion
            return PartialView("_Delete", result);
        }

        /// <summary>
        /// 還原 操作View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult _Redo(long id)
        {
            if (!CheckUserAuth("Delete")) return RedirectToAction("NoAuthModal", "Error", new { @id = "RedoModal" });

            var get = _TbmARC_IndexService.GetById(id);
            var _param = new SP_TGetARCIndexByCondition(
                string.Format("{0:yyyy/MM/dd}", get.fdCREATED_DATE.AddDays(-1)),
                string.Format("{0:yyyy/MM/dd}", get.fdCREATED_DATE.AddDays(1)),
                get.fsSTATUS, get.fsTYPE, id);

            //var result = _TbmARC_IndexService.GetARCIndexByCondition(_param).FirstOrDefault();
            /*20200925_調整_減少非必要資料傳遞 */
            var result = _TbmARC_IndexService.GetARCIndexByCondition(_param)
                        .Select(s => new DeleteConfirmViewModel().DataConvert(s) )
                        .FirstOrDefault();

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "刪除紀錄管理-還原操作"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { SubjectId = id }),
                User.Identity.Name);
            #endregion
            return PartialView("_Redo", result);
        }

        /// <summary>
        /// 刪除 實體檔案
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fsTYPE"></param>
        /// <param name="fsFILE_NO"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(long id, string type, string fileno)
        {
            var _param = new { id, type, fileno };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();
            #region _檢查_
            if (!CheckUserAuth("Delete"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            try
            {
                var p = new SP_TDeleteARC
                {
                    IndexId = id,
                    Type = type,
                    FileNo = fileno,
                    UseNameBy = User.Identity.Name
                };
                res = _TbmARC_IndexService.DeleteARC(p);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M003",        //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "刪除記錄實體檔案", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(p),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed //417 - 執行失敗。
                };
                #region Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Delete",
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "刪除實體檔案.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Delete",
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Result = res, Exception = ex },
                    LogString = "刪除實體檔案.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 回復媒體檔案
        /// <para> 2020-06-04 : 回復刪除的媒資檔案,會影響DashBoard的入庫統計值與圖表的統計數值,加入更新處理(SignalR) </para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Redo(long id)
        {
            var _param = new { id };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();
            #region _檢查_
            if (!CheckUserAuth("Delete"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            try
            {
                var param = new SP_TRestoreARC
                {
                    IndexId = id,
                    UseNameBy = User.Identity.Name
                };
                res = _TbmARC_IndexService.RestoreARC(param);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M010",        //[@USER_ID(@USER_NAME)] 還原 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "刪除記錄媒體檔案", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(param),
                    User.Identity.Name);
                #endregion

                result = new ResponseResultModel(res)
                {
                    Records = _param,
                    StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed //417 - 執行失敗。
                };
                #region Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Delete",
                    Method = "[Redo]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = res },
                    LogString = "回復媒體檔案.Result"
                });
                #endregion

                //refresh dashboard
                //new BroadcastHub().RefreshCountsOfDashBoard(CurrentUser.Id);
                new BroadcastHub2().RefreshCountsOfDashBoard(CurrentUser.Id);
            }
            catch(Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Delete",
                    Method = "[Redo]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _param, Exception = ex },
                    LogString = "回復媒體檔案.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}
