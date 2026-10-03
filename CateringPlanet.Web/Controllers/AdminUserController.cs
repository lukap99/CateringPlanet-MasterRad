using CateringPlanet.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CateringPlanet.Web.Controllers;

/// <summary>
/// Admin management of customers and managers (Admin only).
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminUserController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IAccountService _accountService;

    public AdminUserController(IAdminService adminService, IAccountService accountService)
    {
        _adminService = adminService;
        _accountService = accountService;
    }

    /// <summary>Redirects to the customers list.</summary>
    public IActionResult Index() => RedirectToAction(nameof(Users));

    /// <summary>Lists customers with search, sorting, and pagination.</summary>
    public async Task<IActionResult> Users(string? searchString, string? sortOrder, int page = 1)
    {
        var users = (await _adminService.GetUsersInRoleAsync("Customer")).ToList();

        if (!string.IsNullOrEmpty(searchString))
        {
            users = users.Where(u =>
                u.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        users = sortOrder switch
        {
            "name_desc" => users.OrderByDescending(u => u.FullName).ToList(),
            "email_asc" => users.OrderBy(u => u.Email).ToList(),
            "email_desc" => users.OrderByDescending(u => u.Email).ToList(),
            "points_asc" => users.OrderBy(u => u.Points).ToList(),
            "points_desc" => users.OrderByDescending(u => u.Points).ToList(),
            _ => users.OrderBy(u => u.FullName).ToList()
        };

        const int pageSize = 10;
        var totalItems = users.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        ViewData["CurrentFilter"] = searchString;
        ViewData["CurrentSort"] = sortOrder;
        ViewData["CurrentPage"] = page;
        ViewData["PageSize"] = pageSize;
        ViewData["TotalItems"] = totalItems;
        ViewData["TotalPages"] = totalPages;

        return View(users.Skip((page - 1) * pageSize).Take(pageSize).ToList());
    }

    /// <summary>Shows a customer's profile.</summary>
    public async Task<IActionResult> ViewUser(string id)
    {
        var user = await _accountService.GetByIdAsync(id);
        if (user is null)
        {
            return RedirectToAction(nameof(Users));
        }

        return View(user);
    }

    /// <summary>Sets a customer's loyalty points balance to the given value.</summary>
    [HttpPost]
    public async Task<IActionResult> UpdateUserPoints(string id, int points)
    {
        var current = await _accountService.GetPointsAsync(id);
        await _accountService.AddPointsAsync(id, points - current);
        return RedirectToAction(nameof(ViewUser), new { id });
    }

    /// <summary>Shows the edit form for a customer's profile.</summary>
    public async Task<IActionResult> EditCustomerProfile(string id)
    {
        var user = await _accountService.GetByIdAsync(id);
        if (user is null)
        {
            return RedirectToAction(nameof(Users));
        }

        return View(user);
    }

    /// <summary>Updates a customer's name, phone number, and address.</summary>
    [HttpPost]
    public async Task<IActionResult> EditCustomerProfile(string id, string firstName, string lastName, string phone, string address)
    {
        await _accountService.UpdateProfileAsync(id, firstName, lastName, phone, address);
        return RedirectToAction(nameof(ViewUser), new { id });
    }

    /// <summary>Shows the delete confirmation for a customer.</summary>
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _accountService.GetByIdAsync(id);
        if (user is null)
        {
            return RedirectToAction(nameof(Users));
        }

        return View(user);
    }

    /// <summary>Deletes a customer.</summary>
    [HttpPost, ActionName("DeleteUser")]
    public async Task<IActionResult> DeleteUserConfirmed(string id)
    {
        await _adminService.DeleteUserAsync(id);
        return RedirectToAction(nameof(Users));
    }

    /// <summary>Lists managers with search, sorting, and pagination.</summary>
    public async Task<IActionResult> Managers(string? searchString, string? sortOrder, int page = 1)
    {
        var users = (await _adminService.GetUsersInRoleAsync("Manager")).ToList();

        if (!string.IsNullOrEmpty(searchString))
        {
            users = users.Where(u =>
                u.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        users = sortOrder switch
        {
            "name_desc" => users.OrderByDescending(u => u.FullName).ToList(),
            "email_asc" => users.OrderBy(u => u.Email).ToList(),
            "email_desc" => users.OrderByDescending(u => u.Email).ToList(),
            _ => users.OrderBy(u => u.FullName).ToList()
        };

        const int pageSize = 10;
        var totalItems = users.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        ViewData["CurrentFilter"] = searchString;
        ViewData["CurrentSort"] = sortOrder;
        ViewData["CurrentPage"] = page;
        ViewData["PageSize"] = pageSize;
        ViewData["TotalItems"] = totalItems;
        ViewData["TotalPages"] = totalPages;

        return View(users.Skip((page - 1) * pageSize).Take(pageSize).ToList());
    }

    /// <summary>Shows a manager's profile.</summary>
    public async Task<IActionResult> ViewManager(string id)
    {
        var user = await _accountService.GetByIdAsync(id);
        if (user is null)
        {
            return RedirectToAction(nameof(Managers));
        }

        return View(user);
    }

    /// <summary>Shows the create-manager form.</summary>
    public IActionResult CreateManager() => View();

    /// <summary>Creates a manager.</summary>
    [HttpPost]
    public async Task<IActionResult> CreateManager(string email, string password, string firstName, string lastName)
    {
        var (succeeded, error) = await _adminService.CreateUserAsync(email, password, firstName, lastName, "Manager");
        if (!succeeded)
        {
            ModelState.AddModelError(string.Empty, error ?? "Failed to create manager.");
            return View();
        }

        return RedirectToAction(nameof(Managers));
    }

    /// <summary>Shows the edit form for a manager's profile.</summary>
    public async Task<IActionResult> EditManagerProfile(string id)
    {
        var user = await _accountService.GetByIdAsync(id);
        if (user is null)
        {
            return RedirectToAction(nameof(Managers));
        }

        return View(user);
    }

    /// <summary>Updates a manager's name, phone number, and address.</summary>
    [HttpPost]
    public async Task<IActionResult> EditManagerProfile(string id, string firstName, string lastName, string phone, string address)
    {
        await _accountService.UpdateProfileAsync(id, firstName, lastName, phone, address);
        return RedirectToAction(nameof(ViewManager), new { id });
    }

    /// <summary>Shows the delete confirmation for a manager.</summary>
    public async Task<IActionResult> DeleteManager(string id)
    {
        var user = await _accountService.GetByIdAsync(id);
        if (user is null)
        {
            return RedirectToAction(nameof(Managers));
        }

        return View(user);
    }

    /// <summary>Deletes a manager.</summary>
    [HttpPost, ActionName("DeleteManager")]
    public async Task<IActionResult> DeleteManagerConfirmed(string id)
    {
        await _adminService.DeleteUserAsync(id);
        return RedirectToAction(nameof(Managers));
    }
}
