using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Common;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Procedure;
using AIRMAM5.DBEntity.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AIRMAM5.DBEntity.Models.Material;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Subject;
using AIRMAM5.DBEntity.Models.ArcPre;
using AIRMAM5.Filters;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 我的調用清單
    /// </summary>
    /// <remarks>
    /// 主頁列表-資料來源: spGET_MATERIAL_BY_MARKED_BY. 讀取 tbmMATERIAL 資料。
    /// 影片剪輯-資料來源: spGET_MATERIAL_BY_MARKED_BY.
    /// 影片剪輯-存檔   : (多筆)資料新增 tbmMATERIAL
    /// 調用檔案-送出   : spINSERT_BOOKING. 新增 (1)tbmBOOKING (2)tblWORK, 回傳 新增的資料[fnBOOKING_ID]。
    /// </remarks>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class MateriaController : BaseController
    {
        private readonly ProcedureGetService _getService;
        private readonly MaterialService _materialService;
        private readonly BookingTService _bookingTService;
        private BookingService _bookingService;

        /// <summary>
        /// TSM API URL
        /// </summary>
        private readonly static string _tsmUrl = Config.TsmUrl;//ConfigurationManager.AppSettings["fsTSM_API"].ToString();

        /// <summary>
        /// 
        /// </summary>
        public MateriaController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _getService = new ProcedureGetService();
            _materialService = new MaterialService(serilogService);
            _bookingTService = new BookingTService();
        }

        /// <summary>
        /// 首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckUserAuth("Materia")) return View("NoAuth");

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "我的調用清單"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(""),
                User.Identity.Name);
            #endregion
            return View();
        }

        /// <summary>
        /// 查詢列表
        /// </summary>
        /// <returns></returns>
        //[InterceptorOfException]
        [HttpPost]
        public async Task<ActionResult> Search()
        {
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, null);

            try
            {
                //TIPS_20200518_只顯示登入帳號自己的資料, Administrator也是看自己的資料
                var get = _materialService.GetByMarked(User.Identity.Name)
                        .Select(s => new MaterialListModel().ConvertData(s))
                        .ToList();
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "我的調用清單", "OK"),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(new { UserName = User.Identity.Name }),
                    User.Identity.Name);
                #endregion

                result.IsSuccess = true;
                result.StatusCode = HttpStatusCode.OK;
                result.Data = get;
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Materia",
                    Method = "[Search]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { RecordCount = get.Count(), Param = new { UserName = User.Identity.Name } },
                    LogString = "我的調用清單.Resutl",
                });
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
                    Controller = "Materia",
                    Method = "[Search]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = User.Identity.Name, Exception = ex },
                    LogString = "我的調用清單.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return await Task.Run(() => Json(result, JsonRequestBehavior.DenyGet));
        }

        /// <summary>
        /// 刪除調用檔案(可複選)
        /// </summary>
        /// <param name="ids">多筆id組成字串(以","為分隔符號) </param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Delete(List<long> ids)
        {
            var _param = ids; //多筆id組成字串(以","為分隔符號)
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region __檢查__
                if (!CheckUserAuth("Materia"))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (ids == null)
                {
                    result.IsSuccess = false;
                    result.Message = "調用檔案編號有誤";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                var get = _materialService.GetBy(ids);
                res = _materialService.DeleteRange(get);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M003",     //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "調用清單", _str),
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
                    Controller = "Materia",
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "刪除調用清單.Result"
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
                    Controller = "Materia",
                    Method = "[Delete]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "刪除調用清單.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 段落剪輯/粗剪
        /// </summary>
        /// <param name="id">調用編號 [fnMaterial_ID]</param>
        /// <returns></returns>
        public ActionResult _FilmEdit(long id)
        {
            if (!CheckUserAuth("Materia"))
                return RedirectToAction("NoAuthModal", "Error", new { @id = "FilmModal" });
            //
            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "我的調用清單-段落剪輯"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(new { }),
                User.Identity.Name);
            #endregion

            var model = _materialService.GetByMarked(User.Identity.Name).Where(x => x.fnMATERIAL_ID == id)
                .Select(s => new MaterialFilmModel().ConvertData(s))
                .FirstOrDefault();

            return PartialView(model);
        }

        /// <summary>
        /// 段落剪輯/粗剪 SAVE
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FilmEdit(List<MaterialCreateModel> model)
        {
            var _param = model;
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();

            try
            {
                #region 【檢查】
                if (!CheckUserAuth("Materia"))
                {
                    result.IsSuccess = false;
                    result.Message = "您無權限使用此網頁";
                    result.StatusCode = HttpStatusCode.Forbidden; //403-Forbidden：禁止使用。沒有權限。
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (model == null || model.FirstOrDefault() == null)
                {
                    result.IsSuccess = false;
                    result.Message = "剪輯資料錯誤(null)";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                List<tbmMATERIAL> _materials = model.Select(s => new tbmMATERIAL(s)
                {
                    fsMARKED_BY = User.Identity.Name,
                    fdCREATED_DATE = DateTime.Now,
                    fsCREATED_BY = User.Identity.Name
                }).ToList();
                res = _materialService.CreateRange(_materials, User.Identity.Name);
                //回覆 列表資料格式 MaterialListModel()
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "新增段落剪輯", _str),
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
                    Controller = "Materia",
                    Method = "[FilmEdit]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "新增段落剪輯.Result"
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
                    Controller = "Materia",
                    Method = "[FilmEdit]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "新增段落剪輯.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 調用檔案(選單操作view)
        /// </summary>
        /// <param name="ids">調用的編號清單</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _Booking(string ids)
        {
            if (!CheckUserAuth("Materia")) return RedirectToAction("NoAuthModal", "Error", new { @id = "CreateModal" });

            //調用選項Model
            BookingOptionModel model = _bookingTService.GetBookingOption(null, BookingTranTypeEnum.BOOKING);
            model.MaterialIds = ids;
            return PartialView(model);
        }

        /// <summary>
        /// 調用檔案 送出(寫入WORK) SAVE
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Booking(BookingCreateModel model)
        {
            var _param = new { model, urnm = User.Identity.Name };
            ResponseResultModel result = new ResponseResultModel(true, string.Empty, _param);
            VerifyResult res = new VerifyResult();
            try
            {
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
                _bookingService = new BookingService();
                model.MaterialIds = model.MaterialIds.Replace("^", ",");
                res = _bookingService.CreateBy(model, User.Identity.Name);
                string _str = res.IsSuccess ? "成功" : "失敗";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(), 
                    "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "送出調用", _str),
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
                    Controller = "Materia",
                    Method = "[Booking]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "送出調用.Result"
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
                    Controller = "Materia",
                    Method = "[Booking]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Result = res, Exception = ex },
                    LogString = "送出調用.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// (購物車)調用檔案 資料內容
        /// </summary>
        /// <param name="id">調用編號 [fnMaterial_ID] </param>
        /// <returns></returns>
        public ActionResult _Details(long id)
        {
            if (!CheckUserAuth("Materia")) return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });

            var _param = new { fnMaterial_ID = id };
            var _get = _materialService.GetById(id);
            string fileNO = _get.fsFILE_NO;
            FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), _get.fsTYPE);

            /* modified_20210901 */
            //MediaDetailsCommonModel model = new MediaDetailsCommonModel().GetMediaDetailsData(_get.fsFILE_NO, mediatype);
            //if (string.IsNullOrEmpty(model.Viewer.ImageUrl)) {
            //    FileTypeEnum type = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), model.BasicMeta.FileCategory);
            //    switch (type)
            //    {
            //        case FileTypeEnum.V:
            //            model.Viewer.ImageUrl = Url.Content("~/Images/videopreview.png");
            //            break;
            //        case FileTypeEnum.A:
            //            model.Viewer.ImageUrl = Url.Content("~/Images/audiopreview.png");
            //            break;
            //        case FileTypeEnum.D:
            //            model.Viewer.ImageUrl = Url.Content("~/Images/docpreview.png");
            //            break;
            //        case FileTypeEnum.P:
            //            model.Viewer.ImageUrl = Url.Content("~/Images/imagepreview.png");
            //            break;
            //        case FileTypeEnum.S:
            //            break;
            //    }
            //} // ↓↓↓
            MediaDetailsCommonModel model = new MediaDetailsCommonModel();
            string emptyImageUrl = string.Empty;
            switch (mediatype)
            {
                case FileTypeEnum.V:
                    var _arcVideoSer = new ArcVideoService(_serilogService);
                    var _v = _arcVideoSer.GetVideoBySubjectFile(string.Empty, fileNO).FirstOrDefault();
                    model = model.ConvertModelData(_v, mediatype);
                    emptyImageUrl = Url.Content("~/Images/videopreview.png");

                    model.KeyFrame = _arcVideoSer.GetKeyFrame(fileNO).Select(s => new VideoKeyFrameModel().FormatConvert(s, mediatype)).ToList();
                    model.FileSeqment = _arcVideoSer.GetVideoSeqment(fileNO).Select(s => new SubjectFileSeqmentModel().FormatConvert(s, mediatype)).ToList();
                    model.DynamicInfo = _arcVideoSer.GetVideoAttrByFile(fileNO).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();
                    break;
                case FileTypeEnum.A:
                    var _arcAudioSer = new ArcAudioService(_serilogService);
                    var _a = _arcAudioSer.GetArcAudioByIdFile(string.Empty, fileNO).FirstOrDefault();

                    model = model.ConvertModelData(_a, mediatype); //包含  >>>聲音資訊+專輯資訊
                    emptyImageUrl = Url.Content("~/Images/audiopreview.png");

                    model.FileSeqment = _arcAudioSer.GetAudioSeqment(fileNO).Select(s => new SubjectFileSeqmentModel().FormatConvert(s, mediatype)).ToList();
                    model.DynamicInfo = _arcAudioSer.GetAudioAttrByFile(fileNO).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();

                    break;
                case FileTypeEnum.P:
                    var _arcPhotoSer = new ArcPhotoService(_serilogService);
                    var _p = _arcPhotoSer.GetArcPhotoByIdFile(string.Empty, fileNO).FirstOrDefault();

                    model = model.ConvertModelData(_p, mediatype); //包含 >>>圖片資訊+EXIF資訊
                    emptyImageUrl = Url.Content("~/Images/imagepreview.png");

                    model.DynamicInfo = _arcPhotoSer.GetPhotoAttrByFile(fileNO).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();

                    break;
                case FileTypeEnum.D:
                    var _arcDocSer = new ArcDocService(_serilogService);
                    var _d = _arcDocSer.GetArcDocByIdFile(string.Empty, fileNO).FirstOrDefault();

                    model = model.ConvertModelData(_d, mediatype); //包含  >>>文件額外資訊
                    emptyImageUrl = Url.Content("~/Images/docpreview.png");

                    model.DynamicInfo = _arcDocSer.GetDocAttrByFile(fileNO).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();

                    break;
                case FileTypeEnum.S:
                    //
                    break;
            }

            if (string.IsNullOrEmpty(model.Viewer.ImageUrl))
            {
                model.Viewer.ImageUrl = emptyImageUrl;
            }

            #region _DB LOG
                _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "M004",     //[@USER_ID(@USER_NAME)] 檢視 [@DATA_TYPE] 資料 @RESULT
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "我的調用清單", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(_param),
                User.Identity.Name);
            #endregion
            return PartialView("_Details", model);
        }

    }
}