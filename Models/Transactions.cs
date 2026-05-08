using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoExample.Models;

[BsonIgnoreExtraElements]
public class Transactions
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("transactionNumber")]
    public string transactionNumber { get; set; } = null!;

    [BsonElement("employeeId")]
    public string employeeId { get; set; } = null!;

    [BsonElement("items")]
    public List<TransactionItem> items { get; set; } = new();

    [BsonElement("subtotal")]
    public decimal subtotal { get; set; }

    [BsonElement("discountAmount")]
    public decimal discountAmount { get; set; } = 0;

    [BsonElement("discountPercent")]
    [BsonIgnoreIfNull]
    public int? discountPercent { get; set; }

    [BsonElement("discountApprovedByEmployeeId")]
    [BsonIgnoreIfNull]
    public string? discountApprovedByEmployeeId { get; set; }

    [BsonElement("discountApprovedAt")]
    [BsonIgnoreIfNull]
    public DateTime? discountApprovedAt { get; set; }

    [BsonElement("refundAmount")]
    [BsonIgnoreIfNull]
    public decimal? refundAmount { get; set; }

    [BsonElement("refundedAt")]
    [BsonIgnoreIfNull]
    public DateTime? refundedAt { get; set; }

    [BsonElement("refundedByEmployeeId")]
    [BsonIgnoreIfNull]
    public string? refundedByEmployeeId { get; set; }

    [BsonElement("refundApprovedByEmployeeId")]
    [BsonIgnoreIfNull]
    public string? refundApprovedByEmployeeId { get; set; }

    [BsonElement("refundApprovedAt")]
    [BsonIgnoreIfNull]
    public DateTime? refundApprovedAt { get; set; }

    [BsonElement("tax")]
    public decimal tax { get; set; }

    [BsonElement("total")]
    public decimal total { get; set; }

    [BsonElement("paymentMethod")]
    public string paymentMethod { get; set; } = null!;

    [BsonElement("createdAt")]
    public DateTime createdAt { get; set; } = DateTime.UtcNow;
}

public class TransactionItem
{
    [BsonElement("productId")]
    public string productId { get; set; } = null!;

    [BsonElement("sku")]
    public string sku { get; set; } = null!;

    [BsonElement("name")]
    public string name { get; set; } = null!;

    [BsonElement("price")]
    public decimal price { get; set; }

    [BsonElement("quantity")]
    public int quantity { get; set; }

    [BsonElement("lineTotal")]
    public decimal lineTotal { get; set; }
}