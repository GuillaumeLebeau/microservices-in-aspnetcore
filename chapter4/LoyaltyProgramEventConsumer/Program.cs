using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LoyaltyProgramEventConsumer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration(
                    (hostingContext, config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: true);
                        config.AddEnvironmentVariables();
                        config.AddCommandLine(args);
                    }
                )
                .ConfigureServices(
                    (hostingContext, services) =>
                    {
                        services.Configure<EventSubscriberSettings>(hostingContext.Configuration);
                        services.AddOptions();
                        services.AddHostedService<EventSubscriberService>();
                    }
                )
                .ConfigureLogging(
                    (hostingContext, logging) =>
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                        logging.AddDebug();
                    }
                );

            await host.RunConsoleAsync();
        }
    }
}
