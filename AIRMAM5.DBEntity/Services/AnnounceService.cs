using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Announce;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 公告資料表 處理實作
    /// </summary>
    public class AnnounceService : IAnnounceService
    {
        readonly IGenericRepository<tbmANNOUNCE> _announceRepository = new GenericRepository<tbmANNOUNCE>();
        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;

        public AnnounceService()
        {
            _serilogService = new SerilogService();
        }
        public AnnounceService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        /// <summary>
        /// fsANN_ID 取回公告資料
        /// </summary>
        /// <param name="id"> fsANN_ID </param>
        /// <returns></returns>
        public tbmANNOUNCE GetById(long id)
        {
            var query = _announceRepository.Get(x => x.fnANN_ID == id);

            return query;
        }

        /// <summary>
        /// 公告資料是否存在
        /// </summary>
        /// <returns></returns>
        public bool IsExists(long id)
        {
            return _announceRepository.FindBy(x => x.fnANN_ID == id).Any();
        }

        /// <summary>
        /// 系統公告首頁 資料 BY 使用者 【spGET_ANNOUNCE_BY_LOGIN_ID】
        /// </summary>
        /// <param name="username">使用者fsLOGIN_ID</param>
        /// <returns></returns>
        public List<AnnouncePublicViewModel> GetAnnounceInfo(string username)
        {
            // ✘✘--spGET_ANNOUNCE_BY_EFFECTIVE_DATE_AND_USERGROUP_AND_NOT_HIDDEN
            //var get = _db.spGET_ANNOUNCE_BY_EFFECTIVE_DATE_AND_USERGROUP_AND_NOT_HIDDEN(username).DefaultIfEmpty().ToList();

            using (_db = new AIRMAM5DBEntities())
            {
                //2020/01/31:修改預存程序邏輯
                var get = _db.spGET_ANNOUNCE_BY_LOGIN_ID(username).DefaultIfEmpty().ToList();
                if (get == null || get.FirstOrDefault() == null) return new List<AnnouncePublicViewModel>();

                //登入後的公告首頁,不顯示"登入公告" 資訊
                var query = get.Where(x => x.fsTYPE != AnnounceTypeEnum.D.ToString())
                    .Select(s => new AnnouncePublicViewModel().DataConvert(s))
                    .ToList<AnnouncePublicViewModel>();

                return query;
            }
        }

        /// <summary>
        /// 取得目前共用的公告 【spGET_ANNOUNCE_PUBLIC】
        /// </summary>
        public List<spGET_ANNOUNCE_PUBLIC_Result> GetPublicAnnounce()
        {
            using (_db = new AIRMAM5DBEntities())
            {
                //spGET_ANNOUNCE_PUBLIC: 顯示共用的公告。
                //(ANN.fsGROUP_LIST = '') AND (ANN.fsIS_HIDDEN <> 'Y')
                var query = _db.spGET_ANNOUNCE_PUBLIC().DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null) return new List<spGET_ANNOUNCE_PUBLIC_Result>();

                return query;
            }
        }

        /// <summary>
        /// 指定條件(ID,SDate,EDATE,TYPE)取回公告資料 【spGET_ANNOUNCE_BY_ANNID_DATES_TYPE】
        /// </summary>
        /// <param name="annid"></param>
        /// <param name="sdate"></param>
        /// <param name="edate"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<spGET_ANNOUNCE_BY_ANNID_DATES_TYPE_Result> GetBy4Parament(long annid = 0, string sdate = "", string edate = "", string type="")
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ANNOUNCE_BY_ANNID_DATES_TYPE(annid, sdate, edate, type).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_ANNOUNCE_BY_ANNID_DATES_TYPE_Result>();

                return query;
            }
        }

        /// <summary>
        /// 登入頁公告 【spGET_ANNOUNCE_BY_ANNID_DATES_TYPE】
        /// </summary>
        /// <returns></returns>
        public List<spGET_ANNOUNCE_BY_ANNID_DATES_TYPE_Result> GetLoginAnn()
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ANNOUNCE_BY_ANNID_DATES_TYPE(0, string.Empty, string.Empty, AnnounceTypeEnum.D.ToString())
                .DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_ANNOUNCE_BY_ANNID_DATES_TYPE_Result>();

                query = query.Where(x => x.fdSDATE <= DateTime.Now && x.fdEDATE >= DateTime.Now).ToList();
                return query;
            }
        }

        #region 【dbo.tbmANNOUNCE】 新 修 刪
        /// <summary>
        /// Create 【EF 】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult Create(tbmANNOUNCE rec)
        {
            VerifyResult result = new VerifyResult(false, "無效的資料內容");
            try
            {
                if (rec != null)
                {
                    // non-USED ✘✘-- EXECUTE dbo.spINSERT_ANNOUNCE --
                    _announceRepository.Create(rec);
                    
                    result.IsSuccess = true;
                    result.Message = "公告新增成功";
                    result.Data = rec;
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "AnnounceService",
                    Method = "Create",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = rec, Result = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"公告新增失敗【{ex.Message}】 ")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// Update  【EF 】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult Update(tbmANNOUNCE rec)
        {
            VerifyResult result = new VerifyResult(false, "無效的資料內容");
            try
            {
                if (rec != null)
                {
                    // non-USED ✘✘-- EXECUTE dbo.spUPDATE_ANNOUNCE --
                    _announceRepository.Update(rec);

                    result.IsSuccess = true;
                    result.Message = "公告修改成功";
                    result.Data = rec;
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "AnnounceService",
                    Method = "Update",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = rec, Result = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"公告修改失敗【{ex.Message}】 ")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Delete  【spDELETE_ANNOUNCE-> EF 】
        /// </summary>
        /// <param name="annid">公告編號 fnANN_ID </param>
        /// <returns></returns>
        public VerifyResult Delete(long annid)
        {
            VerifyResult result = new VerifyResult(false);
            try
            {
                #region -- EXECUTE dbo.spDELETE_ANNOUNCE --✘✘
                //string _exec = _db.spDELETE_ANNOUNCE(annid, HttpContext.Current.User.Identity.GetUserId()).FirstOrDefault().ToString();
                //if (_exec.IndexOf("ERROR") == -1)
                //{
                //    result.IsSuccess = true;
                //    result.Message = "公告資料已刪除";
                //}
                //else
                //{
                //    result.IsSuccess = false;
                //    result.Message = "公告資料刪除失敗【" + _exec.Split(':')[1] + "】";
                //}
                #endregion
                
                var get = this.GetById(annid);
                _announceRepository.Delete(get);
                result.IsSuccess = true;
                result.Message = "公告資料已刪除";
                
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "AnnounceService",
                    Method = "Delete",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { annid, Exception = ex},
                    LogString = "Exception",
                    ErrorMessage = string.Format($"公告刪除失敗. {ex.Message}")
                });
                #endregion
                result.IsSuccess = true;
                result.Message = string.Format($"公告資料刪除失敗【{ex.Message}】");//ex.Message;
            }
            return result;
        }
        
        #endregion

    }
}
