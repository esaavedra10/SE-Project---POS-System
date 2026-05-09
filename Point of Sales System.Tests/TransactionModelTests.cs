using MongoExample.Models;

namespace Point_of_Sales_System.Tests;

public class TransactionModelTests
{
    [Fact]
    public void Transaction_can_store_payment_cash_tender_and_metadata()
    {
        var created = new DateTime(2026, 5, 9, 12, 0, 0, DateTimeKind.Utc);
        var tx = new Transactions
        {
            transactionNumber = "TXN-TEST-001",
            employeeId = "E100",
            paymentMethod = "Cash",
            subtotal = 50m,
            tax = 4.13m,
            total = 54.13m,
            cashReceived = 60.00m,
            changeDue = 5.87m,
            discountAmount = 0,
            createdAt = created,
            items = new List<TransactionItem>
            {
                new()
                {
                    productId = "507f1f77bcf86cd799439011",
                    sku = "SKU1",
                    name = "Test",
                    price = 50m,
                    quantity = 1,
                    lineTotal = 50m
                }
            }
        };

        Assert.Equal("TXN-TEST-001", tx.transactionNumber);
        Assert.Equal("Cash", tx.paymentMethod);
        Assert.Equal(60.00m, tx.cashReceived);
        Assert.Equal(5.87m, tx.changeDue);
        Assert.Equal(created, tx.createdAt);
        Assert.Single(tx.items);
    }

    [Fact]
    public void Transaction_card_sale_omits_cash_fields()
    {
        var tx = new Transactions
        {
            transactionNumber = "TXN-CARD-1",
            employeeId = "E100",
            paymentMethod = "Card",
            subtotal = 10m,
            tax = 0.83m,
            total = 10.83m,
            cashReceived = null,
            changeDue = null,
            createdAt = DateTime.UtcNow
        };

        Assert.Equal("Card", tx.paymentMethod);
        Assert.Null(tx.cashReceived);
        Assert.Null(tx.changeDue);
    }
}
