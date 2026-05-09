using MongoExample.Models;

namespace MongoExample.Business;

/// <summary>
/// Sale eligibility checks decoupled from HTTP/session (mirrors Make Sale guards).
/// </summary>
public static class ProductSaleRules
{
    /// <summary>
    /// True when the product cannot be sold because it is voided (null product treated as not sellable).
    /// </summary>
    public static bool IsBlockedBecauseVoided(Products? product) =>
        product == null || product.isVoided;
}
