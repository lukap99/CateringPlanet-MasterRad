using CateringPlanet.Domain.Entities;

namespace CateringPlanet.Domain.Interfaces;

/// <summary>
/// Data access contract for the fixed <see cref="OrderDiscount"/> lookup table.
/// </summary>
public interface IOrderDiscountRepository
{
    /// <summary>Gets all discount tiers.</summary>
    Task<IEnumerable<OrderDiscount>> GetAllAsync();

    /// <summary>Gets a discount tier by id, or <c>null</c> if not found.</summary>
    Task<OrderDiscount?> GetByIdAsync(int id);
}
