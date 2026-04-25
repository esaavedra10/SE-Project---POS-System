using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoExample.Models;

public class Employees
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string name { get; set; } = null!;

    [BsonElement("EID")]
    public string EID { get; set; } = null!;

    [BsonElement("PID")]
    public string PID { get; set; } = null!;

    [BsonElement("DID")]
    public string DID { get; set; } = null!;

    [BsonElement("password")]
    public string password { get; set; } = null!;
}