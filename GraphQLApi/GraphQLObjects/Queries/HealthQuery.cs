using GraphQLApi.ConfigurationOptions;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using GraphQLApi.GraphQLObjects.Clients;

namespace GraphQLApi.GraphQLObjects.Queries;

public class HealthQuery : Query, IHealth
{
    private readonly HttpClient _httpClient;
    private readonly OptionsConfiguration _optionsConfiguration;
    private readonly ILogger<HealthQuery> _logger;

    public HealthQuery(HttpClient httpClient, IOptions<OptionsConfiguration> options,
            ILogger<HealthQuery> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _optionsConfiguration = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _httpClient.BaseAddress = new Uri(_optionsConfiguration.BaseAddress);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> GetHealthAsync(string eTag, string jwtToken)
    {
        if (string.IsNullOrEmpty(_optionsConfiguration.HealthCheckEndPoint))
        {
            _logger.LogError("HealthCheck endpoint configuration is missing.");
            throw new InvalidOperationException("HealthCheck endpoint configuration is missing.");
        }

        _logger.LogInformation("Getting response from external REST API with ETag.");

        var request = new HttpRequestMessage(HttpMethod.Get, _optionsConfiguration.HealthCheckEndPoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        if (!string.IsNullOrEmpty(eTag))
        {
            _logger.LogInformation($"Found {eTag} in the subsequent request.");
            var quotedETag = new EntityTagHeaderValue($"\"{eTag}\"");
            request.Headers.IfNoneMatch.Add(quotedETag);
        }

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

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            _logger.LogInformation("Response not modified.");
            return null;
        }

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