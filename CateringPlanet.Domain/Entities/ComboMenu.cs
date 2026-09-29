using System;
using System.Collections.Generic;

namespace CateringPlanet.Domain.Entities;

/// <summary>
/// A fixed-price bundle of food items offered as a combo menu.
/// </summary>
public class ComboMenu
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal DiscountPercent { get; set; }

    public bool IsVisible { get; set; }

    public decimal Price { get; set; }

    /// <summary>
    /// Stores only the image filename, never a full path.
    /// The folder path is composed at runtime by <c>ImageUrlResolver</c>.
    /// </summary>
    public string? ImageFileName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual ICollection<ComboMenuItem> ComboMenuItems { get; set; } = new List<ComboMenuItem>();

    public virtual ICollection<OrderComboItem> OrderComboItems { get; set; } = new List<OrderComboItem>();
}
