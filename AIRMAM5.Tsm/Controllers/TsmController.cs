using AIRMAM5.Tsm.Common;
using AIRMAM5.Tsm.Models;
using AIRMAM5.Tsm.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace AIRMAM5.Tsm.Controllers
{
    [RoutePrefix("Tsm")]
    public class TsmController : ApiController
    {
        /// <summary>
        /// 批次取得檔案在TSM的狀態
        /// </summary>
        /// <param name="clsFILE_STATUS_ARGS">檔案編號</param>
        /// <returns>檔案狀態</returns>
        [Route("GetFileStatusInTsm")]
        [ResponseType(typeof(List<clsFILE_STATUS_RESULT>))]
        public HttpResponseMessage PostGetFileStatusInTsm(clsFILE_STATUS_ARGS clsFILE_STATUS_ARGS)
        {
            try
            {
                if(clsFILE_STATUS_ARGS != null && clsFILE_STATUS_ARGS.lstFILE_TSM_PATH != null && clsFILE_STATUS_ARGS.lstFILE_TSM_PATH.Count > 0)
                {
                    var lstFILE_STATUS = repTSM.fnGET_FILE_STATUS(clsFILE_STATUS_ARGS);

                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true, Data = lstFILE_STATUS }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }
                else
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "請輸入要查詢的檔案編號" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }

        /// <summary>
        /// 取得在架上所有磁帶的資訊
        /// </summary>
        /// <returns></returns>
        [Route("GetTapeInfoInLib")]
        [ResponseType(typeof(List<clsTAPE_INFO_RESULT>))]
        public HttpResponseMessage GetTapeInfoInLib()
        {
            try
            {
                var lstTAPE_INFO = repTSM.fnGET_TAPE_INFO_IN_LIB();

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true, Data = lstTAPE_INFO.Where(f => f.VOL_TYPE == "DATA") }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }

        /// <summary>
        /// 取得納管過的所有磁帶的資訊
        /// </summary>
        /// <returns></returns>
        [Route("GetTapeInfoAll")]
        [ResponseType(typeof(List<clsTAPE_INFO_RESULT>))]
        public HttpResponseMessage GetTapeInfoAll()
        {
            try
            {
                var lstTAPE_INFO = repTSM.fnGET_TAPE_INFO_ALL();

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true, Data = lstTAPE_INFO.Where(f => f.VOL_TYPE == "DATA") }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }

        /// <summary>
        /// 磁帶下架
        /// </summary>
        /// <param name="clsTAPE_CHECK_OUT_ARGS">磁帶編號</param>
        /// <returns></returns>
        [Route("TapeCheckOut")]
        [ResponseType(typeof(clsRETURN))]
        public HttpResponseMessage PostTapeCheckOut(clsTAPE_CHECK_OUT_ARGS clsTAPE_CHECK_OUT_ARGS)
        {
            try
            {
                if(clsTAPE_CHECK_OUT_ARGS != null && clsTAPE_CHECK_OUT_ARGS.lstTAPE_NO != null && clsTAPE_CHECK_OUT_ARGS.lstTAPE_NO.Count > 0)
                {
                    if(clsTAPE_CHECK_OUT_ARGS.lstTAPE_NO.Count > int.Parse(Properties.Settings.Default.fnMAX_CHECKOUT_COUNT))
                    {
                        return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "同時最多"+ Properties.Settings.Default.fnMAX_CHECKOUT_COUNT + "捲磁帶下架" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                    }

                    string fsERROR = string.Empty;

                    string fsRESULT = repTSM.fnTAPE_CHECK_OUT(clsTAPE_CHECK_OUT_ARGS.lstTAPE_NO);

                    if (!string.IsNullOrEmpty(fsRESULT)) fsERROR += "下架磁帶錯誤:" + fsRESULT + "\r\n";

                    if(!string.IsNullOrEmpty(fsERROR))
                        return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = fsERROR }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                    else
                        return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "至少要有一捲磁帶編號" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }

        /// <summary>
        /// 磁帶上架
        /// </summary>
        /// <returns></returns>
        [Route("TapeCheckIn")]
        [ResponseType(typeof(clsRETURN))]
        public HttpResponseMessage PostTapeCheckIn()
        {
            try
            {
                //磁帶上架
                string fsRESULT = repTSM.fnTAPE_CHECK_IN();

                if (!string.IsNullOrEmpty(fsRESULT))
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "上架指令失敗" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }

        /// <summary>
        /// 檢查有無上架程序正在執行
        /// </summary>
        /// <returns></returns>
        [Route("IsTapeCheckInWorking")]
        [ResponseType(typeof(clsRETURN))]
        public HttpResponseMessage GetIsCheckInWorking()
        {
            try
            {
                //磁帶上架
                bool fsRESULT = repTSM.fnGET_TSM_HAS_WORK("CHECKIN LIBVOLUME");
                
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true, Data = fsRESULT }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }

        /// <summary>
        /// 檢查磁帶是否在架上
        /// </summary>
        /// <param name="clsTAPE_IS_IN_LIB_ARGS">要查詢的磁帶編號</param>
        /// <returns></returns>
        [Route("IsTapeInLib")]
        [ResponseType(typeof(clsRETURN))]
        public HttpResponseMessage PostIsTapeInLib(clsTAPE_IS_IN_LIB_ARGS clsTAPE_IS_IN_LIB_ARGS)
        {
            try
            {
                List<clsTAPE_IS_IN_LIB_RESULT> lstTAPE_IS_IN_LIB_RESULT = new List<clsTAPE_IS_IN_LIB_RESULT>();
                //磁帶是否在架上
                if (clsTAPE_IS_IN_LIB_ARGS.lstTAPE_NO != null && clsTAPE_IS_IN_LIB_ARGS.lstTAPE_NO.Count > 0)
                {
                    foreach (var item in clsTAPE_IS_IN_LIB_ARGS.lstTAPE_NO)
                    {
                        bool fsRESULT = repTSM.fnTAPE_IS_IN_LIB(item);
                        lstTAPE_IS_IN_LIB_RESULT.Add(new clsTAPE_IS_IN_LIB_RESULT() { fsTAPE_NO = item, fbIS_IN_LIB = fsRESULT });
                    }
                }

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true, Data = lstTAPE_IS_IN_LIB_RESULT }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }
    }
}
