namespace GraphQLApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguredHttpClient<TService, TImplementation>(
        this IServiceCollection services, 
        string baseAddress,
        Action<HttpClient> configureClient = null) 
        where TService : class
        where TImplementation : class, TService
    {
        services
            .AddHttpClient<TService, TImplementation>(client =>
            {
                client.BaseAddress = new Uri(baseAddress);
                configureClient?.Invoke(client);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            });

        services.AddScoped<TService, TImplementation>();

        return services;
    }
}