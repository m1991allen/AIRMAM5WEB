using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// tbmSYNONYMS 同義詞資料
    /// </summary>
    public class SynonymsService
    {
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "fail");
        readonly ISerilogService _serilogService;
        private readonly IGenericRepository<tbmSYNONYMS> _synonymsRepository = new GenericRepository<tbmSYNONYMS>();

        public SynonymsService()
        {
            //_db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }

        /// <summary>
        /// 取出 tbmSYNONYMS 主檔資料
        /// </summary>
        /// <param name="idx">主檔編號</param>
        /// <param name="type">分類 .SYNO_TYPE</param>
        /// <param name="text">調彚關鍵字</param>
        /// <returns></returns>
        public List<spGET_SYNONYMS_Result> GetByParam(long idx, string type = "", string text = "")
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_SYNONYMS(idx, type, text).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_SYNONYMS_Result>();

                return query;
            }
        }

        /// <summary>
        /// 同義詞id 取得同議詞資料
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public tbmSYNONYMS GetById(long idx)
        {
            var query = _synonymsRepository.FindBy(x => x.fnINDEX_ID == idx);

            if (query == null || query.FirstOrDefault() == null)
                return new tbmSYNONYMS();

            return query.FirstOrDefault();
        }

        /// <summary>
        /// 新增同義詞 : 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult CreateSynonyms(tbmSYNONYMS rec)
        {
            try
            {
                // ✘✘-- EXECUTE dbo.spINSERT_SYNONYMS --

                _synonymsRepository.Create(rec);

                result.IsSuccess = true;
                result.Message = "同義詞已新增.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TbmSynonymsService",
                    Method = "CreateSynonyms",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"同義詞新增失敗. {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"同義詞新增失敗【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// 編輯 同義詞 : Execute spUPDATE_SYNONYMS
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateSynonyms(tbmSYNONYMS rec)
        {
            try
            {
                //-- Execute dbo.spUPDATE_SYNONYMS --✘✘✘
                _synonymsRepository.Update(rec);

                result.IsSuccess = true;
                result.Message = "同義詞已更新.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TbmSynonymsService",
                    Method = "UpdateSynonyms",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"同義詞編輯失敗. {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"同義詞編輯失敗【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// 刪除 同義詞 : Execute spDELETE_SYNONYMS
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public VerifyResult DeleteSynonyms(long idx, string username)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    var _exec = _db.spDELETE_SYNONYMS(idx, username).FirstOrDefault();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = "同義詞已刪除.";
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "同義詞刪除失敗【" + _exec.Split(':')[1] + "】";
                    }
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TbmSynonymsService",
                    Method = "DeleteSynonyms",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"同義詞刪除失敗. {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"同義詞刪除失敗【{ex.Message}】");
            }

            return result;
        }

    }
}
