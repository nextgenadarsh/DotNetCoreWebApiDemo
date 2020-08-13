using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCoreWebApiDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders(); // Unregister any logging provider added by CreateDefaultBuilder
                    logging.AddConsole();
                    logging.AddDebug();
                    //logging.AddEventLog(eventLogSettings =>
                    //{
                    //    eventLogSettings.SourceName = "DotNetCoreWebApiDemoLogSource";
                    //});
                    //logging.AddAzureWebAppDiagnostics();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
            
    }
}
