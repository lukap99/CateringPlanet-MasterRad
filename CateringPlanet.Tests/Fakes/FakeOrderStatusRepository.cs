using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;

namespace CateringPlanet.Tests.Fakes;

/// <summary>
/// In-memory <see cref="IOrderStatusRepository"/> fake used by unit tests.
/// </summary>
public class FakeOrderStatusRepository : IOrderStatusRepository
{
    public List<OrderStatus> Statuses { get; } = new();

    public Task<IEnumerable<OrderStatus>> GetAllAsync()
        => Task.FromResult(Statuses.OrderBy(s => s.Id).AsEnumerable());

    public Task<OrderStatus?> GetByIdAsync(int id)
        => Task.FromResult(Statuses.FirstOrDefault(s => s.Id == id));
}
