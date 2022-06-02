using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Material;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 預借清單 [tbmMATERIAL]
    /// </summary>
    public class MaterialService
    {
        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "無效的資料內容");
        readonly IGenericRepository<tbmMATERIAL> _materialRepository = new GenericRepository<tbmMATERIAL>();

        public MaterialService()
        {
            _serilogService = new SerilogService();
            //this._db = new AIRMAM5DBEntities();
        }
        public MaterialService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        /// <summary>
        /// [fnMATERIAL_ID] 取回借調資料 【EF 】
        /// </summary>
        /// <returns></returns>
        public tbmMATERIAL GetById(long materialid)
        {
            var query = _materialRepository.Get(x => x.fnMATERIAL_ID == materialid);

            return query ?? new tbmMATERIAL();
        }

        /// <summary>
        /// 依借調者帳號[fsLOGIN_ID] 取回借調資料 【EF FindBy】
        /// </summary>
        /// <param name="loginid">使用者帳號 [fsLOGIN_ID] </param>
        /// <returns></returns>
        public List<tbmMATERIAL> GetByUser(string loginid)
        {
            var query = _materialRepository.FindBy(x => x.fsMARKED_BY == loginid).ToList();

            return query ?? new List<tbmMATERIAL>();
        }

        /// <summary>
        /// 多筆借調編號[fnMATERIAL_ID] 取回借調資料 【EF FindBy】
        /// </summary>
        /// <param name="loginid">借調編號 [fsLOGIN_ID] List </param>
        /// <returns></returns>
        public List<tbmMATERIAL> GetBy(List<long> lst)
        {
            string _ids = string.Join("^", lst);
            var query = _materialRepository.FindBy(x => _ids.IndexOf(x.fnMATERIAL_ID.ToString()) >= 0)
                .DefaultIfEmpty().ToList();

            return query ?? new List<tbmMATERIAL>();
        }

        #region 【DB Procedure】
        /// <summary>
        /// 依借調者帳號[fsLOGIN_ID] 取回借調資料 【spGET_MATERIAL_BY_MARKED_BY】
        /// </summary>
        /// <returns></returns>
        public List<spGET_MATERIAL_BY_MARKED_BY_Result> GetByMarked(string loginid)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_MATERIAL_BY_MARKED_BY(loginid).DefaultIfEmpty().ToList();

                return (query == null || query.FirstOrDefault() == null) ? new List<spGET_MATERIAL_BY_MARKED_BY_Result>() : query;
            }
        }
        #endregion

        #region 【調用清單 tbmMATERIAL】新增/ 刪除
        /// <summary>
        /// Create 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult CreateBy(tbmMATERIAL rec)
        {
            try
            {
                if (rec == null) return result;
                _materialRepository.Create(rec);

                result.IsSuccess = true;
                result.Message = "調用資料已新增";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.ERR
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "MaterialService",
                    Method = "[CreateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"預借清單新增失敗. {ex.Message}")
                });
                #endregion

                result.IsSuccess = false;
                result.Message = string.Format($"調用檔案新增失敗. {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Create Multiple Record 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult CreateRange(List<tbmMATERIAL> rec, string createdby)
        {
            try
            {
                if (rec == null) return result;
                _materialRepository.CreateRange(rec);
                //
                var get = (from a in this.GetByMarked(createdby)
                           join b in rec on a.fnMATERIAL_ID equals b.fnMATERIAL_ID
                           select new MaterialListModel().ConvertData(a)
                           ).ToList();
                
                result.IsSuccess = true;
                result.Message = "調用檔案新增完成";
                result.Data = get;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "MaterialService",
                    Method = "[CreateRange]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"預借清單新增失敗. {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"調用檔案新增失敗. {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// Delete Multiple Record 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult DeleteRange(List<tbmMATERIAL> rec)
        {
            try
            {
                if (rec == null) return result;
                _materialRepository.RemoveRange(rec);

                result.IsSuccess = true;
                result.Message = "調用檔案刪除成功.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "MaterialService",
                    Method = "[DeleteRange]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"調用檔案刪除失敗. {ex.Message}")
                });
                #endregion

                result.IsSuccess = false;
                result.Message = string.Format($"調用檔案刪除失敗. {ex.Message}");
            }
            return result;
        }
        #endregion
    }
}
