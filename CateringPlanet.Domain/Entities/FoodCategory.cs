using System;
using System.Collections.Generic;

namespace CateringPlanet.Domain.Entities;

/// <summary>
/// Groups food items into menu categories (e.g. "Supe i čorbe").
/// </summary>
public class FoodCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    /// <summary>
    /// URL-safe lowercase form of <see cref="Name"/> used to build the food image folder
    /// (e.g. "Supe i čorbe" -> "supe-i-corbe").
    /// </summary>
    public string Slug { get; set; } = null!;

    public decimal DiscountPercent { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual ICollection<Food> Foods { get; set; } = new List<Food>();
}
