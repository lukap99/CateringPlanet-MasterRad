namespace CateringPlanet.Application.DTOs;

/// <summary>
/// The shopping cart state. Structural totals are computed from <see cref="Items"/>;
/// <see cref="BuyingPrice"/> and <see cref="PointsDifference"/> are set by the cart service.
/// </summary>
public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();

    public int ItemCount => Items.Count;

    /// <summary>Sum of all line totals before the order discount and delivery.</summary>
    public decimal BasePrice => Items.Sum(i => i.LineTotal);

    /// <summary>Final total after the order discount and delivery price.</summary>
    public decimal BuyingPrice { get; set; }

    /// <summary>Points earned (positive) or spent (negative) for this order.</summary>
    public int PointsDifference { get; set; }

    public string? ContactName { get; set; }

    public string? ContactPhone { get; set; }

    public string? DeliveryLocation { get; set; }

    public DeliveryMethodDto? DeliveryMethod { get; set; }

    public PaymentMethodDto? PaymentMethod { get; set; }

    public OrderDiscountDto? OrderDiscount { get; set; }

    /// <summary>Monetary amount reduced by the order discount (before delivery).</summary>
    public decimal DiscountReduction => BasePrice * (OrderDiscount?.DiscountPercent ?? 0m) / 100m;

    public string BasePriceString => BasePrice.ToString("N2");

    public string BuyingPriceString => BuyingPrice.ToString("N2");

    public string DiscountReductionString => DiscountReduction.ToString("N2");
}
