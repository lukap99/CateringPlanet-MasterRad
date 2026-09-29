using CateringPlanet.Domain.Entities;

namespace CateringPlanet.Domain.Interfaces;

/// <summary>
/// Data access contract for <see cref="ComboMenu"/> entities.
/// </summary>
public interface IComboMenuRepository
{
    /// <summary>Gets all combo menus with their items and food names loaded.</summary>
    Task<IEnumerable<ComboMenu>> GetAllAsync();

    /// <summary>Gets all visible combo menus with items loaded.</summary>
    Task<IEnumerable<ComboMenu>> GetVisibleAsync();

    /// <summary>Gets a single combo menu by id, or <c>null</c> if not found.</summary>
    Task<ComboMenu?> GetByIdAsync(int id);

    /// <summary>Adds a new combo menu.</summary>
    Task AddAsync(ComboMenu combo);

    /// <summary>Updates an existing combo menu.</summary>
    Task UpdateAsync(ComboMenu combo);

    /// <summary>Deletes a combo menu by id (no-op if not found).</summary>
    Task DeleteAsync(int id);

    /// <summary>Adds a food item to a combo menu.</summary>
    Task AddItemAsync(ComboMenuItem item);

    /// <summary>Updates the amount of a combo menu item.</summary>
    Task UpdateItemAmountAsync(int comboMenuItemId, int amount);

    /// <summary>Removes a combo menu item by its join-entity id.</summary>
    Task RemoveItemAsync(int comboMenuItemId);
}
