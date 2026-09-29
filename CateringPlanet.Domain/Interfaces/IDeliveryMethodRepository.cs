using CateringPlanet.Domain.Entities;

namespace CateringPlanet.Domain.Interfaces;

/// <summary>
/// Data access contract for the fixed <see cref="DeliveryMethod"/> lookup table.
/// </summary>
public interface IDeliveryMethodRepository
{
    /// <summary>Gets all delivery methods.</summary>
    Task<IEnumerable<DeliveryMethod>> GetAllAsync();

    /// <summary>Gets a delivery method by id, or <c>null</c> if not found.</summary>
    Task<DeliveryMethod?> GetByIdAsync(int id);
}
