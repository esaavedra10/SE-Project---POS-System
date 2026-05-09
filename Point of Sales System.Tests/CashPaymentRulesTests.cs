using MongoExample.Business;

namespace Point_of_Sales_System.Tests;

public class CashPaymentRulesTests
{
    [Theory]
    [InlineData(10.00, 10.00, true)]
    [InlineData(10.01, 10.00, true)]
    [InlineData(10.00, 10.01, false)]
    [InlineData(9.99, 10.00, false)]
    public void TryValidateCashPayment_requires_received_gte_total(
        decimal received,
        decimal total,
        bool expectedOk)
    {
        var ok = CashPaymentRules.TryValidateCashPayment(received, total, out var error);
        Assert.Equal(expectedOk, ok);
        if (expectedOk)
            Assert.Null(error);
        else
            Assert.NotNull(error);
    }

    [Fact]
    public void TryValidateCashPayment_null_received_fails()
    {
        var ok = CashPaymentRules.TryValidateCashPayment(null, 5m, out var error);
        Assert.False(ok);
        Assert.Equal("Enter cash received for cash payment.", error);
    }

    [Theory]
    [InlineData(20.00, 12.34, 7.66)]
    [InlineData(10.00, 10.00, 0.00)]
    [InlineData(15.126, 10.00, 5.13)]
    public void ComputeChangeDue_rounds_to_two_decimals(decimal received, decimal total, decimal expectedChange)
    {
        Assert.Equal(expectedChange, CashPaymentRules.ComputeChangeDue(received, total));
    }
}
