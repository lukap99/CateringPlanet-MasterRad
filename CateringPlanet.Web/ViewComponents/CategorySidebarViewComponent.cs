using CateringPlanet.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CateringPlanet.Web.ViewComponents;

/// <summary>
/// Renders the menu category sidebar with the currently selected category highlighted.
/// Replaces the legacy <c>MenuController.MenuCategories</c> child action.
/// </summary>
public class CategorySidebarViewComponent : ViewComponent
{
    private readonly IMenuService _menuService;

    public CategorySidebarViewComponent(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// Loads categories and renders the sidebar view.
    /// </summary>
    /// <param name="selectedCategoryId">Currently selected category (0 = all, -1 = combo menu).</param>
    /// <param name="isComboMenuView">Whether the combo-only view is active.</param>
    public async Task<IViewComponentResult> InvokeAsync(int selectedCategoryId, bool isComboMenuView)
    {
        var categories = await _menuService.GetCategoriesAsync();
        ViewData["SelectedCategoryId"] = selectedCategoryId;
        ViewData["IsComboMenuView"] = isComboMenuView;
        return View(categories);
    }
}
