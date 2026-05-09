using MongoExample.Business;
using MongoExample.Models;

namespace Point_of_Sales_System.Tests;

public class ReportAggregationTests
{
    [Fact]
    public void TotalDiscounts_sums_discountAmount()
    {
        var list = new List<Transactions>
        {
            new() { total = 100m, discountAmount = 5m },
            new() { total = 50m, discountAmount = 0m },
            new() { total = 20m, discountAmount = 2.5m }
        };

        Assert.Equal(7.5m, ReportAggregation.TotalDiscounts(list));
    }

    [Fact]
    public void DiscountTransactionCount_counts_positive_discountAmount()
    {
        var list = new List<Transactions>
        {
            new() { discountAmount = 1m },
            new() { discountAmount = 0m },
            new() { discountAmount = 0.01m }
        };

        Assert.Equal(2, ReportAggregation.DiscountTransactionCount(list));
    }

    [Fact]
    public void Refund_aggregates_only_refunded_transactions()
    {
        var refundedAt = DateTime.UtcNow;
        var list = new List<Transactions>
        {
            new() { total = 100m, refundedAt = refundedAt, refundAmount = 100m },
            new() { total = 50m, refundedAt = null, refundAmount = null },
            new() { total = 25m, refundedAt = refundedAt, refundAmount = 25m }
        };

        Assert.Equal(125m, ReportAggregation.TotalRefundAmounts(list));
        Assert.Equal(2, ReportAggregation.RefundTransactionCount(list));
    }

    [Fact]
    public void RefundAmount_defaults_to_zero_when_null_on_refunded_row()
    {
        var list = new List<Transactions>
        {
            new() { refundedAt = DateTime.UtcNow, refundAmount = null }
        };

        Assert.Equal(0m, ReportAggregation.TotalRefundAmounts(list));
        Assert.Equal(1, ReportAggregation.RefundTransactionCount(list));
    }
}
