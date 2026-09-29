using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;

namespace CateringPlanet.Tests.Fakes;

/// <summary>
/// In-memory <see cref="IPaymentMethodRepository"/> fake used by unit tests.
/// </summary>
public class FakePaymentMethodRepository : IPaymentMethodRepository
{
    public List<PaymentMethod> Methods { get; } = new();

    public Task<IEnumerable<PaymentMethod>> GetAllAsync() => Task.FromResult(Methods.AsEnumerable());

    public Task<PaymentMethod?> GetByIdAsync(int id)
        => Task.FromResult(Methods.FirstOrDefault(m => m.Id == id));
}
