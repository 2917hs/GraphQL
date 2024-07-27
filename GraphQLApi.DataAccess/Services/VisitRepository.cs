using GraphQLApi.Database.Data;
using GraphQLApi.Database.Models;
using MongoDB.Driver;

namespace GraphQLApi.DataAccess.Repositories;

public class VisitRepository : IVisitRepository
{
    private readonly IMongoCollection<Visit> _visitsCollection;

    public VisitRepository(MongoDbContext mongoContext)
    {
        _visitsCollection = mongoContext.Visits ?? throw new ArgumentNullException(nameof(mongoContext));
    }

    public IEnumerable<Visit> GetAllVisits()
    {
        return _visitsCollection.Find(_ => true).ToList();
    }
}
