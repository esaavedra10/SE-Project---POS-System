using MongoExample.Business;
using MongoExample.Models;

namespace Point_of_Sales_System.Tests;

public class ProductSaleRulesTests
{
    [Fact]
    public void Voided_product_is_blocked_for_sale()
    {
        var p = new Products
        {
            sku = "V1",
            name = "Voided",
            isVoided = true,
            stock = 99
        };

        Assert.True(ProductSaleRules.IsBlockedBecauseVoided(p));
    }

    [Fact]
    public void Active_product_is_not_blocked_for_sale_because_voided()
    {
        var p = new Products
        {
            sku = "A1",
            name = "Active",
            isVoided = false,
            stock = 5
        };

        Assert.False(ProductSaleRules.IsBlockedBecauseVoided(p));
    }

    [Fact]
    public void Null_product_treated_as_blocked()
    {
        Assert.True(ProductSaleRules.IsBlockedBecauseVoided(null));
    }
}
