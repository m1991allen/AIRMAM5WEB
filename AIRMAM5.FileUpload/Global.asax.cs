using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AIRMAM5.FileUpload
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            UnityConfig.RegisterComponents();                           // <----- Add this line
        }

        //protected void Application_BeginRequest(Object sender, EventArgs e)
        //{
        //    if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
        //    {
        //        HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache");
        //        HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
        //        HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
        //        HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
        //        HttpContext.Current.Response.End();
        //    }
        //}
    }
}
