namespace CateringPlanet.Application.DTOs;

/// <summary>
/// Read model for a food category shown in menu sidebars and filters.
/// </summary>
public class FoodCategoryDto
{
    /// <summary>Food category id.</summary>
    public int Id { get; set; }

    /// <summary>Display name of the category.</summary>
    public string Name { get; set; } = null!;

    /// <summary>URL-friendly identifier used in routes.</summary>
    public string Slug { get; set; } = null!;

    /// <summary>Category-level discount percent applied to its foods.</summary>
    public decimal DiscountPercent { get; set; }
}
