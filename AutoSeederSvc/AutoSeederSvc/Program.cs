using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace AutoSeederSvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string logDir = $"{Environment.CurrentDirectory}\\Logs\\{DateTime.Now.ToString("yyy-MM-dd")}Log";

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(logDir)
            .WriteTo.Console()
            .CreateLogger();


            try
            {
                Log.Information("Starting AutoSeeder service");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal($"Fatal error starting service - {ex.GetBaseException().Message}");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }



        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
           .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                })
            .UseSerilog();
    }
}
