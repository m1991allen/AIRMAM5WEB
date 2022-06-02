using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.BatchBooking;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Material;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.Filters;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 批次調用
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class BatchBookingController : BaseController
    {
        readonly string CONTR_NAEM = "BatchBooking";
        readonly ArcVideoService _arcVideoService;
        readonly ArcAudioService _arcAudioService;
        readonly ArcPhotoService _arcPhotoService;
        readonly ArcDocService _arcDocService;
        readonly BookingTService _bookingTService;
        readonly BookingService _bookingService;

        public BatchBookingController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService, ICodeService codeService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            //
            _arcVideoService = new ArcVideoService(serilogService);
            _arcAudioService = new ArcAudioService(serilogService);
            _arcDocService = new ArcDocService(serilogService);
            _arcPhotoService = new ArcPhotoService(serilogService);
            _bookingTService = new BookingTService(serilogService, codeService);
            _bookingService = new BookingService(serilogService);
        }

        /// <summary>
        /// 首頁 //TODO 需確認 _DBLog是否正確
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return View("NoAuth");
            #region _DB LOG   
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "批次調用"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 主題檔案列表資料
        /// </summary>
        /// <param name="subjectid">主題編號 fsSUBJECT_ID </param>
        /// <param name="type">媒體的樣板類型: V, A, P, D, S</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult MediaFileList(string subjectid, string type)
        {
            var _param = new { subjectid, type };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);

            try
            {
                if (!CheckUserAuth(CONTR_NAEM))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此功能";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), type);

                string _fileUrl = string.Empty, _imgUrl = string.Empty, _subjPath = string.Empty, _filePath = string.Empty;
                switch (mediatype)
                {
                    case FileTypeEnum.V:
                        var _video = _arcVideoService.GetVideioBySubjectId(subjectid)
                            .Select(s => new BatchBookingShowListModel<spGET_ARC_VIDEO_BY_SUBJ_ID_Result>(s, mediatype));
                        result.Data = _video;
                        break;
                    case FileTypeEnum.A:
                        var _audio = _arcAudioService.GetArcAudioBySubjectId(subjectid)
                            .Select(s => new BatchBookingShowListModel<spGET_ARC_AUDIO_BY_SUBJ_ID_Result>(s, mediatype));
                        result.Data = _audio;
                        break;
                    case FileTypeEnum.P:
                        var _photo = _arcPhotoService.GetArcPhotoBySubjectId(subjectid)
                            .Select(s => new BatchBookingShowListModel<spGET_ARC_PHOTO_BY_SUBJ_ID_Result>(s, mediatype));
                        result.Data = _photo;
                        break;
                    case FileTypeEnum.D:
                        var _doc = _arcDocService.GetArcDocBySubjectId(subjectid)
                            .Select(s => new BatchBookingShowListModel<spGET_ARC_DOC_BY_SUBJ_ID_Result>(s, mediatype));
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
                    LogString = "媒資檔案列表.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 加入 確定調用燈箱
        /// </summary>
        /// <returns></returns>
        public ActionResult _Booking()
        { 
            //調用選項Model
            BookingOptionModel model = _bookingTService.GetBookingOption(null, BookingTranTypeEnum.BOOKING);
            //model.MaterialIds = string.Empty; 
            /* Tip: 批次調用不須透過"我的調用清單勾選再送出調用，此值為空值。 */

            return PartialView(model);
        }

        /// <summary>
        /// 確定調用燈箱 送出SAVE
        /// </summary>
        /// <param name="model">批次調用參數 <see cref="BatchBookingCreateModel"/> </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Booking(BatchBookingCreateModel model)
        {
            var _param = new { model, urnm = User.Identity.Name };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            #region 【檢查】
            if (!CheckUserAuth("Materia"))
            {
                result.IsSuccess = false;
                result.Message = "您無權限使用此網頁";
                result.StatusCode = HttpStatusCode.Forbidden; //403-Forbidden：禁止使用。沒有權限。
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            if (model == null)
            {
                result.IsSuccess = false;
                result.Message = "調用資料錯誤(null)";
                result.StatusCode = HttpStatusCode.BadRequest;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            VerifyResult res = new VerifyResult();

            try
            {
                model.FileNos = model.FileNos.Replace("^", ",");
                res = _bookingService.CreateBatchBooking(model, User.Identity.Name);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "送出批次調用", _str),
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
                    Method = "[Booking]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "送出批次調用.Result"
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
                    Method = "[Booking]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "送出批次調用.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}