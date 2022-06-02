using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Linq;
using System.Reflection;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 自訂欄位 tbmTEMPLATE_FIELDS
    /// </summary>
    public class TemplateFieldsService
    {
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "fail");
        readonly ISerilogService _serilogService;
        readonly IGenericRepository<tbmTEMPLATE> _templateRepository = new GenericRepository<tbmTEMPLATE>();
        readonly IGenericRepository<tbmTEMPLATE_FIELDS> _templateFieldsRepository = new GenericRepository<tbmTEMPLATE_FIELDS>();

        public TemplateFieldsService()
        {
            //_db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }

        /// <summary>
        /// 取指定 自訂欄位名稱 資料 [tbmTEMPLATE_FIELDS]
        /// </summary>
        /// <param name="tempid">樣板編號ID </param>
        /// <param name="fieldname">自訂欄位名 </param>
        /// <returns></returns>
        public tbmTEMPLATE_FIELDS GetFieldBy(int tempid, string fieldname)
        {
            return _templateFieldsRepository.Get(x => x.fnTEMP_ID == tempid && x.fsFIELD == fieldname);
        }

        #region 【檢查判斷】
        /// <summary>
        /// 樣板-自訂欄位名稱 是否重複 
        /// </summary>
        /// <param name="tempid"></param>
        /// <param name="fieldname"></param>
        /// <param name="field">欄位代碼,如:fsATTRIBUTE1, fsATTRIBUTE2...。輸入值時,表示要排除該欄位代碼的重複條件。 </param>
        /// <returns></returns>
        public bool ChkTemplateFieldName(int tempid, string fieldname, string field = null)
        {
            var qry = _templateFieldsRepository
                .FindBy(x => x.fnTEMP_ID == tempid && x.fsFIELD_NAME == fieldname)
                .Where(x => field == null ? true : x.fsFIELD != field);

            return qry.Any();
        }

        /// <summary>
        /// 樣板-自訂欄位中,進階檢索數量不可超過6個
        /// </summary>
        /// <param name="tempid"></param>
        /// <param name="field"></param>
        /// <param name="is_search"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool ChkSearchFieldOverNum(int tempid, string field, bool is_search, int num = 6)
        {
            var qry = _templateFieldsRepository.FindBy(x => x.fnTEMP_ID == tempid && x.fsFIELD != field && x.fsIS_SEARCH == "Y");

            int cnt = qry.Count();

            if (cnt >= 6 && is_search) return true; else return false;
        }
        #endregion

        #region 【tbmTEMPLATE_FIELDS】自訂樣板欄位: 新修刪
        /// <summary>
        /// 新增 樣板欄位。 :【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult CreateField(tbmTEMPLATE_FIELDS rec)
        {
            try
            {
                // ✘✘✘-- EXECUTE dbo.spINSERT_TEMPLATE_FIELDS --

                using (_db = new AIRMAM5DBEntities())
                {
                    #region 新增欄位代號,取得可用欄位
                    // 取樣版編號目前已設定的欄位代號資料
                    var tmpFields = _db.spGET_TEMPLATE_FIELDS_BY_TEMP_ID(rec.fnTEMP_ID).DefaultIfEmpty().ToList();

                    if (tmpFields == null || tmpFields.FirstOrDefault() == null)
                    {
                        rec.fsFIELD = "fsATTRIBUTE1";
                    }
                    else
                    {
                        // 自訂欄位數量 從[預編詮釋資料表tbmARC_PRE]取得
                        string _col = string.Format($"fsATTRIBUTE");
                        //PropertyInfo[] props = typeof(spGET_ARC_PRE_Result).GetProperties();//.Where(x => x.Name.StartsWith("fsATTRIBUTE"));
                        PropertyInfo[] props = typeof(tbmARC_PRE).GetProperties();

                        for (int i = 1; i <= props.Count(); i++)
                        {
                            bool _exist = false;
                            _col = string.Format($"fsATTRIBUTE{i}");
                            foreach (var itm in tmpFields)
                            {
                                if (_col == itm.fsFIELD) { _exist = true; break; }
                            }
                            if (!_exist)
                            {
                                rec.fsFIELD = _col;
                                //排序由前端決定
                                //rec.fnORDER = i;
                                break;
                            }
                        }
                    }
                    #endregion
                }

                rec.fdCREATED_DATE = DateTime.Now;
                _templateFieldsRepository.Create(rec);

                result = new VerifyResult(true, "樣板欄位已新增.") { Data = rec };
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TemplateService",
                    Method = "CreateField",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"樣板欄位新增失敗【{ex.Message}】 ")
                });
                #endregion
                result = new VerifyResult(false, string.Format($"樣板欄位新增失敗【{ex.Message}】 "));
            }

            return result;
        }

        /// <summary>
        /// 編輯 樣板欄位 tbmTEMPLATE:【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateField(tbmTEMPLATE_FIELDS rec)
        {
            VerifyResult result = new VerifyResult();

            try
            {
                // ✘✘--- Execute dbo.spUPDATE_TEMPLATE_FIELDS ---

                /* marked --------------
                //var get = _templateFieldsRepository.FindBy(x => x.fnTEMP_ID == rec.fnTEMP_ID && x.fsFIELD == rec.fsFIELD).FirstOrDefault();
                ////get.fsFIELD = rec.fsFIELD; //屬性 'fsFIELD' 是此物件之索引鍵資訊的一部分，不能修改
                //get.fsFIELD_NAME = rec.fsFIELD_NAME;
                //get.fsFIELD_TYPE = rec.fsFIELD_TYPE;
                //get.fnFIELD_LENGTH = rec.fnFIELD_LENGTH ?? 0;
                //get.fsDESCRIPTION = rec.fsDESCRIPTION ?? string.Empty;
                //get.fnORDER = rec.fnORDER;
                //get.fnCTRL_WIDTH = rec.fnCTRL_WIDTH ?? 0;
                //get.fsMULTILINE = rec.IsMultiline ? "Y" : "N";
                //get.fsISNULLABLE = rec.IsNullable ? "Y" : "N";
                //get.fsDEFAULT = rec.fsDEFAULT ?? string.Empty;
                //get.fsCODE_ID = rec.fsCODE_ID ?? string.Empty;
                //get.fnCODE_CNT = rec.fnCODE_CNT ?? 1;  //0:多選、1:單選
                //get.fsCODE_CTRL = rec.fsCODE_CTRL ?? string.Empty;
                //get.fsIS_SEARCH = rec.IsSearch ? "Y" : "N";
                //get.fdUPDATED_DATE = DateTime.Now;
                //get.fsUPDATED_BY = rec.fsUPDATED_BY; */

                _templateFieldsRepository.Update(rec);
                result = new VerifyResult(true, "樣板欄位已更新.") { Data = rec };
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TemplateFieldsService",
                    Method = "UpdateField",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"樣板欄位更新失敗. {ex.Message}")
                });
                #endregion
                result = new VerifyResult(false, string.Format($"樣板欄位更新失敗【{ex.Message}】 "));
            }

            return result;
        }

        /// <summary>
        /// 刪除 樣板欄位 tbmTEMPLATE:【EF Update】
        /// </summary>
        /// <param name="tempid"></param>
        /// <param name="fields"></param>
        /// <param name="deleteby"></param>
        /// <returns></returns>
        public VerifyResult DeleteField(int tempid, string fields, string deleteby)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    #region --- Execute dbo.spDELETE_TEMPLATE_FIELDS ---
                    //SP檢核: 檢查是否有DIR使用此樣版
                    //1.樣板被使用，不得刪除此樣板!
                    //2.此樣板已有預編詮釋資料，不得刪除此樣板欄位!
                    var _exec = _db.spDELETE_TEMPLATE_FIELDS(tempid, fields, deleteby).FirstOrDefault();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result = new VerifyResult(true, "樣板欄位已刪除.");
                    }
                    else
                    {
                        result = new VerifyResult(false, "樣板欄位刪除失敗【" + _exec.Split(':')[1] + "】");
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TemplateFieldsService",
                    Method = "DeleteField",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = new { tempid, deleteby }, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"樣板欄位刪除失敗. {ex.Message}")
                });
                #endregion
                result = new VerifyResult(false, string.Format($"樣板欄位刪除失敗【{ex.Message}】 "));
            }

            return result;
        }
        #endregion

    }
}
