using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoExample.Models;

public class Products
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string name { get; set; } = null!;

    [BsonElement("category")]
    public string category { get; set; } = null!;

    [BsonElement("brand")]
    public string brand { get; set; } = null!;

    [BsonElement("sku")]
    public string sku { get; set; } = null!;

    [BsonElement("price")]
    public decimal price { get; set; }

    [BsonElement("stock")]
    public int stock { get; set; }

    [BsonElement("taxable")]
    public bool taxable { get; set; }

    [BsonElement("ageRestricted")]
    [BsonIgnoreIfNull]
    public bool? ageRestricted { get; set; }
}