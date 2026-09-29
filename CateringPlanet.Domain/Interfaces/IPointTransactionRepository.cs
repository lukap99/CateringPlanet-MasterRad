using CateringPlanet.Domain.Entities;

namespace CateringPlanet.Domain.Interfaces;

/// <summary>
/// Data access contract for <see cref="PointTransaction"/> loyalty-points movements.
/// </summary>
public interface IPointTransactionRepository
{
    /// <summary>Gets all point transactions for a user (newest first).</summary>
    Task<IEnumerable<PointTransaction>> GetByUserAsync(string userId);

    /// <summary>Gets the point transaction recorded for an order, or <c>null</c>.</summary>
    Task<PointTransaction?> GetByOrderAsync(int orderId);

    /// <summary>Adds a new point transaction.</summary>
    Task AddAsync(PointTransaction transaction);
}
