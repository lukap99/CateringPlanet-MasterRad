using CateringPlanet.Domain.Entities;

namespace CateringPlanet.Domain.Interfaces;

/// <summary>
/// Data access contract for <see cref="Order"/> entities.
/// </summary>
public interface IOrderRepository
{
    /// <summary>Gets all orders (newest first) with full line/lookup details loaded.</summary>
    Task<IEnumerable<Order>> GetAllAsync();

    /// <summary>Gets all orders for a user (newest first) with full details loaded.</summary>
    Task<IEnumerable<Order>> GetByUserAsync(string userId);

    /// <summary>Gets a single order by id with full details, or <c>null</c> if not found.</summary>
    Task<Order?> GetByIdAsync(int id);

    /// <summary>Adds a new order (and its line items via cascade).</summary>
    Task AddAsync(Order order);

    /// <summary>Updates an existing order.</summary>
    Task UpdateAsync(Order order);
}
