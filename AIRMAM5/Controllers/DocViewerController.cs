using AIRMAM5.APIServer;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.Filters;
using AIRMAM5.Models.DocViewer;
using DotnetDaddy.DocumentConfig;
using DotnetDaddy.DocumentViewer;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// DocnutViewer文件檢視器
    /// </summary>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class DocViewerController : BaseController
    {
        /// <summary>
        /// T_tbmARC_IndexService 檔案刪除資料桶主檔.Service
        /// </summary>
        readonly T_tbmARC_IndexService _TtbmARC_IndexService;

        protected string _BasePath = Config.DocViewerBasePath;

        /// <summary>
        /// 
        /// </summary>
        public DocViewerController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _TtbmARC_IndexService = new T_tbmARC_IndexService(serilogService, tblLogService);
        }

        #region Model: DocViewModel,ViewerFileModel,ViewerTempFileNameModel -->移至 AIRMAM5.Models.DocViewer
        ///// <summary>
        ///// Index Data Model
        ///// </summary>
        //public class DocViewModel : SubjFileNoModel
        //{
        //    public string ViewerScripts { get; set; } = string.Empty;
        //    public string ViewerCSS { get; set; } = string.Empty;
        //    public string ViewerID { get; set; } = string.Empty;
        //    public string ViewerObject { get; set; } = string.Empty;
        //    public string ViewerInit { get; set; } = string.Empty;
        //    public string token { get; set; } = string.Empty;

        //    /// <summary>
        //    /// 當次預覽檔名 ex: 0R48JPT0ZL4B.docx 
        //    /// </summary>
        //    public string ViewFileName { get; set; }

        //    ///// <summary>
        //    ///// 當次預覽檔路徑
        //    ///// </summary>
        //    //public string ViewerPath { get; set; } = string.Empty;

        //    /// <summary>
        //    /// 檔案類別: 正常檔案normal、刪除檔案del
        //    /// </summary>
        //    public string FileKind { get; set; } = string.Empty;
        //}

        ///// <summary>
        ///// 文件檢視(解密)檔案 參數 (比照 API: AIRMAM5.FileUpload.Models.ViewerFileModel)
        ///// </summary>
        //public class ViewerFileModel : SubjFileNoModel
        //{
        //    /// <summary>
        //    /// 使用者ID
        //    /// </summary>
        //    public string ViewUserId { get; set; }
        //    /// <summary>
        //    /// 使用者顯示名稱
        //    /// </summary>
        //    public string ViewUserName { get; set; }
        //    /// <summary>
        //    /// 區別檔案: 正常檔案normal、刪除檔案del, 預設normal
        //    /// </summary>
        //    public string Kind { get; set; } = "normal";
        //}

        ///// <summary>
        ///// 文件檢視api 回覆當次解密後臨時檔名 (比照 API: AIRMAM5.FileUpload.Models.ViewerTempFileNameModel)
        ///// </summary>
        //public class ViewerTempFileNameModel
        //{
        //    /// <summary>
        //    /// 文件檢視 解密後臨時檔名(不包含副檔名)
        //    /// </summary>
        //    public string TempFileName { get; set; } = string.Empty;
        //}
        #endregion

        /// <summary>
        /// 首頁(含範例Doc)
        /// </summary>
        /// <param name="fnm">預覽的檔案完整路徑</param>
        /// <returns></returns>
        public ActionResult Index(string fnm)
        {
            var viewer = new DocViewer
            {
                ID = "ctlDoc",
                IncludeJQuery = false,
                DebugMode = true,
                BasePath = _BasePath,//"AIRMAM5",//BasePath = "/",
                // You will need to change the base path if the MVC project is inside a folder; 
                // eg. BasePath = "TestViewer"; if the Url is such:  http://localhost:xxx/TestViewer/
                FitType = "width",
                FixedZoom = true,
                ShowHyperlinks = true // use this to enable link for doc and pdf
                // TimeOut = 30 // use only if you want to auto close document within 30 mins of inactivity
            };

            // if you want to use an older version of jQuery specify this above contructor: 
            // OldJQuery = true

            //// Get the required client side script and css
            /* DocViewModel *///ViewBag.ViewerScripts = viewer.ReferenceScripts(); // Please make sure you have copied Doconut.lic in bin folder
            /* DocViewModel *///ViewBag.ViewerCSS = viewer.ReferenceCss();         // Download link provided in trial email   
            /* DocViewModel *///ViewBag.ViewerID = viewer.ClientID;
            /* DocViewModel *///ViewBag.ViewerObject = viewer.JsObject;

            //var token = viewer.OpenDocument(Server.MapPath("~/files/Sample.doc"));   // open default document (optional)
            //var token = viewer.OpenDocument(@"D:\_Code\DocViewerEx\files\Sample.doc");
            var token = viewer.OpenDocument(string.Format($@"{fnm}"));
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new Exception(viewer.InternalError);
            }

            //// Get final Init arguments to render the viewer
            /* DocViewModel *///ViewBag.ViewerInit = viewer.GetAjaxInitArguments(token); // you may also pass an empty string
            /* DocViewModel *///ViewBag.token = token;                                   // initiate value for JS token variable.

            #region 【ViewBag改以Model】
            var m = new DocViewModel()
            {
                ViewerScripts = viewer.ReferenceScripts(),
                ViewerCSS = viewer.ReferenceCss(),
                ViewerID = viewer.ClientID,
                ViewerObject = viewer.JsObject,
                ViewerInit = viewer.GetAjaxInitArguments(token),
                token = token
            };
            #endregion
            return View(m);
        }
        
        /// <summary>
        /// 打開檔案
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult OpenFile(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("File not found");
            }

            var viewer = new DocViewer
            {
                ID = "ctlDoc",  // ID is important and required.
                DebugMode = true
                // TimeOut = 30 // use only if you want to auto close document within 30 mins of inactivity
            };

            BaseConfig config = null;

            switch (new FileInfo(name).Extension.ToUpper())
            {
                case ".DWG":
                case ".DXF":
                    config = new CadConfig { ShowColor = false, WhiteBackground = true, ShowModel = false, ShowLayouts = true, LineWidth = 1, Check3DSolid = false };
                    break;
                case ".DOC":
                case ".DOCX":
                    config = new WordConfig { ConvertPdf = true, ExtractHyperlinks = true }; // specify false if links are not needed
                    break;
                case ".TXT":
                    config = new WordConfig { PaperSize = DocPaperSize.A4 };
                    break;
                case ".EML":
                case ".MSG":
                    var emlConf = new EmailConfig { EmailEncoding = Encoding.UTF8, ConvertHtml = false };
                    emlConf.PdfConfiguration.DefaultRender = true;
                    config = emlConf;
                    break;
                case ".XLS":
                case ".XLSX":
                case ".ODS":
                    config = new ExcelConfig { SplitWorksheets = true, ShowEmptyWorkSheets = false };
                    break;
                case ".PDF":
                    config = new PdfConfig { DefaultRender = true, ExtractHyperlinks = true };// specify false if links are not needed
                    break;
                case ".PNG":
                case ".BMP":
                case ".JPG":
                case ".JPEG":
                case ".PSD":
                case ".GIF":
                    config = new ImageConfig { MaxImagePixelSize = 2000 };
                    break;
            }

            var token = viewer.OpenDocument(Server.MapPath("~/files/" + name), config);

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new Exception(viewer.InternalError);
            }

            // (optional)
            // You need to store this in session if you want to 
            // call methods like export, annotation export etc on
            // the document

            // Session[token] = viewer;

            return Content(token);
        }

        #region ((Grid項目)) marked_20191204:這裡不使用
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Grid()
        {
            // Init the main viewer object.

            var viewer = new DocViewer
            {
                ID = "ctlDoc",
                IncludeJQuery = false,
                DebugMode = false,
                BasePath = _BasePath, // "/",
                ResourcePath = "/",
                FitType = "",
                Zoom = 40,
                TimeOut = 20
            };

            // Get the required client side script and css

            ViewBag.ViewerScripts = viewer.ReferenceScripts();
            ViewBag.ViewerCSS = viewer.ReferenceCss();
            ViewBag.ViewerID = viewer.ClientID;
            ViewBag.ViewerObject = viewer.JsObject;

            // Get final Init arguments to render the viewer
            ViewBag.ViewerInit = viewer.GetAjaxInitArguments("");

            // Have three sample files in the Grid

            var filesToView = new[]
            {
                new FileInfo(Server.MapPath("~/files/Sample.doc")),
                new FileInfo(Server.MapPath("~/files/Sample.ppt")),
                new FileInfo(Server.MapPath("~/files/Sample.pdf"))
            };

            var fileToken = new Hashtable();
            var fileCount = 1;

            foreach (var file in filesToView)
            {
                var viewerDummy = new DocViewer
                {
                    ID = "viewerDummy" + fileCount,
                    IncludeJQuery = false,
                    DebugMode = false,
                    BasePath = _BasePath, //"/",
                    FitType = "",
                    Zoom = 50
                };

                // If only you can to explicitly open each document, before hand.
                var token = viewerDummy.OpenDocument(file.FullName);

                if (!string.IsNullOrWhiteSpace(token))
                {
                    fileToken.Add(file.Name, token);
                    fileCount++;
                }
            }

            return View(fileToken);
        }
        #endregion
        
        /// <summary>
        /// 列印
        /// </summary>
        /// <returns></returns>
        public ActionResult Print()
        {
            return View();
        }

        #region ((上傳檔案)) marked_20191204:這裡不使用
        /// <summary>
        /// 上傳檔案 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFile()
        {
            var isSavedSuccessfully = true;
            var fName = "";

            try
            {
                foreach (string fileName in Request.Files)
                {
                    var file = Request.Files[fileName];

                    if (null == file) continue;

                    if (file.ContentLength <= 0) continue;

                    // check for any malicious file types
                    var invalidFiles = ".EXE .JS .JAR .VBS .VB .SFX .BAT .DLL .TMP .PY .ASP .ASPX .ASHX .ASMX .AXD .PHP .MSI .COM .CMD .VBE .LNK .ZIP .RAR .7Z";
                    var fileExtension = new FileInfo(file.FileName).Extension.ToUpper();

                    if (invalidFiles.IndexOf(fileExtension, StringComparison.Ordinal) > -1)
                    {
                        throw new Exception("Invalid file extension");
                    }

                    fName = DateTime.Now.ToShortDateString().Replace("/", "-") + "--" + file.FileName;

                    foreach (char c in Path.GetInvalidFileNameChars())
                    {
                        fName = fName.Replace(c, '-');
                    }

                    // Remove special characters
                    fName = fName.Replace("%", "-").Replace("&", "-").Replace("#", "-").Replace(";", "-").Replace("+", "-").Replace(" ", "-");
                    var filePath = Server.MapPath(@"~\files") + "\\" + fName;

                    file.SaveAs(filePath);
                }

            }
            catch (Exception)
            {
                isSavedSuccessfully = false;
            }

            return Content(isSavedSuccessfully ? fName : "");
        }
        #endregion

        #region 預覽文件檔案 : Document Viewer
        /// <summary>
        /// 預覽文件內容 Document Viewer
        /// </summary>
        /// <param name="fsSUBJECT_ID"></param>
        /// <param name="fileNo"></param>
        /// <param name="kind">區別檔案: 正常檔案normal、刪除檔案del, 預設normal </param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> _PreviewDoc(string fsSUBJECT_ID, string fileNo, string kind = "normal")
        {
            var _param = new { fsSUBJECT_ID, fileNo, kind };
            DocViewModel rtnm = new DocViewModel();
            string _Url = Config.UploadUrl
                , _ApiUrl = string.Format($"{_Url}api/Upload/DecryptFiles")
                , FileExt = string.Empty/*副檔名*/;

            try
            {
                switch (kind.ToUpper())
                {
                    case "DEL":
                        /* Modified_20210830 *///var _docDel = new T_tbmARC_IndexService().GetArcDocBySubjFile(fsSUBJECT_ID, fileNo).FirstOrDefault();
                        var _docDel = _TtbmARC_IndexService.GetArcDocBySubjFile(fsSUBJECT_ID, fileNo).FirstOrDefault();
                        if (_docDel == null) { return View("NotFound"); }
                        FileExt = _docDel.fsFILE_TYPE;
                        break;
                    case "NORMAL":
                    default:
                        var _doc = new ArcDocService(_serilogService).GetArcDocByIdFile(fsSUBJECT_ID, fileNo).FirstOrDefault();
                        if (_doc == null) { return View("NotFound"); }
                        FileExt = _doc.fsFILE_TYPE;
                        break;
                }

                ViewerFileModel _agrs = new ViewerFileModel
                {
                    fsSUBJECT_ID = fsSUBJECT_ID,
                    fsFILE_NO = fileNo,
                    ViewUserId = User.Identity.GetUserId(),  //API端會再檢查此UserId是否存在。
                    ViewUserName = User.Identity.Name,
                    Kind = kind
                };

                //API
                var _apiResult = await new CallAPIService().ApiPostAsync(_ApiUrl, _agrs);
                string _str = _apiResult.IsSuccess ? string.Format($"Upload/DecryptFiles成功: {_apiResult.Message}") : string.Format($"GetFileStatusInTsm失敗: {_apiResult.Message}")
                    , _apiData = JsonConvert.SerializeObject(_apiResult.Data);
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "DocViewer",
                    Method = "[_PreviewDoc]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Params = _agrs, APIURL = _ApiUrl, FileStr = _apiResult },
                    LogString = "預覽.Result",
                });
                #endregion

                //回覆: 檔案解密後可用的 "檔名"(不包含路徑與副檔)Model 如: 0R48JPT0ZL4B
                var _TempFile = JsonConvert.DeserializeObject<ViewerTempFileNameModel>(_apiData);
                /*mdf_20200317*///string _viewUrl = string.Format(@"{0}{1}.{2}", _doc.fsDV_TEMP_FOLDER, _TempFile.TempFileName, _doc.fsFILE_TYPE);
                string _tempFolder = new ConfigService().GetConfigBy("DV_TEMP_FOLDER").FirstOrDefault().fsVALUE,
                    _viewUrl = string.Format(@"{0}{1}.{2}", _tempFolder, _TempFile.TempFileName, FileExt);

                #region DocViewer
                var viewer = new DocViewer
                {
                    //ID = _TempFile.TempFileName,//"ctlDoc",
                    ID = "ctlDoc",
                    IncludeJQuery = false,
                    DebugMode = true,
                    BasePath = _BasePath,// BasePath = "/",
                    // You will need to change the base path if the MVC project is inside a folder; 
                    // eg. BasePath = "TestViewer"; if the Url is such:  http://localhost:xxx/TestViewer/
                    FitType = "width",
                    FixedZoom = true,
                    ShowHyperlinks = true // use this to enable link for doc and pdf
                    // TimeOut = 30 // use only if you want to auto close document within 30 mins of inactivity
                };
                var token = viewer.OpenDocument(string.Format(@"{0}", _viewUrl));
                if (string.IsNullOrWhiteSpace(token)) { throw new Exception(viewer.InternalError); }
                rtnm = new DocViewModel()
                {
                    ViewerScripts = viewer.ReferenceScripts(),
                    ViewerCSS = viewer.ReferenceCss(),
                    ViewerID = viewer.ClientID,
                    ViewerObject = viewer.JsObject,
                    ViewerInit = viewer.GetAjaxInitArguments(token),
                    token = token,
                    fsSUBJECT_ID = fsSUBJECT_ID,
                    fsFILE_NO = fileNo,
                    //ViewFileName = string.Format("{0}.{1}", _TempFile.TempFileName, _doc.fsFILE_TYPE)
                    ViewFileName = string.Format("{0}.{1}", _TempFile.TempFileName, FileExt)
                    , FileKind = kind
                };
                #endregion
            }
            catch (Exception ex)
            {
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "DocViewer",
                    Method = "[_PreviewDoc]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = _param, Exception = ex },
                    LogString = "預覽.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return View("Index", rtnm);
        }
        #endregion

    }
}