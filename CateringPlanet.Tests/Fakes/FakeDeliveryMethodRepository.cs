using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;

namespace CateringPlanet.Tests.Fakes;

/// <summary>
/// In-memory <see cref="IDeliveryMethodRepository"/> fake used by unit tests.
/// </summary>
public class FakeDeliveryMethodRepository : IDeliveryMethodRepository
{
    public List<DeliveryMethod> Methods { get; } = new();

    public Task<IEnumerable<DeliveryMethod>> GetAllAsync() => Task.FromResult(Methods.AsEnumerable());

    public Task<DeliveryMethod?> GetByIdAsync(int id)
        => Task.FromResult(Methods.FirstOrDefault(m => m.Id == id));
}
