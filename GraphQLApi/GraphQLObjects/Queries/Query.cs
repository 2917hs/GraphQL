using System.Net;
using System.Net.Http.Headers;
using GraphQLApi.ConfigurationOptions;
using GraphQLApi.Database.Data;
using GraphQLApi.Database.Models;
using GraphQLApi.GraphQLObjects.Clients;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GraphQLApi.GraphQLObjects.Queries;

public class Query : IHealth, IOpenShift, IVisit
{
    private readonly HttpClient _httpClient;
    private readonly OptionsConfiguration _optionsConfiguration;
    private readonly ILogger<Query> _logger;
    private readonly MongoDbContext _mongoContext;
    private readonly InMemoryDbContext _inMemoryDbContext;
    public Query(HttpClient httpClient, 
        IOptions<OptionsConfiguration> options,
        ILogger<Query> logger,
        MongoDbContext mongoContext,
        InMemoryDbContext inMemoryDbContext)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _optionsConfiguration = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _httpClient.BaseAddress = new Uri(_optionsConfiguration.BaseAddress);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mongoContext = mongoContext ?? throw new ArgumentNullException(nameof(mongoContext));
        _inMemoryDbContext = inMemoryDbContext ?? throw new ArgumentNullException(nameof(inMemoryDbContext));
    }

    protected Query()
    {
        throw new NotImplementedException();
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
    
    public IQueryable<Payer> GetPayers(int? chhaId = null, string? chhaName = null, string? chhaInitial = null)
    {
        IQueryable<Payer> query = _inMemoryDbContext.Payers;

        _logger.LogDebug("Querying Payer list");

        if (chhaId.HasValue && !string.IsNullOrEmpty(chhaInitial))
        {
            query = query.Where(p => p.ChhaId == chhaId.Value || p.ChhaInitial.Contains(chhaInitial));
        }
        else if (chhaId.HasValue)
        {
            query = query.Where(p => p.ChhaId == chhaId.Value);
        }
        else if (!string.IsNullOrEmpty(chhaInitial))
        {
            query = query.Where(p => p.ChhaInitial.Contains(chhaInitial));
        }

        _logger.LogDebug("Filtered query");

        return query;
    }
    
    public IMongoCollection<Visit> GetAllVisits()
    {
        return _mongoContext.Visits;
    }
}