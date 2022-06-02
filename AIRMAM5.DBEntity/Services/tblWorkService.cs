using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Booking;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Works;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 工作表資料 tblWORK
    /// </summary>
    public class TblWorkService
    {
        protected AIRMAM5DBEntities _db;
        readonly ISerilogService _serilogService;
        readonly IGenericRepository<tblWORK> _tblWorkRepository = new GenericRepository<tblWORK>();

        public TblWorkService()
        {
            //_db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }

        #region _9/13另新增sp,不使用 spGET_L_WORK
        ///// <summary>
        ///// 依參數 取出L_WORK主檔資料
        ///// </summary>
        ///// <param name="param">spGET_L_WORK 查詢參數Model </param>
        ///// <returns></returns>
        //public List<spGET_L_WORK_Result> GetByParam(SP_GetLWork param)
        //{
        //    List<spGET_L_WORK_Result> result = new List<spGET_L_WORK_Result>();
        //    try
        //    {
        //        result = _db.spGET_L_WORK(
        //            param.WorkId,
        //            param.GroupId,
        //            param.WorkType,
        //            param.WorkStatus,
        //            param.BegDate,
        //            param.EndDate).DefaultIfEmpty().ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        _serilogService.SerilogWriter(new SerilogInputModel
        //        {
        //            Controller = "tblWorkService",
        //            Method = "GetByParam",
        //            EventLevel = SerilogLevelEnum.Error,
        //            Input = ex,
        //            LogString = "Exception",
        //            ErrorMessage = string.Format($"上傳轉檔紀錄. {ex.Message}")
        //        });
        //    }
        //    return result;
        //}
        #endregion

        /// <summary>
        /// 依工作ID [fnWORK_ID] 取出 轉檔工作編號資料
        /// </summary>
        /// <param name="workid">工作檔案編號 [fnWORK_ID] </param>
        /// <returns></returns>
        public tblWORK GetById(long workid)
        {
            return _tblWorkRepository.FindBy(x => x.fnWORK_ID == workid).FirstOrDefault();
        }

        /// <summary>
        /// 依[fsTYPE]、檔案編號 取出工作檔資料
        /// </summary>
        /// <param name="worktype">工作類型 [fsTYPE] 如BOOKING、TRANSCODING、COPYFILE... </param>
        /// <param name="fileno">檔案編號 [_ITEM_ID] </param>
        /// <returns></returns>
        public tblWORK GetByTypeFileno(string worktype, string fileno)
        {
            return _tblWorkRepository
                .FindBy(x => x.fsTYPE == worktype && x.C_ITEM_ID == fileno)
                .OrderByDescending(o => o.fnWORK_ID).FirstOrDefault();
        }

        /// <summary>
        /// 取 多筆轉檔工作編號資料 【spGET_L_WORK_MERGE】
        /// </summary>
        /// <param name="workid"></param>
        /// <returns></returns>
        //public List<tblWORK> GetMultipleById(List<long> workid)
        public List<LWorkProgressModel> GetMultipleById(List<long> workid)
        {
            string _ids = string.Join(";", workid);
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_L_WORK_MERGE(_ids).DefaultIfEmpty()
                    .Select(s => new LWorkProgressModel().FormatConversion(s))
                    .ToList();

                return query;
            }
        }

        #region ----------【審核調用】----------
        /// <summary>
        /// 取 轉檔工作資料表中 需要審核調用資料   【spGET_L_WORK_BY_BOOKING_APPROVE】
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<spGET_L_WORK_BY_BOOKING_APPROVE_Result> GetLWorkBookingApprove(VerifyDateSerarchModel param)
        {
            string _ids = (param.WorkIds == null || param.WorkIds.Length == 0) ? string.Empty : string.Join(";", param.WorkIds);

            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_L_WORK_BY_BOOKING_APPROVE(
                string.Format($"{param.StartDate:yyyy-MM-dd}"), //param.BeginDate
                string.Format($"{param.EndDate:yyyy-MM-dd}"), //param.EndDate,
                param.ApproveStatus ?? string.Empty,
                param.UserId ?? string.Empty, _ids).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_L_WORK_BY_BOOKING_APPROVE_Result>();

                return query;
            }
        }
        
        /// <summary>
        /// 調用檔案 審核確認處理(過審、不過審)  【spUPDATE_L_WORK_BY_BOOKING_APPROVE】
        /// </summary>
        /// <param name="workIds">轉檔工作編號 fnWORK_ID (多筆;)</param>
        /// <param name="isPass">審核結果 true、false </param>
        /// <param name="remark">審核備註 </param>
        /// <returns></returns>
        public VerifyResult BookingConfirmApprove(int[] workIds, bool isPass, string remark, string approveUser)
        {
            VerifyResult result = new VerifyResult(false, "normal");
            var param = new { workIds, isPass, remark, approveUser };

            try
            {
                string _ids = string.Join(";", workIds);
                string _username = HttpContext.Current == null ? approveUser : HttpContext.Current.User.Identity.Name;
                string _status = isPass ? WorkApproveEnum._C.ToString() : WorkApproveEnum._R.ToString();

                using (_db = new AIRMAM5DBEntities())
                {
                    var _exec = _db.spUPDATE_L_WORK_BY_BOOKING_APPROVE(_ids, _status, _username, remark ?? string.Empty).FirstOrDefault();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = "調用檔案 審核完成";
                        result.Data = this.GetLWorkBookingApprove(new VerifyDateSerarchModel
                        {
                            StartDate = string.Empty,
                            EndDate = string.Empty,
                            ApproveStatus = string.Empty,
                            ApproveStatusList = new List<System.Web.Mvc.SelectListItem>(),
                            LoginIdList = new List<System.Web.Mvc.SelectListItem>(),
                            UserId = string.Empty,
                            WorkIds = workIds
                        }).Select(s => new VerifyBookingListModel().DataConvert(s))
                        .OrderByDescending(r => r.BookingDate).ToList();
                    }
                    else
                    {
                        result.Message = _exec.Split(':')[1];
                    }
                }
            }
            catch (Exception ex )
            {
                result.IsSuccess = false;
                result.Message = ex.InnerException.Message ?? ex.Message;
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "tblWorkService",
                    Method = "[BookingConfirmApprove]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = param, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"(SP)調用審核確認處理【{ex.Message}】")
                });
                #endregion
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 取出上傳轉檔主檔資料  【spGET_L_WORK_BY_TRANSCODE】
        /// </summary>
        public List<spGET_L_WORK_BY_TRANSCODE_Result> GetLWorkByTranscode(GetLWorkByTranscodeParam param)
        {
            List<spGET_L_WORK_BY_TRANSCODE_Result> result = new List<spGET_L_WORK_BY_TRANSCODE_Result>();

            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    result = _db.spGET_L_WORK_BY_TRANSCODE(
                    param.WorkId,
                    param.StartDate,
                    param.EndDate,
                    param.WorkStatus).DefaultIfEmpty().ToList();

                    if (result == null || result.FirstOrDefault() == null) result = new List<spGET_L_WORK_BY_TRANSCODE_Result>();
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "tblWorkService",
                    Method = "[GetLWorkByTranscode]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = param, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"(SP)上傳轉檔紀錄【{ex.Message}】")
                });
                #endregion
            }

            return result;
        }
        
        /// <summary>
        /// 取出調用轉檔資料   【spGET_L_WORK_BY_BOOKING】
        /// </summary>
        /// <returns></returns>
        public List<spGET_L_WORK_BY_BOOKING_Result> GetLWorkByBooking(GetLWorkByBookingParam param)
        {
            List<spGET_L_WORK_BY_BOOKING_Result> result = new List<spGET_L_WORK_BY_BOOKING_Result>();

            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    result = _db.spGET_L_WORK_BY_BOOKING(
                    param.WorkId,
                    param.LoginId,
                    param.StartDate,
                    param.EndDate,
                    param.WorkStatus).DefaultIfEmpty().ToList();

                    if (result == null || result.FirstOrDefault() == null)
                        result = new List<spGET_L_WORK_BY_BOOKING_Result>();
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "tblWorkService",
                    Method = "[GetLWorkByBooking]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = param, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"(SP)調用轉檔資料【{ex.Message}】")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 重新轉檔(可多筆) 【spUPDATE_L_WORK_RETRAN】
        /// </summary>
        /// <returns></returns>
        public UpdateLWorkReTranResult UpdateLWorkReTran(List<string> workids, string username)
        {
            UpdateLWorkReTranResult result = new UpdateLWorkReTranResult(true, string.Empty);
            
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    foreach (string i in workids)
                    {
                        var _exec = _db.spUPDATE_L_WORK_RETRAN(long.Parse(i), username).FirstOrDefault();

                        if (_exec.IndexOf("ERROR") == -1)
                        {
                            if (_exec.ToUpper() == "N")
                            {
                                //記錄不能轉檔的workid
                                result.UnProcessed.Add(i);
                            }
                            else
                            {
                                //OK 轉檔處理中
                                result.Processed.Add(i);
                            }
                        }
                        else
                        {
                            result.Failure.Add(i);
                            //error msg = _exec.Split(':')[1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "tblWorkService",
                    Method = "[UpdateLWorkReTran]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = new { workids , username }, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"(SP)重新轉檔. {ex.Message}")
                });
                #endregion
                result = new UpdateLWorkReTranResult(false, string.Format($"重新轉檔失敗【{ex.Message}】"));
            }

            return result;
        }

        /// <summary>
        /// 重新轉檔(可多筆) 【spUPDATE_REBOOKING】
        /// </summary>
        /// <param name="workids"></param>
        /// <remarks> (@fsSTATUS＜'00' OR @fsSTATUS＞＝'90') 才可重新調用 </remarks>
        /// <returns></returns>
        public UpdateLWorkReTranResult UpdateLWorkReBooking(List<string> workids, string username)
        {
            UpdateLWorkReTranResult result = new UpdateLWorkReTranResult(true, string.Empty);

            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    foreach (string i in workids)
                    {
                        var _exec = _db.spUPDATE_REBOOKING(long.Parse(i), username).FirstOrDefault();
                        if (_exec.IndexOf("ERROR") == -1)
                        {
                            if (_exec.ToUpper() == "N")
                            {
                                //不能重設調用 的workid
                                result.UnProcessed.Add(i);
                            }
                            else
                            {
                                //OK 
                                result.Processed.Add(i);
                            }
                        }
                        else
                        {
                            //procedure exception
                            result.Failure.Add(i);
                            //error msg = _exec.Split(':')[1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "tblWorkService",
                    Method = "[UpdateLWorkReBooking]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = workids, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"(SP)重新借調. {ex.Message}")
                });
                #endregion
                result = new UpdateLWorkReTranResult(false, string.Format($"重新借調失敗【{ex.Message}】"));
            }

            return result;
        }

        /// <summary>
        /// 檔案編號 是否正在轉碼中(=TRANSCODE)  【dbo.spGET_L_WORK_IS_TRANSCODE_BY_TYPE_FILENO】
        /// </summary>
        /// <param name="type">檔案類別 </param>
        /// <param name="fileno">檔案編號 </param>
        /// <param name="act">執行動作 ex: 'DELETE'</param>
        /// <returns></returns>
        public bool IsWorkOnTranscode(string type, string fileno, string act)
        {
            bool result = true;
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    var _exec = _db.spGET_L_WORK_IS_TRANSCODE_BY_TYPE_FILENO(type, fileno, act).FirstOrDefault();
                    if (_exec == IsTrueFalseEnum.N.ToString()) { result = false; } else { result = true; }
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "tblWorkService",
                    Method = "[IsWorkOnTranscode]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = new { type, fileno, act }, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"(SP)是否正在轉碼中. {ex.Message}")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 調用取消(可多筆) 【spUPDATE_BOOKING_CANCEL】
        /// </summary>
        /// <param name="workids"></param>
        /// <param name="username"></param>
        /// <remarks> fsSTATUS IN ('_C','00') 才可取消。 </remarks>
        /// <returns></returns>
        public UpdateLWorkReTranResult CancelBooking(List<string> workids, string username)
        {
            UpdateLWorkReTranResult result = new UpdateLWorkReTranResult(true, string.Empty);

            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    foreach (string i in workids)
                    {
                        var _exec = _db.spUPDATE_BOOKING_CANCEL(long.Parse(i), username).FirstOrDefault();

                        if (_exec.IndexOf("ERROR") == -1)
                        {
                            if (_exec.ToUpper() == "Y")
                            {
                                //OK 
                                result.Processed.Add(i);
                            }
                            else
                            {
                                result.UnProcessed.Add(i);
                                //狀態不符, 
                            }
                        }
                        else
                        {
                            //procedure exception 的workid
                            result.Failure.Add(i);
                            //error msg = _exec.Split(':')[1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "tblWorkService",
                    Method = "[CancelBooking]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Param = workids, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"(SP)取消調用. {ex.Message}")
                });
                #endregion
                result = new UpdateLWorkReTranResult(false, string.Format($"取消調用失敗【{ex.Message}】"));
            }

            return result;
        }

        #region ---------- CURD 【tblWORK】: 新 修 刪
        /// <summary>
        /// 新建 tblWORK: 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult CreateBy(tblWORK rec)
        {
            VerifyResult result = new VerifyResult();

            try
            {
                // ✘✘-- EXECUTE dbo.spINSERT_L_WORK --

                rec.fdSTART_WORK_TIME = DateTime.Now;
                rec.fdCREATED_DATE = DateTime.Now;
                _tblWorkRepository.Create(rec);
                
                result.IsSuccess = true;
                result.Message = "轉檔工作已新增(已新增)";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"轉檔工作新增失敗【{ex.Message}】");
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TblWorkService",
                    Method = "[CreateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"WORK新增失敗【{ex.Message}】 ")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 變更工作/轉檔 優先權 【spUPDATE_L_WORK_PRIORITY】
        /// </summary>
        /// <returns></returns>
        public VerifyResult UpdateLWorkPriority(long workid, int priority)
        {
            VerifyResult result = new VerifyResult(true, "");
            string _username = HttpContext.Current == null ? "admin" : HttpContext.Current.User.Identity.Name;

            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    var _exec = _db.spUPDATE_L_WORK_PRIORITY(workid, priority.ToString(), _username).FirstOrDefault();
                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.Message = "轉檔工作優先權,修改成功!";
                    }
                    else
                    {
                        result.Message = _exec.Split(':')[1];//"工作優先權 修改失敗!";
                    }
                }
            }
            catch (Exception ex)
            {
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "tblWorkService",
                    Method = "[UpdateLWorkPriority]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"變更轉檔優先權【{ex.Message}】")
                });
                #endregion
                //result = new VerifyResult(false, ex.Message);
                result.IsSuccess = false;
                result.Message = string.Format($"轉檔工作優先權異動失敗【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// Update 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateBy(tblWORK rec)
        {
            VerifyResult result = new VerifyResult();

            try
            {
                if (rec == null) return result;
                _tblWorkRepository.Update(rec);
                //result = new VerifyResult(true, "WORK資料成功更新") { Data = rec };
                result.IsSuccess = true;
                result.Message = string.Format($"轉檔工作資料成功更新!");
            }
            catch (Exception ex)
            {
                //result = new VerifyResult(false, string.Format($"WORK資料更新失敗【{ex.Message}】"));
                result.IsSuccess = false;
                result.Message = string.Format($"轉檔工作資料更新失敗【{ex.Message}】");
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TblWorkService",
                    Method = "[UpdateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"WORK資料更新失敗【{ex.Message}】 ")
                });
                #endregion
            }

            return result;
        }
        #endregion

    }
}
