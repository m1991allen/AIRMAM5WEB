using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Subject;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 影片主檔 tbmARC_VIDEO
    /// </summary>
    public class ArcVideoService
    {
        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult();
        private readonly IGenericRepository<tbmARC_VIDEO> _arcVideoRepository = new GenericRepository<tbmARC_VIDEO>();
        private readonly IGenericRepository<tbmARC_VIDEO_D> _videoD = new GenericRepository<tbmARC_VIDEO_D>();
        private readonly IGenericRepository<tbmARC_VIDEO_K> _videoK = new GenericRepository<tbmARC_VIDEO_K>();

        public ArcVideoService()
        {
            //_db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }
        public ArcVideoService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        /// <summary>
        /// 取 ARC_VIDEO 入庫項目-影片檔主檔 資料 【EF dbo.tbmARC_VIDEO】
        /// </summary>
        /// <param name="fileno">檔案編號 fsFILE_NO </param>
        /// <returns></returns>
        public tbmARC_VIDEO GetByFileno(string fileno)
        {
            return _arcVideoRepository.Get(x => x.fsFILE_NO == fileno);
        }

        /// <summary>
        /// 多筆檔案編號 取 ARC_VIDEO 影片檔主檔 資料
        /// </summary>
        /// <param name="fnos"></param>
        /// <returns></returns>
        public List<tbmARC_VIDEO> GetByFileNos(string[] fnos)
        {
            var _nos = string.Join(";", fnos);
            var query = _arcVideoRepository.FindBy(x=> SqlFunctions.CharIndex(x.fsFILE_NO, _nos)> 0).ToList();

            return query;
        }

        /// <summary>
        /// 主題編號 取 ARC_VIDEO 影片檔主檔 資料
        /// </summary>
        /// <param name="subjid"></param>
        /// <returns></returns>
        public List<tbmARC_VIDEO> GetBySubjId(string subjid)
        {
            var query = _arcVideoRepository.FindBy(x => x.fsSUBJECT_ID == subjid);

            return query.Any() ? query.OrderBy(b => new { b.fsSUBJECT_ID, b.fsFILE_NO }).ToList() : new List<tbmARC_VIDEO>();
        }

        #region ---------- 【Procedure】
        /// <summary>
        /// 取 ARC_VIDEO 入庫項目-影片檔主檔 資料 【dbo.spGET_ARC_VIDEO_BY_SUBJ_ID】
        /// </summary>
        /// <param name="subjid">主題編號 fsSUBJECT_ID</param>
        /// <param name="fileno">檔案編號 fsFILE_NO (選填) </param>
        /// <returns></returns>
        public List<spGET_ARC_VIDEO_BY_SUBJ_ID_Result> GetVideioBySubjectId (string subjid, string fileno = null)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_VIDEO_BY_SUBJ_ID(subjid).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_ARC_VIDEO_BY_SUBJ_ID_Result>();

                query = query.Where(x => string.IsNullOrEmpty(fileno) ? true : x.fsFILE_NO == fileno).ToList();

                return query;
            }
        }

        /// <summary>
        /// 依 SUBJ_DI+ FILE_NO 取回 tbmARC_VIDEO 資料內容欄位 【dbo.spGET_ARC_VIDEO】
        /// <para> TIP: 預存語法是 SELECT * from tbmARC_VIDEO ,回傳結果會因為資料表新增/刪除影響, 要注意。 </para>
        /// </summary>
        /// <param name="subjid">主題編號 fsSUBJECT_ID (允許空值查詢) </param>
        /// <param name="fileno">檔案編號 fsFILE_NO (允許空值查詢) </param>
        /// <returns></returns>
        public List<spGET_ARC_VIDEO_Result> GetVideoBySubjectFile(string subjid, string fileno)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_VIDEO(fileno, subjid).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_ARC_VIDEO_Result>();

                return query;
            }
        }

        /// <summary>
        /// 取回 tbmARC_VIDEO 自訂欄位內容 【dbo.spGET_ARC_VIDEO_ATTRIBUTE】
        /// </summary>
        /// <param name="fileno">檔案編號 fsFILE_NO </param>
        /// <returns></returns>
        public List<spGET_ARC_VIDEO_ATTRIBUTE_Result> GetVideoAttrByFile(string fileno)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_VIDEO_ATTRIBUTE(fileno).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_ARC_VIDEO_ATTRIBUTE_Result>();

                return query;
            }
        }
        #endregion

        #region ---------- CURD 【tbmARC_VIDEO】: 新 修 刪, 置換
        /// <summary>
        /// 新建存檔 tbmARC_VIDEO: 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult CreateBy(tbmARC_VIDEO rec, int arePreId = 0)
        {
            try
            {
                //non-Use -- EXECUTE dbo.spINSERT_ARC_VIDEO --✘✘✘
                //TODO:當有指定"預編詮釋"資料ID, 要檢查此預編的樣板和Video的樣板是否相同
                if (arePreId > 0)
                {
                    #region 【判斷選定的預編樣板是否與此影片樣板相同】
                    using (_db = new AIRMAM5DBEntities())
                    {
                        var _subj = _db.tbmSUBJECT.Where(x => x.fsSUBJ_ID == rec.fsSUBJECT_ID).FirstOrDefault();
                        var _direc = _db.tbmDIRECTORIES.Where(x => x.fnDIR_ID == _subj.fnDIR_ID).FirstOrDefault();
                        var _arcpre = _db.tbmARC_PRE.Where(x => x.fnPRE_ID == arePreId).FirstOrDefault();

                        if (_direc.fnTEMP_ID_VIDEO != _arcpre.fnTEMP_ID)
                        {
                            result = new VerifyResult(true, "此影片需要的樣板與預編詮釋資料的樣板不符.") { Data = rec };
                            return result;
                        }
                        //else
                        //{
                        //    //取出預編資料寫入rec
                        //    var arcPre = _db.tbmARC_PRE.Where(x => x.fnPRE_ID == arePreId).FirstOrDefault();
                        //    rec.fsTITLE = arcPre.fsTITLE;
                        //    rec.fsDESCRIPTION = arcPre.fsDESCRIPTION;
                        //    rec.fsATTRIBUTE1 = arcPre.fsATTRIBUTE1;
                        //    rec.fsATTRIBUTE2 = arcPre.fsATTRIBUTE2;
                        //      ....
                        //    rec.fsATTRIBUTE59 = arcPre.fsATTRIBUTE59;
                        //    rec.fsATTRIBUTE60 = arcPre.fsATTRIBUTE60;
                        //}
                    }
                    #endregion

                    rec = new ArcPreService().UseArcPreAttribute(arePreId, rec);
                }

                _arcVideoRepository.Create(rec);
                
                result.IsSuccess = true;
                result.Message = "媒資影片已修改更新.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcVideoService",
                    Method = "CreateBy",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"影片檔案新增失敗【{ex.Message}】 ")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"影片檔案新增失敗【{ex.Message}】");
            }
            return result;
        }

        /// <summary>
        /// 表單Form - '單筆/多筆批次'修改更新 媒資影片 tbmARC_VIDEO: 【EF Update】
        /// </summary>
        /// <param name="form"> 如果沒有存在"接口名_IsEdit=on",就不能更新, 如: fsATTRIBUTE4_IsEdit=on </param>
        /// <param name="updateby"></param>
        /// <remarks>20210910_ADDED_新增[版權]欄位 </remarks>
        /// <returns></returns>
        public VerifyResult UpdateMultiple(FormCollection form, string updateby)
        {
            try
            {
                string[] _fileno = form["fsFILE_NO"].ToString().Split(new char[] { ',' });
                //↑form裡面的fsFILE_NO 在批次修改裡,代表前端回傳的多批檔案編號.(請以逗號(,)為分隔符號)

                string _subjId = form["SubjectId"].ToString();

                //取 目錄節點資料.影片的樣板id
                int _videoTempid = new DirectoriesService().GetDirBySubjId(_subjId).fnTEMP_ID_VIDEO;

                List<tbmARC_VIDEO> getVideo = new List<tbmARC_VIDEO>();
                using (_db = new AIRMAM5DBEntities())
                {
                    var qry = _db.tbmARC_VIDEO.Where(x => _fileno.Contains(x.fsFILE_NO));
                    getVideo = qry.ToList();
                    getVideo.ForEach(x => {
                        x.fsTITLE = string.IsNullOrEmpty(form["Title_IsEdit"]) ? x.fsTITLE 
                                    : (form["Title_IsEdit"].ToString().ToUpper() == "ON" ? form["Title"].ToString() : x.fsTITLE);

                        x.fsDESCRIPTION = string.IsNullOrEmpty(form["Description_IsEdit"]) ? x.fsDESCRIPTION 
                                    : (form["Description_IsEdit"].ToString().ToUpper() == "ON" ? form["Description"].ToString() : x.fsDESCRIPTION);

                        x.fnFILE_SECRET = string.IsNullOrEmpty(form["FileSecret_IsEdit"]) ? x.fnFILE_SECRET
                                    : (form["FileSecret_IsEdit"].ToString().ToUpper() == "ON" ? short.Parse(form["FileSecret"].ToString()) : x.fnFILE_SECRET);

                        x.fdUPDATED_DATE = DateTime.Now;
                        x.fsUPDATED_BY = updateby;

                        /* added_20210831_語音轉文字 */
                        x.fsS2T_CONTENT = string.IsNullOrEmpty(form["Voice2TextContent_IsEdit"]) ? x.fsS2T_CONTENT
                                    : (form["Voice2TextContent_IsEdit"].ToString().ToUpper() == "ON" ? form["Voice2TextContent"].ToString() : x.fsS2T_CONTENT);
                        /* added_20210909_版權 */
                        x.fsLICENSE = string.IsNullOrEmpty(form["LicenseCode_IsEdit"]) ? x.fsLICENSE
                                    : (form["LicenseCode_IsEdit"].ToString().ToUpper() == "ON" ? form["LicenseCode"].ToString() : x.fsLICENSE);
                        /* added_20211122_自訂標籤HashTag */
                        x.fsHASH_TAG = string.IsNullOrEmpty(form["HashTag_IsEdit"]) ? x.fsHASH_TAG
                                    : (form["HashTag_IsEdit"].ToString().ToUpper() == "ON" ? form["HashTag"].ToString() : x.fsHASH_TAG);

                        //樣板自訂欄位資料(依欄位屬性給值).SetValue 
                        //Tips_20200409:增加參數判斷批次修改"接口名_IsEdit=on",預設=false
                        new TemplateService().AttriFieldsSetValue<tbmARC_VIDEO>(_videoTempid, x, form, true);
                    });

                    _db.SaveChanges();
                }

                result.IsSuccess = true;
                result.Message = "媒資影片已修改更新.";

                var _v = this.GetVideoBySubjectFile(_subjId, _fileno[0]).FirstOrDefault();
                var _data = getVideo.Select(s => new SubjectFileMetaViewModel().FormatConversion(_v, FileTypeEnum.V)).ToList();
                result.Data = _data;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"媒資影片修改失敗【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcVideioService",
                    Method = "[UpdateMultiple]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = "FormCollection", Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"媒資影片修改失敗【{ex.Message}】 ")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 檔案搬移 - '單筆/多筆批次'修改更新 媒資影片 tbmARC_VIDEO: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateByMove(List<tbmARC_VIDEO> rec)
        {
            try
            {
                _arcVideoRepository.UpdateMultiple(rec);

                result.IsSuccess = true;
                result.Message = "影片檔案搬移更新完成.";
                //TODO:搬移更新後,暫不回傳Data內容
                result.Data = new { };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"影片檔案搬移更新失敗【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcVideioService",
                    Method = "[UpdateByMove]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = "FormCollection", Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"影片檔案搬移更新失敗【{ex.Message}】 ")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 罝換影片檔案 【spUPDATE_ARC_VIDEO_CHANGE】
        /// <para> Result.Data = tbmARC_VIDEO 影片資料 </para>
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="delFK">是否刪除關鍵影格: Y/ N </param>
        /// <returns></returns>
        public VerifyResult ChangeVideo(tbmARC_VIDEO rec, string delFK)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    #region -- EXECUTE dbo.spUPDATE_ARC_VIDEO_CHANGE --
                    var _exec = _db.spUPDATE_ARC_VIDEO_CHANGE(
                        rec.fsFILE_NO, rec.fsFILE_TYPE, rec.fsFILE_TYPE_H, rec.fsFILE_TYPE_L
                        , rec.fsFILE_SIZE, rec.fsFILE_SIZE_H, rec.fsFILE_SIZE_L
                        , rec.fsFILE_PATH, rec.fsFILE_PATH_H, rec.fsFILE_PATH_L
                        , rec.fxMEDIA_INFO, rec.fnWIDTH, rec.fnHEIGHT, rec.fdBEG_TIME, rec.fdEND_TIME, rec.fdDURATION
                        , rec.fsRESOL_TAG, rec.fsUPDATED_BY, delFK, rec.fnFILE_SECRET, rec.fsORI_FILE_NAME).FirstOrDefault().ToString();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = "影片檔案已置換.";
                        result.Data = this.GetByFileno(rec.fsFILE_NO);
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "影片檔案置換失敗【" + _exec.Split(':')[1] + "】";
                    }
                    #endregion

                    ////TODO:要判斷是否刪除關鍵影格 DeleteFK
                    //_arcVideoRepository.Create(rec);
                    //result = new VerifyResult(true, "影片檔案已置換.") { Data = rec };
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcVideoService",
                    Method = "[ChangeVideo]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"置換影片檔案失敗【{ex.Message}】 ")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"影片檔案置換失敗【{ex.Message}】");
            }

            return result;
        }
        #endregion

        #region --------------- 關鍵影格 GET/INSERT/UPDATE/DELETE, 代表圖 --------------- 
        /// <summary>
        /// 取回指定 關鍵影格 【EF GEt】
        /// </summary>
        /// <param name="fileno"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public tbmARC_VIDEO_K GetKeyFrameBy(string fileno, string time)
        {
            //return _db.tbmARC_VIDEO_K.FirstOrDefault(x => x.fsFILE_NO == fileno && x.fsTIME == time);
            return _videoK.FindBy(x => x.fsFILE_NO == fileno && x.fsTIME == time).FirstOrDefault();
        }

        /// <summary>
        /// 取 ARC_VIDEO_K 入庫項目-影片關鍵影格檔 資料 【spGET_ARC_VIDEO_K】: 取 影片-關鍵影格資料
        /// </summary>
        /// <param name="fileno">檔案編號 fsFILE_NO </param>
        /// <param name="time">時間 fsTIME(選填) </param>
        public List<spGET_ARC_VIDEO_K_Result> GetKeyFrame(string fileno, string time = "")
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_VIDEO_K(fileno, time).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                {
                    return new List<spGET_ARC_VIDEO_K_Result>();
                }

                return query;
            }
        }
        /// <summary>
        /// 新增關鍵影格記錄  【spINSERT_ARC_VIDEO_K】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult InsertKeyFrameBy(tbmARC_VIDEO_K rec)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    #region -- EXECUTE dbo.spINSERT_ARC_VIDEO_K --
                    var _exec = _db.spINSERT_ARC_VIDEO_K(
                        rec.fsFILE_NO, rec.fsTIME, rec.fsTITLE, rec.fsDESCRIPTION
                        , rec.fsFILE_PATH, rec.fsFILE_SIZE, rec.fsFILE_TYPE, rec.fsCREATED_BY).FirstOrDefault().ToString();

                    //預存成功會回傳: 關鍵影格圖片檔路徑
                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = "關鍵影格已新增";
                        //result.Data = _exec;
                        result.Data = new { KeyframeImage = _exec };
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "關鍵影格新增失敗【" + _exec.Split(':')[1] + "】";
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcVideoService",
                    Method = "[InsertVideoKeyFrame]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"關鍵影格新增失敗【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// 更新/編輯 關鍵影格記錄  【spUPDATE_ARC_VIDEO_K】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateKeyFrameBy(tbmARC_VIDEO_K rec)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    #region -- EXECUTE dbo.spUPDATE_ARC_VIDEO_K --
                    var _exec = _db.spUPDATE_ARC_VIDEO_K(
                        rec.fsFILE_NO, rec.fsTIME, rec.fsTITLE, rec.fsDESCRIPTION
                        , rec.fsFILE_PATH, rec.fsFILE_SIZE, rec.fsFILE_TYPE, rec.fsCREATED_BY).FirstOrDefault().ToString();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = "關鍵影格已更新.";
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "關鍵影格更新失敗【" + _exec.Split(':')[1] + "】";
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcVideoService",
                    Method = "[UpdateKeyFrameBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"關鍵影格更新失敗【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// 刪除 關鍵影格記錄  【EF Delete】
        /// </summary>
        /// <returns></returns>
        //public VerifyResult DeleteKeyFrameBy(tbmARC_VIDEO_K rec)
        public VerifyResult DeleteKeyFrameMultiple(List<tbmARC_VIDEO_K> rec)
        {
            try
            {
                //non-USE -- EXECUTE dbo.spDELETE_ARC_VIDEO_K --✘✘✘
                _videoK.RemoveRange(rec);

                result.IsSuccess = true;
                result.Message = "關鍵影格已刪除.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcVideoService",
                    Method = "[DeleteKeyFrameMultiple]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion

                result.IsSuccess = false;
                result.Message = string.Format($"關鍵影格刪除失敗【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// 修改影片的代表圖(重新設定代表圖)  【spUPDATE_ARC_VIDEO_K_HEADFRAME】
        /// </summary>
        /// <returns></returns>
        public VerifyResult UpdateHeadFrameBy(string fileno, string time, string updateby)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    #region -- EXECUTE dbo.spUPDATE_ARC_VIDEO_K_HEADFRAME --
                    var _exec = _db.spUPDATE_ARC_VIDEO_K_HEADFRAME(fileno, time, updateby).FirstOrDefault().ToString();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = "代表圖設定成功.";
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "代表圖設定失敗【" + _exec.Split(':')[1] + "】";
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcVideoService",
                    Method = "[UpdateHeadFrameBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = new { }, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"代表圖設定失敗【{ex.Message}】");
            }
            return result;
        }
        #endregion

        #region --------------- 段落描述 GET/INSERT/UPDATE/DELETE --------------- 
        /// <summary>
        /// 取回 影片段落描述 【EF GEt】
        /// </summary>
        /// <param name="fileno">檔案編號 fsFILE_NO </param>
        /// <param name="time"></param>
        /// <returns></returns>
        public List<tbmARC_VIDEO_D> GetParagraphBy(string fileno, int seqno = 0)
        {
            var query = _videoD.FindBy(x => x.fsFILE_NO == fileno && (seqno <= 0 ? true : x.fnSEQ_NO == seqno))
                .DefaultIfEmpty()
                .ToList();

            if (query == null || query.FirstOrDefault() == null) return new List<tbmARC_VIDEO_D>();

            return query;
        }

        /// <summary>
        /// 取 ARC_VIDEO_D 入庫項目-影片段落描述 資料 【spGET_ARC_VIDEO_D】
        /// </summary>
        /// <param name="fileno">檔案編號 fsFILE_NO </param>
        /// <param name="seqno">流水號 fnSEQ_NO(選填) </param>
        public List<spGET_ARC_VIDEO_D_Result> GetVideoSeqment(string fileno, int seqno = 0)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_VIDEO_D(fileno, seqno).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null) return new List<spGET_ARC_VIDEO_D_Result>();

                return query;
            }
        }

        /// <summary>
        /// 新增 影片段落描述資料 【EF Create 】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult InsertParagraph(tbmARC_VIDEO_D rec)
        {
            try
            {
                //non-Use -- EXECUTE dbo.spINSERT_ARC_VIDEO_D --✘✘✘
                var query = _videoD.FindBy(x => x.fsFILE_NO == rec.fsFILE_NO);

                int _seqno = query.Any() ? query.Max(x => x.fnSEQ_NO) : 0;

                rec.fnSEQ_NO = _seqno + 1;
                _videoD.Create(rec);

                result.IsSuccess = true;
                result.Message = "段落描述已新增.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcVideoService",
                    Method = "[InsertParagraph]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = new { }, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"段落描述新增失敗【{ex.Message}】");
            }
            return result;
        }

        /// <summary>
        /// 編輯 影片段落描述資料 【EF Update 】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult EditParagraph(tbmARC_VIDEO_D rec)
        {
            try
            {
                //non-Use -- EXECUTE dbo.spUPDATE_ARC_VIDEO_D --✘✘✘
                _videoD.Update(rec);

                result.IsSuccess = true;
                result.Message = "段落描述已修改.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcVideoService",
                    Method = "[EditParagraph]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = new { }, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"段落描述修改失敗【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// 刪除 影片段落描述資料 【EF Delete 】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult DeleteParagraph(tbmARC_VIDEO_D rec)
        {
            try
            {
                //non-Use -- EXECUTE dbo.spDELETE_ARC_VIDEO_D --✘✘✘
                _videoD.Delete(rec);

                result.IsSuccess = true;
                result.Message = "段落描述已刪除.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcVideoService",
                    Method = "[DeleteParagraph]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = new { }, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"段落描述刪除失敗【{ex.Message}】");
            }

            return result;
        }
        #endregion
    }
}
