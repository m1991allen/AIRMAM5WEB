using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Procedure;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.SubjExtend;
using AIRMAM5.DBEntity.Procedure;
using AIRMAM5.Filters;
using AIRMAM5.Utility.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 媒資管理 > 主題與檔案維護.附加功能
    /// </summary>
    /// <remarks> 新聞文稿對應、公文系統合約簽呈 </remarks>
    [InterceptorOfController(Keyword = "AuthCookie")]
    public class SubjExtendController : BaseController
    {
        readonly ISubjectService _subjectService;
        readonly ProcedureGetService _getService;
        /// <summary>
        /// 功能controller
        /// </summary>
        readonly string cntr = "Subject";

        public SubjExtendController(ISerilogService serilogService, IFunctionsService functionService, ITblLogService tblLogService, ISubjectService subjectService)
            : base(serilogService, functionService)
        {
            _serilogService = serilogService;
            _subjectService = subjectService;
            _getService = new ProcedureGetService();
            _tblLogService = tblLogService;
        }

        /// <summary>
        /// 擴充功能{新聞文稿/公文對應} View
        /// </summary>
        /// <returns></returns>
        public JsonResult SubjExtendView(string type)
        {
            type = type.ToUpper();
            ResponseResultModel result = new ResponseResultModel(false, "", new { actButton = type });
            SubjExtendTypeEnum typeEnum = (SubjExtendTypeEnum)Enum.Parse(typeof(SubjExtendTypeEnum), type);
            string typeDesc = GetEnums.GetDescriptionText(typeEnum);

            if (CheckUserAuth(cntr))
            {
                #region _DB LOG
                _tblLogService.Insert_L_Log(
                    TbzCodeIdEnum.MSG001.ToString(),
                    "S003",        //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
                    string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, typeDesc),
                    string.Format($"位置: {Request.UserHostAddress} "),
                    JsonConvert.SerializeObject(""),
                    User.Identity.Name);
                #endregion
                List<SubjExtendColModel> md = _subjectService.SubjExtendQryCols(type);

                result.Data = md;
                result.IsSuccess = true;
                result.StatusCode = HttpStatusCode.OK;

                return Json(result, JsonRequestBehavior.DenyGet);
            }

            result.Message = "您無權限使用此網頁";
            result.StatusCode = HttpStatusCode.Forbidden;
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 擴充功能{新聞文稿/公文對應} 查詢
        /// </summary>
        /// <remarks>
        ///  1、文稿查詢：有變動的查詢條件，當某一查詢值為多個(如.日期區間)，則以字串陣列回傳值至"GenericValue"。
        ///  2、合約公文對應：
        ///  </remarks>
        /// <returns></returns>
        public JsonResult SubjExtendSearch(SubjExtendSearchModel<string[]> searches)
        {
            var param = searches;
            ResponseResultModel result = new ResponseResultModel(false, "", param);

            searches.ExecType = searches.ExecType.ToUpper();
            SubjExtendTypeEnum typeEnum = (SubjExtendTypeEnum)Enum.Parse(typeof(SubjExtendTypeEnum), searches.ExecType);
            string typeEnumStr = string.Format("{0}_查詢", GetEnums.GetDescriptionText(typeEnum));

            if (CheckUserAuth(cntr))
            {
                try
                {
                    string cols = string.Empty, vals = string.Empty;
                    bool chk = true;

                    searches.ExecParams.ForEach(f =>
                    {
                        string v = string.Empty;
                        var datatype = (CodeTEMP002Enum)Enum.Parse(typeof(CodeTEMP002Enum), f.FieldType.ToUpper());

                        switch (datatype)
                        {
                            case CodeTEMP002Enum.DATETIME:
                                string[] ary = f.GenericValue; //TIP:這裡的日期值,前端一定是傳遞"日期起迄字串陣列"
                                if (ary.Length == 2)
                                {
                                    bool d1 = DateTimeExtensions.CheckDateIsMatch(ary[0]), d2 = DateTimeExtensions.CheckDateIsMatch(ary[1]);
                                    if (!d1 || !d2)
                                    {
                                        result.Message = "日期起迄格式不符!! ";
                                        result.StatusCode = HttpStatusCode.ExpectationFailed;
                                        chk = false; return;
                                    }

                                    v = string.Format($"{ary[0]}~{ary[1]}");
                                }
                                else
                                {
                                    result.Message = "日期區間需要起、迄值!!";
                                    result.StatusCode = HttpStatusCode.ExpectationFailed;
                                    chk = false; return;
                                }

                                break;
                            case CodeTEMP002Enum.INTEGER:
                                bool r = int.TryParse(f.Value.ToString(), out int num);
                                if (!r)
                                {
                                    result.Message = "數值欄位內容不正確";
                                    result.StatusCode = HttpStatusCode.ExpectationFailed;
                                    chk = false; return;
                                }

                                v = f.Value.ToString();
                                break;
                            default:
                                v = f.Value.ToString();
                                break;
                        }

                        cols = string.Concat(cols, f.Field, ";");
                        vals = string.Concat(vals, v, ";");
                    });

                    if (!chk) { return Json(result, JsonRequestBehavior.AllowGet); }

                    cols = cols.Substring(0, cols.Length - 1);
                    vals = vals.Substring(0, vals.Length - 1);
                    // searches.ExecType 呼叫不同的預存程序
                    // TIP: 回傳格式統一 SearchResultModel<T>
                    switch (typeEnum)
                    {
                        case SubjExtendTypeEnum.INEWS:
                            SearchResultModel<spGET_INEWS_Result> _inewsResult = new SearchResultModel<spGET_INEWS_Result>();
                            Get_INews_Param get_INews = new Get_INews_Param { Columns = cols, Values = vals };
                            var get = _getService.SubjExtendGetINews(get_INews).ToList();

                            var _datas = get.Where(x => x.fsKEY != ""); //資料內容
                            _inewsResult.PKeyCol = get.Any() && _datas.FirstOrDefault() != null ? _datas.FirstOrDefault().fsKEY : string.Empty;
                            _inewsResult.DataList = _datas.ToList();
                            _inewsResult.DataTitle = get.Where(x => x.fsKEY == "").FirstOrDefault(); //TIP: 第一筆是標題資訊, fsKEY==""。

                            result.Data = _inewsResult;
                            break;
                        case SubjExtendTypeEnum.CONTRACT:
                            // TODO: 合約公文對應 資料來源 預存

                            break;
                        default:
                            //
                            break;
                    }

                    #region _DB LOG
                    _tblLogService.Insert_L_Log(
                        TbzCodeIdEnum.MSG001.ToString(),
                        "M005",        //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
                        string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, typeEnumStr, "OK"),
                        string.Format($"位置: {Request.UserHostAddress} "),
                        JsonConvert.SerializeObject(param),
                        User.Identity.Name);
                    #endregion
                    #region _Serilog.Verbose
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "SubjExtend",
                        Method = "[SubjExtendSearch]",
                        EventLevel = SerilogLevelEnum.Verbose,
                        Input = new { Param = param, Result = result },
                        LogString = string.Format($"{typeEnumStr}.Result")
                    });
                    #endregion

                    result.IsSuccess = true;
                    result.Message = typeEnumStr + "完成。";
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = ex.Message;
                    result.StatusCode = HttpStatusCode.InternalServerError;
                    #region _Serilog.Error
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "SubjExtend",
                        Method = "[SubjExtendSearch]",
                        EventLevel = SerilogLevelEnum.Error,
                        Input = new { Param = param, Result = result, Exception = ex },
                        LogString = string.Format($"{typeEnumStr}.Exception"),
                        ErrorMessage = ex.Message
                    });
                    #endregion
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }

            result.Message = string.Format($"您無權限使用 {typeEnumStr}");
            result.StatusCode = HttpStatusCode.Forbidden;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 擴充功能{新聞文稿/公文對應} 選取
        /// </summary>
        /// <remarks>
        /// 新聞文稿：✔存檔處理->更新文稿自訂欄位
        /// 公文對應：
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubjExtendUpdate(SubjExtendUpdateModel<string[]> setModel)
        {
            var param = setModel;
            ResponseResultModel result = new ResponseResultModel(false, "", param);

            setModel.ExecType = setModel.ExecType.ToUpper();
            SubjExtendTypeEnum typeEnum = (SubjExtendTypeEnum)Enum.Parse(typeof(SubjExtendTypeEnum), setModel.ExecType);
            string typeEnumStr = string.Format("{0}_選取", GetEnums.GetDescriptionText(typeEnum));

            if (CheckUserAuth(cntr))
            {
                if (string.IsNullOrEmpty(setModel.FileNo))
                {
                    result.IsSuccess = false;
                    result.Message = "檔案編號不正確(Empty)";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                try
                {
                    if (setModel.Fields.Length != setModel.Values.Length)
                    {
                        result.IsSuccess = false;
                        result.Message = "欄位名稱和值的數量不相等~!";
                        result.StatusCode = HttpStatusCode.BadRequest;
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }

                    //CALL Procedure
                    VerifyResult res = new VerifyResult();
                    string resSTR = string.Empty;
                    switch (typeEnum)
                    {
                        case SubjExtendTypeEnum.INEWS:
                            Update_INews_Param<string> upd = new Update_INews_Param<string> { UpdateBy = User.Identity.Name }.CustomConvert(setModel);
                            res = _subjectService.UpdateINews(upd);
                            resSTR = res.IsSuccess ? "成功" : "失敗";
                            break;
                        case SubjExtendTypeEnum.CONTRACT:
                            // TODO: 合約公文對應 選取更新存檔's 預存

                            break;
                        default:
                            //
                            break;
                    }

                    #region _DB LOG 
                    _tblLogService.Insert_L_Log(
                        TbzCodeIdEnum.MSG001.ToString(),
                        "M002",        //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
                        string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, typeEnumStr, resSTR),
                        string.Format($"位置: {Request.UserHostAddress} "),
                        JsonConvert.SerializeObject(param),
                        User.Identity.Name);
                    #endregion
                    //TIP: 更新成功-前端要重load頁面。
                    result = new ResponseResultModel(res)
                    {
                        Records = param,
                        Message = typeEnumStr + "更新" + resSTR,
                        StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
                    };
                    #region _Serilog.Info
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "SubjExtend",
                        Method = "[SubjExtendUpdate]",
                        EventLevel = SerilogLevelEnum.Information,
                        Input = new { Param = param, Result = result },
                        LogString = string.Format($"{typeEnumStr}.Result")
                    });
                    #endregion
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = ex.Message;
                    result.StatusCode = HttpStatusCode.InternalServerError;
                    #region _Serilog.Error
                    _serilogService.SerilogWriter(new SerilogInputModel
                    {
                        Controller = "SubjExtend",
                        Method = "[SubjExtendUpdate]",
                        EventLevel = SerilogLevelEnum.Error,
                        Input = new { Param = param, Result = result, Exception = ex },
                        LogString = string.Format($"{typeEnumStr}.Exception"),
                        ErrorMessage = ex.Message
                    });
                    #endregion
                }
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            result.Message = string.Format($"您無權限使用 {typeEnumStr}");
            result.StatusCode = HttpStatusCode.Forbidden;
            return Json(result, JsonRequestBehavior.DenyGet);
        }


        #region >>>新聞文稿對應✘ 改共用處理
        ///// <summary>
        ///// 新聞文稿 View
        ///// </summary>
        ///// <returns></returns>
        //public JsonResult NewsDraft()
        //{
        //    ResponseResultModel result = new ResponseResultModel(false, "", new { actButton = "INEWS" });

        //    if (CheckUserAuth(cntr))
        //    {
        //        #region _DB LOG
        //        _tblLogService.Insert_L_Log(
        //            TbzCodeIdEnum.MSG001.ToString(),
        //            "S003",        //[@USER_ID(@USER_NAME)] 進入 [@WORK_PAGE]
        //            string.Format(FormatString.UsePageParams, CurrentUser.UserName, CurrentUser.fsNAME, "文稿對應"),
        //            string.Format($"位置: {Request.UserHostAddress} "),
        //            JsonConvert.SerializeObject(""),
        //            User.Identity.Name);
        //        #endregion
        //        List<SubjExtendColModel> md = _subjectService.SubjExtendQryCols("INEWS");

        //        result.Data = md;
        //        result.IsSuccess = true;
        //        result.StatusCode = HttpStatusCode.OK;

        //        return Json(result, JsonRequestBehavior.DenyGet);
        //    }

        //    result.Message = "您無權限使用此網頁";
        //    result.StatusCode = HttpStatusCode.Forbidden;
        //    return Json(result, JsonRequestBehavior.DenyGet);
        //}
        ///// <summary>
        ///// 新聞文稿查詢
        ///// </summary>
        ///// <remarks>
        /////  1、文稿查詢：有變動的查詢條件，當某一查詢值為多個(如.日期區間)，則以字串陣列回傳值至"GenericValue"。 </remarks>
        ///// <returns></returns>
        //public JsonResult NewsDraftSearch(List<SearchParam<string[]>> searches)
        //{
        //    var param = new { actButton = "INEWS_Qry", searches };
        //    ResponseResultModel result = new ResponseResultModel(false, "", param);

        //    //if (CheckUserAuth(cntr))
        //    //{
        //        try
        //        {
        //            string cols = string.Empty, vals = string.Empty;
        //            bool chk = true;

        //            searches.ForEach(f =>
        //            {
        //                string v = string.Empty;
        //                var datatype = (CodeTEMP002Enum)Enum.Parse(typeof(CodeTEMP002Enum), f.FieldType.ToUpper());

        //                switch (datatype)
        //                {
        //                    case CodeTEMP002Enum.DATETIME:
        //                        string[] ary = f.GenericValue; //TIP:這裡的日期值,前端一定是傳遞"日期起迄字串陣列"
        //                        if (ary.Length == 2)
        //                        {
        //                            bool d1 = DateTimeExtensions.CheckDateIsMatch(ary[0]), d2 = DateTimeExtensions.CheckDateIsMatch(ary[1]);
        //                            if (!d1 || !d2)
        //                            {
        //                                result.Message = "日期起迄格式不符!! ";
        //                                result.StatusCode = HttpStatusCode.ExpectationFailed;
        //                                chk = false; return;
        //                            }

        //                            v = string.Format($"{ary[0]}~{ary[1]}");
        //                        }
        //                        else
        //                        {
        //                            result.Message = "日期區間需要起、迄值!!";
        //                            result.StatusCode = HttpStatusCode.ExpectationFailed;
        //                            chk = false; return;
        //                        }

        //                        break;
        //                    case CodeTEMP002Enum.INTEGER:
        //                        bool r = int.TryParse(f.Value.ToString(), out int num);
        //                        if (!r)
        //                        {
        //                            result.Message = "數值欄位內容不正確";
        //                            result.StatusCode = HttpStatusCode.ExpectationFailed;
        //                            chk = false; return;
        //                        }

        //                        v = f.Value.ToString();
        //                        break;
        //                    default:
        //                        v = f.Value.ToString();
        //                        break;
        //                }

        //                cols = string.Concat(cols, f.Field, ";");
        //                vals = string.Concat(vals, v, ";");
        //            });

        //            if (!chk) { return Json(result, JsonRequestBehavior.AllowGet); }

        //            cols = cols.Substring(0, cols.Length - 1);
        //            vals = vals.Substring(0, vals.Length - 1);
        //            Get_INews_Param get_INews = new Get_INews_Param { Columns = cols, Values = vals };
        //            var get = _getService.SubjExtendGetINews(get_INews).ToList();
                
        //            #region _DB LOG
        //            _tblLogService.Insert_L_Log(
        //                TbzCodeIdEnum.MSG001.ToString(),
        //                "M005",        //[@USER_ID(@USER_NAME)] 查詢 [@DATA_TYPE] 資料 @RESULT
        //                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "文稿對應", "OK"),
        //                string.Format($"位置: {Request.UserHostAddress} "),
        //                JsonConvert.SerializeObject(param),
        //                User.Identity.Name);
        //            #endregion
        //            #region _Serilog.Verbose
        //            _serilogService.SerilogWriter(new SerilogInputModel
        //            {
        //                Controller = "SubjExtend",
        //                Method = "[NewsDraftSearch]",
        //                EventLevel = SerilogLevelEnum.Verbose,
        //                Input = new { Param = param, Result = result },
        //                LogString = string.Format($"文稿查詢.Result")
        //            });
        //            #endregion

        //            result.Data = get.Count() > 0 && get.FirstOrDefault() != null ? get : new List<spGET_INEWS_Result>();
        //            result.IsSuccess = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            result.IsSuccess = false;
        //            result.Message = ex.Message;
        //            result.StatusCode = HttpStatusCode.InternalServerError;
        //            #region _Serilog.Error
        //            _serilogService.SerilogWriter(new SerilogInputModel
        //            {
        //                Controller = "SubjExtend",
        //                Method = "[NewsDraftSearch]",
        //                EventLevel = SerilogLevelEnum.Error,
        //                Input = new { Param = param, Result = result, Exception = ex },
        //                LogString = string.Format($"文稿查詢.Exception"),
        //                ErrorMessage = ex.Message
        //            });
        //            #endregion
        //        }

        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    //}

        //    result.Message = "您無權限使用文稿查詢";
        //    result.StatusCode = HttpStatusCode.Forbidden;
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        ///// <summary>
        ///// 新聞文稿選取(✔存檔處理->更新文稿自訂欄位)
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public JsonResult NewsDraftSetSave(NewsDarftSetModel setModel)
        //{
        //    var param = new { actButton = "INEWS_SET", setModel };
        //    ResponseResultModel result = new ResponseResultModel(false, "", param);

        //    //if (CheckUserAuth(cntr))
        //    //{
        //        if (string.IsNullOrEmpty(setModel.FileNo))
        //        {
        //            result.IsSuccess = false;
        //            result.Message = "檔案編號不正確(Empty)";
        //            result.StatusCode = HttpStatusCode.BadRequest;
        //            return Json(result, JsonRequestBehavior.DenyGet);
        //        }

        //        try
        //        {
        //            //CALL Procedure
        //            Update_INews_Param upd = new Update_INews_Param { UpdateBy = "developer" }.CustomConvert(setModel);
        //            VerifyResult res = _subjectService.UpdateINews(upd);
        //            string ss = res.IsSuccess ? "成功" : "失敗";

        //            #region _DB LOG
        //            _tblLogService.Insert_L_Log(
        //                TbzCodeIdEnum.MSG001.ToString(),
        //                "M002",        //[@USER_ID(@USER_NAME)] 修改 [@DATA_TYPE] 資料 @RESULT
        //                string.Format(FormatString.LogParams, CurrentUser.UserName, CurrentUser.fsNAME, "文稿選取", ss),
        //                string.Format($"位置: {Request.UserHostAddress} "),
        //                JsonConvert.SerializeObject(param),
        //                User.Identity.Name);
        //            #endregion
        //            //TIP: 更新成功-前端要重load頁面。
        //            result = new ResponseResultModel(res)
        //            {
        //                Records = param,
        //                StatusCode = res.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed
        //            };
        //            #region _Serilog.Info
        //            _serilogService.SerilogWriter(new SerilogInputModel
        //            {
        //                Controller = "SubjExtend",
        //                Method = "[NewsDraftSetSave]",
        //                EventLevel = SerilogLevelEnum.Information,
        //                Input = new { Param = param, Result = result },
        //                LogString = string.Format($"文稿選取存檔.Result")
        //            });
        //            #endregion
        //        }
        //        catch (Exception ex)
        //        {
        //            result.IsSuccess = false;
        //            result.Message = ex.Message;
        //            result.StatusCode = HttpStatusCode.InternalServerError;
        //            #region _Serilog.Error
        //            _serilogService.SerilogWriter(new SerilogInputModel
        //            {
        //                Controller = "SubjExtend",
        //                Method = "[NewsDraftSetSave]",
        //                EventLevel = SerilogLevelEnum.Error,
        //                Input = new { Param = param, Result = result, Exception = ex },
        //                LogString = string.Format($"文稿選取存檔.Exception"),
        //                ErrorMessage = ex.Message
        //            });
        //            #endregion
        //        }
        //        return Json(result, JsonRequestBehavior.DenyGet);
        //    //}

        //    result.Message = "您無權限使用文稿查詢";
        //    result.StatusCode = HttpStatusCode.Forbidden;
        //    return Json(result, JsonRequestBehavior.DenyGet);
        //}
        #endregion

    }
}
