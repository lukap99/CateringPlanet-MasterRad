using System;
using System.Collections.Generic;

namespace CateringPlanet.Domain.Entities;

/// <summary>
/// Represents a single food item on the catering menu.
/// </summary>
public class Food
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? UnitOfMeasurement { get; set; }

    public int FoodCategoryId { get; set; }

    public bool IsVisible { get; set; }

    public string? Ingredients { get; set; }

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }

    /// <summary>
    /// Stores only the image filename (e.g. "juneca_supa.jpg"), never a full path.
    /// The folder path is composed at runtime by <c>ImageUrlResolver</c>.
    /// </summary>
    public string? ImageFileName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual FoodCategory FoodCategory { get; set; } = null!;

    public virtual ICollection<ComboMenuItem> ComboMenuItems { get; set; } = new List<ComboMenuItem>();

    public virtual ICollection<OrderFoodItem> OrderFoodItems { get; set; } = new List<OrderFoodItem>();
}
