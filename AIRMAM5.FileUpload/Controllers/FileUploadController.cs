// ============================================= 
// 描述: FileUploadController 
// 記錄: <2019/08/20><David.Sin><新增本程式> 
// ============================================= 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.IO;
using AIRMAM5.FileUpload.Common;
using AIRMAM5.FileUpload.Models;
using AIRMAM5.FileUpload.Repositorys;
using TagLib;

namespace AIRMAM5.FileUpload.Controllers
{
    public class FileUploadController : Controller
    {
        [HttpPost]
        public void UploadFile()
        {
            var queryString = Request.Form;
            if (queryString.Count == 0) return;


            // Read parameters
            var uploadToken = queryString.Get("upload_Token");
            int resumableChunkNumber = int.Parse(queryString.Get("resumableChunkNumber"));
            var resumableFilename = queryString.Get("resumableFilename");
            var resumableIdentifier = queryString.Get("resumableIdentifier");
            int resumableChunkSize = int.Parse(queryString.Get("resumableChunkSize"));
            double resumableTotalSize = double.Parse(queryString.Get("resumableTotalSize"));

            //檔案類型(MEDIATYPE_TO_V、MEDIATYPE_TO_A、MEDIATYPE_TO_P、MEDIATYPE_TO_D)
            string fsTYPE = Request.QueryString[0];
            //主題編號
            string fsSUBJ_ID = Request.QueryString[1];
            //上傳者
            string fsLOGIN_ID = Request.QueryString[2];
            //標題定義(1.檔名為標題、2.主題標題為標題、3.自訂標題)
            string fsTITLE_DEFINE = Request.QueryString[3];
            //標題(若為自訂標題)
            string fsTITLE = Request.QueryString[4];
            //被置換的檔案編號(新上傳不用寫值)
            string fsFILE_NO = Request.QueryString[5];
            //string fsSECURITY_KEY = Request.QueryString[6];
            //上傳站存資料夾名稱
            string fsFOLDER = Request.QueryString[6];
            //預編詮釋資料編號，不用預編傳0
            string fnPRE_ID = Request.QueryString[7];
            //置換後是否要刪除關鍵影格
            string fbDELETE_KF = Request.QueryString[8];

            //定義變數
            //暫存路徑
            string fsTEMP_FOLDER = new repCONFIG().fnGET_CFG("UPLOAD_FILE_PATH")[0].fsVALUE + fsLOGIN_ID + @"\" + fsSUBJ_ID + @"\" + fsFOLDER;
            //目標路徑
            string fsTARGET_FOLDER = new repCONFIG().fnGET_CFG("UPLOAD_FILE_PATH")[0].fsVALUE + fsLOGIN_ID;
            //暫存檔案
            string fsCHUNK_FILE = string.Format(@"{0}\{1}.part{2}", fsTEMP_FOLDER, resumableFilename, resumableChunkNumber.ToString("0000"));
            //目標檔案
            string fsTARGET_FILE = string.Format(@"{0}\{1}", fsTARGET_FOLDER, resumableFilename);

            if (!System.IO.Directory.Exists(fsTEMP_FOLDER)) System.IO.Directory.CreateDirectory(fsTEMP_FOLDER);
            if (!System.IO.Directory.Exists(fsTARGET_FOLDER)) System.IO.Directory.CreateDirectory(fsTARGET_FOLDER);

            if (Request.Files.Count == 1)
            {
                try
                {
                    // 儲存暫存檔案
                    if (!Directory.Exists(fsTEMP_FOLDER))
                        Directory.CreateDirectory(fsTEMP_FOLDER);

                    if (!System.IO.File.Exists(fsCHUNK_FILE))
                    {
                        Request.Files[0].SaveAs(fsCHUNK_FILE);
                    }

                    var fsCHUNK_FILEs = System.IO.Directory.GetFiles(fsTEMP_FOLDER);

                    //檢查暫存檔是否都完成了
                    if ((fsCHUNK_FILEs.Length + 1) * (long)resumableChunkSize >= resumableTotalSize)
                    {
                        if (System.IO.File.Exists(fsTARGET_FILE)) System.IO.File.Delete(fsTARGET_FILE);

                        //開始合併檔案
                        using (var fs = new FileStream(fsTARGET_FILE, FileMode.CreateNew))
                        {
                            foreach (string file in fsCHUNK_FILEs.OrderBy(x => x))
                            {
                                var buffer = System.IO.File.ReadAllBytes(file);
                                fs.Write(buffer, 0, buffer.Length);
                                System.IO.File.Delete(file);
                            }
                        }

                        //判斷是否為置換
                        if (string.IsNullOrEmpty(fsFILE_NO))
                        {
                            //新上傳的
                            //取得檔案編號
                            fsFILE_NO = new repL_NO().fnGET_L_NO("ARC", "媒體資料檔", DateTime.Now.ToString("yyyyMMdd"), "_", 7, fsLOGIN_ID);

                            //根據使用者選擇決定標題從哪裡來(若為3，則直接使用傳進來的標題)
                            if (fsTITLE_DEFINE == "1")
                                fsTITLE = Path.GetFileNameWithoutExtension(fsTARGET_FILE);
                            else if (fsTITLE_DEFINE == "2")
                                fsTITLE = new repSUBJ().fnGET_SUBJ(fsSUBJ_ID)[0].fsTITLE;


                            string fsFILE_NAME = Path.GetFileName(fsTARGET_FILE);
                            string fsEXT = Path.GetExtension(fsTARGET_FILE).Replace(".", "");

                            string fsFILE_TYPE = fsEXT;
                            string fsFILE_TYPE_H = string.Empty;
                            string fsFILE_TYPE_L = string.Empty;

                            string fsFILE_SIZE = (resumableTotalSize >= 1024 * 1024 * 1024 ? (Math.Round(resumableTotalSize / 1024 / 1024 / 1024, 2).ToString() + "GB") : (resumableTotalSize >= 1024 * 1024 ? (Math.Round(resumableTotalSize / 1024 / 1024, 2).ToString() + "MB") : (resumableTotalSize >= 1024 ? (Math.Round(resumableTotalSize / 1024, 2).ToString() + "KB") : resumableTotalSize.ToString())));
                            string fsFILE_SIZE_H = resumableTotalSize.ToString();
                            string fsFILE_SIZE_L = string.Empty;

                            string fsFILE_PATH = string.Empty;
                            string fsFILE_PATH_H = string.Empty;
                            string fsFILE_PATH_L = string.Empty;

                            string fxMEDIA_INFO = string.Empty;

                            if (fsTYPE == "MEDIATYPE_TO_V")
                            {
                                //組成完整路徑
                                fsFILE_PATH = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_V_H")[0].fsVALUE + DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString("00") + @"\" + DateTime.Now.Day.ToString("00") + @"\";
                                fsFILE_PATH_H = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_V_H")[0].fsVALUE + DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString("00") + @"\" + DateTime.Now.Day.ToString("00") + @"\";
                                fsFILE_PATH_L = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_V_L")[0].fsVALUE + DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString("00") + @"\" + DateTime.Now.Day.ToString("00") + @"\";

                                //搬至高解區
                                if (!System.IO.Directory.Exists(fsFILE_PATH_H)) System.IO.Directory.CreateDirectory(fsFILE_PATH_H);
                                if (System.IO.File.Exists(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT)) System.IO.File.Delete(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);
                                System.IO.File.Move(fsTARGET_FILE, fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                //抓取MediaInfo
                                CommonLib.Global.SetEXE_MEDIAINFO_PATH(Properties.Settings.Default.fsMEDIA_INFO);
                                CommonLib.Global.SetWORKING_TEMP_DIR(Properties.Settings.Default.fsTEMP_PATH);
                                CommonLib.Class.Media.VideoFile v = new CommonLib.Class.Media.VideoFile(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                fxMEDIA_INFO = Common.clsMEDIA_INFO.fnGET_FILE_MEDIA_INFO(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                //新增至資料庫
                                clsARC_VIDEO clsARC_VIDEO = new clsARC_VIDEO();
                                clsARC_VIDEO.fsFILE_NO = fsFILE_NO;
                                clsARC_VIDEO.fsTITLE = fsTITLE;
                                clsARC_VIDEO.fsSUBJECT_ID = fsSUBJ_ID;
                                clsARC_VIDEO.fsDESCRIPTION = string.Empty;
                                clsARC_VIDEO.fsFILE_STATUS = "Y";
                                clsARC_VIDEO.fsFILE_TYPE = fsFILE_TYPE;
                                clsARC_VIDEO.fsFILE_SIZE = fsFILE_SIZE;
                                clsARC_VIDEO.fsFILE_PATH = fsFILE_PATH;

                                clsARC_VIDEO.fsFILE_TYPE_H = fsFILE_TYPE;
                                clsARC_VIDEO.fsFILE_SIZE_H = fsFILE_SIZE_H;
                                clsARC_VIDEO.fsFILE_PATH_H = fsFILE_PATH_H;

                                clsARC_VIDEO.fsFILE_TYPE_L = string.Empty;
                                clsARC_VIDEO.fsFILE_SIZE_L = string.Empty;
                                clsARC_VIDEO.fsFILE_PATH_L = fsFILE_PATH_L;

                                if (v.GetVideoInfo())
                                {
                                    clsARC_VIDEO.fdDURATION = v.VideoDuration;
                                }

                                clsARC_VIDEO.fxMEDIA_INFO = fxMEDIA_INFO;
                                clsARC_VIDEO.fsRESOL_TAG = string.Empty;
                                clsARC_VIDEO.fnPRE_ID = int.Parse(fnPRE_ID);
                                clsARC_VIDEO.fsCREATED_BY = fsLOGIN_ID;

                                string fsRESULT = new repARC_VIDEO().fnINSERT_ARC_VDO(clsARC_VIDEO);

                                if (fsRESULT.IndexOf("ERROR") > -1)
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsARC_VIDEO.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=影片檔案;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_VIDEO), clsARC_VIDEO.fsCREATED_BY.Trim());
                                }
                                else
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsARC_VIDEO.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=影片檔案;@RESULT=成功;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_VIDEO), clsARC_VIDEO.fsCREATED_BY.Trim());
                                    //新增tblWORK

                                    clsL_WORK clsL_WORK = new clsL_WORK();
                                    clsL_WORK.fsTYPE = "TRANSCODE";
                                    clsL_WORK.fsPARAMETERS = "V;" + fsFILE_NO + ";" + fsSUBJ_ID + ";" + Properties.Settings.Default.fsMEDIA_V_TO + ";";
                                    clsL_WORK.fsSTATUS = "00";
                                    clsL_WORK.fsPROGRESS = "0";
                                    clsL_WORK.fsPRIORITY = "5";
                                    clsL_WORK.fsRESULT = string.Empty;
                                    clsL_WORK.fsNOTE = string.Empty;
                                    clsL_WORK.fsCREATED_BY = fsLOGIN_ID;
                                    clsL_WORK._ITEM_ID = fsFILE_NO;

                                    fsRESULT = new repL_WORK().fnINSERT_L_WORK(clsL_WORK);
                                    if (fsRESULT.IndexOf("ERROR") > -1)
                                        repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsL_WORK.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=WORK;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsL_WORK), clsARC_VIDEO.fsCREATED_BY.Trim());

                                }
                            }
                            else if (fsTYPE == "MEDIATYPE_TO_A")
                            {
                                //新增至資料庫
                                clsARC_AUDIO clsARC_AUDIO = new clsARC_AUDIO();

                                //組成完整路徑
                                fsFILE_PATH = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_A")[0].fsVALUE + DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString("00") + @"\" + DateTime.Now.Day.ToString("00") + @"\";
                                fsFILE_PATH_H = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_A")[0].fsVALUE + DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString("00") + @"\" + DateTime.Now.Day.ToString("00") + @"\";
                                fsFILE_PATH_L = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_A")[0].fsVALUE + DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString("00") + @"\" + DateTime.Now.Day.ToString("00") + @"\";

                                //搬至高解區
                                if (!System.IO.Directory.Exists(fsFILE_PATH_H)) System.IO.Directory.CreateDirectory(fsFILE_PATH_H);
                                if (System.IO.File.Exists(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT)) System.IO.File.Delete(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);
                                System.IO.File.Move(fsTARGET_FILE, fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                //產生專輯封面
                                var audioFile = TagLib.File.Create(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                if (audioFile.Tag.Pictures.Length > 0)
                                {
                                    MemoryStream ms = new MemoryStream(audioFile.Tag.Pictures[0].Data.Data);
                                    ms.Seek(0, SeekOrigin.Begin);
                                    using (FileStream file = new FileStream(fsFILE_PATH_H + fsFILE_NO + ".jpg", FileMode.Create, System.IO.FileAccess.Write))
                                    {
                                        byte[] bytes = new byte[ms.Length];
                                        ms.Read(bytes, 0, (int)ms.Length - 1);
                                        file.Write(bytes, 0, bytes.Length - 1);
                                    }
                                    ms.Close();
                                    ms.Dispose();

                                    clsARC_AUDIO.fcALBUM_PICTURE = true;
                                }
                                else
                                {
                                    clsARC_AUDIO.fcALBUM_PICTURE = false;
                                }

                                //抓取MediaInfo
                                fxMEDIA_INFO = Common.clsMEDIA_INFO.fnGET_FILE_MEDIA_INFO(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);
                                
                                clsARC_AUDIO.fsFILE_NO = fsFILE_NO;
                                clsARC_AUDIO.fsTITLE = fsTITLE;
                                clsARC_AUDIO.fsSUBJECT_ID = fsSUBJ_ID;
                                clsARC_AUDIO.fsDESCRIPTION = string.Empty;
                                clsARC_AUDIO.fsFILE_STATUS = "Y";
                                clsARC_AUDIO.fsFILE_TYPE = fsFILE_TYPE;
                                clsARC_AUDIO.fsFILE_SIZE = fsFILE_SIZE;
                                clsARC_AUDIO.fsFILE_PATH = fsFILE_PATH;

                                clsARC_AUDIO.fsFILE_TYPE_H = fsFILE_TYPE;
                                clsARC_AUDIO.fsFILE_SIZE_H = fsFILE_SIZE_H;
                                clsARC_AUDIO.fsFILE_PATH_H = fsFILE_PATH_H;

                                clsARC_AUDIO.fsFILE_TYPE_L = string.Empty;
                                clsARC_AUDIO.fsFILE_SIZE_L = string.Empty;
                                clsARC_AUDIO.fsFILE_PATH_L = fsFILE_PATH_L;
                                clsARC_AUDIO.fxMEDIA_INFO = fxMEDIA_INFO;

                                clsARC_AUDIO.fsALBUM = audioFile.Tag.Album;
                                clsARC_AUDIO.fsALBUM_TITLE = audioFile.Tag.Title;
                                clsARC_AUDIO.fsALBUM_ARTISTS = string.Join(";", audioFile.Tag.AlbumArtists);
                                clsARC_AUDIO.fsALBUM_PERFORMERS = string.Join(";", audioFile.Tag.Performers);
                                clsARC_AUDIO.fsALBUM_COMPOSERS = string.Join(";", audioFile.Tag.Composers);
                                clsARC_AUDIO.fnALBUM_YEAR = (int)audioFile.Tag.Year;
                                clsARC_AUDIO.fsALBUM_COPYRIGHT = audioFile.Tag.Copyright;
                                clsARC_AUDIO.fsALBUM_GENRES = string.Join(";", audioFile.Tag.Genres);
                                clsARC_AUDIO.fsALBUM_COMMENT = audioFile.Tag.Comment;

                                clsARC_AUDIO.fnPRE_ID = int.Parse(fnPRE_ID);

                                clsARC_AUDIO.fsCREATED_BY = fsLOGIN_ID;

                                string fsRESULT = new repARC_AUDIO().fnINSERT_ARC_AUDIO(clsARC_AUDIO);

                                if (fsRESULT.IndexOf("ERROR") > -1)
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsARC_AUDIO.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=聲音檔案;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_AUDIO), clsARC_AUDIO.fsCREATED_BY.Trim());
                                }
                                else
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsARC_AUDIO.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=聲音檔案;@RESULT=成功;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_AUDIO), clsARC_AUDIO.fsCREATED_BY.Trim());
                                    //新增tblWORK

                                    clsL_WORK clsL_WORK = new clsL_WORK();
                                    clsL_WORK.fsTYPE = "TRANSCODE";
                                    clsL_WORK.fsPARAMETERS = "A;" + fsFILE_NO + ";" + fsSUBJ_ID + ";" + Properties.Settings.Default.fsMEDIA_A_TO + ";";
                                    clsL_WORK.fsSTATUS = "00";
                                    clsL_WORK.fsPROGRESS = "0";
                                    clsL_WORK.fsPRIORITY = "5";
                                    clsL_WORK.fsRESULT = string.Empty;
                                    clsL_WORK.fsNOTE = string.Empty;
                                    clsL_WORK.fsCREATED_BY = fsLOGIN_ID;
                                    clsL_WORK._ITEM_ID = fsFILE_NO;

                                    fsRESULT = new repL_WORK().fnINSERT_L_WORK(clsL_WORK);
                                    if (fsRESULT.IndexOf("ERROR") > -1)
                                        repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsL_WORK.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=WORK;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsL_WORK), clsARC_AUDIO.fsCREATED_BY.Trim());

                                }
                            }
                            else if (fsTYPE == "MEDIATYPE_TO_P")
                            {
                                //組成完整路徑
                                fsFILE_PATH = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_P")[0].fsVALUE + DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString("00") + @"\" + DateTime.Now.Day.ToString("00") + @"\";
                                fsFILE_PATH_H = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_P")[0].fsVALUE + DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString("00") + @"\" + DateTime.Now.Day.ToString("00") + @"\";
                                fsFILE_PATH_L = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_P")[0].fsVALUE + DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString("00") + @"\" + DateTime.Now.Day.ToString("00") + @"\";

                                //搬至高解區
                                if (!System.IO.Directory.Exists(fsFILE_PATH_H)) System.IO.Directory.CreateDirectory(fsFILE_PATH_H);
                                if (System.IO.File.Exists(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT)) System.IO.File.Delete(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);
                                System.IO.File.Move(fsTARGET_FILE, fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                //抓取MediaInfo
                                fxMEDIA_INFO = Common.clsMEDIA_INFO.fnGET_FILE_MEDIA_INFO(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                //抓取Exif
                                Dictionary<string, string> _dicEXIF = new clsIMAGE_EXIF().fnGET_IMAGE_EXIF(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                //新增至資料庫
                                clsARC_PHOTO clsARC_PHOTO = new clsARC_PHOTO();
                                clsARC_PHOTO.fsFILE_NO = fsFILE_NO;
                                clsARC_PHOTO.fsTITLE = fsTITLE;
                                clsARC_PHOTO.fsSUBJECT_ID = fsSUBJ_ID;
                                clsARC_PHOTO.fsDESCRIPTION = string.Empty;
                                clsARC_PHOTO.fsFILE_STATUS = "Y";
                                clsARC_PHOTO.fsFILE_TYPE = fsFILE_TYPE;
                                clsARC_PHOTO.fsFILE_SIZE = fsFILE_SIZE;
                                clsARC_PHOTO.fsFILE_PATH = fsFILE_PATH;

                                clsARC_PHOTO.fsFILE_TYPE_H = fsFILE_TYPE;
                                clsARC_PHOTO.fsFILE_SIZE_H = fsFILE_SIZE_H;
                                clsARC_PHOTO.fsFILE_PATH_H = fsFILE_PATH_H;

                                clsARC_PHOTO.fsFILE_TYPE_L = string.Empty;
                                clsARC_PHOTO.fsFILE_SIZE_L = string.Empty;
                                clsARC_PHOTO.fsFILE_PATH_L = fsFILE_PATH_L;

                                clsARC_PHOTO.fxMEDIA_INFO = fxMEDIA_INFO;

                                if (_dicEXIF != null && _dicEXIF.Count > 0)
                                {
                                    clsARC_PHOTO.fnWIDTH = _dicEXIF["fnWIDTH"];
                                    clsARC_PHOTO.fnHEIGHT = _dicEXIF["fnHEIGHT"];
                                    clsARC_PHOTO.fnXDPI = _dicEXIF["fnXDPI"];
                                    clsARC_PHOTO.fnYDPI = _dicEXIF["fnYDPI"];
                                    clsARC_PHOTO.fsCAMERA_MAKE = _dicEXIF["fsCAMERA_MAKE"];
                                    clsARC_PHOTO.fsCAMERA_MODEL = _dicEXIF["fsCAMERA_MODEL"];
                                    clsARC_PHOTO.fsFOCAL_LENGTH = _dicEXIF["fsFOCAL_LENGTH"];
                                    clsARC_PHOTO.fsEXPOSURE_TIME = _dicEXIF["fsEXPOSURE_TIME"];
                                    clsARC_PHOTO.fsAPERTURE = _dicEXIF["fsAPERTURE"];
                                    clsARC_PHOTO.fnISO = int.Parse(_dicEXIF["fnISO"]);
                                }
                                else
                                {
                                    clsARC_PHOTO.fnWIDTH = string.Empty;
                                    clsARC_PHOTO.fnHEIGHT = string.Empty;
                                    clsARC_PHOTO.fnXDPI = string.Empty;
                                    clsARC_PHOTO.fnYDPI = string.Empty;
                                    clsARC_PHOTO.fsCAMERA_MAKE = string.Empty;
                                    clsARC_PHOTO.fsCAMERA_MODEL = string.Empty;
                                    clsARC_PHOTO.fsFOCAL_LENGTH = string.Empty;
                                    clsARC_PHOTO.fsEXPOSURE_TIME = string.Empty;
                                    clsARC_PHOTO.fsAPERTURE = string.Empty;
                                    clsARC_PHOTO.fnISO = 0;
                                }
                                clsARC_PHOTO.fnPRE_ID = int.Parse(fnPRE_ID);
                                clsARC_PHOTO.fsCREATED_BY = fsLOGIN_ID;

                                string fsRESULT = new repARC_PHOTO().fnINSERT_ARC_PHOTO(clsARC_PHOTO);

                                if (fsRESULT.IndexOf("ERROR") > -1)
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsARC_PHOTO.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=圖片檔案;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_PHOTO), clsARC_PHOTO.fsCREATED_BY.Trim());
                                }
                                else
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsARC_PHOTO.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=圖片檔案;@RESULT=成功;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_PHOTO), clsARC_PHOTO.fsCREATED_BY.Trim());
                                    //新增tblWORK

                                    clsL_WORK clsL_WORK = new clsL_WORK();
                                    clsL_WORK.fsTYPE = "TRANSCODE";
                                    clsL_WORK.fsPARAMETERS = "P;" + fsFILE_NO + ";" + fsSUBJ_ID + ";" + Properties.Settings.Default.fsMEDIA_P_TO + ";";
                                    clsL_WORK.fsSTATUS = "00";
                                    clsL_WORK.fsPROGRESS = "0";
                                    clsL_WORK.fsPRIORITY = "5";
                                    clsL_WORK.fsRESULT = string.Empty;
                                    clsL_WORK.fsNOTE = string.Empty;
                                    clsL_WORK.fsCREATED_BY = fsLOGIN_ID;
                                    clsL_WORK._ITEM_ID = fsFILE_NO;

                                    fsRESULT = new repL_WORK().fnINSERT_L_WORK(clsL_WORK);
                                    if (fsRESULT.IndexOf("ERROR") > -1)
                                        repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsL_WORK.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=WORK;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsL_WORK), clsARC_PHOTO.fsCREATED_BY.Trim());

                                }
                            }
                            else if (fsTYPE == "MEDIATYPE_TO_D")
                            {

                                //組成完整路徑
                                fsFILE_PATH = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_D")[0].fsVALUE + DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString("00") + @"\" + DateTime.Now.Day.ToString("00") + @"\";
                                //fsFILE_PATH_H = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_D")[0].fsVALUE + DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString("00") + @"\" + DateTime.Now.Day.ToString("00") + @"\";
                                //fsFILE_PATH_L = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_D")[0].fsVALUE + DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString("00") + @"\" + DateTime.Now.Day.ToString("00") + @"\";

                                //搬至高解區
                                if (!System.IO.Directory.Exists(fsFILE_PATH)) System.IO.Directory.CreateDirectory(fsFILE_PATH);
                                if (System.IO.File.Exists(fsFILE_PATH + fsFILE_NO + "." + fsEXT)) System.IO.File.Delete(fsFILE_PATH + fsFILE_NO + "." + fsEXT);
                                System.IO.File.Move(fsTARGET_FILE, fsFILE_PATH + fsFILE_NO + "." + fsEXT);

                                //抓取MediaInfo
                                fxMEDIA_INFO = Common.clsMEDIA_INFO.fnGET_FILE_MEDIA_INFO(fsFILE_PATH + fsFILE_NO + "." + fsEXT);

                                //判斷是否為用ifilter或轉txt的項目，若不可轉，則也不用送work了
                                string[] fsTO_IFILTER_EXTs = new repCONFIG().fnGET_CFG("MEDIATYPE_TO_IFILTER")[0].fsVALUE.Split(';');
                                string[] fsTO_TXT_EXTs = new repCONFIG().fnGET_CFG("MEDIATYPE_TO_TXT")[0].fsVALUE.Split(';');

                                bool fbIS_TO_IFILTER = false;
                                bool fbIS_TO_TXT = false;

                                if (fsTO_IFILTER_EXTs != null && fsTO_IFILTER_EXTs.Count() > 0)
                                    if (fsTO_IFILTER_EXTs.FirstOrDefault(f => f == fsEXT.ToLower()) != null)
                                        fbIS_TO_IFILTER = true;

                                if (fsTO_TXT_EXTs != null && fsTO_TXT_EXTs.Count() > 0)
                                    if (fsTO_TXT_EXTs.FirstOrDefault(f => f == fsEXT.ToLower()) != null)
                                        fbIS_TO_TXT = true;

                                //新增至資料庫
                                clsARC_DOC clsARC_DOC = new clsARC_DOC();
                                clsARC_DOC.fsFILE_NO = fsFILE_NO;
                                clsARC_DOC.fsTITLE = fsTITLE;
                                clsARC_DOC.fsSUBJECT_ID = fsSUBJ_ID;
                                clsARC_DOC.fsDESCRIPTION = string.Empty;
                                clsARC_DOC.fsFILE_STATUS = "Y";
                                clsARC_DOC.fsFILE_TYPE = fsFILE_TYPE;
                                clsARC_DOC.fsFILE_SIZE = fsFILE_SIZE;
                                clsARC_DOC.fsFILE_PATH = fsFILE_PATH;

                                //clsARC_DOC.fsFILE_TYPE_1 = fsFILE_TYPE;
                                //clsARC_DOC.fsFILE_SIZE_1 = fsFILE_SIZE_H;
                                //clsARC_DOC.fsFILE_PATH_1 = fsFILE_PATH_H;

                                //clsARC_DOC.fsFILE_TYPE_2 = string.Empty;
                                //clsARC_DOC.fsFILE_SIZE_2 = string.Empty;
                                //clsARC_DOC.fsFILE_PATH_2 = string.Empty;

                                clsARC_DOC.fxMEDIA_INFO = fxMEDIA_INFO;
                                clsARC_DOC.fsCONTENT = string.Empty;

                                System.IO.FileInfo fileInfo = new FileInfo(fsFILE_PATH + fsFILE_NO + "." + fsEXT);

                                clsARC_DOC.fdFILE_CREATED_DATE = fileInfo.CreationTime.ToString("yyyy/MM/dd HH:mm:ss");
                                clsARC_DOC.fdFILE_UPDATED_DATE = fileInfo.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss");
                                clsARC_DOC.fnPRE_ID = int.Parse(fnPRE_ID);
                                clsARC_DOC.fsCREATED_BY = fsLOGIN_ID;

                                string fsRESULT = new repARC_DOC().fnINSERT_ARC_DOC(clsARC_DOC);

                                if (fsRESULT.IndexOf("ERROR") > -1)
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsARC_DOC.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=文件檔案;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_DOC), clsARC_DOC.fsCREATED_BY.Trim());
                                }
                                else
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsARC_DOC.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=文件檔案;@RESULT=成功;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_DOC), clsARC_DOC.fsCREATED_BY.Trim());

                                    //下面用不到原始檔案了，就把檔案加密
                                    Common.clsSECURITY.AddFileEncryption(fsFILE_PATH + fsFILE_NO + "." + fsEXT, fsFILE_PATH + fsFILE_NO + ".enc");

                                    //新增tblWORK
                                    clsL_WORK clsL_WORK = new clsL_WORK();
                                    clsL_WORK.fsTYPE = "TRANSCODE";
                                    if (!fbIS_TO_IFILTER && !fbIS_TO_TXT)
                                        clsL_WORK.fsPARAMETERS = "D;" + fsFILE_NO + ";" + fsSUBJ_ID + ";" + string.Empty + ";";
                                    else if (fbIS_TO_TXT)
                                        clsL_WORK.fsPARAMETERS = "D;" + fsFILE_NO + ";" + fsSUBJ_ID + ";X;";
                                    else if (fbIS_TO_IFILTER)
                                        clsL_WORK.fsPARAMETERS = "D;" + fsFILE_NO + ";" + fsSUBJ_ID + ";L;";

                                    clsL_WORK.fsSTATUS = "00";
                                    clsL_WORK.fsPROGRESS = "0";
                                    clsL_WORK.fsPRIORITY = "5";
                                    clsL_WORK.fsRESULT = string.Empty;
                                    clsL_WORK.fsNOTE = string.Empty;
                                    clsL_WORK.fsCREATED_BY = fsLOGIN_ID;
                                    clsL_WORK._ITEM_ID = fsFILE_NO;

                                    fsRESULT = new repL_WORK().fnINSERT_L_WORK(clsL_WORK);
                                    if (fsRESULT.IndexOf("ERROR") > -1)
                                        repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsL_WORK.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=WORK;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsL_WORK), clsARC_DOC.fsCREATED_BY.Trim());

                                }
                            }


                        }
                        else
                        {
                            string fsFILE_NAME = Path.GetFileName(fsTARGET_FILE);
                            string fsEXT = Path.GetExtension(fsTARGET_FILE).Replace(".", "");

                            string fsFILE_TYPE = fsEXT;
                            string fsFILE_TYPE_H = string.Empty;
                            string fsFILE_TYPE_L = string.Empty;

                            string fsFILE_SIZE = (resumableTotalSize >= 1024 * 1024 * 1024 ? (Math.Round(resumableTotalSize / 1024 / 1024 / 1024, 2).ToString() + "GB") : (resumableTotalSize >= 1024 * 1024 ? (Math.Round(resumableTotalSize / 1024 / 1024, 2).ToString() + "MB") : (resumableTotalSize >= 1024 ? (Math.Round(resumableTotalSize / 1024, 2).ToString() + "KB") : resumableTotalSize.ToString())));
                            string fsFILE_SIZE_H = resumableTotalSize.ToString();
                            string fsFILE_SIZE_L = string.Empty;

                            string fsFILE_PATH = string.Empty;
                            string fsFILE_PATH_H = string.Empty;
                            string fsFILE_PATH_L = string.Empty;

                            string fxMEDIA_INFO = string.Empty;

                            //置換
                            if (fsTYPE == "MEDIATYPE_TO_V")
                            {

                                //組成完整路徑
                                fsFILE_PATH = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_V_H")[0].fsVALUE + fsFILE_NO.Substring(0, 4) + @"\" + fsFILE_NO.Substring(4, 2) + @"\" + fsFILE_NO.Substring(6, 2) + @"\";
                                fsFILE_PATH_H = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_V_H")[0].fsVALUE + fsFILE_NO.Substring(0, 4) + @"\" + fsFILE_NO.Substring(4, 2) + @"\" + fsFILE_NO.Substring(6, 2) + @"\";
                                fsFILE_PATH_L = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_V_L")[0].fsVALUE + fsFILE_NO.Substring(0, 4) + @"\" + fsFILE_NO.Substring(4, 2) + @"\" + fsFILE_NO.Substring(6, 2) + @"\";

                                //搬至高解區
                                if (!System.IO.Directory.Exists(fsFILE_PATH_H)) System.IO.Directory.CreateDirectory(fsFILE_PATH_H);
                                if (System.IO.File.Exists(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT)) System.IO.File.Delete(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);
                                System.IO.File.Move(fsTARGET_FILE, fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                //抓取MediaInfo
                                fxMEDIA_INFO = Common.clsMEDIA_INFO.fnGET_FILE_MEDIA_INFO(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                //更新資料庫
                                clsARC_VIDEO clsARC_VIDEO = new clsARC_VIDEO();
                                clsARC_VIDEO.fsFILE_NO = fsFILE_NO;
                                clsARC_VIDEO.fsFILE_TYPE = fsFILE_TYPE;
                                clsARC_VIDEO.fsFILE_SIZE = fsFILE_SIZE;
                                clsARC_VIDEO.fsFILE_PATH = fsFILE_PATH;

                                clsARC_VIDEO.fsFILE_TYPE_H = fsFILE_TYPE;
                                clsARC_VIDEO.fsFILE_SIZE_H = fsFILE_SIZE_H;
                                clsARC_VIDEO.fsFILE_PATH_H = fsFILE_PATH_H;

                                clsARC_VIDEO.fsFILE_TYPE_L = string.Empty;
                                clsARC_VIDEO.fsFILE_SIZE_L = string.Empty;
                                clsARC_VIDEO.fsFILE_PATH_L = fsFILE_PATH_L;

                                clsARC_VIDEO.fxMEDIA_INFO = fxMEDIA_INFO;
                                clsARC_VIDEO.fsRESOL_TAG = string.Empty;

                                clsARC_VIDEO.fsUPDATED_BY = fsLOGIN_ID;

                                clsARC_VIDEO.fbDELETE_KF = (fbDELETE_KF == "Y" ? true : false);

                                string fsRESULT = new repARC_VIDEO().fnUPDATE_ARC_VDO_CHANGE(clsARC_VIDEO);

                                if (fsRESULT.IndexOf("ERROR") > -1)
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M022", "@USER_ID=" + clsARC_VIDEO.fsUPDATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=置換影片檔案;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_VIDEO), clsARC_VIDEO.fsUPDATED_BY.Trim());
                                }
                                else
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M022", "@USER_ID=" + clsARC_VIDEO.fsUPDATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=置換影片檔案;@RESULT=成功;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_VIDEO), clsARC_VIDEO.fsUPDATED_BY.Trim());
                                    //新增tblWORK

                                    clsL_WORK clsL_WORK = new clsL_WORK();
                                    clsL_WORK.fsTYPE = "TRANSCODE";
                                    clsL_WORK.fsPARAMETERS = "V;" + fsFILE_NO + ";" + fsSUBJ_ID + ";" + Properties.Settings.Default.fsMEDIA_V_TO + ";";
                                    clsL_WORK.fsSTATUS = "00";
                                    clsL_WORK.fsPROGRESS = "0";
                                    clsL_WORK.fsPRIORITY = "5";
                                    clsL_WORK.fsRESULT = string.Empty;
                                    clsL_WORK.fsNOTE = string.Empty;
                                    clsL_WORK.fsCREATED_BY = fsLOGIN_ID;
                                    clsL_WORK._ITEM_ID = fsFILE_NO;

                                    fsRESULT = new repL_WORK().fnINSERT_L_WORK(clsL_WORK);
                                    if (fsRESULT.IndexOf("ERROR") > -1)
                                        repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsL_WORK.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=WORK;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsL_WORK), clsARC_VIDEO.fsCREATED_BY.Trim());
                                }
                            }
                            else if (fsTYPE == "MEDIATYPE_TO_A")
                            {
                                //更新資料庫
                                clsARC_AUDIO clsARC_AUDIO = new clsARC_AUDIO();

                                //組成完整路徑
                                fsFILE_PATH = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_A")[0].fsVALUE + fsFILE_NO.Substring(0, 4) + @"\" + fsFILE_NO.Substring(4, 2) + @"\" + fsFILE_NO.Substring(6, 2) + @"\";
                                fsFILE_PATH_H = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_A")[0].fsVALUE + fsFILE_NO.Substring(0, 4) + @"\" + fsFILE_NO.Substring(4, 2) + @"\" + fsFILE_NO.Substring(6, 2) + @"\";
                                fsFILE_PATH_L = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_A")[0].fsVALUE + fsFILE_NO.Substring(0, 4) + @"\" + fsFILE_NO.Substring(4, 2) + @"\" + fsFILE_NO.Substring(6, 2) + @"\";

                                //搬至高解區
                                if (!System.IO.Directory.Exists(fsFILE_PATH_H)) System.IO.Directory.CreateDirectory(fsFILE_PATH_H);
                                if (System.IO.File.Exists(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT)) System.IO.File.Delete(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);
                                System.IO.File.Move(fsTARGET_FILE, fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                //產生專輯封面
                                var audioFile = TagLib.File.Create(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                if (audioFile.Tag.Pictures.Length > 0)
                                {
                                    MemoryStream ms = new MemoryStream(audioFile.Tag.Pictures[0].Data.Data);
                                    ms.Seek(0, SeekOrigin.Begin);
                                    using (FileStream file = new FileStream(fsFILE_PATH_H + fsFILE_NO + ".jpg", FileMode.Create, System.IO.FileAccess.Write))
                                    {
                                        byte[] bytes = new byte[ms.Length];
                                        ms.Read(bytes, 0, (int)ms.Length - 1);
                                        file.Write(bytes, 0, bytes.Length - 1);
                                    }
                                    ms.Close();
                                    ms.Dispose();

                                    clsARC_AUDIO.fcALBUM_PICTURE = true;
                                }
                                else
                                {
                                    clsARC_AUDIO.fcALBUM_PICTURE = false;
                                }

                                //抓取MediaInfo
                                fxMEDIA_INFO = Common.clsMEDIA_INFO.fnGET_FILE_MEDIA_INFO(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                
                                clsARC_AUDIO.fsFILE_NO = fsFILE_NO;
                                clsARC_AUDIO.fsFILE_TYPE = fsFILE_TYPE;
                                clsARC_AUDIO.fsFILE_SIZE = fsFILE_SIZE;
                                clsARC_AUDIO.fsFILE_PATH = fsFILE_PATH;

                                clsARC_AUDIO.fsFILE_TYPE_H = fsFILE_TYPE;
                                clsARC_AUDIO.fsFILE_SIZE_H = fsFILE_SIZE_H;
                                clsARC_AUDIO.fsFILE_PATH_H = fsFILE_PATH_H;

                                clsARC_AUDIO.fsFILE_TYPE_L = string.Empty;
                                clsARC_AUDIO.fsFILE_SIZE_L = string.Empty;
                                clsARC_AUDIO.fsFILE_PATH_L = fsFILE_PATH_L;
                                clsARC_AUDIO.fxMEDIA_INFO = fxMEDIA_INFO;

                                clsARC_AUDIO.fsALBUM = audioFile.Tag.Album;
                                clsARC_AUDIO.fsALBUM_TITLE = audioFile.Tag.Title;
                                clsARC_AUDIO.fsALBUM_ARTISTS = string.Join(";", audioFile.Tag.AlbumArtists);
                                clsARC_AUDIO.fsALBUM_PERFORMERS = string.Join(";", audioFile.Tag.Performers);
                                clsARC_AUDIO.fsALBUM_COMPOSERS = string.Join(";", audioFile.Tag.Composers);
                                clsARC_AUDIO.fnALBUM_YEAR = (int)audioFile.Tag.Year;
                                clsARC_AUDIO.fsALBUM_COPYRIGHT = audioFile.Tag.Copyright;
                                clsARC_AUDIO.fsALBUM_GENRES = string.Join(";", audioFile.Tag.Genres);
                                clsARC_AUDIO.fsALBUM_COMMENT = audioFile.Tag.Comment;

                                clsARC_AUDIO.fsUPDATED_BY = fsLOGIN_ID;

                                string fsRESULT = new repARC_AUDIO().fnUPDATE_ARC_AUDIO_CHANGE(clsARC_AUDIO);

                                if (fsRESULT.IndexOf("ERROR") > -1)
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M022", "@USER_ID=" + clsARC_AUDIO.fsUPDATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=置換聲音檔案;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_AUDIO), clsARC_AUDIO.fsUPDATED_BY.Trim());
                                }
                                else
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M022", "@USER_ID=" + clsARC_AUDIO.fsUPDATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=置換聲音檔案;@RESULT=成功;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_AUDIO), clsARC_AUDIO.fsUPDATED_BY.Trim());
                                    //新增tblWORK

                                    clsL_WORK clsL_WORK = new clsL_WORK();
                                    clsL_WORK.fsTYPE = "TRANSCODE";
                                    clsL_WORK.fsPARAMETERS = "A;" + fsFILE_NO + ";" + fsSUBJ_ID + ";" + Properties.Settings.Default.fsMEDIA_A_TO + ";";
                                    clsL_WORK.fsSTATUS = "00";
                                    clsL_WORK.fsPROGRESS = "0";
                                    clsL_WORK.fsPRIORITY = "5";
                                    clsL_WORK.fsRESULT = string.Empty;
                                    clsL_WORK.fsNOTE = string.Empty;
                                    clsL_WORK.fsCREATED_BY = fsLOGIN_ID;
                                    clsL_WORK._ITEM_ID = fsFILE_NO;

                                    fsRESULT = new repL_WORK().fnINSERT_L_WORK(clsL_WORK);
                                    if (fsRESULT.IndexOf("ERROR") > -1)
                                        repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsL_WORK.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=WORK;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsL_WORK), clsARC_AUDIO.fsCREATED_BY.Trim());

                                }
                            }
                            else if (fsTYPE == "MEDIATYPE_TO_P")
                            {
                                //組成完整路徑
                                fsFILE_PATH = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_P")[0].fsVALUE + fsFILE_NO.Substring(0, 4) + @"\" + fsFILE_NO.Substring(4, 2) + @"\" + fsFILE_NO.Substring(6, 2) + @"\";
                                fsFILE_PATH_H = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_P")[0].fsVALUE + fsFILE_NO.Substring(0, 4) + @"\" + fsFILE_NO.Substring(4, 2) + @"\" + fsFILE_NO.Substring(6, 2) + @"\";
                                fsFILE_PATH_L = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_P")[0].fsVALUE + fsFILE_NO.Substring(0, 4) + @"\" + fsFILE_NO.Substring(4, 2) + @"\" + fsFILE_NO.Substring(6, 2) + @"\";

                                //搬至高解區
                                if (!System.IO.Directory.Exists(fsFILE_PATH_H)) System.IO.Directory.CreateDirectory(fsFILE_PATH_H);
                                if (System.IO.File.Exists(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT)) System.IO.File.Delete(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);
                                System.IO.File.Move(fsTARGET_FILE, fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                //抓取MediaInfo
                                fxMEDIA_INFO = Common.clsMEDIA_INFO.fnGET_FILE_MEDIA_INFO(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                //抓取Exif
                                Dictionary<string, string> _dicEXIF = new clsIMAGE_EXIF().fnGET_IMAGE_EXIF(fsFILE_PATH_H + fsFILE_NO + "_H." + fsEXT);

                                //新增至資料庫
                                clsARC_PHOTO clsARC_PHOTO = new clsARC_PHOTO();
                                clsARC_PHOTO.fsFILE_NO = fsFILE_NO;
                                clsARC_PHOTO.fsFILE_TYPE = fsFILE_TYPE;
                                clsARC_PHOTO.fsFILE_SIZE = fsFILE_SIZE;
                                clsARC_PHOTO.fsFILE_PATH = fsFILE_PATH;

                                clsARC_PHOTO.fsFILE_TYPE_H = fsFILE_TYPE;
                                clsARC_PHOTO.fsFILE_SIZE_H = fsFILE_SIZE_H;
                                clsARC_PHOTO.fsFILE_PATH_H = fsFILE_PATH_H;

                                clsARC_PHOTO.fsFILE_TYPE_L = string.Empty;
                                clsARC_PHOTO.fsFILE_SIZE_L = string.Empty;
                                clsARC_PHOTO.fsFILE_PATH_L = fsFILE_PATH_L;

                                clsARC_PHOTO.fxMEDIA_INFO = fxMEDIA_INFO;

                                if (_dicEXIF != null && _dicEXIF.Count > 0)
                                {
                                    clsARC_PHOTO.fnWIDTH = _dicEXIF["fnWIDTH"];
                                    clsARC_PHOTO.fnHEIGHT = _dicEXIF["fnHEIGHT"];
                                    clsARC_PHOTO.fnXDPI = _dicEXIF["fnXDPI"];
                                    clsARC_PHOTO.fnYDPI = _dicEXIF["fnYDPI"];
                                    clsARC_PHOTO.fsCAMERA_MAKE = _dicEXIF["fsCAMERA_MAKE"];
                                    clsARC_PHOTO.fsCAMERA_MODEL = _dicEXIF["fsCAMERA_MODEL"];
                                    clsARC_PHOTO.fsFOCAL_LENGTH = _dicEXIF["fsFOCAL_LENGTH"];
                                    clsARC_PHOTO.fsEXPOSURE_TIME = _dicEXIF["fsEXPOSURE_TIME"];
                                    clsARC_PHOTO.fsAPERTURE = _dicEXIF["fsAPERTURE"];
                                    clsARC_PHOTO.fnISO = int.Parse(_dicEXIF["fnISO"]);
                                }
                                else
                                {
                                    clsARC_PHOTO.fnWIDTH = string.Empty;
                                    clsARC_PHOTO.fnHEIGHT = string.Empty;
                                    clsARC_PHOTO.fnXDPI = string.Empty;
                                    clsARC_PHOTO.fnYDPI = string.Empty;
                                    clsARC_PHOTO.fsCAMERA_MAKE = string.Empty;
                                    clsARC_PHOTO.fsCAMERA_MODEL = string.Empty;
                                    clsARC_PHOTO.fsFOCAL_LENGTH = string.Empty;
                                    clsARC_PHOTO.fsEXPOSURE_TIME = string.Empty;
                                    clsARC_PHOTO.fsAPERTURE = string.Empty;
                                    clsARC_PHOTO.fnISO = 0;
                                }

                                clsARC_PHOTO.fsUPDATED_BY = fsLOGIN_ID;

                                string fsRESULT = new repARC_PHOTO().fnUPDATE_ARC_PHOTO_CHANGE(clsARC_PHOTO);

                                if (fsRESULT.IndexOf("ERROR") > -1)
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M022", "@USER_ID=" + clsARC_PHOTO.fsUPDATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=置換圖片檔案;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_PHOTO), clsARC_PHOTO.fsUPDATED_BY.Trim());
                                }
                                else
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M022", "@USER_ID=" + clsARC_PHOTO.fsUPDATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=置換圖片檔案;@RESULT=成功;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_PHOTO), clsARC_PHOTO.fsUPDATED_BY.Trim());
                                    //新增tblWORK

                                    clsL_WORK clsL_WORK = new clsL_WORK();
                                    clsL_WORK.fsTYPE = "TRANSCODE";
                                    clsL_WORK.fsPARAMETERS = "P;" + fsFILE_NO + ";" + fsSUBJ_ID + ";" + Properties.Settings.Default.fsMEDIA_P_TO + ";";
                                    clsL_WORK.fsSTATUS = "00";
                                    clsL_WORK.fsPROGRESS = "0";
                                    clsL_WORK.fsPRIORITY = "5";
                                    clsL_WORK.fsRESULT = string.Empty;
                                    clsL_WORK.fsNOTE = string.Empty;
                                    clsL_WORK.fsCREATED_BY = fsLOGIN_ID;
                                    clsL_WORK._ITEM_ID = fsFILE_NO;

                                    fsRESULT = new repL_WORK().fnINSERT_L_WORK(clsL_WORK);
                                    if (fsRESULT.IndexOf("ERROR") > -1)
                                        repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsL_WORK.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=WORK;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsL_WORK), clsARC_PHOTO.fsUPDATED_BY.Trim());

                                }
                            }
                            else if (fsTYPE == "MEDIATYPE_TO_D")
                            {
                                //組成完整路徑
                                fsFILE_PATH = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_D")[0].fsVALUE + fsFILE_NO.Substring(0, 4) + @"\" + fsFILE_NO.Substring(4, 2) + @"\" + fsFILE_NO.Substring(6, 2) + @"\";
                                //fsFILE_PATH_H = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_D")[0].fsVALUE + fsFILE_NO.Substring(0, 4) + @"\" + fsFILE_NO.Substring(4, 2) + @"\" + fsFILE_NO.Substring(6, 2) + @"\";
                                //fsFILE_PATH_L = new repCONFIG().fnGET_CFG("MEDIA_FOLDER_D")[0].fsVALUE + fsFILE_NO.Substring(0, 4) + @"\" + fsFILE_NO.Substring(4, 2) + @"\" + fsFILE_NO.Substring(6, 2) + @"\";

                                //搬至高解區
                                if (!System.IO.Directory.Exists(fsFILE_PATH)) System.IO.Directory.CreateDirectory(fsFILE_PATH);
                                if (System.IO.File.Exists(fsFILE_PATH + fsFILE_NO + "." + fsEXT)) System.IO.File.Delete(fsFILE_PATH + fsFILE_NO + "." + fsEXT);
                                System.IO.File.Move(fsTARGET_FILE, fsFILE_PATH + fsFILE_NO + "." + fsEXT);

                                //抓取MediaInfo
                                fxMEDIA_INFO = Common.clsMEDIA_INFO.fnGET_FILE_MEDIA_INFO(fsFILE_PATH + fsFILE_NO + "." + fsEXT);

                                //判斷是否為用ifilter或轉txt的項目，若不可轉，則也不用送work了
                                string[] fsTO_IFILTER_EXTs = new repCONFIG().fnGET_CFG("MEDIATYPE_TO_IFILTER")[0].fsVALUE.Split(';');
                                string[] fsTO_TXT_EXTs = new repCONFIG().fnGET_CFG("MEDIATYPE_TO_TXT")[0].fsVALUE.Split(';');

                                bool fbIS_TO_IFILTER = false;
                                bool fbIS_TO_TXT = false;

                                if (fsTO_IFILTER_EXTs != null && fsTO_IFILTER_EXTs.Count() > 0)
                                    if (fsTO_IFILTER_EXTs.FirstOrDefault(f => f == fsEXT.ToLower()) != null)
                                        fbIS_TO_IFILTER = true;

                                if (fsTO_TXT_EXTs != null && fsTO_TXT_EXTs.Count() > 0)
                                    if (fsTO_TXT_EXTs.FirstOrDefault(f => f == fsEXT.ToLower()) != null)
                                        fbIS_TO_TXT = true;

                                //新增至資料庫
                                clsARC_DOC clsARC_DOC = new clsARC_DOC();
                                clsARC_DOC.fsFILE_NO = fsFILE_NO;
                                clsARC_DOC.fsFILE_TYPE = fsFILE_TYPE;
                                clsARC_DOC.fsFILE_SIZE = fsFILE_SIZE;
                                clsARC_DOC.fsFILE_PATH = fsFILE_PATH;

                                //clsARC_DOC.fsFILE_TYPE_1 = fsFILE_TYPE;
                                //clsARC_DOC.fsFILE_SIZE_1 = fsFILE_SIZE_H;
                                //clsARC_DOC.fsFILE_PATH_1 = fsFILE_PATH_H;

                                //clsARC_DOC.fsFILE_TYPE_2 = (fbIS_TO_PDF == true ? "pdf" : string.Empty);
                                //clsARC_DOC.fsFILE_SIZE_2 = string.Empty;
                                //clsARC_DOC.fsFILE_PATH_2 = (fbIS_TO_PDF == true ? fsFILE_PATH_L : string.Empty);

                                clsARC_DOC.fxMEDIA_INFO = fxMEDIA_INFO;

                                System.IO.FileInfo fileInfo = new FileInfo(fsFILE_PATH + fsFILE_NO + "." + fsEXT);

                                clsARC_DOC.fdFILE_CREATED_DATE = fileInfo.CreationTime.ToString("yyyy/MM/dd HH:mm:ss");
                                clsARC_DOC.fdFILE_UPDATED_DATE = fileInfo.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss");
                                clsARC_DOC.fsUPDATED_BY = fsLOGIN_ID;

                                string fsRESULT = new repARC_DOC().fnUPDATE_ARC_DOC_CHANGE(clsARC_DOC);

                                if (fsRESULT.IndexOf("ERROR") > -1)
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M022", "@USER_ID=" + clsARC_DOC.fsUPDATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=置換文件檔案;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_DOC), clsARC_DOC.fsUPDATED_BY.Trim());
                                }
                                else
                                {
                                    repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M022", "@USER_ID=" + clsARC_DOC.fsUPDATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=置換文件檔案;@RESULT=成功;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsARC_DOC), clsARC_DOC.fsUPDATED_BY.Trim());

                                    //下面用不到原始檔案了，就把檔案加密
                                    Common.clsSECURITY.AddFileEncryption(fsFILE_PATH + fsFILE_NO + "." + fsEXT, fsFILE_PATH + fsFILE_NO + ".enc");

                                    //新增tblWORK
                                    clsL_WORK clsL_WORK = new clsL_WORK();
                                    clsL_WORK.fsTYPE = "TRANSCODE";

                                    if (!fbIS_TO_IFILTER && !fbIS_TO_TXT)
                                        clsL_WORK.fsPARAMETERS = "D;" + fsFILE_NO + ";" + fsSUBJ_ID + ";" + string.Empty + ";";
                                    else if (fbIS_TO_TXT)
                                        clsL_WORK.fsPARAMETERS = "D;" + fsFILE_NO + ";" + fsSUBJ_ID + ";X;";
                                    else if (fbIS_TO_IFILTER)
                                        clsL_WORK.fsPARAMETERS = "D;" + fsFILE_NO + ";" + fsSUBJ_ID + ";L;";

                                    clsL_WORK.fsSTATUS = "00";
                                    clsL_WORK.fsPROGRESS = "0";
                                    clsL_WORK.fsPRIORITY = "5";
                                    clsL_WORK.fsRESULT = string.Empty;
                                    clsL_WORK.fsNOTE = string.Empty;
                                    clsL_WORK.fsCREATED_BY = fsLOGIN_ID;
                                    clsL_WORK._ITEM_ID = fsFILE_NO;

                                    fsRESULT = new repL_WORK().fnINSERT_L_WORK(clsL_WORK);
                                    if (fsRESULT.IndexOf("ERROR") > -1)
                                        repL_LOG.fnINSERT_L_LOG_BY_PARAMETERS("MSG001", "M001", "@USER_ID=" + clsL_WORK.fsCREATED_BY.Trim() + ";@USER_NAME= ;@DATA_TYPE=WORK;@RESULT=失敗;", Request.UserHostAddress, Newtonsoft.Json.JsonConvert.SerializeObject(clsL_WORK), clsARC_DOC.fsUPDATED_BY.Trim());
                                }
                            }

                        }



                        //刪除暫存路徑
                        if (System.IO.Directory.GetFiles(fsTEMP_FOLDER).Count() == 0)
                            System.IO.Directory.Delete(fsTEMP_FOLDER);
                    }

                    Response.Status = "200 OK";
                    Response.StatusCode = 200;
                }
                catch (Exception ex)
                {
                    //刪除暫存路徑
                    if (System.IO.Directory.GetFiles(fsTEMP_FOLDER).Count() == 0)
                        System.IO.Directory.Delete(fsTEMP_FOLDER);

                    Response.Status = "503 Service Unavailable";
                    Response.StatusCode = 503;
                }

            }
            else
            {
                //刪除暫存路徑
                if (System.IO.Directory.GetFiles(fsTEMP_FOLDER).Count() == 0)
                    System.IO.Directory.Delete(fsTEMP_FOLDER);

                Response.Status = "404 Not Found";
                Response.StatusCode = 404;
            }
        }
    }
}