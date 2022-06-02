using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.ArcPre;
using AIRMAM5.DBEntity.Models.Booking;
using AIRMAM5.DBEntity.Models.Common;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Subject;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.Filters;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 審核調用
    /// <para> 開放功能使用 就能使用. 不指定特定角色群組。</para>
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class VerifyBookingController : BaseController
    {
        readonly string CONTR_NAEM = "VerifyBooking";
        readonly UsersService _usersService;
        readonly TblWorkService _tblWorkService;

        public VerifyBookingController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService, ICodeService codeService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _tbzCodeService = codeService;
            //
            _usersService = new UsersService(serilogService);
            _tblWorkService = new TblWorkService();
        }

        /// <summary>
        /// 首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return View("NoAuth");

            #region _DB LOG
            _tblLogService.Insert_L_Log(
                TbzCodeIdEnum.MSG001.ToString(),
                "S003",     //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "審核調用"),
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
            //TIP_20200924: 審核狀態不需提供"無須審核"狀態選項(預存程序只會讀_A,_C,_R)
            var approveState = _tbzCodeService.GetCodeItemList(TbzCodeIdEnum.WORK_APPROVE.ToString(), true, false, true);
            var _remove = approveState.Find(x => x.Value == WorkApproveEnum._N.ToString());
            approveState.Remove(_remove);

            VerifyDateSerarchModel model = new VerifyDateSerarchModel(30)
            {
                ApproveStatusList = approveState,
                //有開放使用，就是顯示全部使用者帳號資料
                LoginIdList = _usersService.GetUsersList(User.Identity.GetUserId(), true), //下拉選單會有"全部"項目
                ApproveStatus = string.Empty,
                UserId = string.Empty
            };

            return PartialView("_Search", model);
        }

        /// <summary>
        ///  查詢審核列表
        /// </summary>
        /// <param name="VerifyStatus">審核狀態</param>
        /// <param name="LoginId">帳號關鍵字</param>
        /// <param name="Name">名稱關鍵字</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(VerifyDateSerarchModel model)
        {
            var _param = model;
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

                model.ApproveStatus = (model.ApproveStatus == "*" || string.IsNullOrEmpty(model.ApproveStatus)) ? string.Empty : model.ApproveStatus;
                model.UserId = (model.UserId == "*" || string.IsNullOrEmpty(model.UserId)) ? string.Empty : _usersService.GetById(model.UserId).fsLOGIN_ID;
                model.WorkIds = model.WorkIds ?? Array.Empty<int>();
                var get = _tblWorkService.GetLWorkBookingApprove(model);

                #region//Tips: 可操作功能者,不限制帳號清單資料。
                ////非"系統管理員"帳號,開啟此頁面,只顯示登入帳號的資料。
                //if (_usersService.CurrentUserIsAdmin)
                //{
                //    string _bkur = string.IsNullOrEmpty(model.UserId) ? string.Empty : _usersService.GetBy(model.UserId).FirstOrDefault().fsLOGIN_ID;
                //    result.Data = get
                //        .Where(x=> string.IsNullOrEmpty(model.UserId) ? true :  x.fsCREATED_BY == _bkur)
                //        .Select(s => new VerifyBookingListModel(s)).OrderByDescending(r => r.BookingDate).ToList();
                //}
                //else
                //{
                //    result.Data = get.Where(x => x.fsCREATED_BY == CurrentUser.UserName)
                //        .Select(s => new VerifyBookingListModel(s)).OrderByDescending(r => r.BookingDate).ToList();
                //}
                #endregion

                result.Data = get.Select(s => new VerifyBookingListModel().DataConvert(s))
                                    .OrderByDescending(r => r.BookingDate).ToList();
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M005",     //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "審核調用", "OK"),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = (ex.InnerException != null) ? ex.InnerException.InnerException.Message : ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Search]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Exception = ex },
                    LogString = "審核列表.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 將審核資料標註為已過審或不通過審核
        /// </summary>
        /// <param name="VerifyId">多個審核Id = 轉檔工作Id [fnWORK_ID] </param>
        /// <param name="IsPass">是否通過</param>
        /// <param name="Reason">原因</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Verify(int[] VerifyIds, bool IsPass, string Reason)
        {
            var _param = new { VerifyIds, IsPass, Reason };
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

                res = _tblWorkService.BookingConfirmApprove(VerifyIds, IsPass, Reason, CurrentUser.UserName);
                string _str = res.IsSuccess ? "成功" : "失敗";
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "M021",     //[@USER_ID(@USER_NAME)] 執行 [@DATA_TYPE] @RESULT
                    string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "審核調用", _str),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(_param),
                    User.Identity.Name);
                #endregion

                result.IsSuccess = res.IsSuccess;
                result.Message = string.Format($"借調審核 {_str}");
                result.Data = res.Data;
                result.StatusCode = HttpStatusCode.OK;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Verify]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _param, Result = result },
                    LogString = "審核調用處理.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = (ex.InnerException != null) ? ex.InnerException.InnerException.Message : ex.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = CONTR_NAEM,
                    Method = "[Verify]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = _param, Result = res, Exception = ex },
                    LogString = "借調審核.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 審核不通過頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult _Delete()
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "DeleteModal" });

            return PartialView("_Delete");
        }

        /// <summary>
        /// 審核資料詳細頁面
        /// </summary>
        /// <param name="id">審核Id => 轉檔工作Id [fnWORK_ID] </param>
        /// <returns></returns>
        public ActionResult _Details(int id)
        {
            if (!CheckUserAuth(CONTR_NAEM)) return RedirectToAction("NoAuthModal", "Error", new { @id = "DetailModal" });

            var _param = new { fnWORK_ID = id };
            var _get = _tblWorkService.GetById(id);
            string fileNO = _get.C_ITEM_ID;
            FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), _get.C_ARC_TYPE);

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
                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "審核調用", "OK"),
                string.Format($"位置: {Request.UserHostAddress} "),
                JsonConvert.SerializeObject(_param),
                User.Identity.Name);
            #endregion
            return PartialView("_Details", model);
        }

    }
}