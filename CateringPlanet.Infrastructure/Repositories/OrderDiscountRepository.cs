using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;
using CateringPlanet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CateringPlanet.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IOrderDiscountRepository"/>.
/// </summary>
public class OrderDiscountRepository : IOrderDiscountRepository
{
    private readonly CateringPlanetDbContext _context;

    public OrderDiscountRepository(CateringPlanetDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderDiscount>> GetAllAsync()
        => await _context.OrderDiscounts.ToListAsync();

    /// <inheritdoc />
    public async Task<OrderDiscount?> GetByIdAsync(int id)
        => await _context.OrderDiscounts.FirstOrDefaultAsync(d => d.Id == id);
}
