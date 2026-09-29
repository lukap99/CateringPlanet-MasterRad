namespace CateringPlanet.Application.DTOs;

/// <summary>
/// A single line in the shopping cart (either a food or a combo menu).
/// </summary>
public class CartItemDto
{
    /// <summary>Food id or combo menu id, depending on <see cref="IsCombo"/>.</summary>
    public int ItemId { get; set; }

    public string Name { get; set; } = null!;

    public bool IsCombo { get; set; }

    public int Amount { get; set; }

    /// <summary>Selling price per unit (already discounted).</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Line total = <see cref="UnitPrice"/> * <see cref="Amount"/>.</summary>
    public decimal LineTotal => UnitPrice * Amount;

    public string? ImageFileName { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string LineTotalString => LineTotal.ToString("N2");
}
