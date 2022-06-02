using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 系統設定檔-tbzCONFIG
    /// </summary>
    public class ConfigService
    {
        protected AIRMAM5DBEntities _db;
        readonly SerilogService _serilogService;
        private readonly IGenericRepository<tbzCONFIG> _configRepository = new GenericRepository<tbzCONFIG>();

        public ConfigService()
        {
            //_db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }

        /// <summary>
        /// 取 tbmCONFIG 指定key資料  【dbo.spGET_CONFIG_Result】
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<spGET_CONFIG_Result> GetConfigBy(string key)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                return _db.spGET_CONFIG(key).DefaultIfEmpty().ToList();
            }
        }

        /// <summary>
        /// 編輯存檔 tbzCONFIG: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateBy(tbzCONFIG rec)
        {
            VerifyResult result = new VerifyResult();

            try
            {
                //#region -- EXECUTE dbo.spUPDATE_CONFIG --✘✘✘
                rec.fdUPDATED_DATE = DateTime.Now;
                _configRepository.Update(rec);
                
                result.IsSuccess = true;
                result.Message = "設定值編輯存檔成功";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ConfigService",
                    Method = "UpdateBy",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"設定值編輯存檔失敗【{ex.Message}】 ")
                });
                #endregion

                result.IsSuccess = false;
                result.Message = string.Format($"設定值編輯存檔失敗【{ex.Message}】");
            }

            return result;
        }

    }
}
