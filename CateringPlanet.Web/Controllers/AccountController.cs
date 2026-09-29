using System.Security.Claims;
using CateringPlanet.Application.Abstractions;
using CateringPlanet.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CateringPlanet.Web.Controllers;

/// <summary>
/// Account management: login, registration, logout, and profile editing.
/// Ported from the legacy CateringAgency AccountController and adapted to ASP.NET Core Identity.
/// </summary>
[Authorize]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>Redirects the default account route to the login page.</summary>
    public IActionResult Index() => RedirectToAction(nameof(Login));

    /// <summary>Shows the login form.</summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
        => View(new LoginViewModel { ReturnUrl = returnUrl });

    /// <summary>Validates credentials and signs the user in via Identity.</summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (succeeded, error) = await _accountService.LoginAsync(model.Email, model.Password, model.RememberMe);
        if (!succeeded)
        {
            ModelState.AddModelError(string.Empty, error ?? "Invalid username or password.");
            return View(model);
        }

        // Only honor the return URL when it points back to this site (open-redirect protection).
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    /// <summary>Shows the registration form.</summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => View(new RegisterViewModel());

    /// <summary>Registers a new customer and signs them in immediately.</summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Public self-registration always creates a Customer (unified role model).
        var (succeeded, error) = await _accountService.RegisterAsync(
            model.Email, model.Password, model.FirstName, model.LastName, "Customer",
            model.PhoneNumber, model.Address);

        if (!succeeded)
        {
            ModelState.AddModelError(string.Empty, error ?? "Registration failed.");
            return View(model);
        }

        // The user is signed in right after registering.
        await _accountService.LoginAsync(model.Email, model.Password, rememberMe: false);
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Signs the current user out.
    /// Implemented as GET because the ported navbars link to Account/Logout with plain anchor tags.
    /// </summary>
    public async Task<IActionResult> Logout()
    {
        await _accountService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }

    /// <summary>Shows the read-only account details page.</summary>
    [HttpGet]
    public async Task<IActionResult> AccountDetails()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = userId is null ? null : await _accountService.GetByIdAsync(userId);
        if (user is null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new EditAccountViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Points = user.Points,
            PhoneNumber = user.Phone,
            Address = user.Address
        });
    }

    /// <summary>Shows the profile edit form (name, phone, and address; email and points are read-only).</summary>
    [HttpGet]
    public async Task<IActionResult> EditAccount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = userId is null ? null : await _accountService.GetByIdAsync(userId);
        if (user is null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new EditAccountViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Points = user.Points,
            PhoneNumber = user.Phone,
            Address = user.Address
        });
    }

    /// <summary>Saves the profile changes (first/last name only).</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAccount(EditAccountViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return RedirectToAction(nameof(Login));
        }

        await _accountService.UpdateProfileAsync(userId, model.FirstName, model.LastName,
            model.PhoneNumber ?? string.Empty, model.Address ?? string.Empty);
        return RedirectToAction(nameof(AccountDetails));
    }
}
