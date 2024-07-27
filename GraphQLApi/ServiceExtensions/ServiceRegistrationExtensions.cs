using GraphQLApi.Filters;
using GraphQLApi.GraphQLObjects.Clients;
using GraphQLApi.GraphQLObjects.Queries;
using GraphQLApi.GraphQLObjects.Services;

namespace GraphQLApi.ServiceExtensions;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddErrorFilter<ErrorFilter>();
        services.AddScoped<IPayer, PayerQuery>();
        services.AddScoped<PayerService>();
        services.AddHttpContextAccessor();

        return services;
    }
}
