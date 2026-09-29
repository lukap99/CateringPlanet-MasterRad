using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;

namespace CateringPlanet.Tests.Fakes;

/// <summary>
/// In-memory <see cref="IPointTransactionRepository"/> fake used by unit tests.
/// </summary>
public class FakePointTransactionRepository : IPointTransactionRepository
{
    public List<PointTransaction> Transactions { get; } = new();

    public Task<IEnumerable<PointTransaction>> GetByUserAsync(string userId)
        => Task.FromResult(Transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.CreatedAt).AsEnumerable());

    public Task<PointTransaction?> GetByOrderAsync(int orderId)
        => Task.FromResult(Transactions.FirstOrDefault(t => t.OrderId == orderId));

    public Task AddAsync(PointTransaction transaction)
    {
        Transactions.Add(transaction);
        return Task.CompletedTask;
    }
}
