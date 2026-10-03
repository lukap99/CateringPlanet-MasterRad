using CateringPlanet.Application.DTOs;
using CateringPlanet.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CateringPlanet.Web.Controllers;

/// <summary>
/// Public menu browsing and adding items to the shopping cart.
/// </summary>
public class MenuController : Controller
{
    private readonly IMenuService _menuService;
    private readonly ICartService _cartService;

    public MenuController(IMenuService menuService, ICartService cartService)
    {
        _menuService = menuService;
        _cartService = cartService;
    }

    /// <summary>
    /// Shows visible foods (optionally filtered by category or name) plus combo menus.
    /// </summary>
    /// <param name="id">Optional category id; null/0 shows all items.</param>
    /// <param name="searchString">Optional text to filter foods by name.</param>
    public async Task<IActionResult> Index(int? id, string? searchString)
    {
        // selectedCategoryId: 0 = all items, >0 = specific category.
        var selectedCategoryId = id is > 0 ? id.Value : 0;

        ViewData["SelectedCategoryId"] = selectedCategoryId;
        ViewData["IsComboMenuView"] = false;
        ViewData["CurrentFilter"] = searchString;

        IEnumerable<FoodDto> foods;
        IEnumerable<ComboMenuDto> combos;

        if (selectedCategoryId <= 0)
        {
            foods = await _menuService.GetVisibleFoodsAsync(null, searchString);
            combos = await _menuService.GetVisibleCombosAsync();
        }
        else
        {
            foods = await _menuService.GetVisibleFoodsAsync(selectedCategoryId, searchString);
            combos = Array.Empty<ComboMenuDto>();
        }

        ViewData["ComboItems"] = combos;

        TransferMessages();
        return View(foods.ToList());
    }

    /// <summary>
    /// Shows the combo-menu-only view.
    /// </summary>
    public async Task<IActionResult> ComboMenu()
    {
        // selectedCategoryId -1 marks the combo-menu-only view in the sidebar.
        ViewData["SelectedCategoryId"] = -1;
        ViewData["IsComboMenuView"] = true;
        ViewData["ComboItems"] = await _menuService.GetVisibleCombosAsync();

        TransferMessages();
        return View("Index", new List<FoodDto>());
    }

    /// <summary>
    /// Adds a food item to the cart and redirects back to the originating view.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddToCart(int FoodId, int Amount, int selectedCategoryId, bool isComboMenuView)
    {
        if (Amount <= 0)
        {
            TempData["ErrorMessage"] = "Amount added to cart cannot be 0";
            return RedirectAfterAdd(isComboMenuView, selectedCategoryId);
        }

        var food = await _menuService.GetFoodAsync(FoodId);
        if (food is null)
        {
            TempData["ErrorMessage"] = "Food item not found.";
            return RedirectAfterAdd(isComboMenuView, selectedCategoryId);
        }

        _cartService.AddFood(food, Amount);
        TempData["AddMessage"] = $"{food.Name} (× {Amount}) added to cart";
        return RedirectAfterAdd(isComboMenuView, selectedCategoryId);
    }

    /// <summary>
    /// Adds a combo menu to the cart and redirects back to the originating view.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddComboMenuToCart(int ComboMenuId, int Amount, int selectedCategoryId, bool isComboMenuView)
    {
        if (Amount <= 0)
        {
            TempData["ErrorMessage"] = "Amount added to cart cannot be 0";
            return RedirectAfterAdd(isComboMenuView, selectedCategoryId);
        }

        var combo = await _menuService.GetComboAsync(ComboMenuId);
        if (combo is null)
        {
            TempData["ErrorMessage"] = "Combo menu not found.";
            return RedirectAfterAdd(isComboMenuView, selectedCategoryId);
        }

        _cartService.AddCombo(combo, Amount);
        TempData["AddMessage"] = $"Combo menu: {combo.Name} × {Amount} added to cart";
        return RedirectAfterAdd(isComboMenuView, selectedCategoryId);
    }

    /// <summary>Redirects to the combo-only view or the category-filtered index view.</summary>
    private IActionResult RedirectAfterAdd(bool isComboMenuView, int selectedCategoryId)
        => isComboMenuView
            ? RedirectToAction(nameof(ComboMenu))
            : RedirectToAction(nameof(Index), new { id = selectedCategoryId });

    /// <summary>Copies TempData flash messages into ViewData for the next render.</summary>
    private void TransferMessages()
    {
        if (TempData.ContainsKey("AddMessage"))
        {
            ViewData["AddMessage"] = TempData["AddMessage"]?.ToString();
        }
        if (TempData.ContainsKey("ErrorMessage"))
        {
            ViewData["ErrorMessage"] = TempData["ErrorMessage"]?.ToString();
        }
    }
}
