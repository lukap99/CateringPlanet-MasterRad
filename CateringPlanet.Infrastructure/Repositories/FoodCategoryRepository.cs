using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;
using CateringPlanet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CateringPlanet.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IFoodCategoryRepository"/>.
/// </summary>
public class FoodCategoryRepository : IFoodCategoryRepository
{
    private readonly CateringPlanetDbContext _context;

    public FoodCategoryRepository(CateringPlanetDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FoodCategory>> GetAllAsync()
        => await _context.FoodCategories.ToListAsync();

    /// <inheritdoc />
    public async Task<FoodCategory?> GetByIdAsync(int id)
        => await _context.FoodCategories.FirstOrDefaultAsync(c => c.Id == id);

    /// <inheritdoc />
    public async Task AddAsync(FoodCategory category)
    {
        _context.FoodCategories.Add(category);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(FoodCategory category)
    {
        _context.FoodCategories.Update(category);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        var category = await _context.FoodCategories.FindAsync(id);
        if (category is not null)
        {
            _context.FoodCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
