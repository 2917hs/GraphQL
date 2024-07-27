using GraphQLApi.ConfigurationOptions;
using Microsoft.Extensions.Options;
using System.Net;
using GraphQLApi.GraphQLObjects.Clients;

namespace GraphQLApi.GraphQLObjects.Services;

public class OpenShiftService
{
    private readonly IOpenShift _openShift;
    private readonly ILogger<OpenShiftService> _logger;
    private readonly TestKeysSettings _testKeysSettings;

    public OpenShiftService(IOpenShift openShift, ILogger<OpenShiftService> logger, IOptions<TestKeysSettings> testKeysSettings)
    {
        _openShift = openShift ?? throw new ArgumentNullException(nameof(openShift));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _testKeysSettings = testKeysSettings?.Value ?? throw new ArgumentNullException(nameof(testKeysSettings));
    }

    public async Task<string> GetOpenShiftAsync(string globalProviderId, string globalOfficeId, [Service] IHttpContextAccessor httpContextAccessor)
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

        string output;
        try
        {
            if (_testKeysSettings.IsTest)
            {
                jwtToken = _testKeysSettings.JwtToken;
            }

            output = await _openShift.GetOpenShiftAsync(globalProviderId, globalOfficeId, jwtToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting open shift data from external API.");
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return null;
        }

        return output;
    }
}
