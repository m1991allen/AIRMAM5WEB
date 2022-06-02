using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.FileUpload.Common;
using AIRMAM5.FileUpload.Models;
using AIRMAM5.Utility.Common;
using AIRMAM5.Utility.Extensions;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;

namespace AIRMAM5.FileUpload.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/UploadOld")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UploadController : ApiController
    {
        private ILogger _logger = Log.Logger;
        readonly SerilogService _serilogService = new SerilogService();
        readonly UsersService _usersService = new UsersService();
        readonly TblLogService _tblLogService = new TblLogService();
        readonly ConfigService _configService = new ConfigService();
        readonly TblWorkService _tblWorkService = new TblWorkService();
        readonly SubjectService _subjectService = new SubjectService();

        /// <summary>
        /// 檔案上傳API
        /// </summary>
        /// <returns></returns>
        [Route("UploadFile")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadFile()
        {
            VerifyResult result = new VerifyResult(true, "Success!!");
            HttpRequest Req = HttpContext.Current.Request;
            var rForm = Req.Form;
            var rFiles = Req.Files;
            #region _Serilog 
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "Upload",
                Method = "[UploadFile]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { ReqForm = rForm, ReqFiles = rFiles },
                LogString = "檔案上傳.Begin"
            });
            #endregion

            try
            {
                #region ----Form表單欄位資訊
                int.TryParse(rForm["resumableChunkNumber"].ToString(), out int resumableChunkNumber); //當前上傳中塊的索引。第一個塊是1（此處不以0為基數）。
                int.TryParse(rForm["resumableTotalChunks"].ToString(), out int resumableTotalChunks); //塊總數。
                long.TryParse(rForm["resumableChunkSize"].ToString(), out long resumableChunkSize); //常規塊大小。
                long.TryParse(rForm["resumableCurrentChunkSize"].ToString(), out long resumableCurrentChunkSize); //當前塊大小
                double.TryParse(rForm["resumableTotalSize"].ToString(), out double resumableTotalSize); //總文件大小。
                string resumableFilename = rForm["resumableFilename"] == null ? string.Empty : rForm["resumableFilename"].ToString() //原始文件名
                    , resumableIdentifier = rForm["resumableIdentifier"] == null ? string.Empty : rForm["resumableIdentifier"].ToString(); //請求中包含的文件的唯一標識符

                //檔案類型(MEDIATYPE_TO_V、MEDIATYPE_TO_A、MEDIATYPE_TO_P、MEDIATYPE_TO_D)
                string mediaType = rForm["MediaType"].ToString()
                    //主題編號
                    , subjId = rForm["SubjId"].ToString()
                    //上傳者
                    , loginId = rForm["LoginId"].ToString()
                    //標題(若為自訂標題)
                    , customTitle = rForm["CustomTitle"].ToString()
                    //被置換的檔案編號(新上傳不用寫值)
                    , fileNo = rForm["FileNo"].ToString()
                    //上傳暫存資料夾名稱
                    , folder = rForm["Folder"].ToString()
                    //置換後是否要刪除關鍵影格
                    , deleteKF = rForm["DeleteKF"].ToString() ?? string.Empty;

                ////標題定義(1.檔名為標題、2.主題標題為標題、3.自訂標題)、4.預編詮釋資料標題 為標題
                int.TryParse(rForm["TitleDefine"].ToString(), out int titleDefine);
                //預編詮釋資料編號，不用預編傳0
                int.TryParse(rForm["PreId"].ToString(), out int arcPreid);
                //Added_20200302:檔案機密等級
                int.TryParse(rForm["FileSecret"].ToString(), out int fileSecret);
                #endregion

                #region ----暫存路徑與目標路徑
                //暫存路徑 fsTEMP_FOLDER
                string tempFolder = _configService.GetConfigBy("UPLOAD_FILE_PATH").FirstOrDefault().fsVALUE + loginId + @"\" + folder + @"\" + Path.GetFileNameWithoutExtension(resumableFilename);
                //目標路徑 fsTARGET_FOLDER
                string targetFolder = _configService.GetConfigBy("UPLOAD_FILE_PATH").FirstOrDefault().fsVALUE + loginId;
                //暫存檔案 fsCHUNK_FILE
                string tempFile = string.Format(@"{0}\{1}.part{2}", tempFolder, resumableFilename, resumableChunkNumber.ToString("0000"));
                //目標檔案 fsTARGET_FILE
                string targetFile = string.Format(@"{0}\{1}", targetFolder, resumableFilename);

                if (!Directory.Exists(tempFolder)) { Directory.CreateDirectory(tempFolder); }
                if (!Directory.Exists(targetFolder)) { Directory.CreateDirectory(targetFolder); }
                #endregion ----路徑 End

                if (Req.Files.Count != 1)
                {
                    //刪除暫存路徑
                    if (Directory.GetFiles(tempFolder).Count() == 0) { Directory.Delete(tempFolder); }
                    result = new VerifyResult(false, "404 Not Found");
                    return new ResponseMessageResult(Request.CreateResponse(HttpStatusCode.NotFound, result));
                }

                // 暫存檔案FOLDER 判斷
                if (!Directory.Exists(tempFolder)) { Directory.CreateDirectory(tempFolder); }
                // 儲存暫存檔案
                if (!File.Exists(tempFile)) { Req.Files[0].SaveAs(tempFile); }

                result.IsSuccess = true;
                result.Message = "200 OK";

                // 儲存資料夾中的檔案
                string[] partFiles = Directory.GetFiles(tempFolder, "*.*");
                long partFilesTotalSize = 0;
                foreach (string name in partFiles)
                {
                    //FileInfo info = new FileInfo(name);
                    //partFilesTotalSize += info.Length;
                    partFilesTotalSize += new FileInfo(name).Length;
                }

                #region -----檢查暫存檔案大小 是否都完成上傳---
                MediaProcessModel updParam = new MediaProcessModel();//處理的參數打包model
                //if ((partFiles.Length + 1) * (long)resumableChunkSize >= resumableTotalSize)
                if (partFilesTotalSize >= resumableTotalSize)
                {
                    //產生識別檔案
                    foreach (var file in Directory.EnumerateFiles(tempFolder, "*.prt"))
                    {
                        File.Delete(file);
                    }
                    File.AppendAllText(tempFolder + @"\" + resumableChunkNumber.ToString("0000") + ".prt", string.Empty);

                    //亂數等待，看自己是不是最後一個
                    Thread.Sleep(new Random().Next(1, 5) * 1000);

                    //檢查自己的part是不是跟prt一樣
                    string[] prtFiles = Directory.GetFiles(tempFolder, "*.prt");

                    if (prtFiles != null && prtFiles.Count() > 0 && Path.GetFileNameWithoutExtension(prtFiles[0]) == resumableChunkNumber.ToString("0000"))
                    {
                        if (File.Exists(targetFile)) { File.Delete(targetFile); }
                        //開始合併檔案
                        using (var fs = new FileStream(targetFile, FileMode.CreateNew))
                        {
                            foreach (string file in partFiles.OrderBy(x => x))
                            {
                                var buffer = File.ReadAllBytes(file);
                                fs.Write(buffer, 0, buffer.Length);
                                File.Delete(file);
                            }
                        }

                        //Thread.Sleep(1000);
                        //處理的參數打包model
                        updParam = new MediaProcessModel
                        {
                            TempFolder = tempFolder,
                            TempFile = tempFile,
                            TargetFolder = targetFolder,
                            TargetFile = targetFile,
                            //
                            ResumableChunkNumber = resumableChunkNumber,
                            ResumableFilename = resumableFilename,
                            ResumableIdentifier = resumableIdentifier,
                            ResumableChunkSize = resumableChunkSize,
                            ResumableTotalSize = resumableTotalSize,
                            SubjId = subjId,
                            MediaType = mediaType,
                            TitleDefine = titleDefine,
                            CustomTitle = customTitle,
                            PreId = arcPreid,
                            LoginId = loginId,
                            FileNo = fileNo,
                            Folder = folder,
                            DeleteKeyframe = deleteKF,
                            FileSecret = fileSecret //Added_20200302:機密等級。
                        };

                        string _name = _usersService.GetBy("", updParam.LoginId, "").FirstOrDefault().fsNAME;
                        VerifyResult res = new VerifyResult(true);
                        switch (mediaType)
                        {
                            case "MEDIATYPE_TO_V":
                                res = await SaveFileVedio(updParam, _name);
                                break;
                            case "MEDIATYPE_TO_A":
                                res = await SaveFileAudio(updParam, _name);
                                break;
                            case "MEDIATYPE_TO_P":
                                res = await SaveFilePhoto(updParam, _name);
                                break;
                            case "MEDIATYPE_TO_D":
                                res = await SaveFileDoc(updParam, _name);
                                break;
                        }

                        //刪除暫存路徑
                        Directory.Delete(tempFolder, true);

                        if (!res.IsSuccess)
                        {
                            result.IsSuccess = false;
                            result.Message = "503 Service Unavailable";
                            //return new ResponseMessageResult(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, result));
                        }
                        else
                        {
                            result.IsSuccess = true;
                            result.Message = "檔案【" + resumableFilename + "】上傳完成";
                        }
                    }
                }
                #endregion -----檢查暫存檔 是否都完成上傳---END

                
                #region _Serilog 
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload",
                    Method = "[UploadFile]",
                    EventLevel = SerilogLevelEnum.Debug,
                    Input = new { Param = updParam, Result = result },
                    LogString = "檔案上傳.Result"
                });
                #endregion
                //return new ResponseMessageResult(Request.CreateResponse(HttpStatusCode.OK, result));
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                #region _Serilog 
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload",
                    Method = "[UploadFile]",
                    EventLevel = SerilogLevelEnum.Debug,
                    Input = new { Result = result },
                    LogString = "檔案上傳.Exception",
                    ErrorMessage = ex.ToString()
                });
                #endregion
                return new ResponseMessageResult(Request.CreateResponse(HttpStatusCode.BadRequest, result));
            }

            return new ResponseMessageResult(Request.CreateResponse(HttpStatusCode.OK, result));
        }

        /// <summary>
        /// 影片上傳後資料記錄(db): 新上傳檔案與置換檔案。
        /// </summary>
        /// <param name="m"></param>
        /// <param name="unm">上傳者顯示名稱 dbo.tbmUSERS.[fsNAME] </param>
        /// <returns></returns>
        [NonAction]
        public async Task<VerifyResult> SaveFileVedio(MediaProcessModel m, string unm)
        {
            var _paramd = new { UploadModel = m, UserName = unm };
            VerifyResult result = new VerifyResult();
            bool _ischg = (string.IsNullOrEmpty(m.FileNo)) ? false : true; //是否為置換(上傳檔案)

            try
            {
                #region ===== 參數設置 =====
                var _videoService = new ArcVideoService();
                string fsFILE_NAME = Path.GetFileName(m.TargetFile)  //-沒使用的參數
                    , fsEXT = Path.GetExtension(m.TargetFile).Replace(".", "")
                    , fsFILE_TYPE = fsEXT
                    , fsFILE_TYPE_H = string.Empty
                    , fsFILE_TYPE_L = string.Empty
                    , fsFILE_SIZE = m.ResumableTotalSize.ToString()//(m.ResumableTotalSize >= 1024 * 1024 * 1024 ? (Math.Round(m.ResumableTotalSize / 1024 / 1024 / 1024, 2).ToString() + "GB") : (m.ResumableTotalSize >= 1024 * 1024 ? (Math.Round(m.ResumableTotalSize / 1024 / 1024, 2).ToString() + "MB") : (m.ResumableTotalSize >= 1024 ? (Math.Round(m.ResumableTotalSize / 1024, 2).ToString() + "KB") : m.ResumableTotalSize.ToString())))
                    , fsFILE_SIZE_H = m.ResumableTotalSize.ToString()
                    , fsFILE_SIZE_L = string.Empty
                    , fsFILE_PATH = string.Empty
                    , fsFILE_PATH_H = string.Empty
                    , fsFILE_PATH_L = string.Empty
                    , fxMEDIA_INFO = string.Empty
                    //找完整路徑的key (from dbo.tbzCOFIG)
                    , _fullPathKey = "MEDIA_FOLDER_V_H", _fullPathKey_H = "MEDIA_FOLDER_V_H", _fullPathKey_L = "MEDIA_FOLDER_V_L";
                #endregion
                #region  ===== 判斷否為置換: 檔案編號、檔名處理 =====
                string _y = string.Format($"{DateTime.Now:yyyy}"), _m = string.Format($"{DateTime.Now:MM}"), _d = string.Format($"{DateTime.Now:dd}");
                if (_ischg)
                {
                    //置換檔案
                    _y = m.FileNo.Substring(0, 4);
                    _m = m.FileNo.Substring(4, 2);
                    _d = m.FileNo.Substring(6, 2);
                }
                else
                {
                    //新上傳的:取得檔案編號
                    string _type = "ARC", _head = string.Format($"{DateTime.Now:yyyyMMdd}");
                    m.FileNo = new TblNoService().GetNewNo(_type, "媒體資料檔", _head, "_", 7, m.LoginId);
                    //根據使用者選擇決定標題從哪裡來(若為3，則直接使用傳進來的標題)
                    if (m.TitleDefine == 1)
                    {
                        m.CustomTitle = Path.GetFileNameWithoutExtension(m.TargetFile);
                    }
                    else if (m.TitleDefine == 2)
                    {
                        m.CustomTitle = _subjectService.GetBy(m.SubjId).fsTITLE;
                    }
                    //ADDED_20200320:增加「4.預編詮釋資料標題 為標題」
                    else if (m.TitleDefine == 4 && m.PreId > 0)
                    {
                        m.CustomTitle = new ArcPreService().GetById(m.PreId).fsTITLE;
                    }
                }
                #endregion
                #region ===== 處理步驟 =====
                //1、組成完整路徑
                fsFILE_PATH = string.Format(@"{0}{1}\{2}\{3}\", _configService.GetConfigBy(_fullPathKey).FirstOrDefault().fsVALUE, _y, _m, _d);
                fsFILE_PATH_H = string.Format(@"{0}{1}\{2}\{3}\", _configService.GetConfigBy(_fullPathKey_H).FirstOrDefault().fsVALUE, _y, _m, _d);
                fsFILE_PATH_L = string.Format(@"{0}{1}\{2}\{3}\", _configService.GetConfigBy(_fullPathKey_L).FirstOrDefault().fsVALUE, _y, _m, _d);
                //2、搬至高解區
                if (!Directory.Exists(fsFILE_PATH_H)) { Directory.CreateDirectory(fsFILE_PATH_H); }
                if (File.Exists(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT)) { File.Delete(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT); }
                File.Move(m.TargetFile, fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);

                //3、抓取MediaInfo
                string _mediaInfo = ConfigurationManager.AppSettings["fsMEDIA_INFO"].ToString()
                        , _tempPath = ConfigurationManager.AppSettings["fsTEMP_PATH"].ToString();
                CommonLib.Global.SetEXE_MEDIAINFO_PATH(_mediaInfo);
                CommonLib.Global.SetWORKING_TEMP_DIR(_tempPath);
                CommonLib.Class.Media.VideoFile v = new CommonLib.Class.Media.VideoFile(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);
                fxMEDIA_INFO = clsMEDIA_INFO.fnGET_FILE_MEDIA_INFO(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);
                v.GetVideoInfo();

                //4、置換 或 新上傳 db.tbmARC_VIDEO
                tbmARC_VIDEO _VIDEO = new tbmARC_VIDEO();
                if (_ischg)
                {   ////更新資料庫  db.tbmARC_VIDEO
                    _VIDEO = _videoService.GetByFileno(m.FileNo);
                    _VIDEO.fsFILE_NO = m.FileNo;
                    _VIDEO.fsFILE_TYPE = fsFILE_TYPE;
                    _VIDEO.fsFILE_SIZE = fsFILE_SIZE;
                    _VIDEO.fsFILE_PATH = fsFILE_PATH;
                    _VIDEO.fsFILE_TYPE_H = fsFILE_TYPE;
                    _VIDEO.fsFILE_SIZE_H = fsFILE_SIZE_H;
                    _VIDEO.fsFILE_PATH_H = fsFILE_PATH_H;
                    _VIDEO.fsFILE_TYPE_L = string.Empty;
                    _VIDEO.fsFILE_SIZE_L = string.Empty;
                    _VIDEO.fsFILE_PATH_L = fsFILE_PATH_L;
                    _VIDEO.fsHEAD_FRAME = string.Empty;
                    _VIDEO.fdDURATION = (decimal)v.VideoDuration;
                    _VIDEO.fxMEDIA_INFO = fxMEDIA_INFO;
                    _VIDEO.fnWIDTH = v.VideoWidth;
                    _VIDEO.fnHEIGHT = v.VideoHeight;
                    _VIDEO.fsRESOL_TAG = string.Empty;
                    _VIDEO.fsUPDATED_BY = m.LoginId;
                    _VIDEO.fdUPDATED_DATE = DateTime.Now;
                    //Added_20200302:機密等級。
                    _VIDEO.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());
                    //
                    result = _videoService.ChangeVideo(_VIDEO, m.DeleteKeyframe);
                }
                else
                {
                    ////新增至資料庫 db.tbmARC_VIDEO
                    _VIDEO.fsFILE_NO = m.FileNo;
                    _VIDEO.fsTITLE = StringExtensions.ReplaceStr(m.CustomTitle);
                    _VIDEO.fsDESCRIPTION = string.Empty;
                    _VIDEO.fsSUBJECT_ID = m.SubjId;
                    _VIDEO.fsFILE_STATUS = IsTrueFalseEnum.Y.ToString();
                    _VIDEO.fnFILE_SECRET = 0; //SP預設為0
                    _VIDEO.fsFILE_TYPE = fsFILE_TYPE;
                    _VIDEO.fsFILE_SIZE = fsFILE_SIZE;
                    _VIDEO.fsFILE_PATH = fsFILE_PATH;
                    _VIDEO.fsFILE_TYPE_H = fsFILE_TYPE;
                    _VIDEO.fsFILE_SIZE_H = fsFILE_SIZE_H;
                    _VIDEO.fsFILE_PATH_H = fsFILE_PATH_H;
                    _VIDEO.fsFILE_TYPE_L = string.Empty;
                    _VIDEO.fsFILE_SIZE_L = string.Empty;
                    _VIDEO.fsFILE_PATH_L = fsFILE_PATH_L;
                    _VIDEO.fsHEAD_FRAME = string.Empty;
                    _VIDEO.fdDURATION = (decimal)v.VideoDuration;
                    _VIDEO.fxMEDIA_INFO = fxMEDIA_INFO;
                    _VIDEO.fnWIDTH = v.VideoWidth;
                    _VIDEO.fnHEIGHT = v.VideoHeight;
                    _VIDEO.fsRESOL_TAG = string.Empty;
                    _VIDEO.fsCREATED_BY = m.LoginId;
                    _VIDEO.fdCREATED_DATE = DateTime.Now;
                    //Added_20200302:機密等級。
                    _VIDEO.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());
                    //
                    result = _videoService.CreateBy(_VIDEO, m.PreId);
                }
                #endregion

                string _str = result.IsSuccess ? "成功" : "失敗", _act = (_ischg ? "置換" : "") + "影片檔案";

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    //M001=[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    //M022=[@USER_ID(@USER_NAME)] 置換 [@DATA_TYPE] 資料 @RESULT
                    (_ischg ? "M022" : "M001"),
                    string.Format(FormatString.LogParams, m.LoginId, (unm ?? string.Empty), _act, _str),
                    string.Format($"位置: "),
                    JsonConvert.SerializeObject(_VIDEO),
                    User.Identity.Name);
                #endregion

                if (result.IsSuccess)
                {
                    string _mediaV2 = ConfigurationManager.AppSettings["fsMEDIA_V_TO"].ToString()
                        , _media2 = _mediaV2
                        , _param = string.Format($"V;{m.FileNo};{m.SubjId};{_media2};");

                    ////新增tblWORK---- 
                    tblWORK _WORK = new tblWORK()
                    {
                        fsTYPE = "TRANSCODE",
                        fsPARAMETERS = _param,
                        fsSTATUS = "00",
                        fsPROGRESS = "0",
                        fsPRIORITY = "5",
                        fsRESULT = string.Empty,
                        fsNOTE = string.Empty,
                        fsCREATED_BY = m.LoginId,
                        fdCREATED_DATE = DateTime.Now,
                        C_ITEM_ID = m.FileNo,
                        C_ARC_TYPE = FileTypeEnum.V.ToString()
                    };
                    var res = _tblWorkService.CreateBy(_WORK);
                    _str = res.IsSuccess ? "成功" : "失敗";
                    #region _DB LOG
                    _tblLogService.Insert_L_Log(
                        TbzCodeIdEnum.MSG001.ToString(),
                        "M001",     //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                        string.Format(FormatString.LogParams, m.LoginId, (unm ?? string.Empty), "tblWORK_轉檔工作", _str),
                        string.Format($"位置: "),
                        JsonConvert.SerializeObject(_WORK),
                        User.Identity.Name);
                    #endregion
                }

                #region _Serilog 
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[SaveFileVedio]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _paramd, Result = result },
                    LogString = "影片上傳.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[SaveFileVedio]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _paramd, Exception = ex },
                    LogString = "影片上傳.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return await Task.Run(() => result);
        }

        /// <summary>
        /// 聲音檔案上傳後資料記錄(db): 新上傳檔案與置換檔案。
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<VerifyResult> SaveFileAudio(MediaProcessModel m, string unm)
        {
            var _paramd = new { UploadModel = m, UserName = unm };
            VerifyResult result = new VerifyResult();
            bool _ischg = (string.IsNullOrEmpty(m.FileNo)) ? false : true; //是否為置換(上傳檔案)
            try
            {
                #region ===== 參數設置 =====
                string fsFILE_NAME = Path.GetFileName(m.TargetFile)  //-沒使用的參數
                    , fsEXT = Path.GetExtension(m.TargetFile).Replace(".", "")
                    , fsFILE_TYPE = fsEXT
                    , fsFILE_TYPE_H = string.Empty
                    , fsFILE_TYPE_L = string.Empty
                    , fsFILE_SIZE = m.ResumableTotalSize.ToString()//(m.ResumableTotalSize >= 1024 * 1024 * 1024 ? (Math.Round(m.ResumableTotalSize / 1024 / 1024 / 1024, 2).ToString() + "GB") : (m.ResumableTotalSize >= 1024 * 1024 ? (Math.Round(m.ResumableTotalSize / 1024 / 1024, 2).ToString() + "MB") : (m.ResumableTotalSize >= 1024 ? (Math.Round(m.ResumableTotalSize / 1024, 2).ToString() + "KB") : m.ResumableTotalSize.ToString())))
                    , fsFILE_SIZE_H = m.ResumableTotalSize.ToString()
                    , fsFILE_SIZE_L = string.Empty
                    , fsFILE_PATH = string.Empty
                    , fsFILE_PATH_H = string.Empty
                    , fsFILE_PATH_L = string.Empty
                    , fxMEDIA_INFO = string.Empty
                    //找完整路徑的key (from dbo.tbzCOFIG)
                    , _fullPathKey = "MEDIA_FOLDER_A", _fullPathKey_H = "MEDIA_FOLDER_A", _fullPathKey_L = "MEDIA_FOLDER_A";
                #endregion
                #region ===== 判斷是否為置換: 檔案編號、檔名處理 =====
                string _y = string.Format($"{DateTime.Now:yyyy}"), _m = string.Format($"{DateTime.Now:MM}"), _d = string.Format($"{DateTime.Now:dd}");
                if (_ischg)
                {
                    //置換檔案
                    _y = m.FileNo.Substring(0, 4);
                    _m = m.FileNo.Substring(4, 2);
                    _d = m.FileNo.Substring(6, 2);
                }
                else
                {
                    //新上傳的:取得檔案編號
                    string _type = "ARC", _head = string.Format($"{DateTime.Now:yyyyMMdd}");
                    m.FileNo = new TblNoService().GetNewNo(_type, "媒體資料檔", _head, "_", 7, m.LoginId);

                    //根據使用者選擇決定標題從哪裡來(若為3，則直接使用傳進來的標題)
                    if (m.TitleDefine == 1)
                    {
                        m.CustomTitle = Path.GetFileNameWithoutExtension(m.TargetFile);
                    }
                    else if (m.TitleDefine == 2)
                    {
                        m.CustomTitle = _subjectService.GetBy(m.SubjId).fsTITLE;
                    }
                    //ADDED_20200320:增加「4.預編詮釋資料標題 為標題」
                    else if (m.TitleDefine == 4 && m.PreId > 0)
                    {
                        m.CustomTitle = new ArcPreService().GetById(m.PreId).fsTITLE;
                    }
                }
                #endregion
                #region ===== 處理步驟 =====
                //1、組成完整路徑
                fsFILE_PATH = string.Format(@"{0}{1}\{2}\{3}\", _configService.GetConfigBy(_fullPathKey).FirstOrDefault().fsVALUE, _y, _m, _d);
                fsFILE_PATH_H = string.Format(@"{0}{1}\{2}\{3}\", _configService.GetConfigBy(_fullPathKey_H).FirstOrDefault().fsVALUE, _y, _m, _d);
                fsFILE_PATH_L = string.Format(@"{0}{1}\{2}\{3}\", _configService.GetConfigBy(_fullPathKey_L).FirstOrDefault().fsVALUE, _y, _m, _d);
                //2、搬至高解區
                if (!Directory.Exists(fsFILE_PATH_H)) { Directory.CreateDirectory(fsFILE_PATH_H); }
                if (File.Exists(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT)) { File.Delete(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT); }
                File.Move(m.TargetFile, fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);
                //3.產生專輯封面
                var audioFile = TagLib.File.Create(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);
                bool _albumPic = false;
                if (audioFile.Tag.Pictures.Length > 0)
                {
                    MemoryStream ms = new MemoryStream(audioFile.Tag.Pictures[0].Data.Data);
                    ms.Seek(0, SeekOrigin.Begin);
                    using (FileStream file = new FileStream(fsFILE_PATH_H + m.FileNo + ".jpg", FileMode.Create, FileAccess.Write))
                    {
                        byte[] bytes = new byte[ms.Length];
                        ms.Read(bytes, 0, (int)ms.Length - 1);
                        file.Write(bytes, 0, bytes.Length - 1);
                    }
                    ms.Close();
                    ms.Dispose();
                    _albumPic = true;
                }
                //4.抓取MediaInfo
                //fxMEDIA_INFO = clsMEDIA_INFO.fnGET_FILE_MEDIA_INFO(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);
                string _mediaInfo = ConfigurationManager.AppSettings["fsMEDIA_INFO"].ToString()
                        , _tempPath = ConfigurationManager.AppSettings["fsTEMP_PATH"].ToString();
                CommonLib.Global.SetEXE_MEDIAINFO_PATH(_mediaInfo);
                CommonLib.Global.SetWORKING_TEMP_DIR(_tempPath);
                CommonLib.Class.Media.AudioFile a = new CommonLib.Class.Media.AudioFile(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);
                fxMEDIA_INFO = clsMEDIA_INFO.fnGET_FILE_MEDIA_INFO(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);
                a.GetAudioInfo();

                //5、新增或更新 db.tbmARC_AUDIO
                tbmARC_AUDIO _AUDIO = new tbmARC_AUDIO();
                ArcAudioService _audioService = new ArcAudioService();
                if (_ischg)
                {   ////更新資料庫  db.tbmARC_AUDIO
                    _AUDIO = _audioService.GetByFileno(m.FileNo);
                    _AUDIO.fsFILE_NO = m.FileNo;
                    _AUDIO.fsFILE_TYPE = fsFILE_TYPE;
                    _AUDIO.fsFILE_SIZE = fsFILE_SIZE;
                    _AUDIO.fsFILE_PATH = fsFILE_PATH;
                    _AUDIO.fsFILE_TYPE_H = fsFILE_TYPE;
                    _AUDIO.fsFILE_SIZE_H = fsFILE_SIZE_H;
                    _AUDIO.fsFILE_PATH_H = fsFILE_PATH_H;
                    _AUDIO.fsFILE_TYPE_L = string.Empty;
                    _AUDIO.fsFILE_SIZE_L = string.Empty;
                    _AUDIO.fsFILE_PATH_L = fsFILE_PATH_L;
                    _AUDIO.fdDURATION = (decimal)a.AudioDuration;
                    _AUDIO.fxMEDIA_INFO = fxMEDIA_INFO;
                    _AUDIO.fsALBUM = audioFile.Tag.Album;
                    _AUDIO.fsALBUM_TITLE = audioFile.Tag.Title;
                    _AUDIO.fsALBUM_ARTISTS = string.Join(";", audioFile.Tag.AlbumArtists);
                    _AUDIO.fsALBUM_PERFORMERS = string.Join(";", audioFile.Tag.Performers);
                    _AUDIO.fsALBUM_COMPOSERS = string.Join(";", audioFile.Tag.Composers);
                    _AUDIO.fnALBUM_YEAR = (short)audioFile.Tag.Year;
                    _AUDIO.fsALBUM_COPYRIGHT = audioFile.Tag.Copyright;
                    _AUDIO.fsALBUM_GENRES = string.Join(";", audioFile.Tag.Genres);
                    _AUDIO.fsALBUM_COMMENT = audioFile.Tag.Comment;
                    _AUDIO.fsUPDATED_BY = m.LoginId;
                    //Added_20200302:機密等級。
                    _AUDIO.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());
                    //
                    result = _audioService.ChangeAudio(_AUDIO, m.DeleteKeyframe);
                }
                else
                {   ////新增至資料庫 db.tbmARC_AUDIO
                    _AUDIO.fsFILE_NO = m.FileNo;
                    _AUDIO.fsTITLE = StringExtensions.ReplaceStr(m.CustomTitle);
                    _AUDIO.fsDESCRIPTION = string.Empty;
                    _AUDIO.fsSUBJECT_ID = m.SubjId;
                    _AUDIO.fsFILE_STATUS = IsTrueFalseEnum.Y.ToString();
                    _AUDIO.fnFILE_SECRET = 0; //SP預設為0
                    _AUDIO.fsFILE_TYPE = fsFILE_TYPE;
                    _AUDIO.fsFILE_SIZE = fsFILE_SIZE;
                    _AUDIO.fsFILE_PATH = fsFILE_PATH;
                    _AUDIO.fsFILE_TYPE_H = fsFILE_TYPE;
                    _AUDIO.fsFILE_SIZE_H = fsFILE_SIZE_H;
                    _AUDIO.fsFILE_PATH_H = fsFILE_PATH_H;
                    _AUDIO.fsFILE_TYPE_L = string.Empty;
                    _AUDIO.fsFILE_SIZE_L = string.Empty;
                    _AUDIO.fsFILE_PATH_L = fsFILE_PATH_L;
                    _AUDIO.fdDURATION = (decimal)a.AudioDuration;
                    _AUDIO.fxMEDIA_INFO = fxMEDIA_INFO;
                    //
                    _AUDIO.fsALBUM = audioFile.Tag.Album;
                    _AUDIO.fsALBUM_TITLE = audioFile.Tag.Title;
                    _AUDIO.fsALBUM_ARTISTS = string.Join(";", audioFile.Tag.AlbumArtists);
                    _AUDIO.fsALBUM_PERFORMERS = string.Join(";", audioFile.Tag.Performers);
                    _AUDIO.fsALBUM_COMPOSERS = string.Join(";", audioFile.Tag.Composers);
                    _AUDIO.fnALBUM_YEAR = (short)audioFile.Tag.Year;
                    _AUDIO.fsALBUM_COPYRIGHT = audioFile.Tag.Copyright;
                    _AUDIO.fsALBUM_GENRES = string.Join(";", audioFile.Tag.Genres);
                    _AUDIO.fsALBUM_COMMENT = audioFile.Tag.Comment;
                    _AUDIO.fcALBUM_PICTURE = _albumPic ? "Y" : "N";
                    _AUDIO.fsCREATED_BY = m.LoginId;
                    _AUDIO.fdCREATED_DATE = DateTime.Now;
                    //Added_20200302:機密等級。
                    _AUDIO.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());
                    //
                    result = _audioService.CreateBy(_AUDIO, m.PreId);
                }
                #endregion

                string _str = result.IsSuccess ? "成功" : "失敗", _act = (_ischg ? "置換" : "") + "聲音檔案";
                #region _Serilog 
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[SaveFileAudio]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _paramd, Result = result },
                    LogString = "聲音上傳.Result"
                });
                #endregion

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    //M001=[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    //M022=[@USER_ID(@USER_NAME)] 置換 [@DATA_TYPE] 資料 @RESULT
                    (_ischg ? "M022" : "M001"),
                    string.Format(FormatString.LogParams, m.LoginId, unm, _act, _str),
                    string.Format($"位置: "),
                    JsonConvert.SerializeObject(_AUDIO),
                    User.Identity.Name);
                #endregion
                if (result.IsSuccess)
                {
                    string _mediaA2 = ConfigurationManager.AppSettings["fsMEDIA_A_TO"].ToString()
                        , _media2 = _mediaA2
                        , _param = string.Format($"A;{m.FileNo};{m.SubjId};{_media2};");

                    ////新增tblWORK----
                    tblWORK _WORK = new tblWORK()
                    {
                        fsTYPE = "TRANSCODE",
                        fsPARAMETERS = _param, //"A;" + fsFILE_NO + ";" + fsSUBJ_ID + ";" + Properties.Settings.Default.fsMEDIA_A_TO + ";";
                        fsSTATUS = "00",
                        fsPROGRESS = "0",
                        fsPRIORITY = "5",
                        fsRESULT = string.Empty,
                        fsNOTE = string.Empty,
                        fsCREATED_BY = m.LoginId,
                        fdCREATED_DATE = DateTime.Now,
                        C_ITEM_ID = m.FileNo,
                        C_ARC_TYPE = FileTypeEnum.A.ToString()
                    };
                    var res = _tblWorkService.CreateBy(_WORK);
                    _str = res.IsSuccess ? "成功" : "失敗";
                    #region _DB LOG
                    _tblLogService.Insert_L_Log(
                        TbzCodeIdEnum.MSG001.ToString(),
                        "M001",        //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                        string.Format(FormatString.LogParams, m.LoginId, unm, "tblWORK_轉檔工作", _str),
                        string.Format($"位置: "),
                        JsonConvert.SerializeObject(_WORK),
                        User.Identity.Name);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[SaveFileAudio]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _paramd, Result = result, Exception = ex },
                    LogString = "聲音上傳.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return await Task.Run(() => result);
        }

        /// <summary>
        /// 圖片檔案上傳後資料記錄(db): 新上傳檔案與置換檔案。
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<VerifyResult> SaveFilePhoto(MediaProcessModel m, string unm)
        {
            var _paramd = new { UploadModel = m, UserName = unm };
            VerifyResult result = new VerifyResult();
            bool _ischg = (string.IsNullOrEmpty(m.FileNo)) ? false : true; //是否為置換(上傳檔案)
            try
            {
                #region ===== 參數設置 =====
                string fsFILE_NAME = Path.GetFileName(m.TargetFile)  //-沒使用的參數
                    , fsEXT = Path.GetExtension(m.TargetFile).Replace(".", "")
                    , fsFILE_TYPE = fsEXT
                    , fsFILE_TYPE_H = string.Empty
                    , fsFILE_TYPE_L = string.Empty
                    , fsFILE_SIZE = m.ResumableTotalSize.ToString()//(m.ResumableTotalSize >= 1024 * 1024 * 1024 ? (Math.Round(m.ResumableTotalSize / 1024 / 1024 / 1024, 2).ToString() + "GB") : (m.ResumableTotalSize >= 1024 * 1024 ? (Math.Round(m.ResumableTotalSize / 1024 / 1024, 2).ToString() + "MB") : (m.ResumableTotalSize >= 1024 ? (Math.Round(m.ResumableTotalSize / 1024, 2).ToString() + "KB") : m.ResumableTotalSize.ToString())))
                    , fsFILE_SIZE_H = m.ResumableTotalSize.ToString()
                    , fsFILE_SIZE_L = string.Empty
                    , fsFILE_PATH = string.Empty
                    , fsFILE_PATH_H = string.Empty
                    , fsFILE_PATH_L = string.Empty
                    , fxMEDIA_INFO = string.Empty
                    //找完整路徑的key (from dbo.tbzCOFIG)
                    , _fullPathKey = "MEDIA_FOLDER_P", _fullPathKey_H = "MEDIA_FOLDER_P", _fullPathKey_L = "MEDIA_FOLDER_P";
                #endregion
                #region ===== 判斷是否為置換: 檔案編號、檔名處理 =====
                string _y = string.Format($"{DateTime.Now:yyyy}"), _m = string.Format($"{DateTime.Now:MM}"), _d = string.Format($"{DateTime.Now:dd}");
                if (_ischg)
                {
                    //置換檔案
                    _y = m.FileNo.Substring(0, 4);
                    _m = m.FileNo.Substring(4, 2);
                    _d = m.FileNo.Substring(6, 2);
                }
                else
                {
                    //新上傳的:取得檔案編號
                    string _type = "ARC", _head = string.Format($"{DateTime.Now:yyyyMMdd}");
                    m.FileNo = new TblNoService().GetNewNo(_type, "媒體資料檔", _head, "_", 7, m.LoginId);

                    //根據使用者選擇決定標題從哪裡來(若為3，則直接使用傳進來的標題)
                    if (m.TitleDefine == 1)
                    {
                        m.CustomTitle = Path.GetFileNameWithoutExtension(m.TargetFile);
                    }
                    else if (m.TitleDefine == 2)
                    {
                        m.CustomTitle = _subjectService.GetBy(m.SubjId).fsTITLE;
                    }
                    //ADDED_20200320:增加「4.預編詮釋資料標題 為標題」
                    else if (m.TitleDefine == 4 && m.PreId > 0)
                    {
                        m.CustomTitle = new ArcPreService().GetById(m.PreId).fsTITLE;
                    }
                }
                #endregion
                #region ===== 處理步驟 =====
                //1、組成完整路徑
                fsFILE_PATH = string.Format(@"{0}{1}\{2}\{3}\", _configService.GetConfigBy(_fullPathKey).FirstOrDefault().fsVALUE, _y, _m, _d);
                fsFILE_PATH_H = string.Format(@"{0}{1}\{2}\{3}\", _configService.GetConfigBy(_fullPathKey_H).FirstOrDefault().fsVALUE, _y, _m, _d);
                fsFILE_PATH_L = string.Format(@"{0}{1}\{2}\{3}\", _configService.GetConfigBy(_fullPathKey_L).FirstOrDefault().fsVALUE, _y, _m, _d);
                //2、搬至高解區
                if (!Directory.Exists(fsFILE_PATH_H)) { Directory.CreateDirectory(fsFILE_PATH_H); }
                if (File.Exists(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT)) { File.Delete(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT); }
                File.Move(m.TargetFile, fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);
                //3.抓取MediaInfo
                fxMEDIA_INFO = clsMEDIA_INFO.fnGET_FILE_MEDIA_INFO(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);
                //4.抓取Exif
                Dictionary<string, string> _dicEXIF = new clsIMAGE_EXIF().fnGET_IMAGE_EXIF(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);
                bool _exifExists = (_dicEXIF != null && _dicEXIF.Count > 0) ? true : false;

                //5、新增或更新 db.tbmARC_PHOTO
                tbmARC_PHOTO _PHOTO = new tbmARC_PHOTO();
                ArcPhotoService _photoService = new ArcPhotoService();
                if (_ischg)
                {   ////更新資料庫  db.tbmARC_PHOTO
                    _PHOTO = _photoService.GetByFileno(m.FileNo);
                    _PHOTO.fsFILE_NO = m.FileNo;
                    _PHOTO.fsFILE_TYPE = fsFILE_TYPE;
                    _PHOTO.fsFILE_SIZE = fsFILE_SIZE;
                    _PHOTO.fsFILE_PATH = fsFILE_PATH;
                    _PHOTO.fsFILE_TYPE_H = fsFILE_TYPE;
                    _PHOTO.fsFILE_SIZE_H = fsFILE_SIZE_H;
                    _PHOTO.fsFILE_PATH_H = fsFILE_PATH_H;
                    _PHOTO.fsFILE_TYPE_L = string.Empty;
                    _PHOTO.fsFILE_SIZE_L = string.Empty;
                    _PHOTO.fsFILE_PATH_L = fsFILE_PATH_L;
                    _PHOTO.fxMEDIA_INFO = fxMEDIA_INFO;
                    //
                    _PHOTO.fnWIDTH = _exifExists ? int.Parse(_dicEXIF["fnWIDTH"]) : 0;
                    _PHOTO.fnHEIGHT = _exifExists ? int.Parse(_dicEXIF["fnHEIGHT"]) : 0;
                    _PHOTO.fnXDPI = _exifExists ? int.Parse(_dicEXIF["fnXDPI"]) : 0;
                    _PHOTO.fnYDPI = _exifExists ? int.Parse(_dicEXIF["fnYDPI"]) : 0;
                    _PHOTO.fsCAMERA_MAKE = _exifExists ? _dicEXIF["fsCAMERA_MAKE"] : string.Empty;
                    _PHOTO.fsCAMERA_MODEL = _exifExists ? _dicEXIF["fsCAMERA_MODEL"] : string.Empty;
                    _PHOTO.fsFOCAL_LENGTH = _exifExists ? _dicEXIF["fsFOCAL_LENGTH"] : string.Empty;
                    _PHOTO.fsEXPOSURE_TIME = _exifExists ? _dicEXIF["fsEXPOSURE_TIME"] : string.Empty;
                    _PHOTO.fsAPERTURE = _exifExists ? _dicEXIF["fsAPERTURE"] : string.Empty;
                    _PHOTO.fnISO = _exifExists ? int.Parse(_dicEXIF["fnISO"]) : 0;
                    _PHOTO.fsUPDATED_BY = m.LoginId;
                    _PHOTO.fdUPDATED_DATE = DateTime.Now;
                    //Added_20200302:機密等級。
                    _PHOTO.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());
                    //
                    result = _photoService.UpdateBy(_PHOTO);
                }
                else
                {   ////新增至資料庫 db.tbmARC_PHOTO
                    _PHOTO.fsFILE_NO = m.FileNo;
                    _PHOTO.fsTITLE = StringExtensions.ReplaceStr(m.CustomTitle);
                    _PHOTO.fsDESCRIPTION = string.Empty;
                    _PHOTO.fsSUBJECT_ID = m.SubjId;
                    _PHOTO.fsFILE_STATUS = IsTrueFalseEnum.Y.ToString();
                    _PHOTO.fnFILE_SECRET = 0; //SP預設為0
                    _PHOTO.fsFILE_TYPE = fsFILE_TYPE;
                    _PHOTO.fsFILE_SIZE = fsFILE_SIZE;
                    _PHOTO.fsFILE_PATH = fsFILE_PATH;
                    _PHOTO.fsFILE_TYPE_H = fsFILE_TYPE;
                    _PHOTO.fsFILE_SIZE_H = fsFILE_SIZE_H;
                    _PHOTO.fsFILE_PATH_H = fsFILE_PATH_H;
                    _PHOTO.fsFILE_TYPE_L = string.Empty;
                    _PHOTO.fsFILE_SIZE_L = string.Empty;
                    _PHOTO.fsFILE_PATH_L = fsFILE_PATH_L;
                    //
                    _PHOTO.fxMEDIA_INFO = fxMEDIA_INFO;
                    _PHOTO.fnWIDTH = _exifExists ? int.Parse(_dicEXIF["fnWIDTH"]) : 0;
                    _PHOTO.fnHEIGHT = _exifExists ? int.Parse(_dicEXIF["fnHEIGHT"]) : 0;
                    _PHOTO.fnXDPI = _exifExists ? int.Parse(_dicEXIF["fnXDPI"]) : 0;
                    _PHOTO.fnYDPI = _exifExists ? int.Parse(_dicEXIF["fnYDPI"]) : 0;
                    _PHOTO.fsCAMERA_MAKE = _exifExists ? _dicEXIF["fsCAMERA_MAKE"] : string.Empty;
                    _PHOTO.fsCAMERA_MODEL = _exifExists ? _dicEXIF["fsCAMERA_MODEL"] : string.Empty;
                    _PHOTO.fsFOCAL_LENGTH = _exifExists ? _dicEXIF["fsFOCAL_LENGTH"] : string.Empty;
                    _PHOTO.fsEXPOSURE_TIME = _exifExists ? _dicEXIF["fsEXPOSURE_TIME"] : string.Empty;
                    _PHOTO.fsAPERTURE = _exifExists ? _dicEXIF["fsAPERTURE"] : string.Empty;
                    _PHOTO.fnISO = _exifExists ? int.Parse(_dicEXIF["fnISO"]) : 0;
                    _PHOTO.fsCREATED_BY = m.LoginId;
                    _PHOTO.fdCREATED_DATE = DateTime.Now;
                    //Added_20200302:機密等級。
                    _PHOTO.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());
                    //
                    result = _photoService.CreateBy(_PHOTO, m.PreId);
                }
                #endregion

                string _str = result.IsSuccess ? "成功" : "失敗", _act = (_ischg ? "置換" : "") + "圖片檔案";
                #region _Serilog 
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[SaveFilePhoto]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _paramd, Result = result },
                    LogString = "圖片上傳.Result"
                });
                #endregion

                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    //M001=[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    //M022=[@USER_ID(@USER_NAME)] 置換 [@DATA_TYPE] 資料 @RESULT
                    (_ischg ? "M022" : "M001"),
                    string.Format(FormatString.LogParams, m.LoginId, unm, _act, _str),
                    string.Format($"位置: "),
                    JsonConvert.SerializeObject(_PHOTO),
                    User.Identity.Name);
                #endregion

                if (result.IsSuccess)
                {
                    string _mediaP2 = ConfigurationManager.AppSettings["fsMEDIA_P_TO"].ToString()
                        , _media2 = _mediaP2
                        , _param = string.Format($"P;{m.FileNo};{m.SubjId};{_media2};");

                    ////新增tblWORK----
                    tblWORK _WORK = new tblWORK()
                    {
                        fsTYPE = "TRANSCODE",
                        fsPARAMETERS = _param,
                        fsSTATUS = "00",
                        fsPROGRESS = "0",
                        fsPRIORITY = "5",
                        fsRESULT = string.Empty,
                        fsNOTE = string.Empty,
                        fsCREATED_BY = m.LoginId,
                        fdCREATED_DATE = DateTime.Now,
                        C_ITEM_ID = m.FileNo,
                        C_ARC_TYPE = FileTypeEnum.P.ToString()
                    };
                    var res = _tblWorkService.CreateBy(_WORK);
                    _str = res.IsSuccess ? "成功" : "失敗";
                    #region _DB LOG
                    _tblLogService.Insert_L_Log(
                        TbzCodeIdEnum.MSG001.ToString(),
                        "M001",        //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                        string.Format(FormatString.LogParams, m.LoginId, unm, "tblWORK_轉檔工作", _str),
                        string.Format($"位置: "),
                        JsonConvert.SerializeObject(_WORK),
                        User.Identity.Name);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[SaveFilePhoto]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _paramd, Result = result, Exception = ex },
                    LogString = "圖片上傳.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return await Task.Run(() => result);
        }

        /// <summary>
        /// 文件檔案上傳後資料記錄(db): 新上傳檔案與置換檔案。
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<VerifyResult> SaveFileDoc(MediaProcessModel m, string unm)
        {
            var _paramd = new { UploadModel = m, UserName = unm };
            VerifyResult result = new VerifyResult();
            bool _ischg = (string.IsNullOrEmpty(m.FileNo)) ? false : true; //是否為置換(上傳檔案)
            try
            {
                #region ===== 參數設置 =====
                string fsFILE_NAME = Path.GetFileName(m.TargetFile)  //-沒使用的參數
                    , fsEXT = Path.GetExtension(m.TargetFile).Replace(".", "")
                    , fsFILE_TYPE = fsEXT
                    , fsFILE_TYPE_H = string.Empty
                    , fsFILE_TYPE_L = string.Empty
                    , fsFILE_SIZE = m.ResumableTotalSize.ToString()//(m.ResumableTotalSize >= 1024 * 1024 * 1024 ? (Math.Round(m.ResumableTotalSize / 1024 / 1024 / 1024, 2).ToString() + "GB") : (m.ResumableTotalSize >= 1024 * 1024 ? (Math.Round(m.ResumableTotalSize / 1024 / 1024, 2).ToString() + "MB") : (m.ResumableTotalSize >= 1024 ? (Math.Round(m.ResumableTotalSize / 1024, 2).ToString() + "KB") : m.ResumableTotalSize.ToString())))
                    , fsFILE_SIZE_H = m.ResumableTotalSize.ToString()
                    , fsFILE_SIZE_L = string.Empty
                    , fsFILE_PATH = string.Empty
                    , fsFILE_PATH_H = string.Empty
                    , fsFILE_PATH_L = string.Empty
                    , fxMEDIA_INFO = string.Empty
                    //找完整路徑的key (from dbo.tbzCOFIG)
                    , _fullPathKey = "MEDIA_FOLDER_D", _fullPathKey_H = "MEDIA_FOLDER_D", _fullPathKey_L = "MEDIA_FOLDER_D";
                #endregion
                #region ===== 判斷是否為置換: 檔案編號、檔名處理 =====
                string _y = string.Format($"{DateTime.Now:yyyy}"), _m = string.Format($"{DateTime.Now:MM}"), _d = string.Format($"{DateTime.Now:dd}");
                if (_ischg)
                {
                    //置換檔案
                    _y = m.FileNo.Substring(0, 4);
                    _m = m.FileNo.Substring(4, 2);
                    _d = m.FileNo.Substring(6, 2);
                }
                else
                {
                    //新上傳的:取得檔案編號
                    string _type = "ARC", _head = string.Format($"{DateTime.Now:yyyyMMdd}");
                    m.FileNo = new TblNoService().GetNewNo(_type, "媒體資料檔", _head, "_", 7, m.LoginId);
                    //根據使用者選擇決定標題從哪裡來(若為3，則直接使用傳進來的標題)
                    if (m.TitleDefine == 1)
                    {
                        m.CustomTitle = Path.GetFileNameWithoutExtension(m.TargetFile);
                    }
                    else if (m.TitleDefine == 2)
                    {
                        m.CustomTitle = _subjectService.GetBy(m.SubjId).fsTITLE;
                    }
                    //ADDED_20200320:增加「4.預編詮釋資料標題 為標題」
                    else if (m.TitleDefine == 4 && m.PreId > 0)
                    {
                        m.CustomTitle = new ArcPreService().GetById(m.PreId).fsTITLE;
                    }
                }
                #endregion
                #region ===== 處理步驟 =====
                //1、組成完整路徑
                fsFILE_PATH = string.Format(@"{0}{1}\{2}\{3}\", _configService.GetConfigBy(_fullPathKey).FirstOrDefault().fsVALUE, _y, _m, _d);
                //fsFILE_PATH_H = string.Format(@"{0}{1}\{2}\{3}\", new ConfigService().GetConfigBy(_fullPathKey_H).FirstOrDefault().fsVALUE, _y, _m, _d);
                //fsFILE_PATH_L = string.Format(@"{0}{1}\{2}\{3}\", new ConfigService().GetConfigBy(_fullPathKey_L).FirstOrDefault().fsVALUE, _y, _m, _d);
                //2、搬至高解區
                if (!Directory.Exists(fsFILE_PATH)) Directory.CreateDirectory(fsFILE_PATH);
                if (File.Exists(fsFILE_PATH + m.FileNo + "." + fsEXT)) File.Delete(fsFILE_PATH + m.FileNo + "." + fsEXT);
                File.Move(m.TargetFile, fsFILE_PATH + m.FileNo + "." + fsEXT);
                //3.抓取MediaInfo
                fxMEDIA_INFO = clsMEDIA_INFO.fnGET_FILE_MEDIA_INFO(fsFILE_PATH + m.FileNo + "." + fsEXT);

                //4.判斷是否為用ifilter或轉txt的項目，若不可轉，則也不用送work了
                string[] fsTO_IFILTER_EXTs = _configService.GetConfigBy("MEDIATYPE_TO_IFILTER").FirstOrDefault().fsVALUE.Split(';');
                string[] fsTO_TXT_EXTs = _configService.GetConfigBy("MEDIATYPE_TO_TXT").FirstOrDefault().fsVALUE.Split(';');
                bool fbIS_TO_IFILTER = false, fbIS_TO_TXT = false;
                if (fsTO_IFILTER_EXTs != null && fsTO_IFILTER_EXTs.Count() > 0)
                {
                    if (fsTO_IFILTER_EXTs.FirstOrDefault(f => f == fsEXT.ToLower()) != null) { fbIS_TO_IFILTER = true; }
                }
                if (fsTO_TXT_EXTs != null && fsTO_TXT_EXTs.Count() > 0)
                {
                    if (fsTO_TXT_EXTs.FirstOrDefault(f => f == fsEXT.ToLower()) != null) { fbIS_TO_TXT = true; }
                }

                FileInfo fileInfo = new FileInfo(fsFILE_PATH + m.FileNo + "." + fsEXT);

                //5.新增或更新資料表 dbo.tbmARC_DOC
                tbmARC_DOC _DOC = new tbmARC_DOC();
                ArcDocService _docService = new ArcDocService();
                if (_ischg)
                {   ////更新資料庫  db.tbmARC_DOC
                    _DOC = _docService.GetByFileno(m.FileNo);
                    _DOC.fsFILE_NO = m.FileNo;
                    _DOC.fsFILE_TYPE = fsFILE_TYPE;
                    _DOC.fsFILE_SIZE = fsFILE_SIZE;
                    _DOC.fsFILE_PATH = fsFILE_PATH;
                    _DOC.fxMEDIA_INFO = fxMEDIA_INFO;
                    _DOC.fdFILE_CREATED_DATE = fileInfo.CreationTime;
                    _DOC.fdFILE_UPDATED_DATE = fileInfo.LastWriteTime;
                    _DOC.fsUPDATED_BY = m.LoginId;
                    //Added_20200302:機密等級。
                    _DOC.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());
                    //
                    result = _docService.UpdateBy(_DOC);
                }
                else
                {
                    _DOC.fsFILE_NO = m.FileNo;
                    _DOC.fsTITLE = StringExtensions.ReplaceStr(m.CustomTitle);
                    _DOC.fsDESCRIPTION = string.Empty;
                    _DOC.fsSUBJECT_ID = m.SubjId;
                    _DOC.fsFILE_STATUS = IsTrueFalseEnum.Y.ToString();
                    _DOC.fnFILE_SECRET = 0; //SP預設為0
                    _DOC.fsFILE_TYPE = fsFILE_TYPE;
                    _DOC.fsFILE_SIZE = fsFILE_SIZE;
                    _DOC.fsFILE_PATH = fsFILE_PATH;
                    _DOC.fxMEDIA_INFO = fxMEDIA_INFO;
                    _DOC.fsCONTENT = string.Empty;
                    _DOC.fdFILE_CREATED_DATE = fileInfo.CreationTime;
                    _DOC.fdFILE_UPDATED_DATE = fileInfo.LastWriteTime;
                    _DOC.fsCREATED_BY = m.LoginId;
                    _DOC.fdCREATED_DATE = DateTime.Now;
                    //Added_20200302:機密等級。
                    _DOC.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());
                    //
                    result = _docService.CreateBy(_DOC, m.PreId);
                }
                #endregion

                string _str = result.IsSuccess ? "成功" : "失敗", _act = (_ischg ? "置換" : "") + "文件檔案";
                #region _Serilog 
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[SaveFileDoc]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _paramd, Result = result },
                    LogString = "文件上傳.Result"
                });
                #endregion
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    //M001=[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                    //M022=[@USER_ID(@USER_NAME)] 置換 [@DATA_TYPE] 資料 @RESULT
                    (_ischg ? "M022" : "M001"),
                    string.Format(FormatString.LogParams, m.LoginId, unm, _act, _str),
                    string.Format($"位置: "),
                    JsonConvert.SerializeObject(_DOC),
                    User.Identity.Name);
                #endregion

                if (result.IsSuccess)
                {
                    //下面用不到原始檔案了，就把檔案加密
                    CommonSecurity.AddFileEncryption(fsFILE_PATH + m.FileNo + "." + fsEXT, fsFILE_PATH + m.FileNo + ".enc");
                    //
                    string _media2 = string.Empty;
                    if (!fbIS_TO_IFILTER && !fbIS_TO_TXT) _media2 = string.Empty; else if (fbIS_TO_TXT) _media2 = "X"; else if (fbIS_TO_IFILTER) _media2 = "L";
                    string _param = string.Format($"D;{m.FileNo};{m.SubjId};{_media2};");

                    //新增tblWORK----
                    tblWORK _WORK = new tblWORK()
                    {
                        fsTYPE = "TRANSCODE",
                        fsPARAMETERS = _param,
                        fsSTATUS = "00",
                        fsPROGRESS = "0",
                        fsPRIORITY = "5",
                        fsRESULT = string.Empty,
                        fsNOTE = string.Empty,
                        fsCREATED_BY = m.LoginId,
                        fdCREATED_DATE = DateTime.Now,
                        C_ITEM_ID = m.FileNo,
                        C_ARC_TYPE = FileTypeEnum.D.ToString()
                    };
                    var res = _tblWorkService.CreateBy(_WORK);
                    _str = res.IsSuccess ? "成功" : "失敗";
                    #region _DB LOG
                    _tblLogService.Insert_L_Log(
                        TbzCodeIdEnum.MSG001.ToString(),
                        "M001",        //[@USER_ID(@USER_NAME)] 新增 [@DATA_TYPE] 資料 @RESULT
                        string.Format(FormatString.LogParams, m.LoginId, unm, "tblWORK_轉檔工作", _str),
                        string.Format($"位置: "),
                        JsonConvert.SerializeObject(_WORK),
                        User.Identity.Name);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[SaveFilePhoto]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _paramd, Result = result, Exception = ex },
                    LogString = "文件上傳.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return await Task.Run(() => result);
        }

        #region 【檔案解密後產生檢視用檔案、刪除暫檢視用檔案】
        /// <summary>
        /// 檔案解密後產生檢視用檔案路徑URL
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [Route("DecryptFiles")]
        [HttpPost]
        public VerifyResult DecryptFiles(ViewerFileModel m)
        {
            VerifyResult result = new VerifyResult() { Data = m };
            try
            {
                #region ---檢查:使用者id、主題與檔案編號
                //var ur = _usersService.FindUserByUserId(m.ViewUserId);
                var _is = _usersService.IsExists(m.ViewUserId);
                if (_is == false)
                {
                    result.Message = string.Format($"使用者錯誤 {m.ViewUserName} ");
                    _logger.Information(string.Format($"UploadAPI_DecryptFiles:\n {JsonConvert.SerializeObject(result)} "));
                    return result;
                }
                ////
                //var _doc = new ArcDocService().GetArcDocByIdFile(m.fsSUBJECT_ID, m.fsFILE_NO).FirstOrDefault();
                //if (_doc == null)
                //{
                //    result.Message = string.Format($"主題與檔案編號錯誤 {m.fsFILE_NO} ");
                //    _logger.Information(string.Format($"UploadAPI_DecryptFiles:\n {JsonConvert.SerializeObject(result)} "));
                //    return result;
                //}
                ////加密檔案實際路徑, 解密檔案暫存路徑
                string _tempFolder = _configService.GetConfigBy("DV_TEMP_FOLDER").FirstOrDefault().fsVALUE  //Document Viewer 還原暫存檔路徑
                    , _encFile = string.Empty  //檔案實際完整路徑 = \\172.20.142.35\media\D\2016\11\24\fileno.nec
                    , _tmpfnm = StringExtensions.GenerateRandomStr(12)   //解密後臨時檔名(12碼)
                    , _decFile = string.Format("{0}{1}.docx", _tempFolder, _tmpfnm)  //解密檔案暫存完整路徑
                    ;

                bool notFound = false;  //檔案不存在: true/false
                switch (m.Kind.ToUpper())
                {
                    case "DEL":
                        var _docDel = new T_tbmARC_IndexService().GetArcDocBySubjFile(m.fsSUBJECT_ID, m.fsFILE_NO).FirstOrDefault();
                        if (_docDel == null) { notFound = true; }

                        _encFile = string.Format("{0}{1}.enc", _docDel.fsFILE_PATH, m.fsFILE_NO);
                        _decFile = string.Format("{0}{1}.{2}", _tempFolder, _tmpfnm, _docDel.fsFILE_TYPE);
                        break;
                    case "NORMAL":
                    default:
                        var _doc = new ArcDocService().GetArcDocByIdFile(m.fsSUBJECT_ID, m.fsFILE_NO).FirstOrDefault();
                        if (_doc == null) { notFound = true; }

                        _encFile = string.Format("{0}{1}.enc", _doc.fsFILE_PATH, m.fsFILE_NO);
                        _decFile = string.Format("{0}{1}.{2}", _tempFolder, _tmpfnm, _doc.fsFILE_TYPE);
                        break;
                }
                if (notFound)
                {
                    result.Message = string.Format($"主題與檔案編號錯誤 {m.fsFILE_NO} ");
                    _logger.Information(string.Format($"UploadAPI_DecryptFiles:\n {JsonConvert.SerializeObject(result)} "));
                    return result;
                }
                #endregion

                //////加密檔案實際路徑, 解密檔案暫存路徑
                //string _encFile = string.Format("{0}{1}.enc", _doc.fsFILE_PATH, m.fsFILE_NO)  //檔案實際完整路徑 \\172.20.142.35\media\D\2016\11\24\fileno.nec
                //    , _tmpfnm = StringExtensions.GenerateRandomStr(12)   //解密後臨時檔名(12碼)
                //    , _decFile = string.Format("{0}{1}.{2}", _doc.fsDV_TEMP_FOLDER, _tmpfnm, _doc.fsFILE_TYPE); //解密檔案暫存完整路徑

                CommonSecurity.RemoveFileEncryption(_encFile, _decFile);

                result.IsSuccess = true;
                result.Message = "OK";
                result.Data = new ViewerTempFileNameModel { TempFileName = _tmpfnm };  //回覆 "檔名"(不包含路徑、副檔名)
                #region _Serilog 
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[DecryptFiles]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = m, Response = result },
                    LogString = "檔案解密檢視用檔案.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[DecryptFiles]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = m, Exception = ex },
                    LogString = "檔案解密檢視用檔案.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return result;
        }
        /// <summary>
        /// 刪除 檔案解密後的檢視用檔案
        /// </summary>
        /// <returns></returns>
        [Route("RemoveFile")]
        [HttpPost]
        public VerifyResult RemoveTempFile(ViewerRemoveFileModel m)
        {
            VerifyResult result = new VerifyResult() { Data = m };
            try
            {
                var _doc = new ArcDocService().GetArcDocByIdFile(m.fsSUBJECT_ID, m.fsFILE_NO).FirstOrDefault();
                string _ViewerTempPath = _doc == null ? string.Empty : _doc.fsDV_TEMP_FOLDER; //解密檔案-暫存檔案路徑
                if (Directory.Exists(_ViewerTempPath))
                {
                    if (File.Exists(_ViewerTempPath + m.ViewFileName))
                    {
                        File.Delete(_ViewerTempPath + m.ViewFileName);
                    }
                }

                result.IsSuccess = true;
                result.Message = "OK";
                #region _Serilog 
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[RemoveTempFile]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = m, Response = result },
                    LogString = "刪除檔案解密檢視用檔案.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[RemoveTempFile]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = m, Exception = ex },
                    LogString = "刪除檔案解密檢視用檔案.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return result;
        }
        #endregion

    }
}
