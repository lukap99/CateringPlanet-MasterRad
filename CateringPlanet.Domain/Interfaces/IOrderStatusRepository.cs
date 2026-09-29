using CateringPlanet.Domain.Entities;

namespace CateringPlanet.Domain.Interfaces;

/// <summary>
/// Data access contract for <see cref="OrderStatus"/> lookup rows.
/// </summary>
public interface IOrderStatusRepository
{
    /// <summary>Gets all order statuses ordered by id.</summary>
    Task<IEnumerable<OrderStatus>> GetAllAsync();

    /// <summary>Gets a status by id, or <c>null</c> if not found.</summary>
    Task<OrderStatus?> GetByIdAsync(int id);
}
