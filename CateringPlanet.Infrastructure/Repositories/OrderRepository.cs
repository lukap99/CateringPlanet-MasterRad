using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;
using CateringPlanet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CateringPlanet.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IOrderRepository"/>.
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly CateringPlanetDbContext _context;

    public OrderRepository(CateringPlanetDbContext context)
    {
        _context = context;
    }

    // Loads all lookup and line-item navigations needed to map an order to OrderDto.
    private IQueryable<Order> WithDetails()
        => _context.Orders
            .Include(o => o.OrderFoodItems).ThenInclude(i => i.Food)
            .Include(o => o.OrderComboItems).ThenInclude(i => i.ComboMenu)
            .Include(o => o.Status)
            .Include(o => o.PaymentStatus)
            .Include(o => o.PaymentMethod)
            .Include(o => o.Delivery)
            .Include(o => o.Discount);

    /// <inheritdoc />
    public async Task<IEnumerable<Order>> GetAllAsync()
        => await WithDetails().OrderByDescending(o => o.DateCreated).ToListAsync();

    /// <inheritdoc />
    public async Task<IEnumerable<Order>> GetByUserAsync(string userId)
        => await WithDetails().Where(o => o.UserId == userId).OrderByDescending(o => o.DateCreated).ToListAsync();

    /// <inheritdoc />
    public async Task<Order?> GetByIdAsync(int id)
        => await WithDetails().FirstOrDefaultAsync(o => o.Id == id);

    /// <inheritdoc />
    public async Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}
