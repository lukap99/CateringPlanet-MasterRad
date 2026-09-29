using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;
using CateringPlanet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CateringPlanet.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IPaymentMethodRepository"/>.
/// </summary>
public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly CateringPlanetDbContext _context;

    public PaymentMethodRepository(CateringPlanetDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PaymentMethod>> GetAllAsync()
        => await _context.PaymentMethods.ToListAsync();

    /// <inheritdoc />
    public async Task<PaymentMethod?> GetByIdAsync(int id)
        => await _context.PaymentMethods.FirstOrDefaultAsync(p => p.Id == id);
}
