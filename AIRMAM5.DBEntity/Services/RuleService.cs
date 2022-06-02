using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Rule;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// (流程)規則  資料表: tbmRULE(規則定義資料表), tbmRULE_FILTER(規則條件資料表), tbmRULE_TABLE(規則目標資料表欄位定義資料表)
    /// </summary>
    public class RuleService
    {
        ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "無效的資料內容");
        readonly IGenericRepository<tbmRULE> _ruleRepository = new GenericRepository<tbmRULE>();
        readonly IGenericRepository<tbmRULE_FILTER> _ruleFilterRepository = new GenericRepository<tbmRULE_FILTER>();
        readonly IGenericRepository<tbmRULE_TABLE> _ruleTableRepository = new GenericRepository<tbmRULE_TABLE>();

        /// <summary>
        /// (流程)規則  資料表: tbmRULE(規則定義資料表), tbmRULE_FILTER(規則條件資料表), tbmRULE_TABLE(規則目標資料表欄位定義資料表)
        /// </summary>
        public RuleService()
        {
            _serilogService = new SerilogService();
            //this._db = new AIRMAM5DBEntities();
        }
        /// <summary>
        /// (流程)規則  資料表: tbmRULE(規則定義資料表), tbmRULE_FILTER(規則條件資料表), tbmRULE_TABLE(規則目標資料表欄位定義資料表)
        /// </summary>
        public RuleService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        #region >>> IsExists
        /// <summary>
        /// 流程規則 [tbmRULE] 是否有資料
        /// </summary>
        /// <param name="category">流程類別 參考代碼 <see cref="TbzCodeIdEnum.RULE"/>
        ///         <para>子代碼:0調用 BOOKING、1入庫 UPLOAD、2轉檔 TRANSCODE </para></param>
        /// <returns></returns>
        public bool RuleIsExists(string category)
        {
            var query = _ruleRepository.FindBy(x => x.fsRULECATEGORY == category);

            return query.Any() ? true : false;
        }

        /// <summary>
        /// 規則條件邏輯資料表 [tbmRULE_FILTER] 是否已有資料
        /// </summary>
        /// <param name="category"> 規則類別(RULE): 參考代碼 <see cref="TbzCodeIdEnum.RULE"/>
        ///         <para>子代碼:0調用 BOOKING、1入庫 UPLOAD、2轉檔 TRANSCODE </para>
        /// </param>
        /// <returns></returns>
        public bool IsExists(string category, string table = null, string column = null)
        {
            var query = _ruleFilterRepository.FindBy(x => x.fsRULECATEGORY == category)
                .Where(y => (string.IsNullOrEmpty(table) ? true : y.fsTABLE == table) 
                        && (string.IsNullOrEmpty(column) ? true : y.fsCOLUMN == column));

            return query.Any() ? true : false;
        }
        #endregion

        /// <summary>
        ///  tbmRULE(規則定義資料表)
        /// </summary>
        /// <param name="category">規則類別: 參考代碼"RULE", 調用 BOOKING、入庫 UPLOAD、轉檔 TRANSCODE , 預設:null(全部) </param>
        /// <returns></returns>
        public List<tbmRULE> GetRuleBy(string category = null)
        {
            var query = _ruleRepository.FindBy(x => string.IsNullOrEmpty(category) ? true : x.fsRULECATEGORY == category).ToList();

            return query;
        }

        /// <summary>
        /// tbmRULE_FILTER(規則條件邏輯資料表)
        /// </summary>
        /// <param name="category"></param>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public List<tbmRULE_FILTER> GetRuleFilterBy(string category, string table=null, string column=null)
        {
            var query = _ruleFilterRepository.FindBy(x => x.fsRULECATEGORY ==category)
                .Where(y => (string.IsNullOrEmpty(table) ? true : y.fsTABLE == table) && (string.IsNullOrEmpty(column) ? true : y.fsCOLUMN == column))
                .ToList();

            if (query == null || query.FirstOrDefault() == null)
                return new List<tbmRULE_FILTER>();

            return query;
        }

        /// <summary>
        /// tbmRULE_TABLE(規則目標資料表欄位定義資料表)
        /// </summary>
        /// <param name="category"> (規則類別) : 0調用 BOOKING、1入庫 UPLOAD、2轉檔 TRANSCODE </param>
        /// <param name="table"> 規則資料表 u 預設:null(不指定)</param>
        /// <returns></returns>
        public List<tbmRULE_TABLE> GetRuleTableBy(string category, string table = null )
        {
            var query = _ruleTableRepository//.FindBy(x => x.fsRULECATEGORY == category)
                .FindBy(x => SqlFunctions.CharIndex(category, x.fsRULECATEGORY) > 0)
                .Where(x => string.IsNullOrEmpty(table) ? true : x.fsTABLE == table)
                .ToList();

            return query;
        }

        /// <summary>
        /// 規則設定的條件資料(規則類別+Table+column) 
        /// </summary>
        /// <param name="category"> (規則類別): 0調用 BOOKING、1入庫 UPLOAD、2轉檔 TRANSCODE </param>
        /// <param name="table"> 規則資料表 預設:空值(不指定) </param>
        /// <param name="column"> 資料表欄位 預設:空值(不指定) </param>
        /// <returns></returns>
        public List<spGET_RULE_BY_RULE_TABLE_COLUMN_Result> GetRule(string category, string table="", string column="")
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_RULE_BY_RULE_TABLE_COLUMN(category, table, column).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_RULE_BY_RULE_TABLE_COLUMN_Result>();

                return query;
            }
        }

        /// <summary>
        /// 目前未設定過的資料表與欄位 資料 (規則目標資料表欄位定義資料表)
        /// </summary>
        /// <param name="category"> (規則類別): 調用 BOOKING、入庫 UPLOAD、...(代碼=RULE) </param>
        /// <param name="table"> 規則資料表 預設:空值(不指定)</param>
        public List<tbmRULE_TABLE> UnspecifyRuleTableColumns(string category, string table = "")
        {
            _serilogService.WriterText("【RuleService_UnspecifyRuleTableColumns】", new { category, table });
            var query = GetRuleTableBy(category, table);
            
            ////流程規則已設定的資料表
            //var _filterTables = _ruleFilterRepository.FindBy(x => x.fsTABLE.IndexOf("*") < 0)
            //    .GroupBy(g => new { g.fsRULECATEGORY }).ToList()
            //    .Select(s => new
            //    {
            //        RuleCategory = s.Key.fsRULECATEGORY,    //EX: "BOOKING"
            //        tables = string.Join(";", s.Select(i => i.fsTABLE).Distinct())    //EX: "tbmARC_AUDIO;tbmARC_DOC;tbmARC_VIDEO;tbmBOOKING_T;tbmDIRECTORIES;tbmGROUPS"
            //    });
            //流程規則已設定的資料表、資料欄位
            var _filterTabCols = _ruleFilterRepository.GetAll()
                .GroupBy(g => new { g.fsRULECATEGORY, g.fsTABLE }).ToList()
                .Select(s => new RuleFilterKeyModel
                {
                    RuleCategory = s.Key.fsRULECATEGORY,    //EX: "BOOKING"
                    RuleTable = s.Key.fsTABLE,  //EX: "tbmARC_AUDIO"
                    RuleColumn = string.Join(";", s.Select(i => i.fsCOLUMN).Distinct())  //EX: "fnFILE_SECRET;fsFILE_TYPE"
                }).Where(x => x.RuleTable.IndexOf("*") < 0).ToList();

            var tableCols = (from tb in query
                             join f in _filterTabCols
                                    on new { b1 = tb.fsRULECATEGORY, b2 = tb.fsTABLE } equals new { b1 = f.RuleCategory, b2 = f.RuleTable }
                                    into objTmp
                             from rf in objTmp.DefaultIfEmpty()
                             where (rf == null ? true : rf.RuleColumn.IndexOf(tb.fsCOLUMN) < 0)
                             select tb
                           ).ToList();

            return tableCols;
        }

        /* 列表資料 改使用 Procedure
        /// <summary>
        /// 規則設定(規則類別+Table) 資料
        /// </summary>
        /// <param name="category"></param>
        public List<RuleTableModel> SearchRecord(string category)
        {
            var _tab = GetRuleTableBy(category).GroupBy(g => new { g.fsRULECATEGORY, g.fsTABLE })
                .Select(s => new
                {
                    RuleCategory = s.Key.fsRULECATEGORY,
                    RuleTable = s.Key.fsTABLE,
                    RuleTableName = s.Max(m => m.fsTABLE_NAME)
                }).ToList();
            
            var _main = (from a in _ruleRepository.FindBy(x => x.fsRULECATEGORY == category)
                         join f in _ruleFilterRepository.FindBy(x => x.fsRULECATEGORY == category) on a.fsRULECATEGORY equals f.fsRULECATEGORY
                         join b in _ruleTableRepository.GetAll().Select(s => new
                         {
                             RuleCategory = s.fsRULECATEGORY,
                             RuleTable = s.fsTABLE,
                             RuleTableName = s.fsTABLE_NAME
                         }).Distinct() on new { rr = a.fsRULECATEGORY, tt = f.fsTARGETTABLE } equals new { rr = b.RuleCategory, tt = b.RuleTable }
                         orderby new { a.fsRULECATEGORY, f.fnPRIORITY, f.fsTARGETTABLE }
                         select new RuleTableModel
                         {
                             RuleCategory = a.fsRULECATEGORY,
                             RuleName = a.fsRULENAME,
                             RuleTable = f.fsTARGETTABLE,
                             TableName = b.RuleTableName
                         }).Distinct().ToList();
            return _main;

        }
        /// <summary>
        /// 規則條件(Table+Column)資料內容
        /// </summary>
        public List<RuleListFilterModel> GetRuleFilterRec(string category, string table = null)
        {
            var _tabcolumn = GetRuleTableBy(category).GroupBy(g => new { g.fsRULECATEGORY, g.fsTABLE, g.fsCOLUMN })
                .Select(s => new
                {
                    RuleCategory = s.Key.fsRULECATEGORY,
                    RuleTable = s.Key.fsTABLE,
                    RuleTableName = s.Max(m => m.fsTABLE_NAME),
                    RuleColumn = s.Key.fsCOLUMN,
                    RuleColumnName = s.Max(m => m.fsCOLUMN_NAME)
                }).ToList();

            var _filter = (from f in _ruleFilterRepository.FindBy(x => x.fsRULECATEGORY == category && x.fsTARGETTABLE == table)
                           join b in _ruleTableRepository.GetAll().Select(s => new
                           {
                               RuleCategory = s.fsRULECATEGORY,
                               RuleTable = s.fsTABLE,
                               RuleTableName = s.fsTABLE_NAME,
                               RuleColumn = s.fsCOLUMN,
                               RuleColumnName = s.fsCOLUMN_NAME
                           }).Distinct() on new { rr = f.fsRULECATEGORY, tt = f.fsTARGETTABLE, ff = f.fsFILTERFIELD } equals new { rr = b.RuleCategory, tt = b.RuleTable, ff = b.RuleColumn }
                           where f.fbISENABLED == true
                           orderby new { f.fsRULECATEGORY, f.fnPRIORITY, f.fsTARGETTABLE }
                           select new RuleListFilterModel
                           {
                               RuleCategory = f.fsRULECATEGORY,
                               RuleTable = f.fsTARGETTABLE,
                               //b.RuleTableName,
                               RuleColumn = b.RuleColumn,
                               ColumnName = b.RuleColumnName,
                               Operator = f.fsOPERATOR,
                               //OperatorStr = GetEnums.GetDescriptionText<OperatorEnum>(f.fsOPERATOR),
                               FilterValue = f.fsFILTERVALUE,
                               Priority = f.fnPRIORITY,
                               //f.fbISENABLED,
                               IsEnabled = f.fbISENABLED == true ? "啟用" : "禁用"
                           }).OrderBy(o => o.RuleCategory).ThenBy(t => t.Priority).ThenBy(t => t.RuleTable).ToList();
            return _filter;
        }
        */

        #region >>> 下拉清單
        /// <summary>
        /// 流程類別選單(規則若都建立, 回傳 null)
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetRuleListForCreate()
        {
            var _had = GetRuleBy().Select(s => s.fsRULECATEGORY).ToArray();

            var query = from a in new CodeService().GetCodeItemList(TbzCodeIdEnum.RULE.ToString(), true, false, false)
                        where (!_had.Contains(a.Value))
                        select a;

            return query.Any() ? query.ToList() : null;//new List<SelectListItem>();
        }
        #endregion

        /// <summary>
        /// 規則邏輯組成 sql script
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected string AppendCondition(EditRuleFilterModel m)
        {
            //SELECT * FROM {0:Table} WHERE {1:Column} {2:Operator} {3:Value})
            string rtn = string.Empty, _op = string.Empty, _val = string.Empty;
            OperatorEnum _enum = (OperatorEnum)Enum.Parse(typeof(OperatorEnum), m.Operator);

            switch (_enum)
            {
                case OperatorEnum.Exclude:
                    _op = " NOT IN";
                    if (m.FieldType.ToUpper() == "INT" || m.FieldType.ToUpper() == "FLOAT" || m.FieldType.ToUpper() == "DECIMAL")
                    {
                        _val = string.Format("({0})", m.FilterValue.Replace(";", ", "));
                    }
                    else
                    {
                        _val = string.Format("('{0}')", m.FilterValue.Replace(";", "', '"));
                    }
                    break;
                case OperatorEnum.Equal:
                    _op = " = ";
                    if (m.FieldType.ToUpper() == "INT" || m.FieldType.ToUpper() == "FLOAT" || m.FieldType.ToUpper() == "DECIMAL")
                    {
                        _val = string.Format("{0}", m.FilterValue);
                    }
                    else
                    {
                        _val = string.Format("'{0}'", m.FilterValue);
                    }
                    break;
                case OperatorEnum.Between:
                    _op = "Between ";
                    var _ary = m.FilterValue.Split(new char[] { ';' });
                    if (m.FieldType.ToUpper() == "INT" || m.FieldType.ToUpper() == "FLOAT" || m.FieldType.ToUpper() == "DECIMAL")
                    {
                        _val = string.Format("{0} and {1} ", _ary[0], _ary[1]);
                    }
                    else
                    {
                        _val = string.Format("'{0}' and '{1}' ", _ary[0], _ary[1]);
                    }
                    break;
                case OperatorEnum.Include:
                default:
                    _op = "IN";
                    if (m.FieldType.ToUpper() == "INT" || m.FieldType.ToUpper() == "FLOAT" || m.FieldType.ToUpper() == "DECIMAL")
                    {
                        _val = string.Format("({0})", m.FilterValue.Replace(";", ", "));
                    }
                    else
                    {
                        _val = string.Format("('{0}')", m.FilterValue.Replace(";", "', '"));
                    }
                    break;
            }

            rtn = string.Format("SELECT * FROM {0} WHERE {1} {2} {3}", m.TargetTable, m.FilterField, _op, _val);
            return rtn;
        }

        #region >>> CURD: [tbmRULE]
        /// <summary>
        /// 新建規則(主表+篩選條件表) 【EF Create 】
        /// </summary>
        /// <param name="m">前端新增資料欄位 Model <see cref="CreateRuleModel"/> </param>
        /// <param name="ur">使用者帳號 </param>
        /// <returns></returns>
        public VerifyResult CreateNewRule(CreateRuleModel m, string ur)
        {
            if (m == null) return result;

            using (_db = new AIRMAM5DBEntities())
            {
                using (var _trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        #region (1) Insert tbmRULE -
                        tbmRULE _r = new tbmRULE
                        {
                            fsRULECATEGORY = m.RuleMaster.RuleCategory,
                            fsRULENAME = m.RuleMaster.RuleName,
                            fbISENABLED = m.RuleMaster.IsEnabled,
                            fsNOTE = m.RuleMaster.Note ?? string.Empty,
                            fdCREATED_DATE = DateTime.Now,
                            fsCREATED_BY = ur
                        };
                        _ruleRepository.Create(_r);
                        #endregion

                        #region (2) Insert tbmRULE_FILTER -
                        var _fs = m.Filters.Select(s => new tbmRULE_FILTER
                        {
                            fsRULECATEGORY = m.RuleMaster.RuleCategory,
                            fsTABLE = s.TargetTable,
                            fsCOLUMN = s.FilterField,
                            fsOPERATOR = s.Operator,
                            fsFILTERVALUE = s.FilterValue ?? string.Empty,
                            fnPRIORITY = s.Priority,
                            fbISENABLED = s.IsEnabled,
                            fsNOTE = s.Note ?? string.Empty,
                            fsWHERE_CLAUSE = s.WhereClause ?? "AND",
                            fsSCRIPTS = s.Operator == "*" ? string.Empty : this.AppendCondition(s), //"SELECT * FROM .... ",
                            fdCREATED_DATE = DateTime.Now,
                            fsCREATED_BY = ur
                        }).OrderBy(o => o.fsTABLE).ThenBy(t => t.fnPRIORITY).ThenBy(t => t.fsCOLUMN).ToList();
                        _ruleFilterRepository.CreateRange(_fs);
                        #endregion

                        _trans.Commit();
                        result.IsSuccess = true;
                        result.Message = string.Format($"規則({m.RuleMaster.RuleCategory}) 新建完成. ");

                        //回傳資料
                        var _get = GetRule(m.RuleMaster.RuleCategory)
                                .GroupBy(g => new { g.fsRULECATEGORY, g.fsRULENAME, g.fsTABLE, g.fsTABLE_NAME })
                                .Select(s => new RuleListModel().ConvertData(s))
                                .Distinct().ToList();

                        _get.ForEach(a => a.RuleFilters = this.GetRule(a.RuleCategory, a.RuleTable)
                                .Select(e => new RuleListFilterModel().DataConvert(e))
                                .OrderBy(o => o.RuleCategory).ThenBy(t => t.Priority).ToList());
                        //_get.ForEach(a =>
                        //{
                        //    var p = this.GetRule(a.RuleCategory, a.RuleTable)
                        //        .Select(e => new RuleListFilterModel().DataConvert(e))
                        //        .OrderBy(o => o.RuleCategory).ThenBy(t => t.Priority).ToList();
                        //    a.RuleFilters = (List<RuleListFilterModel>)(object)p;
                        //});

                        result.Data = _get;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = string.Format($"規則({m.RuleMaster.RuleCategory}) 新增失敗. {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            //var _innerEx = ex.InnerException.InnerException;
                            result.Message = ex.InnerException.InnerException.Message;
                        }
                        #region Serilog.Err
                        _serilogService.SerilogWriter(new SerilogInputModel
                        {
                            Controller = "RuleService",
                            Method = "[CreateNewRule]",
                            EventLevel = SerilogLevelEnum.Error,
                            Input = new { param = m, exception = ex },
                            LogString = "Exception",
                            ErrorMessage = string.Format($"新建規則({m.RuleMaster.RuleCategory}) 新增異常. {ex.Message}")
                        });
                        #endregion
                    }
                    finally
                    {
                        _trans.Dispose();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 編輯 流程規則 [tbmRULE]
        /// <para> 回傳資料格式：EditRuleModel </para>
        /// </summary>
        /// <param name="rec"> 流程規則主檔 <see cref="tbmRULE"/> </param>
        /// <returns></returns>
        public VerifyResult Update(tbmRULE rec)
        {
            if (rec == null) return result;

            try
            {
                _ruleRepository.Update(rec);

                result.IsSuccess = true;
                result.Message = string.Format($"編輯流程規則已存檔. ");
                result.Data = new EditRuleModel().DataConvert(rec);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"編輯流程規則異常. {ex.Message}");
                if (ex.InnerException != null)
                {
                    result.Message = ex.InnerException.InnerException.Message;
                }
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "RuleService",
                    Method = "[Update]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = rec, exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"編輯流程規則異常. {ex.Message}")
                });
                #endregion
            }

            return result;
        }
        #endregion

        #region >>> CURD: [tbmRULE_FILTER]
        /// <summary>
        /// 新增 流程規則條件(多筆)
        /// </summary>
        /// <param name="rec"> 流程規則條件清單 <see cref="tbmRULE_FILTER"/> </param>
        /// <returns></returns>
        public VerifyResult Create_RuleFilters(List<tbmRULE_FILTER> rec)
        {
            if (rec == null || rec.Count() < 1) return result;

            try
            {
                _ruleFilterRepository.CreateRange(rec);

                result.IsSuccess = true;
                result.Message = string.Format($"新增規則條件:已存檔. ");
                result.Data = rec;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"新增規則條件:異常. {ex.Message}");
                if (ex.InnerException != null)
                {
                    result.Message = ex.InnerException.InnerException.Message;
                }
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "RuleService",
                    Method = "[Create_RuleFilters]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = rec, exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"新增規則條件:異常. {ex.Message}")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 編輯 流程規則條件(單筆)
        /// </summary>
        /// <param name="rec"> 流程規則條件 <see cref="tbmRULE_FILTER"/> </param>
        /// <returns></returns>
        public VerifyResult UpdateSignleFilter(tbmRULE_FILTER rec)
        {
            if (rec == null) return result;

            try
            {
                _ruleFilterRepository.Update(rec);

                result.IsSuccess = true;
                result.Message = string.Format($"編輯規則條件:已存檔. ");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"編輯規則條件:異常. {ex.Message}");
                if (ex.InnerException != null)
                {
                    result.Message = ex.InnerException.InnerException.Message;
                }
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "RuleService",
                    Method = "[UpdateSignleFilter]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = rec, exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"編輯規則條件:異常. {ex.Message}")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 編輯 流程規則條件(多筆)
        /// </summary>
        /// <param name="rec"> 流程規則條件清單 <see cref="tbmRULE_FILTER"/> </param>
        /// <returns></returns>
        public VerifyResult Update_RuleFilters(List<tbmRULE_FILTER> rec)
        {
            if (rec == null || rec.Count() < 1) return result;

            try
            {
                _ruleFilterRepository.UpdateMultiple(rec);

                result.IsSuccess = true;
                result.Message = string.Format($"編輯規則條件:已存檔. ");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"編輯規則條件:異常. {ex.Message}");
                if (ex.InnerException != null)
                {
                    result.Message = ex.InnerException.InnerException.Message;
                }
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "RuleService",
                    Method = "[Update_RuleFilters]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = rec, exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"編輯規則條件:異常. {ex.Message}")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 刪除 流程規則條件 [tbmRULE_FILTER]
        /// <para> 回覆 Data格式: EditRuleFilterModel </para>
        /// </summary>
        /// <param name="rec"> 流程規則條件 <see cref="tbmRULE_FILTER"/> </param>
        /// <returns></returns>
        public VerifyResult Delete(tbmRULE_FILTER rec)
        {
            if (rec == null) return result;

            try
            {
                var _get = this.GetRule(rec.fsRULECATEGORY, rec.fsTABLE, rec.fsCOLUMN).FirstOrDefault();
                _ruleFilterRepository.Delete(rec);

                result.IsSuccess = true;
                result.Message = string.Format($"流程規則【{_get.fsRULENAME}】條件【{_get.fsTABLE_NAME}_{_get.fsCOLUMN_NAME}】已刪除. ");
                result.Data = new EditRuleFilterModel().DataConvert(rec);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"刪除流程規則條件:異常. {ex.Message}");
                if (ex.InnerException != null)
                {
                    result.Message = ex.InnerException.InnerException.Message;
                }
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "RuleService",
                    Method = "[Update]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { param = rec, exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"刪除流程規則條件:異常. {ex.Message}")
                });
                #endregion
            }

            return result;
        }
        #endregion
    }
}
