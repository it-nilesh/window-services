using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.EventLog;

namespace WinServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<EventLogSettings>(config =>
                      {
                          config.LogName = "Test Services";
                          config.SourceName = "Test Services";
                      });
                    services.RegisterWinServices<ServicesRegister>();
                })
            .UseWindowsService()
            .UseSystemd();
    }
}
