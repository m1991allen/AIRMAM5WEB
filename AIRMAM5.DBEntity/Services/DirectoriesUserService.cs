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
    /// 系統目錄使用權限-使用者 TbmDIR_User
    /// </summary>
    public class DirectoriesUserService
    {
        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "fail");
        private readonly IGenericRepository<tbmDIR_USER> _dirUserRepository = new GenericRepository<tbmDIR_USER>();

        public DirectoriesUserService()
        {
            //this._db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }
        public DirectoriesUserService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        /// <summary>
        /// 目錄Id + 使用者帳號 找資料
        /// </summary>
        /// <param name="dirid">目錄Id </param>
        /// <param name="loginid">使用者帳號 預設null </param>
        /// <returns></returns>
        public List<tbmDIR_USER> GetBy(long dirid, string loginid = null)
        {
            //List<tbmDIR_USER> query = _dirUserRepository.FindBy(x => x.fnDIR_ID == dirid && x.fsLOGIN_ID == loginid).ToList();
            //if (loginid == null)
            //{
            //    query = _dirUserRepository.FindBy(x => x.fnDIR_ID == dirid).ToList();
            //}
            //return query;

            var qry = _dirUserRepository.FindBy(x => x.fnDIR_ID == dirid && (loginid == null ? true : x.fsLOGIN_ID == loginid));

            return qry.ToList();
        }

        /// <summary>
        /// 新增 系統目錄使用者 使用權限 tbmDIR_USER: 【EF Create】
        /// </summary>
        /// <returns></returns>
        public VerifyResult CreateBy(tbmDIR_USER rec)
        {
            try
            {
                //#region -- spINSERT_DIR_USER_BY_USER_DIR --✘✘✘
                _dirUserRepository.Create(rec);

                result.IsSuccess = true;
                result.Message = "系統目錄使用者 權限已建立.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "DirectoriesUserService",
                    Method = "[CreateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"系統目錄使用者 權限新增失敗.\n {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"系統目錄使用者 權限新增失敗【{ex.Message}】");
            }

            return result;
        }
        /// <summary>
        /// 修改 系統目錄使用者 使用權限 tbmDIR_USER: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateBy(tbmDIR_USER rec)
        {
            try
            {
                //#region -- spUPDATE_DIR_USER_BY_USER_DIR --✘✘✘
                _dirUserRepository.Update(rec);

                result.IsSuccess = true;
                result.Message = "系統目錄使用者 權限已更新.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "DirectoriesUserService",
                    Method = "[UpdateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"系統目錄使用者 權限修改失敗.\n {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"系統目錄使用者 權限修改失敗【{ex.Message}】");
            }
            return result;
        }

        /// <summary>
        /// 刪除 系統目錄使用者 使用權限 Execute dbo.spDELETE_DIR_USER
        /// </summary>
        /// <param name="dirid"></param>
        /// <param name="username"></param>
        /// <param name="deleteby"></param>
        /// <returns></returns>
        public VerifyResult DeleteBy(long dirid, string username, string deleteby)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    #region -- spDELETE_DIR_USER --
                    var _exec = _db.spDELETE_DIR_USER(dirid, username, deleteby).FirstOrDefault();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = "系統目錄使用者 權限已刪除.";
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "系統目錄使用者 權限刪除失敗【" + _exec.Split(':')[1] + "】";
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "DirectoriesUserService",
                    Method = "[DeleteBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"系統目錄使用者 權限刪除失敗.\n {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"系統目錄使用者 權限修改失敗【{ex.Message}】");
            }
            return result;
        }

    }
}
