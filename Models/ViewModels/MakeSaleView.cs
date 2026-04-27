using MongoExample.Models;

namespace MongoExample.Models.ViewModels;

public class MakeSaleView
{
    // SKU typed by cashier
    public string? SkuInput { get; set; }

    // Current cart items
    public List<SaleCartItem> CartItems { get; set; } = new();

    // Totals
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    // Payment selection
    public string PaymentMethod { get; set; } = "Cash";

    // Optional message for success/error text
    public string? Message { get; set; }
}

public class SaleCartItem
{
    public string? ProductId { get; set; }
    public string sku { get; set; } = null!;
    public string name { get; set; } = null!;
    public string category { get; set; } = null!;
    public decimal price { get; set; }
    public int quantity { get; set; }
    public decimal lineTotal { get; set; }
}