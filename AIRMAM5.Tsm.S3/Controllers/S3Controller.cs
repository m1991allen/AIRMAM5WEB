using AIRMAM5.Tsm.S3.Models;
using AIRMAM5.Tsm.S3.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;


namespace AIRMAM5.Tsm.S3.Controllers
{
    [RoutePrefix("Tsm")]
    public class S3Controller : ApiController
    {
        /// <summary>
        /// 批次取得檔案在S3的狀態
        /// </summary>
        /// <param name="clsFILE_STATUS_ARGS">檔案編號</param>
        /// <returns>檔案狀態</returns>
        [Route("GetFileStatusInTsm")]
        [ResponseType(typeof(List<clsFILE_STATUS_RESULT>))]
        public HttpResponseMessage PostGetFileStatusInTsm(clsFILE_STATUS_ARGS clsFILE_STATUS_ARGS)
        {
            try
            {
                if (clsFILE_STATUS_ARGS != null && clsFILE_STATUS_ARGS.lstFILE_TSM_PATH != null && clsFILE_STATUS_ARGS.lstFILE_TSM_PATH.Count > 0)
                {
                    List<clsFILE_STATUS_RESULT> lstFILE_STATUS_RESULT = new List<clsFILE_STATUS_RESULT>();
                    lstFILE_STATUS_RESULT = new repS3().fnGET_FILE_IS_IN_S3_STD(clsFILE_STATUS_ARGS);
                    //foreach (var item in clsFILE_STATUS_ARGS.lstFILE_TSM_PATH)
                    //{
                    //    var status = new repS3().fnGET_FILE_IS_IN_S3_STD(item.fsFILE_TSM_PATH);

                    //    if(status == "Online")
                    //        lstFILE_STATUS_RESULT.Add(new clsFILE_STATUS_RESULT() { FILE_NO = item.fsFILE_NO, FILE_STATUS = FileStatus.Online });
                    //    else if (status == "Offline")
                    //        lstFILE_STATUS_RESULT.Add(new clsFILE_STATUS_RESULT() { FILE_NO = item.fsFILE_NO, FILE_STATUS = FileStatus.Offline });
                    //    else if (status == "Empty")
                    //        lstFILE_STATUS_RESULT.Add(new clsFILE_STATUS_RESULT() { FILE_NO = item.fsFILE_NO, FILE_STATUS = FileStatus.Empty });
                    //    else
                    //        lstFILE_STATUS_RESULT.Add(new clsFILE_STATUS_RESULT() { FILE_NO = item.fsFILE_NO, FILE_STATUS = FileStatus.Error });
                    //}

                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true, Data = lstFILE_STATUS_RESULT }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }
                else
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "請輸入要查詢的檔案編號" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }

        /// <summary>
        /// Recall檔案
        /// </summary>
        /// <param name="clsRECALL_ARGS">檔案回溯參數</param>
        /// <returns>檔案狀態</returns>
        [Route("Recall")]
        [ResponseType(typeof(clsRETURN))]
        public HttpResponseMessage PostRecall(clsRECALL_ARGS clsRECALL_ARGS)
        {
            try
            {
                if(new repS3().fnRECALL_FILE(clsRECALL_ARGS.fsFILE_PATH, clsRECALL_ARGS.fnRESERVE_DAYS, clsRECALL_ARGS.fsRECALL_MODE))
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }
                else
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }
    }
}
