using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.CodeSet;
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
    /// 代碼主、明細表 tbzCODE_SET、tbzCODE 
    /// </summary>
    public class CodeService : ICodeService
    {
        private readonly IGenericRepository<tbzCODE_SET> _codeSetRepository = new GenericRepository<tbzCODE_SET>();
        private readonly IGenericRepository<tbzCODE> _codeDetRepository = new GenericRepository<tbzCODE>();

        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "資料不正確");

        //public CodeService(AIRMAM5DBEntities db)
        //{
        //    _db = db;
        //    _serilogService = new SerilogService();
        //    _codeSetRepository = new GenericRepository<tbzCODE_SET>(_db);
        //    _codeDetRepository = new GenericRepository<tbzCODE>(_db);
        //}

        public CodeService()
        {
            _serilogService = new SerilogService();
        }
        public CodeService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        /// <summary>
        /// 取 編輯子代碼檔 View Modal資料
        /// </summary>
        /// <param name="codeid">代碼主表代碼 fsCODE_ID </param>
        /// <returns></returns>
        public CodeEditViewModel GetEditById(string codeid)
        {
            CodeEditViewModel md = new CodeEditViewModel();
            var _main = this.GetCodeMaster(codeid).FirstOrDefault();
            var _det = this.GetCodeDetail(codeid);

            md.CodeSet = new CodeSetEditModel().ConvertData(_main);
            md.CodeList = _det.Select(s => new CodeDataModel().FormatConversion(s)).ToList();
            md.Code = new CodeDataModel();

            return md;
        }

        #region 【檢查判斷】
        /// <summary>
        /// 檢查 主檔代碼[fsCODE_ID]是否被使用 From dbo.tbmTEMPLATE_FIELDS 【dbo.spGET_CODE_CUSTOM_USED】
        /// </summary>
        /// <param name="codeid">代碼主表代碼 fsCODE_ID </param>
        /// <param name="code">子代碼 fsCODE (預設空值 </param>
        /// <returns></returns>
        public bool ChkCodeIsUsedFromTemplateFields(string codeid, string code)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                //TODO-20191004: Procedure參數[fsCODE] 實際上並未使用.
                var chk = _db.spGET_CODE_CUSTOM_USED(codeid, code).FirstOrDefault().ToString();
                if (chk.ToUpper() == IsTrueFalseEnum.Y.ToString()) return true; else return false;
            }
        }

        /// <summary>
        /// 檢查 子代碼是否存在
        /// </summary>
        /// <returns></returns>
        public bool ChkCodeIsHad(string codeid, string code)
        {
            var query = _codeDetRepository.FindBy(x => x.fsCODE_ID == codeid && x.fsCODE == code);
            if (query.Any()) return true; else return false;
        }
        #endregion

        #region 【tbzCODE_SET】 代碼主表 QUERY/GET
        /// <summary>
        /// 代碼主表id 取 [tbzCODE_SET]資料
        /// </summary>
        /// <param name="codeid"></param>
        /// <returns></returns>
        public tbzCODE_SET GetCodeSetById(string codeid)
        {
            return _codeSetRepository.FindBy(x => x.fsCODE_ID == codeid).FirstOrDefault();
        }

        /// <summary>
        /// 取 代碼主表[tbzCODE_SET]: EXECUTE dbo.spGET_CODE_SET. (參數預設為空白,傳回全部資料)
        /// </summary>
        /// <param name="codeid">代碼主表代碼 fsCODE_ID </param>
        /// <param name="title">代碼主表標題 fsTITLE </param>
        /// <param name="type">代碼主表分類 fsTYPE </param>
        /// <returns></returns>
        public List<spGET_CODE_SET_Result> GetCodeMaster(string codeid = "", string title = "", string type = "")
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_CODE_SET(codeid, title, type).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_CODE_SET_Result>();

                return query;
            }
        }

        #endregion

        #region 【tbzCODE】 子代碼(明細) QUERY/GET
        /// <summary>
        /// 代碼主表id+ 子代碼 取 [tbzCODE]資料
        /// </summary>
        /// <param name="codeid">主代碼 fsCODE_ID </param>
        /// <param name="code">子代碼 fsCODE (選填)</param>
        /// <returns></returns>
        public List<tbzCODE> GetCodeById(string codeid, string code = "")
        {
            return _codeDetRepository.FindBy(x => x.fsCODE_ID == codeid)
                .Where(x => string.IsNullOrEmpty(code) ? true : x.fsCODE == code).ToList();
        }

        /// <summary>
        /// 取 子代碼檔[tbzCODE] : Execute dbo.spGET_CODE. (參數預設為空白,傳回全部資料)
        /// </summary>
        /// <param name="codeid">代碼主檔代碼 fsCODE_ID </param>
        /// <param name="code">子代碼 fsCODE </param>
        /// <param name="name">子代碼名稱 fsNAME </param>
        /// <returns></returns>
        public List<spGET_CODE_Result> GetCodeDetail(string codeid = "", string code = "", string name = "")
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_CODE(codeid, code, name).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_CODE_Result>();

                return query;
            }
        }

        /// <summary>
        /// 取 指定代碼主表代碼對應的 子代碼名稱 tbzCODE.fsNAME
        /// </summary>
        /// <param name="codeid">代碼主檔代碼 fsCODE_ID </param>
        /// <param name="code">子代碼 fsCODE </param>
        /// <returns></returns>
        public string GetCodeName(TbzCodeIdEnum codeid, string code)
        {
            var query = GetCodeDetail(codeid.ToString(), code);
            if (query.Any()) return query.FirstOrDefault().fsNAME;

            return null;
        }
        #endregion

        #region ---------- DropDownList【SelectListItem】
        /// <summary>
        /// 布林選項下拉清單 (True/False , 是/否, 有效/無效,....)
        /// </summary>
        /// <param name="s"> 指定顯示的中文. True/False , 是/否, 有效/無效, ..... </param>
        /// <returns></returns>
        public List<SelectListItem> GetBoolItemList(string[] s)
        {
            var items = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = IsTrueFalseEnum.Y.ToString(), //true.ToString(),
                    Text = s[0].ToString(),
                    //Selected = true,
                },
                new SelectListItem
                {
                    Value = IsTrueFalseEnum.N.ToString(),//false.ToString(),
                    Text = s[1].ToString()
                },
            };

            return items;
        }

        /// <summary>
        /// 指定主表CodeID 取明細代碼列表 
        /// </summary>
        /// <param name="codeid">主代碼</param>
        /// <param name="isenabled">是否啟用 </param>
        /// <param name="showcode">預設顯示"fsCODE fsNAEM"、True顯示"fsCODE fsNAEM"、False顯示"fsNAME" </param>
        /// <param name="hadall">是否顯示"全部"項目 預設false不顯示 </param>
        /// <returns></returns>
        public List<SelectListItem> GetCodeItemList(string codeid, bool? isenabled = null, bool showcode = false, bool hadall = false)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            string _is = isenabled == null ? null : (isenabled == true ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString());
            if (hadall)
            {
                listItems.Add(new SelectListItem { Text = " 全部- ", Value = "*", Selected = true });
            }

            var query = GetCodeDetail(codeid);
            query = query.Where(x => isenabled == null ? true : x.fsIS_ENABLED == _is).ToList();
            if (query.Any())
            {
                if(query.FirstOrDefault() == null) return listItems;

                var _get = query.OrderBy(r => r.fsCODE_ID).ThenBy(b => b.fnORDER).ThenBy(b => b.fsCODE)
                    .AsEnumerable()
                    .Select(s => new SelectListItem
                    {
                        Value = s.fsCODE,
                        Text = showcode == false ? s.fsNAME : string.Format($"{s.fsCODE} {s.fsNAME}")
                    }).ToList();
                listItems.AddRange(_get);
            }

            return listItems;
        }

        /// <summary>
        /// [fsCODE_ID] 傳入多筆[fsCODE] 符合者選取(selected) 回傳 SelectListItem
        /// </summary>
        /// <param name="codeid">明細代碼ID [fsCODE_ID]</param>
        /// <param name="codes">多筆代碼fsCode (多筆;分隔)</param>
        /// <returns></returns>
        public List<SelectListItem> CodeListItemSelected(string codeid, string codes)
        {
            List<SelectListItem> get = new List<SelectListItem>();
            var _codes = codes.Split(new char[] { ';' });

            var query = GetCodeDetail(codeid.ToString()).ToList();
            if (query.Any())
            {
                get = (from a in query
                        select new SelectListItem
                        {
                            Value = a.fsCODE,
                            Text = string.Format($"{a.fsNAME} "),
                            Selected = codes.IndexOf(a.fsCODE) > -1 ? true : false
                        }).ToList();
            }

            return get;
        }

        /// <summary>
        /// [fsCODE_ID] 指定的[fsCODE]內容  回傳 SelectListItem
        /// </summary>
        /// <param name="codeid">明細代碼ID [fsCODE_ID]</param>
        /// <param name="codes">多筆代碼fsCode (多筆;分隔)</param>
        /// <returns></returns>
        public List<SelectListItem> SpecifyCodeListItem(string codeid, string codes)
        {
            List<SelectListItem> get = new List<SelectListItem>();
            var _codes = codes.Split(new char[] { ';' });
            var query = GetCodeDetail(codeid).ToList();

            if (query.Any())
            {
                get = (from a in query
                       join b in _codes on a.fsCODE equals b
                       select new SelectListItem
                       {
                           Value = a.fsCODE,
                           Text = string.Format($"{a.fsNAME} "),
                       }).ToList();
            }

            return get;
        }

        /// <summary>
        /// 主代碼+子代碼 資料清單
        /// </summary>
        /// <param name="type">系統代碼S/自訂代碼C : CodeSetTypeEnum </param>
        /// <param name="isenabled">是否啟用, 預設=null </param>
        /// <param name="showcode">預設=true 顯示"fsTITLE(fsCODE_ID)"、True顯示"fsTITLE(fsCODE_ID)"、False顯示"fsTITLE" </param>
        /// <param name="hadall">是否顯示"全部"項目 預設false不顯示 </param>
        /// <returns></returns>
        public List<MainSubCodeListModel> GetMainSubList(string type, bool? isenabled = null, bool showcode = true, bool hadall = false)
        {
            string _is = isenabled == null ? null : (isenabled == true ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString());
            var query = GetCodeMaster(string.Empty, string.Empty, type).Where(x => isenabled == null ? true : x.fsIS_ENABLED == _is).ToList();
            if (query == null || query.FirstOrDefault() == null)
                return new List<MainSubCodeListModel>();

            var getList = query.Select(s => new MainSubCodeListModel
            {
                MainCodeId = s.fsCODE_ID,
                MainCodeName = s.fsTITLE,
                SubCodeList = this.GetCodeItemList(s.fsCODE_ID,isenabled, showcode, hadall)
            }).ToList();

            return getList;
        }
        #endregion

        #region 【tbzCODE_SET】 代碼主表: 新 修 刪
        /// <summary>
        /// 新建 代碼主表 tbzCODE_SET: 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult CreateCodeSet(tbzCODE_SET rec)
        {
            //VerifyResult result = new VerifyResult();
            try
            {
                //EXECUTE dbo.spINSERT_CODE_SET --✘✘
                _codeSetRepository.Create(rec);

                result.IsSuccess = true;
                result.Message = string.Format($"代碼主檔({rec.fsCODE_ID})已新增 ");
                result.Data = this.GetCodeMaster(rec.fsCODE_ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "CodeService",
                    Method = "[CreateCodeSet]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"代碼主檔新增失敗【{ex.Message}】 ")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"代碼主檔 新增失敗【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// 編輯 代碼主表 tbzCODE_SET : 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateCodeSet(tbzCODE_SET rec)
        {
            //VerifyResult result = new VerifyResult(false, "代碼主檔更新失敗(資料不正確)");
            result.Message = "代碼主檔更新失敗(資料不正確)";
            try
            {
                //Execute dbo.spUPDATE_CODE_SET ---✘✘
                _codeSetRepository.Update(rec);

                result.IsSuccess = true;
                result.Message = string.Format($"代碼主檔({rec.fsCODE_ID})已更新 ");
                result.Data = GetCodeMaster(rec.fsCODE_ID, rec.fsTITLE, rec.fsTYPE).FirstOrDefault();
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "CodeService",
                    Method = "[UpdateCodeSet]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"代碼主檔編輯失敗. {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"代碼主檔 編輯失敗【{ex.Message}】");
            }
            return result;
        }

        /// <summary>
        /// 刪除 代碼主表 tbzCODE_SET : Execute dbo.spDELETE_CODE_SET
        /// </summary>
        /// <param name="codeid"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public VerifyResult DeleteCodeSet(string codeid, string username)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    #region --- Execute dbo.spDELETE_CODE_SET ---
                    var _exec = _db.spDELETE_CODE_SET(codeid, username).FirstOrDefault();
                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = "代碼主檔已刪除";
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = string.Format($"代碼主檔刪除失敗【{_exec.Split(':')[1]}】");
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "CodeService",
                    Method = "[DeleteCodeSet]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"代碼主檔刪除失敗. {ex.Message}")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"代碼主檔 刪除失敗【{ex.Message}】");
            }

            return result;
        }
        #endregion

        #region 【tbzCODE】 子代碼(明細): 新 修 刪
        /// <summary>
        /// 新建 子代碼 tbzCODE: 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult CreateCodeDet(tbzCODE rec)
        {
            //VerifyResult result = new VerifyResult();
            try
            {
                // EXECUTE dbo.spINSERT_CODE --✘✘
                var code = _codeDetRepository.FindBy(x => x.fsCODE_ID == rec.fsCODE_ID && x.fsCODE == rec.fsCODE);
                if (code.Any())
                {
                    result.IsSuccess = false;
                    result.Message = string.Format($"子代碼{rec.fsCODE}已存在.");
                    result.Data = new CodeDataModel().FormatConversion(rec);
                    return result;
                }

                _codeDetRepository.Create(rec);
                result.IsSuccess =true;
                result.Message = string.Format($"子代碼{rec.fsCODE}已新增.");
                result.Data = new CodeDataModel().FormatConversion(rec);
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "CodeService",
                    Method = "[CreateCodeDet]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"子代碼檔新增失敗【{ex.Message}】 ")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"子代碼檔新增失敗! \r\n 【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// 修改 子代碼 tbzCODE: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateCodeDet(tbzCODE rec)
        {
            //VerifyResult result = new VerifyResult();
            try
            {
                // EXECUTE dbo.spINSERT_CODE --✘✘
                _codeDetRepository.Update(rec);

                result.IsSuccess = true;
                result.Message = string.Format($"子代碼({rec.fsCODE})已更新 ");
                result.Data = new CodeDataModel().FormatConversion(rec);
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "CodeService",
                    Method = "[UpdateCodeDet]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"子代碼修改失敗【{ex.Message}】 ")
                });
                #endregion
                result = new VerifyResult(false, string.Format($"子代碼檔修改失敗【{ex.Message}】"));
            }

            return result;
        }
        /// <summary>
        /// 刪除 子代碼 tbzCODE: 【EF Delete】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult DeleteCodeDet(CodeIdsModel param)
        {
            //VerifyResult result = new VerifyResult();
            try
            {
                // EXECUTE dbo.spDELETE_CODE --✘✘

                var rec = _codeDetRepository.FindBy(x => x.fsCODE_ID == param.CodeId && x.fsCODE == param.Code).FirstOrDefault();
                _codeDetRepository.Delete(rec);
                result.IsSuccess = true;
                result.Message = "子代碼已刪除";
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "CodeService",
                    Method = "[DeleteCodeDet]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = param, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"子代碼刪除失敗【{ex.Message}】 ")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"子代碼檔修改失敗【{ex.Message}】");
            }

            return result;
        }
        #endregion

    }

}
