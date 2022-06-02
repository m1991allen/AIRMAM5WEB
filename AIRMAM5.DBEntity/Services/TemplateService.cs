using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Template;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AIRMAM5.DBEntity.Models.TemplateFields;
using AIRMAM5.DBEntity.Interface;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 自訂欄位樣版 dbo.tbmTemplate
    /// </summary>
    public class TemplateService
    {
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "fail");
        readonly ISerilogService _serilogService;
        readonly IGenericRepository<tbmTEMPLATE> _templateRepository = new GenericRepository<tbmTEMPLATE>();
        readonly IGenericRepository<tbmTEMPLATE_FIELDS> _templateFieldsRepository = new GenericRepository<tbmTEMPLATE_FIELDS>();

        public TemplateService()
        {
            //_db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }

        /// <summary>
        /// 指定樣板id 取 [tbmTEMPLATE] 
        /// </summary>
        /// <param name="tempid"></param>
        /// <returns></returns>
        public tbmTEMPLATE GetById(int tempid)
        {
            return _templateRepository.Get(x => x.fnTEMP_ID == tempid);
        }

        #region 【Procedure】
        /// <summary>
        /// 取出 tbmTEMPLATE 資料。【spGET_TEMPLATE】
        /// </summary>
        /// <param name="tempid">自訂欄位樣版編號</param>
        /// <param name="table">提供使用的目的資料表 fsCODE_ID='TEMP001'</param>
        /// <returns></returns>
        public List<spGET_TEMPLATE_Result> GetByParam(int tempid = 0, string table = "")
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_TEMPLATE(tempid, table).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_TEMPLATE_Result>();

                return query;
            }
        }

        /// <summary>
        /// 依照 TEMP_ID 取出TEMPLATE_FIELDS 主檔 資料【spGET_TEMPLATE_FIELDS_BY_TEMP_ID】 
        /// </summary>
        /// <param name="tempid">自訂欄位樣版編號</param>
        /// <returns></returns>
        public List<spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result> GetTemplateFieldsById(int tempid)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_TEMPLATE_FIELDS_BY_TEMP_ID(tempid).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result>();

                return query;
            }
        }
        
        /// <summary>
        /// 目錄節點編號[fnDIR_ID] (主題/影片/聲音/圖片/文件)自訂欄位樣板資料
        /// </summary>
        /// <param name="dirid"></param>
        /// <returns></returns>
        public List<spGET_DIRECTORIES_TEMPLATE_BY_DIR_ID_Result> GetDirTemplateById(int dirid)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_DIRECTORIES_TEMPLATE_BY_DIR_ID(dirid).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_DIRECTORIES_TEMPLATE_BY_DIR_ID_Result>();

                return query;
            }
        }
        #endregion

        /// <summary>
        /// 依 樣版編號取出 tbmTEMPLATE, tbmTEMPLATE_FIELDS  資料檢視 (Procedure)
        /// </summary>
        /// <param name="tempid">自訂欄位樣版編號</param>
        /// <returns></returns>
        public TemplateFieldsViewModel GetFieldsViewById(int tempid)
        {
            TemplateFieldsViewModel vmd = new TemplateFieldsViewModel() { fnTEMP_ID = tempid };

            vmd.TemplateMain = this.GetByParam(tempid)//.Select(s => new tbmTEMPLATE(s))
                .Select(s => new tbmTemplateGeneric<spGET_TEMPLATE_Result>().FormatConversion(s))
                .FirstOrDefault();

            vmd.CustomFieldList = this.GetTemplateFieldsById(tempid)
                    .Select(s => new ChooseTypeViewModel().FormatConversion(s))
                    //.Select(s => new ChooseTypeViewGeneric<spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result>().FormatConversion(s))
                    .ToList();

            return vmd;
        }

        /// <summary>
        /// 依 樣版編號取出 tbmTEMPLATE, tbmTEMPLATE_FIELDS  資料編輯  (Procedure)
        /// </summary>
        /// <param name="tempid"></param>
        /// <returns></returns>
        public TemplateFieldsEditModel GetFieldsEditById(int tempid)
        {
            var query = this.GetFieldsViewById(tempid);

            TemplateFieldsEditModel md = new TemplateFieldsEditModel()
            {
                fnTEMP_ID = tempid,
                TemplateMain = query.TemplateMain,
                CustomFieldList = query.CustomFieldList,
                //FieldTypes
                //TableList
            };
            
            return md;
        }

        #region 【取樣板自訂欄位+欄位值】---動態欄位
        /* ✘✘✘---Marked_20200309 : 
        ///// <summary>
        ///// (樣版動態欄位)依 自訂欄位樣板ID 取回自訂欄位列表 和對應的Model中的欄位值
        ///// </summary>
        ///// <typeparam name="T">樣版欄位對應的Model </typeparam>
        ///// <typeparam name="TV">樣版欄位對應的Model資料 </typeparam>
        ///// <param name="templateid">自訂欄位樣板ID </param>
        ///// <param name="model">Model資料內容 </param>
        ///// <returns></returns>
        ////public List<ArcPreAttributeModel> AttriFieldsGetValue<T, TV>(int templateid, TV model)
        //public List<ArcPreAttributeModel> AttriFieldsGetValue<T>(int templateid, T model)
        //{
        //    #region 樣版動態欄位.GetValue
        //    List<ArcPreAttributeModel> attrFields = new List<ArcPreAttributeModel>();
        //    var _query = this.GetById(templateid);
        //    FileTypeEnum mediatype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), _query.fsTABLE);

        //    //樣板自訂欄位資料(欄位屬性設定值)
        //    var getFields = this.GetTemplateFieldsById(templateid);   //spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result
        //    foreach (var f in getFields)
        //    {
        //        if (f == null) { continue; }
        //        var _value = typeof(T).GetProperties().FirstOrDefault(x => x.Name == f.fsFIELD).GetValue(model);
        //        var _attrVal = _value == null ? string.Empty : _value.ToString();
        //        //ArcPreAttributeModel _arrtF = new ArcPreAttributeModel(f) { FieldValue = _attrVal ?? "" };
        //        ArcPreAttributeModel<spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result> _tmpAttrs = new ArcPreAttributeModel<spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result>();
        //        ArcPreAttributeModel _arrtF = _tmpAttrs.FormatConversion(f, mediatype);

        //        attrFields.Add(_arrtF);
        //    }
        //    #endregion
        //    return attrFields;
        //} */

        /// <summary>
        /// (樣版動態欄位)將 表單Form內容, 依欄位名 填入指定(值)資料Model 名稱對應的欄位中。
        ///   [T]: 樣版欄位取值的來源Model。 [TV]:樣版欄位資料的Model
        /// </summary>
        /// <typeparam name="T">樣版欄位取值的來源Model </typeparam>
        /// <typeparam name="TV">樣版欄位資料的Model </typeparam>
        /// <param name="templateid">自訂欄位樣板ID </param>
        /// <param name="model">Model資料內容 </param>
        /// <param name="form">表單內容
        /// <para> 2020/4/9 如果沒有存在"接口名_IsEdit=on",就不能更新, 如: fsATTRIBUTE4_IsEdit=on </para></param>
        /// <param name="batchParam"> 是否有批次修改參數 如: "接口名_IsEdit=on"。false: 直接修改欄位值、true: 需要確認"接口名_IsEdit=on" </param>
        /// <returns></returns>
        public T AttriFieldsSetValue<T>(int templateid, T model, FormCollection form, bool batchParam = false)
        {
            var getFields = this.GetTemplateFieldsById(templateid);

            #region 樣板自訂欄位資料(依欄位屬性給值).SetValue
            foreach (var f in getFields)
            {
                string colName = f.fsFIELD, colNameChk = string.Format($"{f.fsFIELD}_IsEdit");
                string colValue = form[colName] ?? string.Empty;
                bool colCheck = string.IsNullOrEmpty(form[colNameChk]) ? false : (form[colNameChk].ToString().ToUpper() == "ON" ? true : false);

                if (f.fsFIELD_TYPE.ToUpper() == "CODE")
                {
                    colValue = string.IsNullOrEmpty(colValue) ? string.Empty : colValue.Replace(",", ";") + ";";
                }

                //typeof(T).GetProperties().FirstOrDefault(x => x.Name == colName).SetValue(model, colValue);
                if ((batchParam && colCheck) || (batchParam == false))
                    typeof(T).GetProperties().FirstOrDefault(x => x.Name == colName).SetValue(model, colValue);
            }
            #endregion

            return model;
        }
        #endregion

        #region 【SelectListItem】
        /// <summary>
        /// 取 自訂欄位樣板資料 選單(V,A,P,D,S)
        ///   多樣板選單, [table]可以分隔符號組成字串傳入,例: V,A,S 或 V;A;P;D;S
        /// </summary>
        /// <param name="table">提供使用的目的資料表 fsCODE_ID= TEMP001(V,A,P,D)</param>
        /// <param name="issearch">是否可檢索</param>
        /// <returns></returns>
        public List<SelectListItem> GetTemplateList(string table, bool? issearch = null)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            string _type = table, _is = issearch != null ? (issearch == true ? "Y" : "N") : null;
            
            var query = _templateRepository.FindBy(x => table.IndexOf(x.fsTABLE) >= 0);
            if (query.Any() && query.FirstOrDefault() != null)
            {
                query = query.Where(x => issearch == null ? true : x.fcIS_SEARCH == _is);//.ToList();
                listItems = query
                    .Select(s => new SelectListItem
                    {
                        Text = s.fsNAME,
                        Value = s.fnTEMP_ID.ToString()
                    }).ToList();
            }

            return listItems;
        }

        /// <summary>
        /// 取回 指定ID的樣版資料LIST
        /// </summary>
        /// <param name="tempids"></param>
        /// <returns></returns>
        public List<SelectListItem> GetTemplateListItem(List<TemplateIdModel> tempids)
        {
            List<int> ids = tempids.Select(s => s.fnTEMP_ID).ToList();
            var query = (from a in _templateRepository.GetAll()
                         join i in ids on new { no = a.fnTEMP_ID } equals new { no = i }
                         select new SelectListItem
                         {
                             Text = a.fsNAME,
                             Value = i.ToString()
                         }).Distinct().ToList();

            return query;
        }
        #endregion

        #region 【檢查判斷】
        /// <summary>
        /// 樣板名稱 是否重複 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ChkTemplateName(string name)
        {
            return  _templateRepository.FindBy(x => x.fsNAME == name).Any();
        }

        ///// <summary>
        ///// 樣板欄位名稱 是否重複 
        ///// </summary>
        ///// <param name="tempid"></param>
        ///// <param name="fieldname"></param>
        ///// <returns></returns>
        //public bool ChkTemplateFieldName(int tempid, string fieldname)
        //{
        //    return _templateFieldsRepository.FindBy(x => x.fnTEMP_ID == tempid && x.fsFIELD_NAME == fieldname).Any();
        //}

        ///// <summary>
        ///// 樣板欄位中,進階檢索數量不可超過４個
        ///// </summary>
        ///// <param name="tempid"></param>
        ///// <param name="fieldname"></param>
        ///// <returns></returns>
        //public bool ChkSearchFieldOverNum(int tempid, int num = 4)
        //{
        //    int cnt = _templateFieldsRepository.FindBy(x => x.fnTEMP_ID == tempid && x.fsIS_SEARCH == "Y").Count();
        //    if (cnt >= 4) return true; else return false;
        //}
        #endregion

        #region 【tbmTEMPLATE】自訂樣板: 新修刪
        /// <summary>
        /// 新增 自訂樣板 :【EF Create】。
        /// 複製COPY時,樣版欄位資料會一併複製產生。
        /// </summary>
        /// <param name="rec">資料內容 </param>
        /// <param name="template">NEW全新樣板 或 COPY複製樣板(預設=NEW) </param>
        /// <param name="existtemplate">既有樣板編號fnTEMP_ID(New樣板則為0) </param>
        /// <returns></returns>
        public VerifyResult CreateBy(tbmTEMPLATE rec, string template = "NEW", int existtemplate = 0)
        {
            using (var _db = new AIRMAM5DBEntities())
            {
                // ✘✘✘-- EXECUTE dbo.spINSERT_TEMPLATE --
                using (var _trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        #region (tbmTEMPLATE)樣板名稱檢查
                        if (ChkTemplateName(rec.fsNAME))
                        {
                            result = new VerifyResult(false, "樣板名稱重複.") { Data = rec };
                            _trans.Dispose();
                            return result;
                        }
                        #endregion
                        _templateRepository.Create(rec);

                        #region (tbmTEMPLATE_FIELDS)複製樣版新增,樣版欄位一併複製建立
                        if (template.ToUpper() == "COPY" && existtemplate > 0)
                        {
                            var _getfields = _templateFieldsRepository.FindBy(x => x.fnTEMP_ID == existtemplate);
                            foreach (var r in _getfields)
                            {
                                //_db.tbmTEMPLATE_FIELDS.Add(new tbmTEMPLATE_FIELDS(r)
                                //{
                                //    fnTEMP_ID = rec.fnTEMP_ID,
                                //    fdCREATED_DATE = DateTime.Now,
                                //    fsCREATED_BY = rec.fsCREATED_BY
                                //}); /* Marked_&_Modified_20210719 */
                                tbmTEMPLATE_FIELDS data = new tbmTEMPLATE_FIELDS().FormatConvert(r);
                                data.fnTEMP_ID = rec.fnTEMP_ID; //TIP: 要用新_fnTEMP_ID
                                data.fdCREATED_DATE = DateTime.Now;
                                data.fsCREATED_BY = rec.fsCREATED_BY;

                                _db.tbmTEMPLATE_FIELDS.Add(data);
                            }

                            _db.SaveChanges();
                        }
                        #endregion

                        _trans.Commit();
                        /* 回覆前端資料Model */
                        var rtnRec = new TemplateNewCopyModel(rec);
                        result = new VerifyResult(true, "自訂樣板已新增.") { Data = rtnRec };
                    }
                    catch (Exception ex)
                    {
                        _trans.Rollback();
                        #region Serilog.Err
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "TemplateService",
                            Method = "CreateBy",
                            EventLevel = SerilogLevelEnum.Error,
                            Input = new { Params = rec, Exception = ex },
                            LogString = "Exception",
                            ErrorMessage = string.Format($"自訂樣板新增失敗【{ex.Message}】 ")
                        });
                        #endregion
                        result = new VerifyResult(false, string.Format($"自訂樣板新增失敗【{ex.Message}】 "));
                    }
                    finally
                    {
                        _trans.Dispose();
                    }
                }
            } //using _db ...
            return result;
        }

        /// <summary>
        /// 編輯 自訂樣板 tbmTEMPLATE:【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateBy(tbmTEMPLATE rec)
        {
            try
            {
                // ✘✘✘--- Execute dbo.spUPDATE_TEMPLATE ---

                _templateRepository.Update(rec);
                result = new VerifyResult(true, "自訂樣板已更新.") { Data = rec };
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TemplateService",
                    Method = "UpdateBy",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"自訂樣板更新失敗. {ex.Message}")
                });
                #endregion
                result = new VerifyResult(false, string.Format($"自訂樣板更新失敗【{ex.Message}】 "));
            }

            return result;
        }
        
        /// <summary>
        /// 刪除 自訂樣板   Execute dbo.spDELETE_TEMPLATE
        /// </summary>
        /// <param name="tempid"></param>
        /// <param name="deleteby"></param>
        /// <returns></returns>
        public VerifyResult DeleteBy(int tempid, string deleteby)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    #region --- Execute dbo.spDELETE_TEMPLATE ---
                    //SP檢核: 樣板被使用，不得刪除此樣板!
                    var _exec = _db.spDELETE_TEMPLATE(tempid, deleteby).FirstOrDefault();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result = new VerifyResult(true, "自訂樣板已刪除.");
                    }
                    else
                    {
                        result = new VerifyResult(false, "自訂樣板刪除失敗【" + _exec.Split(new char[] { ':' })[1] + "】");
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TemplateService",
                    Method = "DeleteBy",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = new { tempid, deleteby }, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"自訂樣板刪除失敗. {ex.Message}")
                });
                #endregion
                result = new VerifyResult(false, string.Format($"自訂樣板刪除失敗【{ex.Message}】 "));
            }
            return result;
        }
        #endregion

    }
}
