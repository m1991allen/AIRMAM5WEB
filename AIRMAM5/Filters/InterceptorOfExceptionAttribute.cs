using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace AIRMAM5.Filters
{
    /// <summary>
    /// Exception 攔截器擴增屬性
    /// </summary>
    public class InterceptorOfExceptionAttribute : ExceptionFilterAttribute
    {
        readonly SerilogService _serilogService = new SerilogService();

        public string Method { get; set; }

        //TODO: 無法執行到
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnException(actionExecutedContext);

            // 運行中的 Controller & Action 資訊
            string controllerName = actionExecutedContext.ActionContext.ControllerContext.Controller.GetType().Name;
            string actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;

            //_Serilog
            _serilogService.SerilogWriter(new SerilogInputModel
            {
                Controller = controllerName,
                Method = actionName,
                ErrorMessage = actionExecutedContext.Exception.Message,
                EventLevel = SerilogLevelEnum.Fatal,
                LogString = "Exception",
                Input = actionExecutedContext.Exception
            });

            string _msg = string.Format($"[{controllerName}.{actionName}() 【OnException】. ");
            _serilogService.SerilogWriter(_msg, SerilogLevelEnum.Information);

            // 重新打包回傳的訊息
            actionExecutedContext.Response = actionExecutedContext.Request
                .CreateResponse(System.Net.HttpStatusCode.InternalServerError, actionExecutedContext.Exception.Message);
        }
    }
}