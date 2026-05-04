using MongoExample.Models;

namespace MongoExample.Models.ViewModels;

public class MakeSaleView
{
    // SKU typed by cashier
    public string? SkuInput { get; set; }
    public string? NameInput { get; set; }

    // Current cart items
    public List<SaleCartItem> CartItems { get; set; } = new();

    // Totals (Subtotal = discounted subtotal for payment math; use SubtotalBeforeDiscount for line sum)
    public decimal SubtotalBeforeDiscount { get; set; }
    public decimal DiscountAmountApplied { get; set; }
    public decimal DiscountedSubtotal { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    // Payment selection
    public string PaymentMethod { get; set; } = "Cash";

    // Optional message for success/error text
    public string? Message { get; set; }
    public string? PendingRestrictedSku { get; set; }
    public bool ShowApprovalLogin { get; set; }
    public string? ApprovalEmployeeId { get; set; }
    public string? ApprovalPassword { get; set; }

    // Discount workflow (percentage presets only)
    public bool ShowDiscountApprovalPrompt { get; set; }
    public bool DiscountApprovalGranted { get; set; }
    public bool ShowDiscountPercentOptions { get; set; }
    public int? SelectedDiscountPercent { get; set; }
    public string? DiscountApprovalEmployeeId { get; set; }
    public string? DiscountApprovalPassword { get; set; }
    public string? DiscountApprovedByEmployeeId { get; set; }
    public DateTime? DiscountApprovedAt { get; set; }
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