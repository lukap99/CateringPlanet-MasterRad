using CateringPlanet.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CateringPlanet.Infrastructure.Data;

/// <summary>
/// Application user, extending ASP.NET Core Identity with catering-specific fields.
/// Replaces the legacy <c>[dbo].[user]</c> table.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>User's first name (maps to <c>first_name</c>).</summary>
    public string FirstName { get; set; } = null!;

    /// <summary>User's last name (maps to <c>last_name</c>).</summary>
    public string LastName { get; set; } = null!;

    /// <summary>Loyalty points balance (maps to <c>points</c>).</summary>
    public int Points { get; set; }

    /// <summary>Delivery address prefill for checkout, optional.</summary>
    public string? Address { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();
}
