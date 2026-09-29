using System;
using System.Collections.Generic;

namespace CateringPlanet.Domain.Entities;

/// <summary>
/// A food line item within an order, capturing price at the time of purchase.
/// </summary>
public class OrderFoodItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int FoodId { get; set; }

    public decimal BasePrice { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal Price { get; set; }

    public int Amount { get; set; }

    public virtual Food Food { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
