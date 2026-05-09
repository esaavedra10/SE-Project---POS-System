namespace MongoExample.Business;

/// <summary>
/// Pure helpers for cash tender validation and change (used by sale completion and tests).
/// </summary>
public static class CashPaymentRules
{
    public static bool TryValidateCashPayment(decimal? cashReceived, decimal total, out string? errorMessage)
    {
        if (!cashReceived.HasValue)
        {
            errorMessage = "Enter cash received for cash payment.";
            return false;
        }

        if (cashReceived.Value < total)
        {
            errorMessage = "Cash received must be greater than or equal to the total.";
            return false;
        }

        errorMessage = null;
        return true;
    }

    public static decimal ComputeChangeDue(decimal cashReceived, decimal total) =>
        Math.Round(cashReceived - total, 2);
}
