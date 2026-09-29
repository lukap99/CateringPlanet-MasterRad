namespace CateringPlanet.Application.DTOs;

/// <summary>
/// A single line (food or combo) inside an order.
/// </summary>
public class OrderLineDto
{
    /// <summary>Name of the food or combo menu.</summary>
    public string Name { get; set; } = null!;

    /// <summary>True when the line is a combo menu; false for a single food.</summary>
    public bool IsCombo { get; set; }

    /// <summary>Number of portions.</summary>
    public int Amount { get; set; }

    /// <summary>Unit price before the line discount.</summary>
    public decimal BasePrice { get; set; }

    /// <summary>Discount percent applied to the line.</summary>
    public decimal DiscountPercent { get; set; }

    /// <summary>Line total price (already discounted).</summary>
    public decimal Price { get; set; }

    public string PriceString => Price.ToString("N2");
}
