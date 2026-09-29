using CateringPlanet.Domain.Entities;

namespace CateringPlanet.Domain.Interfaces;

/// <summary>
/// Data access contract for <see cref="FoodCategory"/> entities.
/// </summary>
public interface IFoodCategoryRepository
{
    /// <summary>Gets all food categories with their foods loaded (for counts).</summary>
    Task<IEnumerable<FoodCategory>> GetAllAsync();

    /// <summary>Gets a single category by id, or <c>null</c> if not found.</summary>
    Task<FoodCategory?> GetByIdAsync(int id);

    /// <summary>Adds a new category.</summary>
    Task AddAsync(FoodCategory category);

    /// <summary>Updates an existing category.</summary>
    Task UpdateAsync(FoodCategory category);

    /// <summary>Deletes a category by id (no-op if not found).</summary>
    Task DeleteAsync(int id);
}
