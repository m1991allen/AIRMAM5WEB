using AIRMAM5.Controllers;
using AIRMAM5.DBEntity.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AIRMAM5.Filters
{
    /// <summary>
    /// Controller 攔截器擴增屬性
    /// </summary>
    /// ↓↓↓類和方法都使用時,加上這個特性,此時都其作用,不加,只方法起作用
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class InterceptorOfControllerAttribute : ActionFilterAttribute, IActionFilter
    {
        // OnActionExecuted 在執行操作方法(Action)後, 由 ASP.NET MVC 框架呼叫。
        // OnActionExecuting 在執行操作方法(Action)之前, 由 ASP.NET MVC 框架呼叫。
        // OnResultExecuted 在執行操作結果後, 由 ASP.NET MVC 框架呼叫。
        // OnResultExecuting 在執行操作結果之前, 由 ASP.NET MVC 框架呼叫。

        /// <summary>
        /// 專案紀錄 Serilog Service
        /// </summary>
        readonly SerilogService _serilogService = new SerilogService();

        /// <summary>
        /// Controller Method Name
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 過濾邏輯關鍵字
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 執行 Action 之前執行
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // 判斷Client Cookie
            HttpCookie cookieName = HttpContext.Current.Request.Cookies.Get("User");
            if (cookieName == null)
            {
                // iframe頁面 keyword= AuthCookie, 轉向 403 Error頁.
                if (Keyword.ToUpper() == "AUTHCOOKIE")
                {
                    ////filterContext.Result = new RedirectResult("../AIRMAM5/Account/LogOff");
                    //string _url = string.Format("http://{0}", HttpContext.Current.Request.Url.Authority);
                    //filterContext.Result = new RedirectResult(_url);

                    filterContext.Result = new RedirectResult("/AIRMAM5/Shared/NoAuth");
                }
                else { filterContext.Result = new RedirectResult("/AIRMAM5"); }
                
            }

            //// 運行中的 Controller & Action 資訊
            //string controllerName = filterContext.Controller.GetType().Name;
            //string actionName = filterContext.ActionDescriptor.ActionName;

            //// 參數資訊 parametersInfo
            //string paramInfo = JsonConvert.SerializeObject(filterContext.ActionParameters, new JsonSerializerSettings()
            //{
            //    ContractResolver = new ReadablePropertiesOnlyResolver()
            //}).Replace("{", string.Empty).Replace("}", string.Empty);

            //// _Serilog
            //string _msg = string.Format($"[{controllerName}].{actionName}() in【OnActionExecuting】. \r\n    {paramInfo}");
            //_serilogService.SerilogWriter(_msg, SerilogLevelEnum.Information);
        }

        /*
        /// <summary>
        /// 執行 Action 之後執行
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            // 運行中的 Controller & Action 資訊
            string controllerName = filterContext.Controller.GetType().Name;
            string actionName = filterContext.ActionDescriptor.ActionName;
            string routeData = JsonConvert.SerializeObject(filterContext.RouteData.Values).Replace("{", string.Empty).Replace("}", string.Empty);

            //_Serilog
            string _msg = string.Format($"[{controllerName}.{actionName}() out【OnActionExecuted】. \r\n    {routeData}");
            _serilogService.SerilogWriter(_msg, SerilogLevelEnum.Information);
        }

        /// <summary>
        /// 執行 Action Result 之前執行
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);

            // 運行中的 Controller & Action 資訊
            string controllerName = filterContext.Controller.GetType().Name;
            //string actionName = filterContext.ActionDescriptor.ActionName;

            // 資訊 
            string resultInfo = JsonConvert.SerializeObject(filterContext.Result, new JsonSerializerSettings()
            {
                ContractResolver = new ReadablePropertiesOnlyResolver()
            }).Replace("{", string.Empty).Replace("}", string.Empty);

            //_Serilog
            string _msg = string.Format($"[{controllerName}.{Method}() 【OnResultExecuting】. \r\n    {resultInfo}");
            //_serilogService.SerilogWriter(_msg, SerilogLevelEnum.Information);
        }

        /// <summary>
        /// 執行 Action Result 之後執行
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);

            // 運行中的 Controller & Action 資訊
            string controllerName = filterContext.Controller.GetType().Name;
            //string actionName = filterContext.ActionDescriptor.ActionName;

            // 資訊 
            string resultInfo = JsonConvert.SerializeObject(filterContext.Result).Replace("{", string.Empty).Replace("}", string.Empty);

            //_Serilog
            string _msg = string.Format($"[{controllerName}.{Method}() 【OnResultExecuted】. \r\n    {resultInfo}");
            //_serilogService.SerilogWriter(_msg, SerilogLevelEnum.Information);
        }

        */
    }

}