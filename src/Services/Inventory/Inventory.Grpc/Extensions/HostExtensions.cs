using Common.Logging;
using Serilog;

namespace Inventory.Grpc.Extensions;

public static class HostExtensions
{
    internal static void AddAppConfigurations(this ConfigureHostBuilder host)
    {
        host.ConfigureAppConfiguration((context, config) =>
        {
            var env = context.HostingEnvironment;
            config.AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true,
                    true)
                .AddEnvironmentVariables();
        }).UseSerilog(Serilogger.Configure);
    }
}