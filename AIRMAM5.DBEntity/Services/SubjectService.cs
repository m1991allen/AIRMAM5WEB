using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Procedure;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.SubjExtend;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 主題檔_資料表 tbmSUBJECT
    /// </summary>
    public class SubjectService : ISubjectService
    {
        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "fail");
        private readonly IGenericRepository<tbmSUBJECT> _subJectRepository = new GenericRepository<tbmSUBJECT>();

        public SubjectService()
        {
            //_db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }
        public SubjectService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        /// <summary>
        /// 主題檔資料 tbmSUBJECT
        /// </summary>
        /// <param name="subjid"></param>
        /// <param name="dirid"></param>
        /// <returns></returns>
        public tbmSUBJECT GetBy(string subjid = null, long dirid = 0)
        {
            var query = _subJectRepository
                .FindBy(x => (string.IsNullOrEmpty(subjid) ? true : x.fsSUBJ_ID == subjid))
                .Where(y => (dirid == 0 ? true : y.fnDIR_ID == dirid));

            if (query == null || query.FirstOrDefault() == null) return new tbmSUBJECT();

            return query.FirstOrDefault();
        }

        /// <summary>
        /// 多筆主題編號 取 tbmSUBJECT 資料
        /// </summary>
        /// <param name="fnos"></param>
        /// <returns></returns>
        public List<tbmSUBJECT> GetByIds(string[] subjids)
        {
            var _ids = string.Join(";", subjids);
            var query = _subJectRepository.FindBy(x => SqlFunctions.CharIndex(x.fsSUBJ_ID, _ids) > 0).ToList();
            
            return query;
        }

        /// <summary>
        /// 取 主題編號fsSUBJ_ID 使用的"主題"樣版ID [fnTEMP_ID_SUBJECT]
        /// </summary>
        public int GetSubjTemplateId(string subjid)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = (from a in _db.tbmSUBJECT.Where(x => x.fsSUBJ_ID == subjid)
                             join b in _db.tbmDIRECTORIES on a.fnDIR_ID equals b.fnDIR_ID
                             where a.fsSUBJ_ID == subjid
                             select b);

                return query.Any() ? query.FirstOrDefault().fnTEMP_ID_SUBJECT : 0;
            }
        }

        /// <summary>
        /// 依 主題編號[fsSUBJ_ID] 取回 tbmSUBJECT 自訂欄位 【dbo.spGET_SUBJECT_ATTRIBUTE】
        /// </summary>
        /// <param name="subjid"></param>
        /// <returns></returns>
        public List<spGET_SUBJECT_ATTRIBUTE_Result> GetSubjAttribute(string subjid)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_SUBJECT_ATTRIBUTE(subjid).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_SUBJECT_ATTRIBUTE_Result>();

                return query;
            }
        }

        #region ---------- CURD 【tbmSUBJECT】: 新 修 刪
        /// <summary>
        /// 新建 媒資主題 tbmSUBJECT: 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="head">主題編號取號時指定 編號前段的值。預設空值=當日日期，格式: yyyyMMdd。</param>
        /// <returns></returns>
        public VerifyResult CreateBy(tbmSUBJECT rec, string head = "")
        {
            try
            {
                // -- EXECUTE dbo.spINSERT_SUBJECT --✘✘✘

                #region --- 取新號碼[fsSUBJ_ID] From dbo.tblNO ---
                var _tblnoService = new TblNoService();  /* Modified_20201211_使用共用參數值、head內容由前端表單獲取。 */

                string _type = SysNumberTypeEnum.SUBJECT.ToString(), 
                    _head = string.IsNullOrEmpty(head) ? string.Format($"{DateTime.Now:yyyyMMdd}") : head,
                    _body = SysNumberBodySet.BodySUBJECT, _name = SysNumberNameSet.NameSUBJECT;

                int _len = (int)SysNumberLenEnum.SUBJECT;
                string newSubjId =_tblnoService.GetNewNo(_type, _name, _head, _body, _len, rec.fsCREATED_BY);
                #endregion

                rec.fsSUBJ_ID = newSubjId;
                rec.fsTYPE1 = rec.fsTYPE1 ?? string.Empty;
                rec.fsTYPE2 = rec.fsTYPE2 ?? string.Empty;
                rec.fsTYPE3 = rec.fsTYPE3 ?? string.Empty;
                _subJectRepository.Create(rec);
                
                result.IsSuccess = true;
                result.Message = "媒資主題已新增";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"媒資主題 新增失敗【{ex.Message}】");
                if (ex.InnerException != null)
                {
                    result.Message = string.Format($"媒資主題 新增失敗【{ex.InnerException.Message}】");
                }
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "SubjectService",
                    Method = "[CreateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Result = result, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"媒資主題新增失敗【{ex.Message}】 ")
                });
                #endregion
            }

            return result;
        }
        
        /// <summary>
        /// 表單Form - 修改更新 媒資主題 tbmSUBJECT: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateBy(FormCollection form, string updateby)
        {
            try
            {
                // -- EXECUTE dbo.spUPDATE_SUBJECT --✘✘✘

                string _subjId = form["SubjectId"].ToString();
                int _subjTempid = this.GetSubjTemplateId(_subjId);

                tbmSUBJECT getSubj = _subJectRepository.Get(x => x.fsSUBJ_ID == _subjId);
                getSubj.fsTITLE = form["Title"].ToString();
                getSubj.fsDESCRIPTION = form["Description"].ToString();
                getSubj.fdUPDATED_DATE = DateTime.Now;
                getSubj.fsUPDATED_BY = updateby;

                //樣板自訂欄位資料(依欄位屬性給值).SetValue --------------->> Tips: 改成共用。
                //Tips_20200409:增加參數判斷批次修改"接口名_IsEdit=on",預設=false
                new TemplateService().AttriFieldsSetValue<tbmSUBJECT>(_subjTempid, getSubj, form);

                _subJectRepository.Update(getSubj);

                result.IsSuccess = true;
                result.Message = "媒資主題已更新";
                result.Data = new { fsSUBJ_ID = _subjId, getSubj.fsTITLE };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"媒資主題修改失敗【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "SubjectService",
                    Method = "[UpdateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = "FormCollection", Result = result, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 檔案搬移 - '單筆/多筆批次'修改更新 媒資主題 tbmSUBJECT: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateByMove(List<tbmSUBJECT> rec)
        {
            try
            {
                _subJectRepository.UpdateMultiple(rec);

                result.IsSuccess = true;
                result.Message = "主題搬移目錄節點,更新完成.";
                //TODO:搬移更新後,暫不回傳Data內容
                result.Data = new { };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"主題搬移目錄節點,更新失敗【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "SubjectService",
                    Method = "[UpdateByMove]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = "FormCollection", Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"主題搬移目錄節點_Exception【{ex.Message}】 ")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 刪除 媒資主題 tbmSUBJECT: 【EF Update】
        /// </summary>
        /// <param name="subjid"></param>
        /// <param name="deleteby"></param>
        /// <returns></returns>
        public VerifyResult DeleteBy(string subjid, string deleteby)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    #region --- Execute dbo.spDELETE_SUBJECT ---
                    //SP檢核: 主題尚有現存或暫刪除的影音圖文，不得刪除此主題!
                    var _exec = _db.spDELETE_SUBJECT(subjid, deleteby).FirstOrDefault();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = string.Format($"主題檔案({subjid})已刪除。");
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = string.Format($"主題檔案({subjid})刪除失敗。【{_exec.Split(new char[] { ':' })[1]}】");
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"主題檔案({subjid})刪除失敗。【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "SubjectService",
                    Method = "[DeleteBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = new { subjid, deleteby }, Result = result, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return result;
        }
        #endregion

        #region >>> 擴充功能:新聞文稿,公文系統
        /// <summary>
        /// 擴充功能{新聞文稿/公文系統} 動態的查詢欄位 
        /// </summary>
        /// <param name="type">對應表類型 dbo.tbmCOLUMN_MAPPING.fsTYPE, i.g.INEWS、CONTRACT </param>
        /// <returns></returns>
        public List<SubjExtendColModel> SubjExtendQryCols(string type)
        {
            List<SubjExtendColModel> list = new List<SubjExtendColModel>();
            using (_db = new AIRMAM5DBEntities())
            {
                list = _db.tbmCOLUMN_MAPPING.Where(x => x.fsTYPE == type && x.fbIS_SEARCH == true)
                    .OrderBy(b => b.fnORDER)
                    .Select(s => new SubjExtendColModel
                    {
                        Text = s.fsNAME,
                        Value = s.fsMAIN_COLUMN,
                        DataType = s.fsDATATYPE.ToLower()
                    }).ToList();
            }

            return list;
        }
        
        /// <summary>
        /// 擴充功能{新聞文稿} 選取後更新自訂欄位存檔  dbo.spUPDATE_INEWS
        /// </summary>
        /// <param name="upd">選取存檔參數 (預存參數) </param>
        /// <returns></returns>
        public VerifyResult UpdateINews(Update_INews_Param<string> upd)
        {
            string msg = string.Format($"主題檔案({upd.FileNo})文稿對應存檔 ");

            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    //var exec = _db.spUPDATE_INEWS(upd.FileNo, upd.StoryID, upd.UpdateBy).FirstOrDefault();
                    var exec = _db.spUPDATE_INEWS(upd.FileNo, upd.Fields, upd.Values, upd.UpdateBy).FirstOrDefault();
                    if (exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = string.Format($"{msg} 完成。");
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = string.Format($"{msg} 失敗。【{exec.Split(new char[] { ':' })[1]}】");
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"{msg} 發生例外。【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "SubjectService",
                    Method = "[UpdateINews]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = upd, Result = result, Exception = ex },
                    LogString = "文稿對應存檔.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
            }

            return result;
        }
        #endregion
    }
}
