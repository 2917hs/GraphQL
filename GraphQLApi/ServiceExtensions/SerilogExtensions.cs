using Serilog;

namespace GraphQLApi.ServiceExtensions;

public static class SerilogExtensions
{
    public static IHostBuilder UseCustomSerilog(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        return hostBuilder.UseSerilog();
    }
}