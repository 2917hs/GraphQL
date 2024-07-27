using GraphQLApi.ConfigurationOptions;
using Serilog;

namespace GraphQLApi.ServiceExtensions;

public static class LoadOptionsExtensions
{
    public static IServiceCollection ConfigureMobileUiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OptionsConfiguration>(configuration.GetSection("MobileUIServices"));
        var apiSettings = configuration.GetSection("MobileUIServices").Get<OptionsConfiguration>();

        if (apiSettings == null || string.IsNullOrEmpty(apiSettings.BaseAddress))
        {
            Log.Fatal("The base address for the API is not configured.");
            throw new ArgumentNullException(nameof(apiSettings.BaseAddress), "The base address for the API is not configured.");
        }

        if (string.IsNullOrEmpty(apiSettings.HealthCheckEndPoint))
        {
            Log.Fatal("The end point for the API is not configured.");
            throw new ArgumentNullException(nameof(apiSettings.HealthCheckEndPoint), "The end point for the API is not configured.");
        }

        if (string.IsNullOrEmpty(apiSettings.OpenShiftEndPoint))
        {
            Log.Fatal("The end point for the API is not configured.");
            throw new ArgumentNullException(nameof(apiSettings.OpenShiftEndPoint), "The end point for the API is not configured.");
        }

        return services;
    }
}
