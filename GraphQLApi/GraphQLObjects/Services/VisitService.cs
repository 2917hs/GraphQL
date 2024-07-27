using GraphQLApi.Database.Models;
using GraphQLApi.GraphQLObjects.Clients;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GraphQLApi.GraphQLObjects.Services;

public class VisitService
{
    private readonly IVisit _visit;

    public VisitService(IVisit visit)
    {
        _visit = visit ?? throw new ArgumentNullException(nameof(visit));
    }

    public IEnumerable<Visit> GetAllVisits(ObjectId? id)
    {
        var collection = _visit.GetAllVisits();
        if (id.HasValue)
        {
            var filter = Builders<Visit>.Filter.Eq(v => v.Id, id);
            return collection.Find(filter).ToList();
        }
        return collection.Find(_ => true).ToList();
    }
}
