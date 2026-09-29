using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;
using CateringPlanet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CateringPlanet.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IPointTransactionRepository"/>.
/// </summary>
public class PointTransactionRepository : IPointTransactionRepository
{
    private readonly CateringPlanetDbContext _context;

    public PointTransactionRepository(CateringPlanetDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PointTransaction>> GetByUserAsync(string userId)
        => await _context.PointTransactions.Where(p => p.UserId == userId).OrderByDescending(p => p.CreatedAt).ToListAsync();

    /// <inheritdoc />
    public async Task<PointTransaction?> GetByOrderAsync(int orderId)
        => await _context.PointTransactions.FirstOrDefaultAsync(p => p.OrderId == orderId);

    /// <inheritdoc />
    public async Task AddAsync(PointTransaction transaction)
    {
        _context.PointTransactions.Add(transaction);
        await _context.SaveChangesAsync();
    }
}
