namespace CateringPlanet.Application.DTOs;

/// <summary>
/// Read model for a combo menu bundle with its item list and pre-computed price.
/// </summary>
public class ComboMenuDto
{
    /// <summary>Combo menu id.</summary>
    public int Id { get; set; }

    /// <summary>Display name of the combo menu.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Combo-level discount percent applied to the bundle.</summary>
    public decimal DiscountPercent { get; set; }

    /// <summary>Whether the combo is visible on the public menu.</summary>
    public bool IsVisible { get; set; }

    /// <summary>Base price before the combo discount.</summary>
    public decimal Price { get; set; }

    /// <summary>Final selling price after the combo discount.</summary>
    public decimal SellingPrice { get; set; }

    /// <summary>Stored image file name (without folder), if any.</summary>
    public string? ImageFileName { get; set; }

    /// <summary>Resolved image URL produced by <c>ImageUrlResolver</c>.</summary>
    public string ImageUrl { get; set; } = null!;

    /// <summary>Foods bundled in this combo menu.</summary>
    public ICollection<ComboMenuItemDto> Items { get; set; } = new List<ComboMenuItemDto>();

    /// <summary>Human-readable item summary, e.g. "Ćevapi x 2, Pljeskavica x 1".</summary>
    public string ListOfItems => string.Join(", ", Items.Select(i => $"{i.FoodName} x {i.Amount}"));

    public string PriceString => Price.ToString("N2");

    public string SellingPriceString => SellingPrice.ToString("N2");

    /// <summary>Order quantity bound to the combo item form (default 1).</summary>
    public int Amount { get; set; } = 1;
}
