using System;
using System.Collections.Generic;

namespace CateringPlanet.Domain.Entities;

/// <summary>
/// Fixed lookup of payment methods (e.g. "Kartica", "Gotovina").
/// </summary>
public class PaymentMethod
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
