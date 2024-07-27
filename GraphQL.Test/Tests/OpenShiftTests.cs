using GraphQLApi.ConfigurationOptions;
using GraphQLApi.GraphQLObjects.Clients;
using GraphQLApi.GraphQLObjects.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace GraphQLApi.Test.Tests;

public class OpenShiftTests
{
    private readonly Mock<IOpenShift> _mockGraphQlQuery;
    private readonly Mock<ILogger<OpenShiftService>> _mockLogger;
    private readonly Mock<IOptions<TestKeysSettings>> _mockTestKeysSettings;

    public OpenShiftTests()
    {
        _mockGraphQlQuery = new Mock<IOpenShift>();
        _mockLogger = new Mock<ILogger<OpenShiftService>>();
        _mockTestKeysSettings = new Mock<IOptions<TestKeysSettings>>();
    }

    [Fact]
    public async Task GetOpenShiftAsync_WithValidParameters_ReturnsExpectedResults()
    {
        // Arrange
        var expectedOutput = "Expected output";
        var mockTestKeysSettings = new Mock<IOptions<TestKeysSettings>>();
        mockTestKeysSettings.Setup(o => o.Value).Returns(new TestKeysSettings { IsTest = true, JwtToken = "your_jwt_token" });

        _mockGraphQlQuery.Setup(q => q.GetOpenShiftAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                         .ReturnsAsync(expectedOutput);

        var openShiftService = new OpenShiftService(
            _mockGraphQlQuery.Object,
            _mockLogger.Object,
            mockTestKeysSettings.Object
        );

        // Act
        var result = await openShiftService.GetOpenShiftAsync("globalProviderID", "globalOfficeID", CreateHttpContextAccessorWithAuthorization());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedOutput, result);
    }

    private IHttpContextAccessor CreateHttpContextAccessorWithAuthorization()
    {
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(new DefaultHttpContext
        {
            Request = { Headers = { ["Authorization"] = "Bearer testToken" } }
        });

        return mockHttpContextAccessor.Object;
    }
}
