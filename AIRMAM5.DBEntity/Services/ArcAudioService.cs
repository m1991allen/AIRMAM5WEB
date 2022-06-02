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
    /// 聲音主檔 tbmARC_AUDIO
    /// </summary>
    public class ArcAudioService
    {
        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult();
        private readonly IGenericRepository<tbmARC_AUDIO> _arcAudioRepository = new GenericRepository<tbmARC_AUDIO>();
        private readonly IGenericRepository<tbmARC_AUDIO_D> _audioD = new GenericRepository<tbmARC_AUDIO_D>();

        public ArcAudioService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }
        public ArcAudioService()
        {
            _serilogService = new SerilogService();
        }

        /// <summary>
        /// 取 ARC_AUDIO 入庫項目-聲音檔 資料 【EF dbo.tbmARC_AUDIO】
        /// </summary>
        /// <returns></returns>
        public tbmARC_AUDIO GetByFileno(string fileno)
        {
            return _arcAudioRepository.Get(x => x.fsFILE_NO == fileno);
        }

        /// <summary>
        /// 多筆檔案編號 取 ARC_AUDIO 聲音檔主檔 資料
        /// </summary>
        /// <param name="fnos"></param>
        /// <returns></returns>
        public List<tbmARC_AUDIO> GetByFileNos(string[] fnos)
        {
            var _nos = string.Join(";", fnos);
            var query = _arcAudioRepository.FindBy(x => SqlFunctions.CharIndex(x.fsFILE_NO, _nos) > 0).ToList();
            
            return query;
        }

        /// <summary>
        /// 依 SUBJ_ID取 ARC_AUDIO 入庫項目-聲音檔 資料 【dbo.spGET_ARC_AUDIO_BY_SUBJ_ID】
        /// </summary>
        /// <param name="subjid">主題Id [fsSUBJ_ID] </param>
        /// <param name="fileno">檔案編號 [fsFILE_NO] </param>
        /// <returns></returns>
        public List<spGET_ARC_AUDIO_BY_SUBJ_ID_Result> GetArcAudioBySubjectId (string subjid, string fileno = null)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_AUDIO_BY_SUBJ_ID(subjid).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_ARC_AUDIO_BY_SUBJ_ID_Result>();

                query = query.Where(x => fileno == null ? true : x.fsFILE_NO == fileno).ToList();

                return query;
            }
        }

        /// <summary>
        /// 依 SUBJ_DI+ FILE_NO 取回 tbmARC_AUDIO 資料內容欄位 【dbo.spGET_ARC_AUDIO】
        /// <para> TIP: 預存語法是 SELECT * from tbmARC_AUDIO ,回傳結果會因為資料表新增/刪除影響, 要注意。 </para>
        /// </summary>
        /// <param name="subjid">主題編號 fsSUBJECT_ID (允訐空值查詢) </param>
        /// <param name="fileno">檔案編號 fsFILE_NO (允訐空值查詢) </param>
        /// <returns></returns>
        public List<spGET_ARC_AUDIO_Result> GetArcAudioByIdFile(string subjid, string fileno)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_AUDIO(fileno, subjid).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_ARC_AUDIO_Result>();

                return query;
            }
        }

        /// <summary>
        /// 依 FILE_NO 取回 tbmARC_AUDIO 自訂欄位 【dbo.spGET_ARC_AUDIO_ATTRIBUTE】
        /// </summary>
        /// <param name="fileno">檔案編號 fsFILE_NO </param>
        /// <returns></returns>
        public List<spGET_ARC_AUDIO_ATTRIBUTE_Result> GetAudioAttrByFile(string fileno)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_AUDIO_ATTRIBUTE(fileno).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_ARC_AUDIO_ATTRIBUTE_Result>();

                return query;
            }
        }

        #region ---------- CURD 【tbmARC_AUDIO】: 新 修 刪
        /// <summary>
        /// 新建存檔 tbmARC_AUDIO: 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="arePreId"></param>
        /// <returns></returns>
        public VerifyResult CreateBy(tbmARC_AUDIO rec, int arePreId = 0)
        {
            try
            {
                // Non-Use: EXECUTE dbo.spINSERT_ARC_AUDIO --✘✘✘

                using (_db = new AIRMAM5DBEntities())
                {
                    //Tips: 當有指定"預編詮釋"資料ID, 要檢查此預編的樣板和Audio的樣板是否相同
                    if (arePreId > 0)
                    {
                        #region 【判斷選定的預編樣板是否與此聲音樣板相同】
                        var _subj = _db.tbmSUBJECT.Where(x => x.fsSUBJ_ID == rec.fsSUBJECT_ID).FirstOrDefault();
                        var _direc = _db.tbmDIRECTORIES.Where(x => x.fnDIR_ID == _subj.fnDIR_ID).FirstOrDefault();
                        var _arcpre = _db.tbmARC_PRE.Where(x => x.fnPRE_ID == arePreId).FirstOrDefault();

                        if (_direc.fnTEMP_ID_AUDIO != _arcpre.fnTEMP_ID)
                        {
                            result = new VerifyResult(true, "此聲音需要的樣板與預編詮釋資料的樣板不符.") { Data = rec };
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
                        //      .....
                        //    rec.fsATTRIBUTE59 = arcPre.fsATTRIBUTE59;
                        //    rec.fsATTRIBUTE60 = arcPre.fsATTRIBUTE60;
                        //}
                        #endregion

                        rec = new ArcPreService().UseArcPreAttribute(arePreId, rec);
                    }
                }

                _arcAudioRepository.Create(rec);
                result.IsSuccess = true;
                result.Message = "聲音檔案已新增.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcAudioService",
                    Method = "CreateBy",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"聲音檔案新增失敗【{ex.Message}】 ")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"聲音檔案新增失敗【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// 表單Form : '單筆/多筆批次'修改更新 媒資聲音 tbmARC_AUDIO: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateMultiple(FormCollection form, string updateby)
        {
            try
            {
                //↓↓form裡面的fsFILE_NO 在批次修改裡,代表前端回傳的多批檔案編號.(請以逗號(,)為分隔符號)
                string[] _fileno = form["fsFILE_NO"].ToString().Split(new char[] { ',', ';' });
                string _subjId = form["SubjectId"].ToString();

                //↓↓取 目錄節點資料.影片的樣板id
                int _audioTempid = new DirectoriesService().GetDirBySubjId(_subjId).fnTEMP_ID_AUDIO;

                List<tbmARC_AUDIO> getAudio = new List<tbmARC_AUDIO>();
                using (_db = new AIRMAM5DBEntities())
                {
                    getAudio = _db.tbmARC_AUDIO.Where(x => _fileno.Contains(x.fsFILE_NO)).ToList();

                    getAudio.ForEach(x => {
                        x.fsTITLE = string.IsNullOrEmpty(form["Title_IsEdit"]) ? x.fsTITLE 
                                    : (form["Title_IsEdit"].ToString().ToUpper() == "ON" ? form["Title"].ToString() : x.fsTITLE);

                        x.fsDESCRIPTION = string.IsNullOrEmpty(form["Description_IsEdit"]) ? x.fsDESCRIPTION 
                                    : (form["Description_IsEdit"].ToString().ToUpper() == "ON" ? form["Description"].ToString() : x.fsDESCRIPTION);

                        x.fnFILE_SECRET = string.IsNullOrEmpty(form["FileSecret_IsEdit"]) ? x.fnFILE_SECRET
                                    : (form["FileSecret_IsEdit"].ToString().ToUpper() == "ON" ? short.Parse(form["FileSecret"].ToString()) : x.fnFILE_SECRET);
                        x.fdUPDATED_DATE = DateTime.Now;
                        x.fsUPDATED_BY = updateby;

                        /* added_20210831_語音轉文字 */
                        x.fsS2T_CONTENT = string.IsNullOrEmpty(form["Voice2TextContent"]) ? x.fsS2T_CONTENT
                                    : (form["Voice2TextContent_IsEdit"].ToString().ToUpper() == "ON" ? form["Voice2TextContent"].ToString() : x.fsS2T_CONTENT);
                        /* 版權欄位 */
                        x.fsLICENSE = string.IsNullOrEmpty(form["LicenseCode_IsEdit"]) ? x.fsLICENSE
                                    : (form["LicenseCode_IsEdit"].ToString().ToUpper() == "ON" ? form["LicenseCode"].ToString() : x.fsLICENSE);
                        /* added_20211122_自訂標籤HashTag */
                        x.fsHASH_TAG = string.IsNullOrEmpty(form["HashTag_IsEdit"]) ? x.fsHASH_TAG
                                    : (form["HashTag_IsEdit"].ToString().ToUpper() == "ON" ? form["HashTag"].ToString() : x.fsHASH_TAG);

                        //樣板自訂欄位資料(依欄位屬性給值).SetValue 
                        //Tips_20200409:增加參數判斷批次修改"接口名_IsEdit=on",預設=false
                        new TemplateService().AttriFieldsSetValue<tbmARC_AUDIO>(_audioTempid, x, form, true);
                    });

                    _db.SaveChanges();
                }

                result.IsSuccess = true;
                result.Message = "媒資聲音已修改更新.";

                var _a = this.GetArcAudioByIdFile(_subjId, _fileno[0]).FirstOrDefault();
                var _data = getAudio.Select(s => new SubjectFileMetaViewModel().FormatConversion(_a, FileTypeEnum.A)).ToList();

                result.Data = _data;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"媒資聲音修改失敗【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcAudioService",
                    Method = "[UpdateMultiple]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = "FormCollection", Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"媒資聲音修改失敗【{ex.Message}】 ")
                });
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 單筆修改 媒資聲音 tbmARC_AUDIO: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateBy(tbmARC_AUDIO rec)
        {
            try
            {
                rec.fdUPDATED_DATE = DateTime.Now;
                _arcAudioRepository.Update(rec);

                result.IsSuccess = true;
                result.Message = "聲音資料修改成功.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"聲音資料修改失敗【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcAudioService",
                    Method = "[UpdateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"媒資聲音修改失敗【{ex.Message}】 ")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 檔案搬移 - '單筆/多筆批次'修改更新 媒資聲音 tbmARC_AUDIO: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateByMove(List<tbmARC_AUDIO> rec)
        {
            //VerifyResult result = new VerifyResult();
            try
            {
                _arcAudioRepository.UpdateMultiple(rec);

                result.IsSuccess = true;
                result.Message = "聲音檔案搬移更新完成.";
                //TODO:搬移更新後,暫不回傳Data內容
                result.Data = new { };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"聲音檔案搬移更新失敗【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcAudioService",
                    Method = "[UpdateByMove]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"聲音檔案搬移更新失敗【{ex.Message}】 ")
                });
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 罝換聲音檔案 【spUPDATE_ARC_AUDIO_CHANGE】: 判斷是否刪除"段落描述"
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="delFK">是否刪除段落描述: Y/ N </param>
        /// <returns></returns>
        public VerifyResult ChangeAudio(tbmARC_AUDIO rec, string delFK)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                using (var _trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        //non-Use -- EXECUTE dbo.spUPDATE_ARC_AUDIO_CHANGE
                        // Modified_at_20200304//TODO:要判斷是否刪除段落描述 DeleteFK
                        //_arcVideoRepository.Create(rec);
                        //result = new VerifyResult(true, "聲音檔案已置換.") { Data = rec };

                        //1、Update dbo.tbmARC_AUDIO
                        //var _arcAudio = new GenericRepository<tbmARC_AUDIO>(_db);
                        _arcAudioRepository.Update(rec);

                        //2、Checked "delFK":是否刪除"段落描述"
                        if (delFK == IsTrueFalseEnum.Y.ToString())
                        {
                            //var _arcAudioD = new GenericRepository<tbmARC_AUDIO_D>(_db);
                            var _get = _audioD.FindBy(x => x.fsFILE_NO == rec.fsFILE_NO);
                            if (_get.Any()) { _audioD.RemoveRange(_get.ToList()); }
                        }

                        result.IsSuccess = true;
                        result.Message = "聲音檔案已置換";
                        result.Data = rec;
                        _trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        _trans.Rollback();

                        #region Serilog.Err
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "ArcAudioService",
                            Method = "[ChangeAudio]",
                            EventLevel = SerilogLevelEnum.Error,
                            Input = new { Params = rec, Exception = ex },
                            LogString = "Exception",
                            ErrorMessage = string.Format($"置換聲音檔案失敗【{ex.Message}】 ")
                        });
                        #endregion
                        result.IsSuccess = false;
                        result.Message = string.Format($"聲音檔案置換失敗【{ex.Message}】");
                    }
                    finally { _trans.Dispose(); }
                }
            }

            return result;
        }
        #endregion

        #region --------------- 段落描述 GET/INSERT/UPDATE/DELETE --------------- 
        /// <summary>
        /// 取 ARC_AUDIO_D 入庫項目-聲音段落描述 資料 【spGET_ARC_AUDIO_D】
        /// </summary>
        /// <param name="fileno">檔案編號 fsFILE_NO </param>
        /// <param name="seqno">流水號 fnSEQ_NO(選填) </param>
        public List<spGET_ARC_AUDIO_D_Result> GetAudioSeqment(string fileno, int seqno = 0)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_AUDIO_D(fileno, seqno).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_ARC_AUDIO_D_Result>();

                return query;
            }
        }
        /// <summary>
        /// 取回 聲音段落描述 【EF GEt】
        /// </summary>
        /// <param name="fileno"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public List<tbmARC_AUDIO_D> GetParagraphBy(string fileno, int seqno = 0)
        {
            var query = _audioD.FindBy(x => x.fsFILE_NO == fileno && (seqno <= 0 ? true : x.fnSEQ_NO == seqno))
                .DefaultIfEmpty().ToList();

            return query;
        }

        /// <summary>
        /// 新增 聲音段落描述資料 【EF Create 】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult InsertParagraph(tbmARC_AUDIO_D rec)
        {
            try
            {
                // non-Use -- EXECUTE dbo.spINSERT_ARC_AUDIO_D --

                var query = _audioD.FindBy(x => x.fsFILE_NO == rec.fsFILE_NO);
                int _seqno = query.Any() ? query.Max(x => x.fnSEQ_NO) : 0;
                rec.fnSEQ_NO = _seqno + 1;
                _audioD.Create(rec);

                result.IsSuccess = true;
                result.Message = "段落描述已新增.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcAudioService",
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
        /// 編輯 聲音段落描述資料 【EF Update 】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult EditParagraph(tbmARC_AUDIO_D rec)
        {
            try
            {
                // Non-Use : EXECUTE dbo.spUPDATE_ARC_AUDIO_D --
                _audioD.Update(rec);

                result.IsSuccess = true;
                result.Message = "段落描述已修改.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcAudioService",
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
        /// 刪除 聲音段落描述資料 【EF Delete 】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult DeleteParagraph(tbmARC_AUDIO_D rec)
        {
            try
            {
                //#region Non-Use : EXECUTE dbo.spDELETE_ARC_AUDIO_D --✘✘✘
                _audioD.Delete(rec);

                result.IsSuccess = true;
                result.Message = "段落描述已刪除.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcAudioService",
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
