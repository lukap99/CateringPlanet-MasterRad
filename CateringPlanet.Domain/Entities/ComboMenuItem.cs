using System;
using System.Collections.Generic;

namespace CateringPlanet.Domain.Entities;

/// <summary>
/// Links a food item to a combo menu with a given quantity.
/// </summary>
public class ComboMenuItem
{
    public int Id { get; set; }

    public int FoodId { get; set; }

    public int ComboMenuId { get; set; }

    public int Amount { get; set; }

    public virtual ComboMenu ComboMenu { get; set; } = null!;

    public virtual Food Food { get; set; } = null!;
}
