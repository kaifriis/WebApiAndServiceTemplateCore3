using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Autofac.Extensions.DependencyInjection;

namespace Service.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Use static Serilog logger. Injection of ILogger instance is not supported in the main program class in .net Core 3+
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) //Filtering out information log items
                .Enrich.FromLogContext()
                .WriteTo.File(@"C:\svc\WorkerService\Log.txt")
                .CreateLogger();


            try
            {
                Log.Information("Starting up service");
                var processModule = Process.GetCurrentProcess().MainModule;
                if (processModule != null)
                {
                    var pathToExe = processModule.FileName;
                    var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                    Directory.SetCurrentDirectory(pathToContentRoot);
                }
                else
                {
                    Log.Error("processModule == null");
                }

                CreateHostBuilder(args).Build().Run();
                

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog()
                .UseWindowsService() //The worker service is added in WebStartup class
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<WebStartup>();
                });
    }
}
