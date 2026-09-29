using CateringPlanet.Application.Helpers;

namespace CateringPlanet.Tests;

/// <summary>
/// Unit tests for <see cref="PricingService"/> — the centralized price,
/// discount and loyalty-points rules (matches thesis Listing 4.1).
/// </summary>
public class PricingServiceTests
{
    [Fact]
    public void FoodSellingPrice_PrimenjujePopustJelaIKategorijeMultiplikativno()
    {
        var result = PricingService.FoodSellingPrice(1000m, 10m, 5m);
        Assert.Equal(855m, result);
    }

    [Theory]
    [InlineData(1000, 10, 900)]
    [InlineData(800, 0, 800)]
    [InlineData(500, 100, 0)]
    public void ComboSellingPrice_PrimenjujePopust(decimal basePrice, decimal discount, decimal expected)
    {
        var result = PricingService.ComboSellingPrice(basePrice, discount);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void OrderTotal_PrimenjujePopustIDodajeDostavu()
    {
        var result = PricingService.OrderTotal(1575m, 10m, 300m);
        Assert.Equal(1717.5m, result);
    }

    [Theory]
    [InlineData(1575, 3)]
    [InlineData(1000, 2)]
    [InlineData(500, 1)]
    public void PointsEarned_JedanPoenNa500Dinara(decimal amount, int expected)
    {
        var result = PricingService.PointsEarned(amount);
        Assert.Equal(expected, result);
    }
}
