using GraphQLApi.Database.Models;
using MongoDB.Driver;

namespace GraphQLApi.Database.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient mongoClient, string databaseName)
    {
        _database = mongoClient.GetDatabase(databaseName);
    }

    public IMongoCollection<Visit> Visits => _database.GetCollection<Visit>("Visit");
}
