using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;

namespace CateringPlanet.Tests.Fakes;

/// <summary>
/// In-memory <see cref="IFoodRepository"/> fake used by unit tests.
/// </summary>
public class FakeFoodRepository : IFoodRepository
{
    public List<Food> Foods { get; } = new();

    public int NextId { get; set; } = 1;

    public int VisibleCalled { get; private set; }

    public int VisibleByCategoryCalled { get; private set; }

    public int UpdateCalls { get; private set; }

    public Task<IEnumerable<Food>> GetAllAsync() => Task.FromResult(Foods.AsEnumerable());

    public Task<IEnumerable<Food>> GetVisibleAsync()
    {
        VisibleCalled++;
        return Task.FromResult(Foods.Where(f => f.IsVisible));
    }

    public Task<IEnumerable<Food>> GetByCategoryAsync(int categoryId)
        => Task.FromResult(Foods.Where(f => f.FoodCategoryId == categoryId));

    public Task<IEnumerable<Food>> GetVisibleByCategoryAsync(int categoryId)
    {
        VisibleByCategoryCalled++;
        return Task.FromResult(Foods.Where(f => f.FoodCategoryId == categoryId && f.IsVisible));
    }

    public Task<Food?> GetByIdAsync(int id)
        => Task.FromResult(Foods.FirstOrDefault(f => f.Id == id));

    public Task AddAsync(Food food)
    {
        if (food.Id == 0)
        {
            food.Id = NextId++;
        }

        Foods.Add(food);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Food food)
    {
        UpdateCalls++;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Foods.RemoveAll(f => f.Id == id);
        return Task.CompletedTask;
    }
}
