using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.AcrDelete;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using AIRMAM5.Utility.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 刪除記錄表 (檔案資源回收表)
    /// - t_tbmARC_INDEX  (主檔)
    /// - t_tbmARC_VIDEO   (刪除記錄表-影片主檔)
    /// - t_tbmARC_VIDEO_D (刪除記錄表-影片段落描述檔)
    /// - t_tbmARC_VIDEO_K (刪除記錄表-關鍵影格)
    /// </summary>
    public class T_tbmARC_IndexService
    {
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult();
        private readonly IGenericRepository<t_tbmARC_INDEX> _arcIndexRepository = new GenericRepository<t_tbmARC_INDEX>();
        readonly ISerilogService _serilogService;
        readonly ITblLogService _tblLogService;

        public T_tbmARC_IndexService()
        {
            //this._db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
            _tblLogService = new TblLogService();
        }
        public T_tbmARC_IndexService(ISerilogService serilogService, ITblLogService tblLogService)
        {
            _serilogService = serilogService;
            _tblLogService = tblLogService;
        }

        /// <summary>
        /// 刪除記錄表 (檔案資源回收表) 單筆資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public t_tbmARC_INDEX GetById(long id)
        {
            return _arcIndexRepository.Get(x => x.fnINDEX_ID == id);
        }

        /// <summary>
        /// 依照查詢條件取出t_tbmARC_INDEX主檔資料 【sp_t_GET_ARC_INDEX_BY_CONDITION】
        /// </summary>
        /// <returns></returns>
        public List<sp_t_GET_ARC_INDEX_BY_CONDITION_Result> GetARCIndexByCondition(SP_TGetARCIndexByCondition param)
        {
            List<sp_t_GET_ARC_INDEX_BY_CONDITION_Result> result = new List<sp_t_GET_ARC_INDEX_BY_CONDITION_Result>();

            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    result = _db.sp_t_GET_ARC_INDEX_BY_CONDITION(
                        param.StartDate,
                        param.EndDate,
                        param.Status,
                        param.Type, param.IndexId).DefaultIfEmpty().ToList();

                    if (result == null || result.FirstOrDefault() == null)
                        result = new List<sp_t_GET_ARC_INDEX_BY_CONDITION_Result>();
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "T_tbmARC_IndexService",
                    Method = "[GetARCIndexByCondition]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"查詢刪除記錄(Index). {ex.Message}")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 回復媒體檔案 【sp_t_RESTORE_ARC】
        /// </summary>
        /// <returns></returns>
        public VerifyResult RestoreARC(SP_TRestoreARC param)
        {
            VerifyResult result = new VerifyResult(true, "OK");

            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    #region -- sp_t_RESTORE_ARC --
                    var _exec = _db.sp_t_RESTORE_ARC(param.IndexId, param.UseNameBy).FirstOrDefault();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = "媒體檔案 回復成功!";
                        result.Data = this.GetARCIndexByCondition(new SP_TGetARCIndexByCondition
                        {
                            StartDate = string.Empty,
                            EndDate = string.Empty,
                            Status = string.Empty,
                            Type = string.Empty,
                            IndexId = param.IndexId
                        }).FirstOrDefault();
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = string.Format("媒體檔案 回復失敗【{0}】", _exec.Split(':')[1]);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "T_tbmARC_IndexService",
                    Method = "RestoreARC",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"回復媒體檔案. {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"媒體檔案 回復失敗ex【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// 刪除 媒體實體檔案
        /// </summary>
        /// <returns></returns>
        public VerifyResult DeleteARC(SP_TDeleteARC p)
        {
            //VerifyResult result = new VerifyResult();
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    //取得要刪除實體檔案 {影音圖文}的路徑檔名
                    var files = _db.sp_t_GET_tbmARC_FILE_PATHS(p.IndexId, p.Type, p.FileNo).DefaultIfEmpty().ToList();

                    //修改t_tbmARC_INDEX的狀態"D"(Deleted)
                    var _exec = _db.sp_t_DELETE_ARC(p.IndexId, p.UseNameBy).FirstOrDefault();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        foreach (var file in files)
                        {
                            var resDel = CommonFile.DeleteFile(file);
                            #region _DB LOG
                            _tblLogService.Insert_L_Log(
                                TbzCodeIdEnum.MSG001.ToString(),
                                "M003",     //[@USER_ID(@USER_NAME)] 刪除 [@DATA_TYPE] 資料 @RESULT
                                string.Format(FormatString.LogParams, p.UseNameBy, string.Empty, "刪除實體檔案:" + file, resDel),
                                "",//Request.UserHostAddress,
                                JsonConvert.SerializeObject(p),
                                p.UseNameBy);
                            #endregion
                        }

                        result.IsSuccess = true;
                        result.Message = "媒體檔案 刪除成功!";
                        result.Data = this.GetARCIndexByCondition(new SP_TGetARCIndexByCondition
                        {
                            StartDate = string.Empty,
                            EndDate = string.Empty,
                            Status = string.Empty,
                            Type = string.Empty,
                            IndexId = p.IndexId
                        }).FirstOrDefault();
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = string.Format("媒體檔案 刪除失敗【{0}】", _exec.Split(':')[1]);
                    }

                    #region Serilog.Info
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "T_tbmARC_IndexService",
                        Method = "[RestoreARC]",
                        EventLevel = SerilogLevelEnum.Information,
                        Input = new { p, result },
                        LogString = "Result"
                    });
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "T_tbmARC_IndexService",
                    Method = "[RestoreARC]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"刪除媒體檔案. {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"媒體檔案刪除失敗ex【{ex.Message}】");
            }

            return result;
        }

        #region ----- 刪除記錄表-影片 (檔案資源回收表) -----
        /// <summary>
        /// 影片刪除記錄- 主檔
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public sp_t_GET_ARC_VIDEO_Result GetArcVideoById(long id)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.sp_t_GET_ARC_VIDEO(id).FirstOrDefault();

                if (query == null)
                    return new sp_t_GET_ARC_VIDEO_Result();

                return query;
            }
        }

        /// <summary>
        /// 影片刪除記錄- 自訂欄位
        /// </summary>
        /// <param name="fileno"></param>
        /// <returns></returns>
        public List<sp_t_GET_ARC_VIDEO_ATTRIBUTE_Result> GetArcVideoAttributeById(string fileno)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.sp_t_GET_ARC_VIDEO_ATTRIBUTE(fileno).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<sp_t_GET_ARC_VIDEO_ATTRIBUTE_Result>();

                return query;
            }
        }

        /// <summary>
        /// 影片刪除記錄- 關鍵影格
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<sp_t_GET_ARC_VIDEO_K_Result> GetArcVideoKeyById(long id)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.sp_t_GET_ARC_VIDEO_K(id).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<sp_t_GET_ARC_VIDEO_K_Result>();

                return query;
            }
        }

        /// <summary>
        /// 影片刪除記錄- 段落描述
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<sp_t_GET_ARC_VIDEO_D_Result> GetArcVideoDescrById(long id)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.sp_t_GET_ARC_VIDEO_D(id).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<sp_t_GET_ARC_VIDEO_D_Result>();

                return query;
            }
        }
        #endregion

        #region ----- 刪除記錄表-聲音 (檔案資源回收表) -----
        /// <summary>
        /// 聲音刪除記錄- 主檔
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public sp_t_GET_ARC_AUDIO_Result GetArcAudioById(long id)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.sp_t_GET_ARC_AUDIO(id).FirstOrDefault();

                if (query == null)
                    return new sp_t_GET_ARC_AUDIO_Result();

                return query;
            }
        }

        /// <summary>
        /// 聲音刪除記錄- 自訂欄位
        /// </summary>
        /// <param name="fileno"></param>
        /// <returns></returns>
        public List<sp_t_GET_ARC_AUDIO_ATTRIBUTE_Result> GetArcAudioAttributeById(string fileno)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.sp_t_GET_ARC_AUDIO_ATTRIBUTE(fileno).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<sp_t_GET_ARC_AUDIO_ATTRIBUTE_Result>();

                return query;
            }
        }

        /// <summary>
        /// 聲音刪除記錄- 段落描述
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<sp_t_GET_ARC_AUDIO_D_Result> GetArcAudioDescrById(long id)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.sp_t_GET_ARC_AUDIO_D(id).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<sp_t_GET_ARC_AUDIO_D_Result>();

                return query;
            }
        }
        #endregion

        #region ----- 刪除記錄表-圖片 (檔案資源回收表) -----
        /// <summary>
        /// 圖片刪除記錄- 主檔
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public sp_t_GET_ARC_PHOTO_Result GetArcPhotoById(long id)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.sp_t_GET_ARC_PHOTO(id).FirstOrDefault();

                if (query == null)
                    return new sp_t_GET_ARC_PHOTO_Result();

                return query;
            }
        }

        /// <summary>
        /// 圖片刪除記錄- 自訂欄位
        /// </summary>
        /// <param name="fileno"></param>
        /// <returns></returns>
        public List<sp_t_GET_ARC_PHOTO_ATTRIBUTE_Result> GetArcPhotoAttributeById(string fileno)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.sp_t_GET_ARC_PHOTO_ATTRIBUTE(fileno).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<sp_t_GET_ARC_PHOTO_ATTRIBUTE_Result>();

                return query;
            }
        }
        #endregion

        #region ----- 刪除記錄表-文件 (檔案資源回收表) -----
        /// <summary>
        /// 文件刪除記錄- 主檔
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public sp_t_GET_ARC_DOC_Result GetArcDocById(long id)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.sp_t_GET_ARC_DOC(id).FirstOrDefault();

                if (query == null)
                    return new sp_t_GET_ARC_DOC_Result();

                return query;
            }
        }

        /// <summary>
        /// 文件刪除記錄- 自訂欄位
        /// </summary>
        /// <param name="fileno"></param>
        /// <returns></returns>
        public List<sp_t_GET_ARC_DOC_ATTRIBUTE_Result> GetArcDocAttributeById(string fileno)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.sp_t_GET_ARC_DOC_ATTRIBUTE(fileno).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<sp_t_GET_ARC_DOC_ATTRIBUTE_Result>();

                return query;
            }
        }

        public List<t_tbmARC_DOC> GetArcDocBySubjFile(string subjid, string fileno)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.t_tbmARC_DOC.Where(x => x.fsSUBJECT_ID == subjid && x.fsFILE_NO == fileno).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<t_tbmARC_DOC>();

                return query;
            }
        }
        #endregion
    }
}
