using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.ArcPre;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 預編詮釋資料 tbmARC_PRE
    /// </summary>
    public class ArcPreService
    {
        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult();
        private readonly IGenericRepository<tbmARC_PRE> _arcPreRepository = new GenericRepository<tbmARC_PRE>();

        /// <summary>
        /// 預編詮釋資料 tbmARC_PRE
        /// </summary>
        public ArcPreService()
        {
            //_db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }
        /// <summary>
        /// 預編詮釋資料 tbmARC_PRE
        /// </summary>
        public ArcPreService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        /// <summary>
        /// 取出ARC_PRE預編詮釋資料【spGET_ARC_PRE】
        /// </summary>
        /// <param name="param">fnPRE_ID, fsNAME, fsTYPE, fnTEMP_ID </param>
        /// <returns></returns>
        public List<spGET_ARC_PRE_Result> GetByParam(spGET_ARC_PRE_Param param)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_PRE(param.fnPRE_ID, param.PreName, param.PreType, param.PreTempId).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null) return new List<spGET_ARC_PRE_Result>();

                return query;
            }
        }

        /// <summary>
        /// 取出 ARC_PRE 自訂欄位資料【spGET_ARC_PRE_ATTRIBUTE】
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<spGET_ARC_PRE_ATTRIBUTE_Result> GetAttributeByParam(spGET_ARC_PRE_ATTRIBUTE_Param param)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_PRE_ATTRIBUTE(param.fnPRE_ID, param.PreTempId).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null) return new List<spGET_ARC_PRE_ATTRIBUTE_Result>();

                return query;
            }
        }

        /// <summary>
        /// 取 預編詮釋資料 
        /// </summary>
        /// <param name="fnpreid">預編詮釋ID</param>
        /// <param name="pretempid">套用樣版ID ((預設可不指定) </param>
        /// <returns></returns>
        public tbmARC_PRE GetById(long fnpreid, int pretempid = 0)
        {
            var get = _arcPreRepository.FindBy(x => x.fnPRE_ID == fnpreid && (pretempid == 0 ? true : x.fnTEMP_ID == pretempid)).FirstOrDefault();

            return get;
        }

        /// <summary>
        /// 套用預編詮資料(至影音圖文資料中)
        /// </summary>
        /// <typeparam name="T">資料套用至 目標資料表(ex,影音圖文資料表 tbmARC_DOC...)</typeparam>
        /// <param name="fnpreid">預編詮釋資料ID </param>
        /// <param name="target">目標資料表 資料內容 </param>
        public T UseArcPreAttribute<T>(long fnpreid, T target)
        {
            var _arcPre = this.GetById(fnpreid);
            //fsTITLE, fsDESCRIPTION, fsATTRIBUTE??
            var _attr = typeof(T).GetProperties().Select(x => x.Name).Where(x => x.Contains("fsATTRIBUTE")).ToList();

            foreach (var f in _attr)
            {
                var _value = typeof(tbmARC_PRE).GetProperties().FirstOrDefault(x => x.Name == f).GetValue(_arcPre);
                typeof(T).GetProperties().FirstOrDefault(x => x.Name == f).SetValue(target, _value);
            }

            return target;
        }

        #region ---------- DropDownList【SelectListItem】
        /// <summary>
        /// 依分類(fsTYPE) 取 預編詮釋資料清單
        /// </summary>
        /// <param name="type">類別: 主題、影、音、圖、文 </param>
        /// <param name="tempid">樣板Id(選填)</param>
        /// <param name="noopts">內容是否有"無"的選單項目 預設:false </param>
        /// <returns></returns>
        public List<SelectListItem> GetArcPreListBy(string type, long tempid = 0, bool noopts = false)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            var query = _arcPreRepository.FindBy(x => x.fsTYPE == type).Where(x => tempid == 0 ? true : x.fnTEMP_ID == tempid);
            if (query.Any())
            {
                listItems = query.Select(s => new SelectListItem
                {
                    Value = s.fnPRE_ID.ToString(),
                    Text = s.fsNAME
                }).ToList();
            }

            if (noopts) { listItems.Insert(0, new SelectListItem { Value = "0", Text = "無", Selected = true }); }

            return listItems;
        }
        #endregion

        #region ----------新、修、刪
        /// <summary>
        /// 新建存檔 tbmARC_PRE: 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult CreateBy(tbmARC_PRE rec)
        {
            try
            {
                //#region -- EXECUTE dbo.spINSERT_ARC_PRE --✘✘✘
                _arcPreRepository.Create(rec);

                result.IsSuccess = true;
                result.Message = "預編詮釋已新增";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcPreService",
                    Method = "[CreateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"預編詮釋新增失敗【{ex.Message}】 ")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"預編詮釋新增失敗【{ex.Message}】");
            }
            return result;
        }

        /// <summary>
        /// 編輯存檔處理***
        /// </summary>
        /// <returns></returns>
        public VerifyResult UpdateBy(tbmARC_PRE rec)
        {
            //預編類型,使用樣板: 不可編輯。
            try
            {
                //#region -- EXECUTE dbo.spUPDATE_ARC_PRE --✘✘✘
                _arcPreRepository.Update(rec);

                result.IsSuccess = true;
                result.Message = "預編詮釋編輯存檔成功";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcPreService",
                    Method = "[UpdateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"預編詮釋編輯存檔失敗【{ex.Message}】 ")
                });
                #endregion

                result.IsSuccess = false;
                result.Message = string.Format($"預編詮釋編輯存檔失敗【{ex.Message}】");
            }
            return result;
        }

        /// <summary>
        /// 刪除 【spDELETE_ARC_PRE】
        /// </summary>
        /// <param name="fnpreid">預編詮釋編號 fnPRD_ID</param>
        /// <param name="deleteby"></param>
        /// <returns></returns>
        public VerifyResult DeleteBy(long fnpreid, string deleteby)
        {
            try
            {
                //#region -- EXECUTE dbo.spDELETE_ARC_PRE --✘✘✘
                var get = GetById(fnpreid);
                if (get == null)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"預編詮釋({fnpreid})不存在.");
                }
                else
                {
                    _arcPreRepository.Delete(get);

                    result.IsSuccess = true;
                    result.Message = "預編詮釋已刪除.";
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format($"預編詮釋刪除失敗【{ex.Message}】 ");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcPreService",
                    Method = "[DeleteBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = new { fnpreid, deleteby }, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = msg
                });
                #endregion
                result.IsSuccess = false;
                result.Message = msg;
            }
            return result;
        }
        #endregion

    }
}
