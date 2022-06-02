using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Services
{
    public class LicenseService : ILicenseService
    {
        protected AIRMAM5DBEntities _db;
        protected VerifyResult verifyResult = new VerifyResult();
        readonly IGenericRepository<tbmLICENSE> _licenseRepository = new GenericRepository<tbmLICENSE>();
        readonly ISerilogService _serilogService;

        public LicenseService()
        {
            _serilogService = new SerilogService();
        }
        public LicenseService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        /// <summary>
        /// 版權代碼是否存在
        /// </summary>
        /// <param name="code">版權代碼 fsCODE </param>
        /// <returns></returns>
        public bool IsExists(string code)
        {
            return _licenseRepository.FindBy(x => x.fsCODE == code).Any();
        }
        /// <summary>
        /// 版權名稱是否存在
        /// </summary>
        /// <param name="name">版權名稱 fsNAME </param>
        /// <returns></returns>
        public bool IsExistsByName(string name)
        {
            //return _licenseRepository.FindBy(x => x.fsNAME.Contains(name.Trim())).Any();
            return _licenseRepository.FindBy(x => x.fsNAME.ToUpper() == name.Trim().ToUpper()).Any();
        }

        /// <summary>
        /// 版權資料
        /// </summary>
        /// <param name="code">版權代碼 [fsCODE], 不指定取回全部資料。</param>
        /// <returns></returns>
        public List<tbmLICENSE> GetBy(string code = null)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var qry = _db.tbmLICENSE.Where(x => string.IsNullOrEmpty(code) ? true : x.fsCODE == code).DefaultIfEmpty();

                return qry.ToList();
            }
        }

        /// <summary>
        /// 搜尋版權資料 (指定日期時, 搜尋[版權到期日期]大於等 enddt )
        /// </summary>
        /// <param name="name">版權名稱(模糊比對 </param>
        /// <param name="IsActive">是否指定啟用true/禁用false/全部null 資料, 預設是全部null。</param>
        /// <param name="enddt">授權結束日期(可為空值 </param>
        /// <remarks>  TIP_20211008_決議:不用特別限制版權條件
        /// </remarks>
        /// <returns></returns>
        public List<tbmLICENSE> SearchBy(string name, bool? IsActive = null, string enddt = "")
        {
            DateTime? due = string.IsNullOrEmpty(enddt) ? (DateTime?)null : new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            using (_db = new AIRMAM5DBEntities())
            {
                var qry = _db.tbmLICENSE//.Where(x => x.fcIS_ACTIVE == true)
                    .Where(x => IsActive == null ? true : (IsActive == true ? x.fcIS_ACTIVE == true : x.fcIS_ACTIVE == false))
                    .Where(x => string.IsNullOrEmpty(name) ? true : x.fsNAME.Contains(name))
                    .Where(x=> string.IsNullOrEmpty(enddt) ? true : x.fdENDDATE >= due)
                    .DefaultIfEmpty();

                return qry.Any() ? qry.ToList() : null;
            }
        }

        /// <summary>
        /// 版權資料選單
        /// </summary>
        /// <param name="IsActive">是否指定啟用true/禁用false/全部null 資料, 預設是全部null。</param>
        /// <param name="IsOverdue">是否包括逾期資料, 預設為不包括逾期資料false。</param>
        /// <remarks>  TIP_20211008_決議:不用特別限制版權條件
        /// </remarks>
        /// <returns></returns>
        public List<SelectListItem> GetListItem(bool? IsActive = null, bool IsOverdue = false)
        {
            DateTime due = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            using (_db = new AIRMAM5DBEntities())
            {
                var qry = _db.tbmLICENSE//.Where(x => x.fcIS_ACTIVE == true)
                    .Where(x => IsActive == null ? true : (IsActive == true ? x.fcIS_ACTIVE == true : x.fcIS_ACTIVE == false))
                    .Where(x => IsOverdue == false ? (x.fdENDDATE == null || x.fdENDDATE >= due ) : true)
                    //.AsEnumerable()
                    .OrderBy(b => b.fnORDER)
                    .Select(s => new SelectListItem
                    {
                        Value = s.fsCODE,
                        Text = s.fsNAME
                    }).DefaultIfEmpty();

                return qry.ToList();
            }
        }

        #region 【dbo.tbmANNOUNCE】 修改/新增
        /// <summary>
        /// 編輯存檔
        /// </summary>
        /// <typeparam name="T">傳入的資料格式model </typeparam>
        /// <param name="rec">傳入的資料內容 </param>
        /// <returns> <see cref="tbmLICENSE"/></returns>
        public VerifyResult Update<T>(T rec)//Update(tbmLICENSE rec)
        {
            VerifyResult result = new VerifyResult(false, "無效的資料內容");
            if (rec == null) { return result; }

            try
            {
                var upd = new tbmLICENSE().DataConvert(rec);
                _licenseRepository.Update(upd);

                result.IsSuccess = true;
                result.Message = "版權資料修改成功";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "LicenseService",
                    Method = "Update",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = rec, Result = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"版權資料修改失敗【{ex.Message}】 ")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 新增存檔
        /// </summary>
        /// <typeparam name="T">傳入的資料格式model </typeparam>
        /// <param name="rec">傳入的資料內容 </param>
        /// <returns> <see cref="tbmLICENSE"/></returns>
        public VerifyResult Create<T>(T rec)
        {
            VerifyResult result = new VerifyResult(false, "無效的資料內容");
            if (rec == null) { return result; }

            try
            {
                var ins = new tbmLICENSE().DataConvert(rec);
                _licenseRepository.Create(ins);

                result.IsSuccess = true;
                result.Message = "版權資料新增成功";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "LicenseService",
                    Method = "Create",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = rec, Result = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"版權資料新增失敗【{ex.Message}】 ")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = ex.Message;
            }

            return result;
        }

        #endregion
    }
}
