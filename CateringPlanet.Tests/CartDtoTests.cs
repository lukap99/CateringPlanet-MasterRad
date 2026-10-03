using System.Text.Json;
using CateringPlanet.Application.DTOs;

namespace CateringPlanet.Tests;

/// <summary>
/// Unit tests for the computed properties of <see cref="CartDto"/>.
/// </summary>
public class CartDtoTests
{
    private static CartDto CartWithItems() => new()
    {
        Items =
        {
            new CartItemDto { ItemId = 5, Name = "Juneća supa", IsCombo = false, Amount = 2, UnitPrice = 855m },
            new CartItemDto { ItemId = 3, Name = "Ručak", IsCombo = true, Amount = 1, UnitPrice = 720m }
        }
    };

    [Fact]
    public void BasePrice_SumsLineTotals()
    {
        var cart = CartWithItems();
        Assert.Equal(2430m, cart.BasePrice);
        Assert.Equal(2, cart.ItemCount);
    }

    [Fact]
    public void DiscountReduction_WithDiscount_ComputesPercentOfBasePrice()
    {
        var cart = CartWithItems();
        cart.OrderDiscount = new OrderDiscountDto { Id = 2, Name = "Srebrni", DiscountPercent = 10m, PointCost = 300 };
        Assert.Equal(243m, cart.DiscountReduction);
    }

    [Fact]
    public void DiscountReduction_WithoutDiscount_IsZero()
    {
        var cart = CartWithItems();
        Assert.Equal(0m, cart.DiscountReduction);
    }

    [Fact]
    public void RequestedDatetime_SurvivesJsonRoundTrip()
    {
        var cart = CartWithItems();
        cart.RequestedDatetime = new DateTime(2026, 12, 31, 18, 30, 0);

        var json = JsonSerializer.Serialize(cart);
        var restored = JsonSerializer.Deserialize<CartDto>(json);

        Assert.NotNull(restored);
        Assert.Equal(cart.RequestedDatetime, restored.RequestedDatetime);
    }
}
