using CateringPlanet.Domain.Entities;

namespace CateringPlanet.Domain.Interfaces;

/// <summary>
/// Data access contract for <see cref="Food"/> items.
/// </summary>
public interface IFoodRepository
{
    /// <summary>Gets all food items with their category loaded.</summary>
    Task<IEnumerable<Food>> GetAllAsync();

    /// <summary>Gets all visible food items with their category loaded.</summary>
    Task<IEnumerable<Food>> GetVisibleAsync();

    /// <summary>Gets all food items in a given category.</summary>
    Task<IEnumerable<Food>> GetByCategoryAsync(int categoryId);

    /// <summary>Gets all visible food items in a given category.</summary>
    Task<IEnumerable<Food>> GetVisibleByCategoryAsync(int categoryId);

    /// <summary>Gets a single food item by id, or <c>null</c> if not found.</summary>
    Task<Food?> GetByIdAsync(int id);

    /// <summary>Adds a new food item.</summary>
    Task AddAsync(Food food);

    /// <summary>Updates an existing food item.</summary>
    Task UpdateAsync(Food food);

    /// <summary>Deletes a food item by id (no-op if not found).</summary>
    Task DeleteAsync(int id);
}
