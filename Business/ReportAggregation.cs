using MongoExample.Models;

namespace MongoExample.Business;

/// <summary>
/// Daily report aggregates from in-memory transaction lists (same formulas as ReportsController).
/// </summary>
public static class ReportAggregation
{
    public static decimal TotalDiscounts(IEnumerable<Transactions> transactions) =>
        transactions.Sum(x => x.discountAmount);

    public static int DiscountTransactionCount(IEnumerable<Transactions> transactions) =>
        transactions.Count(x => x.discountAmount > 0);

    public static decimal TotalRefundAmounts(IEnumerable<Transactions> transactions) =>
        transactions.Where(x => x.refundedAt.HasValue).Sum(x => x.refundAmount ?? 0m);

    public static int RefundTransactionCount(IEnumerable<Transactions> transactions) =>
        transactions.Count(x => x.refundedAt.HasValue);
}
