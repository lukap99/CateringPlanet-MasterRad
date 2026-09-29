using System;
using System.Collections.Generic;

namespace CateringPlanet.Domain.Entities;

/// <summary>
/// A fixed discount tier that can be redeemed with loyalty points.
/// </summary>
public class OrderDiscount
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal DiscountPercent { get; set; }

    public int PointCost { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
