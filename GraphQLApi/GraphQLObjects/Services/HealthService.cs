using System.Net;
using GraphQLApi.GraphQLObjects.Clients;

namespace GraphQLApi.GraphQLObjects.Services;

public class HealthService
{
    private readonly IHealth _health;
    private readonly ILogger<HealthService> _logger;

    public HealthService(IHealth health, ILogger<HealthService> logger)
    {
        _health = health;
        _logger = logger;
    }

    public async Task<string> GetHealthAsync(string eTag, [Service] IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        var authorizationHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            _logger.LogError("Authorization header is missing or invalid.");
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }

        var jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();

        if (string.IsNullOrEmpty(eTag))
        {
            _logger.LogWarning("eTag is null or empty.");
        }

        string output;
        try
        {
            output = await _health.GetHealthAsync(eTag, jwtToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting health status from external API.");
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return null;
        }

        if (output == null)
        {
            _logger.LogInformation("Response not modified.");
            httpContext.Response.StatusCode = (int)HttpStatusCode.NotModified;
            return null;
        }

        // Generate a new ETag for the response
        var newETag = GenerateETag("GraphQLApi");
        httpContext.Response.Headers["ETag"] = newETag;

        return output;
    }

    private string GenerateETag(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new ArgumentNullException(nameof(data));
        }

        return $"\"{data}\"";
    }
}

