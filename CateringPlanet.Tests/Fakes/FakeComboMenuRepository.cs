using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;

namespace CateringPlanet.Tests.Fakes;

/// <summary>
/// In-memory <see cref="IComboMenuRepository"/> fake used by unit tests.
/// </summary>
public class FakeComboMenuRepository : IComboMenuRepository
{
    public List<ComboMenu> Combos { get; } = new();

    public List<ComboMenuItem> Items { get; } = new();

    public int NextId { get; set; } = 1;

    public int UpdateCalls { get; private set; }

    public Task<IEnumerable<ComboMenu>> GetAllAsync() => Task.FromResult(Combos.AsEnumerable());

    public Task<IEnumerable<ComboMenu>> GetVisibleAsync()
        => Task.FromResult(Combos.Where(c => c.IsVisible));

    public Task<ComboMenu?> GetByIdAsync(int id)
        => Task.FromResult(Combos.FirstOrDefault(c => c.Id == id));

    public Task AddAsync(ComboMenu combo)
    {
        if (combo.Id == 0)
        {
            combo.Id = NextId++;
        }

        Combos.Add(combo);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ComboMenu combo)
    {
        UpdateCalls++;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Combos.RemoveAll(c => c.Id == id);
        return Task.CompletedTask;
    }

    public Task AddItemAsync(ComboMenuItem item)
    {
        Items.Add(item);
        return Task.CompletedTask;
    }

    public Task UpdateItemAmountAsync(int comboMenuItemId, int amount)
    {
        var item = Items.FirstOrDefault(i => i.Id == comboMenuItemId);
        if (item is not null)
        {
            item.Amount = amount;
        }

        return Task.CompletedTask;
    }

    public Task RemoveItemAsync(int comboMenuItemId)
    {
        Items.RemoveAll(i => i.Id == comboMenuItemId);
        return Task.CompletedTask;
    }
}
