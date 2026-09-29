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

    /// <summary>Lists customers.</summary>
    public async Task<IActionResult> Users()
        => View(await _adminService.GetUsersInRoleAsync("Customer"));

    /// <summary>Shows the edit form for a customer.</summary>
    public async Task<IActionResult> EditUser(string id)
    {
        var user = await _accountService.GetByIdAsync(id);
        if (user is null)
        {
            return RedirectToAction(nameof(Users));
        }

        return View(user);
    }

    /// <summary>Updates a customer's name.</summary>
    [HttpPost]
    public async Task<IActionResult> EditUser(string id, string firstName, string lastName)
    {
        await _adminService.UpdateUserAsync(id, firstName, lastName, "Customer");
        return RedirectToAction(nameof(Users));
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

    /// <summary>Lists managers.</summary>
    public async Task<IActionResult> Managers()
        => View(await _adminService.GetUsersInRoleAsync("Manager"));

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

    /// <summary>Shows the edit form for a manager.</summary>
    public async Task<IActionResult> EditManager(string id)
    {
        var user = await _accountService.GetByIdAsync(id);
        if (user is null)
        {
            return RedirectToAction(nameof(Managers));
        }

        return View(user);
    }

    /// <summary>Updates a manager's name.</summary>
    [HttpPost]
    public async Task<IActionResult> EditManager(string id, string firstName, string lastName)
    {
        await _adminService.UpdateUserAsync(id, firstName, lastName, "Manager");
        return RedirectToAction(nameof(Managers));
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
