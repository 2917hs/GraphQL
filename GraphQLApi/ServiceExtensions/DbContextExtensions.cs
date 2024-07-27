using GraphQLApi.DataAccess.Repositories;
using GraphQLApi.Database.Data;
using GraphQLApi.GraphQLObjects.Clients;
using GraphQLApi.GraphQLObjects.Queries;
using GraphQLApi.GraphQLObjects.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace GraphQLApi.ServiceExtensions;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContext(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment env)
    {
        services.AddDbContext<InMemoryDbContext>(options =>
            options.UseInMemoryDatabase("InMemoryDb"));

        services.AddSingleton<IMongoClient>(new MongoClient(configuration.GetConnectionString("MongoDB")));
        services.AddScoped(serviceProvider =>
        {
            var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
            var databaseName = "local-visitdb";
            return new MongoDbContext(mongoClient, databaseName);
        });

        //services.AddScoped<IVisitRepository, VisitRepository>();
        // services.AddScoped<IVisit, VisitQuery>();
        // services.AddScoped<VisitService>();

        return services;
    }
}
