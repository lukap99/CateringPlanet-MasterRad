using System.Security.Claims;
using CateringPlanet.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CateringPlanet.Api.Controllers;

/// <summary>
/// Current-user profile endpoint. Requires a valid JWT.
/// </summary>
[Route("api/account")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>Gets the current user's profile (name, e-mail, points, role).</summary>
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _accountService.GetByIdAsync(userId);
        return user is null ? NotFound() : Ok(user);
    }
}
