using System;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using Microsoft.Owin;
using Owin;
using Serilog;
using Serilog.Events;

[assembly: OwinStartup(typeof(AIRMAM5.FileUpload.Startup1))]

namespace AIRMAM5.FileUpload
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {
            // 如需如何設定應用程式的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkID=316888
            #region _Serilog's setup
            string serilogPath = ConfigurationManager.AppSettings["SerilogPath"].ToString();
            string logFolder = (string.IsNullOrWhiteSpace(serilogPath) || serilogPath.ToUpper() == "LOCAL")
                ? HostingEnvironment.MapPath("~") + @"\App_Data\"
                : serilogPath;

            logFolder = string.Format(@"{0}apiLogs_{1:yyyyMMdd}\", logFolder, DateTime.Now);
            if (!Directory.Exists(logFolder)) { Directory.CreateDirectory(logFolder); }

            var logConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Error)
                .WriteTo.RollingFile(
                    Path.Combine(logFolder, @"log-{Hour}.txt"),
                    //outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
                    fileSizeLimitBytes: null, //52428800,
                    retainedFileCountLimit: 200);

            Log.Logger = logConfig.CreateLogger();
            #endregion

        }
    }
}
