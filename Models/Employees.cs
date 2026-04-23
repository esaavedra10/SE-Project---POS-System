using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace MongoExample.Models;

public class Employees
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [BsonElement("name")]
    [JsonPropertyName("name")]
    public string name { get; set; } = null!;

    [BsonElement("EID")]
    [JsonPropertyName("EID")]
    public string EID { get; set; } = null!;

    [BsonElement("PID")]
    [JsonPropertyName("PID")]
    public string PID { get; set; } = null!;

    [BsonElement("DID")]
    [JsonPropertyName("DID")]
    public string DID { get; set; } = null!;
}
