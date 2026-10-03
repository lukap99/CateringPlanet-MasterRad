using CateringPlanet.Application.DTOs;
using CateringPlanet.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CateringPlanet.Web.Controllers;

/// <summary>
/// Admin management of foods, categories, and combo menus (Manager/Admin only).
/// </summary>
[Authorize(Roles = "Manager,Admin")]
public class AdminMenuController : Controller
{
    private readonly IMenuService _menuService;
    private readonly IImageStorageService _imageStorage;

    public AdminMenuController(IMenuService menuService, IImageStorageService imageStorage)
    {
        _menuService = menuService;
        _imageStorage = imageStorage;
    }

    /// <summary>Lists foods (optionally filtered by category) plus combo menus.</summary>
    public async Task<IActionResult> Index(int? id, string? searchString)
    {
        var selectedCategoryId = id is > 0 ? id.Value : 0;

        IEnumerable<FoodDto> foods;
        IEnumerable<ComboMenuDto> combos;

        if (selectedCategoryId > 0)
        {
            foods = await _menuService.GetFoodsByCategoryAsync(selectedCategoryId);
            combos = Array.Empty<ComboMenuDto>();
        }
        else
        {
            foods = await _menuService.GetAllFoodsAsync();
            combos = await _menuService.GetAllCombosAsync();
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            foods = foods.Where(f => f.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            ViewData["CurrentFilter"] = searchString;
        }

        ViewData["SelectedCategoryId"] = selectedCategoryId;
        ViewData["IsComboMenuView"] = false;
        ViewData["Categories"] = await _menuService.GetCategoriesAsync();
        ViewData["ComboItems"] = combos;

        return View(foods.ToList());
    }

    /// <summary>Lists combo menus only.</summary>
    public async Task<IActionResult> ComboMenu()
    {
        ViewData["SelectedCategoryId"] = -1;
        ViewData["IsComboMenuView"] = true;
        ViewData["Categories"] = await _menuService.GetCategoriesAsync();
        ViewData["ComboItems"] = await _menuService.GetAllCombosAsync();

        return View("Index", new List<FoodDto>());
    }

    // ---- Food CRUD ----

    /// <summary>Shows the create-food form.</summary>
    public async Task<IActionResult> CreateFood()
    {
        ViewData["Categories"] = await _menuService.GetCategoriesAsync();
        return View(new FoodDto { IsVisible = true });
    }

    /// <summary>Creates a food item (with optional validated image upload).</summary>
    [HttpPost]
    public async Task<IActionResult> CreateFood(FoodDto food, IFormFile? imageFile)
    {
        if (imageFile is not null)
        {
            var slug = await GetCategorySlugAsync(food.FoodCategoryId);
            var result = await _imageStorage.SaveAsync(
                imageFile.OpenReadStream(), food.Name, Path.GetExtension(imageFile.FileName), imageFile.Length, $"food/{slug}");
            if (!result.Success)
            {
                ModelState.AddModelError("imageFile", result.Error!);
                ViewData["Categories"] = await _menuService.GetCategoriesAsync();
                return View(food);
            }

            food.ImageFileName = result.FileName;
            try
            {
                await _menuService.CreateFoodAsync(food);
            }
            catch
            {
                // Keep the file system consistent with the database on failure.
                await _imageStorage.DeleteAsync(result.FileName!, $"food/{slug}");
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        await _menuService.CreateFoodAsync(food);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Shows the edit-food form.</summary>
    public async Task<IActionResult> EditFood(int id)
    {
        var food = await _menuService.GetFoodAsync(id);
        if (food is null)
        {
            return RedirectToAction(nameof(Index));
        }

        ViewData["Categories"] = await _menuService.GetCategoriesAsync();
        return View(food);
    }

    /// <summary>Updates a food item (replaces/moves its image file as needed).</summary>
    [HttpPost]
    public async Task<IActionResult> EditFood(FoodDto food, IFormFile? imageFile, string? originalCategorySlug)
    {
        var existing = await _menuService.GetFoodAsync(food.Id);
        if (existing is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var oldFileName = existing.ImageFileName;
        var oldSlug = string.IsNullOrEmpty(originalCategorySlug) ? existing.FoodCategorySlug : originalCategorySlug;
        var newSlug = await GetCategorySlugAsync(food.FoodCategoryId);

        if (imageFile is not null)
        {
            var result = await _imageStorage.SaveAsync(
                imageFile.OpenReadStream(), food.Name, Path.GetExtension(imageFile.FileName), imageFile.Length, $"food/{newSlug}");
            if (!result.Success)
            {
                ModelState.AddModelError("imageFile", result.Error!);
                ViewData["Categories"] = await _menuService.GetCategoriesAsync();
                return View(food);
            }

            food.ImageFileName = result.FileName;
            await _menuService.UpdateFoodAsync(food);

            // Remove the old file unless the new filename collides with it (same name can reproduce the same slug).
            if (!string.IsNullOrEmpty(oldFileName) && !string.Equals(oldFileName, food.ImageFileName, StringComparison.Ordinal))
            {
                await _imageStorage.DeleteAsync(oldFileName, $"food/{oldSlug}");
            }

            return RedirectToAction(nameof(Index));
        }

        await _menuService.UpdateFoodAsync(food);

        // No new image but the category changed: move the existing file to the new slug folder.
        if (!string.IsNullOrEmpty(oldFileName) && !string.Equals(oldSlug, newSlug, StringComparison.OrdinalIgnoreCase))
        {
            await _imageStorage.MoveAsync(oldFileName, $"food/{oldSlug}", $"food/{newSlug}");
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>Shows the delete-food confirmation.</summary>
    public async Task<IActionResult> DeleteFood(int id)
    {
        var food = await _menuService.GetFoodAsync(id);
        if (food is null)
        {
            return RedirectToAction(nameof(Index));
        }

        return View(food);
    }

    /// <summary>Deletes a food item and its stored image file.</summary>
    [HttpPost, ActionName("DeleteFood")]
    public async Task<IActionResult> DeleteFoodConfirmed(int id)
    {
        var food = await _menuService.GetFoodAsync(id);
        if (food is null)
        {
            return RedirectToAction(nameof(Index));
        }

        await _menuService.DeleteFoodAsync(id);

        if (!string.IsNullOrEmpty(food.ImageFileName))
        {
            await _imageStorage.DeleteAsync(food.ImageFileName, $"food/{food.FoodCategorySlug}");
        }

        return RedirectToAction(nameof(Index));
    }

    // ---- Category ----

    /// <summary>Shows the create-category form.</summary>
    public IActionResult CreateCategory() => View(new FoodCategoryDto());

    /// <summary>Creates a food category.</summary>
    [HttpPost]
    public async Task<IActionResult> CreateCategory(FoodCategoryDto category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
        {
            ModelState.AddModelError(nameof(category.Name), "Name is required.");
            return View(category);
        }

        await _menuService.CreateCategoryAsync(category);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Shows the edit-category-discount form.</summary>
    public async Task<IActionResult> EditCategoryDiscount()
    {
        ViewData["Categories"] = await _menuService.GetCategoriesAsync();
        return View();
    }

    /// <summary>Updates a category's name (when provided) and its discount percent.</summary>
    [HttpPost]
    public async Task<IActionResult> EditCategoryDiscount(int foodCategoryId, string? name, decimal discountPercent)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            await _menuService.UpdateCategoryAsync(foodCategoryId, name);
        }

        await _menuService.UpdateCategoryDiscountAsync(foodCategoryId, discountPercent);
        return RedirectToAction(nameof(Index));
    }

    // ---- Combo ----

    /// <summary>Shows the create-combo form.</summary>
    public IActionResult CreateComboItem() => View(new ComboMenuDto { IsVisible = true });

    /// <summary>Creates a combo menu (with optional validated image upload) and redirects to its edit screen.</summary>
    [HttpPost]
    public async Task<IActionResult> CreateComboItem(ComboMenuDto combo, IFormFile? imageFile)
    {
        if (imageFile is not null)
        {
            var result = await _imageStorage.SaveAsync(
                imageFile.OpenReadStream(), combo.Name, Path.GetExtension(imageFile.FileName), imageFile.Length, "combo");
            if (!result.Success)
            {
                ModelState.AddModelError("imageFile", result.Error!);
                return View(combo);
            }

            combo.ImageFileName = result.FileName;
            try
            {
                var newId = await _menuService.CreateComboAsync(combo);
                return RedirectToAction(nameof(EditComboMenu), new { id = newId });
            }
            catch
            {
                // Keep the file system consistent with the database on failure.
                await _imageStorage.DeleteAsync(result.FileName!, "combo");
                throw;
            }
        }

        var id = await _menuService.CreateComboAsync(combo);
        return RedirectToAction(nameof(EditComboMenu), new { id });
    }

    /// <summary>Shows the edit-combo form with an inline food picker (search + category filter).</summary>
    public async Task<IActionResult> EditComboMenu(int id, string? searchString, int? categoryId)
    {
        var combo = await _menuService.GetComboAsync(id);
        if (combo is null)
        {
            return RedirectToAction(nameof(Index));
        }

        IEnumerable<FoodDto> foods = await _menuService.GetAllFoodsAsync();
        if (!string.IsNullOrEmpty(searchString))
        {
            foods = foods.Where(f => f.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        if (categoryId is > 0)
        {
            foods = foods.Where(f => f.FoodCategoryId == categoryId.Value).ToList();
        }

        ViewData["Foods"] = foods.ToList();
        ViewData["Categories"] = await _menuService.GetCategoriesAsync();
        ViewData["CurrentFilter"] = searchString;
        ViewData["CurrentCategory"] = categoryId ?? 0;
        return View(combo);
    }

    /// <summary>Updates combo details (replaces its image file when a new one is uploaded).</summary>
    [HttpPost]
    public async Task<IActionResult> EditComboMenu(ComboMenuDto combo, IFormFile? imageFile)
    {
        var existing = await _menuService.GetComboAsync(combo.Id);
        if (existing is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var oldFileName = existing.ImageFileName;

        if (imageFile is not null)
        {
            var result = await _imageStorage.SaveAsync(
                imageFile.OpenReadStream(), combo.Name, Path.GetExtension(imageFile.FileName), imageFile.Length, "combo");
            if (!result.Success)
            {
                ModelState.AddModelError("imageFile", result.Error!);
                ViewData["Foods"] = await _menuService.GetAllFoodsAsync();
                ViewData["Categories"] = await _menuService.GetCategoriesAsync();
                return View(combo);
            }

            combo.ImageFileName = result.FileName;
            await _menuService.UpdateComboAsync(combo);

            // Remove the old file unless the new filename collides with it.
            if (!string.IsNullOrEmpty(oldFileName) && !string.Equals(oldFileName, combo.ImageFileName, StringComparison.Ordinal))
            {
                await _imageStorage.DeleteAsync(oldFileName, "combo");
            }

            return RedirectToAction(nameof(EditComboMenu), new { id = combo.Id });
        }

        await _menuService.UpdateComboAsync(combo);
        return RedirectToAction(nameof(EditComboMenu), new { id = combo.Id });
    }

    /// <summary>Adds a food item to a combo menu.</summary>
    [HttpPost]
    public async Task<IActionResult> AddItemToComboMenu(int comboMenuId, int foodId, int amount)
    {
        if (amount <= 0)
        {
            amount = 1;
        }

        await _menuService.AddComboMenuItemAsync(comboMenuId, foodId, amount);
        return RedirectToAction(nameof(EditComboMenu), new { id = comboMenuId });
    }

    /// <summary>Updates the amount of a combo menu item.</summary>
    [HttpPost]
    public async Task<IActionResult> EditComboMenuItemAmount(int comboMenuId, int comboMenuItemId, int amount)
    {
        if (amount <= 0)
        {
            amount = 1;
        }

        await _menuService.UpdateComboMenuItemAmountAsync(comboMenuId, comboMenuItemId, amount);
        return RedirectToAction(nameof(EditComboMenu), new { id = comboMenuId });
    }

    /// <summary>Removes a food item from a combo menu.</summary>
    public async Task<IActionResult> DeleteComboMenuItem(int comboMenuId, int comboMenuItemId)
    {
        await _menuService.DeleteComboMenuItemAsync(comboMenuId, comboMenuItemId);
        return RedirectToAction(nameof(EditComboMenu), new { id = comboMenuId });
    }

    /// <summary>Deletes a combo menu and its stored image file.</summary>
    public async Task<IActionResult> DeleteComboMenu(int id)
    {
        var combo = await _menuService.GetComboAsync(id);
        if (combo is null)
        {
            return RedirectToAction(nameof(Index));
        }

        await _menuService.DeleteComboAsync(id);

        if (!string.IsNullOrEmpty(combo.ImageFileName))
        {
            await _imageStorage.DeleteAsync(combo.ImageFileName, "combo");
        }

        return RedirectToAction(nameof(Index));
    }

    // ---- Image helpers ----

    /// <summary>Resolves the URL slug for a category (fallback "misc").</summary>
    private async Task<string> GetCategorySlugAsync(int categoryId)
        => (await _menuService.GetCategoriesAsync()).FirstOrDefault(c => c.Id == categoryId)?.Slug ?? "misc";
}
