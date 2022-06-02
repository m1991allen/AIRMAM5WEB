using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Web.Hosting;

[assembly: OwinStartupAttribute(typeof(AIRMAM5.Startup))]
namespace AIRMAM5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            #region _Serilog's setup
            string serilogPath = Config.SerilogPath;//ConfigurationManager.AppSettings["SerilogPath"].ToString();
            string logFolder = (string.IsNullOrWhiteSpace(serilogPath) || serilogPath.ToUpper() == "LOCAL")
                ? HostingEnvironment.MapPath("~") + @"\App_Data\"
                : serilogPath;

            logFolder = string.Format(@"{0}Logs_{1:yyyyMMdd}\", logFolder, DateTime.Now);
            if (!Directory.Exists(logFolder)) { Directory.CreateDirectory(logFolder); }

            var logConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Error)
                .WriteTo.RollingFile(Path.Combine(logFolder, @"log-{Hour}.txt"),
                    //outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}", 
                    fileSizeLimitBytes: null, 
                    retainedFileCountLimit: 200);

            Log.Logger = logConfig.CreateLogger();
            #endregion

            #region >>> SignalR SEtup
            ////app.MapSignalR();
            app.UseCors(CorsOptions.AllowAll);
            //app.UseCors(SignalrCorsOptions.Value);
            app.MapSignalR(new HubConfiguration { EnableJSONP = true });

            //app.Map("/signalr", map =>
            //{
            //    map.UseCors(CorsOptions.AllowAll);
            //    //map.UseCors(SignalrCorsOptions.Value);

            //    var hubConfiguration = new HubConfiguration
            //    {
            //        EnableJSONP = true
            //    };

            //    map.RunSignalR(hubConfiguration);
            //});
            #endregion
        }

        //private static readonly Lazy<CorsOptions> SignalrCorsOptions = new Lazy<CorsOptions>(() =>
        //{
        //    return new CorsOptions
        //    {
        //        PolicyProvider = new CorsPolicyProvider
        //        {
        //            PolicyResolver = context =>
        //            {
        //                var policy = new CorsPolicy
        //                {
        //                    AllowAnyOrigin = true,
        //                    AllowAnyMethod = true,
        //                    AllowAnyHeader = true,
        //                    SupportsCredentials = false,
        //                };

        //                // Add allowed origins.
        //                //policy.Origins.Add("http://localhost");
        //                //policy.Origins.Add("http://172.20.142.35/AIRMAM5"); //
        //                //policy.Origins.Add("http://172.20.142.37/AIRMAM5.FileUpload");  //
        //                //policy.Origins.Add("http://172.20.142.38/AIRMAM5.FileUpload"); //

        //                return Task.FromResult(policy);
        //            }
        //        }
        //    };
        //});

    }
}
