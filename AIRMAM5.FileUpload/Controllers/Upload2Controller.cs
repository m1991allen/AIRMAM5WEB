using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
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
using System.Diagnostics;
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
    [RoutePrefix("api/Upload")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class Upload2Controller : ApiController
    {
        readonly ISerilogService _serilogService;
        readonly ITblLogService _tblLogService;
        readonly ISubjectService _subjectService;
        readonly ArcPreService _arcPreService;
        /// <summary>
        /// T_tbmARC_IndexService 檔案刪除資料桶主檔 Service
        /// </summary>
        readonly T_tbmARC_IndexService _TtbmARC_IndexService;

        readonly UsersService _usersService;
        readonly ConfigService _configService;
        readonly TblWorkService _tblWorkService;
        readonly TblNoService _tblNoService;
        /// <summary>
        /// 系統取號分類
        /// </summary>
        readonly string _type = SysNumberTypeEnum.ARC.ToString();
        /// <summary>
        /// 系統取號的日期部分 ---> Modified_20201211_head內容由前端表單獲取。
        /// </summary>
        private string _head = string.Empty;//string.Format($"{DateTime.Now:yyyyMMdd}");
        private DateTime _headDT = DateTime.Now;
        private string _body = SysNumberBodySet.BodyARC;
        private string _name = SysNumberNameSet.NameARC;
        /// <summary>
        /// 系統取號流水序號位數
        /// </summary>
        private int _len = (int)SysNumberLenEnum.ARC;
        public Upload2Controller()
        {
            _serilogService = new SerilogService();
            _tblLogService = new TblLogService(_serilogService);
            _subjectService = new SubjectService(_serilogService);
            _arcPreService = new ArcPreService(_serilogService);
            _TtbmARC_IndexService = new T_tbmARC_IndexService(_serilogService, _tblLogService);
            _usersService = new UsersService(_serilogService);
            _configService = new ConfigService();
            _tblWorkService = new TblWorkService();
            _tblNoService = new TblNoService();
        }

        public Upload2Controller(ISerilogService serilogService, ITblLogService tblLogService, ISubjectService subjectService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
            _subjectService = subjectService;
            _arcPreService = new ArcPreService(serilogService);
            _TtbmARC_IndexService = new T_tbmARC_IndexService(serilogService, tblLogService);
            _usersService = new UsersService(serilogService);
            _configService = new ConfigService();
            _tblWorkService = new TblWorkService();
            _tblNoService = new TblNoService();
        }

        /// <summary>
        /// 檔案上傳API
        /// </summary>
        /// <returns></returns>
        [Route("UploadFile")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadFile(CancellationToken cancellationToken)
        {
            VerifyResult result = new VerifyResult(true, "201 Created"); //201 or 204 
            HttpStatusCode statusCode = HttpStatusCode.Created;
            HttpRequest Req = HttpContext.Current.Request;
            var rForm = Req.Form;
            var rFiles = Req.Files;

            try
            {
                #region >>>>> Form表單欄位資訊, ResumableJS
                int.TryParse(rForm["resumableChunkNumber"].ToString(), out int resumableChunkNumber); //當前上傳中塊的索引。第一個塊是1（此處不以0為基數）。
                int.TryParse(rForm["resumableTotalChunks"].ToString(), out int resumableTotalChunks); //塊總數。
                long.TryParse(rForm["resumableChunkSize"].ToString(), out long resumableChunkSize); //常規塊大小。
                long.TryParse(rForm["resumableCurrentChunkSize"].ToString(), out long resumableCurrentChunkSize); //當前塊大小
                double.TryParse(rForm["resumableTotalSize"].ToString(), out double resumableTotalSize); //總文件大小。
                string resumableFilename = rForm["resumableFilename"] == null ? string.Empty : rForm["resumableFilename"].ToString() //原始文件名
                    , resumableIdentifier = rForm["resumableIdentifier"] == null ? string.Empty : rForm["resumableIdentifier"].ToString(); //請求中包含的文件的唯一標識符

                //檔案類型(MEDIATYPE_TO_V、MEDIATYPE_TO_A、MEDIATYPE_TO_P、MEDIATYPE_TO_D)
                string mediaType = rForm["MediaType"].ToString()
                    , subjId = rForm["SubjId"].ToString()           //主題編號
                    , loginId = rForm["LoginId"].ToString()         //上傳者
                    , customTitle = rForm["CustomTitle"].ToString() //標題(若為自訂標題)
                    , fileNo = rForm["FileNo"].ToString()           //被置換的檔案編號(新上傳不用寫值)
                    , folder = rForm["Folder"].ToString()           //上傳暫存資料夾名稱
                    , deleteKF = rForm["DeleteKF"].ToString() ?? string.Empty; //置換後是否要刪除關鍵影格

                //20201211_前端增加傳入:檔案編號日期(TIP_20210819:置換檔案上傳,無此參數)
                _head = rForm["DateInFileNo"] == null ? string.Empty : rForm["DateInFileNo"].ToString();
                if (DateTime.TryParse(_head, out _headDT))
                {
                    _head = string.Format($"{_headDT:yyyyMMdd}");
                }

                ////標題定義(1.檔名為標題、2.主題標題為標題、3.自訂標題)、4.預編詮釋資料標題 為標題
                int.TryParse(rForm["TitleDefine"].ToString(), out int titleDefine);
                //預編詮釋資料編號，不用預編傳0
                int.TryParse(rForm["PreId"].ToString(), out int arcPreid);
                //Added_20200302:檔案機密等級
                int.TryParse(rForm["FileSecret"].ToString(), out int fileSecret);
                //20210913_ADDED_檔案版權
                string fileLicense = string.IsNullOrEmpty(rForm["FileLicense"].ToString()) ? string.Empty : rForm["FileLicense"].ToString();
                #endregion
                #region _Serilog.Info
                var _Input = new
                {
                    ReqFiles = new
                    {
                        subjId,
                        customTitle,
                        fileNo
                    },
                    ResumableJS = new
                    {
                        resumableChunkNumber,
                        resumableTotalChunks,
                        resumableChunkSize,
                        resumableCurrentChunkSize,
                        resumableTotalSize,
                        resumableFilename,
                        resumableIdentifier
                    },
                };
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[UploadFile]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = _Input,
                    LogString = "檔案上傳_Input"
                });
                Debug.WriteLine(string.Format(" {0:yyyy-MM-dd HH:mm:ss} 【Upload-Input】 {1}", DateTime.Now, _Input));
                #endregion

                #region >>>>> 暫存路徑與目標路徑
                //目標路徑 fsTARGET_FOLDER
                string targetFolder = _configService.GetConfigBy("UPLOAD_FILE_PATH").FirstOrDefault().fsVALUE + loginId;
                //目標檔案 fsTARGET_FILE
                string targetFile = string.Format(@"{0}\{1}", targetFolder, resumableFilename);

                //暫存路徑 fsTEMP_FOLDER    //_20200520_上傳檔案大小不需要分割,不用暫存資料夾。
                string tempFolder = (resumableTotalChunks == 1) 
                    ? string.Empty 
                    : _configService.GetConfigBy("UPLOAD_FILE_PATH").FirstOrDefault().fsVALUE + loginId + @"\" + folder + @"\" + Path.GetFileNameWithoutExtension(resumableFilename);
                //暫存檔案(.part0000) fsCHUNK_FILE  //_20200520_上傳檔案大小不需要分割,不用產生暫存.part000檔案。
                string tempFile = (resumableTotalChunks == 1) 
                    ? string.Empty 
                    : string.Format(@"{0}\{1}.part{2}", tempFolder, resumableFilename, resumableChunkNumber.ToString("0000"));
                
                //_20200520_上傳檔案大小不需要分割,無需 part000檔案。
                if (resumableTotalChunks > 1) { if (!Directory.Exists(tempFolder)) { Directory.CreateDirectory(tempFolder); } }
                //_20200520_Marked_&_Modified_↑↑//if (!Directory.Exists(tempFolder)) { Directory.CreateDirectory(tempFolder); }
                if (!Directory.Exists(targetFolder)) { Directory.CreateDirectory(targetFolder); }
                #endregion ----路徑 End

                //刪除暫存路徑
                if (Req.Files.Count != 1)
                {
                    if (Directory.GetFiles(tempFolder).Count() == 0) { Directory.Delete(tempFolder); }
                    result.IsSuccess = false;
                    result.Message = "404 Not Found";
                    statusCode = HttpStatusCode.NotFound;
                    return new ResponseMessageResult(Request.CreateResponse(statusCode, result));
                }

                /*//_20200520_Marked_重複了// 暫存檔案FOLDER 判斷*/
                //if (!Directory.Exists(tempFolder)) { Directory.CreateDirectory(tempFolder); }
                // 儲存暫存檔案(分割檔案 *.part0000)
                if (resumableTotalChunks > 1)
                {
                    if (!File.Exists(tempFile)) { Req.Files[0].SaveAs(tempFile); }
                }
                else
                {   
                    if (!File.Exists(targetFile)) { Req.Files[0].SaveAs(targetFile); }  //_20200520_ADDed_
                }

                result.IsSuccess = true;
                result.Message = "檔案上傳處理中，完成將以訊息通知，請稍候~!";
                /* tip: 分割檔案續傳時，resumableJS前端偵測進度100% 即會立即顯示訊息, 並不會理會未完成的 檔案合併、資料庫儲存 程序。*/

                #region >>>>> 統計上傳檔案數量/大小
                string[] partFiles = Directory.GetFiles((string.IsNullOrEmpty(tempFolder) ? targetFolder : tempFolder), "*.*");
                double partFilesTotalSize = 0;
                foreach (string name in partFiles)
                {
                    partFilesTotalSize += new FileInfo(name).Length;    // 統計儲存資料夾中的檔案大小
                }
                #endregion

                MediaProcessModel updParam = new MediaProcessModel();   //處理的參數打包model
                VerifyResult res = new VerifyResult(true, "");          //資料庫處理Result

                //判斷上傳檔案大小、數量
                if (partFilesTotalSize >= resumableTotalSize && (resumableChunkNumber == resumableTotalChunks || partFiles.Count() == resumableChunkNumber))
                {
                    Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} ({Thread.CurrentThread.ManagedThreadId})【UploadFile】: resumableChunkNumber= {resumableChunkNumber} : 切割檔案上傳OK "));
                    #region _Serilog(Debug)
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "Upload2",
                        Method = "[UploadFile]",
                        EventLevel = SerilogLevelEnum.Debug,
                        Input = new { MESSAGE = string.Format($" ({Thread.CurrentThread.ManagedThreadId})【UploadFile】: resumableChunkNumber= {resumableChunkNumber} : 切割檔案上傳OK ") },
                        LogString = "檔案上傳_"
                    });
                    #endregion
                    //合併切割檔案
                    var mg = await FileMergeAsync(resumableTotalChunks, resumableChunkNumber, resumableTotalSize, (string.IsNullOrEmpty(tempFolder) ? targetFolder : tempFolder), targetFile).ConfigureAwait(false);

                    if (mg)
                    {
                        Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} ({Thread.CurrentThread.ManagedThreadId})【UploadFile】-已合併 : resumableChunkNumber= {resumableChunkNumber} : 資料庫處理_begin "));
                        #region _Serilog(Debug)
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "Upload2",
                            Method = "[UploadFile]",
                            EventLevel = SerilogLevelEnum.Debug,
                            Input = new { MESSAGE = string.Format($" ({Thread.CurrentThread.ManagedThreadId})【UploadFile】-已合併 : resumableChunkNumber= {resumableChunkNumber} : 資料庫處理_begin ") },
                            LogString = "檔案上傳_"
                        });
                        #endregion
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
                            FileSecret = fileSecret,
                            FileLicense = fileLicense
                        };

                        var _name = _usersService.GetBy(string.Empty, updParam.LoginId, string.Empty).FirstOrDefault();//.fsNAME;
                        switch (mediaType)
                        {
                            case "MEDIATYPE_TO_V":
                                res = await SaveFileVedio(updParam, _name.fsNAME).ConfigureAwait(false);
                                #region//TIP: 成功後,寫一筆通知訊息紀錄 tbmNOTIFY (暫只限定"影片",比較容易有大檔案發生)
                                if (res.IsSuccess)
                                {
                                    var arcv = (tbmARC_VIDEO)res.Data;
                                    await Task.Run(() =>
                                    {
                                        tbmNOTIFY _tbmNOTIFY = new tbmNOTIFY
                                        {
                                            fsTITLE = "檔案上傳處理",
                                            fsCONTENT = string.Format($"調用類型：影片檔<br />檔案編號：{arcv.fsFILE_NO}<br />檔案標題：{arcv.fsTITLE}<br />作業訊息：上傳入庫資料資訊儲存成功!"),
                                            fnCATEGORY = (int)NotifyCategoryEnum.預設,
                                            fsNOTICE_TARGET = string.Empty,
                                            fdEXPIRE_DATE = DateTime.Now.AddDays(7),
                                            fdCREATED_DATE = DateTime.Now,
                                            fsCREATED_BY = _name.fsLOGIN_ID
                                        };
                                        this.UploadNotify(_tbmNOTIFY, _name.fsUSER_ID);

                                        #region _Serilog(Debug)
                                        Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} ({Thread.CurrentThread.ManagedThreadId})【UploadFile】- 影片SAVE: 通知訊息_DONE "));
                                        _serilogService.SerilogWriter(new SerilogInputModel
                                        {
                                            Controller = "Upload2",
                                            Method = "[UploadFile]",
                                            EventLevel = SerilogLevelEnum.Debug,
                                            Input = new { MESSAGE = string.Format($" ({Thread.CurrentThread.ManagedThreadId})【UploadFile】- 影片SAVE: 通知訊息_DONE ") },
                                            LogString = "檔案上傳_"
                                        });
                                        #endregion
                                    }).ConfigureAwait(false);
                                }
                                #endregion
                                break;
                            case "MEDIATYPE_TO_A":
                                res = await SaveFileAudio(updParam, _name.fsNAME).ConfigureAwait(false);
                                break;
                            case "MEDIATYPE_TO_P":
                                res = await SaveFilePhoto(updParam, _name.fsNAME).ConfigureAwait(false);
                                break;
                            case "MEDIATYPE_TO_D":
                                res = await SaveFileDoc(updParam, _name.fsNAME).ConfigureAwait(false);
                                break;
                        }

                        #region _Serilog(Debug)
                        Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} ({Thread.CurrentThread.ManagedThreadId})【UploadFile】- resumableChunkNumber= {resumableChunkNumber} : 資料庫處理_end "));
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "Upload2",
                            Method = "[UploadFile]",
                            EventLevel = SerilogLevelEnum.Debug,
                            Input = string.Format($" ({Thread.CurrentThread.ManagedThreadId})【UploadFile】- resumableChunkNumber= {resumableChunkNumber} : 資料庫處理_end "),
                            LogString = "檔案上傳_"
                        });
                        #endregion
                        #region ✘✘-TIP: 新上傳檔案(非置換)成功後,更新DashBoard =>應該在轉檔完成後呼叫更新DashBoard
                        if (string.IsNullOrEmpty(updParam.FileNo))
                        {
                            await Task.Run(() =>
                            {
                                var _hubs = new HubServices();
                                _hubs.RefreshDashBoardCounter(_name.fsUSER_ID);
                                _hubs.HubDispose();
                            });
                        }
                        #endregion

                        //刪除暫存路徑
                        if (!string.IsNullOrEmpty(tempFolder)) { Directory.Delete(tempFolder, true); }

                        result.IsSuccess = res.IsSuccess;
                        result.Message = res.IsSuccess ? "檔案【" + resumableFilename + "】上傳完成" : string.Format($"503 Service Unavailable. ({res.Message})");
                        statusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ServiceUnavailable;
                    }
                }
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(string.Format($" [UploadFile] IS Exception!!! "));
                //Debug.WriteLine(string.Format($" [UploadFile]-Exception: {ex.Message} \n {ex.StackTrace}"));
                result.IsSuccess = false;
                result.Message = ex.Message;
                statusCode = HttpStatusCode.BadRequest;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[UploadFile]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Result = result, Exception = ex },
                    LogString = "檔案上傳_Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            Debug.WriteLine(string.Format(" {0:yyyy-MM-dd HH:mm:ss} 【Upload2_[UploadFile]_(API)檔案上傳.Response】- statusCode= {1} , result.Message= {2} ", DateTime.Now, statusCode, result.Message));
            #region _Serilog
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "Upload2",
                Method = "[UploadFile]",
                EventLevel = SerilogLevelEnum.Information,
                Input = string.Format(" 【Upload2_[UploadFile]_(API)檔案上傳.Response】- statusCode= {0} , result.Message= {1} ", statusCode, result.Message),
                LogString = "檔案上傳_Response"
            });
            #endregion
            return new ResponseMessageResult(Request.CreateResponse(statusCode, result));
        }

        /// <summary>
        /// 檢查上傳檔案
        /// </summary>
        /// <param name="totalchunks"></param>
        /// <param name="chunknumber"></param>
        /// <param name="totalsize"></param>
        /// <param name="tempfolder"></param>
        /// <param name="targetfile"></param>
        /// <returns></returns>
        private async Task<bool> CheckFileMergeAsync(int totalchunks, int chunknumber, double totalsize, string tempfolder, string targetfile)
        {
            bool result = false;

            try
            {
                await Task.Run(() =>
                {
                    // 統計儲存資料夾中的檔案
                    string[] partFiles = Directory.GetFiles(tempfolder, "*.*");

                    double partFilesTotalSize = 0;
                    foreach (string name in partFiles)
                    {
                        partFilesTotalSize += new FileInfo(name).Length;
                    }
                    //Debug.WriteLine(string.Format($" 【Task-Check】-[CheckFileMerge]-({Thread.CurrentThread.ManagedThreadId}): chunknumber={chunknumber} , partFilesTotalSize= {partFilesTotalSize} "));

                    if (partFilesTotalSize >= totalsize && (chunknumber == totalchunks || partFiles.Count() == chunknumber))
                    {
                        if (totalchunks > 1)
                        {
                            if (File.Exists(targetfile)) { File.Delete(targetfile); }
                            //合併檔案
                            using (var fs = new FileStream(targetfile, FileMode.CreateNew))
                            {
                                foreach (string file in partFiles.OrderBy(x => x))
                                {
                                    var buffer = File.ReadAllBytes(file);
                                    fs.Write(buffer, 0, buffer.Length);
                                    File.Delete(file);
                                }
                            }
                        }

                        result = true;
                        Debug.WriteLine(string.Format($" ({Thread.CurrentThread.ManagedThreadId})【Task-Check】-[CheckFileMerge]: chunknumber={chunknumber} -----完成上傳,資料記錄-----。 "));
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(string.Format($" ({Thread.CurrentThread.ManagedThreadId})【Task-Check】-[CheckFileMerge]: chunknumber={chunknumber} IS Exception!!! "));
                //Debug.WriteLine(string.Format($" ({Thread.CurrentThread.ManagedThreadId})【Task-Check】-[CheckFileMerge]-Exception: {ex.Message} \n {ex.StackTrace}"));
                #region _Serilog 
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[CheckFileMerge]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Result = result, Exception = ex },
                    LogString = "(API)檢查檔案與合併.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw;
            }
            return result;
        }

        /// <summary>
        /// 檔案合併處理async
        /// </summary>
        /// <param name="totalchunks">檔案(切割)分塊總數量 </param>
        /// <param name="chunknumber">塊數 </param>
        /// <param name="totalsize">檔案總大小 </param>
        /// <param name="tempfolder">檔案分塊上傳暫存資料夾 </param>
        /// <param name="targetfile">上傳檔案目標檔案 </param>
        /// <returns>布林值。True, False </returns>
        [NonAction]
        private Task<bool> FileMergeAsync(int totalchunks, int chunknumber, double totalsize, string tempfolder, string targetfile)
        {
            bool result = false;
            try
            {
                #region _Serilog.Debug
                Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} 【FileMergeAsync】({Thread.CurrentThread.ManagedThreadId}): chunknumber={chunknumber}/totalchunks={totalchunks} "));
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[FileMergeAsync]",
                    EventLevel = SerilogLevelEnum.Debug,
                    Input = string.Format($" 【FileMergeAsync】({Thread.CurrentThread.ManagedThreadId}): chunknumber={chunknumber}/totalchunks={totalchunks} "),
                    LogString = "檔案合併處理_"
                });
                #endregion
                // 統計儲存資料夾中的檔案
                string[] partFiles = Directory.GetFiles(tempfolder, "*.*");

                double partFilesTotalSize = 0;
                foreach (string name in partFiles)
                {
                    partFilesTotalSize += new FileInfo(name).Length;
                }
                Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} 【FileMergeAsync】({Thread.CurrentThread.ManagedThreadId}): chunknumber={chunknumber} , partFilesTotalSize= {partFilesTotalSize} "));

                if (partFilesTotalSize >= totalsize && (chunknumber == totalchunks || partFiles.Count() == chunknumber))
                {
                    if (totalchunks > 1)
                    {
                        if (File.Exists(targetfile)) { File.Delete(targetfile); }

                        //合併檔案
                        using (var fs = new FileStream(targetfile, FileMode.CreateNew))
                        {
                            #region _Serilog.Debug
                            Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} 【FileMergeAsync】({Thread.CurrentThread.ManagedThreadId})--------------------------合併.BEGIN: fileCount: {partFiles.Length} "));
                            _serilogService.SerilogWriter(new SerilogInputModel
                            {
                                Controller = "Upload2",
                                Method = "[FileMergeAsync]",
                                EventLevel = SerilogLevelEnum.Debug,
                                Input = new { MESSAGE = string.Format($" 【FileMergeAsync】({Thread.CurrentThread.ManagedThreadId})--------------------------合併.BEGIN: fileCount: {partFiles.Length} ") },
                                LogString = "檔案合併處理_"
                            });
                            #endregion
                            foreach (string file in partFiles.OrderBy(x => x))
                            {
                                var buffer = File.ReadAllBytes(file);
                                fs.Write(buffer, 0, buffer.Length);
                                File.Delete(file);
                            }

                            #region _Serilog.Debug
                            Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} 【FileMergeAsync】({Thread.CurrentThread.ManagedThreadId})--------------------------合併.END "));
                            _serilogService.SerilogWriter(new SerilogInputModel
                            {
                                Controller = "Upload2",
                                Method = "[FileMergeAsync]",
                                EventLevel = SerilogLevelEnum.Debug,
                                Input = new { MESSAGE = string.Format($" 【FileMergeAsync】({Thread.CurrentThread.ManagedThreadId})--------------------------合併.END ") },
                                LogString = "檔案合併處理_"
                            });
                            #endregion
                        }
                    }

                    result = true;
                    Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} 【FileMergeAsync】({Thread.CurrentThread.ManagedThreadId})-----完成檔案合併。 "));
                    #region _Serilog.Debug
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "Upload2",
                        Method = "[FileMergeAsync]",
                        EventLevel = SerilogLevelEnum.Debug,
                        Input = new { MESSAGE = string.Format($" 【FileMergeAsync】({Thread.CurrentThread.ManagedThreadId})-----完成檔案合併。 ") },
                        LogString = "檔案合併處理_"
                    });
                    #endregion
                }
            }
            catch (Exception ex)
            {                
                //Debug.WriteLine(string.Format($" 【FileMergeAsync】({Thread.CurrentThread.ManagedThreadId})-[Exception]: chunknumber={chunknumber}/totalchunks={totalchunks} , totalsize={totalsize} , tempfolder={tempfolder} , targetfile={targetfile} "));
                //Debug.WriteLine(string.Format($" 【FileMergeAsync】({Thread.CurrentThread.ManagedThreadId})-[Exception]-Exception: {ex.Message} \n {ex.StackTrace}"));
                #region _Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[FileMergeAsync]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Result = result, Exception = ex },
                    LogString = "檔案合併處理_Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw;
            }
            return Task.Run(() => result);
        }

        /// <summary>
        /// 通知訊息記錄
        /// </summary>
        /// <param name="notify"> </param>
        /// <param name="touserid">被通知的使用者Id </param>
        /// <returns></returns>
        [NonAction]
        private VerifyResult UploadNotify(tbmNOTIFY notify, string touserid)
        {
            NotifyService notifyService = new NotifyService();

            VerifyResult result = notifyService.Create(notify, "UploadAPI", touserid);
            if (result.IsSuccess)
            {
                Task.Run(()=>
                {
                    var _hubs = new HubServices();
                    _hubs.Upload2Notify(touserid);
                    _hubs.HubDispose();
                });
            }

            #region _Serilog.Debug
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = "Upload2",
                Method = "[UploadNotify]",
                EventLevel = SerilogLevelEnum.Debug,
                Input = new { tbmNOTIFY = notify, To = touserid, Result = result },
                LogString = "通知訊息新增_Result"
            });
            #endregion
            return result;
        }

        #region ===================【檔案資料記錄】
        /// <summary>
        /// 影片上傳後資料記錄(db): 新上傳檔案與置換檔案。
        /// </summary>
        /// <param name="m">處理參數 <see cref="MediaProcessModel"/></param>
        /// <param name="unm">上傳者顯示名稱 dbo.tbmUSERS.[fsNAME] </param>
        /// <returns></returns>
        [NonAction]
        public async Task<VerifyResult> SaveFileVedio(MediaProcessModel m, string unm)
        {
            VerifyResult result = new VerifyResult();
            bool _ischg = (string.IsNullOrEmpty(m.FileNo)) ? false : true; //是否為置換(上傳檔案)
            var _paramd = new { UploadModel = m, UserName = unm, IsExchange = _ischg };

            try
            {
                #region ===== 參數設置 =====
                var _videoService = new ArcVideoService(_serilogService);
                string fsFILE_NAME = Path.GetFileName(m.TargetFile)  //-沒使用的參數
                    //, originFileName = Path.GetFileName(m.TargetFile)   //+檔案名稱
                    , originFileName = Path.GetFileNameWithoutExtension(m.TargetFile)   //20200925_沒有副檔名的指定路徑字串的檔案名稱
                    , fsEXT = Path.GetExtension(m.TargetFile).Replace(".", "").ToLower()    //副檔名   //20200902副檔名一律改為小寫
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
                //string _y = string.Format($"{DateTime.Now:yyyy}"), _m = string.Format($"{DateTime.Now:MM}"), _d = string.Format($"{DateTime.Now:dd}");
                string _y = string.Format($"{_headDT:yyyy}"), _m = string.Format($"{_headDT:MM}"), _d = string.Format($"{_headDT:dd}");
                if (_ischg)
                {
                    //置換檔案
                    _y = m.FileNo.Substring(0, 4);
                    _m = m.FileNo.Substring(4, 2);
                    _d = m.FileNo.Substring(6, 2);
                    m.CustomTitle = originFileName; //+原始檔名:檔案名稱不包含副檔名
                }
                else
                {
                    //新上傳的:取得檔案編號  /* Modified_20201211_使用共用變數、head內容由前端表單獲取。 */
                    m.FileNo = _tblNoService.GetNewNo(_type, _name, _head, _body, _len, m.LoginId);
                    
                    //根據使用者選擇決定標題從哪裡來(若為3，則直接使用傳進來的標題)
                    if (m.TitleDefine == 1)
                    {
                        //m.CustomTitle = Path.GetFileNameWithoutExtension(m.TargetFile); //檔案名稱不包含副檔名
                        m.CustomTitle = originFileName; //+原始檔名:檔案名稱不包含副檔名
                    }
                    else if (m.TitleDefine == 2)
                    {
                        m.CustomTitle = _subjectService.GetBy(m.SubjId).fsTITLE;
                    }
                    //ADDED_20200320:增加「4.預編詮釋資料標題 為標題」
                    else if (m.TitleDefine == 4 && m.PreId > 0)
                    {
                        m.CustomTitle = _arcPreService.GetById(m.PreId).fsTITLE;
                    }
                }
                #endregion

                #region ===== 處理步驟 =====
                //1、組成完整路徑
                fsFILE_PATH = string.Format(@"{0}{1}\{2}\{3}\", _configService.GetConfigBy(_fullPathKey).FirstOrDefault().fsVALUE, _y, _m, _d);
                fsFILE_PATH_H = string.Format(@"{0}{1}\{2}\{3}\", _configService.GetConfigBy(_fullPathKey_H).FirstOrDefault().fsVALUE, _y, _m, _d);
                fsFILE_PATH_L = string.Format(@"{0}{1}\{2}\{3}\", _configService.GetConfigBy(_fullPathKey_L).FirstOrDefault().fsVALUE, _y, _m, _d);

                //2、搬至高解區
                #region _Serilog.Debug
                Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} 【SaveFileVedio】 處理步驟: 搬至高解區 ----- BEGIN "));
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[SaveFileVedio]",
                    EventLevel = SerilogLevelEnum.Debug,
                    Input = string.Format($" 【SaveFileVedio】 處理步驟: 搬至高解區 ----- BEGIN "),
                    LogString = "影片上傳DB記錄_"
                });
                #endregion
                if (!Directory.Exists(fsFILE_PATH_H)) { Directory.CreateDirectory(fsFILE_PATH_H); }
                if (File.Exists(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT)) { File.Delete(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT); }
                File.Move(m.TargetFile, fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);
                #region _Serilog.Debug
                Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} 【SaveFileVedio】 處理步驟: 搬至高解區 ----- END "));
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[SaveFileVedio]",
                    EventLevel = SerilogLevelEnum.Debug,
                    Input = string.Format($" 【SaveFileVedio】 處理步驟: 搬至高解區 ----- END "),
                    LogString = "影片上傳DB記錄_"
                });
                #endregion

                //3、抓取MediaInfo
                string _mediaInfo = ConfigurationManager.AppSettings["fsMEDIA_INFO"].ToString(), _tempPath = ConfigurationManager.AppSettings["fsTEMP_PATH"].ToString();
                CommonLib.Global.SetEXE_MEDIAINFO_PATH(_mediaInfo);
                CommonLib.Global.SetWORKING_TEMP_DIR(_tempPath);
                CommonLib.Class.Media.VideoFile v = new CommonLib.Class.Media.VideoFile(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);
                fxMEDIA_INFO = clsMEDIA_INFO.fnGET_FILE_MEDIA_INFO(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);

                //因為有時候會沒有取到相關資訊
                int retry = 0;
                do
                {
                    if (retry > 0) Thread.Sleep(2000);

                    v.GetVideoInfo();
                    retry += 1;

                } while ((v.VideoWidth == 0 || v.VideoHeight == 0 || v.VideoDuration == 0) && retry <= 3);

                if(v.VideoWidth == 0 || v.VideoHeight == 0 || v.VideoDuration == 0)
                {
                    result.IsSuccess = false;
                    result.Message = "取得Video Info失敗";
                    #region Serilog.Err
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "Upload2",
                        Method = "[SaveFileVedio]",
                        EventLevel = SerilogLevelEnum.Error,
                        Input = new { Param = _paramd, Exception = "取得Video Info失敗" },
                        LogString = "(API)影片上傳.Exception",
                        ErrorMessage = "取得Video Info失敗"
                    });
                    #endregion
                }

                //4、置換 或 新上傳 db.tbmARC_VIDEO
                #region _Serilog.Debug
                Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} 【SaveFileVedio】 處理步驟: DB處理[tbmARC_VIDEO] ----- BEGIN "));
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[SaveFileVedio]",
                    EventLevel = SerilogLevelEnum.Debug,
                    Input = string.Format($" 【SaveFileVedio】 處理步驟: DB處理[tbmARC_VIDEO] ----- BEGIN "),
                    LogString = "影片上傳DB記錄_"
                });
                #endregion

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
                    _VIDEO.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());
                    _VIDEO.fsORI_FILE_NAME = originFileName ?? string.Empty;    //+原始檔名(不含副檔名)
                    _VIDEO.fsLICENSE = m.FileLicense; //20210913_ADDED_檔案版權
                    //
                    result = _videoService.ChangeVideo(_VIDEO, m.DeleteKeyframe);
                }
                else
                {   ////新增至資料庫 db.tbmARC_VIDEO
                    _VIDEO.fsFILE_NO = m.FileNo;
                    _VIDEO.fsTITLE = StringExtensions.ReplaceStr(m.CustomTitle);
                    _VIDEO.fsDESCRIPTION = (m.PreId > 0 ? _arcPreService.GetById(m.PreId).fsDESCRIPTION : string.Empty);
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
                    _VIDEO.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());
                    _VIDEO.fsORI_FILE_NAME = originFileName ?? string.Empty;    //+原始檔名(不含副檔名)
                    _VIDEO.fsLICENSE = m.FileLicense; //20210913_ADDED_檔案版權
                    //
                    result = _videoService.CreateBy(_VIDEO, m.PreId);
                }
                #endregion ===== 處理步驟 =====

                string _str = result.IsSuccess ? "成功" : "失敗", _act = (_ischg ? "置換" : "") + "影片檔案";

                #region _Serilog.Debug
                Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} 【SaveFileVedio】 處理步驟: DB處理[tbmARC_VIDEO] ----- END "));
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[SaveFileVedio]",
                    EventLevel = SerilogLevelEnum.Debug,
                    Input = string.Format($" 【SaveFileVedio】 處理步驟: DB處理[tbmARC_VIDEO] ----- END : {_act }{ _str } "),
                    LogString = "影片上傳DB記錄_"
                });
                #endregion
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

                    #region _Serilog.Debug
                    Debug.WriteLine(string.Format($" {DateTime.Now:yyyy-MM-dd HH:mm:ss} 【SaveFileVedio】 處理步驟: DB處理[tblWORK] ----- END2 "));
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "Upload2",
                        Method = "[SaveFileVedio]",
                        EventLevel = SerilogLevelEnum.Debug,
                        Input = string.Format($" 【SaveFileVedio】 處理步驟: DB處理[tblWORK] ----- END2 : {_str }"),
                        LogString = "影片上傳DB記錄_"
                    });
                    #endregion
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

                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[SaveFileVedio]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _paramd, Result = result },
                    LogString = "(API)影片上傳.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[SaveFileVedio]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _paramd, Exception = ex },
                    LogString = "(API)影片上傳.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return await Task.Run(() => result);
        }

        /// <summary>
        /// 聲音檔案上傳後資料記錄(db): 新上傳檔案與置換檔案。
        /// </summary>
        /// <param name="m">處理參數 <see cref="MediaProcessModel"/></param>
        /// <param name="unm">上傳者顯示名稱 dbo.tbmUSERS.[fsNAME] </param>
        /// <returns></returns>
        [NonAction]
        public async Task<VerifyResult> SaveFileAudio(MediaProcessModel m, string unm)
        {
            VerifyResult result = new VerifyResult();
            bool _ischg = (string.IsNullOrEmpty(m.FileNo)) ? false : true; //是否為置換(上傳檔案)
            var _paramd = new { UploadModel = m, UserName = unm, IsExchange = _ischg };

            try
            {
                #region ===== 參數設置 =====
                string fsFILE_NAME = Path.GetFileName(m.TargetFile)  //-沒使用的參數
                    //, originFileName = Path.GetFileName(m.TargetFile)   //+檔案名稱
                    , originFileName = Path.GetFileNameWithoutExtension(m.TargetFile)   //20200925_沒有副檔名的指定路徑字串的檔案名稱
                    , fsEXT = Path.GetExtension(m.TargetFile).Replace(".", "").ToLower()          //副檔名   //20200902副檔名一律改為小寫
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
                //string _y = string.Format($"{DateTime.Now:yyyy}"), _m = string.Format($"{DateTime.Now:MM}"), _d = string.Format($"{DateTime.Now:dd}");
                string _y = string.Format($"{_headDT:yyyy}"), _m = string.Format($"{_headDT:MM}"), _d = string.Format($"{_headDT:dd}");
                if (_ischg)
                {
                    //置換檔案
                    _y = m.FileNo.Substring(0, 4);
                    _m = m.FileNo.Substring(4, 2);
                    _d = m.FileNo.Substring(6, 2);
                    m.CustomTitle = originFileName; //+原始檔名:檔案名稱不包含副檔名
                }
                else
                {
                    //新上傳的:取得檔案編號  /* Modified_20201211_使用共用變數、head內容由前端表單獲取。 */
                    m.FileNo = _tblNoService.GetNewNo(_type, _name, _head, _body, _len, m.LoginId);

                    //根據使用者選擇決定標題從哪裡來(若為3，則直接使用傳進來的標題)
                    if (m.TitleDefine == 1)
                    {
                        //m.CustomTitle = Path.GetFileNameWithoutExtension(m.TargetFile); //檔案名稱不包含副檔名
                        m.CustomTitle = originFileName; //+原始檔名:檔案名稱不包含副檔名
                    }
                    else if (m.TitleDefine == 2)
                    {
                        m.CustomTitle = _subjectService.GetBy(m.SubjId).fsTITLE;
                    }
                    //ADDED_20200320:增加「4.預編詮釋資料標題 為標題」
                    else if (m.TitleDefine == 4 && m.PreId > 0)
                    {
                        m.CustomTitle = _arcPreService.GetById(m.PreId).fsTITLE;
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
                
                //因為有時候會沒有取到相關資訊
                int retry = 0;
                do
                {
                    if (retry > 0) Thread.Sleep(2000);

                    a.GetAudioInfo();
                    retry += 1;
                } while (a.AudioDuration == 0 && retry <= 3);

                if(a.AudioDuration == 0)
                {
                    result.IsSuccess = false;
                    result.Message = "取得Audio Info失敗";
                    #region Serilog.Err
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "Upload2",
                        Method = "[SaveFileAudio]",
                        EventLevel = SerilogLevelEnum.Error,
                        Input = new { Param = _paramd, Result = result, Exception = "取得Audio Info失敗" },
                        LogString = "(API)聲音上傳.Exception",
                        ErrorMessage = "取得Audio Info失敗"
                    });
                    #endregion
                }

                //5、新增或更新 db.tbmARC_AUDIO
                tbmARC_AUDIO _AUDIO = new tbmARC_AUDIO();
                ArcAudioService _audioService = new ArcAudioService(_serilogService);
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
                    _AUDIO.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());  //+20200302:機密等級。
                    _AUDIO.fsORI_FILE_NAME = originFileName ?? string.Empty;  //+原始檔名(不含副檔名)
                    _AUDIO.fsLICENSE = m.FileLicense; //20210913_ADDED_檔案版權
                    //
                    result = _audioService.ChangeAudio(_AUDIO, m.DeleteKeyframe);
                }
                else
                {   ////新增至資料庫 db.tbmARC_AUDIO
                    _AUDIO.fsFILE_NO = m.FileNo;
                    _AUDIO.fsTITLE = StringExtensions.ReplaceStr(m.CustomTitle);
                    _AUDIO.fsDESCRIPTION = (m.PreId > 0 ? _arcPreService.GetById(m.PreId).fsDESCRIPTION : string.Empty);
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
                    _AUDIO.fcALBUM_PICTURE = _albumPic ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString();
                    _AUDIO.fsCREATED_BY = m.LoginId;
                    _AUDIO.fdCREATED_DATE = DateTime.Now;
                    _AUDIO.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());  //+20200302:機密等級。
                    _AUDIO.fsORI_FILE_NAME = originFileName ?? string.Empty;  //+原始檔名(不含副檔名)
                    _AUDIO.fsLICENSE = m.FileLicense; //20210913_ADDED_檔案版權
                    //
                    result = _audioService.CreateBy(_AUDIO, m.PreId);
                }
                #endregion

                string _str = result.IsSuccess ? "成功" : "失敗", _act = (_ischg ? "置換" : "") + "聲音檔案";

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
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[SaveFileAudio]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _paramd, Result = result },
                    LogString = "(API)聲音上傳.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[SaveFileAudio]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _paramd, Result = result, Exception = ex },
                    LogString = "(API)聲音上傳.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return await Task.Run(() => result);
        }

        /// <summary>
        /// 圖片檔案上傳後資料記錄(db): 新上傳檔案與置換檔案。
        /// </summary>
        /// <param name="m">處理參數 <see cref="MediaProcessModel"/></param>
        /// <param name="unm">上傳者顯示名稱 dbo.tbmUSERS.[fsNAME] </param>
        /// <returns></returns>
        [NonAction]
        public async Task<VerifyResult> SaveFilePhoto(MediaProcessModel m, string unm)
        {
            VerifyResult result = new VerifyResult();
            bool _ischg = (string.IsNullOrEmpty(m.FileNo)) ? false : true; //是否為置換(上傳檔案)
            var _paramd = new { UploadModel = m, UserName = unm, IsExchange = _ischg };

            try
            {
                #region ===== 參數設置 =====
                string fsFILE_NAME = Path.GetFileName(m.TargetFile)  //-沒使用的參數
                    //, originFileName = Path.GetFileName(m.TargetFile)   //+檔案名稱
                    , originFileName = Path.GetFileNameWithoutExtension(m.TargetFile)   //20200925_沒有副檔名的指定路徑字串的檔案名稱
                    , fsEXT = Path.GetExtension(m.TargetFile).Replace(".", "").ToLower()          //副檔名   //20200902副檔名一律改為小寫
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
                //string _y = string.Format($"{DateTime.Now:yyyy}"), _m = string.Format($"{DateTime.Now:MM}"), _d = string.Format($"{DateTime.Now:dd}");
                string _y = string.Format($"{_headDT:yyyy}"), _m = string.Format($"{_headDT:MM}"), _d = string.Format($"{_headDT:dd}");
                if (_ischg)
                {
                    //置換檔案
                    _y = m.FileNo.Substring(0, 4);
                    _m = m.FileNo.Substring(4, 2);
                    _d = m.FileNo.Substring(6, 2);
                    m.CustomTitle = originFileName; //+原始檔名:檔案名稱不包含副檔名
                }
                else
                {
                    //新上傳的:取得檔案編號  /* Modified_20201211_使用共用變數、head內容由前端表單獲取。 */
                    //string _type = "ARC", _head = string.Format($"{DateTime.Now:yyyyMMdd}");
                    m.FileNo = _tblNoService.GetNewNo(_type, _name, _head, _body, _len, m.LoginId);//(_type, "媒體資料檔", _head, "_", 7, m.LoginId);

                    //根據使用者選擇決定標題從哪裡來(若為3，則直接使用傳進來的標題)
                    if (m.TitleDefine == 1)
                    {
                        //m.CustomTitle = Path.GetFileNameWithoutExtension(m.TargetFile); //檔案名稱不包含副檔名
                        m.CustomTitle = originFileName; //+原始檔名:檔案名稱不包含副檔名
                    }
                    else if (m.TitleDefine == 2)
                    {
                        m.CustomTitle = _subjectService.GetBy(m.SubjId).fsTITLE;
                    }
                    //ADDED_20200320:增加「4.預編詮釋資料標題 為標題」
                    else if (m.TitleDefine == 4 && m.PreId > 0)
                    {
                        m.CustomTitle = _arcPreService.GetById(m.PreId).fsTITLE;
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
                //Dictionary<string, string> _dicEXIF = new clsIMAGE_EXIF().fnGET_IMAGE_EXIF(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);
                //bool _exifExists = (_dicEXIF != null && _dicEXIF.Count > 0) ? true : false;
                ImageEXIF _imgExif = new clsIMAGE_EXIF().GetImage_EXIF(fsFILE_PATH_H + m.FileNo + "_H." + fsEXT);

                //5、新增或更新 db.tbmARC_PHOTO
                tbmARC_PHOTO _PHOTO = new tbmARC_PHOTO();
                ArcPhotoService _photoService = new ArcPhotoService(_serilogService);
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
                    _PHOTO.fnWIDTH = _imgExif.WIDTH;//_exifExists ? int.Parse(_dicEXIF["fnWIDTH"]) : 0;
                    _PHOTO.fnHEIGHT = _imgExif.HEIGHT;//_exifExists ? int.Parse(_dicEXIF["fnHEIGHT"]) : 0;
                    _PHOTO.fnXDPI = _imgExif.XDPI;//_exifExists ? int.Parse(_dicEXIF["fnXDPI"]) : 0;
                    _PHOTO.fnYDPI = _imgExif.YDPI;//_exifExists ? int.Parse(_dicEXIF["fnYDPI"]) : 0;
                    _PHOTO.fsCAMERA_MAKE = _imgExif.CAMERA_MAKE;//_exifExists ? _dicEXIF["fsCAMERA_MAKE"] : string.Empty;
                    _PHOTO.fsCAMERA_MODEL = _imgExif.CAMERA_MODEL;//_exifExists ? _dicEXIF["fsCAMERA_MODEL"] : string.Empty;
                    _PHOTO.fsFOCAL_LENGTH = _imgExif.FOCAL_LENGTH;//_exifExists ? _dicEXIF["fsFOCAL_LENGTH"] : string.Empty;
                    _PHOTO.fsEXPOSURE_TIME = _imgExif.EXPOSURE_TIME;//_exifExists ? _dicEXIF["fsEXPOSURE_TIME"] : string.Empty;
                    _PHOTO.fsAPERTURE = _imgExif.APERTURE;//_exifExists ? _dicEXIF["fsAPERTURE"] : string.Empty;
                    _PHOTO.fnISO = _imgExif.ISO;//_exifExists ? int.Parse(_dicEXIF["fnISO"]) : 0;
                    _PHOTO.fsUPDATED_BY = m.LoginId;
                    _PHOTO.fdUPDATED_DATE = DateTime.Now;
                    _PHOTO.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());
                    _PHOTO.fsORI_FILE_NAME = originFileName ?? string.Empty;  //+原始檔名:含副檔名
                    _PHOTO.fsLICENSE = m.FileLicense; //20210913_ADDED_檔案版權
                    //
                    result = _photoService.UpdateBy(_PHOTO);
                }
                else
                {   ////新增至資料庫 db.tbmARC_PHOTO
                    _PHOTO.fsFILE_NO = m.FileNo;
                    _PHOTO.fsTITLE = StringExtensions.ReplaceStr(m.CustomTitle);
                    _PHOTO.fsDESCRIPTION = (m.PreId > 0 ? _arcPreService.GetById(m.PreId).fsDESCRIPTION : string.Empty);
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
                    _PHOTO.fnWIDTH = _imgExif.WIDTH;//_exifExists ? int.Parse(_dicEXIF["fnWIDTH"]) : 0;
                    _PHOTO.fnHEIGHT = _imgExif.HEIGHT;//_exifExists ? int.Parse(_dicEXIF["fnHEIGHT"]) : 0;
                    _PHOTO.fnXDPI = _imgExif.XDPI;//_exifExists ? int.Parse(_dicEXIF["fnXDPI"]) : 0;
                    _PHOTO.fnYDPI = _imgExif.YDPI;//_exifExists ? int.Parse(_dicEXIF["fnYDPI"]) : 0;
                    _PHOTO.fsCAMERA_MAKE = _imgExif.CAMERA_MAKE;
                    _PHOTO.fsCAMERA_MODEL = _imgExif.CAMERA_MODEL;//_exifExists ? _dicEXIF["fsCAMERA_MODEL"] : string.Empty;
                    _PHOTO.fsFOCAL_LENGTH = _imgExif.FOCAL_LENGTH;//_exifExists ? _dicEXIF["fsFOCAL_LENGTH"] : string.Empty;
                    _PHOTO.fsEXPOSURE_TIME = _imgExif.EXPOSURE_TIME;//_exifExists ? _dicEXIF["fsEXPOSURE_TIME"] : string.Empty;
                    _PHOTO.fsAPERTURE = _imgExif.APERTURE;//_exifExists ? _dicEXIF["fsAPERTURE"] : string.Empty;
                    _PHOTO.fnISO = _imgExif.ISO;//_exifExists ? int.Parse(_dicEXIF["fnISO"]) : 0;
                    _PHOTO.fsCREATED_BY = m.LoginId;
                    _PHOTO.fdCREATED_DATE = DateTime.Now;
                    _PHOTO.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());
                    _PHOTO.fsORI_FILE_NAME = originFileName ?? string.Empty;  //+原始檔名:含副檔名
                    _PHOTO.fsLICENSE = m.FileLicense; //20210913_ADDED_檔案版權
                    //
                    result = _photoService.CreateBy(_PHOTO, m.PreId);
                }
                #endregion

                string _str = result.IsSuccess ? "成功" : "失敗", _act = (_ischg ? "置換" : "") + "圖片檔案";
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
                #region _Serilog .Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[SaveFilePhoto]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _paramd, Result = result },
                    LogString = "(API)圖片上傳.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[SaveFilePhoto]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _paramd, Result = result, Exception = ex },
                    LogString = "(API)圖片上傳.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return await Task.Run(() => result);
        }

        /// <summary>
        /// 文件檔案上傳後資料記錄(db): 新上傳檔案與置換檔案。
        /// </summary>
        /// <param name="m">處理參數 <see cref="MediaProcessModel"/></param>
        /// <param name="unm">上傳者顯示名稱 dbo.tbmUSERS.[fsNAME] </param>
        /// <returns></returns>
        [NonAction]
        public async Task<VerifyResult> SaveFileDoc(MediaProcessModel m, string unm)
        {
            VerifyResult result = new VerifyResult();
            bool _ischg = (string.IsNullOrEmpty(m.FileNo)) ? false : true; //是否為置換(上傳檔案)
            var _paramd = new { UploadModel = m, UserName = unm, IsExchange = _ischg };

            try
            {
                #region ===== 參數設置 =====
                string fsFILE_NAME = Path.GetFileName(m.TargetFile)  //-沒使用的參數
                    , originFileName = Path.GetFileNameWithoutExtension(m.TargetFile)   //20201023_沒有副檔名的指定路徑字串的檔案名稱
                    , fsEXT = Path.GetExtension(m.TargetFile).Replace(".", "").ToLower()          //副檔名   //20200902副檔名一律改為小寫
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
                //string _y = string.Format($"{DateTime.Now:yyyy}"), _m = string.Format($"{DateTime.Now:MM}"), _d = string.Format($"{DateTime.Now:dd}");
                string _y = string.Format($"{_headDT:yyyy}"), _m = string.Format($"{_headDT:MM}"), _d = string.Format($"{_headDT:dd}");
                if (_ischg)
                {
                    //置換檔案
                    _y = m.FileNo.Substring(0, 4);
                    _m = m.FileNo.Substring(4, 2);
                    _d = m.FileNo.Substring(6, 2);
                    m.CustomTitle = originFileName; //+原始檔名:檔案名稱不包含副檔名
                }
                else
                {
                    //新上傳的:取得檔案編號  /* Modified_20201211_使用共用變數、head內容由前端表單獲取。 */
                    //string _type = "ARC", _head = string.Format($"{DateTime.Now:yyyyMMdd}");
                    m.FileNo = _tblNoService.GetNewNo(_type, _name, _head, _body, _len, m.LoginId);//(_type, "媒體資料檔", _head, "_", 7, m.LoginId);

                    //根據使用者選擇決定標題從哪裡來(若為3，則直接使用傳進來的標題)
                    if (m.TitleDefine == 1)
                    {
                        //m.CustomTitle = Path.GetFileNameWithoutExtension(m.TargetFile); //檔案名稱不包含副檔名
                        m.CustomTitle = originFileName; //+原始檔名:檔案名稱不包含副檔名
                    }
                    else if (m.TitleDefine == 2)
                    {
                        m.CustomTitle = _subjectService.GetBy(m.SubjId).fsTITLE;
                    }
                    //ADDED_20200320:增加「4.預編詮釋資料標題 為標題」
                    else if (m.TitleDefine == 4 && m.PreId > 0)
                    {
                        m.CustomTitle = _arcPreService.GetById(m.PreId).fsTITLE;
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
                ArcDocService _docService = new ArcDocService(_serilogService);

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
                    _DOC.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());
                    _DOC.fsORI_FILE_NAME = originFileName ?? string.Empty;  //+原始檔名:含副檔名
                    _DOC.fsLICENSE = m.FileLicense; //20210913_ADDED_檔案版權
                    //
                    result = _docService.UpdateBy(_DOC);
                }
                else
                {
                    _DOC.fsFILE_NO = m.FileNo;
                    _DOC.fsTITLE = StringExtensions.ReplaceStr(m.CustomTitle);
                    _DOC.fsDESCRIPTION = (m.PreId > 0 ? new ArcPreService(_serilogService).GetById(m.PreId).fsDESCRIPTION : string.Empty);
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
                    _DOC.fnFILE_SECRET = short.Parse(m.FileSecret.ToString());
                    _DOC.fsORI_FILE_NAME = originFileName ?? string.Empty;  //+原始檔名:含副檔名
                    _DOC.fsLICENSE = m.FileLicense; //20210913_ADDED_檔案版權
                    //
                    result = _docService.CreateBy(_DOC, m.PreId);
                }
                #endregion

                string _str = result.IsSuccess ? "成功" : "失敗", _act = (_ischg ? "置換" : "") + "文件檔案";
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
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[SaveFileDoc]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = _paramd, Result = result },
                    LogString = "(API)文件上傳.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "Upload2",
                    Method = "[SaveFilePhoto]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = _paramd, Result = result, Exception = ex },
                    LogString = "(API)文件上傳.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }
            return await Task.Run(() => result);
        }
        #endregion

        #region ===================【檔案解密後產生檢視用檔案、刪除暫檢視用檔案】
        /// <summary>
        /// 檔案解密後產生檢視用檔案路徑URL
        /// </summary>
        /// <param name="m">文件檢視參數 <see cref="ViewerFileModel"/> </param>
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
                    #region _Serilog.Info
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "Upload2",
                        Method = "[DecryptFiles]",
                        EventLevel = SerilogLevelEnum.Information,
                        Input = new { Param = m, Result = result },
                        LogString = "(API)檔案解密檢視用檔案.Result"
                    });
                    #endregion
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
                        var _docDel = _TtbmARC_IndexService.GetArcDocBySubjFile(m.fsSUBJECT_ID, m.fsFILE_NO).FirstOrDefault();
                        if (_docDel == null) { notFound = true; }

                        _encFile = string.Format("{0}{1}.enc", _docDel.fsFILE_PATH, m.fsFILE_NO);
                        _decFile = string.Format("{0}{1}.{2}", _tempFolder, _tmpfnm, _docDel.fsFILE_TYPE);
                        break;
                    case "NORMAL":
                    default:
                        var _doc = new ArcDocService(_serilogService).GetArcDocByIdFile(m.fsSUBJECT_ID, m.fsFILE_NO).FirstOrDefault();
                        if (_doc == null) { notFound = true; }

                        _encFile = string.Format("{0}{1}.enc", _doc.fsFILE_PATH, m.fsFILE_NO);
                        _decFile = string.Format("{0}{1}.{2}", _tempFolder, _tmpfnm, _doc.fsFILE_TYPE);
                        break;
                }

                if (notFound)
                {
                    result.Message = string.Format($"主題與檔案編號錯誤 {m.fsFILE_NO} ");
                    #region _Serilog .Info
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "Upload2",
                        Method = "[DecryptFiles]",
                        EventLevel = SerilogLevelEnum.Information,
                        Input = new { Param = m, Result = result },
                        LogString = "(API)檔案解密檢視用檔案.Result"
                    });
                    #endregion
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
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[DecryptFiles]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = m, Response = result },
                    LogString = "(API)檔案解密檢視用檔案.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[DecryptFiles]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = m, Exception = ex },
                    LogString = "(API)檔案解密檢視用檔案.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 刪除 檔案解密後的檢視用檔案
        /// </summary>
        /// <param name="m">文件檢視結束移除參數 <see cref="ViewerRemoveFileModel"/></param>
        /// <returns></returns>
        [Route("RemoveFile")]
        [HttpPost]
        public VerifyResult RemoveTempFile(ViewerRemoveFileModel m)
        {
            VerifyResult result = new VerifyResult() { Data = m };
            try
            {
                var _doc = new ArcDocService(_serilogService).GetArcDocByIdFile(m.fsSUBJECT_ID, m.fsFILE_NO).FirstOrDefault();

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
                #region _Serilog.Info
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[RemoveTempFile]",
                    EventLevel = SerilogLevelEnum.Information,
                    Input = new { Param = m, Response = result },
                    LogString = "(API)刪除檔案解密檢視用檔案.Result"
                });
                #endregion
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "L_Upload",
                    Method = "[RemoveTempFile]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = m, Exception = ex },
                    LogString = "(API)刪除檔案解密檢視用檔案.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return result;
        }
        #endregion

    }
}