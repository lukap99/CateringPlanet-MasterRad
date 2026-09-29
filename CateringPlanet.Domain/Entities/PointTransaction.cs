using System;
using System.Collections.Generic;

namespace CateringPlanet.Domain.Entities;

/// <summary>
/// A single loyalty-points movement (earning or spending) for a user.
/// </summary>
public class PointTransaction
{
    public int Id { get; set; }

    /// <summary>
    /// Identity user id (GUID string) — references the <c>AspNetUsers</c> table.
    /// </summary>
    public string UserId { get; set; } = null!;

    public int? OrderId { get; set; }

    public int Amount { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Order? Order { get; set; }
}
