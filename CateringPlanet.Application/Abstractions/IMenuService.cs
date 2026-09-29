using CateringPlanet.Application.DTOs;

namespace CateringPlanet.Application.Abstractions;

/// <summary>
/// Provides read access to the public menu (foods, combos, categories) as DTOs.
/// </summary>
public interface IMenuService
{
    /// <summary>Gets visible foods, optionally filtered to a category.</summary>
    Task<IEnumerable<FoodDto>> GetVisibleFoodsAsync(int? categoryId);

    /// <summary>Gets all foods in a category (visible or not).</summary>
    Task<IEnumerable<FoodDto>> GetFoodsByCategoryAsync(int categoryId);

    /// <summary>Gets all visible combo menus.</summary>
    Task<IEnumerable<ComboMenuDto>> GetVisibleCombosAsync();

    /// <summary>Gets all food categories.</summary>
    Task<IEnumerable<FoodCategoryDto>> GetCategoriesAsync();

    /// <summary>Gets a single food item, or <c>null</c> if not found.</summary>
    Task<FoodDto?> GetFoodAsync(int id);

    /// <summary>Gets a single combo menu, or <c>null</c> if not found.</summary>
    Task<ComboMenuDto?> GetComboAsync(int id);

    /// <summary>Gets all foods (including hidden) for the admin menu.</summary>
    Task<IEnumerable<FoodDto>> GetAllFoodsAsync();

    /// <summary>Gets all combos (including hidden) for the admin menu.</summary>
    Task<IEnumerable<ComboMenuDto>> GetAllCombosAsync();

    /// <summary>Creates a food item and returns its new id.</summary>
    Task<int> CreateFoodAsync(FoodDto food);

    /// <summary>Updates an existing food item.</summary>
    Task UpdateFoodAsync(FoodDto food);

    /// <summary>Deletes a food item by id.</summary>
    Task DeleteFoodAsync(int id);

    /// <summary>Creates a food category and returns its new id.</summary>
    Task<int> CreateCategoryAsync(FoodCategoryDto category);

    /// <summary>Updates a category's discount percent.</summary>
    Task UpdateCategoryDiscountAsync(int categoryId, decimal discountPercent);

    /// <summary>Creates a combo menu and returns its new id.</summary>
    Task<int> CreateComboAsync(ComboMenuDto combo);

    /// <summary>Updates an existing combo menu.</summary>
    Task UpdateComboAsync(ComboMenuDto combo);

    /// <summary>Deletes a combo menu by id.</summary>
    Task DeleteComboAsync(int id);

    /// <summary>Adds a food item to a combo menu.</summary>
    Task AddComboMenuItemAsync(int comboMenuId, int foodId, int amount);

    /// <summary>Updates the amount of a combo menu item.</summary>
    Task UpdateComboMenuItemAmountAsync(int comboMenuId, int comboMenuItemId, int amount);

    /// <summary>Removes a food item from a combo menu.</summary>
    Task DeleteComboMenuItemAsync(int comboMenuId, int comboMenuItemId);
}
