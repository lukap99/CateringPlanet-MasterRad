using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;
using CateringPlanet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CateringPlanet.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IOrderStatusRepository"/>.
/// </summary>
public class OrderStatusRepository : IOrderStatusRepository
{
    private readonly CateringPlanetDbContext _context;

    public OrderStatusRepository(CateringPlanetDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderStatus>> GetAllAsync()
        => await _context.OrderStatuses.OrderBy(s => s.Id).ToListAsync();

    /// <inheritdoc />
    public async Task<OrderStatus?> GetByIdAsync(int id)
        => await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Id == id);
}
