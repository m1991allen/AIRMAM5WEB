using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.BatchBooking;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Material;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Linq;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 調用紀錄檔 db.[tbmBOOKING]
    /// </summary>
    public class BookingService
    {
        readonly IGenericRepository<tbmBOOKING> _bookingRepository = new GenericRepository<tbmBOOKING>();
        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "fail");
        
        public BookingService()
        {
            //this._db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }
        public BookingService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
            
        }

        #region ---------- CURD 【tbmBOOKING】: 新 修 刪
        /// <summary>
        /// 新建 調用紀錄檔 tbmBOOKING: 【spINSERT_BOOKING】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult CreateBy(BookingCreateModel rec, string createdby)
        {
            try
            {
                //#region ✘✘-- EXECUTE dbo.spINSERT_BOOKING --

                using (_db = new AIRMAM5DBEntities())
                {
                    //_Marked_&_Modified_20200326: 調用檔案要先檢查是否有流程規則，需要審核。
                    #region -- EXECUTE dbo.spINSERT_BOOKING_APPROVE -- 
                    var _exec = _db.spINSERT_BOOKING_APPROVE(
                    rec.ResonId,
                    rec.ResonStr,
                    rec.DescStr,
                    rec.ProfileVideo,
                    rec.ProfileAudio,
                    rec.WaterMark,
                    rec.PathStr,
                    DateTime.Now.ToString(),
                    createdby,
                    rec.MaterialIds).FirstOrDefault().ToString();
                    #endregion

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        //新增成功,回覆@fnBOOKING_ID
                        long.TryParse(_exec, out long _bookid);
                        //var get = _bookingRepository.Get(x => x.fnBOOKING_ID == _bookid);
                        result.IsSuccess = true;
                        result.Message = "調用紀錄已新增";
                        result.Data = new { fnBOOKING_ID = _bookid };//get//
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = string.Format($"調用紀錄新增失敗【{_exec.Split(':')[1]}】 ");
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"調用紀錄檔新增失敗【{ex.Message}】 ");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "BookingService",
                    Method = "[CreateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"調用紀錄檔新增失敗【{ex.Message}】 ")
                });
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 批次調用 確定建立: 【spINSERT_BOOKING_Batach】
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="createdby"></param>
        /// <returns></returns>
        public VerifyResult CreateBatchBooking(BatchBookingCreateModel rec, string createdby)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    // 批次調用檔案 無論是否有流程規則，不用過審。
                    #region -- EXECUTE dbo.spINSERT_BOOKING_BATCH -- 
                    var _exec = _db.spINSERT_BOOKING_BATCH(
                    rec.ResonId,
                    rec.ResonStr,
                    rec.DescStr,
                    rec.ProfileVideo,
                    rec.ProfileAudio,
                    rec.WaterMark,
                    rec.PathStr,
                    DateTime.Now,
                    createdby,
                    rec.FileNos).FirstOrDefault().ToString();
                    #endregion

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        //新增成功,回覆@fnBOOKING_ID
                        long.TryParse(_exec, out long _bookid);

                        result.IsSuccess = true;
                        result.Message = "批次調用紀錄已新增";
                        result.Data = new { fnBOOKING_ID = _bookid };
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = string.Format($"批次調用紀錄新增失敗【{_exec.Split(':')[1]}】 ");
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"批次調用紀錄檔新增失敗【{ex.Message}】 ");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "BookingService",
                    Method = "[CreateBatchBooking]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"批次調用紀錄檔新增失敗【{ex.Message}】 ")
                });
                #endregion
            }

            return result;
        }

        #endregion

    }
}
