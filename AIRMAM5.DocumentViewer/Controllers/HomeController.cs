using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
//using Microsoft.Ajax.Utilities;

// MISSING DLL REFERENCES !!!

using DotnetDaddy.DocumentConfig;
using DotnetDaddy.DocumentViewer;

namespace AIRMAM5.DocumentViewer.Controllers
{
    /// <summary>
    /// DocnutViewer文件檢視器
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// 首頁(含範例Doc)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var viewer = new DocViewer
            {
                ID = "ctlDoc",
                IncludeJQuery = false,
                DebugMode = true,
                BasePath = "/",
                // You will need to change the base path if the MVC project is inside a folder; 
                // eg. BasePath = "TestViewer"; if the Url is such:  http://localhost:xxx/TestViewer/
                FitType = "width",
                FixedZoom = true,
                ShowHyperlinks = true // use this to enable link for doc and pdf
                // TimeOut = 30 // use only if you want to auto close document within 30 mins of inactivity
            };

            // if you want to use an older version of jQuery specify this above contructor: 
            // OldJQuery = true

            // Get the required client side script and css

            ViewBag.ViewerScripts = viewer.ReferenceScripts(); // Please make sure you have copied Doconut.lic in bin folder
            ViewBag.ViewerCSS = viewer.ReferenceCss();         // Download link provided in trial email   
            ViewBag.ViewerID = viewer.ClientID;
            ViewBag.ViewerObject = viewer.JsObject;


            var token = viewer.OpenDocument(Server.MapPath("~/files/Sample.doc"));   // open default document (optional)

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new Exception(viewer.InternalError);
            }


            // Get final Init arguments to render the viewer

            ViewBag.ViewerInit = viewer.GetAjaxInitArguments(token); // you may also pass an empty string

            ViewBag.token = token;      // initiate value for JS token variable.

            return View();
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Grid()
        {
            // Init the main viewer object.

            var viewer = new DocViewer { ID = "ctlDoc", IncludeJQuery = false, DebugMode = false, BasePath = "/", ResourcePath = "/", FitType = "", Zoom = 40, TimeOut = 20 };

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
                var viewerDummy = new DocViewer { ID = "viewerDummy" + fileCount, IncludeJQuery = false, DebugMode = false, BasePath = "/", FitType = "", Zoom = 50 };

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
        /// <summary>
        /// 列印
        /// </summary>
        /// <returns></returns>
        public ActionResult Print()
        {
            return View();
        }

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

                    if (null == file)
                        continue;

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

    }

}