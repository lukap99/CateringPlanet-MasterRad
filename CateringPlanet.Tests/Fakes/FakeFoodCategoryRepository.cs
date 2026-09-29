using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;

namespace CateringPlanet.Tests.Fakes;

/// <summary>
/// In-memory <see cref="IFoodCategoryRepository"/> fake used by unit tests.
/// </summary>
public class FakeFoodCategoryRepository : IFoodCategoryRepository
{
    public List<FoodCategory> Categories { get; } = new();

    public int NextId { get; set; } = 1;

    public int UpdateCalls { get; private set; }

    public Task<IEnumerable<FoodCategory>> GetAllAsync() => Task.FromResult(Categories.AsEnumerable());

    public Task<FoodCategory?> GetByIdAsync(int id)
        => Task.FromResult(Categories.FirstOrDefault(c => c.Id == id));

    public Task AddAsync(FoodCategory category)
    {
        if (category.Id == 0)
        {
            category.Id = NextId++;
        }

        Categories.Add(category);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(FoodCategory category)
    {
        UpdateCalls++;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Categories.RemoveAll(c => c.Id == id);
        return Task.CompletedTask;
    }
}
