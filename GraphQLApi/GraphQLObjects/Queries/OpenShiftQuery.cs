using GraphQLApi.ConfigurationOptions;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using GraphQLApi.GraphQLObjects.Clients;

namespace GraphQLApi.GraphQLObjects.Queries;

public class OpenShiftQuery : IOpenShift
{
    private readonly HttpClient _httpClient;
    private readonly OptionsConfiguration _optionsConfiguration;
    private readonly ILogger<OpenShiftQuery> _logger;

    public OpenShiftQuery(HttpClient httpClient, IOptions<OptionsConfiguration> options,
            ILogger<OpenShiftQuery> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _optionsConfiguration = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _httpClient.BaseAddress = new Uri(_optionsConfiguration.BaseAddress);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> GetOpenShiftAsync(string globalProviderId, string globalOfficeId, string jwtToken)
    {
        if (string.IsNullOrEmpty(_optionsConfiguration.OpenShiftEndPoint))
        {
            _logger.LogError("OpenShift endpoint configuration is missing.");
            throw new InvalidOperationException("OpenShift endpoint configuration is missing.");
        }

        _logger.LogInformation("Getting response from external REST API for OpenShift.");

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{_optionsConfiguration.OpenShiftEndPoint}?globalProviderID={globalProviderId}&globalOfficeID={globalOfficeId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request);
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "HTTP request error occurred.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred during HTTP request.");
            throw;
        }

        _logger.LogInformation("Response status code: {StatusCode}", response.StatusCode);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error response from external API: {StatusCode}", response.StatusCode);
            throw;
        }

        _logger.LogInformation("Receiving data from the external REST API.");
        var data = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("Received data from the external REST API at {Time}", DateTime.UtcNow);
        _logger.LogDebug("Data received: {Data}", data);

        return data;
    }
}