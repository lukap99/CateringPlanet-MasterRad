using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;

namespace CateringPlanet.Tests.Fakes;

/// <summary>
/// In-memory <see cref="IOrderRepository"/> fake used by unit tests.
/// </summary>
public class FakeOrderRepository : IOrderRepository
{
    public List<Order> Orders { get; } = new();

    public int NextId { get; set; } = 1;

    public Task<IEnumerable<Order>> GetAllAsync()
        => Task.FromResult(Orders.OrderByDescending(o => o.DateCreated).AsEnumerable());

    public Task<IEnumerable<Order>> GetByUserAsync(string userId)
        => Task.FromResult(Orders.Where(o => o.UserId == userId).OrderByDescending(o => o.DateCreated).AsEnumerable());

    public Task<Order?> GetByIdAsync(int id)
        => Task.FromResult(Orders.FirstOrDefault(o => o.Id == id));

    public Task AddAsync(Order order)
    {
        if (order.Id == 0)
        {
            order.Id = NextId++;
        }

        Orders.Add(order);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order) => Task.CompletedTask;
}
