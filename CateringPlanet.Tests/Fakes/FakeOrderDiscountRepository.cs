using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;

namespace CateringPlanet.Tests.Fakes;

/// <summary>
/// In-memory <see cref="IOrderDiscountRepository"/> fake used by unit tests.
/// </summary>
public class FakeOrderDiscountRepository : IOrderDiscountRepository
{
    public List<OrderDiscount> Discounts { get; } = new();

    public Task<IEnumerable<OrderDiscount>> GetAllAsync() => Task.FromResult(Discounts.AsEnumerable());

    public Task<OrderDiscount?> GetByIdAsync(int id)
        => Task.FromResult(Discounts.FirstOrDefault(d => d.Id == id));
}
