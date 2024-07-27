using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GraphQLApi.Database.Models;

public class Visit
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("CreatedDate")]
    public DateTime CreatedDate { get; set; }

    [BsonElement("CreatedUTCDate")]
    public DateTime CreatedUtcDate { get; set; }

    [BsonElement("VisSkillType")]
    public string VisSkillType { get; set; }

    [BsonElement("VisitID")]
    public string VisitId { get; set; }
}
