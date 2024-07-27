using GraphQLApi.Database.Models;
using MongoDB.Driver;

namespace GraphQLApi.GraphQLObjects.Clients;

public interface IVisit
{
    IMongoCollection<Visit> GetAllVisits();
}