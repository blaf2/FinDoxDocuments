using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.IO;

namespace FinDoxDocumentsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            NLog.GlobalDiagnosticsContext.Set("LogDirectory", logPath);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging(options =>
                {
                    options.ClearProviders();
                    options.SetMinimumLevel(LogLevel.Trace);
                }).UseNLog();
    }
}
