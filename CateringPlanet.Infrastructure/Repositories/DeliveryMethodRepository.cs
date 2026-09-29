using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;
using CateringPlanet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CateringPlanet.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IDeliveryMethodRepository"/>.
/// </summary>
public class DeliveryMethodRepository : IDeliveryMethodRepository
{
    private readonly CateringPlanetDbContext _context;

    public DeliveryMethodRepository(CateringPlanetDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DeliveryMethod>> GetAllAsync()
        => await _context.DeliveryMethods.ToListAsync();

    /// <inheritdoc />
    public async Task<DeliveryMethod?> GetByIdAsync(int id)
        => await _context.DeliveryMethods.FirstOrDefaultAsync(d => d.Id == id);
}
