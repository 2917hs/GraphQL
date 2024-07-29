using GraphQLApi.DataAccess.Repositories;
using GraphQLApi.Database.Data;
using GraphQLApi.Database.Models;
using MongoDB.Driver;

namespace GraphQLApi.DataAccess.Services;

public class VisitRepository(MongoDbContext mongoContext) : IVisitRepository
{
    private readonly IMongoCollection<Visit> _visitsCollection = mongoContext.Visits ?? throw new ArgumentNullException(nameof(mongoContext));

    public IEnumerable<Visit> GetAllVisits()
    {
        return _visitsCollection.Find(_ => true).ToList();
    }
}
