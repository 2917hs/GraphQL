using GraphQLApi.Database.Data;
using GraphQLApi.Database.Models;
using GraphQLApi.GraphQLObjects.Clients;
using MongoDB.Driver;

namespace GraphQLApi.GraphQLObjects.Queries;

public class VisitQuery(MongoDbContext mongoContext) : IVisit
{
    private readonly MongoDbContext _mongoContext = mongoContext ?? throw new ArgumentNullException(nameof(mongoContext));

    public IMongoCollection<Visit> GetAllVisits()
    {
        return _mongoContext.Visits;
    }
}