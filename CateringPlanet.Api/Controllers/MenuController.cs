using CateringPlanet.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CateringPlanet.Api.Controllers;

/// <summary>
/// Public menu read endpoints (categories, foods, combos).
/// </summary>
[Route("api/menu")]
[ApiController]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>Gets all food categories.</summary>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
        => Ok(await _menuService.GetCategoriesAsync());

    /// <summary>Gets visible foods, optionally filtered to a category id.</summary>
    /// <param name="categoryId">Optional category ID to filter foods by.</param>
    [HttpGet("foods")]
    public async Task<IActionResult> GetFoods([FromQuery] int? categoryId)
        => Ok(await _menuService.GetVisibleFoodsAsync(categoryId));

    /// <summary>Gets all visible combo menus.</summary>
    [HttpGet("combos")]
    public async Task<IActionResult> GetCombos()
        => Ok(await _menuService.GetVisibleCombosAsync());
}
