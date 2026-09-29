using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;
using CateringPlanet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CateringPlanet.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IComboMenuRepository"/>.
/// </summary>
public class ComboMenuRepository : IComboMenuRepository
{
    private readonly CateringPlanetDbContext _context;

    public ComboMenuRepository(CateringPlanetDbContext context)
    {
        _context = context;
    }

    // Loads the nested food names so DTO mapping can build the item list without N+1 queries.
    private IQueryable<ComboMenu> WithItems()
        => _context.ComboMenus.Include(c => c.ComboMenuItems).ThenInclude(i => i.Food);

    /// <inheritdoc />
    public async Task<IEnumerable<ComboMenu>> GetAllAsync()
        => await WithItems().ToListAsync();

    /// <inheritdoc />
    public async Task<IEnumerable<ComboMenu>> GetVisibleAsync()
        => await WithItems().Where(c => c.IsVisible).ToListAsync();

    /// <inheritdoc />
    public async Task<ComboMenu?> GetByIdAsync(int id)
        => await WithItems().FirstOrDefaultAsync(c => c.Id == id);

    /// <inheritdoc />
    public async Task AddAsync(ComboMenu combo)
    {
        _context.ComboMenus.Add(combo);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(ComboMenu combo)
    {
        _context.ComboMenus.Update(combo);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        var combo = await _context.ComboMenus.FindAsync(id);
        if (combo is not null)
        {
            _context.ComboMenus.Remove(combo);
            await _context.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task AddItemAsync(ComboMenuItem item)
    {
        _context.ComboMenuItems.Add(item);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateItemAmountAsync(int comboMenuItemId, int amount)
    {
        var item = await _context.ComboMenuItems.FindAsync(comboMenuItemId);
        if (item is not null)
        {
            item.Amount = amount;
            await _context.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task RemoveItemAsync(int comboMenuItemId)
    {
        var item = await _context.ComboMenuItems.FindAsync(comboMenuItemId);
        if (item is not null)
        {
            _context.ComboMenuItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
