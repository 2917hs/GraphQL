using GraphQLApi.ConfigurationOptions;
using GraphQLApi.Database.Data;
using GraphQLApi.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Diagnostics;
using GraphQLApi.GraphQLObjects.Queries;

namespace GraphQLApi.Test.Tests;

public class PayerTests
{
    private readonly Mock<IOptions<OptionsConfiguration>> _mockOptions;
    private readonly ILogger<PayerQuery> _logger;
    private readonly InMemoryDbContext _inMemoryDbContext;

    public PayerTests()
    {
        _mockOptions = new Mock<IOptions<OptionsConfiguration>>();

        var serviceProvider = new ServiceCollection()
            .AddLogging(configure => configure.AddConsole())
            .BuildServiceProvider();

        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger<PayerQuery>();

        // Initialize InMemoryDbContext
        _inMemoryDbContext = GetInMemoryDataContext("TestDatabase");
    }

    private InMemoryDbContext GetInMemoryDataContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<InMemoryDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        return new InMemoryDbContext(options);
    }

    private void SeedDataContext(InMemoryDbContext context)
    {
        context.Payers.AddRange(
            new Payer { ChhaId = 1, ChhaInitial = "ABC", ChhaName = "Name1" },
            new Payer { ChhaId = 2, ChhaInitial = "XYZ", ChhaName = "Name2" }
        );
        context.SaveChanges();
    }

    [Fact]
    public void GetPayers_WithValidParameters_ReturnsExpectedResults()
    {
        // Arrange
        SeedDataContext(_inMemoryDbContext);

        var options = new OptionsConfiguration
        {
            BaseAddress = "https://localhost:5135/",
            HealthCheckEndPoint = "health",
            OpenShiftEndPoint = "private/v1/provider/10/office/20/openshifts"
        };
        _mockOptions.Setup(o => o.Value).Returns(options);

        var graphQlQuery = new PayerQuery(
            new HttpClient(), 
            _mockOptions.Object,
            _logger,
            _inMemoryDbContext
        );

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = graphQlQuery.GetPayers(1, null, "ABC");
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, p => p.ChhaId == 1 && p.ChhaInitial == "ABC" && p.ChhaName == "Name1" );
        _logger.LogInformation($"GetPayers_WithValidParameters executed in {stopwatch.ElapsedMilliseconds} ms");
    }

    [Fact]
    public void GetPayers_WithNullParameters_ReturnsAllPayers()
    {
        // Arrange
        SeedDataContext(_inMemoryDbContext);

        var options = new OptionsConfiguration
        {
            BaseAddress = "https://localhost:5135/",
            HealthCheckEndPoint = "health",
            OpenShiftEndPoint = "private/v1/provider/10/office/20/openshifts"
        };
        _mockOptions.Setup(o => o.Value).Returns(options);

        var graphQlQuery = new PayerQuery(
            new HttpClient(), 
            _mockOptions.Object,
            _logger,
            _inMemoryDbContext
        );

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = graphQlQuery.GetPayers(null, null, null);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _logger.LogInformation($"GetPayers_WithNullParameters executed in {stopwatch.ElapsedMilliseconds} ms");
    }
}
