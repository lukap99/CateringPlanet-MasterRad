using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;
using CateringPlanet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CateringPlanet.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IFoodRepository"/>.
/// </summary>
public class FoodRepository : IFoodRepository
{
    private readonly CateringPlanetDbContext _context;

    public FoodRepository(CateringPlanetDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Food>> GetAllAsync()
        => await _context.Foods.Include(f => f.FoodCategory).ToListAsync();

    /// <inheritdoc />
    public async Task<IEnumerable<Food>> GetVisibleAsync()
        => await _context.Foods.Include(f => f.FoodCategory).Where(f => f.IsVisible).ToListAsync();

    /// <inheritdoc />
    public async Task<IEnumerable<Food>> GetByCategoryAsync(int categoryId)
        => await _context.Foods.Include(f => f.FoodCategory).Where(f => f.FoodCategoryId == categoryId).ToListAsync();

    /// <inheritdoc />
    public async Task<IEnumerable<Food>> GetVisibleByCategoryAsync(int categoryId)
        => await _context.Foods.Include(f => f.FoodCategory).Where(f => f.FoodCategoryId == categoryId && f.IsVisible).ToListAsync();

    /// <inheritdoc />
    public async Task<Food?> GetByIdAsync(int id)
        => await _context.Foods.Include(f => f.FoodCategory).FirstOrDefaultAsync(f => f.Id == id);

    /// <inheritdoc />
    public async Task AddAsync(Food food)
    {
        _context.Foods.Add(food);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Food food)
    {
        _context.Foods.Update(food);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        var food = await _context.Foods.FindAsync(id);
        if (food is not null)
        {
            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();
        }
    }
}
