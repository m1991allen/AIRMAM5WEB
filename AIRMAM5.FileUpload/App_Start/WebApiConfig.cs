using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AIRMAM5.FileUpload
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //永遠只回應 JSON 格式
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            // Web API 設定和服務

            //var cors = new EnableCorsAttribute(origins: "*", headers: "*", methods: "*");
            config.EnableCors();

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "{controller}/{action}/{id}",
            //    defaults: new { controller = "FileUpload", action = "UploadFile", id = RouteParameter.Optional }
            //);
        }
    }
}
