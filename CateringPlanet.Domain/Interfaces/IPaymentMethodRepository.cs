using CateringPlanet.Domain.Entities;

namespace CateringPlanet.Domain.Interfaces;

/// <summary>
/// Data access contract for the fixed <see cref="PaymentMethod"/> lookup table.
/// </summary>
public interface IPaymentMethodRepository
{
    /// <summary>Gets all payment methods.</summary>
    Task<IEnumerable<PaymentMethod>> GetAllAsync();

    /// <summary>Gets a payment method by id, or <c>null</c> if not found.</summary>
    Task<PaymentMethod?> GetByIdAsync(int id);
}
