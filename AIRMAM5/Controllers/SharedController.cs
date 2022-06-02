using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.Utility.Common;
using AIRMAM5.DBEntity.Models.Function;
using AIRMAM5.DBEntity.Models.Material;
using AIRMAM5.DBEntity.Models.Search;
using AIRMAM5.DBEntity.Models.Template;
using AIRMAM5.DBEntity.Interface;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 共用
    /// </summary>
    public class SharedController : Controller
    {
        readonly ISerilogService _serilogService;
        readonly IFunctionsService _functionsService;
        private readonly TemplateService _templateService;
        private readonly ICodeService _codeService;
        private readonly BookingTService _bookingTService;
        private readonly ArcPreService _arcPreService;
        private readonly ConfigService _configService;

        public SharedController(ISerilogService serilogService, IFunctionsService functionService, ICodeService codeService)
        {
            _serilogService = serilogService;
            _functionsService = functionService;
            _arcPreService = new ArcPreService(serilogService);
            _templateService = new TemplateService();
            _codeService = codeService;
            _bookingTService = new BookingTService();
            _configService = new ConfigService();
        }

        public ActionResult NoAuth()
        {
            return View();
        }

        #region _LoginPartial:【檢索選單】
        /// <summary>
        /// LoginPartialModel : 檢索選單資料: 排除"主題"項目
        /// </summary>
        /// <remarks> 20200108_MDF: 修改回傳前資料model. </remarks>
        /// <returns></returns>
        public ActionResult SearchParamData()
        {
            LoginPartialModel model = new LoginPartialModel();

            #region 【檢索選單】
            SearchColumnViewModel _search = new SearchColumnViewModel();
            _search.SearchType.RemoveAt(4);  //不顯示"主題 Subject_DEV"項目
            _search.SearchTemplate = _templateService.GetByParam()  //不顯示"主題 S" 樣板
                .Where(x => x.fsTABLE.IndexOf(FileTypeEnum.S.ToString()) < 0 && x.fcIS_SEARCH == IsTrueFalseEnum.Y.ToString())
                .Select(s => new TemplateBaseModel().DataConvert(s)).ToList();

            //檢索時,最多可使用幾個欄位查詢
            var get = _configService.GetConfigBy("SEARCH_MAX_COLUMN").FirstOrDefault();
            if (get != null)
            {
                int.TryParse(get.fsVALUE, out int _val);
                _search.SearchMaxColumn = _val;
            }
            model.SearchColumn = _search;
            #endregion

            #region 【使用者 登入資訊】
            UsersService _usersService = new UsersService(_serilogService);
            model.UserInfo = _usersService.GetUserInfo();
            var _kies = CommonSecurity.CookieDecrypt(Request.Cookies["User"].Value).Split(new char[] { '-' });
            long.TryParse(_kies[6], out long _loginLogid);
            model.UserInfo.LoginLogid = _loginLogid;
            #endregion

            #region 【UI上方右側常駐之功能項目】
            //FunctionsService _funcser = new FunctionsService();
            //model.QuickMenu = _funcser.GetQuickMenu().Select(s => new UserFavoriteModel /* Modified_20210830 DI */
            model.QuickMenu = _functionsService.GetQuickMenu().Select(s => new UserFavoriteModel
            {
                FuncId = s.fsFUNC_ID,
                FunctionName = s.fsNAME,
                ControllerName = s.fsCONTROLLER,
                ActionName = s.fsACTION,
                FavoriteUrl = string.Format($"/{s.fsCONTROLLER}/{s.fsACTION}"),
                Icon = s.fsICON ?? string.Empty
            }).ToList();
            #endregion

            SerilogService _serilogSer = new SerilogService();
            #region _Serilog
            _serilogSer.SerilogWriter(new SerilogInputModel
            {
                Controller = "Shared",
                Method = "[SearchParamData]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { Result = model },
                LogString = "Result_LoginPartial"
            });
            #endregion

            return PartialView("_LoginPartial", model);
        }

        /// <summary>
        /// 檢索:樣板類型選單: V,A,P,D,S。 
        ///     *多樣板選單, [table]可以分隔符號組成字串傳入,例: V,A,S 或 V;A;P;D;S
        /// </summary>
        /// <param name="type">類型: V,A,P,D,S </param>
        /// <returns></returns>
        public JsonResult GetTemplateList(string type)
        {
            List<SelectListItem> _template = _templateService.GetTemplateList(type, true);

            return Json(_template, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 檢索:樣板自訂欄位選單
        /// </summary>
        /// <param name="tempid">樣板ID </param>
        /// <param name="search">是否可進階搜尋(預設null 不指定條件)</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAttriFieldList(int tempid, bool? search = null)
        {
            var get = _templateService.GetTemplateFieldsById(tempid);
            List<SelectListItem> result = get
                .Where(x => search == null ? true : x.fsIS_SEARCH == (search == true ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString()))
                .Select(s => new SelectListItem
                {
                    Value = s.fsFIELD,
                    Text = s.fsFIELD_NAME
                }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 指定主表CodeID 取明細代碼列表 (前端使用API)
        /// </summary>
        /// <param name="codeid">主代碼</param>
        /// <param name="isenabled">是否啟用 </param>
        /// <param name="showcode">預設顯示"fsCODE fsNAEM"、True顯示"fsCODE fsNAEM"、False顯示"fsNAME" </param>
        /// <returns></returns>
        public JsonResult GetCodeItemList(string codeid, bool? isenabled = null, bool? showcode = null)
        {
            var get = _codeService.GetCodeItemList(codeid, isenabled, showcode ?? true);

            return Json(get, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 調用選項Model
        /// </summary>
        /// <param name="id">調用樣板id dbo.[tbmBOOKING_T].[fnID] </param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBookingOption(int id)
        {
            BookingOptionModel get = _bookingTService.GetBookingOption(id);
            //if (get.WatermarkList.Any()) {
            //    get.WatermarkList.Add(new SelectListItem { Value="00",  Text="無", Selected=true });
            //}
            return Json(get, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 依分類(fsTYPE)+自訂欄位樣版編號(fnTEMP_ID) 取 預編詮釋資料清單
        /// </summary>
        /// <param name="type">類別: 主題S、影V、音A、圖P、文D </param>
        /// <param name="template">樣板Id(選填)</param>
        /// <param name="noopts">內容是否有"無"的選單項目 預設:false </param>
        /// <returns></returns>
        public JsonResult GetArcPreList(string type, long template = 0, bool noopts = false)
        {
            var get = _arcPreService.GetArcPreListBy(type, template, noopts);

            return Json(get, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取 上傳檔案類型可接受的副檔名清單 (fsCODE_ID = 'Upload_MediaType' 的子代碼)
        /// </summary>
        /// <param name="type">上傳檔案媒體類型 </param>
        /// <returns></returns>
        public JsonResult GetMediaTypeExt(string type)
        {
            string[] result = new string[] { };

            var get = _configService.GetConfigBy(type).FirstOrDefault();
            if (get != null)
            {
                result = get.fsVALUE.Split(new char[] { ';' });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
