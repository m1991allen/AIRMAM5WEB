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
    /// 系統目錄使用權限-群組 TbmDIR_Group
    /// </summary>
    public class DirectoriesGroupService
    {
        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "fail");
        private readonly IGenericRepository<tbmDIR_GROUP> _dirGroupRepository = new GenericRepository<tbmDIR_GROUP>();

        public DirectoriesGroupService()
        {
            //this._db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }
        public DirectoriesGroupService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        /// <summary>
        /// 目錄Id + 角色群組id 找資料
        /// </summary>
        /// <param name="dirid">目錄Id </param>
        /// <param name="groupid">角色群組id 預設null </param>
        /// <returns></returns>
        public List<tbmDIR_GROUP> GetBy(long dirid, string groupid = null)
        {
            var query = _dirGroupRepository
                .FindBy(x => (string.IsNullOrEmpty(groupid) ? true : x.fsGROUP_ID == groupid) && x.fnDIR_ID == dirid)
                .ToList();

            return query;
        }

        /// <summary>
        /// 新增 系統目錄角色群組 使用權限 tbmDIR_GROUP: 【EF Create】
        /// </summary>
        /// <returns></returns>
        public VerifyResult CreateBy(tbmDIR_GROUP inst)
        {
            try
            {
                //#region --- spINSERT_DIR_GROUP_BY_GROUP_DIR ----✘✘✘
                _dirGroupRepository.Create(inst);

                result.IsSuccess = true;
                result.Message = "系統目錄角色群組 權限已建立.";
                result.Data = inst;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "DirectoriesGroupService",
                    Method = "CreateBy",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "目錄角色群組權限新增.Exception",
                    ErrorMessage = string.Format($"系統目錄角色群組 權限新增失敗.\n {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"系統目錄角色群組 權限新增失敗【{ex.Message}】");
                result.Data = new { };
            }

            return result;
        }

        /// <summary>
        /// 修改 系統目錄角色群組 使用權限 tbmDIR_GROUP: 【EF Update】
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public VerifyResult UpdateBy(tbmDIR_GROUP inst)
        {
            try
            {
                //#region --- spUPDATE_DIR_GROUP_BY_GROUP_DIR ----✘✘✘
                _dirGroupRepository.Update(inst);

                result.IsSuccess = true;
                result.Message = "系統目錄角色群組 權限已更新.";
                result.Data = inst;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "DirectoriesGroupService",
                    Method = "UpdateBy",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "目錄角色群組權限修改.Exception",
                    ErrorMessage = string.Format($"系統目錄角色群組權限修改失敗.\n {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"系統目錄角色群組權限修改失敗【{ex.Message}】");
                result.Data = new { };
            }

            return result;
        }

        /// <summary>
        /// 刪除 系統目錄角色群組 使用權限 EXECUTE dbo.spDELETE_DIR_GROUP
        /// </summary>
        /// <param name="dirid"></param>
        /// <param name="groupid"></param>
        /// <param name="deleteby"></param>
        /// <returns></returns>
        public VerifyResult DeleteBy(long dirid, string groupid, string deleteby)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    #region --- spDELETE_DIR_GROUP ----
                    var _exec = _db.spDELETE_DIR_GROUP(dirid, groupid, deleteby).FirstOrDefault();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = "系統目錄角色群組 權限已刪除.";
                    }
                    else
                    {
                        result.IsSuccess = true;
                        result.Message = "系統目錄角色群組 權限刪除失敗【" + _exec.Split(':')[1] + "】";
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "DirectoriesGroupService",
                    Method = "DeleteBy",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"系統目錄角色群組 權限刪除失敗.\n {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"系統目錄角色群組 權限刪除失敗【{ex.Message}】");
                result.Data = new { };
            }

            return result;
        }

    }
}
