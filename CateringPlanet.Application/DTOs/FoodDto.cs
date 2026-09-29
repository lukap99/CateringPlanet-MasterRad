namespace CateringPlanet.Application.DTOs;

/// <summary>
/// Read model for a single food item on the public menu.
/// Carries pre-computed selling price and resolved image URL for direct view binding.
/// </summary>
public class FoodDto
{
    // test comment asdfg
    /// <summary>Food id.</summary>
    public int Id { get; set; }

    /// <summary>Display name of the food.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Unit of measurement (e.g. "g", "kom"), if defined.</summary>
    public string? UnitOfMeasurement { get; set; }

    /// <summary>Category id the food belongs to.</summary>
    public int FoodCategoryId { get; set; }

    /// <summary>Display name of the food's category.</summary>
    public string FoodCategoryName { get; set; } = null!;

    /// <summary>URL-friendly identifier of the food's category.</summary>
    public string FoodCategorySlug { get; set; } = null!;

    /// <summary>Whether the food is visible on the public menu.</summary>
    public bool IsVisible { get; set; }

    /// <summary>Ingredients description shown on the details page.</summary>
    public string? Ingredients { get; set; }

    /// <summary>Base price before any discounts (item or category).</summary>
    public decimal Price { get; set; }

    /// <summary>Item-level discount percent applied to <see cref="Price"/>.</summary>
    public decimal DiscountPercent { get; set; }

    /// <summary>Category-level discount percent applied on top of the item discount.</summary>
    public decimal CategoryDiscountPercent { get; set; }

    /// <summary>Final selling price per unit after both discounts.</summary>
    public decimal SellingPrice { get; set; }

    /// <summary>Stored image file name (without folder), if any.</summary>
    public string? ImageFileName { get; set; }

    /// <summary>Resolved image URL produced by <c>ImageUrlResolver</c>.</summary>
    public string ImageUrl { get; set; } = null!;

    public string PriceString => Price.ToString("N2");

    public string SellingPriceString => SellingPrice.ToString("N2");

    /// <summary>Order quantity bound to the menu item form (default 1).</summary>
    public int Amount { get; set; } = 1;
}
