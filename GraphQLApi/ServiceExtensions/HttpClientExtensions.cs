using GraphQLApi.ConfigurationOptions;
using GraphQLApi.Extensions;
using GraphQLApi.GraphQLObjects.Clients;
using GraphQLApi.GraphQLObjects.Queries;

namespace GraphQLApi.ServiceExtensions;

public static class HttpClientExtensions
{
    public static IServiceCollection AddCustomHttpClients(this IServiceCollection services, OptionsConfiguration apiSettings)
    {
        services.AddConfiguredHttpClient<IHealth, HealthQuery>(apiSettings.HealthCheckEndPoint);
        services.AddConfiguredHttpClient<IOpenShift, OpenShiftQuery>(apiSettings.OpenShiftEndPoint);

        return services;
    }
}